// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace AzureDeploymentCmdlets.Cmdlet
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using AzureDeploymentCmdlets.WAPPSCmdlet;
    using AzureDeploymentCmdlets.Properties;
    using AzureDeploymentCmdlets.Model;

    /// <summary>
    /// Deletes the specified hosted service from Windows Azure.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "AzureService")]
    public class RemoveAzureServiceCommand : ServiceManagementCmdletBase
    {
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "name of subscription which has this service")]
        public string Subscription
        {
            get;
            set;
        }

        public RemoveAzureServiceCommand() { }

        public RemoveAzureServiceCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        public void RemoveAzureServiceProcess(string rootName, string inSubscription)
        {
            string serviceName;
            ServiceSettings settings = base.GetDefaultSettings(rootName, null, null, null, null, inSubscription, out serviceName);
            subscriptionId = new GlobalComponents(GlobalPathInfo.GlobalSettingsDirectory).GetSubscriptionId(settings.Subscription);
            SafeWriteObjectWithTimestamp(Resources.RemoveServiceStartMessage, serviceName);
            SafeWriteObjectWithTimestamp(Resources.RemoveDeploymentMessage);
            StopAndRemove(rootName, serviceName, settings.Subscription, ArgumentConstants.Slots[Slot.Production]);
            StopAndRemove(rootName, serviceName, settings.Subscription, ArgumentConstants.Slots[Slot.Staging]);
            SafeWriteObjectWithTimestamp(Resources.RemoveServiceMessage);
            RemoveService(serviceName);
        }

        private void StopAndRemove(string rootName, string serviceName, string subscription, string slot)
        {
            GetDeploymentStatus getDeployment = new GetDeploymentStatus(this.Channel);

            if (getDeployment.DeploymentExists(rootName, serviceName, slot, subscription))
            {
                DeploymentStatusManager setDeployment = new DeploymentStatusManager(this.Channel);
                setDeployment.SetDeploymentStatusProcess(rootName, DeploymentStatus.Suspended, slot, subscription, serviceName);

                getDeployment.WaitForState(DeploymentStatus.Suspended, rootName, serviceName, slot, subscription);

                RemoveAzureDeploymentCommand removeDeployment = new RemoveAzureDeploymentCommand(this.Channel);
                removeDeployment.RemoveAzureDeploymentProcess(rootName, serviceName, slot, subscription);

                while (getDeployment.DeploymentExists(rootName, serviceName, slot, subscription)) ;
            }
        }

        private void RemoveService(string serviceName)
        {
            SafeWriteObjectWithTimestamp(string.Format(Resources.RemoveAzureServiceWaitMessage, serviceName));

            InvokeInOperationContext(() =>
                {
                    this.RetryCall(s => this.Channel.DeleteHostedService(s, serviceName));
                });
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                RemoveAzureServiceProcess(base.GetServiceRootPath(), Subscription);
                SafeWriteObjectWithTimestamp(Resources.CompleteMessage);    
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
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

namespace AzureDeploymentCmdlets.Model
{
    using System;
    using System.Management.Automation;
    using System.ServiceModel;
    using AzureDeploymentCmdlets.WAPPSCmdlet;
    using AzureDeploymentCmdlets.Properties;
    using AzureDeploymentCmdlets.Model;

    /// <summary>
    /// Deletes the specified deployment. Note that the deployment should be in suspended state.
    /// </summary>
    class RemoveAzureDeploymentCommand : ServiceManagementCmdletBase
    {
        [Parameter(Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Deployment slot. Staging | Production")]
        public string Slot
        {
            get;
            set;
        }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Service name.")]
        public string ServiceName
        {
            get;
            set;
        }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Subscription")]
        public string Subscription
        {
            get;
            set;
        }

        public RemoveAzureDeploymentCommand()
        {
        }

        public RemoveAzureDeploymentCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        public string RemoveAzureDeploymentProcess(string rootPath, string inServiceName, string inSlot, string inSubscription)
        {
            string results;
            string serviceName;
            ServiceSettings settings = InitializeArguments(rootPath, inServiceName, inSlot, inSubscription, out serviceName);
            results = RemoveDeployment(serviceName, settings.Slot);

            return results;
        }

        private ServiceSettings InitializeArguments(string rootPath, string inServiceName, string inSlot, string inSubscription, out string serviceName)
        {
            ServiceSettings settings = base.GetDefaultSettings(rootPath, inServiceName, inSlot, null, null, inSubscription, out serviceName);
            subscriptionId = new GlobalComponents(GlobalPathInfo.GlobalSettingsDirectory).GetSubscriptionId(settings.Subscription);
            return settings;
        }

        private string RemoveDeployment(string serviceName, string slot)
        {
            string results;

            InvokeInOperationContext(() =>
            {
                this.RetryCall(s => this.Channel.DeleteDeploymentBySlot(s, serviceName, slot));
            });

            results = string.Format(Resources.DeploymentRemovedMessage, slot, serviceName);

            return results;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                string results = this.RemoveAzureDeploymentProcess(base.GetServiceRootPath(), ServiceName, Slot, Subscription);
                SafeWriteObject(results);
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }

        public void Initialize()
        {
            this.ProcessRecord();
        }
    }
}
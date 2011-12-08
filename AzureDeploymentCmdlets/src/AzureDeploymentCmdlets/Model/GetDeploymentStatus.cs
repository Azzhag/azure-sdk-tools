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

    /// <summary>
    /// Gets the status for a specified deployment. This class is candidate for being cmdlet so it has this name which similar to cmdlets.
    /// </summary>
    public class GetDeploymentStatus : ServiceManagementCmdletBase
    {
        public GetDeploymentStatus()
        {

        }

        public GetDeploymentStatus(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        [Parameter(Position = 0, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Deployment slot. Staging | Production")]
        public string Slot
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Service name")]
        public string ServiceName
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Subscription name")]
        public string Subscription
        {
            get;
            set;
        }

        public string GetDeploymentStatusProcess(string rootPath, string inServiceName, string inSlot, string subscription)
        {
            string serviceName;
            string slot;

            InitializeArguments(rootPath, inServiceName, inSlot, subscription, out serviceName, out slot);
            string deploymentStatus = GetStatus(serviceName, slot);

            return deploymentStatus;
        }

        private void InitializeArguments(string rootPath, string inServiceName, string inSlot, string subscription, out string serviceName, out string slot)
        {
            ServiceSettings settings = base.GetDefaultSettings(rootPath, inServiceName, inSlot, null, null, subscription, out serviceName);
            subscriptionId = new GlobalComponents(GlobalPathInfo.GlobalSettingsDirectory).GetSubscriptionId(settings.Subscription);
            slot = settings.Slot;
        }

        public string GetStatus(string serviceName, string slot)
        {
            Deployment deployment;

            try
            {
                deployment = this.RetryCall(s => this.Channel.GetDeploymentBySlot(s, serviceName, slot));
            }
            catch (EndpointNotFoundException)
            {
                // If we reach here that means the service or slot doesn't exist
                //
                throw new EndpointNotFoundException(string.Format(Resources.ServiceSlotDoesNotExist, serviceName, slot));
            }

            return deployment.Status;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                string result = this.GetDeploymentStatusProcess(base.GetServiceRootPath(), ServiceName, Slot, Subscription);
                SafeWriteObject(result);
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
        
        /// <summary>
        /// This method waits until the deployment reaches a specified state.
        /// Remark: Caller for this method should handle any thrown exception
        /// </summary>
        /// <param name="state">The state which method will wait on</param>
        /// <param name="rootPath">The service root path name (can be null)</param>
        /// <param name="inServiceName">Service that has the deployment to use (can be null)</param>
        /// <param name="inSlot">Type of the slot for deployment which method will wait on</param>
        /// <param name="subscription">Subscription name which has the service</param>
        public void WaitForState(string state, string rootPath, string inServiceName, string inSlot, string subscription)
        {
            string serviceName;
            string slot;

            InitializeArguments(rootPath, inServiceName, inSlot, subscription, out serviceName, out slot);

            do
            {
                // Delay the request for some time
                //
                System.Threading.Thread.Sleep(int.Parse(Resources.StandardRetryDelayInMs));

            } while (GetStatus(serviceName, slot) != state);
        }

        public bool DeploymentExists(string rootPath, string inServiceName, string inSlot, string subscription)
        {
            string serviceName;
            string slot;

            InitializeArguments(rootPath, inServiceName, inSlot, subscription, out serviceName, out slot);

            try
            {
                GetStatus(serviceName, slot);
                return true;
            }
            catch (EndpointNotFoundException)
            {
                // Reaching this means there's no deployment with this slot
                //
                return false;
            }
        }
    }
}
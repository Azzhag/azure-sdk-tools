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
    using System.Linq;
    using System.Management.Automation;
    using System.ServiceModel;
    using AzureDeploymentCmdlets.WAPPSCmdlet;
    using AzureDeploymentCmdlets.Properties;
    using AzureDeploymentCmdlets.Model;
    using System.Text;

    /// <summary>
    /// Lists all storage services underneath the subscription.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureStorageAccounts")]
    public class GetAzureStorageAccountsCommand : ServiceManagementCmdletBase
    {
        [Parameter(Position = 0, Mandatory = false, HelpMessage = "Subscription name")]
        [Alias("sn")]
        public string Subscription { get; set; }

        public GetAzureStorageAccountsCommand() { }

        public GetAzureStorageAccountsCommand(IServiceManagement channel)
        {
            this.Channel = channel;
        }

        public StorageServiceList GetStorageAccounts()
        {
            StorageServiceList storageServices = null;
            storageServices = this.RetryCall(s => this.Channel.ListStorageServices(s));

            return storageServices;
        }

        public string GetStorageServicesProcess(string subscription)
        {
            InitializeArguments(subscription);
            StorageServiceList storageAccounts = GetStorageAccounts();
            GetStorageAccountsKey(storageAccounts);
            string result = FormatResult(storageAccounts);

            return result;
        }

        private void GetStorageAccountsKey(StorageServiceList storageAccounts)
        {
            foreach (StorageService service in storageAccounts)
            {
                service.StorageServiceKeys = this.RetryCall(s => this.Channel.GetStorageKeys(s, service.ServiceName)).StorageServiceKeys;
            }
        }

        public string FormatResult(StorageServiceList storageServices)
        {
            StringBuilder sb = new StringBuilder();

            bool needsSpacing = false;
            foreach (StorageService service in storageServices)
            {
                if (needsSpacing)
                {
                    sb.AppendLine().AppendLine();
                }
                needsSpacing = true;

                sb.AppendFormat("{0, -16}{1}", Resources.StorageAccountName, service.ServiceName);
                sb.AppendLine();
                sb.AppendFormat("{0, -16}{1}", Resources.StoragePrimaryKey, service.StorageServiceKeys.Primary);
                sb.AppendLine();
                sb.AppendFormat("{0, -16}{1}", Resources.StorageSecondaryKey, service.StorageServiceKeys.Secondary);
            }

            return sb.ToString();
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                string result = this.GetStorageServicesProcess(Subscription);
                SafeWriteObject(result);
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }

        private void InitializeArguments(string subscription)
        {
            string serviceName;
            string subscriptionName = base.GetDefaultSettings(null, null, null, null, null, subscription, out serviceName).Subscription;
            subscriptionId = new GlobalComponents(GlobalPathInfo.GlobalSettingsDirectory).GetSubscriptionId(subscriptionName);
        }
    }
}
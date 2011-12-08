// ----------------------------------------------------------------------------------
// 
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
    using AzureDeploymentCmdlets.Properties;
    using AzureDeploymentCmdlets.Model;

    /// <summary>
    /// Configure the default storage account for deploying. Stores the new storage name in service settings
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "AzureDeploymentStorage")]
    public class SetAzureDeploymentStorageCommand : SetSettings
    {
        [Parameter(Position = 0, Mandatory = true)]
        [Alias("n")]
        public string AccountName { get; set; }

        public void SetAzureDeploymentStorageProcess(string newStorage, string settingsPath)
        {
            ServiceSettings settings = ServiceSettings.Load(settingsPath);
            settings.StorageAccountName = newStorage;
            settings.Save(settingsPath);
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                this.SetAzureDeploymentStorageProcess(AccountName, base.GetServiceSettingsPath(false));
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
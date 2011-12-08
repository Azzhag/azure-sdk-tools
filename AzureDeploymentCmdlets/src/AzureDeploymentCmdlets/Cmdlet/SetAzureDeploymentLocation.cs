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
    /// Configure the default location for deploying. Stores the new location in settings.json
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "AzureDeploymentLocation")]
    public class SetAzureDeploymentLocationCommand : SetSettings
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string Location { get; set; }

        public void SetAzureDeploymentLocationProcess(string newLocation, string settingsPath)
        {
            ServiceSettings settings = ServiceSettings.Load(settingsPath);
            settings.Location = newLocation;
            settings.Save(settingsPath);
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                this.SetAzureDeploymentLocationProcess(Location, base.GetServiceSettingsPath(false));
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
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
    using System.Security.Permissions;
    using AzureDeploymentCmdlets.Model;
    using AzureDeploymentCmdlets.Properties;
    using AzureDeploymentCmdlets.Utilities;
    using System.Net.Sockets;
    using System.Net;

    /// <summary>
    /// Get publish profile
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzurePublishSettings")]
    public class GetAzurePublishSettingsCommand : CmdletBase
    {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                this.GetAzurePublishSettingsProcess(Resources.PublishSettingsUrl);
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }

        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        internal void GetAzurePublishSettingsProcess(string url)
        {
            Validate.ValidateStringIsNullOrEmpty(url, "publish settings url");
            Validate.ValidateInternetConnection();

            General.LaunchWebPage(url);
        }
    }
}
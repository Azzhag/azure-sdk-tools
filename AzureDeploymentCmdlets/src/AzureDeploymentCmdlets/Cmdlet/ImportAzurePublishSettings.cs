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
    using System.IO;
    using System.Management.Automation;
    using System.Security.Cryptography.X509Certificates;
    using System.Xml;
    using AzureDeploymentCmdlets.Properties;
    using AzureDeploymentCmdlets.Utilities;
    using AzureDeploymentCmdlets.Model;

    /// <summary>
    /// Register the azure publish file downloaded from the portal (includes the certificate and subscription information). Called once for the machine
    /// </summary>
    [Cmdlet(VerbsData.Import, "AzurePublishSettings")]
    public class ImportAzurePublishSettingsCommand : CmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Name of *.PublishSettings file")]
        public string Path { get; set; }

        /// <summary>
        /// Acts as main method of setting the azure publish profile by doing the following:
        /// 1. Extracts the certificate binary from *.azurePublish file
        /// 2. Create a X509Certificate2 certificate and adds it to store
        /// 3. Save the extracted certificate and *.azurePublis file to ..\ApplicationData\Azure SDK folder
        /// </summary>
        /// <returns>Message to display for to the user</returns>
        public string ImportAzurePublishSettingsProcess(string publishSettingsFilePath, string azureSdkPath)
        {
            GlobalComponents components = new GlobalComponents(publishSettingsFilePath, azureSdkPath);
            SafeWriteObject(string.Format(Resources.CertificateImportedMessage, components.Certificate.FriendlyName));
            string msg = string.Format(Resources.PublishSettingsSetSuccessfully, GlobalPathInfo.GlobalSettingsDirectory);
            return msg;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                string result = this.ImportAzurePublishSettingsProcess(this.ResolvePath(Path), GlobalPathInfo.GlobalSettingsDirectory);
                SafeWriteObject(result);
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
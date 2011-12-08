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

using System.Text.RegularExpressions;

namespace AzureDeploymentCmdlets.Cmdlet
{
    using System;
    using System.Management.Automation;
    using System.Security.Permissions;
    using System.Text;
    using AzureDeploymentCmdlets.Model;
    using AzureDeploymentCmdlets.Properties;

    /// <summary>
    /// Runs the service in the emulator
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "AzureEmulator")]
    public class StartAzureEmulatorCommand : CmdletBase
    {
        [Parameter(Mandatory = false)]
        [Alias("ln")]
        public SwitchParameter Launch { get; set; }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public string StartAzureEmulatorProcess(string rootPath)
        {
            string standardOutput;
            string standardError;

            StringBuilder message = new StringBuilder();
            AzureService service = new AzureService(rootPath ,null);
            SafeWriteObject(string.Format(Resources.CreatingPackageMessage, "local"));
            service.CreatePackage(DevEnv.Local, out standardOutput, out standardError);
            SafeWriteObject(Resources.StartingEmulator);
            service.StartEmulator(Launch.ToBool(), out standardOutput, out standardError);
            SafeWriteObject(standardOutput);
            SafeWriteObject(Resources.StartedEmulator);
            return message.ToString();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                string result = this.StartAzureEmulatorProcess(base.GetServiceRootPath());
                SafeWriteObject(result);
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
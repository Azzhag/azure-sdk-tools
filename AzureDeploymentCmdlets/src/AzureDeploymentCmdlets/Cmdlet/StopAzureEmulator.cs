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
    using System.Text;
    using AzureDeploymentCmdlets.Model;
    using AzureDeploymentCmdlets.Properties;

    /// <summary>
    /// Runs the service in the emulator
    /// </summary>
    [Cmdlet(VerbsLifecycle.Stop, "AzureEmulator")]
    public class StopAzureEmulatorCommand : CmdletBase
    {
        [Parameter(Mandatory = false)]
        [Alias("ln")]
        public SwitchParameter Launch { get; set; }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public string StopAzureEmulatorProcess()
        {
            string standardOutput;
            string standardError;

            AzureService service = new AzureService();
            SafeWriteObject(Resources.StopEmulatorMessage);
            service.StopEmulator(out standardOutput, out standardError);
            SafeWriteObject(Resources.StoppedEmulatorMessage);
            return null;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                string result = this.StopAzureEmulatorProcess();
                SafeWriteObject(result);
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
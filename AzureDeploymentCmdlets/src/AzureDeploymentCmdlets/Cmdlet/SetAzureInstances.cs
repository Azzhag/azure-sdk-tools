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
    using AzureDeploymentCmdlets.ServiceConfigurationSchema;
    using AzureDeploymentCmdlets.Utilities;
    using AzureDeploymentCmdlets.Model;
    using System.Linq;
    using AzureDeploymentCmdlets.Properties;

    /// <summary>
    /// Configure the number of instances for a web/worker role. Updates the cscfg with the number of instances
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "AzureInstances")]
    public class SetAzureInstancesCommand : CmdletBase
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string RoleName { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        public int Instances { get; set; }

        public void SetAzureInstancesProcess(string roleName, int instances, string rootPath)
        {
            AzureService service = new AzureService(rootPath, null);
            service.SetRoleInstances(service.Paths, roleName, instances);
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                this.SetAzureInstancesProcess(RoleName, Instances, base.GetServiceRootPath());
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
// ----------------------------------------------------------------------------------
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


namespace AzureDeploymentCmdlets.Node.Cmdlet
{
    using System;
    using System.Management.Automation;
    using AzureDeploymentCmdlets.Model;
    using AzureDeploymentCmdlets.Properties;

    /// <summary>
    /// Create scaffolding for a new node web role, change cscfg file and csdef to include the added web role
    /// </summary>
    [Cmdlet(VerbsCommon.Add, "AzureNodeWebRole")]
    public class AddAzureNodeWebRoleCommand : AddRole
    {
        internal string AddAzureNodeWebRoleProcess(string webRoleName, int instances, string rootPath)
        {
            string result;
            AzureService service = new AzureService(rootPath, null);
            RoleInfo webRole = service.AddWebRole(webRoleName, instances);
            try
            {
                service.ChangeRolePermissions(webRole);
            }
            catch (UnauthorizedAccessException)
            {
                SafeWriteObject(Resources.AddRoleMessageInsufficientPermissions);
            }

            result = string.Format(Resources.AddRoleMessageCreate, rootPath, webRole.Name);
            return result;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                string result = AddAzureNodeWebRoleProcess(Name, Instances, base.GetServiceRootPath());
                SafeWriteObject(result);
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
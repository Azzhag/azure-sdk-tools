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


using System.Linq;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.ServiceDefinitionSchema;
using AzureDeploymentCmdlets.Utilities;

namespace AzureDeploymentCmdlets.Model
{
    /// <summary>
    /// RoleInfo implementation for WebRole
    /// </summary>
    internal class WebRoleInfo : RoleInfo
    {
        public WebRoleInfo(string name, int instanceCount) : base(name, instanceCount) { }

        internal override void AddRoleToDefinition(ServiceDefinition def, object template)
        {
            WebRole webRole = template as WebRole;
            var toAdd = new WebRole[] { webRole };

            if (def.WebRole != null)
            {
                def.WebRole = def.WebRole.Concat(toAdd).ToArray();
            }
            else
            {
                def.WebRole = toAdd;
            }
        }
    }
}
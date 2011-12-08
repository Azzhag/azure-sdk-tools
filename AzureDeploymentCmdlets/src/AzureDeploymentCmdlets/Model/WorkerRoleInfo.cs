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


using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Xml.Serialization;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.ServiceConfigurationSchema;
using AzureDeploymentCmdlets.ServiceDefinitionSchema;
using AzureDeploymentCmdlets.Utilities;
using System.Linq;
using System.Collections;
using AzureDeploymentCmdlets.Model;


namespace AzureDeploymentCmdlets.Model
{
    /// <summary>
    /// Role Info for a worker role
    /// </summary>
    internal class WorkerRoleInfo : RoleInfo
    {
        public WorkerRoleInfo(string name, int instanceCount = 1) : base(name, instanceCount) { }

        internal override void AddRoleToDefinition(ServiceDefinition def, object template)
        {
            base.AddRoleToDefinition(def, template);
            WorkerRole workerRole = template as WorkerRole;
            var toAdd = new WorkerRole[] { workerRole };

            if (def.WorkerRole != null)
            {
                def.WorkerRole = def.WorkerRole.Concat(toAdd).ToArray();
            }
            else
            {
                def.WorkerRole = toAdd;
            }
        }
    }
}

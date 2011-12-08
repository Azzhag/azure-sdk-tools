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

namespace AzureDeploymentCmdlets.Model
{
    /// <summary>
    /// Base class for describing roles that we will create.
    /// </summary>
    public abstract class RoleInfo
    {
        public int InstanceCount { get; private set; }
        public string Name { get; private set; }

        public RoleInfo(string name, int instanceCount)
        {
            Name = name;
            InstanceCount = instanceCount;
        }

        internal virtual void AddRoleToDefinition(ServiceDefinition serviceDefinition, object template)
        {
            Validate.ValidateNullArgument(template, string.Format(Resources.NullRoleSettingsMessage, "service definition"));
            Validate.ValidateNullArgument(serviceDefinition, Resources.NullServiceDefinitionMessage);
        }

        /// <summary>
        /// Checks for equality between provided object and this object
        /// </summary>
        /// <param name="obj">This object can be type of RoleInfo, WebRoleInfo, WorkerRoleInfo, WebRole or WorkerRole</param>
        /// <returns>True if they are equals, false if not</returns>
        public override bool Equals(object obj)
        {
            Validate.ValidateNullArgument(obj, string.Empty);
            bool equals;
            RoleInfo roleInfo = obj as RoleInfo;
            WebRole webRole = obj as WebRole;
            WorkerRole workerRole = obj as WorkerRole;
            RoleSettings role = obj as RoleSettings;

            if (roleInfo != null)
            {
                equals = this.InstanceCount.Equals(roleInfo.InstanceCount) &&
                    this.Name.Equals(roleInfo.Name);
            }
            else if (webRole != null)
            {
                equals = this.Name.Equals(webRole.name);
            }
            else if (workerRole != null)
            {
                equals = this.Name.Equals(workerRole.name);
            }
            else if (role != null)
            {
                equals = this.Name.Equals(role.name) &&
                    this.InstanceCount.Equals(role.Instances.count);
            }
            else
            {
                equals = false;
            }

            return equals;
        }

        public override int GetHashCode()
        {
            return
                InstanceCount.GetHashCode() ^
                Name.GetHashCode();
        }
    }
}
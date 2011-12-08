using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Utilities;
using AzureDeploymentCmdlets.ServiceConfigurationSchema;
using AzureDeploymentCmdlets.ServiceDefinitionSchema;

namespace AzureDeploymentCmdlets.Scaffolding
{
    public static class NodeRules
    {
        public static void AddRoleToConfig(string path, Dictionary<string, object> parameters)
        {
            RoleInfo role = parameters["Role"] as RoleInfo;
            ServiceComponents components = parameters["Components"] as ServiceComponents;
            ServicePathInfo paths = parameters["Paths"] as ServicePathInfo;
            RoleSettings settings = General.DeserializeXmlFile<ServiceConfiguration>(path).Role[0];

            components.AddRoleToConfiguration(settings, DevEnv.Cloud);
            components.AddRoleToConfiguration(settings, DevEnv.Local);
            components.Save(paths);
        }

        public static void AddWebRoleToDef(string path, Dictionary<string, object> parameters)
        {
            RoleInfo role = parameters["Role"] as RoleInfo;
            ServiceComponents components = parameters["Components"] as ServiceComponents;
            ServicePathInfo paths = parameters["Paths"] as ServicePathInfo;
            WebRole webRole = General.DeserializeXmlFile<ServiceDefinition>(path).WebRole[0];

            role.AddRoleToDefinition(components.Definition, webRole);
            components.Save(paths);
        }

        public static void AddWorkerRoleToDef(string path, Dictionary<string, object> parameters)
        {
            RoleInfo role = parameters["Role"] as RoleInfo;
            ServiceComponents components = parameters["Components"] as ServiceComponents;
            ServicePathInfo paths = parameters["Paths"] as ServicePathInfo;
            WorkerRole workerRole = General.DeserializeXmlFile<ServiceDefinition>(path).WorkerRole[0];

            role.AddRoleToDefinition(components.Definition, workerRole);
            components.Save(paths);
        }
    }
}
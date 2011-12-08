using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using AzureDeploymentTool.Model;
using AzureDeploymentTool.Properties;
using AzureDeploymentTool.ServiceDefinitionSchema;

namespace AzureDeploymentTool.Utilities
{
    class CsPack
    {
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static void CreatePackage(ServiceDefinition definition, string rootPath, PackageType type, out string standardOutput, out string standardError)
        {
            string arguments;

            arguments = ConstructArgs(definition, rootPath, type);
            Execute(arguments, out standardOutput, out standardError);
        }

        private static string ConstructArgs(ServiceDefinition serviceDefinition, string rootPath, PackageType type)
        {
            string arguments;
            string rolesArg = "";
            string sitesArg = "";

            if (serviceDefinition == null) throw new ArgumentNullException("serviceDefinition", string.Format(Resources.InvalidOrEmptyArgumentMessage, "Service definition"));
            if (string.IsNullOrEmpty(rootPath) || System.IO.File.Exists(rootPath)) throw new ArgumentException(Resources.InvalidRootNameMessage, "rootPath");

            if (serviceDefinition.WebRole != null)
            {
                foreach (WebRole webRole in serviceDefinition.WebRole)
                {
                    rolesArg += string.Format(Resources.RoleArgTemplate, webRole.name, rootPath);

                    foreach (Site site in webRole.Sites.Site)
                    {
                        sitesArg += string.Format(Resources.SitesArgTemplate, webRole.name, site.name, rootPath);
                    }
                }
            }

            if (serviceDefinition.WorkerRole != null)
            {
                foreach (WorkerRole workerRole in serviceDefinition.WorkerRole)
                {
                    rolesArg += string.Format(Resources.RoleArgTemplate, workerRole.name, rootPath);
                }
            }

            arguments = string.Format((type == PackageType.Local) ? Resources.CsPackLocalArg : Resources.CsPackCloudArg, rootPath, rolesArg, sitesArg);
            return arguments;
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private static void Execute(string arguments, out string standardOutput, out string standardError)
        {
            ProcessStartInfo pi = new ProcessStartInfo(
                Path.Combine(General.AzureSDKBinFolder, Resources.CsPackExe),
                arguments);
            ProcessHelper.StartAndWaitForProcess(pi, out standardOutput, out standardError);
        }
    }
}
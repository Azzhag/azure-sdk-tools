using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzureDeploymentCmdlets.Properties;
using System.IO;
using AzureDeploymentCmdlets.Utilities;

namespace AzureDeploymentCmdlets.Model
{
    public class ServicePathInfo
    {
        public string Definition { get; private set; }
        public string CloudConfiguration { get; private set; }
        public string LocalConfiguration { get; private set; }
        public string Settings { get; private set; }
        public string CloudPackage { get; private set; }
        public string LocalPackage { get; private set; }
        public string RootPath { get; private set; }

        public ServicePathInfo(string rootPath)
        {
            Validate.ValidateStringIsNullOrEmpty(rootPath, "service root");
            Validate.ValidatePathName(rootPath, Resources.InvalidRootNameMessage);

            RootPath = rootPath;
            Definition = Path.Combine(rootPath, Resources.ServiceDefinitionFileName);
            CloudConfiguration = Path.Combine(rootPath, Resources.CloudServiceConfigurationFileName);
            LocalConfiguration = Path.Combine(rootPath, Resources.LocalServiceConfigurationFileName);
            Settings = Path.Combine(rootPath, Resources.SettingsFileName);
            CloudPackage = Path.Combine(rootPath, Resources.CloudPackageFileName);
            LocalPackage = Path.Combine(rootPath, Resources.LocalPackageFileName);
        }
    }
}
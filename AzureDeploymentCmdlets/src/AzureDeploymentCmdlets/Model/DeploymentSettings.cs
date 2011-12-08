using System;
using System.IO;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Utilities;

namespace AzureDeploymentCmdlets.Model
{
    public class DeploymentSettings
    {
        public ServiceSettings ServiceSettings { get; private set; }
        public string PackagePath { get; private set; }
        public string ConfigPath { get; private set; }
        public string Label { get; private set; }
        public string DeploymentName { get; private set; }
        public string SubscriptionId { get; private set; }

        public DeploymentSettings(ServiceSettings settings, string packagePath, string configPath, string label, string deploymentName)
        {
            Validate.ValidateNullArgument(settings, Resources.InvalidServiceSettingMessage);
            Validate.ValidateFileFull(packagePath, Resources.Package);
            Validate.ValidateFileFull(configPath, Resources.ServiceConfiguration);
            Validate.ValidateStringIsNullOrEmpty(label, "Label");
            Validate.ValidateStringIsNullOrEmpty(deploymentName, "Deployment name");

            this.ServiceSettings = settings;
            this.PackagePath = packagePath;
            this.ConfigPath = configPath;
            this.Label = label;
            this.DeploymentName = deploymentName;

            if (!string.IsNullOrEmpty(settings.Subscription))
            {
                GlobalComponents globalComponents = new GlobalComponents(GlobalPathInfo.GlobalSettingsDirectory);
                SubscriptionId = globalComponents.GetSubscriptionId(settings.Subscription);
            }
            else
            {
                throw new ArgumentNullException("settings.Subscription", Resources.InvalidSubscriptionNameMessage);
            }
        }
    }
}
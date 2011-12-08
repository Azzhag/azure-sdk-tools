using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Utilities;
using System.IO;

namespace AzureDeploymentCmdlets.AzureTools
{
    public class AzureTool
    {
        public string AzureSdkDirectory { get; private set; }
        public string AzureSdkBinDirectory { get; private set; }
        public string AzureEmulatorDirectory { get; private set; }
        public string AzureSdkVersion { get; private set; }

        public AzureTool()
        {
            string min = Resources.MinSupportAzureSdkVersion;
            string max = Resources.MaxSupportAzureSdkVersion;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(Resources.AzureSdkRegistryKeyName);
            AzureSdkVersion = key.GetSubKeyNames().Where(n => (n.CompareTo(min) == 1 && n.CompareTo(max) == -1) || n.CompareTo(min) == 0 || n.CompareTo(max) == 0).Max<string>();
            
            if (string.IsNullOrEmpty(AzureSdkVersion) && key.GetSubKeyNames().Length > 0)
            {
                throw new Exception(string.Format(Resources.AzureSdkVersionNotSupported, min, max));
            }
            else if (string.IsNullOrEmpty(AzureSdkVersion) && key.GetSubKeyNames().Length == 0)
            {
                throw new Exception(Resources.AzureSdkNotInstalledMessage);
            }
            else
            {
                string keyName = Path.Combine(Resources.AzureSdkRegistryKeyName, AzureSdkVersion);
                AzureSdkDirectory = (string)Registry.GetValue(Path.Combine(Registry.LocalMachine.Name, keyName), Resources.AzureSdkInstallPathRegistryKeyValue, null);
                AzureSdkBinDirectory = Path.Combine(AzureSdkDirectory, Resources.RoleBinFolderName);
                AzureEmulatorDirectory = AzureSdkDirectory.Replace(string.Format(Resources.AzureSdkDirectory, AzureSdkVersion), Resources.AzureEmulatorPathPortion);
            }
        }
    }
}
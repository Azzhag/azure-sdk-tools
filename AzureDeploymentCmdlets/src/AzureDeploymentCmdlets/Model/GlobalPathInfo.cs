using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzureDeploymentCmdlets.Properties;
using System.IO;

namespace AzureDeploymentCmdlets.Model
{
    internal class GlobalPathInfo
    {
        public string PublishSettings { get; private set; }
        public string GlobalSettings { get; private set; }
        public static readonly string AzureSdkAppDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.AzureSdkDirectoryName);

        /// <summary>
        /// Path to the global settings directory used by GlobalComponents.
        /// </summary>
        private static string _globalSettingsDirectory = null;

        /// <summary>
        /// Gets a path to the global settings directory, which defaults to
        /// AzureSdkAppDir.  This can be set internally for the purpose of
        /// testing.
        /// </summary>
        public static string GlobalSettingsDirectory
        {
            get { return _globalSettingsDirectory ?? AzureSdkAppDir; }
            internal set { _globalSettingsDirectory = value; }
        }
	
        public string AzureSdkDirectory { get; private set; }

        public GlobalPathInfo(string rootPath)
        {
            PublishSettings = Path.Combine(rootPath, Resources.PublishSettingsFileName);
            GlobalSettings = Path.Combine(rootPath, Resources.SettingsFileName);
            AzureSdkDirectory = rootPath;
        }
    }
}
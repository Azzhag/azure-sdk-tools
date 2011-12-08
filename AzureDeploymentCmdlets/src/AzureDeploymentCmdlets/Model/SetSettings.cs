using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace AzureDeploymentCmdlets.Model
{
    public class SetSettings : CmdletBase
    {
        // Uncomment this to enable global set for settings
        //[Parameter(Position = 1, Mandatory = false)]
        //[Alias("g")]
        public SwitchParameter Global { get; set; }

        internal string GetServiceSettingsPath(bool global)
        {
            string path;

            if (global)
            {
                path = new GlobalComponents(GlobalPathInfo.GlobalSettingsDirectory).GlobalPaths.GlobalSettings;
            }
            else
            {
                path = new AzureService(base.GetServiceRootPath(), null).Paths.Settings;
            }

            return path;
        }
    }
}
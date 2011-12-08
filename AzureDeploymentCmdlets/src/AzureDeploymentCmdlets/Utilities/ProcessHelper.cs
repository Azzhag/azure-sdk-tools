using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security.Permissions;

namespace AzureDeploymentCmdlets.Utilities
{
    internal static class ProcessHelper
    {
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        internal static void Start(string target)
        {
            Process.Start(target);
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        internal static void StartAndWaitForProcess(ProcessStartInfo processInfo, out string standardOutput, out string standardError)
        {
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            Process p = Process.Start(processInfo);
            p.WaitForExit();
            standardOutput = p.StandardOutput.ReadToEnd();
            standardError = p.StandardError.ReadToEnd();
        }
    }
}

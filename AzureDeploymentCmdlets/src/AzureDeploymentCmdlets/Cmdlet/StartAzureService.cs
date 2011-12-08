using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzureDeploymentCmdlets.Model;
using System.Management.Automation;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.WAPPSCmdlet;

namespace AzureDeploymentCmdlets.Cmdlet
{
    /// <summary>
    /// Starts the deployment of specified slot in the azure service
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "AzureService")]
    public class StartAzureService : DeploymentStatusManager
    {
        /// <summary>
        /// SetDeploymentStatus will handle the execution of this cmdlet
        /// </summary>
        public StartAzureService()
        {
            Status = DeploymentStatus.Running;
        }

        public StartAzureService(IServiceManagement channel): base(channel)
        {
            Status = DeploymentStatus.Running;
        }

        public override string SetDeploymentStatusProcess(string rootPath, string newStatus, string slot, string subscription, string serviceName)
        {
            SafeWriteObjectWithTimestamp(Resources.StartServiceMessage, serviceName);
            var message = base.SetDeploymentStatusProcess(rootPath, newStatus, slot, subscription, serviceName);
            if (string.IsNullOrEmpty(message))
            {
                SafeWriteObjectWithTimestamp(Resources.CompleteMessage);
            }
            else
            {
                SafeWriteObjectWithTimestamp(message);
            }
            return message;
        }
    }
}
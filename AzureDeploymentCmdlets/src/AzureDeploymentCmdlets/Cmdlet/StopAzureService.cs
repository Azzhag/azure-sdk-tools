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
    /// Stops the deployment of specified slot in the azure service
    /// </summary>
    [Cmdlet(VerbsLifecycle.Stop, "AzureService")]
    public class StopAzureService : DeploymentStatusManager
    {
        /// <summary>
        /// SetDeploymentStatus will handle the execution of this cmdlet
        /// </summary>
        public StopAzureService()
        {
            base.Status = DeploymentStatus.Suspended;
        }

        public StopAzureService(IServiceManagement channel)
            : base(channel)
        {
            Status = DeploymentStatus.Suspended;
        }

        public override string SetDeploymentStatusProcess(string rootPath, string newStatus, string slot, string subscription, string serviceName)
        {
            SafeWriteObjectWithTimestamp(Resources.StopServiceMessage, serviceName);
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

// ----------------------------------------------------------------------------------
// 
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace AzureDeploymentCmdlets.Cmdlet
{
    using System;
    using System.IO;
    using System.Management.Automation;
    using System.Reflection;
    using System.Xml.Serialization;
    using AzureDeploymentCmdlets.Properties;
    using AzureDeploymentCmdlets.ServiceConfigurationSchema;
    using AzureDeploymentCmdlets.ServiceDefinitionSchema;
    using AzureDeploymentCmdlets.Utilities;
    using AzureDeploymentCmdlets.Model;

    /// <summary>
    /// Create scaffolding for a new hosted service. Generates a basic folder structure, 
    /// default cscfg file which wires up node/iisnode at startup in Azure as well as startup.js. 
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureService")]
    public class NewAzureServiceCommand : CmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Name of the node.js project")]
        public string Name { get; set; }

        internal string NewAzureServiceProcess(string parentDirectory, string serviceName)
        {
            string message;
            AzureService newService;

            // Create scaffolding structure
            //
            newService = new AzureService(parentDirectory, serviceName, null);
            
            message = string.Format(Resources.NewServiceCreatedMessage, newService.Paths.RootPath);

            return message;
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                
                // Create new service
                //
                string result = this.NewAzureServiceProcess(base.CurrentPath(), Name);
                // Set current directory to the root of the new service
                //
                SessionState.Path.SetLocation(Path.Combine(base.CurrentPath(), Name));
                SafeWriteObject(result);
            }
            catch (Exception ex)
            {
                SafeWriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}
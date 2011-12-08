// ----------------------------------------------------------------------------------
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

namespace AzureDeploymentCmdlets
{
    using System.ComponentModel;
    using System.Management.Automation;

    /// <summary>
    /// Snap-in package definition for the Azure Deployment Tool
    /// </summary>
    [RunInstaller(true)]
    public class AzureDeploymentCmdletsSnapIn : PSSnapIn
    {
        /// <summary>
        /// Create an instance of AzureDeploymentCmdletsSnapIn class
        /// </summary>
        public AzureDeploymentCmdletsSnapIn()
            : base()
        {
        }

        /// <summary>
        /// Specify a description of the AzureDeploymentCmdlets powershell snap-in
        /// </summary>
        public override string Description
        {
            get { return "Cmdlets to create, configure, emulate and deploy Azure services"; }
        }

        /// <summary>
        /// Specify the name of the AzureDeploymentCmdlets powershell snap-in
        /// </summary>
        public override string Name
        {
            get { return "AzureDeploymentCmdlets"; }
        }

        /// <summary>
        /// Specify the vendor for the AzureDeploymentCmdlets powershell snap-in
        /// </summary>
        public override string Vendor
        {
            get { return "Microsoft Corporation"; }
        }
    }
}

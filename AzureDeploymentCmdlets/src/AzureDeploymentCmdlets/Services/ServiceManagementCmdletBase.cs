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

namespace AzureDeploymentCmdlets.WAPPSCmdlet
{
    using System;
    using AzureDeploymentCmdlets.Model;

    public class ServiceManagementCmdletBase : CloudCmdlet<IServiceManagement>
    {
        protected override IServiceManagement CreateChannel()
        {
            if (this.ServiceBinding == null)
            {
                this.ServiceBinding = ConfigurationConstants.WebHttpBinding(this.MaxStringContentLength);
            }

            if (string.IsNullOrEmpty(this.ServiceEndpoint))
            {
                this.ServiceEndpoint = ConfigurationConstants.ServiceManagementEndpoint;
            }

            return ServiceManagementHelper.CreateServiceManagementChannel(this.ServiceBinding, new Uri(this.ServiceEndpoint), this.certificate);
        }
    }
}

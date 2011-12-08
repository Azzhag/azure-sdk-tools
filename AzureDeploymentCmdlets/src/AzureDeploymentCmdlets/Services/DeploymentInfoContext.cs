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
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;

    public class DeploymentInfoContext : ManagementOperationContext
    {
        private readonly XNamespace ns = "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration";

        private Deployment innerDeployment = new Deployment();

        public DeploymentInfoContext(Deployment innerDeployment)
        {
            this.innerDeployment = innerDeployment;

            if (this.innerDeployment.RoleInstanceList != null)
            {
                this.RoleInstanceList = new List<AzureDeploymentCmdlets.Concrete.RoleInstance>();
                foreach (var roleInstance in this.innerDeployment.RoleInstanceList)
                {
                    this.RoleInstanceList.Add(new AzureDeploymentCmdlets.Concrete.RoleInstance(roleInstance));
                }
            }

            if (!string.IsNullOrEmpty(this.innerDeployment.Configuration))
            {
                string xmlString = ServiceManagementHelper.DecodeFromBase64String(this.innerDeployment.Configuration);

                XDocument doc = null;
                using (var stringReader = new StringReader(xmlString))
                {
                    XmlReader reader = XmlReader.Create(stringReader);
                    doc = XDocument.Load(reader);
                }

                this.OSVersion = doc.Root.Attribute("osVersion") != null ?
                                 doc.Root.Attribute("osVersion").Value :
                                 string.Empty;

                this.RolesConfiguration = new Dictionary<string, RoleConfiguration>();

                var roles = doc.Root.Descendants(this.ns + "Role");

                foreach (var role in roles)
                {
                    this.RolesConfiguration.Add(role.Attribute("name").Value, new RoleConfiguration(role));
                }
            }
        }

        public string Slot
        {
            get
            {
                return this.innerDeployment.DeploymentSlot;
            }
        }

        public string Name
        {
            get
            {
                return this.innerDeployment.Name;
            }
        }

        public Uri Url
        {
            get
            {
                return this.innerDeployment.Url;
            }
        }

        public string Status
        {
            get
            {
                return this.innerDeployment.Status;
            }
        }

        public IList<AzureDeploymentCmdlets.Concrete.RoleInstance> RoleInstanceList
        {
            get;
            protected set;
        }

        public string Configuration
        {
            get
            {
                return string.IsNullOrEmpty(this.innerDeployment.Configuration) ?
                    string.Empty :
                    ServiceManagementHelper.DecodeFromBase64String(this.innerDeployment.Configuration);
            }
        }

        public string DeploymentId
        {
            get
            {
                return this.innerDeployment.PrivateID;
            }
        }

        public string Label
        {
            get
            {
                return string.IsNullOrEmpty(this.innerDeployment.Label) ?
                    string.Empty :
                    ServiceManagementHelper.DecodeFromBase64String(this.innerDeployment.Label);
            }
        }

        public string OSVersion { get; set; }

        public IDictionary<string, RoleConfiguration> RolesConfiguration
        {
            get;
            protected set;
        }

        public XDocument SerializeRolesConfiguration()
        {
            XDocument document = new XDocument();

            XElement rootElement = new XElement(this.ns + "ServiceConfiguration");
            document.Add(rootElement);

            rootElement.SetAttributeValue("serviceName", this.ServiceName);
            rootElement.SetAttributeValue("osVersion", this.OSVersion);
            rootElement.SetAttributeValue("xmlns", this.ns.ToString());

            foreach (var roleConfig in this.RolesConfiguration)
            {
                rootElement.Add(roleConfig.Value.Serialize());
            }

            return document;
        }
    }
}

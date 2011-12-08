﻿// ----------------------------------------------------------------------------------
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

using System.IO;
using AzureDeploymentCmdlets.Cmdlet;
using AzureDeploymentCmdlets.Node.Cmdlet;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Test.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureDeploymentCmdlets.Test.Tests.Node.Cmdlet
{
    [TestClass]
    public class AddAzureNodeWebRoleTests : TestBase
    {
        [TestMethod]
        public void AddAzureNodeWebRoleProcess()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                new NewAzureServiceCommand().NewAzureServiceProcess(files.RootPath, "AzureService");
                new AddAzureNodeWebRoleCommand().AddAzureNodeWebRoleProcess("WebRole", 1, Path.Combine(files.RootPath, "AzureService"));

                AzureAssert.ScaffoldingExists(Path.Combine(files.RootPath, "AzureService", "WebRole"), Path.Combine(Resources.NodeScaffolding, Resources.WebRole));
            }
        }
    }
}
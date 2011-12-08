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

using System;
using System.IO;
using System.Reflection;
using AzureDeploymentCmdlets.Cmdlet;
using AzureDeploymentCmdlets.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureDeploymentCmdlets.Test.TestData;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Test.Utilities;

namespace AzureDeploymentCmdlets.Test.Tests.Cmdlet
{
    [TestClass]
    public class NewAzureServiceTests : TestBase
    {
        public void NewAzureServiceProcessTest()
        {
            string serviceName = "AzureService";
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                new NewAzureServiceCommand().NewAzureServiceProcess(files.RootPath, serviceName);

                AzureAssert.AzureServiceExists(files.RootPath, Resources.GeneralScaffolding, serviceName);
            }
        }
    }
}
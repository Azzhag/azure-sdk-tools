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

using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Test.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace AzureDeploymentCmdlets.Test.Tests.Model
{
    [TestClass]
    public class ServiceSettingsTests : TestBase
    {
        [TestMethod]
        public void ServiceSettingsTest()
        {
            ServiceSettings settings = new ServiceSettings();
            AzureAssert.AreEqualServiceSettings(string.Empty, string.Empty, string.Empty, string.Empty, settings);
        }
    }
}
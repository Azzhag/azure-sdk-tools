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
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Test.TestData;
using AzureDeploymentCmdlets.Test.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureDeploymentCmdlets.Test.Tests.Model
{
    [TestClass]
    public class GlobalPathInfoTest
    {
        [TestMethod]
        public void GlobalPathInfoTests()
        {
            GlobalPathInfo pathInfo = new GlobalPathInfo(Data.AzureSdkAppDir);
            string azureSdkPath = Data.AzureSdkAppDir;
            AzureAssert.AreEqualGlobalPathInfo(azureSdkPath, Path.Combine(azureSdkPath, Resources.SettingsFileName), Path.Combine(azureSdkPath, Resources.PublishSettingsFileName), pathInfo);
        }
    }
}
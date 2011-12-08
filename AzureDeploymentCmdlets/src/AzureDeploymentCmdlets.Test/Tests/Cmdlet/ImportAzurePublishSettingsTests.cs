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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureDeploymentCmdlets.Cmdlet;
using System.IO;
using AzureDeploymentCmdlets.Properties;
using System.Security.Cryptography.X509Certificates;
using AzureDeploymentCmdlets.Test.Model;
using AzureDeploymentCmdlets.Utilities;
using AzureDeploymentCmdlets.Test.Utilities;
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.PublishSettingsSchema;
using AzureDeploymentCmdlets.Test.TestData;

namespace AzureDeploymentCmdlets.Test.Tests.Cmdlet
{
    [TestClass]
    public class ImportAzurePublishSettingsTests
    {
        [TestCleanup()]
        public void TestCleanup()
        {
            try { new RemoveAzurePublishSettingsCommand().RemovePublishSettingsProcess(Data.AzureSdkAppDir); }
            catch { }
        }

        [TestMethod]
        public void ImportAzurePublishSettingsProcessTests()
        {
            GlobalPathInfo globalPathInfo = new GlobalPathInfo(Data.AzureSdkAppDir);

            foreach (string filePath in Data.ValidPublishSettings)
            {
                new ImportAzurePublishSettingsCommand().ImportAzurePublishSettingsProcess(filePath, Data.AzureSdkAppDir);
                PublishData expectedPublishSettings = General.DeserializeXmlFile<PublishData>(filePath);
                PublishData actualPublishSettings = General.DeserializeXmlFile<PublishData>(globalPathInfo.PublishSettings);
                string thmbprint = actualPublishSettings.Items[0].ManagementCertificate;
                AzureAssert.AreEqualGlobalComponents(thmbprint, globalPathInfo, new ServiceSettings(), actualPublishSettings, new GlobalComponents(Data.AzureSdkAppDir));
            }
        }

        [TestMethod]
        public void ImportAzurePublishSettingsProcessTestsFail()
        {   
            foreach (string filePath in Data.InvalidPublishSettings)
            {
                try
                {
                    new ImportAzurePublishSettingsCommand().ImportAzurePublishSettingsProcess(filePath, Data.AzureSdkAppDir);
                    Assert.Fail("no exception thrown");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex is InvalidOperationException);
                    Assert.AreEqual<string>(ex.Message, string.Format(Resources.InvalidPublishSettingsSchema, filePath));
                }
            }
        }
    }
}
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
using AzureDeploymentCmdlets.Cmdlet;
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.PublishSettingsSchema;
using AzureDeploymentCmdlets.Test.Model;
using AzureDeploymentCmdlets.Test.TestData;
using AzureDeploymentCmdlets.Test.Utilities;
using AzureDeploymentCmdlets.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureDeploymentCmdlets.Test.Tests.Model
{
    [TestClass]
    public class GlobalComponentsTests
    {
        [TestMethod]
        public void GlobalComponentsLoadExisting()
        {
            foreach (string fileName in Data.ValidPublishSettings)
            {
                // Prepare
                new ImportAzurePublishSettingsCommand().ImportAzurePublishSettingsProcess(fileName, Data.AzureSdkAppDir);
                GlobalComponents globalComponents = new GlobalComponents(Data.AzureSdkAppDir);
                PublishData actualPublishSettings = General.DeserializeXmlFile<PublishData>(Path.Combine(Data.AzureSdkAppDir, Resources.PublishSettingsFileName));
                PublishData expectedPublishSettings = General.DeserializeXmlFile<PublishData>(fileName);

                // Assert
                AzureAssert.AreEqualGlobalComponents(actualPublishSettings.Items[0].ManagementCertificate, new GlobalPathInfo(Data.AzureSdkAppDir), new ServiceSettings(), expectedPublishSettings, globalComponents);
                
                // Clean
                new RemoveAzurePublishSettingsCommand().RemovePublishSettingsProcess(Data.AzureSdkAppDir);
            }
        }

        [TestMethod]
        public void GlobalComponentsCreateNew()
        {
            foreach (string fileName in Data.ValidPublishSettings)
            {
                // Prepare
                GlobalComponents globalComponents = new GlobalComponents(fileName, Data.AzureSdkAppDir);
                PublishData actualPublishSettings = General.DeserializeXmlFile<PublishData>(Path.Combine(Data.AzureSdkAppDir, Resources.PublishSettingsFileName));
                PublishData expectedPublishSettings = General.DeserializeXmlFile<PublishData>(fileName);

                // Assert
                AzureAssert.AreEqualGlobalComponents(actualPublishSettings.Items[0].ManagementCertificate, new GlobalPathInfo(Data.AzureSdkAppDir), new ServiceSettings(), expectedPublishSettings, globalComponents);

                // Clean
                new RemoveAzurePublishSettingsCommand().RemovePublishSettingsProcess(Data.AzureSdkAppDir);
            }
        }

        [TestMethod]
        public void GlobalComponentsCreateNewEmptyAzureDirectoryFail()
        {
            foreach (string fileName in Data.ValidPublishSettings)
            {
                try
                {
                    new GlobalComponents(fileName, string.Empty);
                    Assert.Fail("No exception thrown");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex is ArgumentException);
                    Assert.AreEqual<string>(ex.Message, string.Format(Resources.InvalidOrEmptyArgumentMessage, Resources.AzureSdkDirectoryName));
                    Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
                }
            }
        }

        [TestMethod]
        public void GlobalComponentsCreateNewNullAzureDirectoryFail()
        {
            foreach (string fileName in Data.ValidPublishSettings)
            {
                try
                {
                    new GlobalComponents(fileName, null);
                    Assert.Fail("No exception thrown");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex is ArgumentException);
                    Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
                }
            }
        }

        [TestMethod]
        public void GlobalComponentsCreateNewInvalidAzureDirectoryFail()
        {
            foreach (string fileName in Data.ValidPublishSettings)
            {
                foreach (string invalidDirectoryName in Data.InvalidServiceRootName)
                {
                    try
                    {
                        new GlobalComponents(fileName, invalidDirectoryName);
                        Assert.Fail("No exception thrown");
                    }
                    catch (Exception ex)
                    {
                        Assert.IsTrue(ex is ArgumentException);
                        Assert.AreEqual<string>(ex.Message, "Illegal characters in path.");
                        Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
                    }
                }
            }
        }

        [TestMethod]
        public void GlobalComponentsCreateNewInvalidPublishSettingsSchemaFail()
        {
            foreach (string fileName in Data.InvalidPublishSettings)
            {
                try
                {
                    new GlobalComponents(fileName, Data.AzureSdkAppDir);
                    Assert.Fail("No exception thrown");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex is InvalidOperationException);
                    Assert.AreEqual<string>(ex.Message, string.Format(Resources.InvalidPublishSettingsSchema, fileName));
                    Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
                }
            }
        }

        [TestMethod]
        public void GlobalComponentsLoadExistingEmptyAzureDirectoryFail()
        {
            foreach (string fileName in Data.ValidPublishSettings)
            {
                try
                {
                    new GlobalComponents(string.Empty);
                    Assert.Fail("No exception thrown");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex is FileNotFoundException);
                    Assert.AreEqual<string>(ex.Message, Resources.GlobalComponents_Load_PublishSettingsNotFound);
                    Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
                }
            }
        }

        [TestMethod]
        public void GlobalComponentsLoadExistingNullAzureDirectoryFail()
        {
            foreach (string fileName in Data.ValidPublishSettings)
            {
                try
                {
                    new GlobalComponents(null);
                    Assert.Fail("No exception thrown");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex is ArgumentException);
                    Assert.AreEqual<string>(ex.Message, "Value cannot be null.\r\nParameter name: path1");
                    Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
                }
            }
        }

        [TestMethod]
        public void GlobalComponentsLoadExistingInvalidDirectoryNameAzureDirectoryFail()
        {
            foreach (string fileName in Data.ValidPublishSettings)
            {
                foreach (string invalidDirectoryName in Data.InvalidServiceRootName)
                {
                    try
                    {
                        new GlobalComponents(invalidDirectoryName);
                        Assert.Fail("No exception thrown");
                    }
                    catch (Exception ex)
                    {
                        Assert.IsTrue(ex is ArgumentException);
                        Assert.AreEqual<string>(ex.Message, "Illegal characters in path.");
                        Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
                    }
                }
            }
        }

        [TestMethod]
        public void GlobalComponentsLoadDoesNotExistAzureDirectoryFail()
        {
            foreach (string fileName in Data.ValidPublishSettings)
            {
                foreach (string invalidDirectoryName in Data.InvalidServiceRootName)
                {
                    try
                    {
                        new GlobalComponents("DoesNotExistDirectory");
                        Assert.Fail("No exception thrown");
                    }
                    catch (Exception ex)
                    {
                        Assert.IsTrue(ex is FileNotFoundException);
                        Assert.AreEqual<string>(ex.Message, Resources.GlobalComponents_Load_PublishSettingsNotFound);
                        Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
                    }
                }
            }
        }

        [TestMethod]
        public void GlobalComponentsCreateNewEmptyPublishSettingsFileFail()
        {
            try
            {
                new GlobalComponents(string.Empty, Data.AzureSdkAppDir);
                Assert.Fail("No exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentException);
                Assert.AreEqual<string>(ex.Message, string.Format(Resources.InvalidOrEmptyArgumentMessage, Resources.PublishSettings));
                Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
            }
        }

        [TestMethod]
        public void GlobalComponentsCreateNewNullPublishSettingsFileFail()
        {
            try
            {
                new GlobalComponents(null, Data.AzureSdkAppDir);
                Assert.Fail("No exception thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentException);
                Assert.AreEqual<string>(ex.Message, string.Format(Resources.InvalidOrEmptyArgumentMessage, Resources.PublishSettings));
                Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
            }
        }

        [TestMethod]
        public void GlobalComponentsCreateNewInvalidPublishSettingsFileFail()
        {
            foreach (string invalidFileName in Data.InvalidFileName)
            {
                Action<ArgumentException> verification = ex =>
                {
                    Assert.AreEqual<string>(ex.Message, Resources.IllegalPath);
                    Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
                };

                Testing.AssertThrows<ArgumentException>(() => new GlobalComponents(invalidFileName, Data.AzureSdkAppDir), verification);
            }
        }

        [TestMethod]
        public void GlobalComponentsLoadInvalidPublishSettingsSchemaFail()
        {
            Testing.AssertThrows<FileNotFoundException>(
                () => new GlobalComponents("DoesNotExistDirectory"),
                ex =>
                {
                    Assert.AreEqual<string>(ex.Message, Resources.GlobalComponents_Load_PublishSettingsNotFound);
                    Assert.IsFalse(Directory.Exists(Data.AzureSdkAppDir));
                });
        }
    }
}
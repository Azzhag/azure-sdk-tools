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
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Test.TestData;
using AzureDeploymentCmdlets.Test.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureDeploymentCmdlets.Test.Tests.Model
{
    [TestClass]
    public class ServicePathInfoTests
    {
        [TestMethod]
        public void ServicePathInfoTest()
        {
            ServicePathInfo paths = new ServicePathInfo("MyService");
            AzureAssert.AreEqualServicePathInfo("MyService", paths);
        }

        [TestMethod]
        public void ServicePathInfoTestEmptyRootPathFail()
        {
            try
            {
                ServicePathInfo paths = new ServicePathInfo(string.Empty);
                Assert.Fail("No exception was thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentException);
                Assert.AreEqual<string>(string.Format(Resources.InvalidOrEmptyArgumentMessage, "service root"), ex.Message);
            }
        }

        [TestMethod]
        public void ServicePathInfoTestNullRootPathFail()
        {
            try
            {
                ServicePathInfo paths = new ServicePathInfo(null);
                Assert.Fail("No exception was thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentException);
                Assert.AreEqual<string>(string.Format(Resources.InvalidOrEmptyArgumentMessage, "service root"), ex.Message);
            }
        }

        [TestMethod]
        public void ServicePathInfoTestInvalidRootPathFail()
        {
            foreach (string invalidDirectoryName in Data.InvalidServiceRootName)
            {
                try
                {
                    ServicePathInfo paths = new ServicePathInfo(invalidDirectoryName);
                    Assert.Fail("No exception was thrown");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(ex is ArgumentException);
                    Assert.AreEqual<string>(Resources.InvalidRootNameMessage, ex.Message);
                }
            }
        }
    }
}
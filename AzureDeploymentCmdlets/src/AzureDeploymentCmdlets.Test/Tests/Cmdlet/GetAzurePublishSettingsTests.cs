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
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Utilities;
using System.Net;

namespace AzureDeploymentCmdlets.Test.Tests.Cmdlet
{
    [TestClass]
    public class GetAzurePublishSettingsTests : TestBase
    {
        [TestMethod]
        public void GetAzurePublishSettingsProcessTest()
        {
            new GetAzurePublishSettingsCommand().GetAzurePublishSettingsProcess(Resources.PublishSettingsUrl);
        }

        /// <summary>
        /// Happy case, user has internet connection and uri specified is valid.
        /// </summary>
        [TestMethod]
        public void GetAzurePublishSettingsProcessTestFail()
        {
            Assert.IsFalse(string.IsNullOrEmpty(Resources.PublishSettingsUrl));
        }

        /// <summary>
        /// The url doesn't exist.
        /// </summary>
        [TestMethod]
        public void GetAzurePublishSettingsProcessTestEmptyDnsFail()
        {
            string emptyDns = string.Empty;
            string expectedMsg = string.Format(Resources.InvalidOrEmptyArgumentMessage, "publish settings url");
            
            try
            {
                new GetAzurePublishSettingsCommand().GetAzurePublishSettingsProcess(emptyDns);
                Assert.Fail("No exception was thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentException));
                Assert.IsTrue(string.Compare(expectedMsg, ex.Message, true) == 0);
            }
        }
    }
}
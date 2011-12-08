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
using AzureDeploymentCmdlets.Cmdlet;
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Test.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureDeploymentCmdlets.Test.Tests.Cmdlet
{
    [TestClass]
    public class SetAzureDeploymentSubscriptionTests : TestBase
    {
        [TestMethod]
        public void AzureSetDeploymentSubscriptionProcessTests()
        {
            foreach (string item in Data.ValidSubscriptionName)
            {
                using (FileSystemHelper files = new FileSystemHelper(this))
                {
                    // Create new empty settings file
                    //
                    ServicePathInfo paths = new ServicePathInfo(files.RootPath);
                    ServiceSettings settings = new ServiceSettings();
                    settings.Save(paths.Settings);

                    new SetAzureDeploymentSubscriptionCommand().SetAzureDeploymentSubscriptionProcess(item, paths.Settings);

                    // Assert subscription is changed
                    //
                    settings = ServiceSettings.Load(paths.Settings);
                    Assert.AreEqual<string>(item, settings.Subscription);
                }
            }
        }

        [TestMethod]
        public void AzureSetDeploymentSubscriptionProcessTestsEmptyFail()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                // Create new empty settings file
                //
                ServicePathInfo paths = new ServicePathInfo(files.RootPath);
                ServiceSettings settings = new ServiceSettings();
                settings.Save(paths.Settings);

                Testing.AssertThrows<ArgumentException>(() => new SetAzureDeploymentSubscriptionCommand().SetAzureDeploymentSubscriptionProcess(string.Empty, paths.Settings), string.Format(Resources.InvalidOrEmptyArgumentMessage, "Subscription"));
            }
        }

        [TestMethod]
        public void AzureSetDeploymentSubscriptionProcessTestsNullFail()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                // Create new empty settings file
                //
                ServicePathInfo paths = new ServicePathInfo(files.RootPath);
                ServiceSettings settings = new ServiceSettings();
                settings.Save(paths.Settings);

                Testing.AssertThrows<ArgumentException>(() => new SetAzureDeploymentSubscriptionCommand().SetAzureDeploymentSubscriptionProcess(null, paths.Settings), string.Format(Resources.InvalidOrEmptyArgumentMessage, "Subscription"));
            }
        }
    }
}

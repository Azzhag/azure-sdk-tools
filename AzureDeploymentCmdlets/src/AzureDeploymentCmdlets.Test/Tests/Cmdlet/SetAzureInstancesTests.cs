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
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.ServiceConfigurationSchema;
using AzureDeploymentCmdlets.Test.TestData;
using AzureDeploymentCmdlets.Properties;

namespace AzureDeploymentCmdlets.Test.Tests.Cmdlet
{
    [TestClass]
    public class SetAzureInstancesTests : TestBase
    {
        private const string serviceName = "AzureService";

        [TestMethod]
        public void SetAzureInstancesProcessTests()
        {
            int newRoleInstances = 10;

            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                service.AddWebRole();
                new SetAzureInstancesCommand().SetAzureInstancesProcess("WebRole1", newRoleInstances, service.Paths.RootPath);
                service = new AzureService(service.Paths.RootPath, null);

                Assert.AreEqual<int>(newRoleInstances, service.Components.CloudConfig.Role[0].Instances.count);
                Assert.AreEqual<int>(newRoleInstances, service.Components.LocalConfig.Role[0].Instances.count);
            }
        }

        [TestMethod]
        public void SetAzureInstancesProcessTestsRoleNameDoesNotExistFail()
        {
            string roleName = "WebRole1";

            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                Testing.AssertThrows<ArgumentException>(() => service.SetRoleInstances(service.Paths, roleName, 10), string.Format(Resources.RoleNotFoundMessage, roleName));
            }
        }

        [TestMethod]
        public void SetAzureInstancesProcessTestsEmptyRoleNameFail()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                Testing.AssertThrows<ArgumentException>(() => service.SetRoleInstances(service.Paths, string.Empty, 10), string.Format(Resources.InvalidOrEmptyArgumentMessage, Resources.RoleName));
            }
        }

        [TestMethod]
        public void SetAzureInstancesProcessTestsNullRoleNameFail()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                Testing.AssertThrows<ArgumentException>(() => service.SetRoleInstances(service.Paths, null, 10), string.Format(Resources.InvalidOrEmptyArgumentMessage, Resources.RoleName));
            }
        }

        [TestMethod]
        public void SetAzureInstancesProcessTestsLargeRoleInstanceFail()
        {
            string roleName = "WebRole1";

            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                Testing.AssertThrows<ArgumentException>(() => service.SetRoleInstances(service.Paths, roleName, 2000), string.Format(Resources.InvalidInstancesCount, roleName));
            }
        }

        [TestMethod]
        public void SetAzureInstancesProcessNegativeRoleInstanceFail()
        {
            string roleName = "WebRole1";

            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                Testing.AssertThrows<ArgumentException>(() => service.SetRoleInstances(service.Paths, roleName, -1), string.Format(Resources.InvalidInstancesCount, roleName));
            }
        }
    }
}
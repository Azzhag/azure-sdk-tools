// ----------------------------------------------------------------------------------
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
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Test.Utilities;
using System.IO;
using AzureDeploymentCmdlets.Properties;

namespace AzureDeploymentCmdlets.Test.Tests.AzureTools
{
    [TestClass]
    public class CsPackTests : TestBase
    {
        private const string serviceName = "AzureService";

        [TestMethod]
        public void CreateLocalPackageWithOneWebRoleTest()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                string standardOutput;
                string standardError;
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                service.AddWebRole();
                service.CreatePackage(DevEnv.Local, out standardOutput, out standardError);

                AzureAssert.ScaffoldingExists(Path.Combine(service.Paths.LocalPackage, @"roles\WebRole1\approot"), Path.Combine(Resources.NodeScaffolding, Resources.WebRole));
            }
        }

        [TestMethod]
        public void CreateLocalPackageWithWorkerRoleTest()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                string standardOutput;
                string standardError;
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                service.AddWorkerRole();
                service.CreatePackage(DevEnv.Local, out standardOutput, out standardError);

                AzureAssert.ScaffoldingExists(Path.Combine(service.Paths.LocalPackage, @"roles\WorkerRole1\approot"), Path.Combine(Resources.NodeScaffolding, Resources.WorkerRole));
            }
        }

        [TestMethod]
        public void CreateLocalPackageWithMultipleRoles()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                string standardOutput;
                string standardError;
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                service.AddWorkerRole();
                service.AddWebRole();
                service.CreatePackage(DevEnv.Local, out standardOutput, out standardError);

                AzureAssert.ScaffoldingExists(Path.Combine(service.Paths.LocalPackage, @"roles\WorkerRole1\approot"), Path.Combine(Resources.NodeScaffolding, Resources.WorkerRole));
                AzureAssert.ScaffoldingExists(Path.Combine(service.Paths.LocalPackage, @"roles\WebRole1\approot"), Path.Combine(Resources.NodeScaffolding, Resources.WebRole));
            }
        }
    }
}
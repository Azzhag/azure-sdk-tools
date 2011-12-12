// ----------------------------------------------------------------------------------
//
// Copyright 2011 Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
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
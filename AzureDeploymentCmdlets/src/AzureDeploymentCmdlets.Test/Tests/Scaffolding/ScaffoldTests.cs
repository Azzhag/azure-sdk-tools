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
using AzureDeploymentCmdlets.Scaffolding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureDeploymentCmdlets.Test.Tests.Scaffolding
{
    [TestClass]
    public class ScaffoldTests : TestBase
    {
        [TestMethod]
        public void ParseTests()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                string path = files.CreateEmptyFile("Scaffold.xml");
                File.WriteAllText(path, Properties.Resources.ValidScaffoldXml);

                Scaffold scaffold = Scaffold.Parse(path);

                Assert.AreEqual(scaffold.Files.Count, 6);
                Assert.AreEqual(scaffold.Files[0].PathExpression, "modules\\.*");
                Assert.AreEqual(scaffold.Files[1].Path, @"bin/node123dfx65.exe");
                Assert.AreEqual(scaffold.Files[1].TargetPath, @"/bin/node.exe");
                Assert.AreEqual(scaffold.Files[2].Path, @"bin/iisnode.dll");
                Assert.AreEqual(scaffold.Files[3].Path, @"bin/setup.cmd");
                Assert.AreEqual(scaffold.Files[4].Path, "Web.config");
                Assert.AreEqual(scaffold.Files[4].Rules.Count, 1);
                Assert.AreEqual(scaffold.Files[5].Path, "WebRole.xml");
                Assert.AreEqual(scaffold.Files[5].Copy, false);
                Assert.AreEqual(scaffold.Files[5].Rules.Count, 1);
            }
        }
    }
}

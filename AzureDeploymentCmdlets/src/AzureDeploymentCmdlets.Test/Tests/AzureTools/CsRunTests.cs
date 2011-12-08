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

using AzureDeploymentCmdlets.AzureTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureDeploymentCmdlets.Test.Tests.AzureTools
{
    [TestClass]
    public class CsRunTests : TestBase
    {
        [TestMethod]
        public void RoleInfoIsExtractedFromEmulatorOutput()
        {
            var dummyEmulatorOutput = "Exported interface at http://127.0.0.1:81/.\r\nExported interface at tcp://127.0.0.1:8080/.";
            var output = CsRun.GetRoleInfoMessage(dummyEmulatorOutput);
            Assert.IsTrue(output.Contains("Role is running at http://127.0.0.1:81"));
            Assert.IsTrue(output.Contains("Role is running at tcp://127.0.0.1:8080"));
        }
    }
}
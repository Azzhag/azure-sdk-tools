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

using System.IO;
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Test.TestData;
using AzureDeploymentCmdlets.Test.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureDeploymentCmdlets.Test.Tests.Model
{
    [TestClass]
    public class GlobalPathInfoTest
    {
        [TestMethod]
        public void GlobalPathInfoTests()
        {
            GlobalPathInfo pathInfo = new GlobalPathInfo(Data.AzureSdkAppDir);
            string azureSdkPath = Data.AzureSdkAppDir;
            AzureAssert.AreEqualGlobalPathInfo(azureSdkPath, Path.Combine(azureSdkPath, Resources.SettingsFileName), Path.Combine(azureSdkPath, Resources.PublishSettingsFileName), pathInfo);
        }
    }
}
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzureDeploymentCmdlets.Test.Utilities;
using AzureDeploymentCmdlets.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureDeploymentCmdlets.Model;

namespace AzureDeploymentCmdlets.Test.Tests.Cmdlet
{
    [TestClass]
    public class CmdletBaseTests
    {
        [TestMethod]
        public void SafeWriteObjectWritesToWriter()
        {
            var writer = new FakeWriter();
            var cmd = new FakeCmdlet(writer);
            cmd.Write("Test");
            Assert.AreEqual("Test", writer.Messages[0]);
        }

    }

    public class FakeCmdlet : CmdletBase
    {
        public FakeCmdlet(IMessageWriter writer) : base(writer)
        {
            
        }

        public void Write(string message)
        {
            SafeWriteObject(message);
        }
    }
}

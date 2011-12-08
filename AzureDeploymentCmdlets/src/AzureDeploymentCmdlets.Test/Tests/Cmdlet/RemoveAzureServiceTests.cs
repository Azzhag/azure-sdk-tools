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
using AzureDeploymentCmdlets.WAPPSCmdlet;
using AzureDeploymentCmdlets.Test.TestData;
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Cmdlet;
using System.ServiceModel;

namespace AzureDeploymentCmdlets.Test.Tests.Cmdlet
{
    [TestClass]
    public class RemoveAzureServiceTests : TestBase
    {
        private const string serviceName = "AzureService";

        [TestMethod]
        public void RemoveAzureServiceProcessTest()
        {
            SimpleServiceManagement channel = new SimpleServiceManagement();
            bool serviceDeleted = false;
            bool deploymentDeleted = false;
            channel.GetDeploymentBySlotThunk = ar =>
            {
                if (deploymentDeleted) throw new EndpointNotFoundException();
                return new Deployment(serviceName, ArgumentConstants.Slots[Slot.Production], DeploymentStatus.Suspended);
            };
            channel.DeleteHostedServiceThunk = ar => serviceDeleted = true;
            channel.DeleteDeploymentBySlotThunk = ar =>
            {
                deploymentDeleted = true;
            };

            using (FileSystemHelper files = new FileSystemHelper(this))
            {

                files.CreateAzureSdkDirectoryAndImportPublishSettings();
                AzureService service = new AzureService(files.RootPath, serviceName, null);
                new RemoveAzureServiceCommand(channel).RemoveAzureServiceProcess(service.Paths.RootPath, string.Empty);
                Assert.IsTrue(deploymentDeleted);
                Assert.IsTrue(serviceDeleted);
            }
        }
    }
}
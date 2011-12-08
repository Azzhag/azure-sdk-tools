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
using System.IO;
using System.IO.Packaging;
using System.Linq;
using AzureDeploymentCmdlets.Cmdlet;
using AzureDeploymentCmdlets.Node.Cmdlet;
using AzureDeploymentCmdlets.WAPPSCmdlet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureDeploymentCmdlets.Model;
using System.ServiceModel;

namespace AzureDeploymentCmdlets.Test.Tests.Cmdlet
{
    /// <summary>
    /// Tests for the Publish-AzureService command.
    /// </summary>
    [TestClass]
    public class PublishAzureServiceCommandTests : TestBase
    {
        /// <summary>
        /// Test a basic publish scenario.
        ///</summary>
        [TestMethod]
        public void PublishAzureServiceCreateBasicPackageTest()
        {
            // Create a temp directory that we'll use to "publish" our service
            using (FileSystemHelper files = new FileSystemHelper(this) { EnableMonitoring = true })
            {
                // Import our default publish settings
                files.CreateAzureSdkDirectoryAndImportPublishSettings();

                // Create a new channel to mock the calls to Azure and
                // determine all of the results that we'll need.
                SimpleServiceManagement channel = new SimpleServiceManagement();
                
                // Create a new service that we're going to publish
                string serviceName = "TEST_SERVICE_NAME";
                NewAzureServiceCommand newService = new NewAzureServiceCommand();
                newService.NewAzureServiceProcess(files.RootPath, serviceName);
                string servicePath = files.CreateDirectory(serviceName);
                
                // Get the publishing process started by creating the package
                PublishAzureServiceCommand publishService = new PublishAzureServiceCommand(channel);
                publishService.InitializeSettingsAndCreatePackage(servicePath);
                
                // Verify the generated files
                files.AssertFiles(new Dictionary<string, Action<string>>()
                {
                    {
                        serviceName + @"\deploymentSettings.json",
                        null
                    },
                    {
                        serviceName + @"\ServiceDefinition.csdef",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\ServiceConfiguration.Cloud.cscfg",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\ServiceConfiguration.Local.cscfg",
                        p => File.ReadAllText(p).Contains(serviceName)
                    },
                    {
                        serviceName + @"\cloud_package.cspkg",
                        p =>
                        {
                            using (Package package = Package.Open(p))
                            {
                                Assert.AreEqual(5, package.GetParts().Count());
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Test a publish scenario with worker and web roles.
        ///</summary>
        [TestMethod]
        public void PublishAzureServiceCreateWorkersPackageTest()
        {
            // Create a temp directory that we'll use to "publish" our service
            using (FileSystemHelper files = new FileSystemHelper(this) { EnableMonitoring = true })
            {
                // Import our default publish settings
                files.CreateAzureSdkDirectoryAndImportPublishSettings();

                // Create a new channel to mock the calls to Azure and
                // determine all of the results that we'll need.
                SimpleServiceManagement channel = new SimpleServiceManagement();

                // Create a new service that we're going to publish
                string serviceName = "TEST_SERVICE_NAME";
                NewAzureServiceCommand newService = new NewAzureServiceCommand();
                newService.NewAzureServiceProcess(files.RootPath, serviceName);
                string servicePath = files.CreateDirectory(serviceName);
                // Add web and worker roles
                AddAzureNodeWebRoleCommand newWebRole = new AddAzureNodeWebRoleCommand();
                string webRoleName = "NODE_WEB_ROLE";
                string webRolePath = newWebRole.AddAzureNodeWebRoleProcess(webRoleName, 2, servicePath);
                AddAzureNodeWorkerRoleCommand newWorkerRole = new AddAzureNodeWorkerRoleCommand();
                string workerRoleName = "NODE_WORKER_ROLE";
                string workerRolePath = newWorkerRole.AddAzureNodeWorkerRoleProcess(workerRoleName, 2, servicePath);

                // Get the publishing process started by creating the package
                PublishAzureServiceCommand publishService = new PublishAzureServiceCommand(channel);
                publishService.InitializeSettingsAndCreatePackage(servicePath);
                
                // Verify the generated files
                Action<string> verifyContainsNames =
                    p =>
                    {
                        string contents = File.ReadAllText(p);
                        Assert.IsTrue(contents.Contains(webRoleName));
                        Assert.IsTrue(contents.Contains(workerRoleName));
                    };
                files.AssertFiles(new Dictionary<string, Action<string>>()
                {
                    { serviceName + @"\deploymentSettings.json", null },
                    { serviceName + '\\' + webRoleName + @"\server.js", null },
                    { serviceName + '\\' + workerRoleName + @"\server.js", null },
                    { serviceName + @"\ServiceDefinition.csdef", verifyContainsNames },
                    { serviceName + @"\ServiceConfiguration.Cloud.cscfg", verifyContainsNames },
                    { serviceName + @"\ServiceConfiguration.Local.cscfg", verifyContainsNames },
                    {
                        serviceName + @"\cloud_package.cspkg",
                        p =>
                        {
                            using (Package package = Package.Open(p))
                            {
                                Assert.AreEqual(7, package.GetParts().Count());
                            }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Test basic azure service deployment using a mock for the Azure
        /// calls.
        ///</summary>
        [TestMethod]
        public void PublishAzureServiceSimpleDeployTest()
        {
            // Create a temp directory that we'll use to "publish" our service
            using (FileSystemHelper files = new FileSystemHelper(this) { EnableMonitoring = true })
            {
                // Import our default publish settings
                files.CreateAzureSdkDirectoryAndImportPublishSettings();

                // Create a new channel to mock the calls to Azure and
                // determine all of the results that we'll need.
                bool createdHostedService = false;
                bool createdOrUpdatedDeployment = false;
                SimpleServiceManagement channel = new SimpleServiceManagement();
                channel.GetStorageServiceThunk = ar => new StorageService();
                channel.CreateHostedServiceThunk = ar => createdHostedService = true;
                channel.GetHostedServiceWithDetailsThunk = ar => { throw new EndpointNotFoundException(); };
                channel.GetStorageKeysThunk = ar => new StorageService() { StorageServiceKeys = new StorageServiceKeys() { Primary = "VGVzdEtleSE=" } };
                channel.CreateOrUpdateDeploymentThunk = ar => createdOrUpdatedDeployment = true;
                channel.GetDeploymentBySlotThunk = ar => new Deployment() { 
                    Status = DeploymentStatus.Starting,
                    RoleInstanceList = new RoleInstanceList(
                        new RoleInstance[] { 
                            new RoleInstance() {
                                InstanceName = "Role_IN_0",
                                InstanceStatus = RoleInstanceStatus.Ready 
                            } })
                };

                // Create a new service that we're going to publish
                string serviceName = "TEST_SERVICE_NAME";
                NewAzureServiceCommand newService = new NewAzureServiceCommand();
                newService.NewAzureServiceProcess(files.RootPath, serviceName);
                string servicePath = files.CreateDirectory(serviceName);

                // Get the publishing process started by creating the package
                PublishAzureServiceCommand publishService = new PublishAzureServiceCommand(channel);
                publishService.ShareChannel = true;
                publishService.SkipUpload = true;
                publishService.PublishService(servicePath);
                AzureService service = new AzureService(Path.Combine(files.RootPath, serviceName), null);

                // Verify the publish service attempted to create and update
                // the service through the mock.
                Assert.IsTrue(createdHostedService);
                Assert.IsTrue(createdOrUpdatedDeployment);
                Assert.AreEqual<string>(serviceName, service.ServiceName);
            }
        }

        /// <summary>
        /// Helper to create a function which will simulate returning different
        /// values each time its called.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="responses">
        /// Compute the nth reponse when the function is called for the nth
        /// time.
        /// </param>
        /// <returns>
        /// Function to simulate different responses to multiple calls.
        /// </returns>
        public static Func<SimpleServiceManagementAsyncResult, T> MultiCallResponseBuilder<T>(params Func<T>[] responses)
        {
            int count = -1;
            return ar =>
                {
                    count = Math.Min(count + 1, responses.Length - 1);
                    return responses[count]();
                };
        }

        /// <summary>
        /// Test more complex azure service deployment using a mock for the
        /// Azure calls.
        ///</summary>
        [TestMethod]
        public void PublishAzureServiceInvolvedDeployTest()
        {
            // Create a temp directory that we'll use to "publish" our service
            using (FileSystemHelper files = new FileSystemHelper(this) { EnableMonitoring = true })
            {
                // Import our default publish settings
                files.CreateAzureSdkDirectoryAndImportPublishSettings();

                bool createdHostedService = false;
                bool createdOrUpdatedDeployment = false;
                bool storageCreated = false;
                StorageService storageService = new StorageService()
                {
                    StorageServiceProperties = new StorageServiceProperties() { Status = StorageAccountStatus.Creating }
                };
                Deployment deployment = new Deployment()
                {
                    Status = DeploymentStatus.Deploying,
                    RoleInstanceList = new RoleInstanceList(
                        new RoleInstance[] { 
                            new RoleInstance() {
                                InstanceName = "Role_IN_0",
                                InstanceStatus = RoleInstanceStatus.Ready 
                            } })
                };

                // Create a new channel to mock the calls to Azure and
                // determine all of the results that we'll need.
                SimpleServiceManagement channel = new SimpleServiceManagement();
                channel.GetStorageServiceThunk = MultiCallResponseBuilder(
                    () => null,
                    () => storageService,
                    () => { storageService.StorageServiceProperties.Status = StorageAccountStatus.Created; return storageService; });
                channel.CreateStorageAccountThunk = ar => storageCreated = true;
                channel.CreateHostedServiceThunk = ar => createdHostedService = true;
                channel.GetHostedServiceWithDetailsThunk = ar => { throw new EndpointNotFoundException(); };
                channel.GetStorageKeysThunk = ar => new StorageService() { StorageServiceKeys = new StorageServiceKeys() { Primary = "VGVzdEtleSE=" } };
                channel.CreateOrUpdateDeploymentThunk = ar =>
                    {
                        createdOrUpdatedDeployment = true;
                        channel.GetDeploymentBySlotThunk = MultiCallResponseBuilder(
                    () => deployment,
                    () => { deployment.Status = DeploymentStatus.Starting; return deployment; });
                    };
                channel.GetDeploymentBySlotThunk = ar => { throw new EndpointNotFoundException(); };

                // Create a new service that we're going to publish
                string serviceName = "TEST_SERVICE_NAME";
                NewAzureServiceCommand newService = new NewAzureServiceCommand();
                newService.NewAzureServiceProcess(files.RootPath, serviceName);
                string servicePath = files.CreateDirectory(serviceName);

                // Get the publishing process started by creating the package
                PublishAzureServiceCommand publishService = new PublishAzureServiceCommand(channel);
                publishService.ShareChannel = true;
                publishService.SkipUpload = true;
                publishService.Launch = true;
                publishService.PublishService(servicePath);
                AzureService service = new AzureService(Path.Combine(files.RootPath, serviceName), null);

                // Verify the publish service attempted to create and update
                // the service through the mock.
                Assert.IsTrue(storageCreated);
                Assert.IsTrue(createdHostedService);
                Assert.IsTrue(createdOrUpdatedDeployment);
                Assert.AreEqual(StorageAccountStatus.Created, storageService.StorageServiceProperties.Status);
                Assert.AreEqual(DeploymentStatus.Starting, deployment.Status);
                Assert.AreEqual<string>(serviceName, service.ServiceName);
            }
        }

        /// <summary>
        /// Test upgrading azure service deployment using a mock for the Azure
        /// calls.
        ///</summary>
        [TestMethod]
        public void PublishAzureServiceUpgradeTest()
        {
            // Create a temp directory that we'll use to "publish" our service
            using (FileSystemHelper files = new FileSystemHelper(this) { EnableMonitoring = true })
            {
                // Import our default publish settings
                files.CreateAzureSdkDirectoryAndImportPublishSettings();

                // Create a new channel to mock the calls to Azure and
                // determine all of the results that we'll need.
                bool createdHostedService = false;
                bool createdOrUpdatedDeployment = false;
                bool upgradedDeployment = false;
                SimpleServiceManagement channel = new SimpleServiceManagement();
                channel.GetStorageServiceThunk = ar => new StorageService();
                channel.CreateHostedServiceThunk = ar => createdHostedService = true;
                channel.GetHostedServiceWithDetailsThunk = ar => new HostedService { Deployments = new DeploymentList() { new Deployment { DeploymentSlot = "Production" } } };
                channel.GetStorageKeysThunk = ar => new StorageService() { StorageServiceKeys = new StorageServiceKeys() { Primary = "VGVzdEtleSE=" } };
                channel.CreateOrUpdateDeploymentThunk = ar => createdOrUpdatedDeployment = true;
                channel.UpgradeDeploymentThunk = ar => upgradedDeployment = true;
                channel.GetDeploymentBySlotThunk = ar => new Deployment() { 
                    Status = DeploymentStatus.Starting, 
                    RoleInstanceList = new RoleInstanceList(
                        new RoleInstance[] { 
                            new RoleInstance() {
                                InstanceName = "Role_IN_0",
                                InstanceStatus = RoleInstanceStatus.Ready 
                            } }) };

                // Create a new service that we're going to publish
                string serviceName = "TEST_SERVICE_NAME";
                NewAzureServiceCommand newService = new NewAzureServiceCommand();
                newService.NewAzureServiceProcess(files.RootPath, serviceName);
                string servicePath = files.CreateDirectory(serviceName);

                // Get the publishing process started by creating the package
                PublishAzureServiceCommand publishService = new PublishAzureServiceCommand(channel);
                publishService.ShareChannel = true;
                publishService.SkipUpload = true;
                publishService.PublishService(servicePath);

                // Verify the publish service upgraded the deployment
                Assert.IsFalse(createdHostedService);
                Assert.IsFalse(createdOrUpdatedDeployment);
                Assert.IsTrue(upgradedDeployment);
            }
        }

        /// <summary>
        /// Test that service name will change if user supplied a name parameter
        /// </summary>
        [TestMethod]
        public void PublishAzureServiceChangeServiceNameTest()
        {
            // Create a temp directory that we'll use to "publish" our service
            using (FileSystemHelper files = new FileSystemHelper(this) { EnableMonitoring = true })
            {
                // Import our default publish settings
                files.CreateAzureSdkDirectoryAndImportPublishSettings();

                // Create a new channel to mock the calls to Azure and
                // determine all of the results that we'll need.
                bool createdHostedService = false;
                bool createdOrUpdatedDeployment = false;
                SimpleServiceManagement channel = new SimpleServiceManagement();
                channel.GetStorageServiceThunk = ar => new StorageService();
                channel.CreateHostedServiceThunk = ar => createdHostedService = true;
                channel.GetHostedServiceWithDetailsThunk = ar => { throw new EndpointNotFoundException(); };
                channel.GetStorageKeysThunk = ar => new StorageService() { StorageServiceKeys = new StorageServiceKeys() { Primary = "VGVzdEtleSE=" } };
                channel.CreateOrUpdateDeploymentThunk = ar => createdOrUpdatedDeployment = true;
                channel.UpgradeDeploymentThunk = ar => createdOrUpdatedDeployment = true;
                channel.GetDeploymentBySlotThunk = ar => new Deployment() { 
                    Status = DeploymentStatus.Starting,
                    RoleInstanceList = new RoleInstanceList(
                         new RoleInstance[] { 
                            new RoleInstance() {
                                InstanceName = "Role_IN_0",
                                InstanceStatus = RoleInstanceStatus.Ready 
                            } })
                };

                // Create a new service that we're going to publish
                string serviceName = "TEST_SERVICE_NAME";
                NewAzureServiceCommand newService = new NewAzureServiceCommand();
                newService.NewAzureServiceProcess(files.RootPath, serviceName);
                string servicePath = files.CreateDirectory(serviceName);

                // Get the publishing process started by creating the package
                PublishAzureServiceCommand publishService = new PublishAzureServiceCommand(channel);
                publishService.ShareChannel = true;
                publishService.SkipUpload = true;
                string newServiceName = "NewServiceName";
                publishService.Name = newServiceName;
                publishService.PublishService(servicePath);
                AzureService service = new AzureService(Path.Combine(files.RootPath, serviceName), null);

                // Verify the publish service attempted to create and update
                // the service through the mock.
                Assert.IsTrue(createdHostedService);
                Assert.IsTrue(createdOrUpdatedDeployment);
                Assert.AreEqual<string>(newServiceName, service.ServiceName);
            }
        }

        /// <summary>
        /// Test scenario when a service already exists but no such deployment for the slot.
        /// </summary>
        [TestMethod]
        public void PublishAzureServiceCreateDeploymentForExistingService()
        {
            // Create a temp directory that we'll use to "publish" our service
            using (FileSystemHelper files = new FileSystemHelper(this) { EnableMonitoring = true })
            {
                // Import our default publish settings
                files.CreateAzureSdkDirectoryAndImportPublishSettings();

                // Create a new channel to mock the calls to Azure and
                // determine all of the results that we'll need.
                bool createdHostedService = false;
                bool createdOrUpdatedDeployment = false;
                SimpleServiceManagement channel = new SimpleServiceManagement();
                channel.GetStorageServiceThunk = ar => new StorageService();
                channel.CreateHostedServiceThunk = ar => { };
                channel.GetHostedServiceWithDetailsThunk = ar => null;
                channel.GetStorageKeysThunk = ar => new StorageService() { StorageServiceKeys = new StorageServiceKeys() { Primary = "VGVzdEtleSE=" } };
                channel.CreateOrUpdateDeploymentThunk = ar => createdOrUpdatedDeployment = true;
                channel.GetDeploymentBySlotThunk = ar => 
                {
                    if (createdOrUpdatedDeployment)
                    {
                        Deployment deployment = new Deployment("TEST_SERVICE_NAME", "Production", DeploymentStatus.Running);
                        deployment.RoleInstanceList = new RoleInstanceList(new RoleInstance[] { new RoleInstance() { InstanceName = "Role_IN_0", InstanceStatus = RoleInstanceStatus.Ready } });
                        return deployment;
                    }
                    else
                    {
                        throw new EndpointNotFoundException();
                    }
                };

                // Create a new service that we're going to publish
                string serviceName = "TEST_SERVICE_NAME";
                NewAzureServiceCommand newService = new NewAzureServiceCommand();
                newService.NewAzureServiceProcess(files.RootPath, serviceName);
                string servicePath = files.CreateDirectory(serviceName);

                // Get the publishing process started by creating the package
                PublishAzureServiceCommand publishService = new PublishAzureServiceCommand(channel);
                publishService.ShareChannel = true;
                publishService.SkipUpload = true;
                publishService.PublishService(servicePath);
                AzureService service = new AzureService(Path.Combine(files.RootPath, serviceName), null);

                // Verify the publish service attempted to create and update
                // the service through the mock.
                Assert.IsFalse(createdHostedService);
                Assert.IsTrue(createdOrUpdatedDeployment);
                Assert.AreEqual<string>(serviceName, service.ServiceName);
            }
        }
    }
}
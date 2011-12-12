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
using System.Collections.Generic;
using System.Linq;
using System.Security;
using AzureDeploymentCmdlets.Cmdlet;
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Node.Cmdlet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureDeploymentCmdlets.Test
{
    /// <summary>
    /// Basic unit tests for the Enable-AzureRemoteDesktop command.
    /// </summary>
    [TestClass]
    public class EnableAzureRemoteDesktopCommandTest : TestBase
    {
        /// <summary>
        /// Invoke the Enable-AzureRemoteDesktop command.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        private static void EnableRemoteDesktop(string username, string password)
        {
            SecureString securePassword = null;
            if (password != null)
            {
                securePassword = new SecureString();
                foreach (char ch in password)
                {
                    securePassword.AppendChar(ch);
                }
                securePassword.MakeReadOnly();
            }

            EnableAzureRemoteDesktopCommand command = new EnableAzureRemoteDesktopCommand();
            command.Username = username;
            command.Password = securePassword;
            command.EnableRemoteDesktop();
        }

        private static void VerifyWebRole(ServiceDefinitionSchema.WebRole role, bool isForwarder)
        {
            Assert.AreEqual(isForwarder ? 1 : 0, role.Imports.Where(i => i.moduleName == "RemoteForwarder").Count());
            Assert.AreEqual(1, role.Imports.Where(i => i.moduleName == "RemoteAccess").Count());
        }

        private static void VerifyWorkerRole(ServiceDefinitionSchema.WorkerRole role, bool isForwarder)
        {
            Assert.AreEqual(isForwarder ? 1 : 0, role.Imports.Where(i => i.moduleName == "RemoteForwarder").Count());
            Assert.AreEqual(1, role.Imports.Where(i => i.moduleName == "RemoteAccess").Count());
        }

        private static void VerifyRoleSettings(AzureService service)
        {
            IEnumerable<ServiceConfigurationSchema.RoleSettings> settings =
                Enumerable.Concat(
                    service.Components.CloudConfig.Role,
                    service.Components.LocalConfig.Role);
            foreach (ServiceConfigurationSchema.RoleSettings roleSettings in settings)
            {
                Assert.AreEqual(
                    1,
                    roleSettings
                        .Certificates
                        .Where(c => c.name == "Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption")
                        .Count());
            }
        }
        
        /// <summary>
        /// Perform basic parameter validation.
        /// </summary>
        [TestMethod]
        public void EnableRemoteDesktopBasicParameterValidation()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                files.CreateAzureSdkDirectoryAndImportPublishSettings();
                files.CreateNewService("NEW_SERVICE");

                Testing.AssertThrows<ArgumentException>(
                    () => EnableRemoteDesktop(null, null));
                Testing.AssertThrows<ArgumentException>(
                    () => EnableRemoteDesktop(string.Empty, string.Empty));
                Testing.AssertThrows<ArgumentException>(
                    () => EnableRemoteDesktop("user", null));
                Testing.AssertThrows<ArgumentException>(
                    () => EnableRemoteDesktop("user", string.Empty));
                Testing.AssertThrows<ArgumentException>(
                    () => EnableRemoteDesktop("user", "short"));
                Testing.AssertThrows<ArgumentException>(
                    () => EnableRemoteDesktop("user", "onlylower"));
                Testing.AssertThrows<ArgumentException>(
                    () => EnableRemoteDesktop("user", "ONLYUPPER"));
                Testing.AssertThrows<ArgumentException>(
                    () => EnableRemoteDesktop("user", "1234567890"));
            }
        }

        /// <summary>
        /// Enable remote desktop for an empty service.
        /// </summary>
        [TestMethod]
        public void EnableRemoteDesktopForEmptyService()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                files.CreateAzureSdkDirectoryAndImportPublishSettings();
                files.CreateNewService("NEW_SERVICE");
                Testing.AssertThrows<InvalidOperationException>(() =>
                    EnableRemoteDesktop("user", "GoodPassword!"));
            }
        }

        /// <summary>
        /// Enable remote desktop for a simple web role.
        /// </summary>
        [TestMethod]
        public void EnableRemoteDesktopForWebRole()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                files.CreateAzureSdkDirectoryAndImportPublishSettings();
                string root = files.CreateNewService("NEW_SERVICE");
                new AddAzureNodeWebRoleCommand().AddAzureNodeWebRoleProcess("WebRole", 1, root);
                EnableRemoteDesktop("user", "GoodPassword!");

                // Verify the role has been setup with forwarding, access,
                // and certs
                AzureService service = new AzureService(root, null);
                VerifyWebRole(service.Components.Definition.WebRole[0], true);
                VerifyRoleSettings(service);
            }
        }

        /// <summary>
        /// Enable remote desktop for web and worker roles.
        /// </summary>
        [TestMethod]
        public void EnableRemoteDesktopForWebAndWorkerRoles()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                files.CreateAzureSdkDirectoryAndImportPublishSettings();
                string root = files.CreateNewService("NEW_SERVICE");
                new AddAzureNodeWebRoleCommand().AddAzureNodeWebRoleProcess("WebRole", 1, root);
                new AddAzureNodeWorkerRoleCommand().AddAzureNodeWorkerRoleProcess("WorkerRole", 1, root);
                EnableRemoteDesktop("user", "GoodPassword!");

                // Verify the roles have been setup with forwarding, access,
                // and certs
                AzureService service = new AzureService(root, null);
                VerifyWebRole(service.Components.Definition.WebRole[0], false);
                VerifyWorkerRole(service.Components.Definition.WorkerRole[0], true);
                VerifyRoleSettings(service);
            }
        }

        /// <summary>
        /// Enable remote desktop for multiple web and worker roles.
        /// </summary>
        [TestMethod]
        public void EnableRemoteDesktopForMultipleWebAndWorkerRolesTwice()
        {
            using (FileSystemHelper files = new FileSystemHelper(this))
            {
                files.CreateAzureSdkDirectoryAndImportPublishSettings();
                string root = files.CreateNewService("NEW_SERVICE");
                new AddAzureNodeWebRoleCommand().AddAzureNodeWebRoleProcess("WebRole_1", 1, root);
                new AddAzureNodeWebRoleCommand().AddAzureNodeWebRoleProcess("WebRole_2", 1, root);
                new AddAzureNodeWorkerRoleCommand().AddAzureNodeWorkerRoleProcess("WorkerRole_1", 1, root);
                new AddAzureNodeWorkerRoleCommand().AddAzureNodeWorkerRoleProcess("WorkerRole_2", 1, root);
                
                EnableRemoteDesktop("user", "GoodPassword!");
                EnableRemoteDesktop("other", "OtherPassword!");

                // Verify the roles have been setup with forwarding, access,
                // and certs
                AzureService service = new AzureService(root, null);
                VerifyWebRole(service.Components.Definition.WebRole[0], false);
                VerifyWebRole(service.Components.Definition.WebRole[0], false);
                VerifyWorkerRole(service.Components.Definition.WorkerRole[0], true);
                VerifyWorkerRole(service.Components.Definition.WorkerRole[1], false);
                VerifyRoleSettings(service);
            }
        }
    }
}

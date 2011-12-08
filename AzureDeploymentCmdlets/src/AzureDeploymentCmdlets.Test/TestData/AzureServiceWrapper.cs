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
using AzureDeploymentCmdlets.Model;
using System.IO;

namespace AzureDeploymentCmdlets.Test.TestData
{
    class AzureServiceWrapper : AzureService
    {
        public AzureServiceWrapper(string rootPath, string serviceName, string scaffoldingPath) : base(rootPath, serviceName, scaffoldingPath) { }
        
        public AzureServiceWrapper(string rootPath, string scaffoldingPath) : base(rootPath, scaffoldingPath) { }

        public void AddRole(int webRole, int workerRole)
        {
            for (int i = 0; i < webRole; i++)
            {
                AddWebRole();
            }

            for (int i = 0; i < workerRole; i++)
            {
                AddWorkerRole();
            }
        }

        public void CreateVirtualCloudPackage()
        {
            File.Create(Paths.CloudPackage);
        }
    }
}
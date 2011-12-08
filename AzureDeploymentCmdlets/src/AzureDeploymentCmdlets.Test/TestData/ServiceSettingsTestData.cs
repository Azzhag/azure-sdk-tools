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

using System.Collections.Generic;
using AzureDeploymentCmdlets.Model;

namespace AzureDeploymentCmdlets.Test.TestData
{
    class ServiceSettingsTestData
    {
        // To Do: Add bad cases for ServiceSettings
        public List<ServiceSettings> Good;
        public List<ServiceSettings> Bad;
        public Dictionary<ServiceSettingsState, ServiceSettings> Data;
        private static ServiceSettingsTestData instance;

        public static ServiceSettingsTestData Instance { get { if (instance == null) instance = new ServiceSettingsTestData(); return instance; } }

        private ServiceSettingsTestData()
        {
            InitializeGood();
            InitializeBad();
            InitializeData();
        }

        private void InitializeData()
        {
            ServiceSettings settings;

            Data = new Dictionary<ServiceSettingsState, ServiceSettings>();
            Data.Add(ServiceSettingsState.Default, new ServiceSettings());
            
            settings = new ServiceSettings();
            settings.Location = ArgumentConstants.Locations[Location.SouthCentralUS];
            settings.Slot = ArgumentConstants.Slots[Slot.Production];
            settings.StorageAccountName = "mystore";
            settings.Subscription = "TestSubscription2";
            Data.Add(ServiceSettingsState.Sample1, settings);

            settings = new ServiceSettings();
            settings.Location = ArgumentConstants.Locations[Location.SouthCentralUS];
            settings.Slot = ArgumentConstants.Slots[Slot.Production];
            settings.StorageAccountName = "mystore";
            settings.Subscription = "Does not exist subscription";
            Data.Add(ServiceSettingsState.DoesNotExistSubscription, settings);
        }

        private void InitializeGood()
        {
            Good = new List<ServiceSettings>();
        }

        private void InitializeBad()
        {
            Bad = new List<ServiceSettings>();
        }
    }

    enum ServiceSettingsState
    {
        Default,
        Sample1,
        DoesNotExistSubscription
    }
}
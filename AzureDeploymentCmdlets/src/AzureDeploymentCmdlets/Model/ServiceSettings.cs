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
using System.IO;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Utilities;
using System.Web.Script.Serialization;

namespace AzureDeploymentCmdlets.Model
{
    public class ServiceSettings
    {
        // Flag indicating whether the ServiceSettings have already been loaded
        // and should begin validating any properties (which is used to allow
        // the deserializer to set empty values without tripping validation)
        private bool _shouldValidate = false;
        
        public string Slot
        {
            get { return _slot; }
            set
            {
                if (_shouldValidate)
                {
                    Validate.ValidateStringIsNullOrEmpty(value, "Slot");
                    if (!ArgumentConstants.Slots.ContainsValue(value.ToLower()))
                    {
                        throw new ArgumentException(string.Format(Resources.InvalidServiceSettingElement, "Slot"));
                    }
                }
                
                _slot = value ?? string.Empty;
            }
        }
        private string _slot = null;

        public string Location
        {
            get { return _location; }
            set
            {
                if (_shouldValidate)
                {
                    Validate.ValidateStringIsNullOrEmpty(value, "Location");
                    if (!ArgumentConstants.Locations.ContainsValue(value.ToLower()))
                    {
                        throw new ArgumentException(string.Format(Resources.InvalidServiceSettingElement, "Location"));
                    }
                }

                _location = value ?? string.Empty;
            }
        }
        private string _location = null;

        public string Subscription
        {
            get { return _subscription; }
            set
            {
                if (_shouldValidate)
                {
                    Validate.ValidateStringIsNullOrEmpty(value, "Subscription");
                }
                _subscription = value ?? string.Empty;
            }
        }
        private string _subscription = null;

        public string StorageAccountName
        {
            get { return _storageAccountName; }
            set
            {
                if (_shouldValidate)
                {
                    Validate.ValidateStringIsNullOrEmpty(value, "StorageAccountName");
                }
                _storageAccountName = value ?? string.Empty;
            }
        }
        private string _storageAccountName = null;
        
        public ServiceSettings()
        {
            _slot = string.Empty;
            _location = string.Empty;
            _subscription = string.Empty;
            _storageAccountName = string.Empty;
        }
        
        public static ServiceSettings Load(string path)
        {
            Validate.ValidateFileFull(path, Resources.ServiceSettings);

            string text = File.ReadAllText(path);
            ServiceSettings settings = new JavaScriptSerializer().Deserialize<ServiceSettings>(text);
            settings._shouldValidate = true;
            
            return settings;
        }

        public static ServiceSettings LoadDefault(string path, string slot, string location, string subscription, string storageAccountName, string suppliedServiceName, string serviceDefinitionName, out string serviceName)
        {
            ServiceSettings local;
            ServiceSettings global = new GlobalComponents(GlobalPathInfo.GlobalSettingsDirectory).GlobalSettings;
            ServiceSettings defaultServiceSettings = new ServiceSettings();

            // Load local settings if user provided local settings path
            //
            if (string.IsNullOrEmpty(path))
            {
                local = new ServiceSettings();
            }
            else
            {
                Validate.ValidateFileFull(path, Resources.ServiceSettings);
                local = Load(path);
            }

            defaultServiceSettings._slot = GetDefaultSlot(local.Slot, global.Slot, slot);
            defaultServiceSettings._location = GetDefaultLocation(local.Location, global.Location, location);
            defaultServiceSettings._subscription = GetDefaultSubscription(local.Subscription, global.Subscription, subscription);
            serviceName = GetServiceName(suppliedServiceName, serviceDefinitionName);
            defaultServiceSettings._storageAccountName = GetDefaultStorageName(local.StorageAccountName, global.StorageAccountName, storageAccountName, serviceName).ToLower();

            return defaultServiceSettings;
        }

        private static string GetServiceName(string suppliedServiceName, string serviceDefinitionName)
        {
            // If user supplied value as parameter then return it
            //
            if (!string.IsNullOrEmpty(suppliedServiceName))
            {
                return suppliedServiceName;
            }
            else
            {
                // Check to see if you have service name from *csdef
                //
                if (string.IsNullOrEmpty(serviceDefinitionName))
                {
                    // This line will be touched only if the cmd running doesn't require service name
                    //
                    return string.Empty;
                }
                else
                {
                    return serviceDefinitionName;
                }
            }
        }

        private static string GetDefaultStorageName(string localStorageName, string globalStorageAccountName, string storageAccountName, string serviceName)
        {
            // If user supplied value as parameter then return it
            //
            if (!string.IsNullOrEmpty(storageAccountName))
            {
                return storageAccountName;
            }
            // User already has value in local service settings
            //
            else if (!string.IsNullOrEmpty(localStorageName))
            {
                return localStorageName;
            }
            // User already has value in global service settings
            //
            else if (!string.IsNullOrEmpty(globalStorageAccountName))
            {
                return globalStorageAccountName;
            }
            // If none of previous succeed, use service name as storage account name
            //
            else if (!String.IsNullOrEmpty(serviceName))
            {
                serviceName = serviceName.Replace("-", "x2d");
            }
            return serviceName;
        }

        private static string GetDefaultSubscription(string localSubscription, string globalSubscription, string subscription)
        {
            // If user supplied value as parameter then return it
            //
            if (!string.IsNullOrEmpty(subscription))
            {
                return subscription;
            }
            // User already has value in local service settings
            //
            else if (!string.IsNullOrEmpty(localSubscription))
            {
                return localSubscription;
            }
            // User already has value in global service settings
            //
            else if (!string.IsNullOrEmpty(globalSubscription))
            {
                return globalSubscription;
            }
            // If none of previous succeed, use first entry in *PublishSettings as subscription name
            //
            else
            {
                return new GlobalComponents(GlobalPathInfo.GlobalSettingsDirectory).PublishSettings.Items[0].Subscription[0].Name;
            }
        }

        private static string GetDefaultLocation(string localLocation, string globalLocation, string location)
        {
            // If user supplied value as parameter then return it
            //
            if (!string.IsNullOrEmpty(location))
            {
                if (ArgumentConstants.Locations.ContainsValue(location.ToLower()))
                {
                    return location.ToLower();
                }
                else
                {
                    throw new ArgumentException(string.Format(Resources.InvalidServiceSettingElement, "Location"));
                };
            }
            // User already has value in local service settings
            //
            else if (!string.IsNullOrEmpty(localLocation))
            {
                return localLocation.ToLower();
            }
            // User already has value in global service settings
            //
            else if (!string.IsNullOrEmpty(globalLocation))
            {
                return globalLocation.ToLower();
            }
            // If none of previous succeed, get random location from "North Central US" or "South Central US"
            //
            else
            {
                int randomLocation = General.GetRandomFromTwo((int)Model.Location.NorthCentralUS, (int)Model.Location.SouthCentralUS);
                return ArgumentConstants.Locations[(Model.Location)randomLocation];
            }
        }

        private static string GetDefaultSlot(string localSlot, string globalSlot, string slot)
        {
            // If user supplied value as parameter then return it
            //
            if (!string.IsNullOrEmpty(slot))
            {
                if (ArgumentConstants.Slots.ContainsValue(slot.ToLower()))
                {
                    return slot.ToLower();
                }
                else
                {
                    throw new ArgumentException(string.Format(Resources.InvalidServiceSettingElement, "Slot"));
                }
            }
            // User already has value in local service settings
            //
            else if (!string.IsNullOrEmpty(localSlot))
            {
                return localSlot.ToLower();
            }
            // User already has value in global service settings
            //
            else if (!string.IsNullOrEmpty(globalSlot))
            {
                return globalSlot.ToLower();
            }
            // If none of previous succeed, use Production as default slot
            //
            else
            {
                return ArgumentConstants.Slots[Model.Slot.Production];
            }
        }
        
        public void Save(string path)
        {
            Validate.ValidateStringIsNullOrEmpty(path, Resources.ServiceSettings);
            Validate.ValidateDirectoryFull(Path.GetDirectoryName(path), Resources.ServiceSettings);

            File.WriteAllText(path, new JavaScriptSerializer().Serialize(this));
        }
        
        public override bool Equals(object obj)
        {
            ServiceSettings other = (ServiceSettings)obj;

            return
                this.Location.Equals(other.Location) &&
                this.Slot.Equals(other.Slot) &&
                this.StorageAccountName.Equals(other.StorageAccountName) &&
                this.Subscription.Equals(other.Subscription);
        }
        
        public override int GetHashCode()
        {
            return
                Location.GetHashCode() ^
                Slot.GetHashCode() ^
                StorageAccountName.GetHashCode() ^
                Subscription.GetHashCode();
        }
    }
}
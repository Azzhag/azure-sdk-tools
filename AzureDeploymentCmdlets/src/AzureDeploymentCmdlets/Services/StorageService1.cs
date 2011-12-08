// ----------------------------------------------------------------------------------
// 
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

namespace AzureDeploymentCmdlets.WAPPSCmdlet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;

    /// <summary>
    /// This class represents a storage account.
    /// </summary>
    [DataContract(Namespace = Constants.ServiceManagementNS)]
    public class CreateStorageServiceInput : IExtensibleDataObject
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string ServiceName { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string Label { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string AffinityGroup { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false)]
        public string Location { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }

    /// <summary>
    /// This class represents a storage account label and description.
    /// </summary>
    [DataContract(Namespace = Constants.ServiceManagementNS)]
    public class UpdateStorageServiceInput : IExtensibleDataObject
    {
        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string Label { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }

    /// <summary>
    /// The storage service-related part of the API
    /// </summary>
    public partial interface IServiceManagement
    {
        /// <summary>
        /// Creates a new storage account.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "POST", UriTemplate = @"{subscriptionId}/services/storageservices")]
        IAsyncResult BeginCreateStorageAccount(string subscriptionId, CreateStorageServiceInput createStorageServiceInput, AsyncCallback callback, object state);
        void EndCreateStorageAccount(IAsyncResult asyncResult);

        /// <summary>
        /// Deletes the specified storage account from Windows Azure.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "DELETE", UriTemplate = @"{subscriptionId}/services/storageservices/{storageAccountName}")]
        IAsyncResult BeginDeleteStorageAccount(string subscriptionId, string storageAccountName, AsyncCallback callback, object state);
        void EndDeleteStorageAccount(IAsyncResult asyncResult);

        /// <summary>
        /// Updates the label and/or the description for a storage account in Windows Azure.
        /// </summary>
        [OperationContract(AsyncPattern = true)]
        [WebInvoke(Method = "PUT", UriTemplate = @"{subscriptionId}/services/storageservices/{storageAccountName}")]
        IAsyncResult BeginUpdateStorageAccount(string subscriptionId, string storageAccountName, UpdateStorageServiceInput updateStorageServiceInput, AsyncCallback callback, object state);
        void EndUpdateStorageAccount(IAsyncResult asyncResult);
    }

    public static partial class ServiceManagementExtensionMethods
    {
        public static void CreateStorageAccount(this IServiceManagement proxy, string subscriptionId, CreateStorageServiceInput createStorageServiceInput)
        {
            proxy.EndCreateStorageAccount(proxy.BeginCreateStorageAccount(subscriptionId, createStorageServiceInput, null, null));
        }

        public static void DeleteStorageAccount(this IServiceManagement proxy, string subscriptionId, string storageAccountName)
        {
            proxy.EndDeleteStorageAccount(proxy.BeginDeleteStorageAccount(subscriptionId, storageAccountName, null, null));
        }

        public static void UpdateStorageAccount(this IServiceManagement proxy, string subscriptionId, string storageAccountName, string label, string description)
        {
            var input = new UpdateStorageServiceInput()
            {
                Label = ServiceManagementHelper.EncodeToBase64String(label),
                Description = description
            };

            proxy.EndUpdateStorageAccount(proxy.BeginUpdateStorageAccount(subscriptionId, storageAccountName, input, null, null));
        }
    }
}
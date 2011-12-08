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
    using System.Data.Services.Client;
    using System.Globalization;
    using System.Management.Automation;
    using System.Xml.Linq;

    public static class CmdletExtensions
    {
        public static string ResolvePath(this PSCmdlet cmdlet, string path)
        {
            var result = cmdlet.SessionState.Path.GetResolvedPSPathFromPSPath(path);
            string fullPath = string.Empty;

            if (result != null && result.Count > 0)
            {
                fullPath = result[0].Path;
            }

            return fullPath;
        }

        public static void WriteVerbose(this PSCmdlet cmdlet, string format, params object[] args)
        {
            var text = string.Format(CultureInfo.InvariantCulture, format, args);
            cmdlet.WriteVerbose(text);
        }

        public static Exception ProcessExceptionDetails(this PSCmdlet cmdlet, Exception exception)
        {
            if ((exception is DataServiceQueryException) && (exception.InnerException != null))
            {
                var dscException = FindDataServiceClientException(exception.InnerException);

                if (dscException == null)
                {
                    return new InnerDataServiceException(exception.InnerException.Message);
                }
                else
                {
                    var message = dscException.Message;
                    try
                    {
                        XNamespace ns = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
                        XDocument doc = XDocument.Parse(message);
                        return new InnerDataServiceException(doc.Root.Element(ns + "message").Value);
                    }
                    catch
                    {
                        return new InnerDataServiceException(message);
                    }
                }
            }

            return exception;
        }

        private static Exception FindDataServiceClientException(Exception ex)
        {
            if (ex is DataServiceClientException)
            {
                return ex;
            }
            else if (ex.InnerException != null)
            {
                return FindDataServiceClientException(ex.InnerException);
            }

            return null;
        }
    }
}
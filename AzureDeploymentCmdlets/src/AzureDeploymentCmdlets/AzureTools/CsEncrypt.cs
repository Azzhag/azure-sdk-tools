// ----------------------------------------------------------------------------
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------

﻿using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Utilities;

namespace AzureDeploymentCmdlets.AzureTools
{
    public class CsEncrypt : AzureTool
    {
        private string _toolPath = null;

        public CsEncrypt()
        {
            _toolPath = Path.Combine(AzureSdkBinDirectory, Resources.CsEncryptExe);
        }
        
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public string CreateCertificate()
        {
            string standardOutput = null;
            string standardError = null;
            Execute(Resources.CsEncryptCreateCertificateArg, out standardOutput, out standardError);

            Match match = Regex.Match(standardOutput, @"^Thumbprint\s*:\s*(.*)$", RegexOptions.Multiline);
            if (!match.Success || match.Groups.Count <= 0)
            {
                throw new InvalidOperationException(string.Format(Resources.CsEncrypt_CreateCertificate_CreationFailed, standardError));
            }
            
            return match.Groups[1].Value;
        }
        
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private void Execute(string arguments, out string standardOutput, out string standardError)
        {
            ProcessStartInfo pi = new ProcessStartInfo(_toolPath, arguments);
            ProcessHelper.StartAndWaitForProcess(pi, out standardOutput, out standardError);
        }
    }
}

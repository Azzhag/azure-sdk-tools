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
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using AzureDeploymentCmdlets.Test.Model;
using AzureDeploymentCmdlets.Cmdlet;
using AzureDeploymentCmdlets.Model;
using AzureDeploymentCmdlets.Test.TestData;

namespace AzureDeploymentCmdlets.Test
{
    /// <summary>
    /// Utility used to create files and directories and clean them up when
    /// complete.
    /// </summary>
    public class FileSystemHelper : IDisposable
    {
        /// <summary>
        /// Gets the path to the root of the file system utilized by the test.
        /// </summary>
        public string RootPath { get; private set; }

        /// <summary>
        /// Gets or sets the path to a temporary Azure SDK which we use to
        /// store and cleanup after global files.
        /// </summary>
        public string AzureSdkPath { get; private set; }

        /// <summary>
        /// Gets a reference to the test class using the FileSystemHelper to
        /// provide access to its logging.
        /// </summary>
        public TestBase TestInstance { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private FileSystemWatcher _watcher = null;

        /// <summary>
        /// Gets or sets a value indicating whether to enable monitoring on
        /// the portion of the file system being managed by FileSystemHelper.
        /// </summary>
        public bool EnableMonitoring
        {
            get { return _watcher != null; }
            set
            {
               if (!value && _watcher != null)
                {
                    // Dispose of the watcher if we're turning it off
                    DisposeWatcher();
                }
                else if (value && _watcher == null)
                {
                    // Create a file system watcher
                    _watcher = new FileSystemWatcher();
                    _watcher.Path = RootPath;
                    _watcher.IncludeSubdirectories = true;
                    _watcher.Changed += (s, e) => Log("<Watcher>  Changed {0}", GetRelativePath(e.FullPath));
                    _watcher.Created += (s, e) => Log("<Watcher>  Created at {0}", GetRelativePath(e.FullPath));
                    _watcher.Deleted += (s, e) => Log("<Watcher>  Deleted at {0}", GetRelativePath(e.FullPath));
                    _watcher.Renamed += (s, e) => Log("<Watcher>  Renamed {0} to {1}", GetRelativePath(e.OldFullPath), GetRelativePath(e.FullPath));
                    _watcher.EnableRaisingEvents = true;
                }
                
            }
        }
        
        /// <summary>
        /// Initializes a new FileSystemHelper to a random temp directory.
        /// </summary>
        /// <param name="testInstance">
        /// Reference to the test class (to access its logging).
        /// </param>
        public FileSystemHelper(TestBase testInstance)
            : this(testInstance, GetTemporaryDirectoryName())
        {
        }
        
        /// <summary>
        /// Initialize a new FileSystemHelper to a specific directory.
        /// </summary>
        /// <param name="testInstance">
        /// Reference to the test class (to access its logging).
        /// </param>
        /// <param name="rootPath">The root directory.</param>
        public FileSystemHelper(TestBase testInstance, string rootPath)
        {
            Debug.Assert(testInstance != null);
            Debug.Assert(!string.IsNullOrEmpty(rootPath));
            
            TestInstance = testInstance;

            // Set the directory and create it if necessary.
            RootPath = rootPath;
            if (!Directory.Exists(rootPath))
            {
                Log("Creating directory {0}", rootPath);
                Directory.CreateDirectory(rootPath);
            }
        }
        
        /// <summary>
        /// Destroy the files and directories created during the test.
        /// </summary>
        public void Dispose()
        {
            if (RootPath != null)
            {
                // Cleanup any certificates added during the test
                if (!string.IsNullOrEmpty(AzureSdkPath))
                {
                    new RemoveAzurePublishSettingsCommand()
                        .RemovePublishSettingsProcess(AzureSdkPath);
                    GlobalPathInfo.GlobalSettingsDirectory = null;
                    AzureSdkPath = null;
                }

                Log("Deleting directory {0}", RootPath);
                Directory.Delete(RootPath, true);
                
                DisposeWatcher();

                // Note: We can't clear the RootPath until we've disposed the
                // watcher because its handlers will attempt to get relative
                // paths which will fail if there is no RootPath
                RootPath = null;
            }
        }
        
        /// <summary>
        /// Dispose of the FileSystemWatcher we're using to monitor changes to
        /// the FileSystem.
        /// </summary>
        private void DisposeWatcher()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }
        
        /// <summary>
        /// Log a message from the FileSytemHelper.
        /// </summary>
        /// <param name="format">Message format.</param>
        /// <param name="args">Message argument.</param>
        private void Log(string format, params object[] args)
        {
            TestInstance.Log("[FileSystemHelper]  " + format, args);
        }
        
        /// <summary>
        /// Get the path of a file relative to the FileSystemHelper's root.
        /// </summary>
        /// <param name="fullPath">The full path to the file.</param>
        /// <returns>The path relative to the FileSystemHelper's root.</returns>
        public string GetRelativePath(string fullPath)
        {
            Debug.Assert(!string.IsNullOrEmpty(fullPath));
            Debug.Assert(fullPath.StartsWith(RootPath, StringComparison.OrdinalIgnoreCase));
            return fullPath.Substring(RootPath.Length, fullPath.Length - RootPath.Length);
        }

        /// <summary>
        /// Get the full path to a file given a path relative to the
        /// FileSystemHelper's root.
        /// </summary>
        /// <param name="relativePath">Relative path.</param>
        /// <returns>Corresponding full path.</returns>
        public string GetFullPath(string relativePath)
        {
            Debug.Assert(!string.IsNullOrEmpty(relativePath));
            return Path.Combine(RootPath, relativePath);
        }
        
        /// <summary>
        /// Create a random directory name that doesn't yet exist on disk.
        /// </summary>
        /// <returns>A random, non-existant directory name.</returns>
        private static string GetTemporaryDirectoryName()
        {
            string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            while (Directory.Exists(path))
            {
                path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }
            
            return path;
        }
        
        /// <summary>
        /// Create a new directory relative to the root.
        /// </summary>
        /// <param name="relativePath">Relative path to the directory.</param>
        /// <returns>The full path to the directory.</returns>
        public string CreateDirectory(string relativePath)
        {
            Debug.Assert(!string.IsNullOrEmpty(relativePath));
            
            string path = Path.Combine(RootPath, relativePath);
            if (!Directory.Exists(path))
            {
                Log("Creating directory {0}", path);
                Directory.CreateDirectory(path);
            }
            
            return path;
        }
        
        /// <summary>
        /// Create an empty file relative to the root.
        /// </summary>
        /// <param name="relativePath">Relative path to the file.</param>
        /// <returns>The full path to the file.</returns>
        public string CreateEmptyFile(string relativePath)
        {
            Debug.Assert(!string.IsNullOrEmpty(relativePath));
            
            string path = Path.Combine(RootPath, relativePath);
            if (!File.Exists(path))
            {
                Log("Creating empty file {0}", path);
                File.WriteAllText(path, "");
            }
            
            return path;
        }

        /// <summary>
        /// Create a temporary Azure SDK directory to simulate global files.
        /// </summary>
        /// <returns>The path to the temporary Azure SDK directory.</returns>
        public string CreateAzureSdkDirectoryAndImportPublishSettings()
        {
            return CreateAzureSdkDirectoryAndImportPublishSettings(Data.ValidPublishSettings[0]);
        }

        /// <summary>
        /// Create a temporary Azure SDK directory to simulate global files.
        /// </summary>
        /// <param name="publishSettingsPath">
        /// Path to the publish settings.
        /// </param>
        /// <returns>The path to the temporary Azure SDK directory.</returns>
        public string CreateAzureSdkDirectoryAndImportPublishSettings(string publishSettingsPath)
        {
            Debug.Assert(!string.IsNullOrEmpty(publishSettingsPath));
            Debug.Assert(File.Exists(publishSettingsPath));
            Debug.Assert(string.IsNullOrEmpty(AzureSdkPath));
            
            AzureSdkPath = CreateDirectory("AzureSdk");
            GlobalPathInfo.GlobalSettingsDirectory = AzureSdkPath;
            new ImportAzurePublishSettingsCommand()
                .ImportAzurePublishSettingsProcess(publishSettingsPath, AzureSdkPath);
            GlobalPathInfo.GlobalSettingsDirectory = AzureSdkPath;

            return AzureSdkPath;
        }
    }
}

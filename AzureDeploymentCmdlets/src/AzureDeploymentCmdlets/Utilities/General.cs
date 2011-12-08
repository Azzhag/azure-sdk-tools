using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Xml.Serialization;
using AzureDeploymentCmdlets.Properties;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace AzureDeploymentCmdlets.Utilities
{
    internal static class General
    {
        private static Assembly _assembly = Assembly.GetExecutingAssembly();

        public static string GetNodeModulesPath()
        {
            return Path.Combine(GetAssemblyDirectory(), Resources.NodeModulesPath);
        }

        public static string GetAssemblyDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static string GetWithProgramFilesPath(string directoryName, bool throwIfNotFound)
        {
            string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            if (Directory.Exists(Path.Combine(programFilesPath, directoryName)))
            {
                return Path.Combine(programFilesPath, directoryName);
            }
            else
            {
                if (programFilesPath.IndexOf(Resources.x86InProgramFiles) == -1)
                {
                    programFilesPath += Resources.x86InProgramFiles;
                    if (throwIfNotFound)
                    {
                        Validate.ValidateDirectoryExists(Path.Combine(programFilesPath, directoryName));
                    }
                    return Path.Combine(programFilesPath, directoryName);
                }
                else
                {
                    programFilesPath = programFilesPath.Replace(Resources.x86InProgramFiles, string.Empty);
                    if (throwIfNotFound)
                    {
                        Validate.ValidateDirectoryExists(Path.Combine(programFilesPath, directoryName));
                    }
                    return Path.Combine(programFilesPath, directoryName);
                }
            }
        }

        public static T DeserializeXmlFile<T>(string fileName, string exceptionMessage = null)
        {
            Validate.ValidateFileFull(fileName, string.Format(Resources.PathDoesNotExistForElement, string.Empty, fileName));
            
            T item = default(T);
            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (Stream s = new FileStream(fileName, FileMode.Open))
            {
                try { item = (T)xmlSerializer.Deserialize(s); }
                catch
                {
                    if (!string.IsNullOrEmpty(exceptionMessage))
                    {
                        throw new InvalidOperationException(exceptionMessage);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
            return item;
        }

        public static void SerializeXmlFile<T>(T obj, string fileName)
        {
            Validate.ValidatePathName(fileName, string.Format(Resources.PathDoesNotExistForElement, string.Empty, fileName));
            Validate.ValidateStringIsNullOrEmpty(fileName, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (Stream stream = new FileStream(fileName, FileMode.Create))
            {
                xmlSerializer.Serialize(stream, obj);
            }
        }

        public static T DeserializeXmlStream<T>(Stream stream)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            T obj = (T)xmlSerializer.Deserialize(stream);
            stream.Close();

            return obj;
        }

        public static byte[] GetResourceContents(string resourceName)
        {
            Stream stream = _assembly.GetManifestResourceStream(resourceName);
            byte[] contents = new byte[stream.Length];
            stream.Read(contents, (int)stream.Position, (int)stream.Length);
            return contents;
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static void LaunchWebPage(string target)
        {
            ProcessHelper.Start(target);
        }

        public static void CreateDirectories(IEnumerable<string> directories)
        {
            foreach (string directoryName in directories)
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        public static void CopyFilesFromResources(IDictionary<string, string> filesPair)
        {
            foreach (KeyValuePair<string, string> filePair in filesPair)
            {
                File.WriteAllBytes(filePair.Value, General.GetResourceContents(filePair.Key));
            }
        }

        /// <summary>
        /// Write all of the given bytes to a file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="mode">Mode to open the file.</param>
        /// <param name="bytes">Contents of the file.</param>
        public static void WriteAllBytes(string path, FileMode mode, byte[] bytes)
        {
            Debug.Assert(!string.IsNullOrEmpty(path));
            Debug.Assert(Enum.IsDefined(typeof(FileMode), mode));
            Debug.Assert(bytes != null && bytes.Length > 0);
            
            // Note: We're not wrapping the file in a using statement because
            // that could lead to a double dispose when the writer is disposed.
            FileStream file = null;
            try
            {
                file = new FileStream(path, mode);
                using (BinaryWriter writer = new BinaryWriter(file))
                {
                    // Clear the reference to file so it won't get disposed twice
                    file = null;

                    writer.Write(bytes);
                }
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                }
            }
        }

        public static int GetRandomFromTwo(int first, int second)
        {
            return (new Random(DateTime.Now.Millisecond).Next(2) == 0) ? first : second;
        }

        public static string[] GetResourceNames(string resourcesFullFolderName)
        {
            return _assembly.GetManifestResourceNames().Where<string>(item => item.StartsWith(resourcesFullFolderName)).ToArray<string>();
        }

        public static TResult InvokeMethod<T, TResult>(string methodName, object instance, params object[] arguments)
        {
            MethodInfo info = typeof(T).GetMethod(methodName);
            return (TResult)info.Invoke(instance, arguments);
        }

        public static X509Certificate2 GetCertificateFromStore(string thumbprint)
        {
            Validate.ValidateStringIsNullOrEmpty(thumbprint, "certificate thumbprint");

            X509Store store = new X509Store(StoreName.My, System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

            if (certificates.Count == 0)
            {
                throw new ArgumentException(string.Format(Resources.CertificateNotFoundInStore, thumbprint));
            }
            else
            {
                return certificates[0];
            }
        }

        public static void AddCertificateToStore(X509Certificate2 certificate)
        {
            Validate.ValidateNullArgument(certificate, Resources.InvalidCertificate);
            X509Store store = new X509Store(StoreName.My, System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(certificate);
        }

        public static void RemoveCertificateFromStore(X509Certificate2 certificate)
        {
            Validate.ValidateNullArgument(certificate, Resources.InvalidCertificate);
            X509Store store = new X509Store(StoreName.My, System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Remove(certificate);
        }
    }
}
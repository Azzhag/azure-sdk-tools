using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AzureDeploymentCmdlets.Properties;
using AzureDeploymentCmdlets.Utilities;

namespace AzureDeploymentCmdlets.Scaffolding
{
    public delegate void ScaffoldRule(string path, Dictionary<string, object> parameters);

    public class Scaffold
    {
        public List<ScaffoldFile> Files { get; private set; }

        public Scaffold()
        {
            Files = new List<ScaffoldFile>();
        }

        public static void ReplaceParameterRule(string path, Dictionary<string, object> parameters)
        {
            string contents = File.ReadAllText(path);

            foreach (KeyValuePair<string, object> pair in parameters)
            {
                contents = contents.Replace(string.Format("${0}$", pair.Key), pair.Value.ToString());
            }

            File.WriteAllText(path, contents);
        }

        public static void GenerateScaffolding(string sourcePath, string destPath, Dictionary<string, object> parameters)
        {
            sourcePath = Path.GetFullPath(sourcePath);
            Scaffold scaffold = Parse(Path.Combine(sourcePath, Resources.ScaffoldXml));

            foreach (ScaffoldFile file in scaffold.Files)
            {
                string sourceDirectory = string.IsNullOrEmpty(file.SourceDirectory) ? sourcePath : Path.Combine(General.GetAssemblyDirectory(), file.SourceDirectory);

                foreach (string path in GetPaths(sourceDirectory, file))
                {
                    string tempPath;

                    if (string.IsNullOrEmpty(file.PathExpression))
                    {
                        tempPath = !string.IsNullOrEmpty(file.TargetPath) ? file.TargetPath : GetRelativePath(sourceDirectory, path);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(file.TargetPath))
                        {
                            tempPath = GetRelativePath(sourceDirectory, path);
                        }
                        else
                        {
                            tempPath = GetRelativePath(sourceDirectory, path);
                            string t1 = tempPath.Substring(tempPath.IndexOf('\\') + 1);
                            tempPath = Path.Combine(file.TargetPath, t1);
                        }
                    }

                    tempPath = Path.Combine(destPath, tempPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(tempPath));

                    File.Copy(path, tempPath);

                    foreach (ScaffoldRule rule in file.Rules)
                    {
                        rule(tempPath, parameters);
                    }

                    if (!file.Copy)
                    {
                        File.Delete(tempPath);
                    }
                }
            }
        }

        private static List<string> GetPaths(string sourcePath, ScaffoldFile file)
        {
            List<string> paths = new List<string>();
            if (!string.IsNullOrEmpty(file.PathExpression))
            {
                foreach (string fullPath in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
                {
                    string relativePath = GetRelativePath(sourcePath, fullPath);
                    if (Regex.IsMatch(relativePath, file.PathExpression))
                    {
                        paths.Add(fullPath);
                    }
                }
            }
            else
            {
                paths.Add(Path.Combine(sourcePath, file.Path));
            }

            return paths;
        }

        internal static string GetRelativePath(string sourcePath, string fullPath)
        {
            string relativePath = fullPath.Substring(sourcePath.Length, fullPath.Length - sourcePath.Length).TrimStart('\\');
            return relativePath;
        }

        internal static Scaffold Parse(string path)
        {
            Debug.Assert(File.Exists(path));
            XDocument document = XDocument.Load(path);
            Scaffold scaffold = new Scaffold();

            foreach (XElement fileElement in document.Root.Elements(XName.Get("ScaffoldFile")))
            {
                ScaffoldFile scaffoldFile = new ScaffoldFile();
                scaffoldFile.Path = GetAttribute(fileElement, "Path");
                scaffoldFile.Copy = bool.Parse(GetAttribute(fileElement, "Copy") ?? "true");
                scaffoldFile.TargetPath = GetAttribute(fileElement, "TargetPath");
                scaffoldFile.PathExpression = GetAttribute(fileElement, "PathExpression");
                scaffoldFile.SourceDirectory = GetAttribute(fileElement, "SourceDirectory");

                foreach (XElement ruleElement in fileElement.Elements())
                {
                    string[] names = ruleElement.Name.LocalName.Split('.');
                    string className = string.Format("{0}.{1}", typeof(Scaffold).Namespace, names[0]);
                    string methodName = names[1];
                    Type type = typeof(Scaffold).Assembly.GetType(className);
                    MethodInfo ruleInfo = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public);
                    ScaffoldRule rule = Delegate.CreateDelegate(typeof(ScaffoldRule), ruleInfo) as ScaffoldRule;
                    scaffoldFile.Rules.Add(rule);
                }

                scaffold.Files.Add(scaffoldFile);
            }

            return scaffold;
        }

        private static string GetAttribute(XElement fileElement, string attributeName)
        {
            string value = null;
            XAttribute attribute = fileElement.Attribute(XName.Get(attributeName));
            if (attribute != null)
            {
                value = attribute.Value;
            }

            return value;
        }
    }
}
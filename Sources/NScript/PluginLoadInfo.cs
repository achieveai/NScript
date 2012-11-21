//-----------------------------------------------------------------------
// <copyright file="PluginLoadInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using NScript.Converter;
    using NScript.Utils;

    /// <summary>
    /// Definition for PluginLoadInfo
    /// </summary>
    public class PluginLoadInfo
    {
        /// <summary>
        /// Backing field for AssemblyName
        /// </summary>
        private string assemblyName;

        /// <summary>
        /// Backing field for ClassName
        /// </summary>
        private string className;

        /// <summary>
        /// Backing field for String
        /// </summary>
        private IList<Tuple<string, string>> pluginArgs;

        /// <summary>
        /// Prevents a default instance of the <see cref="PluginLoadInfo"/> class from being created.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="pluginArgs">The plugin args.</param>
        private PluginLoadInfo(
            string assemblyName,
            string className,
            IList<Tuple<string, string>> pluginArgs)
        {
            this.assemblyName = assemblyName;
            this.className = className;
            this.pluginArgs = pluginArgs;
        }

        /// <summary>
        /// Gets the name of the assembly.
        /// </summary>
        /// <value>
        /// The name of the assembly.
        /// </value>
        public string AssemblyName
        { get { return this.assemblyName; } }

        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        /// <value>
        /// The name of the class.
        /// </value>
        public string ClassName
        { get { return this.className; } }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        public IList<Tuple<string, string>> Arguments
        { get { return this.pluginArgs; } }

        /// <summary>
        /// Loads the plugin infos.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static List<PluginLoadInfo> LoadPluginInfos(
            string fileName,
            List<string> hintPaths)
        {
            List<PluginLoadInfo> rv = new List<PluginLoadInfo>();
            XDocument xmlDoc = XDocument.Load(fileName);

            foreach (var pluginNode in xmlDoc.Descendants("Plugin"))
            {
                IXmlLineInfo xmlLineInfo = pluginNode;
                List<Tuple<string, string>> args = new List<Tuple<string, string>>();
                string pluginAssembly = PluginLoadInfo.GetValue(pluginNode, "Assembly");
                string pluginClassName = PluginLoadInfo.GetValue(pluginNode, "ClassName");

                if (pluginAssembly == null
                    || pluginClassName == null)
                {
                    Logger.Instance.LogError(
                        new ErrorInfo(
                            fileName,
                            xmlLineInfo.LineNumber,
                            xmlLineInfo.LinePosition,
                            "",
                            "Assembly name or class name not defined for plugin"));

                    continue;
                }

                var tmpFileName = pluginAssembly;
                if (!tmpFileName.ToLowerInvariant().EndsWith(".dll"))
                {
                    tmpFileName += ".dll";
                }

                tmpFileName = ParseOptions.FindFile(tmpFileName, hintPaths);
                if (tmpFileName == null)
                {
                    Logger.Instance.LogError(
                        new ErrorInfo(
                            fileName,
                            xmlLineInfo.LineNumber,
                            xmlLineInfo.LinePosition,
                            "",
                            string.Format(
                                "Assembly name: \"{0}\" not found",
                                pluginAssembly)));
                }

                pluginAssembly = tmpFileName;
                foreach (var arg in pluginNode.Elements("Argument"))
                {
                    IXmlLineInfo argLineInfo = arg;
                    string argName = PluginLoadInfo.GetValue(arg, "Name");
                    string argValue = PluginLoadInfo.GetValue(arg, "Value");

                    if (string.IsNullOrEmpty(argName))
                    {
                        Logger.Instance.LogError(
                            new ErrorInfo(
                                fileName,
                                argLineInfo.LineNumber,
                                argLineInfo.LinePosition,
                                "",
                                "Argument name not defined."));

                        continue;
                    }

                    args.Add(Tuple.Create(argName, argValue));
                }

                rv.Add(
                    new PluginLoadInfo(
                        pluginAssembly,
                        pluginClassName,
                        args));
            }

            return rv;
        }

        /// <summary>
        /// Gets the plugin.
        /// </summary>
        /// <returns>Plugin</returns>
        public IConverterPlugin GetPlugin()
        {
            Assembly pluginAssembly = Assembly.LoadFrom(this.AssemblyName);
            Type className = pluginAssembly.GetType(this.ClassName, true, true);

            IConverterPlugin converterPlugin = (IConverterPlugin)Activator.CreateInstance(className);
            converterPlugin.ParseArgs(this.Arguments);

            return converterPlugin;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <returns>Value for given Key.</returns>
        private static string GetValue(
            XElement element,
            string keyName)
        {
            XElement elem = element.Element(keyName);
            if (elem != null)
            { return elem.Value; }

            XAttribute attr = element.Attribute(keyName);
            if (attr != null)
            { return attr.Value; }

            return null;
        }
    }
}
//-----------------------------------------------------------------------
// <copyright file="ParseOptions.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Lib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NScript.Utils;

    /// <summary>
    /// Definition for ParseOptions
    /// </summary>
    public class ParseOptions
    {
        /// <summary>
        /// Backing field for ReferenceDlls.
        /// </summary>
        private List<string> referenceDlls = new List<string>();

        /// <summary>
        /// Backing field for reference path.
        /// </summary>
        private List<string> referencePath = new List<string>();

        /// <summary>
        /// Backing field for plugin hint path.
        /// </summary>
        private List<string> pluginHintPath = new List<string>();

        /// <summary>
        /// Backing field for JsFileName
        /// </summary>
        private string jsFileName;

        /// <summary>
        /// Backing field for MainFileName.
        /// </summary>
        private string entryAssembly;

        /// <summary>
        /// Backing field for plugin config fileName.
        /// </summary>
        private string pluginConfigFileName;

        /// <summary>
        /// Number of Javascripts to create.
        /// </summary>
        private int jsParts = 1;

        /// <summary>
        /// Is Script created for release (fully minified)?
        /// </summary>
        private bool minify = false;

        private bool uglify = false;

        /// <summary>
        /// Prevents a default instance of the <see cref="ParseOptions"/> class from being created.
        /// </summary>
        private ParseOptions()
        {
        }

        /// <summary>
        /// Gets or sets the name of the js file.
        /// </summary>
        /// <value>
        /// The name of the js file.
        /// </value>
        public string JsFileName => this.jsFileName;

        /// <summary>
        /// Gets or sets the name of the main file.
        /// </summary>
        /// <value>
        /// The name of the main file.
        /// </value>
        public string EntryAssembly => this.entryAssembly;

        /// <summary>
        /// Gets the reference DLLS.
        /// </summary>
        public List<string> ReferenceDlls => this.referenceDlls;

        /// <summary>
        /// Gets the reference path.
        /// </summary>
        public List<string> ReferencePath => this.referencePath;

        /// <summary>
        /// Gets the name of the plugin config file.
        /// </summary>
        /// <value>
        /// The name of the plugin config file.
        /// </value>
        public string PluginConfigFileName => this.pluginConfigFileName;

        /// <summary>
        /// Gets the plugin hint paths.
        /// </summary>
        public List<string> PluginHintPaths => this.pluginHintPath;

        /// <summary>
        /// Number of js files to create.
        /// </summary>
        public int JsParts => this.jsParts;

        /// <summary>
        /// Is script ready for relase (i.e. fully minified).
        /// </summary>
        public bool Minify => this.minify;

        public bool Uglify => this.uglify;

        /// <summary>
        /// Parses the args.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>ParseOptions if parsing was successful, else null.</returns>
        public static ParseOptions ParseArgs(string[] args)
        {
            ParseOptions options = new ParseOptions();
            if (args.Length <= 5)
            {
                return null;
            }

            CurrentOption option = CurrentOption.None;
            string referenceDllsOption = null;

            for (int iArg = 0; iArg < args.Length; iArg++)
            {
                switch (args[iArg].ToLowerInvariant())
                {
                    case "-outjs":
                        option = CurrentOption.None;
                        if (options.JsFileName == null)
                        {
                            if (++iArg < args.Length)
                            {
                                options.jsFileName = args[iArg];
                            }
                        }
                        else
                        {
                            Logger.Instance.LogError("Output js file specified at least twice");
                            return null;
                        }
                        continue;
                    case "-entryassembly":
                        option = CurrentOption.None;
                        if (options.EntryAssembly == null)
                        {
                            if (++iArg < args.Length)
                            {
                                options.entryAssembly = args[iArg];
                            }
                        }
                        else
                        {
                            Logger.Instance.LogError("Input entry assembly file specified at least twice");
                            return null;
                        }
                        continue;
                    case "-pluginconfig":
                        option = CurrentOption.None;
                        if (options.pluginConfigFileName == null)
                        {
                            if (++iArg < args.Length)
                            {
                                options.pluginConfigFileName = args[iArg];
                            }
                        }
                        else
                        {
                            Logger.Instance.LogError("Input plugin configuration file specified at least twice");
                            return null;
                        }
                        continue;
                    case "-pluginhintpath":
                        option = CurrentOption.PluginHintPaths;
                        continue;
                    case "-references":
                        option = CurrentOption.References;
                        continue;
                    case "-referencehintpath":
                        option = CurrentOption.ReferenceHintPaths;
                        continue;
                    case "-jsparts":
                        option = CurrentOption.JsParts;
                        continue;
                    case "-minify":
                        options.minify = true;
                        continue;
                    case "-uglify":
                        options.uglify = true;
                        continue;
                    default:
                        break;
                }

                if (option == CurrentOption.None)
                {
                    Logger.Instance.LogError("Unknown option: " + args[iArg]);
                    return null;
                }

                switch (option)
                {
                    case CurrentOption.References:
                        referenceDllsOption = args[iArg];
                        break;
                    // case CurrentOption.ReferenceHintPaths:
                    case CurrentOption.PluginHintPaths:
                        {
                            var paths = args[iArg].Trim();
                            List<ErrorInfo> fileErrors = options.GetFiles(
                                paths,
                                option == CurrentOption.ReferenceHintPaths
                                    ? options.referencePath
                                    : options.pluginHintPath,
                                true);

                            if (fileErrors.Count > 0)
                            {
                                foreach (var fileError in fileErrors)
                                {
                                    Logger.Instance.LogError(fileError);
                                }
                                return null;
                            }
                            break;
                        }
                    case CurrentOption.JsParts:
                        if (!int.TryParse(args[iArg], out options.jsParts)
                            || options.jsParts > 10 || options.jsParts <= 0)
                        {
                            Logger.Instance.LogError("JsParts not int or invalid");
                            return null;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(referenceDllsOption))
            {
                string[] referenceDlls = referenceDllsOption.Split(';', ',');
                for (int iReferenceDll = 0; iReferenceDll < referenceDlls.Length; iReferenceDll++)
                {
                    string referenceDll = referenceDlls[iReferenceDll].Trim();

                    if (string.IsNullOrEmpty(referenceDll)) continue;

                    referenceDll = ParseOptions.FindFile(
                        referenceDll,
                        options.referenceDlls);

                    if (referenceDll == null)
                    {
                        Logger.Instance.LogError(
                            new ErrorInfo(
                                referenceDll,
                                0,
                                0,
                                "Cs2Js001",
                                "file not found"));
                    }
                    else
                    {
                        options.referenceDlls.Add(referenceDll);
                    }
                }
            }

            if (string.IsNullOrEmpty(options.JsFileName) || options.JsFileName.Trim().Length == 0)
            {
                Logger.Instance.LogError("Output JS file not specified");
            }

            if (string.IsNullOrEmpty(options.EntryAssembly) || options.EntryAssembly.Trim().Length == 0)
            {
                Logger.Instance.LogError("Input Main Assembly file not specified");
            }

            if (!string.IsNullOrEmpty(Path.GetDirectoryName(options.JsFileName.Trim())) &&
                !Directory.Exists(Path.GetDirectoryName(options.JsFileName)))
            {
                Logger.Instance.LogError(String.Format(
                    "Directory for JS file not found ({0})",
                    Path.GetDirectoryName(options.JsFileName)));
            }

            if (options.ReferenceDlls.Count == 0)
            {
                Logger.Instance.LogError(
                    "Reference dlls not specified");
            }

            return Logger.Instance.HasErrors ? null : options;
        }

        /// <summary>
        /// Prints the usage.
        /// </summary>
        public static void PrintUsage()
        {
            Console.WriteLine("NScript -outJs <JSFileName> -references <references (dll paths)... > -entryAssembly <assembly with entrypoint> [-pluginConfig <plugin for JsGenerator>] [-pluginHintPath <; seperated directories to find plugin dlls in>] [-referenceHintPath <;seperated directories to find reference dlls in>]");
            Environment.Exit(1);
        }

        /// <summary>
        /// Finds the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="hintDirectories">The hint directories.</param>
        /// <returns>fileName if the file was found, else null.</returns>
        public static string FindFile(string fileName, List<string> hintDirectories)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            if (File.Exists(fileName))
            {
                return fileName;
            }

            if (hintDirectories != null)
            {
                for (int iDirectory = 0; iDirectory < hintDirectories.Count; iDirectory++)
                {
                    string fullFileName = Path.Combine(hintDirectories[iDirectory], fileName);
                    if (File.Exists(fullFileName))
                    {
                        return fullFileName;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="fileNameOrWildcard">The file name or wildcard.</param>
        /// <param name="listToAddTo">The list to add to.</param>
        /// <returns>List of errors encountered.</returns>
        private List<ErrorInfo> GetFiles(
            string fileNameOrWildcard,
            List<string> listToAddTo,
            bool isDirectory)
        {
            List<ErrorInfo> fileErrors = new List<ErrorInfo>();

            string[] fileSplits = fileNameOrWildcard.Split(
                new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(path =>
                {
                    if (path.EndsWith("\\") || path.EndsWith("/"))
                    {
                        path = path.Substring(0, path.Length - 1);
                    }

                    return path;
                })
                .ToArray();

            string fileName;

            string[] files;

            foreach (var singleFileName in fileSplits)
            {
                var dirPath = Path.GetDirectoryName(singleFileName);

                if (dirPath == string.Empty)
                {
                    dirPath = "./";
                }

                // If the folder doesn't exist, ignore it
                if (Directory.Exists(dirPath))
                {
                    fileName = Path.GetFileName(singleFileName);
                    files = isDirectory
                        ? Directory.GetDirectories(dirPath, fileName)
                        : Directory.GetFiles(dirPath, fileName);

                    if (files.Length == 0)
                    {
                        // ignore wildcards
                        if (!fileName.Contains("*"))
                        {
                            fileErrors.Add(
                                new ErrorInfo(
                                    singleFileName,
                                    0,
                                    0,
                                    "Cs2Js001",
                                    isDirectory
                                        ? "directory not found"
                                        : "file not found"));
                        }
                    }
                    else
                    {
                        for (int iFile = 0; iFile < files.Length; ++iFile)
                        {
                            listToAddTo.Add(files[iFile]);
                        }
                    }
                }
            }

            return fileErrors;
        }

        /// <summary>
        /// Option enum.
        /// </summary>
        private enum CurrentOption
        {
            None,
            OutJS,
            References,
            MainAssembly,
            ReferenceHintPaths,
            PluginConf,
            PluginHintPaths,
            JsParts,
        }
    }
}
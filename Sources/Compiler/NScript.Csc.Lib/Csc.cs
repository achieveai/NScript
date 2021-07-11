//-----------------------------------------------------------------------
// <copyright file="Csc.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Csc.Lib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Emit;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class CscCompiler
    {
        public static int Main(string[] args)
        {
            var loader = new NScriptAnalyzerAssemblyLoader();
            return DesktopBuildClient.Run(
                args,
                RequestLanguage.CSharpCompile,
                Csc.Run,
                loader);
        }
    }


    /// <summary>
    /// Definition for Csc
    /// </summary>
    internal sealed class Csc : CSharpCompiler
    {
        private readonly List<string> rawArguments;

        internal Csc(
            string responseFile,
            BuildPaths buildPaths,
            string[] args,
            IAnalyzerAssemblyLoader analyzerLoader)
            : base(
                CSharpCommandLineParser.Default,
                responseFile,
                args,
                buildPaths,
                Environment.GetEnvironmentVariable("LIB"),
                analyzerLoader)
        {
            this.rawArguments =
                args.SelectMany(arg =>
                {
                    if (arg.StartsWith("@"))
                    {
                        var respFile = arg.Substring(1);
                        if (respFile[0] == '"')
                        { respFile = respFile.Replace("\"", ""); }

                        if (File.Exists(respFile))
                        {
                            return File.ReadAllText(respFile)
                                .Replace(" /", "\r/")
                                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        }
                    }

                    return new[] { arg };
                })
                .ToList();
        }

        internal static int Run(
            string[] args,
            BuildPaths buildPaths,
            TextWriter textWriter,
            IAnalyzerAssemblyLoader analyzerLoader)
        {
            FatalError.Handler = FailFast.OnFatalException;

            var responseFile = Path.Combine(
                buildPaths.ClientDirectory,
                CSharpCompiler.ResponseFileName);

            var compiler = new Csc(
                responseFile,
                buildPaths,
                args,
                analyzerLoader);

            if (compiler.Arguments.OutputFileName.Equals(
                "mscorlib.dll",
                StringComparison.InvariantCultureIgnoreCase))
            {
                compiler.Arguments.EmitOptions = compiler
                    .Arguments
                    .EmitOptions
                    .WithRuntimeMetadataVersion("v4.100.191");
            }

            return ConsoleUtil.RunWithUtf8Output(
                compiler.Arguments.Utf8Output,
                textWriter,
                tw => compiler.Run(tw));
        }

        protected override void OnBeforeCompilation(Compilation compilation)
        {
            var (resources, tmp) = SerializationHelper.InjectIntoCompilation((CSharpCompilation)compilation);

            this.Arguments.ManifestResources =
                this
                    .GetResourceFilePaths()
                    .Concat(resources)
                    .AsImmutable();

        }

        private IEnumerable<ResourceDescription> GetResourceFilePaths()
        {
            const string htmlString = ".html",
                cssString = ".css",
                xhtmlString = ".xhtml";

            var jObject = new JObject();
            foreach (var res in this.rawArguments)
            {
                if (!res.StartsWith("-resource:")
                    && !res.StartsWith("/resource:"))
                { continue; }

                var resource = res.Substring("-resource:".Length);
                var resourceParts = resource.Split(',');

                if (resourceParts[0][0] == '"')
                { resourceParts[0] = resourceParts[0].Replace("\"", ""); }

                if (resourceParts.Length != 2
                    || !File.Exists(resourceParts[0]))
                { continue; }

                var fileName = Path.GetFullPath(resourceParts[0]);
                jObject.Add(resourceParts[1], fileName);
            }

            var resourceInfoStream = new MemoryStream();
            var txtWriter = new StreamWriter(resourceInfoStream);
            var writer = new JsonTextWriter(txtWriter);
            jObject.WriteTo(writer);
            writer.Flush();
            txtWriter.Flush();
            resourceInfoStream.Position = 0;

            var rv = this.Arguments.ManifestResources
                .Select(_ =>
                {
                    var fileName = _.FileName;
                    if (string.IsNullOrEmpty(fileName)
                        || (!string.Equals(
                                Path.GetExtension(fileName),
                                htmlString,
                                StringComparison.InvariantCultureIgnoreCase)
                            && !string.Equals(
                                Path.GetExtension(fileName),
                                cssString,
                                StringComparison.InvariantCultureIgnoreCase)
                            && !string.Equals(
                                Path.GetExtension(fileName),
                                xhtmlString,
                                StringComparison.InvariantCultureIgnoreCase)))
                    {
                        return _;
                    }

                    return new ResourceDescription(
                        _.ResourceName,
                        null,
                        () => _.DataProvider(),
                        _.IsPublic,
                        false,
                        false);
                })
                .Concat(new[]
                {
                    new ResourceDescription(
                        "$$ResInfo$$",
                        () => resourceInfoStream,
                        false)
                });

            return rv;
        }
    }
}

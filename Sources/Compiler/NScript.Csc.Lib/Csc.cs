//-----------------------------------------------------------------------
// <copyright file="Csc.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Csc.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.ErrorReporting;
    using Microsoft.CodeAnalysis.Emit;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NScript.Csc.Lib.Service;
    using NScript.Utils;

    public static class CscCompiler
    {
        public static int Main(string[] args)
        {
            // Strip NScript-specific flags (--log, --run-id) out of args before Roslyn
            // sees them; Roslyn's CommandLineParser would reject unknown switches.
            // Supports both space-delimited ("--log path") and colon-delimited ("--log:path") forms,
            // matching csc conventions. Also honors NSCRIPT_LOG_PATH / NSCRIPT_LOG_RUNID env vars.
            var strippedArgs = ExtractNScriptFlags(args, out var logPath, out var runId);

            // /service (manual runs) or NSCRIPT_SERVICE=1 (MSBuild, through CscEnvironment)
            // forwards the compile to the build service; NSS001 + local compile otherwise.
            bool watch = ServiceArgs.IsWatchRequestedByEnvironment();
            bool useService = ServiceArgs.TryStripServiceFlag(strippedArgs, out strippedArgs)
                | ServiceArgs.IsServiceRequestedByEnvironment()
                | watch;

            CompilerLog.Initialize(logPath, "csc", runId);

            try
            {
                if (useService)
                {
                    int? serviceExitCode = ServiceClient.TryRun(ServiceProtocol.KindCompile, strippedArgs, "csc", watch);
                    if (serviceExitCode.HasValue)
                    {
                        return serviceExitCode.Value;
                    }
                }

                return RunLocal(strippedArgs);
            }
            finally
            {
                CompilerLog.Shutdown();
            }
        }

        /// <summary>
        /// Compiles inside the build-service daemon. Unlike <see cref="Main"/> it takes the
        /// working directory explicitly, writes diagnostics only to <paramref name="textWriter"/>
        /// (never switching the console encoding; the client does that on replay), and reuses
        /// one analyzer loader for the life of the process. The caller sets the process cwd
        /// to <paramref name="workingDir"/> (the @rsp and -resource: paths are cwd-relative).
        /// </summary>
        /// <param name="utf8Output">Whether the args asked for /utf8output.</param>
        public static int RunInProcess(string[] args, string workingDir, TextWriter textWriter, out bool utf8Output)
            => RunInProcess(args, workingDir, textWriter, captureInputs: false, out utf8Output, out _);

        /// <summary>
        /// <see cref="RunInProcess(string[], string, TextWriter, out bool)"/> that also reports
        /// what the compile read and wrote (watch mode), with input hashes taken before the
        /// compile ran. <paramref name="inputs"/> is null when not asked for or when the
        /// arguments did not parse.
        /// </summary>
        public static int RunInProcess(string[] args, string workingDir, TextWriter textWriter, bool captureInputs, out bool utf8Output, out CompileInputs inputs)
        {
            var buildPaths = new BuildPaths(
                clientDir: AppContext.BaseDirectory,
                workingDir: workingDir,
                sdkDir: BuildClient.GetSystemSdkDirectory(),
                tempDir: BuildServerConnection.GetTempPath(workingDir));

            bool utf8 = false;
            CompileInputs captured = null;
            var client = new DesktopBuildClient(
                RequestLanguage.CSharpCompile,
                (a, paths, writer, loader) => Csc.RunWithWriter(a, paths, writer, loader, captureInputs, out utf8, out captured),
                SharedAnalyzerLoader.Value);

            int exitCode = client.RunCompilation(
                args.Where(arg => !arg.TrimStart().StartsWith("/analyzerconfig:")),
                buildPaths,
                textWriter).ExitCode;
            utf8Output = utf8;
            inputs = captured;
            return exitCode;
        }

        /// <summary>
        /// The local compile, kept out of <see cref="Main"/> so a service client never JITs
        /// Roslyn-dependent code.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int RunLocal(string[] args)
        {
            var loader = new NScriptAnalyzerAssemblyLoader();
            return DesktopBuildClient.Run(
                args,
                RequestLanguage.CSharpCompile,
                Csc.Run,
                loader);
        }

        /// <summary>
        /// One analyzer loader per daemon, as VBCSCompiler does: a loader per request would
        /// load every analyzer assembly again into a new load context.
        /// </summary>
        private static readonly Lazy<IAnalyzerAssemblyLoader> SharedAnalyzerLoader =
            new Lazy<IAnalyzerAssemblyLoader>(() => new NScriptAnalyzerAssemblyLoader());

        /// <summary>
        /// Removes NScript-only switches (<c>--log</c>, <c>--run-id</c>, and their
        /// short aliases) from the argv and returns the path and run id through out
        /// parameters. Supports both <c>--log path</c> and <c>--log:path</c> forms.
        /// Public for testability — downstream callers should not rely on it.
        /// </summary>
        public static string[] ExtractNScriptFlags(string[] args, out string logPath, out string runId)
        {
            logPath = null;
            runId = null;
            if (args == null)
            {
                return Array.Empty<string>();
            }

            var filtered = new List<string>(args.Length);
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (TryConsumeFlag(args, ref i, arg, "--log", "-log", "/log", out var pathValue))
                {
                    logPath = pathValue;
                    continue;
                }

                if (TryConsumeFlag(args, ref i, arg, "--run-id", "-runid", "/runid", out var runIdValue))
                {
                    runId = runIdValue;
                    continue;
                }

                filtered.Add(arg);
            }

            return filtered.ToArray();
        }

        private static bool TryConsumeFlag(
            string[] args,
            ref int index,
            string current,
            string longForm,
            string shortForm,
            string slashForm,
            out string value)
        {
            value = null;
            if (string.IsNullOrEmpty(current))
            {
                return false;
            }

            // Space-delimited: "--log" <path>
            if (string.Equals(current, longForm, StringComparison.OrdinalIgnoreCase)
                || string.Equals(current, shortForm, StringComparison.OrdinalIgnoreCase)
                || string.Equals(current, slashForm, StringComparison.OrdinalIgnoreCase))
            {
                if (index + 1 < args.Length)
                {
                    value = args[index + 1];
                    index++; // consume the value
                }

                return true;
            }

            // Colon- or equals-delimited: "--log:path", "--log=path"
            if (TryParseInline(current, longForm, out value)
                || TryParseInline(current, shortForm, out value)
                || TryParseInline(current, slashForm, out value))
            {
                return true;
            }

            return false;
        }

        private static bool TryParseInline(string current, string flagName, out string value)
        {
            value = null;
            if (current.Length <= flagName.Length
                || !current.StartsWith(flagName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var delimiter = current[flagName.Length];
            if (delimiter != ':' && delimiter != '=')
            {
                return false;
            }

            value = current.Substring(flagName.Length + 1);
            return true;
        }
    }

    /// <summary>
    /// Definition for Csc
    /// </summary>
    internal sealed class Csc : CSharpCompiler
    {
        private readonly List<string> rawArguments;
        private ImmutableArray<ResourceDescription> manifestResources;
        private long createCompilationMs;

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
            FatalError.SetHandlers(FailFast.Handler, nonFatalHandler: null);

            var responseFile = Path.Combine(
                buildPaths.ClientDirectory,
                CSharpCompiler.ResponseFileName);

            var compiler = new Csc(
                responseFile,
                buildPaths,
                args,
                analyzerLoader);

            return ConsoleUtil.RunWithUtf8Output(
                compiler.Arguments.Utf8Output,
                textWriter,
                tw => compiler.Run(tw));
        }

        /// <summary>
        /// <see cref="Run"/> for the build service: writes straight to
        /// <paramref name="textWriter"/> and reports /utf8output instead of applying it.
        /// </summary>
        internal static int RunWithWriter(
            string[] args,
            BuildPaths buildPaths,
            TextWriter textWriter,
            IAnalyzerAssemblyLoader analyzerLoader,
            bool captureInputs,
            out bool utf8Output,
            out CompileInputs inputs)
        {
            FatalError.SetHandlers(FailFast.Handler, nonFatalHandler: null);

            var responseFile = Path.Combine(
                buildPaths.ClientDirectory,
                CSharpCompiler.ResponseFileName);

            var compiler = new Csc(
                responseFile,
                buildPaths,
                args,
                analyzerLoader);

            utf8Output = compiler.Arguments.Utf8Output;
            inputs = captureInputs && !compiler.Arguments.Errors.Any(d => d.Severity == DiagnosticSeverity.Error)
                ? CompileInputs.FromArguments(compiler.Arguments, buildPaths.WorkingDirectory)
                : null;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            int exitCode = compiler.Run(textWriter);
            CompilerLog.ForComponent("Csc").Information(
                "CscRun completed in {ElapsedMs}ms CreateCompilationMs={CreateCompilationMs} ExitCode={ExitCode}",
                sw.ElapsedMilliseconds,
                compiler.createCompilationMs,
                exitCode);
            return exitCode;
        }

        /// <summary>
        /// Times compilation creation (parse + reference resolution) so the service can
        /// tell how much of a warm stage-1 run a live compilation would save.
        /// </summary>
        public override Compilation CreateCompilation(
            TextWriter consoleOutput,
            TouchedFileLogger touchedFilesLogger,
            ErrorLogger errorLogger,
            ImmutableArray<AnalyzerConfigOptionsResult> analyzerConfigOptions,
            AnalyzerConfigOptionsResult globalConfigOptions)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var compilation = base.CreateCompilation(consoleOutput, touchedFilesLogger, errorLogger, analyzerConfigOptions, globalConfigOptions);
            this.createCompilationMs = sw.ElapsedMilliseconds;
            return compilation;
        }

        protected override void OnBeforeCompilation(Compilation compilation)
        {
            var (resources, tmp) = SerializationHelper.InjectIntoCompilation((CSharpCompilation)compilation);

            manifestResources =
                this
                    .GetResourceFilePaths()
                    .Concat(resources)
                    .AsImmutable();
        }

        protected override ImmutableArray<ResourceDescription> GetManifestResources(Compilation compilation)
            => manifestResources.IsDefault
                ? base.GetManifestResources(compilation)
                : manifestResources;

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

//-----------------------------------------------------------------------
// <copyright file="BuildClient.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Csc.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;

    internal struct RunCompilationResult
    {
        internal static readonly RunCompilationResult Succeeded = new RunCompilationResult(CommonCompiler.Succeeded);

        internal static readonly RunCompilationResult Failed = new RunCompilationResult(CommonCompiler.Failed);

        internal int ExitCode { get; }

        internal bool RanOnServer { get; }

        internal RunCompilationResult(int exitCode, bool ranOnServer = false)
        {
            ExitCode = exitCode;
            RanOnServer = ranOnServer;
        }
    }

    /// <summary>
    /// Client class that handles communication to the server.
    /// </summary>
    internal abstract class BuildClient
    {
        protected static bool IsRunningOnWindows => Path.DirectorySeparatorChar == '\\';

        /// <summary>
        /// Returns the directory that contains mscorlib, or null when running on CoreCLR.
        /// </summary>
        public static string GetSystemSdkDirectory()
        {
            return RuntimeEnvironment.GetRuntimeDirectory();
        }

        /// <summary>
        /// Run a compilation through the compiler server and print the output
        /// to the console. If the compiler server fails, run the fallback
        /// compiler.
        /// </summary>
        internal RunCompilationResult RunCompilation(
            IEnumerable<string> originalArguments,
            BuildPaths buildPaths,
            TextWriter textWriter = null)
        {
            textWriter = textWriter ?? Console.Out;

            var args = originalArguments
                .Select(arg => arg.Trim())
                .ToArray();

            List<string> parsedArgs;
            bool hasShared;
            string keepAliveOpt;
            string sessionKeyOpt;
            string errorMessageOpt;
            if (!CommandLineParser.TryParseClientArgs(
                    args,
                    out parsedArgs,
                    out hasShared,
                    out keepAliveOpt,
                    out sessionKeyOpt,
                    out errorMessageOpt))
            {
                textWriter.WriteLine(errorMessageOpt);
                return RunCompilationResult.Failed;
            }

            if (hasShared)
            {
                sessionKeyOpt = sessionKeyOpt ?? GetSessionKey(buildPaths);
                var libDirectory = Environment.GetEnvironmentVariable("LIB");
                var serverResult = RunServerCompilation(textWriter, parsedArgs, buildPaths, libDirectory, sessionKeyOpt, keepAliveOpt);
                if (serverResult.HasValue)
                {
                    Debug.Assert(serverResult.Value.RanOnServer);
                    return serverResult.Value;
                }
            }

            // It's okay, and expected, for the server compilation to fail.  In that case just fall 
            // back to normal compilation. 
            var exitCode = RunLocalCompilation(parsedArgs.ToArray(), buildPaths, textWriter);
            return new RunCompilationResult(exitCode);
        }

        private static bool TryEnableMulticoreJitting(out string errorMessage)
        {
            errorMessage = null;
            try
            {
                // Enable multi-core JITing
                // https://blogs.msdn.microsoft.com/dotnet/2012/10/18/an-easy-solution-for-improving-app-launch-performance/
                var profileRoot = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RoslynCompiler",
                    "ProfileOptimization");
                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                var profileName = assemblyName.Name + assemblyName.Version + ".profile";
                Directory.CreateDirectory(profileRoot);
            }
            catch (Exception e)
            {
                errorMessage = "Exception enabling multicore JIT: " + e.Message;
                return false;
            }

            return true;
        }

        public Task<RunCompilationResult> RunCompilationAsync(
            IEnumerable<string> originalArguments,
            BuildPaths buildPaths,
            TextWriter textWriter = null)
        {
            var tcs = new TaskCompletionSource<RunCompilationResult>();
            ThreadStart action = () =>
            {
                try
                {
                    var result = RunCompilation(originalArguments, buildPaths, textWriter);
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            };

            var thread = new Thread(action);
            thread.Start();

            return tcs.Task;
        }

        protected abstract int RunLocalCompilation(
            string[] arguments,
            BuildPaths buildPaths,
            TextWriter textWriter);

        /// <summary>
        /// Runs the provided compilation on the server.  If the compilation cannot be completed on the server then null
        /// will be returned.
        /// </summary>
        internal RunCompilationResult? RunServerCompilation(
            TextWriter textWriter,
            List<string> arguments,
            BuildPaths buildPaths,
            string libDirectory,
            string sessionName,
            string keepAlive)
        {
            BuildResponse buildResponse;

            try
            {
                var buildResponseTask = RunServerCompilation(
                    arguments,
                    buildPaths,
                    sessionName,
                    keepAlive,
                    libDirectory,
                    CancellationToken.None);
                buildResponse = buildResponseTask.Result;

                Debug.Assert(buildResponse != null);
                if (buildResponse == null)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

            switch (buildResponse.Type)
            {
                case BuildResponse.ResponseType.Completed:
                    {
                        var completedResponse = (CompletedBuildResponse)buildResponse;
                        return ConsoleUtil.RunWithUtf8Output(
                            completedResponse.Utf8Output,
                            textWriter,
                            tw =>
                            {
                                tw.Write(completedResponse.Output);
                                return new RunCompilationResult(completedResponse.ReturnCode, ranOnServer: true);
                            });
                    }

                case BuildResponse.ResponseType.MismatchedVersion:
                case BuildResponse.ResponseType.IncorrectHash:
                case BuildResponse.ResponseType.Rejected:
                case BuildResponse.ResponseType.AnalyzerInconsistency:
                    // Build could not be completed on the server.
                    return null;
                default:
                    // Will not happen with our server but hypothetically could be sent by a rogue server.  Should
                    // not let that block compilation.
                    Debug.Assert(false);
                    return null;
            }
        }

        protected abstract Task<BuildResponse> RunServerCompilation(List<string> arguments, BuildPaths buildPaths, string sessionName, string keepAlive, string libDirectory, CancellationToken cancellationToken);

        protected abstract string GetSessionKey(BuildPaths buildPaths);

        /// <summary>
        /// Returns the compiler arguments to forward to Roslyn.
        ///
        /// Historically this re-parsed Windows' native command line via
        /// <c>GetCommandLine()</c> + <c>Skip(1)</c> to preserve quoting of forms
        /// like <c>/reference:"a,b"</c> from interactive shells (.NET's runtime
        /// arg parser strips outer quotes). That trick assumed argv[0] was the
        /// compiler itself (self-contained <c>csc.exe</c> apphost).
        ///
        /// Under the consolidated <c>Cs2Jsc</c> dotnet-tool hosting model, the
        /// native command line is <c>dotnet.exe Cs2Jsc.dll csc /noconfig @rsp</c>
        /// — <c>Skip(1)</c> leaves <c>Cs2Jsc.dll</c> and the <c>csc</c>
        /// subcommand selector in the argv that gets handed to Roslyn, which
        /// then treats both as positional source files and fails with CS2015
        /// / CS2001 on every build.
        ///
        /// Since callers (<see cref="CscCompiler.Main"/> via
        /// <c>Cs2Jsc/Program.Main</c>) already produce a clean argv with the
        /// selector stripped, and the MSBuild invocation path threads the
        /// real arguments through a response file (whose quoting is preserved
        /// by Roslyn's own response-file parser), we just trust the caller.
        /// The lost behavior — preserving <c>/reference:"a,b"</c> from a
        /// hand-typed terminal invocation — was already broken under dotnet
        /// hosting and is vanishingly rare under MSBuild-driven builds.
        /// </summary>
        protected static IEnumerable<string> GetCommandLineArgs(IEnumerable<string> args) => args;
    }
}

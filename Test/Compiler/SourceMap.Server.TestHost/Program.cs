// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace OwaSourceMapper.Server.TestHost
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.Extensions.DependencyInjection;
    using OwaSourceMapper.Server;

    /// <summary>
    /// End-to-end test host for the Playwright browser suite. Two subcommands:
    /// <list type="bullet">
    ///   <item>
    ///     <term><c>emit &lt;workDir&gt;</c></term>
    ///     <description>Produces a compiler-shaped <c>{mapName}.map</c> + companion
    ///       <c>{mapName}.js</c> under <c>{workDir}/maps</c>, and mirrors the
    ///       bundled <c>Fixtures</c> directory into <c>{workDir}/src</c> so the
    ///       map's <c>sourcesLong</c> entries resolve to real bytes.</description>
    ///   </item>
    ///   <item>
    ///     <term><c>serve &lt;workDir&gt;</c></term>
    ///     <description>Starts Kestrel on an ephemeral port, wires
    ///       <c>/sourcemap</c> to <see cref="SourceMapFileHandler"/>, serves the
    ///       <c>{workDir}/maps</c> directory as static content (for the
    ///       <c>.js</c> + <c>.map</c> pair), and prints <c>LISTENING
    ///       http://127.0.0.1:{port}</c> on stdout so the node harness can parse
    ///       the URL and drive Playwright against it.</description>
    ///   </item>
    /// </list>
    /// The two subcommands are deliberately decoupled so the node test can
    /// emit fixtures up front (cheap, deterministic) and only pay the Kestrel
    /// startup cost once per scenario group.
    /// </summary>
    public static class Program
    {
        private const string DefaultMapName = "app";
        private const string PathPrefix = "/sourcemap";

        public static int Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.Error.WriteLine("Usage: SourceMap.Server.TestHost (emit|serve) <workDir>");
                return 2;
            }

            string cmd = args[0];
            try
            {
                return cmd switch
                {
                    "emit" => RunEmit(args),
                    "serve" => RunServe(args),
                    _ => InvalidUsage(cmd),
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("TestHost error: " + ex.Message);
                Console.Error.WriteLine(ex.StackTrace);
                return 1;
            }
        }

        private static int InvalidUsage(string cmd)
        {
            Console.Error.WriteLine("Unknown subcommand: " + cmd);
            Console.Error.WriteLine("Expected: emit | serve");
            return 2;
        }

        private static int RunEmit(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("emit requires <workDir>");
                return 2;
            }

            string workDir = Path.GetFullPath(args[1]);
            string mapsDir = Path.Combine(workDir, "maps");
            string srcDir = Path.Combine(workDir, "src");
            Directory.CreateDirectory(mapsDir);
            Directory.CreateDirectory(srcDir);

            string fixtureRoot = Path.Combine(AppContext.BaseDirectory, "Fixtures");
            var fixtures = new List<string>();
            foreach (var rel in new[] { "Program.cs", "View.xwml", "Skin.cshtml" })
            {
                string src = Path.Combine(fixtureRoot, rel);
                if (!File.Exists(src))
                {
                    Console.Error.WriteLine("Missing fixture: " + src);
                    return 3;
                }

                string dst = Path.Combine(srcDir, rel);
                File.Copy(src, dst, overwrite: true);
                fixtures.Add(dst);
            }

            EmitMap(DefaultMapName, mapsDir, fixtures, sourceRoot: PathPrefix + "/" + DefaultMapName);
            Console.WriteLine("EMITTED " + Path.Combine(mapsDir, DefaultMapName + ".map"));
            return 0;
        }

        private static int RunServe(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("serve requires <workDir>");
                return 2;
            }

            string workDir = Path.GetFullPath(args[1]);
            string mapsDir = Path.Combine(workDir, "maps");
            string srcDir = Path.Combine(workDir, "src");

            if (!Directory.Exists(mapsDir))
            {
                Console.Error.WriteLine("maps dir missing; run `emit` first: " + mapsDir);
                return 3;
            }

            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ContentRootPath = workDir,
                Args = args,
            });
            // Use an ephemeral port so concurrent test runs cannot collide.
            builder.WebHost.UseUrls("http://127.0.0.1:0");

            var app = builder.Build();

            // Serve the companion .js + .map straight off disk as static files
            // so Chromium can load them with `fetch` semantics identical to a
            // production deploy. Fallback mimes are set for .js/.map.
            app.UseDefaultFiles();
            app.UseStaticFiles(new Microsoft.AspNetCore.Builder.StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(mapsDir),
                RequestPath = "/maps",
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream",
            });

            var handlerOptions = new SourceMapFileHandlerOptions
            {
                MapsDirectory = mapsDir,
                AllowedSourceRoots = new List<string> { srcDir },
            };
            app.MapSourceMapFiles(PathPrefix, handlerOptions);

            // Minimal HTML host page — loads /maps/{mapName}.js which, thanks to
            // the //# sourceMappingURL comment, triggers a /maps/{mapName}.map
            // fetch, and then a /sourcemap/{mapName}/{shortName} fetch when the
            // browser pulls a source.
            app.MapGet("/fixture.html", () => Microsoft.AspNetCore.Http.Results.Content(
                "<!doctype html><html><head><title>fixture</title></head>"
                + "<body><script src=\"/maps/" + DefaultMapName + ".js\"></script></body></html>",
                "text/html; charset=utf-8"));

            app.StartAsync().GetAwaiter().GetResult();

            var server = app.Services.GetRequiredService<IServer>();
            var addresses = server.Features.Get<IServerAddressesFeature>();
            foreach (var addr in addresses.Addresses)
            {
                Console.WriteLine("LISTENING " + addr);
            }

            // Block until parent signals exit via stdin close (node harness
            // invariant) — clean shutdown on Ctrl+C / SIGTERM is handled by
            // ASP.NET Core's lifetime.
            Console.In.ReadToEnd();
            app.StopAsync().GetAwaiter().GetResult();
            return 0;
        }

        private static void EmitMap(
            string mapName,
            string outputDir,
            IList<string> fixtureFiles,
            string sourceRoot)
        {
            var map = new OwaSourceMapper.SourceMap
            {
                File = mapName + ".js",
                EmitLegacyAshxHandler = false,
                SourceRoot = sourceRoot,
            };

            for (int i = 0; i < fixtureFiles.Count; i++)
            {
                map.AddMapping(
                    sLine: 0,
                    sCol: i * 8,
                    tLine: 0,
                    tCol: 0,
                    file: Path.GetFullPath(fixtureFiles[i]));
            }

            map.Write(outputDir);

            string jsPath = Path.Combine(outputDir, mapName + ".js");
            File.WriteAllText(jsPath,
                "// SourceMap.Server.TestHost fixture\n"
                + "(function fixtureEntry(){ return 1; })();\n"
                + "//# sourceMappingURL=" + mapName + ".map\n");
        }
    }
}

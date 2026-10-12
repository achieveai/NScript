//-----------------------------------------------------------------------
// <copyright file="ProbeXwmlInitTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    /// <summary>
    /// Probe.XwmlInit (M3 slice 2): every build logs one event, from GetPostJavascript, with
    /// that build's template counts. Nothing is cached across builds, so each template
    /// document read is a miss. Logging does not change the generated code.
    /// </summary>
    [TestClass]
    public class ProbeXwmlInitTests
    {
        private const string Template = "Sunlight.Framework.UI.Test.Templates.TestTemplate1.html";

        private string logPath;

        [TestInitialize]
        public void Setup()
        {
            Helper.Initialize();
            CompilerLog.Shutdown();
            this.logPath = Path.Combine(Path.GetTempPath(), "nscript-probe-xwml-" + Guid.NewGuid().ToString("N") + ".jsonl");
        }

        [TestCleanup]
        public void Cleanup()
        {
            CompilerLog.Shutdown();
            if (File.Exists(this.logPath))
            {
                File.Delete(this.logPath);
            }
        }

        /// <summary>Runs the plugin's build steps for one template and returns the generated code.</summary>
        private static string Build()
        {
            var plugin = Helper.CreatePlugin(null);
            Assert.IsNotNull(plugin.CodeGenerator.GetTemplateGetterIdentifier(Template));
            plugin.GetMethodsToEmitPass1();
            return Helper.ConvertCodeToString(plugin.GetPostJavascript());
        }

        [TestMethod]
        public void Probe_LogsOneEventPerBuild_WithThatBuildsCounts_AndKeepsTheCode()
        {
            var unlogged = Build();

            CompilerLog.Initialize(this.logPath, "test");
            var first = Build();
            var second = Build();
            CompilerLog.Shutdown();

            var events = File.ReadAllLines(this.logPath)
                .Select(line => JsonDocument.Parse(line).RootElement)
                .Where(e => e.GetProperty("@mt").GetString().StartsWith("Probe.XwmlInit ", StringComparison.Ordinal))
                .ToList();
            Assert.AreEqual(2, events.Count, "One Probe.XwmlInit per build.");
            foreach (var e in events)
            {
                int Field(string name) => e.GetProperty(name).GetInt32();
                Assert.AreEqual(1, Field("Templates"), "Documents read by this build only.");
                Assert.AreEqual(1, Field("TemplatesParsed"));
                Assert.AreEqual(0, Field("Hits"));
                Assert.AreEqual(Field("Templates"), Field("Misses"));
                Assert.IsTrue(Field("StyleSheets") >= 0);
                Assert.IsTrue(Field("TotalMs") >= Field("InitMs") + Field("OverwriteMs") + Field("ParseMs") + Field("GenMs"), "TotalMs covers every phase.");
            }

            Assert.AreEqual(unlogged, first, "Logging the probe must not change the generated code.");
            Assert.AreEqual(unlogged, second);
        }
    }
}

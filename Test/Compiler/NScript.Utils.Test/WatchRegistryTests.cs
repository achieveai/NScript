namespace NScript.Utils.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NScript.Csc.Lib.Service;
    using NScript.Lib.Service;

    /// <summary>
    /// Planner contracts of watch mode. The registry's only IO is the injected probe and
    /// stamp reader, so every test here runs on fake paths, fake hashes and fake MVIDs.
    /// </summary>
    [TestClass]
    public class WatchRegistryTests
    {
        private static readonly string Root = Path.Combine(Path.GetTempPath(), "nscript-watch-fake");

        private readonly Dictionary<string, string> disk = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly HashSet<string> locked = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, AssemblyStamp> stamps = new Dictionary<string, AssemblyStamp>(StringComparer.OrdinalIgnoreCase);

        private WatchRegistry registry;

        [TestInitialize]
        public void Setup()
            => this.registry = new WatchRegistry(
                path => this.locked.Contains(path)
                    ? FileProbe.Locked
                    : this.disk.TryGetValue(path, out var hash) ? FileProbe.Present(hash) : FileProbe.Missing,
                path => this.stamps.TryGetValue(path, out var stamp) ? stamp : (AssemblyStamp?)null);

        private static string P(params string[] parts) => Path.Combine(Root, Path.Combine(parts));

        private static string Out(string project) => P(project, "obj", project + ".dll");

        private static string Src(string project, string file = "A.cs") => P(project, file);

        private static AssemblyStamp Stamp(int n) => new AssemblyStamp(new Guid(n, 0, 0, new byte[8]), new DateTime(2026, 1, 1).AddSeconds(n));

        private CompileInputs Inputs(string project, IEnumerable<string> sources, IEnumerable<string> resources = null, IEnumerable<string> references = null)
        {
            var sourceList = sources.ToList();
            var resourceList = (resources ?? Array.Empty<string>()).ToList();
            var hashes = sourceList.Concat(resourceList).Where(this.disk.ContainsKey).ToDictionary(f => f, f => this.disk[f], StringComparer.OrdinalIgnoreCase);
            return new CompileInputs(Out(project), null, null, sourceList, resourceList, (references ?? Array.Empty<string>()).ToList(), hashes);
        }

        /// <summary>Registers a compile whose sources exist on the fake disk with hash "h0".</summary>
        private ProjectRecord Register(string project, string[] references = null, string[] resources = null, string[] buildFiles = null, string[] otherFiles = null)
        {
            var sources = new[] { Src(project) };
            foreach (var file in sources.Concat(resources ?? Array.Empty<string>()).Concat(buildFiles ?? Array.Empty<string>()).Concat(otherFiles ?? Array.Empty<string>()))
            {
                this.disk.TryAdd(file, "h0");
            }

            return this.registry.RegisterCompile(
                P(project),
                new[] { "@" + project + ".rsp" },
                this.Inputs(project, sources, resources, references),
                0,
                null,
                sources.Concat(otherFiles ?? Array.Empty<string>()),
                (buildFiles ?? Array.Empty<string>()).ToDictionary(f => f, f => this.disk[f], StringComparer.OrdinalIgnoreCase));
        }

        private WatchPlan Change(params string[] paths)
        {
            this.registry.Apply(this.registry.Classify(paths));
            return this.registry.Plan();
        }

        private void Compiled(ProjectRecord project, int exitCode = 0)
            => this.registry.RecordCompileAttempt(
                project.Key,
                this.Inputs(project.Name.Replace(".dll", string.Empty), project.Inputs.Sources, project.Inputs.Resources, project.Inputs.References),
                exitCode);

        /// <summary>
        /// Contract 1: a C# change compiles the owner and its transitive dependents in
        /// dependency order (not registration order), and the bundle of the changed project
        /// is emitted first.
        /// </summary>
        [TestMethod]
        public void Plan_CsChange_CompilesDependentsInOrder_OwnerBundleFirst()
        {
            var c = this.Register("C", references: new[] { Out("B") });
            var b = this.Register("B", references: new[] { Out("A") });
            var a = this.Register("A");
            this.registry.RegisterBundle(P("web"), new[] { "-outJs" }, P("web", "Other.js"), Out("B"), Array.Empty<string>());
            this.registry.RegisterBundle(P("web"), new[] { "-outJs" }, P("web", "A.js"), Out("A"), Array.Empty<string>());

            this.disk[Src("A")] = "h1";
            var plan = this.Change(Src("A"));

            CollectionAssert.AreEqual(new[] { a.Key, b.Key, c.Key }, plan.Compiles.Select(p => p.Key).ToArray());
            CollectionAssert.AreEqual(new[] { P("web", "A.js"), P("web", "Other.js") }, plan.Bundles.Select(x => x.Key).ToArray());
        }

        /// <summary>
        /// P7: a framework save (no bundle's own project changed). The most recently registered
        /// bundle goes first, and each bundle is emitted as soon as the compiles it reads are
        /// done, not after every compile of the batch; a compile no bundle reads goes last.
        /// </summary>
        [TestMethod]
        public void Schedule_FrameworkChange_LatestBundleFirst_EachEmittedOnceItsCompilesAreDone()
        {
            var z = this.Register("Z", references: new[] { Out("F") });
            var f = this.Register("F");
            var x = this.Register("X", references: new[] { Out("F") });
            var u = this.Register("U", references: new[] { Out("F") });
            var t = this.Register("T", references: new[] { Out("U"), Out("F") });
            this.registry.RegisterBundle(P("web"), new[] { "-outJs" }, P("web", "X.js"), Out("X"), new[] { Out("F") });
            this.registry.RegisterBundle(P("web"), new[] { "-outJs" }, P("web", "T.js"), Out("T"), new[] { Out("U"), Out("F") });

            this.disk[Src("F")] = "h1";
            var steps = this.registry.Schedule(this.Change(Src("F")));

            CollectionAssert.AreEqual(
                new[] { f.Key, u.Key, t.Key, P("web", "T.js"), x.Key, P("web", "X.js"), z.Key },
                steps.Select(s => s.Compile?.Key ?? s.Emit.Key).ToArray());
        }

        /// <summary>Contract 2: a resource-only change (a skin) recompiles only its owner.</summary>
        [TestMethod]
        public void Plan_ResourceOnlyChange_CompilesOnlyOwner()
        {
            var skin = P("A", "Shell.skin.cshtml");
            var a = this.Register("A", resources: new[] { skin });
            this.Register("B", references: new[] { Out("A") });

            this.disk[skin] = "h1";
            var plan = this.Change(skin);

            CollectionAssert.AreEqual(new[] { a.Key }, plan.Compiles.Select(p => p.Key).ToArray());
        }

        /// <summary>
        /// Contract 3: a reference with the owner's file name and MVID is MSBuild's copy. The
        /// edge survives edits that change both MVIDs, makes the copy's reader a dependent,
        /// and a pending copy blocks that reader until it is cleared.
        /// </summary>
        [TestMethod]
        public void CopyEdges_FreshMvidMatch_SurvivesEdits_PendingBlocksReader()
        {
            var copy = P("B", "bin", "A.dll");
            var a = this.Register("A");
            var b = this.Register("B", references: new[] { copy });
            this.stamps[Out("A")] = Stamp(1);
            this.stamps[copy] = Stamp(1);

            this.registry.RefreshCopyEdges();
            var edge = this.registry.CopiesOf(a.Key).Single();
            Assert.AreEqual(copy, edge.Target);

            for (int edit = 2; edit <= 3; edit++)
            {
                // The obj DLL was rebuilt (new MVID) and the copy is one edit behind.
                this.stamps[Out("A")] = Stamp(edit);
                this.stamps[copy] = Stamp(edit - 1);
                this.registry.RefreshCopyEdges();
                this.disk[Src("A")] = "h" + edit;
                var plan = this.Change(Src("A"));

                Assert.AreEqual(1, this.registry.CopyEdges.Count);
                CollectionAssert.AreEqual(new[] { a.Key, b.Key }, plan.Compiles.Select(p => p.Key).ToArray());
                this.Compiled(a);
            }

            this.registry.MarkCopyPending(edge);
            StringAssert.Contains(this.registry.CompileBlockReason(b), "copy pending");
            this.registry.ClearCopyPending(edge);
            Assert.IsNull(this.registry.CompileBlockReason(b));
        }

        /// <summary>
        /// Contract 4: a failed compile is red: it blocks dependents and bundles until its
        /// obj DLL changes (somebody built it), not before.
        /// </summary>
        [TestMethod]
        public void Red_BlocksDependentsAndBundles_ClearsWhenObjChanges()
        {
            var a = this.Register("A");
            var b = this.Register("B", references: new[] { Out("A") });
            var bundle = this.registry.RegisterBundle(P("web"), new[] { "-outJs" }, P("web", "B.js"), Out("B"), new[] { Out("A") });
            this.stamps[Out("A")] = Stamp(1);
            this.disk[Src("A")] = "h1";
            this.Change(Src("A"));

            this.Compiled(a, exitCode: 1);

            StringAssert.Contains(this.registry.CompileBlockReason(b), "red");
            StringAssert.Contains(this.registry.BundleBlockReason(bundle), "red");
            Assert.AreEqual(0, this.registry.RefreshRed().Count, "Same obj stamp: still red.");

            this.stamps[Out("A")] = Stamp(2);
            CollectionAssert.AreEqual(new[] { a.Key }, this.registry.RefreshRed().ToArray());
            Assert.AreEqual(0, this.registry.Red.Count);
        }

        /// <summary>
        /// Contract 6: every compile attempt refreshes the recorded hashes, so H0 -> H1 -> H0
        /// is two compiles ending at H0, also when the H1 compile failed. A project blocked
        /// behind a red dependency stays dirty.
        /// </summary>
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        public void HashRefresh_EditAndRevert_TwoCompilesEndingAtH0(int h1ExitCode)
        {
            var a = this.Register("A");
            var b = this.Register("B", references: new[] { Out("A") });

            this.disk[Src("A")] = "h1";
            Assert.AreEqual(a.Key, this.Change(Src("A")).Compiles.First().Key);
            this.Compiled(a, h1ExitCode);
            Assert.AreEqual("h1", a.Hashes[Src("A")]);
            if (h1ExitCode != 0)
            {
                Assert.IsNotNull(this.registry.CompileBlockReason(b));
                CollectionAssert.Contains(this.registry.Dirty.ToArray(), b.Key, "A blocked project stays dirty.");
            }

            this.disk[Src("A")] = "h0";
            Assert.AreEqual(a.Key, this.Change(Src("A")).Compiles.First().Key);
            this.Compiled(a);
            Assert.AreEqual("h0", a.Hashes[Src("A")]);
        }

        /// <summary>
        /// Contract 7: what a file event means after the debounce. Outputs under obj/bin, dot
        /// files, files that existed but are no input, and same-content saves change nothing.
        /// </summary>
        [TestMethod]
        public void Classify_IgnoredAndUnchangedPaths_AreDropped()
        {
            var notInput = P("A", "Docs", "notes.html");
            this.Register("A", otherFiles: new[] { notInput });
            this.disk[P("A", ".A.cs.swp")] = "x";
            this.disk[P("A", "obj", "Gen.cs")] = "x";
            this.disk[P("A", "bin", "A.dll")] = "x";

            var changes = this.registry.Classify(new[] { Src("A"), notInput, P("A", ".A.cs.swp"), P("A", "obj", "Gen.cs"), P("A", "bin", "A.dll") });

            Assert.AreEqual(0, changes.Changed.Count);
            Assert.AreEqual(0, changes.NeedsBuild.Count);
            Assert.AreEqual(5, changes.Dropped.Count);
            CollectionAssert.AreEqual(new[] { notInput }, changes.NotInputs);
        }

        /// <summary>
        /// Contract 7: changes MSBuild must see first: the project file, an imported props
        /// file, a new source file and a deleted input each need a dotnet build.
        /// </summary>
        [TestMethod]
        public void Classify_BuildFilesNewAndDeletedSources_NeedBuild()
        {
            var csproj = P("A", "A.csproj");
            var props = P("Directory.Build.props");
            this.Register("A", buildFiles: new[] { csproj, props });
            this.Register("B");

            this.disk[csproj] = "h1";
            this.disk[props] = "h1";
            this.disk[P("B", "New.cs")] = "h0";
            this.disk.Remove(Src("B"));
            var changes = this.registry.Classify(new[] { csproj, props, P("B", "New.cs"), Src("B") });

            var reasons = changes.NeedsBuild.Select(n => n.Reason).ToList();
            Assert.AreEqual(4, reasons.Count, string.Join("; ", reasons));
            Assert.IsTrue(reasons.Any(r => r.StartsWith("new file", StringComparison.Ordinal)), string.Join("; ", reasons));
            Assert.IsTrue(reasons.Any(r => r.StartsWith("deleted", StringComparison.Ordinal)), string.Join("; ", reasons));
            Assert.AreEqual(0, changes.Changed.Count);
        }

        /// <summary>
        /// Contract 7 with ruling C1: a file counts as deleted only if it is still missing
        /// after the debounce. Visual Studio (write temp, rename original away, rename temp
        /// in) and vim (rename original to A.cs~, write new, probe file 4913) saves are one
        /// change and no NeedsBuild; a locked file waits for the next window.
        /// </summary>
        [TestMethod]
        public void Classify_EditorSaveSequences_AreOneChange_LockedIsPending()
        {
            var a = this.Register("A");
            this.disk[Src("A")] = "h1";

            var vs = this.registry.Classify(new[] { P("A", "A.cs~RF1a2b3c.TMP"), Src("A"), P("A", "ve-4F2A.tmp") });
            var vim = this.registry.Classify(new[] { P("A", "A.cs~"), P("A", "4913"), Src("A"), P("A", ".A.cs.swp") });

            foreach (var changes in new[] { vs, vim })
            {
                CollectionAssert.AreEqual(new[] { Src("A") }, changes.Changed);
                Assert.AreEqual(0, changes.NeedsBuild.Count);
                Assert.AreEqual(0, changes.Pending.Count);
            }

            this.locked.Add(Src("A"));
            CollectionAssert.AreEqual(new[] { Src("A") }, this.registry.Classify(new[] { Src("A") }).Pending);
            Assert.AreEqual(a.Key, this.registry.Projects.Single().Key);
        }

        /// <summary>
        /// Regression (M5 D2): an emit that failed (the bundle was held open by a reader) was
        /// marked done, so the edit stayed lost until the next save. A failed bundle stays
        /// dirty; only a successful emit clears it.
        /// </summary>
        [TestMethod]
        public void RecordBundleRun_Failed_KeepsBundleInNextPlan()
        {
            var project = this.Register("App");
            var bundle = this.registry.RegisterBundle(P("App"), new[] { "-outJs" }, P("web", "App.js"), project.Key, Array.Empty<string>());

            this.disk[Src("App")] = "h1";
            Assert.AreEqual(1, this.Change(Src("App")).Bundles.Count);
            this.Compiled(project);

            this.registry.RecordBundleRun(bundle.Key, succeeded: false);
            var next = this.registry.Plan();
            Assert.AreEqual(0, next.Compiles.Count);
            Assert.AreEqual(bundle.Key, next.Bundles[0].Key);

            this.registry.RecordBundleRun(bundle.Key, succeeded: true);
            Assert.AreEqual(0, this.registry.Plan().Bundles.Count);
        }

        /// <summary>
        /// Regression (M5 D3): a compile that failed only because a reference DLL was locked
        /// (a plain build rewriting lib\Debug) marked the project red, and a same-content save
        /// could not recover it. It now stays dirty, not red, and is in the next plan.
        /// </summary>
        [TestMethod]
        public void RecordCompileAttempt_ReferenceUnreadable_StaysDirtyNotRed()
        {
            var project = this.Register("App");
            this.disk[Src("App")] = "h1";
            this.Change(Src("App"));

            this.registry.RecordCompileAttempt(project.Key, this.Inputs("App", new[] { Src("App") }), 1, referenceUnreadable: true);

            Assert.AreEqual(0, this.registry.Red.Count);
            Assert.AreEqual(project.Key, this.registry.Plan().Compiles.Single().Key);
            Assert.IsNull(this.registry.CompileBlockReason(project));
        }
    }
}

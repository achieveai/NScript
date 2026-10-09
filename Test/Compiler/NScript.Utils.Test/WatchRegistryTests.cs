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
        /// D-E3-1: a same-named reference that cannot be read when edges are refreshed (a
        /// share-none lock) still counts as MSBuild's copy: an edge whose copy is pending, so
        /// its reader is planned and kept, never skipped. A missing file is no copy, and a
        /// target in a ref folder pairs only with the reference assembly.
        /// </summary>
        [TestMethod]
        public void CopyEdges_UnreadableTarget_IsPendingEdge_MissingIsNot()
        {
            var refint = P("A", "obj", "refint", "A.dll");
            var copy = P("A", "bin", "A.dll");
            var refCopy = P("A", "bin", "ref", "A.dll");
            var missing = P("C", "bin", "A.dll");
            this.disk[Src("A")] = "h0";
            var a = this.registry.RegisterCompile(
                P("A"),
                new[] { "@A.rsp" },
                new CompileInputs(Out("A"), null, refint, new[] { Src("A") }, Array.Empty<string>(), Array.Empty<string>(), new Dictionary<string, string> { [Src("A")] = "h0" }),
                0,
                null,
                new[] { Src("A") },
                new Dictionary<string, string>());
            this.stamps[Out("A")] = Stamp(1);
            this.stamps[refint] = Stamp(2);
            this.locked.Add(copy);
            this.locked.Add(refCopy);
            var reader = this.registry.RegisterBundle(P("web"), new[] { "-outJs" }, P("web", "T.js"), P("T", "obj", "T.dll"), new[] { copy, refCopy, missing });

            this.registry.RefreshCopyEdges();

            CollectionAssert.AreEquivalent(
                new[] { Out("A") + " -> " + copy, refint + " -> " + refCopy },
                this.registry.CopyEdges.Select(e => e.Source + " -> " + e.Target).ToArray());
            Assert.IsTrue(this.registry.CopyEdges.Single(e => e.Target == refCopy).IsRefAssembly);
            CollectionAssert.AreEquivalent(new[] { copy, refCopy }, this.registry.CopyPending.Select(e => e.Target).ToArray());

            this.disk[Src("A")] = "h1";
            var plan = this.Change(Src("A"));
            CollectionAssert.AreEqual(new[] { reader.Key }, plan.Bundles.Select(b => b.Key).ToArray());
            this.Compiled(a);
            StringAssert.Contains(this.registry.BundleBlockReason(reader), "copy pending");
        }

        /// <summary>
        /// D-E3-1: edges are known from registration, before any batch. A registration refreshes
        /// only its own record's edges: another project's copy that a parallel build is writing
        /// right now (unreadable) is left alone until its reader registers or a batch runs.
        /// </summary>
        [TestMethod]
        public void CopyEdges_FoundAtRegistration_OnlyForTheRegisteredRecord()
        {
            var copyA = P("A", "bin", "A.dll");
            var copyB = P("B", "bin", "B.dll");
            this.stamps[Out("A")] = Stamp(1);
            this.stamps[copyA] = Stamp(1);
            this.stamps[Out("B")] = Stamp(2);
            this.Register("A");
            this.Register("B");
            var readerA = this.registry.RegisterBundle(P("web"), new[] { "-outJs" }, P("web", "A.js"), copyA, Array.Empty<string>());

            // B.js registered while its copy of B was an older build; then MSBuild rewrites
            // the copy (locked) while C registers.
            this.stamps[copyB] = Stamp(9);
            this.registry.RegisterBundle(P("web"), new[] { "-outJs" }, P("web", "B.js"), P("T", "obj", "T.dll"), new[] { copyB });
            this.stamps.Remove(copyB);
            this.locked.Add(copyB);
            this.Register("C");

            Assert.AreEqual(copyA + " " + readerA.Key, string.Join(";", this.registry.CopyEdges.Select(e => e.Target + " " + e.HolderKey)));
            Assert.AreEqual(0, this.registry.CopyPending.Count);

            this.locked.Remove(copyB);
            this.stamps[copyB] = Stamp(2);
            this.registry.RegisterBundle(P("web"), new[] { "-outJs" }, P("web", "B.js"), P("T", "obj", "T.dll"), new[] { copyB });
            CollectionAssert.AreEquivalent(new[] { copyA, copyB }, this.registry.CopyEdges.Select(e => e.Target).ToArray());
            Assert.AreEqual(0, this.registry.CopyPending.Count);
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
        /// Critic F1: an obj DLL that cannot be read (deleted by a clean, locked mid-write)
        /// is not "rebuilt outside watch". Clearing red there let the bundle emit from
        /// whatever DLL turned up next. Red clears only on a readable DLL that differs.
        /// </summary>
        [TestMethod]
        public void Red_ObjUnreadable_StaysRed_ClearsOnReadableNewObj()
        {
            var a = this.Register("A");
            this.stamps[Out("A")] = Stamp(1);
            this.disk[Src("A")] = "h1";
            this.Change(Src("A"));
            this.Compiled(a, exitCode: 1);

            this.stamps.Remove(Out("A"));
            Assert.AreEqual(0, this.registry.RefreshRed().Count, "An unreadable obj DLL cleared red.");
            CollectionAssert.AreEqual(new[] { a.Key }, this.registry.Red.ToArray());

            this.stamps[Out("A")] = Stamp(2);
            CollectionAssert.AreEqual(new[] { a.Key }, this.registry.RefreshRed().ToArray());
        }

        /// <summary>
        /// The other side of F1: a compile that failed before any obj DLL existed is red with
        /// no stamp; the first readable DLL (somebody built it) clears it.
        /// </summary>
        [TestMethod]
        public void Red_NoObjAtFailure_ClearsWhenObjAppears()
        {
            var a = this.Register("A");
            this.disk[Src("A")] = "h1";
            this.Change(Src("A"));
            this.Compiled(a, exitCode: 1);
            Assert.AreEqual(0, this.registry.RefreshRed().Count, "Still no obj DLL: still red.");

            this.stamps[Out("A")] = Stamp(1);
            CollectionAssert.AreEqual(new[] { a.Key }, this.registry.RefreshRed().ToArray());
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

        /// <summary>
        /// M3-1: a skin-only change patches its owner instead of compiling it, dependents are
        /// not touched, and the bundle is still emitted. NSCRIPT_RESOURCE_PATCH=off compiles.
        /// </summary>
        [TestMethod]
        public void Schedule_ResourceOnlyChange_PatchesOwner_UnlessPatchingIsOff()
        {
            var skin = P("A", "Shell.skin.cshtml");
            var a = this.Register("A", resources: new[] { skin });
            this.Register("B", references: new[] { Out("A") });
            this.registry.RegisterBundle(P("web"), new[] { "-outJs" }, P("web", "A.js"), Out("A"), Array.Empty<string>());

            this.disk[skin] = "h1";
            var plan = this.Change(skin);
            var steps = this.registry.Schedule(plan);

            CollectionAssert.AreEqual(new[] { a.Key, P("web", "A.js") }, steps.Select(s => s.Compile?.Key ?? s.Emit.Key).ToArray());
            Assert.IsTrue(steps[0].Patch);

            this.registry.ResourcePatchEnabled = false;
            Assert.IsFalse(this.registry.Schedule(plan)[0].Patch);
            CollectionAssert.Contains(this.registry.StatusFields().ToArray(), new KeyValuePair<string, string>("WatchResourcePatch", "off"));
        }

        /// <summary>M3-8: a .cs and a skin of one project saved in one window compile; a compile embeds the skin too.</summary>
        [TestMethod]
        public void Schedule_CsAndSkinInOneWindow_Compiles()
        {
            var skin = P("A", "Shell.skin.cshtml");
            var a = this.Register("A", resources: new[] { skin });

            this.disk[skin] = "h1";
            this.disk[Src("A")] = "h1";
            var step = this.registry.Schedule(this.Change(skin, Src("A"))).Single();

            Assert.AreEqual(a.Key, step.Compile.Key);
            Assert.IsFalse(step.Patch);
        }

        /// <summary>
        /// Pin (architect-m4): a red project is never patched. Its last compile failed, so only
        /// a compile can prove its sources; a skin change plans a compile.
        /// </summary>
        [TestMethod]
        public void Red_SkinChange_PlansCompileNotPatch()
        {
            var skin = P("A", "Shell.skin.cshtml");
            var a = this.Register("A", resources: new[] { skin });
            this.disk[Src("A")] = "h1";
            this.Change(Src("A"));
            this.Compiled(a, exitCode: 1);
            CollectionAssert.Contains(this.registry.Red.ToArray(), a.Key);

            this.disk[skin] = "h1";
            var step = this.registry.Schedule(this.Change(skin)).Single();

            Assert.AreEqual(a.Key, step.Compile.Key);
            Assert.IsFalse(step.Patch);
        }

        /// <summary>
        /// A patch is demoted to a compile by a C# change in the project or in a dependency,
        /// also one from an earlier window: the dependent stays compile-dirty while it is
        /// blocked behind its red dependency.
        /// </summary>
        [TestMethod]
        public void PatchOnly_DemotedByCsChange_InProjectOrDependency_AcrossWindows()
        {
            var skinA = P("A", "Shell.skin.cshtml");
            var skinB = P("B", "Page.skin.cshtml");
            var a = this.Register("A", resources: new[] { skinA });
            var b = this.Register("B", references: new[] { Out("A") }, resources: new[] { skinB });

            this.disk[skinA] = "h1";
            this.Change(skinA);
            Assert.IsTrue(this.registry.IsPatchOnly(a));

            // The batch has not run yet; a C# save in A lands in the next window.
            this.disk[Src("A")] = "h1";
            this.Change(Src("A"));
            Assert.IsFalse(this.registry.IsPatchOnly(a));
            Assert.IsFalse(this.registry.IsPatchOnly(b), "B is a dependent of A's C# change.");

            this.Compiled(a, exitCode: 1);
            this.disk[skinB] = "h1";
            this.Change(skinB);

            Assert.IsNotNull(this.registry.CompileBlockReason(b));
            Assert.IsFalse(this.registry.IsPatchOnly(b), "A's C# change still has to reach B.");
        }

        /// <summary>
        /// Critic F5, ordering: a skin edit in a library (patch) and a .cs edit in its reader
        /// (compile) in one window. The library is patched first, the reader compiles against
        /// the patched DLL's copy (a patch keeps the MVID, so the copy edge holds), then the
        /// bundle is emitted.
        /// </summary>
        [TestMethod]
        public void Schedule_LibrarySkinAndReaderCs_PatchThenCompileThenEmit()
        {
            var skin = P("Controls", "Button.skin.cshtml");
            var copy = P("View", "bin", "Controls.dll");
            var controls = this.Register("Controls", resources: new[] { skin });
            var view = this.Register("View", references: new[] { copy });
            this.registry.RegisterBundle(P("web"), new[] { "-outJs" }, P("web", "View.js"), Out("View"), new[] { copy });
            this.stamps[Out("Controls")] = Stamp(1);
            this.stamps[copy] = Stamp(1);
            this.registry.RefreshCopyEdges();

            this.disk[skin] = "h1";
            this.disk[Src("View")] = "h1";
            var steps = this.registry.Schedule(this.Change(Src("View"), skin));

            CollectionAssert.AreEqual(new[] { controls.Key, view.Key, P("web", "View.js") }, steps.Select(s => s.Compile?.Key ?? s.Emit.Key).ToArray());
            CollectionAssert.AreEqual(new[] { true, false, false }, steps.Select(s => s.Patch).ToArray());
            Assert.AreEqual(copy, this.registry.CopiesOf(controls.Key).Single().Target);
        }

        /// <summary>
        /// Critic F5, bookkeeping: patch and compile record the same kind of hash, so the
        /// sequence patch v2, compile v3 (M3-8), revert to v2 is a change again, and a
        /// same-content save after a patch is not.
        /// </summary>
        [TestMethod]
        public void RecordPatch_ThenCompile_ThenRevert_IsAChangeAgain()
        {
            var skin = P("A", "Shell.skin.cshtml");
            var a = this.Register("A", resources: new[] { skin });

            this.disk[skin] = "v2";
            Assert.IsTrue(this.registry.Schedule(this.Change(skin)).Single().Patch);
            this.registry.RecordPatch(a.Key, new Dictionary<string, string> { [skin] = "v2" });
            Assert.AreEqual(0, this.registry.Dirty.Count);
            Assert.AreEqual(0, this.Change(skin).Compiles.Count, "Same content as the patch embedded.");

            this.disk[skin] = "v3";
            this.disk[Src("A")] = "h1";
            Assert.IsFalse(this.registry.Schedule(this.Change(skin, Src("A"))).Single().Patch);
            this.Compiled(a);

            this.disk[skin] = "v2";
            var step = this.registry.Schedule(this.Change(skin)).Single();
            Assert.AreEqual(a.Key, step.Compile.Key);
            Assert.IsTrue(step.Patch);
        }
    }
}

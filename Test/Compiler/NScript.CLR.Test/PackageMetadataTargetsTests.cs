//-----------------------------------------------------------------------
// <copyright file="PackageMetadataTargetsTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.Test
{
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Structural-invariant tests for the MSBuild targets that ship the zero-config
    /// framework source-map flow (work item #97). These tests are intentionally
    /// static-analysis style — they parse the XML and assert on the presence of the
    /// targets / conditions / item declarations that downstream consumer builds
    /// depend on, so an accidental edit that removes a guard or renames a target is
    /// caught without needing a full pack/restore cycle (those scenarios are
    /// covered by the manual smoke test documented in the PR / decision log).
    /// </summary>
    [TestClass]
    public class PackageMetadataTargetsTests
    {
        private static string GetRepoRoot()
        {
            // The test DLL lives at TestBin/Release/net8.0/NScript.CLR.Test.dll — climb
            // four levels to reach the worktree root. Mirrors the helper in
            // LoggerStructureTests so the two stay in sync.
            string dir = Path.GetDirectoryName(typeof(PackageMetadataTargetsTests).Assembly.Location);
            for (int i = 0; i < 4; i++)
            {
                dir = Path.GetDirectoryName(dir);
            }

            return dir;
        }

        private static XDocument LoadFrameworkPackageMetadataTargets()
        {
            string path = Path.Combine(
                GetRepoRoot(),
                "Sources", "Framework", "NScript.PackageMetadata.targets");
            Assert.IsTrue(File.Exists(path),
                $"Expected packaging targets at {path}");
            return XDocument.Load(path);
        }

        private static XDocument LoadSdkTargets()
        {
            string path = Path.Combine(
                GetRepoRoot(),
                "Sources", "Compiler", "NScript.Sdk", "Sdk", "Sdk.targets");
            Assert.IsTrue(File.Exists(path),
                $"Expected SDK targets at {path}");
            return XDocument.Load(path);
        }

        [TestMethod]
        public void FrameworkTargets_DefineCaptureTargetWithBeforeCoreCompileAndGenerateNuspec()
        {
            // The target must run before BOTH CoreCompile (so the embedded resource
            // is on disk in time for csc) and GenerateNuspec (so the buildTransitive
            // .props file exists when Pack reads <None> items).
            var doc = LoadFrameworkPackageMetadataTargets();
            var target = doc.Descendants("Target")
                .FirstOrDefault(t => (string)t.Attribute("Name") == "CaptureNScriptPackageRepoMetadata");

            Assert.IsNotNull(target, "CaptureNScriptPackageRepoMetadata target missing");
            string beforeTargets = (string)target.Attribute("BeforeTargets");
            Assert.IsTrue(beforeTargets != null && beforeTargets.Contains("CoreCompile"),
                $"BeforeTargets must include CoreCompile (was '{beforeTargets}')");
            Assert.IsTrue(beforeTargets != null && beforeTargets.Contains("GenerateNuspec"),
                $"BeforeTargets must include GenerateNuspec (was '{beforeTargets}')");
        }

        [TestMethod]
        public void FrameworkTargets_CaptureTargetIsIdempotent()
        {
            // The target must guard against re-running by checking
            // _NScriptPackageMetadataCaptured — without that, sequential incremental
            // builds would rewrite the .bin/.props files needlessly and potentially
            // skew @(EmbeddedResource) timestamps causing rebuilds.
            var doc = LoadFrameworkPackageMetadataTargets();
            var target = doc.Descendants("Target")
                .First(t => (string)t.Attribute("Name") == "CaptureNScriptPackageRepoMetadata");
            string condition = (string)target.Attribute("Condition");

            Assert.IsTrue(condition != null && condition.Contains("_NScriptPackageMetadataCaptured"),
                $"Target Condition must guard with _NScriptPackageMetadataCaptured (was '{condition}')");
        }

        [TestMethod]
        public void FrameworkTargets_EmbedsResourceWithCanonicalLogicalName()
        {
            // The DLL resource MUST be named exactly "$$NScriptPackageRepo$$" — that's
            // the constant in PackageRepoMetadata.ResourceName and the value
            // PackageRepoMetadata.TryReadFromAssembly searches for. Any drift here
            // breaks every consumer.
            var doc = LoadFrameworkPackageMetadataTargets();
            var resource = doc.Descendants("EmbeddedResource")
                .FirstOrDefault(r => (string)r.Attribute("LogicalName") == "$$NScriptPackageRepo$$");

            Assert.IsNotNull(resource,
                "EmbeddedResource with LogicalName=$$NScriptPackageRepo$$ missing");
            Assert.AreEqual(
                PackageRepoMetadata.ResourceName,
                (string)resource.Attribute("LogicalName"),
                "Targets-file resource name must match PackageRepoMetadata.ResourceName constant");
        }

        [TestMethod]
        public void FrameworkTargets_PacksBuildTransitivePropsUnderConventionPath()
        {
            // NuGet only auto-imports .props files from buildTransitive/<PackageId>.props
            // (or buildTransitive/<PackageId>.targets). Any other location means the
            // consumer build silently ignores the metadata, so this is a critical
            // invariant.
            var doc = LoadFrameworkPackageMetadataTargets();
            var noneItem = doc.Descendants("None")
                .FirstOrDefault(n => (string)n.Attribute("Pack") == "true"
                    && ((string)n.Attribute("PackagePath") ?? string.Empty)
                        .StartsWith("buildTransitive/", System.StringComparison.Ordinal));

            Assert.IsNotNull(noneItem,
                "Expected <None Pack='true' PackagePath='buildTransitive/...'/> for the build-transitive props file");
            Assert.IsTrue(((string)noneItem.Attribute("PackagePath")).EndsWith(".props",
                System.StringComparison.Ordinal),
                "buildTransitive package path must end with .props");
        }

        [TestMethod]
        public void FrameworkTargets_EmbedsRedactedOriginUrlNotRaw()
        {
            // PR #98 review feedback (credential leak — MEDIUM): the redaction
            // regex (`://[^/@]+@` -> `://***@`) must be applied to the value
            // written into BOTH carriers (`.repo.bin` Lines and the
            // buildTransitive `.props` body), not just the build log message.
            // A stray PAT or password in a developer's `git remote get-url origin`
            // would otherwise ship inside the published NuGet.
            //
            // This test asserts the structural invariant by inspecting every
            // `_NScriptPkgBinLines` and `_NScriptPkgPropsLines` item declaration:
            //   - Each Include value must NOT contain a raw `$(_NScriptPkgOrigin)`
            //     reference (which would emit credentials verbatim).
            //   - At least one bin line and one props line MUST reference
            //     `$(_NScriptPkgOriginRedacted)` so we don't regress to "never
            //     emits origin at all".
            //
            // The raw `_NScriptPkgOrigin` property is allowed to exist elsewhere
            // (it is the input to the redaction regex and gates the
            // _NScriptPkgMetadataAvailable check) — only the persisted Include
            // values are constrained.
            var doc = LoadFrameworkPackageMetadataTargets();

            var binIncludes = doc.Descendants("_NScriptPkgBinLines")
                .Select(e => (string)e.Attribute("Include") ?? string.Empty)
                .ToList();
            var propsIncludes = doc.Descendants("_NScriptPkgPropsLines")
                .Select(e => (string)e.Attribute("Include") ?? string.Empty)
                .ToList();

            Assert.IsTrue(binIncludes.Count > 0,
                "Expected at least one _NScriptPkgBinLines Include in the targets file");
            Assert.IsTrue(propsIncludes.Count > 0,
                "Expected at least one _NScriptPkgPropsLines Include in the targets file");

            foreach (var include in binIncludes)
            {
                Assert.IsFalse(
                    System.Text.RegularExpressions.Regex.IsMatch(include, @"\$\(_NScriptPkgOrigin\)"),
                    $"_NScriptPkgBinLines Include leaks raw origin URL into .repo.bin: '{include}'. " +
                    "Use $(_NScriptPkgOriginRedacted) instead — see PR #98 review.");
            }

            foreach (var include in propsIncludes)
            {
                Assert.IsFalse(
                    System.Text.RegularExpressions.Regex.IsMatch(include, @"\$\(_NScriptPkgOrigin\)"),
                    $"_NScriptPkgPropsLines Include leaks raw origin URL into buildTransitive props: '{include}'. " +
                    "Use $(_NScriptPkgOriginRedacted) instead — see PR #98 review.");
            }

            Assert.IsTrue(
                binIncludes.Any(i => i.Contains("$(_NScriptPkgOriginRedacted)")),
                "Expected at least one _NScriptPkgBinLines Include to embed $(_NScriptPkgOriginRedacted) — " +
                "the .repo.bin payload must carry the origin URL (redacted).");
            Assert.IsTrue(
                propsIncludes.Any(i => i.Contains("$(_NScriptPkgOriginRedacted)")),
                "Expected at least one _NScriptPkgPropsLines Include to embed $(_NScriptPkgOriginRedacted) — " +
                "the buildTransitive .props must carry the origin URL (redacted).");
        }

        [TestMethod]
        public void SdkTargets_DefineAutoResolveTargetBeforeSecondaryRepoMetadata()
        {
            // ComputeNScriptPackageMetadataFromReferences sets NScriptSecondaryRepoRoot
            // / NScriptSecondaryCommitSha / NScriptSecondarySourceRepoOriginUrl, which
            // ComputeNScriptSecondaryRepoMetadata then turns into
            // NScriptSecondarySourceMapRoot. Order must be enforced via BeforeTargets.
            var doc = LoadSdkTargets();
            var target = doc.Descendants("Target")
                .FirstOrDefault(t => (string)t.Attribute("Name") == "ComputeNScriptPackageMetadataFromReferences");

            Assert.IsNotNull(target,
                "ComputeNScriptPackageMetadataFromReferences target missing from Sdk.targets");
            Assert.AreEqual("ComputeNScriptSecondaryRepoMetadata",
                (string)target.Attribute("BeforeTargets"),
                "Auto-resolve target must run BEFORE ComputeNScriptSecondaryRepoMetadata");
        }

        [TestMethod]
        public void SdkTargets_AutoResolveSkipsWhenManualOverrideSet()
        {
            // AC #3 from #97: manual NScriptSecondaryRepoRoot must beat the auto-resolved
            // value. The simplest, most-readable way to enforce this is to skip the
            // auto-resolve target when the user already set the property.
            var doc = LoadSdkTargets();
            var target = doc.Descendants("Target")
                .First(t => (string)t.Attribute("Name") == "ComputeNScriptPackageMetadataFromReferences");
            string condition = (string)target.Attribute("Condition");

            Assert.IsTrue(condition != null && condition.Contains("'$(NScriptSecondaryRepoRoot)' == ''"),
                $"Auto-resolve Condition must skip when NScriptSecondaryRepoRoot is non-empty (was '{condition}')");
        }

        [TestMethod]
        public void SdkTargets_AutoResolveGatedByRepoLinkedSourceMapsFlag()
        {
            // The whole feature is opt-in via <RepoLinkedSourceMaps>true</...>. The
            // auto-resolve target must respect that flag too, otherwise a project
            // that disabled source-map repo linking would still pay the cost of
            // iterating @(NScriptPackagesWithRepoMetadata).
            var doc = LoadSdkTargets();
            var target = doc.Descendants("Target")
                .First(t => (string)t.Attribute("Name") == "ComputeNScriptPackageMetadataFromReferences");
            string condition = (string)target.Attribute("Condition");

            Assert.IsTrue(condition != null && condition.Contains("'$(RepoLinkedSourceMaps)' == 'true'"),
                $"Auto-resolve Condition must gate on RepoLinkedSourceMaps=true (was '{condition}')");
        }

        [TestMethod]
        public void SdkTargets_AutoResolveGatedByNonEmptyMetadataItemGroup()
        {
            // If no referenced package ships metadata (e.g. consumer references an
            // older NScript build), the target must skip — otherwise we'd write empty
            // strings into the secondary-repo properties and break the downstream
            // regex builder. Gate on @(NScriptPackagesWithRepoMetadata)!='' to keep
            // the no-metadata case a quiet no-op.
            var doc = LoadSdkTargets();
            var target = doc.Descendants("Target")
                .First(t => (string)t.Attribute("Name") == "ComputeNScriptPackageMetadataFromReferences");
            string condition = (string)target.Attribute("Condition");

            Assert.IsTrue(condition != null
                && condition.Contains("'@(NScriptPackagesWithRepoMetadata)' != ''"),
                $"Auto-resolve Condition must gate on a non-empty metadata item group (was '{condition}')");
        }
    }
}

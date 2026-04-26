// Copyright (c) Microsoft. All rights reserved.

namespace OwaSourceMapper.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for <see cref="RepoUrlBuilder"/> — provider auto-detection across the
    /// half-dozen Git remote URL shapes we expect in the wild (https/ssh, with/without
    /// <c>.git</c> suffix, modern dev.azure.com, legacy *.visualstudio.com).
    /// </summary>
    [TestClass]
    public class RepoUrlBuilderTests
    {
        [TestMethod]
        public void TryBuildRawUrl_GitHubHttps_ConstructsRawGithubusercontentUrl()
        {
            bool ok = RepoUrlBuilder.TryBuildRawUrl(
                "https://github.com/achieveai/NScript.git",
                "abc123def456",
                out string url,
                out var provider);

            Assert.IsTrue(ok);
            Assert.AreEqual(RepoUrlBuilder.Provider.GitHub, provider);
            Assert.AreEqual("https://raw.githubusercontent.com/achieveai/NScript/abc123def456/", url);
        }

        [TestMethod]
        public void TryBuildRawUrl_GitHubHttpsNoDotGit_ConstructsRawUrl()
        {
            bool ok = RepoUrlBuilder.TryBuildRawUrl(
                "https://github.com/owner/repo",
                "deadbeef",
                out string url,
                out _);

            Assert.IsTrue(ok);
            Assert.AreEqual("https://raw.githubusercontent.com/owner/repo/deadbeef/", url);
        }

        [TestMethod]
        public void TryBuildRawUrl_GitHubSsh_ConstructsRawUrl()
        {
            bool ok = RepoUrlBuilder.TryBuildRawUrl(
                "git@github.com:achieveai/NScript.git",
                "sha",
                out string url,
                out var provider);

            Assert.IsTrue(ok);
            Assert.AreEqual(RepoUrlBuilder.Provider.GitHub, provider);
            Assert.AreEqual("https://raw.githubusercontent.com/achieveai/NScript/sha/", url);
        }

        [TestMethod]
        public void TryBuildRawUrl_GitHubSshUrlPrefix_ConstructsRawUrl()
        {
            bool ok = RepoUrlBuilder.TryBuildRawUrl(
                "ssh://git@github.com/owner/repo.git",
                "sha",
                out string url,
                out _);

            Assert.IsTrue(ok);
            Assert.AreEqual("https://raw.githubusercontent.com/owner/repo/sha/", url);
        }

        [TestMethod]
        public void TryBuildRawUrl_AzureDevOpsModernHttps_ConstructsItemsApiUrl()
        {
            bool ok = RepoUrlBuilder.TryBuildRawUrl(
                "https://dev.azure.com/contoso/MyProject/_git/MyRepo",
                "shaXYZ",
                out string url,
                out var provider);

            Assert.IsTrue(ok);
            Assert.AreEqual(RepoUrlBuilder.Provider.AzureDevOps, provider);
            StringAssert.StartsWith(url, "https://dev.azure.com/contoso/MyProject/_apis/git/repositories/MyRepo/items?");
            StringAssert.Contains(url, "versionDescriptor.version=shaXYZ");
            StringAssert.Contains(url, "versionDescriptor.versionType=commit");
            StringAssert.EndsWith(url, "path=/");
        }

        [TestMethod]
        public void TryBuildRawUrl_AzureDevOpsModernSsh_Detected()
        {
            bool ok = RepoUrlBuilder.TryBuildRawUrl(
                "git@ssh.dev.azure.com:v3/contoso/MyProject/MyRepo",
                "shaXYZ",
                out string url,
                out var provider);

            Assert.IsTrue(ok);
            Assert.AreEqual(RepoUrlBuilder.Provider.AzureDevOps, provider);
            StringAssert.Contains(url, "/contoso/MyProject/_apis/git/repositories/MyRepo/items?");
        }

        [TestMethod]
        public void TryBuildRawUrl_AzureDevOpsLegacyVisualStudioCom_Detected()
        {
            bool ok = RepoUrlBuilder.TryBuildRawUrl(
                "https://contoso.visualstudio.com/MyProject/_git/MyRepo",
                "sha",
                out string url,
                out var provider);

            Assert.IsTrue(ok);
            Assert.AreEqual(RepoUrlBuilder.Provider.AzureDevOps, provider);
            StringAssert.Contains(url, "/contoso/MyProject/_apis/git/repositories/MyRepo/items?");
        }

        [TestMethod]
        public void TryBuildRawUrl_UnknownProvider_ReturnsFalse()
        {
            bool ok = RepoUrlBuilder.TryBuildRawUrl(
                "https://gitlab.com/owner/repo.git",
                "sha",
                out string url,
                out var provider);

            Assert.IsFalse(ok);
            Assert.IsNull(url);
            Assert.AreEqual(RepoUrlBuilder.Provider.Unknown, provider);
        }

        [TestMethod]
        public void TryBuildRawUrl_NullOrEmptyInputs_ReturnsFalse()
        {
            Assert.IsFalse(RepoUrlBuilder.TryBuildRawUrl(null, "sha", out _, out _));
            Assert.IsFalse(RepoUrlBuilder.TryBuildRawUrl("https://github.com/o/r", null, out _, out _));
            Assert.IsFalse(RepoUrlBuilder.TryBuildRawUrl(string.Empty, "sha", out _, out _));
            Assert.IsFalse(RepoUrlBuilder.TryBuildRawUrl("https://github.com/o/r", "  ", out _, out _));
        }

        [TestMethod]
        public void TryBuildRawUrl_MalformedUrl_ReturnsFalse()
        {
            Assert.IsFalse(RepoUrlBuilder.TryBuildRawUrl("not-a-url-at-all", "sha", out _, out _));
            Assert.IsFalse(RepoUrlBuilder.TryBuildRawUrl("https://github.com", "sha", out _, out _));
            Assert.IsFalse(RepoUrlBuilder.TryBuildRawUrl("https://github.com/owner", "sha", out _, out _));
        }

        [TestMethod]
        public void DetectProvider_RecognizesEachProviderWithoutSha()
        {
            Assert.AreEqual(RepoUrlBuilder.Provider.GitHub,
                RepoUrlBuilder.DetectProvider("https://github.com/o/r.git"));
            Assert.AreEqual(RepoUrlBuilder.Provider.AzureDevOps,
                RepoUrlBuilder.DetectProvider("https://dev.azure.com/o/p/_git/r"));
            Assert.AreEqual(RepoUrlBuilder.Provider.Unknown,
                RepoUrlBuilder.DetectProvider("https://example.com/x"));
            Assert.AreEqual(RepoUrlBuilder.Provider.Unknown,
                RepoUrlBuilder.DetectProvider(null));
        }
    }
}

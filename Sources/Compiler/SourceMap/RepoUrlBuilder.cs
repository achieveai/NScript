// Copyright (c) Microsoft. All rights reserved.

namespace OwaSourceMapper
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Constructs the <c>sourceRoot</c> URL that points to a remote repository's raw-file
    /// endpoint (GitHub or Azure DevOps).
    /// </summary>
    /// <remarks>
    /// The MSBuild SDK target <c>ComputeNScriptRepoMetadata</c> in <c>Sdk.targets</c> emits the
    /// same URL shape using inline <c>Regex.Match</c> property functions (so the build doesn't
    /// have to load a separate task assembly). This class is the canonical regression-test
    /// target for that URL shape — keep the regex patterns here and in <c>Sdk.targets</c> in
    /// sync, and rely on <c>RepoUrlBuilderTests</c> to catch drift.
    /// </remarks>
    public static class RepoUrlBuilder
    {
        // GitHub variants:
        //   https://github.com/{owner}/{repo}.git
        //   https://github.com/{owner}/{repo}
        //   git@github.com:{owner}/{repo}.git
        //   ssh://git@github.com/{owner}/{repo}.git
        private static readonly Regex GitHubHttps = new Regex(
            @"^https?://(?:[^/@]+@)?github\.com/(?<owner>[^/]+)/(?<repo>[^/]+?)(?:\.git)?/?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex GitHubSsh = new Regex(
            @"^(?:ssh://)?git@github\.com[:/](?<owner>[^/]+)/(?<repo>[^/]+?)(?:\.git)?/?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Modern ADO: https://dev.azure.com/{org}/{project}/_git/{repo}
        // SSH ADO:    git@ssh.dev.azure.com:v3/{org}/{project}/{repo}
        // Legacy:     https://{org}.visualstudio.com/{project}/_git/{repo}
        // Legacy SSH: ssh://{org}@vs-ssh.visualstudio.com:22/{project}/_git/{repo}
        private static readonly Regex AdoModernHttps = new Regex(
            @"^https?://(?:[^/@]+@)?dev\.azure\.com/(?<org>[^/]+)/(?<project>[^/]+)/_git/(?<repo>[^/]+?)(?:\.git)?/?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex AdoModernSsh = new Regex(
            @"^git@ssh\.dev\.azure\.com:v3/(?<org>[^/]+)/(?<project>[^/]+)/(?<repo>[^/]+?)(?:\.git)?/?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex AdoLegacyHttps = new Regex(
            @"^https?://(?:[^/@]+@)?(?<org>[^/.]+)\.visualstudio\.com/(?<project>[^/]+)/_git/(?<repo>[^/]+?)(?:\.git)?/?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Identifies the supported source-control hosting providers.
        /// </summary>
        public enum Provider
        {
            /// <summary>Provider could not be auto-detected from the remote URL.</summary>
            Unknown,

            /// <summary>GitHub (github.com).</summary>
            GitHub,

            /// <summary>Azure DevOps (modern <c>dev.azure.com</c> or legacy <c>*.visualstudio.com</c>).</summary>
            AzureDevOps,
        }

        /// <summary>
        /// Builds a raw-file URL that ends with a trailing slash so callers can append
        /// repo-relative source paths to form full URLs.
        /// </summary>
        /// <param name="remoteUrl">       The Git remote URL (output of <c>git remote get-url origin</c>). </param>
        /// <param name="commitSha">       The commit SHA (output of <c>git rev-parse HEAD</c>). May not be null/empty. </param>
        /// <param name="rawUrl">          On success, the constructed raw-file URL ending with <c>/</c>. </param>
        /// <param name="detectedProvider"> On success, the auto-detected provider. </param>
        /// <returns>True if <paramref name="remoteUrl"/> was recognised; false otherwise.</returns>
        public static bool TryBuildRawUrl(
            string remoteUrl,
            string commitSha,
            out string rawUrl,
            out Provider detectedProvider)
        {
            rawUrl = null;
            detectedProvider = Provider.Unknown;

            if (string.IsNullOrWhiteSpace(remoteUrl) || string.IsNullOrWhiteSpace(commitSha))
            {
                return false;
            }

            string trimmedRemote = remoteUrl.Trim();
            string trimmedSha = commitSha.Trim();

            if (TryParseGitHub(trimmedRemote, out var owner, out var repo))
            {
                detectedProvider = Provider.GitHub;
                rawUrl = string.Format(
                    "https://raw.githubusercontent.com/{0}/{1}/{2}/",
                    owner,
                    repo,
                    trimmedSha);
                return true;
            }

            if (TryParseAzureDevOps(trimmedRemote, out var adoOrg, out var adoProject, out var adoRepo))
            {
                detectedProvider = Provider.AzureDevOps;
                // ADO has no github-style raw endpoint, so we use the Items API and bake the
                // SHA + a trailing `path=/` into the prefix. DevTools fetches `sourceRoot + sources[i]`
                // as a plain GET, which lands as `…&path=/{relativePath}` — the shape Items API expects.
                rawUrl = string.Format(
                    "https://dev.azure.com/{0}/{1}/_apis/git/repositories/{2}/items?api-version=7.1&versionDescriptor.version={3}&versionDescriptor.versionType=commit&path=/",
                    adoOrg,
                    adoProject,
                    adoRepo,
                    trimmedSha);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Provider-detection helper that does not construct a URL. Useful when callers want
        /// to log or branch on provider without resolving a SHA.
        /// </summary>
        public static Provider DetectProvider(string remoteUrl)
        {
            if (string.IsNullOrWhiteSpace(remoteUrl))
            {
                return Provider.Unknown;
            }

            string trimmed = remoteUrl.Trim();
            if (TryParseGitHub(trimmed, out _, out _)) { return Provider.GitHub; }
            if (TryParseAzureDevOps(trimmed, out _, out _, out _)) { return Provider.AzureDevOps; }
            return Provider.Unknown;
        }

        private static bool TryParseGitHub(string remoteUrl, out string owner, out string repo)
        {
            owner = null;
            repo = null;

            var match = GitHubHttps.Match(remoteUrl);
            if (!match.Success)
            {
                match = GitHubSsh.Match(remoteUrl);
            }

            if (!match.Success)
            {
                return false;
            }

            owner = match.Groups["owner"].Value;
            repo = match.Groups["repo"].Value;
            return !string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(repo);
        }

        private static bool TryParseAzureDevOps(string remoteUrl, out string org, out string project, out string repo)
        {
            org = null;
            project = null;
            repo = null;

            var match = AdoModernHttps.Match(remoteUrl);
            if (!match.Success) { match = AdoModernSsh.Match(remoteUrl); }
            if (!match.Success) { match = AdoLegacyHttps.Match(remoteUrl); }

            if (!match.Success)
            {
                return false;
            }

            org = match.Groups["org"].Value;
            project = match.Groups["project"].Value;
            repo = match.Groups["repo"].Value;
            return !string.IsNullOrEmpty(org) && !string.IsNullOrEmpty(project) && !string.IsNullOrEmpty(repo);
        }
    }
}

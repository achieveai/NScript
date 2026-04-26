# Repo-Linked Source Maps

By default, NScript-generated source maps reference the original C# files via the
local-machine paths recorded at build time and resolved through the ASP.NET Core
`SourceMapFileHandler` (or the legacy `SrcMapper.ashx`). This works while the build
machine is also the host serving the application, but breaks the moment the build
artifacts are deployed somewhere that doesn't have access to the source tree.

**Repo-linked source maps** point browser DevTools directly at the originating Git
provider's raw-file URL (GitHub `raw.githubusercontent.com` or Azure DevOps
`dev.azure.com/{org}/{project}/_apis/git/repositories/.../items?…`) for the exact
commit that produced the JS. DevTools fetches the source over HTTPS using the
developer's existing browser session — no special handler required on the host,
and zero risk of source files being out of date.

This is **opt-in**: set `<RepoLinkedSourceMaps>true</RepoLinkedSourceMaps>` on the
generating project. Builds without that property continue to emit local-path maps
exactly as before.

---

## Quick Start

### GitHub

```xml
<PropertyGroup>
  <GenerateJs>true</GenerateJs>
  <RepoLinkedSourceMaps>true</RepoLinkedSourceMaps>
</PropertyGroup>
```

That's it. During `ScriptGenerate`, MSBuild captures `git remote get-url origin`,
`git rev-parse HEAD`, and `git rev-parse --show-toplevel`, detects GitHub from the
remote URL, and emits a map whose `sourceRoot` is

```
https://raw.githubusercontent.com/{owner}/{repo}/{commitSha}/
```

Each entry in the map's `sources[]` array becomes the repo-relative forward-slash
path of the original file (e.g. `Sources/Framework/Sunlight.Framework/Logger.cs`),
so when DevTools concatenates `sourceRoot + sources[i]` it gets a working
raw-file URL that always matches the deployed JS.

### Azure DevOps

The same flag works for Azure DevOps remotes — both modern (`dev.azure.com/{org}/{project}/_git/{repo}`)
and legacy (`{org}.visualstudio.com/{project}/_git/{repo}`):

```xml
<PropertyGroup>
  <GenerateJs>true</GenerateJs>
  <RepoLinkedSourceMaps>true</RepoLinkedSourceMaps>
</PropertyGroup>
```

For ADO the emitted `sourceRoot` is the
[Items API](https://learn.microsoft.com/en-us/rest/api/azure/devops/git/items/get):

```
https://dev.azure.com/{org}/{project}/_apis/git/repositories/{repo}/items?api-version=7.1&versionDescriptor.version={sha}&versionDescriptor.versionType=commit&path=/
```

The trailing `path=/` lets the appended `sources[i]` substitute as the path
(e.g. `…path=/Sources/Framework/Sunlight.Framework/Logger.cs`).

> **Note** — fetching from a private ADO repo requires the developer's browser to
> already have a valid session cookie for `dev.azure.com`. This is the normal
> case for engineers who use the Azure DevOps web UI; CI/headless contexts will
> see 401s and need to fall back to local-disk source maps.

---

## CI Overrides

The MSBuild target prefers explicit values over `git` invocations, so CI can
feed pre-resolved metadata directly without paying for a `git` shell-out:

```xml
<PropertyGroup>
  <RepoLinkedSourceMaps>true</RepoLinkedSourceMaps>

  <!-- Override one or more — anything left blank is auto-detected via git -->
  <SourceRepoOriginUrl>https://github.com/owner/repo.git</SourceRepoOriginUrl>
  <SourceRepoCommitSha>$(GITHUB_SHA)</SourceRepoCommitSha>
  <NScriptRepoRoot>$(GITHUB_WORKSPACE)</NScriptRepoRoot>
</PropertyGroup>
```

For Azure Pipelines the equivalent variables are `$(BUILD_SOURCEVERSION)` and
`$(BUILD_SOURCESDIRECTORY)`. GitLab CI uses `$CI_COMMIT_SHA` and `$CI_PROJECT_DIR`.

To force a particular provider when the auto-detection regex doesn't fit your
URL shape, set `SourceRepoProvider` to `GitHub` or `AzureDevOps`. The default
value `auto` runs the detection chain.

```xml
<SourceRepoProvider>GitHub</SourceRepoProvider>
```

---

## Behaviour When Git Metadata Is Missing

Building outside a git checkout (CI cache restore, source-only ZIP) emits a
build warning and skips the repo-linked sourceRoot, falling back to the local-
disk behaviour:

```
RepoLinkedSourceMaps requested but git metadata could not be captured (origin='' sha=''). Skipping repo-linked sourceRoot.
```

If git succeeds but the remote URL doesn't match GitHub or Azure DevOps:

```
RepoLinkedSourceMaps requested but origin URL '…' did not match GitHub or Azure DevOps; emitting map without repo-linked sourceRoot.
```

In both cases the build still produces a working map — just the legacy local-
path variant.

---

## Handler 302 Redirect (Optional)

When you ship the ASP.NET Core `SourceMapFileHandler` AND want to keep serving
local files for developers who run from source while still letting external
testers / production-build users get the repo-hosted sources, opt into the 302
redirect:

```csharp
endpoints.MapSourceMapFiles("/sourcemap", new SourceMapFileHandlerOptions
{
    MapsDirectory = mapsDir,
    AllowedSourceRoots = new[] { repoRoot },
    RepoUrlRedirectOnMiss = true,
});
```

With that flag set, when a request hits the handler and the local file isn't
available — file missing on disk, outside the allow-list, etc. — and the parsed
map carries an `http(s)://` `sourceRoot`, the handler answers `302 Found` with
`Location: {sourceRoot}{sourceName}` instead of `404 Not Found`. The browser
follows the redirect using its existing session cookies for the Git provider.

> **SECURITY** — A tampered `.map` could redirect the browser to an attacker-
> controlled URL. Only enable `RepoUrlRedirectOnMiss` in deployments that trust
> the maps they ship. The handler defends against the obvious abuse vectors:
>
> - The redirect only fires for `http://` / `https://` `sourceRoot` values —
>   `javascript:`, `data:`, `file://`, relative paths, and the legacy `.ashx`
>   format are all refused (404 instead).
> - The redirect only fires when the requested short name appears verbatim in
>   the parsed map's `sources[]` array, so a request for an arbitrary path
>   appended after the prefix won't escape into a redirect.

---

## Limitations

- **Provider scope** — only GitHub and Azure DevOps remotes are auto-detected.
  Bitbucket, GitLab, Gitea, etc. are not in scope; for those, set
  `SourceMapRoot` directly to the right URL pattern instead of using
  `RepoLinkedSourceMaps`.
- **Public-or-authenticated only** — the redirect / direct-fetch path relies on
  the browser already having credentials for the provider. There is no
  proxy/token-based fetch story; that's deliberate scope.
- **Snapshot builds** — the captured commit SHA is the value of `HEAD` at the
  moment `dotnet build` runs. If you build with uncommitted changes, the
  resulting map points at the previous commit's source — not what you actually
  compiled. Either commit before building, or override `SourceRepoCommitSha`.
- **Force pushes / rewritten history** — the URL is keyed on the commit SHA, so
  a force-push that rewrites or deletes that SHA will turn the map into a 404
  reference. The local-disk fallback is more durable in that respect.

---

## Underlying Properties Reference

| Property | Default | Purpose |
| --- | --- | --- |
| `RepoLinkedSourceMaps` | `false` | Master switch — turns on `ComputeNScriptRepoMetadata` and emits `-repoRoot` to the converter. |
| `SourceRepoOriginUrl` | (from `git remote get-url origin`) | Override the detected remote URL. |
| `SourceRepoCommitSha` | (from `git rev-parse HEAD`) | Override the detected commit SHA. |
| `NScriptRepoRoot` | (from `git rev-parse --show-toplevel`) | Override the detected worktree root used to rebase `sources[]`. |
| `SourceRepoProvider` | `auto` | Force `GitHub` / `AzureDevOps` when auto-detection misfires. |
| `SourceMapRoot` | (computed) | When already set, the metadata-capture target is skipped entirely — bring-your-own URL workflow. |

The compiler-side flags accepted by the converter are `-sourceMapRoot <url>` and
`-repoRoot <absolute-path>`. The two are paired: `-repoRoot` is rejected when
`-sourceMapRoot` is absent, since rebasing `sources[]` only makes sense when
combined with a non-local URL.

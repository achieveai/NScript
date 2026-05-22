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

## Zero-Config NuGet Consumer Scenario

When you consume NScript via NuGet (rather than via a local repo checkout), the
framework NuGet packages — `Mcqdb.NScript.Sunlight.Framework`,
`…Sunlight.Framework.UI`, `…Sunlight.Framework.Data`, etc. — carry the originating
Git remote URL + commit SHA + build-machine worktree root inside the package.
Setting `<RepoLinkedSourceMaps>true</RepoLinkedSourceMaps>` is enough to get
**framework source files** to resolve to the right
`https://raw.githubusercontent.com/achieveai/NScript/{sha}/Sources/Framework/…`
URLs at build time — no `NScriptSecondaryRepoRoot` / `NScriptSecondarySourceMapRoot`
needed.

```xml
<PropertyGroup>
  <GenerateJs>true</GenerateJs>
  <RepoLinkedSourceMaps>true</RepoLinkedSourceMaps>
</PropertyGroup>
```

This works because each framework NuGet ships:

1. A `buildTransitive/<PackageId>.props` file that MSBuild auto-imports when the
   package is referenced. The .props appends an entry to the shared
   `@(NScriptPackagesWithRepoMetadata)` item group, recording the package's
   origin URL, commit SHA, and build-machine repo root.
2. A `$$NScriptPackageRepo$$` resource embedded in the framework DLL (text key=value
   payload, parseable via `NScript.CLR.PackageRepoMetadata.TryReadFromAssembly`).
   This is the carrier of last resort for MSBuild-less tooling.

On the consumer side, `Sdk.targets` runs a small target,
`ComputeNScriptPackageMetadataFromReferences`, that picks the first entry from
`@(NScriptPackagesWithRepoMetadata)` and feeds the existing secondary-repo
metadata pipeline. The result is identical to manually setting
`NScriptSecondaryRepoRoot` / `NScriptSecondarySourceRepoOriginUrl` /
`NScriptSecondaryCommitSha` to the right values.

### Inspecting the auto-resolved metadata

The build emits a high-importance log line listing the detected packages and
the chosen values:

```
NScript package metadata: auto-resolved from package(s) 'Mcqdb.NScript.Sunlight.Framework;Mcqdb.NScript.Sunlight.Framework.UI;...' — origin='https://github.com/achieveai/NScript.git' sha='b0a4cc6e...' root='B:\sources\NScript'
```

For deeper debugging, the per-package `buildTransitive` .props also sets diagnostic
properties named `NScriptPackageOriginUrl_<id>` / `NScriptPackageCommitSha_<id>` /
`NScriptPackageRepoRoot_<id>` (where `<id>` is the package ID with `.` replaced
by `_`). You can query any of these via
`dotnet msbuild -getProperty:NScriptPackageOriginUrl_Mcqdb_NScript_Sunlight_Framework`.

### Manual override still wins

If you set `<NScriptSecondaryRepoRoot>…</NScriptSecondaryRepoRoot>` yourself, the
auto-resolve target detects that the property is already populated and skips
entirely — the manual value flows through to the existing
`ComputeNScriptSecondaryRepoMetadata` regex builder exactly as before. This
gives you a clean escape hatch for two cases:

- You have a local NScript checkout (e.g. for `dotnet pack`-on-save dev loops)
  and want source maps to point at uncommitted changes on disk rather than the
  package's frozen SHA.
- The package was packed outside a git checkout and shipped without metadata
  (see below).

### Packages without metadata

If a framework NuGet was built outside a git checkout (corrupted source-only
ZIP, very old NScript version, etc.), it ships without the
`buildTransitive` .props and without the embedded DLL resource. The consumer
build then sees an empty `@(NScriptPackagesWithRepoMetadata)`, the auto-resolve
target is a no-op, and the build falls back to the legacy local-path source map
behavior (with the same warning emitted as the no-git-checkout case described
above). The build never fails because of missing metadata.

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
| `NScriptSecondaryRepoRoot` | (auto-detected from `@(NScriptPackagesWithRepoMetadata)`) | Build-machine worktree root of a secondary dependency repo whose source files should resolve to a different `https://` URL. Manual setting wins over auto-detection. |
| `NScriptSecondarySourceMapRoot` | (computed) | Raw-file base URL for the secondary repo. Auto-derived from origin URL + SHA via the same regex builder used for the primary repo. |
| `NScriptSecondaryCommitSha` | (auto-detected) | Override the detected secondary commit SHA. |
| `NScriptSecondarySourceRepoOriginUrl` | (auto-detected) | Override the detected secondary origin URL. |
| `NScriptSecondaryRepoProvider` | `auto` | Force `GitHub` / `AzureDevOps` when secondary auto-detection misfires. |

The compiler-side flags accepted by the converter are `-sourceMapRoot <url>`,
`-repoRoot <absolute-path>`, `-secondarySourceRoot <url>`, and
`-secondaryRepoRoot <absolute-path>`. The primary pair (`-sourceMapRoot` +
`-repoRoot`) is required for any repo-linked source map; the secondary pair is
optional and enables multi-repo emission. `-repoRoot` is rejected when
`-sourceMapRoot` is absent, since rebasing `sources[]` only makes sense when
combined with a non-local URL.

`@(NScriptPackagesWithRepoMetadata)` is the item group that the auto-resolve
target enumerates. Each entry has `OriginUrl`, `CommitSha`, and `RepoRoot`
metadata and is `Include`'d by its source package's `PackageId`. Inspect with
`<Message Text="@(NScriptPackagesWithRepoMetadata->'%(Identity): %(OriginUrl) @ %(CommitSha) under %(RepoRoot)')"/>`
in your project for debugging.

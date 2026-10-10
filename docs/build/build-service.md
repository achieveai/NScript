# Build service

> **Audience:** *Contributors* and *App authors* who build NScript apps in Debug.

## TL;DR

- The build service is a background `nscript.exe` daemon. It keeps the compiler warm.
- In watch mode it also watches your sources. A save rebuilds the JS, with no `dotnet build`.
  - TodoApp, one `.cs` line: about 0.9 s (measured; see the trigger plan under Cross-links).
- A plain `dotnet build` then asks the daemon first. If all is current, it skips the project-reference builds.
- Release-style builds never use it.

## Quick start

```bash
# 1. Start watching. This builds once and starts the daemon.
dotnet build Test/Framework/TodoApp/TodoApp.csproj -p:NScriptWatch=true

# 2. Edit and save. The daemon rebuilds the JS. Follow watch.log (see Logs).

# 3. Build as usual. It asks the daemon first.
dotnet build Test/Framework/TodoApp/TodoApp.csproj

# 4. Stop watching.
NScriptToolSet/bin/Debug/net8.0/nscript.exe service --stop
```

## What it is

- One daemon per toolset build. The key is the toolset folder, the user and elevation.
- The first service build starts it. It runs from a shadow copy of the toolset.
- `-p:NScriptService=true`: compiles through the warm daemon. No watching.
- `-p:NScriptWatch=true`: implies the service. It also registers every project the build compiles, and the JS bundle.
  - Run it once per app you want watched. TodoApp, then TodoApp.Test, gives 11 projects and 2 bundles.
- The daemon writes dev-mode JS: unminified and unoptimized (`nscript -devMode`).
- Builds with `Minify`, `Uglify` or `JsOptimize` (the Release default) never use the service.
- The daemon exits after 10 min idle, or 8 h idle while watching.
- A rebuilt toolset ends the watch at the next save. watch.log says `compiler rebuilt`. Run the watch build again.

## What a save does

Saves within a short debounce window form one batch.

| You save | The daemon |
|---|---|
| `.cs` in a watched project | Recompiles it and the projects that use it. Re-emits each bundle that includes them. |
| Skin (`.skin.cshtml`), CSS, XWML (`.html`) | Patches the new resources into the DLL (no C# compile). Re-emits the bundles. |
| The same bytes again | Ignores it. |
| A new file, a deleted input, or a `.csproj` / `.props` / `.targets` edit | Logs `NEEDS BUILD <project>: <reason>`. That project waits for `dotnet build -p:NScriptWatch=true`. |
| Code that does not compile | Logs `compile <dll> FAILED` and the compiler errors. The last good JS stays (`KEPT ... (last good; ...)`). Fix and save again. |

- A save that arrives mid-batch stops the batch early. The next batch picks up the rest (`superseded batch`).
- A locked JS file is retried. Then it is `KEPT ... (stale: output in use ...)`. Save again.
- A file added, or a build file edited, while the watch build itself runs (from the moment MSBuild reads the project) is a change too: `NEEDS BUILD`. Run the watch build again.

### When a file watcher fails

- **Overflow** (too many changes at once): watch.log says `watcher overflow on <folder> (...); rescanning`. The daemon rechecks every input and build file and keeps watching.
- **Any other watcher error** (a watched folder removed, a network share gone): watch.log says `watcher lost <folder> (...); watch stopped`.
  - The daemon stops and deletes its markers. Edits in that folder would go unseen.
  - The next `dotnet build` runs in full. Run `dotnet build -p:NScriptWatch=true` to watch again.

## `dotnet build` while watching

- The daemon keeps a marker, `nscript.watch`, next to each watched project's obj DLL (`obj/<Config>/<TFM>/`).
- A build that finds the marker runs `nscript service --sync <obj dll> --build-props <hash>` before `BeforeBuild`.
- **Yes** (exit 0): the daemon built this project, its references and its JS.
  - MSBuild skips the project-reference builds and reads the references the daemon vouched for.
  - It keeps the daemon's JS, unless another build rewrote it.
  - Message: `NScript watch: <App> current, skipped <N> project builds`.
  - After a skin or CSS save it also keeps the daemon's patched DLL. The patch leaves the PDB older than the resource, so MSBuild would recompile. The build skips csc instead (the `CoreCompile` target still runs).
    - Only when embedded resources alone are newer than the compile outputs, and none is newer than the DLL.
    - Any other newer input (a `.cs` file, an unvouched reference, `.editorconfig`, an analyzer) or a missing output still compiles.
    - Message: `NScript watch: <App> resources are in the watch's DLL; csc skipped`.
  - The patch also leaves every project that reads the patched DLL older than it. The daemon does not recompile them for a resource. So a reference the daemon vouched for (an `nscript-ref` line) that is newer than the outputs skips csc too.
    - A reference it did not vouch for still compiles.
    - Message: `NScript watch: <App> vouched references are newer, its DLL is the watch's; csc skipped`.
- **No** (exit 1), **busy** (exit 2) or no daemon: today's full build.
  - Message: `NScript watch: full build (<reason>)`.
- **Other compile properties: no.** The daemon replays the watch build's compiler command line. So a build may sync only with the same compile-affecting properties.
  - The list is `_NScriptWatchPropsHash` in `NScript.WatchProps.targets` (next to Sdk.targets): `DefineConstants`, `TreatWarningsAsErrors`, `WarningsAsErrors`, `WarningsNotAsErrors`, `NoWarn`, `Optimize`, `LangVersion`, `Nullable`, `CheckForOverflowUnderflow`, `AllowUnsafeBlocks`, `DebugType`, `DebugSymbols`.
  - The watch compile sends a hash of them with its request. The daemon keeps it on that project's registration.
  - `--sync` asks with the build's own hash. The daemon answers no when it differs, or is missing: `NScript watch: full build (nscript service: sync no <App>.dll: build properties differ from the watch build ...)`.
  - It also answers no when a project the key reads was registered again with another hash since the key was: `... <Lib>.dll was registered again with other properties`. Example: a `-p:DefineConstants=X` watch build that failed before reaching the app.
  - Examples that build in full: `-p:DefineConstants=X`, `-p:TreatWarningsAsErrors=true`. The next watch build with those values registers them; syncs with them then answer yes.
- **An obj DLL another build rewrote** (such as that `-p:DefineConstants=X` build) is recompiled before the answer.
  - At `--sync` the daemon compares each watched obj DLL with the one it last wrote.
  - For each one that changed or is gone, watch.log says `sync <App>.dll: <Project>.dll rewritten outside watch; recompiling`.
  - The daemon recompiles that project and the projects that use it, re-emits the bundles, then answers. If the build that got yes still compiled locally, the next sync recompiles that project once more. After that, syncs compile nothing.
  - One-time cost: one batch, as for a `.cs` save in those projects. After a full build with other properties, that is every watched project.
- `--sync` waits up to 30 s for a running batch, then answers busy.
- Turn it off: `dotnet build -p:NScriptWatchSync=false`.
- Never syncs: solution (`.sln`) builds, design-time builds, Release-style JS.
- After `--stop` the markers are gone. The next build is a normal build. It regenerates normal JS.

## `nscript service` commands

Use the `nscript.exe` of the toolset your build used. In this repo, Test/Framework projects use `NScriptToolSet/bin/Debug/net8.0/nscript.exe`.

With the NuGet packages, the toolset is the `Mcqdb.NScript.Cs2Jsc` tool, and its `Cs2Jsc` hosts the daemon. Run the commands as `dotnet Cs2Jsc cs2jsc service --status` (or `--stop`), from the folder whose tool manifest pins that version.

| Command | Does | Exit code |
|---|---|---|
| `nscript service --status` | Prints pid, toolset, `LogPath`, `WatchLog`, watched projects and bundles, red projects, `WatchNeedsBuild`, last batch. Resets the idle timer. | 0; 1 = no daemon |
| `nscript service --stop` | Stops after in-flight requests. Deletes the markers. watch.log names each output left `STALE`. | 0; 1 = no daemon |
| `nscript service --stop --force` | Kills a wedged daemon. Only when its lock is held and the pid and start time match. | 0 = killed; 1 = no live daemon |
| `nscript service --sync <obj dll> --build-props <hash>` | What `dotnet build` runs. Without `--build-props` the answer is no. Prints the answer, then `nscript-ref` and `nscript-jsforeign` lines. | 0 yes; 1 no; 2 busy |
| `nscript service --foreground` | Runs a daemon in this console. For debugging. | 0 when it stops |

## Logs

- **watch.log**: one readable line per event.
  - Path: `%LOCALAPPDATA%\NScript\service\<key>\<hash16>.run\watch.log`. `--status` prints it as `WatchLog`.
  - Lines start with words like `register`, `sync`, `change`, `compile`, `patch`, `emit`, `done`, `watcher`, `watch stopped`, `NEEDS BUILD`, `KEPT`, `STALE`.
- **service.jsonl**: structured JSONL (Serilog compact format).
  - Path: `%LOCALAPPDATA%\NScript\service\<key>\service.jsonl`. `--status` prints it as `LogPath`.
  - Events include `WatchChange`, `WatchStep`, `WatchBatchEnd`, `WatchSync`, `WatchForeignOutput`, `WatchOverflow`, `WatchStop`, `WatchStale`.
  - Query it with DuckDB `read_json_auto`.
- `<key>` is 16 hex characters from the toolset folder, the user and elevation.

## Environment variables

- The daemon reads its settings once, at start. To change one: `--stop`, set it, build again.
- A daemon setting it cannot read falls back to its default. One warning names the variable and the value: `ServiceConfigWarning` in service.jsonl, and on the console with `--foreground`.

| Variable | Default | Effect | Kind |
|---|---|---|---|
| `NSCRIPT_SERVICE` | unset | `1` or `true`: csc goes through the service. | Set by Sdk.targets; don't set |
| `NSCRIPT_WATCH` | unset | `1` or `true`: register with the watch. Implies the service. | Set by Sdk.targets and `Sources/Framework/Directory.Build.props`; don't set |
| `NSCRIPT_WATCH_SDKDIR` | unset | The NScript.Sdk folder to watch for `Sdk.props` / `Sdk.targets` edits. | Set by Sdk.targets; don't set |
| `NSCRIPT_WATCH_EVALUATED_UTC_TICKS` | unset | When MSBuild evaluated the project. A file added or a build file written after it stays `NEEDS BUILD`. | Set by Sdk.targets and `Sources/Framework/Directory.Build.props`; don't set |
| `NSCRIPT_WATCH_PROPS_HASH` | unset | The hash of the compile properties (`NScript.WatchProps.targets`). Kept on the registration; `--sync` must match it. Unset: syncs answer no. | Set by `NScript.WatchProps.targets` (imported by Sdk.targets and `Sources/Framework/Directory.Build.props`); don't set |
| `NSCRIPT_SERVICE_IDLE_SECONDS` | `600` | Idle exit, not watching. Positive whole seconds. | Setting |
| `NSCRIPT_WATCH_IDLE_SECONDS` | `28800` (8 h) | Idle exit while watching. Positive whole seconds. | Setting |
| `NSCRIPT_SERVICE_REQUEST_TIMEOUT` | `600` | Per-request watchdog. Positive whole seconds. | Setting |
| `NSCRIPT_SESSION_IDLE_SECONDS` | `1800` | How long an unused JS session stays warm. `0` keeps it until exit. A bad value warns and uses the default. | Setting |
| `NSCRIPT_SERVICE_LOG_LEVEL` | `Information` | Level for service.jsonl: `Verbose`, `Debug`, `Information`, `Warning`, `Error`, `Fatal`. | Setting |
| `NSCRIPT_SERVICE_LOG` | `%LOCALAPPDATA%\NScript\service\<key>\service.jsonl` | Path of service.jsonl. | Setting |
| `NSCRIPT_SYNC_WAIT_SECONDS` | `30` | How long `--sync` waits before busy. Read per build. A bad value prints one line; the build runs in full. | Setting |
| `NSCRIPT_RESOURCE_PATCH` | on | `off`: skin, CSS and XWML saves recompile instead of patching. | Dev-only |
| `NSCRIPT_SESSION` | on in dev mode | `off`: no warm JS session; every emit is cold. | Diagnostic |
| `NSCRIPT_DEV_CHUNKS` | on in dev mode | `off`: no per-method chunks in dev JS. Output is the same. | Diagnostic |
| `NSCRIPT_DEV_CHUNK_INDEX` | off | `1`: also writes `<out>.chunks.tsv` (name, start line, lines). | Diagnostic |
| `NSCRIPT_RAZOR_CACHE` | on | `off`: turns off the Razor skin content cache. | Diagnostic |
| `NSCRIPT_METHOD_CACHE` | on in dev mode | `off`: a warm emit converts every method again instead of replaying unchanged ones. Output is the same. | Diagnostic |
| `NSCRIPT_VERIFY_INCREMENTAL` | off | `1`: after each warm emit, builds the same inputs cold into `%TEMP%\nscript-verify\` and compares .js and .map. A difference prints a warning; service.jsonl logs `VerifyIncremental` with the first differing line. Doubles each warm emit. | Diagnostic |
| `NSCRIPT_LOG_PATH` | unset | Compiler JSONL log path when `--log` is not passed. See README, "Compiler Structured Logging". | Setting |
| `NSCRIPT_LOG_RUNID` | unset | Run id when `--run-id` is not passed. | Setting |
| `NSCRIPT_WATCH_DROP_EVENTS` | unset | `;`-separated extensions (e.g. `.cs`) whose watcher events are ignored. watch.log prints `TEST HOOK`. | **Test hook.** Never set it for real work |

## Known limits

- **No C# symbols after a skin or CSS save.** The patched DLL names no PDB (no CodeView entry), so a .NET debugger loads no symbols for it. The old PDB stays in `obj` and `bin`. The JS and its source maps are unaffected. The next `.cs` save, or any build that is not synced, writes a matching DLL and PDB again.
- **Dev-mode JS:** the daemon writes dev-mode JS; a local build writes normal JS. The file on disk is whichever wrote last.
- **Solution builds skip the sync.** `dotnet build NScript_Full.sln` builds every project as before.
- **A synced build skips the framework projects.** Their commit SHA stamp and the NuGet packages in `NScriptToolSet` are not refreshed. A full build fixes both.
- **Files that arrive with old times:** a file moved in from the same volume, or copied with its times kept, after MSBuild evaluated the project can be missed. The sync may then answer yes without it. Touch the file or run a watch build.
- **A new non-source resource with an old time:** a resource such as a `.png` or `.json` added through a glob, copied with its times kept, can be missed by a synced build after a skin or CSS save. This happens when its time falls between the last watch compile and the patch. Touch the file or run a watch build.
- **After another watch registration:** registering again rewrites the toolset's `lib` DLLs. The next synced build of an app that was already registered then compiles locally once and writes normal JS. The sync after that heals it.
- **Two builds with different properties at once:** the property check runs when `--sync` starts. A watch build with other `-p:` values that registers a referenced project during the sync's wait is not seen by that sync.
- **Windows Defender:** the first read of a freshly written DLL takes about 80 ms under real-time scanning. The second read takes about 2 ms. The service and batch builds both pay it. NScript does not change scan settings.

## Cross-links

- Design: [plans/2026-10-07-incremental-build-service-design.md](../plans/2026-10-07-incremental-build-service-design.md)
- `dotnet build` sync: [plans/2026-10-08-dotnet-build-trigger.md](../plans/2026-10-08-dotnet-build-trigger.md)
- SDK targets: [msbuild-sdk.md](msbuild-sdk.md)

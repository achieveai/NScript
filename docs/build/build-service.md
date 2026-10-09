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

## `dotnet build` while watching

- The daemon keeps a marker, `nscript.watch`, next to each watched project's obj DLL (`obj/<Config>/<TFM>/`).
- A build that finds the marker runs `nscript service --sync <obj dll>` before `BeforeBuild`.
- **Yes** (exit 0): the daemon built this project, its references and its JS.
  - MSBuild skips the project-reference builds and reads the references the daemon vouched for.
  - It keeps the daemon's JS, unless another build rewrote it.
  - Message: `NScript watch: <App> current, skipped <N> project builds`.
- **No** (exit 1), **busy** (exit 2) or no daemon: today's full build.
  - Message: `NScript watch: full build (<reason>)`.
- `--sync` waits up to 30 s for a running batch, then answers busy.
- Turn it off: `dotnet build -p:NScriptWatchSync=false`.
- Never syncs: solution (`.sln`) builds, design-time builds, Release-style JS.
- After `--stop` the markers are gone. The next build is a normal build. It regenerates normal JS.

## `nscript service` commands

Use the `nscript.exe` of the toolset your build used. In this repo, Test/Framework projects use `NScriptToolSet/bin/Debug/net8.0/nscript.exe`.

| Command | Does | Exit code |
|---|---|---|
| `nscript service --status` | Prints pid, toolset, `LogPath`, `WatchLog`, watched projects and bundles, red projects, `WatchNeedsBuild`, last batch. Resets the idle timer. | 0; 1 = no daemon |
| `nscript service --stop` | Stops after in-flight requests. Deletes the markers. watch.log names each output left `STALE`. | 0; 1 = no daemon |
| `nscript service --stop --force` | Kills a wedged daemon. Only when its lock is held and the pid and start time match. | 0 = killed; 1 = no live daemon |
| `nscript service --sync <obj dll>` | What `dotnet build` runs. Prints the answer, then `nscript-ref` and `nscript-jsforeign` lines. | 0 yes; 1 no; 2 busy |
| `nscript service --foreground` | Runs a daemon in this console. For debugging. | 0 when it stops |

## Logs

- **watch.log**: one readable line per event.
  - Path: `%LOCALAPPDATA%\NScript\service\<key>\<hash16>.run\watch.log`. `--status` prints it as `WatchLog`.
  - Lines start with words like `register`, `sync`, `change`, `compile`, `patch`, `emit`, `done`, `NEEDS BUILD`, `KEPT`, `STALE`.
- **service.jsonl**: structured JSONL (Serilog compact format).
  - Path: `%LOCALAPPDATA%\NScript\service\<key>\service.jsonl`. `--status` prints it as `LogPath`.
  - Events include `WatchChange`, `WatchStep`, `WatchBatchEnd`, `WatchStop`, `WatchStale`.
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
| `NSCRIPT_LOG_PATH` | unset | Compiler JSONL log path when `--log` is not passed. See README, "Compiler Structured Logging". | Setting |
| `NSCRIPT_LOG_RUNID` | unset | Run id when `--run-id` is not passed. | Setting |
| `NSCRIPT_WATCH_DROP_EVENTS` | unset | `;`-separated extensions (e.g. `.cs`) whose watcher events are ignored. watch.log prints `TEST HOOK`. | **Test hook.** Never set it for real work |

## Known limits

- **Skin or CSS save, then `dotnet build`:** the build still recompiles the app locally and writes normal (non-dev) JS. The patched DLL has no PDB link. The next build is current. Pending a decision.
- **Dev-mode JS:** the daemon writes dev-mode JS; a local build writes normal JS. The file on disk is whichever wrote last.
- **Solution builds skip the sync.** `dotnet build NScript_Full.sln` builds every project as before.
- **A synced build skips the framework projects.** Their commit SHA stamp and the NuGet packages in `NScriptToolSet` are not refreshed. A full build fixes both.
- **Windows Defender:** the first read of a freshly written DLL takes about 80 ms under real-time scanning. The second read takes about 2 ms. The service and batch builds both pay it. NScript does not change scan settings.

## Cross-links

- Design: [plans/2026-10-07-incremental-build-service-design.md](../plans/2026-10-07-incremental-build-service-design.md)
- `dotnet build` sync: [plans/2026-10-08-dotnet-build-trigger.md](../plans/2026-10-08-dotnet-build-trigger.md)
- SDK targets: [msbuild-sdk.md](msbuild-sdk.md)

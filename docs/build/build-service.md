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

- **Overflow** (too many changes at once): watch.log says `watcher overflow on <folder> (...); rescanning`. The daemon rechecks every input and build file, new ones included, and keeps watching.
- **Any other watcher error** (a watched folder removed, a network share gone): watch.log says `watcher lost <folder> (...); watch stopped`.
  - The daemon stops and deletes its markers. Edits in that folder would go unseen.
  - The next `dotnet build` runs in full. Run `dotnet build -p:NScriptWatch=true` to watch again.
- **A watcher that cannot start** (such as the Linux inotify watch limit): watch.log says `watcher could not start (...); watch stopped, dotnet build runs in full`. No marker is written, so builds do not sync.
- **A DLL another build rewrote** since the daemon wrote it: the next batch logs `foreign <dll> rewritten outside watch; recompiling`. A skin or CSS save then compiles that project instead of patching the other build's DLL.

## Live patching in the open page

With `NSCRIPT_PATCH_EMIT=1` on the daemon, a `.cs` body save reaches the open page without a reload. The page keeps its state: typed text, open panes, objects.

```bash
# Start the daemon with patch emit (stop a running one first: it reads its settings once).
NSCRIPT_PATCH_EMIT=1 dotnet build Test/Framework/TodoApp/TodoApp.csproj -p:NScriptWatch=true
# Serve Test/Framework/TestWebApplication with any static server. Open TodoApp.htm. Edit, save.
```

- **No host code.** Each dev bundle carries a small client after its closure. Any server works: node, IIS, ASP.NET Core, a webpack copy.
  - The bundle's last lines: `/*@L NScript.Hmr*/`, then `__nsHmrBoot("<file>|<bundle id>|<ws url>|<token>|<gen>:<seq>");`.
  - They run in the `finally` of a `try` around the closure. So a bundle whose start throws still has a client: `load failed: <error>; the next build reloads`. It takes no patch; the next save reloads it onto its script.
  - The client connects to the daemon: `ws://127.0.0.1:<port>/nscript-hmr`. It never goes through the page's host.
  - Each patch rewrites `<gen>:<seq>` in the same write as the patch. So any copy of the bundle says which code it holds.
- **A body save that patches** (`patched N methods`): the page runs the new methods at once. Console: `[nscript-hmr] applied <gen>:<seq> (N methods)`.
  - Objects that already exist keep their state and run the new methods.
  - Event handlers bound before the save (`this.OnClick += this.HandleClick`) run the new body: patch-emit bundles bind instance methods late, reading the method at each call. Other bundles bind as before.
  - Code that already ran keeps its old result. A changed constructor or a method with lambdas gives a note: `note: ctor <type>..ctor keeps old code until it runs again`, or `closure ...`.
  - A changed static used as a value (a delegate to a static method, a callback) keeps the function it was given: `note: stored <type>.<method> ...`.
  - A generic type's methods go live too (`List<T>.Add`, `ObservableCollection<T>.Add`). Each instantiation (`List<int>`, `List<string>`) builds its prototype in its own closure of the type's factory.
    - In a patch-emit bundle each factory has a marker, `/*@G <type>*/`, and an eval hook in its closure. The patch goes before the marker, so each later load and new tab runs it.
    - The page runs it in every instantiation made so far (`factory.__nsInsts`) and keeps it for the ones made later (`factory.__nsPatches`).
    - A generic method (`M<T>`), a generic struct or interface, or a body that needs a type the closure has no local for (`T[]` in a type that never used it) emits in full: `new build`.
- **An XWML CSS save** (a colour in a linked `.css`) goes live: `applied <gen>:<seq> (0 methods)`.
  - Each document's style element takes the new CSS. So does a document made later (an iframe), and a new tab reads it from the bundle.
  - In a patch-emit bundle the CSS line reads `DocStorageGetter.__nsCss || "<css>"`, and each style element is kept on `DocStorageGetter.__nsStyles`.
- **An XWML skin save** goes live too, with or without CSS (a class added and used): each open control that shows the skin is skinned again. Console: `reskinned <N> <template>`, then `applied`.
  - Each skin written again takes the next storage index. XWML's stay below Razor's (from 100), which share the per-document DOM cache. The patch that would reach 100 emits in full: `Miss=xwml storage index 100 reaches Razor's`.
  - The control and its view model stay. The DOM inside the skin is made again, so state that lives only in that DOM goes: focus, a caret, text typed and not yet bound, scroll. The page may flash.
  - The skin object stays the same, with the new factory. A list's item skin, or any other holder, makes the new look for items added later.
  - In a patch-emit bundle each skin factory keeps what it made (`DocStorageGetter.__nsTrack`, weak references), and the line `Name_var = null` reads `if (Name_var) DocStorageGetter.__nsReskin(...)`: nothing at load, the re-skin when a patch runs it.
- **A Razor skin save** (`.skin.cshtml`) goes live the same way: `reskinned <N> <template>`. In TodoApp the rows (`TodoItemControl`) and the shell (`AppShell`) are skinned again around the same rows.
  - The skin is written again with its old names and a new storage index.
  - A bundle with no Razor CSS stays without it; a skin save there still goes live.
  - It emits in full (`new build`) when the save adds or removes a skin, changes its `@model`, control or `@styles`, uses a class its CSS does not declare (the build error shows as `build failed`), or calls a method no skin called before (`Miss=razor skin calls <method>`).
- **A Razor CSS save** (a skin's `@styles` sheet) goes live when its classes stay the same: the style element takes the new CSS, and a new tab reads it from the bundle. A class added or removed emits in full (`Miss=razor css classes changed <file>`).
  - In a patch-emit bundle the CSS line reads `DocStorageGetter.__nsRazorCss || "<css>"`, and the style element is kept on `DocStorageGetter.__nsRazorStyle`.
- **A save the watch cannot build keeps the page running**, with no reload. The console says why the page's code is not current, once per new reason:
  - `build failed: TodoItemControl.cs(52,30): error CS1002: ; expected`: the first compiler error. A later save with another error logs the new one.
  - `NEEDS BUILD TodoApp: new file <path>; run dotnet build -p:NScriptWatch=true`: that build reloads the page onto the new code.
  - `kept last good: <why>`: any other reason the watch kept the last good JS, e.g. `copy pending <dll>`.
  - The fix goes live as a patch, with any good edit saved alongside the break. The notice ends with it. A notice that ends with no patch logs `build ok`.
  - A page that opens or reconnects while a notice stands gets it after its hello.
- **Anything else reloads the page by itself**, with the reason on the console (`reloading: <reason>`, then `reloaded: <reason>`):
  - `new build`: any full emit (a miss, a new field or type, compaction, a new daemon).
  - `load-time <method>`: a static constructor, the entry point or a module initializer.
  - `skin`: a skin change a plugin cannot take live. `literal <name>`: a literal the page cannot take live.
  - `delegate <method>`: a method a capturing delegate holds (`StaticInstanceCreate`, generic delegates). The delegate keeps the old function. Rare in dev bundles.
  - `gap`: the page missed a patch.
  - `bundle <gen>:<seq>`: the page found newer code in its script without a message, e.g. after a daemon restart.
  - `live patch failed`: the patch threw in the page. Console: `live patch failed: <error>` first.
  - `publish failed`: the daemon could not hand the patch to the socket. Its next build emits in full.
  - `plain bundle`: the page's script has no live patch hook, e.g. a build without `NSCRIPT_PATCH_EMIT` replaced it.
  - A new method or a new static initializer changes the type's shape: `new build`, not a live patch.
- **The page reloads only when its own script holds other code.** It fetches its script again (`cache: "no-cache"`) and reads the stamp.
  - The copy is still old (a webpack copy that lags): `waiting for bundle <gen>`. It tries again with backoff, 1 s to 30 s, paused while the tab is hidden.
  - A fetch error, a non-200 or an HTML page: `self-check failed: <status>`. It waits too.
  - At most 2 automatic reloads in 60 s. Then `reload-only: 2 reloads in 60 s; live patches still apply`. The socket stays; the first reload after the 60 s goes through.
- **The daemon stops or restarts:** `disconnected; reconnecting in Ns`, then `cannot reach the build service; retrying` (state `disconnected`). The next watch build has a new port, token and gen; the page finds it at its next check (up to 30 s) and reloads onto it.
- **Turn it off for one browser:** `localStorage.nscriptHmr = "off"`, then reload. Console: `off (localStorage)`.
- **State in the page:** `window.__nsHmr["<file>"]` has `gen`, `seq`, `baseSeq` (the stamp at load), `applied`, `notes`, `note` (the notice that stands, or null), `lastReload`, `failed` (the error the closure threw while it loaded), `state` (`connected`, `disconnected`, `waiting`, `off`, `reload-only`).
- **The daemon side:** watch.log `hmr listening ws://127.0.0.1:<port>/nscript-hmr`, `hmr connect <file> at <gen>:<seq> clients=N`, `hmr send <file> <gen>:<s>..<t> clients=N`, `hmr reload <file> <reason> clients=N`, `hmr note <file> <note|cleared> clients=N`, `hmr close <file> clients=N`. service.jsonl `Hmr*` events. `--status` prints `HmrUrl`, without the token.
- **Security:** the socket listens on 127.0.0.1 only, refuses any other `Host` (a DNS-rebinding page), and needs the daemon's 128-bit token, which the bundle holds. Any site can load a bundle with a script tag and read that token, so a browser page must also come from a local origin: a loopback host, `*.localhost`, `*.test`, or one `NSCRIPT_HMR_ORIGINS` lists; any other `Origin` (`null` too) gets 403. It is read-only: it ignores what pages send. At most 16 pages at once. **Never deploy a `NSCRIPT_PATCH_EMIT` bundle**: it carries the token.
- A bundle built with `NSCRIPT_PATCH_EMIT=1` outside the daemon has no socket url. Its client only checks its script, and reloads when it changes.
- Chromium (Chrome, Edge) is what was tested. Firefox is best effort.
- **Tests:** `Test/Framework/TestWebApplication/hmr-client.test.mjs` (the client in a fake page, part of `npm run test:unit`) and `HmrServerTests` (the socket). `node e2e-hmr.mjs` in that folder runs the save-to-page path for real (a method body, then an event handler on old and new rows, then a generic method on old and new instantiations and a new tab); it needs the watch above running and exits 2 (skipped) without it. `node e2e-hmr-css.mjs` does the same for an XWML CSS save on the UI test page; it needs the watch on `Test/Framework/Sunlight.Framework.UI.Test`. `node e2e-hmr-skin.mjs` saves an XWML skin there (an element added, then a class added and used) and checks the open control, a held skin and a new tab. `node e2e-hmr-razor-skin.mjs` saves TodoApp's Razor skins (a row, the shell) and `AppShell.css`, with the TodoApp watch running. `node e2e-hmr-notice.mjs` breaks a TodoApp build (a syntax error, then a type error), fixes it, and adds a file, with the TodoApp watch running. `node e2e-hmr-load-error.mjs` makes TodoApp's Main throw, checks the client still boots, then saves the fix, with the TodoApp watch running.

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
  - Also the map roots: `SourceMapRoot`, `NScriptRepoRoot`, `NScriptSecondarySourceMapRoot`, `NScriptSecondaryRepoRoot`. The daemon writes the JS and map with the watch build's roots, and a synced build keeps them. Only `-p:` and project values count. A repo-linked root that a target computes from git later in the build is not seen.
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
| `nscript service --status` | Prints pid, toolset, `LogPath`, `WatchLog`, watched projects and bundles, red projects, `WatchNeedsBuild`, last batch, `HmrUrl` (the live patch socket and its pages, or `off`). Resets the idle timer. | 0; 1 = no daemon |
| `nscript service --stop` | Stops after in-flight requests. Deletes the markers. watch.log names each output left `STALE`. | 0; 1 = no daemon |
| `nscript service --stop --force` | Kills a wedged daemon. Only when its lock is held and the pid and start time match. | 0 = killed; 1 = no live daemon |
| `nscript service --sync <obj dll> --build-props <hash>` | What `dotnet build` runs. Without `--build-props` the answer is no. Prints the answer, then `nscript-ref` and `nscript-jsforeign` lines. | 0 yes; 1 no; 2 busy |
| `nscript service --foreground` | Runs a daemon in this console. For debugging. | 0 when it stops |

## Logs

- **watch.log**: one readable line per event.
  - Path: `%LOCALAPPDATA%\NScript\service\<key>\<hash16>.run\watch.log`. `--status` prints it as `WatchLog`.
  - Lines start with words like `register`, `sync`, `change`, `compile`, `patch`, `emit`, `done`, `hmr`, `watcher`, `watch stopped`, `NEEDS BUILD`, `KEPT`, `STALE`.
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
| `NSCRIPT_HMR_ORIGINS` | unset | Comma-separated page origins (`https://dev.example.com`) that may open the live patch socket, besides loopback, `*.localhost` and `*.test`. Read when the daemon starts. | Dev-only |
| `NSCRIPT_SERVICE_LOG_LEVEL` | `Information` | Level for service.jsonl: `Verbose`, `Debug`, `Information`, `Warning`, `Error`, `Fatal`. | Setting |
| `NSCRIPT_SERVICE_LOG` | `%LOCALAPPDATA%\NScript\service\<key>\service.jsonl` | Path of service.jsonl. | Setting |
| `NSCRIPT_SYNC_WAIT_SECONDS` | `30` | How long `--sync` waits before busy. Read per build. A bad value prints one line; the build runs in full. | Setting |
| `NSCRIPT_RESOURCE_PATCH` | on | `off`: skin, CSS and XWML saves recompile instead of patching. | Dev-only |
| `NSCRIPT_SESSION` | on in dev mode | `off`: no warm JS session; every emit is cold. | Diagnostic |
| `NSCRIPT_DEV_CHUNKS` | on in dev mode | `off`: no per-method chunks in dev JS. Output is the same. | Diagnostic |
| `NSCRIPT_DEV_CHUNK_INDEX` | off | `1`: also writes `<out>.chunks.tsv` (name, start line, lines). A patched emit does not rewrite it, so its lines go stale until the next full emit. | Diagnostic |
| `NSCRIPT_RAZOR_CACHE` | on | `off`: turns off the Razor skin content cache. | Diagnostic |
| `NSCRIPT_METHOD_CACHE` | on in dev mode | `off`: a warm emit converts every method again instead of replaying unchanged ones. Output is the same. | Diagnostic |
| `NSCRIPT_PLUGIN_METHOD_CACHE` | on | `off`: the method cache stops replaying plugin conversions that say they can be replayed ([AutoFire] accessors); they convert every build. Output is the same. | Diagnostic |
| `NSCRIPT_XWML_REUSE` | on in dev mode | `off`: a warm emit parses every XWML template again instead of reusing the last parse. Reuse needs every template and style sheet unchanged; a new request order retries the build once (service.jsonl `Builder.Retry Reason=xwml-document-order`). `Probe.XwmlInit` logs `Reused` and `ReuseMiss`. Output is the same. | Diagnostic |
| `NSCRIPT_PATCH_EMIT` | off | `1`: after a body-only `.cs` save, a dev-mode emit writes only the changed methods into the last bundle, before each type's `/*@T <type>*/` marker line (a generic type's: its factory's `/*@G <type>*/` line), and moves the map's lines to match. watch.log shows `(bodies-refreshed; patched N methods)`. A method the bundle never emitted is added the same way when a changed body calls it. Anything it cannot patch (a type or field the bundle never emitted, a new virtual call, a generic method, a type a generic factory's closure has no local for, a skin's inlined getter or setter) emits in full: service.jsonl `Patch.Emit Patched=False Miss=<reason>`. A skin, CSS or XWML save patches too (`resources-refreshed`): only the changed XWML documents are written again, before the `/*@S XwmlParser*/` marker line, with the same function names and CSS class names; when the page's CSS changed it is written again in place, as the string on the line after the `/*@L XwmlParser.Css*/` marker, so the bundle does not grow (dev builds prefix each sheet apart, so only a changed sheet is prefixed again), and the new skin takes a storage index past the old ones. Razor skins work the same way, before `/*@S RazorSkinParser*/`, and their CSS after `/*@L RazorSkinParser.Css*/`. A `.cs` save and a skin or CSS save in one batch patch both. A change a full build would refuse (a class removed or renamed, a template removed) emits in full and reports the error. A patched bundle runs like a cold one but is not byte-equal to it; a full emit starts over. Dev bundles also register `window.__nsPatch["<file>.js"]`, which evals a fragment inside the bundle's closure, and carry the live patch client (see "Live patching in the open page"). Read when the daemon starts. | Dev-only |
| `NSCRIPT_PATCH_COMPACT_PERCENT` | `50` | With `NSCRIPT_PATCH_EMIT`: once patches grow the bundle more than this percent past its last full emit, the next save emits in full (`Miss=compaction`), dropping the old code. Whole number, `0` or more. Read per build. | Setting |
| `NSCRIPT_VERIFY_INCREMENTAL` | off | `1`: after each warm emit, builds the same inputs cold into `%TEMP%\nscript-verify\` and compares .js and .map. A difference prints a warning; service.jsonl logs `VerifyIncremental` with the first differing line. Doubles each warm emit. A patched emit (`NSCRIPT_PATCH_EMIT`) is not meant to equal a cold build, so it always reports a difference. | Diagnostic |
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
- **Same user, not elevated.** The supported use is the user's own non-elevated session. The run folder and the shadow toolset are not guarded against another process of the same user.
  - `nscript service --stop --force` kills the pid in `daemon.pid` when the daemon lock is held and the start time matches. It does not prove that pid holds the lock. Do not run it elevated.
- **Signed DLLs:** a skin or CSS save in a strong-name signed project compiles instead of patching. The patch cannot re-sign the DLL. Authenticode-signed outputs are not supported while watching.
- **Linux is untested.** The watch was tested on Windows only. Review reports that on some .NET 8 runtimes a non-recursive Linux watcher stops after an inotify overflow. The daemon rescans, but does not restart that watcher.
- **Windows Defender:** the first read of a freshly written DLL takes about 80 ms under real-time scanning. The second read takes about 2 ms. The service and batch builds both pay it. NScript does not change scan settings.

## Cross-links

- Design: [plans/2026-10-07-incremental-build-service-design.md](../plans/2026-10-07-incremental-build-service-design.md)
- `dotnet build` sync: [plans/2026-10-08-dotnet-build-trigger.md](../plans/2026-10-08-dotnet-build-trigger.md)
- SDK targets: [msbuild-sdk.md](msbuild-sdk.md)

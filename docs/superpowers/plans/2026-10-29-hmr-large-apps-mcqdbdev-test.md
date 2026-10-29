# Handoff: test #110 / #111 on MCQdbDEV

Branch `hmr-large-app-live-patch` on origin, off master a872aa23. This file ships on that branch.

## What changed
- **#110:** no 100-skin cap. XWML (`stateStore`) and Razor (`razorStore`) keep separate per-page caches, both from 0. CSS and skin saves patch live in big apps.
- **#111 leaks fixed:** a skin disposed before its delayed setup no longer listens to its view model (B1). A disposed XWML binding now removes its last listener (B6, old off-by-one in `LiveBinder.CleanRegistrations`).
- **Console:** `reskinned <N> <FileName> (reset: focus, caret, scroll, unbound typed text)`. Short file name, not the path. No note when N = 0.

## Proven locally (generated 650-skin app + suites)
- CSS save live in 150-200 ms, 220 KB sheet included. Skin saves redraw. No reloads. New tabs right.
- All suites green: QUnit x5, e2e-todo 55/55, e2e-sheet 11/11, bench --check OK, e2e-hmr-css/skin/razor-skin. Pre-commit hook 1071/0.

## Run on MCQdbDEV
### 0. Setup
- Build this branch's NScript the way that machine consumes it. NuGet: wipe `NScriptToolSet/bin` before packing (stale binaries).
- `nscript.exe service --stop` on any old daemon. Start the watch with `NSCRIPT_PATCH_EMIT=1`, or the page only reloads.
- Optional: the dev bundle has 2 `function DocStorageGetter` (XWML + Razor). `razorStore` appears in Razor's.

### 1. #110: `.less` colour save
- Edit a colour in `FeatureExploreView.less` (gulp → `.css`, ~7 s, out of scope).
- Expect console `[nscript-hmr] applied <gen>:<seq> (0 methods)`. No `reloading:`. No `reskinned` (pure CSS).
- Expect service.jsonl `Patch.Emit Patched=true`, no `Miss`. The text `reaches Razor's` no longer exists.
- Record: save → colour change ms, `Probe.XwmlPatch ReadMs/ParseMs/CssMs`, largest `<style>` length, largest WS frame vs 1,048,576.

### 2. Why the index rose 656 → 661 on CSS saves
- Note the time, save, then list files gulp rewrote after it (`*.html, *.css, *.less`).
- `Probe.XwmlPatch Changed=` should match. Changed > 1 means gulp rewrites several files.
- A `reskinned` line on a CSS-only save names a document parsed again. List them.
- A new or renamed `.css` from gulp: expect `Miss="xwml resource-set"` and a reload. Separate issue if seen.

### 3. #111: skin `.html` save
- Edit text in `FeatureExploreView.html`. Expect `reskinned <N> FeatureExploreView (reset: ...)`, then `applied`. No reload.
- By hand, screenshot each:
  - `FeatureItemVerticalV4` rows in a `ListView`: old rows redraw; a new row uses the new skin.
  - Inner + outer saved together: both redraw. Known: the inner is built once extra (B4, backlog).
  - Bound skins: `ItemSkin="{...}"` list and a `SelectBoxSkin` select. Open, pick an item.
  - An open popup/dialog redraws now; a hidden one when shown.
  - A skin with 0 live views: `reskinned 0 <name>`, no error.
- After each: no console error. Controls that cache parts (`ApplySkinInternal`, `GetChildById`, `FindElement`): click into them after a re-skin. A dead part = B2.
- Leak check: heap snapshot after 5 saves of a list skin. Detached old roots should not grow per save.

### 4. Report back
- Per row: pass / fail / blocked, with console lines, service.jsonl rows, screenshots.

## Known limits / backlog
- **B4:** nested inner+outer save builds the inner skin one extra time. Screen correct, dev only. Fix needs a two-phase re-skin flush across both plugins (design in the follow-up).
- **C-001 (pre-existing):** a deactivated (not disposed) Razor graph skin still wires on its queued activation.
- **B5 (pre-existing):** `ListView.cs:136` HeaderSkin with a FixedList dereferences a null list.
- **Replay window:** with a 220 KB sheet, ~4 CSS saves fill it; a tab reconnecting after that reloads.
- **Two bundles on one release page** may still share a cache field by minified name. Co-loading bundles is already unsupported (issue #51).
- Two templates with the same file name in different folders log the same `reskinned` name.

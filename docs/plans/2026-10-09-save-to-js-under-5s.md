# Plan: every MCQdbDev save reaches JS in 4–5 s

**Done when:**
- The MCQdbDev NeetPG stress loop (n=10 each for .cs, skin and LESS, host load under 50%) gives a save→JS median of 4.5 s or less for each kind, and a p90 of 6 s or less.
- Every incremental emit in that loop is byte-identical to a full dev-mode emit of the same inputs (verify mode, below).
- All PR gates are green: QUnit, e2e-todo, e2e-sheet, bench --check and the compiler tests.

## Where the time goes today (warm daemon, ~30% host load)

| Save | Compile | Emit | Biggest emit parts |
|---|---|---|---|
| .cs | 6.0 s (McqdbClient) | 10 s, cold | converts 13,054 methods (5 s), XWML 1.9 s, new ConverterContext 0.8 s, writer 1 s |
| skin / LESS | 0.7 s patch | 9.2 s, warm | converts 13,054 methods (4.5 s), XWML 1.8 s, writer 1 s |

Every save converts every method. That is the core problem.

## Key dots

1. **Method cache (stage 2).** The warm session caches each method's JS text and the symbols it resolved, keyed by method and body hash. A cache hit replays those resolves, so the walk and the names stay the same, and writes the cached text. Only changed methods convert. Target: convert 7 s → under 1.5 s.
2. **Writer reuse.** Unchanged functions reuse their rendered text and source-map segments. Target: writer 1 s → 0.3 s.
3. **The session survives a body-only .cs change.** If a recompiled DLL has the same declaration surface (types, members, signatures, attributes, constants), the session keeps its Cecil modules and swaps in only the new method bodies. There is no cold rebuild. Target: cold → warm, saving 0.8 s.
4. **Fast stage 1 for body-only .cs edits.** The daemon keeps a live compilation per project. It rebinds only the methods in the changed files, using Roslyn's internal method filter, which the fork's InternalsVisibleTo already exposes. It serializes just those bodies, and JS goes out first. The full compile that rewrites the DLL runs afterwards and is cancelled by the next save. Target: 6 s → under 1 s before JS.
5. **XWML template cache**, only if 1–4 leave skin saves above 4 s. Templates are cached by content, the same pattern as the Razor and CSS caches.

Any case the fast path does not cover falls back to today's full path. That includes a surface change, an added or removed file, a new anonymous type, compile errors, and a changed method that pulls in code not yet in the bundle.

## Proof
- **Verify mode** (`NSCRIPT_VERIFY_INCREMENTAL=1`): after each incremental emit, the daemon runs a full emit in memory and logs `VerifyMismatch` with the first differing line. The stress loop runs with it on and must show zero mismatches.
- Scenario tests on TodoApp:
  - body edit;
  - body edit that calls a new method;
  - signature change;
  - new file;
  - skin edit;
  - CSS edit.
- Each test checks the emit kind (incremental or full) and checks the JS against a full build.
- MCQdbDev n=10 numbers, with host load recorded, as in this round.

## Material risks
- **Hidden side effects of conversion.** A method's conversion may do more than resolve symbols, for example register helpers or record errors. A replayed hit would miss them. Verify mode catches this; each such case is fixed or forced to a cache miss.
- **Dev-mode fallback names** (6,409 on NeetPG) must be stable across builds. If not, those methods are never cached.
- **Size.** This is the deferred slice 2 plus a stage-1 fork use. Expect several days of work. I will build it in the order 1 → 2 → 3 → 4 (→ 5), measure after each, and report each milestone.
- **Stale IL.** Until the background compile finishes, the DLL on disk holds the old IL. A synced `dotnet build` in that window waits for the compile.

**Now / next:** a spike of the method cache on TodoApp with verify mode, then MCQdbDev.

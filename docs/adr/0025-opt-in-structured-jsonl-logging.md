# ADR 0025: Opt-in Structured JSONL Logging for the Core Pipeline

- Status: Accepted
- Date: 2026-04-16
- Deciders: NScript maintainers
- Technical Area: Compiler tooling / observability

## Context

The NScript compiler is a two-stage pipeline: Stage 1 (`csc`) emits an assembly
with an embedded AST resource, and Stage 2 (`cs2jsc`) converts that AST into
JavaScript (see ADR 0006). Each stage logs primarily through `Console.WriteLine`
and stage-local facilities. `RazorSkinParser` had a bespoke Serilog sink writing
to a hard-coded file path (`logs/razor-skin-compiler.log.jsonl`), with no
coordination with the rest of the pipeline.

Debugging compilation issues often requires correlating events across stages and
components. Unstructured console lines make this hard, and the Razor sink being
separate means its events cannot be interleaved with other pipeline events on
the same timeline.

## Decision Drivers

- produce machine-parseable logs that can be interleaved across stages
- keep default behavior (no logging, no file I/O) unchanged for existing users
- avoid adding a second logging framework — reuse the Serilog dependency already
  present in `RazorSkinParser`
- enable cross-process (csc + cs2jsc) correlation via a shared run id
- consolidate the orphan `RazorSkinParser` sink under the same facility

## Options Considered

### Option 1: Shared `CompilerLog` static in `NScript.Utils` wrapping Serilog

Introduce a `CompilerLog` static type in `NScript.Utils` that wraps Serilog with
a `CompactJsonFormatter` and a shared-append `WriteTo.File` sink. Both stage
entry points parse a `--log <path>` flag (and optional `--run-id <id>`), call
`CompilerLog.Initialize` before work, and `CompilerLog.Shutdown` on exit. Each
component fetches a per-component logger via `CompilerLog.ForComponent("X")`.
When `Initialize` is not called, `ForComponent` returns a silent no-op logger.

Pros:

- reuses existing Serilog dependency — no new framework
- shared file-sink + run id enables cross-process correlation
- opt-in by construction — no file I/O when the flag is omitted
- consolidates the Razor sink

Cons:

- introduces a global mutable singleton (mitigated by idempotent `Initialize`)

### Option 2: Microsoft.Extensions.Logging with a custom JSONL provider

Use MEL's `ILogger` abstraction and ship a custom JSONL sink.

Pros:

- aligns with ASP.NET/host conventions

Cons:

- adds a second logging framework alongside Serilog
- requires writing a JSONL formatter we would otherwise get from Serilog

### Option 3: Hand-rolled JSON writer

Write a minimal JSON sink with no logging library.

Pros:

- zero new dependencies

Cons:

- reinvents enrichment, file-sink locking, and schema evolution

## Decision

Adopt **Option 1**. Add `CompilerLog` in `NScript.Utils`, wire it into both
stage entry points (`csc` and `cs2jsc`), and route `RazorSkinCompiler.Logger`
through it. Emit structured events from `Builder`, `ConverterContext`,
`XwmlTemplatingPlugin`, and `SerializationHelper`.

CLI surface:

- `--log <path>` (and `-log <path>`, `--log:path`, `--log=path`) enables logging
- `--run-id <id>` (and `-runid`, `--run-id:id`) sets the cross-process run id
- `NSCRIPT_LOG_PATH` and `NSCRIPT_LOG_RUNID` env vars are honored as fallbacks
  for MSBuild response-file scenarios

Each JSONL entry carries `@t`, `@l`, `Component`, `Stage`, `RunId`, `Pid`, and
`MachineName` alongside the event-specific fields.

## Consequences

Positive:

- deterministic, grep-able, timestamped log stream across both compiler stages
- Razor/XWML events now live on the same timeline as Builder/Converter events
- behavior unchanged when the flag is omitted

Negative:

- `RazorSkinParser`'s previous hard-coded `logs/razor-skin-compiler.log.jsonl`
  file is no longer written. Consumers that relied on that path must now pass
  `--log` (or set `NSCRIPT_LOG_PATH`). Release notes call this out.

## References

- Issue #10 — Add opt-in structured JSONL logging to the NScript core pipeline
- ADR 0006 — Two-stage compiler pipeline (source of the cross-stage correlation need)

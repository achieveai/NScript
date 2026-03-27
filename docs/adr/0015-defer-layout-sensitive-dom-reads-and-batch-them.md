# ADR 0015: Defer Layout-Sensitive DOM Reads and Batch Them

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: DOM interop and layout safety

## Context

NScript exposes DOM and other native browser APIs through attributed CLR facades in the `System.Web.Html` and related framework namespaces.

The current framework already exposes many DOM properties and methods directly, including layout-sensitive reads such as:

- `ClientWidth`, `ClientHeight`, `OffsetWidth`, `OffsetHeight`, `ScrollWidth`, and `ScrollHeight` on [Sources/Framework/System.Web.Html/Element.cs](../../Sources/Framework/System.Web.Html/Element.cs)
- `GetBoundingClientRect()` on [Sources/Framework/System.Web.Html/Element.cs](../../Sources/Framework/System.Web.Html/Element.cs)
- `GetComputedStyle()` on [Sources/Framework/System.Web.Html/Window.cs](../../Sources/Framework/System.Web.Html/Window.cs)

The framework also already has scheduling primitives that can defer work to the next visual turn:

- global `requestAnimationFrame` exposure in [Sources/Framework/System.Web/Globals.cs](../../Sources/Framework/System.Web/Globals.cs)
- higher-level scheduling wrappers in [Sources/Framework/Sunlight.Framework/TaskScheduler.cs](../../Sources/Framework/Sunlight.Framework/TaskScheduler.cs)

The architectural problem is that synchronous reads of layout-sensitive DOM properties can force style or layout recalculation when interleaved with writes. If ordinary DOM wrappers expose those reads as just another synchronous property access, application and generated code can unintentionally trigger layout thrashing and reflow.

That is a correctness and performance issue large enough to standardize at the architecture level.

## Decision Drivers

- prevent accidental forced layout and layout thrashing
- keep DOM writes predictable and cheap from the caller's point of view
- preserve the ability to read geometry and computed layout when needed without making every caller reason about browser reflow timing
- give generated UI code a safe default interop model

## Options Considered

### Option 1: Allow synchronous reads and writes uniformly on DOM wrappers

Expose layout-sensitive DOM properties the same way as ordinary fields or getters.

Pros:

- simplest object model
- closest to raw browser API shape

Cons:

- easy to trigger forced layout accidentally
- encourages interleaving reads and writes that degrade rendering performance
- makes generated and handwritten UI code fragile under optimization pressure

### Option 2: Keep synchronous writes, but require deferred batched access for layout-sensitive reads

Expose write-oriented DOM APIs directly, while routing layout-affecting reads through an asynchronous measurement callback model that batches reads together.

Pros:

- prevents unintentional synchronous layout reads in normal code paths
- preserves access to measured values when needed
- aligns naturally with browser frame scheduling through `requestAnimationFrame`

Cons:

- introduces asynchronous APIs for some DOM access patterns
- differs from the raw browser API surface

### Option 3: Hide almost all DOM APIs behind a virtual DOM or retained rendering abstraction

Avoid direct DOM exposure and require higher-level rendering models.

Pros:

- strongest protection against accidental layout work

Cons:

- far beyond the current NScript framework model
- removes the direct host interop that the framework already uses heavily

## Decision

NScript will standardize on a split DOM exposure model:

- DOM and native operations that are write-oriented or otherwise layout-safe may remain directly exposed
- layout-sensitive DOM reads must not be exposed as ordinary synchronous reads for general consumption
- instead, layout-sensitive reads must be requested through a callback-based measurement API that defers execution and batches all reads together

The measurement pipeline is:

1. caller requests one or more layout-sensitive property or method reads
2. NScript records those requested reads rather than executing them immediately
3. the runtime schedules a measurement pass on `requestAnimationFrame`, or a tiny timeout fallback if frame scheduling is unavailable
4. during that scheduled pass, NScript performs the queued reads together
5. after the batch is complete, NScript invokes the queued callbacks with the measured values

This makes deferred measurement the default architectural contract for layout-sensitive DOM access.

## Scope of Layout-Sensitive Reads

This rule applies to reads that may force style or layout calculation, including but not limited to:

- element client, offset, and scroll dimension getters
- geometry APIs such as `GetBoundingClientRect()`
- computed-style queries when used for layout-dependent values

Not every native read is layout-sensitive. Pure data access and write operations are not subject to this batching rule unless a later ADR extends it.

## Native Exposure Policy

For native browser facades more generally:

- keep direct exposure for operations that are semantically writes, event hookup, attribute mutation, DOM tree mutation, or other non-measurement actions
- treat measurement and layout observation as a special asynchronous capability, not as ordinary field access
- do not add new synchronous layout-read wrappers casually, even if the raw browser API is synchronous

Existing synchronous wrappers for layout-sensitive properties should be treated as legacy exposure and should not define future API direction.

## Scheduling Contract

The default scheduling target for measurement batches is `requestAnimationFrame`, as already exposed in [Sources/Framework/System.Web/Globals.cs](../../Sources/Framework/System.Web/Globals.cs) and wrapped by [Sources/Framework/Sunlight.Framework/TaskScheduler.cs](../../Sources/Framework/Sunlight.Framework/TaskScheduler.cs).

If `requestAnimationFrame` is unavailable, NScript may use a small timeout fallback. The important architectural requirement is batching and deferral, not a specific timer primitive.

## Consequences

Positive:

- accidental forced layout becomes harder to trigger from framework-level DOM usage
- DOM write paths stay simple while measurement becomes explicitly scheduled
- generated UI code can preserve DOM integrity and render performance more reliably

Negative:

- some DOM access patterns become asynchronous even though the browser API is synchronous
- wrapper APIs for geometry and layout need additional runtime machinery
- existing synchronous layout getters become architectural debt to contain or replace over time

## References

- [docs/adr/0010-model-native-javascript-types-through-attributed-clr-facades.md](0010-model-native-javascript-types-through-attributed-clr-facades.md)
- [Sources/Framework/System.Web.Html/Element.cs](../../Sources/Framework/System.Web.Html/Element.cs)
- [Sources/Framework/System.Web.Html/Window.cs](../../Sources/Framework/System.Web.Html/Window.cs)
- [Sources/Framework/System.Web.Html/ClientRect.cs](../../Sources/Framework/System.Web.Html/ClientRect.cs)
- [Sources/Framework/System.Web/Globals.cs](../../Sources/Framework/System.Web/Globals.cs)
- [Sources/Framework/Sunlight.Framework/TaskScheduler.cs](../../Sources/Framework/Sunlight.Framework/TaskScheduler.cs)
- [Sources/Framework/Sunlight.Framework.UI/Helpers/SkinBinderHelper.cs](../../Sources/Framework/Sunlight.Framework.UI/Helpers/SkinBinderHelper.cs)
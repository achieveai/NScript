# Framework Web & DOM

> **Audience:** *App authors*.

## TL;DR

`System.Web` and `System.Web.Html` are NScript's typed bindings to the browser environment. `System.Web` covers the global helpers (`encodeURIComponent`, `JSON`, timers, `XMLHttpRequest`, `WebSocket`, `Performance`); `System.Web.Html` covers the DOM (`Document`, `Element`, all common element subclasses, events). Both are thin façades modelled via the `[ImportedType]` / `[Extended]` attribute pattern (see [ADR 0010](../adr/0010-model-native-javascript-types-through-attributed-clr-facades.md) and [interop/json-and-imported-types.md](../interop/json-and-imported-types.md)).

## Reference — `System.Web`

| Type | Purpose |
|---|---|
| `Globals` | `[GlobalMethods]` static class. Hosts `EncodeURIComponent`, `DecodeURIComponent`, `EncodeURI`, `DecodeURI`, `SetTimeout`, `ClearTimeout`, `SetInterval`, etc. |
| `Window` | The browser window. Lives in `System.Web.Html`. |
| `JSON` | `JSON.Parse(string)` / `JSON.Stringify(object)` — the standard JS `JSON` global with .NET-shaped names. |
| `Blob`, `URL` | Standard `Blob` and `URL` constructors. `URL.CreateObjectURL`, `RevokeObjectURL`. |
| `WebSocket` | Browser WebSocket API. |
| `XMLHttpRequest` | XHR with `ReadyState` enum, `OnReadyStateChange`, `Open`, `Send`, response props. |
| `Performance` | `performance.now()` and the timing API. |

## Reference — `System.Web.Html`

| Type | Purpose |
|---|---|
| `Document` | Static-style accessors for the `document` global: `GetElementById`, `CreateElement`, `Body`, `QuerySelector`, etc. |
| `Element` (base) | Common DOM element surface: `InnerHTML`, `ClassName`, `Style`, `AddEventListener`, attribute getters / setters, `OffsetWidth`, `OffsetHeight`, `GetBoundingClientRect`, etc. |
| Element subclasses | `DivElement`, `InputElement`, `AnchorElement`, `ImageElement`, `CanvasElement`, `TableElement` / `TableRowElement` / `TableCellElement`, `FormElement`, `SelectElement`, `OptionElement`, `TextAreaElement`, `IFrameElement`, `MediaElement`, `AudioElement`, `VideoElement`, `ScriptElement`, `MapElement`, `AreaElement` |
| Events | `ElementEvent` (base), `MutableEvent`, `MessageEvent`, `ErrorEvent`, `TouchEvent`, `WheelEvent`, `GestureEvent`, `CustomEvent` |
| Form / file APIs | `FormData`, `FileInput`, `File`, `FileReader`, `DataTransfer` |
| Layout | `ClientRect`, `Style`, `TokenList`, `NodeCollection`, `DomList<T>`, `Selection`, `Orientation` |
| Geo / graphics | `GeoLocation/`, `Graphics/` (canvas helpers) |

The full file list is under `Sources/Framework/System.Web/` and `Sources/Framework/System.Web.Html/`.

## Quick start

### Mutate the DOM

```csharp
using System.Web.Html;

var container = Document.GetElementById("app");
container.InnerHTML = "<h1>Hello!</h1>";
container.ClassName = "ready";
```

### Listen to events

```csharp
using System.Web.Html;

var btn = Document.GetElementById("save");
btn.AddEventListener("click", evt =>
{
    evt.PreventDefault();
    DoSave();
});
```

### Fetch JSON via XHR

```csharp
using System.Web;

void LoadItems(Action<string[]> onSuccess)
{
    var xhr = new XMLHttpRequest();
    xhr.Open("GET", "/api/items", true);
    xhr.OnReadyStateChange = () =>
    {
        if (xhr.ReadyState != ReadyState.Done) return;
        if (xhr.Status != 200) return;
        var ids = (string[])JSON.Parse(xhr.ResponseText);
        onSuccess(ids);
    };
    xhr.Send(null);
}
```

### Cross the EventBinder boundary

DOM events on `Element` subclasses route through `Sunlight.Framework.EventBinder`, which is what `[CallContext]` uses to root a new ambient context per user gesture. If you bypass `Element.AddEventListener` (e.g. by using `[Script]` to hand-roll `addEventListener`), call-context propagation may break. Always prefer the typed `AddEventListener` overload. See [ADR 0014](../adr/0014-standardize-the-observable-framework-as-the-reactive-binding-contract.md).

## Examples

### Use `Globals` static helpers

```csharp
using System.Web;

string url = Globals.EncodeURI("/path with spaces?x=1");
int handle = Globals.SetTimeout(() => Console.WriteLine("tick"), 100);
Globals.ClearTimeout(handle);
```

`Globals` is annotated `[GlobalMethods]`, so calls compile to bare JS function calls (`encodeURI(...)`) rather than `Globals.encodeURI(...)`.

### Read layout-sensitive measurements

Layout reads (`OffsetWidth`, `GetBoundingClientRect`, …) trigger forced reflow if interleaved with writes. Batch them via the `Sunlight.Framework.LayoutBatcher` async-property pattern when reading at scale; see [ADR 0015](../adr/0015-defer-layout-sensitive-dom-reads-and-batch-them.md) and the `framework/sunlight-core.md` section on `LayoutBatcher`.

### File upload via `FormData`

```csharp
var form = new FormData();
form.Append("file", fileInput.Files[0]);
form.Append("description", "screenshot");

var xhr = new XMLHttpRequest();
xhr.Open("POST", "/upload", true);
xhr.Send(form);
```

## Known gotchas

### `Element` is not `[JsonType]`

`Element` and its subclasses are `[ImportedType]` façades — they wrap real native DOM objects. Do not pass them through `structuredClone`, `JSON.stringify`, IndexedDB `put()`, or any path that expects a plain data record. Use a `[JsonType]` DTO for serialisable data; see [interop/json-and-imported-types.md](../interop/json-and-imported-types.md).

### Property access compiles to property access — no defensive nulls

`element.InnerHTML = ...` is a direct property write. If `element` is `null` (the lookup failed), you'll get a TypeError at runtime, not a `NullReferenceException`. Consider `if (element != null)` or use `Document.QuerySelector` and check the result.

### Event handler `this` in lambdas

C# lambdas capture the outer `this`. The compiler binds DOM event handlers so the lambda's `this` is *not* the element that fired the event. If you need the firing element, read `evt.Target` (or the more specific `evt.CurrentTarget`).

### `CustomEvent` does not survive `[Conditional]` argument stripping

`Logger.Debug("event", new string[]{"type", evt.Type})` — if the `evt` capture lives only inside the call, `[Conditional("DEBUG")]` strips the entire call site in Release including the `evt.Type` read. Don't rely on Trace/Debug calls for event-side effects.

### `[Script]` attributes on extern methods

Many `System.Web` methods are `extern` with a `[Script]` body. If you reference these methods from outside the `System.Web` assembly via reflection-style helpers, the body is still inlined — but if you copy/paste-clone a method without copying its `[Script]` attribute, the resulting method is a no-op.

## Diagnostics

| Symptom | Cause |
|---|---|
| `Cannot read properties of null (reading 'innerHTML')` | `Document.GetElementById` returned null (DOM not ready, or wrong id) |
| `JSON.parse: unexpected token` | XHR returned HTML / 4xx body instead of JSON; check `xhr.Status` first |
| `evt.target` is undefined in handler | Bound the listener with raw `[Script]` instead of typed `AddEventListener` |
| Layout thrash / slow scroll | Read/write/read interleave; route layout reads through `LayoutBatcher` |

## Cross-links

- [ADR 0010 — Imported types pattern](../adr/0010-model-native-javascript-types-through-attributed-clr-facades.md)
- [ADR 0015 — Layout-batched DOM reads](../adr/0015-defer-layout-sensitive-dom-reads-and-batch-them.md)
- [Interop attributes reference](../interop/attributes.md)
- [JsonType / ImportedType patterns](../interop/json-and-imported-types.md)
- [Sunlight Core](sunlight-core.md) for `EventBinder`, `CallContext`, and `LayoutBatcher`

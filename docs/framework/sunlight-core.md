# Sunlight.Framework (core)

> **Audience:** *App authors* building MVVM apps.

## TL;DR

`Sunlight.Framework` is the **canonical reactive contract** for NScript ([ADR 0014](../adr/0014-standardize-the-observable-framework-as-the-reactive-binding-contract.md)). It is the platform's MVVM substrate: observable objects and collections, data binders, IoC, an event bus, an async-aware task scheduler, ambient call context, structured logging, and the layout-batch coordinator. Templates ([XWML](../templates/xwml.md), [Razor](../templates/razor.md)) and UI controls ([Sunlight.Framework.UI](sunlight-ui.md)) are layered on top of it.

## Reference — what is in `Sunlight.Framework`

| Subsystem | Location | Purpose |
|---|---|---|
| Observables | `Observables/` | `ObservableObject`, `ObservableCollection<T>`, `INotifyPropertyChanged`, `INotifyCollectionChanged`, `AttachedProperty<T>`, `ExtensibleObservableObject`, `ObservableCollectionTransformer`, `HeaderInjectableTransformer` |
| Binders | `Binders/` | `DataBinder`, `OneTimeDataBinder`, `OneWayBinder`, `TargetBinder`, `SourcePropertyBinder`, `IConverter`, `IValueConverter`, `ValueConverter`, `DataBindingMode` |
| IoC | `IoC/` + `IocContainer.cs` | `IocContainer`, `IocHelper` |
| Events | `EventBus.cs` | Type-keyed pub/sub |
| Async | `TaskScheduler.cs`, `Lazy.cs`, `LazyAsync.cs`, `Factory.cs` | Cooperative scheduler, lazy initialisation, factory wrapper |
| Ambient context | `CallContext.cs` | W3C-traceparent-compatible call context (`TraceId`, `SpanId`, `ActionId`, `Depth`) |
| Logging | `Logger.cs` + `Logging/` | `Logger`, `NamedLogger`, `ILogSink`, `ConsoleSink`, `HttpLogSink`, `LogEvent`, `LogLevel` (see [framework-logging.md](../framework-logging.md)) |
| Layout batching | `LayoutBatcher.cs` | `requestAnimationFrame` coordinator for layout-sensitive DOM reads (see [ADR 0015](../adr/0015-defer-layout-sensitive-dom-reads-and-batch-them.md)) |
| Helpers | `ExceptionHelpers.cs` | `ThrowOnArgumentNull`, etc. |
| Properties | `Properties/` | `AttachedProperty<T>` extension methods (`SetValue` / `GetValue` on `INotifyPropertyChanged`) |

## Quick start — an `ObservableObject`

```csharp
using Sunlight.Framework.Observables;

public class TodoItem : ObservableObject
{
    private string title = "";
    public string Title
    {
        get { return this.title; }
        set
        {
            if (this.title == value) return;
            this.title = value;
            this.FirePropertyChanged("Title");
        }
    }
}
```

`FirePropertyChanged("Title")` notifies every subscriber — including bindings produced from a Razor / XWML template — that `Title` has changed. Subscribers cache by exact property name string; renames are not transparent.

### Subscribe directly

```csharp
var item = new TodoItem();
item.AddPropertyChangedListener("Title",
    (sender, propName) => Console.WriteLine("title changed"));

item.Title = "Buy milk";   // fires
```

`AnyPropertyListener` is the equivalent for "any property". Both are stored on the object as a `StringDictionary<Action<...>>`; there is no per-property allocation until a listener is added.

### `ObservableCollection<T>`

```csharp
var todos = new ObservableCollection<TodoItem>();
todos.CollectionChanged += (s, e) => RefreshList();
todos.Add(new TodoItem());
todos.RemoveAt(0);
```

`CollectionChangedAction` covers `Add`, `Remove`, `Replace`, `Reset`. Bound list views (`ListView` in `Sunlight.Framework.UI`) consume these directly.

## Reference — Binders

| Type | Mode | Use |
|---|---|---|
| `OneTimeDataBinder` | `OneTime` | Read once at template bind time; never tracks changes |
| `OneWayBinder` | `OneWay` | Source → target; subscribes to `INotifyPropertyChanged` |
| `TargetBinder` | `TwoWay` (target side) | Combines a source binder with a write-back path |
| `SourcePropertyBinder` | helper | Reads from a property path on a source object |
| `ValueConverter` / `IValueConverter` | both | Inserts a transform between source and target |

`DataBindingMode` is the enum: `OneTime`, `OneWay`, `TwoWay`. Templates default to `OneTime` unless [ADR 0020](../adr/0020-auto-detect-binding-mode-from-roslyn-semantic-analysis.md)'s analyzer determines the property is observable.

In Razor templates these binders are largely transparent — the compile-time DAG ([ADR 0018](../adr/0018-replace-independent-binders-with-compile-time-reactive-binding-graph.md)) generates the equivalent reactive plumbing. You construct them by hand only when wiring custom controls.

## Reference — IoC

`IocContainer` is a typed service-locator with two flavours of registration: per-resolve factory and singleton.

```csharp
var ioc = new IocContainer();

// Singleton — first call creates, subsequent calls return same instance
ioc.RegisterSingleton<IClock>(c => new SystemClock());

// Per-resolve — factory invoked on every Get
ioc.Register<IRequestHandler>(c => new HttpRequestHandler(c.Get<IClock>()));

var clock = ioc.Get<IClock>();
```

Generics on `IocContainer` are erased through `[IgnoreGenericArguments]`; the registry key is the runtime `TypeId` of `T`. As a consequence: do not register two services with the same closed-generic shape unless you intend the second to overwrite the first.

## Reference — `EventBus`

Process-wide pub/sub keyed on the message type:

```csharp
public class FilterChanged { public string Filter; }

bus.Subscribe<FilterChanged>(msg => RefreshView(msg.Filter));
bus.Publish(new FilterChanged { Filter = "open" });
```

`oneTimeValues` lets you publish a "sticky" value that any subscriber added later sees once. Use `OneTimePublish<T>(T)` for the late-subscriber pattern.

## Reference — `TaskScheduler`

Cooperative scheduler that wraps `setTimeout`, `setInterval`, `setImmediate`, and `requestAnimationFrame`. Uses:

- `TaskScheduler.QueueImmediate(Action)` — runs after the current sync stack
- `TaskScheduler.Delay(int ms, Action)` — runs after a delay
- `TaskScheduler.RequestAnimationFrame(Action)` — runs on the next paint
- All scheduled work captures the ambient `CallContext` and restores it before invoking the callback

The scheduler is what makes `async`/`await` correlate trace/span ids across async boundaries. Don't hand-roll `Globals.SetTimeout` for in-app work — you'll lose call-context propagation.

## Reference — `CallContext`

Ambient context that flows across async boundaries. Analogous to .NET `AsyncLocal<T>`, but JS-single-threaded so a static `Current` is safe for synchronous code:

```csharp
var ctx = new CallContext("trace-abc", "span-1", parentSpan: null, depth: 0, actionId: 1);
using (ctx.Activate())
{
    // CallContext.Current returns ctx here; survives await
    DoWork();
}
```

`EventBinder` roots a new `CallContext` for each user gesture (DOM event whose target is an `Element`). Async I/O completion events (IndexedDB, XHR onload) explicitly skip rooting so they run *under the action that issued the request*.

## Reference — `LayoutBatcher`

Layout-sensitive DOM reads (`OffsetWidth`, `GetBoundingClientRect`, …) interleaved with writes cause forced reflow. `LayoutBatcher` batches reads via `requestAnimationFrame` and resolves continuations through `setImmediate`:

```csharp
public class CardView : UIElement
{
    public Task<double> ActualWidthAsync => LayoutBatcher.ReadAsync(this.Element, e => e.OffsetWidth);
}
```

The split rAF (measure) + setImmediate (dispatch) is what prevents same-frame layout thrash from resolver code. See [ADR 0015](../adr/0015-defer-layout-sensitive-dom-reads-and-batch-them.md).

## Reference — `Logger`

See the dedicated page: [Structured client logging](../framework-logging.md). Highlights:

- `Logger.ForCategory("MyApp.MyComponent")` returns a cached `NamedLogger`
- Property bags use `string[]` flat key/value arrays (NScript minifies field names)
- `Trace` / `Debug` are `[Conditional("DEBUG")]` — stripped from Release at the *caller's* compilation
- `ILogSink` is the extension point; built-ins are `ConsoleSink` and `HttpLogSink`

## Examples

### MVVM with binders by hand

```csharp
public class TitleViewModel : ObservableObject
{
    string title = "";
    public string Title
    {
        get { return this.title; }
        set { if (this.title != value) { this.title = value; FirePropertyChanged("Title"); } }
    }
}

var vm = new TitleViewModel();
var titleEl = Document.GetElementById("title");

new OneWayBinder(
    source: vm,
    sourcePath: "Title",
    target: titleEl,
    targetSetter: (el, v) => ((Element)el).InnerHTML = (string)v);

vm.Title = "Inbox";   // titleEl.innerHTML now reads "Inbox"
```

In practice you write a template ([Razor](../templates/razor.md) preferred); binders are illustrative.

### Lazy initialisation with creation callback

```csharp
var lazyService = new Lazy<IDataService>(() => container.Get<IDataService>());
lazyService.OnCreated += () => Console.WriteLine("service constructed");
var s = lazyService.Value;   // factory runs, OnCreated fires once
```

### Pub/sub through `EventBus`

```csharp
public class UserSignedIn { public string UserId; }

bus.Subscribe<UserSignedIn>(m => Logger.ForCategory("Auth").Info("signed in",
    new string[] { "userId", m.UserId }));

bus.Publish(new UserSignedIn { UserId = "u1" });
```

## Known gotchas

### `FirePropertyChanged` uses string names

If you rename a property, you must update every `FirePropertyChanged("OldName")` call site. There is no `nameof` analyzer wiring; misspellings are silent (no warning, no exception, just no notification).

### `ObservableCollection<T>.RemoveAt(0)`

Inherits the `List<T>.RemoveAt(0)` O(n) characteristic; see [framework/core.md](core.md#known-gotchas).

### Singleton resolution before construction

`IocContainer.Get<T>` constructs lazily on first access. If two singletons cyclically depend on each other, the second `Get<T>` call inside the first factory hits an under-construction state. Break cycles with `Func<T>` parameters or `Lazy<T>`.

### `EventBus.Subscribe<T>` has no automatic unsubscribe

Subscribers are held by the bus until `UnSubscribe<T>` is called — this is a leak source for view-scoped subscribers. Dispose subscriptions in your view tear-down.

### `CallContext` reset must zero coupled fields

If you roll a custom helper that resets `CallContext.Current`, also reset `eventDispatchDepth`. Resetting only `current` lets the outer `OnEventDispatchEnd` silently restore stale `prev` (WI-21 incident).

### `LayoutBatcher` and EventBinder gating

`LayoutBatcher` continuations resolve through `setImmediate`, which is gated by `EventBinder.OnEventDispatch` for user-gesture rooting. Async I/O continuations explicitly skip rooting; do not invent your own dispatch path that bypasses `EventBinder` (you will create orphan call-context roots).

## Diagnostics

| Symptom | Cause |
|---|---|
| Property change does not propagate to view | Missing `FirePropertyChanged` or wrong property-name string |
| Memory grows when navigating between views | `EventBus` subscriptions or `AddPropertyChangedListener` not torn down |
| Stack overflow in property setter | Setter calls `FirePropertyChanged` without the `if (old == new) return;` guard |
| Trace/Debug logs missing in Release | Expected — `[Conditional("DEBUG")]` stripped them |
| Forced reflow in profile | Layout reads not routed through `LayoutBatcher` |

## Cross-links

- [ADR 0014 — Observable framework as reactive contract](../adr/0014-standardize-the-observable-framework-as-the-reactive-binding-contract.md)
- [ADR 0015 — Defer layout-sensitive DOM reads](../adr/0015-defer-layout-sensitive-dom-reads-and-batch-them.md)
- [ADR 0018 — Compile-time DAG binding](../adr/0018-replace-independent-binders-with-compile-time-reactive-binding-graph.md)
- [ADR 0020 — Auto-detect binding mode](../adr/0020-auto-detect-binding-mode-from-roslyn-semantic-analysis.md)
- [Sunlight UI](sunlight-ui.md) layered on top
- [Razor templates](../templates/razor.md), [XWML templates](../templates/xwml.md)
- [Structured client logging](../framework-logging.md)

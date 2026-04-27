# C# language support and limitations

> **Audience:** *App authors* deciding whether a given C# feature will compile cleanly to JavaScript under NScript.

## TL;DR

NScript supports the C# 8 surface area for non-ref, non-reflective code targeting `netstandard2.1`. The translatable subset includes classes, interfaces, generics, LINQ, lambdas, async/await (compiled through state machines into Promises), pattern matching basics, indices/ranges, and null-coalescing. The hard exclusions are anything that requires the .NET runtime: `dynamic`, reflection, P/Invoke, `unsafe` / pointers, and iterator methods (`yield return` / `yield break`). Some C# 8 features are partially implemented or open work — see [csharp8-todos.md](../../csharp8-todos.md) at the repo root.

> **C# 9–13 status:** Framework and test-framework projects are now built with `<LangVersion>13</LangVersion>`. The transparent C# 9–13 syntactic features that ride the existing pipeline are listed in [`csharp9-13-status.md`](./csharp9-13-status.md); semantic features (records, `with`, `init`, non-trivial patterns, collection expressions, `required`, primary constructors on classes) are sequenced into follow-up phases of issue #47.

## Reference — feature support matrix

| Feature | Status | Notes |
|---|---|---|
| Classes, structs (as classes), interfaces | ✅ Supported | Structs are emitted as classes; value semantics not preserved across assignment. See [ADR 0008](../adr/0008-define-how-class-and-interface-hierarchies-map-to-javascript.md). |
| Generics (open and closed) | ✅ Supported | Type arguments are first-class at runtime via JS function type metadata ([ADR 0007](../adr/0007-define-the-javascript-runtime-type-model.md)). |
| `IgnoreGenericArguments` opt-out | ✅ Supported | Skips type-argument emission at instantiation sites; used for type-erased natives. |
| Inheritance, virtual dispatch, abstract members | ✅ Supported | Prototype-chain wiring; non-virtual methods are devirtualised ([ADR 0023](../adr/0023-devirtualize-non-virtual-methods-and-inline-trivial-accessors.md)). |
| Interfaces with explicit metadata | ✅ Supported | Metadata maps emitted via `baseInterfaces` ([ADR 0008](../adr/0008-define-how-class-and-interface-hierarchies-map-to-javascript.md)). |
| `[PseudoInterfaceType]` | ✅ Supported | Structural interfaces with no runtime metadata. |
| LINQ-to-Objects | ✅ Supported | Compiled lambdas; `IEnumerable<T>` operators are framework code. |
| Lambdas, closures | ✅ Supported | JS function expressions; closure capture matches C# semantics. |
| `async` / `await` | ✅ Supported | State machine compilation; awaitables back onto JS Promises (`Task` ↔ `Promise`). |
| `Task<T>`, `Promise<T>` | ✅ Supported | `Task` is `[ImportedType]` aliased to JS `Promise`. |
| `IObservable<T>` (Rx) | ⚠️ Partial | `Sunlight.Framework.Observables` provides the reactive contract used by templates; it's *not* a port of System.Reactive. |
| Pattern matching (basic — `is Type`, `case`) | ✅ Supported | Type tests compile to `instanceof` plus metadata checks. |
| Switch expressions | ⚠️ Partial | Some forms; see [csharp8-todos.md](../../csharp8-todos.md). |
| Property patterns / positional patterns | ❌ Open work | Listed in C# 8 todos. |
| Indices and ranges (`x[^1]`, `x[1..3]`) | ⚠️ Partial | On the C# 8 todo list. |
| Null-coalescing `??`, `??=` | ✅ Supported | `??=` is supported. |
| Null-conditional `?.`, `?[]` | ✅ Supported | Compiled to JS short-circuit chains. |
| Tuples, deconstruction | ✅ Supported | `ValueTuple` emits as a class; deconstruction is sugar over field reads. |
| Local functions | ✅ Supported | Including static local functions. |
| `using` declarations | ❌ Open work | Listed in C# 8 todos. |
| Nullable reference types (NRT) | ⚠️ Annotation-only | NScript respects `?` for code-gen but does not enforce non-null at runtime. |
| Default interface methods | ❌ Open work | Listed in C# 8 todos. |
| `dynamic` | ❌ Not supported | No DLR equivalent on the JS target. Use `[Script]` or `object` casts. |
| Reflection (`Type.GetMethod`, `Activator.CreateInstance` with args, `RuntimeTypeHandle`) | ❌ Not supported | Limited surface only — `typeof(T)` works for the runtime type model; full reflection does not. See [framework/core.md](../framework/core.md). |
| Iterators (`yield return`, `yield break`) | ❌ Not supported | The state-machine lowering for iterators isn't ported. Build collections eagerly or use Observables. |
| Asynchronous streams (`IAsyncEnumerable<T>`) | ❌ Open work | Listed in C# 8 todos. |
| `unsafe`, pointers, `stackalloc` | ❌ Not supported | No native memory model on the JS target. |
| P/Invoke, `[DllImport]` | ❌ Not supported | No interop boundary — use `[Script]` for native JS instead. |
| `lock` / monitors | ⚠️ No-op | JS is single-threaded; `lock` compiles but provides no synchronisation. |
| `try` / `catch` / `finally` | ✅ Supported | Compiled to JS `try`/`catch`/`finally`. |
| `throw` expressions | ⚠️ Bug | Listed as open issue in [csharp8-todos.md](../../csharp8-todos.md). Use `throw` *statements* until fixed. |
| `params T[]` | ✅ Supported | Variadic JS calls. |
| Operator overloading | ✅ Supported | Operators emit as static methods. |
| Extension methods | ✅ Supported | Static methods invoked with instance syntax. |
| `string` interpolation `$"…"` | ✅ Supported | Including verbatim `@$"…"`. |
| Numeric `decimal` | ⚠️ Maps to `number` | No 128-bit decimal in JS; loss of precision past 2^53. |
| Numeric `long` / `ulong` | ⚠️ Maps to `number` | Loss of precision past 2^53. Use `BigInt` via `[Script]` for large integers. |
| `DateTime`, `TimeSpan` | ⚠️ Limited | Maps to JS `Date`; only the subset shipped in `mscorlib` is available. See [framework/core.md](../framework/core.md). |

## Reference — feature → emission summary

```mermaid
flowchart LR
    A[C# Source] --> B{Roslyn frontend}
    B -- Bound expressions --> C[BondToAst]
    C --> D[JST]
    D --> E{cs2jsc emit}
    E --> F[JavaScript]

    subgraph Lowered features
      L1[async/await<br/>→ state machine]
      L2[lambdas<br/>→ JS functions]
      L3[generics<br/>→ runtime type args]
      L4[LINQ<br/>→ method chains]
    end

    B -.-> L1
    B -.-> L2
    B -.-> L3
    B -.-> L4
```

All C# constructs lower into the JST tree first; cs2jsc emits from JST to JavaScript.

## Examples

### What works

```csharp
public async Task<List<TodoDto>> LoadAsync()
{
    var resp = await fetchClient.GetAsync("/api/todos");
    var json = await resp.Content.ReadAsStringAsync();
    return ((TodoDto[])JSON.Parse(json))
        .Where(t => !t.Completed)
        .OrderBy(t => t.Title)
        .ToList();
}
```

LINQ + async + lambdas + generics — all standard NScript output.

### What doesn't (yet)

```csharp
// ❌ yield return — iterators not supported
public IEnumerable<int> Counter()
{
    for (int i = 0; i < 10; i++) yield return i;
}

// ❌ dynamic
public void Print(dynamic x) => Console.WriteLine(x.Value);

// ❌ throw expression (open bug)
var name = input ?? throw new ArgumentNullException(nameof(input));
```

For iterators, return a materialised `List<T>` or expose an `ObservableCollection<T>`. For `dynamic`, cast to `object` and route through `[Script]` if you genuinely need member access on an untyped JS value. For `throw` expressions, expand to a statement form until [csharp8-todos.md](../../csharp8-todos.md) closes the bug.

## Known gotchas

### Structs do not have value semantics

A `struct Point { int X; int Y; }` compiles to a JS class. Assignment copies the *reference*, not the fields. Code that relies on struct copy semantics (e.g. mutating a `List<Point>` element via `list[i].X = 5`) will not behave like .NET. Prefer immutable patterns (`with`-style copies) or use classes explicitly.

### `long` precision silently lost above 2^53

There is no `long` in JavaScript. NScript emits `number`. Computations beyond `Number.MAX_SAFE_INTEGER` (≈ 9 × 10¹⁵) lose precision with no error. For larger ranges, route through `BigInt` via `[Script]`.

### `lock` compiles but is a no-op

JS is single-threaded; `lock(obj) { ... }` emits as a bare block. This is silent — the code runs, but you don't get critical-section semantics. There is no scenario where you need a lock in NScript.

### `decimal` is `number`

`decimal d = 0.1m + 0.2m;` produces `0.30000000000000004` in JS. If you need exact-decimal arithmetic for currency, do the math in cents (integers) or pull a JS bignum library via `[Script]`.

### Reflection is limited to the runtime type model

`typeof(T)`, `obj.GetType()`, basic `IsAssignableFrom` work. `Type.GetMethod`, `MethodInfo.Invoke`, `Activator.CreateInstance(type, args)`, `Expression`-tree reflection — none of those work. Code that needs them (DI containers, serializers, mappers) must be specialised for NScript's metadata model — usually via `IocContainer` (registration-driven, not reflection-driven) or attribute-driven plugins.

### `[Conditional("DEBUG")]` strips at the *caller's* compilation

If you ship a Release-built framework, `Logger.Debug(...)` calls inside *your app* (Debug-built) still emit, but calls from inside the Release framework are stripped. Mix-and-match builds are normal — be aware that diagnostic depth depends on which assembly's call site is doing the logging.

### NRT annotations don't enforce at runtime

`string?` vs `string` is a compile-time hint. The emitter doesn't generate null-checks. Treat NRT as documentation; defensive checks are still on you for boundaries.

### `Task` and `Promise` are the same runtime object

NScript's `Task` is an `[ImportedType]` over JS `Promise`. `await task` and `await promise` are interchangeable. There is no separate scheduler — `Task.Run` doesn't fork a thread; it just defers via a microtask.

## Diagnostics

| Symptom | Cause |
|---|---|
| `'yield' is not supported` | Iterator method — rewrite as eager list or `ObservableCollection<T>` |
| `dynamic is not supported` | Use `object` + cast / `[Script]` instead |
| `Cannot invoke method 'Foo' via reflection` | Reflection isn't supported; build the call site statically |
| `Activator.CreateInstance with parameters not supported` | Only parameterless `Activator.CreateInstance<T>()` is supported |
| Wrong arithmetic on large integers | `long` precision loss past 2^53 |
| `Cannot find type 'IAsyncEnumerable'` | Async streams not yet supported |

## Cross-links

- [csharp8-todos.md](../../csharp8-todos.md) — open C# 8 work items
- [Getting started](../getting-started/README.md) — day-1 pitfalls
- [Framework Core](../framework/core.md) — BCL surface details
- [Interop attributes](../interop/attributes.md) — opt-outs and emission control
- [`[Script]` and dynamic JS](../interop/dynamic.md) — escape hatch for unsupported APIs
- [ADR 0007 — Runtime type model](../adr/0007-define-the-javascript-runtime-type-model.md)
- [ADR 0008 — Class & interface mapping](../adr/0008-define-how-class-and-interface-hierarchies-map-to-javascript.md)

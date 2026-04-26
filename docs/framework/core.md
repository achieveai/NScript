# Framework Core — supported BCL surface

> **Audience:** *App authors*.

## TL;DR

NScript ships its own `mscorlib.dll` and `System.Core.dll` (under `Sources/Framework/mscorlib/` and `Sources/Framework/System.Core/`). These are *not* the official .NET BCL — they are a curated subset shaped for translation to JavaScript, with a few NScript-specific additions. This page summarises what is available, what behaves identically to .NET, and the well-known gotchas.

## Reference — what is in `mscorlib`

The full file list lives under `Sources/Framework/mscorlib/`. Highlights:

| Area | Types |
|---|---|
| Primitives & boxing | `Object`, `String`, `Boolean`, `Char`, `Byte`, `SByte`, `Int16`, `UInt16`, `Int32`, `UInt32`, `Int64`, `UInt64`, `Single`, `Double`, `Decimal`, `Number`, `Nullable<T>`, `ValueTuple` |
| Time | `DateTime`, `DateFormatInfo`, `NumberFormatInfo`, `CultureInfo` |
| Delegates / events | `Delegate`, `MulticastDelegate`, `Action`, `Action<...>` (up to 8 args), `Func<...>`, `EventHandler`, `EventArgs`, `EventBinder`, `EventTarget` |
| Collections | `Array`, `NativeArray`, `Collections.Generic.List<T>`, `Dictionary<TKey,TValue>`, `StringDictionary<T>`, `IEnumerable<T>`, `IEnumerator<T>`, `HashSet<T>`, LINQ enumerable extensions in `System.Linq` |
| Exceptions | `Exception`, `ArgumentException`, `InvalidOperationException`, `NotSupportedException`, `NotImplementedException`, `Error`, `ErrorEvent` |
| Async | `Threading.Tasks.Task`, `Task<T>`, `IAsyncStateMachine`, `Promise`, async/await machinery in `Runtime.CompilerServices` |
| Strings | `StringBuilder`, `String.Format`, `RegularExpression`, `String.Replace` (`StringReplaceCallback`) |
| Math | `Math`, `Number` |
| Reflection (limited) | `Type`, `RuntimeFieldHandle`, `RuntimeTypeHandle`, `Activator` |
| Misc | `Guid`, `IntPtr`, `Environment`, `Diagnostics.Debug`, `Threading.Thread` (extremely limited — JS is single-threaded) |

`System.Core` adds extended LINQ and async streams scaffolding. `Microsoft.CSharp` adds the late-binding plumbing used by `dynamic` (see [interop/dynamic.md](../interop/dynamic.md)).

## Quick start

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public static class CountVowels
{
    public static int Run(IEnumerable<string> words)
    {
        return words
            .SelectMany(w => w.ToCharArray())
            .Count(c => "aeiou".IndexOf(c) >= 0);
    }
}
```

This compiles cleanly: generics, LINQ, lambdas, and string indexing all work as in .NET.

## What works identically to .NET

- Generic types and methods, including constraints
- Lambdas and closures
- LINQ-to-objects (`Where`, `Select`, `OrderBy`, `GroupBy`, `Aggregate`, `ToList`, `ToArray`, `Distinct`, `First`/`Single`/etc.)
- `async` / `await` (Task-based, lowered into a state machine; see ADR 0006)
- Pattern matching (`is` patterns, switch expressions for C# 8 — see [csharp8-todos.md](../../csharp8-todos.md))
- Null-coalescing (`??`, `??=`)
- Range and index operators (`..`, `^`)
- `using` / `using var` for `IDisposable`
- Tuples and value tuples
- `String.Format` and interpolated strings

## Known gotchas

### `List<T>.RemoveAt(0)` is O(n)

`List<T>` is implemented over a JS native array. `RemoveAt(0)` lowers to `Array.splice(0, 1)`, which shifts every subsequent element. For high-frequency eviction (queue-like usage), prefer a small ring buffer or batch removal. Reference: WI-11 `HttpLogSink` redesign.

### Integer arithmetic is JS double semantics

All numeric primitives compile through JavaScript `Number` (IEEE-754 double). Beyond `2^53` you lose integer precision. `Int64` / `UInt64` use a polyfill but are still backed by doubles in many operations — do not assume bit-exact 64-bit semantics. `Decimal` is similarly non-faithful: it does not give .NET-style decimal arithmetic.

### `Activator.CreateInstance` works only with parameterless types known at compile time

NScript codegen generates a factory function only for constructors that take parameters (see ADR 0007 / 0008). Parameterless constructors are invoked via `new Type()` from resolved type identifiers. `Activator.CreateInstance(typeof(Foo))` works for compile-time-known types but cannot enumerate types or load them by name at runtime.

### No reflection

There is a `Type` class with `FullName` / `TypeId` / `BaseType`, but enumerating fields, methods, or invoking by name is not supported. Code that depends on `MethodInfo.Invoke`, `FieldInfo.SetValue`, or `Assembly.GetTypes()` will not compile or will fail at runtime. If you need polymorphic dispatch, use interfaces or delegates.

### `string[]` for property bags, not anonymous types

When passing key/value data through interop or to `Logger.*`, use a flat `string[]` of alternating keys and values. C# anonymous objects survive Roslyn but get their property names minified by NScript, breaking JSON serialisation and interop. See [framework-logging.md](../framework-logging.md) for the rationale.

### `DateTime` semantics

`DateTime` wraps a JS `Date`. Methods like `AddDays`, `ToString(format)` work, but `DateTime.Kind`, time-zone-aware conversions, and `DateTimeOffset` are not faithful — the JS `Date` model is a single UTC moment plus host time-zone display. Rely on UTC for any cross-system contract.

### `InternalsVisibleTo` works at the Roslyn level only

The `InternalsVisibleToAttribute` is present in `mscorlib` but marked `[NonScriptable]` — Roslyn honors it for compilation, but it has no runtime effect.

## Diagnostics

| Compile error | Likely cause |
|---|---|
| `The type or namespace name 'X' could not be found` referencing a BCL type | NScript's `mscorlib` does not export it — check `Sources/Framework/mscorlib/` |
| `dynamic` cannot be used | Not supported. See [interop/dynamic.md](../interop/dynamic.md) |
| Method `extern` produces an empty JS function | Missing `[Script("...")]` body — see [interop/attributes.md](../interop/attributes.md) |
| Generated JS calls a method that is undefined at runtime | Likely a `[NonScriptable]` member compiled into a call site that should have been gated by `[Conditional]` |

## Cross-links

- [ADR 0007 — Runtime type model](../adr/0007-define-the-javascript-runtime-type-model.md)
- [ADR 0008 — Class/interface mapping to JS](../adr/0008-define-how-class-and-interface-hierarchies-map-to-javascript.md)
- [ADR 0011 — Arrays as a wrapped substrate](../adr/0011-treat-arrays-as-a-special-wrapped-runtime-substrate.md)
- [Limitations & unsupported C# features](../language/limitations.md)
- [csharp8-todos.md](../../csharp8-todos.md) — current C# 8 status

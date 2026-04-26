# `[Script]` blocks and dynamic JS interop

> **Audience:** *App authors* and *binders* writing inline JavaScript that needs to reach back into NScript symbols.

## TL;DR

Inline JavaScript bodies live on `[Script("...")]` attributes attached to `extern` methods or constructors. They are *not* opaque text — `JsniResolver` parses them as JavaScript, scopes them against the surrounding declaration, and resolves identifiers against parameters, locals, members of the enclosing type, types in scope, and a controlled set of registered globals ([ADR 0009](../adr/0009-prefer-inline-script-attribute-for-javascript-dependencies.md), [ADR 0012](../adr/0012-parse-and-resolve-script-blocks-against-types-members-and-known-globals.md)). Cross-type symbol references use the JSNI-style `@{[Assembly]Namespace.Type::Member}` syntax, which is rewritten to the minified runtime name. Unresolved identifiers fail compilation.

## Reference — directive table

| Directive | Form | Purpose |
|---|---|---|
| Bare identifier | `value`, `evt` | Parameter or local |
| Member of enclosing type | `this.field` | Member of the `[Script]`-bearing declaration |
| Same-type static via JSNI | `@{Type::Member}` | Static member of the same enclosing type |
| Cross-type via JSNI | `@{[Assembly]Namespace.Type::Member}` | Static or instance reference in another assembly. Compiles to the resolved (often minified) runtime name. |
| Bracket access | `obj['x']` | Always literal-property access; opt out of resolution |

## Reference — JSNI symbol syntax

The form `@{[AssemblyName]Namespace.TypeName::MemberName}` mirrors GWT's JSNI but adapts to NScript's resolver pipeline:

- `[AssemblyName]` — required when the target lives in a different assembly. Omit for same-assembly references (`@{Type::Member}`).
- `Namespace.TypeName` — fully qualified type name as the resolver sees it (post-`[ScriptNamespace]`/`[IgnoreNamespace]` shaping).
- `::Member` — the member's C# name. The resolver substitutes the emitted (possibly minified) JS name at compile time.

This is what makes inline JS *minification-safe*: even if `CallContext.current` is renamed to `ab`, the JSNI reference `@{[Sunlight.Framework]Sunlight.Framework.CallContext::current}` resolves to `ab` in the emitted code.

## Quick start

### A method-local helper

```csharp
using System.Runtime.CompilerServices;

public static class JsHelpers
{
    [Script("return value === undefined;")]
    public static extern bool IsUndefined(object value);
}
```

`value` resolves to the method parameter. The body emits as the function body of `IsUndefined` after parameter renaming.

### Reading another type's static field

```csharp
[Script("return !!@{[Sunlight.Framework]Sunlight.Framework.CallContext::current};")]
public static extern bool HasAmbientContext();
```

The JSNI form ensures the `current` field reference survives minification.

### Two-callback Promise wiring

```csharp
[Script(@"
    var ctx = @{[Sunlight.Framework]Sunlight.Framework.CallContext::current};
    return p.then(
        function(v) { @{[Sunlight.Framework]Sunlight.Framework.CallContext::current} = ctx; return v; },
        function(e) { @{[Sunlight.Framework]Sunlight.Framework.CallContext::current} = ctx; throw e; }
    );
")]
public static extern Promise WrapPromise(Promise p);
```

Real example from `Sunlight.Framework.CallContext` — captures the ambient context, restores it inside the `.then` continuations so async paths see the call-site context.

## Examples

### Calling an unmodelled global

If you need a global that has no `[GlobalMethods]` C# façade, you can route through `[Script]`:

```csharp
[Script("return Math.sign(x);")]
public static extern int Sign(double x);
```

`Math` is in the resolver's known-globals list. If you reach for something that isn't (`window.cdnLib.foo()`), declare a `[GlobalMethods]` class for it instead — the resolver will reject unknown bare identifiers.

### Exposing diagnostic hooks

```csharp
[Script(@"
    if (typeof window !== 'undefined') {
        window.__myApp = window.__myApp || {};
        window.__myApp.dump = function() { return @{[MyApp]MyApp.State::Snapshot}(); };
    }
")]
private static extern void ExposeDebugProbe();
```

Since NScript wraps emitted code in an IIFE, this is the canonical way to expose a debug surface to DevTools / Playwright.

### Boundary check that can't be expressed in C#

```csharp
[Script("return !!(evt && evt.target && evt.target.tagName);")]
private static extern bool IsUserGestureEvent(object evt);
```

Real example again — distinguishes a DOM-Element-based event from an `EventTarget`-based one (IndexedDB success, etc.) by string-truthy `tagName` access. Cleaner than nested null-checks plus `instanceof`.

## Known gotchas

### Unresolved identifiers fail compilation, not runtime

`[Script("doFoo()")]` where `doFoo` is neither a parameter, member, type, nor a known global produces a build error from `JsniResolver`. This is a feature ([ADR 0012](../adr/0012-parse-and-resolve-script-blocks-against-types-members-and-known-globals.md)) — typos and missing dependencies surface during compile rather than as `ReferenceError` in production.

### `this` in a `[Script]` body refers to the JS receiver, not C# `this`

Inside an instance method's `[Script]` body, `this` is the JavaScript receiver of the emitted function. NScript instance methods compile to functions with explicit `this` parameter; if the method is called as a free function, `this` is `undefined`. Prefer JSNI references (`@{Type::Member}`) when you need stable member access.

### `extern` is required on the C# declaration

`[Script("...")]` is only valid on `extern` methods or constructors — there is no body for the C# compiler to compile, so it must be `extern`. Forgetting `extern` produces a CS0500 from Roslyn before NScript ever sees the method.

### Bracket access disables resolution

`obj['x']` is treated as plain literal-property lookup. The resolver does not try to bind `'x'` against any member. Use this when you need to access a property whose name might collide with a renamed C# member.

### Multi-line bodies must be raw strings

C# verbatim-string `@"..."` is the standard pattern for multi-line `[Script]` bodies. Beware: `"` inside the body must be doubled (`""`).

### Resolver runs against the current scope, not the call site

Identifiers resolve based on the enclosing declaration's scope, not where the method is *called*. A `[Script]` body cannot see the caller's locals.

### Trace/Debug calls inside `[Script]` bodies are not stripped by `[Conditional]`

`[Conditional("DEBUG")]` strips at the *caller* — but a `[Script]` body is opaque to the C# compiler. If you embed a debug log inside the JS string, it survives Release builds. Drive logging from outside the `[Script]` body (in C#) instead.

### Performance: every `[Script]` body parses at compile time

The JS parser runs over each `[Script]` body. Long, repetitive bodies are fine but slow incremental compiles. Extract shared helpers into a regular C# method that calls a smaller `[Script]` block.

## Diagnostics

| Symptom | Cause |
|---|---|
| `Cannot resolve identifier 'foo'` from `[Script]` body | Identifier missing from parameters/members/types/known-globals; add a `[GlobalMethods]` declaration if it's a real global |
| `JSNI member 'X' not found on type 'T'` | Wrong member name in `@{[Asm]NS.T::X}`; check casing and that `X` exists on `T` |
| `Assembly 'X' not referenced` from JSNI | The target type lives in an assembly not on the project's reference list |
| `[Script]` body compiles but emits wrong runtime calls | Likely a missing JSNI reference — bare identifier matched a parameter rather than the intended member; switch to JSNI |
| Identifier renamed by minifier breaks call into JS | `[Script]` body uses a bare identifier instead of JSNI; or the target needs `[PreserveName]` |

## Cross-links

- [ADR 0009 — Inline `[Script]` for JS dependencies](../adr/0009-prefer-inline-script-attribute-for-javascript-dependencies.md)
- [ADR 0012 — Inline script resolution](../adr/0012-parse-and-resolve-script-blocks-against-types-members-and-known-globals.md)
- [Interop attributes reference](attributes.md)
- [JsonType / ImportedType deep dive](json-and-imported-types.md)
- [Compiler plugins (resolver internals)](../compiler/plugins.md)

# Interop attributes reference

> **Audience:** *App authors* and *framework binders*.

## TL;DR

NScript ships ~20 attributes in `System.Runtime.CompilerServices` (the `mscorlib` clone, *not* the BCL `System.Runtime.CompilerServices`) that control how the compiler emits, names, and wraps types and members. They fall into five families: **inline JS bodies** (`Script`, `Mixin`), **type model** (`ImportedType`, `JsonType`, `Extended`, `NonScriptable`, `GlobalMethods`), **naming** (`ScriptName`, `ScriptAlias`, `ScriptNamespace`, `IgnoreNamespace`, `PreserveCase`, `PreserveName`), **emission control** (`ScriptSkip`, `IntrinsicProperty`, `IntrinsicField`, `AlternateSignature`, `IgnoreGenericArguments`), and **enum/resource shape** (`NamedValues`, `NumericValues`, `Resources`). See [ADR 0009](../adr/0009-prefer-inline-script-attribute-for-javascript-dependencies.md) and [ADR 0010](../adr/0010-model-native-javascript-types-through-attributed-clr-facades.md) for the design rationale.

## Reference — full attribute table

| Attribute | Targets | Effect | ADR |
|---|---|---|---|
| `[Script("body")]` | Method, Constructor | Replaces the method body with the inline JS string at conversion time. The body is parsed into JST and resolved against the surrounding scope so identifiers must exist (see [ADR 0012](../adr/0012-parse-and-resolve-script-blocks-against-types-members-and-known-globals.md)). | [0009](../adr/0009-prefer-inline-script-attribute-for-javascript-dependencies.md) |
| `[Mixin("expr")]` | Class | Emits a single JS expression for the whole class — used for trivial pass-through types. | — |
| `[Extended]` | Type | Marks a type as "extended" — generally an attribute that influences the converter; not present at runtime. Always paired with `[NonScriptable]`. | — |
| `[NonScriptable]` | Type | Type is invisible to the JS emitter — no class, no metadata. Used for compiler-only attributes and helpers. | — |
| `[ImportedType]` | Type | Type is a façade for a real native JS type (`Element`, `Date`, `Promise` …). Member accesses compile to direct property/method calls; no instance is allocated by NScript. | [0010](../adr/0010-model-native-javascript-types-through-attributed-clr-facades.md) |
| `[JsonType]` | Type | Type represents a plain JSON record. Member access wraps in `importedExtension` to unbox the JS-side property. Use for serialisable DTOs. | [0010](../adr/0010-model-native-javascript-types-through-attributed-clr-facades.md), [interop/json-and-imported-types](json-and-imported-types.md) |
| `[PseudoInterfaceType]` | Type | Interface is structural — no runtime metadata is emitted; assignability is duck-typed. Useful for typing native callback shapes. | [0010](../adr/0010-model-native-javascript-types-through-attributed-clr-facades.md) |
| `[GlobalMethods]` | Class | Static class whose methods compile to bare global calls (`encodeURI(...)` rather than `Globals.encodeURI(...)`). | [0010](../adr/0010-model-native-javascript-types-through-attributed-clr-facades.md) |
| `[ScriptName("name")]` | Type, Method, Property, Field, Event | Renames the symbol in emitted JS but keeps namespace/context. Use when the JS API uses a different identifier (e.g. `[ScriptName("addEventListener")] AddEventListener`). | — |
| `[ScriptAlias("alias")]` | Method, Property | Emits the symbol at the *global* scope. Different from `ScriptName` which preserves enclosing context. | — |
| `[ScriptNamespace("name")]` | Type, Assembly | Overrides the .NET namespace for emission purposes. `[ScriptNamespace("")]` flattens to global. | — |
| `[IgnoreNamespace]` | Type | Equivalent to `[ScriptNamespace("")]`. The type's name is emitted without namespace prefix. | — |
| `[PreserveCase]` | Member | Skip the default first-letter-lowercasing of member names (`Foo` stays `Foo` instead of becoming `foo`). | — |
| `[PreserveName]` | Member, Class | Skip minification renaming for this symbol. Required for names that are observed externally (DOM event names, JSON field names, etc.). | [0021](../adr/0021-require-resolved-identifiers-for-all-generated-javascript-symbols.md) |
| `[ScriptSkip]` | Method | Method call is elided in emitted JS — the receiver is substituted for the call expression. Used for identity / no-op casts. | — |
| `[IntrinsicProperty]` | Property | Property accesses compile to direct field access (`obj.X` not `obj.get_X()`). The property must have only get/set bodies that map 1:1 to a JS field of the same name. | — |
| `[IntrinsicField]` | Field | Symmetric to `[IntrinsicProperty]` — the field is referenced by literal name. | — |
| `[AlternateSignature]` | Method, Constructor | Method is an extern overload for tooling — the body is unused; one canonical overload supplies the JS. | — |
| `[IgnoreGenericArguments]` | Method, Type | Generic type arguments are not emitted into the JS instantiation site. Used when the JS implementation is type-erased. | — |
| `[NamedValues]` | Enum | Enum values emit as their *name* strings (`"Red"`) rather than ordinals. | — |
| `[NumericValues]` | Enum | Enum values emit as numeric literals (default behavior — explicit form). | — |
| `[Resources]` | Class | Marks a static class whose string fields are populated from external `.resx`/resource bundles at compile time. | — |
| `[EntryPoint]` | Method | Marks the static `Main()` entry-point. The method's containing class becomes the application root. | — |

## Reference — common compositions

Most NScript-specific attributes carry both `[Extended]` and `[NonScriptable]`. The pattern is intentional:

- `[NonScriptable]` keeps the *attribute type itself* out of the emitted JS (it's purely a compiler hint).
- `[Extended]` opts the type into the converter's "extended attribute" lookup table — without it, the converter ignores it.

When you write a custom converter plugin (see [compiler/plugins.md](../compiler/plugins.md)) that defines its own attribute, follow the same pattern.

## Reference — naming attribute decision tree

```mermaid
flowchart TB
    Q1{Need to change<br/>the emitted name?}
    Q1 -- no --> Done1[Use the C# identifier as-is]
    Q1 -- yes --> Q2{Is this a global<br/>symbol \(no namespace\)?}
    Q2 -- yes --> A1["[ScriptAlias(\"globalName\")]"]
    Q2 -- no --> Q3{Just want to skip<br/>case-lowering?}
    Q3 -- yes --> A2["[PreserveCase]"]
    Q3 -- no --> Q4{Want to skip<br/>minification?}
    Q4 -- yes --> A3["[PreserveName]"]
    Q4 -- no --> A4["[ScriptName(\"name\")]"]
```

## Quick start

### Inline JS for a single method

```csharp
using System.Runtime.CompilerServices;

public static class JsHelpers
{
    [Script("return value === undefined;")]
    public static extern bool IsUndefined(object value);
}
```

The body must parse as valid JST. Identifiers like `value` resolve against the parameter list. See [ADR 0012](../adr/0012-parse-and-resolve-script-blocks-against-types-members-and-known-globals.md) for resolution rules.

### Wrap a native JS API

```csharp
using System.Runtime.CompilerServices;

[ImportedType, IgnoreNamespace, ScriptName("Date")]
public sealed class JsDate
{
    public extern JsDate();
    public extern int GetTime();

    [PreserveCase] public extern int Year { get; }
}

// Usage compiles to: var d = new Date(); d.getTime();
var d = new JsDate();
int t = d.GetTime();
```

`[IgnoreNamespace]` removes `MyApp.JsDate` → `Date`; `[ScriptName("Date")]` is needed only if the *class name* (not just namespace) differs.

### Mark a type as a global helper

```csharp
[GlobalMethods, NonScriptable]
public static class Globals
{
    [PreserveCase] public extern static string EncodeURI(string s);
}

// Usage: Globals.EncodeURI("/x") compiles to: encodeURI("/x")
```

`[NonScriptable]` here is correct even on a class with bodies — `[GlobalMethods]` rewrites every call site, so the class itself is never instantiated.

## Examples

### Field-name interop

```csharp
[JsonType, IgnoreNamespace]
public class TodoDto
{
    [PreserveCase] public string Title { get; set; }
    [PreserveCase] public bool Completed { get; set; }
}
```

`[PreserveCase]` keeps `Title` and `Completed` as PascalCase to match a server-side schema. Without it, the emitter lowercases to `title` / `completed`.

### Skip-call optimisation

```csharp
[ScriptSkip]
public static T As<T>(this object o) where T : class => (T)o;

// Usage: x.As<Foo>().Bar() compiles to: x.bar()
```

Useful for identity casts, fluent extension methods that just return their receiver, and type-tag wrappers.

### Alternate signatures for tooling

```csharp
public class Logger
{
    [AlternateSignature] public extern void Log(string msg);
    [AlternateSignature] public extern void Log(string msg, params object[] args);

    public void Log(string msg, object[] args = null) { /* canonical body */ }
}
```

The `extern` overloads provide IntelliSense surfaces; the non-extern version is what the compiler emits.

## Known gotchas

### `[Extended]` + `[NonScriptable]` is the norm

Almost every interop attribute carries both. If you create a custom one and forget `[Extended]`, the converter silently ignores it. If you forget `[NonScriptable]`, the attribute type itself appears in the emitted JS (wasted bytes and a confusing class definition).

### `[Script]` body identifiers must resolve

`[Script("doFoo()")]` where `doFoo` is not a parameter, local, member, or `[GlobalMethods]` static is a *compile* failure ([ADR 0012](../adr/0012-parse-and-resolve-script-blocks-against-types-members-and-known-globals.md)). To call a true global that's not modelled, route through a `[GlobalMethods]` declaration first.

### `[ScriptName]` vs `[ScriptAlias]`

`ScriptName` only renames *within the existing context* — `MyApp.Foo` becomes `MyApp.Bar`. `ScriptAlias` *also lifts to global scope* — `MyApp.Foo` becomes plain `Bar`. They're often confused; pick based on whether the JS API namespace matches your C# namespace.

### `[PreserveName]` defends against minifier renaming, not casing

The minifier ([ADR 0024](../adr/0024-deduplicate-structurally-identical-functions-after-minification.md)) renames symbols to short tokens. `[PreserveName]` opts out. It does *not* affect first-letter lowering — that's `[PreserveCase]`. Apply both if you want exactly the C# identifier in emitted JS.

### `[IntrinsicProperty]` requires 1:1 mapping

`[IntrinsicProperty] public int X { get; set; }` works because the get/set are pass-throughs to a backing field. If your getter does *anything* (computed value, side effect), `[IntrinsicProperty]` will produce wrong code — drop the attribute and let the converter emit `get_X()` / `set_X()` calls.

### `[ImportedType]` instances cannot be serialized

Field accesses on `[ImportedType]` types go through native property gets, not through NScript metadata. Passing them to `JSON.Stringify`, `structuredClone`, or `IndexedDB` is fine *only if the underlying native object is plain data* — but `Element`, `Date`, `RegExp` etc. are not. See [interop/json-and-imported-types.md](json-and-imported-types.md).

### `[JsonType]` types should not be created in codegen plugins

Per the project [JST codegen rules](../compiler/plugins.md), `[JsonType]` adds an `importedExtension` wrapper on field access — if you hand-build a JST object literal, the wrapper won't match. Use a real `new TypedDto()` allocation instead.

## Diagnostics

| Symptom | Cause |
|---|---|
| `Cannot resolve name 'foo'` in `[Script]` body | Identifier in the inline body has no matching parameter, member, or global |
| Property access compiles to `obj.get_X()` not `obj.X` | Missing `[IntrinsicProperty]` |
| Method call appears in JS but you wanted it elided | Missing `[ScriptSkip]` |
| Symbol renamed by minifier, breaking external interop | Missing `[PreserveName]` |
| Custom interop attribute is silently ignored by converter | Missing `[Extended]` |
| `[Mixin]` produces nothing at runtime | Class is correctly marked but nothing referenced it; demand-driven conversion ([ADR 0022](../adr/0022-demand-driven-conversion-and-dead-code-elimination.md)) elided it |

## Cross-links

- [ADR 0009 — Inline `[Script]` for JS dependencies](../adr/0009-prefer-inline-script-attribute-for-javascript-dependencies.md)
- [ADR 0010 — Imported types pattern](../adr/0010-model-native-javascript-types-through-attributed-clr-facades.md)
- [ADR 0012 — Inline script resolution](../adr/0012-parse-and-resolve-script-blocks-against-types-members-and-known-globals.md)
- [ADR 0021 — Resolved identifiers in codegen](../adr/0021-require-resolved-identifiers-for-all-generated-javascript-symbols.md)
- [JsonType / ImportedType deep dive](json-and-imported-types.md)
- [`[Script]` blocks and dynamic JS](dynamic.md)
- [Compiler plugins](../compiler/plugins.md)

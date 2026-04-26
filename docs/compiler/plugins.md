# Compiler plugins (`IConverterPlugin`)

> **Audience:** *Contributors* extending the converter pipeline with custom code-generation, template processors, or runtime emit hooks.

## TL;DR

The converter is extensible via `IConverterPlugin`-derived plugin interfaces. There are three plugin shapes: **`IMethodConverterPlugin`** (intercept method bodies), **`ITypeConverterPlugin`** (intercept type emission), and **`IRuntimeConverterPlugin`** (contribute method references and emit code outside any specific type). Plugins are registered via `PluginConfig.xml` (XML-based, not reflection-discovered). Built-in plugins: `XwmlTemplatingPlugin` (XWML templates), `RazorTemplatingPlugin` (Razor `.skin.cshtml`), and `TestGenerator` (QUnit registration emission). All JST identifier creation must follow the [JST codegen rules](#reference--jst-codegen-rules) — raw string identifiers will be silently broken by the minifier ([ADR 0021](../adr/0021-require-resolved-identifiers-for-all-generated-javascript-symbols.md)).

## Reference — plugin interfaces

| Interface | When invoked | Use for |
|---|---|---|
| `IConverterPlugin` (base) | Once at startup via `Initialize(ClrContext, RuntimeScopeManager)` and `ParseArgs(args)` | Wire-up; common to all plugin shapes |
| `IMethodConverterPlugin` | Per method, with `IntrestLevel` selecting *when* in the method emission | Pre/post statements, encapsulation, or full overwrite of a method body |
| `ITypeConverterPlugin` | Per type, with the same `IntrestLevel` enum | Pre/post statements, encapsulation, or full overwrite of a type definition |
| `IRuntimeConverterPlugin` | Twice (pass1 + passN) for method refs + once for pre/post JS | Emit code that doesn't belong to any user-defined type (test runners, bootstrappers) |

## Reference — `IntrestLevel` enum (sic)

Note the project misspelling — `IntrestLevel` not `InterestLevel`. Honor the existing identifier in your plugin code.

| Value | Effect |
|---|---|
| `None` | Plugin doesn't care about this method/type |
| `PreEmitStatements` | Plugin's statements run *before* the body's emitted statements |
| `PostEmitStatements` | Plugin's statements run *after* the body's emitted statements |
| `Encapsulate` | Plugin wraps the body's emitted statements (receives them as input, returns wrapped) |
| `Overwrite` | Plugin replaces the body's emitted statements entirely |

A plugin returns `IntrestLevel` from `GetInterestLevel(...)` and the matching method (`GetPreInsertionStatements`, `GetPostInsertionStatements`, `GetEncapsulationStatements`, `GetOverwrite`) is called.

## Reference — plugin registration (`PluginConfig.xml`)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Plugins>
  <Plugin Assembly="XwmlParser" ClassName="XwmlParser.XwmlTemplatingPlugin" />
  <Plugin Assembly="RazorSkinParser" ClassName="NScript.RazorSkin.RazorTemplatingPlugin" />
  <Plugin Assembly="NScript.Converter.Plugins" ClassName="NScript.Converter.Plugins.TestGenerator" />
</Plugins>
```

- `Assembly` — assembly file name (no `.dll`).
- `ClassName` — fully qualified type implementing one of the plugin interfaces.

`Cs2Jsc` ships with a default `PluginConfig.xml` registering only `XwmlTemplatingPlugin` (see `Sources/Compiler/Cs2Jsc/PluginConfig.xml`). Razor and `TestGenerator` are opt-in per project via `<PluginConfig>` in the project file. Project-specific configs replace the default.

## Reference — built-in plugins

| Plugin | Assembly | Implements | Purpose |
|---|---|---|---|
| `XwmlTemplatingPlugin` | `XwmlParser` | `IMethodConverterPlugin` + `IRuntimeConverterPlugin` | Detects `[TemplateFile(*.html)]` types, parses XWML templates, emits skin factory + binder code via `IBindingStrategy` ([ADR 0019](../adr/0019-extract-ibindingstrategy-from-skininstance.md)). Registered globally in `Cs2Jsc/PluginConfig.xml`. |
| `RazorTemplatingPlugin` | `RazorSkinParser` | `IMethodConverterPlugin` + `IRuntimeConverterPlugin` | As above for `.skin.cshtml` templates ([ADR 0017](../adr/0017-add-razor-skin-templates-as-a-second-template-frontend.md)). Registered per-project in `PluginConfig.xml`. |
| `TestGenerator` | `NScript.Converter.Plugins` | `IRuntimeConverterPlugin` | Scans for `[TestFixture]` / `[Test]` attributes from `SunlightUnit`, emits `QUnit.module()` + `QUnit.test()` calls. Opt-in per-project via `PluginConfig.xml`. See [testing/README.md](../testing/README.md). |

## Reference — JST codegen rules

These rules are mandatory ([ADR 0021](../adr/0021-require-resolved-identifiers-for-all-generated-javascript-symbols.md), reiterated in `CLAUDE.md`):

1. **All identifiers must be resolved through the scope system.** Never use raw string names for variables, fields, methods, or types. The compiler renames everything during minification. Use `RuntimeScopeManager.Resolve()`, `ResolveStatic()`, `ResolveType()`, or `ResolveFactory()`.

2. **Object literal field keys:** Use `InlineObjectInitializer.AddInitializer(IIdentifier, Expression)` with a resolved field identifier, NOT the `(string, Expression)` overload. String keys produce unminified names that don't match runtime field access.

3. **Type constructors (parameterless):** Use `ResolveType(typeDef)[0]` + `new` syntax. `ResolveFactory()` only works for constructors WITH parameters — parameterless constructors don't get factory functions in NScript.

4. **`[JsonType]` attribute:** Adds `importedExtension` wrapper on field access. Don't use for types created as object literals in codegen — the wrapper won't match. Use typed instances (`new Type()`) instead.

5. **Raw body function expressions:** If unavoidable (complex computed expressions), use `enforceSuggestion=true` on the `IdentifierScope` so parameter names match the raw body text. Prefer fully resolved JST expressions where possible.

## Quick start — a minimal `IMethodConverterPlugin`

```csharp
using System.Collections.Generic;
using NScript.CLR;
using NScript.Converter;
using NScript.Converter.TypeSystemConverter;
using NScript.JST;
using Mono.Cecil;

namespace MyApp.Plugins
{
    public class TraceLogPlugin : IMethodConverterPlugin
    {
        private RuntimeScopeManager scopes;

        public void Initialize(ClrContext clrContext, RuntimeScopeManager rsm)
        {
            this.scopes = rsm;
        }

        public void ParseArgs(IList<Tuple<string, string>> args) { }

        public IntrestLevel GetInterestLevel(MethodDefinition m, ConverterContext ctx)
        {
            return m.HasCustomAttribute("MyApp.TracedAttribute")
                ? IntrestLevel.PreEmitStatements
                : IntrestLevel.None;
        }

        public List<Statement> GetPreInsertionStatements(MethodConverter mc)
        {
            // Resolve `console.log` via the scope system, NOT a raw string
            var consoleLog = scopes.ResolveStatic("console", "log");
            var msg = new StringLiteral(mc.MethodDefinition.FullName);
            var call = new CallExpression(consoleLog, new[] { msg });
            return new List<Statement> { new ExpressionStatement(call) };
        }

        public List<Statement> GetPostInsertionStatements(MethodConverter mc) => new();
        public List<Statement> GetEncapsulationStatements(MethodConverter mc, List<Statement> body) => body;
        public List<Statement> GetOverwrite(MethodConverter mc) => null;
    }
}
```

Register in `PluginConfig.xml`:

```xml
<Plugin Assembly="MyApp.Plugins" ClassName="MyApp.Plugins.TraceLogPlugin" />
```

## Examples

### `IRuntimeConverterPlugin` — emit a bootstrap call

```csharp
public class BootstrapPlugin : IRuntimeConverterPlugin
{
    private ClrContext clrContext;
    private RuntimeScopeManager scopes;

    public void Initialize(ClrContext c, RuntimeScopeManager r) { clrContext = c; scopes = r; }
    public void ParseArgs(IList<Tuple<string, string>> args) { }

    public List<MethodReference> GetMethodsToEmitPass1() => new();
    public List<MethodReference> GetMethodsToEmitPassN() => new();
    public List<Statement> GetPreJavascript() => new();

    public List<Statement> GetPostJavascript()
    {
        // Emit a call to MyApp.AppShell.Boot() at end of file
        var bootMethod = clrContext.ResolveMethod("MyApp", "MyApp.AppShell", "Boot");
        var bootCall = new CallExpression(scopes.Resolve(bootMethod), new Expression[0]);
        return new List<Statement> { new ExpressionStatement(bootCall) };
    }
}
```

`GetMethodsToEmitPass1` / `GetMethodsToEmitPassN` let a plugin force inclusion of method references that the demand-driven DCE pass ([ADR 0022](../adr/0022-demand-driven-conversion-and-dead-code-elimination.md)) wouldn't otherwise reach. Two passes exist because pass1 contributes refs that may transitively pull in further refs — passN runs once those have been resolved.

### `ITypeConverterPlugin` — wrap a type definition

```csharp
public IntrestLevel GetInterestLevel(TypeDefinition t)
    => t.HasCustomAttribute("MyApp.WrappedAttribute") ? IntrestLevel.Encapsulate : IntrestLevel.None;

public List<Statement> GetEncapsulationStatements(TypeConverter tc, List<Statement> typeBody)
{
    // Wrap the emitted type in an IIFE
    var iife = new FunctionExpression(parameters: new[] { /*…*/ }, body: typeBody);
    var call = new CallExpression(iife, new Expression[0]);
    return new List<Statement> { new ExpressionStatement(call) };
}
```

## Known gotchas

### Plugin discovery is XML-based, not attribute-discovered

There is no `[Plugin]` attribute scan. If your `PluginConfig.xml` doesn't list the plugin, `Cs2Jsc` won't load it — even if the assembly is in the same folder. This is intentional ([CLAUDE.md](../../CLAUDE.md)) — it keeps the plugin set explicit and reproducible.

### `IntrestLevel` (sic) is the spelling

The enum is misspelled in the source. Don't "fix" it locally — that breaks every plugin in the tree. The fix would be a coordinated rename across the converter and all plugins.

### `Initialize` is called *before* any conversion happens

Stash `ClrContext` and `RuntimeScopeManager` here. Don't try to resolve types from `Initialize` — type resolution lookups depend on the converter having walked the assembly graph, which happens later. Resolve lazily in `GetInterestLevel` / `GetPreInsertionStatements`.

### `GetMethodsToEmitPass1` vs `GetMethodsToEmitPassN`

If your runtime plugin needs to force a method reference to be emitted, returning it from `GetMethodsToEmitPass1` is usually right. Use `PassN` only if your decision depends on what pass1 contributed (e.g. you need to walk the result of pass1 to compute references).

### Raw string identifiers will be silently broken

```csharp
// ❌ WRONG — minifier will rename `console`
var bad = new MemberAccess(new RawIdentifier("console"), "log");

// ✅ RIGHT
var good = scopes.ResolveStatic("console", "log");
```

The bug surfaces only at runtime (`undefined is not a function`). Always go through the scope manager. See [ADR 0021](../adr/0021-require-resolved-identifiers-for-all-generated-javascript-symbols.md).

### Object literal field keys with strings break `[JsonType]` consumers

```csharp
// ❌ WRONG — emits unminified key 'foo' that doesn't match obj.foo at runtime
ioi.AddInitializer("foo", value);

// ✅ RIGHT
ioi.AddInitializer(scopes.ResolveField(typeDef, "Foo"), value);
```

### Plugins run inside the DCE pass

If your plugin's contributed method references depend on method-body inspection, ensure the methods you're inspecting are *reachable* — DCE may have already pruned them. The pass1/passN split exists precisely to handle this.

### `Encapsulate` is rarely the right choice

`PreEmitStatements` + `PostEmitStatements` covers most cases (logging, instrumentation). Use `Encapsulate` only when you genuinely need to wrap the body (try/catch injection, IIFE wrapping). `Overwrite` is for full takeover (template-emitted methods, `[Script]` body replacement).

## Diagnostics

| Symptom | Cause |
|---|---|
| Plugin appears not to fire | Missing entry in `PluginConfig.xml`; assembly path wrong; `GetInterestLevel` returns `None` |
| `Cannot resolve type 'X'` from plugin | Resolution attempted in `Initialize` before assembly graph walk; defer to per-method/per-type entry points |
| Emitted JS calls minified name that doesn't exist | Plugin used raw string identifier instead of `RuntimeScopeManager.Resolve` |
| `Cannot find member 'foo'` at runtime | Object literal key is a string, not a resolved `IIdentifier` |
| Plugin's contributed method refs missing from output | DCE eliminated them; return them from `GetMethodsToEmitPass1` to force inclusion |

## Cross-links

- [Compiler pipeline](pipeline.md) — where plugins fit
- [ADR 0021 — Resolved identifiers](../adr/0021-require-resolved-identifiers-for-all-generated-javascript-symbols.md)
- [ADR 0022 — DCE](../adr/0022-demand-driven-conversion-and-dead-code-elimination.md)
- [Razor templates](../templates/razor.md), [XWML templates](../templates/xwml.md)
- [Testing](../testing/README.md) — `TestGenerator` plugin
- [MSBuild SDK](../build/msbuild-sdk.md) — how `PluginConfig.xml` is wired in

# ADR 0021: Require Resolved Identifiers for All Generated JavaScript Symbols

- Status: Accepted
- Date: 2026-03-29
- Deciders: NScript maintainers
- Technical Area: JST code generation, minification, identifier scope system

## Context

NScript generates JavaScript from C# source code. The generated JS must work in both Debug builds (human-readable names) and Release/retail builds (minified names). The minification pass (`IdentifierMinifiedNamer`) reassigns all identifier names to short tokens (e.g., `a`, `b`, `c_a`) based on usage frequency and scope nesting.

This system only works when every name in the generated JavaScript is represented as a resolved `IIdentifier` within the JST (JavaScript Syntax Tree). If any name is emitted as a raw string — a bare `"SetTextContent"` instead of a resolved `IIdentifier` pointing to the `SetTextContent` method — the minifier cannot rename it. The raw string will reference a name that no longer exists in the minified output, causing a runtime `ReferenceError` or `TypeError`.

This is not a theoretical risk. During the Razor skin template work (ADR-0017), raw JS string emission was the root cause of multiple test failures in Release builds. The `GraphDescriptorEmitter` initially emitted graph descriptors as raw JS strings with unresolved function names like `SetTextContent` and `SkinBinderHelper`. These worked in Debug but broke in Release because the minifier renamed the targets while the raw references stayed as-is.

Relevant paths:

- identifier interface: [Sources/Compiler/NScript.JST/IIdentifier.cs](../../Sources/Compiler/NScript.JST/IIdentifier.cs)
- scope and identifier tracking: [Sources/Compiler/NScript.JST/IdentifierScope.cs](../../Sources/Compiler/NScript.JST/IdentifierScope.cs)
- minified name assignment: [Sources/Compiler/NScript.JST/IdentifierMinifiedNamer.cs](../../Sources/Compiler/NScript.JST/IdentifierMinifiedNamer.cs)
- runtime scope resolution: [Sources/Compiler/NScript.Converter/TypeSystemConverter/RuntimeScopeManager.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/RuntimeScopeManager.cs)
- script-block resolution (related): ADR-0012

## Decision Drivers

- Generated JavaScript must work identically in Debug and Release builds
- The minifier must be able to rename every symbol — no name can be invisible to it
- Compiler plugins (XWML, Razor) must follow the same rules as the core converter
- Raw string emission should be a compile-time error or loud warning, not a silent correctness bug
- Native browser globals (`window`, `document`, `setTimeout`, `Array`, etc.) are an explicit exception — they are not NScript-controlled and must not be renamed

## Options Considered

### Option 1: Allow raw string emission with documentation warnings

Let code generators emit raw JS strings when convenient (e.g., for simple expressions), and rely on developer discipline to avoid it for names that the minifier controls.

Pros:

- Simpler code generation for simple cases
- Less ceremony in plugin code

Cons:

- Silent breakage in Release builds — the most expensive kind of bug
- No compile-time safety net
- Every plugin author must independently learn and remember which names are safe
- The Razor pipeline proved this approach fails in practice

### Option 2: Require all symbols to be resolved IIdentifiers in JST (chosen)

Every name in generated JavaScript must be represented as an `IIdentifier` registered in an `IdentifierScope`. The minifier sees and controls all names. Raw strings are only used for literal values (string content, HTML templates), never for symbol references.

Pros:

- Minification-safe by construction — if it compiles, it minifies
- Single rule, no exceptions for plugin authors to memorize
- Broken resolution fails at compile time (null identifier) rather than runtime
- Usage tracking enables dead-code elimination and optimal name assignment

Cons:

- More verbose code generation — every symbol requires a resolution call
- Computed expressions in Razor getters may need `enforceSuggestion=true` fallback when full resolution is not feasible

## Decision

All symbols emitted in generated JavaScript must be resolved through the NScript identifier scope system. No generated JavaScript may contain a bare string name that refers to an NScript-controlled symbol.

### Symbol Categories and Resolution Methods

| Symbol Category | Example | Resolution Method |
| --- | --- | --- |
| Local variables | `domStore`, `htmlRoot`, `objStorage` | `SimpleIdentifier.CreateScopeIdentifier(scope, name)` |
| Function parameters | `skinFactory`, `doc`, `dc` | `IdentifierScope` constructor with parameter names |
| Instance fields | `descriptor.NodeCount`, `state.Values` | `RuntimeScopeManager.Resolve(FieldDefinition)` |
| Instance methods (virtual) | `dc.get_name()` | `RuntimeScopeManager.Resolve(MethodDefinition)` |
| Static methods | `SetTextContent(elem, val)` | `RuntimeScopeManager.ResolveStatic(MethodDefinition)` |
| Static fields | `GraphNodeType.Source` | `RuntimeScopeManager.ResolveStatic(FieldDefinition)` |
| Type constructors (with params) | `SkinInstance_factory(...)` | `RuntimeScopeManager.ResolveFactory(MethodDefinition)` |
| Type constructors (parameterless) | `new GraphState()` | `RuntimeScopeManager.ResolveType(TypeDefinition)[0]` + `new` |
| Type references | `UISkinableElement`, `OrderViewModel` | `RuntimeScopeManager.ResolveType(TypeReference)` |
| Global functions | `DocStorageGetter(doc)` | Scope lookup by `OriginalSuggestedName` |
| Object literal field keys | `{ nodeCount: 15, getters: [...] }` | `RuntimeScopeManager.Resolve(FieldDefinition)` on the target type |
| Native browser globals | `window`, `document`, `Array` | `RawNameIdentifier.Create(scope, name)` with `enforceSuggestion=true` |

### How Identifier Resolution Works

1. **Registration**: When the converter processes a type or method, it registers identifiers in an `IdentifierScope` via `RuntimeScopeManager`. Each identifier gets a `SuggestedName` (the human-readable name) and is tracked for usage.

2. **JST construction**: Code generators build JST nodes (`IdentifierExpression`, `MethodCallExpression`, `IndexExpression`, etc.) that reference `IIdentifier` objects — never raw strings.

3. **Usage tracking**: When a JST node references an identifier, `AddUsage(scope)` records that the identifier is used in that scope. The minifier uses these counts to assign shorter names to more frequently used identifiers.

4. **Minification**: `IdentifierMinifiedNamer.MinifyNames()` walks all scopes, sorts identifiers by usage frequency, and assigns the shortest available names to the most-used identifiers. In Debug builds, names are `suggestedName_minifiedSuffix`; in Release builds, names are just the minified token.

5. **Output**: `JSWriter` calls `identifier.GetName()` which returns the final (potentially minified) name.

### The enforceSuggestion Escape Hatch

Some generated code contains raw JavaScript body text where full JST resolution is not feasible — specifically, complex computed expressions in Razor template getters (e.g., `return dc.get_price() * dc.get_quantity()`). For these cases:

- The `IdentifierScope` is created with `enforceSuggestion=true`
- Parameter identifiers (e.g., `dc`) use their suggested name as-is — the minifier will not rename them
- Property accessor names within the raw body must be pre-resolved to their minified form using field/method name maps built from Cecil metadata

This is a controlled fallback, not an exemption from the rule. The scope still participates in the identifier system; only the body text is raw. The preferred path is always full JST expression trees (`TryBuildResolvedPropertyGetter`, `TryBuildComputedJSTExpression`).

### Rules for Compiler Plugin Authors

1. **Never use string overloads for object literal field keys.** Use `InlineObjectInitializer.AddInitializer(IIdentifier, Expression)`, not `AddInitializer(string, Expression)`. String keys produce unminified names.

2. **Never emit `RawJavaScriptStatement` with NScript-controlled names.** If a statement references a function, type, or member that NScript generates, it must be a proper JST node with a resolved identifier.

3. **Use `ResolveType` for parameterless constructors, `ResolveFactory` for constructors with parameters.** Parameterless constructors do not get factory functions in NScript — `ResolveFactory` returns null for them.

4. **Native DOM APIs are safe as raw strings** (`className`, `setAttribute`, `innerHTML`, `appendChild`, etc.) because NScript does not control their names. Use `enforceSuggestion=true` on the scope containing these raw references.

5. **If resolution returns null, fail loudly.** Log a compile error or warning. Do not silently fall back to a string key — that creates a Release-only bug that is extremely hard to diagnose.

## Consequences

Positive:

- Generated JavaScript is minification-safe by construction in all code paths
- Release build breakage from unresolved names is eliminated as a bug class
- The minifier has complete visibility into all symbol usage, enabling optimal name assignment
- Plugin authors get a clear, enforceable rule rather than implicit conventions

Negative:

- Code generation is more verbose — resolving every field, method, and type reference requires Cecil lookups and scope manager calls
- The `enforceSuggestion` fallback for raw function bodies is a controlled loophole that requires discipline to use correctly
- Plugin initialization must resolve all needed types and methods upfront (e.g., `RazorKnownTypes`), adding setup complexity

## References

- Extends ADR-0012 (script-block resolution) to cover all generated JavaScript, not just `[Script]` attribute bodies
- Related: ADR-0006 (compiler pipeline), ADR-0017 (Razor templates — where this rule was violated and corrected)
- Key implementation: `RuntimeScopeManager.Resolve*()` methods, `IdentifierMinifiedNamer`, `IdentifierScope`

# ADR 0017: Add Razor Skin Templates as a Second Template Frontend

- Status: Accepted
- Date: 2026-03-29
- Deciders: NScript maintainers
- Technical Area: Template language, compiler plugin architecture, external dependencies

## Context

ADR-0016 established XWML as the canonical template language. XWML works well for simple property-path bindings but has limitations:

- No expression support — bindings are restricted to single property paths like `{PropertyName, Mode=OneWay}`. Computed values (e.g., `Price * Quantity`) require explicit converter classes.
- Manual binding mode annotation — every binding must declare `Mode=OneTime`, `Mode=OneWay`, or `Mode=TwoWay` explicitly.
- No inline control flow — `@if`/`@foreach` are not supported; reactive lists require separate `ListView` + `ListViewItem` + `Skin` setup.
- No reusable helpers — no `@functions` block for shared formatting or computed properties.
- No standard tooling — XWML is a custom XML dialect with no IDE support (no IntelliSense, no syntax highlighting).

Razor (`.cshtml`) is a well-known template syntax from the ASP.NET ecosystem with mature tooling, standard C# expression support, and a parser available as a standalone NuGet package (`Microsoft.AspNetCore.Razor.Language`).

Relevant paths:

- new compiler plugin: [Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs](../../Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs)
- Razor parser phase: [Sources/Compiler/RazorSkinParser/RazorParserPhase.cs](../../Sources/Compiler/RazorSkinParser/RazorParserPhase.cs)
- template IR: [Sources/Compiler/RazorSkinParser/TemplateIR/IRNode.cs](../../Sources/Compiler/RazorSkinParser/TemplateIR/IRNode.cs)
- existing XWML plugin: [Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs](../../Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs)

## Decision Drivers

- Enable full C# expressions in template bindings without converter boilerplate
- Auto-detect binding mode from type analysis (see ADR-0020) instead of manual annotation
- Support reactive `@if`/`@foreach` as first-class template constructs
- Leverage existing Razor tooling for partial IDE support (syntax highlighting, basic IntelliSense)
- Coexist with XWML — do not break existing templates or require migration
- Produce identical runtime output (`SkinInstance` objects) so the UI framework is unaware of which template language was used

## Options Considered

### Option 1: Extend XWML with expression and control-flow support

Add expression binding syntax (e.g., `{= Price * Quantity}`) and control-flow directives (e.g., `<xwml:if condition="...">`) to the existing XWML parser.

Pros:

- No new external dependency
- Single template language to maintain
- Existing tests and tooling continue to work

Cons:

- XWML's XML parser is not designed for embedded C# expressions — would require significant grammar changes
- Control-flow directives in XML are verbose and unintuitive
- No IDE tooling benefit — still a custom language
- Expression parsing would need a C# expression parser (effectively reimplementing part of Razor)

### Option 2: Add Razor as a second template frontend (chosen)

Introduce `.skin.cshtml` files processed by a new `RazorTemplatingPlugin` that uses `Microsoft.AspNetCore.Razor.Language` for parsing and produces the same `SkinInstance` runtime output as XWML.

Pros:

- Standard Razor syntax with full C# expression support
- Mature parser with well-defined AST (no custom grammar work)
- Partial IDE support out of the box (VS Code Razor extension)
- Clean separation — new plugin, new project, no changes to XWML
- Both template types produce identical `SkinInstance` objects at runtime

Cons:

- New external dependency (`Microsoft.AspNetCore.Razor.Language` ~2.5 MB)
- Two template languages to maintain during coexistence period
- Razor's generated C# class structure requires a Roslyn analysis phase to resolve types (see ADR-0020)
- Custom directives (`@control`, `@using` for type resolution) require pre-processing before the Razor parser

### Option 3: Replace XWML with a custom expression-aware template language

Design a new template language from scratch with expression support, control flow, and NScript-specific features.

Pros:

- Full control over syntax and semantics
- No external dependency
- Can be optimized specifically for NScript's compilation model

Cons:

- Massive engineering effort (parser, AST, code generation, error reporting)
- No existing tooling or community familiarity
- Must still solve the same problems Razor already solves

## Decision

NScript adds Razor (`.skin.cshtml`) as a second template frontend alongside XWML.

The Razor pipeline is implemented as a new compiler plugin (`RazorTemplatingPlugin`) that implements the same `IMethodConverterPlugin` + `IRuntimeConverterPlugin` interfaces as `XwmlTemplatingPlugin`. Both plugins can run simultaneously in the same compilation — the plugin system routes `[Skin("name")]` attributes to the correct plugin based on file extension:

- `.skin.cshtml` → Razor pipeline
- `.html` / `.htm` → XWML pipeline

The Razor pipeline follows a 5-phase architecture:

1. **Preprocessing** — extract `@control`, `@using` directives (non-standard Razor) before parsing
2. **Razor parsing** — `RazorProjectEngine.Process()` produces a `RazorCodeDocument` with generated C# and syntax tree
3. **Roslyn semantic analysis** — generated C# is analyzed for type information and observable classification (ADR-0020)
4. **Template IR construction** — Razor syntax tree + type info → `SkinTemplateNode` intermediate representation
5. **JST code generation** — IR → JavaScript AST nodes with scope-resolved identifiers

The runtime output is identical: both pipelines produce `SkinInstance` objects via factory methods, registered through the same `Skin` constructor.

### Template Syntax

```razor
@model Sunlight.App.ViewModels.OrderViewModel
@control Sunlight.Framework.UI.UISkinableElement

<div class="order-panel">
    <h1>@Model.CustomerName</h1>
    <span class="total">@(Model.Price * Model.Quantity)</span>

    @if (Model.IsLoading)
    {
        <div class="spinner">Loading...</div>
    }

    <button onclick="@Model.OnSubmit">Submit</button>
</div>
```

### Coexistence and Migration Path

1. **Phase 1** (current): Razor plugin works alongside XWML. New skins use Razor.
2. **Phase 2** (future): Migrate existing XWML skins to Razor (automatable for simple templates).
3. **Phase 3** (future): Deprecate and remove XWML plugin.

## Consequences

Positive:

- Template authors can use full C# expressions, eliminating converter boilerplate
- `@if`/`@foreach` replace manual `ListView` + `ListViewItem` setup for common patterns
- Binding mode is auto-detected (ADR-0020), removing a class of annotation errors
- Partial Razor IDE support (syntax highlighting, brace matching) works immediately
- Zero regression risk for existing XWML templates — the pipeline is entirely additive

Negative:

- New external dependency: `Microsoft.AspNetCore.Razor.Language` (parser-only package, not the full ASP.NET runtime)
- Two template pipelines to maintain during the coexistence period
- `@control` and type-resolution `@using` are non-standard Razor extensions handled by preprocessing, which may confuse developers expecting standard Razor behavior
- The Razor pipeline requires a Roslyn analysis phase (ADR-0020) that XWML does not need, adding compilation time for Razor templates

## References

- Supersedes nothing; extends ADR-0016 (XWML remains canonical for existing templates)
- Related: ADR-0018 (reactive binding graph), ADR-0019 (IBindingStrategy), ADR-0020 (Roslyn analysis)
- Design spec: [docs/superpowers/specs/2026-03-26-razor-skin-templates-design.md](../superpowers/specs/2026-03-26-razor-skin-templates-design.md)

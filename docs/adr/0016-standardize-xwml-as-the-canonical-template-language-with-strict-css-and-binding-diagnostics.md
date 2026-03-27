# ADR 0016: Standardize XWML as the Canonical Template Language with Strict CSS and Binding Diagnostics

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Template language, CSS matching, and binding semantics

## Context

NScript has a full template frontend in `XwmlParser`, plus an older `NScript.Template.Compiler` codepath. The active production integration is the XWML parser and code generator that plug directly into JavaScript conversion through `XwmlTemplatingPlugin`.

Relevant paths include:

- template frontend integration in [Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs](../../Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs)
- document and template parsing in [Sources/Compiler/XwmlParser/HtmlParser.cs](../../Sources/Compiler/XwmlParser/HtmlParser.cs) and [Sources/Compiler/XwmlParser/TemplateParser.cs](../../Sources/Compiler/XwmlParser/TemplateParser.cs)
- CSS parsing and stylesheet handling in [Sources/Compiler/XwmlParser/CssStyleSheet.cs](../../Sources/Compiler/XwmlParser/CssStyleSheet.cs) and [Sources/Compiler/XwmlParser/DocumentContext.cs](../../Sources/Compiler/XwmlParser/DocumentContext.cs)
- CSS class-name discovery and serialization in [Sources/Compiler/XwmlParser/CssClassNameFinderVisitor.cs](../../Sources/Compiler/XwmlParser/CssClassNameFinderVisitor.cs) and [Sources/Compiler/XwmlParser/CssSerializerVisitor.cs](../../Sources/Compiler/XwmlParser/CssSerializerVisitor.cs)
- binding grammar and binder generation in [Sources/Compiler/XwmlParser/Binding/BindingParser.cs](../../Sources/Compiler/XwmlParser/Binding/BindingParser.cs) and [Sources/Compiler/XwmlParser/Binding/BinderInfo.cs](../../Sources/Compiler/XwmlParser/Binding/BinderInfo.cs)
- CSS-name-backed static values in [Sources/Compiler/XwmlParser/StaticValues/CssNameValue.cs](../../Sources/Compiler/XwmlParser/StaticValues/CssNameValue.cs)
- final code generation and stylesheet emission in [Sources/Compiler/XwmlParser/CodeGenerator.cs](../../Sources/Compiler/XwmlParser/CodeGenerator.cs)

The older [Sources/Compiler/NScript.Template.Compiler/TemplateParser.cs](../../Sources/Compiler/NScript.Template.Compiler/TemplateParser.cs) exists, but per the translation-layer architecture in [docs/adr/0013-define-nscript-as-a-multi-frontend-translation-architecture.md](0013-define-nscript-as-a-multi-frontend-translation-architecture.md), it is not the canonical template contract unless explicitly elevated later.

## Decision Drivers

- make the active template language contract explicit
- keep template parsing, CSS naming, and binding generation aligned with one production frontend
- fail early on unresolved or mismatched symbols rather than generating partial UI code
- keep CSS name compression safe by ensuring selectors and runtime class references share the same identifier mapping

## Decision

NScript standardizes on XWML, as implemented by `XwmlParser`, as the canonical template language and binding frontend.

The template contract includes all of the following as one coherent system:

- markup parsing
- namespace-to-type resolution
- CSS parsing and stylesheet linking
- CSS class-name matching and compression
- binding syntax and binder generation
- strict diagnostics for unresolved or unsupported constructs

The older `NScript.Template.Compiler` path is not the authoritative template definition for new architecture work.

## Template Language Surface

The canonical XWML surface is:

- HTML-like template markup parsed from embedded `.html`, `.htm`, `.xhtml`, and `.xml` resources discovered by [Sources/Compiler/XwmlParser/CodeGenerator.cs](../../Sources/Compiler/XwmlParser/CodeGenerator.cs)
- stylesheet resources loaded from embedded `.css` and `.less` assets in the same resource pipeline
- root template declarations through `template` and `skin`
- required typed roots using `ControlType` and `DataContextType`, enforced by [Sources/Compiler/XwmlParser/TemplateParser.cs](../../Sources/Compiler/XwmlParser/TemplateParser.cs)
- XML namespace prefixes resolved to CLR namespaces by [Sources/Compiler/XwmlParser/DocumentContext.cs](../../Sources/Compiler/XwmlParser/DocumentContext.cs)
- typed node resolution through [Sources/Compiler/XwmlParser/HtmlParser.cs](../../Sources/Compiler/XwmlParser/HtmlParser.cs), including typed elements, attached properties, UI elements, panel types, observable types, and HTML nodes

If namespaces, types, control types, or data-context types cannot be resolved, template conversion fails.

## CSS Parsing and CSS Name Matching

CSS is part of the template contract, not an unrelated asset pipeline.

The accepted CSS model is:

- inline `<style>` blocks are parsed into `CssStyleSheet`
- linked stylesheets using `<link rel="stylesheet">` are resolved as embedded resources relative to the current template resource
- all stylesheets participate in dependency ordering and are emitted through [Sources/Compiler/XwmlParser/CodeGenerator.cs](../../Sources/Compiler/XwmlParser/CodeGenerator.cs)
- emitted CSS is post-processed through the autoprefixer integration in [Sources/Compiler/XwmlParser/CssStyleSheet.cs](../../Sources/Compiler/XwmlParser/CssStyleSheet.cs)

CSS class names participate in a shared identifier mapping:

- class selectors discovered in parsed CSS are registered to identifiers
- those identifiers are compressed once through `ParserContext.CompressCssNames`
- CSS serialization rewrites selectors using those identifiers
- runtime class-value generation for template properties and HTML attributes resolves through the same identifiers

That shared mapping is what keeps CSS selector renaming and runtime class assignment synchronized.

## CSS Strictness Rules

The canonical CSS diagnostics are strict by design:

- if a CSS class referenced from template markup or a `[CssName]` property value cannot be resolved, conversion fails
- if a local style block or linked stylesheet overwrites a class name already defined in an incompatible scope, conversion fails
- if a class name is redeclared after being shared from a previous stylesheet, the parser reports an error unless it is only being used in a nested modifier context
- CSS variables used through `var()` must be declared in `:root`; unresolved CSS variables produce a conversion error across the accumulated stylesheet set
- class names provided through `KnownCssClasses` are treated as reserved well-known names and cannot be overwritten by template-local registration

## Binding Syntax

The canonical binding syntax is the XWML binding grammar implemented by [Sources/Compiler/XwmlParser/Binding/BindingParser.cs](../../Sources/Compiler/XwmlParser/Binding/BindingParser.cs).

Bindings are recognized in both `{...}` and `[...]` forms, with escaped `{{` text support in text-node splitting logic.

The supported binding concepts are:

- `Path`
- `Mode`
- `Converter`
- `Source`

Supported binding modes are:

- `OneTime`
- `OneWay`
- `TwoWay`

Supported binding sources are:

- `DataContext` as the default
- `TemplateParent`
- a static CLR type reference

Property paths support dotted traversal and typed casts inside the path, as documented and implemented in `BindingParser`.

## Binding Functionality

The current binding system supports these target categories:

- typed property targets
- typed field targets
- HTML attribute targets
- HTML `style.<name>` targets
- HTML `class.<cssName>` boolean class toggles
- DOM event targets via `event.<name>`

The current template surface also supports static class application through the plain `class` attribute, but plain `class` does not support binding. Dynamic class binding must use `class.<cssName>`.

The plain `style` attribute is supported as static markup, but whole-attribute `style` binding is not the canonical binding surface. Dynamic style binding is per-style-property through `style.<name>`.

Method-valued source bindings are supported for delegate-like targets through method-path resolution and delegate getter generation.

Two-way binding is supported where the target binding kind and target metadata allow it. `DefaultDataBindingAttribute` may define a preferred mode, optional default value, and a strictness rule that rejects conflicting explicit binding modes.

## Current Functional Limits

This ADR records one important current limitation rather than hiding it:

- CLR event-target binding has a target-binding type in `EventTargetBindingInfo`, but its generation path is currently not implemented

So the canonical supported event-binding surface today is DOM-style event binding through `event.<name>`, not arbitrary typed CLR event target binding.

Similarly, `Source=::NamedPart` is mentioned in comments but is not part of the currently standardized supported source model.

## Strict Diagnostics for Unresolvable Symbols

The template frontend is fail-fast by default. It does not silently ignore unresolved symbols.

Template conversion must fail, or at minimum report conversion errors, when any of the following cannot be resolved correctly:

- XML namespace prefixes
- CLR types referenced in tags, `ControlType`, `DataContextType`, `Source`, or converter references
- attached properties
- typed properties or fields
- method references used for delegate binding
- converter methods and converter arguments
- CSS class names referenced from template markup or `[CssName]` properties
- linked stylesheet resources
- malformed binding syntax such as unmatched braces or invalid converter format

This strictness is architectural, not incidental. Templates are typed code-like inputs and must obey typed resolution rules.

## Consequences

Positive:

- NScript has one canonical template language contract for production work
- CSS selector compression and runtime class-value generation stay synchronized
- unresolved template, CSS, and binding symbols fail early instead of producing broken UI output
- template code generation remains aligned with the observable binding model from [docs/adr/0014-standardize-the-observable-framework-as-the-reactive-binding-contract.md](0014-standardize-the-observable-framework-as-the-reactive-binding-contract.md)

Negative:

- template authoring is stricter than loosely typed HTML templating systems
- some seemingly natural shorthand forms remain unsupported or only partially implemented
- legacy template compiler paths remain a source of conceptual duplication until explicitly retired or redefined

## References

- [docs/adr/0013-define-nscript-as-a-multi-frontend-translation-architecture.md](0013-define-nscript-as-a-multi-frontend-translation-architecture.md)
- [docs/adr/0014-standardize-the-observable-framework-as-the-reactive-binding-contract.md](0014-standardize-the-observable-framework-as-the-reactive-binding-contract.md)
- [Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs](../../Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs)
- [Sources/Compiler/XwmlParser/HtmlParser.cs](../../Sources/Compiler/XwmlParser/HtmlParser.cs)
- [Sources/Compiler/XwmlParser/TemplateParser.cs](../../Sources/Compiler/XwmlParser/TemplateParser.cs)
- [Sources/Compiler/XwmlParser/DocumentContext.cs](../../Sources/Compiler/XwmlParser/DocumentContext.cs)
- [Sources/Compiler/XwmlParser/CssStyleSheet.cs](../../Sources/Compiler/XwmlParser/CssStyleSheet.cs)
- [Sources/Compiler/XwmlParser/CssClassNameFinderVisitor.cs](../../Sources/Compiler/XwmlParser/CssClassNameFinderVisitor.cs)
- [Sources/Compiler/XwmlParser/CssSerializerVisitor.cs](../../Sources/Compiler/XwmlParser/CssSerializerVisitor.cs)
- [Sources/Compiler/XwmlParser/StaticValues/CssNameValue.cs](../../Sources/Compiler/XwmlParser/StaticValues/CssNameValue.cs)
- [Sources/Compiler/XwmlParser/Binding/BindingParser.cs](../../Sources/Compiler/XwmlParser/Binding/BindingParser.cs)
- [Sources/Compiler/XwmlParser/Binding/BinderInfo.cs](../../Sources/Compiler/XwmlParser/Binding/BinderInfo.cs)
- [Sources/Compiler/XwmlParser/Binding/PropertyTargetBindingInfo.cs](../../Sources/Compiler/XwmlParser/Binding/PropertyTargetBindingInfo.cs)
- [Sources/Compiler/XwmlParser/NodeInfos/HtmlNodeInfo.cs](../../Sources/Compiler/XwmlParser/NodeInfos/HtmlNodeInfo.cs)
- [Sources/Compiler/XwmlParser/NodeInfos/TypeNodeInfo.cs](../../Sources/Compiler/XwmlParser/NodeInfos/TypeNodeInfo.cs)
- [Sources/Compiler/NScript.Template.Compiler/TemplateParser.cs](../../Sources/Compiler/NScript.Template.Compiler/TemplateParser.cs)
# ADR 0012: Parse and Resolve Script Blocks Against Types, Members, and Known Globals

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Script block binding and minification safety

## Context

NScript allows inline JavaScript bodies through `ScriptAttribute`, but those script blocks are not treated as opaque text.

The current converter path parses script bodies into JST and resolves names against NScript's own type system, member model, and a controlled set of known globals:

- script import path in [Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs)
- script-block resolver in [Sources/Compiler/NScript.Converter/TypeSystemConverter/JsniResolver.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/JsniResolver.cs)
- parser contract in [Sources/Compiler/NScript.JSParser/IResolver.cs](../../Sources/Compiler/NScript.JSParser/IResolver.cs)
- lexical and local-scope tracking in [Sources/Compiler/NScript.JSParser/ScopeResolver.cs](../../Sources/Compiler/NScript.JSParser/ScopeResolver.cs)
- known-global registration and namespace-safe alias resolution in [Sources/Compiler/NScript.Converter/TypeSystemConverter/RuntimeScopeManager.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/RuntimeScopeManager.cs) and [Sources/Compiler/NScript.Converter/TypeSystemConverter/NamespaceManager.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/NamespaceManager.cs)

This behavior matters because inline JavaScript has to coexist with generated code, global names, namespace shaping, and minification. If script bodies were emitted as unbound raw text, NScript could accidentally collide with globals, miss dependencies, or lose track of what names must stay stable.

## Decision Drivers

- keep inline script interoperable with NScript's typed member and type model
- make script-body references minification-safe where NScript controls the names
- explicitly track the global namespace surface that script blocks are allowed to rely on
- fail fast on unresolved names rather than silently emitting broken JavaScript

## Options Considered

### Option 1: Parse and resolve script blocks into JST using compiler-aware resolution

Treat script bodies as JavaScript source that is parsed, scoped, and bound against locals, arguments, types, members, aliases, and approved globals.

Pros:

- lets script bodies participate in the same name-resolution rules as generated code
- supports stable binding to types and members even when generated names are transformed
- makes unresolved references a compiler error instead of a runtime surprise

Cons:

- requires a dedicated parser and resolver pipeline
- makes script blocks subject to compiler binding rules rather than pure raw JS freedom

### Option 2: Emit script blocks as raw JavaScript text without semantic binding

Treat the `ScriptAttribute` payload as opaque text and splice it into output as-is.

Pros:

- simplest implementation model
- maximum freedom for handwritten JavaScript

Cons:

- no reliable linkage to NScript types and members
- easy to break under renaming, namespace shaping, or minification
- unresolved names fail late at runtime

### Option 3: Allow partial parsing but tolerate unresolved names as ambient globals

Parse scripts structurally but allow unknown identifiers to pass through implicitly.

Pros:

- more permissive authoring model
- less friction when calling ad hoc globals

Cons:

- weakens safety and dependency visibility
- makes minification and namespace-collision behavior harder to reason about
- turns many authoring mistakes into runtime failures

## Decision

NScript will parse `ScriptAttribute` bodies and resolve them against:

- script-local declarations
- method arguments
- `this`
- known global identifiers explicitly registered by the runtime scope manager
- NScript types, fields, and methods resolved through `JsniResolver`
- explicit script aliases and namespace-managed global paths

Script blocks are therefore part of the compiler's semantic model, not raw text blobs.

This decision implies:

- inline JavaScript is parsed into JST before emission
- name binding must go through the parser and resolver pipeline
- NScript keeps an explicit model of approved known globals and script aliases so generated code can coexist with the global namespace without accidental capture
- references to NScript types and members are linked through compiler resolution rather than hard-coded textual assumptions

## Failure Policy

If a name in a script block cannot be resolved, NScript will fail conversion with an error.

This is already consistent with the current implementation:

- unresolved local or global identifier lookup in [Sources/Compiler/NScript.Converter/TypeSystemConverter/JsniResolver.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/JsniResolver.cs) throws an exception
- unresolved types, fields, or methods referenced through the JSNI resolution path also throw exceptions
- parse or resolution failures are wrapped by `MethodConverter.ImportJsScript` and surfaced as conversion errors

Unresolved names are not treated as acceptable ambient globals by default.

## Controlled Escape Hatch

The current implementation preserves a narrow escape hatch for explicitly raw global identifiers using the `@:` prefix handled by `JsniResolver`. That escape hatch is intentional and explicit. It does not change the default rule: unresolved names are errors.

## Minification and Namespace Implications

This design is what lets NScript take a deliberate position on the global namespace:

- names that belong to NScript-managed members, aliases, and types are resolved through compiler-owned identifiers and namespace managers
- known ambient globals are explicitly enumerated rather than inferred accidentally
- script blocks therefore do not blindly depend on whatever names happen to survive later transformations

That makes inline script substantially safer in the presence of minification and global namespace shaping than a raw-text model would be.

## Consequences

Positive:

- script blocks remain linked to NScript's type and member model
- unresolved names fail early during conversion
- the compiler retains visibility into global-name usage and namespace interactions
- minification and generated-name changes are less likely to break script blocks silently

Negative:

- script authors must work within the parser and resolver model
- adding new ambient globals requires explicit support rather than accidental usage
- some handwritten JavaScript patterns may need explicit escape hatches or resolver support

## References

- [docs/adr/0009-prefer-inline-script-attribute-for-javascript-dependencies.md](0009-prefer-inline-script-attribute-for-javascript-dependencies.md)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/JsniResolver.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/JsniResolver.cs)
- [Sources/Compiler/NScript.JSParser/IResolver.cs](../../Sources/Compiler/NScript.JSParser/IResolver.cs)
- [Sources/Compiler/NScript.JSParser/Parser.cs](../../Sources/Compiler/NScript.JSParser/Parser.cs)
- [Sources/Compiler/NScript.JSParser/ScopeResolver.cs](../../Sources/Compiler/NScript.JSParser/ScopeResolver.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/RuntimeScopeManager.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/RuntimeScopeManager.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/NamespaceManager.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/NamespaceManager.cs)
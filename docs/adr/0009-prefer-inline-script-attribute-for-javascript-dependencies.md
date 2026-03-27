# ADR 0009: Prefer Inline Script Attribute for JavaScript Dependencies

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: JavaScript dependency authoring

## Context

NScript needs a way to express JavaScript-native behavior that has no direct C# implementation body, such as calls into browser APIs, global functions, or small runtime shims.

The codebase already supports this through [Sources/Framework/mscorlib/Runtime/CompilerServices/ScriptAttribute.cs](../../Sources/Framework/mscorlib/Runtime/CompilerServices/ScriptAttribute.cs), and the converter consumes those inline JavaScript bodies in [Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs). The tests around [Test/Framework/RealScript/JsScriptImport.cs](../../Test/Framework/RealScript/JsScriptImport.cs) and [Test/Compiler/NScript.Converter.Test/MethodConverterTests/ScriptImportTests.cs](../../Test/Compiler/NScript.Converter.Test/MethodConverterTests/ScriptImportTests.cs) verify that this is a real supported path, not a side feature.

This creates an architectural choice: should NScript primarily model JavaScript dependencies through inline attributed members inside the C# type system, or through separate external JavaScript source files that are managed independently from the C# declarations?

## Decision Drivers

- keep the NScript framework and compiler self-contained
- keep JavaScript interop declarations close to the C# surface that uses them
- allow JS snippets to participate in the same naming, type, and member-resolution model as the rest of the framework
- reduce hidden coupling between C# APIs and detached JavaScript files

## Options Considered

### Option 1: Prefer inline JavaScript via `ScriptAttribute`

Store small JavaScript dependency bodies directly on extern C# members.

Pros:

- keeps interop logic colocated with the C# API surface
- makes framework packages more self-sufficient because behavior ships with the same source and assembly surface
- allows inline JS to reference resolved C# members through NScript's interpolation syntax

Cons:

- large JavaScript bodies become harder to maintain in attribute strings
- inline scripts are less convenient for standalone JS tooling

### Option 2: Prefer separate external JavaScript source files

Keep most JavaScript interop code in external JS files and bind C# declarations to them indirectly.

Pros:

- better ergonomics for large standalone JavaScript implementations
- easier to use JS-native tooling on those files directly

Cons:

- weakens the self-contained nature of the framework
- splits API declaration from runtime behavior across different assets
- makes dependency tracking and packaging less explicit

### Option 3: Only allow declarative renaming and never inline JavaScript bodies

Limit interop to `ScriptName`, `ScriptAlias`, and related mapping attributes.

Pros:

- simplest attribute model
- avoids embedding JavaScript strings in C# source

Cons:

- insufficient for behaviors that require custom JS bodies
- does not match the current framework and converter design

## Decision

NScript will prefer inline JavaScript expressed through `ScriptAttribute` for JavaScript dependency bodies that are part of the framework or runtime interop surface.

This decision means:

- small and medium JavaScript dependency bodies should live on the corresponding C# extern members rather than in detached external script files
- the primary authoring model is C# declaration first, with JavaScript embedded where needed
- the framework should remain as self-sufficient as practical, with behavior encoded in the same typed API definitions that expose it

External JavaScript files are not banned. They remain appropriate for tests, fixtures, expected-output baselines, or truly large standalone JavaScript assets. But they are not the preferred architectural mechanism for routine runtime dependencies.

## Scope and Boundaries

This ADR applies to JavaScript dependency bodies such as:

- inline browser or host interop
- native runtime helpers
- special-case methods whose semantics are best expressed directly in JS

It does not require every external JavaScript asset in the repository to be rewritten into attributes. Large libraries, generated assets, or test baselines can remain external when that is the better fit.

## Consequences

Positive:

- NScript framework code stays more self-contained
- interop behavior remains close to its declared API surface
- compiler-aware member interpolation inside script bodies remains available

Negative:

- attribute strings can become awkward for large implementations
- debugging and formatting inline JS is less convenient than working in dedicated JS files

## References

- [Sources/Framework/mscorlib/Runtime/CompilerServices/ScriptAttribute.cs](../../Sources/Framework/mscorlib/Runtime/CompilerServices/ScriptAttribute.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs)
- [Test/Framework/RealScript/JsScriptImport.cs](../../Test/Framework/RealScript/JsScriptImport.cs)
- [Test/Compiler/NScript.Converter.Test/MethodConverterTests/ScriptImportTests.cs](../../Test/Compiler/NScript.Converter.Test/MethodConverterTests/ScriptImportTests.cs)
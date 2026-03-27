# ADR 0010: Model Native JavaScript Types Through Attributed CLR Facades

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Native interop type modeling

## Context

NScript reuses many native JavaScript and host-provided types instead of reimplementing them from scratch. The framework expresses that reuse through attributed CLR facade types and members.

Representative examples include:

- imported native facades such as [Sources/Framework/mscorlib/Boolean.cs](../../Sources/Framework/mscorlib/Boolean.cs)
- extended host types such as [Sources/Framework/mscorlib/Type.cs](../../Sources/Framework/mscorlib/Type.cs)
- browser object facades such as [Sources/Framework/System.Web.Html/Element.cs](../../Sources/Framework/System.Web.Html/Element.cs) and [Sources/Framework/System.Web.Html/Window.cs](../../Sources/Framework/System.Web.Html/Window.cs)
- JSON-like data carriers marked with `JsonType`, such as entries under [Sources/Framework/System.Web/Performance.cs](../../Sources/Framework/System.Web/Performance.cs)

The attribute taxonomy used by the compiler includes `ExtendedAttribute`, `ImportedTypeAttribute`, `JsonTypeAttribute`, `ScriptNameAttribute`, `ScriptAliasAttribute`, `ImplementAttribute`, and `IntrinsicFieldAttribute`. The converter relies on these through [Sources/Compiler/NScript.Converter/ConverterContext.cs](../../Sources/Compiler/NScript.Converter/ConverterContext.cs) and [Sources/Compiler/NScript.Converter/ConverterKnownReferences.cs](../../Sources/Compiler/NScript.Converter/ConverterKnownReferences.cs).

## Decision Drivers

- reuse native runtime capabilities instead of recreating them in managed form when a direct mapping already exists
- keep C# source expressive while preserving accurate JavaScript behavior
- make the compiler's interop rules explicit and attribute-driven
- support multiple native-reuse modes instead of forcing one universal model

## Options Considered

### Option 1: Use attributed CLR facades to express different native-reuse modes

Model native JS and host types through C# types whose semantics are controlled by attributes.

Pros:

- flexible enough for imported, extended, and JSON-like scenarios
- keeps interop in the typed C# world
- lets the converter enforce different rules per category

Cons:

- attribute combinations create a more complex model
- misuse of attributes can produce subtle behavior differences

### Option 2: Use one single native-type mechanism for everything

Treat all native-facing types the same way regardless of whether they are full objects, prototype-extended objects, or plain JSON shapes.

Pros:

- simpler conceptual model at first glance

Cons:

- not expressive enough for the actual runtime patterns in the codebase
- would blur the distinction between imported objects, extended globals, and JSON data carriers

### Option 3: Avoid native facades and reimplement everything in NScript-owned runtime types

Wrap or recreate native behaviors behind fully managed abstractions.

Pros:

- more control over behavior
- less direct coupling to host object shape

Cons:

- far more runtime work
- loses the main value of direct browser and JavaScript interop
- does not match the current framework design

## Decision

NScript will continue to model native JavaScript and host-provided types through attributed CLR facades, with different attribute combinations representing different reuse modes.

The accepted reuse modes are:

- `ImportedType`: the C# type is primarily a facade over an existing native object shape
- `Extended`: the C# type augments or maps onto an existing JavaScript-native type or function model while still allowing compiler-managed members and metadata
- `JsonType`: the C# type represents plain JSON-like data rather than a behavior-rich imported object

Supporting mapping attributes refine that facade:

- `ScriptName` maps CLR names to native JS names without changing containment semantics
- `ScriptAlias` maps a member to a global alias or direct global access path
- `Implement` marks fields on extended types that NScript actually stores itself
- `IntrinsicField` and related intrinsic attributes identify members that map directly to native storage or syntax

This means native reuse in NScript is intentionally attribute-driven rather than convention-only.

## Practical Interpretation

- use `ImportedType` when the underlying object already exists in the host and NScript mainly needs a typed facade
- use `Extended` when the type should participate in NScript's runtime type model or add compiler-managed behavior while still mapping to a native JS object or constructor
- use `JsonType` when the object should behave like a JSON payload shape with field-like property semantics rather than a fully imported behavioral object

## Consequences

Positive:

- the framework can express multiple kinds of native reuse cleanly
- the converter can enforce different legality rules for each category
- interop stays explicit in source code rather than hidden in ad hoc conventions

Negative:

- the attribute model has a learning curve
- incorrect attribute choices can change runtime semantics significantly

## References

- [Sources/Framework/mscorlib/Runtime/CompilerServices/PsudoTypeAttribute.cs](../../Sources/Framework/mscorlib/Runtime/CompilerServices/PsudoTypeAttribute.cs)
- [Sources/Framework/mscorlib/Runtime/CompilerServices/ExtendedAttribute.cs](../../Sources/Framework/mscorlib/Runtime/CompilerServices/ExtendedAttribute.cs)
- [Sources/Framework/mscorlib/Runtime/CompilerServices/ScriptNameAttribute.cs](../../Sources/Framework/mscorlib/Runtime/CompilerServices/ScriptNameAttribute.cs)
- [Sources/Framework/mscorlib/Runtime/CompilerServices/ScriptAliasAttribute.cs](../../Sources/Framework/mscorlib/Runtime/CompilerServices/ScriptAliasAttribute.cs)
- [Sources/Framework/mscorlib/Runtime/CompilerServices/ImplementAttribute.cs](../../Sources/Framework/mscorlib/Runtime/CompilerServices/ImplementAttribute.cs)
- [Sources/Framework/mscorlib/Runtime/CompilerServices/IntrinsicFieldAttribute.cs](../../Sources/Framework/mscorlib/Runtime/CompilerServices/IntrinsicFieldAttribute.cs)
- [Sources/Compiler/NScript.Converter/ConverterContext.cs](../../Sources/Compiler/NScript.Converter/ConverterContext.cs)
- [Sources/Framework/mscorlib/Boolean.cs](../../Sources/Framework/mscorlib/Boolean.cs)
- [Sources/Framework/System.Web.Html/Window.cs](../../Sources/Framework/System.Web.Html/Window.cs)
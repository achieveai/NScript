# ADR 0007: Define the JavaScript Runtime Type Model

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: JavaScript runtime type system

## Context

The next major architectural boundary after the Roslyn and pipeline decisions is the JavaScript runtime type model used by the NScript converter.

The codebase already has a concrete runtime type system centered on [Sources/Framework/mscorlib/Type.cs](../../Sources/Framework/mscorlib/Type.cs). The converter then emits registration calls through [Sources/Compiler/NScript.Converter/TypeSystemConverter/TypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/TypeConverter.cs), [Sources/Compiler/NScript.Converter/TypeSystemConverter/ReferenceTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/ReferenceTypeConverter.cs), [Sources/Compiler/NScript.Converter/TypeSystemConverter/StructTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/StructTypeConverter.cs), [Sources/Compiler/NScript.Converter/TypeSystemConverter/InterfaceTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/InterfaceTypeConverter.cs), and [Sources/Compiler/NScript.Converter/TypeSystemConverter/EnumTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/EnumTypeConverter.cs).

This runtime model determines how classes, interfaces, structs, and enums are represented in JavaScript and what metadata later features such as `typeof`, casting, IoC, event routing, and interface checks rely on.

## Decision Drivers

- keep runtime type identity explicit and stable in JavaScript
- support class, interface, struct, and enum concepts with one coherent runtime model
- preserve enough metadata for casts, assignability checks, factories, and reflection-like use cases
- align emitted converter output with the existing `System.Type` runtime surface

## Options Considered

### Option 1: Use constructor functions as the runtime type object and attach .NET-like metadata directly to them

Represent runtime types as JavaScript function objects with metadata fields such as full name, base type, type id, and interface lists.

Pros:

- matches the existing `System.Type` runtime design
- integrates naturally with prototype-based JavaScript instances
- keeps type identity and construction closely aligned

Cons:

- mixes callable constructor behavior with type metadata
- requires discipline around how metadata fields are populated and consumed

### Option 2: Separate runtime type metadata from constructors entirely

Create a distinct metadata object for every type and keep constructors separate.

Pros:

- cleaner conceptual separation between construction and type information
- could simplify some reflection-style operations

Cons:

- would require redesigning the existing runtime and converter output
- complicates places that currently assume the constructor is the type object

### Option 3: Use only JavaScript prototype and `instanceof` behavior without explicit type metadata

Lean on native JavaScript object identity and avoid a richer NScript runtime type model.

Pros:

- simplest runtime surface in theory
- reduces custom metadata

Cons:

- insufficient for interface checks, type ids, enums, structs, and .NET-like runtime semantics
- does not match current NScript runtime requirements

## Decision

NScript standardizes on a JavaScript runtime type model in which the emitted type object is the primary runtime identity, and for reference types that object is the constructor function itself.

The accepted model is:

- runtime types are represented by JavaScript values that act as `System.Type`-compatible type objects
- for classes and interfaces, the runtime type object is a JavaScript function object, consistent with [Sources/Framework/mscorlib/Type.cs](../../Sources/Framework/mscorlib/Type.cs) being marked with `ScriptName("Function")`
- metadata is stored directly on the runtime type object rather than in a detached registry-only structure

The standard metadata fields are:

- `FullName`
- `TypeId`
- `BaseType`
- `IsClass`
- `IsStruct`
- `IsEnum`
- `IsInterface`
- `IsNullable`
- `Prototype`
- `interfaces`
- `baseInterfaces`

The standard registration shapes are:

- reference types register with `(typeName, parentType, interfaces)`
- struct types register with `(typeName, interfaces)`
- interface types register with `(typeName)`
- enum types register with `(typeName, isFlag)`

This means NScript's runtime type model is intentionally richer than raw JavaScript constructor identity. It is a .NET-inspired type representation implemented on top of JavaScript objects and functions.

## Type Categories

Within this runtime model:

- classes carry constructor behavior, prototype state, base-type metadata, and interface metadata
- structs are runtime types with value-type semantics layered on top of JavaScript objects and boxed values
- interfaces are runtime type descriptors used for type relationship checks and registration metadata rather than for direct instance construction
- enums are runtime types with both enum metadata and value-to-name or name-to-value maps

## Consequences

Positive:

- the runtime has a single explicit place to store type identity and hierarchy metadata
- class construction and runtime type identity remain aligned
- features like `TypeId`, `BaseType`, enum registration, and interface checks have a stable contract

Negative:

- the runtime model is more opinionated and complex than plain JavaScript constructors
- converter and runtime code must stay synchronized on metadata field names and registration signatures

## References

- [Sources/Framework/mscorlib/Type.cs](../../Sources/Framework/mscorlib/Type.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/TypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/TypeConverter.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/ReferenceTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/ReferenceTypeConverter.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/StructTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/StructTypeConverter.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/InterfaceTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/InterfaceTypeConverter.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/EnumTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/EnumTypeConverter.cs)
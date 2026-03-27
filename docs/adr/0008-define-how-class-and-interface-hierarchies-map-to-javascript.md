# ADR 0008: Define How Class and Interface Hierarchies Map to JavaScript

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: JavaScript inheritance and interface semantics

## Context

ADR 0007 records the JavaScript runtime type model. The next decision is how NScript maps .NET class and interface hierarchies onto that model.

The converter already implements a concrete mapping strategy:

- inheritance order is resolved through [Sources/Compiler/NScript.Converter/DependencyBuilder/InheritanceDependencyBuilder.cs](../../Sources/Compiler/NScript.Converter/DependencyBuilder/InheritanceDependencyBuilder.cs)
- class prototype setup occurs in [Sources/Compiler/NScript.Converter/TypeSystemConverter/TypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/TypeConverter.cs)
- reference-type registration passes parent type and implemented interfaces in [Sources/Compiler/NScript.Converter/TypeSystemConverter/ReferenceTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/ReferenceTypeConverter.cs)
- interface registration is intentionally lightweight in [Sources/Compiler/NScript.Converter/TypeSystemConverter/InterfaceTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/InterfaceTypeConverter.cs)
- runtime interface instance checks are implemented through `baseInterfaces` in [Sources/Framework/mscorlib/Type.cs](../../Sources/Framework/mscorlib/Type.cs)

This behavior is architectural because it decides what "inheritance" and "implements" mean at runtime in emitted JavaScript.

## Decision Drivers

- preserve familiar .NET class inheritance semantics where practical
- support interface membership checks at runtime
- keep emitted JavaScript compatible with the existing `System.Type` runtime logic
- avoid making interfaces depend on JavaScript prototype inheritance

## Options Considered

### Option 1: Use JavaScript prototype inheritance for classes and metadata-based membership for interfaces

Map class inheritance through constructor and prototype wiring, and represent interfaces as metadata relationships checked separately from prototype inheritance.

Pros:

- fits JavaScript's natural class and prototype model
- avoids pretending interfaces are concrete prototype carriers
- matches the current `System.Type.IsInstanceOfType` behavior

Cons:

- requires separate logic for class inheritance and interface membership
- interface metadata must be maintained correctly for assignability checks

### Option 2: Treat interfaces like prototype parents as well

Attempt to model interfaces in the same structural way as class prototype inheritance.

Pros:

- one apparent mechanism for all hierarchy relationships

Cons:

- does not fit JavaScript's single-prototype model well
- complicates or distorts .NET interface semantics
- does not match current runtime behavior

### Option 3: Use metadata-only relationships for both classes and interfaces

Avoid meaningful prototype inheritance and represent the full hierarchy only in metadata tables.

Pros:

- makes hierarchy queries uniform in one sense
- reduces reliance on prototype construction for inheritance setup

Cons:

- loses the advantages of JavaScript's prototype-based instance dispatch for classes
- would require redesigning current converter output

## Decision

NScript will map class and interface hierarchies to JavaScript using two complementary mechanisms:

- classes use constructor and prototype inheritance semantics
- interfaces use explicit runtime metadata and membership maps

The accepted class mapping is:

- a reference type may register an immediate `BaseType`
- if a non-extended, non-pseudo, non-`System.Object` base type exists, the derived type prototype is initialized from a new base-type instance
- the converter then assigns instance members and virtual mappings on the derived prototype
- class registration records both the immediate base type and the implemented interface list

The accepted interface mapping is:

- interfaces are registered as runtime type descriptors, not as concrete prototype parents
- interface converters do not emit prototype initializers or virtual initializers of their own
- implemented interfaces are recorded in type metadata and used later for runtime membership checks
- runtime interface checks are performed through the lazily built `baseInterfaces` map in `System.Type`, not by `instanceof`

## Runtime Membership Semantics

At runtime:

- non-interface `IsInstanceOfType` checks use JavaScript class identity semantics, specifically `instance instanceof this` or constructor equality
- interface `IsInstanceOfType` checks rely on the `baseInterfaces` metadata map associated with the instance constructor
- `baseInterfaces` is built by combining direct interfaces with inherited interfaces from the base type chain

This means NScript treats interface implementation as runtime metadata that is transitive through the class hierarchy, while class inheritance remains prototype-based.

## Ordering Semantics

NScript must continue to initialize hierarchy relationships in dependency order:

- base classes before derived classes
- base interfaces before types that depend on them

The existing dependency ordering in [Sources/Compiler/NScript.Converter/DependencyBuilder/InheritanceDependencyBuilder.cs](../../Sources/Compiler/NScript.Converter/DependencyBuilder/InheritanceDependencyBuilder.cs) is therefore part of the accepted hierarchy model.

## Non-Goals and Current Limits

- imported or pseudo-interface variants are not expanded by this ADR; current converter behavior still rejects unsupported imported-interface cases
- this ADR does not define virtual dispatch slot mapping in detail; that belongs in a later ADR focused on virtual and interface method dispatch

## Consequences

Positive:

- class inheritance stays aligned with JavaScript runtime behavior
- interface checks remain possible without abusing the prototype chain
- base-type and interface relationships stay explicit and queryable through runtime metadata

Negative:

- class inheritance and interface membership use different underlying mechanisms
- subtle bugs are possible if interface metadata is not kept in sync with emitted type registration

## References

- [docs/adr/0007-define-the-javascript-runtime-type-model.md](0007-define-the-javascript-runtime-type-model.md)
- [Sources/Framework/mscorlib/Type.cs](../../Sources/Framework/mscorlib/Type.cs)
- [Sources/Compiler/NScript.Converter/DependencyBuilder/InheritanceDependencyBuilder.cs](../../Sources/Compiler/NScript.Converter/DependencyBuilder/InheritanceDependencyBuilder.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/TypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/TypeConverter.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/ReferenceTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/ReferenceTypeConverter.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/InterfaceTypeConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/InterfaceTypeConverter.cs)
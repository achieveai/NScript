# ADR 0011: Treat Arrays as a Special Wrapped Runtime Substrate

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Array runtime and conversion semantics

## Context

Arrays in NScript are an intentional exception to many otherwise regular type-conversion rules.

The codebase already treats arrays specially across multiple layers:

- framework surface types in [Sources/Framework/mscorlib/Array.cs](../../Sources/Framework/mscorlib/Array.cs)
- internal wrapper implementation in [Sources/Framework/mscorlib/InternalArrayImpl.cs](../../Sources/Framework/mscorlib/InternalArrayImpl.cs)
- direct native-array facade in [Sources/Framework/mscorlib/NativeArray.cs](../../Sources/Framework/mscorlib/NativeArray.cs)
- compiler-side type fixing and known references in [Sources/Compiler/NScript.Converter/ConverterKnownReferences.cs](../../Sources/Compiler/NScript.Converter/ConverterKnownReferences.cs)
- member remapping from `System.Array` to `ArrayImpl` in [Sources/Compiler/NScript.Converter/ExpressionsConverter/MemberReferenceConverter.cs](../../Sources/Compiler/NScript.Converter/ExpressionsConverter/MemberReferenceConverter.cs)
- wrapper and extraction behavior in [Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs)
- default value behavior in [Sources/Compiler/NScript.Converter/ExpressionsConverter/DefaultValueConverter.cs](../../Sources/Compiler/NScript.Converter/ExpressionsConverter/DefaultValueConverter.cs)

This is not accidental complexity. Arrays sit at the boundary between .NET array semantics and JavaScript native array behavior.

## Decision Drivers

- preserve .NET-style array typing and framework APIs where practical
- reuse JavaScript native arrays for performance and interop convenience
- keep array access and common operations close to native JavaScript behavior
- isolate array special-casing instead of letting it leak unpredictably into all type logic

## Options Considered

### Option 1: Treat arrays as wrapped runtime types backed by native arrays

Model `T[]` as a managed-facing array abstraction that internally wraps a `NativeArray<T>` through `ArrayG<T>` and related converter special cases.

Pros:

- preserves a .NET-facing array API surface
- still reuses native JS arrays underneath
- matches the current compiler and runtime design

Cons:

- requires many targeted exceptions in compiler logic
- arrays no longer behave exactly like ordinary reference types or ordinary imported types

### Option 2: Treat arrays as plain native JavaScript arrays everywhere

Erase most wrapper semantics and use native arrays directly as the only model.

Pros:

- simpler JavaScript runtime representation
- fewer wrappers in principle

Cons:

- loses the .NET array abstraction expected by the framework and converter
- complicates typed member mapping and runtime APIs such as `System.Array`

### Option 3: Treat arrays like ordinary NScript reference types

Make arrays follow the same registration and prototype rules as normal types.

Pros:

- more uniform type model

Cons:

- does not fit native JS array behavior well
- would add overhead and complexity for a foundational runtime primitive

## Decision

NScript will continue to treat arrays as a special wrapped runtime substrate.

The accepted model is:

- C# array types remain first-class CLR-visible types in the compiler model
- the converter rewrites array types to `ArrayG<T>`-style runtime wrappers where needed
- the runtime wrapper stores and operates over a `NativeArray<T>` backing store
- array-related member references from `System.Array` are remapped to the corresponding `ArrayImpl` operations during conversion
- arrays are considered wrapped types by the converter and therefore participate in special wrapper and extraction logic when crossing imported or extended boundaries

## Practical Semantics

This means arrays are exceptional in at least these ways:

- they are not treated like ordinary reference-type registrations in the JavaScript runtime type model
- they have special default-value behavior, with `default(T[])` becoming `null`
- they use conversion hooks to bridge between managed array wrappers and native arrays
- array APIs are partially represented by `System.Array`, partially by `ArrayImpl`, and physically backed by `NativeArray`

NScript therefore treats arrays as a hybrid of:

- CLR-visible typed arrays
- runtime wrapper objects
- native JavaScript arrays

## Consequences

Positive:

- the framework keeps a .NET-like array programming model
- emitted JavaScript can still rely on efficient native array behavior underneath
- imported and native interop can bridge arrays without collapsing the managed abstraction entirely

Negative:

- arrays require pervasive special-casing in compiler and runtime code
- array behavior is less regular than other types and must be documented explicitly
- future work on lists, spans, or collection interop must account for the array exception model

## References

- [Sources/Framework/mscorlib/Array.cs](../../Sources/Framework/mscorlib/Array.cs)
- [Sources/Framework/mscorlib/InternalArrayImpl.cs](../../Sources/Framework/mscorlib/InternalArrayImpl.cs)
- [Sources/Framework/mscorlib/NativeArray.cs](../../Sources/Framework/mscorlib/NativeArray.cs)
- [Sources/Compiler/NScript.Converter/ConverterKnownReferences.cs](../../Sources/Compiler/NScript.Converter/ConverterKnownReferences.cs)
- [Sources/Compiler/NScript.Converter/ExpressionsConverter/MemberReferenceConverter.cs](../../Sources/Compiler/NScript.Converter/ExpressionsConverter/MemberReferenceConverter.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs)
- [Sources/Compiler/NScript.Converter/ExpressionsConverter/DefaultValueConverter.cs](../../Sources/Compiler/NScript.Converter/ExpressionsConverter/DefaultValueConverter.cs)
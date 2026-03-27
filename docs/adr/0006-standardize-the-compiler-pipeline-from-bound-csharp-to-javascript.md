# ADR 0006: Standardize the Compiler Pipeline from Bound C# to JavaScript

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Compiler pipeline architecture

## Context

NScript's compiler architecture crosses several representations and artifacts rather than doing a single in-memory source-to-source transform.

The current codebase already follows a concrete pipeline:

- Roslyn binds C# code and exposes bound method bodies through the custom fork hooks
- `NScript.Csc.Lib` converts those bound method bodies into NScript's own serialized AST model
- the serialized AST is embedded into the emitted DLL as a resource
- `cs2jsc` later loads the emitted DLLs, extracts the AST resources, reconstructs NScript CLR AST, and generates JavaScript from that reconstructed model plus assembly metadata

This flow is important enough to record explicitly because it defines the authoritative handoff between compilation and JavaScript generation.

Relevant code paths include:

- bound-body capture in [Sources/Compiler/NScript.Csc.Lib/SerializationHelper.cs](../../Sources/Compiler/NScript.Csc.Lib/SerializationHelper.cs)
- resource injection and emit in [Sources/Compiler/NScript.Csc.Lib/Csc.cs](../../Sources/Compiler/NScript.Csc.Lib/Csc.cs)
- assembly loading and resource gating in [Sources/Compiler/NScript.CLR/ClrContext.cs](../../Sources/Compiler/NScript.CLR/ClrContext.cs)
- AST extraction in [Sources/Compiler/NScript.Converter/ConverterContext.cs](../../Sources/Compiler/NScript.Converter/ConverterContext.cs)
- AST reconstruction in [Sources/Compiler/JsCsc.Lib/BondToAst.cs](../../Sources/Compiler/JsCsc.Lib/BondToAst.cs)
- JavaScript generation in [Sources/Compiler/NScript.Converter/Builder.cs](../../Sources/Compiler/NScript.Converter/Builder.cs)

## Decision Drivers

- keep the compiler pipeline explicit from Roslyn capture through JavaScript output
- preserve the emitted DLL as the durable handoff artifact between stages
- avoid coupling JavaScript generation directly to live Roslyn compilation objects
- keep AST, resources, and emitted assembly metadata aligned

## Options Considered

### Option 1: Treat emitted DLLs with embedded AST resources as the authoritative handoff artifact

Capture bound bodies during compilation, embed the serialized AST in the emitted assembly, and later generate JavaScript by loading the DLLs and extracting those resources.

Pros:

- clearly separates C# compilation from JavaScript conversion
- keeps later stages independent of live Roslyn state
- ties serialized AST and assembly metadata to the same compiled artifact

Cons:

- requires a multi-stage pipeline
- introduces a serialization and deserialization contract that must be preserved

### Option 2: Generate JavaScript directly from the Roslyn compilation in-memory

Perform JavaScript generation immediately during Roslyn compilation without using emitted DLL resources as a handoff.

Pros:

- fewer artifact transitions
- no later AST extraction step

Cons:

- couples JavaScript generation tightly to the Roslyn host process
- makes downstream conversion and testing less independent
- does not match the current assembly-based conversion model

### Option 3: Infer everything later only from IL and metadata

Emit normal assemblies and reconstruct the needed model without embedding serialized AST resources.

Pros:

- removes custom AST resources from the emitted DLLs
- simplifies the output artifact shape conceptually

Cons:

- loses the bound-body information that motivated the Roslyn fork
- forces downstream stages to recover meaning from later compiler artifacts instead of using captured semantic form

## Decision

NScript standardizes on a two-stage compiler pipeline in which the authoritative handoff artifact is the compiled DLL with embedded serialized AST resources.

The accepted production pipeline is:

1. C# source is compiled by the custom Roslyn-based compiler entry point in `NScript.Csc.Lib`.
2. `Csc.OnBeforeCompilation` installs the resource injection step before emit.
3. `SerializationHelper` registers `CSharpCompilation.OnBoundExpressionGenerated` and captures each bound method body and any initializers exposed by Roslyn.
4. `BoundAstToAstBase` and `SymbolSerializer` translate Roslyn's bound representation into NScript's serialized AST model.
5. The resulting `FullAst` payload is serialized and embedded into the emitted assembly as `$$BstInfo$$`.
6. Resource-file metadata needed later by the converter is embedded as `$$ResInfo$$`.
7. Roslyn emits the DLL and PDB. That emitted DLL is the durable handoff artifact for the next stage.
8. `cs2jsc`, via [Sources/Compiler/NScript.Lib/CommandLine.cs](../../Sources/Compiler/NScript.Lib/CommandLine.cs) and [Sources/Compiler/NScript.Converter/Builder.cs](../../Sources/Compiler/NScript.Converter/Builder.cs), loads the main assembly and references through `ClrContext`.
9. `ClrContext` only retains assemblies that contain `$$BstInfo$$`, which defines the AST-bearing assemblies that participate in conversion.
10. `ConverterContext` extracts `$$BstInfo$$` and `$$ResInfo$$` from each loaded module.
11. `BondToAst` reconstructs NScript CLR AST from the extracted serialized method bodies, resolving members and modules through Mono.Cecil-backed assembly metadata.
12. `RuntimeScopeManager` and the converter pipeline analyze dependencies, convert the reconstructed AST into JST statements, and `JSWriter` emits the final JavaScript output.

JavaScript generation is therefore defined as operating on compiled assemblies plus embedded AST resources, not directly on raw source files.

## Additional Clarifications

- The captured representation is not a Roslyn syntax tree dump. It is an NScript-owned serialized AST derived from Roslyn's bound method bodies.
- The emitted DLL remains essential even after AST capture because later stages resolve methods, types, references, and resources through assembly metadata.
- The resource extraction step is an intentional part of the architecture, not an implementation accident.
- Legacy or test-oriented paths such as `JsCsc.Lib.DriverWrapper` may still exist, but the authoritative production pipeline is the Roslyn capture -> DLL/resource handoff -> `cs2jsc` extraction and JavaScript generation path recorded here.

## Consequences

Positive:

- the stage boundary between compilation and JavaScript generation is explicit
- serialized AST and emitted metadata stay synchronized inside one artifact
- downstream conversion can operate from DLLs and resources without keeping Roslyn alive

Negative:

- serialization compatibility becomes part of the compiler contract
- debugging pipeline issues may require inspecting both metadata and embedded resources
- the overall system is more complex than a direct source-to-source transpiler

## References

- [docs/adr/0005-constrain-nscript-to-a-small-roslyn-integration-contract.md](0005-constrain-nscript-to-a-small-roslyn-integration-contract.md)
- [Sources/Compiler/NScript.Csc.Lib/Csc.cs](../../Sources/Compiler/NScript.Csc.Lib/Csc.cs)
- [Sources/Compiler/NScript.Csc.Lib/SerializationHelper.cs](../../Sources/Compiler/NScript.Csc.Lib/SerializationHelper.cs)
- [Sources/Compiler/NScript.Csc.Lib/BoundAstToAstBase.cs](../../Sources/Compiler/NScript.Csc.Lib/BoundAstToAstBase.cs)
- [Sources/Compiler/NScript.CLR/ClrContext.cs](../../Sources/Compiler/NScript.CLR/ClrContext.cs)
- [Sources/Compiler/NScript.Converter/ConverterContext.cs](../../Sources/Compiler/NScript.Converter/ConverterContext.cs)
- [Sources/Compiler/JsCsc.Lib/BondToAst.cs](../../Sources/Compiler/JsCsc.Lib/BondToAst.cs)
- [Sources/Compiler/NScript.Converter/Builder.cs](../../Sources/Compiler/NScript.Converter/Builder.cs)
- [Sources/Compiler/NScript.Lib/CommandLine.cs](../../Sources/Compiler/NScript.Lib/CommandLine.cs)
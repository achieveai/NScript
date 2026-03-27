# ADR 0013: Define NScript as a Multi-Frontend Translation Architecture

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Translation-layer architecture

## Context

NScript is not a single source-to-source transpiler. The codebase already contains multiple translation frontends and multiple intermediate representations, all of which feed the final JavaScript output at different points.

The current architecture includes at least these translation paths:

- C# compilation and bound-body capture through [Sources/Compiler/NScript.Csc.Lib/Csc.cs](../../Sources/Compiler/NScript.Csc.Lib/Csc.cs) and [Sources/Compiler/NScript.Csc.Lib/BoundAstToAstBase.cs](../../Sources/Compiler/NScript.Csc.Lib/BoundAstToAstBase.cs)
- serialized AST reconstruction through [Sources/Compiler/JsCsc.Lib/BondToAst.cs](../../Sources/Compiler/JsCsc.Lib/BondToAst.cs)
- JavaScript generation through [Sources/Compiler/NScript.Converter/Builder.cs](../../Sources/Compiler/NScript.Converter/Builder.cs) and `RuntimeScopeManager`
- inline JavaScript parsing through [Sources/Compiler/NScript.JSParser/Parser.cs](../../Sources/Compiler/NScript.JSParser/Parser.cs) and [Sources/Compiler/NScript.Converter/TypeSystemConverter/JsniResolver.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/JsniResolver.cs)
- template and skin translation through [Sources/Compiler/XwmlParser/TemplateParser.cs](../../Sources/Compiler/XwmlParser/TemplateParser.cs), [Sources/Compiler/XwmlParser/CodeGenerator.cs](../../Sources/Compiler/XwmlParser/CodeGenerator.cs), and [Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs](../../Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs)
- older or alternate frontends such as [Sources/Compiler/JsCsc.Lib/AstConverter.cs](../../Sources/Compiler/JsCsc.Lib/AstConverter.cs) and [Sources/Compiler/NScript.Template.Compiler/TemplateParser.cs](../../Sources/Compiler/NScript.Template.Compiler/TemplateParser.cs)

Without an explicit architectural decision, it is too easy to describe the system as if there were one pipeline, when in practice there are multiple frontends converging on shared runtime and backend layers.

## Decision Drivers

- make the actual translation boundaries explicit
- define where different source languages and source-like inputs enter the system
- clarify which intermediate representations are authoritative and what semantics each layer owns
- separate production-authoritative layers from legacy or auxiliary ones

## Options Considered

### Option 1: Treat NScript as a multi-frontend layered translation architecture

Define the system as several frontends feeding shared intermediate and backend layers.

Pros:

- matches the real codebase
- makes extension points and convergence points explicit
- helps future ADRs place responsibilities at the right layer

Cons:

- more complex than describing NScript as a single compiler pipeline
- requires explicit distinction between canonical and auxiliary paths

### Option 2: Describe NScript as one canonical compiler pipeline only

Reduce the architectural story to the C#-to-JS production pipeline.

Pros:

- simpler explanation
- works for the main Roslyn-driven path

Cons:

- hides the inline-JS and template frontends
- makes plugin-generated code and script parsing look accidental instead of architectural

### Option 3: Treat each translator as an independent subsystem without a shared architectural model

Allow C#, script, and template translation paths to remain only loosely related.

Pros:

- minimal upfront architectural framing

Cons:

- obscures shared runtime and backend contracts
- makes it harder to reason about ownership of semantics, naming, and diagnostics

## Decision

NScript is defined as a multi-frontend translation architecture with explicit convergence layers.

The accepted layer model is:

1. **Source frontends**
   - C# frontend via Roslyn bound-body capture
   - inline JavaScript frontend for `ScriptAttribute` bodies
   - template frontend for XWML or skin resources

2. **Frontend-specific semantic layers**
   - Roslyn bound nodes and symbols for C#
   - JS parser plus `JsniResolver` for inline JavaScript
   - template parsers and type resolvers for template resources

3. **NScript-owned intermediate layers**
   - serialized AST resources and NScript CLR AST for the C# path
   - JST statements and expressions as the shared JavaScript-oriented output IR

4. **Shared backend and runtime integration layers**
   - converter context, runtime scope management, dependency analysis, naming, alias resolution, and minification-sensitive symbol control
   - final JavaScript emission through `JSWriter`

The architecture therefore has multiple frontends but shared runtime and emission contracts.

## Authoritative Convergence Points

The authoritative convergence points are:

- **compiled assembly plus embedded resources** as the handoff artifact for C# compilation, as already recorded in [docs/adr/0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md](0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md)
- **JST** as the backend-facing JavaScript representation used before final emission
- **RuntimeScopeManager and related converter services** as the shared naming, dependency, and symbol-shaping layer across generated JavaScript

Not every frontend uses every IR. That is intentional.

- C# travels through Roslyn bound nodes, serialized AST, and CLR AST before reaching JST
- inline JavaScript enters later, after being parsed and resolved directly into JST-oriented structures
- template translation produces JST-level statements through the converter plugin model rather than flowing through the C# serialized AST path

## Layer Ownership

The architectural ownership is:

- Roslyn owns C# language binding and bound semantic form
- `NScript.Csc.Lib` owns translation from Roslyn bound form into NScript's serialized AST
- `BondToAst` and NScript CLR AST own the reconstructed compiler-neutral semantic representation for the C# pipeline
- JS parser plus `JsniResolver` own parsing and binding of inline script blocks
- XWML parser and template code generation own typed template-resource translation
- converter runtime layers own naming, dependency analysis, type mapping, alias handling, and final JavaScript shaping

## Production vs Auxiliary Paths

The production-authoritative path remains the Roslyn-based C# pipeline recorded in ADR 0006.

The inline-JS and XWML template paths are also production architecture, but they are auxiliary frontends that join later in the pipeline.

Older or alternate components such as [Sources/Compiler/JsCsc.Lib/AstConverter.cs](../../Sources/Compiler/JsCsc.Lib/AstConverter.cs) and [Sources/Compiler/NScript.Template.Compiler/TemplateParser.cs](../../Sources/Compiler/NScript.Template.Compiler/TemplateParser.cs) are not elevated by this ADR to equal production-authoritative status unless a later ADR explicitly does so.

## Consequences

Positive:

- future ADRs can discuss C#, script, observables, and templating without pretending they all enter at the same layer
- extension points such as converter plugins fit into an explicit architecture rather than looking ad hoc
- semantics ownership becomes clearer at each boundary

Negative:

- the architecture is more complex to explain than a single compiler pipeline
- multiple frontends mean more care is needed to keep shared runtime and naming rules consistent

## References

- [docs/adr/0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md](0006-standardize-the-compiler-pipeline-from-bound-csharp-to-javascript.md)
- [Sources/Compiler/NScript.Csc.Lib/Csc.cs](../../Sources/Compiler/NScript.Csc.Lib/Csc.cs)
- [Sources/Compiler/NScript.Csc.Lib/BoundAstToAstBase.cs](../../Sources/Compiler/NScript.Csc.Lib/BoundAstToAstBase.cs)
- [Sources/Compiler/JsCsc.Lib/BondToAst.cs](../../Sources/Compiler/JsCsc.Lib/BondToAst.cs)
- [Sources/Compiler/NScript.Converter/Builder.cs](../../Sources/Compiler/NScript.Converter/Builder.cs)
- [Sources/Compiler/NScript.JSParser/Parser.cs](../../Sources/Compiler/NScript.JSParser/Parser.cs)
- [Sources/Compiler/NScript.Converter/TypeSystemConverter/JsniResolver.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/JsniResolver.cs)
- [Sources/Compiler/XwmlParser/TemplateParser.cs](../../Sources/Compiler/XwmlParser/TemplateParser.cs)
- [Sources/Compiler/XwmlParser/CodeGenerator.cs](../../Sources/Compiler/XwmlParser/CodeGenerator.cs)
- [Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs](../../Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs)
- [Sources/Compiler/JsCsc.Lib/AstConverter.cs](../../Sources/Compiler/JsCsc.Lib/AstConverter.cs)
- [Sources/Compiler/NScript.Template.Compiler/TemplateParser.cs](../../Sources/Compiler/NScript.Template.Compiler/TemplateParser.cs)
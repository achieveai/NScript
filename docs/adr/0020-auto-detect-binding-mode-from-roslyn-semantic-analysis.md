# ADR 0020: Auto-Detect Binding Mode from Roslyn Semantic Analysis

- Status: Accepted
- Date: 2026-03-29
- Deciders: NScript maintainers
- Technical Area: Compiler pipeline, template binding semantics

## Context

In XWML templates, every binding must explicitly declare its mode:

```xml
<span>{Name, Mode=OneWay}</span>
<span>{AppVersion, Mode=OneTime}</span>
```

This is error-prone: forgetting `Mode=OneWay` on an observable property silently produces a static binding that never updates. Conversely, using `Mode=OneWay` on a non-observable property wastes a listener registration.

The information needed to determine binding mode already exists in the type system — `ObservableObject` subclasses fire `PropertyChanged`, plain types do not. The compiler can analyze this at build time instead of requiring manual annotation.

Relevant paths:

- Roslyn analysis phase: [Sources/Compiler/RazorSkinParser/RoslynAnalysisPhase.cs](../../Sources/Compiler/RazorSkinParser/RoslynAnalysisPhase.cs)
- observable analyzer: [Sources/Compiler/RazorSkinParser/ObservableAnalyzer.cs](../../Sources/Compiler/RazorSkinParser/ObservableAnalyzer.cs)
- binding classification: [Sources/Compiler/RazorSkinParser/TemplateIR/BindingClassification.cs](../../Sources/Compiler/RazorSkinParser/TemplateIR/BindingClassification.cs)
- observable framework contract: ADR-0014

## Decision Drivers

- Eliminate manual binding mode annotations and their associated error class
- Leverage the observable framework contract (ADR-0014) — types that inherit `ObservableObject` or implement `INotifyPropertyChanged` are observable by definition
- Support computed expressions (`@(Model.Price * Model.Quantity)`) where multiple observable properties contribute to a single binding
- Keep the analysis at compile time — no runtime cost
- Produce the same binding quality as hand-annotated XWML (no over-subscription, no missed subscriptions)

## Options Considered

### Option 1: Convention-based detection (property name patterns)

Treat properties matching certain naming patterns as observable (e.g., properties on classes ending in `ViewModel`).

Pros:

- Simple to implement
- No Roslyn dependency

Cons:

- Fragile — naming conventions are not enforced
- Cannot detect mixed types (observable and non-observable properties on the same class)
- No support for computed expressions

### Option 2: Roslyn semantic analysis of generated C# (chosen)

Feed the Razor-generated C# class into a Roslyn `CSharpCompilation`, resolve all types, and walk `MemberAccessExpression` nodes to classify each property reference.

Pros:

- Type-accurate — uses the actual type hierarchy to determine observability
- Handles computed expressions by walking all member accesses in the expression tree
- Handles chained access (`Model.Customer.Address.City`) by checking each intermediate type
- Leverages Roslyn APIs already familiar to the NScript compiler team

Cons:

- Requires constructing a Roslyn compilation with type stubs for the model type (adds compilation time)
- Roslyn is a heavyweight dependency (though NScript's Stage 1 already uses it)
- Observable detection is conservative — all properties on `ObservableObject` subclasses are treated as observable, even if they don't actually fire notifications (see Limitations below)

### Option 3: Cecil-based analysis (post-compilation)

Analyze the compiled model DLL via Mono.Cecil to check type hierarchies and property attributes.

Pros:

- Uses Cecil which is already in NScript's Stage 2 pipeline
- No Roslyn compilation needed

Cons:

- Runs too late in the pipeline — binding classification is needed during IR construction (Phase 3), before JST emission (Phase 4). Cecil operates on compiled DLLs which are Stage 1 output. The Razor pipeline needs classification during Stage 2 compilation.
- Cannot analyze expressions embedded in the Razor-generated C# class — only works on pre-compiled types

## Decision

The Razor compiler includes a **Roslyn semantic analysis phase** that auto-detects binding mode from type information.

### Classification Algorithm

For each `@` expression in the template:

1. The Razor parser generates a C# class with `Write(expression)` calls
2. The generated C# is added to a Roslyn `CSharpCompilation` alongside framework type stubs
3. The `RoslynAnalysisPhase` walks all `MemberAccessExpressionSyntax` nodes in each `Write()` call
4. For each member access, `SemanticModel.GetTypeInfo()` resolves the declaring type
5. `ObservableAnalyzer.IsObservableType()` checks if the type inherits from `ObservableObject` or implements `INotifyPropertyChanged`
6. Classification result:
   - **0 observable dependencies → OneTime** (evaluated once at activation)
   - **1+ observable dependencies → OneWay** (live binding, watches all dependency properties)
   - **Expression in event attribute → Event** (method reference or lambda)

### Model Type Stubs

The analysis needs type information for the `@model` type, which is defined in a different assembly. The plugin generates lightweight C# stubs from Cecil type metadata:

```csharp
// Generated stub for Roslyn analysis
namespace MyApp.ViewModels {
    public class OrderViewModel : ObservableObject {
        public string CustomerName { get; set; }
        public decimal Price { get; set; }
        public ObservableCollection<OrderItem> Orders { get; set; }
    }
}
```

Stubs include property declarations, base types, and generic type parameters — enough for Roslyn to resolve member access types without the full implementation.

### Dependency Tracking for Computed Expressions

For `@(Model.Price * Model.Quantity)`:

```
Dependencies = { (DataContext, "Price"), (DataContext, "Quantity") }
```

Both properties are on an `ObservableObject` subclass, so the binding is classified as OneWay watching both. At runtime, either property changing triggers re-evaluation of the full expression.

### Limitations

- **Conservative detection**: all properties on `ObservableObject` subclasses are assumed observable. Properties that have setters but don't call `FirePropertyChanged` are still classified as OneWay. This over-subscribes but never misses an update.
- **Method calls**: `@Model.GetTotal()` — no property to watch. Classified as OneTime. A future `[DependsOn("Price", "Quantity")]` attribute could address this.
- **No cross-assembly analysis**: only the model type and its direct references are stubbed. Deep transitive dependencies use fallback classification.

## Consequences

Positive:

- Template authors never need to annotate binding modes — the compiler determines the correct mode from the type system
- Computed expressions like `@(Model.Price * Model.Quantity)` automatically watch both properties
- Misclassification errors (forgetting `Mode=OneWay`) are eliminated by design
- The observable framework contract (ADR-0014) is enforced at compile time — if a type doesn't extend `ObservableObject`, its properties cannot accidentally be treated as reactive

Negative:

- Model type stub generation adds ~10-50ms per template to compilation time
- Conservative detection may over-subscribe on types with non-notifying properties (wastes listener registrations, not a correctness issue)
- The stub generator must handle Cecil type mapping (generics, nested types, enums) — this is a non-trivial code generation surface

## References

- Related: ADR-0014 (observable framework contract), ADR-0017 (Razor templates), ADR-0018 (binding graph)
- Design spec: [docs/superpowers/specs/2026-03-26-razor-skin-templates-design.md](../superpowers/specs/2026-03-26-razor-skin-templates-design.md) (Section 3: Expression Analysis)

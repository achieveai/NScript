# ADR 0014: Standardize the Observable Framework as the Reactive Binding Contract

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Reactive data and UI binding

## Context

NScript already has a custom observable framework in `Sunlight.Framework.Observables`, and that framework is not isolated from the rest of the system. It is the reactive contract that template generation and UI binding depend on.

The current implementation includes:

- property change contract in [Sources/Framework/Sunlight.Framework/Observables/INotifyPropertyChanged.cs](../../Sources/Framework/Sunlight.Framework/Observables/INotifyPropertyChanged.cs)
- property notification base class in [Sources/Framework/Sunlight.Framework/Observables/ObservableObject.cs](../../Sources/Framework/Sunlight.Framework/Observables/ObservableObject.cs)
- collection change contracts in [Sources/Framework/Sunlight.Framework/Observables/INotifyCollectionChanged.cs](../../Sources/Framework/Sunlight.Framework/Observables/INotifyCollectionChanged.cs)
- list implementation and mutation semantics in [Sources/Framework/Sunlight.Framework/Observables/ObservableCollection.cs](../../Sources/Framework/Sunlight.Framework/Observables/ObservableCollection.cs)
- extensible property bag support in [Sources/Framework/Sunlight.Framework/Observables/ExtensibleObservableObject.cs](../../Sources/Framework/Sunlight.Framework/Observables/ExtensibleObservableObject.cs)
- derived and transformed collection patterns in [Sources/Framework/Sunlight.Framework/Observables/ObservableCollectionTransformer.cs](../../Sources/Framework/Sunlight.Framework/Observables/ObservableCollectionTransformer.cs)

This framework is also wired directly into the template pipeline:

- template code generation resolves the observable contract in [Sources/Compiler/XwmlParser/KnownTemplateTypes.cs](../../Sources/Compiler/XwmlParser/KnownTemplateTypes.cs)
- `AutoFireAttribute` support in [Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs](../../Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs) requires the observable interface and emits property-change notifications into generated setters

That makes observability a platform contract, not just a convenience library.

## Decision Drivers

- provide a lightweight reactive model that works well in generated JavaScript
- keep property and collection notifications explicit and predictable
- support code-generated UI binding without depending on reflection-heavy or desktop-.NET notification patterns
- allow template compilation to target a stable reactive abstraction

## Options Considered

### Option 1: Standardize on the existing custom observable framework

Treat `ObservableObject`, `ObservableCollection<T>`, and the custom notify interfaces as the canonical reactive binding contract.

Pros:

- matches the existing framework and template codegen
- uses lightweight callback shapes and string property names that are easy to generate in JavaScript
- supports both property and collection notifications in one consistent platform model

Cons:

- diverges from standard desktop .NET event-pattern APIs
- depends on string property names rather than strongly typed member expressions

### Option 2: Align fully with standard .NET notification patterns

Replace the custom callback-based contracts with the standard `PropertyChanged` and `NotifyCollectionChangedEventArgs` style patterns.

Pros:

- more familiar to .NET developers
- closer to standard framework conventions

Cons:

- does not match the current implementation or template plugin expectations
- introduces extra event-pattern complexity that is less natural for generated JS

### Option 3: Avoid a formal observable contract and let templates poll or bind ad hoc

Treat reactivity as a UI-framework concern without a shared model object contract.

Pros:

- fewer framework abstractions

Cons:

- makes code generation and binding semantics much less predictable
- duplicates reactive logic across binders and template layers
- does not match the current system

## Decision

NScript standardizes on the existing `Sunlight.Framework.Observables` framework as the canonical reactive binding contract for generated UI and bindable model objects.

The accepted model is:

- `INotifyPropertyChanged` is the canonical property-observation surface
- `ObservableObject` is the canonical base class for bindable object state
- `INotifyCollectionChanged<T>` and `ObservableCollection<T>` are the canonical collection-observation surface and mutable bindable list type
- `ExtensibleObservableObject` is the standard escape hatch for attached or dynamically keyed observable state
- transformer utilities such as `ObservableCollectionTransformer<T, U>` are valid first-class patterns for derived observable projections

## Property Notification Semantics

The standardized property notification contract is:

- property listeners are keyed by property name strings
- listeners are registered and removed through explicit methods, not through standard CLR event members
- `ObservableObject` may fire one property, multiple properties, or all properties in response to a single logical change
- linked properties are part of the contract: a change to one source property may intentionally notify other dependent property names
- `AnyPropertyListener` is part of the supported runtime model for broad invalidation or instrumentation cases

This means the observable framework is intentionally optimized for generated code and binding systems that already know the property names they care about.

## Collection Notification Semantics

The standardized collection contract is:

- `ObservableCollection<T>` owns mutable ordered collection state
- collection notifications use NScript's own `CollectionChangedAction` and `CollectionChangedEventArgs<T>` types rather than desktop-.NET event args
- supported mutation actions are `Add`, `Remove`, `Replace`, and `Reset`
- the collection enforces a simple reentrancy rule by rejecting mutation while a collection notification is in flight
- `Count` invalidation is part of the observable behavior and must be raised when collection size changes

## Template and Codegen Implications

The template system is allowed to depend directly on this contract.

In particular:

- generated template and binder code may assume the presence of `INotifyPropertyChanged`
- `AutoFireAttribute`-driven property rewriting is an officially supported mechanism for converting property setters into observable setters
- the observable framework is therefore part of the code generation contract between user model types and generated UI glue

## Consequences

Positive:

- UI binding and template code generation target a stable, explicit reactive contract
- the framework stays lightweight and JavaScript-oriented
- property objects, dynamic property bags, and observable lists all fit within one cohesive model

Negative:

- the contract is NScript-specific rather than standard .NET notification idioms
- string-based property names remain a correctness and refactoring risk
- templates and binders become coupled to this specific observable model

## References

- [Sources/Framework/Sunlight.Framework/Observables/INotifyPropertyChanged.cs](../../Sources/Framework/Sunlight.Framework/Observables/INotifyPropertyChanged.cs)
- [Sources/Framework/Sunlight.Framework/Observables/ObservableObject.cs](../../Sources/Framework/Sunlight.Framework/Observables/ObservableObject.cs)
- [Sources/Framework/Sunlight.Framework/Observables/INotifyCollectionChanged.cs](../../Sources/Framework/Sunlight.Framework/Observables/INotifyCollectionChanged.cs)
- [Sources/Framework/Sunlight.Framework/Observables/ObservableCollection.cs](../../Sources/Framework/Sunlight.Framework/Observables/ObservableCollection.cs)
- [Sources/Framework/Sunlight.Framework/Observables/ExtensibleObservableObject.cs](../../Sources/Framework/Sunlight.Framework/Observables/ExtensibleObservableObject.cs)
- [Sources/Framework/Sunlight.Framework/Observables/ObservableCollectionTransformer.cs](../../Sources/Framework/Sunlight.Framework/Observables/ObservableCollectionTransformer.cs)
- [Sources/Compiler/XwmlParser/KnownTemplateTypes.cs](../../Sources/Compiler/XwmlParser/KnownTemplateTypes.cs)
- [Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs](../../Sources/Compiler/XwmlParser/XwmlTemplatingPlugin.cs)
- [Sources/Framework/Sunlight.Framework.UI/Attributes/AutoFireAttribute.cs](../../Sources/Framework/Sunlight.Framework.UI/Attributes/AutoFireAttribute.cs)
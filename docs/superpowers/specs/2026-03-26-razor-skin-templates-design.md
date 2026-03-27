# Razor Skin Templates for NScript

## Overview

Replace the current XWML template system with Razor-based `.cshtml` skin templates that support full C# expression computation, automatic observable detection for binding mode classification, reactive control flow, and event-driven UX-to-code updates. The system compiles Razor templates at build time into JavaScript factory methods that produce `SkinInstance` objects — the same runtime infrastructure used today.

### Goals

- Author skin templates using standard Razor syntax (`.cshtml`)
- Support full C# expressions in templates (not just property path bindings)
- Auto-detect binding mode (OneTime vs OneWay) from observable type analysis
- Reactive `@if`/`@foreach` blocks that update the DOM when observable inputs change
- Event handling via inline lambdas replacing TwoWay binding
- Reusable `@functions` blocks compiled to JS helpers
- Coexist with XWML templates initially; deprecate XWML once Razor is stable

### Non-Goals

- Runtime Razor execution (all compilation is at build time)
- TwoWay binding (replaced by OneWay + event-driven updates)
- Full Blazor component model (no component lifecycle, no cascading parameters)

## 1. Template Format & Syntax

### File Convention

Skin templates use the extension `.skin.cshtml` to distinguish from regular Razor pages.

### Template Structure

```razor
@model Sunlight.App.ViewModels.OrderViewModel
@control Sunlight.Framework.UI.UISkinableElement

@functions {
    string FormatPrice(decimal price) => price.ToString("C");
    string ItemClass(bool active) => active ? "item-active" : "item-inactive";
    string FullName => $"{Model.FirstName} {Model.LastName}";
}

<div class="order-panel">
    <h1>@Model.CustomerName</h1>
    <span class="total">@FormatPrice(Model.Price * Model.Quantity)</span>

    @if (Model.IsLoading)
    {
        <div class="spinner">Loading...</div>
    }
    else
    {
        <ul>
        @foreach (var order in Model.Orders)
        {
            <li class="@ItemClass(order.IsComplete)">
                @order.Title - @FormatPrice(order.Amount)
            </li>
        }
        </ul>
    }

    <button onclick="@Model.OnSubmit">Submit</button>
    <button onclick="@((evt) => Model.Cancel())">Cancel</button>

    <div style="display: @(Control.IsHidden ? "none" : "")">
        @Control.CssClass
    </div>
</div>
```

### Directives

| Directive | Required | Purpose |
|-----------|----------|---------|
| `@model TypeName` | Yes | Declares the DataContext type (replaces XWML `DataContextType`) |
| `@control TypeName` | No | Declares the TemplateParent type (replaces XWML `ControlType`). Defaults to `UISkinableElement` |
| `@functions { }` | No | Reusable helper methods and computed properties |

**Note on `@control`:** This is not a standard Razor directive. It is handled via a pre-processing step before the Razor parser runs: the `@control` line is extracted and removed from the template, then passed as metadata to subsequent phases. The cleaned template is then fed to the standard Razor parser. Alternatively, it can be registered as a custom Razor directive via `RazorProjectEngine` configuration.

### Skin Part IDs

Named element references (for programmatic access from code) use the standard `id` attribute:

```razor
<div id="headerPanel">@Model.Title</div>
<input id="searchBox" value="@Model.Query" onchange="@((e) => Model.Query = e.ElementEvent.Value)" />
```

Part IDs are collected during Phase 3 and mapped to element indices in the `partIdMapping` dictionary, identical to current XWML behavior.

### Expression Syntax

| Syntax | Meaning |
|--------|---------|
| `@Model.Property` | DataContext property access |
| `@Control.Property` | TemplateParent property access |
| `@(expression)` | Any C# expression, auto-classified |
| `@HelperMethod(args)` | Call to `@functions` method |
| `@if (cond) { } else { }` | Conditional rendering (reactive if observable) |
| `@foreach (var x in coll) { }` | List rendering (reactive if observable collection) |
| `onclick="@expr"` | Event wiring (method ref or lambda) |

## 2. Compilation Pipeline

The compilation flows through 5 phases:

```
.skin.cshtml --> Phase 1: Razor Parser (Microsoft.AspNetCore.Razor.Language)
             --> Phase 2: Roslyn Semantic Analysis (NScript compilation context)
             --> Phase 3: Template IR Builder (observable detection + classification)
             --> Phase 4: JS Factory Generator (extended SkinCodeGenerator)
             --> Phase 5: Integration (Skin registration, [Skin] attribute wiring)
```

### Phase 1: Razor Parsing

**Input:** `.skin.cshtml` file
**Tool:** `Microsoft.AspNetCore.Razor.Language.RazorProjectEngine`
**Output:** Generated C# source code (class with `ExecuteAsync()` method)

The Razor parser converts template markup and `@` expressions into a C# class:

- HTML becomes `WriteLiteral("...")` calls
- Expressions become `Write(expression)` calls
- `@if`/`@foreach` become C# control flow
- `@functions` become class methods

The generated class inherits from a custom base class (`RazorSkinTemplate<TModel, TControl>`) that provides:

- `Model` property (typed as `@model` type) -- represents the DataContext
- `Control` property (typed as `@control` type) -- represents the TemplateParent

**Important:** `RazorProjectEngine.Process()` returns a `RazorCodeDocument` containing both the generated C# and the original Razor syntax tree (`RazorSyntaxTree`). Phase 3 needs both: the C# for Roslyn type analysis, and the Razor tree for HTML structure extraction.

### Phase 2: Roslyn Semantic Analysis

**Input:** Generated C# source + NScript compilation context (type references)
**Tool:** Roslyn `CSharpCompilation` + `SemanticModel` (already part of NScript)
**Output:** Typed syntax tree with resolved type information for every expression

The generated C# is added to the NScript compilation as a syntax tree. Roslyn resolves all types:

- `Model.Price` resolves to `decimal` on a type extending `ObservableObject`
- `Model.Orders` resolves to `ObservableCollection<Order>`
- `Control.IsHidden` resolves to `bool` on `UIElement`

### Phase 3: Template IR Builder

**Input:** Typed Roslyn syntax tree + original Razor syntax tree (for HTML structure)
**Output:** Template Intermediate Representation tree

IR node types:

- `HtmlNode` -- static HTML content
- `ExpressionBindingNode` -- `@(expr)` with classified binding mode and dependency set
- `ConditionalNode` -- `@if(expr) { ... } else { ... }` with reactive/static flag
- `LoopNode` -- `@foreach(var x in expr)` with observable/fixed classification
- `EventNode` -- event wiring with handler expression
- `FunctionNode` -- helper from `@functions` block

The critical observable detection logic runs here (see Section 3).

### Phase 4: JS Factory Generator

**Input:** Template IR
**Output:** JavaScript source code (factory method + skin getter)

Extends the existing `SkinCodeGenerator` pattern to handle new IR node types:

- `HtmlNode` --> `innerHTML` string in DOM cache
- `ExpressionBindingNode` --> `SkinBinderInfo` with getter chain
- `ConditionalNode` (reactive) --> `ConditionalBinder`
- `LoopNode` (reactive) --> `CollectionBinder`
- `EventNode` --> DOM event handler registration
- `FunctionNode` --> standalone or context-scoped JS function

### Phase 5: Integration

The generated JS factory is registered the same way current XWML factories are:

- A `Skin` object with `factoryMethod` returns a `SkinInstance`
- The `[Skin("templateName")]` attribute on a property triggers the skin getter
- Both XWML and Razor skins produce identical `SkinInstance` objects at runtime

## 3. Expression Analysis & Observable Detection

### Classification Algorithm

For each `@` expression in the template:

1. Get the Roslyn `SyntaxNode` for the expression
2. Get the `SemanticModel`
3. Walk all `MemberAccessExpression` nodes in the expression tree
4. For each member access:
   - Resolve the declaring type via `SemanticModel.GetTypeInfo()`
   - Check if type inherits from `ObservableObject` or implements `INotifyPropertyChanged`
   - If YES: record `(sourceType, propertyName)` as an observable dependency
5. Classify:
   - 0 observable dependencies --> **OneTime**
   - 1+ observable dependencies --> **OneWay** (live binding)
   - Expression is `Action`/delegate in event attribute --> **Event**

### Observable Property Detection

```csharp
bool IsObservableProperty(IPropertySymbol property)
{
    var type = property.ContainingType;

    // Check 1: Type inherits from ObservableObject
    if (InheritsFrom(type, "Sunlight.Framework.Observables.ObservableObject"))
        return true;

    // Check 2: Type implements INotifyPropertyChanged
    if (Implements(type, "Sunlight.Framework.Observables.INotifyPropertyChanged"))
        return true;

    return false;
}

bool IsObservableCollection(ITypeSymbol type)
{
    return Implements(type, "Sunlight.Framework.Observables.IObservableCollection");
}
```

A property is considered **actually observable** if:

- Its declaring type passes the checks above, AND
- The property has `[AutoFire]` attribute, OR
- The property setter calls `FirePropertyChanged`, OR
- The property has `[DefaultDataBinding]` attribute

### Classification Examples

| Expression | Analysis | Classification |
|------------|----------|----------------|
| `@Model.Name` | `Name` on `ObservableObject` subclass | OneWay -- watch `Name` |
| `@Model.AppVersion` | `AppVersion` is plain field, no notification | OneTime |
| `@(Model.Price * Model.Quantity)` | Both observable | OneWay -- watch `Price` AND `Quantity` |
| `@FormatPrice(Model.Total)` | `Total` observable, `FormatPrice` static | OneWay -- watch `Total` |
| `@(Model.IsActive ? "active" : "inactive")` | `IsActive` observable | OneWay -- watch `IsActive` |
| `@Control.CssClass` | `CssClass` on `UIElement` (observable) | OneWay (TemplateParent source) |
| `@Model.OnSubmit` in `onclick` | Method reference | Event |
| `@((evt) => Model.Cancel())` in `onclick` | Lambda | Event (inline handler) |

### Dependency Tracking for Computed Expressions

For `@(Model.Price * Model.Quantity)`, the compiler extracts:

```
Dependencies = { (DataContext, "Price"), (DataContext, "Quantity") }
```

At runtime, the generated binder watches both properties and recomputes the full expression when either changes. Implemented as a single `LiveBinder` that:

1. Listens to `PropertyChanged` on DataContext for "Price" and "Quantity"
2. On change: re-evaluates `price * quantity` via getter functions
3. Sets result on the target element

### Chained Property Access

For `@Model.Customer.Address.City`:

- Watches `Customer` on Model
- Watches `Address` on Customer
- Watches `City` on Address
- If any intermediate object changes, re-resolves the full chain

This matches the current `LiveBinder` chain-walking behavior.

### Method Calls

`@Model.GetTotal()` -- no property to watch. Treated as **OneTime** unless the method carries a `[DependsOn("Price", "Quantity")]` attribute (future enhancement).

## 4. Reactive Control Flow

### Reactive @if / @else

When an `@if` condition references observable properties, it becomes a **reactive conditional block**.

```razor
@if (Model.IsLoading)
{
    <div class="spinner">Loading...</div>
}
else
{
    <div class="content">@Model.Data</div>
}
```

**Compiled behavior:**

1. Both branches are pre-rendered as template fragments in the DOM cache
2. A `ConditionalBinder` watches `Model.IsLoading`
3. When the condition changes, the active branch is swapped

**Performance strategies** (benchmark both):

- **Show/Hide:** Both branches in DOM, toggle `display:none`. Faster switching, more memory.
- **Add/Remove:** Only active branch in DOM. Less memory, clone on each toggle.

**Non-reactive @if:** If the condition references only non-observable properties, it evaluates once at activation (OneTime). Only the matching branch is ever rendered.

### Reactive @foreach

When the collection is `IObservableCollection`, the loop becomes **incremental**.

```razor
@foreach (var order in Model.Orders)
{
    <li class="@ItemClass(order.IsComplete)">@order.Title</li>
}
```

**Compiled behavior:**

1. Loop body becomes a template fragment
2. A `CollectionBinder` subscribes to `CollectionChanged`
3. On change:
   - **Add:** clone fragment, bind item as DataContext, insert at position
   - **Remove:** dispose bindings, remove DOM element
   - **Replace:** rebind existing element with new item data
   - **Reset:** clear all, re-render from scratch
4. Each item's inner bindings operate independently

This effectively replaces `ListView` for the common case -- the template is self-contained without needing separate `ListView` + `ListViewItem` + `Skin` setup.

**Non-reactive @foreach:** If the collection is `IList`/`List<T>`, items render once at activation.

### Nesting

Reactive blocks nest naturally:

```razor
@if (Model.HasOrders)
{
    @foreach (var order in Model.Orders)
    {
        @if (order.IsUrgent)
        {
            <li class="urgent">@order.Title</li>
        }
    }
}
```

Each level manages its own lifecycle. When a parent deactivates, all children deactivate recursively (same as current `SkinInstance.Deactivate()` pattern).

## 5. Event Handling

Events are detected when a `@` expression appears in a DOM event attribute (`onclick`, `onchange`, `onfocus`, etc.). The compiler auto-detects the handler type.

### Method Reference

```razor
<button onclick="@Model.OnSubmit">Submit</button>
```

Compiler sees `Model.OnSubmit` resolves to `Action` or `Action<UIEvent>`. Generates a DOM event listener that calls `dataContext.OnSubmit()`.

### Inline Lambda

```razor
<button onclick="@((evt) => Model.IsActive = false)">Deactivate</button>
```

The lambda body compiles to JS. `Model.IsActive = false` becomes `dataContext.set_isActive(false)` in JS. The property setter fires `FirePropertyChanged("IsActive")`, triggering any OneWay bindings watching `IsActive`.

### Lambda with Event Data

```razor
<input onchange="@((evt) => Model.Name = evt.ElementEvent.Value)" />
```

The lambda receives the event object, reads from it, and writes to the model. This replaces TwoWay binding: `onchange` pushes UX-to-code, and a OneWay binding on `@Model.Name` elsewhere pushes code-to-UX.

### Event Lifecycle

- Handlers registered during `SkinInstance.Activate()`
- Unregistered during `Deactivate()` / `Dispose()`
- For items in reactive `@foreach`, each item's events follow its lifecycle

## 6. @functions Blocks

```razor
@functions {
    string FormatPrice(decimal price) => price.ToString("C");
    string ItemClass(bool active) => active ? "item-active" : "item-inactive";
    string FullName => $"{Model.FirstName} {Model.LastName}";
}
```

### Compilation Rules

- **Pure functions** (no `Model`/`Control` references): compile to standalone JS helper functions, shared across all instances
- **Model-dependent functions** (like `FullName`): compile to instance-scoped functions that receive `dataContext` as parameter

### Generated JS

```javascript
// Pure -- shared static
function FormatPrice(price) { return price.toString("C"); }

// Model-dependent -- receives context
function FullName(dataContext) {
    return dataContext.get_firstName() + " " + dataContext.get_lastName();
}
```

### Observable Detection in Functions

When a `@functions` computed property references observable properties, calls to it inherit reactivity. `@FullName` referencing `Model.FirstName` and `Model.LastName` is classified as OneWay watching both properties.

## 7. Generated JS Output

The generated JS follows the same pattern as current XWML output, extended for new node types.

### Factory Structure

```javascript
// Helper functions (from @functions)
function FormatPrice(price) { return /* ... */; }

// Getter functions for binding paths
function getter_Name(src) { return src.get_name(); }
function getter_Price(src) { return src.get_price(); }
function getter_Quantity(src) { return src.get_quantity(); }
function computed_Total(src) { return src.get_price() * src.get_quantity(); }

// Factory method
function OrderSkin_factory(skinFactory, doc) {
    var domStore, htmlRoot, objStorage;

    if (!(domStore = DocStorageGetter(doc))[0]) {
        domStore[0] = doc.createElement("div");
        domStore[0].innerHTML = "...template HTML...";

        tmplStore[0] = tmplStore[0] ? tmplStore[0] : [
            // Binders array: SkinBinderInfo objects
            SkinBinderInfo_factory(
                [getter_Name], ["CustomerName"],
                SetTextContent, ONEWAY | DATACONTEXT, 0, 0, null, ""),
            SkinBinderInfo_factory(
                [computed_Total], ["Price", "Quantity"],
                SetTextContent, ONEWAY | DATACONTEXT, 1, 1, null, ""),
            // ... conditional binders, collection binders, event binders
        ];
    }

    htmlRoot = domStore[0].cloneNode(true);
    objStorage = new Array(N);
    // Map element references...

    return SkinInstance_factory(
        skinFactory, htmlRoot, childUIElements,
        objStorage, tmplStore[0], partMap,
        liveBinderCount, extraObjectCount);
}

// Cached singleton getter
var OrderSkin_var = null;
function OrderSkin() {
    if (!OrderSkin_var)
        OrderSkin_var = Skin_factory(
            UISkinableElement, OrderViewModel,
            OrderSkin_factory, "0");
    return OrderSkin_var;
}
```

### New Binder Types

In addition to existing `SkinBinderInfo` types, new binder types are needed:

- **MultiDependencyBinder:** For computed expressions watching multiple properties. Extends `LiveBinder` to register listeners on N properties and recompute via a single getter function.
- **ConditionalBinder:** Watches a boolean observable, swaps between two DOM fragment templates.
- **CollectionBinder:** Watches an `IObservableCollection`, manages incremental DOM fragment add/remove/replace.
- **EventBinder:** Registers DOM event handlers with method references or compiled lambda functions.

## 8. Integration with NScript Compiler

### Plugin Registration

A new `RazorTemplatingPlugin` is registered alongside the existing `XwmlTemplatingPlugin`:

- Detects `.skin.cshtml` files in the project
- Triggered by the same `[Skin("templateName")]` attribute
- Produces the same output format (JS factory methods)

### Coexistence

Both plugins can run simultaneously:

- `[Skin("OrderSkin")]` where `OrderSkin.skin.cshtml` exists --> Razor pipeline
- `[Skin("LegacySkin")]` where `LegacySkin.html` exists --> XWML pipeline
- Runtime behavior identical -- both produce `SkinInstance` objects

### Migration Path

1. **Phase 1:** Razor plugin works alongside XWML. New skins use Razor.
2. **Phase 2:** Migrate existing XWML skins to Razor (can be automated for simple templates).
3. **Phase 3:** Deprecate and remove XWML plugin.

### Dependencies

New NuGet dependency: `Microsoft.AspNetCore.Razor.Language` (for Phase 1 parsing). This is the parser-only package, not the full ASP.NET runtime.

## 9. Performance Considerations

Since runtime performance testing is a key requirement, the design preserves the current performance characteristics while adding new capabilities:

### Preserved Optimizations

- **DOM caching:** Templates cached per-document, cloned on instantiation (same as XWML)
- **Pre-computed binders:** `SkinBinderInfo` array built at compile time, not runtime
- **OneTime bindings:** Evaluated once, no listeners registered

### New Performance Costs

- **MultiDependencyBinder:** More listeners per expression vs single-property binders. Cost: O(dependencies) listener registrations per binding.
- **ConditionalBinder:** Fragment cloning on branch switch. Benchmark Show/Hide vs Add/Remove.
- **CollectionBinder:** Per-item fragment cloning and binding setup. Comparable to current `ListView` cost.

### Benchmarking Plan

Each new binder type will be benchmarked against the XWML equivalent:

1. Simple property binding -- Razor OneWay vs XWML OneWay LiveBinder
2. Computed expression -- Razor MultiDependencyBinder vs manual XWML with converter
3. Reactive @if -- ConditionalBinder vs manual show/hide in code
4. Reactive @foreach -- CollectionBinder vs ListView with ItemSkin
5. Event handling -- Razor event binder vs XWML DOM event binding
6. Large list performance (1000+ items) -- incremental updates
7. Deep nesting (nested @if/@foreach) -- lifecycle overhead

Results will determine which strategy (Show/Hide vs Add/Remove for conditionals, etc.) becomes the default.

## 10. Summary

| Aspect | Current (XWML) | New (Razor) |
|--------|----------------|-------------|
| Template syntax | Custom `{Prop, Mode=X}` | Standard Razor `@Model.Prop` |
| Expressions | Property paths only | Full C# expressions |
| Binding mode | Manual (`Mode=OneWay`) | Auto-detected from types |
| Converters | Explicit `Converter=vm:Type.Method` | Just use expressions |
| Control flow | None (delegate to ListView) | Reactive `@if`/`@foreach` |
| Reuse | None | `@functions` blocks |
| Two-way binding | `TwoWay` binder | OneWay + event lambda |
| Event handling | `{OnClick}` in event attr | `@Model.OnClick` or `@((e) => ...)` |
| TemplateParent | `Source=TemplateParent` | `@Control.Property` |
| Runtime output | SkinInstance + SkinBinderInfo | SkinInstance + SkinBinderInfo (same) |
| IDE support | None | Razor IntelliSense (partial) |

# Razor Skin Template Browser Test Coverage — Design Spec

## Purpose

Comprehensive browser-based test suite for the Razor skin template system. These C# test classes compile to JavaScript via NScript and run in QUnit in a real browser, verifying that compiled templates actually work at runtime — not just that the generated JS text looks correct.

## Scope

- **Target**: Legacy mode only (`_useGraphMode = false`, the active SkinBinderInfo path)
- **Test count**: ~43 new tests + 12 existing = ~55 total
- **Complexity range**: Simple text binding → complex real-life scenarios with nested control flow, collections, and events
- **Out of scope**: Graph mode tests, TwoWay binding (not supported by Razor compiler), @functions runtime tests (compiler-level only)

## Existing Coverage (12 tests)

| ID | Test | Status |
|---|---|---|
| B01 | OneTime text (RazorPlainVM) | Exists |
| B02 | OneWay text with updates | Exists |
| B03 | Multiple property changes | Exists |
| B04 | Multiple independent bindings | Exists |
| L01 | Activate renders initial values | Exists |
| L02 | DataContext before activate | Exists |
| L03 | Change DataContext after activate | Exists |
| G01-G04 | Graph mode duplicates | Exists |

## ViewModel Changes

### Extend `RazorTestVM` (in RazorTestViewModels.cs)

Add observable properties needed by new test templates:

| Property | Type | Purpose |
|---|---|---|
| `Price` | int | Computed expression tests |
| `Quantity` | int | Computed expression tests |
| `DisplayStyle` | string | Style binding tests |
| `Title` | string | Attribute binding tests |
| `ShowDetails` | bool | Multi-branch conditional tests |
| `Query` | string | Real-life scenario tests |
| `ClickCount` | int | Event handler verification |
| `LastClickedElement` | object | DOM event param verification |

All follow existing pattern: private backing field + `FirePropertyChanged("PropertyName")` in setter.

### Extend `RazorTestVM` methods

| Method | Signature | Purpose |
|---|---|---|
| `IncrementClick()` | `void IncrementClick()` | Event test: `ClickCount++` |
| `OnDomEvent(Element elem, ElementEvent evt)` | standard DOM handler | DOM event param test |

### Extend `RazorItemVM`

| Property | Type | Purpose |
|---|---|---|
| `Status` | string | Per-item conditional display |

## New Templates (~18 files)

All placed in `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/`.

### Attribute/Style/Class Binding Templates

**RazorClassBinding.skin.cshtml**
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1" class="@Model.CssClass">Content</div>
```

**RazorStyleBinding.skin.cshtml**
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1" style="display: @Model.DisplayStyle">Styled</div>
```

**RazorAttrBinding.skin.cshtml**
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1" title="@Model.Title" data-count="@Model.Count">Attributed</div>
```

**RazorMultiAttr.skin.cshtml**
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1" class="@Model.CssClass" title="@Model.Title" data-count="@Model.Count">Multi</div>
```

### Computed Expression Template

**RazorComputed.skin.cshtml**
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1"><span class="total">@(Model.Price * Model.Quantity)</span></div>
```

### Conditional Templates

**RazorIfOnly.skin.cshtml**
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
@if (Model.IsActive)
{
    <span class="active-content">Active</span>
}
</div>
```

**RazorIfElse.skin.cshtml**
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
@if (Model.IsActive)
{
    <span class="if-branch">Active</span>
}
else
{
    <span class="else-branch">Inactive</span>
}
</div>
```

**RazorIfElseIf.skin.cshtml** — Tests chained conditionals using nested @if in @else (uses proven boolean conditions, not comparison expressions):
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
@if (Model.IsActive)
{
    <span class="branch-active">Active</span>
}
else
{
    @if (Model.ShowDetails)
    {
        <span class="branch-details">Details</span>
    }
    else
    {
        <span class="branch-default">Default</span>
    }
}
</div>
```

Note: `ShowDetails` bool property to be added to `RazorTestVM`.

**RazorIfBindings.skin.cshtml** — Bindings inside conditional branches:
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
@if (Model.IsActive)
{
    <span class="active-name">@Model.Name</span>
}
else
{
    <span class="inactive-msg">Disabled</span>
}
</div>
```

**RazorNestedIf.skin.cshtml** — @if nested inside @if:
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
@if (Model.IsActive)
{
    @if (Model.Count > 0)
    {
        <span class="active-with-items">Has Items</span>
    }
    else
    {
        <span class="active-empty">No Items</span>
    }
}
</div>
```

**RazorStaticIf.skin.cshtml** — Non-observable condition (OneTime):
```razor
@model Sunlight.Framework.UI.Test.RazorPlainVM

<div data-test="1">
@if (Model.IsStatic)
{
    <span class="static-content">Static</span>
}
</div>
```

Note: `RazorPlainVM` needs an `IsStatic` bool property added.

### Collection Templates

**RazorForeach.skin.cshtml** — Basic foreach rendering:
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
<ul class="item-list">
@foreach (var item in Model.Items)
{
    <li class="item">@item.Name</li>
}
</ul>
</div>
```

**RazorForeachBindings.skin.cshtml** — Reactive bindings inside foreach items:
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
@foreach (var item in Model.Items)
{
    <div class="item-row">
        <span class="item-name">@item.Name</span>
        <span class="item-status">@item.IsComplete</span>
    </div>
}
</div>
```

### Nested Control Flow Templates

**RazorIfInForeach.skin.cshtml** — @if inside @foreach:
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
@foreach (var item in Model.Items)
{
    @if (item.IsComplete)
    {
        <li class="done">@item.Name ✓</li>
    }
    else
    {
        <li class="pending">@item.Name</li>
    }
}
</div>
```

**RazorForeachInIf.skin.cshtml** — @foreach inside @if:
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
@if (Model.IsActive)
{
    <ul class="active-list">
    @foreach (var item in Model.Items)
    {
        <li>@item.Name</li>
    }
    </ul>
}
else
{
    <span class="disabled-msg">List disabled</span>
}
</div>
```

### Event Templates

**RazorEventClick.skin.cshtml** — Method reference event handler:
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
<button class="btn-click" onclick="@Model.IncrementClick">Click Me</button>
<span class="click-count">@Model.ClickCount</span>
</div>
```

**RazorEventLambda.skin.cshtml** — Lambda event handler:
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
<button class="btn-lambda" onclick="@((e) => Model.IncrementClick())">Lambda Click</button>
<span class="click-count">@Model.ClickCount</span>
</div>
```

### Real-Life Scenario Templates

**RazorTodoApp.skin.cshtml** — Complete todo-list pattern:
```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
<div class="todo-header">
    <span class="todo-count">@Model.Count</span>
</div>
<ul class="todo-list">
@foreach (var item in Model.Items)
{
    @if (item.IsComplete)
    {
        <li class="todo-done"><span class="todo-name">@item.Name</span></li>
    }
    else
    {
        <li class="todo-pending"><span class="todo-name">@item.Name</span></li>
    }
}
</ul>
</div>
```

## Test Matrix (43 new tests)

### A: Attribute/Style/Class Binding (7 tests)

| ID | Test Name | Template | What It Verifies |
|---|---|---|---|
| A01 | TestRazorClassBindingInitial | RazorClassBinding | `class` attr reflects initial `CssClass` value |
| A02 | TestRazorClassBindingUpdate | RazorClassBinding | Changing `CssClass` updates element className |
| A03 | TestRazorStyleBindingInitial | RazorStyleBinding | `style` attr includes initial `DisplayStyle` |
| A04 | TestRazorStyleBindingUpdate | RazorStyleBinding | Changing `DisplayStyle` updates style |
| A05 | TestRazorAttrBindingInitial | RazorAttrBinding | `title` and `data-count` reflect initial values |
| A06 | TestRazorAttrBindingUpdate | RazorAttrBinding | Changing `Title` updates title attribute |
| A07 | TestRazorMultiAttrBinding | RazorMultiAttr | Multiple attributes on one element all bind correctly |

### C: Computed Expressions (3 tests)

| ID | Test Name | Template | What It Verifies |
|---|---|---|---|
| C01 | TestRazorComputedInitial | RazorComputed | `Price * Quantity` renders correct initial product |
| C02 | TestRazorComputedPriceChange | RazorComputed | Changing `Price` triggers recompute and DOM update |
| C03 | TestRazorComputedQuantityChange | RazorComputed | Changing `Quantity` triggers recompute and DOM update |

### IF: Conditionals (11 tests)

| ID | Test Name | Template | What It Verifies |
|---|---|---|---|
| IF01 | TestRazorIfOnlyTrue | RazorIfOnly | `@if(true)` renders content |
| IF02 | TestRazorIfOnlyFalse | RazorIfOnly | `@if(false)` renders nothing |
| IF03 | TestRazorIfElseShowsTrue | RazorIfElse | True branch shown when `IsActive=true` |
| IF04 | TestRazorIfElseShowsFalse | RazorIfElse | Else branch shown when `IsActive=false` |
| IF05 | TestRazorIfElseToggle | RazorIfElse | Toggle `IsActive` swaps branches |
| IF06 | TestRazorIfElseIfFirstBranch | RazorIfElseIf | `IsActive=true` shows first branch |
| IF07 | TestRazorIfElseIfSecondBranch | RazorIfElseIf | `IsActive=false, ShowDetails=true` shows second branch |
| IF08 | TestRazorIfElseIfDefaultBranch | RazorIfElseIf | Both false shows default branch |
| IF09 | TestRazorIfBindingsActive | RazorIfBindings | Binding inside true branch renders and updates |
| IF10 | TestRazorNestedIfBothTrue | RazorNestedIf | Both conditions true shows inner content |
| IF11 | TestRazorStaticIf | RazorStaticIf | Non-observable `@if` renders once, no reactivity |

### FE: Foreach/Collections (6 tests)

| ID | Test Name | Template | What It Verifies |
|---|---|---|---|
| FE01 | TestRazorForeachInitialRender | RazorForeach | Collection items render as `<li>` elements |
| FE02 | TestRazorForeachAddItem | RazorForeach | `Items.Add()` appends new `<li>` |
| FE03 | TestRazorForeachRemoveItem | RazorForeach | `Items.Remove()` removes corresponding `<li>` |
| FE04 | TestRazorForeachClear | RazorForeach | `Items.Clear()` removes all `<li>` elements |
| FE05 | TestRazorForeachMultipleOps | RazorForeach | Sequential add/remove/add maintains correct DOM |
| FE06 | TestRazorForeachItemBinding | RazorForeachBindings | Item bindings render initially; `item.Name = "new"` updates only that item's `<span>` |

### N: Nested Control Flow (3 tests)

| ID | Test Name | Template | What It Verifies |
|---|---|---|---|
| N01 | TestRazorIfInForeach | RazorIfInForeach | Per-item conditional renders correct branch |
| N02 | TestRazorForeachInIfActive | RazorForeachInIf | List renders when `IsActive=true` |
| N03 | TestRazorForeachInIfToggle | RazorForeachInIf | Toggling `IsActive` shows/hides entire list |

### E: Events (3 tests)

| ID | Test Name | Template | What It Verifies |
|---|---|---|---|
| E01 | TestRazorEventMethodRef | RazorEventClick | Click fires `IncrementClick`, `ClickCount` updates |
| E02 | TestRazorEventLambda | RazorEventLambda | Lambda click fires `IncrementClick` |
| E03 | TestRazorEventUpdatesBinding | RazorEventClick | Click → ClickCount change → DOM span updates reactively |

### L: Lifecycle (5 tests — extends existing 3)

| ID | Test Name | Template | What It Verifies |
|---|---|---|---|
| L04 | TestRazorDeactivateStopsUpdates | RazorSimpleText | After `Deactivate()`, VM changes don't update DOM |
| L05 | TestRazorReactivateResumes | RazorSimpleText | After re-`Activate()`, VM changes update DOM again |
| L06 | TestRazorDisposeCleanup | RazorMultiBinding | After `Dispose()`, no further updates |
| L07 | TestRazorNullDataContext | RazorSimpleText | Setting `DataContext = null` clears bound text |
| L08 | TestRazorEmptyStringBinding | RazorSimpleText | Empty string property renders as empty text node |

### RL: Real-Life Scenarios (4 tests)

| ID | Test Name | Template | What It Verifies |
|---|---|---|---|
| RL01 | TestRazorTodoInitialRender | RazorTodoApp | Initial items render with count, correct done/pending classes |
| RL02 | TestRazorTodoAddItem | RazorTodoApp | Add item → new `<li>` appears + count updates |
| RL03 | TestRazorTodoToggleComplete | RazorTodoApp | Toggle `IsComplete` swaps `todo-done`/`todo-pending` class |
| RL04 | TestRazorTodoRemoveItem | RazorTodoApp | Remove item → `<li>` disappears + count updates |

## Sub-Control Tests — Deferred

After investigating the sub-control code generation pipeline, sub-controls require:
1. A PascalCase-named NScript-compiled UI control class
2. A factory function generated by the NScript compiler
3. The control's own template compiled through the same pipeline

Creating a dummy `TestChildControl` with its own Razor skin is feasible but requires that the test project contain a compiled control class discoverable by the template plugin. This is a separate piece of work worth its own design iteration. For now, the test suite covers all other template features comprehensively.

If sub-control tests are high priority, two approaches:
- **Quick**: Use existing `ListView` with `ObservableList` property binding
- **Proper**: Create `TestChildControl : UISkinableElement` with its own `.skin.cshtml` and `[Skin]` attribute

## Files to Create/Modify

### New Files (18 templates + 1 test file)

| File | Purpose |
|---|---|
| `RazorTemplates/RazorClassBinding.skin.cshtml` | Class binding template |
| `RazorTemplates/RazorStyleBinding.skin.cshtml` | Style binding template |
| `RazorTemplates/RazorAttrBinding.skin.cshtml` | Attribute binding template |
| `RazorTemplates/RazorMultiAttr.skin.cshtml` | Multi-attribute template |
| `RazorTemplates/RazorComputed.skin.cshtml` | Computed expression template |
| `RazorTemplates/RazorIfOnly.skin.cshtml` | @if without @else |
| `RazorTemplates/RazorIfElse.skin.cshtml` | @if/@else |
| `RazorTemplates/RazorIfElseIf.skin.cshtml` | Chained @if/@else if/@else |
| `RazorTemplates/RazorIfBindings.skin.cshtml` | Bindings inside conditionals |
| `RazorTemplates/RazorNestedIf.skin.cshtml` | Nested @if |
| `RazorTemplates/RazorStaticIf.skin.cshtml` | Non-observable @if |
| `RazorTemplates/RazorForeach.skin.cshtml` | Basic @foreach |
| `RazorTemplates/RazorForeachBindings.skin.cshtml` | @foreach with item bindings |
| `RazorTemplates/RazorIfInForeach.skin.cshtml` | @if inside @foreach |
| `RazorTemplates/RazorForeachInIf.skin.cshtml` | @foreach inside @if |
| `RazorTemplates/RazorEventClick.skin.cshtml` | Method ref event |
| `RazorTemplates/RazorEventLambda.skin.cshtml` | Lambda event |
| `RazorTemplates/RazorTodoApp.skin.cshtml` | Real-life todo scenario |

### Modified Files

| File | Changes |
|---|---|
| `RazorTestViewModels.cs` | Add Price, Quantity, DisplayStyle, Title, ShowDetails, Query, ClickCount, LastClickedElement, IncrementClick(), OnDomEvent(); extend RazorItemVM with Status; add IsStatic to RazorPlainVM |
| `RazorSkinTemplatesClass.cs` | Add 18 new `[Skin]` property registrations |
| `Sunlight.Framework.UI.Test.csproj` | Add 18 new `<EmbeddedResource>` entries |
| `RazorSkinTemplateTests.cs` | Add 43 new test methods organized by category |

## Test Pattern (standard for all tests)

```csharp
[Test]
public static void TestRazorIfElseToggle(Assert assert)
{
    var element = Window.Instance.Document.CreateElement("div");
    var control = new UISkinableElement(element);

    var vm = new RazorTestVM();
    vm.IsActive = true;
    control.DataContext = vm;
    control.Skin = RazorSkinTemplatesClass.RazorIfElse;
    control.Activate();

    // Initial state: true branch visible
    var ifBranch = element.QuerySelector("[data-test] .if-branch");
    assert.NotEqual(null, ifBranch, "If branch should be visible when IsActive=true");

    // Toggle to false
    vm.IsActive = false;
    var elseBranch = element.QuerySelector("[data-test] .else-branch");
    assert.NotEqual(null, elseBranch, "Else branch should appear when IsActive=false");

    // Toggle back to true
    vm.IsActive = true;
    ifBranch = element.QuerySelector("[data-test] .if-branch");
    assert.NotEqual(null, ifBranch, "If branch should reappear when IsActive=true");
}
```

## Success Criteria

1. All 43 new tests pass in the browser (QUnit green)
2. All 12 existing tests continue to pass (no regressions)
3. Solution builds without errors: `dotnet build NScript_Full.sln -c Debug`
4. Each test category has at least one "initial render" test and one "reactive update" test
5. Collection tests verify incremental DOM operations (add/remove), not just final state

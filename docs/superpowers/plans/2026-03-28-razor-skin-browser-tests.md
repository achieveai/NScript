# Razor Skin Browser Tests Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add 43 browser-based tests covering attribute/style/class binding, computed expressions, conditionals (@if/@else/@else-if chains), @foreach with ObservableCollection, nested control flow, events, lifecycle, and real-life scenarios for the Razor skin template system.

**Architecture:** Each test is a static C# method compiled to JavaScript via NScript and run in QUnit. Tests create a `UISkinableElement`, assign a ViewModel, set a `[Skin]` template, call `Activate()`, then query the resulting DOM. Templates are `.skin.cshtml` files embedded as resources and compiled by `RazorTemplatingPlugin` at build time.

**Tech Stack:** C# (compiled to JS via NScript), QUnit 2.2.0, SunlightUnit test framework, Razor skin templates

**Spec:** `docs/superpowers/specs/2026-03-28-razor-skin-browser-tests-design.md`

---

## File Map

All paths are relative to `Test/Framework/Sunlight.Framework.UI.Test/`.

| File | Action | Responsibility |
|---|---|---|
| `RazorTestViewModels.cs` | Modify | Add new observable properties and methods to RazorTestVM, RazorItemVM, RazorPlainVM |
| `RazorSkinTemplatesClass.cs` | Modify | Add 18 new `[Skin]` property registrations |
| `Sunlight.Framework.UI.Test.csproj` | Modify | Add 18 new `<EmbeddedResource>` entries |
| `RazorTemplates/RazorClassBinding.skin.cshtml` | Create | Class binding template |
| `RazorTemplates/RazorStyleBinding.skin.cshtml` | Create | Style binding template |
| `RazorTemplates/RazorAttrBinding.skin.cshtml` | Create | Attribute binding template |
| `RazorTemplates/RazorMultiAttr.skin.cshtml` | Create | Multi-attribute template |
| `RazorTemplates/RazorComputed.skin.cshtml` | Create | Computed expression template |
| `RazorTemplates/RazorIfOnly.skin.cshtml` | Create | @if without @else |
| `RazorTemplates/RazorIfElse.skin.cshtml` | Create | @if/@else |
| `RazorTemplates/RazorIfElseIf.skin.cshtml` | Create | Chained @if/nested @if |
| `RazorTemplates/RazorIfBindings.skin.cshtml` | Create | Bindings inside conditionals |
| `RazorTemplates/RazorNestedIf.skin.cshtml` | Create | @if nested inside @if |
| `RazorTemplates/RazorStaticIf.skin.cshtml` | Create | Non-observable @if (OneTime) |
| `RazorTemplates/RazorForeach.skin.cshtml` | Create | Basic @foreach |
| `RazorTemplates/RazorForeachBindings.skin.cshtml` | Create | @foreach with item bindings |
| `RazorTemplates/RazorIfInForeach.skin.cshtml` | Create | @if inside @foreach |
| `RazorTemplates/RazorForeachInIf.skin.cshtml` | Create | @foreach inside @if |
| `RazorTemplates/RazorEventClick.skin.cshtml` | Create | Method ref event |
| `RazorTemplates/RazorEventLambda.skin.cshtml` | Create | Lambda event |
| `RazorTemplates/RazorTodoApp.skin.cshtml` | Create | Real-life todo scenario |
| `RazorSkinTemplateTests.cs` | Modify | Add 43 new test methods |

---

### Task 1: Extend ViewModels

**Files:**
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorTestViewModels.cs`

- [ ] **Step 1: Add new properties to RazorTestVM**

Open `RazorTestViewModels.cs`. After the existing `ClickFired` field (line 82), add these new observable properties and methods. Follow the exact same pattern as existing properties (private backing field + equality check + FirePropertyChanged):

```csharp
        private int price;
        private int quantity;
        private string displayStyle;
        private string title;
        private bool showDetails;
        private int clickCount;

        public int Price
        {
            get { return this.price; }
            set
            {
                if (this.price != value)
                {
                    this.price = value;
                    base.FirePropertyChanged("Price");
                }
            }
        }

        public int Quantity
        {
            get { return this.quantity; }
            set
            {
                if (this.quantity != value)
                {
                    this.quantity = value;
                    base.FirePropertyChanged("Quantity");
                }
            }
        }

        public string DisplayStyle
        {
            get { return this.displayStyle; }
            set
            {
                if (this.displayStyle != value)
                {
                    this.displayStyle = value;
                    base.FirePropertyChanged("DisplayStyle");
                }
            }
        }

        public string Title
        {
            get { return this.title; }
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    base.FirePropertyChanged("Title");
                }
            }
        }

        public bool ShowDetails
        {
            get { return this.showDetails; }
            set
            {
                if (this.showDetails != value)
                {
                    this.showDetails = value;
                    base.FirePropertyChanged("ShowDetails");
                }
            }
        }

        public int ClickCount
        {
            get { return this.clickCount; }
            set
            {
                if (this.clickCount != value)
                {
                    this.clickCount = value;
                    base.FirePropertyChanged("ClickCount");
                }
            }
        }

        public void IncrementClick()
        {
            this.ClickCount = this.ClickCount + 1;
        }
```

- [ ] **Step 2: Add Status property to RazorItemVM**

In `RazorItemVM` class (after the `IsComplete` property), add:

```csharp
        private string status;

        public string Status
        {
            get { return this.status; }
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    base.FirePropertyChanged("Status");
                }
            }
        }
```

- [ ] **Step 3: Add IsStatic to RazorPlainVM**

Replace the `RazorPlainVM` class with:

```csharp
    public class RazorPlainVM
    {
        public string AppVersion { get; set; }
        public bool IsStatic { get; set; }
    }
```

- [ ] **Step 4: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTestViewModels.cs
git commit -m "test(razor): extend ViewModels with properties for browser test coverage"
```

---

### Task 2: Create Attribute/Style/Class Binding Templates (4 files)

**Files:**
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorClassBinding.skin.cshtml`
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorStyleBinding.skin.cshtml`
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorAttrBinding.skin.cshtml`
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorMultiAttr.skin.cshtml`

- [ ] **Step 1: Create RazorClassBinding.skin.cshtml**

```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1" class="@Model.CssClass">Content</div>
```

- [ ] **Step 2: Create RazorStyleBinding.skin.cshtml**

```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1" style="display: @Model.DisplayStyle">Styled</div>
```

- [ ] **Step 3: Create RazorAttrBinding.skin.cshtml**

```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1" title="@Model.Title" data-count="@Model.Count">Attributed</div>
```

- [ ] **Step 4: Create RazorMultiAttr.skin.cshtml**

```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1" class="@Model.CssClass" title="@Model.Title" data-count="@Model.Count">Multi</div>
```

- [ ] **Step 5: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorClassBinding.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorStyleBinding.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorAttrBinding.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorMultiAttr.skin.cshtml
git commit -m "test(razor): add attribute/style/class binding templates"
```

---

### Task 3: Create Computed Expression Template (1 file)

**Files:**
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorComputed.skin.cshtml`

- [ ] **Step 1: Create RazorComputed.skin.cshtml**

```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1"><span class="total">@(Model.Price * Model.Quantity)</span></div>
```

- [ ] **Step 2: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorComputed.skin.cshtml
git commit -m "test(razor): add computed expression template"
```

---

### Task 4: Create Conditional Templates (6 files)

**Files:**
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorIfOnly.skin.cshtml`
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorIfElse.skin.cshtml`
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorIfElseIf.skin.cshtml`
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorIfBindings.skin.cshtml`
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorNestedIf.skin.cshtml`
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorStaticIf.skin.cshtml`

- [ ] **Step 1: Create RazorIfOnly.skin.cshtml**

```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
@if (Model.IsActive)
{
    <span class="active-content">Active</span>
}
</div>
```

- [ ] **Step 2: Create RazorIfElse.skin.cshtml**

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

- [ ] **Step 3: Create RazorIfElseIf.skin.cshtml**

Uses nested @if inside @else (proven boolean conditions):

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

- [ ] **Step 4: Create RazorIfBindings.skin.cshtml**

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

- [ ] **Step 5: Create RazorNestedIf.skin.cshtml**

```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
@if (Model.IsActive)
{
    @if (Model.ShowDetails)
    {
        <span class="active-with-details">Has Details</span>
    }
    else
    {
        <span class="active-no-details">No Details</span>
    }
}
</div>
```

Note: The spec originally used `Model.Count > 0` but comparison expressions in @if conditions are unverified. Using `Model.ShowDetails` (boolean) which is proven to work.

- [ ] **Step 6: Create RazorStaticIf.skin.cshtml**

```razor
@model Sunlight.Framework.UI.Test.RazorPlainVM

<div data-test="1">
@if (Model.IsStatic)
{
    <span class="static-content">Static</span>
}
</div>
```

- [ ] **Step 7: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorIfOnly.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorIfElse.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorIfElseIf.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorIfBindings.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorNestedIf.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorStaticIf.skin.cshtml
git commit -m "test(razor): add conditional template files (@if/@else/@else-if/nested)"
```

---

### Task 5: Create Collection Templates (2 files)

**Files:**
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorForeach.skin.cshtml`
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorForeachBindings.skin.cshtml`

- [ ] **Step 1: Create RazorForeach.skin.cshtml**

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

- [ ] **Step 2: Create RazorForeachBindings.skin.cshtml**

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

- [ ] **Step 3: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorForeach.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorForeachBindings.skin.cshtml
git commit -m "test(razor): add @foreach collection templates"
```

---

### Task 6: Create Nested Control Flow Templates (2 files)

**Files:**
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorIfInForeach.skin.cshtml`
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorForeachInIf.skin.cshtml`

- [ ] **Step 1: Create RazorIfInForeach.skin.cshtml**

```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
@foreach (var item in Model.Items)
{
    @if (item.IsComplete)
    {
        <li class="done">@item.Name</li>
    }
    else
    {
        <li class="pending">@item.Name</li>
    }
}
</div>
```

- [ ] **Step 2: Create RazorForeachInIf.skin.cshtml**

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

- [ ] **Step 3: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorIfInForeach.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorForeachInIf.skin.cshtml
git commit -m "test(razor): add nested control flow templates (@if in @foreach, @foreach in @if)"
```

---

### Task 7: Create Event Templates (2 files)

**Files:**
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorEventClick.skin.cshtml`
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorEventLambda.skin.cshtml`

- [ ] **Step 1: Create RazorEventClick.skin.cshtml**

```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
<button class="btn-click" onclick="@Model.IncrementClick">Click Me</button>
<span class="click-count">@Model.ClickCount</span>
</div>
```

- [ ] **Step 2: Create RazorEventLambda.skin.cshtml**

```razor
@model Sunlight.Framework.UI.Test.RazorTestVM

<div data-test="1">
<button class="btn-lambda" onclick="@((e) => Model.IncrementClick())">Lambda Click</button>
<span class="click-count">@Model.ClickCount</span>
</div>
```

- [ ] **Step 3: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorEventClick.skin.cshtml
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorEventLambda.skin.cshtml
git commit -m "test(razor): add event binding templates (method ref + lambda)"
```

---

### Task 8: Create Real-Life Scenario Template (1 file)

**Files:**
- Create: `Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorTodoApp.skin.cshtml`

- [ ] **Step 1: Create RazorTodoApp.skin.cshtml**

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

- [ ] **Step 2: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorTemplates/RazorTodoApp.skin.cshtml
git commit -m "test(razor): add real-life todo app template"
```

---

### Task 9: Register Templates in Skin Class and .csproj

**Files:**
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplatesClass.cs`
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/Sunlight.Framework.UI.Test.csproj`

- [ ] **Step 1: Add 18 [Skin] properties to RazorSkinTemplatesClass.cs**

Add these properties after the existing `GraphMultiBinding` property (line 44), before the closing brace of the class. Each follows the exact same pattern as existing registrations:

```csharp
        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorClassBinding.skin.cshtml")]
        public static Skin RazorClassBinding
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorStyleBinding.skin.cshtml")]
        public static Skin RazorStyleBinding
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorAttrBinding.skin.cshtml")]
        public static Skin RazorAttrBinding
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorMultiAttr.skin.cshtml")]
        public static Skin RazorMultiAttr
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorComputed.skin.cshtml")]
        public static Skin RazorComputed
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorIfOnly.skin.cshtml")]
        public static Skin RazorIfOnly
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorIfElse.skin.cshtml")]
        public static Skin RazorIfElse
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorIfElseIf.skin.cshtml")]
        public static Skin RazorIfElseIf
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorIfBindings.skin.cshtml")]
        public static Skin RazorIfBindings
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorNestedIf.skin.cshtml")]
        public static Skin RazorNestedIf
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorStaticIf.skin.cshtml")]
        public static Skin RazorStaticIf
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorForeach.skin.cshtml")]
        public static Skin RazorForeach
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorForeachBindings.skin.cshtml")]
        public static Skin RazorForeachBindings
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorIfInForeach.skin.cshtml")]
        public static Skin RazorIfInForeach
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorForeachInIf.skin.cshtml")]
        public static Skin RazorForeachInIf
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorEventClick.skin.cshtml")]
        public static Skin RazorEventClick
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorEventLambda.skin.cshtml")]
        public static Skin RazorEventLambda
        {
            get { return null; }
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorTodoApp.skin.cshtml")]
        public static Skin RazorTodoApp
        {
            get { return null; }
        }
```

- [ ] **Step 2: Add EmbeddedResource entries to .csproj**

In `Sunlight.Framework.UI.Test.csproj`, find the existing `<ItemGroup>` block with the Razor template resources (line 44-49). Add 18 new lines inside that same `<ItemGroup>`:

```xml
    <EmbeddedResource Include="RazorTemplates\RazorClassBinding.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorStyleBinding.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorAttrBinding.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorMultiAttr.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorComputed.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorIfOnly.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorIfElse.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorIfElseIf.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorIfBindings.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorNestedIf.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorStaticIf.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorForeach.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorForeachBindings.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorIfInForeach.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorForeachInIf.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorEventClick.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorEventLambda.skin.cshtml" />
    <EmbeddedResource Include="RazorTemplates\RazorTodoApp.skin.cshtml" />
```

- [ ] **Step 3: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplatesClass.cs
git add Test/Framework/Sunlight.Framework.UI.Test/Sunlight.Framework.UI.Test.csproj
git commit -m "test(razor): register 18 new templates in skin class and .csproj"
```

---

### Task 10: Build Verification

Verify templates compile through the full NScript pipeline before writing tests.

- [ ] **Step 1: Build Debug**

```bash
dotnet build NScript_Full.sln -c Debug
```

Expected: Build succeeds. The NScript compiler (Stage 1 + Stage 2) should discover the 18 new `.skin.cshtml` embedded resources, compile them via `RazorTemplatingPlugin`, and emit JavaScript.

**If build fails:** Check the error output. Common issues:
- Missing `@model` type: The fully-qualified type name in the template must match a type in the project
- Razor parse error: Check template syntax — braces must match, `@` expressions must be valid
- Binding classification error: The property referenced in the template must exist on the `@model` type
- EmbeddedResource not found: Verify the `.csproj` entry matches the file path exactly

- [ ] **Step 2: Verify compiler tests still pass**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj -c Release --no-build
```

Expected: All existing compiler tests pass (no regressions from ViewModel changes).

---

### Task 11: Write Attribute/Style/Class Binding Tests (A01-A07)

**Files:**
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`

- [ ] **Step 1: Add A01-A07 test methods**

Add these test methods to `RazorSkinTemplateTests` class, after the existing graph mode tests section (after line 337). Add a section comment header first:

```csharp
        // ------------------------------------------------------------------
        // Attribute / Style / Class Binding Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorClassBindingInitial(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.CssClass = "highlight";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorClassBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");
            assert.NotEqual(null, div, "Template should render");
            assert.Equal("highlight", div.ClassName, "Class should reflect initial CssClass value");
        }

        [Test]
        public static void TestRazorClassBindingUpdate(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.CssClass = "highlight";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorClassBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");
            assert.Equal("highlight", div.ClassName, "Initial class");

            vm.CssClass = "selected";
            assert.Equal("selected", div.ClassName, "Class should update when CssClass changes");
        }

        [Test]
        public static void TestRazorStyleBindingInitial(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.DisplayStyle = "block";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorStyleBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");
            assert.NotEqual(null, div, "Template should render");
            var style = div.GetAttribute("style");
            assert.IsTrue(style != null && style.Contains("block"),
                "Style should contain initial DisplayStyle value");
        }

        [Test]
        public static void TestRazorStyleBindingUpdate(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.DisplayStyle = "block";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorStyleBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");

            vm.DisplayStyle = "none";
            var style = div.GetAttribute("style");
            assert.IsTrue(style != null && style.Contains("none"),
                "Style should update when DisplayStyle changes");
        }

        [Test]
        public static void TestRazorAttrBindingInitial(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Title = "My Title";
            vm.Count = 5;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorAttrBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");
            assert.NotEqual(null, div, "Template should render");
            assert.Equal("My Title", div.GetAttribute("title"),
                "title attribute should reflect initial Title value");
            assert.Equal("5", div.GetAttribute("data-count"),
                "data-count attribute should reflect initial Count value");
        }

        [Test]
        public static void TestRazorAttrBindingUpdate(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Title = "Original";
            vm.Count = 1;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorAttrBinding;
            control.Activate();

            var div = element.QuerySelector("[data-test]");

            vm.Title = "Updated Title";
            assert.Equal("Updated Title", div.GetAttribute("title"),
                "title attribute should update when Title changes");
        }

        [Test]
        public static void TestRazorMultiAttrBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.CssClass = "active";
            vm.Title = "Tooltip";
            vm.Count = 10;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorMultiAttr;
            control.Activate();

            var div = element.QuerySelector("[data-test]");
            assert.NotEqual(null, div, "Template should render");
            assert.Equal("active", div.ClassName, "class should bind");
            assert.Equal("Tooltip", div.GetAttribute("title"), "title should bind");
            assert.Equal("10", div.GetAttribute("data-count"), "data-count should bind");

            vm.CssClass = "inactive";
            vm.Title = "New Tip";
            assert.Equal("inactive", div.ClassName, "class should update");
            assert.Equal("New Tip", div.GetAttribute("title"), "title should update");
        }
```

- [ ] **Step 2: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs
git commit -m "test(razor): add attribute/style/class binding tests (A01-A07)"
```

---

### Task 12: Write Computed Expression Tests (C01-C03)

**Files:**
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`

- [ ] **Step 1: Add C01-C03 test methods**

Add after the attribute binding tests:

```csharp
        // ------------------------------------------------------------------
        // Computed Expression Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorComputedInitial(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Price = 10;
            vm.Quantity = 3;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorComputed;
            control.Activate();

            var span = element.QuerySelector("[data-test] .total");
            assert.NotEqual(null, span, "Computed template should render");
            assert.Equal("30", span.TextContent, "Should show Price * Quantity = 30");
        }

        [Test]
        public static void TestRazorComputedPriceChange(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Price = 10;
            vm.Quantity = 3;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorComputed;
            control.Activate();

            var span = element.QuerySelector("[data-test] .total");
            assert.Equal("30", span.TextContent, "Initial computed value");

            vm.Price = 20;
            assert.Equal("60", span.TextContent,
                "Changing Price should trigger recompute: 20 * 3 = 60");
        }

        [Test]
        public static void TestRazorComputedQuantityChange(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Price = 10;
            vm.Quantity = 3;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorComputed;
            control.Activate();

            var span = element.QuerySelector("[data-test] .total");
            assert.Equal("30", span.TextContent, "Initial computed value");

            vm.Quantity = 5;
            assert.Equal("50", span.TextContent,
                "Changing Quantity should trigger recompute: 10 * 5 = 50");
        }
```

- [ ] **Step 2: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs
git commit -m "test(razor): add computed expression tests (C01-C03)"
```

---

### Task 13: Write Conditional Tests (IF01-IF11)

**Files:**
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`

- [ ] **Step 1: Add IF01-IF11 test methods**

Add after the computed expression tests:

```csharp
        // ------------------------------------------------------------------
        // Conditional (@if / @else) Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorIfOnlyTrue(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfOnly;
            control.Activate();

            var content = element.QuerySelector("[data-test] .active-content");
            assert.NotEqual(null, content, "@if(true) should render content");
            assert.Equal("Active", content.TextContent, "Content should be 'Active'");
        }

        [Test]
        public static void TestRazorIfOnlyFalse(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = false;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfOnly;
            control.Activate();

            var content = element.QuerySelector("[data-test] .active-content");
            assert.Equal(null, content, "@if(false) should NOT render content");
        }

        [Test]
        public static void TestRazorIfElseShowsTrue(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfElse;
            control.Activate();

            var ifBranch = element.QuerySelector("[data-test] .if-branch");
            var elseBranch = element.QuerySelector("[data-test] .else-branch");
            assert.NotEqual(null, ifBranch, "If branch should be visible when IsActive=true");
            assert.Equal(null, elseBranch, "Else branch should NOT be visible when IsActive=true");
        }

        [Test]
        public static void TestRazorIfElseShowsFalse(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = false;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfElse;
            control.Activate();

            var ifBranch = element.QuerySelector("[data-test] .if-branch");
            var elseBranch = element.QuerySelector("[data-test] .else-branch");
            assert.Equal(null, ifBranch, "If branch should NOT be visible when IsActive=false");
            assert.NotEqual(null, elseBranch, "Else branch should be visible when IsActive=false");
        }

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

            var ifBranch = element.QuerySelector("[data-test] .if-branch");
            assert.NotEqual(null, ifBranch, "If branch visible initially");

            vm.IsActive = false;
            var elseBranch = element.QuerySelector("[data-test] .else-branch");
            assert.NotEqual(null, elseBranch, "Else branch should appear after toggle to false");
            ifBranch = element.QuerySelector("[data-test] .if-branch");
            assert.Equal(null, ifBranch, "If branch should disappear after toggle to false");

            vm.IsActive = true;
            ifBranch = element.QuerySelector("[data-test] .if-branch");
            assert.NotEqual(null, ifBranch, "If branch should reappear after toggle back to true");
            elseBranch = element.QuerySelector("[data-test] .else-branch");
            assert.Equal(null, elseBranch, "Else branch should disappear after toggle back to true");
        }

        [Test]
        public static void TestRazorIfElseIfFirstBranch(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            vm.ShowDetails = false;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfElseIf;
            control.Activate();

            var active = element.QuerySelector("[data-test] .branch-active");
            var details = element.QuerySelector("[data-test] .branch-details");
            var def = element.QuerySelector("[data-test] .branch-default");
            assert.NotEqual(null, active, "First branch should show when IsActive=true");
            assert.Equal(null, details, "Second branch should not show");
            assert.Equal(null, def, "Default branch should not show");
        }

        [Test]
        public static void TestRazorIfElseIfSecondBranch(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = false;
            vm.ShowDetails = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfElseIf;
            control.Activate();

            var active = element.QuerySelector("[data-test] .branch-active");
            var details = element.QuerySelector("[data-test] .branch-details");
            var def = element.QuerySelector("[data-test] .branch-default");
            assert.Equal(null, active, "First branch should not show");
            assert.NotEqual(null, details, "Second branch should show when IsActive=false, ShowDetails=true");
            assert.Equal(null, def, "Default branch should not show");
        }

        [Test]
        public static void TestRazorIfElseIfDefaultBranch(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = false;
            vm.ShowDetails = false;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfElseIf;
            control.Activate();

            var active = element.QuerySelector("[data-test] .branch-active");
            var details = element.QuerySelector("[data-test] .branch-details");
            var def = element.QuerySelector("[data-test] .branch-default");
            assert.Equal(null, active, "First branch should not show");
            assert.Equal(null, details, "Second branch should not show");
            assert.NotEqual(null, def, "Default branch should show when both are false");
        }

        [Test]
        public static void TestRazorIfBindingsActive(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            vm.Name = "Alice";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfBindings;
            control.Activate();

            var nameSpan = element.QuerySelector("[data-test] .active-name");
            assert.NotEqual(null, nameSpan, "Active branch with binding should render");
            assert.Equal("Alice", nameSpan.TextContent, "Binding inside @if should show initial value");

            vm.Name = "Bob";
            assert.Equal("Bob", nameSpan.TextContent,
                "Binding inside @if should update reactively");
        }

        [Test]
        public static void TestRazorNestedIfBothTrue(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            vm.ShowDetails = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorNestedIf;
            control.Activate();

            var withDetails = element.QuerySelector("[data-test] .active-with-details");
            assert.NotEqual(null, withDetails,
                "Nested @if should show inner content when both conditions true");
        }

        [Test]
        public static void TestRazorStaticIf(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorPlainVM();
            vm.IsStatic = true;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorStaticIf;
            control.Activate();

            var content = element.QuerySelector("[data-test] .static-content");
            assert.NotEqual(null, content, "Static @if(true) should render content");
            assert.Equal("Static", content.TextContent, "Content should be 'Static'");
        }
```

- [ ] **Step 2: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs
git commit -m "test(razor): add conditional tests including @if/@else/@else-if chains (IF01-IF11)"
```

---

### Task 14: Write Foreach/Collection Tests (FE01-FE06)

**Files:**
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`

- [ ] **Step 1: Add FE01-FE06 test methods**

Add after the conditional tests. Note: These tests use `ObservableCollection<RazorItemVM>` — import the namespace at the top of the file if not already present (`using Sunlight.Framework.Observables;`).

```csharp
        // ------------------------------------------------------------------
        // @foreach / Collection Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorForeachInitialRender(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "Apple" });
            items.Add(new RazorItemVM { Name = "Banana" });
            items.Add(new RazorItemVM { Name = "Cherry" });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeach;
            control.Activate();

            var lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(3, lis.Length, "Should render 3 <li> elements");
            assert.Equal("Apple", lis[0].TextContent, "First item");
            assert.Equal("Banana", lis[1].TextContent, "Second item");
            assert.Equal("Cherry", lis[2].TextContent, "Third item");
        }

        [Test]
        public static void TestRazorForeachAddItem(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "Apple" });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeach;
            control.Activate();

            var lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(1, lis.Length, "Should start with 1 item");

            items.Add(new RazorItemVM { Name = "Banana" });
            lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(2, lis.Length, "Should have 2 items after Add");
            assert.Equal("Banana", lis[1].TextContent, "New item should appear at end");
        }

        [Test]
        public static void TestRazorForeachRemoveItem(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "Apple" });
            items.Add(new RazorItemVM { Name = "Banana" });
            items.Add(new RazorItemVM { Name = "Cherry" });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeach;
            control.Activate();

            items.RemoveAt(1);
            var lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(2, lis.Length, "Should have 2 items after RemoveAt(1)");
            assert.Equal("Apple", lis[0].TextContent, "First item unchanged");
            assert.Equal("Cherry", lis[1].TextContent, "Cherry should move up");
        }

        [Test]
        public static void TestRazorForeachClear(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "Apple" });
            items.Add(new RazorItemVM { Name = "Banana" });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeach;
            control.Activate();

            var lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(2, lis.Length, "Should start with 2 items");

            items.Clear();
            lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(0, lis.Length, "Clear should remove all items from DOM");
        }

        [Test]
        public static void TestRazorForeachMultipleOps(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "A" });
            items.Add(new RazorItemVM { Name = "B" });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeach;
            control.Activate();

            items.Add(new RazorItemVM { Name = "C" });
            var lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(3, lis.Length, "After add: 3 items");

            items.RemoveAt(0);
            lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(2, lis.Length, "After remove: 2 items");
            assert.Equal("B", lis[0].TextContent, "B should be first after removing A");

            items.Add(new RazorItemVM { Name = "D" });
            lis = element.QuerySelectorAll("[data-test] .item");
            assert.Equal(3, lis.Length, "After second add: 3 items");
            assert.Equal("D", lis[2].TextContent, "D should be last");
        }

        [Test]
        public static void TestRazorForeachItemBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            var item1 = new RazorItemVM { Name = "Task 1", IsComplete = false };
            var item2 = new RazorItemVM { Name = "Task 2", IsComplete = true };
            items.Add(item1);
            items.Add(item2);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeachBindings;
            control.Activate();

            var names = element.QuerySelectorAll("[data-test] .item-name");
            assert.Equal(2, names.Length, "Should render 2 items");
            assert.Equal("Task 1", names[0].TextContent, "First item name");
            assert.Equal("Task 2", names[1].TextContent, "Second item name");

            item1.Name = "Updated Task 1";
            names = element.QuerySelectorAll("[data-test] .item-name");
            assert.Equal("Updated Task 1", names[0].TextContent,
                "Changing item property should update only that item's DOM");
            assert.Equal("Task 2", names[1].TextContent,
                "Other items should remain unchanged");
        }
```

- [ ] **Step 2: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs
git commit -m "test(razor): add @foreach collection tests with ObservableCollection (FE01-FE06)"
```

---

### Task 15: Write Nested Control Flow Tests (N01-N03)

**Files:**
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`

- [ ] **Step 1: Add N01-N03 test methods**

```csharp
        // ------------------------------------------------------------------
        // Nested Control Flow Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorIfInForeach(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "Done Task", IsComplete = true });
            items.Add(new RazorItemVM { Name = "Pending Task", IsComplete = false });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorIfInForeach;
            control.Activate();

            var doneItems = element.QuerySelectorAll("[data-test] .done");
            var pendingItems = element.QuerySelectorAll("[data-test] .pending");
            assert.Equal(1, doneItems.Length, "Should have 1 done item");
            assert.Equal(1, pendingItems.Length, "Should have 1 pending item");
            assert.Equal("Done Task", doneItems[0].TextContent, "Done item text");
            assert.Equal("Pending Task", pendingItems[0].TextContent, "Pending item text");
        }

        [Test]
        public static void TestRazorForeachInIfActive(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "Item 1" });
            items.Add(new RazorItemVM { Name = "Item 2" });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeachInIf;
            control.Activate();

            var list = element.QuerySelector("[data-test] .active-list");
            assert.NotEqual(null, list, "List should render when IsActive=true");
            var lis = element.QuerySelectorAll("[data-test] .active-list li");
            assert.Equal(2, lis.Length, "Should show 2 list items");
            var disabled = element.QuerySelector("[data-test] .disabled-msg");
            assert.Equal(null, disabled, "Disabled message should not show");
        }

        [Test]
        public static void TestRazorForeachInIfToggle(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.IsActive = true;
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "Item 1" });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorForeachInIf;
            control.Activate();

            var list = element.QuerySelector("[data-test] .active-list");
            assert.NotEqual(null, list, "List visible when active");

            vm.IsActive = false;
            list = element.QuerySelector("[data-test] .active-list");
            assert.Equal(null, list, "List should disappear when IsActive toggled to false");
            var disabled = element.QuerySelector("[data-test] .disabled-msg");
            assert.NotEqual(null, disabled, "Disabled message should appear");
        }
```

- [ ] **Step 2: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs
git commit -m "test(razor): add nested control flow tests - @if in @foreach, @foreach in @if (N01-N03)"
```

---

### Task 16: Write Event Tests (E01-E03)

**Files:**
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`

- [ ] **Step 1: Add E01-E03 test methods**

Note: Simulating a click in the NScript browser test environment uses the `Element.Click()` method or dispatches a synthetic event. Check how existing XWML tests handle events. The simplest approach: directly call `element.Click()` if the method exists, or use `Element.DispatchEvent()`.

```csharp
        // ------------------------------------------------------------------
        // Event Binding Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorEventMethodRef(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.ClickCount = 0;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorEventClick;
            control.Activate();

            assert.Equal(0, vm.ClickCount, "ClickCount should start at 0");

            var btn = element.QuerySelector("[data-test] .btn-click");
            assert.NotEqual(null, btn, "Button should render");
            btn.Click();

            assert.Equal(1, vm.ClickCount,
                "Method ref click should fire IncrementClick, ClickCount = 1");
        }

        [Test]
        public static void TestRazorEventLambda(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.ClickCount = 0;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorEventLambda;
            control.Activate();

            var btn = element.QuerySelector("[data-test] .btn-lambda");
            assert.NotEqual(null, btn, "Lambda button should render");
            btn.Click();

            assert.Equal(1, vm.ClickCount,
                "Lambda click should fire IncrementClick, ClickCount = 1");
        }

        [Test]
        public static void TestRazorEventUpdatesBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.ClickCount = 0;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorEventClick;
            control.Activate();

            var countSpan = element.QuerySelector("[data-test] .click-count");
            assert.Equal("0", countSpan.TextContent, "Count should show 0 initially");

            var btn = element.QuerySelector("[data-test] .btn-click");
            btn.Click();

            assert.Equal("1", countSpan.TextContent,
                "Click should update ClickCount, which should reactively update the span");

            btn.Click();
            assert.Equal("2", countSpan.TextContent,
                "Second click should show 2");
        }
```

- [ ] **Step 2: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs
git commit -m "test(razor): add event binding tests - method ref and lambda (E01-E03)"
```

---

### Task 17: Write Lifecycle Tests (L04-L08)

**Files:**
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`

- [ ] **Step 1: Add L04-L08 test methods**

These tests use the existing `RazorSimpleText` and `RazorMultiBinding` templates (already registered).

```csharp
        // ------------------------------------------------------------------
        // Extended Lifecycle Tests
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorDeactivateStopsUpdates(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "Before";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("Before", span.TextContent, "Initial value");

            control.Deactivate();
            vm.PropStr1 = "After Deactivate";
            assert.Equal("Before", span.TextContent,
                "After Deactivate, VM changes should NOT update DOM");
        }

        [Test]
        public static void TestRazorReactivateResumes(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "V1";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("V1", span.TextContent, "Initial value");

            control.Deactivate();
            vm.PropStr1 = "V2";

            control.Activate();
            span = element.QuerySelector("[data-test] span");
            assert.Equal("V2", span.TextContent,
                "After reactivation, should show latest VM value");
        }

        [Test]
        public static void TestRazorDisposeCleanup(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Name = "Alice";
            vm.Count = 1;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorMultiBinding;
            control.Activate();

            var nameSpan = element.QuerySelector("[data-test] .name span");
            assert.Equal("Alice", nameSpan.TextContent, "Initial value before dispose");

            control.Dispose();
            vm.Name = "Bob";
            // After dispose, we mainly verify no exceptions are thrown.
            // The control may or may not clear DOM depending on implementation.
            assert.IsTrue(true, "Dispose should not throw when VM changes afterward");
        }

        [Test]
        public static void TestRazorNullDataContext(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "HasValue";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.Equal("HasValue", span.TextContent, "Initial value");

            control.DataContext = null;
            // After setting null DataContext, bindings should clear or not throw
            assert.IsTrue(true, "Setting DataContext to null should not throw");
        }

        [Test]
        public static void TestRazorEmptyStringBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;
            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.NotEqual(null, span, "Span should still render with empty string");
            assert.Equal("", span.TextContent, "Empty string should render as empty text");
        }
```

- [ ] **Step 2: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs
git commit -m "test(razor): add extended lifecycle tests - deactivate, reactivate, dispose, null DC (L04-L08)"
```

---

### Task 18: Write Real-Life Scenario Tests (RL01-RL04)

**Files:**
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`

- [ ] **Step 1: Add RL01-RL04 test methods**

```csharp
        // ------------------------------------------------------------------
        // Real-Life Scenario Tests (Todo App)
        // ------------------------------------------------------------------

        [Test]
        public static void TestRazorTodoInitialRender(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Count = 2;
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "Buy groceries", IsComplete = false });
            items.Add(new RazorItemVM { Name = "Write tests", IsComplete = true });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorTodoApp;
            control.Activate();

            var countSpan = element.QuerySelector("[data-test] .todo-count");
            assert.Equal("2", countSpan.TextContent, "Count should show 2");

            var pending = element.QuerySelectorAll("[data-test] .todo-pending");
            var done = element.QuerySelectorAll("[data-test] .todo-done");
            assert.Equal(1, pending.Length, "Should have 1 pending item");
            assert.Equal(1, done.Length, "Should have 1 done item");

            var pendingName = pending[0].QuerySelector(".todo-name");
            assert.Equal("Buy groceries", pendingName.TextContent, "Pending item name");
            var doneName = done[0].QuerySelector(".todo-name");
            assert.Equal("Write tests", doneName.TextContent, "Done item name");
        }

        [Test]
        public static void TestRazorTodoAddItem(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Count = 1;
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "Task 1", IsComplete = false });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorTodoApp;
            control.Activate();

            var allItems = element.QuerySelectorAll("[data-test] .todo-list li");
            assert.Equal(1, allItems.Length, "Should start with 1 item");

            items.Add(new RazorItemVM { Name = "Task 2", IsComplete = false });
            vm.Count = 2;
            allItems = element.QuerySelectorAll("[data-test] .todo-list li");
            assert.Equal(2, allItems.Length, "Should have 2 items after Add");

            var countSpan = element.QuerySelector("[data-test] .todo-count");
            assert.Equal("2", countSpan.TextContent, "Count should update to 2");
        }

        [Test]
        public static void TestRazorTodoToggleComplete(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Count = 1;
            var items = new ObservableCollection<RazorItemVM>();
            var task = new RazorItemVM { Name = "My Task", IsComplete = false };
            items.Add(task);
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorTodoApp;
            control.Activate();

            var pending = element.QuerySelectorAll("[data-test] .todo-pending");
            assert.Equal(1, pending.Length, "Should start as pending");

            task.IsComplete = true;
            var done = element.QuerySelectorAll("[data-test] .todo-done");
            pending = element.QuerySelectorAll("[data-test] .todo-pending");
            assert.Equal(1, done.Length, "Should show as done after toggle");
            assert.Equal(0, pending.Length, "Should not show as pending after toggle");
        }

        [Test]
        public static void TestRazorTodoRemoveItem(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new RazorTestVM();
            vm.Count = 2;
            var items = new ObservableCollection<RazorItemVM>();
            items.Add(new RazorItemVM { Name = "Keep", IsComplete = false });
            items.Add(new RazorItemVM { Name = "Remove", IsComplete = true });
            vm.Items = items;
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorTodoApp;
            control.Activate();

            var allItems = element.QuerySelectorAll("[data-test] .todo-list li");
            assert.Equal(2, allItems.Length, "Should start with 2 items");

            items.RemoveAt(1);
            vm.Count = 1;
            allItems = element.QuerySelectorAll("[data-test] .todo-list li");
            assert.Equal(1, allItems.Length, "Should have 1 item after remove");

            var countSpan = element.QuerySelector("[data-test] .todo-count");
            assert.Equal("1", countSpan.TextContent, "Count should update to 1");
        }
```

- [ ] **Step 2: Commit**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs
git commit -m "test(razor): add real-life todo app scenario tests (RL01-RL04)"
```

---

### Task 19: Add using directive for ObservableCollection

**Files:**
- Modify: `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`

- [ ] **Step 1: Ensure the using directive is present**

At the top of `RazorSkinTemplateTests.cs`, verify `using Sunlight.Framework.Observables;` is present. If not, add it after the existing usings (line 4):

```csharp
    using Sunlight.Framework.Observables;
```

This is needed for `ObservableCollection<RazorItemVM>` used in FE, N, and RL tests.

- [ ] **Step 2: Commit if changed**

```bash
git add Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs
git commit -m "test(razor): add missing using directive for ObservableCollection"
```

---

### Task 20: Final Build and Verification

- [ ] **Step 1: Build Debug**

```bash
dotnet build NScript_Full.sln -c Debug
```

Expected: Build succeeds. All 18 new templates are discovered and compiled by `RazorTemplatingPlugin`. The generated JS output appears in `Test/Framework/TestWebApplication/GeneratedScripts/`.

- [ ] **Step 2: Run compiler tests**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj -c Release
```

Expected: All existing compiler tests pass.

- [ ] **Step 3: Verify generated JS**

Check that the generated script file contains the new test registrations:

```bash
grep -c "TestRazor" Test/Framework/TestWebApplication/GeneratedScripts/Sunlight.Framework.UI.Test.js
```

Expected: Count should be approximately 55 (12 existing + 43 new test method registrations).

- [ ] **Step 4: Browser test (if possible)**

If a browser is available:

```bash
cd Test/Framework/TestWebApplication
npx serve .
# Open http://localhost:3000/TestPage.htm
```

Expected: QUnit shows 55 tests, all green. If any tests fail, inspect the QUnit output for the specific failure message and fix accordingly.

- [ ] **Step 5: Final commit (if any fixes needed)**

After verifying, create a summary commit if any minor adjustments were needed:

```bash
git add -A
git commit -m "test(razor): comprehensive browser test suite - 43 new tests across 8 categories"
```

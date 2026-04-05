# Design: TodoItemControl — Reusable Todo Item Sub-Control

**Date:** 2026-04-05  
**Status:** Approved

## Problem

`AppShell.skin.cshtml` has near-identical todo item templates in two places:
- Active todo list (lines 28-32): checkbox + title + star, with draggable + onclick + ondragstart
- Completed todo list (lines 43-47): checkbox + title + star, with onclick

This duplication means any UI change to a todo item must be made twice.

## Solution

Extract a `TodoItemControl` Razor sub-control that:
1. Renders a single todo item (checkbox, title, star) from a `TodoItemViewModel` data context
2. Handles drag-start as a drag source
3. Replaces both inline foreach bodies in `AppShell.skin.cshtml`

## Components

### 1. `TodoItemControl.cs` (new)
- Extends `UISkinableElement`
- Has a `[Skin]` static property pointing to `TodoItemControl.skin.cshtml`
- Data context is `TodoItemViewModel` (set automatically by the parent `@foreach`)

### 2. `TodoItemControl.skin.cshtml` (new)
- `@model TodoApp.ViewModels.TodoItemViewModel`
- `@styles "TodoApp.RazorTemplates.AppShell.css"` (shares parent CSS)
- Template:
  ```html
  <div class="@Model.CssClass" draggable="true" ondragstart="@Model.OnDragStart">
      <div class="@Model.CheckboxClass" onclick="@Model.ToggleComplete">✓</div>
      <div class="todo-title">@Model.Title</div>
      <div class="@Model.StarClass" onclick="@Model.ToggleImportant">@Model.StarText</div>
  </div>
  ```

### 3. `TodoItemViewModel` changes
- Add `OnDragStart(object e, object ev)` method — calls `appViewModel.OnDragStart(this)`
- Add `StarClass` computed property (replaces inline ternary)
- Add `StarText` computed property (replaces inline ternary for ★/☆)

### 4. `AppShell.skin.cshtml` changes
Both foreach blocks simplify to:
```html
@foreach (var todo in Model.CurrentTodos)
{
    <TodoItemControl onclick="@Model.OnSelectTodo(todo)" />
}
```

## Drag-and-Drop

- **Drag source only** (no drop target on items, only folders accept drops)
- `draggable="true"` set on the control's root div
- `ondragstart` calls `TodoItemViewModel.OnDragStart()` → `AppViewModel.OnDragStart(this)`
- This moves drag responsibility from parent template closures into the ViewModel

## CSS

- The control shares `AppShell.css` via `@styles`
- All CSS class names already registered via `[CssClass]` in `AppShellCss.cs`
- No new CSS classes needed

## Testing

- Existing E2E tests should continue passing (same DOM structure)
- Update `buildClassMap` if the control introduces a new wrapper element
- Verify drag-drop still works in E2E drag tests

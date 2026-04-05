# Design: TodoItemControl — Reusable Todo Item Sub-Control

**Date:** 2026-04-05  
**Status:** Partially Implemented

## Problem

`AppShell.skin.cshtml` has near-identical todo item templates in two places:
- Active todo list: checkbox + title + star, with draggable + onclick + ondragstart
- Completed todo list: checkbox + title + star, with onclick

This duplication means any UI change to a todo item must be made twice. Additionally, inline ternary expressions for CSS classes (checked/star states) add noise to the template.

## Solution: ViewModel-Driven Templates

### What was implemented

Moved computed display logic from inline template expressions into `TodoItemViewModel` properties:
- `StarClass` — computed CSS class for star icon (`star` vs `star important`)
- `StarText` — computed text glyph (`★` vs `☆`)
- `CheckboxClass` — (already existed, now used consistently)
- `OnDragStart(object e, object ev)` — delegates drag to `AppViewModel`
- `OnSelect()` — (already existed) delegates selection to `AppViewModel`

Both template loops now use clean property bindings (`@todo.StarClass`, `@todo.StarText`, `@todo.OnSelect`) instead of inline ternary expressions and parent-model lambda closures.

### What was NOT implemented (deferred)

The `<TodoItemControl />` sub-control approach was attempted but the compiler's `SubControlNode` code generation is not yet implemented for `@foreach` loops. The `GraphTopologyBuilder` has a placeholder comment: "Sub-controls could be expanded later." The runtime `GraphEngine__RenderCollectionItems` treats item templates as raw HTML strings and doesn't instantiate sub-control types.

The `TodoItemControl.cs` and `TodoItemControl.skin.cshtml` files are retained as groundwork for when sub-control support in foreach loops is added to the compiler.

## Files Changed

- `TodoItemViewModel.cs` — Added `StarClass`, `StarText`, `OnDragStart` + updated `UpdateComputedProperties`
- `AppShell.skin.cshtml` — Both foreach bodies use ViewModel properties instead of inline ternaries
- `Controls/TodoItemControl.cs` — New (groundwork for future sub-control support)
- `RazorTemplates/TodoItemControl.skin.cshtml` — New (groundwork)
- `TodoApp.csproj` — Added TodoItemControl.skin.cshtml as EmbeddedResource

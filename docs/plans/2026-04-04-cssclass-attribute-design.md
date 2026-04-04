# Design: [CssClass] Attribute for CSS Name Minification in Razor Templates

## Date: 2026-04-04
## Status: Approved (Strategy A — literal matching, full-format attribute)

## Problem

The Razor CSS pipeline (`@styles` directive) can minify CSS class names in:
- **Static HTML** — `class="pane-header"` in `.skin.cshtml` templates
- **CSS rules** — `.pane-header { ... }` in the stylesheet

But it **cannot** minify CSS class names in:
- **Dynamic C# code** — `Model.LeftPaneClass = "pane-left"` (runtime strings)
- **Ternary expressions** — `@(IsCollapsed ? "pane-left collapsed" : "pane-left")`

This means `CompressNames()` (identifier minification) breaks templates with
dynamic class references because the CSS rules use minified names but C# strings
stay unminified.

## Solution: [CssClass] Attribute on Const String Members

```csharp
// In ViewModel:
[CssClass("TodoApp.RazorTemplates.AppShell.css:pane-left")]
public const string PaneLeft = "pane-left";

[CssClass("TodoApp.RazorTemplates.AppShell.css:collapsed")]
public const string Collapsed = "collapsed";

// Usage — each const becomes a separate string literal in JS:
public string LeftPaneClass => IsCollapsed
    ? PaneLeft + " " + Collapsed    // "a" + " " + "b" after minification
    : PaneLeft;                      // "a"
```

### Attribute Rules
1. **Target**: `const string` fields only — throw compile error on anything else
2. **Format**: `[CssClass("EmbeddedResourceName:CssClassName")]`
   - Resource name: assembly-qualified embedded resource (e.g., `TodoApp.RazorTemplates.AppShell.css`)
   - Class name: CSS class to resolve (must exist in that stylesheet)
3. **Validation**: If the CSS class doesn't exist in the specified stylesheet → compile error
4. **Cross-validation**: The const value MUST equal the CSS class name (redundancy is intentional for readability)

### Why `const` + Literal Matching (Strategy A)

**Constraint**: NScript's Stage 1 compiler folds const field references to literal values
(`BoundAstToAstBase.cs:840`). By Stage 2 (JS emission), `PaneLeft` is just the string
`"pane-left"` — the field symbol is gone.

**Approach**: The Razor plugin scans the assembly for `[CssClass]` const fields, builds
a value→identifier map, and replaces matching `StringLiteralExpression` nodes in the JST.

This mirrors XWML's `CssNameValue` pattern:
- `CssNameValue` splits class strings on spaces, resolves each via `TryGetCssClassIdentifier()`
- Emits `IdentifierStringExpression` (the identifier, not the raw string)
- Our approach does the same, but as a JST post-processing pass instead of template parsing

### Compound Classes

Each CSS class is a separate const. String concatenation preserves individual literals:

```csharp
// C# code:
PaneLeft + " " + Collapsed
// After const folding, JS has: "pane-left" + " " + "collapsed"
// After CSS replacement:       "a" + " " + "b"
// JS runtime result:           "a b"
```

**NOT supported**: `PaneLeft + " collapsed"` — the space-prefixed string ` collapsed`
won't match. Must use individual consts for each class token.

## Architecture

### Component Overview

```
┌──────────────────────────────────────────────────────┐
│ Framework: CssClassAttribute definition              │
│ (Sunlight.Framework.UI/Attributes/CssClassAttribute) │
└──────────────┬───────────────────────────────────────┘
               │ attribute on const string fields
               ▼
┌──────────────────────────────────────────────────────┐
│ Stage 1: Roslyn serialization                        │
│ (const folded to literal — [CssClass] info lost)     │
└──────────────┬───────────────────────────────────────┘
               │ DLL with embedded AST
               ▼
┌──────────────────────────────────────────────────────┐
│ Stage 2: RazorTemplatingPlugin.Initialize()          │
│ 1. Scan assembly types for [CssClass] const fields   │
│ 2. Parse attribute → (resource, className)           │
│ 3. Resolve via RazorCssManager.TryGetCssClassId()    │
│ 4. Build cssLiteralReplacements map                  │
│ 5. Enable CompressNames()                            │
└──────────────┬───────────────────────────────────────┘
               │ map: { "pane-left" => IIdentifier }
               ▼
┌──────────────────────────────────────────────────────┐
│ JST Post-Processing (in GetPostJavascript or plugin) │
│ Walk JST tree, replace StringLiteralExpression nodes  │
│ whose value matches registered CSS class → emit       │
│ IdentifierExpression (minified name)                 │
└──────────────────────────────────────────────────────┘
```

### New Files
- `Sources/Framework/Sunlight.Framework.UI/Attributes/CssClassAttribute.cs`

### Modified Files
- `Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs`
  - Add `ScanCssClassAttributes()` method
  - Add `_cssLiteralReplacements` dictionary
  - Modify `LoadCssForTemplates()` to call `CompressNames()` when replacements exist
  - Add JST visitor for literal replacement
- `Sources/Compiler/RazorSkinParser/RazorCssManager.cs`
  - May need to expose CSS scope for identifier lookup
- `Test/Framework/TodoApp/ViewModels/AppViewModel.cs` (or new CssClasses.cs)
  - Add `[CssClass]` consts for TodoApp demo
- `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs`
  - QUnit tests for minified class name verification

## Implementation Tasks

### Task 1: Define CssClassAttribute
- Create `CssClassAttribute.cs` in `Sunlight.Framework.UI/Attributes/`
- `[AttributeUsage(AttributeTargets.Field)]`
- Constructor: `CssClassAttribute(string cssClassReference)`
- Property: `string CssClassReference { get; }`

### Task 2: Scan and Validate [CssClass] Fields
- In `RazorTemplatingPlugin.Initialize()`, after loading CSS:
  - Walk all types in assembly via Cecil
  - Find const string fields with `[CssClass]` attribute
  - Validate: field must be const, type must be string
  - Parse attribute: split on `:` → (resourceName, className)
  - Validate: resource must be a loaded CSS file
  - Validate: className must exist in that CSS file's RazorCssManager
  - Validate: field's const value must equal className
  - Build `_cssLiteralReplacements` dictionary

### Task 3: Enable CompressNames()
- Re-enable `CompressNames()` call in `LoadCssForTemplates()`
- Only when `[CssClass]` consts cover all dynamic class references

### Task 4: JST Literal Replacement Pass
- Create `CssLiteralReplacementVisitor` (JST visitor)
- Walk all StringLiteralExpression nodes
- If value matches a key in `_cssLiteralReplacements`, replace with
  `IdentifierExpression` using the resolved CSS identifier
- Apply after JST generation but before JS emission

### Task 5: TodoApp Demo
- Create `CssClasses.cs` in TodoApp with [CssClass] consts
- Update AppViewModel to reference consts instead of string literals
- Verify minified output works end-to-end

### Task 6: QUnit Tests
- Test: const with [CssClass] resolves to minified name
- Test: non-const [CssClass] throws compile error
- Test: missing CSS class throws compile error
- Test: compound classes via concatenation work
- Test: end-to-end rendering with minified classes

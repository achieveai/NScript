# Design: [CssClass] Attribute for CSS Name Minification in Razor Templates

## Date: 2026-04-04
## Status: Approved — space-aware literal matching, global replacement, full-format attribute
## Updated: 2026-04-04 (fleet analysis: GPT-5.4 ×2, Opus ×1)

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
// CssClasses.cs — one file per CSS resource
public static class AppShellCss
{
    [CssClass("TodoApp.RazorTemplates.AppShell.css:pane-left")]
    public const string PaneLeft = "pane-left";

    [CssClass("TodoApp.RazorTemplates.AppShell.css:collapsed")]
    public const string Collapsed = "collapsed";

    [CssClass("TodoApp.RazorTemplates.AppShell.css:todo-item")]
    public const string TodoItem = "todo-item";

    [CssClass("TodoApp.RazorTemplates.AppShell.css:selected")]
    public const string Selected = "selected";
}

// ViewModel usage:
public string LeftPaneClass => IsCollapsed
    ? AppShellCss.PaneLeft + " " + AppShellCss.Collapsed
    : AppShellCss.PaneLeft;
```

### Attribute Rules
1. **Target**: `const string` fields only — throw compile error on anything else
2. **Format**: `[CssClass("EmbeddedResourceName:CssClassName")]`
   - Resource name: assembly-qualified embedded resource (e.g., `TodoApp.RazorTemplates.AppShell.css`)
   - Class name: CSS class to resolve (must exist in that stylesheet)
3. **Validation**: If the CSS class doesn't exist in the specified stylesheet → compile error
4. **Cross-validation**: The const value MUST equal the CSS class name (redundancy is intentional for readability)

## Critical Finding: Const Folding and String Concatenation

### Const Field Folding (confirmed)
NScript's Stage 1 compiler folds const field references to literal values
(`BoundAstToAstBase.cs:840`). By Stage 2 (JS emission), `AppShellCss.PaneLeft`
is just the string `"pane-left"` — the field symbol and `[CssClass]` attribute are gone.

### Const Concatenation Folding (confirmed by fleet analysis)
Roslyn folds `const + " " + const` into a SINGLE string literal at compile time.
Both GPT-5.4 and Opus confirmed this independently.

```csharp
// C# code:
AppShellCss.PaneLeft + " " + AppShellCss.Collapsed

// What the JST actually sees (SINGLE folded literal):
"pane-left collapsed"

// NOT three separate nodes — Roslyn collapses the entire expression
```

This means the old assumption that "concatenation preserves individual literals"
was **WRONG**. The replacement strategy must handle folded compound strings.

## Solution: Space-Aware Literal Matching (XWML Proven Pattern)

This follows exactly the same pattern as XWML's `CssNameValue.cs:24`:

```
For each StringLiteralExpression in the JST:
  1. Split value on spaces → ["pane-left", "collapsed"]
  2. Try to resolve EACH token via TryGetCssClassIdentifier()
  3. If ALL tokens resolve → replace with IdentifierStringExpression
  4. If ANY token fails → leave the literal unchanged (not a CSS string)
```

### Replacement Examples

| C# Expression | JST After Const Folding | After Space-Aware Replacement |
|---|---|---|
| `AppShellCss.PaneLeft` | `"pane-left"` | `IdentifierExpression(id_a)` → `"a"` |
| `PaneLeft + " " + Collapsed` | `"pane-left collapsed"` | `IdentifierStringExpression([id_a, " ", id_b])` → `"a b"` |
| `$"{PaneLeft} {Collapsed}"` | `"pane-left collapsed"` | same as above |
| `"hello world"` | `"hello world"` | unchanged ("hello" not a CSS class) |
| `"collapsed"` (in CSS context) | `"collapsed"` | `IdentifierExpression(id_c)` → `"c"` |

### Why Global Replacement is Safe

The replacement is **global** (all StringLiteralExpression nodes) but safe because:
- ALL space-separated tokens must be registered CSS classes for replacement to trigger
- CSS class names like `pane-left`, `todo-item`, `btn-check` are domain-specific
- Generic words like "collapsed" could match, but only if explicitly registered via `[CssClass]`
- If a collision occurs, the developer controls the CSS class names and can rename

### Minification Identity Guarantee

All three outputs resolve through the SAME `IIdentifier` from the SAME `_cssScope`:

```
CSS:  .pane-left{...}  → .a{...}     via GetSerializedCss() → IIdentifier.GetName()
HTML: class="pane-left" → class="a"   via ReplaceCssClassNamesInHtml() → IIdentifier.GetName()
JS:   "pane-left"       → "a"         via IdentifierStringExpression → IIdentifier.GetName()
```

Same IIdentifier object → guaranteed consistent minification.

## Compound CSS Names (Hyphens)

CSS class names use hyphens (`pane-left`) which are illegal in C# identifiers.
The const VALUE carries the actual CSS name. The C# identifier is for readability:

| CSS class name | C# const name | Const value |
|---|---|---|
| `pane-left` | `PaneLeft` | `"pane-left"` |
| `btn-toggle-left` | `BtnToggleLeft` | `"btn-toggle-left"` |
| `todo-item` | `TodoItem` | `"todo-item"` |
| `selected` | `Selected` | `"selected"` |

The `[CssClass("resource:className")]` attribute provides the formal connection.
The const VALUE provides the matching string for JST replacement.

## Architecture

### Pipeline Flow

```
┌──────────────────────────────────────────────────────┐
│ Framework: CssClassAttribute definition              │
│ (Sunlight.Framework.UI/Attributes/CssClassAttribute) │
└──────────────┬───────────────────────────────────────┘
               │ [CssClass] on const string fields
               ▼
┌──────────────────────────────────────────────────────┐
│ Stage 1: Roslyn compilation                          │
│ - Const values embedded in DLL metadata              │
│ - Const REFERENCES folded to literals in bound tree  │
│ - [CssClass] attributes preserved on FIELD DEFS      │
└──────────────┬───────────────────────────────────────┘
               │ DLL with: field defs + [CssClass] attrs + embedded AST
               ▼
┌──────────────────────────────────────────────────────┐
│ Stage 2: RazorTemplatingPlugin.Initialize()          │
│ 1. Load CSS from @styles → RazorCssManager per file  │
│ 2. Scan assembly for [CssClass] const fields (Cecil) │
│ 3. Parse attribute → (resourceName, className)       │
│ 4. Validate: className exists in CSS file            │
│ 5. Validate: field is const string                   │
│ 6. Register: constValue → IIdentifier (from CSS)     │
│ 7. Call CompressNames() on each RazorCssManager      │
└──────────────┬───────────────────────────────────────┘
               │ map: { "pane-left" → IIdentifier }
               ▼
┌──────────────────────────────────────────────────────┐
│ JST Generation (normal pipeline)                     │
│ - Const refs become StringLiteralExpression nodes    │
│ - "pane-left collapsed" is a single literal          │
└──────────────┬───────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────┐
│ JST Post-Processing: CssLiteralReplacementVisitor    │
│ For each StringLiteralExpression:                    │
│   1. Split value on spaces                           │
│   2. Resolve each token → IIdentifier                │
│   3. ALL match → IdentifierStringExpression          │
│   4. ANY miss → leave unchanged                      │
└──────────────┬───────────────────────────────────────┘
               │
               ▼
┌──────────────────────────────────────────────────────┐
│ JS Emission                                          │
│ IdentifierStringExpression.GetName() → minified      │
│ "pane-left collapsed" → "a b"                        │
└──────────────────────────────────────────────────────┘
```

### Key: IdentifierStringExpression (JST)

`IdentifierStringExpression` (from `NScript.JST/Expressions/`) wraps a list of
`IIdentifier` nodes and space separators. When serialized, it calls
`id.GetName()` on each — producing the minified name. This is the same
node type that XWML's `CssNameValue` emits.

For a single-token class (`"pane-left"` → 1 identifier), we can use either
`IdentifierExpression` or a single-element `IdentifierStringExpression`.

For a compound class (`"pane-left collapsed"` → 2 identifiers), we MUST use
`IdentifierStringExpression` with the space separator between them.

### New Files
- `Sources/Framework/Sunlight.Framework.UI/Attributes/CssClassAttribute.cs`
- `Sources/Compiler/RazorSkinParser/CssLiteralReplacementVisitor.cs` (JST visitor)

### Modified Files
- `Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs`
  - Add `ScanCssClassAttributes()` method
  - Add `_cssClassMap: Dictionary<string, IIdentifier>` (class name → CSS identifier)
  - Call `CompressNames()` after scanning
  - Apply `CssLiteralReplacementVisitor` to JST output
- `Sources/Compiler/RazorSkinParser/RazorCssManager.cs`
  - May need to expose `TryGetCssClassIdentifier()` publicly (verify accessibility)
- `Test/Framework/TodoApp/ViewModels/CssClasses.cs` (new — TodoApp CSS consts)
- `Test/Framework/TodoApp/ViewModels/AppViewModel.cs` (use CSS consts)
- `Test/Framework/Sunlight.Framework.UI.Test/RazorSkinTemplateTests.cs` (QUnit tests)

## Implementation Tasks

### Task 1: Define CssClassAttribute
- Create `CssClassAttribute.cs` in `Sunlight.Framework.UI/Attributes/`
- `[AttributeUsage(AttributeTargets.Field)]`
- Constructor: `CssClassAttribute(string cssClassReference)`
- Property: `string CssClassReference { get; }`
- Must be `const`-only enforcement (validated at scan time, not by attribute itself)

### Task 2: Scan and Validate [CssClass] Fields
- In `RazorTemplatingPlugin.Initialize()`, after loading CSS:
  - Walk all types in assembly via Mono.Cecil
  - Find const string fields with `[CssClass]` attribute
  - Validate: field must be `const string` (else compile error with clear message)
  - Parse attribute arg: split on `:` → (resourceName, className)
  - Validate: resource must match a loaded CSS file
  - Validate: className must exist in that CSS file's RazorCssManager
  - Validate: field's const value must equal className
  - Register: value → IIdentifier from `TryGetCssClassIdentifier()`

### Task 3: Enable CompressNames()
- Call `cssManager.CompressNames()` for each RazorCssManager that has [CssClass] registrations
- This assigns short minified names to all CSS identifiers

### Task 4: CssLiteralReplacementVisitor (JST Post-Processing)
- Create JST visitor that walks the tree
- For each `StringLiteralExpression`:
  - Split value on spaces
  - Try to resolve each token against `_cssClassMap`
  - If ALL tokens resolve:
    - Single token → replace with `IdentifierExpression`
    - Multiple tokens → replace with `IdentifierStringExpression` (with space separators)
  - If any token fails → leave unchanged
- Apply visitor to all generated JST before emission

### Task 5: TodoApp Demo
- Create `CssClasses.cs` with [CssClass] consts for all classes used in AppShell.css
- Update AppViewModel/TodoItemViewModel to use consts for class names
- Verify minified output works end-to-end

### Task 6: QUnit Tests
- Test: single class const resolves to minified name in rendered DOM
- Test: compound class (const concatenation → folded literal) resolves correctly
- Test: reactive binding update produces minified class names
- Test: CSS rules use matching minified names
- Test: non-CSS strings left unchanged

### Task 7: Validation Error Tests
- Test: [CssClass] on non-const field → compile error
- Test: [CssClass] with non-existent CSS class → compile error
- Test: [CssClass] with wrong resource name → compile error

## Dependencies
```
Task 1 → Task 2 → Task 3 → Task 4 → Task 5 + Task 6 + Task 7
```

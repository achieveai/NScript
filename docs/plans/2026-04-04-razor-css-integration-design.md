# Design: Wire RazorCssManager into Razor Compile Pipeline

**Date:** 2026-04-04
**Status:** Approved (autonomous — user delegated decisions)

## Context

The `@styles` directive and `RazorCssManager` were built in a prior session but never wired into the actual compile pipeline. Templates compile but CSS class names aren't validated, minified, or emitted. The XWML template system has full CSS support — the Razor side needs to match it.

## Design

### Architecture

```
@styles "file.css"  →  RazorSkinPreprocessor  →  IR.StylesheetResourceNames
                                                        ↓
Embedded .css resources  →  RazorTemplatingPlugin.Initialize()
                                                        ↓
                           RazorCssManager.AddStylesheet(name, cssText)
                                                        ↓
                           TemplateIRBuilder.ValidateCssClasses(ir, cssManager)
                                                        ↓
                           RazorCssManager.CompressNames()
                                                        ↓
RazorTemplatingPlugin.GetPostJavascript()  →  RazorSkinJSTGenerator(ir, ..., cssManager)
                                                        ↓
                           cssManager.ReplaceCssClassNames(htmlContent)  →  minified HTML
                                                        ↓
                           ConverterContext.AddCssContribution(serializedCss)
                                                        ↓
XWML CodeGenerator.GetAllCss()  →  appends Razor contributions  →  <style> tag in DOM
```

### Changes Required

#### W1. `RazorTemplatingPlugin.Initialize()` — CSS Loading

After each template IR is compiled (line ~162), check `ir.StylesheetResourceNames`:
- For each resource name, scan `module.Resources` for matching embedded `.css` resource
- Read CSS text, call `cssManager.AddStylesheet(resourceName, cssText)`
- Call `cssManager.ValidateCssVariables()` after all sheets loaded
- Store `cssManager` in a dictionary: `_templateCssManagers[templateName] = cssManager`
- Call `TemplateIRBuilder.ValidateCssClasses(ir, cssManager)` for compile-time validation
- Call `cssManager.CompressNames()` after validation

#### W2. `RazorSkinJSTGenerator` — CSS Class Replacement

Add `RazorCssManager cssManager = null` parameter to constructor. Before the innerHTML string literal is created (line ~272), if `cssManager != null`:
- Use regex to find `class="..."` attributes in `htmlContent`
- For each class name, call `cssManager.ReplaceCssClassNames(classValue)`
- Replace in the HTML string

#### W3. `RazorTemplatingPlugin.GetPostJavascript()` — CSS Contribution

When creating `RazorSkinJSTGenerator` (line ~930), pass the stored `_templateCssManagers[templateName]`.

After all templates are generated, collect and contribute CSS:
```csharp
foreach (var cssManager in _templateCssManagers.Values)
{
    var css = cssManager.GetSerializedCss();
    _runtimeScopeManager.Context.ConverterContext.AddCssContribution(css);
}
```

XWML's `GetAllCss()` already reads these contributions (CodeGenerator.cs:1444-1450).

### TodoApp Demo

- Extract TodoApp CSS classes used in `AppShell.skin.cshtml` into `TodoApp/RazorTemplates/AppShell.css`
- Add as `EmbeddedResource` in `TodoApp.csproj`
- Add `@styles "TodoApp.RazorTemplates.AppShell.css"` to template
- Classes flow through pipeline → validated → minified → emitted into `<style>` tag

### QUnit Browser Tests

Using existing `RazorStyledTemplate` and `RazorMultiStyled` templates (already registered):

1. **TestRazorStyledTemplate_Renders** — template compiles, renders DOM with @styles
2. **TestRazorStyledTemplate_HasCssClass** — h1 has CSS class (original or minified)
3. **TestRazorStyledTemplate_CssApplied** — `<style>` element in DOM contains CSS rules
4. **TestRazorMultiStyled_Renders** — multi-stylesheet template renders correctly
5. **TestRazorMultiStyled_BothSheetsApplied** — CSS from both sheets present in `<style>`
6. **TestRazorStyledTemplate_ReactiveWithCss** — reactive binding works with styled template

## Risks & Mitigations

| Risk | Mitigation |
|------|------------|
| CSS resource names don't match embedded resource names | Use same pattern as template matching: full resource name matching |
| Minified names break E2E tests (classes referenced by name) | TodoApp.htm still has its own `<style>` block; only extracted classes go through pipeline |
| No XWML plugin active → no `<style>` tag emitted | Only the TodoApp uses both plugins; framework tests also register both via PluginConfig.xml |
| Class replacement regex misses edge cases | Use same approach as `RazorCssManager.ReplaceCssClassNames()` — split on spaces, replace known classes |

## Decision Log

- **Approach chosen:** Wire existing pieces (not rebuild) — all components exist, just need connecting
- **HTML replacement location:** In JSTGenerator on final htmlContent string (simpler than modifying CodeGenerator)
- **CSS contribution scope:** Per-template CssManager, contributed once per template with stylesheets
- **TodoApp CSS extraction:** Extract only classes referenced in the skin template, keep rest in TodoApp.htm

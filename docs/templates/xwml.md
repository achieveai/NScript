# XWML templates (legacy)

> **Audience:** *App authors* maintaining XWML templates, or porting to Razor.

## TL;DR

XWML is NScript's **original XML-based template language** ([ADR 0016](../adr/0016-standardize-xwml-as-the-canonical-template-language-with-strict-css-and-binding-diagnostics.md)) — a `.html` file declares a `<skin>` element with a `ControlType` and `DataContextType`, and binds properties on the data context to attributes via `{PropertyName}` syntax (with optional `, Mode=OneTime|OneWay|TwoWay`). It is parsed by `XwmlParser` into the same `SkinInstance` runtime substrate that [Razor](razor.md) emits, so XWML and Razor controls coexist freely. Razor is the recommended path for *new* templates ([ADR 0017](../adr/0017-add-razor-skin-templates-as-a-second-template-frontend.md)); this page documents XWML for maintenance.

## Quick start

```xml
<!DOCTYPE html>
<html lang="en" xmlns="http://www.w3.org/1999/xhtml"
      xmlns:vm="MyApp.ViewModels!MyApp"
      xmlns:ctrl="Sunlight.Framework.UI!Sunlight.Framework.UI">
    <head><meta charset="utf-8" /><title /></head>
    <body>
        <skin ControlType="ctrl:UISkinableElement"
              DataContextType="vm:TodoItemViewModel">
            <div class="{CssClass}" onclick="{ToggleComplete}">
                <span class="todo-title">{Title}</span>
            </div>
        </skin>
    </body>
</html>
```

The XML namespace `vm:` is declared with the syntax `xmlns:vm="Namespace!AssemblyName"` — left of the bang is the .NET namespace, right is the assembly name. This is XWML-specific (it's how `XwmlParser` resolves type references at compile time).

## Reference — top-level structure

| Element / attribute | Purpose |
|---|---|
| `<html xmlns:prefix="...">` | Standard XHTML root with extra prefix declarations for type references |
| `<skin ControlType="..." DataContextType="...">` | The template root. `ControlType` is the host `UIElement` subclass; `DataContextType` is the type whose properties bind. |
| `<elementName attr="{Property}">` | Any inner element binds attributes via `{...}` syntax. |
| `<vm:CustomControl Prop="{Foo}">…</vm:CustomControl>` | A nested custom control — must be discoverable through the namespace declaration. |
| `id="Part1"` | Names a placeholder; the host control receives a reference via `[SkinPart]` / `[TemplatePart]`. |

## Reference — binding syntax

```text
{PropertyName}
{PropertyName, Mode=OneTime}
{PropertyName, Mode=OneWay}
{PropertyName, Mode=TwoWay}
```

| Mode | Behavior | Default for |
|---|---|---|
| `OneTime` | Read once at materialisation. No subscription. | non-observable properties |
| `OneWay` | Subscribes to source `INotifyPropertyChanged`; writes target on every change. | `ObservableObject` properties |
| `TwoWay` | Source → target plus target → source write-back path. | form input attributes (e.g. `value`, `checked`) on observable properties |

If you omit `Mode=`, XwmlParser picks the default based on the source-property analysis (analogous to Razor's auto-detection, but with explicit override available).

`IsStrict` on `[DefaultDataBinding]` rejects mismatched modes at compile time. See [Sunlight UI](../framework/sunlight-ui.md#reference--sunlightframeworkui-attributes).

## Reference — strict CSS

XWML enforces the same strict-class semantics as Razor: every `class="foo"` token is checked against the parsed declared CSS files. The diagnostic shape mirrors [templates/razor.md](razor.md#reference--strict-css). `CssParser` lives at `Sources/Compiler/CssParser/`; `Autoprefixer` runs the parsed CSS through vendor-prefixing.

## Examples

### One-way property binding

```xml
<skin ControlType="ctrl:UISkinableElement" DataContextType="vm:CountModel">
    <div>
        Count: <span>{Count}</span>
    </div>
</skin>
```

If `CountModel.Count` is `[AutoFire]` (or the type is an `ObservableObject` and the setter calls `FirePropertyChanged`), the span text updates reactively.

### Two-way input binding

```xml
<input type="text" value="{Name, Mode=TwoWay}" />
```

Target → source write-back is wired via `change` / `input` events depending on the input element type.

### Custom control with bound part

```xml
<skin ControlType="vm:TestSkinableWithTestUIElementPart"
      DataContextType="vm:TestViewModelB">
    <vm:TestUIElement id="Part1"
                      TwoWayLooseBinding="{PropInt1, Mode=TwoWay}"
                      OneWayStrictBinding="{PropStr1, Mode=OneWay}">
        This is a test.
    </vm:TestUIElement>
</skin>
```

`id="Part1"` makes this child control accessible to the host via `[SkinPart]` / `[TemplatePart]` (see [Sunlight UI attributes](../framework/sunlight-ui.md#reference--sunlightframeworkui-attributes)).

## Coexistence with Razor

XWML (`.html`) and Razor (`.skin.cshtml`) both produce the same `SkinInstance` runtime contract. A project can declare some controls in XWML and others in Razor without bridging code. The `SkinFactory` registry is namespace-flat — both sources register into the same lookup.

[ADR 0017](../adr/0017-add-razor-skin-templates-as-a-second-template-frontend.md) is explicit: Razor is the second frontend; both are supported. New templates should prefer Razor for:

- Auto-detected binding modes (no `Mode=` clutter).
- Compile-time DAG dedup ([ADR 0018](../adr/0018-replace-independent-binders-with-compile-time-reactive-binding-graph.md)).
- Full C# expressions (`@(cond ? a : b)`) instead of attribute-only binding syntax.

XWML retains some advantages:

- More familiar to teams with WPF / Silverlight / XAML background.
- `id="..."`-based part wiring is sometimes more ergonomic than name-based child-control resolution.

## Known gotchas

### Namespace declaration syntax

`xmlns:vm="MyApp.ViewModels!MyApp"` — the bang is XWML-specific. A standard XML namespace URI (`xmlns:vm="urn:my-app"`) will not resolve types and the parser will emit "type not found" errors.

### `{PropertyName}` is path-flat

Unlike Razor's `@Model.Foo.Bar`, XWML binding paths are typically a single property name. Drill down by exposing a flattened property on the data context.

### `Mode=` is per-binding, not per-attribute

Each `{...}` carries its own mode. Mixing modes inside the same attribute (e.g. concatenating two bindings into a single class string) is not supported — use a viewmodel property that returns the composed result.

### The XWML XML root must be valid XHTML

`XwmlParser` uses an XML reader, not an HTML5 parser. Self-closing tags must be explicit (`<br />`, not `<br>`). Unescaped `&` characters are errors.

### CSS class diagnostics fire even on dynamic values

`class="{CssClass}"` is allowed and the parser cannot inspect the runtime string. But static class names within attributes must be declared in a referenced CSS file. Dynamic classes computed in the viewmodel are not validated.

## Diagnostics

| Symptom | Cause |
|---|---|
| "Type 'X' not found" | Wrong namespace declaration (`xmlns:prefix="..."`) — check the `Namespace!Assembly` shape |
| Binding never updates | Source property is not observable; `Mode=` defaulted to `OneTime` |
| `[DefaultDataBinding(IsStrict=true)]` mismatch | The template's explicit `Mode=` differs from the property's strict default |
| `CSS class 'foo' used in template not found` | Class missing from any declared CSS |

## Cross-links

- [ADR 0016 — XWML strict CSS / binding diagnostics](../adr/0016-standardize-xwml-as-the-canonical-template-language-with-strict-css-and-binding-diagnostics.md)
- [ADR 0017 — Razor as second frontend](../adr/0017-add-razor-skin-templates-as-a-second-template-frontend.md)
- [Razor templates](razor.md) — recommended for new templates
- [Sunlight UI](../framework/sunlight-ui.md) — `Skin`, `SkinInstance`, attributes

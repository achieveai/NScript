# Razor Pipeline Review Fixes - Research Notes

## Key XWML Patterns from TestTextBinding1.js

1. **Type name mangling**: `Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory` (dots/namespace separators become `__`)
2. **Skin_factory call**: `Sunlight__Framework__UI__Skin_factory(ControlType, ModelType, factory, "0")` - uses underscores for namespace separator in Skin_factory args (single `_`)
3. **tmplStore**: global `tmplStore = new Array(1);` - declared AFTER the functions (at end)
4. **SkinBinderInfo_factory**: fully qualified `Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], ["PropName"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 17, 0, 0, null, "")`
5. **GetElementFromPath**: `Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1])`
6. **SkinInstance_factory**: fully qualified
7. **partMap**: `{"Part1": 0}` dictionary mapping id to element index

## Fix Plan

### CRITICAL (C1-C4)
- C1: Simplify regex in ExpressionJsEmitter.cs
- C2: Use Lazy<> for thread-safe cache in RoslynAnalysisPhase.cs
- C3: Move regex to static fields in TemplateIRBuilder.cs
- C4: Add HasConstructorArguments guard in RazorTemplatingPlugin.cs

### HIGH (H1-H8)
- H1: Pass framework stubs to Compile() in plugin Initialize()
- H2: Emit JS-mangled binder names instead of ConditionalBinder_setup/CollectionBinder_setup
- H3: Use element path indices for event wiring instead of data-event markers
- H4: Mangle type names with double-underscore for JS
- H5: Add Control. prefix property resolution
- H6: Fix attribute regex alternation
- H8: Build partIdMapping from elements with id attributes

### MEDIUM (M1-M10)
- M1: Prefix tmplStore with template name
- M2: Check function names before applying property-getter regex
- M3: Recurse into nested branches
- M5: Extract generic CollectNodes<T>
- M6: Extract ClassifySource helper
- M9: Add XML comment about Roslyn version
- M10: Add [Ignore] to SnapshotGenerator

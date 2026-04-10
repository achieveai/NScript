# Design: Sub-Control Runtime Instantiation in @foreach Loops

**Date:** 2026-04-06  
**Status:** Approved  
**Supersedes:** `2026-04-06-subcontrol-template-inlining-design.md` (compile-time approach rejected)

## Problem

AppShell.skin.cshtml has duplicated todo item templates across two @foreach loops. The compiler parses `<TodoItemControl />` tags into SubControlNode IR but code generation is stubbed out.

MCQdbDev shows controls are REAL components with behavior (AudioControl, RaisedButtonControl, TagSelectorControl). TodoApp should demonstrate this pattern with runtime instantiation.

## Approach: Runtime Sub-Control Instantiation

When `<TodoItemControl />` appears in a @foreach loop, the compiler emits sub-control factory info in CollectionTargetInfo. At runtime, GraphEngine.RenderCollectionItems creates real control instances — each with its own Skin, binding graph, and lifecycle.

## Architecture

### Compile-Time

```
TemplateIRBuilder:  <TodoItemControl/> → SubControlNode (existing, unchanged)

GraphTopologyBuilder.ProcessLoop:
  SubControlNode in ItemTemplate → SubControlTopology { TypeName, MarkerIdx }

GraphDescriptorJSTEmitter.EmitCollectionTargetInfo:
  SubControlInfos[]: { MarkerIdx, TypeFactory, SkinFactory }

RazorSkinCodeGenerator.CollectItemTemplateHtml:
  SubControlNode → <div data-ns-subctl="0"></div> marker
```

### Runtime

```
GraphEngine.RenderCollectionItems:
  for each item:
    1. Clone item HTML + create child graph (existing)
    2. For each SubControlInfo:
       - Find marker via data-ns-subctl
       - ctl = TypeFactory(marker)
       - ctl.Skin = SkinFactory()
       - ctl.DataContext = item
       - ctl.Activate()
```

## Files to Modify

**Compiler (3):**
1. `GraphTopologyBuilder.cs` — ProcessSubControl, SubControlTopology class
2. `GraphDescriptorJSTEmitter.cs` — Emit SubControlInfos in CollectionTargetInfo  
3. `RazorSkinCodeGenerator.cs` — SubControlNode → `<div data-ns-subctl>` marker

**Framework (2):**
4. `GraphDescriptor.cs` — SubControlInfo class, field on CollectionTargetInfo
5. `GraphEngine.cs` — Instantiate controls in RenderCollectionItems

**App (1):**
6. `AppShell.skin.cshtml` — Replace foreach bodies with `<TodoItemControl />`

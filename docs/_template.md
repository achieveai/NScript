# Page Title

> **Audience:** *App authors* or *Contributors* — pick one.

## TL;DR

3–5 lines. What the reader will be able to do or understand after reading this page.

## Quick start

A small, complete, runnable code snippet. No `// ...` ellipses unless they hide irrelevant boilerplate.

```csharp
// Concrete example here.
```

## Reference

The API surface, attribute table, or pipeline diagram for the topic. Tables preferred over prose lists for member-by-member references.

## Examples

2–4 canonical patterns the reader will reach for.

## Known gotchas & limitations

The sharp edges. A docs page that omits these is a docs page that lies.

## Diagnostics

Compiler / runtime messages the reader will see, with the cause of each.

## Cross-links

- Related docs page: `path/to/page.md`
- ADR: `../adr/NNNN-decision.md`

---

## Authoring conventions

- One H1 (`#`) per page; H2/H3 only below that. No deeper nesting.
- Cross-link to ADRs in `docs/adr/`; never re-derive a decision recorded there.
- Every code example must compile under NScript's supported subset (no `dynamic`, no `yield return`, no reflection, no P/Invoke).
- Diagrams: prefer Mermaid (renders on GitHub). Images only when Mermaid can't express the shape.
- Attribute reference tables use a uniform shape: **Name | Target | Effect | Example | ADR**.
- Logging / property-bag examples use `string[]` flat key/value arrays — NScript minifies field names, so anonymous-object property names break in the emitted JS (see `framework-logging.md`).
- Filenames are kebab-case.

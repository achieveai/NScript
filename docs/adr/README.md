# Architecture Decision Records

This directory stores Architecture Decision Records (ADRs) for NScript.

An ADR captures a single important technical decision, the context behind it, the options considered, and the consequences of the choice.

## Naming

Use the format `NNNN-short-title.md`.

Examples:

- `0001-adopt-architecture-decision-records.md`
- `0002-standardize-compiler-entry-point.md`

Number ADRs sequentially so they remain easy to reference in discussions and pull requests.

## Status

Use one of these statuses near the top of each ADR:

- `Proposed`
- `Accepted`
- `Superseded`
- `Deprecated`

If an ADR replaces an older one, mention the earlier ADR in the new record and update the older record to note that it has been superseded.

## Process

1. Copy [adr-template.md](./adr-template.md) to a new numbered file.
2. Fill in the context, options, decision, and consequences.
3. Open a pull request and review the ADR alongside the code change it affects.
4. Mark the ADR as `Accepted` when the decision is approved.

## Scope

Create an ADR when a decision is likely to matter beyond a single small code change, especially for:

- compiler architecture
- framework boundaries
- build and packaging strategy
- external dependencies
- testing strategy
- public extension points

Avoid ADRs for routine refactors or localized implementation details that do not carry broader architectural impact.
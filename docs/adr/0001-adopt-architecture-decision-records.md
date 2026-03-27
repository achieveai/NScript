# ADR 0001: Adopt Architecture Decision Records

- Status: Accepted
- Date: 2026-03-26
- Deciders: NScript maintainers
- Technical Area: Engineering process and architecture governance

## Context

NScript has compiler, framework, tooling, packaging, and test infrastructure spread across multiple projects. Important technical decisions can affect several parts of the repository over time, but the reasoning behind those decisions is easy to lose when it only lives in pull request discussions or commit history.

The repository now has a dedicated [docs/adr](./README.md) location for Architecture Decision Records. A clear convention is needed so future architectural choices are documented consistently and remain discoverable.

## Decision Drivers

- preserve architectural context over time
- make major technical decisions easy to review and reference
- keep decision documentation lightweight and versioned with code

## Options Considered

### Option 1: Record decisions as ADRs in source control

Store numbered markdown records in the repository under `docs/adr`.

Pros:

- decisions are versioned alongside code
- rationale is easy to find in pull requests and history
- format is lightweight and simple to maintain

Cons:

- requires discipline to keep records current
- adds a small documentation step to architectural changes

### Option 2: Rely on pull requests and commit history only

Use code review discussions and git history as the source of truth.

Pros:

- no new documentation process
- no additional files to maintain

Cons:

- decision rationale is fragmented and harder to discover later
- superseded decisions are difficult to track clearly

### Option 3: Maintain architecture decisions outside the repository

Use an external wiki or document store for major decisions.

Pros:

- can support richer long-form documents
- may fit broader organizational documentation practices

Cons:

- documentation can drift away from code changes
- external systems are harder to keep aligned and review together with implementation

## Decision

NScript will use Architecture Decision Records stored in `docs/adr` to document important architectural and cross-cutting technical decisions.

Each ADR will use sequential numbering and the repository template in [docs/adr/adr-template.md](adr-template.md). ADRs should be created for decisions with meaningful impact on compiler architecture, framework boundaries, build and packaging strategy, dependency choices, testing strategy, or public extension points.

## Consequences

Positive:

- major technical decisions gain durable, searchable context
- maintainers can understand why prior choices were made before changing them
- architectural discussions become easier to review and reference over time

Negative:

- contributors must spend some effort writing ADRs for qualifying decisions
- stale ADR status or missing supersession links could reduce usefulness if not maintained

## References

- [docs/adr/README.md](README.md)
- [docs/adr/adr-template.md](adr-template.md)
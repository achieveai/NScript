---
title: A build shortcut is proven only on a consumer with every reference edge type, checked through every downstream stage
date: 2026-10-09
type: process
component: build service / watch sync (Sdk.targets, ServiceHost.Watch.cs)
tags: [build-service, watch-sync, equivalence, project-reference, stage-2, manual-testing]
applies_when:
  - A change lets a build skip work (sync, cache, copy, incremental reuse) and claims the output equals a full build
  - Proof rows compare csc command lines or DLL hashes only
skip_when: [The shortcut has no project references and no downstream stage]
source: branch build-service-slice0, defect D-S3-4 (fixed in 58e90cac), final-review F-001/F-002 and D-F001
status: applied
revalidate_when: The watch sync contract or the two-stage pipeline changes
---

## Problem
The `dotnet build` watch sync answered "current" and skipped the compile.
D-S3-4 survived three test rounds: the synced app with an NScript ProjectReference
produced different JS from a full build. Later, F-001, F-002 and D-F001 each let
the sync say "yes" while the daemon's DLL lacked a file or used other build
properties.

## What didn't work
- Equivalence rows on a project with no NScript project-to-project reference: the reference path was never exercised.
- Comparing only csc command lines and DLLs: stage 2 (cs2jsc JS) diverged unseen.
- Closure conditions that named the mechanism ("marker present") rather than the user outcome (same JS, same build result).
- Taking the "current" baseline after the compile: files added during the build were silently accepted.

## Solution
- Run the first slice as the end-to-end user walk: register, save, plain `dotnet build`, compare bin DLL and every emitted JS bundle with a full build. Re-run it per commit sha.
- Pick a consumer that has every edge type (NScript ProjectReference, skin/CSS resources, plain .cs).
- Add hostile-timing rows: a file added or a csproj edited between evaluation and compile, a `-p:` override, another build rewriting obj outputs.

## Why it works
A shortcut is a claim that two paths give identical outputs. Each untested edge
type or downstream consumer is a place where the paths can differ while the
checked outputs still match. Timing rows catch baselines taken at the wrong moment.

---
title: The pre-commit hook builds and tests the whole working tree, so commit only while no worker has unstaged edits or a build running
date: 2026-10-29
type: process
component: pre-commit hook / Agile team mode
tags: [pre-commit, hook, working-tree, red-first, shared-bin, agile-team]
applies_when:
  - A lead commits staged work while a worker edits other files in the same checkout
  - A worker writes red-first tests or runs builds while a commit is in progress
skip_when: [Only one agent works in the checkout, or workers use separate worktrees]
source: branch hmr-large-app-live-patch, aborted B1+B6 commit (2026-10-29), recommitted as 5d8efe55
status: applied
revalidate_when: The pre-commit hook starts building only the staged tree, or work moves to per-worker worktrees
---

## Problem
The lead staged B1+B6 and committed. The hook aborted with
`XwmlParser.Test ... FAILED (44 passed, 1 failed)`. The failing test was a
worker's unstaged red-first test for the next fix, not the staged change. Its
build also wrote the shared `Test/Compiler/bin` while the hook ran, so even the
hook's passing numbers were suspect.

## What didn't work
- Staging only the finished files: the hook builds and tests the working tree, not the index.
- `git stash` to hide the worker's edits: banned over another agent's uncommitted work.

## Solution
- Gate workers: "edit now, build after I say commit done".
- Commit only while every worker is idle with no unstaged red tests.
- Otherwise wait until the next change is green and commit both, in order, as separate commits.

## Why it works
The hook compiles what is on disk and runs every compiler test project from
one shared bin folder. Any concurrent edit or build is part of its run. With
no other writer active, the hook result is about the staged change alone.

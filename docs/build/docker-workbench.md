# Docker workbench

This repository now includes a repo-level `Dockerfile` for an **interactive Linux development
workbench** rather than an app-runtime image. It is intentionally the same kind of setup as the
LmDotnetTools workbench:

- build the image once
- bind-mount the repo into `/workspace`
- mount auth/config from the host at run time instead of baking it into the image
- isolate generated Linux artifacts from the Windows checkout
- run builds, tests, browser validation, and agent CLIs inside the container

For NScript specifically, the workbench also handles the repo's custom compiler/emitter flow, so an
LLM can do real development work in-container instead of only reading files.

## What this is for

Use this workbench when you want:

- a consistent Linux environment for NScript development
- Claude, Copilot, or Codex running against the mounted repo
- container-local `bin` / `obj` / `node_modules` outputs instead of mixing Linux artifacts into a
  Windows host checkout
- the repo's real build + browser-test path available from a single container

Use it as the default answer to: "How should an LLM work on NScript inside Docker?"

## Quick start

1. Build the image:

   ```powershell
   .\scripts\docker-workbench-build.ps1
   ```

2. Start an interactive shell:

   ```powershell
   .\scripts\docker-workbench-run.ps1 `
     -Mount @(
       "$HOME\.claude=/home/dev/.claude",
       "$HOME\.claude.json=/home/dev/.claude.json",
       "$HOME\.codex=/home/dev/.codex",
       "C:\path\to\copilot-config=/home/dev/.config/github-copilot"
     )
   ```

3. Inside the container, validate the mounted repo:

   ```powershell
   pwsh scripts/docker-workbench-verify.ps1
   ```

4. If you want Claude to work directly inside the container, either launch it from the shell:

   ```powershell
   claude
   ```

   or start it from the host in one shot:

   ```powershell
   .\scripts\docker-workbench-run.ps1 `
     -Mount @(
       "$HOME\.claude=/home/dev/.claude",
       "$HOME\.claude.json=/home/dev/.claude.json"
     ) `
     -ContainerCommand @("claude")
   ```

## How this aligns with LmDotnetTools

The overall model is intentionally the same as the sibling repo:

- **interactive development image**, not runtime image
- **pinned repo toolchain** inside the image
- **bind-mounted workspace** at `/workspace`
- **host-mounted auth/config**, never copied into the image
- **optional host Docker access**, kept opt-in
- **container-managed artifact isolation**
- **repo-specific verification script**

The difference is the repo-specific surface:

- NScript must support its custom `csc` / `nscript` pipeline in Linux
- NScript verification includes both solution builds and `Test\Framework\TestWebApplication`
  browser validation
- the workbench has to support XWML/CSS generation and the ClearScript/V8 dependency path

## What's in the image

The image is aligned to NScript's actual build and test surface:

- .NET SDK 10 (`global.json`)
- .NET 8 installed side-by-side for the repo's `net8.0` compiler toolchain
- Node.js 22 + npm
- PowerShell 7
- git
- Docker CLI
- Python 3 + `uv` / `uvx`
- Playwright Chromium system dependencies
- Playwright Chromium browser binaries
- npm-installed agent CLIs:
  - `@anthropic-ai/claude-code`
  - `@anthropic-ai/claude-agent-sdk`
  - `@github/copilot`
  - `@openai/codex`

Optional build flag:

```powershell
.\scripts\docker-workbench-build.ps1 -InstallOptionalCopilotSdk
```

## What is intentionally **not** in the image

The image is intentionally **not** preloaded with your auth or local operator state.

Do **not** bake any of these into the image:

- Claude auth/config
- Codex auth/config
- Copilot auth/config
- API keys
- local token caches
- Docker host credentials

Instead, mount them from the host at run time.

The repo's `.dockerignore` excludes `.mcp.json` and similar local state from the build context, so
those files are not copied into the built image. If they exist in your checkout, they are still
visible at run time through the bind-mounted `/workspace`.

## Build the image

PowerShell helper:

```powershell
.\scripts\docker-workbench-build.ps1
```

Raw Docker command:

```powershell
docker build -t nscript-workbench .
```

Useful options:

```powershell
.\scripts\docker-workbench-build.ps1 -Pull
.\scripts\docker-workbench-build.ps1 -NoCache
.\scripts\docker-workbench-build.ps1 -UserUid 1000 -UserGid 1000
.\scripts\docker-workbench-build.ps1 -InstallOptionalCopilotSdk
```

## Run an interactive workbench

Default behavior of `.\scripts\docker-workbench-run.ps1`:

- mounts the repo at `/workspace`
- uses `/workspace` as the container working directory
- launches `pwsh -NoLogo` by default
- keeps generated artifacts in Docker volumes instead of the host checkout
- runs the entrypoint that fixes isolated volume ownership, normalizes Linux shell wrappers, and then
  drops to the non-root `dev` user

Baseline run:

```powershell
.\scripts\docker-workbench-run.ps1
```

Recommended run with agent mounts:

```powershell
.\scripts\docker-workbench-run.ps1 `
  -Mount @(
    "$HOME\.claude=/home/dev/.claude",
    "$HOME\.claude.json=/home/dev/.claude.json",
    "$HOME\.codex=/home/dev/.codex",
    "C:\path\to\copilot-config=/home/dev/.config/github-copilot"
  )
```

Pass through environment variables only when you really need them:

```powershell
.\scripts\docker-workbench-run.ps1 `
  -EnvVar "ANTHROPIC_API_KEY=$env:ANTHROPIC_API_KEY" `
  -EnvVar "OPENAI_API_KEY=$env:OPENAI_API_KEY"
```

## Launching Claude, Copilot, or Codex inside the workbench

The image already contains the CLIs. What matters is mounting the right host config.

### Claude

Mount both the Claude directory and the top-level JSON file:

```powershell
.\scripts\docker-workbench-run.ps1 `
  -Mount @(
    "$HOME\.claude=/home/dev/.claude",
    "$HOME\.claude.json=/home/dev/.claude.json"
  )
```

Then either:

```powershell
claude
```

inside the shell, or launch it directly:

```powershell
.\scripts\docker-workbench-run.ps1 `
  -Mount @(
    "$HOME\.claude=/home/dev/.claude",
    "$HOME\.claude.json=/home/dev/.claude.json"
  ) `
  -ContainerCommand @("claude")
```

### Copilot

Mount your Copilot config directory to the path the CLI expects:

```powershell
.\scripts\docker-workbench-run.ps1 `
  -Mount @(
    "C:\path\to\copilot-config=/home/dev/.config/github-copilot"
  )
```

Then run:

```powershell
copilot
```

### Codex

Mount your Codex state directory:

```powershell
.\scripts\docker-workbench-run.ps1 `
  -Mount @(
    "$HOME\.codex=/home/dev/.codex"
  )
```

Then run:

```powershell
codex
```

## Running verification from the host

You do not have to enter the shell first. You can ask the host-side runner to execute the verify
script directly:

```powershell
.\scripts\docker-workbench-run.ps1 `
  -ContainerCommand @("pwsh", "-NoLogo", "-File", "scripts/docker-workbench-verify.ps1")
```

This is the easiest "prove the image + mounted repo are ready" command for both humans and LLMs.

## What the verify script actually proves

Run this inside the container:

```powershell
pwsh scripts/docker-workbench-verify.ps1
```

Default flow:

1. tool versions and core CLI availability
2. agent CLI checks (`claude`, `copilot`, `codex`, `uv`, `uvx`, `docker`)
3. `dotnet build NScript_Full.sln -c Debug`
4. `dotnet build NScript_Full.sln -c Release`
5. `npm ci` in `Test\Framework\TestWebApplication`
6. `npx playwright install chromium`
7. `npm test` in `Test\Framework\TestWebApplication`

That browser test phase covers:

- `Sunlight.Framework.Test`
- `Sunlight.Framework.UI.Test`
- `TodoApp.Test`
- `Sunlight.Framework.Data.Test`
- Todo app Playwright E2E validation

Useful narrower loops:

```powershell
pwsh scripts/docker-workbench-verify.ps1 -SkipReleaseBuild
pwsh scripts/docker-workbench-verify.ps1 -SkipBrowserValidation
```

## Artifact isolation and why it matters

The run helper isolates generated outputs into container-managed Docker volumes instead of writing
them back into the Windows checkout.

That includes:

- project `bin`
- project `obj`
- `NScriptToolSet/bin`
- `NScriptToolSet/lib`
- top-level repo-managed `node_modules`
- generated JS output directories such as `GeneratedScripts`

Why this is the default:

- Linux build outputs should not fight with Windows host outputs
- `npm ci` needs to be free to remove and recreate dependency trees
- NScript's custom compiler/emitter outputs are rebuilt inside the container and should stay
  container-local unless you intentionally want otherwise

If you explicitly want the bind-mounted repo to expose generated artifacts directly inside the
container, use:

```powershell
.\scripts\docker-workbench-run.ps1 -DisableGeneratedArtifactIsolation
```

Leave isolation enabled unless you have a concrete reason to inspect host-visible outputs.

## Optional host-Docker-access profile

Host Docker access stays opt-in because mounting the Docker socket effectively grants privileged
host control to processes in the container.

Use this only when you intentionally want the workbench to launch or control sibling containers.

```powershell
.\scripts\docker-workbench-run.ps1 `
  -EnableHostDockerAccess `
  -RunAsRoot `
  -Mount @(
    "$HOME\.claude=/home/dev/.claude",
    "$HOME\.claude.json=/home/dev/.claude.json"
  )
```

Notes:

- `-RunAsRoot` disables the normal privilege drop and leaves the requested command running as root
- if you do not need sibling-container workflows, do **not** enable host Docker access

## Recommended LLM workflow in this repo

If an LLM is asked to work on NScript using Docker, the intended workflow is:

1. build the workbench image
2. run the workbench with the repo mounted at `/workspace`
3. mount only the auth/config needed for the chosen CLI
4. run `pwsh scripts/docker-workbench-verify.ps1` before claiming the environment is ready
5. do development work from inside the container
6. rerun either the full verify script or the smallest repo command that proves the specific change

For NScript, "environment ready" means more than "the shell starts":

- the custom compiler/emitter chain works in Linux
- XWML/CSS generation works in Linux
- browser-test dependencies work in Linux
- the mounted repo can complete its passing build + browser validation path

## NScript-specific Linux notes

These repo-specific behaviors are now handled for you by the workbench-support changes:

- non-Windows `csc` / `nscript` shims are used in Linux
- clean framework/test builds can bootstrap the custom NScript toolset automatically
- the Linux ClearScript/V8 native dependency path is restored for XWML/CSS processing
- nested dependency directories under `node_modules` are not isolated as separate mount points, so
  `npm ci` can clean and recreate them normally

## Important scope note about the default gate

The full:

```powershell
dotnet test NScript_Full.sln -c Release --no-build
```

suite is **not** the default workbench gate, because the host baseline already contains unrelated
pre-existing red compiler tests.

The workbench verify script therefore answers a narrower and more useful question:

> "Can a human or LLM launch the NScript repo in a Linux container and complete the repo's passing
> build + browser validation workflow?"

Right now, the answer is **yes**, and `scripts/docker-workbench-verify.ps1` is the command that
proves it.

# Browser-Based Framework Testing

This document describes how NScript's framework tests run in a browser and how to execute them locally.

## Overview

Framework tests are written in C# using the `SunlightUnit` test framework, compiled to JavaScript by the NScript compiler pipeline, and executed in a browser via QUnit 2.2.0. Each compiled bundle is served from its **own** static page inside the `TestWebApplication` project — `Sunlight.Framework.TestPage.htm`, `Sunlight.Framework.UI.TestPage.htm`, `DataTestPage.htm`, and `TodoApp.TestPage.htm`. The legacy `TestPage.htm` is now a no-bundle index that links to those pages.

> **Why one bundle per page?** Each NScript-compiled bundle is its own IIFE
> minified independently and writes runtime metadata (type-check method,
> is-class flag, FQN, interface map, …) to slots on `Function.prototype`. The
> minifier picks DIFFERENT short letters per bundle, so co-loading two bundles
> on one page silently clobbers earlier metadata with the next bundle's
> letter mapping — producing wrong-looking `TypeError`s such as
> `b.m is not a function` or `InvalidCast`. See issue #51 for the full
> reproduction. Until a runtime-metadata fix lands, every test suite has its
> own page.

## Architecture

```
C# Test Source                NScript Compiler                   Browser
--------------                ----------------                   -------
[TestFixture]          dotnet build (Debug)              *.TestPage.htm loads:
[Test] methods    -->  Stage 1: Roslyn --> DLL      -->    QUnit 2.2.0
Assert.Equal()         Stage 2: DLL --> JavaScript         ONE generated .js file
                       + TestGenerator plugin                    |
                       (emits QUnit.module/test)                 v
                                                          QUnit discovers and
                                                          runs all tests
```

### Pipeline Detail

1. **C# compilation** -- MSBuild compiles the test `.csproj` using the NScript custom compiler (`CscToolPath` from `Test/Framework/Directory.Build.props` points to `NScriptToolSet/bin/Debug/`).
2. **JavaScript generation** -- The `AfterCompile` MSBuild target in `Sources/Compiler/NScript.Sdk/Sdk/Sdk.targets` invokes `nscript.exe` to convert the compiled DLL into JavaScript.
3. **TestGenerator plugin** -- Registered via `PluginConfig.xml` in each test project. Scans the assembly for `[TestFixture]` classes and `[Test]` methods, then emits `QUnit.module()` and `QUnit.test()` registration calls at the end of the generated JS file.
4. **Output** -- Generated `.js` and `.map` (source map) files land in `Test/Framework/TestWebApplication/GeneratedScripts/`.
5. **Execution** -- Each `*.TestPage.htm` includes QUnit and exactly one generated script. Open the page for the suite you want; QUnit runs its tests automatically. `TestPage.htm` is an index page listing the four suites — open it first if you don't know which page you want.

## Key Components

### SunlightUnit (`Sources/Framework/SunlightUnit/`)

Custom C# test framework that maps to QUnit at the JavaScript level:

| File | Purpose |
|------|---------|
| `Attributes.cs` | `[TestFixture]`, `[Test]`, `[TestSetup]`, `[TestCaseSetup]`, `[TestTearDown]`, `[TestCaseTearDown]` |
| `Assert.cs` | Assertion API -- `[ImportedType]` with `[ScriptName("QUnit.assert")]`. Methods use `[ScriptAlias]` to map to QUnit (e.g., `Equal` -> `equal`, `IsTrue` -> `ok`) |
| `TestSetup.cs` | `TestSetup.Module()` -> `QUnit.module()`, `TestSetup.Test()` -> `QUnit.test()`. `TestEnvironment` maps lifecycle hooks (`before`, `beforeEach`, `after`, `afterEach`) |
| `TestRunner.cs` | Async helpers: `WaitForAsyncCompletion()` -> `QUnit.stop()`, `ResumeOnAsyncCompleted()` -> `QUnit.start()` |

### TestGenerator Plugin (`Sources/Compiler/NScript.Converter.Plugins/TestGenerator.cs`)

Implements `IRuntimeConverterPlugin`. During compilation:

- **`GetMethodsToEmitPass1()`** -- Finds all `[TestFixture]` classes, returns their `[Test]`/`[TestSetup]`/`[TestTearDown]` methods for JS emission.
- **`GetPostJavascript()`** -- Emits QUnit registration code:
  ```javascript
  QUnit.module("Namespace.TestClassName", {
    before: setupFn,
    beforeEach: caseSetupFn,
    after: teardownFn,
    afterEach: caseTeardownFn
  });
  QUnit.test("TestMethodName", testMethodFunction);
  ```

### Test Projects

| Project | Location | What it tests |
|---------|----------|---------------|
| `Sunlight.Framework.Test` | `Test/Framework/Sunlight.Framework.Test/` | Core framework: IoC container, event bus, observable objects/collections, data binders |
| `Sunlight.Framework.UI.Test` | `Test/Framework/Sunlight.Framework.UI.Test/` | UI framework: template rendering, skin binding, live binders, list views, UI elements |

Both projects have:
- `<GenerateJs>True</GenerateJs>` -- enables JS generation
- `<JsOutputPath>../TestWebApplication/GeneratedScripts</JsOutputPath>` -- output destination
- `<PluginConfig>PluginConfig.xml</PluginConfig>` -- loads TestGenerator (and XwmlTemplatingPlugin for UI tests)

### TestWebApplication (`Test/Framework/TestWebApplication/`)

Static web host containing:
- `TestPage.htm` -- index page linking to the four per-suite pages (loads no bundles)
- `Sunlight.Framework.TestPage.htm` -- core framework suite (one bundle)
- `Sunlight.Framework.UI.TestPage.htm` -- UI framework suite (one bundle)
- `DataTestPage.htm` -- data-layer suite (one bundle)
- `TodoApp.TestPage.htm` -- TodoApp suite (one bundle)
- `Scripts/QUnit.2.2.0.js` -- QUnit framework
- `Styles/QUnit.2.2.0.css` -- QUnit styling
- `GeneratedScripts/` -- Build output directory for compiled JS test files

## How to Run

### Prerequisites

- .NET 8.0 SDK
- Node.js (for static server and Playwright)
- A Debug build of the compiler (framework tests use the Debug compiler)

### Step 1: Build (generates the JavaScript test files)

```bash
dotnet build NScript_Full.sln -c Debug
```

This compiles the C# test projects and triggers `nscript.exe` to generate:
- `Test/Framework/TestWebApplication/GeneratedScripts/Sunlight.Framework.Test.js`
- `Test/Framework/TestWebApplication/GeneratedScripts/Sunlight.Framework.UI.Test.js`

### Step 2: Serve the test page

Any static HTTP server works. The simplest options:

```bash
cd Test/Framework/TestWebApplication

# Option A: Node.js
npx serve .

# Option B: Python
python -m http.server 8080
```

### Step 3: Open in browser

Navigate to `http://localhost:<port>/TestPage.htm` and click through to the per-suite page you want (or jump directly to e.g. `Sunlight.Framework.TestPage.htm`). QUnit automatically discovers and runs all registered tests on the suite-specific page. Results appear inline with pass/fail counts and expandable details for each assertion. Run each suite as a separate page load — never paste multiple `<script>` tags onto one page.

### Step 4 (Optional): Automated with Playwright

For CI or headless execution:

```bash
cd Test/Framework/TestWebApplication
npm init -y
npm install playwright
npx playwright install chromium
```

Then run a script that:
1. Starts a local HTTP server
2. Launches Chromium via Playwright
3. Iterates each per-suite page (`Sunlight.Framework.TestPage.htm`, `Sunlight.Framework.UI.TestPage.htm`, `DataTestPage.htm`, `TodoApp.TestPage.htm`), opening a fresh page per suite
4. Waits for `#qunit-banner` to get class `qunit-pass` or `qunit-fail`
5. Extracts results from `#qunit-tests > li` elements and aggregates pass/fail counts

The repo already ships two such runners — `run-qunit.mjs` (synthesises a one-bundle page per suite) and `run-tests.js` (iterates the static `*.TestPage.htm` files). Use either; do not roll your own that loads multiple bundles on a single page.

```javascript
import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { chromium } from 'playwright';

// 1. Create static file server on auto-assigned port
// 2. browser = await chromium.launch({ headless: true });
// 3. for (const suite of ['Sunlight.Framework.TestPage.htm', ...]) {
//      const page = await browser.newPage();
//      await page.goto(`http://localhost:${port}/${suite}`);
//      // 4. Wait for #qunit-banner.qunit-pass | .qunit-fail
//      // 5. Extract results from #qunit-tests > li elements
//      await page.close();
//    }
```

## Writing New Framework Tests

### 1. Create a test class

```csharp
using SunlightUnit;

namespace YourNamespace
{
    [TestFixture]
    public class MyNewTests
    {
        [Test]
        public static void TestSomething(Assert assert)
        {
            var result = 2 + 2;
            assert.Equal(4, result, "2 + 2 should equal 4");
        }

        [TestCaseSetup]
        public static void BeforeEachTest(Assert assert)
        {
            // Runs before each [Test] method
        }
    }
}
```

### 2. Ensure the project has correct settings

In the `.csproj`:
```xml
<PropertyGroup>
  <GenerateJs>True</GenerateJs>
  <JsOutputPath>../TestWebApplication/GeneratedScripts</JsOutputPath>
  <PluginConfig>PluginConfig.xml</PluginConfig>
</PropertyGroup>
```

In `PluginConfig.xml`:
```xml
<Plugins>
  <Plugin Assembly="NScript.Converter.Plugins"
         ClassName="NScript.Converter.Plugins.TestGenerator" />
</Plugins>
```

### 3. Add a per-suite page that loads only your project's JS

Create a new `YourProject.TestPage.htm` next to the existing `*.TestPage.htm` pages, mirroring `Sunlight.Framework.TestPage.htm` (including the comment block that explains why bundles are not co-loaded). Then:

- add a single `<script src="GeneratedScripts/YourProject.js"></script>` line for *only* your project's bundle,
- add the new page to `TestWebApplication.csproj` under `<ItemGroup>` (`<Content Include="YourProject.TestPage.htm" />`),
- link it from `TestPage.htm` (the index),
- add it to the `SUITE_PAGES` array in `run-tests.js` and the `QUNIT_SUITES` array in `run-qunit.mjs`.

Do NOT add a `<script>` tag for your bundle to any existing page — co-loading two bundles on one page corrupts `Function.prototype` runtime metadata (issue #51).

### 4. Build and run

```bash
dotnet build NScript_Full.sln -c Debug
cd Test/Framework/TestWebApplication
npx serve .
# Open http://localhost:3000/TestPage.htm and click through to your suite,
# or jump directly to http://localhost:3000/YourProject.TestPage.htm
```

## Assert API Reference

| C# Method | QUnit Equivalent | Description |
|-----------|-----------------|-------------|
| `assert.IsTrue(value, msg)` | `ok()` | Value is truthy |
| `assert.Equal(actual, expected, msg)` | `equal()` | Loose equality (`==`) |
| `assert.StrictEqual(actual, expected, msg)` | `strictEqual()` | Strict equality (`===`) |
| `assert.NotEqual(actual, expected, msg)` | `notEqual()` | Not loosely equal |
| `assert.NotStrictEqual(actual, expected, msg)` | `notStrictEqual()` | Not strictly equal |
| `assert.DeepEqual(actual, expected, msg)` | `deepEqual()` | Deep/structural equality |
| `assert.NotDeepEqual(actual, expected, msg)` | `notDeepEqual()` | Not deeply equal |
| `assert.Throws(fn, msg)` | `throws()` | Function throws |
| `assert.Expect(count)` | `expect()` | Expected assertion count |
| `assert.Async()` | `async()` | Returns callback for async completion |
| `assert.VerifySteps(steps)` | `verifySteps()` | Step verification |

## Test Lifecycle Attributes

| Attribute | QUnit Hook | Scope |
|-----------|-----------|-------|
| `[TestSetup]` | `before` | Once before all tests in the module |
| `[TestCaseSetup]` | `beforeEach` | Before each test method |
| `[TestTearDown]` | `after` | Once after all tests in the module |
| `[TestCaseTearDown]` | `afterEach` | After each test method |

## Troubleshooting

- **"GeneratedScripts/ is empty"** -- You need to build in Debug mode first: `dotnet build NScript_Full.sln -c Debug`. Framework tests require the Debug compiler.
- **"Tests don't appear in QUnit"** -- Check that the `<script>` tag for your project's JS file is on the right per-suite `*.TestPage.htm`.
- **"QUnit shows 0 tests"** -- Verify `PluginConfig.xml` includes `TestGenerator`. Without it, `QUnit.module()`/`QUnit.test()` calls are not emitted.
- **"CORS errors in browser"** -- You must serve via HTTP, not open the page directly as a `file://` URL. Use any static HTTP server.
- **"`b.m is not a function` / wrong-looking `InvalidCast` errors"** -- Almost always a sign that two bundles got loaded on the same page (issue #51). Check that the page you opened only has ONE `<script>` tag for a `GeneratedScripts/*.js` file.

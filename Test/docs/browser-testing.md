# Browser-Based Framework Testing

This document describes how NScript's framework tests run in a browser and how to execute them locally.

## Overview

Framework tests are written in C# using the `SunlightUnit` test framework, compiled to JavaScript by the NScript compiler pipeline, and executed in a browser via QUnit 2.2.0. The generated JavaScript is served by a static web page (`TestPage.htm`) inside the `TestWebApplication` project.

## Architecture

```
C# Test Source                NScript Compiler                   Browser
--------------                ----------------                   -------
[TestFixture]          dotnet build (Debug)              TestPage.htm loads:
[Test] methods    -->  Stage 1: Roslyn --> DLL      -->    QUnit 2.2.0
Assert.Equal()         Stage 2: DLL --> JavaScript         Generated .js files
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
5. **Execution** -- `TestPage.htm` includes QUnit and the generated scripts. Opening it in a browser runs all tests automatically.

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
- `TestPage.htm` -- QUnit test runner HTML page
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

Navigate to `http://localhost:<port>/TestPage.htm`. QUnit automatically discovers and runs all registered tests. Results appear inline with pass/fail counts and expandable details for each assertion.

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
3. Navigates to `TestPage.htm`
4. Waits for `#qunit-banner` to get class `qunit-pass` or `qunit-fail`
5. Extracts results from `#qunit-tests > li` elements

See the example script pattern:
```javascript
import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { chromium } from 'playwright';

// 1. Create static file server on auto-assigned port
// 2. browser = await chromium.launch({ headless: true });
// 3. page.goto(`http://localhost:${port}/TestPage.htm`);
// 4. page.waitForFunction(() => {
//      const b = document.getElementById('qunit-banner');
//      return b && (b.className.includes('qunit-pass') || b.className.includes('qunit-fail'));
//    });
// 5. Extract results from #qunit-tests > li elements
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

### 3. Include the generated JS in TestPage.htm

Add a `<script>` tag in `TestPage.htm`:
```html
<script src="GeneratedScripts/YourProject.js" type="text/javascript"></script>
```

### 4. Build and run

```bash
dotnet build NScript_Full.sln -c Debug
cd Test/Framework/TestWebApplication
npx serve .
# Open http://localhost:3000/TestPage.htm
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
- **"Tests don't appear in QUnit"** -- Check that the `<script>` tag for your project's JS file is in `TestPage.htm`.
- **"QUnit shows 0 tests"** -- Verify `PluginConfig.xml` includes `TestGenerator`. Without it, `QUnit.module()`/`QUnit.test()` calls are not emitted.
- **"CORS errors in browser"** -- You must serve via HTTP, not open `TestPage.htm` directly as a `file://` URL. Use any static HTTP server.

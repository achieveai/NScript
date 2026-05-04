# Test CLAUDE.md

Guidance for working with the NScript test suite. See the root `CLAUDE.md` for the overall compiler pipeline.

## Two Categories of Tests

- **Compiler tests** (`Test/Compiler/`) -- Standard .NET unit tests using MSTest. Run with `dotnet test`. These verify the C#-to-JavaScript compilation pipeline itself (method conversion, CSS parsing, JS parsing, CLR analysis).
- **Framework tests** (`Test/Framework/`) -- C# tests compiled to JavaScript by the NScript compiler, then executed in a browser via QUnit 2.2.0. These verify the runtime framework behavior (observable objects, data binding, templates, UI elements).

## Browser-Based Framework Testing

Full architecture and how-to guide: `Test/docs/browser-testing.md`

### Quick Start

```bash
# 1. Build in Debug (framework tests require the Debug compiler)
dotnet build NScript_Full.sln -c Debug

# 2. Serve the test web app
cd Test/Framework/TestWebApplication
npx serve .

# 3. Open http://localhost:3000/TestPage.htm in any browser
#    (this is now an index page; pick a per-suite page from the list)
```

> **Each test suite runs on its own page.** Co-loading multiple NScript bundles
> on a single page corrupts `Function.prototype` runtime metadata because the
> minifier picks per-IIFE letters that collide on the shared prototype (see
> issue #51). `TestPage.htm` is now a link index; the actual suites are at
> `Sunlight.Framework.TestPage.htm`, `Sunlight.Framework.UI.TestPage.htm`,
> `DataTestPage.htm`, and `TodoApp.TestPage.htm`.

### How It Works

C# test code using `[TestFixture]` / `[Test]` attributes from the `SunlightUnit` framework gets compiled to JavaScript by the NScript compiler. The `TestGenerator` plugin (`Sources/Compiler/NScript.Converter.Plugins/TestGenerator.cs`) scans for these attributes and emits `QUnit.module()` / `QUnit.test()` registration calls in the generated JS. Each suite-specific QUnit page (e.g. `Sunlight.Framework.TestPage.htm`) loads QUnit plus exactly one generated bundle, and QUnit runs everything automatically.

### Pipeline

```
C# [TestFixture]/[Test]  -->  dotnet build (Debug)  -->  nscript.exe + TestGenerator
                                                              |
                                                              v
                                                    GeneratedScripts/*.js
                                                              |
                                                              v
                                                    Per-suite *.TestPage.htm + QUnit 2.2.0
                                                              |
                                                              v
                                                    Browser executes tests
```

### SunlightUnit Assert Maps to QUnit

The `SunlightUnit.Assert` class uses `[ScriptAlias]` attributes so C# assertion calls become QUnit calls in the generated JS:

- `assert.Equal(actual, expected, msg)` becomes `assert.equal()`
- `assert.IsTrue(value, msg)` becomes `assert.ok()`
- `assert.StrictEqual(actual, expected, msg)` becomes `assert.strictEqual()`
- `assert.DeepEqual(actual, expected, msg)` becomes `assert.deepEqual()`
- `assert.Throws(fn, msg)` becomes `assert.throws()`

### Writing a New Browser Test

1. Create a `[TestFixture]` class with `public static` `[Test]` methods that accept `Assert assert`
2. Project `.csproj` must have `<GenerateJs>True</GenerateJs>`, `<JsOutputPath>../TestWebApplication/GeneratedScripts</JsOutputPath>`, and `<PluginConfig>PluginConfig.xml</PluginConfig>`
3. `PluginConfig.xml` must register `NScript.Converter.Plugins.TestGenerator`
4. Add a **new** `*.TestPage.htm` that loads ONLY your project's generated JS (mirror `Sunlight.Framework.TestPage.htm`); do not co-load it with another bundle, and link it from `TestPage.htm`
5. Build Debug, serve, open in browser

### Automated Execution (Playwright)

For CI or headless runs, use Playwright to launch a browser, navigate to each per-suite page (`Sunlight.Framework.TestPage.htm`, `Sunlight.Framework.UI.TestPage.htm`, `DataTestPage.htm`, `TodoApp.TestPage.htm`), and wait for `#qunit-banner` to gain class `qunit-pass` or `qunit-fail`. Extract per-test results from `#qunit-tests > li` elements. `run-qunit.mjs` and `run-tests.js` both already iterate the per-suite pages — see `Test/docs/browser-testing.md` for the full pattern.

### Key Locations

- `Test/Framework/TestWebApplication/TestPage.htm` -- index page (links to per-suite pages; loads no bundles)
- `Test/Framework/TestWebApplication/Sunlight.Framework.TestPage.htm` -- core framework suite
- `Test/Framework/TestWebApplication/Sunlight.Framework.UI.TestPage.htm` -- UI framework suite
- `Test/Framework/TestWebApplication/DataTestPage.htm` -- data-layer suite
- `Test/Framework/TestWebApplication/TodoApp.TestPage.htm` -- TodoApp suite
- `Test/Framework/TestWebApplication/GeneratedScripts/` -- Compiled JS output
- `Test/Framework/TestWebApplication/Scripts/QUnit.2.2.0.js` -- QUnit framework
- `Test/Framework/Sunlight.Framework.Test/` -- Core framework tests (container, events, observables, binders)
- `Test/Framework/Sunlight.Framework.UI.Test/` -- UI tests (templates, skins, list views)
- `Sources/Framework/SunlightUnit/` -- Test framework (Assert, attributes, lifecycle)
- `Sources/Compiler/NScript.Converter.Plugins/TestGenerator.cs` -- QUnit code emitter plugin
- `Test/Framework/Directory.Build.props` -- Compiler toolset paths (points to Debug build)

## Compiler Tests

Standard MSTest projects under `Test/Compiler/`:

```bash
# Run all compiler tests
dotnet test NScript_Full.sln -c Release

# Run a single project
dotnet test Test/Compiler/CssParser.Test/CssParser.Test.csproj

# Run a specific test by name
dotnet test Test/Compiler/NScriptTest/NScriptTest.csproj --filter "FullyQualifiedName~TestMethodName"
```

### Compiler Test Projects

- `NScript.Converter.Test` -- C#-to-JS method conversion regression tests (data-driven, snapshot-based)
- `NScript.CLR.Test` -- CLR intermediate analysis
- `CssParser.Test` -- CSS parsing
- `NScript.JSParser.Test` -- JavaScript parser
- `NScript.Csc.Lib.Test` -- Custom Roslyn compiler integration
- `RazorSkinParser.Test` -- Razor template parsing

### TestLauncher (`TestLauncher/`)

Console app for debugging individual compiler tests outside the test runner. Instantiates `RegressionTests` and calls a specific test method directly. Useful when you need to attach a debugger to the compilation pipeline.

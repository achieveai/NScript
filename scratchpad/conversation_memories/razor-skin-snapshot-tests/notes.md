# Razor Skin Snapshot Tests - Research Notes

## Key Findings
- Razor skin compiler is a multi-phase pipeline: Preprocess -> Razor Parse -> IR Build -> Roslyn Analysis -> JS Generation
- The XWML test pattern uses EmbeddedResource for templates/expected output; the Razor tests use CopyToOutputDirectory
- All 93 tests pass (40 existing + 53 new)
- The @model directive type name leaks into HTML output for some templates (e.g., "TestVM\n\n<div>...")
  - This is existing compiler behavior, not a test bug
  - The IsModelDirectiveEcho filter doesn't catch all cases

## Test Breakdown
- 16 snapshot tests (DataRow-driven)
- 11 content-validating snapshot assertions
- 11 ExpressionJsEmitter unit tests
- 8 BinderEmitter unit tests
- 10 enhanced TemplateIRBuilder content tests
- 1 snapshot generator utility

## Files Created
- RazorSkinTestHelper.cs - test helper with compile/check methods
- RazorSkinSnapshotTests.cs - main snapshot test class
- ExpressionJsEmitterTests.cs - emitter unit tests
- BinderEmitterTests.cs - binder emitter unit tests
- SnapshotGenerator.cs - utility to regenerate baselines
- 11 template files in Templates/
- 16 expected output files in ExpectedOutput/
- Enhanced TemplateIRBuilderTests.cs with content assertions

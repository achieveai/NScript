# Razor Skin Templates Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a compile-time Razor template engine that parses `.skin.cshtml` files through Roslyn semantic analysis to auto-detect observable bindings and generates JavaScript factory methods producing `SkinInstance` objects.

**Architecture:** 5-phase pipeline: Razor Parser (Microsoft.AspNetCore.Razor.Language) produces C# source from `.skin.cshtml` files. Roslyn semantic analysis resolves types and detects observable properties. A Template IR Builder classifies each expression as OneTime/OneWay/Event. A JS Factory Generator emits JavaScript factory methods using the existing `SkinInstance`/`SkinBinderInfo` runtime. A `RazorTemplatingPlugin` integrates this into the NScript compiler alongside the existing XWML plugin.

**Tech Stack:** C# (.NET 6.0), Microsoft.AspNetCore.Razor.Language, Roslyn (Microsoft.CodeAnalysis), NScript compiler pipeline (NScript.JST, NScript.CLR, NScript.Converter), MSTest

**Spec:** `docs/superpowers/specs/2026-03-26-razor-skin-templates-design.md`

---

## File Structure

### New Project: `Sources/Compiler/RazorSkinParser/`

| File | Responsibility |
|------|---------------|
| `RazorSkinParser.csproj` | Project file with Razor.Language + existing NScript project refs |
| `RazorTemplatingPlugin.cs` | `IMethodConverterPlugin` + `IRuntimeConverterPlugin` — entry point |
| `RazorSkinPreprocessor.cs` | Extract `@control`/`@using` directives, clean template for Razor parser |
| `RazorSkinCompiler.cs` | Orchestrate the 5-phase pipeline end-to-end |
| `RazorParserPhase.cs` | Phase 1: Drive `RazorProjectEngine` to produce C# + syntax tree |
| `RoslynAnalysisPhase.cs` | Phase 2: Add generated C# to Roslyn compilation, get `SemanticModel` |
| `ObservableAnalyzer.cs` | Classify properties as observable using Roslyn type resolution |
| `TemplateIR/TemplateIRBuilder.cs` | Phase 3: Walk Razor syntax tree + Roslyn types, produce IR |
| `TemplateIR/IRNode.cs` | IR node types: `HtmlNode`, `ExpressionBindingNode`, `ConditionalNode`, `LoopNode`, `EventNode`, `FunctionNode`, `SubControlNode` |
| `TemplateIR/BindingClassification.cs` | Enum + dependency set for each binding: OneTime, OneWay, Event |
| `CodeGen/RazorSkinCodeGenerator.cs` | Phase 4: Walk IR tree, emit JS factory methods (extends SkinCodeGenerator patterns) |
| `CodeGen/ExpressionJsEmitter.cs` | Compile C# expressions to JS getter/setter functions |
| `CodeGen/BinderEmitter.cs` | Emit `SkinBinderInfo` factory calls for each binding type |
| `CodeGen/ReactiveBlockEmitter.cs` | Emit ConditionalBinder/CollectionBinder setup code |

### New Project: `Test/Compiler/RazorSkinParser.Test/`

| File | Responsibility |
|------|---------------|
| `RazorSkinParser.Test.csproj` | Test project with MSTest + FluentAssertions |
| `PreprocessorTests.cs` | Tests for `@control`/`@using` extraction |
| `RazorParsingTests.cs` | Tests for Phase 1 Razor → C# generation |
| `ObservableAnalyzerTests.cs` | Tests for observable property classification |
| `TemplateIRBuilderTests.cs` | Tests for Phase 3 IR construction |
| `CodeGenTests.cs` | Tests for Phase 4 JS output (snapshot tests against expected JS) |
| `EndToEndTests.cs` | Full pipeline: `.skin.cshtml` → JS factory string |
| `Templates/` | Test `.skin.cshtml` files |
| `ExpectedOutput/` | Expected JS output for snapshot comparison |

### Modified Files

| File | Change |
|------|--------|
| `Sources/Compiler/NScript.Lib/CommandLine.cs:22-26` | Add `RazorTemplatingPlugin` to plugins list |
| `NScript_Full.sln` | Add `RazorSkinParser` and `RazorSkinParser.Test` projects |

### New Runtime Files (Framework — compiled by NScript custom compiler)

| File | Responsibility |
|------|---------------|
| `Sources/Framework/Sunlight.Framework.UI/Helpers/MultiDependencyBinder.cs` | Runtime binder watching N properties, recomputing via single getter |
| `Sources/Framework/Sunlight.Framework.UI/Helpers/ConditionalBinder.cs` | Runtime binder for reactive `@if`/`@else` DOM fragment swapping |
| `Sources/Framework/Sunlight.Framework.UI/Helpers/CollectionBinder.cs` | Runtime binder for reactive `@foreach` incremental DOM updates |

---

## Task 1: Project Scaffolding

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/RazorSkinParser.csproj`
- Create: `Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj`
- Modify: `NScript_Full.sln`

- [ ] **Step 1: Create the RazorSkinParser project file**

```xml
<!-- Sources/Compiler/RazorSkinParser/RazorSkinParser.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>disable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Razor.Language" Version="6.0.36" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\NScript.CLR\NScript.CLR.csproj" />
    <ProjectReference Include="..\NScript.Converter\NScript.Converter.csproj" />
    <ProjectReference Include="..\NScript.JS.AST\NScript.JS.AST.csproj" />
    <ProjectReference Include="..\NScript.Utils\NScript.Utils.csproj" />
  </ItemGroup>
</Project>
```

- [ ] **Step 2: Create the test project file**

```xml
<!-- Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FluentAssertions" Version="5.9.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="16.10.0" />
    <PackageReference Include="MSTest.TestAdapter" Version="2.2.5" />
    <PackageReference Include="MSTest.TestFramework" Version="2.2.5" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\..\Sources\Compiler\RazorSkinParser\RazorSkinParser.csproj" />
    <ProjectReference Include="..\..\..\Sources\Compiler\NScript.CLR\NScript.CLR.csproj" />
    <ProjectReference Include="..\..\..\Sources\Compiler\NScript.Converter\NScript.Converter.csproj" />
    <ProjectReference Include="..\..\..\Sources\Compiler\NScript.JS.AST\NScript.JS.AST.csproj" />
    <ProjectReference Include="..\..\..\Sources\Compiler\NScript.Utils\NScript.Utils.csproj" />
  </ItemGroup>

  <ItemGroup>
    <None Update="Templates\**\*" CopyToOutputDirectory="PreserveNewest" />
    <None Update="ExpectedOutput\**\*" CopyToOutputDirectory="PreserveNewest" />
  </ItemGroup>
</Project>
```

- [ ] **Step 3: Add both projects to the solution**

```bash
cd b:/sources/NScript
dotnet sln NScript_Full.sln add Sources/Compiler/RazorSkinParser/RazorSkinParser.csproj
dotnet sln NScript_Full.sln add Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj
```

- [ ] **Step 4: Verify the solution builds**

```bash
dotnet build NScript_Full.sln -c Release
```

Expected: Build succeeds (no source files yet, just project scaffolding).

- [ ] **Step 5: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/RazorSkinParser.csproj \
       Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj \
       NScript_Full.sln
git commit -m "Scaffold RazorSkinParser project and test project"
```

---

## Task 2: Template Preprocessor

Extracts `@control` and `@using` directives (not recognized by standard Razor) from `.skin.cshtml` files before they reach the Razor parser.

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/RazorSkinPreprocessor.cs`
- Create: `Test/Compiler/RazorSkinParser.Test/PreprocessorTests.cs`

- [ ] **Step 1: Write the failing tests**

```csharp
// Test/Compiler/RazorSkinParser.Test/PreprocessorTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class PreprocessorTests
    {
        [TestMethod]
        public void ExtractsModelDirective()
        {
            var input = "@model MyApp.ViewModels.OrderVM\n<div>Hello</div>";
            var result = RazorSkinPreprocessor.Process(input);

            result.ModelTypeName.Should().Be("MyApp.ViewModels.OrderVM");
            result.CleanedTemplate.Should().Contain("@model MyApp.ViewModels.OrderVM");
        }

        [TestMethod]
        public void ExtractsControlDirective()
        {
            var input = "@model MyVM\n@control Sunlight.Framework.UI.UISkinableElement\n<div>Hello</div>";
            var result = RazorSkinPreprocessor.Process(input);

            result.ControlTypeName.Should().Be("Sunlight.Framework.UI.UISkinableElement");
            result.CleanedTemplate.Should().NotContain("@control");
        }

        [TestMethod]
        public void DefaultsControlToUISkinableElement()
        {
            var input = "@model MyVM\n<div>Hello</div>";
            var result = RazorSkinPreprocessor.Process(input);

            result.ControlTypeName.Should().Be("Sunlight.Framework.UI.UISkinableElement");
        }

        [TestMethod]
        public void ExtractsUsingDirectives()
        {
            var input = "@model MyVM\n@using Sunlight.Framework.UI\n@using MyApp.Controls\n<div/>";
            var result = RazorSkinPreprocessor.Process(input);

            result.UsingNamespaces.Should().BeEquivalentTo(
                new[] { "Sunlight.Framework.UI", "MyApp.Controls" });
        }

        [TestMethod]
        public void PreservesTemplateBodyAfterDirectiveRemoval()
        {
            var input = "@model MyVM\n@control MyCtrl\n@using NS1\n\n<div class=\"test\">\n    <span>@Model.Name</span>\n</div>";
            var result = RazorSkinPreprocessor.Process(input);

            result.CleanedTemplate.Should().Contain("<div class=\"test\">");
            result.CleanedTemplate.Should().Contain("@Model.Name");
            result.CleanedTemplate.Should().NotContain("@control");
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~PreprocessorTests" -v n
```

Expected: FAIL — `RazorSkinPreprocessor` does not exist.

- [ ] **Step 3: Implement the preprocessor**

```csharp
// Sources/Compiler/RazorSkinParser/RazorSkinPreprocessor.cs
using System;
using System.Collections.Generic;
using System.Text;

namespace NScript.RazorSkin
{
    public class PreprocessorResult
    {
        public string ModelTypeName { get; set; }
        public string ControlTypeName { get; set; }
        public List<string> UsingNamespaces { get; set; } = new List<string>();
        public string CleanedTemplate { get; set; }
    }

    public static class RazorSkinPreprocessor
    {
        private const string DefaultControlType = "Sunlight.Framework.UI.UISkinableElement";

        public static PreprocessorResult Process(string templateSource)
        {
            var result = new PreprocessorResult
            {
                ControlTypeName = DefaultControlType
            };

            var cleanedLines = new StringBuilder();
            var lines = templateSource.Split('\n');

            foreach (var rawLine in lines)
            {
                var line = rawLine.TrimEnd('\r');
                var trimmed = line.TrimStart();

                if (trimmed.StartsWith("@model "))
                {
                    result.ModelTypeName = trimmed.Substring("@model ".Length).Trim();
                    cleanedLines.AppendLine(line); // Keep @model for Razor
                }
                else if (trimmed.StartsWith("@control "))
                {
                    result.ControlTypeName = trimmed.Substring("@control ".Length).Trim();
                    // Remove @control — not valid Razor
                }
                else if (trimmed.StartsWith("@using "))
                {
                    var ns = trimmed.Substring("@using ".Length).Trim();
                    result.UsingNamespaces.Add(ns);
                    cleanedLines.AppendLine(line); // Keep @using for Razor
                }
                else
                {
                    cleanedLines.AppendLine(line);
                }
            }

            result.CleanedTemplate = cleanedLines.ToString().TrimEnd('\r', '\n');
            return result;
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~PreprocessorTests" -v n
```

Expected: All 5 tests PASS.

- [ ] **Step 5: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/RazorSkinPreprocessor.cs \
       Test/Compiler/RazorSkinParser.Test/PreprocessorTests.cs
git commit -m "Add RazorSkinPreprocessor for @control/@using extraction"
```

---

## Task 3: Razor Parsing Phase

Drive `RazorProjectEngine` to parse cleaned `.skin.cshtml` templates and produce both generated C# source and the Razor syntax tree.

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/RazorParserPhase.cs`
- Create: `Test/Compiler/RazorSkinParser.Test/RazorParsingTests.cs`
- Create: `Test/Compiler/RazorSkinParser.Test/Templates/SimpleBinding.skin.cshtml`

- [ ] **Step 1: Create test template file**

```razor
@* Test/Compiler/RazorSkinParser.Test/Templates/SimpleBinding.skin.cshtml *@
@model TestModel

<div>
    <span>@Model.Name</span>
</div>
```

- [ ] **Step 2: Write the failing tests**

```csharp
// Test/Compiler/RazorSkinParser.Test/RazorParsingTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class RazorParsingTests
    {
        [TestMethod]
        public void ParsesSimpleTemplateToGeneratedCSharp()
        {
            var template = "@model TestModel\n\n<div>\n    <span>@Model.Name</span>\n</div>";
            var preprocessed = RazorSkinPreprocessor.Process(template);

            var result = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            result.GeneratedCSharp.Should().NotBeNullOrEmpty();
            result.GeneratedCSharp.Should().Contain("WriteLiteral");
            result.GeneratedCSharp.Should().Contain("Model.Name");
        }

        [TestMethod]
        public void ProducesRazorSyntaxTree()
        {
            var template = "@model TestModel\n\n<div>@Model.Name</div>";
            var preprocessed = RazorSkinPreprocessor.Process(template);

            var result = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            result.SyntaxTree.Should().NotBeNull();
        }

        [TestMethod]
        public void HandlesIfBlocks()
        {
            var template = "@model TestModel\n\n@if (Model.IsActive)\n{\n    <div>Active</div>\n}";
            var preprocessed = RazorSkinPreprocessor.Process(template);

            var result = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            result.GeneratedCSharp.Should().Contain("Model.IsActive");
        }

        [TestMethod]
        public void HandlesForeachBlocks()
        {
            var template = "@model TestModel\n\n@foreach (var item in Model.Items)\n{\n    <li>@item.Name</li>\n}";
            var preprocessed = RazorSkinPreprocessor.Process(template);

            var result = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            result.GeneratedCSharp.Should().Contain("Model.Items");
            result.GeneratedCSharp.Should().Contain("item.Name");
        }

        [TestMethod]
        public void HandlesFunctionsBlock()
        {
            var template = "@model TestModel\n\n@functions {\n    string Format(int x) => x.ToString();\n}\n\n<div>@Format(Model.Count)</div>";
            var preprocessed = RazorSkinPreprocessor.Process(template);

            var result = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            result.GeneratedCSharp.Should().Contain("Format");
        }
    }
}
```

- [ ] **Step 3: Run tests to verify they fail**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~RazorParsingTests" -v n
```

Expected: FAIL — `RazorParserPhase` does not exist.

- [ ] **Step 4: Implement the Razor parsing phase**

```csharp
// Sources/Compiler/RazorSkinParser/RazorParserPhase.cs
using System;
using System.IO;
using Microsoft.AspNetCore.Razor.Language;

namespace NScript.RazorSkin
{
    public class RazorParseResult
    {
        public string GeneratedCSharp { get; set; }
        public RazorSyntaxTree SyntaxTree { get; set; }
        public RazorCodeDocument CodeDocument { get; set; }
    }

    public static class RazorParserPhase
    {
        public static RazorParseResult Parse(string templateName, string cleanedTemplate)
        {
            var projectEngine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create("."),
                builder =>
                {
                    builder.SetRootNamespace("NScript.RazorSkin.Generated");
                });

            var sourceDocument = RazorSourceDocument.Create(
                cleanedTemplate,
                $"{templateName}.skin.cshtml");

            var codeDocument = RazorCodeDocument.Create(sourceDocument);
            projectEngine.Process(codeDocument);

            var csharpDocument = codeDocument.GetCSharpDocument();
            var syntaxTree = codeDocument.GetSyntaxTree();

            return new RazorParseResult
            {
                GeneratedCSharp = csharpDocument.GeneratedCode,
                SyntaxTree = syntaxTree,
                CodeDocument = codeDocument
            };
        }
    }
}
```

- [ ] **Step 5: Run tests to verify they pass**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~RazorParsingTests" -v n
```

Expected: All 5 tests PASS.

- [ ] **Step 6: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/RazorParserPhase.cs \
       Test/Compiler/RazorSkinParser.Test/RazorParsingTests.cs \
       Test/Compiler/RazorSkinParser.Test/Templates/SimpleBinding.skin.cshtml
git commit -m "Add RazorParserPhase using RazorProjectEngine"
```

---

## Task 4: Template IR Data Structures

Define the intermediate representation nodes that the IR builder will produce and the code generator will consume.

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/TemplateIR/IRNode.cs`
- Create: `Sources/Compiler/RazorSkinParser/TemplateIR/BindingClassification.cs`

- [ ] **Step 1: Define the binding classification types**

```csharp
// Sources/Compiler/RazorSkinParser/TemplateIR/BindingClassification.cs
using System.Collections.Generic;

namespace NScript.RazorSkin.TemplateIR
{
    public enum BindingMode
    {
        OneTime,   // No observable dependencies — evaluate once
        OneWay,    // Has observable dependencies — live updates
        Event      // Event handler (method ref or lambda)
    }

    public enum BindingSourceKind
    {
        DataContext,      // @Model.* references
        TemplateParent,   // @Control.* references
        Mixed             // Expression references both Model and Control
    }

    public class ObservableDependency
    {
        public BindingSourceKind SourceKind { get; set; }
        public string PropertyName { get; set; }
        public string PropertyChain { get; set; } // e.g., "Customer.Address.City"

        public ObservableDependency(BindingSourceKind sourceKind, string propertyName, string propertyChain)
        {
            SourceKind = sourceKind;
            PropertyName = propertyName;
            PropertyChain = propertyChain;
        }
    }

    public class BindingClassification
    {
        public BindingMode Mode { get; set; }
        public BindingSourceKind SourceKind { get; set; }
        public List<ObservableDependency> Dependencies { get; set; } = new List<ObservableDependency>();
        public string CSharpExpression { get; set; } // Original C# expression text
    }
}
```

- [ ] **Step 2: Define the IR node types**

```csharp
// Sources/Compiler/RazorSkinParser/TemplateIR/IRNode.cs
using System.Collections.Generic;

namespace NScript.RazorSkin.TemplateIR
{
    public abstract class IRNode
    {
        public List<IRNode> Children { get; set; } = new List<IRNode>();
    }

    /// <summary>Root of a skin template IR tree.</summary>
    public class SkinTemplateNode : IRNode
    {
        public string TemplateName { get; set; }
        public string ModelTypeName { get; set; }
        public string ControlTypeName { get; set; }
        public List<string> UsingNamespaces { get; set; } = new List<string>();
        public List<FunctionNode> Functions { get; set; } = new List<FunctionNode>();
    }

    /// <summary>Static HTML content (no bindings).</summary>
    public class HtmlNode : IRNode
    {
        public string HtmlContent { get; set; }
    }

    /// <summary>An @ expression bound to a DOM target (text, attribute, style, CSS class).</summary>
    public class ExpressionBindingNode : IRNode
    {
        public BindingClassification Classification { get; set; }
        public ExpressionTarget Target { get; set; }
        public string ElementId { get; set; } // Part ID if element has id= attribute
    }

    public enum ExpressionTarget
    {
        TextContent,    // @Model.Name as text node
        Attribute,      // value="@Model.X"
        CssClass,       // class="@expr"
        Style           // style="display: @expr"
    }

    public class AttributeExpressionInfo
    {
        public string AttributeName { get; set; }
    }

    /// <summary>Reactive @if / @else block.</summary>
    public class ConditionalNode : IRNode
    {
        public BindingClassification Condition { get; set; }
        public List<IRNode> TrueBranch { get; set; } = new List<IRNode>();
        public List<IRNode> FalseBranch { get; set; } = new List<IRNode>();
        public bool IsReactive { get; set; } // true if condition has observable dependencies
    }

    /// <summary>Reactive @foreach block.</summary>
    public class LoopNode : IRNode
    {
        public string ItemVariableName { get; set; }       // "order" in @foreach(var order in ...)
        public string CollectionExpression { get; set; }   // "Model.Orders"
        public bool IsObservableCollection { get; set; }   // true → incremental DOM updates
        public BindingSourceKind CollectionSourceKind { get; set; }
        public List<IRNode> ItemTemplate { get; set; } = new List<IRNode>(); // Loop body IR
    }

    /// <summary>DOM event handler (onclick, onchange, etc.).</summary>
    public class EventNode : IRNode
    {
        public string DomEventName { get; set; }           // "click", "change", etc.
        public string HandlerExpression { get; set; }      // "Model.OnSubmit" or "(evt) => ..."
        public bool IsLambda { get; set; }                 // true if inline lambda
    }

    /// <summary>Helper function from @functions block.</summary>
    public class FunctionNode : IRNode
    {
        public string FunctionName { get; set; }
        public string CSharpSource { get; set; }
        public bool IsPure { get; set; }                   // true if no Model/Control references
        public List<ObservableDependency> Dependencies { get; set; } = new List<ObservableDependency>();
    }

    /// <summary>Child UIElement declared as PascalCase tag.</summary>
    public class SubControlNode : IRNode
    {
        public string TypeName { get; set; }               // "ListView", "SearchBox"
        public string ResolvedTypeName { get; set; }       // Fully qualified type name
        public string ElementId { get; set; }              // Part ID from id= attribute
        public List<SubControlPropertyBinding> PropertyBindings { get; set; } = new List<SubControlPropertyBinding>();
        public List<EventNode> EventBindings { get; set; } = new List<EventNode>();
    }

    public class SubControlPropertyBinding
    {
        public string PropertyName { get; set; }           // "ObservableList", "Query"
        public BindingClassification Classification { get; set; }
    }
}
```

- [ ] **Step 3: Verify it compiles**

```bash
dotnet build Sources/Compiler/RazorSkinParser/RazorSkinParser.csproj
```

Expected: Build succeeds.

- [ ] **Step 4: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/TemplateIR/
git commit -m "Add Template IR data structures for Razor skin pipeline"
```

---

## Task 5: Observable Analyzer

Uses Roslyn semantic model to classify whether a property is observable (its containing type inherits from `ObservableObject` or implements `INotifyPropertyChanged`).

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/ObservableAnalyzer.cs`
- Create: `Test/Compiler/RazorSkinParser.Test/ObservableAnalyzerTests.cs`

- [ ] **Step 1: Write the failing tests**

```csharp
// Test/Compiler/RazorSkinParser.Test/ObservableAnalyzerTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NScript.RazorSkin;
using NScript.RazorSkin.TemplateIR;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class ObservableAnalyzerTests
    {
        private static CSharpCompilation CreateCompilationWithTypes(string source)
        {
            // Minimal type stubs for testing
            var frameworkStubs = @"
namespace Sunlight.Framework.Observables
{
    public interface INotifyPropertyChanged { }
    public class ObservableObject : INotifyPropertyChanged
    {
        protected void FirePropertyChanged(string name) { }
    }
    public interface IObservableCollection { }
    public class ObservableCollection<T> : ObservableObject, IObservableCollection { }
}
";
            var tree1 = CSharpSyntaxTree.ParseText(frameworkStubs);
            var tree2 = CSharpSyntaxTree.ParseText(source);

            return CSharpCompilation.Create("TestAssembly",
                new[] { tree1, tree2 },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        [TestMethod]
        public void DetectsObservablePropertyOnObservableObject()
        {
            var source = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string Name { get; set; }
}";
            var compilation = CreateCompilationWithTypes(source);
            var type = compilation.GetTypeByMetadataName("TestVM");
            var prop = type.GetMembers("Name")[0] as IPropertySymbol;

            ObservableAnalyzer.IsObservableProperty(prop).Should().BeTrue();
        }

        [TestMethod]
        public void DetectsNonObservablePropertyOnPlainClass()
        {
            var source = @"
public class PlainObj
{
    public string AppVersion { get; set; }
}";
            var compilation = CreateCompilationWithTypes(source);
            var type = compilation.GetTypeByMetadataName("PlainObj");
            var prop = type.GetMembers("AppVersion")[0] as IPropertySymbol;

            ObservableAnalyzer.IsObservableProperty(prop).Should().BeFalse();
        }

        [TestMethod]
        public void DetectsObservableCollection()
        {
            var source = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public ObservableCollection<string> Items { get; set; }
}";
            var compilation = CreateCompilationWithTypes(source);
            var type = compilation.GetTypeByMetadataName("TestVM");
            var prop = type.GetMembers("Items")[0] as IPropertySymbol;

            ObservableAnalyzer.IsObservableCollection(prop.Type).Should().BeTrue();
        }

        [TestMethod]
        public void DetectsNonObservableList()
        {
            var source = @"
using System.Collections.Generic;
public class TestVM
{
    public List<string> Items { get; set; }
}";
            var compilation = CreateCompilationWithTypes(source);
            var type = compilation.GetTypeByMetadataName("TestVM");
            var prop = type.GetMembers("Items")[0] as IPropertySymbol;

            ObservableAnalyzer.IsObservableCollection(prop.Type).Should().BeFalse();
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~ObservableAnalyzerTests" -v n
```

Expected: FAIL — `ObservableAnalyzer` does not exist.

- [ ] **Step 3: Implement the observable analyzer**

```csharp
// Sources/Compiler/RazorSkinParser/ObservableAnalyzer.cs
using Microsoft.CodeAnalysis;

namespace NScript.RazorSkin
{
    public static class ObservableAnalyzer
    {
        private const string ObservableObjectFullName = "Sunlight.Framework.Observables.ObservableObject";
        private const string INotifyPropertyChangedFullName = "Sunlight.Framework.Observables.INotifyPropertyChanged";
        private const string IObservableCollectionFullName = "Sunlight.Framework.Observables.IObservableCollection";

        public static bool IsObservableProperty(IPropertySymbol property)
        {
            if (property == null)
                return false;

            var containingType = property.ContainingType;
            return IsObservableType(containingType);
        }

        public static bool IsObservableType(ITypeSymbol type)
        {
            if (type == null)
                return false;

            // Check 1: Type inherits from ObservableObject
            var current = type;
            while (current != null)
            {
                if (GetFullName(current) == ObservableObjectFullName)
                    return true;
                current = current.BaseType;
            }

            // Check 2: Type implements INotifyPropertyChanged
            foreach (var iface in type.AllInterfaces)
            {
                if (GetFullName(iface) == INotifyPropertyChangedFullName)
                    return true;
            }

            return false;
        }

        public static bool IsObservableCollection(ITypeSymbol type)
        {
            if (type == null)
                return false;

            // Check if type implements IObservableCollection
            foreach (var iface in type.AllInterfaces)
            {
                if (GetFullName(iface) == IObservableCollectionFullName)
                    return true;
            }

            // Also check the type itself
            if (GetFullName(type) == IObservableCollectionFullName)
                return true;

            return false;
        }

        private static string GetFullName(ITypeSymbol type)
        {
            if (type.ContainingNamespace == null || type.ContainingNamespace.IsGlobalNamespace)
                return type.Name;

            return $"{type.ContainingNamespace.ToDisplayString()}.{type.Name}";
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~ObservableAnalyzerTests" -v n
```

Expected: All 4 tests PASS.

- [ ] **Step 5: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/ObservableAnalyzer.cs \
       Test/Compiler/RazorSkinParser.Test/ObservableAnalyzerTests.cs
git commit -m "Add ObservableAnalyzer for Roslyn-based observable type detection"
```

---

## Task 6: Template IR Builder (Phase 3)

Walks the Razor syntax tree and Roslyn semantic model to produce the Template IR. This is the core intelligence — it classifies every expression.

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/TemplateIR/TemplateIRBuilder.cs`
- Create: `Test/Compiler/RazorSkinParser.Test/TemplateIRBuilderTests.cs`

- [ ] **Step 1: Write the failing tests**

```csharp
// Test/Compiler/RazorSkinParser.Test/TemplateIRBuilderTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;
using NScript.RazorSkin.TemplateIR;
using System.Linq;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class TemplateIRBuilderTests
    {
        [TestMethod]
        public void StaticHtmlProducesHtmlNode()
        {
            var ir = BuildIR("@model TestVM\n\n<div>Hello World</div>");

            ir.Children.Should().ContainSingle()
                .Which.Should().BeOfType<HtmlNode>();
        }

        [TestMethod]
        public void SimpleExpressionProducesExpressionBindingNode()
        {
            var ir = BuildIR("@model TestVM\n\n<div>@Model.Name</div>");

            var binding = ir.Children.OfType<HtmlNode>()
                .SelectMany(h => ir.Children.OfType<ExpressionBindingNode>())
                .FirstOrDefault();

            // At minimum, the IR should contain an expression binding
            ir.Children.OfType<ExpressionBindingNode>().Should().NotBeEmpty();
        }

        [TestMethod]
        public void IfBlockProducesConditionalNode()
        {
            var ir = BuildIR("@model TestVM\n\n@if (Model.IsActive)\n{\n    <div>Active</div>\n}");

            ir.Children.OfType<ConditionalNode>().Should().NotBeEmpty();
        }

        [TestMethod]
        public void ForeachBlockProducesLoopNode()
        {
            var ir = BuildIR("@model TestVM\n\n@foreach (var item in Model.Items)\n{\n    <li>@item.Name</li>\n}");

            ir.Children.OfType<LoopNode>().Should().NotBeEmpty();
        }

        [TestMethod]
        public void FunctionsBlockProducesFunctionNodes()
        {
            var ir = BuildIR("@model TestVM\n\n@functions {\n    string Fmt(int x) => x.ToString();\n}\n\n<div>@Fmt(42)</div>");

            ir.Functions.Should().NotBeEmpty();
            ir.Functions.First().FunctionName.Should().Be("Fmt");
        }

        private SkinTemplateNode BuildIR(string template)
        {
            var preprocessed = RazorSkinPreprocessor.Process(template);
            var parsed = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);

            return TemplateIRBuilder.Build(
                "TestSkin",
                preprocessed,
                parsed);
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~TemplateIRBuilderTests" -v n
```

Expected: FAIL — `TemplateIRBuilder` does not exist.

- [ ] **Step 3: Implement the IR builder**

This is the largest single file. It walks the Razor syntax tree node by node and classifies each:

```csharp
// Sources/Compiler/RazorSkinParser/TemplateIR/TemplateIRBuilder.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace NScript.RazorSkin.TemplateIR
{
    public static class TemplateIRBuilder
    {
        public static SkinTemplateNode Build(
            string templateName,
            PreprocessorResult preprocessed,
            RazorParseResult parsed)
        {
            var root = new SkinTemplateNode
            {
                TemplateName = templateName,
                ModelTypeName = preprocessed.ModelTypeName,
                ControlTypeName = preprocessed.ControlTypeName,
                UsingNamespaces = preprocessed.UsingNamespaces
            };

            var syntaxTree = parsed.SyntaxTree;
            if (syntaxTree?.Root == null)
                return root;

            WalkSyntaxTree(syntaxTree.Root, root, root);
            return root;
        }

        private static void WalkSyntaxTree(
            Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxNode node,
            IRNode currentParent,
            SkinTemplateNode root)
        {
            if (node == null) return;

            foreach (var child in node.ChildNodes())
            {
                var childNode = child as Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxNode;
                if (childNode == null) continue;

                var kind = childNode.Kind;

                // Detect markup literal content (static HTML)
                if (kind == Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxKind.MarkupTextLiteral ||
                    kind == Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxKind.MarkupEphemeralTextLiteral)
                {
                    var content = childNode.GetContent();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        currentParent.Children.Add(new HtmlNode { HtmlContent = content });
                    }
                    continue;
                }

                // Detect C# expression (@Model.Name, @(expr))
                if (kind == Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxKind.CSharpExpressionLiteral)
                {
                    var expression = childNode.GetContent();
                    if (!string.IsNullOrWhiteSpace(expression))
                    {
                        currentParent.Children.Add(CreateExpressionBinding(expression));
                    }
                    continue;
                }

                // Detect @if blocks
                if (IsIfStatement(childNode))
                {
                    var conditional = CreateConditionalNode(childNode);
                    if (conditional != null)
                    {
                        currentParent.Children.Add(conditional);
                        continue;
                    }
                }

                // Detect @foreach blocks
                if (IsForeachStatement(childNode))
                {
                    var loop = CreateLoopNode(childNode);
                    if (loop != null)
                    {
                        currentParent.Children.Add(loop);
                        continue;
                    }
                }

                // Detect @functions block
                if (IsFunctionsBlock(childNode))
                {
                    var functions = ExtractFunctions(childNode);
                    root.Functions.AddRange(functions);
                    continue;
                }

                // Recurse into other nodes
                WalkSyntaxTree(childNode, currentParent, root);
            }
        }

        private static ExpressionBindingNode CreateExpressionBinding(string expression)
        {
            var classification = new BindingClassification
            {
                CSharpExpression = expression.Trim(),
                // Preliminary classification — refined in Phase 2 with Roslyn semantic info
                Mode = BindingMode.OneTime,
                SourceKind = expression.Contains("Model.") ? BindingSourceKind.DataContext
                           : expression.Contains("Control.") ? BindingSourceKind.TemplateParent
                           : BindingSourceKind.DataContext
            };

            return new ExpressionBindingNode
            {
                Classification = classification,
                Target = ExpressionTarget.TextContent
            };
        }

        private static ConditionalNode CreateConditionalNode(
            Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxNode node)
        {
            // Extract condition expression from the if statement
            var content = node.GetContent();
            var condExpr = ExtractConditionExpression(content);

            var conditional = new ConditionalNode
            {
                Condition = new BindingClassification
                {
                    CSharpExpression = condExpr,
                    Mode = BindingMode.OneTime, // Refined with Roslyn later
                    SourceKind = BindingSourceKind.DataContext
                },
                IsReactive = false // Refined with Roslyn later
            };

            // Walk true/false branches
            // NOTE: Full branch extraction requires deeper Razor tree traversal.
            // This is the skeleton — the actual branch walking is handled
            // by traversing the CSharpCodeBlock child nodes of the if/else blocks.

            return conditional;
        }

        private static LoopNode CreateLoopNode(
            Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxNode node)
        {
            var content = node.GetContent();

            // Parse "var item in Model.Items" pattern
            var foreachMatch = ExtractForeachParts(content);
            if (foreachMatch == null) return null;

            return new LoopNode
            {
                ItemVariableName = foreachMatch.Item1,
                CollectionExpression = foreachMatch.Item2,
                IsObservableCollection = false, // Refined with Roslyn later
                CollectionSourceKind = BindingSourceKind.DataContext
            };
        }

        private static List<FunctionNode> ExtractFunctions(
            Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxNode node)
        {
            var functions = new List<FunctionNode>();
            var content = node.GetContent();

            // Simple extraction: find method declarations in the functions block.
            // For now, create a single FunctionNode with the raw source.
            // Full parsing of individual methods uses Roslyn in Phase 2.
            if (!string.IsNullOrWhiteSpace(content))
            {
                functions.Add(new FunctionNode
                {
                    FunctionName = "functions_block",
                    CSharpSource = content,
                    IsPure = !content.Contains("Model.") && !content.Contains("Control.")
                });
            }

            return functions;
        }

        // --- Helper methods ---

        private static bool IsIfStatement(Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxNode node)
        {
            var content = node.GetContent();
            return content != null && content.TrimStart().StartsWith("if");
        }

        private static bool IsForeachStatement(Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxNode node)
        {
            var content = node.GetContent();
            return content != null && content.TrimStart().StartsWith("foreach");
        }

        private static bool IsFunctionsBlock(Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxNode node)
        {
            return node.Kind == Microsoft.AspNetCore.Razor.Language.Syntax.SyntaxKind.RazorDirectiveBody;
        }

        private static string ExtractConditionExpression(string ifContent)
        {
            // Extract expression from "if (Model.IsActive)" → "Model.IsActive"
            if (ifContent == null) return "";
            var start = ifContent.IndexOf('(');
            var end = ifContent.LastIndexOf(')');
            if (start >= 0 && end > start)
                return ifContent.Substring(start + 1, end - start - 1).Trim();
            return ifContent;
        }

        private static Tuple<string, string> ExtractForeachParts(string content)
        {
            // Extract from "foreach (var item in Model.Items)" → ("item", "Model.Items")
            if (content == null) return null;
            var inIdx = content.IndexOf(" in ");
            if (inIdx < 0) return null;

            var varPart = content.Substring(0, inIdx).Trim();
            var collPart = content.Substring(inIdx + 4).Trim().TrimEnd(')', ' ');

            // Remove "foreach (var " prefix
            var lastSpace = varPart.LastIndexOf(' ');
            var itemName = lastSpace >= 0 ? varPart.Substring(lastSpace + 1) : varPart;

            return Tuple.Create(itemName, collPart);
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~TemplateIRBuilderTests" -v n
```

Expected: Tests pass. Some may need adjustment based on exact Razor syntax tree structure — fix any failures by inspecting the actual node kinds produced by the Razor parser.

- [ ] **Step 5: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/TemplateIR/TemplateIRBuilder.cs \
       Test/Compiler/RazorSkinParser.Test/TemplateIRBuilderTests.cs
git commit -m "Add TemplateIRBuilder to walk Razor syntax tree into IR"
```

---

## Task 7: Roslyn Analysis Phase (Phase 2)

Add the generated C# to a Roslyn compilation to get full type resolution, then refine the IR binding classifications using the `ObservableAnalyzer`.

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/RoslynAnalysisPhase.cs`
- Create: `Test/Compiler/RazorSkinParser.Test/RoslynAnalysisTests.cs`

- [ ] **Step 1: Write the failing tests**

```csharp
// Test/Compiler/RazorSkinParser.Test/RoslynAnalysisTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;
using NScript.RazorSkin.TemplateIR;
using System.Linq;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class RoslynAnalysisTests
    {
        private const string FrameworkStubs = @"
namespace Sunlight.Framework.Observables
{
    public interface INotifyPropertyChanged { }
    public class ObservableObject : INotifyPropertyChanged
    {
        protected void FirePropertyChanged(string name) { }
    }
    public interface IObservableCollection { }
    public class ObservableCollection<T> : ObservableObject, IObservableCollection { }
}";

        [TestMethod]
        public void ClassifiesObservablePropertyAsOneWay()
        {
            var vmSource = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string Name { get; set; }
}";
            var template = "@model TestVM\n\n<div>@Model.Name</div>";
            var ir = BuildAndAnalyze(template, vmSource);

            var binding = ir.Children.OfType<ExpressionBindingNode>().First();
            binding.Classification.Mode.Should().Be(BindingMode.OneWay);
        }

        [TestMethod]
        public void ClassifiesNonObservablePropertyAsOneTime()
        {
            var vmSource = @"
public class TestVM
{
    public string AppVersion { get; set; }
}";
            var template = "@model TestVM\n\n<div>@Model.AppVersion</div>";
            var ir = BuildAndAnalyze(template, vmSource);

            var binding = ir.Children.OfType<ExpressionBindingNode>().First();
            binding.Classification.Mode.Should().Be(BindingMode.OneTime);
        }

        [TestMethod]
        public void DetectsObservableCollectionInForeach()
        {
            var vmSource = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public ObservableCollection<string> Items { get; set; }
}";
            var template = "@model TestVM\n\n@foreach (var item in Model.Items)\n{\n    <li>@item</li>\n}";
            var ir = BuildAndAnalyze(template, vmSource);

            var loop = ir.Children.OfType<LoopNode>().First();
            loop.IsObservableCollection.Should().BeTrue();
        }

        private SkinTemplateNode BuildAndAnalyze(string template, string vmSource)
        {
            var preprocessed = RazorSkinPreprocessor.Process(template);
            var parsed = RazorParserPhase.Parse("TestSkin", preprocessed.CleanedTemplate);
            var ir = TemplateIRBuilder.Build("TestSkin", preprocessed, parsed);

            RoslynAnalysisPhase.RefineClassifications(
                ir,
                parsed.GeneratedCSharp,
                new[] { FrameworkStubs, vmSource });

            return ir;
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~RoslynAnalysisTests" -v n
```

Expected: FAIL — `RoslynAnalysisPhase` does not exist.

- [ ] **Step 3: Implement the Roslyn analysis phase**

```csharp
// Sources/Compiler/RazorSkinParser/RoslynAnalysisPhase.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin
{
    public static class RoslynAnalysisPhase
    {
        public static void RefineClassifications(
            SkinTemplateNode ir,
            string generatedCSharp,
            string[] additionalSources)
        {
            // Build a Roslyn compilation from the generated C# + framework stubs
            var trees = new List<SyntaxTree>();
            trees.Add(CSharpSyntaxTree.ParseText(generatedCSharp));
            foreach (var src in additionalSources)
            {
                trees.Add(CSharpSyntaxTree.ParseText(src));
            }

            var compilation = CSharpCompilation.Create(
                "RazorSkinAnalysis",
                trees,
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generatedTree = trees[0];
            var semanticModel = compilation.GetSemanticModel(generatedTree);

            // Resolve model type
            var modelType = ResolveModelType(ir.ModelTypeName, compilation);

            // Walk all IR nodes and refine classifications
            RefineNodes(ir.Children, modelType, compilation, semanticModel);

            // Refine loop nodes
            RefineLoopNodes(ir.Children, modelType, compilation);

            // Refine conditional nodes
            RefineConditionalNodes(ir.Children, modelType);
        }

        private static void RefineNodes(
            List<IRNode> nodes,
            INamedTypeSymbol modelType,
            CSharpCompilation compilation,
            SemanticModel semanticModel)
        {
            foreach (var node in nodes)
            {
                if (node is ExpressionBindingNode binding)
                {
                    RefineExpressionBinding(binding, modelType);
                }

                RefineNodes(node.Children, modelType, compilation, semanticModel);
            }
        }

        private static void RefineExpressionBinding(
            ExpressionBindingNode binding,
            INamedTypeSymbol modelType)
        {
            if (modelType == null) return;

            var expr = binding.Classification.CSharpExpression;
            var dependencies = new List<ObservableDependency>();

            // Extract property references from the expression
            var propertyNames = ExtractPropertyReferences(expr, "Model.");
            foreach (var propName in propertyNames)
            {
                var prop = FindProperty(modelType, propName);
                if (prop != null && ObservableAnalyzer.IsObservableProperty(prop))
                {
                    dependencies.Add(new ObservableDependency(
                        BindingSourceKind.DataContext, propName, propName));
                }
            }

            // Update classification
            binding.Classification.Dependencies = dependencies;
            binding.Classification.Mode = dependencies.Count > 0
                ? BindingMode.OneWay
                : BindingMode.OneTime;
        }

        private static void RefineLoopNodes(
            List<IRNode> nodes,
            INamedTypeSymbol modelType,
            CSharpCompilation compilation)
        {
            foreach (var node in nodes)
            {
                if (node is LoopNode loop && modelType != null)
                {
                    // Check if the collection is observable
                    var collExpr = loop.CollectionExpression;
                    var propName = collExpr.Replace("Model.", "").Split('.')[0];
                    var prop = FindProperty(modelType, propName);
                    if (prop != null)
                    {
                        loop.IsObservableCollection =
                            ObservableAnalyzer.IsObservableCollection(prop.Type);
                    }
                }

                RefineLoopNodes(node.Children, modelType, compilation);
            }
        }

        private static void RefineConditionalNodes(
            List<IRNode> nodes,
            INamedTypeSymbol modelType)
        {
            foreach (var node in nodes)
            {
                if (node is ConditionalNode cond && modelType != null)
                {
                    var propNames = ExtractPropertyReferences(
                        cond.Condition.CSharpExpression, "Model.");
                    foreach (var propName in propNames)
                    {
                        var prop = FindProperty(modelType, propName);
                        if (prop != null && ObservableAnalyzer.IsObservableProperty(prop))
                        {
                            cond.IsReactive = true;
                            cond.Condition.Mode = BindingMode.OneWay;
                            cond.Condition.Dependencies.Add(new ObservableDependency(
                                BindingSourceKind.DataContext, propName, propName));
                        }
                    }
                }

                RefineConditionalNodes(node.Children, modelType);
            }
        }

        private static INamedTypeSymbol ResolveModelType(
            string modelTypeName, CSharpCompilation compilation)
        {
            if (string.IsNullOrEmpty(modelTypeName)) return null;

            // Try direct lookup
            var type = compilation.GetTypeByMetadataName(modelTypeName);
            if (type != null) return type;

            // Try without namespace (short name)
            foreach (var tree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(tree);
                var root = tree.GetRoot();
                foreach (var classDecl in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    if (classDecl.Identifier.Text == modelTypeName)
                    {
                        var symbol = model.GetDeclaredSymbol(classDecl);
                        if (symbol != null) return symbol;
                    }
                }
            }

            return null;
        }

        private static List<string> ExtractPropertyReferences(string expression, string prefix)
        {
            var props = new List<string>();
            var idx = 0;
            while ((idx = expression.IndexOf(prefix, idx, StringComparison.Ordinal)) >= 0)
            {
                idx += prefix.Length;
                var end = idx;
                while (end < expression.Length && (char.IsLetterOrDigit(expression[end]) || expression[end] == '_'))
                    end++;

                if (end > idx)
                    props.Add(expression.Substring(idx, end - idx));
                idx = end;
            }
            return props.Distinct().ToList();
        }

        private static IPropertySymbol FindProperty(INamedTypeSymbol type, string name)
        {
            var current = type;
            while (current != null)
            {
                var prop = current.GetMembers(name).OfType<IPropertySymbol>().FirstOrDefault();
                if (prop != null) return prop;
                current = current.BaseType;
            }
            return null;
        }
    }
}
```

- [ ] **Step 4: Run tests to verify they pass**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~RoslynAnalysisTests" -v n
```

Expected: All 3 tests PASS.

- [ ] **Step 5: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/RoslynAnalysisPhase.cs \
       Test/Compiler/RazorSkinParser.Test/RoslynAnalysisTests.cs
git commit -m "Add RoslynAnalysisPhase for observable classification refinement"
```

---

## Task 8: JS Code Generator — Simple Bindings (Phase 4)

Generate JavaScript factory methods for OneTime/OneWay text and attribute bindings. This follows the existing `SkinCodeGenerator` pattern from XwmlParser.

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinCodeGenerator.cs`
- Create: `Sources/Compiler/RazorSkinParser/CodeGen/ExpressionJsEmitter.cs`
- Create: `Sources/Compiler/RazorSkinParser/CodeGen/BinderEmitter.cs`
- Create: `Test/Compiler/RazorSkinParser.Test/CodeGenTests.cs`

**Reference files to study:**
- `Sources/Compiler/XwmlParser/SkinCodeGenerator.cs` — existing JS factory generation pattern
- `Test/Compiler/XwmlParser.Test/TemplateCode/TestTextBinding1.js` — expected output format

- [ ] **Step 1: Write the failing test**

```csharp
// Test/Compiler/RazorSkinParser.Test/CodeGenTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class CodeGenTests
    {
        [TestMethod]
        public void GeneratesFactoryFunctionForStaticHtml()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div>Hello</div>" });

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("TestSkin_factory");
            js.Should().Contain("createElement");
            js.Should().Contain("innerHTML");
            js.Should().Contain("cloneNode");
            js.Should().Contain("SkinInstance");
        }

        [TestMethod]
        public void GeneratesGetterForOneWayBinding()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            var binding = new ExpressionBindingNode
            {
                Target = ExpressionTarget.TextContent,
                Classification = new BindingClassification
                {
                    Mode = BindingMode.OneWay,
                    SourceKind = BindingSourceKind.DataContext,
                    CSharpExpression = "Model.Name",
                    Dependencies = new System.Collections.Generic.List<ObservableDependency>
                    {
                        new ObservableDependency(BindingSourceKind.DataContext, "Name", "Name")
                    }
                }
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div></div>" });
            ir.Children.Add(binding);

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("get_name");
            js.Should().Contain("SkinBinderInfo");
            js.Should().Contain("\"Name\"");
        }

        [TestMethod]
        public void GeneratesSkinGetterFunction()
        {
            var ir = new SkinTemplateNode
            {
                TemplateName = "TestSkin",
                ModelTypeName = "TestVM",
                ControlTypeName = "UISkinableElement"
            };
            ir.Children.Add(new HtmlNode { HtmlContent = "<div/>" });

            var js = RazorSkinCodeGenerator.Generate(ir);

            js.Should().Contain("TestSkin_var");
            js.Should().Contain("function TestSkin()");
            js.Should().Contain("Skin_factory");
        }
    }
}
```

- [ ] **Step 2: Run tests to verify they fail**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~CodeGenTests" -v n
```

Expected: FAIL — `RazorSkinCodeGenerator` does not exist.

- [ ] **Step 3: Implement ExpressionJsEmitter**

This converts C# expressions to their JavaScript equivalents using NScript naming conventions (`get_propertyName()`).

```csharp
// Sources/Compiler/RazorSkinParser/CodeGen/ExpressionJsEmitter.cs
using System.Text.RegularExpressions;

namespace NScript.RazorSkin.CodeGen
{
    public static class ExpressionJsEmitter
    {
        /// <summary>
        /// Convert a C# property access like "Model.Name" to a JS getter call like "src.get_name()".
        /// </summary>
        public static string ToJsGetter(string csharpExpression, string sourceParam = "src")
        {
            // Replace "Model." with source param
            var expr = csharpExpression
                .Replace("Model.", sourceParam + ".")
                .Replace("Control.", sourceParam + ".");

            // Convert property accesses to getter calls: .PropertyName → .get_propertyName()
            expr = Regex.Replace(expr, @"\.([A-Z])(\w*?)(?=[.\s\)\]\+\-\*\/\,\;]|$)",
                match =>
                {
                    var propName = match.Groups[1].Value.ToLower() + match.Groups[2].Value;
                    return $".get_{propName}()";
                });

            return expr;
        }

        /// <summary>
        /// Convert a C# property name to NScript JS getter function name.
        /// </summary>
        public static string PropertyToGetterName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return propertyName;
            return "get_" + char.ToLower(propertyName[0]) + propertyName.Substring(1);
        }

        /// <summary>
        /// Convert a C# property name to NScript JS setter function name.
        /// </summary>
        public static string PropertyToSetterName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return propertyName;
            return "set_" + char.ToLower(propertyName[0]) + propertyName.Substring(1);
        }
    }
}
```

- [ ] **Step 4: Implement BinderEmitter**

```csharp
// Sources/Compiler/RazorSkinParser/CodeGen/BinderEmitter.cs
using System.Collections.Generic;
using System.Text;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin.CodeGen
{
    public static class BinderEmitter
    {
        public static string EmitSkinBinderInfo(
            ExpressionBindingNode binding,
            int objectIndex,
            int binderIndex)
        {
            var sb = new StringBuilder();
            var deps = binding.Classification.Dependencies;
            var expr = binding.Classification.CSharpExpression;

            // Getter function
            var getterJs = ExpressionJsEmitter.ToJsGetter(expr);
            sb.Append("SkinBinderInfo_factory(");
            sb.Append($"[function(src) {{ return {getterJs}; }}]");

            // Property names array for live binding
            sb.Append(", [");
            for (int i = 0; i < deps.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append($"\"{deps[i].PropertyName}\"");
            }
            sb.Append("]");

            // Target setter
            var setter = binding.Target switch
            {
                ExpressionTarget.TextContent => "SetTextContent",
                ExpressionTarget.Attribute => "SetAttribute",
                ExpressionTarget.CssClass => "SetClassName",
                ExpressionTarget.Style => "SetStyle",
                _ => "SetTextContent"
            };
            sb.Append($", {setter}");

            // Binder type flags
            var flags = binding.Classification.Mode == BindingMode.OneWay ? "17" : "1"; // ONEWAY|DATACONTEXT or ONETIME|DATACONTEXT
            sb.Append($", {flags}");

            // Object index, binder index
            sb.Append($", {objectIndex}, {binderIndex}");

            // Converter (null), default value
            sb.Append(", null, \"\"");
            sb.Append(")");

            return sb.ToString();
        }
    }
}
```

- [ ] **Step 5: Implement RazorSkinCodeGenerator**

```csharp
// Sources/Compiler/RazorSkinParser/CodeGen/RazorSkinCodeGenerator.cs
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin.CodeGen
{
    public static class RazorSkinCodeGenerator
    {
        public static string Generate(SkinTemplateNode ir)
        {
            var sb = new StringBuilder();

            // Collect all bindings
            var bindings = CollectBindings(ir.Children);
            var htmlContent = CollectHtml(ir.Children);
            int liveBinderCount = bindings.Count(b => b.Classification.Mode == BindingMode.OneWay);

            // Generate factory function
            sb.AppendLine($"var {ir.TemplateName}_var = null;");
            sb.AppendLine();

            // Factory method
            sb.AppendLine($"function {ir.TemplateName}_factory(skinFactory, doc) {{");
            sb.AppendLine("  var domStore, htmlRoot, objStorage;");
            sb.AppendLine($"  if (!(domStore = DocStorageGetter(doc))[0]) {{");
            sb.AppendLine($"    domStore[0] = doc.createElement(\"div\");");
            sb.AppendLine($"    domStore[0].innerHTML = \"{EscapeJs(htmlContent)}\";");

            // Binders array
            if (bindings.Count > 0)
            {
                sb.AppendLine("    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [");
                for (int i = 0; i < bindings.Count; i++)
                {
                    var comma = i < bindings.Count - 1 ? "," : "";
                    sb.AppendLine($"      {BinderEmitter.EmitSkinBinderInfo(bindings[i], i, i)}{comma}");
                }
                sb.AppendLine("    ];");
            }
            else
            {
                sb.AppendLine("    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];");
            }

            sb.AppendLine("  }");
            sb.AppendLine("  htmlRoot = domStore[0].cloneNode(true);");
            sb.AppendLine($"  objStorage = new Array({bindings.Count});");

            // Element path mapping (simplified — real impl uses GetElementFromPath)
            for (int i = 0; i < bindings.Count; i++)
            {
                sb.AppendLine($"  objStorage[{i}] = GetElementFromPath(htmlRoot, [{i + 1}]);");
            }

            sb.AppendLine($"  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, {liveBinderCount}, 0);");
            sb.AppendLine("}");
            sb.AppendLine();

            // Skin getter
            sb.AppendLine($"function {ir.TemplateName}() {{");
            sb.AppendLine($"  if (!{ir.TemplateName}_var)");
            sb.AppendLine($"    {ir.TemplateName}_var = Skin_factory({ir.ControlTypeName}, {ir.ModelTypeName}, {ir.TemplateName}_factory, \"0\");");
            sb.AppendLine($"  return {ir.TemplateName}_var;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static List<ExpressionBindingNode> CollectBindings(List<IRNode> nodes)
        {
            var result = new List<ExpressionBindingNode>();
            foreach (var node in nodes)
            {
                if (node is ExpressionBindingNode binding)
                    result.Add(binding);
                result.AddRange(CollectBindings(node.Children));
            }
            return result;
        }

        private static string CollectHtml(List<IRNode> nodes)
        {
            var sb = new StringBuilder();
            foreach (var node in nodes)
            {
                if (node is HtmlNode html)
                    sb.Append(html.HtmlContent);
                // Expression slots get placeholder elements
                else if (node is ExpressionBindingNode)
                    sb.Append("<span></span>");
            }
            return sb.ToString();
        }

        private static string EscapeJs(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "");
        }
    }
}
```

- [ ] **Step 6: Run tests to verify they pass**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~CodeGenTests" -v n
```

Expected: All 3 tests PASS.

- [ ] **Step 7: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/CodeGen/
git add Test/Compiler/RazorSkinParser.Test/CodeGenTests.cs
git commit -m "Add JS code generator for simple OneTime/OneWay bindings"
```

---

## Task 9: RazorTemplatingPlugin Integration

Wire the Razor pipeline into the NScript compiler alongside the existing XWML plugin.

**Files:**
- Create: `Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs`
- Create: `Sources/Compiler/RazorSkinParser/RazorSkinCompiler.cs`
- Modify: `Sources/Compiler/NScript.Lib/CommandLine.cs:22-26`

- [ ] **Step 1: Create the orchestrator that chains all phases**

```csharp
// Sources/Compiler/RazorSkinParser/RazorSkinCompiler.cs
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin
{
    public static class RazorSkinCompiler
    {
        /// <summary>
        /// Full pipeline: .skin.cshtml source → JavaScript factory code.
        /// </summary>
        public static string Compile(
            string templateName,
            string templateSource,
            string[] additionalCSharpSources = null)
        {
            // Phase 1: Preprocess
            var preprocessed = RazorSkinPreprocessor.Process(templateSource);

            // Phase 2: Razor parse
            var parsed = RazorParserPhase.Parse(templateName, preprocessed.CleanedTemplate);

            // Phase 3: Build IR
            var ir = TemplateIRBuilder.Build(templateName, preprocessed, parsed);

            // Phase 4: Roslyn analysis (refine classifications)
            if (additionalCSharpSources != null)
            {
                RoslynAnalysisPhase.RefineClassifications(
                    ir, parsed.GeneratedCSharp, additionalCSharpSources);
            }

            // Phase 5: Generate JS
            return RazorSkinCodeGenerator.Generate(ir);
        }
    }
}
```

- [ ] **Step 2: Create the plugin class (skeleton)**

```csharp
// Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs
using System;
using System.Collections.Generic;
using System.IO;

namespace NScript.RazorSkin
{
    /// <summary>
    /// NScript compiler plugin that processes .skin.cshtml files.
    /// Implements the same interfaces as XwmlTemplatingPlugin for coexistence.
    /// </summary>
    public class RazorTemplatingPlugin
    {
        // NOTE: This class will implement IMethodConverterPlugin and IRuntimeConverterPlugin
        // once wired into the NScript compiler. For now it provides a static entry point
        // for processing .skin.cshtml files found in a project directory.

        public static bool CanHandle(string templateFileName)
        {
            return templateFileName.EndsWith(".skin.cshtml", StringComparison.OrdinalIgnoreCase);
        }

        public static string CompileTemplate(string filePath, string[] frameworkSources)
        {
            var templateName = Path.GetFileNameWithoutExtension(
                Path.GetFileNameWithoutExtension(filePath)); // Remove .skin.cshtml
            var templateSource = File.ReadAllText(filePath);

            return RazorSkinCompiler.Compile(templateName, templateSource, frameworkSources);
        }
    }
}
```

- [ ] **Step 3: Register the plugin in CommandLine.cs**

Modify `Sources/Compiler/NScript.Lib/CommandLine.cs` to add the Razor plugin alongside XWML. This requires adding a project reference to `RazorSkinParser` from `NScript.Lib`. The exact integration depends on the `IConverterPlugin` interface — for now, add a comment placeholder at the registration site:

```csharp
// In CommandLine.cs, lines 22-26, add after XwmlTemplatingPlugin:
var plugins = new List<IConverterPlugin>()
{
    new XwmlTemplatingPlugin(),
    // new RazorTemplatingPlugin(), // TODO: Implement IConverterPlugin interfaces
    new TestGenerator()
};
```

**Note:** Full `IConverterPlugin` implementation requires deeper integration with the NScript converter pipeline. This step registers the skeleton; Task 10 completes the interface implementation.

- [ ] **Step 4: Add project reference from NScript.Lib to RazorSkinParser**

Modify `Sources/Compiler/NScript.Lib/NScript.csproj` to add:
```xml
<ProjectReference Include="..\RazorSkinParser\RazorSkinParser.csproj" />
```

- [ ] **Step 5: Verify the full solution builds**

```bash
dotnet build NScript_Full.sln -c Release
```

Expected: Build succeeds.

- [ ] **Step 6: Commit**

```bash
git add Sources/Compiler/RazorSkinParser/RazorSkinCompiler.cs \
       Sources/Compiler/RazorSkinParser/RazorTemplatingPlugin.cs \
       Sources/Compiler/NScript.Lib/CommandLine.cs \
       Sources/Compiler/NScript.Lib/NScript.csproj
git commit -m "Add RazorTemplatingPlugin skeleton and compiler orchestrator"
```

---

## Task 10: End-to-End Integration Test

Full pipeline test: `.skin.cshtml` → preprocess → parse → IR → analyze → JS output.

**Files:**
- Create: `Test/Compiler/RazorSkinParser.Test/EndToEndTests.cs`
- Create: `Test/Compiler/RazorSkinParser.Test/Templates/TextBinding.skin.cshtml`
- Create: `Test/Compiler/RazorSkinParser.Test/Templates/ComputedExpression.skin.cshtml`
- Create: `Test/Compiler/RazorSkinParser.Test/Templates/ConditionalBlock.skin.cshtml`
- Create: `Test/Compiler/RazorSkinParser.Test/Templates/ForeachBlock.skin.cshtml`

- [ ] **Step 1: Create test templates**

```razor
@* Templates/TextBinding.skin.cshtml *@
@model TestVM

<div>@Model.Name</div>
```

```razor
@* Templates/ComputedExpression.skin.cshtml *@
@model TestVM

<span>@(Model.Price * Model.Quantity)</span>
```

```razor
@* Templates/ConditionalBlock.skin.cshtml *@
@model TestVM

@if (Model.IsActive)
{
    <div>Active</div>
}
else
{
    <div>Inactive</div>
}
```

```razor
@* Templates/ForeachBlock.skin.cshtml *@
@model TestVM

@foreach (var item in Model.Items)
{
    <li>@item.Name</li>
}
```

- [ ] **Step 2: Write end-to-end tests**

```csharp
// Test/Compiler/RazorSkinParser.Test/EndToEndTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class EndToEndTests
    {
        private const string FrameworkStubs = @"
namespace Sunlight.Framework.Observables
{
    public interface INotifyPropertyChanged { }
    public class ObservableObject : INotifyPropertyChanged
    {
        protected void FirePropertyChanged(string name) { }
    }
    public interface IObservableCollection { }
    public class ObservableCollection<T> : ObservableObject, IObservableCollection { }
}";

        private const string TestVMSource = @"
using Sunlight.Framework.Observables;
public class TestVM : ObservableObject
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public bool IsActive { get; set; }
    public ObservableCollection<ItemVM> Items { get; set; }
}
public class ItemVM : ObservableObject
{
    public string Name { get; set; }
}";

        [TestMethod]
        public void SimpleTextBindingProducesValidJs()
        {
            var template = "@model TestVM\n\n<div>@Model.Name</div>";
            var js = RazorSkinCompiler.Compile(
                "TextBinding", template,
                new[] { FrameworkStubs, TestVMSource });

            js.Should().Contain("TextBinding_factory");
            js.Should().Contain("function TextBinding()");
            js.Should().Contain("get_name");
            js.Should().Contain("\"Name\"");
            js.Should().Contain("SkinInstance");
        }

        [TestMethod]
        public void ComputedExpressionBindsMultipleProperties()
        {
            var template = "@model TestVM\n\n<span>@(Model.Price * Model.Quantity)</span>";
            var js = RazorSkinCompiler.Compile(
                "ComputedExpr", template,
                new[] { FrameworkStubs, TestVMSource });

            js.Should().Contain("get_price");
            js.Should().Contain("get_quantity");
            js.Should().Contain("\"Price\"");
            js.Should().Contain("\"Quantity\"");
        }

        [TestMethod]
        public void FullPipelineProducesNonEmptyJs()
        {
            var template = "@model TestVM\n@control UISkinableElement\n\n<div>\n    <h1>@Model.Name</h1>\n    <span>@(Model.Price * Model.Quantity)</span>\n</div>";
            var js = RazorSkinCompiler.Compile(
                "OrderSkin", template,
                new[] { FrameworkStubs, TestVMSource });

            js.Should().NotBeNullOrEmpty();
            js.Should().Contain("OrderSkin_factory");
            js.Should().Contain("OrderSkin_var");
            js.Should().Contain("function OrderSkin()");
        }
    }
}
```

- [ ] **Step 3: Run the tests**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj --filter "ClassName~EndToEndTests" -v n
```

Expected: All 3 tests PASS. If any fail, debug by inspecting the generated C# from Razor (print `parsed.GeneratedCSharp`) and the IR tree structure.

- [ ] **Step 4: Run full test suite**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj -v n
```

Expected: All tests PASS across all test classes.

- [ ] **Step 5: Commit**

```bash
git add Test/Compiler/RazorSkinParser.Test/EndToEndTests.cs \
       Test/Compiler/RazorSkinParser.Test/Templates/
git commit -m "Add end-to-end integration tests for Razor skin pipeline"
```

---

## Task 11: Runtime Binder — MultiDependencyBinder

New runtime binder for expressions watching multiple observable properties. Lives in the Framework project (compiled by NScript custom compiler to JS).

**Files:**
- Create: `Sources/Framework/Sunlight.Framework.UI/Helpers/MultiDependencyBinder.cs`

**Reference:** `Sources/Framework/Sunlight.Framework.UI/Helpers/LiveBinder.cs` — the existing single-property binder.

- [ ] **Step 1: Implement MultiDependencyBinder**

```csharp
// Sources/Framework/Sunlight.Framework.UI/Helpers/MultiDependencyBinder.cs
namespace Sunlight.Framework.UI.Helpers
{
    using System;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Runtime binder that watches N observable properties on a source and
    /// recomputes a value via a getter function when any dependency changes.
    /// Used for Razor template computed expressions like @(Model.Price * Model.Quantity).
    /// </summary>
    public class MultiDependencyBinder
    {
        private object source;
        private object target;
        private Func<object, object> getter;
        private Action<object, object> setter;
        private string[] propertyNames;
        private bool isActive;
        private bool updating;
        private Action<INotifyPropertyChanged, string> propertyChangedCallback;

        public MultiDependencyBinder(
            Func<object, object> getter,
            Action<object, object> setter,
            string[] propertyNames)
        {
            this.getter = getter;
            this.setter = setter;
            this.propertyNames = propertyNames;
            this.propertyChangedCallback = this.OnSourcePropertyChanged;
        }

        public object Source
        {
            get { return this.source; }
            set
            {
                if (this.source == value) return;

                if (this.isActive && this.source is INotifyPropertyChanged oldNotify)
                {
                    UnregisterListeners(oldNotify);
                }

                this.source = value;

                if (this.isActive && this.source is INotifyPropertyChanged newNotify)
                {
                    RegisterListeners(newNotify);
                    FlowValue();
                }
            }
        }

        public object Target
        {
            get { return this.target; }
            set { this.target = value; }
        }

        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                if (this.isActive == value) return;
                this.isActive = value;

                if (value)
                {
                    if (this.source is INotifyPropertyChanged notify)
                    {
                        RegisterListeners(notify);
                    }
                    FlowValue();
                }
                else
                {
                    if (this.source is INotifyPropertyChanged notify)
                    {
                        UnregisterListeners(notify);
                    }
                }
            }
        }

        private void RegisterListeners(INotifyPropertyChanged notify)
        {
            for (int i = 0; i < this.propertyNames.Length; i++)
            {
                notify.AddPropertyChangedListener(
                    this.propertyNames[i],
                    this.propertyChangedCallback);
            }
        }

        private void UnregisterListeners(INotifyPropertyChanged notify)
        {
            for (int i = 0; i < this.propertyNames.Length; i++)
            {
                notify.RemovePropertyChangedListener(
                    this.propertyNames[i],
                    this.propertyChangedCallback);
            }
        }

        private void OnSourcePropertyChanged(INotifyPropertyChanged sender, string propertyName)
        {
            if (this.updating) return;
            FlowValue();
        }

        private void FlowValue()
        {
            if (this.source == null || this.target == null) return;

            this.updating = true;
            try
            {
                var value = this.getter(this.source);
                this.setter(this.target, value);
            }
            finally
            {
                this.updating = false;
            }
        }

        public void Dispose()
        {
            this.IsActive = false;
            this.source = null;
            this.target = null;
        }
    }
}
```

- [ ] **Step 2: Verify framework project builds**

```bash
dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj
```

Expected: Build succeeds.

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/MultiDependencyBinder.cs
git commit -m "Add MultiDependencyBinder for computed expression bindings"
```

---

## Task 12: Runtime Binder — ConditionalBinder

Reactive `@if`/`@else` DOM fragment swapping at runtime.

**Files:**
- Create: `Sources/Framework/Sunlight.Framework.UI/Helpers/ConditionalBinder.cs`

- [ ] **Step 1: Implement ConditionalBinder**

```csharp
// Sources/Framework/Sunlight.Framework.UI/Helpers/ConditionalBinder.cs
namespace Sunlight.Framework.UI.Helpers
{
    using System;
    using System.Web.Html;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Runtime binder for reactive @if/@else blocks.
    /// Watches a boolean condition on the DataContext and swaps DOM fragments.
    /// </summary>
    public class ConditionalBinder
    {
        private object source;
        private Func<object, bool> conditionGetter;
        private string conditionPropertyName;
        private Element parentElement;
        private Element truePlaceholder;
        private Element falsePlaceholder;
        private Element trueTemplate;
        private Element falseTemplate;
        private Element currentElement;
        private bool isActive;
        private bool currentCondition;
        private Action<INotifyPropertyChanged, string> callback;

        public ConditionalBinder(
            Func<object, bool> conditionGetter,
            string conditionPropertyName,
            Element parentElement,
            Element trueTemplate,
            Element falseTemplate)
        {
            this.conditionGetter = conditionGetter;
            this.conditionPropertyName = conditionPropertyName;
            this.parentElement = parentElement;
            this.trueTemplate = trueTemplate;
            this.falseTemplate = falseTemplate;
            this.callback = this.OnPropertyChanged;
        }

        public object Source
        {
            get { return this.source; }
            set
            {
                if (this.source == value) return;

                if (this.isActive && this.source is INotifyPropertyChanged oldNotify)
                {
                    oldNotify.RemovePropertyChangedListener(
                        this.conditionPropertyName, this.callback);
                }

                this.source = value;

                if (this.isActive && this.source is INotifyPropertyChanged newNotify)
                {
                    newNotify.AddPropertyChangedListener(
                        this.conditionPropertyName, this.callback);
                    Evaluate();
                }
            }
        }

        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                if (this.isActive == value) return;
                this.isActive = value;

                if (value)
                {
                    if (this.source is INotifyPropertyChanged notify)
                    {
                        notify.AddPropertyChangedListener(
                            this.conditionPropertyName, this.callback);
                    }
                    Evaluate();
                }
                else
                {
                    if (this.source is INotifyPropertyChanged notify)
                    {
                        notify.RemovePropertyChangedListener(
                            this.conditionPropertyName, this.callback);
                    }
                    RemoveCurrent();
                }
            }
        }

        private void OnPropertyChanged(INotifyPropertyChanged sender, string name)
        {
            Evaluate();
        }

        private void Evaluate()
        {
            if (this.source == null) return;

            var condition = this.conditionGetter(this.source);
            if (condition == this.currentCondition && this.currentElement != null)
                return;

            this.currentCondition = condition;
            RemoveCurrent();

            var template = condition ? this.trueTemplate : this.falseTemplate;
            if (template != null)
            {
                this.currentElement = template.CloneNode(true);
                this.parentElement.AppendChild(this.currentElement);
            }
        }

        private void RemoveCurrent()
        {
            if (this.currentElement != null)
            {
                this.currentElement.Remove();
                this.currentElement = null;
            }
        }

        public void Dispose()
        {
            this.IsActive = false;
            RemoveCurrent();
            this.source = null;
        }
    }
}
```

- [ ] **Step 2: Verify framework project builds**

```bash
dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj
```

Expected: Build succeeds.

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/ConditionalBinder.cs
git commit -m "Add ConditionalBinder for reactive @if/@else DOM swapping"
```

---

## Task 13: Runtime Binder — CollectionBinder

Reactive `@foreach` with incremental DOM updates.

**Files:**
- Create: `Sources/Framework/Sunlight.Framework.UI/Helpers/CollectionBinder.cs`

- [ ] **Step 1: Implement CollectionBinder**

```csharp
// Sources/Framework/Sunlight.Framework.UI/Helpers/CollectionBinder.cs
namespace Sunlight.Framework.UI.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Html;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Runtime binder for reactive @foreach on ObservableCollections.
    /// Manages incremental DOM updates: add, remove, replace, reset.
    /// </summary>
    public class CollectionBinder
    {
        private IObservableCollection collection;
        private IObservableCollection attachedCollection;
        private Element parentElement;
        private Element itemTemplate;
        private Func<Element, object, Element> itemFactory;
        private List<Element> itemElements = new List<Element>();
        private bool isActive;

        public CollectionBinder(
            Element parentElement,
            Element itemTemplate,
            Func<Element, object, Element> itemFactory)
        {
            this.parentElement = parentElement;
            this.itemTemplate = itemTemplate;
            this.itemFactory = itemFactory;
        }

        public IObservableCollection Collection
        {
            get { return this.collection; }
            set
            {
                if (this.collection == value) return;

                DetachCollection();
                this.collection = value;

                if (this.isActive && this.collection != null)
                {
                    AttachCollection();
                    Reset();
                }
            }
        }

        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                if (this.isActive == value) return;
                this.isActive = value;

                if (value && this.collection != null)
                {
                    AttachCollection();
                    Reset();
                }
                else
                {
                    DetachCollection();
                    ClearAll();
                }
            }
        }

        private void AttachCollection()
        {
            if (this.attachedCollection != null) return;
            this.attachedCollection = this.collection;
            this.attachedCollection.CollectionChanged += OnCollectionChanged;
        }

        private void DetachCollection()
        {
            if (this.attachedCollection == null) return;
            this.attachedCollection.CollectionChanged -= OnCollectionChanged;
            this.attachedCollection = null;
        }

        private void OnCollectionChanged(
            INotifyCollectionChanged sender,
            CollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case CollectionChangedAction.Add:
                    OnAdd(args.ChangeIndex, args.NewItems);
                    break;
                case CollectionChangedAction.Remove:
                    OnRemove(args.ChangeIndex, args.OldItems.Count);
                    break;
                case CollectionChangedAction.Replace:
                    OnReplace(args.ChangeIndex, args.NewItems);
                    break;
                case CollectionChangedAction.Reset:
                    Reset();
                    break;
            }
        }

        private void OnAdd(int index, IList newItems)
        {
            Element insertBefore = index < this.itemElements.Count
                ? this.itemElements[index]
                : null;

            for (int i = 0; i < newItems.Count; i++)
            {
                var element = CreateItemElement(newItems[i]);
                if (insertBefore != null)
                {
                    this.parentElement.InsertBefore(element, insertBefore);
                    this.itemElements.Insert(index + i, element);
                }
                else
                {
                    this.parentElement.AppendChild(element);
                    this.itemElements.Add(element);
                }
            }
        }

        private void OnRemove(int index, int count)
        {
            for (int i = count - 1; i >= 0; i--)
            {
                var idx = index + i;
                if (idx < this.itemElements.Count)
                {
                    this.itemElements[idx].Remove();
                    this.itemElements.RemoveAt(idx);
                }
            }
        }

        private void OnReplace(int index, IList newItems)
        {
            for (int i = 0; i < newItems.Count; i++)
            {
                var idx = index + i;
                if (idx < this.itemElements.Count)
                {
                    var oldElement = this.itemElements[idx];
                    var newElement = CreateItemElement(newItems[i]);
                    this.parentElement.InsertBefore(newElement, oldElement);
                    oldElement.Remove();
                    this.itemElements[idx] = newElement;
                }
            }
        }

        private void Reset()
        {
            ClearAll();

            if (this.attachedCollection == null) return;

            int count = this.attachedCollection.Count;
            for (int i = 0; i < count; i++)
            {
                var element = CreateItemElement(this.attachedCollection[i]);
                this.parentElement.AppendChild(element);
                this.itemElements.Add(element);
            }
        }

        private void ClearAll()
        {
            for (int i = this.itemElements.Count - 1; i >= 0; i--)
            {
                this.itemElements[i].Remove();
            }
            this.itemElements.Clear();
        }

        private Element CreateItemElement(object dataItem)
        {
            var element = this.itemTemplate.CloneNode(true);
            return this.itemFactory(element, dataItem);
        }

        public void Dispose()
        {
            DetachCollection();
            ClearAll();
            this.collection = null;
        }
    }
}
```

- [ ] **Step 2: Verify framework project builds**

```bash
dotnet build Sources/Framework/Sunlight.Framework.UI/Sunlight.Framework.UI.csproj
```

Expected: Build succeeds.

- [ ] **Step 3: Commit**

```bash
git add Sources/Framework/Sunlight.Framework.UI/Helpers/CollectionBinder.cs
git commit -m "Add CollectionBinder for reactive @foreach incremental DOM updates"
```

---

## Task 14: Full Solution Build + All Tests

Verify everything compiles and tests pass together.

- [ ] **Step 1: Build the full solution**

```bash
cd b:/sources/NScript
dotnet build NScript_Full.sln -c Release
```

Expected: Build succeeds with no errors.

- [ ] **Step 2: Run all RazorSkinParser tests**

```bash
dotnet test Test/Compiler/RazorSkinParser.Test/RazorSkinParser.Test.csproj -v n
```

Expected: All tests pass.

- [ ] **Step 3: Run existing tests to verify no regressions**

```bash
dotnet test NScript_Full.sln -c Release
```

Expected: No regressions in existing tests.

- [ ] **Step 4: Commit any fixes needed**

If any tests fail, fix and commit.

---

## Future Tasks (Not in This Plan)

These are documented for the next iteration but not implemented in this plan:

1. **JS code generation for events** — Method reference and inline lambda compilation
2. **JS code generation for @functions** — Static/context-scoped JS helpers
3. **JS code generation for reactive blocks** — ConditionalBinder/CollectionBinder setup in factory
4. **Sub-control code generation** — PascalCase tag → UIElement factory call
5. **Full IConverterPlugin implementation** — Wire RazorTemplatingPlugin into NScript converter
6. **Performance benchmarks** — Compare Razor bindings vs XWML bindings
7. **XWML migration tooling** — Automated conversion from `.html` templates to `.skin.cshtml`

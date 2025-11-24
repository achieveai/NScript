# NScript - Write JavaScript in C#

NScript is a C# to JavaScript transpiler that allows you to write client-side web applications entirely in C#. It provides a modern MVVM framework with data binding, skinnable UI controls, and a comprehensive standard library, all while compiling to efficient JavaScript.

## What is NScript?

NScript is a compiler and framework that enables developers to:

- **Write JavaScript in C#**: Use familiar C# syntax and semantics instead of JavaScript
- **Build MVVM applications**: Use the Sunlight.Framework for Model-View-ViewModel pattern
- **Use data binding**: OneTime, OneWay, and TwoWay binding modes for UI synchronization
- **Create templated views**: Combine C# logic with HTML templates
- **Access the DOM**: Use strongly-typed C# APIs instead of JavaScript APIs
- **Leverage C# features**: Use generics, LINQ, async/await, null-coalescing operators, and more (C# 8 features supported)

## Getting Started

### Prerequisites

- Visual Studio 2017 or later
- .NET 8.0 SDK
- ANTLR4Code extension for Visual Studio
- Wix Toolset 3.x (from [http://wixtoolset.org/releases/])

### Installation

1. **Install Visual Studio Extensions**:
   - Install `ANTLR4Code` extension from Visual Studio Marketplace
   - Install `WiX Toolset Visual Studio extension` from Visual Studio Marketplace

2. **Install Wix Toolset**:
   - Download and install from [http://wixtoolset.org/releases/]

3. **Build the Compiler**:

   ```bash
   dotnet build NScript_Full.sln -c Release
   ```

### Building NuGet Packages

To generate NuGet packages with version 1.0.4:

1. **Enable package generation** in `Directory.Build.props`:

   ```xml
   <GenerateNScriptPackages>true</GenerateNScriptPackages>
   ```

2. **Build the solution**:

   ```bash
   dotnet build NScript_Full.sln -c Release
   ```

3. **Publish to NuGet** (from NScriptToolSet directory):

   ```powershell
   # Run the provided PowerShell script
   .\Publish-Packages.ps1 -ApiKey "YOUR_NUGET_API_KEY"
   ```

## How to Use NScript

### Basic Example: Hello World

```csharp
using System.Web.Html;

[EntryPoint]
static class Program
{
    static void Main()
    {
        var container = Document.GetElementById("app");
        container.InnerHTML = "<h1>Hello World from C#!</h1>";
    }
}
```

### MVVM with Data Binding

Create a ViewModel:

```csharp
public class MyViewModel : ObservableObject
{
    private string _name = "";

    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }
}
```

Create a View with binding:

```html
<div>
    <input type="text" value="{Name}" />
    <p>Hello, {Name}!</p>
</div>
```

### Project Structure

A typical NScript application includes:

```text
MyApp/
├── Program.cs              # Entry point with [EntryPoint] attribute
├── Views/                  # MVVM Views
│   ├── MyView.cs
│   └── MyView.html         # HTML templates
├── ViewModels/            # Observable ViewModels
│   └── MyViewModel.cs
└── MyApp.csproj
```

## Supported C# Features

- Classes, Interfaces, Properties
- Methods, Constructors
- Generics, Collections (List<T>, Dictionary<K,V>)
- LINQ
- Lambda expressions
- async/await
- Null-coalescing operators
- Pattern matching
- Indices and ranges
- C# 8 features

## Limitations

The following C# features are not supported:

- `dynamic` keyword
- `yield break` / `yield return`
- Reflection (limited support)
- P/Invoke
- Some advanced C# features are still being implemented

For the latest feature support status, see [csharp8-todos.md](csharp8-todos.md).

## Packages

The NScript project generates 10 NuGet packages:

### Compiler Tools

- `Mcqdb.NScript.Sdk` - MSBuild SDK for compiling NScript projects
- `Mcqdb.NScript.Cs2Jsc` - Command-line C# to JavaScript compiler tool

### Runtime Libraries

- `Mcqdb.NScript.MsCorlib` - Core library (Array, String, Object, etc.)
- `Mcqdb.NScript.System.Core` - Extended collections and utilities
- `Mcqdb.NScript.System.Web` - DOM and web APIs
- `Mcqdb.NScript.System.Web.Html` - HTML element abstractions
- `Mcqdb.NScript.Microsoft.CSharp` - C# language features
- `Mcqdb.NScript.Sunlight.Framework` - MVVM framework
- `Mcqdb.NScript.Sunlight.Framework.UI` - UI controls and skinning
- `Mcqdb.NScript.SunlightUnit` - Unit testing framework

## Framework Architecture

### Sunlight.Framework

The MVVM framework provides:

- **ObservableObject**: Base class for ViewModels with property change notification
- **ObservableCollection<T>**: Bindable collection
- **ICommand**: Command pattern implementation
- **Data Binding**: OneTime, OneWay, TwoWay modes
- **Skin System**: Skinnable UI controls with customizable appearance

### System.Web.Html

Access the DOM with strongly-typed APIs:

```csharp
var element = Document.GetElementById("myId");
element.InnerHTML = "<p>Content</p>";
element.ClassName = "active";
```

## Building From Source

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2017 or later
- ANTLR4Code extension
- Wix Toolset

### Build Steps

```bash
# Restore dependencies
dotnet restore NScript_Full.sln

# Build in Release mode
dotnet build NScript_Full.sln -c Release

# Run tests
dotnet test NScript_Full.sln -c Release

# Generate NuGet packages
# (See "Building NuGet Packages" section above)
```

## Directory Structure

```text
cs2jsc/
├── Sources/
│   ├── Compiler/          # C# to JavaScript compiler
│   │   ├── NScript.Csc.Lib/
│   │   ├── NScript.CLR/
│   │   ├── CssParser/
│   │   └── ...
│   └── Framework/         # Runtime libraries
│       ├── mscorlib/
│       ├── System.Core/
│       ├── System.Web/
│       ├── Sunlight.Framework/
│       └── ...
├── Test/                  # Unit and integration tests
├── NScriptToolSet/        # Build output and packages
├── global.json            # SDK configuration
└── Directory.Build.props  # Build properties
```

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Related Projects

- [Sunlight.Framework](https://github.com/achieveai/NScript) - MVVM framework for NScript
- [NScript SDK](https://github.com/achieveai/NScript) - Official SDK and tools

## References

- C# to JavaScript compiler technology
- MVVM pattern implementation
- DOM manipulation and web APIs
- Data binding architecture

## Support

For issues, questions, or feedback:

- Check existing GitHub issues
- Create a new GitHub issue with detailed information
- Include code examples and error messages

---

**Version**: 1.0.4
**Last Updated**: November 2025

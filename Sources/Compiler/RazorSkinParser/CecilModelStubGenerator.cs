using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NScript.CLR;
using NScript.RazorSkin.CodeGen;
using Serilog;

namespace NScript.RazorSkin
{
    /// <summary>
    /// Generates C# source stubs for model types referenced by @model in Razor templates.
    /// Uses Cecil type information to produce minimal class declarations with properties,
    /// so the Roslyn analysis phase can detect observable properties and promote bindings
    /// from OneTime to OneWay.
    /// </summary>
    public class CecilModelStubGenerator
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        private readonly CecilTypeHelper _typeHelper;

        public CecilModelStubGenerator(ClrContext clrContext)
        {
            _typeHelper = new CecilTypeHelper(clrContext);
        }

        /// <summary>
        /// Generates a C# source stub for the model type referenced by @model in the template.
        /// Uses Cecil type information to produce a minimal class declaration with properties,
        /// so the Roslyn analysis phase can detect observable properties and promote bindings
        /// from OneTime to OneWay.
        /// </summary>
        public string GenerateModelTypeStub(string templateSource)
        {
            // Extract @model type name from the template source
            string modelTypeName = null;
            foreach (var line in templateSource.Split('\n'))
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("@model "))
                {
                    modelTypeName = trimmed.Substring("@model ".Length).Trim();
                    break;
                }
            }

            if (string.IsNullOrEmpty(modelTypeName))
                return null;

            // Find the type in Cecil
            var typeDef = _typeHelper.FindTypeDefinition(modelTypeName);
            if (typeDef == null)
            {
                Log.Debug("Could not find Cecil type {TypeName} for model stub generation", modelTypeName);
                return null;
            }

            // Determine base class
            var baseTypeName = "object";
            var currentBase = typeDef.BaseType;
            while (currentBase != null)
            {
                if (currentBase.FullName == "Sunlight.Framework.Observables.ObservableObject")
                {
                    baseTypeName = "Sunlight.Framework.Observables.ObservableObject";
                    break;
                }
                try { currentBase = currentBase.Resolve()?.BaseType; }
                catch (Mono.Cecil.AssemblyResolutionException) { break; }
                catch (System.Exception) { break; }
            }

            // Build namespace and class declaration
            var ns = typeDef.Namespace;
            var className = typeDef.Name;
            var sb = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(ns))
            {
                sb.AppendLine($"namespace {ns} {{");
            }

            sb.AppendLine($"  public class {className} : {baseTypeName} {{");

            // Generate property stubs
            foreach (var prop in typeDef.Properties)
            {
                var propTypeName = MapCecilTypeToSimpleName(prop.PropertyType);
                if (prop.GetMethod != null && prop.SetMethod != null)
                {
                    sb.AppendLine($"    public {propTypeName} {prop.Name} {{ get; set; }}");
                }
                else if (prop.GetMethod != null)
                {
                    sb.AppendLine($"    public {propTypeName} {prop.Name} {{ get; }}");
                }
            }

            // Generate method stubs (for event handlers)
            foreach (var method in typeDef.Methods)
            {
                if (!method.IsPublic || method.IsConstructor || method.IsGetter || method.IsSetter)
                    continue;
                var retType = MapCecilTypeToSimpleName(method.ReturnType);
                var paramStrs = method.Parameters
                    .Select(p => $"{MapCecilTypeToSimpleName(p.ParameterType)} {p.Name}");
                sb.AppendLine($"    public {retType} {method.Name}({string.Join(", ", paramStrs)}) {{ }}");
            }

            sb.AppendLine("  }");

            if (!string.IsNullOrEmpty(ns))
            {
                sb.AppendLine("}");
            }

            // Generate stubs for types referenced in collection properties AFTER
            // closing the main namespace, so they get their own proper namespace blocks.
            var referencedTypes = new HashSet<string>();
            foreach (var prop in typeDef.Properties)
            {
                if (prop.PropertyType is GenericInstanceType genPropType)
                {
                    foreach (var arg in genPropType.GenericArguments)
                    {
                        if (!arg.IsPrimitive && arg.FullName != "System.String" && arg.FullName != "System.Object")
                        {
                            referencedTypes.Add(arg.FullName);
                        }
                    }
                }
            }

            foreach (var refTypeName in referencedTypes)
            {
                GenerateReferencedTypeStub(sb, refTypeName);
            }

            var stub = sb.ToString();
            Log.Debug("Generated model type stub for {TypeName}: {StubLength} chars, base={BaseType}",
                modelTypeName, stub.Length, baseTypeName);
            return stub;
        }

        private void GenerateReferencedTypeStub(
            System.Text.StringBuilder sb,
            string fullTypeName)
        {
            var refTypeDef = _typeHelper.FindTypeDefinition(fullTypeName);
            if (refTypeDef == null) return;

            // Determine base class for the referenced type
            var refBaseType = "object";
            var refBase = refTypeDef.BaseType;
            while (refBase != null)
            {
                if (refBase.FullName == "Sunlight.Framework.Observables.ObservableObject")
                {
                    refBaseType = "Sunlight.Framework.Observables.ObservableObject";
                    break;
                }
                try { refBase = refBase.Resolve()?.BaseType; }
                catch (Mono.Cecil.AssemblyResolutionException) { break; }
                catch (System.Exception) { break; }
            }

            // If the type is in a different namespace, wrap in its own namespace block
            var refNs = refTypeDef.Namespace;
            var refClassName = refTypeDef.Name;
            bool needsNamespaceClose = false;

            // Only open a namespace if it differs from what's already open
            if (!string.IsNullOrEmpty(refNs))
            {
                sb.AppendLine($"  namespace {refNs} {{");
                needsNamespaceClose = true;
            }

            sb.AppendLine($"    public class {refClassName} : {refBaseType} {{");

            foreach (var prop in refTypeDef.Properties)
            {
                var propTypeName = MapCecilTypeToSimpleName(prop.PropertyType);
                if (prop.GetMethod != null && prop.SetMethod != null)
                    sb.AppendLine($"      public {propTypeName} {prop.Name} {{ get; set; }}");
                else if (prop.GetMethod != null)
                    sb.AppendLine($"      public {propTypeName} {prop.Name} {{ get; }}");
            }

            sb.AppendLine("    }");

            if (needsNamespaceClose)
                sb.AppendLine("  }");
        }

        /// <summary>
        /// Maps a Cecil TypeReference to a simple C# type name for stub generation.
        /// </summary>
        public static string MapCecilTypeToSimpleName(TypeReference typeRef)
        {
            if (typeRef == null) return "object";

            switch (typeRef.FullName)
            {
                case "System.String": return "string";
                case "System.Int32": return "int";
                case "System.Boolean": return "bool";
                case "System.Double": return "double";
                case "System.Single": return "float";
                case "System.Int64": return "long";
                case "System.Decimal": return "decimal";
                case "System.Object": return "object";
                case "System.Void": return "void";
            }

            // Handle generic types like ObservableCollection<RazorItemVM>
            if (typeRef is GenericInstanceType genType)
            {
                var baseName = genType.ElementType.FullName;
                // Strip arity suffix (e.g., ObservableCollection`1 -> ObservableCollection)
                var arityIdx = baseName.IndexOf('`');
                if (arityIdx >= 0) baseName = baseName.Substring(0, arityIdx);
                var args = string.Join(", ", genType.GenericArguments
                    .Select(a => MapCecilTypeToSimpleName(a)));
                return $"{baseName}<{args}>";
            }

            // For other non-primitive types, return the full type name
            return typeRef.FullName;
        }
    }
}

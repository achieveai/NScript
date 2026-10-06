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

        private const string ObservableObjectFullName = "Sunlight.Framework.Observables.ObservableObject";

        /// <summary>
        /// Generates C# source stubs for the model type referenced by @model in the template and
        /// for every type the template can reach from it through properties: the element types of
        /// collection properties and any observable type a property is declared as, transitively
        /// (<c>Model.SelectedTodo.SubTasks</c> needs the stub of <c>SelectedTodo</c>'s type so the
        /// element type of <c>SubTasks</c> can be found). Non-observable object-typed properties
        /// are not followed, which keeps DOM facades and services out of the analysis compilation.
        /// The Roslyn analysis phase uses the stubs to detect observable properties, type loop
        /// variables and promote bindings from OneTime to OneWay.
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

            var sb = new System.Text.StringBuilder();
            var stubbed = new HashSet<string> { typeDef.FullName };
            var pending = new Queue<TypeDefinition>();

            AppendTypeStub(sb, typeDef, includeMethods: true);
            EnqueueReferencedTypes(typeDef, stubbed, pending);

            while (pending.Count > 0)
            {
                var referenced = pending.Dequeue();
                AppendTypeStub(sb, referenced, includeMethods: false);
                EnqueueReferencedTypes(referenced, stubbed, pending);
            }

            var stub = sb.ToString();
            Log.Debug("Generated model type stub for {TypeName}: {StubLength} chars, {TypeCount} types",
                modelTypeName, stub.Length, stubbed.Count);
            return stub;
        }

        /// <summary>
        /// Appends one class stub, in its own namespace block, with its public properties and
        /// (for the model type, whose methods are event handlers) its public methods.
        /// </summary>
        private static void AppendTypeStub(System.Text.StringBuilder sb, TypeDefinition typeDef, bool includeMethods)
        {
            var baseTypeName = IsObservableObjectDerived(typeDef) ? ObservableObjectFullName : "object";
            var ns = typeDef.Namespace;

            if (!string.IsNullOrEmpty(ns))
                sb.AppendLine($"namespace {ns} {{");

            sb.AppendLine($"  public class {typeDef.Name} : {baseTypeName} {{");

            foreach (var prop in typeDef.Properties)
            {
                var propTypeName = MapCecilTypeToSimpleName(prop.PropertyType);
                if (prop.GetMethod != null && prop.SetMethod != null)
                    sb.AppendLine($"    public {propTypeName} {prop.Name} {{ get; set; }}");
                else if (prop.GetMethod != null)
                    sb.AppendLine($"    public {propTypeName} {prop.Name} {{ get; }}");
            }

            if (includeMethods)
            {
                foreach (var method in typeDef.Methods)
                {
                    if (!method.IsPublic || method.IsConstructor || method.IsGetter || method.IsSetter)
                        continue;
                    var retType = MapCecilTypeToSimpleName(method.ReturnType);
                    var paramStrs = method.Parameters
                        .Select(p => $"{MapCecilTypeToSimpleName(p.ParameterType)} {p.Name}");
                    sb.AppendLine($"    public {retType} {method.Name}({string.Join(", ", paramStrs)}) {{ }}");
                }
            }

            sb.AppendLine("  }");

            if (!string.IsNullOrEmpty(ns))
                sb.AppendLine("}");
        }

        /// <summary>
        /// Queues the types a template can reach from <paramref name="typeDef"/>'s properties:
        /// every generic argument of a property type (collection element types, as before) and
        /// every observable property type (a chained path hops through it). Each type is queued once.
        /// </summary>
        private void EnqueueReferencedTypes(
            TypeDefinition typeDef, HashSet<string> stubbed, Queue<TypeDefinition> pending)
        {
            foreach (var prop in typeDef.Properties)
            {
                if (prop.PropertyType is GenericInstanceType genPropType)
                {
                    foreach (var arg in genPropType.GenericArguments)
                        EnqueueReferencedType(arg, requireObservable: false, stubbed, pending);
                }
                else
                {
                    EnqueueReferencedType(prop.PropertyType, requireObservable: true, stubbed, pending);
                }
            }
        }

        private void EnqueueReferencedType(
            TypeReference reference, bool requireObservable,
            HashSet<string> stubbed, Queue<TypeDefinition> pending)
        {
            if (reference == null || reference.IsPrimitive || reference.IsGenericParameter
                || reference.FullName == "System.String" || reference.FullName == "System.Object"
                || stubbed.Contains(reference.FullName))
                return;

            var referenced = _typeHelper.FindTypeDefinition(reference.FullName);
            if (referenced == null) return;
            if (requireObservable && !IsObservableObjectDerived(referenced)) return;

            stubbed.Add(reference.FullName);
            pending.Enqueue(referenced);
        }

        private static bool IsObservableObjectDerived(TypeDefinition typeDef)
        {
            var currentBase = typeDef.BaseType;
            while (currentBase != null)
            {
                if (currentBase.FullName == ObservableObjectFullName)
                    return true;
                try { currentBase = currentBase.Resolve()?.BaseType; }
                catch (Mono.Cecil.AssemblyResolutionException) { return false; }
                catch (System.Exception) { return false; }
            }
            return false;
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

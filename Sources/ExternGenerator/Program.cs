using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternGenerator
{
    public static class Program : object
    {
        const string publicStr = "public ";
        const string externStr = "extern ";
        const string staticStr = "static ";
        const string virtualStr = "virtual ";
        const string overrideStr = "override ";
        const string finalStr = "final ";
        const string sealedStr = "sealed ";
        const string abstractStr = "abstract ";
        const string interfaceStr = "interface ";
        const string enumStr = "enum ";
        const string structStr = "struct ";
        const string delegateStr = "delegate ";
        const string classStr = "class ";
        const string namespaceStr = "namespace ";
        const string protectedInternalStr = "protected internal ";
        const string protectedStr = "protected ";
        const string internalStr = "internal ";
        const string readonlyStr = "readonly ";
        const string constStr = "const ";
        const string voidStr = "void ";

        static void Analyze(
            string assemblyName,
            string outputDir,
            string fullNamePrefix)
        {
            var assembly = AssemblyDefinition.ReadAssembly(assemblyName);

            StringBuilder sb = new StringBuilder();
            foreach (var item in assembly.MainModule.Types)
            {
                if (item.Namespace.StartsWith(fullNamePrefix)
                    && item.Namespace != string.Empty)
                {
                    string directory = System.IO.Path.Combine(
                        outputDir,
                        item.Namespace);

                    sb.Clear();
                    Program.CreateType(
                        item,
                        sb);

                    System.IO.Directory.CreateDirectory(directory);
                    System.IO.File.WriteAllText(
                        directory + "/" + item.Name + ".cs",
                        sb.ToString());
                }
            }
        }

        public static void CreateType(
            TypeDefinition typeDef,
            StringBuilder sb)
        {
            string prefix = "";

            sb.AppendLine(namespaceStr + typeDef.Namespace);
            sb.AppendLine("{");
            prefix = "    ";
            sb.Append(prefix);

            sb.Append("[")
                .Append("Obsolete")
                .Append(", ")
                .Append("System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)")
                .AppendLine("]")
                .Append(prefix);

            if (typeDef.IsPublic)
            { sb.Append(publicStr); }
            else
            { sb.Append(internalStr); }

            if (typeDef.IsSealed && typeDef.IsAbstract)
            { sb.Append(staticStr); }
            else if (typeDef.IsSealed && !typeDef.IsValueType)
            { sb.Append(sealedStr); }
            else if (typeDef.IsAbstract && !typeDef.IsInterface)
            { sb.Append(abstractStr); }

            if (typeDef.IsInterface)
            { sb.Append(interfaceStr); }
            else if (typeDef.IsEnum)
            { sb.Append(enumStr); }
            else if (typeDef.IsValueType)
            { sb.Append(structStr); }
            else if (typeDef.IsDelegate())
            {
                sb.Append(delegateStr);
                return;
            }
            else
            { sb.Append(classStr); }

            bool extended = false;
            sb.WriteName(typeDef.Name);
            if (!typeDef.IsEnum
                && typeDef.BaseType != null
                && !typeDef.IsValueType)
            {
                sb.Append(" : ")
                    .Write(typeDef.BaseType);
                extended = true;
            }

            foreach (var iface in typeDef.Interfaces)
            {
                sb.Append(extended ? ", " : " : ")
                    .Write(iface.InterfaceType);
                extended = true;
            }

            sb.AppendLine(" {");
            prefix = "        ";

            if (typeDef.HasFields)
            foreach (var field in typeDef.Fields)
            {
                if (!typeDef.IsEnum)
                { field.Write(sb, prefix); }
                else if (field.HasConstant)
                {
                    sb.Append(prefix)
                        .WriteName(field.Name)
                        .Append(" = ")
                        .Append(field.Constant)
                        .AppendLine(",");
                }
            }

            if (typeDef.HasEvents)
            foreach (var evt in typeDef.Events)
            { evt.Write(sb, prefix); }

            if (typeDef.HasProperties)
            foreach (var prop in typeDef.Properties)
            { prop.Write(sb, prefix); }

            if (typeDef.HasMethods)
            foreach (var method in typeDef.Methods)
            { method.Write(sb, prefix); }

            sb.AppendLine("    }");
            sb.AppendLine("}");
        }

        static void WriteDelegate(
            TypeDefinition typeDef,
            StringBuilder str)
        {
            foreach (var method in typeDef.Methods)
            {
                if (method.Name == "Invoke")
                {
                    method.WriteMethodStructure(
                        str,
                        typeDef.Name);

                    return;
                }
            }
        }

        static void Write(
            this FieldDefinition field,
            StringBuilder str,
            string prefix)
        {
            if (field.IsPrivate)
            {
                return;
            }

            str.Append(prefix);
            if (field.IsPublic)
            { str.Append(publicStr); }
            else if (field.IsFamilyOrAssembly)
            { str.Append(protectedInternalStr); }
            else if (field.IsFamily)
            { str.Append(protectedStr); }
            else if (field.IsAssembly)
            { str.Append(internalStr); }

            if (field.HasConstant)
            { str.Append(constStr); }
            else if (field.IsInitOnly)
            { str.Append(readonlyStr); }

            if (!field.HasConstant && field.IsStatic)
            {
                str.Append(staticStr);
            }

            str.Write(field.FieldType);

            str.Append(" ")
                .WriteName(field.Name);

            if (field.HasConstant)
            {
                str.Append(" = ");
                if (field.Constant is string)
                {
                    str.Append("\"" + field.Constant.ToString() + "\"");
                }
                else
                {
                    str.Append(field.Constant.ToString());
                }
            }

            str.AppendLine(";");
        }

        static void Write(
            this EventDefinition evt,
            StringBuilder str,
            string prefix)
        {
            if (evt.IsPrivate())
            {
                return;
            }

            str.Append(prefix);
            if (evt.IsPublic())
            { str.Append(publicStr); }
            else if (evt.IsFamilyOrAssembly())
            { str.Append(protectedInternalStr); }
            else if (evt.IsFamily())
            { str.Append(protectedStr); }
            else if (evt.IsAssembly())
            { str.Append(internalStr); }

            if (evt.IsStatic())
            { str.Append(staticStr); }
            else if (evt.IsAbstract())
            { str.Append(abstractStr); }
            else if (evt.IsOverride())
            { str.Append(overrideStr); }
            else if (evt.IsVirtual())
            { str.Append(virtualStr); }

            str.Write(evt.EventType);

            str.Append(" ")
                .WriteName(evt.Name);

            if ((evt.AddMethod == null) != (evt.RemoveMethod == null))
            {
                str.Append("{ ");
                if (evt.AddMethod != null)
                {
                    str.Append("add {} ");
                }
                if (evt.RemoveMethod != null)
                {
                    str.Append("remove {} ");
                }
                str.AppendLine("}");
            }
            else
            {
                str.AppendLine(";");
            }
        }

        static void Write(
            this PropertyDefinition prop,
            StringBuilder str,
            string prefix)
        {
            if (prop.IsPrivate())
            {
                return;
            }

            str.Append(prefix)
                .Append(externStr);
            if (prop.IsPublic())
            { str.Append(publicStr); }
            else if (prop.IsFamilyOrAssembly())
            { str.Append(protectedInternalStr); }
            else if (prop.IsFamily())
            { str.Append(protectedStr); }
            else if (prop.IsAssembly())
            { str.Append(internalStr); }

            if (prop.IsStatic())
            { str.Append(staticStr); }
            else if (prop.IsAbstract())
            { str.Append(abstractStr); }
            else if (prop.IsOverride())
            { str.Append(overrideStr); }
            else if (prop.IsVirtual())
            { str.Append(virtualStr); }

            str.Write(prop.PropertyType);

            str.Append(" ")
                .WriteName(prop.Name);

            str.Append("{ ");
            if (prop.GetMethod != null)
            {
                str.Append("get; ");
            }
            if (prop.SetMethod != null)
            {
                str.Append("set; ");
            }

            str.AppendLine("}");
        }

        static void Write(
            this MethodDefinition method,
            StringBuilder str,
            string prefix)
        {
            if (method.IsPrivate)
            { return; }

            if (method.IsSetter || method.IsGetter || method.IsAddOn || method.IsRemoveOn)
            { return; }

            if (method.IsStatic && method.IsConstructor)
            { return; }

            str.Append(prefix);
            if (!method.DeclaringType.IsInterface)
            {
                if (!method.IsAbstract())
                { str.Append(externStr); }

                if (method.IsPublic)
                { str.Append(publicStr); }
                else if (method.IsFamilyOrAssembly)
                { str.Append(protectedInternalStr); }
                else if (method.IsAssembly)
                { str.Append(internalStr); }
                else if (method.IsFamily)
                { str.Append(protectedStr); }
            }

            if (method.IsStatic)
            { str.Append(staticStr); }

            if (method.IsAbstract())
            { str.Append(abstractStr); }
            else if (method.IsOverride())
            { str.Append(overrideStr); }
            else if (method.IsVirtualMethod())
            { str.Append(virtualStr); }

            method.WriteMethodStructure(
                str,
                method.Name);
        }

        static void WriteMethodStructure(
            this MethodDefinition method,
            StringBuilder str,
            string methodName)
        {
            if (!method.IsConstructor)
            {
                if (method.ReturnType.Name == "Void")
                { str.Append(voidStr); }
                else
                { str.Write(method.ReturnType); }

                str.Append(' ')
                    .WriteName(methodName);
            }
            else
            {
                str.WriteName(method.DeclaringType.Name);
            }

            if (method.HasGenericParameters)
            {
                str.Append('<');
                bool firstAdded = false;
                foreach (var gp in method.GenericParameters)
                {
                    if (!firstAdded)
                    { firstAdded = true; }
                    else
                    { str.Append(", "); }

                    str.WriteName(gp.Name);
                    firstAdded = true;
                }

                str.Append('>');
            }

            str.Append('(');
            if (method.HasParameters)
            {
                bool firstAdded = false;
                foreach (var para in method.Parameters)
                {
                    if (!firstAdded)
                    { firstAdded = true; }
                    else
                    { str.Append(", "); }

                    if (para.IsOut)
                    { str.Append("out "); }
                    else if (para.ParameterType is ByReferenceType)
                    { str.Append("ref "); }

                    str.Write(para.ParameterType)
                        .Append(' ')
                        .WriteName(para.Name);
                }
            }

            str.AppendLine(");");
        }

        static StringBuilder WriteName(
            this StringBuilder sb,
            string name)
        {
            switch (name)
            {
                case "event":
                case "class":
                case "enum":
                case "interface":
                case "for":
                case "break":
                case "int":
                case "double":
                case "float":
                case "char":
                case "byte":
                case "uint":
                case "ulong":
                case "sbyte":
                case "int16":
                case "uint16":
                    sb.Append('@')
                        .Append(name);
                    break;
                default:
                    sb.Append(name);
                    break;
            }

            return sb;
        }

        static StringBuilder Write(
            this StringBuilder sb,
            TypeReference typeRef)
        {
            ByReferenceType byRefType = typeRef as ByReferenceType;

            if (byRefType != null)
            { sb.Write(byRefType.ElementType); }
            else
            {
                GenericInstanceType git = typeRef as GenericInstanceType;
                if (git != null)
                {
                    int genericT = git.FullName.IndexOf('`');
                    sb.Append(git.FullName.Substring(0, genericT))
                        .Append('<');

                    bool firstAdded = false;
                    foreach (var genericParam in git.GenericArguments)
                    {
                        if (firstAdded)
                        { sb.Append(", "); }
                        else
                        { firstAdded = true; }

                        sb.Write(genericParam);
                    }

                    sb.Append('>');
                }
                else
                {
                    sb.Append(typeRef.FullName);
                }
            }

            return sb;
        }

        static StringBuilder Write(
            this StringBuilder sb,
            TypeDefinition typeDef)
        {
            return sb;
        }

        static bool IsPublic(this EventDefinition evt)
        {
            return (evt.AddMethod == null || evt.AddMethod.IsPublic)
                || (evt.RemoveMethod == null || evt.RemoveMethod.IsPublic);
        }

        static bool IsPrivate(this EventDefinition evt)
        {
            return (evt.AddMethod == null || evt.AddMethod.IsPrivate)
                || (evt.RemoveMethod == null || evt.RemoveMethod.IsPrivate);
        }

        static bool IsAssembly(this EventDefinition evt)
        {
            return (evt.AddMethod == null || evt.AddMethod.IsAssembly)
                || (evt.RemoveMethod == null || evt.RemoveMethod.IsAssembly);
        }

        static bool IsFamily(this EventDefinition evt)
        {
            return (evt.AddMethod == null || evt.AddMethod.IsFamily)
                || (evt.RemoveMethod == null || evt.RemoveMethod.IsFamily);
        }

        static bool IsFamilyOrAssembly(this EventDefinition evt)
        {
            return (evt.AddMethod == null || evt.AddMethod.IsFamilyOrAssembly)
                || (evt.RemoveMethod == null || evt.RemoveMethod.IsFamilyOrAssembly);
        }

        static bool IsVirtual(this EventDefinition evt)
        {
            return (evt.AddMethod == null || evt.AddMethod.IsVirtualMethod())
                || (evt.RemoveMethod == null || evt.RemoveMethod.IsVirtualMethod());
        }

        static bool IsStatic(this EventDefinition evt)
        {
            return (evt.AddMethod == null || evt.AddMethod.IsStatic)
                || (evt.RemoveMethod == null || evt.RemoveMethod.IsStatic);
        }

        static bool IsPublic(this PropertyDefinition evt)
        {
            return (evt.GetMethod == null || evt.GetMethod.IsPublic)
                || (evt.SetMethod == null || evt.SetMethod.IsPublic);
        }

        static bool IsPrivate(this PropertyDefinition evt)
        {
            return (evt.GetMethod == null || evt.GetMethod.IsPrivate)
                || (evt.SetMethod == null || evt.SetMethod.IsPrivate);
        }

        static bool IsAssembly(this PropertyDefinition evt)
        {
            return (evt.GetMethod == null || evt.GetMethod.IsAssembly)
                || (evt.SetMethod == null || evt.SetMethod.IsAssembly);
        }

        static bool IsFamily(this PropertyDefinition evt)
        {
            return (evt.GetMethod == null || evt.GetMethod.IsFamily)
                || (evt.SetMethod == null || evt.SetMethod.IsFamily);
        }

        static bool IsFamilyOrAssembly(this PropertyDefinition evt)
        {
            return (evt.GetMethod == null || evt.GetMethod.IsFamilyOrAssembly)
                || (evt.SetMethod == null || evt.SetMethod.IsFamilyOrAssembly);
        }

        static bool IsVirtual(this PropertyDefinition evt)
        {
            return (evt.GetMethod == null || (evt.GetMethod.IsVirtualMethod()))
                || (evt.SetMethod == null || (evt.SetMethod.IsVirtualMethod()));
        }

        static bool IsStatic(this PropertyDefinition evt)
        {
            return (evt.GetMethod == null || evt.GetMethod.IsStatic)
                || (evt.SetMethod == null || evt.SetMethod.IsStatic);
        }

        static bool IsAbstract(this EventDefinition prop)
        {
            return (prop.AddMethod == null || prop.AddMethod.IsAbstract())
                || (prop.RemoveMethod == null || prop.RemoveMethod.IsAbstract());
        }

        static bool IsAbstract(this PropertyDefinition prop)
        {
            return (prop.GetMethod == null || prop.GetMethod.IsAbstract())
                || (prop.SetMethod == null || prop.SetMethod.IsAbstract());
        }

        static bool IsOverride(this EventDefinition prop)
        {
            return (prop.AddMethod == null || prop.AddMethod.IsOverride())
                || (prop.RemoveMethod == null || prop.RemoveMethod.IsOverride());
        }

        static bool IsOverride(this PropertyDefinition prop)
        {
            return (prop.GetMethod == null || prop.GetMethod.IsOverride())
                || (prop.SetMethod == null || prop.SetMethod.IsOverride());
        }

        static bool IsAbstract(this MethodDefinition method)
        {
            return method.IsVirtual && !method.HasBody && !method.DeclaringType.IsInterface;
        }

        static bool IsOverride(this MethodDefinition method)
        {
            return method.IsVirtual && !method.IsNewSlot;
        }

        static bool IsVirtualMethod(this MethodDefinition method)
        {
            return method.IsVirtual
                && method.IsNewSlot
                && !method.IsFinal
                && !method.DeclaringType.IsInterface
                && !method.DeclaringType.IsSealed;
        }

        static bool IsDelegate(this TypeDefinition typeDefinition)
        {
            return typeDefinition.BaseType != null
                && typeDefinition.BaseType.FullName == "System.MulticastDelegate";
        }

        static void Main(string[] args)
        {
            Program.Analyze(
                @"C:\Windows\Microsoft.Net\assembly\GAC_MSIL\System.Dynamic\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Dynamic.dll",
                ".",
                // "System.Dynamic");
                string.Empty);
        }
    }
}

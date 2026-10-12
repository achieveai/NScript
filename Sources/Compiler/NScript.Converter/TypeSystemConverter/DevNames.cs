//-----------------------------------------------------------------------
// <copyright file="DevNames.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Mono.Cecil;
    using NScript.JST;

    /// <summary>
    /// Identity-derived dev-mode names (slice 2, design section 3). Every form is a function
    /// of metadata only, so a body edit never renames anything outside its own function.
    /// <list type="bullet">
    /// <item><c>M</c> mangles a metadata name into JS identifier characters and never emits <c>$</c>.</item>
    /// <item>A type form <c>TF</c> always ends with one <c>$</c> (critic advisory A1), so a
    ///   namespace-less type can never take a browser global's name or a namespace identifier.</item>
    /// <item>Structural keywords follow an empty <c>$</c>-token (<c>$$of$</c>, <c>$$factory</c>), which
    ///   <c>M</c> can never produce.</item>
    /// </list>
    /// </summary>
    public static class DevNames
    {
        private const string Base62 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Kind tags for member scopes, one per <see cref="TypeScopeManager"/> map.
        /// </summary>
        public static class Kind
        {
            public const char InstanceMethod = 'm';
            public const char StaticMethod = 's';
            public const char Field = 'f';
            public const char Property = 'p';
            public const char ImportedExtensionProperty = 'x';
            public const char UnderlyingMethod = 'u';
            public const char UnderlyingStaticMethod = 't';
            public const char VirtualSlot = 'v';
        }

        /// <summary>
        /// Injective mangling: <c>[A-Za-z0-9]</c> kept, <c>.</c> to <c>_</c>, <c>/</c> to <c>_9</c>,
        /// <c>_</c> to <c>_0</c>, <c>`</c> to <c>_1</c>, anything else to <c>_2&lt;hex&gt;_</c>.
        /// </summary>
        public static string M(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Dev names need a non-empty metadata name.", nameof(name));
            }

            var sb = new StringBuilder(name.Length + 8);
            foreach (var ch in name)
            {
                if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9'))
                {
                    sb.Append(ch);
                }
                else if (ch == '.')
                {
                    sb.Append('_');
                }
                else if (ch == '/')
                {
                    sb.Append("_9");
                }
                else if (ch == '_')
                {
                    sb.Append("_0");
                }
                else if (ch == '`')
                {
                    sb.Append("_1");
                }
                else
                {
                    sb.Append("_2").Append(((int)ch).ToString("x")).Append('_');
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Type form. Definitions use the Cecil <c>FullName</c> (nested types include the whole
        /// declaring chain); generic instances bracket their arguments. A type whose outermost type
        /// is not public also carries its assembly (<c>M(asm) $$asm$ M(FullName) $</c>): two
        /// assemblies of one bundle may each have an internal type of the same full name. An
        /// assembly name may start with a digit, a JS name may not: such a one gets a leading
        /// <c>$</c>, which no other form starts with followed by a digit, so the forms stay distinct.
        /// </summary>
        public static string TypeForm(TypeReference type)
        {
            switch (type)
            {
                case GenericParameter gp:
                    return "$$" + (gp.Type == GenericParameterType.Method ? "m" : "t") + gp.Position + "$";
                case GenericInstanceType git:
                    return TypeForm(git.ElementType)
                        + "$of$"
                        + string.Join("$and$", git.GenericArguments.Select(TypeForm))
                        + "$end$";
                default:
                    var definition = type as TypeDefinition ?? type.Resolve();
                    return definition != null && !Outermost(definition).IsPublic
                        ? AssemblyPrefix(definition.Module.Assembly.Name.Name) + "$$asm$" + M(type.FullName) + "$"
                        : M(type.FullName) + "$";
            }
        }

        private static string AssemblyPrefix(string assemblyName)
        {
            var mangled = M(assemblyName);
            return mangled[0] >= '0' && mangled[0] <= '9' ? "$" + mangled : mangled;
        }

        // A nested type's full name starts with its outermost type's, so only that one's
        // visibility decides whether another assembly can have the same full name.
        private static TypeDefinition Outermost(TypeDefinition type)
        {
            while (type.DeclaringType != null)
            {
                type = type.DeclaringType;
            }

            return type;
        }

        /// <summary>Root static member form: <c>TF(type) M(member) [$ sig]</c>.</summary>
        public static string StaticMember(TypeReference declaringType, string memberName, string sig = null)
            => TypeForm(declaringType) + M(memberName) + (sig == null ? string.Empty : "$" + sig);

        /// <summary>Root factory form: <c>TF(type) $factory [$ sig]</c>.</summary>
        public static string Factory(TypeReference declaringType, string sig = null)
            => TypeForm(declaringType) + "$factory" + (sig == null ? string.Empty : "$" + sig);

        /// <summary>Converter-made root helper of a type, e.g. <c>TF(type) $initTracker</c>.</summary>
        public static string TypeHelper(TypeReference declaringType, string helper)
            => TypeForm(declaringType) + "$" + helper;

        /// <summary>Member-scope form: <c>M(name) $ depth kind [$ sig]</c>.</summary>
        public static string Member(string name, int depth, char kind, string sig = null)
            => M(name) + "$" + depth + kind + (sig == null ? string.Empty : "$" + sig);

        /// <summary>Position of <paramref name="scope"/> in its lookup chain (root is 0).</summary>
        public static int Depth(IdentifierScope scope)
        {
            int depth = 0;
            for (var parent = scope.ParentScope; parent != null; parent = parent.ParentScope)
            {
                depth++;
            }

            return depth;
        }

        /// <summary>
        /// Overload signature, or null when the declaring type's metadata has only one method of
        /// this name. Metadata alone decides, so a body edit never adds or drops a sig.
        /// </summary>
        public static string MethodSig(MethodDefinition method)
        {
            var overloads = method.DeclaringType.Methods.Count(m => m.Name == method.Name);
            return overloads > 1 ? Sig(method) : null;
        }

        /// <summary>Overload signature for properties (indexers), or null when not overloaded.</summary>
        public static string PropertySig(PropertyDefinition property)
        {
            var overloads = property.DeclaringType.Properties.Count(p => p.Name == property.Name);
            return overloads > 1
                ? string.Join("$_", property.Parameters.Select(p => TypeForm(p.ParameterType)))
                : null;
        }

        /// <summary>Parameter type forms joined by <c>$_</c>, plus generic arity and, for conversion operators, the return type.</summary>
        public static string Sig(MethodDefinition method)
        {
            var sig = string.Join("$_", method.Parameters.Select(p => TypeForm(p.ParameterType)));
            if (method.HasGenericParameters)
            {
                sig += "$a" + method.GenericParameters.Count;
            }

            if (method.Name == "op_Implicit" || method.Name == "op_Explicit")
            {
                sig += "$r$" + TypeForm(method.ReturnType);
            }

            return sig;
        }

        /// <summary>
        /// Dev type id: <c>"k" + base62(SHA-256(asm|FullName))[0..9]</c>. Stable across builds and
        /// bundles; collisions inside one bundle are reported as NSDEV001 by the caller.
        /// </summary>
        public static string TypeId(TypeDefinition type)
        {
            var key = type.Module.Assembly.Name.Name + "|" + type.FullName;
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
            ulong value = BitConverter.ToUInt64(hash, 0);
            var chars = new char[10];
            chars[0] = 'k';
            for (int i = 9; i >= 1; i--)
            {
                chars[i] = Base62[(int)(value % 62)];
                value /= 62;
            }

            return new string(chars);
        }

        /// <summary>
        /// Sets <paramref name="identifier"/>'s stable name when dev mode is on. Enforced
        /// identifiers keep their names and are left alone.
        /// </summary>
        internal static void Assign(ConverterContext context, IIdentifier identifier, Func<string> stableName)
        {
            if (context.DevMode
                && identifier is SimpleIdentifier simple
                && !simple.ShouldEnforceSuggestion
                && simple.StableName == null)
            {
                simple.StableName = stableName();
            }
        }
    }
}

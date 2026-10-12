namespace NScript.RazorSkin.TemplateIR
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using NScript.Utils;

    /// <summary>
    /// Deep copy of a template IR. Code generation writes Cecil-derived values into the IR
    /// (RazorSkinJSTGenerator sub-control TagName, DomAttributes, ResolvedTypeName,
    /// IsDelegate, IsLiteral; GraphTopologyBuilder RuntimeMarkerIdx), some only
    /// conditionally, so a cached IR is handed out as a fresh copy per build.
    /// Copies IR classes field by field and lists element by element; shares strings, value
    /// types and the immutable <see cref="Location"/>. Any other reference type throws, so a
    /// new IR field of an unexpected type cannot be shared by accident.
    /// </summary>
    public static class IRCloner
    {
        private static readonly MethodInfo MemberwiseCloneMethod =
            typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly ConcurrentDictionary<Type, FieldInfo[]> Fields = new ConcurrentDictionary<Type, FieldInfo[]>();

        public static T Clone<T>(T value)
            where T : class
            => (T)CloneValue(value);

        private static object CloneValue(object value)
        {
            switch (value)
            {
                case null:
                    return null;
                case string _:
                case Location _:
                    return value;
            }

            var type = value.GetType();
            if (type.IsEnum || type.IsPrimitive || IsStringPair(type))
            {
                return value;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var source = (IList)value;
                var copy = (IList)Activator.CreateInstance(type, source.Count);
                foreach (var item in source)
                {
                    copy.Add(CloneValue(item));
                }

                return copy;
            }

            if (type.IsClass && type.Namespace == typeof(IRNode).Namespace)
            {
                var copy = MemberwiseCloneMethod.Invoke(value, null);
                foreach (var field in Fields.GetOrAdd(type, GetInstanceFields))
                {
                    field.SetValue(copy, CloneValue(field.GetValue(value)));
                }

                return copy;
            }

            throw new NotSupportedException("IRCloner: cannot copy a value of type " + type.FullName + ". Add it to IRCloner.");
        }

        private static bool IsStringPair(Type type)
            => type == typeof(KeyValuePair<string, string>);

        private static FieldInfo[] GetInstanceFields(Type type)
        {
            var fields = new List<FieldInfo>();
            for (var current = type; current != null && current != typeof(object); current = current.BaseType)
            {
                fields.AddRange(current.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
            }

            return fields.ToArray();
        }
    }
}

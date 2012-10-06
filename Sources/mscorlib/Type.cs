namespace System
{
    using System.Collections;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [Imported]
    [ScriptName("Function")]
    public sealed class Type
    {
        /// <summary>
        /// Backing field for typeMapping.
        /// </summary>
        [Implement]
        private readonly static Dictionary typeMapping;

        [Implement]
        public readonly bool IsDelegate;

        [Implement]
        public readonly bool IsClass;

        [Implement]
        public readonly bool IsEnum;

        [Implement]
        public readonly bool IsStruct;

        [Implement]
        public readonly bool IsInterface;

        [Implement]
        public readonly Type BaseType;

        [Implement]
        public readonly string FullName;

        [IntrinsicField]
        public readonly string Name;

        [Implement]
        public readonly string TypeId;

        [Implement]
        private readonly Dictionary baseInterfaces;

        /// <summary>
        /// This is pointer that is used to store boxed value.
        /// </summary>
        [Implement]
        private readonly object boxedValue;

        /// <summary>
        /// Mapping for enum value to it's string (used for ToString call).
        /// </summary>
        [Implement]
        private readonly object enumValueToStrMap;

        /// <summary>
        /// Mapping from enum string to value (used for Parsing).
        /// </summary>
        [Implement]
        private readonly object enumStrToValueMap;

        /// <summary>
        /// Mapping from enum string to value (used for Parsing).
        /// </summary>
        [Implement]
        private readonly object enumLowerStrToValueMap;

        /// <summary>
        /// Is enum a flags enum.
        /// </summary>
        [Implement]
        private readonly bool isFlagEnum;

        /// <summary>
        /// Backing field for interfaces.
        /// </summary>
        [Implement]
        private readonly Type[] interfaces;

        /// <summary>
        /// Generic Closure (root) for generic type.
        /// </summary>
        private readonly object genericClosure;

        /// <summary>
        /// Generic Parameters for a generic type.
        /// </summary>
        private readonly Type[] genericParameters;

        [PreserveName]
        [IntrinsicField]
        internal readonly object Prototype;

        public static Type GetType(string typeName)
        {
            return (Type)Type.typeMapping[typeName];
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static extern Type GetTypeFromHandle(RuntimeTypeHandle typeHandle);

        public Type[] GetInterfaces()
        {
            return this.interfaces;
        }

        public extern bool IsAssignableFrom(Type type);

        [Script(@"
            if (!this.@{[mscorlib]System.Type::IsInterface})
                return instance instanceof this;
            else if (instance && !instance.constructor.@{[mscorlib]System.Type::baseInterfaces})
                @{[mscorlib]System.Type::InitializeBaseInterfaces([mscorlib]System.Type)}(instance.constructor);

            return instance
                    && instance.constructor.@{[mscorlib]System.Type::baseInterfaces}
                    && instance.constructor.@{[mscorlib]System.Type::baseInterfaces}[this.@{[mscorlib]System.Type::FullName}];
            ")]
        [KeepInstanceUsage]
        public extern bool IsInstanceOfType(object instance);

        [Script(@"return this.@{[mscorlib]System.Type::IsInstanceOfType([mscorlib]System.Object)}(instance) ? instance : null;")]
        public extern object AsType(object instance);

        [Script(@"
            if (this.@{[mscorlib]System.Type::IsInstanceOfType([mscorlib]System.Object)}(instance) || instance === null) {
                if (this.@{[mscorlib]System.Type::IsStruct})
                    return instance.@{[mscorlib]System.ValueType::boxedValue};
                return instance;
            }

            throw ""InvalidCast to "" + this.@{[mscorlib]System.Type::FullName};
            ")]
        public extern object CastType(object instance);

        [Script(@"return this.@{[mscorlib]System.Type::FullName} ? this.@{[mscorlib]System.Type::FullName} : this.name;")]
        public extern override string ToLocaleString();

        [Script(@"return this.@{[mscorlib]System.Type::FullName} ? this.@{[mscorlib]System.Type::FullName} : this.name;")]
        public extern override string ToString();

        [Script(@"
            this.@{[mscorlib]System.Type::IsClass} = true;
            this.@{[mscorlib]System.Type::FullName} = typeName;
            this.@{[mscorlib]System.Type::BaseType} = parentType;
            this.@{[mscorlib]System.Type::interfaces} = parentType ? interfaces.concat(parentType.@{[mscorlib]System.Type::interfaces}) : interfaces;
            if (!@{[mscorlib]System.Type::typeMapping}) { @{[mscorlib]System.Type::typeMapping} = {}; }
            @{[mscorlib]System.Type::typeMapping}[this.@{[mscorlib]System.Type::FullName}] = this;
        ")]
        internal extern void RegisterReferenceType(
            string typeName,
            Type parentType,
            Type[] interfaces);

        [Script(@"
            this.@{[mscorlib]System.Type::IsStruct} = true;
            this.@{[mscorlib]System.Type::FullName} = typeName;
            this.@{[mscorlib]System.Type::BaseType} = @{[mscorlib]System.ValueType};
            this.@{[mscorlib]System.Type::interfaces} = interfaces;
            if (!@{[mscorlib]System.Type::typeMapping}) { @{[mscorlib]System.Type::typeMapping} = {}; }
            @{[mscorlib]System.Type::typeMapping}[this.@{[mscorlib]System.Type::FullName}] = this;
        ")]
        internal extern void RegisterStructType(
            string typeName,
            Type[] interfaces);

        [Script(@"
            this.@{[mscorlib]System.Type::IsInterface} = true;
            this.@{[mscorlib]System.Type::FullName} = typeName;
        ")]
        internal extern void RegisterInterface(
            string typeName);

        [Script(@"
            this.@{[mscorlib]System.Type::IsEnum} = true;
            this.@{[mscorlib]System.Type::FullName} = typeName;
            this.@{[mscorlib]System.Type::isFlagEnum} = isFlag;
            this.@{[mscorlib]System.Type::BaseType} = @{[mscorlib]System.Enum};
            var enumStrToValueMap = this.@{[mscorlib]System.Type::enumStrToValueMap};
            var valueToStr = {};
            var lowerStrToValue = {};
            for(var key in enumStrToValueMap)
            {
                valueToStr[enumStrToValueMap[key]] = key;
                lowerStrToValue[key.toLower()] = enumStrToValueMap[key];
            }

            this.@{[mscorlib]System.Type::enumValueToStrMap} = valueToStr;
            this.@{[mscorlib]System.Type::enumLowerStrToValueMap} = lowerStrToValue;
            if (!@{[mscorlib]System.Type::typeMapping}) { @{[mscorlib]System.Type::typeMapping} = {}; }
            @{[mscorlib]System.Type::typeMapping}[this.@{[mscorlib]System.Type::FullName}] = this;
        ")]
        internal extern void RegisterEnum(
            string typeName,
            bool isFlag);

        [Script(@"
            if (this.@{[mscorlib]System.Type::IsStruct}) {
              return new (this)(instance);
            } else {
              return instance;
            }")]
        [KeepInstanceUsage]
        private extern object Box(object instance);

        [Script(@"
            if (this.@{[mscorlib]System.Type::IsStruct}) {
              return instance.@{[mscorlib]System.Type::boxedValue};
            } else {
              return instance;
            }")]
        [KeepInstanceUsage]
        private extern object Unbox(object instance);

        [Script(@"
            if (this.@{[mscorlib]System.Type::IsStruct}) return this.@{[mscorlib]System.Type::GetDefaultValue()}();
            throw ""Default constructor not implemented"";")]
        [KeepInstanceUsage]
        private extern object DefaultConstructor();

        [KeepInstanceUsage]
        [Script("return null;")]
        private extern object GetDefaultValue();

        [Script(@"
            var key;
            if (!type.@{[mscorlib]System.Type::baseInterfaces}) {
                var rv = {};
                var baseType = type.@{[mscorlib]System.Type::BaseType};
                if (baseType != null && baseType != @{[mscorlib]System.Object})
                {
                    if (baseType)
                        @{[mscorlib]System.Type::InitializeBaseInterfaces([mscorlib]System.Type)}(type);
                    var baseInterfaces = baseType.@{[mscorlib]System.Type::baseInterfaces};
                    if(baseInterfaces)
                    {
                        for (key in baseInterfaces)
                        {
                            rv[key] = baseInterfaces[key];
                        }
                    }
                }

                var interfaces = type.@{[mscorlib]System.Type::interfaces};
                if (interfaces) {
                    for(key = 0; key < interfaces.length; key++) {
                        rv[interfaces[key].@{[mscorlib]System.Type::FullName}] = interfaces[key];
                    }
                }

                type.@{[mscorlib]System.Type::baseInterfaces} = rv;
            }
        ")]
        private extern static void InitializeBaseInterfaces(Type type);
    }
}
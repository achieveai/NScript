namespace System
{
    using System.Collections;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [Extended]
    [ScriptName("Function")]
    public class Type
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
        public readonly bool IsNullable;

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
            if (instance === null || typeof instance === 'undefined')
                return false;
            if (!this.@{[mscorlib]System.Type::IsInterface})
                return instance instanceof this || (instance !== null && instance.constructor == this);
            else if (!instance.constructor.@{[mscorlib]System.Type::baseInterfaces})
                @{[mscorlib]System.Type::InitializeBaseInterfaces([mscorlib]System.Type)}(instance.constructor);

            return instance.constructor.@{[mscorlib]System.Type::baseInterfaces}
                    && instance.constructor.@{[mscorlib]System.Type::baseInterfaces}[this.@{[mscorlib]System.Type::FullName}];
            ")]
        [KeepInstanceUsage]
        public extern bool IsInstanceOfType(object instance);

        [Script(@"return this.@{[mscorlib]System.Type::IsInstanceOfType([mscorlib]System.Object)}(instance) ? instance : null;")]
        public extern object AsType(object instance);

        [Script(@"
            if (this.@{[mscorlib]System.Type::IsInstanceOfType([mscorlib]System.Object)}(instance) || instance === null || typeof instance === ""undefined"") {
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

        [Script(@"return obj;")]
        [IgnoreGenericArguments]
        public extern static TTo AS<TFrom, TTo>(TFrom obj);

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
            var keys = ({}).constructor.keys(enumStrToValueMap);
            var i;
            for(i = 0; i < keys.length; i++)
            {
                var key = keys[i];
                valueToStr[enumStrToValueMap[key]] = key;
                lowerStrToValue[key.toLowerCase()] = enumStrToValueMap[key];
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
            if (type.@{[mscorlib]System.Type::IsNullable}) {
              return type.@{[mscorlib]System.Type::NullableBox([mscorlib]System.Object)}(instance);
            } else if (type.@{[mscorlib]System.Type::IsStruct}) {
              return new (type)(instance);
            } else {
              return instance;
            }")]
        private extern static object BoxTypeInstance(Type type, object instance);

        [Script(@"
            if (type.@{[mscorlib]System.Type::IsNullable} && instance === null) {
              return null;
            } else if (type.@{[mscorlib]System.Type::IsStruct}) {
              return instance.@{[mscorlib]System.ValueType::boxedValue};
            } else {
              return instance;
            }")]
        private extern static object UnBoxTypeInstance(Type type, object instance);

        [Script(@"
            if (type.@{[mscorlib]System.Type::IsStruct}) return type.@{[mscorlib]System.Type::GetDefaultValue()};
            else if (type.@{[mscorlib]System.Type::DefaultConstructor()}) return type.@{[mscorlib]System.Type::DefaultConstructor()};
            else return function() { return null; };")]
        private extern static object GetDefaultConstructor(Type type);

        [Script(@"
            if (type.@{[mscorlib]System.Type::IsStruct}) return type.@{[mscorlib]System.Type::GetDefaultValue()}();
            return null;")]
        private extern static object GetDefaultValueStatic(Type type);

        [Script(@"
            if (this.@{[mscorlib]System.Type::IsStruct}) return this.@{[mscorlib]System.Type::GetDefaultValue()};
            throw ""Default constructor not implemented"";")]
        [KeepInstanceUsage]
        private extern object DefaultConstructor();

        [Script("return null;")]
        [KeepInstanceUsage]
        private extern object GetDefaultValue();

        [Script("return null;")]
        [KeepInstanceUsage]
        private extern object NullableBox(object instance);

        [Script(@"
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
                        var keys = ({}).constructor.keys(baseInterfaces);
                        var i;
                        for (i = 0; i < keys.length; i++)
                        {
                            var key = keys[i];
                            rv[key] = baseInterfaces[key];
                        }
                    }
                }

                var interfaces = type.@{[mscorlib]System.Type::interfaces};
                if (interfaces) {
                    var idx;
                    for(idx = 0; idx < interfaces.length; idx++) {
                        rv[interfaces[idx].@{[mscorlib]System.Type::FullName}] = interfaces[idx];
                    }
                }

                type.@{[mscorlib]System.Type::baseInterfaces} = rv;
            }
        ")]
        private extern static void InitializeBaseInterfaces(Type type);
    }
}
namespace System
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace]
    [Extended]
    public class Object
    {
        /// <summary>
        /// Pointer to the type. This most of the time is __constructor__.
        /// </summary>
        [PreserveName]
        [IntrinsicField]
        internal readonly Type Constructor;

        [MakeStaticUsage]
        [Script("return this.constructor;")]
        public extern Type GetType();

        [ScriptName("toLocaleString")]
        public extern virtual string ToLocaleString();

        [ScriptName("toString")]
        public extern virtual string ToString();

        [Script("return obj1 === obj2;")]
        [IgnoreGenericArguments]
        public extern static bool Equals<T>(T obj1, T obj2);

        [Script("return obj === null || typeof obj == \"undefined\";")]
        [IgnoreGenericArguments]
        public extern static bool IsNullOrUndefined<T>(T obj);
    }
}
namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class TypeUtils : System.Object {
        internal const System.Reflection.MethodAttributes PublicStatic = System.Reflection.MethodAttributes.Public | Reflection.MethodAttributes.Static;
        extern internal static System.Type GetNonNullableType(System.Type type);
        extern internal static System.Boolean IsNullableType(System.Type type);
        extern internal static System.Boolean AreReferenceAssignable(System.Type dest, System.Type src);
        extern internal static System.Boolean AreAssignable(System.Type dest, System.Type src);
        extern internal static System.Boolean IsImplicitlyConvertible(System.Type source, System.Type destination);
        extern internal static System.Boolean IsImplicitlyConvertible(System.Type source, System.Type destination, System.Boolean considerUserDefined);
        extern internal static System.Reflection.MethodInfo GetUserDefinedCoercionMethod(System.Type convertFrom, System.Type convertToType, System.Boolean implicitOnly);
        extern internal static System.Reflection.MethodInfo FindConversionOperator(System.Reflection.MethodInfo[] methods, System.Type typeFrom, System.Type typeTo, System.Boolean implicitOnly);
    }
}

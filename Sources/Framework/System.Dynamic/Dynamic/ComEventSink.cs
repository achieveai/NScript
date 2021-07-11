namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class ComEventSink : System.MarshalByRefObject, System.IDisposable {
        extern public static System.Dynamic.ComEventSink FromRuntimeCallableWrapper(System.Object rcw, System.Guid sourceIid, System.Boolean createIfNotFound);
        extern public void  AddHandler(System.Int32 dispid, System.Object func);
        extern public void  RemoveHandler(System.Int32 dispid, System.Object func);
        extern public System.Object ExecuteHandler(System.String name, System.Object[] args);
        extern public System.Reflection.FieldInfo GetField(System.String name, System.Reflection.BindingFlags bindingAttr);
        extern public System.Reflection.FieldInfo[] GetFields(System.Reflection.BindingFlags bindingAttr);
        extern public System.Reflection.MemberInfo[] GetMember(System.String name, System.Reflection.BindingFlags bindingAttr);
        extern public System.Reflection.MemberInfo[] GetMembers(System.Reflection.BindingFlags bindingAttr);
        extern public System.Reflection.MethodInfo GetMethod(System.String name, System.Reflection.BindingFlags bindingAttr);
        extern public System.Reflection.MethodInfo GetMethod(System.String name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Type[] types, System.Reflection.ParameterModifier[] modifiers);
        extern public System.Reflection.MethodInfo[] GetMethods(System.Reflection.BindingFlags bindingAttr);
        extern public System.Reflection.PropertyInfo GetProperty(System.String name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Type returnType, System.Type[] types, System.Reflection.ParameterModifier[] modifiers);
        extern public System.Reflection.PropertyInfo GetProperty(System.String name, System.Reflection.BindingFlags bindingAttr);
        extern public System.Reflection.PropertyInfo[] GetProperties(System.Reflection.BindingFlags bindingAttr);
        extern public void  Dispose();
    }
}

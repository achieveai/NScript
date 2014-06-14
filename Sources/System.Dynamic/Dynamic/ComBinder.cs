namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class ComBinder : System.Object {
        extern public static System.Boolean IsComObject(System.Object value);
        extern public static System.Boolean TryBindGetMember(System.Dynamic.GetMemberBinder binder, System.Dynamic.DynamicMetaObject instance, out System.Dynamic.DynamicMetaObject result, System.Boolean delayInvocation);
        extern public static System.Boolean TryBindGetMember(System.Dynamic.GetMemberBinder binder, System.Dynamic.DynamicMetaObject instance, out System.Dynamic.DynamicMetaObject result);
        extern public static System.Boolean TryBindSetMember(System.Dynamic.SetMemberBinder binder, System.Dynamic.DynamicMetaObject instance, System.Dynamic.DynamicMetaObject value, out System.Dynamic.DynamicMetaObject result);
        extern public static System.Boolean TryBindInvoke(System.Dynamic.InvokeBinder binder, System.Dynamic.DynamicMetaObject instance, System.Dynamic.DynamicMetaObject[] args, out System.Dynamic.DynamicMetaObject result);
        extern public static System.Boolean TryBindInvokeMember(System.Dynamic.InvokeMemberBinder binder, System.Dynamic.DynamicMetaObject instance, System.Dynamic.DynamicMetaObject[] args, out System.Dynamic.DynamicMetaObject result);
        extern public static System.Boolean TryBindGetIndex(System.Dynamic.GetIndexBinder binder, System.Dynamic.DynamicMetaObject instance, System.Dynamic.DynamicMetaObject[] args, out System.Dynamic.DynamicMetaObject result);
        extern public static System.Boolean TryBindSetIndex(System.Dynamic.SetIndexBinder binder, System.Dynamic.DynamicMetaObject instance, System.Dynamic.DynamicMetaObject[] args, System.Dynamic.DynamicMetaObject value, out System.Dynamic.DynamicMetaObject result);
        extern public static System.Boolean TryConvert(System.Dynamic.ConvertBinder binder, System.Dynamic.DynamicMetaObject instance, out System.Dynamic.DynamicMetaObject result);
        extern public static System.Collections.Generic.IEnumerable<System.String> GetDynamicMemberNames(System.Object value);
        extern internal static System.Collections.Generic.IList<System.String> GetDynamicDataMemberNames(System.Object value);
        extern internal static System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<System.String, System.Object>> GetDynamicDataMembers(System.Object value, System.Collections.Generic.IEnumerable<System.String> names);
    }
}

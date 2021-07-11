namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class IDispatchComObject : System.Dynamic.ComObject, System.Dynamic.IDynamicMetaObjectProvider {
        extern internal IDispatchComObject(System.Dynamic.IDispatch rcw);
        extern public override System.String ToString();
        extern internal System.Boolean TryGetGetItem(out System.Dynamic.ComMethodDesc value);
        extern internal System.Boolean TryGetSetItem(out System.Dynamic.ComMethodDesc value);
        extern internal System.Boolean TryGetMemberMethod(System.String name, out System.Dynamic.ComMethodDesc method);
        extern internal System.Boolean TryGetMemberEvent(System.String name, out System.Dynamic.ComEventDesc @event);
        extern internal System.Boolean TryGetMemberMethodExplicit(System.String name, out System.Dynamic.ComMethodDesc method);
        extern internal System.Boolean TryGetPropertySetterExplicit(System.String name, out System.Dynamic.ComMethodDesc method, System.Type limitType, System.Boolean holdsNull);
        extern internal override System.Collections.Generic.IList<System.String> GetMemberNames(System.Boolean dataOnly);
        extern internal override System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<System.String, System.Object>> GetMembers(System.Collections.Generic.IEnumerable<System.String> names);
        extern internal System.Boolean TryGetPropertySetter(System.String name, out System.Dynamic.ComMethodDesc method, System.Type limitType, System.Boolean holdsNull);
    }
}

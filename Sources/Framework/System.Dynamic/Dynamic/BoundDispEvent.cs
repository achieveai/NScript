namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class BoundDispEvent : System.Dynamic.DynamicObject {
        extern internal BoundDispEvent(System.Object rcw, System.Guid sourceIid, System.Int32 dispid);
        extern public override System.Boolean TryBinaryOperation(System.Dynamic.BinaryOperationBinder binder, System.Object handler, out System.Object result);
    }
}

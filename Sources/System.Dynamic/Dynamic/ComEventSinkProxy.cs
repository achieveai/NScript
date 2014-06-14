namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class ComEventSinkProxy : System.Runtime.Remoting.Proxies.RealProxy {
        extern public ComEventSinkProxy(System.Dynamic.ComEventSink sink, System.Guid sinkIid);
        extern public override System.IntPtr SupportsInterface(ref System.Guid iid);
        extern public override System.Runtime.Remoting.Messaging.IMessage Invoke(System.Runtime.Remoting.Messaging.IMessage msg);
    }
}

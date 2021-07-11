namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class ComEventSinksContainer : System.Collections.Generic.List<System.Dynamic.ComEventSink>, System.IDisposable {
        extern public static System.Dynamic.ComEventSinksContainer FromRuntimeCallableWrapper(System.Object rcw, System.Boolean createIfNotFound);
        extern public void  Dispose();
        extern protected override void  Finalize();
    }
}

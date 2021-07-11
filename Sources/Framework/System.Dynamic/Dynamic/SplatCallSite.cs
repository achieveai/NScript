namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class SplatCallSite : System.Object {
        internal readonly System.Object _callable;
        internal System.Runtime.CompilerServices.CallSite<System.Func<System.Runtime.CompilerServices.CallSite, System.Object, System.Object[], System.Object>> _site;
        extern internal SplatCallSite(System.Object callable);
        extern internal System.Object Invoke(System.Object[] args);
    }
}

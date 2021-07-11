namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal interface IProvideClassInfo {
        void  GetClassInfo(out System.IntPtr info);
    }
}

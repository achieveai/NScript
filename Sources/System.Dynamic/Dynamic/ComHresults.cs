namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class ComHresults : System.Object {
        internal const System.Int32 S_OK = 0;
        internal const System.Int32 CONNECT_E_NOCONNECTION = -2147220992;
        internal const System.Int32 DISP_E_UNKNOWNINTERFACE = -2147352575;
        internal const System.Int32 DISP_E_MEMBERNOTFOUND = -2147352573;
        internal const System.Int32 DISP_E_PARAMNOTFOUND = -2147352572;
        internal const System.Int32 DISP_E_TYPEMISMATCH = -2147352571;
        internal const System.Int32 DISP_E_UNKNOWNNAME = -2147352570;
        internal const System.Int32 DISP_E_NONAMEDARGS = -2147352569;
        internal const System.Int32 DISP_E_BADVARTYPE = -2147352568;
        internal const System.Int32 DISP_E_EXCEPTION = -2147352567;
        internal const System.Int32 DISP_E_OVERFLOW = -2147352566;
        internal const System.Int32 DISP_E_BADINDEX = -2147352565;
        internal const System.Int32 DISP_E_UNKNOWNLCID = -2147352564;
        internal const System.Int32 DISP_E_ARRAYISLOCKED = -2147352563;
        internal const System.Int32 DISP_E_BADPARAMCOUNT = -2147352562;
        internal const System.Int32 DISP_E_PARAMNOTOPTIONAL = -2147352561;
        internal const System.Int32 E_NOINTERFACE = -2147467262;
        internal const System.Int32 E_FAIL = -2147467259;
        internal const System.Int32 TYPE_E_LIBNOTREGISTERED = -2147319779;
        extern internal static System.Boolean IsSuccess(System.Int32 hresult);
    }
}

namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal struct Variant {
        extern public System.Runtime.InteropServices.VarEnum VariantType{ get; set; }
        extern public System.SByte AsI1{ get; set; }
        extern public System.Int16 AsI2{ get; set; }
        extern public System.Int32 AsI4{ get; set; }
        extern public System.Int64 AsI8{ get; set; }
        extern public System.Byte AsUi1{ get; set; }
        extern public System.UInt16 AsUi2{ get; set; }
        extern public System.UInt32 AsUi4{ get; set; }
        extern public System.UInt64 AsUi8{ get; set; }
        extern public System.IntPtr AsInt{ get; set; }
        extern public System.UIntPtr AsUint{ get; set; }
        extern public System.Boolean AsBool{ get; set; }
        extern public System.Int32 AsError{ get; set; }
        extern public System.Single AsR4{ get; set; }
        extern public System.Double AsR8{ get; set; }
        extern public System.Decimal AsDecimal{ get; set; }
        extern public System.Decimal AsCy{ get; set; }
        extern public System.DateTime AsDate{ get; set; }
        extern public System.String AsBstr{ get; set; }
        extern public System.Object AsUnknown{ get; set; }
        extern public System.Object AsDispatch{ get; set; }
        extern public System.Object AsVariant{ get; set; }
        extern public override System.String ToString();
        extern internal static System.Boolean IsPrimitiveType(System.Runtime.InteropServices.VarEnum varEnum);
        extern public System.Object ToObject();
        extern public void  Clear();
        extern public void  SetAsNull();
        extern public void  SetAsIConvertible(System.IConvertible value);
        extern public void  SetAsByrefI1(ref System.SByte value);
        extern public void  SetAsByrefI2(ref System.Int16 value);
        extern public void  SetAsByrefI4(ref System.Int32 value);
        extern public void  SetAsByrefI8(ref System.Int64 value);
        extern public void  SetAsByrefUi1(ref System.Byte value);
        extern public void  SetAsByrefUi2(ref System.UInt16 value);
        extern public void  SetAsByrefUi4(ref System.UInt32 value);
        extern public void  SetAsByrefUi8(ref System.UInt64 value);
        extern public void  SetAsByrefInt(ref System.IntPtr value);
        extern public void  SetAsByrefUint(ref System.UIntPtr value);
        extern public void  SetAsByrefBool(ref System.Int16 value);
        extern public void  SetAsByrefError(ref System.Int32 value);
        extern public void  SetAsByrefR4(ref System.Single value);
        extern public void  SetAsByrefR8(ref System.Double value);
        extern public void  SetAsByrefDecimal(ref System.Decimal value);
        extern public void  SetAsByrefCy(ref System.Int64 value);
        extern public void  SetAsByrefDate(ref System.Double value);
        extern public void  SetAsByrefBstr(ref System.IntPtr value);
        extern public void  SetAsByrefUnknown(ref System.IntPtr value);
        extern public void  SetAsByrefDispatch(ref System.IntPtr value);
        extern public void  SetAsByrefVariant(ref System.Dynamic.Variant value);
        extern public void  SetAsByrefVariantIndirect(ref System.Dynamic.Variant value);
        extern internal static System.Reflection.PropertyInfo GetAccessor(System.Runtime.InteropServices.VarEnum varType);
        extern internal static System.Reflection.MethodInfo GetByrefSetter(System.Runtime.InteropServices.VarEnum varType);
    }
}

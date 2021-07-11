namespace System.Dynamic
{
    [Obsolete, System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal sealed class SRCategoryAttribute : System.ComponentModel.CategoryAttribute {
        extern public SRCategoryAttribute(System.String category);
        extern protected override System.String GetLocalizedString(System.String value);
    }
}

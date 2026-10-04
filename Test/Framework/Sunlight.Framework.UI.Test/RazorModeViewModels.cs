namespace Sunlight.Framework.UI.Test
{
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI.Attributes;

    /// <summary>
    /// Plain DTO row with AUTO-properties only (issue #102). Deliberately not
    /// observable and with no explicit backing fields, so the compiler-generated
    /// "&lt;X&gt;k__BackingField" fields are what the skin getters must read.
    /// </summary>
    public sealed class RazorModeRow
    {
        public string Name { get; set; }
        public bool IsEditable { get; set; }
        public bool HasDescription { get; set; }
    }

    public class RazorModeVM : ObservableObject
    {
        [AutoFire] public ObservableCollection<RazorModeRow> Rows { get; set; }
        [AutoFire] public ObservableCollection<string> Names { get; set; }
        [AutoFire] public bool Flag { get; set; }
        [AutoFire] public bool Other { get; set; }
        [AutoFire] public string Name { get; set; }
    }
}

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

        // #104 "restore full expression support": a numeric property for / and %, a nested object
        // for deep instance paths (Model.Lead.Name), and a nullable string for ?? ; Decorate is an
        // instance method invoked from a binding (Model.Decorate(Model.Name)).
        [AutoFire] public int Count { get; set; }
        [AutoFire] public RazorModeRow Lead { get; set; }
        [AutoFire] public string Nick { get; set; }

        public string Decorate(string value)
        {
            return "d-" + value;
        }
    }
}

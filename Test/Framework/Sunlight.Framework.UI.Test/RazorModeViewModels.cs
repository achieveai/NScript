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

    /// <summary>
    /// Observable child used to exercise chained-path bindings (Model.Child.Leaf): the leaf
    /// property raises PropertyChanged, so a correct chained subscription updates when the leaf
    /// changes and re-targets when the whole child object is replaced.
    /// </summary>
    public class RazorModeChild : ObservableObject
    {
        [AutoFire] public string Leaf { get; set; }

        // Chained loop source: @foreach (var w in Model.Child.Items). Items are observable so the
        // test can prove the loop variable was typed through the chain (w.Leaf updates live).
        [AutoFire] public ObservableCollection<RazorModeChild> Items { get; set; }

        // Tags is read from C#; TagsView is a computed getter read ONLY by a nested loop
        // (@foreach (var t in c.TagsView)) and must survive dead-code elimination.
        [AutoFire] public ObservableCollection<string> Tags { get; set; }

        public ObservableCollection<string> TagsView
        {
            get { return this.Tags; }
        }
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

        // Observable child for chained-path binding (Model.Child.Leaf) and the chained loop
        // source (Model.Child.Items).
        [AutoFire] public RazorModeChild Child { get; set; }

        // Outer loop whose item template loops over a computed getter of the item (c.TagsView).
        [AutoFire] public ObservableCollection<RazorModeChild> Children { get; set; }

        // A computed getter read ONLY as a @foreach source (@foreach (var rv in Model.RowsView)),
        // never from C#. Without loop-source getter retention it is dead-code-eliminated and
        // mounting throws "get_rowsView is not a function".
        public ObservableCollection<RazorModeRow> RowsView
        {
            get { return this.Rows; }
        }

        public string Decorate(string value)
        {
            return "d-" + value;
        }

        // Issue #82: a computed getter read ONLY by the skin (@Model.ReproComputed), never from C#.
        // Without getter retention it is dead-code-eliminated and the emitted binding getter calls a
        // missing function at mount.
        public string ReproComputed
        {
            get { return "rc-" + this.Name; }
        }
    }
}

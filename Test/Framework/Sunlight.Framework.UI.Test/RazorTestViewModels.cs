namespace Sunlight.Framework.UI.Test
{
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI.Attributes;
    using System.Web.Html;

    /// <summary>
    /// ViewModels specifically for Razor skin template browser tests.
    /// </summary>
    public class RazorTestVM : ObservableObject
    {
        private string draft;

        public int DraftSetCount;

        public string Draft
        {
            get { return this.draft; }
            set
            {
                this.DraftSetCount++;
                if (this.draft == value) return;
                this.draft = value;
                this.FirePropertyChanged("Draft");
            }
        }
        [AutoFire] public string PickedName { get; set; }

        public void Pick(RazorItemVM item)
        {
            this.PickedName = item.Name;
        }

        [AutoFire] public RazorItemVM Child { get; set; }
        [AutoFire] public string Name { get; set; }
        [AutoFire] public bool IsActive { get; set; }
        [AutoFire] public string CssClass { get; set; }
        [AutoFire] public int Count { get; set; }
        [AutoFire] public ObservableCollection<RazorItemVM> Items { get; set; }

        public bool ClickFired;

        public void OnClick()
        {
            this.ClickFired = true;
        }

        public void OnDomClick(Element elem, ElementEvent evt)
        {
            this.ClickFired = true;
            this.ClickCount++;
        }

        [AutoFire] public int Price { get; set; }
        [AutoFire] public int Quantity { get; set; }
        [AutoFire] public string DisplayStyle { get; set; }
        [AutoFire] public string Title { get; set; }
        [AutoFire] public bool ShowDetails { get; set; }
        [AutoFire] public int ClickCount { get; set; }

        public void IncrementClick()
        {
            this.ClickCount = this.ClickCount + 1;
        }
    }

    public class RazorItemVM : ObservableObject
    {
        [AutoFire] public string Name { get; set; }
        [AutoFire] public bool IsComplete { get; set; }
        [AutoFire] public string Status { get; set; }
        [AutoFire] public int SelectCount { get; set; }

        public void Select()
        {
            this.SelectCount++;
        }
    }

    /// <summary>
    /// Non-observable VM for OneTime binding tests.
    /// </summary>
    public class RazorPlainVM
    {
        public string AppVersion { get; set; }
        public bool IsStatic { get; set; }
    }
}

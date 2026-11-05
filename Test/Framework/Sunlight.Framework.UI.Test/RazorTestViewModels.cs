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

        /// <summary>
        /// Referenced only from an @if branch inside a @foreach (RazorGateOnlyHandler).
        /// Must not be dead-code-eliminated just because no top-level binding names it.
        /// </summary>
        public void OnGateOnlyClick(Element elem, ElementEvent evt)
        {
            this.SelectCount += 10;
        }

        public int FirstChanges;
        public int SecondChanges;
        public int KeyDowns;

        public void OnKeyDown(Element elem, ElementEvent evt)
        {
            this.KeyDowns++;
        }

        public void OnFirstChange(Element elem, ElementEvent evt)
        {
            this.FirstChanges++;
        }

        public void OnSecondChange(Element elem, ElementEvent evt)
        {
            this.SecondChanges++;
        }

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

    /// <summary>
    /// AgentWorkBench binder-probe item (issues #131-#135, #140).
    /// </summary>
    public class ProbeItem : ObservableObject
    {
        [AutoFire] public string Name { get; set; }
        [AutoFire] public string Hint { get; set; }
        [AutoFire] public string Cls { get; set; }
        [AutoFire] public bool Flag { get; set; }
        public int Clicks;

        public ProbeItem(string name)
        {
            this.Name = name;
            this.Hint = "hint-" + name;
            this.Cls = "cls-" + name;
        }

        public void Select(Element elem, ElementEvent evt)
        {
            this.Clicks++;
        }
    }

    /// <summary>
    /// AgentWorkBench binder-probe view model (issues #131-#135, #140).
    /// </summary>
    public class ProbeVm : ObservableObject
    {
        [AutoFire] public string Cls { get; set; }
        [AutoFire] public string Text { get; set; }
        [AutoFire] public string Other { get; set; }
        [AutoFire] public bool Flag { get; set; }
        [AutoFire] public bool Busy { get; set; }
        [AutoFire] public string Choice { get; set; }
        [AutoFire] public int Count { get; set; }
        [AutoFire] public string Maybe { get; set; }
        [AutoFire] public string[] Names { get; set; }
        [AutoFire] public ProbeItem[] Arr { get; set; }
        [AutoFire] public System.Collections.Generic.List<ProbeItem> List { get; set; }
        public ObservableCollection<ProbeItem> Items { get; private set; }
        public int OwnChanges;
        public int GateClicks;
        public int AfterClicks;

        public void OnOwnChange(Element elem, ElementEvent evt)
        {
            this.OwnChanges++;
        }

        public void OnGateClick(Element elem, ElementEvent evt)
        {
            this.GateClicks++;
        }

        public void OnAfterClick(Element elem, ElementEvent evt)
        {
            this.AfterClicks++;
        }

        public ProbeVm()
        {
            this.Cls = "dyn-a";
            this.Text = "t1";
            this.Other = "o1";
            this.Items = new ObservableCollection<ProbeItem>();
        }
    }
}

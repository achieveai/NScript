namespace Sunlight.Framework.UI.Test
{
    using Sunlight.Framework.Observables;
    using System.Web.Html;

    /// <summary>
    /// ViewModels specifically for Razor skin template browser tests.
    /// </summary>
    public class RazorTestVM : ObservableObject
    {
        private string name;
        private bool isActive;
        private string cssClass;
        private int count;
        private ObservableCollection<RazorItemVM> items;

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    base.FirePropertyChanged("Name");
                }
            }
        }

        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                if (this.isActive != value)
                {
                    this.isActive = value;
                    base.FirePropertyChanged("IsActive");
                }
            }
        }

        public string CssClass
        {
            get { return this.cssClass; }
            set
            {
                if (this.cssClass != value)
                {
                    this.cssClass = value;
                    base.FirePropertyChanged("CssClass");
                }
            }
        }

        public int Count
        {
            get { return this.count; }
            set
            {
                if (this.count != value)
                {
                    this.count = value;
                    base.FirePropertyChanged("Count");
                }
            }
        }

        public ObservableCollection<RazorItemVM> Items
        {
            get { return this.items; }
            set
            {
                if (this.items != value)
                {
                    this.items = value;
                    base.FirePropertyChanged("Items");
                }
            }
        }

        public bool ClickFired;

        public void OnClick()
        {
            this.ClickFired = true;
        }

        public void OnDomClick(Element elem, ElementEvent evt)
        {
            this.ClickFired = true;
        }
    }

    public class RazorItemVM : ObservableObject
    {
        private string name;
        private bool isComplete;

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    base.FirePropertyChanged("Name");
                }
            }
        }

        public bool IsComplete
        {
            get { return this.isComplete; }
            set
            {
                if (this.isComplete != value)
                {
                    this.isComplete = value;
                    base.FirePropertyChanged("IsComplete");
                }
            }
        }
    }

    /// <summary>
    /// Non-observable VM for OneTime binding tests.
    /// </summary>
    public class RazorPlainVM
    {
        public string AppVersion { get; set; }
    }
}

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
        private int price;
        private int quantity;
        private string displayStyle;
        private string title;
        private bool showDetails;
        private int clickCount;

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

        public int Price
        {
            get { return this.price; }
            set
            {
                if (this.price != value)
                {
                    this.price = value;
                    base.FirePropertyChanged("Price");
                }
            }
        }

        public int Quantity
        {
            get { return this.quantity; }
            set
            {
                if (this.quantity != value)
                {
                    this.quantity = value;
                    base.FirePropertyChanged("Quantity");
                }
            }
        }

        public string DisplayStyle
        {
            get { return this.displayStyle; }
            set
            {
                if (this.displayStyle != value)
                {
                    this.displayStyle = value;
                    base.FirePropertyChanged("DisplayStyle");
                }
            }
        }

        public string Title
        {
            get { return this.title; }
            set
            {
                if (this.title != value)
                {
                    this.title = value;
                    base.FirePropertyChanged("Title");
                }
            }
        }

        public bool ShowDetails
        {
            get { return this.showDetails; }
            set
            {
                if (this.showDetails != value)
                {
                    this.showDetails = value;
                    base.FirePropertyChanged("ShowDetails");
                }
            }
        }

        public int ClickCount
        {
            get { return this.clickCount; }
            set
            {
                if (this.clickCount != value)
                {
                    this.clickCount = value;
                    base.FirePropertyChanged("ClickCount");
                }
            }
        }

        public void IncrementClick()
        {
            this.ClickCount = this.ClickCount + 1;
        }
    }

    public class RazorItemVM : ObservableObject
    {
        private string name;
        private bool isComplete;
        private string status;

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

        public string Status
        {
            get { return this.status; }
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    base.FirePropertyChanged("Status");
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
        public bool IsStatic { get; set; }
    }
}

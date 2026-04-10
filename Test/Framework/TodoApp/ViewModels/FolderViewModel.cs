namespace TodoApp.ViewModels
{
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Observable wrapper for a todo folder/list. Handles selection state and display.
    /// </summary>
    public class FolderViewModel : ObservableObject
    {
        private string id;
        private string name;
        private string icon;
        private int todoCount;
        private bool isSelected;
        private bool isSystem;
        private string systemType;
        private string cssClass;

        /// <summary>
        /// Reference to the parent AppViewModel for selection callbacks.
        /// </summary>
        private AppViewModel appViewModel;

        public FolderViewModel(AppViewModel appViewModel)
        {
            this.appViewModel = appViewModel;
            this.cssClass = AppShellCss.FolderItem;
            this.UpdateComputedProperties();
        }

        public string Id
        {
            get { return this.id; }
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    base.FirePropertyChanged("Id");
                }
            }
        }

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

        public string Icon
        {
            get { return this.icon; }
            set
            {
                if (this.icon != value)
                {
                    this.icon = value;
                    base.FirePropertyChanged("Icon");
                }
            }
        }

        public int TodoCount
        {
            get { return this.todoCount; }
            set
            {
                if (this.todoCount != value)
                {
                    this.todoCount = value;
                    base.FirePropertyChanged("TodoCount");
                }
            }
        }

        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    base.FirePropertyChanged("IsSelected");
                    this.UpdateComputedProperties();
                }
            }
        }

        public bool IsSystem
        {
            get { return this.isSystem; }
            set
            {
                if (this.isSystem != value)
                {
                    this.isSystem = value;
                    base.FirePropertyChanged("IsSystem");
                }
            }
        }

        public string SystemType
        {
            get { return this.systemType; }
            set
            {
                if (this.systemType != value)
                {
                    this.systemType = value;
                    base.FirePropertyChanged("SystemType");
                }
            }
        }

        /// <summary>
        /// CSS class for the folder row, derived from selection state.
        /// </summary>
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

        private void UpdateComputedProperties()
        {
            if (this.isSelected)
                this.CssClass = AppShellCss.FolderItem + " " + AppShellCss.Selected;
            else
                this.CssClass = AppShellCss.FolderItem;
        }

        /// <summary>
        /// Notifies the parent AppViewModel to select this folder.
        /// </summary>
        public void OnSelect()
        {
            this.appViewModel.OnSelectFolder(this);
        }

    }
}

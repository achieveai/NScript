namespace TodoApp.ViewModels
{
    using Sunlight.Framework.Observables;

    /// <summary>
    /// A single folder tag shown as a chip in the detail pane.
    /// Each tag has a name and a remove action that clears its membership.
    /// </summary>
    public class FolderTagViewModel : ObservableObject
    {
        private string name;
        private string tagType;
        private AppViewModel appViewModel;

        public FolderTagViewModel(AppViewModel parent)
        {
            this.appViewModel = parent;
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

        /// <summary>
        /// Type of tag: "folder" for physical folder, "myday", "important", "completed" for virtual.
        /// </summary>
        public string TagType
        {
            get { return this.tagType; }
            set
            {
                if (this.tagType != value)
                {
                    this.tagType = value;
                    base.FirePropertyChanged("TagType");
                }
            }
        }

        /// <summary>
        /// Removes this specific folder membership from the selected todo.
        /// </summary>
        public void Remove(object e, object ev)
        {
            this.appViewModel.RemoveFolderTag(this.tagType);
        }
    }
}

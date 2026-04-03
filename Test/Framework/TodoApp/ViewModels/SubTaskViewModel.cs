namespace TodoApp.ViewModels
{
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Observable wrapper for a SubTask (step within a todo item).
    /// </summary>
    public class SubTaskViewModel : ObservableObject
    {
        private string id;
        private string title;
        private bool isCompleted;
        private string cssClass;
        private string checkboxClass;

        public SubTaskViewModel()
        {
            this.cssClass = "subtask-item";
            this.checkboxClass = "btn-check";
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

        public bool IsCompleted
        {
            get { return this.isCompleted; }
            set
            {
                if (this.isCompleted != value)
                {
                    this.isCompleted = value;
                    base.FirePropertyChanged("IsCompleted");
                    this.UpdateComputedProperties();
                }
            }
        }

        /// <summary>
        /// CSS class for the subtask row, derived from completion state.
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

        /// <summary>
        /// CSS class for the checkbox button, derived from completion state.
        /// </summary>
        public string CheckboxClass
        {
            get { return this.checkboxClass; }
            set
            {
                if (this.checkboxClass != value)
                {
                    this.checkboxClass = value;
                    base.FirePropertyChanged("CheckboxClass");
                }
            }
        }

        private void UpdateComputedProperties()
        {
            if (this.isCompleted)
                this.CssClass = "subtask-item completed";
            else
                this.CssClass = "subtask-item";

            this.CheckboxClass = this.isCompleted ? "btn-check checked" : "btn-check";
        }

        /// <summary>
        /// Toggles the completed state of this subtask.
        /// TODO: Subtask persistence is not yet implemented — changes are lost on reload.
        /// </summary>
        public void ToggleComplete()
        {
            this.IsCompleted = !this.IsCompleted;
        }

    }
}

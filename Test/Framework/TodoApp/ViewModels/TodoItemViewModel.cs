namespace TodoApp.ViewModels
{
    using System.Runtime.CompilerServices;
    using System.Web.Html;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Observable wrapper for a TodoItem. Holds display state and handles user interactions.
    /// </summary>
    public class TodoItemViewModel : ObservableObject
    {
        private static int nextId = 0;

        private string id;
        private string folderId;
        private string title;
        private bool isCompleted;
        private bool isImportant;
        private bool isMyDay;
        private string dueDate;
        private string notes;
        private string createdAt;
        private bool isSelected;
        private ObservableCollection<SubTaskViewModel> subTasks;
        private string cssClass;
        private string checkboxClass;
        private string starClass;
        private string starText;
        private bool hasDueDate;
        private string dueDateDisplay;

        /// <summary>
        /// Reference to the parent AppViewModel for selection callbacks.
        /// </summary>
        private AppViewModel appViewModel;

        public TodoItemViewModel(AppViewModel appViewModel)
        {
            this.appViewModel = appViewModel;
            this.subTasks = new ObservableCollection<SubTaskViewModel>();
            this.cssClass = AppShellCss.TodoItem;
            this.checkboxClass = AppShellCss.BtnCheck;
            this.starClass = AppShellCss.Star;
            this.starText = "☆";
            this.hasDueDate = false;
            this.dueDateDisplay = "";
            this.UpdateComputedProperties();
        }

        /// <summary>
        /// Generates a unique string ID for new todo items.
        /// </summary>
        public static string GenerateId()
        {
            nextId = nextId + 1;
            return "id_" + nextId.ToString();
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

        public string FolderId
        {
            get { return this.folderId; }
            set
            {
                if (this.folderId != value)
                {
                    this.folderId = value;
                    base.FirePropertyChanged("FolderId");
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

        public bool IsImportant
        {
            get { return this.isImportant; }
            set
            {
                if (this.isImportant != value)
                {
                    this.isImportant = value;
                    base.FirePropertyChanged("IsImportant");
                    this.UpdateComputedProperties();
                }
            }
        }

        public bool IsMyDay
        {
            get { return this.isMyDay; }
            set
            {
                if (this.isMyDay != value)
                {
                    this.isMyDay = value;
                    base.FirePropertyChanged("IsMyDay");
                }
            }
        }

        public string DueDate
        {
            get { return this.dueDate; }
            set
            {
                if (this.dueDate != value)
                {
                    this.dueDate = value;
                    base.FirePropertyChanged("DueDate");
                    this.UpdateComputedProperties();
                }
            }
        }

        public string Notes
        {
            get { return this.notes; }
            set
            {
                if (this.notes != value)
                {
                    this.notes = value;
                    base.FirePropertyChanged("Notes");
                }
            }
        }

        public string CreatedAt
        {
            get { return this.createdAt; }
            set
            {
                if (this.createdAt != value)
                {
                    this.createdAt = value;
                    base.FirePropertyChanged("CreatedAt");
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

        public ObservableCollection<SubTaskViewModel> SubTasks
        {
            get { return this.subTasks; }
            set
            {
                if (this.subTasks != value)
                {
                    this.subTasks = value;
                    base.FirePropertyChanged("SubTasks");
                }
            }
        }

        /// <summary>
        /// CSS class for the todo row, derived from completion and selection state.
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
        /// CSS class for the checkbox button.
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

        /// <summary>
        /// CSS class for the star/importance icon.
        /// </summary>
        public string StarClass
        {
            get { return this.starClass; }
            set
            {
                if (this.starClass != value)
                {
                    this.starClass = value;
                    base.FirePropertyChanged("StarClass");
                }
            }
        }

        /// <summary>
        /// Text glyph for the star icon (★ filled, ☆ empty).
        /// </summary>
        public string StarText
        {
            get { return this.starText; }
            set
            {
                if (this.starText != value)
                {
                    this.starText = value;
                    base.FirePropertyChanged("StarText");
                }
            }
        }

        /// <summary>
        /// True when this item has a due date set.
        /// </summary>
        public bool HasDueDate
        {
            get { return this.hasDueDate; }
            set
            {
                if (this.hasDueDate != value)
                {
                    this.hasDueDate = value;
                    base.FirePropertyChanged("HasDueDate");
                }
            }
        }

        /// <summary>
        /// Formatted due date string for display.
        /// </summary>
        public string DueDateDisplay
        {
            get { return this.dueDateDisplay; }
            set
            {
                if (this.dueDateDisplay != value)
                {
                    this.dueDateDisplay = value;
                    base.FirePropertyChanged("DueDateDisplay");
                }
            }
        }

        private void UpdateComputedProperties()
        {
            // CssClass
            string css = AppShellCss.TodoItem;
            if (this.isSelected) css = css + " " + AppShellCss.Selected;
            if (this.isCompleted) css = css + " " + AppShellCss.Completed;
            this.CssClass = css;

            // CheckboxClass
            this.CheckboxClass = this.isCompleted ? AppShellCss.BtnCheck + " " + AppShellCss.Checked : AppShellCss.BtnCheck;

            // StarClass / StarText
            this.StarClass = this.isImportant ? AppShellCss.Star + " " + AppShellCss.Important : AppShellCss.Star;
            this.StarText = this.isImportant ? "★" : "☆";

            // HasDueDate
            this.HasDueDate = this.dueDate != null && this.dueDate != "";

            // DueDateDisplay
            if (this.dueDate != null && this.dueDate != "")
                this.DueDateDisplay = "Due: " + this.dueDate;
            else
                this.DueDateDisplay = "";
        }

        /// <summary>
        /// Toggles the completed state of this todo item and persists the change.
        /// Stops event propagation to prevent parent onclick (OnSelectTodo) from firing.
        /// Parameters: e = DOM element, ev = DOM Event (passed by NScript event binder).
        /// </summary>
        public void ToggleComplete(object e, object ev)
        {
            if (ev != null) StopPropagation(ev);
            this.IsCompleted = !this.IsCompleted;
            this.appViewModel.SaveTodo(this);
        }

        /// <summary>
        /// Toggles the importance (star) state of this todo item and persists the change.
        /// Stops event propagation to prevent parent onclick (OnSelectTodo) from firing.
        /// Parameters: e = DOM element, ev = DOM Event (passed by NScript event binder).
        /// </summary>
        public void ToggleImportant(object e, object ev)
        {
            if (ev != null) StopPropagation(ev);
            this.IsImportant = !this.IsImportant;
            this.appViewModel.SaveTodo(this);
        }

        [Script("ev.stopPropagation();")]
        private static extern void StopPropagation(object ev);

        /// <summary>
        /// Toggles whether this todo is included in My Day and persists the change.
        /// </summary>
        public void ToggleMyDay()
        {
            this.IsMyDay = !this.IsMyDay;
            this.appViewModel.SaveTodo(this);
        }

        /// <summary>
        /// Notifies the parent AppViewModel to select this todo item.
        /// </summary>
        public void OnSelect()
        {
            this.appViewModel.OnSelectTodo(this);
        }

        /// <summary>
        /// Initiates drag of this todo item. Called by the sub-control's ondragstart.
        /// </summary>
        public void OnDragStart(object e, object ev)
        {
            this.appViewModel.OnDragStart(this);
        }

        /// <summary>
        /// Updates the title when the user edits it in the detail pane input.
        /// </summary>
        public void OnTitleChange(Element e, ElementEvent ev)
        {
            InputElement input = (InputElement)e;
            string newTitle = input.Value;
            if (newTitle != null && newTitle != "")
            {
                this.Title = newTitle;
                this.appViewModel.SaveTodo(this);
            }
        }

        /// <summary>
        /// Requests the parent AppViewModel to delete this todo item.
        /// </summary>
        public void DeleteTodo()
        {
            this.appViewModel.DeleteTodo(this);
        }

        /// <summary>
        /// Adds a new empty subtask to this todo item.
        /// </summary>
        public void AddSubTask()
        {
            var sub = new SubTaskViewModel();
            sub.Id = TodoItemViewModel.GenerateId();
            sub.Title = "New step";
            sub.IsCompleted = false;
            sub.BindToTodo(this.appViewModel, this);
            this.SubTasks.Add(sub);
            this.appViewModel.SaveTodo(this);
        }

    }
}

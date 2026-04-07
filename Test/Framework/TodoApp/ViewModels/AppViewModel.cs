namespace TodoApp.ViewModels
{
    using System.Runtime.CompilerServices;
    using System.Web.Html;
    using Sunlight.Framework.Observables;
    using TodoApp.Services;

    /// <summary>
    /// Root application ViewModel for the Microsoft To Do clone.
    /// Manages folder list, todo list, and detail pane state.
    /// </summary>
    public class AppViewModel : ObservableObject
    {
        private bool isLeftPaneCollapsed;
        private bool isRightPaneCollapsed;
        private string selectedFolderName;
        private ObservableCollection<FolderViewModel> folders;
        private ObservableCollection<TodoItemViewModel> currentTodos;
        private FolderViewModel selectedFolder;
        private TodoItemViewModel selectedTodo;
        private string newTodoTitle;
        private TodoDataService dataService;

        /// <summary>
        /// All todo items across all folders, used for filtering.
        /// </summary>
        private ObservableCollection<TodoItemViewModel> allTodos;

        /// <summary>
        /// Reference to the default "Tasks" system folder, used after data load.
        /// </summary>
        private FolderViewModel defaultTasksFolder;

        private string leftPaneClass;
        private string rightPaneClass;
        private string detailTitle;

        private ObservableCollection<TodoItemViewModel> completedCurrentTodos;
        private ObservableCollection<SubTaskViewModel> detailSubTasks;
        private bool isCompletedSectionVisible;
        private bool isCompletedSectionExpanded;
        private int completedCount;
        private TodoItemViewModel draggedTodo;

        private string completedSectionClass;
        private ObservableCollection<FolderTagViewModel> detailFolderTags;

        public AppViewModel()
        {
            this.isRightPaneCollapsed = true;
            this.leftPaneClass = AppShellCss.PaneLeft;
            this.rightPaneClass = AppShellCss.PaneRight + " " + AppShellCss.Collapsed;
            this.detailTitle = "";
            this.allTodos = new ObservableCollection<TodoItemViewModel>();
            this.currentTodos = new ObservableCollection<TodoItemViewModel>();
            this.completedCurrentTodos = new ObservableCollection<TodoItemViewModel>();
            this.detailSubTasks = new ObservableCollection<SubTaskViewModel>();
            this.isCompletedSectionVisible = true;
            this.isCompletedSectionExpanded = false;
            this.completedCount = 0;
            this.completedSectionClass = AppShellCss.CompletedSection + " " + AppShellCss.Collapsed;
            this.detailFolderTags = new ObservableCollection<FolderTagViewModel>();
        }

        public bool IsLeftPaneCollapsed
        {
            get { return this.isLeftPaneCollapsed; }
            set
            {
                if (this.isLeftPaneCollapsed != value)
                {
                    this.isLeftPaneCollapsed = value;
                    base.FirePropertyChanged("IsLeftPaneCollapsed");
                }
            }
        }

        public bool IsRightPaneCollapsed
        {
            get { return this.isRightPaneCollapsed; }
            set
            {
                if (this.isRightPaneCollapsed != value)
                {
                    this.isRightPaneCollapsed = value;
                    base.FirePropertyChanged("IsRightPaneCollapsed");
                }
            }
        }

        public string SelectedFolderName
        {
            get { return this.selectedFolderName; }
            set
            {
                if (this.selectedFolderName != value)
                {
                    this.selectedFolderName = value;
                    base.FirePropertyChanged("SelectedFolderName");
                }
            }
        }

        public ObservableCollection<FolderViewModel> Folders
        {
            get { return this.folders; }
            set
            {
                if (this.folders != value)
                {
                    this.folders = value;
                    base.FirePropertyChanged("Folders");
                }
            }
        }

        public ObservableCollection<TodoItemViewModel> CurrentTodos
        {
            get { return this.currentTodos; }
            set
            {
                if (this.currentTodos != value)
                {
                    this.currentTodos = value;
                    base.FirePropertyChanged("CurrentTodos");
                }
            }
        }

        public FolderViewModel SelectedFolder
        {
            get { return this.selectedFolder; }
            set
            {
                if (this.selectedFolder != value)
                {
                    this.selectedFolder = value;
                    base.FirePropertyChanged("SelectedFolder");
                }
            }
        }

        public TodoItemViewModel SelectedTodo
        {
            get { return this.selectedTodo; }
            set
            {
                if (this.selectedTodo != value)
                {
                    this.selectedTodo = value;
                    base.FirePropertyChanged("SelectedTodo");
                    this.UpdateDetailProperties();
                }
            }
        }

        /// <summary>
        /// Title of the selected todo, displayed in the detail pane.
        /// Updated when SelectedTodo changes or the todo's title is edited.
        /// </summary>
        public string DetailTitle
        {
            get { return this.detailTitle; }
            set
            {
                if (this.detailTitle != value)
                {
                    this.detailTitle = value;
                    base.FirePropertyChanged("DetailTitle");
                }
            }
        }

        /// <summary>
        /// Collection of folder tag chips for the selected todo.
        /// Each tag represents a folder membership (physical or virtual) with its own remove action.
        /// </summary>
        public ObservableCollection<FolderTagViewModel> DetailFolderTags
        {
            get { return this.detailFolderTags; }
            set
            {
                if (this.detailFolderTags != value)
                {
                    this.detailFolderTags = value;
                    base.FirePropertyChanged("DetailFolderTags");
                }
            }
        }

        public ObservableCollection<TodoItemViewModel> CompletedCurrentTodos
        {
            get { return this.completedCurrentTodos; }
            set
            {
                if (this.completedCurrentTodos != value)
                {
                    this.completedCurrentTodos = value;
                    base.FirePropertyChanged("CompletedCurrentTodos");
                }
            }
        }

        public ObservableCollection<SubTaskViewModel> DetailSubTasks
        {
            get { return this.detailSubTasks; }
            set
            {
                if (this.detailSubTasks != value)
                {
                    this.detailSubTasks = value;
                    base.FirePropertyChanged("DetailSubTasks");
                }
            }
        }

        public bool IsCompletedSectionVisible
        {
            get { return this.isCompletedSectionVisible; }
            set
            {
                if (this.isCompletedSectionVisible != value)
                {
                    this.isCompletedSectionVisible = value;
                    base.FirePropertyChanged("IsCompletedSectionVisible");
                }
            }
        }

        public bool IsCompletedSectionExpanded
        {
            get { return this.isCompletedSectionExpanded; }
            set
            {
                if (this.isCompletedSectionExpanded != value)
                {
                    this.isCompletedSectionExpanded = value;
                    base.FirePropertyChanged("IsCompletedSectionExpanded");
                }
            }
        }

        public int CompletedCount
        {
            get { return this.completedCount; }
            set
            {
                if (this.completedCount != value)
                {
                    this.completedCount = value;
                    base.FirePropertyChanged("CompletedCount");
                }
            }
        }

        public string CompletedSectionClass
        {
            get { return this.completedSectionClass; }
            set
            {
                if (this.completedSectionClass != value)
                {
                    this.completedSectionClass = value;
                    base.FirePropertyChanged("CompletedSectionClass");
                }
            }
        }

        private void UpdateCompletedSectionClass()
        {
            if (!this.isCompletedSectionVisible)
                this.CompletedSectionClass = AppShellCss.CompletedSection + " " + AppShellCss.Hidden;
            else if (!this.isCompletedSectionExpanded)
                this.CompletedSectionClass = AppShellCss.CompletedSection + " " + AppShellCss.Collapsed;
            else
                this.CompletedSectionClass = AppShellCss.CompletedSection;
        }

        private void UpdateDetailProperties()
        {
            if (this.selectedTodo != null)
            {
                this.DetailTitle = this.selectedTodo.Title;
                this.DetailSubTasks = this.selectedTodo.SubTasks;
                this.RefreshFolderTags(this.selectedTodo);
            }
            else
            {
                this.DetailTitle = "";
                this.DetailSubTasks = new ObservableCollection<SubTaskViewModel>();
                this.DetailFolderTags = new ObservableCollection<FolderTagViewModel>();
            }
        }

        private void RefreshFolderTags(TodoItemViewModel todo)
        {
            var tags = new ObservableCollection<FolderTagViewModel>();

            if (todo != null)
            {
                // Physical folder
                string folderId = todo.FolderId;
                string folderName = "Tasks";
                if (folderId != null && folderId != "" && folderId != "tasks")
                {
                    for (int i = 0; i < this.Folders.Count; i++)
                    {
                        if (this.Folders[i].Id == folderId)
                        {
                            folderName = this.Folders[i].Name;
                            break;
                        }
                    }
                }
                var folderTag = new FolderTagViewModel(this);
                folderTag.Name = folderName;
                folderTag.TagType = "folder";
                tags.Add(folderTag);

                // Virtual memberships
                if (todo.IsMyDay)
                {
                    var tag = new FolderTagViewModel(this);
                    tag.Name = "My Day";
                    tag.TagType = "myday";
                    tags.Add(tag);
                }
                if (todo.IsImportant)
                {
                    var tag = new FolderTagViewModel(this);
                    tag.Name = "Important";
                    tag.TagType = "important";
                    tags.Add(tag);
                }
            }

            this.DetailFolderTags = tags;
        }

        public string NewTodoTitle
        {
            get { return this.newTodoTitle; }
            set
            {
                if (this.newTodoTitle != value)
                {
                    this.newTodoTitle = value;
                    base.FirePropertyChanged("NewTodoTitle");
                }
            }
        }

        /// <summary>
        /// The persistence service. Injected before InitializeWithData is called.
        /// </summary>
        public TodoDataService DataService
        {
            get { return this.dataService; }
            set { this.dataService = value; }
        }

        public string LeftPaneClass
        {
            get { return this.leftPaneClass; }
            set
            {
                if (this.leftPaneClass != value)
                {
                    this.leftPaneClass = value;
                    base.FirePropertyChanged("LeftPaneClass");
                }
            }
        }

        public string RightPaneClass
        {
            get { return this.rightPaneClass; }
            set
            {
                if (this.rightPaneClass != value)
                {
                    this.rightPaneClass = value;
                    base.FirePropertyChanged("RightPaneClass");
                }
            }
        }

        private void UpdatePaneClasses()
        {
            this.LeftPaneClass = this.isLeftPaneCollapsed ? AppShellCss.PaneLeft + " " + AppShellCss.Collapsed : AppShellCss.PaneLeft;
            this.RightPaneClass = this.isRightPaneCollapsed ? AppShellCss.PaneRight + " " + AppShellCss.Collapsed : AppShellCss.PaneRight;
        }

        /// <summary>
        /// Toggles the left sidebar panel visibility.
        /// </summary>
        public void ToggleLeftPane()
        {
            this.IsLeftPaneCollapsed = !this.IsLeftPaneCollapsed;
            this.UpdatePaneClasses();
        }

        public void ToggleRightPane()
        {
            this.IsRightPaneCollapsed = !this.IsRightPaneCollapsed;
            this.UpdatePaneClasses();
        }

        /// <summary>
        /// Selects a folder and refreshes the current todo list.
        /// </summary>
        public void OnSelectFolder(FolderViewModel folder)
        {
            if (this.selectedFolder != null)
            {
                this.selectedFolder.IsSelected = false;
            }

            this.SelectedFolder = folder;

            if (folder != null)
            {
                folder.IsSelected = true;
                this.SelectedFolderName = folder.Name;
                this.RefreshCurrentTodos();
            }
        }

        /// <summary>
        /// Selects a todo item and opens the detail pane.
        /// </summary>
        public void OnSelectTodo(TodoItemViewModel todo)
        {
            if (this.selectedTodo != null)
            {
                this.selectedTodo.IsSelected = false;
            }

            if (todo != null)
            {
                todo.IsSelected = true;
                // Open the right pane BEFORE setting SelectedTodo so the outer gate
                // renders the pane DOM before the inner gate tries to render detail content.
                this.IsRightPaneCollapsed = false;
                this.UpdatePaneClasses();
            }

            this.SelectedTodo = todo;
        }

        /// <summary>
        /// Handles keydown on the add-task input. On Enter, creates a new todo with the typed title.
        /// </summary>
        public void AddTodoOnEnter(Element e, ElementEvent ev)
        {
            if (ev.KeyCode != 13) return; // Enter key only

            InputElement input = (InputElement)e;
            string title = input.Value;
            if (title == null || title == "") return;

            this.AddTodoWithTitle(title);
            input.Value = "";
        }

        /// <summary>
        /// Adds a new todo item with the given title to the current folder.
        /// </summary>
        public void AddTodoWithTitle(string title)
        {
            if (this.selectedFolder == null) return;

            string folderId = this.selectedFolder.Id;

            // For system folders that show filtered views, assign to Tasks folder
            if (this.selectedFolder.IsSystem && this.selectedFolder.SystemType != "tasks")
            {
                folderId = "tasks";
            }

            var todo = new TodoItemViewModel(this);
            todo.Id = TodoItemViewModel.GenerateId();
            todo.Title = title;
            todo.IsCompleted = false;
            todo.IsImportant = false;
            todo.IsMyDay = false;
            todo.FolderId = folderId;

            this.allTodos.Add(todo);
            this.CurrentTodos.Add(todo);
            this.selectedFolder.TodoCount = this.selectedFolder.TodoCount + 1;

            this.SaveTodo(todo);
        }

        /// <summary>
        /// Adds a new todo item with default title (for backward compat / tests).
        /// </summary>
        public void AddTodo()
        {
            this.AddTodoWithTitle("New task");
        }

        /// <summary>
        /// Adds a subtask to the currently selected todo.
        /// </summary>
        public void AddSubTaskToSelected()
        {
            if (this.selectedTodo != null)
            {
                this.selectedTodo.AddSubTask();
            }
        }

        /// <summary>
        /// Deletes the currently selected todo.
        /// </summary>
        public void DeleteSelectedTodo()
        {
            if (this.selectedTodo != null)
            {
                this.DeleteTodo(this.selectedTodo);
            }
        }

        /// <summary>
        /// Toggles the collapsed/expanded state of the completed section.
        /// </summary>
        public void ToggleCompletedSection()
        {
            this.IsCompletedSectionExpanded = !this.IsCompletedSectionExpanded;
            this.UpdateCompletedSectionClass();
        }

        /// <summary>
        /// Handles onchange on the detail title input to sync edits back to the todo.
        /// </summary>
        public void OnDetailTitleChange(Element e, ElementEvent ev)
        {
            InputElement input = (InputElement)e;
            string newTitle = input.Value;
            if (this.selectedTodo != null && newTitle != null && newTitle != "")
            {
                this.selectedTodo.Title = newTitle;
                this.DetailTitle = newTitle;
                this.SaveTodo(this.selectedTodo);
            }
        }

        /// <summary>
        /// Handles onkeydown on the add-step input. Creates a subtask on Enter.
        /// </summary>
        public void AddSubTaskOnEnter(Element e, ElementEvent ev)
        {
            if (ev.KeyCode != 13) return;

            InputElement input = (InputElement)e;
            string title = input.Value;
            if (title == null || title == "") return;

            if (this.selectedTodo != null)
            {
                var sub = new SubTaskViewModel();
                sub.Id = TodoItemViewModel.GenerateId();
                sub.Title = title;
                sub.IsCompleted = false;
                this.selectedTodo.SubTasks.Add(sub);
                input.Value = "";
            }
        }

        /// <summary>
        /// Moves the selected todo to a different folder.
        /// </summary>
        public void MoveSelectedToFolder(FolderViewModel folder)
        {
            if (this.selectedTodo == null || folder == null) return;

            if (folder.IsSystem)
            {
                if (folder.SystemType == "myday")
                    this.selectedTodo.IsMyDay = true;
                else if (folder.SystemType == "important")
                    this.selectedTodo.IsImportant = true;
                else
                    this.selectedTodo.FolderId = folder.Id;
            }
            else
            {
                this.selectedTodo.FolderId = folder.Id;
            }

            this.SaveTodo(this.selectedTodo);
            this.RefreshFolderTags(this.selectedTodo);
        }

        /// <summary>
        /// Removes a specific folder membership from the selected todo.
        /// Called by FolderTagViewModel.Remove() with the tag type.
        /// </summary>
        public void RemoveFolderTag(string tagType)
        {
            if (this.selectedTodo == null) return;

            if (tagType == "myday")
                this.selectedTodo.IsMyDay = false;
            else if (tagType == "important")
                this.selectedTodo.IsImportant = false;
            else if (tagType == "folder")
                this.selectedTodo.FolderId = "tasks";

            this.SaveTodo(this.selectedTodo);
            this.RefreshCurrentTodos();
        }

        /// <summary>
        /// Removes all folder memberships, moving the todo back to plain Tasks.
        /// </summary>
        public void RemoveFromFolder()
        {
            if (this.selectedTodo == null) return;
            this.selectedTodo.FolderId = "tasks";
            this.selectedTodo.IsMyDay = false;
            this.selectedTodo.IsImportant = false;
            this.SaveTodo(this.selectedTodo);
            this.RefreshCurrentTodos();
        }

        /// <summary>
        /// Stores the dragged todo for drag-drop operations.
        /// </summary>
        public void OnDragStart(TodoItemViewModel todo)
        {
            this.draggedTodo = todo;
        }

        /// <summary>
        /// Handles a drop on a folder. Assigns the dragged todo to the target folder.
        /// </summary>
        public void OnDropToFolder(FolderViewModel folder)
        {
            if (this.draggedTodo == null || folder == null) return;

            if (folder.IsSystem)
            {
                if (folder.SystemType == "myday")
                    this.draggedTodo.IsMyDay = true;
                else if (folder.SystemType == "important")
                    this.draggedTodo.IsImportant = true;
                else
                    this.draggedTodo.FolderId = folder.Id;
            }
            else
            {
                this.draggedTodo.FolderId = folder.Id;
            }

            this.SaveTodo(this.draggedTodo);

            // Update the folder chip if the dragged todo is currently selected
            if (this.draggedTodo == this.selectedTodo)
            {
                this.RefreshFolderTags(this.draggedTodo);
            }

            this.draggedTodo = null;
        }

        /// <summary>
        /// Prevents the default behavior on dragover to allow drops.
        /// </summary>
        public void OnDragOver(object e, object ev)
        {
            PreventDefault(ev);
        }

        [Script("ev.preventDefault();")]
        private static extern void PreventDefault(object ev);

        /// <summary>
        /// Removes a todo from both the full list and the current view, then deletes from DB.
        /// Closes the detail pane if the deleted todo was selected.
        /// </summary>
        public void DeleteTodo(TodoItemViewModel todo)
        {
            this.allTodos.Remove(todo);
            this.CurrentTodos.Remove(todo);

            if (this.selectedFolder != null)
                this.selectedFolder.TodoCount = this.CurrentTodos.Count;

            if (this.selectedTodo == todo)
            {
                this.SelectedTodo = null;
                this.IsRightPaneCollapsed = true;
            }

            if (this.dataService != null)
            {
                this.dataService.DeleteTodo(todo.Id);
            }
        }

        /// <summary>
        /// Adds a new user-created folder and persists it to the database.
        /// </summary>
        public void AddFolder()
        {
            var folder = new FolderViewModel(this);
            folder.Id = "folder_" + TodoItemViewModel.GenerateId();
            folder.Name = "Untitled list";
            folder.Icon = "\uD83D\uDCCB";
            folder.IsSystem = false;
            folder.SystemType = "";
            this.Folders.Add(folder);

            if (this.dataService != null)
            {
                this.dataService.SaveFolder(folder.Id, folder.Name, folder.Icon, this.Folders.Count);
            }
        }

        /// <summary>
        /// Persists the current state of a todo item to the database.
        /// No-op when no data service is set (in-memory only mode).
        /// </summary>
        public void SaveTodo(TodoItemViewModel todo)
        {
            if (this.dataService != null)
            {
                this.dataService.SaveTodo(
                    todo.Id,
                    todo.FolderId,
                    todo.Title,
                    todo.IsCompleted,
                    todo.IsImportant,
                    todo.IsMyDay,
                    todo.DueDate,
                    todo.Notes);
            }

            // Keep the folder tags in sync when the selected todo's properties change
            if (todo == this.selectedTodo)
            {
                this.RefreshFolderTags(todo);
            }

            // Refresh the current view so folder counts and filtered lists stay in sync
            // after property changes like IsImportant, IsMyDay, or IsCompleted.
            this.RefreshCurrentTodos();
        }

        /// <summary>
        /// Entry point called after IndexedDB is ready.
        /// Creates system folders, loads persisted user folders and todos,
        /// then falls back to sample data if the database is empty.
        /// </summary>
        public void InitializeWithData()
        {
            this.Folders = new ObservableCollection<FolderViewModel>();
            this.CurrentTodos = new ObservableCollection<TodoItemViewModel>();

            this.CreateSystemFolders();

            if (this.dataService != null)
            {
                // Load user-created folders first, then load todos
                this.dataService.GetAllFolders().Then<bool>(delegate(string foldersJson)
                {
                    this.LoadFoldersFromJson(foldersJson);

                    this.dataService.GetAllTodos().Then<bool>(delegate(string todosJson)
                    {
                        this.LoadTodosFromJson(todosJson);
                        return true;
                    });

                    return true;
                });
            }
            else
            {
                this.AddSampleTodos();
                this.UpdateAllFolderCounts();
                this.OnSelectFolder(this.defaultTasksFolder);
            }
        }

        /// <summary>
        /// Creates the four system folders (My Day, Important, Planned, Tasks).
        /// Keeps a reference to the Tasks folder for initial selection.
        /// </summary>
        private void CreateSystemFolders()
        {
            var myDay = new FolderViewModel(this);
            myDay.Id = "myday";
            myDay.Name = "My Day";
            myDay.Icon = "\u2600";
            myDay.IsSystem = true;
            myDay.SystemType = "myday";

            var important = new FolderViewModel(this);
            important.Id = "important";
            important.Name = "Important";
            important.Icon = "\u2605";
            important.IsSystem = true;
            important.SystemType = "important";

            var planned = new FolderViewModel(this);
            planned.Id = "planned";
            planned.Name = "Planned";
            planned.Icon = "\uD83D\uDCC5";
            planned.IsSystem = true;
            planned.SystemType = "planned";

            var tasks = new FolderViewModel(this);
            tasks.Id = "tasks";
            tasks.Name = "Tasks";
            tasks.Icon = "\uD83C\uDFE0";
            tasks.IsSystem = true;
            tasks.SystemType = "tasks";

            var completed = new FolderViewModel(this);
            completed.Id = "completed";
            completed.Name = "Completed";
            completed.Icon = "\u2714";
            completed.IsSystem = true;
            completed.SystemType = "completed";

            this.Folders.Add(myDay);
            this.Folders.Add(important);
            this.Folders.Add(planned);
            this.Folders.Add(tasks);
            this.Folders.Add(completed);

            this.defaultTasksFolder = tasks;
        }

        /// <summary>
        /// Parses a JSON array of folder objects and adds user-created folders to the list.
        /// </summary>
        private void LoadFoldersFromJson(string json)
        {
            object parsed = System.Serialization.Json.Parse(json);
            int length = JsonHelper.GetArrayLength(parsed);

            for (int i = 0; i < length; i++)
            {
                object obj = JsonHelper.GetArrayItem(parsed, i);
                string id = JsonHelper.GetId(obj);

                if (id == null || id == "")
                {
                    continue;
                }

                var folder = new FolderViewModel(this);
                folder.Id = id;
                folder.Name = JsonHelper.GetName(obj);
                folder.Icon = JsonHelper.GetIcon(obj);
                folder.IsSystem = false;
                folder.SystemType = "";
                this.Folders.Add(folder);
            }
        }

        /// <summary>
        /// Parses a JSON array of todo objects and populates the allTodos collection.
        /// If the database is empty, creates sample todos and saves them.
        /// Selects the Tasks folder after loading.
        /// </summary>
        private void LoadTodosFromJson(string json)
        {
            object parsed = System.Serialization.Json.Parse(json);
            int length = JsonHelper.GetArrayLength(parsed);

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    object obj = JsonHelper.GetArrayItem(parsed, i);
                    string id = JsonHelper.GetId(obj);

                    if (id == null || id == "")
                    {
                        continue;
                    }

                    var todo = new TodoItemViewModel(this);
                    todo.Id = id;
                    todo.FolderId = JsonHelper.GetFolderId(obj);
                    todo.Title = JsonHelper.GetTitle(obj);
                    todo.IsCompleted = JsonHelper.GetIsCompleted(obj);
                    todo.IsImportant = JsonHelper.GetIsImportant(obj);
                    todo.IsMyDay = JsonHelper.GetIsMyDay(obj);
                    todo.DueDate = JsonHelper.GetDueDate(obj);
                    todo.Notes = JsonHelper.GetNotes(obj);
                    this.allTodos.Add(todo);
                }
            }
            else
            {
                // First run — populate with sample data and persist it
                this.AddSampleTodos();

                for (int i = 0; i < this.allTodos.Count; i++)
                {
                    this.SaveTodo(this.allTodos[i]);
                }
            }

            this.UpdateAllFolderCounts();
            this.OnSelectFolder(this.defaultTasksFolder);
        }

        /// <summary>
        /// Adds sample todo items across folders for initial demonstration.
        /// </summary>
        private void AddSampleTodos()
        {
            var todo1 = new TodoItemViewModel(this);
            todo1.Id = TodoItemViewModel.GenerateId();
            todo1.Title = "Buy groceries";
            todo1.FolderId = "tasks";
            todo1.IsCompleted = false;
            todo1.IsImportant = true;
            todo1.IsMyDay = true;
            this.allTodos.Add(todo1);

            var todo2 = new TodoItemViewModel(this);
            todo2.Id = TodoItemViewModel.GenerateId();
            todo2.Title = "Read a book";
            todo2.FolderId = "tasks";
            todo2.IsCompleted = false;
            todo2.IsImportant = false;
            todo2.IsMyDay = false;
            this.allTodos.Add(todo2);

            var todo3 = new TodoItemViewModel(this);
            todo3.Id = TodoItemViewModel.GenerateId();
            todo3.Title = "Schedule dentist appointment";
            todo3.FolderId = "tasks";
            todo3.IsCompleted = true;
            todo3.IsImportant = false;
            todo3.IsMyDay = false;
            todo3.DueDate = "2026-04-01";
            this.allTodos.Add(todo3);
        }

        /// <summary>
        /// Refreshes CurrentTodos based on the selected folder's filter criteria.
        /// Splits items into pending (CurrentTodos) and completed (CompletedCurrentTodos).
        /// </summary>
        private void RefreshCurrentTodos()
        {
            this.CurrentTodos.Clear();
            this.CompletedCurrentTodos.Clear();

            if (this.selectedFolder == null)
            {
                this.CompletedCount = 0;
                this.IsCompletedSectionVisible = false;
                return;
            }

            bool isCompletedFolder = this.selectedFolder.IsSystem && this.selectedFolder.SystemType == "completed";

            if (this.selectedFolder.IsSystem)
            {
                string systemType = this.selectedFolder.SystemType;

                for (int i = 0; i < this.allTodos.Count; i++)
                {
                    var todo = this.allTodos[i];
                    bool matches = false;

                    if (systemType == "myday" && todo.IsMyDay)
                        matches = true;
                    else if (systemType == "important" && todo.IsImportant)
                        matches = true;
                    else if (systemType == "planned" && todo.HasDueDate)
                        matches = true;
                    else if (systemType == "tasks")
                        matches = true;
                    else if (systemType == "completed" && todo.IsCompleted)
                        matches = true;

                    if (matches)
                    {
                        if (isCompletedFolder || !todo.IsCompleted)
                            this.CurrentTodos.Add(todo);
                        else
                            this.CompletedCurrentTodos.Add(todo);
                    }
                }
            }
            else
            {
                string folderId = this.selectedFolder.Id;
                for (int i = 0; i < this.allTodos.Count; i++)
                {
                    var todo = this.allTodos[i];
                    if (todo.FolderId == folderId)
                    {
                        if (!todo.IsCompleted)
                            this.CurrentTodos.Add(todo);
                        else
                            this.CompletedCurrentTodos.Add(todo);
                    }
                }
            }

            this.CompletedCount = this.CompletedCurrentTodos.Count;
            this.IsCompletedSectionVisible = !isCompletedFolder;
            this.selectedFolder.TodoCount = this.CurrentTodos.Count + this.CompletedCurrentTodos.Count;
            this.UpdateCompletedSectionClass();
        }

        /// <summary>
        /// Computes correct TodoCount for all system folders based on current allTodos state.
        /// </summary>
        private void UpdateAllFolderCounts()
        {
            int myDayCount = 0;
            int importantCount = 0;
            int plannedCount = 0;
            int tasksCount = 0;
            int completedCount = 0;

            for (int i = 0; i < this.allTodos.Count; i++)
            {
                var todo = this.allTodos[i];
                if (todo.IsMyDay) myDayCount = myDayCount + 1;
                if (todo.IsImportant) importantCount = importantCount + 1;
                if (todo.HasDueDate) plannedCount = plannedCount + 1;
                tasksCount = tasksCount + 1;
                if (todo.IsCompleted) completedCount = completedCount + 1;
            }

            for (int i = 0; i < this.Folders.Count; i++)
            {
                var folder = this.Folders[i];
                if (folder.IsSystem)
                {
                    if (folder.SystemType == "myday") folder.TodoCount = myDayCount;
                    else if (folder.SystemType == "important") folder.TodoCount = importantCount;
                    else if (folder.SystemType == "planned") folder.TodoCount = plannedCount;
                    else if (folder.SystemType == "tasks") folder.TodoCount = tasksCount;
                    else if (folder.SystemType == "completed") folder.TodoCount = completedCount;
                }
            }
        }
    }
}

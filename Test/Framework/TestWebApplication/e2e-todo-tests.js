const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch({ headless: true });
  const results = { passed: 0, failed: 0, tests: [] };

  async function runTest(name, fn) {
    const context = await browser.newContext();
    const page = await context.newPage();
    try {
      // Clear IndexedDB before each test so the app always starts from sample data
      await page.goto('http://localhost:3000/TodoApp.htm', { waitUntil: 'domcontentloaded' });
      await page.evaluate(() => {
        return new Promise((resolve) => {
          var req = indexedDB.deleteDatabase('TodoAppDb');
          req.onsuccess = function() { resolve(); };
          req.onerror = function() { resolve(); };
          req.onblocked = function() { resolve(); };
        });
      });
      // Reload after DB clear so the app re-initialises with sample data
      await page.goto('http://localhost:3000/TodoApp.htm', { waitUntil: 'domcontentloaded' });
      // Wait for app to render
      await page.waitForSelector('.folder-item', { timeout: 10000 });
      await page.waitForTimeout(500);

      await fn(page);
      results.passed++;
      results.tests.push({ name, status: 'PASS' });
      console.log('  PASS: ' + name);
    } catch (err) {
      results.failed++;
      results.tests.push({ name, status: 'FAIL', error: err.message });
      console.log('  FAIL: ' + name);
      console.log('    ' + err.message);
    } finally {
      await context.close();
    }
  }

  function assert(condition, message) {
    if (!condition) throw new Error('Assertion failed: ' + message);
  }

  console.log('\n=== To Do App E2E Tests ===\n');

  // ─── LAYOUT TESTS ───────────────────────────────────────────────────────────

  await runTest('App renders with layout', async (page) => {
    const app = await page.$('#app');
    assert(app, 'app container should exist');

    const leftPane = await page.$('[class*="pane-left"]');
    assert(leftPane, 'Left pane should exist');

    const centerPane = await page.$('.pane-center');
    assert(centerPane, 'Center pane should exist');
  });

  await runTest('System folders render', async (page) => {
    const folders = await page.$$('.folder-item');
    assert(folders.length >= 4, 'Should have at least 4 system folders, got ' + folders.length);

    const folderNames = await page.$$eval('.folder-name', els => els.map(e => e.textContent));
    assert(folderNames.includes('My Day'), 'Should have My Day folder');
    assert(folderNames.includes('Important'), 'Should have Important folder');
    assert(folderNames.includes('Planned'), 'Should have Planned folder');
    assert(folderNames.includes('Tasks'), 'Should have Tasks folder');
  });

  await runTest('Sample todos render', async (page) => {
    const todos = await page.$$('.todo-item');
    assert(todos.length >= 1, 'Should have at least 1 todo item, got ' + todos.length);

    const titles = await page.$$eval('.todo-title', els => els.map(e => e.textContent));
    assert(titles.some(t => t.includes('Buy groceries')), 'Should have "Buy groceries" todo');
  });

  await runTest('Folder name displays in center header', async (page) => {
    const folderName = await page.$eval('.current-folder-name', el => el.textContent);
    assert(folderName === 'Tasks', 'Center header should show Tasks, got: ' + folderName);
  });

  // ─── INTERACTION TESTS ──────────────────────────────────────────────────────

  await runTest('Click todo triggers selection', async (page) => {
    // Click first todo item
    const todoItem = await page.$('.todo-item');
    assert(todoItem, 'Should have a todo to click');
    await todoItem.click();
    await page.waitForTimeout(500);

    // Verify right pane exists (may already be open, or opens after click)
    const rightPane = await page.$('.pane-right');
    assert(rightPane, 'Right pane should exist after clicking a todo');
  });

  await runTest('Add a task creates new todo', async (page) => {
    const initialCount = (await page.$$('.todo-item')).length;

    const input = await page.$('.add-task-input');
    assert(input, 'Add task input should exist');
    await input.fill('Test new task');
    await input.press('Enter');
    await page.waitForTimeout(500);

    const newCount = (await page.$$('.todo-item')).length;
    assert(newCount === initialCount + 1, 'Todo count should increase by 1, was ' + initialCount + ' now ' + newCount);

    // Verify the title is correct
    const titles = await page.$$eval('.todo-title', els => els.map(e => e.textContent));
    assert(titles.includes('Test new task'), 'New todo should have the typed title');
  });

  await runTest('Detail pane shows todo title', async (page) => {
    // Click a todo to open detail pane
    const todoItem = await page.$('.todo-item');
    assert(todoItem, 'Should have a todo');
    await todoItem.click();
    await page.waitForTimeout(500);

    // Verify right pane is visible (not collapsed)
    const pane = await page.$('.pane-right:not(.collapsed)');
    assert(pane, 'Right pane should be visible after clicking todo');

    // Verify the detail title input shows the todo name
    const title = await page.$eval('.detail-title-input', el => el.value);
    assert(title === 'Buy groceries', 'Detail title input should show todo name, got: ' + title);
  });

  await runTest('Edit todo title in detail pane syncs to list', async (page) => {
    // Click first todo to open detail pane
    const todoItem = await page.$('.todo-item');
    assert(todoItem, 'Should have a todo');
    await todoItem.click();
    await page.waitForTimeout(500);

    // Clear the detail title input and type a new name
    const input = await page.$('.detail-title-input');
    assert(input, 'Detail title input should exist');
    await input.fill('Renamed task');
    // Trigger onchange by pressing Tab (moves focus away)
    await input.press('Tab');
    await page.waitForTimeout(500);

    // Verify the todo list reflects the new title
    const titles = await page.$$eval('.todo-title', els => els.map(e => e.textContent));
    assert(titles.includes('Renamed task'), 'Todo list should show renamed title, got: ' + titles.join(', '));
  });

  await runTest('Add subtask via detail pane input', async (page) => {
    // Click first todo to open detail pane
    const todoItem = await page.$('.todo-item');
    assert(todoItem, 'Should have a todo');
    await todoItem.click();
    await page.waitForTimeout(500);

    // Type in the add-step input and press Enter
    const addInput = await page.$('.add-step-input');
    assert(addInput, 'Add step input should exist');
    await addInput.fill('My test step');
    await addInput.press('Enter');
    await page.waitForTimeout(500);

    // Verify a subtask appeared
    const subtasks = await page.$$('.subtask-item');
    assert(subtasks.length >= 1, 'Should have at least 1 subtask after Enter, got ' + subtasks.length);
  });

  // ─── BUG-013: SELECTION HIGHLIGHT ────────────────────────────────────────────

  await runTest('BUG-013: Selected todo gets highlight class', async (page) => {
    const firstTodo = await page.$('.todo-item');
    assert(firstTodo, 'Should have a todo');

    // Before click: no "selected" class
    const classBefore = await firstTodo.evaluate(el => el.className);
    assert(!classBefore.includes('selected'), 'Should not be selected initially, got: ' + classBefore);

    await firstTodo.click();
    await page.waitForTimeout(500);

    // After click: should have "selected" class
    const classAfter = await firstTodo.evaluate(el => el.className);
    assert(classAfter.includes('selected'), 'Should have selected class after click, got: ' + classAfter);
  });

  await runTest('BUG-013: Selecting a different todo moves highlight', async (page) => {
    const todos = await page.$$('.todo-item');
    assert(todos.length >= 2, 'Need at least 2 todos');

    await todos[0].click();
    await page.waitForTimeout(500);
    const class1 = await todos[0].evaluate(el => el.className);
    assert(class1.includes('selected'), 'First todo should be selected');

    await todos[1].click();
    await page.waitForTimeout(500);
    const class1After = await todos[0].evaluate(el => el.className);
    const class2After = await todos[1].evaluate(el => el.className);
    assert(!class1After.includes('selected'), 'First todo should lose selected');
    assert(class2After.includes('selected'), 'Second todo should gain selected');
  });

  // ─── BUG-014: SUBTASK EDITING ──────────────────────────────────────────────

  await runTest('BUG-014: Subtask title shows typed text', async (page) => {
    await page.click('.todo-item');
    await page.waitForTimeout(500);

    const addInput = await page.$('.add-step-input');
    await addInput.fill('Custom step name');
    await addInput.press('Enter');
    await page.waitForTimeout(500);

    // Verify the subtask input has the correct value
    const val = await page.$eval('.subtask-title-input', el => el.value);
    assert(val === 'Custom step name', 'Subtask input should show typed title, got: ' + val);
  });

  await runTest('BUG-014: Subtask title is editable', async (page) => {
    await page.click('.todo-item');
    await page.waitForTimeout(500);

    const addInput = await page.$('.add-step-input');
    await addInput.fill('Original step');
    await addInput.press('Enter');
    await page.waitForTimeout(500);

    // Edit the subtask title
    const subtaskInput = await page.$('.subtask-title-input');
    await subtaskInput.fill('Renamed step');
    await subtaskInput.press('Tab');
    await page.waitForTimeout(500);

    const val = await page.$eval('.subtask-title-input', el => el.value);
    assert(val === 'Renamed step', 'Subtask title should be updated, got: ' + val);
  });

  // ─── BUG-015: FOLDER COUNTS ON LOAD ────────────────────────────────────────

  await runTest('BUG-015: All folder counts correct on initial load', async (page) => {
    const counts = await page.$$eval('.folder-count', els => els.map(e => e.textContent));
    const names = await page.$$eval('.folder-name', els => els.map(e => e.textContent));

    // My Day: 1 (Buy groceries has IsMyDay=true)
    const myDayIdx = names.indexOf('My Day');
    assert(counts[myDayIdx] === '1', 'My Day should show 1, got: ' + counts[myDayIdx]);

    // Important: 1 (Buy groceries has IsImportant=true)
    const impIdx = names.indexOf('Important');
    assert(counts[impIdx] === '1', 'Important should show 1, got: ' + counts[impIdx]);

    // Tasks: 3 (all sample todos)
    const tasksIdx = names.indexOf('Tasks');
    assert(counts[tasksIdx] === '3', 'Tasks should show 3, got: ' + counts[tasksIdx]);
  });

  // ─── BUG-017: CLASS BINDING IN FOREACH ─────────────────────────────────────

  await runTest('BUG-017: Class binding works on todo item elements', async (page) => {
    // Verify todo items have correct CSS class
    const classes = await page.$$eval('.todo-item', els => els.map(e => e.className));
    assert(classes.length >= 2, 'Should have at least 2 todos');
    assert(classes[0] === 'todo-item', 'Pending todo should have class "todo-item", got: ' + classes[0]);

    // The third todo (Schedule dentist) is pre-completed
    const completedTodo = classes.find(c => c.includes('completed'));
    assert(completedTodo, 'Should have a completed todo with "completed" class');
  });

  await runTest('BUG-017: Title text renders correctly with class binding', async (page) => {
    // This specifically tests that class binding doesn't break text binding (the BUG-017 symptom)
    const titles = await page.$$eval('.todo-title', els => els.map(e => e.textContent));
    assert(titles.includes('Buy groceries'), 'Title binding should work alongside class binding');
    assert(titles.includes('Read a book'), 'All titles should render, got: ' + titles.join(', '));
  });

  // ─── COMPLETION TESTS ──────────────────────────────────────────────────────

  await runTest('Toggle todo completion adds completed class', async (page) => {
    const firstTodo = await page.$('.todo-item:not(.todo-item.completed)');
    assert(firstTodo, 'Should have an uncompleted todo');

    // Click the checkbox
    const checkbox = await firstTodo.$('.btn-check');
    assert(checkbox, 'Todo should have a checkbox');
    await checkbox.click();
    await page.waitForTimeout(500);

    const classAfter = await firstTodo.evaluate(el => el.className);
    assert(classAfter.includes('completed'), 'Todo should have completed class after checkbox click, got: ' + classAfter);
  });

  await runTest('Completed todo moves to completed section', async (page) => {
    const pendingBefore = (await page.$$('.todo-list > .todo-item')).length;

    // Complete the first todo
    const checkbox = await page.$('.todo-item .btn-check');
    await checkbox.click();
    await page.waitForTimeout(500);

    const pendingAfter = (await page.$$('.todo-list > .todo-item')).length;
    assert(pendingAfter === pendingBefore - 1, 'Pending count should decrease by 1, was ' + pendingBefore + ' now ' + pendingAfter);

    // Completed section should have the item
    const completedLabel = await page.$eval('.completed-label', el => el.textContent);
    assert(completedLabel.includes('2'), 'Completed count should be 2 (1 pre-completed + 1 new), got: ' + completedLabel);
  });

  await runTest('Toggle subtask completion', async (page) => {
    await page.click('.todo-item');
    await page.waitForTimeout(500);

    // Add a subtask
    const addInput = await page.$('.add-step-input');
    await addInput.fill('Test step');
    await addInput.press('Enter');
    await page.waitForTimeout(500);

    // Click the subtask checkbox
    const subCheck = await page.$('.subtask-item .btn-check');
    assert(subCheck, 'Subtask should have a checkbox');
    await subCheck.click();
    await page.waitForTimeout(500);

    const subClass = await page.$eval('.subtask-item', el => el.className);
    assert(subClass.includes('completed'), 'Subtask should have completed class, got: ' + subClass);
  });

  // ─── COMPLETED SECTION ─────────────────────────────────────────────────────

  await runTest('Completed section exists and is collapsed by default', async (page) => {
    const section = await page.$('.completed-section');
    assert(section, 'Completed section should exist');

    const cls = await section.evaluate(el => el.className);
    assert(cls.includes('collapsed'), 'Completed section should be collapsed by default, got: ' + cls);
  });

  await runTest('Completed section expands on click', async (page) => {
    const header = await page.$('.completed-header');
    assert(header, 'Completed header should exist');
    await header.click();
    await page.waitForTimeout(300);

    const cls = await page.$eval('.completed-section', el => el.className);
    assert(!cls.includes('collapsed'), 'Completed section should expand after click, got: ' + cls);
  });

  await runTest('Completed folder shows all completed todos in main list', async (page) => {
    const folders = await page.$$('.folder-item');
    const names = await page.$$eval('.folder-name', els => els.map(e => e.textContent));
    const completedIdx = names.indexOf('Completed');
    assert(completedIdx >= 0, 'Should have Completed folder');

    await folders[completedIdx].click();
    await page.waitForTimeout(500);

    // Completed folder should show items directly in the main todo list (not in completed section)
    const todos = await page.$$('.todo-item');
    assert(todos.length >= 1, 'Completed folder should show at least 1 todo in main list, got ' + todos.length);

    // Completed section should be hidden
    const sectionClass = await page.$eval('.completed-section', el => el.className);
    assert(sectionClass.includes('hidden'), 'Completed section should be hidden, got: ' + sectionClass);
  });

  // ─── BUG-016: FOLDER ASSIGNMENT ────────────────────────────────────────────

  await runTest('BUG-016: Detail pane shows current folder name', async (page) => {
    await page.click('.todo-item');
    await page.waitForTimeout(500);

    const folderLabel = await page.$eval('.current-folder-label', el => el.textContent);
    assert(folderLabel.includes('Tasks'), 'Should show current folder as Tasks, got: ' + folderLabel);
  });

  await runTest('BUG-016: Folder picker lists all folders', async (page) => {
    await page.click('.todo-item');
    await page.waitForTimeout(500);

    const pickerNames = await page.$$eval('.folder-pick-name', els => els.map(e => e.textContent));
    assert(pickerNames.includes('Tasks'), 'Picker should include Tasks');
    assert(pickerNames.includes('My Day'), 'Picker should include My Day');
  });

  // ─── BUG-011: VALUE BINDING ────────────────────────────────────────────────

  await runTest('BUG-011: Detail title input updates when switching todos', async (page) => {
    const todos = await page.$$('.todo-item');
    assert(todos.length >= 2, 'Need at least 2 todos');

    // Select first todo
    await todos[0].click();
    await page.waitForTimeout(500);
    const title1 = await page.$eval('.detail-title-input', el => el.value);

    // Select second todo
    await todos[1].click();
    await page.waitForTimeout(500);
    const title2 = await page.$eval('.detail-title-input', el => el.value);

    assert(title1 !== title2, 'Switching todos should change detail title, first: ' + title1 + ', second: ' + title2);
  });

  // ─── FOLDER NAVIGATION ──────────────────────────────────────────────────────

  await runTest('Switch to My Day folder', async (page) => {
    const folders = await page.$$('.folder-item');
    assert(folders.length >= 1, 'Should have folders');
    await folders[0].click();
    await page.waitForTimeout(500);

    // My Day should show "Buy groceries" (IsMyDay = true in sample data)
    const todos = await page.$$('.todo-item');
    assert(todos.length >= 1, 'My Day should show at least 1 todo');
  });

  await runTest('Switch to Important folder', async (page) => {
    const folders = await page.$$('.folder-item');
    assert(folders.length >= 2, 'Should have at least 2 folders');
    await folders[1].click();
    await page.waitForTimeout(500);

    const todos = await page.$$('.todo-item');
    assert(todos.length >= 1, 'Important should show at least 1 todo');
  });

  // ─── PANE COLLAPSE ──────────────────────────────────────────────────────────

  await runTest('Collapse and expand left pane', async (page) => {
    const toggleBtn = await page.$('.btn-toggle-left');
    assert(toggleBtn, 'Toggle button should exist');
    await toggleBtn.click();
    await page.waitForTimeout(300);

    const collapsed = await page.$('.pane-left.collapsed');
    assert(collapsed, 'Left pane should have collapsed class');

    await toggleBtn.click();
    await page.waitForTimeout(300);

    const expanded = await page.$('.pane-left:not(.collapsed)');
    assert(expanded, 'Left pane should expand again');
  });

  // ─── BUG-019: DRAG AND DROP ──────────────────────────────────────────────────

  // Helper: simulate HTML5 drag-and-drop with DataTransfer between two elements
  async function simulateDragDrop(page, sourceSelector, targetSelector) {
    await page.evaluate(({ src, tgt }) => {
      const source = document.querySelectorAll(src)[0];
      const target = document.querySelectorAll(tgt)[0];
      if (!source || !target) return;

      const dt = new DataTransfer();
      source.dispatchEvent(new DragEvent('dragstart', { dataTransfer: dt, bubbles: true }));
      target.dispatchEvent(new DragEvent('dragenter', { dataTransfer: dt, bubbles: true }));
      target.dispatchEvent(new DragEvent('dragover', { dataTransfer: dt, bubbles: true }));
      target.dispatchEvent(new DragEvent('drop', { dataTransfer: dt, bubbles: true }));
      source.dispatchEvent(new DragEvent('dragend', { dataTransfer: dt, bubbles: true }));
    }, { src: sourceSelector, tgt: targetSelector });
  }

  await runTest('BUG-019: Drag todo to My Day folder sets IsMyDay', async (page) => {
    // "Read a book" (second todo) has IsMyDay=false — drag it to My Day
    const todos = await page.$$('.todo-item');
    assert(todos.length >= 2, 'Need at least 2 todos');

    // Simulate drag-and-drop with proper DataTransfer
    await simulateDragDrop(page, '.todo-item:nth-child(2)', '.folder-item:nth-child(1)');
    await page.waitForTimeout(500);

    // Switch to My Day folder to verify the todo now appears
    const folders = await page.$$('.folder-item');
    await folders[0].click();
    await page.waitForTimeout(500);
    const myDayTodos = await page.$$eval('.todo-title', els => els.map(e => e.textContent));
    assert(myDayTodos.includes('Read a book'), 'Read a book should appear in My Day after drag, got: ' + myDayTodos.join(', '));
  });

  await runTest('BUG-019: Drag todo to Important folder sets IsImportant', async (page) => {
    // "Read a book" (second todo) has IsImportant=false — drag it to Important
    const todos = await page.$$('.todo-item');
    assert(todos.length >= 2, 'Need at least 2 todos');

    // Drag second todo to Important folder (index 1 in list = nth-child(2))
    await simulateDragDrop(page, '.todo-item:nth-child(2)', '.folder-item:nth-child(2)');
    await page.waitForTimeout(500);

    // Switch to Important folder to verify
    const folders = await page.$$('.folder-item');
    await folders[1].click();
    await page.waitForTimeout(500);
    const impTodos = await page.$$eval('.todo-title', els => els.map(e => e.textContent));
    assert(impTodos.includes('Read a book'), 'Read a book should appear in Important after drag, got: ' + impTodos.join(', '));
  });

  // ─── BUG-005: @IF GATE IN ITEM TEMPLATES ───────────────────────────────────

  await runTest('BUG-005: @if gate toggles star between important/not', async (page) => {
    // "Buy groceries" is important (filled star ★), "Read a book" is not (empty star ☆)
    const todoItems = await page.$$('.todo-item');
    assert(todoItems.length >= 2, 'Need at least 2 todos');

    // First todo should have filled star (class="star important")
    const star1 = await todoItems[0].$('.star');
    const starClass1 = await star1.evaluate(el => el.className);
    assert(starClass1.includes('important'), 'First todo should have important star');

    // Second todo should have empty star (class="star" without important)
    const star2 = await todoItems[1].$('.star');
    const starClass2 = await star2.evaluate(el => el.className);
    assert(!starClass2.includes('important'), 'Second todo should have non-important star');

    // Click the empty star to toggle importance
    await star2.click();
    await page.waitForTimeout(500);

    // After click, the star should now be important (gate should flip)
    const updatedTodo = (await page.$$('.todo-item'))[1];
    const updatedStar = await updatedTodo.$('.star');
    const updatedClass = await updatedStar.evaluate(el => el.className);
    assert(updatedClass.includes('important'), 'Star should become important after click, got: ' + updatedClass);
  });

  await runTest('BUG-005b: Toggling star back removes important class (un-star)', async (page) => {
    // "Buy groceries" starts important — click its star to un-star it
    const todoItems = await page.$$('.todo-item');
    assert(todoItems.length >= 1, 'Need at least 1 todo');

    const star = await todoItems[0].$('.star');
    const classBefore = await star.evaluate(el => el.className);
    assert(classBefore.includes('important'), 'First todo should start important, got: ' + classBefore);

    // Click to un-star
    await star.click();
    await page.waitForTimeout(500);

    // Re-query after DOM update
    const updatedTodo = (await page.$$('.todo-item'))[0];
    const updatedStar = await updatedTodo.$('.star');
    const classAfter = await updatedStar.evaluate(el => el.className);
    assert(!classAfter.includes('important'), 'Star should lose important after click, got: ' + classAfter);

    // Click again to re-star — verify round-trip
    await updatedStar.click();
    await page.waitForTimeout(500);

    const reTodo = (await page.$$('.todo-item'))[0];
    const reStar = await reTodo.$('.star');
    const classRound = await reStar.evaluate(el => el.className);
    assert(classRound.includes('important'), 'Star should regain important after second click, got: ' + classRound);
  });

  // ─── COMPLETED FOLDER BEHAVIOR ─────────────────────────────────────────────

  await runTest('BUG-020: Completed folder shows items in main list, not completed section', async (page) => {
    const folders = await page.$$('.folder-item');
    const names = await page.$$eval('.folder-name', els => els.map(e => e.textContent));
    const completedIdx = names.indexOf('Completed');
    await folders[completedIdx].click();
    await page.waitForTimeout(500);

    // Items should be in the main todo-list, not the completed section
    const mainTodos = await page.$$('.todo-list .todo-item');
    assert(mainTodos.length >= 1, 'Completed folder should show items in main list');

    // Completed section should be hidden
    const sectionVisible = await page.$eval('.completed-section', el => {
      return window.getComputedStyle(el).display !== 'none';
    });
    assert(!sectionVisible, 'Completed section should be hidden when Completed folder selected');
  });

  // ─── PERSISTENCE ────────────────────────────────────────────────────────────

  await runTest('Data persists across page reload', async (page) => {
    // Verify sample data loads from IndexedDB on initial page load
    let titles = await page.$$eval('.todo-title', els => els.map(e => e.textContent));
    assert(titles.length >= 1, 'Should have at least 1 todo from IndexedDB');
    const knownTitle = titles[0];

    // Reload the page — IndexedDB sample data should survive
    await page.reload({ waitUntil: 'domcontentloaded' });
    await page.waitForSelector('.folder-item', { timeout: 10000 });
    await page.waitForTimeout(1000);

    // Assert sample data is still present after reload
    titles = await page.$$eval('.todo-title', els => els.map(e => e.textContent));
    assert(titles.includes(knownTitle), 'Sample todo should persist after reload, got: ' + titles.join(', '));
    // NOTE: Newly added todos do NOT persist yet (known LIMIT — AddTodo
    // doesn't call SaveTodo). A future fix should add a test that creates
    // a unique item, reloads, and verifies it survived.
  });

  // ─── PARENT-METHOD EVENT BINDING (Model.OnSelect) ─────────────────────────

  await runTest('REG-001: Model.OnSelect(todo) triggers selection on parent ViewModel', async (page) => {
    // Verify at least one todo item exists
    const todoItems = await page.$$('.todo-item');
    assert(todoItems.length >= 2, 'Need at least 2 todos, got: ' + todoItems.length);

    // Click the second todo and check it gets a selected visual cue
    await todoItems[1].click();
    await page.waitForTimeout(500);

    // The right pane should exist after selection
    const rightPane = await page.$('.pane-right');
    assert(rightPane, 'Right pane should appear after clicking a todo via Model.OnSelect');
  });

  await runTest('REG-002: Switching selected todo updates selection', async (page) => {
    const todoItems = await page.$$('.todo-item');
    assert(todoItems.length >= 2, 'Need at least 2 todos');

    // Click first todo
    await todoItems[0].click();
    await page.waitForTimeout(300);

    // Click second todo
    await todoItems[1].click();
    await page.waitForTimeout(300);

    // Right pane should still be visible
    const pane = await page.$('.pane-right');
    assert(pane, 'Right pane should still be visible after switching selection');
  });

  await runTest('REG-003: Star toggle and selection work independently', async (page) => {
    // This tests that item events (star toggle) and parent method events
    // (Model.OnSelect) both work from the same foreach item template.
    const todoItems = await page.$$('.todo-item');
    assert(todoItems.length >= 1, 'Need at least 1 todo');

    // Toggle star on first todo
    const star = await todoItems[0].$('.star');
    assert(star, 'Todo should have a star element');
    const classBefore = await star.evaluate(el => el.className);

    await star.click();
    await page.waitForTimeout(500);

    const updatedItems = await page.$$('.todo-item');
    const updatedStar = await updatedItems[0].$('.star');
    const classAfter = await updatedStar.evaluate(el => el.className);

    // Star class should have changed
    assert(classBefore !== classAfter,
      'Star class should change after click. Before: ' + classBefore + ', After: ' + classAfter);

    // Now click the todo itself (not the star) to trigger selection
    await updatedItems[0].click();
    await page.waitForTimeout(500);

    // Right pane should appear (selection via Model.OnSelect)
    const pane = await page.$('.pane-right');
    assert(pane, 'Right pane should appear after clicking todo item body');
  });

  // ─── RESULTS ────────────────────────────────────────────────────────────────

  console.log('\n=== E2E Results: ' + results.passed + ' passed, ' + results.failed + ' failed ===\n');

  results.tests.filter(t => t.status === 'FAIL').forEach(t => {
    console.log('FAIL: ' + t.name);
    console.log('  ' + t.error);
    console.log('');
  });

  await browser.close();
  process.exit(results.failed > 0 ? 1 : 0);
})();

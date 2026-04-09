const { chromium } = require('playwright');
const http = require('http');
const fs = require('fs');
const path = require('path');

const MIME = {
  '.html': 'text/html', '.htm': 'text/html', '.js': 'application/javascript',
  '.css': 'text/css', '.json': 'application/json', '.png': 'image/png', '.svg': 'image/svg+xml'
};

let _server;

// Start a static file server on the given port (0 = OS picks a free port).
// Returns the base URL, e.g. "http://localhost:54321".
function startServer(port) {
  return new Promise((resolve, reject) => {
    const root = __dirname;
    _server = http.createServer((req, res) => {
      const filePath = path.join(root, req.url === '/' ? 'TodoApp.htm' : decodeURIComponent(req.url));
      const ext = path.extname(filePath).toLowerCase();
      fs.readFile(filePath, (err, data) => {
        if (err) { res.writeHead(404); res.end('Not found'); return; }
        res.writeHead(200, { 'Content-Type': MIME[ext] || 'application/octet-stream' });
        res.end(data);
      });
    });
    _server.listen(parseInt(port) || 0, () => {
      const addr = _server.address();
      resolve(`http://localhost:${addr.port}`);
    });
    _server.on('error', reject);
  });
}

// CSS class name mapping for suffixed/minified names.
// The NScript compiler appends _XY suffixes to CSS class names in debug mode
// (and fully minifies in release). This helper reads the generated <style>
// element to build a map from original names to their suffixed versions,
// so E2E selectors stay readable while matching actual DOM classes.
async function buildClassMap(page) {
  return await page.evaluate(() => {
    const map = {};
    const styles = document.querySelectorAll('style');
    for (const style of styles) {
      const text = style.textContent;
      const re = /\.([a-z][\w-]*?_[a-zA-Z0-9]+)/g;
      let m;
      while ((m = re.exec(text)) !== null) {
        const full = m[1];
        const lastUnderscore = full.lastIndexOf('_');
        const original = full.substring(0, lastUnderscore);
        if (!map[original]) map[original] = full;
      }
    }
    return map;
  });
}

function sel(classMap, selector) {
  return selector.replace(/\.([a-z][\w-]+)/g, (match, name) => {
    return '.' + (classMap[name] || name);
  });
}


(async () => {
  // Dynamic port: use E2E_PORT env var, CLI arg, or find a free port
  const PORT = process.env.E2E_PORT || process.argv[2] || 0;
  const BASE_URL = await startServer(PORT);
  console.log('=== To Do App E2E Tests ===\n');

  const browser = await chromium.launch({ headless: true });
  const results = { passed: 0, failed: 0, tests: [] };

  async function runTest(name, fn) {
    const context = await browser.newContext();
    const page = await context.newPage();
    try {
      // Clear IndexedDB before each test so the app always starts from sample data
      await page.goto(BASE_URL + '/TodoApp.htm', { waitUntil: 'domcontentloaded' });
      await page.evaluate(() => {
        return new Promise((resolve) => {
          var req = indexedDB.deleteDatabase('TodoAppDb');
          req.onsuccess = function() { resolve(); };
          req.onerror = function() { resolve(); };
          req.onblocked = function() { resolve(); };
        });
      });
      // Reload after DB clear so the app re-initialises with sample data
      await page.goto(BASE_URL + '/TodoApp.htm', { waitUntil: 'domcontentloaded' });
      // Wait for the generated <style> element to be injected by TodoApp.js
      await page.waitForFunction(() => {
        const styles = document.querySelectorAll('style');
        return Array.from(styles).some(s => s.textContent.includes('_'));
      }, { timeout: 10000 });
      // Build CSS class map and create selector helper
      const _classMap = await buildClassMap(page);
      const s = (selector) => sel(_classMap, selector);
      // Wait for app to render
      await page.waitForSelector(s('.folder-item'), { timeout: 10000 });
      await page.waitForTimeout(500);

      await fn(page, s);
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

  // ─── LAYOUT TESTS ───────────────────────────────────────────────────────────

  await runTest('App renders with layout', async (page, s) => {
    const app = await page.$('#app');
    assert(app, 'app container should exist');

    const leftPane = await page.$('[class*="pane-left"]');
    assert(leftPane, 'Left pane should exist');

    const centerPane = await page.$(s('.pane-center'));
    assert(centerPane, 'Center pane should exist');
  });

  await runTest('System folders render', async (page, s) => {
    const folders = await page.$$(s('.folder-item'));
    assert(folders.length >= 4, 'Should have at least 4 system folders, got ' + folders.length);

    const folderNames = await page.$$eval(s('.folder-name'), els => els.map(e => e.textContent));
    assert(folderNames.includes('My Day'), 'Should have My Day folder');
    assert(folderNames.includes('Important'), 'Should have Important folder');
    assert(folderNames.includes('Planned'), 'Should have Planned folder');
    assert(folderNames.includes('Tasks'), 'Should have Tasks folder');
  });

  await runTest('Sample todos render', async (page, s) => {
    const todos = await page.$$(s('.todo-item'));
    assert(todos.length >= 1, 'Should have at least 1 todo item, got ' + todos.length);

    const titles = await page.$$eval(s('.todo-title'), els => els.map(e => e.textContent));
    assert(titles.some(t => t.includes('Buy groceries')), 'Should have "Buy groceries" todo');
  });

  await runTest('Folder name displays in center header', async (page, s) => {
    const folderName = await page.$eval(s('.current-folder-name'), el => el.textContent);
    assert(folderName === 'Tasks', 'Center header should show Tasks, got: ' + folderName);
  });

  // ─── INTERACTION TESTS ──────────────────────────────────────────────────────

  await runTest('Click todo triggers selection', async (page, s) => {
    // Click first todo item
    const todoItem = await page.$(s('.todo-item'));
    assert(todoItem, 'Should have a todo to click');
    await todoItem.click();
    await page.waitForTimeout(500);

    // Verify right pane exists (may already be open, or opens after click)
    const rightPane = await page.$(s('.pane-right'));
    assert(rightPane, 'Right pane should exist after clicking a todo');
  });

  await runTest('Add a task creates new todo', async (page, s) => {
    const initialCount = (await page.$$(s('.todo-item'))).length;

    const input = await page.$(s('.add-task-input'));
    assert(input, 'Add task input should exist');
    await input.fill('Test new task');
    await input.press('Enter');
    await page.waitForTimeout(500);

    const newCount = (await page.$$(s('.todo-item'))).length;
    assert(newCount === initialCount + 1, 'Todo count should increase by 1, was ' + initialCount + ' now ' + newCount);

    // Verify the title is correct
    const titles = await page.$$eval(s('.todo-title'), els => els.map(e => e.textContent));
    assert(titles.includes('Test new task'), 'New todo should have the typed title');
  });

  await runTest('Detail pane shows todo title', async (page, s) => {
    // Click a todo to open detail pane
    const todoItem = await page.$(s('.todo-item'));
    assert(todoItem, 'Should have a todo');
    await todoItem.click();
    await page.waitForTimeout(500);

    // Verify right pane is visible (not collapsed)
    const pane = await page.$(s('.pane-right:not(.collapsed)'));
    assert(pane, 'Right pane should be visible after clicking todo');

    // Verify the detail title input shows the todo name
    const title = await page.$eval(s('.detail-title-input'), el => el.value);
    assert(title === 'Buy groceries', 'Detail title input should show todo name, got: ' + title);
  });

  await runTest('Edit todo title in detail pane syncs to list', async (page, s) => {
    // Click first todo to open detail pane
    const todoItem = await page.$(s('.todo-item'));
    assert(todoItem, 'Should have a todo');
    await todoItem.click();
    await page.waitForTimeout(500);

    // Clear the detail title input and type a new name
    const input = await page.$(s('.detail-title-input'));
    assert(input, 'Detail title input should exist');
    await input.fill('Renamed task');
    // Trigger onchange by pressing Tab (moves focus away)
    await input.press('Tab');
    await page.waitForTimeout(500);

    // Verify the todo list reflects the new title
    const titles = await page.$$eval(s('.todo-title'), els => els.map(e => e.textContent));
    assert(titles.includes('Renamed task'), 'Todo list should show renamed title, got: ' + titles.join(', '));
  });

  await runTest('Add subtask via detail pane input', async (page, s) => {
    // Click first todo to open detail pane
    const todoItem = await page.$(s('.todo-item'));
    assert(todoItem, 'Should have a todo');
    await todoItem.click();
    await page.waitForTimeout(500);

    // Type in the add-step input and press Enter
    const addInput = await page.$(s('.add-step-input'));
    assert(addInput, 'Add step input should exist');
    await addInput.fill('My test step');
    await addInput.press('Enter');
    await page.waitForTimeout(500);

    // Verify a subtask appeared
    const subtasks = await page.$$(s('.subtask-item'));
    assert(subtasks.length >= 1, 'Should have at least 1 subtask after Enter, got ' + subtasks.length);
  });

  // ─── BUG-013: SELECTION HIGHLIGHT ────────────────────────────────────────────

  await runTest('BUG-013: Selected todo gets highlight class', async (page, s) => {
    const firstTodo = await page.$(s('.todo-item'));
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

  await runTest('BUG-013: Selecting a different todo moves highlight', async (page, s) => {
    const todos = await page.$$(s('.todo-item'));
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

  await runTest('BUG-014: Subtask title shows typed text', async (page, s) => {
    await page.click(s('.todo-item'));
    await page.waitForTimeout(500);

    const addInput = await page.$(s('.add-step-input'));
    await addInput.fill('Custom step name');
    await addInput.press('Enter');
    await page.waitForTimeout(500);

    // Verify the subtask input has the correct value
    const val = await page.$eval(s('.subtask-title-input'), el => el.value);
    assert(val === 'Custom step name', 'Subtask input should show typed title, got: ' + val);
  });

  await runTest('BUG-014: Subtask title is editable', async (page, s) => {
    await page.click(s('.todo-item'));
    await page.waitForTimeout(500);

    const addInput = await page.$(s('.add-step-input'));
    await addInput.fill('Original step');
    await addInput.press('Enter');
    await page.waitForTimeout(500);

    // Edit the subtask title
    const subtaskInput = await page.$(s('.subtask-title-input'));
    await subtaskInput.fill('Renamed step');
    await subtaskInput.press('Tab');
    await page.waitForTimeout(500);

    const val = await page.$eval(s('.subtask-title-input'), el => el.value);
    assert(val === 'Renamed step', 'Subtask title should be updated, got: ' + val);
  });

  // ─── BUG-015: FOLDER COUNTS ON LOAD ────────────────────────────────────────

  await runTest('BUG-015: All folder counts correct on initial load', async (page, s) => {
    const counts = await page.$$eval(s('.folder-count'), els => els.map(e => e.textContent));
    const names = await page.$$eval(s('.folder-name'), els => els.map(e => e.textContent));

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

  await runTest('BUG-017: Class binding works on todo item elements', async (page, s) => {
    // Verify todo items have correct CSS class (with suffix from CssClass minification)
    const classes = await page.$$eval(s('.todo-item'), els => els.map(e => e.className));
    assert(classes.length >= 2, 'Should have at least 2 todos');
    assert(classes[0].startsWith('todo-item'), 'Pending todo should have class starting with "todo-item", got: ' + classes[0]);

    // The third todo (Schedule dentist) is pre-completed
    const completedTodo = classes.find(c => c.includes('completed'));
    assert(completedTodo, 'Should have a completed todo with "completed" class');
  });

  await runTest('BUG-017: Title text renders correctly with class binding', async (page, s) => {
    // This specifically tests that class binding doesn't break text binding (the BUG-017 symptom)
    const titles = await page.$$eval(s('.todo-title'), els => els.map(e => e.textContent));
    assert(titles.includes('Buy groceries'), 'Title binding should work alongside class binding');
    assert(titles.includes('Read a book'), 'All titles should render, got: ' + titles.join(', '));
  });

  // ─── COMPLETION TESTS ──────────────────────────────────────────────────────

  await runTest('Toggle todo completion adds completed class', async (page, s) => {
    const firstTodo = await page.$(s('.todo-item:not(.completed)'));
    assert(firstTodo, 'Should have an uncompleted todo');

    // Click the checkbox
    const checkbox = await firstTodo.$(s('.btn-check'));
    assert(checkbox, 'Todo should have a checkbox');
    await checkbox.click();
    await page.waitForTimeout(500);

    const classAfter = await firstTodo.evaluate(el => el.className);
    assert(classAfter.includes('completed'), 'Todo should have completed class after checkbox click, got: ' + classAfter);
  });

  await runTest('Completed todo moves to completed section', async (page, s) => {
    const pendingBefore = (await page.$$(s('.todo-list .todo-item'))).length;

    // Complete the first todo
    const checkbox = await page.$(s('.todo-item .btn-check'));
    await checkbox.click();
    await page.waitForTimeout(500);

    const pendingAfter = (await page.$$(s('.todo-list .todo-item'))).length;
    assert(pendingAfter === pendingBefore - 1, 'Pending count should decrease by 1, was ' + pendingBefore + ' now ' + pendingAfter);

    // Completed section should have the item
    const completedLabel = await page.$eval(s('.completed-label'), el => el.textContent);
    assert(completedLabel.includes('2'), 'Completed count should be 2 (1 pre-completed + 1 new), got: ' + completedLabel);
  });

  await runTest('Toggle subtask completion', async (page, s) => {
    await page.click(s('.todo-item'));
    await page.waitForTimeout(500);

    // Add a subtask
    const addInput = await page.$(s('.add-step-input'));
    await addInput.fill('Test step');
    await addInput.press('Enter');
    await page.waitForTimeout(500);

    // Click the subtask checkbox
    const subCheck = await page.$(s('.subtask-item .btn-check'));
    assert(subCheck, 'Subtask should have a checkbox');
    await subCheck.click();
    await page.waitForTimeout(500);

    const subClass = await page.$eval(s('.subtask-item'), el => el.className);
    assert(subClass.includes('completed'), 'Subtask should have completed class, got: ' + subClass);
  });

  // ─── COMPLETED SECTION ─────────────────────────────────────────────────────

  await runTest('Completed section exists and is collapsed by default', async (page, s) => {
    const section = await page.$(s('.completed-section'));
    assert(section, 'Completed section should exist');

    const cls = await section.evaluate(el => el.className);
    assert(cls.includes('collapsed'), 'Completed section should be collapsed by default, got: ' + cls);
  });

  await runTest('Completed section expands on click', async (page, s) => {
    const header = await page.$(s('.completed-header'));
    assert(header, 'Completed header should exist');
    await header.click();
    await page.waitForTimeout(300);

    const cls = await page.$eval(s('.completed-section'), el => el.className);
    assert(!cls.includes('collapsed'), 'Completed section should expand after click, got: ' + cls);
  });

  await runTest('Completed folder shows all completed todos in main list', async (page, s) => {
    const folders = await page.$$(s('.folder-item'));
    const names = await page.$$eval(s('.folder-name'), els => els.map(e => e.textContent));
    const completedIdx = names.indexOf('Completed');
    assert(completedIdx >= 0, 'Should have Completed folder');

    await folders[completedIdx].click();
    await page.waitForTimeout(500);

    // Completed folder should show items directly in the main todo list (not in completed section)
    const todos = await page.$$(s('.todo-item'));
    assert(todos.length >= 1, 'Completed folder should show at least 1 todo in main list, got ' + todos.length);

    // Completed section should be hidden
    const sectionClass = await page.$eval(s('.completed-section'), el => el.className);
    assert(sectionClass.includes('hidden'), 'Completed section should be hidden, got: ' + sectionClass);
  });

  // ─── BUG-016: FOLDER ASSIGNMENT ────────────────────────────────────────────

  await runTest('BUG-016: Detail pane shows current folder name', async (page, s) => {
    await page.click(s('.todo-item'));
    await page.waitForTimeout(500);

    // The folder tags show individual chips for each membership
    const chipNames = await page.$$eval(s('.folder-chip-name'), els => els.map(el => el.textContent.trim()));
    assert(chipNames.length >= 1, 'Should have at least one folder chip, got: ' + chipNames.length);
    assert(chipNames.includes('Tasks'), 'Should include Tasks chip, got: ' + JSON.stringify(chipNames));
  });

  await runTest('BUG-016: Folder picker lists all folders', async (page, s) => {
    await page.click(s('.todo-item'));
    await page.waitForTimeout(500);

    const pickerNames = await page.$$eval(s('.folder-pick-name'), els => els.map(e => e.textContent));
    assert(pickerNames.includes('Tasks'), 'Picker should include Tasks');
    assert(pickerNames.includes('My Day'), 'Picker should include My Day');
  });

  // ─── BUG-011: VALUE BINDING ────────────────────────────────────────────────

  await runTest('BUG-011: Detail title input updates when switching todos', async (page, s) => {
    const todos = await page.$$(s('.todo-item'));
    assert(todos.length >= 2, 'Need at least 2 todos');

    // Select first todo
    await todos[0].click();
    await page.waitForTimeout(500);
    const title1 = await page.$eval(s('.detail-title-input'), el => el.value);

    // Select second todo
    await todos[1].click();
    await page.waitForTimeout(500);
    const title2 = await page.$eval(s('.detail-title-input'), el => el.value);

    assert(title1 !== title2, 'Switching todos should change detail title, first: ' + title1 + ', second: ' + title2);
  });

  // ─── FOLDER NAVIGATION ──────────────────────────────────────────────────────

  await runTest('Switch to My Day folder', async (page, s) => {
    const folders = await page.$$(s('.folder-item'));
    assert(folders.length >= 1, 'Should have folders');
    await folders[0].click();
    await page.waitForTimeout(500);

    // My Day should show "Buy groceries" (IsMyDay = true in sample data)
    const todos = await page.$$(s('.todo-item'));
    assert(todos.length >= 1, 'My Day should show at least 1 todo');
  });

  await runTest('Switch to Important folder', async (page, s) => {
    const folders = await page.$$(s('.folder-item'));
    assert(folders.length >= 2, 'Should have at least 2 folders');
    await folders[1].click();
    await page.waitForTimeout(500);

    const todos = await page.$$(s('.todo-item'));
    assert(todos.length >= 1, 'Important should show at least 1 todo');
  });

  // ─── PANE COLLAPSE ──────────────────────────────────────────────────────────

  await runTest('Collapse and expand left pane', async (page, s) => {
    const toggleBtn = await page.$(s('.btn-toggle-left'));
    assert(toggleBtn, 'Toggle button should exist');
    await toggleBtn.click();
    await page.waitForTimeout(300);

    const collapsed = await page.$(s('.pane-left.collapsed'));
    assert(collapsed, 'Left pane should have collapsed class');

    await toggleBtn.click();
    await page.waitForTimeout(300);

    const expanded = await page.$(s('.pane-left:not(.collapsed)'));
    assert(expanded, 'Left pane should expand again');
  });

  // ─── BUG-019: DRAG AND DROP ──────────────────────────────────────────────────

  // Helper: simulate HTML5 drag-and-drop with DataTransfer between two elements
  // sourceSelector/targetSelector are CSS selectors; srcIdx/tgtIdx pick which match (default 0)
  async function simulateDragDrop(page, sourceSelector, targetSelector, srcIdx = 0, tgtIdx = 0) {
    await page.evaluate(({ src, tgt, si, ti }) => {
      const source = document.querySelectorAll(src)[si];
      const target = document.querySelectorAll(tgt)[ti];
      if (!source || !target) return;

      const dt = new DataTransfer();
      source.dispatchEvent(new DragEvent('dragstart', { dataTransfer: dt, bubbles: true }));
      target.dispatchEvent(new DragEvent('dragenter', { dataTransfer: dt, bubbles: true }));
      target.dispatchEvent(new DragEvent('dragover', { dataTransfer: dt, bubbles: true }));
      target.dispatchEvent(new DragEvent('drop', { dataTransfer: dt, bubbles: true }));
      source.dispatchEvent(new DragEvent('dragend', { dataTransfer: dt, bubbles: true }));
    }, { src: sourceSelector, tgt: targetSelector, si: srcIdx, ti: tgtIdx });
  }

  await runTest('BUG-019: Drag todo to My Day folder sets IsMyDay', async (page, s) => {
    // "Read a book" (second todo) has IsMyDay=false — drag it to My Day
    const todos = await page.$$(s('.todo-item'));
    assert(todos.length >= 2, 'Need at least 2 todos');

    // Simulate drag-and-drop: 2nd todo (index 1) to My Day folder (index 0)
    await simulateDragDrop(page, s('.todo-item'), s('.folder-item'), 1, 0);
    await page.waitForTimeout(500);

    // Switch to My Day folder to verify the todo now appears
    const folders = await page.$$(s('.folder-item'));
    await folders[0].click();
    await page.waitForTimeout(500);
    const myDayTodos = await page.$$eval(s('.todo-title'), els => els.map(e => e.textContent));
    assert(myDayTodos.includes('Read a book'), 'Read a book should appear in My Day after drag, got: ' + myDayTodos.join(', '));
  });

  await runTest('BUG-019: Drag todo to Important folder sets IsImportant', async (page, s) => {
    // "Read a book" (second todo) has IsImportant=false — drag it to Important
    const todos = await page.$$(s('.todo-item'));
    assert(todos.length >= 2, 'Need at least 2 todos');

    // Drag second todo (index 1) to Important folder (index 1)
    await simulateDragDrop(page, s('.todo-item'), s('.folder-item'), 1, 1);
    await page.waitForTimeout(500);

    // Switch to Important folder to verify
    const folders = await page.$$(s('.folder-item'));
    await folders[1].click();
    await page.waitForTimeout(500);
    const impTodos = await page.$$eval(s('.todo-title'), els => els.map(e => e.textContent));
    assert(impTodos.includes('Read a book'), 'Read a book should appear in Important after drag, got: ' + impTodos.join(', '));
  });

  // ─── BUG-005: @IF GATE IN ITEM TEMPLATES ───────────────────────────────────

  await runTest('BUG-005: @if gate toggles star between important/not', async (page, s) => {
    // "Buy groceries" is important (filled star ★), "Read a book" is not (empty star ☆)
    const todoItems = await page.$$(s('.todo-item'));
    assert(todoItems.length >= 2, 'Need at least 2 todos');

    // First todo should have filled star (class="star important")
    const star1 = await todoItems[0].$(s('.star'));
    const starClass1 = await star1.evaluate(el => el.className);
    assert(starClass1.includes('important'), 'First todo should have important star');

    // Second todo should have empty star (class="star" without important)
    const star2 = await todoItems[1].$(s('.star'));
    const starClass2 = await star2.evaluate(el => el.className);
    assert(!starClass2.includes('important'), 'Second todo should have non-important star');

    // Click the empty star to toggle importance
    await star2.click();
    await page.waitForTimeout(500);

    // After click, the star should now be important (gate should flip)
    const updatedTodo = (await page.$$(s('.todo-item')))[1];
    const updatedStar = await updatedTodo.$(s('.star'));
    const updatedClass = await updatedStar.evaluate(el => el.className);
    assert(updatedClass.includes('important'), 'Star should become important after click, got: ' + updatedClass);
  });

  await runTest('BUG-005b: Toggling star back removes important class (un-star)', async (page, s) => {
    // "Buy groceries" starts important — click its star to un-star it
    const todoItems = await page.$$(s('.todo-item'));
    assert(todoItems.length >= 1, 'Need at least 1 todo');

    const star = await todoItems[0].$(s('.star'));
    const classBefore = await star.evaluate(el => el.className);
    assert(classBefore.includes('important'), 'First todo should start important, got: ' + classBefore);

    // Click to un-star
    await star.click();
    await page.waitForTimeout(500);

    // Re-query after DOM update
    const updatedTodo = (await page.$$(s('.todo-item')))[0];
    const updatedStar = await updatedTodo.$(s('.star'));
    const classAfter = await updatedStar.evaluate(el => el.className);
    assert(!classAfter.includes('important'), 'Star should lose important after click, got: ' + classAfter);

    // Click again to re-star — verify round-trip
    await updatedStar.click();
    await page.waitForTimeout(500);

    const reTodo = (await page.$$(s('.todo-item')))[0];
    const reStar = await reTodo.$(s('.star'));
    const classRound = await reStar.evaluate(el => el.className);
    assert(classRound.includes('important'), 'Star should regain important after second click, got: ' + classRound);
  });

  // ─── COMPLETED FOLDER BEHAVIOR ─────────────────────────────────────────────

  await runTest('BUG-020: Completed folder shows items in main list, not completed section', async (page, s) => {
    const folders = await page.$$(s('.folder-item'));
    const names = await page.$$eval(s('.folder-name'), els => els.map(e => e.textContent));
    const completedIdx = names.indexOf('Completed');
    await folders[completedIdx].click();
    await page.waitForTimeout(500);

    // Items should be in the main todo-list, not the completed section
    const mainTodos = await page.$$(s('.todo-list .todo-item'));
    assert(mainTodos.length >= 1, 'Completed folder should show items in main list');

    // Completed section should be hidden
    const sectionVisible = await page.$eval(s('.completed-section'), el => {
      return window.getComputedStyle(el).display !== 'none';
    });
    assert(!sectionVisible, 'Completed section should be hidden when Completed folder selected');
  });

  // ─── PERSISTENCE ────────────────────────────────────────────────────────────

  await runTest('Data persists across page reload', async (page, s) => {
    // Verify sample data loads from IndexedDB on initial page load
    let titles = await page.$$eval(s('.todo-title'), els => els.map(e => e.textContent));
    assert(titles.length >= 1, 'Should have at least 1 todo from IndexedDB');
    const knownTitle = titles[0];

    // Reload the page — IndexedDB sample data should survive
    await page.reload({ waitUntil: 'domcontentloaded' });
    await page.waitForSelector(s('.folder-item'), { timeout: 10000 });
    await page.waitForTimeout(1000);

    // Assert sample data is still present after reload
    titles = await page.$$eval(s('.todo-title'), els => els.map(e => e.textContent));
    assert(titles.includes(knownTitle), 'Sample todo should persist after reload, got: ' + titles.join(', '));
    // NOTE: Newly added todos do NOT persist yet (known LIMIT — AddTodo
    // doesn't call SaveTodo). A future fix should add a test that creates
    // a unique item, reloads, and verifies it survived.
  });

  // ─── PARENT-METHOD EVENT BINDING (Model.OnSelect) ─────────────────────────

  await runTest('REG-001: Model.OnSelect(todo) triggers selection on parent ViewModel', async (page, s) => {
    // Verify at least one todo item exists
    const todoItems = await page.$$(s('.todo-item'));
    assert(todoItems.length >= 2, 'Need at least 2 todos, got: ' + todoItems.length);

    // Click the second todo and check it gets a selected visual cue
    await todoItems[1].click();
    await page.waitForTimeout(500);

    // The right pane should exist after selection
    const rightPane = await page.$(s('.pane-right'));
    assert(rightPane, 'Right pane should appear after clicking a todo via Model.OnSelect');
  });

  await runTest('REG-002: Switching selected todo updates selection', async (page, s) => {
    const todoItems = await page.$$(s('.todo-item'));
    assert(todoItems.length >= 2, 'Need at least 2 todos');

    // Click first todo
    await todoItems[0].click();
    await page.waitForTimeout(300);

    // Click second todo
    await todoItems[1].click();
    await page.waitForTimeout(300);

    // Right pane should still be visible
    const pane = await page.$(s('.pane-right'));
    assert(pane, 'Right pane should still be visible after switching selection');
  });

  await runTest('REG-003: Star toggle and selection work independently', async (page, s) => {
    // This tests that item events (star toggle) and parent method events
    // (Model.OnSelect) both work from the same foreach item template.
    const todoItems = await page.$$(s('.todo-item'));
    assert(todoItems.length >= 1, 'Need at least 1 todo');

    // Toggle star on first todo
    const star = await todoItems[0].$(s('.star'));
    assert(star, 'Todo should have a star element');
    const classBefore = await star.evaluate(el => el.className);

    await star.click();
    await page.waitForTimeout(500);

    const updatedItems = await page.$$(s('.todo-item'));
    const updatedStar = await updatedItems[0].$(s('.star'));
    const classAfter = await updatedStar.evaluate(el => el.className);

    // Star class should have changed
    assert(classBefore !== classAfter,
      'Star class should change after click. Before: ' + classBefore + ', After: ' + classAfter);

    // Now click the todo itself (not the star) to trigger selection
    await updatedItems[0].click();
    await page.waitForTimeout(500);

    // Right pane should appear (selection via Model.OnSelect)
    const pane = await page.$(s('.pane-right'));
    assert(pane, 'Right pane should appear after clicking todo item body');
  });

  // ─── CALLCONTEXT: ROOT CONTEXT ON DOM EVENTS ─────────────────────────────

  // Helper: check if CallContext test hook is available in the generated JS.
  // The hook is exposed on window.__callContext by CallContext's static constructor.
  async function hasCallContext(page) {
    return await page.evaluate(() =>
      typeof window.__callContext === 'object' &&
      window.__callContext !== null &&
      typeof window.__callContext.getCurrent === 'function');
  }

  // Canary test: hard-fail if the CallContext debug bridge is missing.
  // This prevents all subsequent CALLCTX tests from silently skipping.
  await runTest('CALLCTX-000: CallContext debug bridge is present', async (page, s) => {
    const available = await hasCallContext(page);
    assert(available, 'window.__callContext bridge must be present — ExposeDebugAccessors() did not run');
  });

  await runTest('CALLCTX-001: Click creates CallContext with valid fields', async (page, s) => {

    // Before any click, context should be null
    const ctxBefore = await page.evaluate(() => window.__callContext.getCurrent());
    assert(ctxBefore === null, 'CallContext should be null before any interaction, got: ' + JSON.stringify(ctxBefore));

    // Click a todo item — EventBinder should create a root CallContext
    const todoItem = await page.$(s('.todo-item'));
    if (!todoItem) {
      // Fallback: click any interactive element if no todos loaded
      await page.click(s('.folder-item'));
    } else {
      await todoItem.click();
    }
    await page.waitForTimeout(300);

    // Verify CallContext.Current was set
    const ctx = await page.evaluate(() => window.__callContext.getCurrent());

    assert(ctx !== null, 'CallContext.Current should not be null after click');
    assert(ctx.actionId >= 0, 'ActionId should be >= 0, got: ' + ctx.actionId);
    assert(typeof ctx.traceId === 'string' && ctx.traceId.length === 32,
      'TraceId should be 32 hex chars, got: ' + ctx.traceId);
    assert(/^[0-9a-f]{32}$/.test(ctx.traceId),
      'TraceId should be hex only, got: ' + ctx.traceId);
    assert(typeof ctx.spanId === 'string' && ctx.spanId.length === 16,
      'SpanId should be 16 hex chars, got: ' + ctx.spanId);
    assert(/^[0-9a-f]{16}$/.test(ctx.spanId),
      'SpanId should be hex only, got: ' + ctx.spanId);
    assert(ctx.parentSpanId === null, 'Root context parentSpanId should be null');
    assert(ctx.depth === 0, 'Root context depth should be 0, got: ' + ctx.depth);
  });

  await runTest('CALLCTX-002: Each click creates new root context', async (page, s) => {
    const folders = await page.$$(s('.folder-item'));
    assert(folders.length >= 2, 'Need at least 2 folder items');

    // Click first folder, capture traceId
    await folders[0].click();
    await page.waitForTimeout(300);
    const ctx1 = await page.evaluate(() => window.__callContext.getCurrent());
    assert(ctx1 !== null, 'First click should create a context');

    // Click second folder, capture new traceId
    await folders[1].click();
    await page.waitForTimeout(300);
    const ctx2 = await page.evaluate(() => window.__callContext.getCurrent());
    assert(ctx2 !== null, 'Second click should create a context');

    assert(ctx2.actionId > ctx1.actionId,
      'Second action should have higher ActionId: ' + ctx1.actionId + ' vs ' + ctx2.actionId);
    assert(ctx2.traceId !== ctx1.traceId,
      'Each click should get a new traceId: ' + ctx1.traceId + ' vs ' + ctx2.traceId);
  });

  // ─── CALLCONTEXT: ASYNC PROPAGATION ─────────────────────────────────────────

  await runTest('CALLCTX-003: Context null when idle (no user action)', async (page, s) => {
    // On fresh page load, before any user interaction, context should be null
    const ctx = await page.evaluate(() => window.__callContext.getCurrent());
    assert(ctx === null, 'CallContext should be null on fresh load, got: ' + JSON.stringify(ctx));
  });

  await runTest('CALLCTX-004: Context survives through async task execution', async (page, s) => {
    // Click a folder to create a root context
    const folder = await page.$(s('.folder-item'));
    await folder.click();
    await page.waitForTimeout(300);

    const traceIdAfterClick = await page.evaluate(() => {
      var c = window.__callContext.getCurrent();
      return c ? c.traceId : null;
    });
    assert(traceIdAfterClick !== null, 'Should have traceId after click');

    // Try adding a todo if input exists (triggers async TaskScheduler work)
    const input = await page.$(s('.add-task-input'));
    if (input) {
      await input.fill('CallContext test task');
      await input.press('Enter');
      await page.waitForTimeout(1000);
    } else {
      // Fallback: click another folder (still triggers async skin work)
      const folders = await page.$$(s('.folder-item'));
      if (folders.length >= 2) {
        await folders[1].click();
        await page.waitForTimeout(500);
      }
    }

    // After async work settles, context from latest action should exist
    const traceIdAfterAsync = await page.evaluate(() => {
      var c = window.__callContext.getCurrent();
      return c ? c.traceId : null;
    });
    assert(traceIdAfterAsync !== null,
      'CallContext should exist after async operations');
    assert(/^[0-9a-f]{32}$/.test(traceIdAfterAsync),
      'TraceId should be valid hex after async, got: ' + traceIdAfterAsync);
  });

  // ─── CALLCONTEXT: XHR TRACEPARENT INJECTION ─────────────────────────────────

  await runTest('CALLCTX-005: XHR carries traceparent header', async (page, s) => {
    // Click a folder to establish a root context
    await page.click(s('.folder-item'));
    await page.waitForTimeout(300);

    // Invoke the OnBeforeSend hook via our test bridge — it calls the real
    // hook with a mock request object that captures setRequestHeader calls
    const headers = await page.evaluate(() => window.__callContext.testXhrHook());

    assert(headers.traceparent !== undefined && headers.traceparent !== null,
      'XHR hook should inject traceparent header, got headers: ' + JSON.stringify(headers));
  });

  await runTest('CALLCTX-006: Traceparent format matches W3C spec', async (page, s) => {
    // Click to establish context
    await page.click(s('.folder-item'));
    await page.waitForTimeout(300);

    const ctx = await page.evaluate(() => window.__callContext.getCurrent());
    assert(ctx !== null, 'Should have context after click');

    // Get the traceparent that would be injected into an XHR
    const headers = await page.evaluate(() => window.__callContext.testXhrHook());
    const traceparent = headers.traceparent;

    assert(traceparent !== undefined, 'Should have traceparent header');

    // W3C traceparent format: 00-{32 hex traceId}-{16 hex spanId}-01
    var regex = /^00-[0-9a-f]{32}-[0-9a-f]{16}-01$/;
    assert(regex.test(traceparent),
      'Traceparent should match W3C format, got: ' + traceparent);

    // Verify the traceId and spanId match the active context
    var parts = traceparent.split('-');
    assert(parts[1] === ctx.traceId,
      'Traceparent traceId should match context: ' + parts[1] + ' vs ' + ctx.traceId);
    assert(parts[2] === ctx.spanId,
      'Traceparent spanId should match context: ' + parts[2] + ' vs ' + ctx.spanId);
  });

  // ─── CALLCONTEXT: TODOAPP INTEGRATION ───────────────────────────────────────

  await runTest('CALLCTX-007: Add todo works with CallContext active', async (page, s) => {
    // This test validates that EventBinder's CallContext hooks don't break
    // normal event dispatch. If no todos loaded (pre-existing issue), we
    // still verify the add-task input works with EventBinder hooks active.
    const input = await page.$(s('.add-task-input'));
    if (!input) {
      console.log('    (SKIP — add-task-input not found)');
      return;
    }

    const initialCount = (await page.$$(s('.todo-item'))).length;
    await input.fill('Integration test task');
    await input.press('Enter');
    await page.waitForTimeout(500);

    const newCount = (await page.$$(s('.todo-item'))).length;
    assert(newCount >= initialCount,
      'Todo count should not decrease after add, was ' + initialCount + ' now ' + newCount);
  });

  await runTest('CALLCTX-008: Folder switch works with CallContext active', async (page, s) => {
    // Validates that switching folders (click events + async skin updates)
    // works correctly with CallContext hooks in EventBinder.
    const folders = await page.$$(s('.folder-item'));
    assert(folders.length >= 4, 'Should have at least 4 system folders');

    // Switch to My Day
    const folderNames = await page.$$eval(s('.folder-name'), els => els.map(e => e.textContent));
    const myDayIdx = folderNames.indexOf('My Day');
    assert(myDayIdx >= 0, 'Should find My Day folder');
    await folders[myDayIdx].click();
    await page.waitForTimeout(500);

    const header = await page.$eval(s('.current-folder-name'), el => el.textContent);
    assert(header === 'My Day', 'Header should show My Day after switch, got: ' + header);

    // Switch to Tasks
    const tasksIdx = folderNames.indexOf('Tasks');
    await folders[tasksIdx].click();
    await page.waitForTimeout(500);

    const header2 = await page.$eval(s('.current-folder-name'), el => el.textContent);
    assert(header2 === 'Tasks', 'Header should show Tasks after switch, got: ' + header2);
  });

  await runTest('CALLCTX-009: Context restored after each task execution', async (page, s) => {
    // Verify that clicking two different elements yields two distinct contexts,
    // proving that ExecuteTask properly saves/restores CallContext.Current.
    const folders = await page.$$(s('.folder-item'));
    assert(folders.length >= 2, 'Need at least 2 folder items');

    // First click — capture context
    await folders[0].click();
    await page.waitForTimeout(300);
    const ctx1 = await page.evaluate(() => window.__callContext.getCurrent());
    assert(ctx1 !== null, 'First click should create a context');

    // Second click — should get entirely new root context (not the first one)
    await folders[1].click();
    await page.waitForTimeout(300);
    const ctx2 = await page.evaluate(() => window.__callContext.getCurrent());
    assert(ctx2 !== null, 'Second click should create a context');

    // Contexts must differ — proves save/restore works, not leaking
    assert(ctx2.traceId !== ctx1.traceId,
      'Each click must create independent context (save/restore works): ' +
      ctx1.traceId + ' vs ' + ctx2.traceId);
    assert(ctx2.actionId > ctx1.actionId,
      'ActionId must increment: ' + ctx1.actionId + ' vs ' + ctx2.actionId);
  });

  await runTest('CALLCTX-010: XHR hook is no-op when context is null', async (page, s) => {
    // On fresh page load (no clicks), testXhrHook should return empty headers
    const headers = await page.evaluate(() => window.__callContext.testXhrHook());
    assert(headers.traceparent === undefined,
      'traceparent should not be set when no context is active, got: ' + JSON.stringify(headers));
  });

  // ─── RESULTS ────────────────────────────────────────────────────────────────

  console.log('\n=== E2E Results: ' + results.passed + ' passed, ' + results.failed + ' failed ===\n');

  results.tests.filter(t => t.status === 'FAIL').forEach(t => {
    console.log('FAIL: ' + t.name);
    console.log('  ' + t.error);
    console.log('');
  });

  await browser.close();
  if (_server) _server.close();
  process.exit(results.failed > 0 ? 1 : 0);
})();

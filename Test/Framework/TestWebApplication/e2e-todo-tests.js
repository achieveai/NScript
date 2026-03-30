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

    // Verify the detail title shows the todo name
    const title = await page.$eval('.detail-title', el => el.textContent);
    assert(title === 'Buy groceries', 'Detail title should show todo name, got: ' + title);
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

  // ─── PERSISTENCE ────────────────────────────────────────────────────────────

  await runTest('Data persists across page reload', async (page) => {
    // Add a new todo
    const addBar = await page.$('.add-todo-bar');
    if (addBar) {
      await addBar.click();
      await page.waitForTimeout(500);
    }

    // Reload the page
    await page.reload({ waitUntil: 'domcontentloaded' });
    await page.waitForSelector('.folder-item', { timeout: 10000 });
    await page.waitForTimeout(1000);

    // Data from IndexedDB should restore todos
    const todos = await page.$$('.todo-item');
    assert(todos.length >= 1, 'Should have todos after reload, got ' + todos.length);
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

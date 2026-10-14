// Live patching end to end (docs/build/build-service.md, "Live patching in the open page"):
// with TodoApp open and a draft typed, a ToggleImportant body edit is saved. The page applies
// it with no reload: an old row's star runs the new code, the draft stays. Reverting the edit
// patches again. Then a HandleClick edit (S2): the handler bound when each row was made picks
// up the new body, on a row from before the save and on one added after it. Then a generic
// edit (S3): ObservableCollection<T>.Add reports a new item at the top. The list the page
// already has runs it, so does an instantiation made after the save and a new tab (the
// bundle's factory); after the revert a new add lands at the end again. Needs the watch
// running with patch emit:
//   NSCRIPT_PATCH_EMIT=1 dotnet build Test/Framework/TodoApp/TodoApp.csproj -p:NScriptWatch=true
// Exit 0 pass, 1 fail, 2 skipped (the bundle has no live patch client).
import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { chromium } from 'playwright';

const ROOT = path.dirname(fileURLToPath(import.meta.url));
const CS = path.join(ROOT, '../TodoApp/ViewModels/TodoItemViewModel.cs');
const OLD = 'this.IsImportant = !this.IsImportant;';
const NEW = 'this.IsImportant = !this.IsImportant; this.Title = this.Title + "*";';
const CONTROL = path.join(ROOT, '../TodoApp/Controls/TodoItemControl.cs');
const HANDLER = 'if (this.OnSelected != null)';
// Only members the bundle already has: a new one is a full emit and a reload.
const HANDLER_NEW = 'var item = this.DataContext as TodoApp.ViewModels.TodoItemViewModel; if (item != null) item.Title = item.Title + "!"; if (this.OnSelected != null)';
const COLLECTION = path.join(ROOT, '../../../Sources/Framework/Sunlight.Framework/Observables/ObservableCollection.cs');
const ADD = 'this.items.Add(o);';
const ADD_NEW = 'this.items.Add(o); if (this.Count > 1) { this.OnCollectionChanged(CollectionChangedAction.Add, 0, new T[] { o }, null); this.FirePropertyChanged("Count"); return; }';
const MIME = { '.htm': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.map': 'application/json' };

const bundle = fs.readFileSync(path.join(ROOT, 'GeneratedScripts/TodoApp.js'), 'utf8');
if (!/__nsHmrBoot\("TodoApp\.js\|[0-9a-f]+\|ws:/.test(bundle)) {
  console.log('SKIPPED: GeneratedScripts/TodoApp.js has no live patch client with a socket. Start the watch with NSCRIPT_PATCH_EMIT=1.');
  process.exit(2);
}

const source = fs.readFileSync(CS, 'utf8');
if (!source.includes(OLD) || source.includes(NEW)) {
  console.log('FAIL: TodoItemViewModel.cs does not hold the expected ToggleImportant body.');
  process.exit(1);
}

const control = fs.readFileSync(CONTROL, 'utf8');
if (!control.includes(HANDLER) || control.includes(HANDLER_NEW)) {
  console.log('FAIL: TodoItemControl.cs does not hold the expected HandleClick body.');
  process.exit(1);
}

const collection = fs.readFileSync(COLLECTION, 'utf8');
if (!collection.includes(ADD) || collection.includes(ADD_NEW)) {
  console.log('FAIL: ObservableCollection.cs does not hold the expected Add body.');
  process.exit(1);
}

// The factory and its Add slot, by the names the dev bundle gives them.
const factory = /\/\*@G Sunlight\.Framework\.Observables\.ObservableCollection`1\*\/\r?\n\s*\S+\.__nsEval[^\n]*\r?\n\s*\((\S+)\.__nsInsts/.exec(bundle);
const addSlot = /\$\$ptyp\.(\S+) = function Sunlight_Framework_Observables_ObservableCollection_11\$Add\(/.exec(bundle);
if (!factory || !addSlot) {
  console.log('FAIL: the bundle has no ObservableCollection factory hook or Add slot.');
  process.exit(1);
}

const server = http.createServer((req, res) => {
  const file = path.join(ROOT, decodeURIComponent(req.url.split('?')[0]));
  fs.readFile(file, (error, data) => {
    if (error) { res.writeHead(404); res.end(); return; }
    res.writeHead(200, { 'Content-Type': MIME[path.extname(file)] || 'application/octet-stream' });
    res.end(data);
  });
});
await new Promise(resolve => server.listen(0, '127.0.0.1', resolve));
const url = `http://127.0.0.1:${server.address().port}/TodoApp.htm`;

const browser = await chromium.launch({ headless: true });
const page = await browser.newPage();
const logs = [], errors = [];
let loads = 0, edited = false, handlerEdited = false, collectionEdited = false;
page.on('console', message => { if (message.text().startsWith('[nscript-hmr]')) logs.push(message.text()); });
page.on('pageerror', error => errors.push(error.message));
const until = async (predicate, ms = 30000) => {
  for (const start = Date.now(); !predicate(); await page.waitForTimeout(100)) {
    if (Date.now() - start > ms) return false;
  }
  return true;
};
const applied = () => logs.filter(line => line.includes(' applied ')).length;
const failures = [];
const check = (ok, what) => { console.log(`${ok ? 'PASS' : 'FAIL'}: ${what}`); if (!ok) failures.push(what); };

try {
  await page.goto(url);
  await page.evaluate(() => new Promise(resolve => { const q = indexedDB.deleteDatabase('TodoAppDb'); q.onsuccess = q.onerror = q.onblocked = () => resolve(); }));
  page.on('load', () => loads++);
  await page.goto(url);
  await page.waitForSelector('input[placeholder="Add a task"]', { timeout: 15000 });
  check(await until(() => logs.some(line => line.includes('connected TodoApp.js'))), 'the page connects to the build service');

  await page.fill('input[placeholder="Add a task"]', 'draft kept');
  const title = () => page.evaluate(() => document.querySelector('[class^=todo-title]').textContent);
  const before = await title();

  fs.writeFileSync(CS, source.replace(OLD, NEW));
  edited = true;
  check(await until(() => applied() >= 1), 'the save is applied live');
  await page.click('[data-star-gate]');
  await page.waitForTimeout(300);
  check((await title()) === before + '*', 'a row made before the save runs the new code');
  check((await page.inputValue('input[placeholder="Add a task"]')) === 'draft kept', 'the draft is kept');

  fs.writeFileSync(CS, source);
  edited = false;
  check(await until(() => applied() >= 2), 'the revert is applied live');

  // S2: rows bind HandleClick when they are made; the delegate reads it at each call.
  fs.writeFileSync(CONTROL, control.replace(HANDLER, HANDLER_NEW));
  handlerEdited = true;
  check(await until(() => applied() >= 3), 'the handler save is applied live');
  await page.press('input[placeholder="Add a task"]', 'Enter');
  await page.waitForFunction(() => [...document.querySelectorAll('[class^=todo-title]')].filter(row => row.offsetParent).length >= 2, null, { timeout: 5000 });
  const rows = page.locator('[class^=todo-title]:visible');
  const texts = () => rows.evaluateAll(list => list.map(row => row.textContent));
  const rowsBefore = await texts();
  await rows.first().click();
  await rows.last().click();
  await page.waitForTimeout(300);
  const rowsAfter = await texts();
  check(rowsAfter[0] === rowsBefore[0] + "!" && rowsAfter[rowsAfter.length - 1] === rowsBefore[rowsBefore.length - 1] + "!",
    `a row from before the save and a new row run the new handler (${JSON.stringify(rowsBefore)} -> ${JSON.stringify(rowsAfter)})`);
  check(!logs.some(line => line.includes('reloading: delegate')), 'the handler edit needs no reload');

  fs.writeFileSync(CONTROL, control);
  handlerEdited = false;
  check(await until(() => applied() >= 4), 'the handler revert is applied live');
  await rows.first().click();
  await page.waitForTimeout(300);
  check((await texts())[0] === rowsAfter[0], 'after the revert a click runs the old handler again');

  // S3: a generic type's methods live in each instantiation's closure.
  const probe = (target, expression) => target.evaluate(text => { window.__nsPatch['TodoApp.js']('window.__probe = (' + text + ');'); return window.__probe; }, expression);
  const F = factory[1], slot = addSlot[1];
  // The runtime gives functions a type's toString: take a plain one from a fresh frame.
  const plainSource = target => target.evaluate(() => { const frame = document.body.appendChild(document.createElement('iframe')); window.__source = frame.contentWindow.Function.prototype.toString; frame.remove(); });
  await plainSource(page);
  const addText = instance => `window.__source.call(${instance}.prototype.${slot})`;
  const isNew = text => text.includes('> 1');
  fs.writeFileSync(COLLECTION, collection.replace(ADD, ADD_NEW));
  collectionEdited = true;
  check(await until(() => applied() >= 5), 'the generic save is applied live');
  check((await probe(page, `${F}.__nsInsts.length`)) > 0, 'the page made instantiations before the save');
  check(isNew(await probe(page, addText(`${F}.__nsInsts[0]`))), 'an instantiation made before the save runs the new Add');
  check(isNew(await probe(page, addText(`${F}(${F}.__nsInsts[0], true)`))), 'an instantiation made after the save runs the new Add');
  await page.fill('input[placeholder="Add a task"]', 'generic first');
  await page.press('input[placeholder="Add a task"]', 'Enter');
  await page.waitForTimeout(300);
  check((await texts())[0] === 'generic first', `the open list runs the new Add: the new row is at the top (${JSON.stringify(await texts())})`);
  const tab = await browser.newPage();
  await tab.goto(url);
  await tab.waitForSelector('input[placeholder="Add a task"]', { timeout: 15000 });
  await plainSource(tab);
  check(isNew(await probe(tab, addText(`${F}.__nsInsts[0]`))), 'a new tab runs the new Add from the bundle');
  await tab.close();

  fs.writeFileSync(COLLECTION, collection);
  collectionEdited = false;
  check(await until(() => applied() >= 6), 'the generic revert is applied live');
  check(!isNew(await probe(page, addText(`${F}(${F}(${F}.__nsInsts[0], true), true)`))), 'an instantiation made after the revert runs the old Add');
  await page.fill('input[placeholder="Add a task"]', 'generic last');
  await page.press('input[placeholder="Add a task"]', 'Enter');
  await page.waitForTimeout(300);
  const afterRevert = await texts();
  check(afterRevert[afterRevert.length - 1] === 'generic last', `after the revert a new row lands at the end (${JSON.stringify(afterRevert)})`);
  check(loads === 1, `no reload (loads=${loads})`);
  check(errors.length === 0, `no page errors (${errors.join('; ')})`);
} finally {
  if (edited) fs.writeFileSync(CS, source);
  if (handlerEdited) fs.writeFileSync(CONTROL, control);
  if (collectionEdited) fs.writeFileSync(COLLECTION, collection);
  console.log(logs.join('\n'));
  await browser.close();
  server.close();
}

process.exit(failures.length === 0 ? 0 : 1);

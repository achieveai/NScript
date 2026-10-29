// Razor skin live patching end to end (docs/build/build-service.md, "Live patching in the open
// page"): with TodoApp open, a row and a draft typed, an edit to TodoItemControl.skin.cshtml
// skins every open row again with no reload. The rows keep their view models, the draft stays,
// a re-skinned row's star still toggles, a row added after the save and a new tab get the new
// skin, and the revert goes live too. Then the app's outer skin (AppShell) is saved: the shell
// is skinned again around the same rows, and adding a row still works. Last a colour in the
// skins' stylesheet (AppShell.css): the open page takes it, a new tab reads it from the bundle.
// Needs the watch running with patch emit:
//   NSCRIPT_PATCH_EMIT=1 dotnet build Test/Framework/TodoApp/TodoApp.csproj -p:NScriptWatch=true
// Exit 0 pass, 1 fail, 2 skipped (the bundle has no live patch client).
import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { chromium } from 'playwright';

const ROOT = path.dirname(fileURLToPath(import.meta.url));
const SKIN = path.join(ROOT, '../TodoApp/RazorTemplates/TodoItemControl.skin.cshtml');
const OLD = '<div class="todo-title">@Model.Title</div>';
const NEW = OLD + '<span hmr="live">live</span>';
const SHELL = path.join(ROOT, '../TodoApp/RazorTemplates/AppShell.skin.cshtml');
const SHELL_OLD = '<div class="app-title">To Do</div>';
const SHELL_NEW = '<div class="app-title">To Do<span hmr="shell">!</span></div>';
const CSS = path.join(ROOT, '../TodoApp/RazorTemplates/AppShell.css');
const CSS_OLD = 'color: var(--color-primary, #2564cf);\n}\n\n.current-folder-name';
const CSS_NEW = 'color: rgb(7, 8, 9);\n}\n\n.current-folder-name';
const MIME = { '.htm': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.map': 'application/json' };

const bundle = fs.readFileSync(path.join(ROOT, 'GeneratedScripts/TodoApp.js'), 'utf8');
if (!/__nsHmrBoot\("TodoApp\.js\|[0-9a-f]+\|ws:/.test(bundle)) {
  console.log('SKIPPED: GeneratedScripts/TodoApp.js has no live patch client with a socket. Start the watch with NSCRIPT_PATCH_EMIT=1.');
  process.exit(2);
}

const source = fs.readFileSync(SKIN, 'utf8');
const shell = fs.readFileSync(SHELL, 'utf8');
const css = fs.readFileSync(CSS, 'utf8').replace(/\r\n/g, '\n');
const cssRaw = fs.readFileSync(CSS, 'utf8');
if (!source.includes(OLD) || source.includes(NEW) || !shell.includes(SHELL_OLD) || !css.includes(CSS_OLD)) {
  console.log('FAIL: the TodoApp skins do not hold ' + OLD + ' and ' + SHELL_OLD);
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
let loads = 0, edited = false, shellEdited = false, cssEdited = false;
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
const INPUT = 'input[placeholder="Add a task"]';
// The visible rows: their titles, and how many show the new skin.
const rows = target => target.evaluate(() => {
  const titles = [...document.querySelectorAll('[class^=todo-title]')].filter(row => row.offsetParent);
  return { titles: titles.map(row => row.textContent), live: titles.filter(row => row.parentElement.querySelector('[hmr=live]')).length };
});
const addRow = async (target, text) => {
  const count = (await rows(target)).titles.length;
  await target.fill(INPUT, text);
  await target.press(INPUT, 'Enter');
  await target.waitForFunction(n => [...document.querySelectorAll('[class^=todo-title]')].filter(row => row.offsetParent).length > n, count, { timeout: 5000 });
};

try {
  await page.goto(url);
  await page.evaluate(() => new Promise(resolve => { const q = indexedDB.deleteDatabase('TodoAppDb'); q.onsuccess = q.onerror = q.onblocked = () => resolve(); }));
  page.on('load', () => loads++);
  await page.goto(url);
  await page.waitForSelector(INPUT, { timeout: 15000 });
  check(await until(() => logs.some(line => line.includes('connected TodoApp.js'))), 'the page connects to the build service');
  await addRow(page, 'first row');
  await page.fill(INPUT, 'draft kept');
  await page.evaluate(() => { window.__keep = 42; });
  const styleCount = await page.evaluate(() => document.querySelectorAll('style').length);
  const before = await rows(page);
  check(before.titles.length >= 2 && before.live === 0, `the open rows show the old skin (${JSON.stringify(before)})`);

  fs.writeFileSync(SKIN, source.replace(OLD, NEW));
  edited = true;
  check(await until(() => applied() >= 1), 'the skin save is applied live');
  check(logs.some(line => /reskinned [1-9]\d* TodoItemControl \(reset: focus, caret, scroll, unbound typed text\)$/.test(line)), 'the console says the rows were skinned again');
  const after = await rows(page);
  check(after.live === after.titles.length && JSON.stringify(after.titles) === JSON.stringify(before.titles),
    `every open row shows the new skin and keeps its view model (${JSON.stringify(after)})`);
  check(await page.inputValue(INPUT) === 'draft kept', 'the draft is kept');
  check(loads === 1 && await page.evaluate(() => window.__keep) === 42, `no reload, state kept (loads=${loads})`);
  const star = () => page.evaluate(() => [...document.querySelectorAll('[data-star-gate]')].find(s => s.offsetParent).textContent);
  const starBefore = await star();
  await page.click('[data-star-gate]');
  await page.waitForTimeout(300);
  check(await star() !== starBefore, `a re-skinned row's star still toggles its view model (${starBefore} -> ${await star()})`);

  await addRow(page, 'row after save');
  const added = await rows(page);
  check(added.live === added.titles.length, `a row added after the save gets the new skin (${JSON.stringify(added)})`);

  const tab = await browser.newPage();
  await tab.goto(url);
  await tab.waitForSelector(INPUT, { timeout: 15000 });
  await addRow(tab, 'new tab row');
  const fresh = await rows(tab);
  check(fresh.live === fresh.titles.length && fresh.live > 0, `a new tab gets the new skin (${JSON.stringify(fresh)})`);
  await tab.close();

  fs.writeFileSync(SKIN, source);
  edited = false;
  check(await until(() => applied() >= 2), 'the revert is applied live');
  const reverted = await rows(page);
  check(reverted.live === 0 && reverted.titles.length === added.titles.length, `the rows show the old skin again (${JSON.stringify(reverted)})`);
  check(loads === 1, `still no reload (loads=${loads})`);

  // The outer skin: the shell is made again around the same view model and rows.
  fs.writeFileSync(SHELL, shell.replace(SHELL_OLD, SHELL_NEW));
  shellEdited = true;
  check(await until(() => applied() >= 3), 'the AppShell save is applied live');
  check(logs.some(line => /reskinned 1 AppShell \(reset: focus, caret, scroll, unbound typed text\)$/.test(line)), 'the console says the shell was skinned again');
  const shelled = await rows(page);
  check(await page.$('[hmr=shell]') !== null && JSON.stringify(shelled.titles) === JSON.stringify(reverted.titles),
    `the new shell shows the same rows (${JSON.stringify(shelled)})`);
  await addRow(page, 'row in new shell');
  check((await rows(page)).titles.includes('row in new shell'), 'adding a row works in the new shell');
  fs.writeFileSync(SHELL, shell);
  shellEdited = false;
  check(await until(() => applied() >= 4), 'the AppShell revert is applied live');
  check(await page.$('[hmr=shell]') === null && loads === 1, `the old shell is back with no reload (loads=${loads})`);

  // The skins' stylesheet: the style element takes the new text.
  const colour = target => target.evaluate(() => getComputedStyle(document.querySelector('[class^=app-title]')).color);
  const colourBefore = await colour(page);
  const crlf = cssRaw.includes('\r\n');
  const cssEdit = crlf ? css.replace(CSS_OLD, CSS_NEW).replace(/\n/g, '\r\n') : css.replace(CSS_OLD, CSS_NEW);
  fs.writeFileSync(CSS, cssEdit);
  cssEdited = true;
  check(await until(() => applied() >= 5), 'the CSS save is applied live');
  check(await colour(page) === 'rgb(7, 8, 9)' && colourBefore !== 'rgb(7, 8, 9)', `the open page takes the new colour (${colourBefore} -> ${await colour(page)})`);
  check(await page.evaluate(() => document.querySelectorAll('style').length) === styleCount, 'no style element added');
  const cssTab = await browser.newPage();
  await cssTab.goto(url);
  await cssTab.waitForSelector(INPUT, { timeout: 15000 });
  check(await colour(cssTab) === 'rgb(7, 8, 9)', 'a new tab reads the new colour from the bundle');
  await cssTab.close();
  fs.writeFileSync(CSS, cssRaw);
  cssEdited = false;
  check(await until(() => applied() >= 6), 'the CSS revert is applied live');
  check(await colour(page) === colourBefore && loads === 1, `the old colour is back with no reload (loads=${loads})`);
  check(errors.length === 0, `no page errors (${errors.join('; ')})`);
} finally {
  if (edited) fs.writeFileSync(SKIN, source);
  if (shellEdited) fs.writeFileSync(SHELL, shell);
  if (cssEdited) fs.writeFileSync(CSS, cssRaw);
  console.log(logs.join('\n'));
  await browser.close();
  server.close();
}
process.exit(failures.length ? 1 : 0);

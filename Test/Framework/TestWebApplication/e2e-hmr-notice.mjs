// Build notices end to end (docs/build/build-service.md, "Live patching in the open page"):
// with TodoApp open, a save that breaks the build logs `build failed: <first error>` and the
// page keeps working with no reload. The fix goes live together with the good edit saved
// alongside the break. A type error gives a new notice. A new .cs file gives NEEDS BUILD.
// Needs the watch running with patch emit:
//   NSCRIPT_PATCH_EMIT=1 dotnet build Test/Framework/TodoApp/TodoApp.csproj -p:NScriptWatch=true
// Exit 0 pass, 1 fail, 2 skipped (the bundle has no live patch client).
import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { chromium } from 'playwright';

const ROOT = path.dirname(fileURLToPath(import.meta.url));
const APP = path.join(ROOT, '../TodoApp');
const VM = path.join(APP, 'ViewModels/TodoItemViewModel.cs');
const CONTROL = path.join(APP, 'Controls/TodoItemControl.cs');
const NEWFILE = path.join(APP, 'HmrNoticeProbe.cs');
const A_OLD = 'this.IsImportant = !this.IsImportant;';
const A_NEW = 'this.IsImportant = !this.IsImportant; this.Title = this.Title + "*";';
const B_OLD = 'if (this.OnSelected != null)';
const B_TYPO = 'int hmrProbe = 1 if (this.OnSelected != null)';
const B_TYPE = 'int hmrProbe = "one"; if (this.OnSelected != null)';
const MIME = { '.htm': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.map': 'application/json' };

const bundle = fs.readFileSync(path.join(ROOT, 'GeneratedScripts/TodoApp.js'), 'utf8');
if (!/__nsHmrBoot\("TodoApp\.js\|[0-9a-f]+\|ws:/.test(bundle)) {
  console.log('SKIPPED: GeneratedScripts/TodoApp.js has no live patch client with a socket. Start the watch with NSCRIPT_PATCH_EMIT=1.');
  process.exit(2);
}

const vmSource = fs.readFileSync(VM, 'utf8'), controlSource = fs.readFileSync(CONTROL, 'utf8');
if (!vmSource.includes(A_OLD) || !controlSource.includes(B_OLD) || fs.existsSync(NEWFILE)) { console.log('FAIL: sources not as expected'); process.exit(1); }
const server = http.createServer((req, res) => {
  const p = decodeURIComponent(req.url.split('?')[0]);
  fs.readFile(path.join(ROOT, p), (e, d) => { if (e) { res.writeHead(404); return res.end(); } res.writeHead(200, { 'Content-Type': MIME[path.extname(p)] || 'application/octet-stream' }); res.end(d); });
});
await new Promise(r => server.listen(0, '127.0.0.1', r));
const url = `http://127.0.0.1:${server.address().port}/TodoApp.htm`;
const t0 = Date.now(), stamp = () => ((Date.now() - t0) / 1000).toFixed(1) + 's';
const browser = await chromium.launch({ headless: true });
const page = await browser.newPage();
const logs = [], errors = [];
let loads = 0;
page.on('console', m => { if (m.text().startsWith('[nscript-hmr]')) logs.push(stamp() + ' ' + m.text()); });
page.on('pageerror', e => errors.push(e.message));
page.on('load', () => loads++);
const until = async (pred, ms = 30000) => { for (const s = Date.now(); !pred(); await page.waitForTimeout(100)) if (Date.now() - s > ms) return false; return true; };
const has = text => logs.some(l => l.includes(text));
const count = text => logs.filter(l => l.includes(text)).length;
const results = [];
const check = (ok, what) => { results.push(ok); console.log(`${ok ? 'PASS' : 'FAIL'}: ${what}`); };
const note = () => page.evaluate(() => window.__nsHmr['TodoApp.js'].note);
const title = () => page.evaluate(() => document.querySelector('[class^=todo-title]').textContent);
try {
  await page.goto(url);
  const input = 'input[placeholder="Add a task"]';
  await page.waitForSelector(input, { timeout: 15000 });
  check(await until(() => has('connected TodoApp.js')), 'connects');
  await page.fill(input, 'draft kept');
  const before = await title();

  // ERR1 + ERR2 save 1: a good edit in the view model and a syntax error in the control.
  fs.writeFileSync(VM, vmSource.replace(A_OLD, A_NEW));
  fs.writeFileSync(CONTROL, controlSource.replace(B_OLD, B_TYPO));
  check(await until(() => has('build failed: ')), 'a syntax error shows build failed');
  console.log('  notice: ' + await note());
  check(/TodoItemControl\.cs\(\d+,\d+\): error CS\d+/.test(await note() || ''), 'the notice names the file, line and error code');
  check(count(' applied ') === 0 && loads === 1, `no patch, no reload (loads=${loads})`);
  check(await page.inputValue(input) === 'draft kept', 'the page keeps working: the draft is there');

  // Type error: a second, different notice.
  fs.writeFileSync(CONTROL, controlSource.replace(B_OLD, B_TYPE));
  check(await until(() => count('build failed: ') >= 2), 'a type error shows a new build failed');
  console.log('  notice: ' + await note());
  check(/CS0029/.test(await note() || ''), 'the type error is CS0029');

  // ERR2 save 2: the fix. Both the good edit and the fix go live in one patch.
  fs.writeFileSync(CONTROL, controlSource);
  check(await until(() => count(' applied ') >= 1), 'the fix is applied live');
  check(await note() === null, 'the notice is cleared');
  check(loads === 1, `still no reload (loads=${loads})`);
  await page.click('[data-star-gate]');
  await page.waitForTimeout(300);
  check((await title()) === before + '*', 'the good edit from the broken save runs: the star adds *');

  // ERR3: a new file needs a real build.
  fs.writeFileSync(NEWFILE, 'namespace TodoApp { internal static class HmrNoticeProbe { } }\n');
  check(await until(() => has('NEEDS BUILD ')), 'a new file shows NEEDS BUILD');
  console.log('  notice: ' + await note());
  check(loads === 1, `no reload (loads=${loads})`);
} finally {
  fs.writeFileSync(VM, vmSource);
  fs.writeFileSync(CONTROL, controlSource);
  if (fs.existsSync(NEWFILE)) fs.unlinkSync(NEWFILE);
  console.log(logs.join('\n'));
  console.log('page errors: ' + errors.join('; '));
  await browser.close();
  server.close();
}
process.exit(results.every(Boolean) ? 0 : 1);

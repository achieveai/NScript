// Live patching end to end (docs/build/build-service.md, "Live patching in the open page"):
// a save makes TodoApp's Main throw. Main runs at load, so the open page reloads onto a bundle
// whose closure throws. Its live patch client still boots (it runs in the closure's finally)
// and says why the load failed. Saving the fix reloads the page by itself, and the app works.
// Needs the watch running with patch emit:
//   NSCRIPT_PATCH_EMIT=1 dotnet build Test/Framework/TodoApp/TodoApp.csproj -p:NScriptWatch=true
// Exit 0 pass, 1 fail, 2 skipped (the bundle has no live patch client).
import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { chromium } from 'playwright';

const ROOT = path.dirname(fileURLToPath(import.meta.url));
const CS = path.join(ROOT, '../TodoApp/Program.cs');
const OLD = 'GraphFlushCoordinator.BatchingEnabled = true;';
const NEW = 'GraphFlushCoordinator.BatchingEnabled = true; if (TaskScheduler.Instance != null) throw new System.Exception("hmr load boom");';
const MIME = { '.htm': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.map': 'application/json' };

const bundle = fs.readFileSync(path.join(ROOT, 'GeneratedScripts/TodoApp.js'), 'utf8');
if (!/__nsHmrBoot\("TodoApp\.js\|[0-9a-f]+\|ws:/.test(bundle)) {
  console.log('SKIPPED: GeneratedScripts/TodoApp.js has no live patch client with a socket. Start the watch with NSCRIPT_PATCH_EMIT=1.');
  process.exit(2);
}

const source = fs.readFileSync(CS, 'utf8');
if (source.split(OLD).length !== 2 || source.includes(NEW)) {
  console.log('FAIL: Program.cs does not hold the expected Main body.');
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
let loads = 0, edited = false;
page.on('console', message => { if (message.text().startsWith('[nscript-hmr]')) logs.push(message.text()); });
page.on('pageerror', error => errors.push(error.message));
page.on('load', () => loads++);
const until = async (predicate, ms = 60000) => {
  for (const start = Date.now(); !predicate(); await page.waitForTimeout(100)) {
    if (Date.now() - start > ms) return false;
  }
  return true;
};
const failures = [];
const check = (ok, what) => { console.log(`${ok ? 'PASS' : 'FAIL'}: ${what}`); if (!ok) failures.push(what); };
const input = 'input[placeholder="Add a task"]';

try {
  await page.goto(url);
  await page.waitForSelector(input, { timeout: 15000 });
  check(await until(() => logs.some(line => line.includes('connected TodoApp.js'))), 'the page connects to the build service');

  fs.writeFileSync(CS, source.replace(OLD, NEW));
  edited = true;
  check(await until(() => loads >= 2), `the Main edit reloads the page (${JSON.stringify(logs.filter(line => line.includes('reloading')))})`);
  check(await until(() => errors.some(error => error.includes('hmr load boom'))), `the reloaded bundle throws at load (${errors.join('; ')})`);
  check(await until(() => logs.some(line => line.includes('load failed: ') && line.includes('hmr load boom'))), 'its client still boots and says why');
  check(await until(() => logs.filter(line => line.includes('connected TodoApp.js')).length >= 2), 'and connects');
  check(await page.$(input) === null, 'the app did not start');

  fs.writeFileSync(CS, source);
  edited = false;
  check(await until(() => loads >= 3), `the fix reloads the page by itself (loads=${loads})`);
  await page.waitForSelector(input, { timeout: 15000 });
  check(await page.evaluate(() => !window.__nsHmrFailed || !window.__nsHmrFailed['TodoApp.js']), 'the fixed bundle loads cleanly');
  check(await until(() => logs.filter(line => line.includes('connected TodoApp.js')).length >= 3), 'and connects again');
  await page.waitForTimeout(3000);
  check(loads === 3, `no further reloads (loads=${loads})`);
  check(errors.filter(error => !error.includes('hmr load boom')).length === 0, `no other page errors (${errors.join('; ')})`);
} finally {
  if (edited) fs.writeFileSync(CS, source);
  console.log(logs.join('\n'));
  await browser.close();
  server.close();
}

process.exit(failures.length === 0 ? 0 : 1);

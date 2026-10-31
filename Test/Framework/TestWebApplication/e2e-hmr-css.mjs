// XWML CSS live patching end to end (docs/build/build-service.md, "Live patching in the open
// page"): with the UI test page open, a colour edit in the linked TestTemplateVMB_CssBinding.css
// restyles the page with no reload. A document made after the save and a new tab get the new
// CSS too, and an emptied part stays empty there. The revert goes live as well. Needs the watch running with patch emit:
//   NSCRIPT_PATCH_EMIT=1 dotnet build Test/Framework/Sunlight.Framework.UI.Test/Sunlight.Framework.UI.Test.csproj -p:NScriptWatch=true
// Exit 0 pass, 1 fail, 2 skipped (the bundle has no live patch client).
import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { chromium } from 'playwright';

const ROOT = path.dirname(fileURLToPath(import.meta.url));
const CSS = path.join(ROOT, '../Sunlight.Framework.UI.Test/Templates/TestTemplateVMB_CssBinding.css');
const BUNDLE = 'Sunlight.Framework.UI.Test.js';
const OLD = 'background-color: black;';
const NEW = 'background-color: rgb(1, 2, 3);';
const MIME = { '.htm': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.map': 'application/json' };

const bundle = fs.readFileSync(path.join(ROOT, 'GeneratedScripts', BUNDLE), 'utf8');
if (!/__nsHmrBoot\("Sunlight\.Framework\.UI\.Test\.js\|[0-9a-f]+\|ws:/.test(bundle)) {
  console.log('SKIPPED: GeneratedScripts/' + BUNDLE + ' has no live patch client with a socket. Start the watch with NSCRIPT_PATCH_EMIT=1.');
  process.exit(2);
}

// The document initializer that holds the page's CSS, by the name the dev bundle gives it.
const getter = /function (DocStorageGetter[\w$]*)\([^)]*\) \{(?:(?!\nfunction )[\s\S])*?\.__nsCss && /.exec(bundle);
if (!getter) { console.log('FAIL: no live CSS hook in the bundle'); process.exit(1); }
const source = fs.readFileSync(CSS, 'utf8');
if (!source.includes(OLD)) { console.log('FAIL: the .css does not hold ' + OLD); process.exit(1); }

const server = http.createServer((req, res) => {
  const p = decodeURIComponent(req.url.split('?')[0]);
  fs.readFile(path.join(ROOT, p), (e, d) => { if (e) { res.writeHead(404); return res.end(); } res.writeHead(200, { 'Content-Type': MIME[path.extname(p)] || 'application/octet-stream' }); res.end(d); });
});
await new Promise(r => server.listen(0, '127.0.0.1', r));
const url = `http://127.0.0.1:${server.address().port}/Sunlight.Framework.UI.TestPage.htm`;

const browser = await chromium.launch({ headless: true });
const page = await browser.newPage();
const logs = [], errors = [];
let loads = 0, edited = false;
page.on('console', m => { if (m.text().startsWith('[nscript-hmr]')) logs.push(m.text()); });
page.on('pageerror', e => errors.push(e.message));
page.on('load', () => loads++);
// Each frame the socket brings: a CSS save brings its own sheet's part alone (#123).
const frames = [];
page.on('websocket', ws => ws.on('framereceived', f => frames.push(String(f.payload))));
const until = async (pred, ms = 30000) => { for (const s = Date.now(); !pred(); await page.waitForTimeout(100)) if (Date.now() - s > ms) return false; return true; };
const applied = () => logs.filter(l => l.includes(' applied ')).length;
const results = [];
const check = (ok, what) => { results.push(ok); console.log(`${ok ? 'PASS' : 'FAIL'}: ${what}`); };
// The colour of a .black element, and how many style elements hold the sheet.
const probe = target => target.evaluate(() => {
  const div = document.createElement('div');
  div.className = 'black';
  document.body.appendChild(div);
  const colour = getComputedStyle(div).backgroundColor;
  div.remove();
  const all = [...document.querySelectorAll('style')];
  // A piece of each other part's CSS, free of characters JSON would escape.
  const others = all.filter(s => !s.textContent.includes('.black')).map(s => s.textContent.replace(/["\\\s]/g, ' ').trim().split(' ')[0]).filter(t => t.length > 4);
  return { colour, styles: all.filter(s => s.textContent.includes('.black')).length, parts: all.length, others };
});
try {
  await page.goto(url);
  await page.waitForSelector('#qunit-banner.qunit-pass, #qunit-banner.qunit-fail', { timeout: 60000 });
  check(await until(() => logs.some(l => l.includes('connected ' + BUNDLE)), 15000), 'connects');
  await page.evaluate(() => { window.__keep = 42; });
  const before = await probe(page);
  check(before.colour === 'rgb(0, 0, 0)' && before.styles === 1, `the page has the old CSS once (${JSON.stringify(before)})`);

  check(before.parts > 1, `the dev bundle writes a style element per CSS part (${before.parts})`);
  fs.writeFileSync(CSS, source.replace(OLD, NEW));
  edited = true;
  const framesBefore = frames.length;
  check(await until(() => applied() >= 1), 'the CSS save is applied live');
  const patch = frames.slice(framesBefore).join('');
  check(patch.length > 0 && before.others.length > 0 && before.others.every(t => !patch.includes(t)),
    `the patch leaves out the other parts' CSS (${patch.length} chars, ${JSON.stringify(before.others)})`);
  const after = await probe(page);
  check(after.colour === 'rgb(1, 2, 3)' && after.styles === 1, `the open page restyles, one style element (${JSON.stringify(after)})`);
  check(loads === 1 && await page.evaluate(() => window.__keep) === 42, `no reload, state kept (loads=${loads})`);
  // The CSS of a document made now, through the bundle's document initializer.
  const laterCss = () => page.evaluate(([file, name]) => {
    const frame = document.createElement('iframe');
    document.body.appendChild(frame);
    window.__nsPatch[file](name + '(window.__frameDoc = document.querySelector("iframe:last-of-type").contentDocument);');
    const text = [...window.__frameDoc.querySelectorAll('style')].map(s => s.textContent).join('');
    frame.remove();
    return text;
  }, [BUNDLE, getter[1]]);
  const later = await laterCss();
  check(later.includes('rgb(1, 2, 3)') || later.includes('rgb(1,2,3)'), 'a document made after the save gets the new CSS');
  const tab = await browser.newPage();
  await tab.goto(url);
  await tab.waitForSelector('#qunit-banner.qunit-pass, #qunit-banner.qunit-fail', { timeout: 60000 });
  check((await probe(tab)).colour === 'rgb(1, 2, 3)', 'a new tab gets the new CSS from the bundle');
  await tab.close();

  // An emptied part stays empty in a later document: it must not fall back to the bundle's CSS.
  // (The sheet itself cannot be emptied: the template uses .black, so that build fails.)
  const emptied = await page.evaluate(([file, name]) => {
    const part = [...document.querySelectorAll('style')].find(s => s.textContent.includes('.black')).__nsPart;
    window.__nsPatch[file]('window.__nsGetter = ' + name + ';');
    const live = window.__nsGetter.__nsCss, had = live[part];
    live[part] = '';
    const frame = document.createElement('iframe');
    document.body.appendChild(frame);
    window.__nsPatch[file](name + '(window.__frameDoc = document.querySelector("iframe:last-of-type").contentDocument);');
    const text = [...window.__frameDoc.querySelectorAll('style')].map(s => s.textContent).join('');
    frame.remove();
    live[part] = had;
    return text;
  }, [BUNDLE, getter[1]]);
  check(!emptied.includes('.black'), 'a document made after its part was emptied has no CSS in that part');

  fs.writeFileSync(CSS, source);
  edited = false;
  check(await until(() => applied() >= 2), 'the revert is applied live');
  check((await probe(page)).colour === 'rgb(0, 0, 0)', 'the open page has the old colour again');
  check(loads === 1, `still no reload (loads=${loads})`);
  check(errors.length === 0, `no page errors (${errors.join('; ')})`);
} finally {
  if (edited) fs.writeFileSync(CSS, source);
  console.log(logs.join('\n'));
  await browser.close();
  server.close();
}
process.exit(results.every(Boolean) ? 0 : 1);

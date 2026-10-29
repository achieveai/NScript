// XWML skin live patching end to end (docs/build/build-service.md, "Live patching in the open
// page"): with the UI test page open, an edit to TestTemplateVMB1.html skins the open control
// (#hmr-skin-probe) again with no reload. The control and its view model stay; the DOM inside
// the skin is made again. A new tab gets the new skin too, and the revert goes live as well.
// Needs the watch running with patch emit:
//   NSCRIPT_PATCH_EMIT=1 dotnet build Test/Framework/Sunlight.Framework.UI.Test/Sunlight.Framework.UI.Test.csproj -p:NScriptWatch=true
// Exit 0 pass, 1 fail, 2 skipped (the bundle has no live patch client).
import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { chromium } from 'playwright';

const ROOT = path.dirname(fileURLToPath(import.meta.url));
const SKIN = path.join(ROOT, '../Sunlight.Framework.UI.Test/Templates/TestTemplateVMB1.html');
const BUNDLE = 'Sunlight.Framework.UI.Test.js';
const OLD = '<div test="test me">{PropStr1}</div>';
const NEW = OLD + '<span hmr="live">live</span>';
// A skin and CSS edit together: a class added in the skin's own style, and used.
const HEAD = '<title></title>';
const CLASS_HEAD = HEAD + '<style> .hmrNew { color: rgb(4, 5, 6); } </style>';
const CLASS_NEW = OLD + '<span hmr="class" class="hmrNew">class</span>';
const MIME = { '.htm': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.map': 'application/json' };

const bundle = fs.readFileSync(path.join(ROOT, 'GeneratedScripts', BUNDLE), 'utf8');
if (!/__nsHmrBoot\("Sunlight\.Framework\.UI\.Test\.js\|[0-9a-f]+\|ws:/.test(bundle)) {
  console.log('SKIPPED: GeneratedScripts/' + BUNDLE + ' has no live patch client with a socket. Start the watch with NSCRIPT_PATCH_EMIT=1.');
  process.exit(2);
}

// The skin's storage variable and the Skin factory field, by the names the dev bundle gives them.
const skinVar = /\b(\S*TestTemplateVMB1_var\S*) = /.exec(bundle);
const factoryField = /__nsReskin = \(function[^\n]*?\}\)\("([^"]+)"/.exec(bundle);
const rootField = /\$\$ptyp\.(rootElement\S*) = null/.exec(bundle);
if (!skinVar || !factoryField || !rootField) { console.log('FAIL: no live skin hook in the bundle'); process.exit(1); }
const source = fs.readFileSync(SKIN, 'utf8');
if (!source.includes(OLD) || !source.includes(HEAD)) { console.log('FAIL: the skin does not hold ' + OLD); process.exit(1); }

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
const until = async (pred, ms = 30000) => { for (const s = Date.now(); !pred(); await page.waitForTimeout(100)) if (Date.now() - s > ms) return false; return true; };
const applied = () => logs.filter(l => l.includes(' applied ')).length;
const results = [];
const check = (ok, what) => { results.push(ok); console.log(`${ok ? 'PASS' : 'FAIL'}: ${what}`); };
// What the open control shows, and whether it is the element and DOM the page had before.
const probe = target => target.evaluate(() => {
  const el = document.querySelector('#hmr-skin-probe');
  const div = el && el.querySelector('[test]');
  return {
    text: div && div.textContent,
    live: !!(el && el.querySelector('[hmr]')),
    sameElement: !!window.__probe && window.__probe === el,
    sameDom: !!window.__probeDiv && window.__probeDiv === div,
    colour: el && el.querySelector('[hmr=class]') ? getComputedStyle(el.querySelector('[hmr=class]')).color : null,
  };
});
const done = target => target.waitForSelector('#qunit-banner.qunit-pass, #qunit-banner.qunit-fail', { timeout: 60000 });
try {
  await page.goto(url);
  await done(page);
  check(await page.$('#qunit-banner.qunit-pass') !== null, 'the UI suite passes');
  check(await until(() => logs.some(l => l.includes('connected ' + BUNDLE)), 15000), 'connects');
  await page.evaluate(() => {
    window.__keep = 42;
    window.__probe = document.querySelector('#hmr-skin-probe');
    window.__probeDiv = window.__probe && window.__probe.querySelector('[test]');
  });
  await page.evaluate(([file, name]) => window.__nsPatch[file]('window.__oldSkin = ' + name + ';'), [BUNDLE, skinVar[1]]);
  const before = await probe(page);
  check(before.text === 'probe' && !before.live, `the open control shows the old skin (${JSON.stringify(before)})`);

  fs.writeFileSync(SKIN, source.replace(OLD, NEW));
  edited = true;
  check(await until(() => applied() >= 1), 'the skin save is applied live');
  check(logs.some(l => /reskinned [1-9]\d* TestTemplateVMB1 \(reset: focus, caret, scroll, unbound typed text\)$/.test(l)), 'the console names the skin and says what a re-skin resets');
  const after = await probe(page);
  check(after.live && after.text === 'probe', `the open control shows the new skin and keeps its view model (${JSON.stringify(after)})`);
  check(after.sameElement && !after.sameDom, 'same control element, new DOM inside the skin');
  check(loads === 1 && await page.evaluate(() => window.__keep) === 42, `no reload, state kept (loads=${loads})`);
  // A holder of the skin (a list's item skin) makes the new look: the skin is the same object, with the new factory.
  const held = await page.evaluate(([file, name, field, root]) => {
    window.__nsPatch[file]('window.__nowSkin = ' + name + ';');
    const made = window.__oldSkin[field](window.__oldSkin, document);
    return { same: window.__nowSkin === window.__oldSkin, newFactory: !!made[root].querySelector('[hmr]') };
  }, [BUNDLE, skinVar[1], factoryField[1], rootField[1]]);
  check(held.same && held.newFactory, `the skin keeps its identity and takes the new factory (${JSON.stringify(held)})`);

  const tab = await browser.newPage();
  await tab.goto(url);
  await done(tab);
  const fresh = await probe(tab);
  check(fresh.live && await tab.$('#qunit-banner.qunit-pass') !== null, `a new tab gets the new skin and its suite passes (${JSON.stringify(fresh)})`);
  await tab.close();

  fs.writeFileSync(SKIN, source);
  edited = false;
  check(await until(() => applied() >= 2), 'the revert is applied live');
  const reverted = await probe(page);
  check(!reverted.live && reverted.text === 'probe', `the open control shows the old skin again (${JSON.stringify(reverted)})`);
  check(loads === 1, `still no reload (loads=${loads})`);

  fs.writeFileSync(SKIN, source.replace(HEAD, CLASS_HEAD).replace(OLD, CLASS_NEW));
  edited = true;
  check(await until(() => applied() >= 3), 'a skin edit that adds and uses a class is applied live');
  const styled = await probe(page);
  check(styled.colour === 'rgb(4, 5, 6)' && styled.text === 'probe', `the open control uses the new class (${JSON.stringify(styled)})`);
  fs.writeFileSync(SKIN, source);
  edited = false;
  check(await until(() => applied() >= 4), 'its revert is applied live');
  check(loads === 1, `no reload for either (loads=${loads})`);
  check(errors.length === 0, `no page errors (${errors.join('; ')})`);
} finally {
  if (edited) fs.writeFileSync(SKIN, source);
  console.log(logs.join('\n'));
  await browser.close();
  server.close();
}
process.exit(results.every(Boolean) ? 0 : 1);

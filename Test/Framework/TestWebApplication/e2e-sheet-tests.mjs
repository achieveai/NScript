// End-to-end tests for SpreadsheetApp.htm: drives the real page with real
// clicks and keystrokes (formula bar, grid keyboard, toolbar) and checks the
// rendered cells. Mirrors the manual scenarios used to accept the app.
//
//   node e2e-sheet-tests.mjs            # all
//   E2E_FILTER=copy node e2e-sheet-tests.mjs
//
import http from 'http';
import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';
import { chromium } from 'playwright';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const MIME = { '.htm': 'text/html', '.html': 'text/html', '.js': 'application/javascript', '.css': 'text/css' };

function startServer() {
  return new Promise((resolve, reject) => {
    const server = http.createServer((req, res) => {
      const filePath = path.join(__dirname, decodeURIComponent(req.url.split('?')[0]));
      fs.readFile(filePath, (err, data) => {
        if (err) { res.writeHead(404); res.end(); return; }
        res.writeHead(200, { 'Content-Type': MIME[path.extname(filePath)] || 'application/octet-stream' });
        res.end(data);
      });
    });
    server.listen(0, () => resolve({ server, url: `http://localhost:${server.address().port}` }));
    server.on('error', reject);
  });
}

// Generated CSS class names carry a _xx suffix (debug) or are minified; map
// the readable names used below to what is actually in the injected <style>.
async function buildClassMap(page) {
  return page.evaluate(() => {
    const map = {};
    for (const style of document.querySelectorAll('style')) {
      const re = /\.([a-z][\w-]*?_[a-zA-Z0-9]+)/g;
      let m;
      while ((m = re.exec(style.textContent)) !== null) {
        const full = m[1];
        const original = full.substring(0, full.lastIndexOf('_'));
        if (!map[original]) map[original] = full;
      }
    }
    return map;
  });
}

const { server, url } = await startServer();
const browser = await chromium.launch({ headless: true });
const FILTER = process.env.E2E_FILTER || '';
const results = { passed: 0, failed: 0 };
console.log(`=== Spreadsheet E2E Tests (${url}) ===\n`);

function assert(cond, msg) { if (!cond) throw new Error('Assertion failed: ' + msg); }

async function runTest(name, fn) {
  if (FILTER && !name.toLowerCase().includes(FILTER.toLowerCase())) return;
  const context = await browser.newContext();
  const page = await context.newPage();
  const pageErrors = [];
  page.on('pageerror', e => pageErrors.push(e.message));
  try {
    await page.goto(url + '/SpreadsheetApp.htm', { waitUntil: 'domcontentloaded' });
    await page.waitForFunction(() => window.__sheet && document.getElementById('sheet'), { timeout: 20000 });
    const cls = await buildClassMap(page);
    const s = sel => sel.replace(/\.([a-z][\w-]+)/g, (m, n) => '.' + (cls[n] || n));
    const rowSel = s('.sheet-row'), cellSel = s('.cell');
    await page.waitForSelector(rowSel);

    // Helpers bound to this page.
    const cell = name => {
      const col = name.charCodeAt(0) - 65, row = parseInt(name.slice(1), 10) - 1;
      return page.locator(rowSel).nth(row).locator(cellSel).nth(col);
    };
    const text = async name => (await cell(name).textContent()).trim();
    const commit = async (name, value) => {
      await cell(name).click();
      await page.fill('#formula-input', value);
      await page.press('#formula-input', 'Enter');
    };
    const grid = async key => { await page.focus('#sheet'); await page.keyboard.press(key); };
    // Exact-text match: a substring match would pick "1,234.50" for "1,234".
    const button = label => page.locator(s('.btn')).filter({ hasText: new RegExp('^' + label.replace(/[.*+?^${}()|[\]\\]/g, '\\$&') + '$') }).first();
    const selectedName = async () => (await page.locator(s('.cell-name')).textContent()).trim();
    const barValue = () => page.inputValue('#formula-input');

    await fn({ page, s, cell, text, commit, grid, button, selectedName, barValue });
    assert(pageErrors.length === 0, 'no page errors, got: ' + pageErrors.join(' | '));
    results.passed++;
    console.log('  PASS: ' + name);
  } catch (err) {
    results.failed++;
    console.log('  FAIL: ' + name);
    console.log('    ' + err.message);
  } finally {
    await context.close();
  }
}

await runTest('seeded sheet renders 50 rows, totals row, and column formats', async t => {
  assert(await t.page.locator(t.s('.sheet-row')).count() === 51, '50 rows + totals');
  assert(await t.text('A1') === '1', 'A1 literal');
  assert(await t.text('C1') === '$1.00', 'C column is currency');
  assert(await t.text('H1') === '2026-01-02', 'H column is a date');
  assert((await t.text('I1')).endsWith('%'), 'I column is percent');
  assert(await t.text('A51') === '1275', 'A total = 1..50 (General has no grouping)');
  assert(await t.selectedName() === 'A1', 'A1 selected on load');
});

await runTest('formula bar edit propagates through dependents and totals', async t => {
  await t.commit('B3', '=A3*10');
  assert(await t.text('B3') === '30', 'B3 = A3*10');
  assert(await t.text('C3') === '$90.00', 'C3 = A3*B3');
  assert(await t.text('B51') === '224', 'B total follows (197 - 3 + 30)');
});

await runTest('Enter commits and moves the selection down', async t => {
  await t.commit('B3', '=A3*10');
  assert(await t.selectedName() === 'B4', 'selection moved to B4');
  assert(await t.barValue() === '4', 'bar shows B4 raw text');
  await t.cell('B3').click();
  assert(await t.barValue() === '=A3*10', 'bar shows the raw formula when re-selected');
});

await runTest('precedent change re-evaluates the formula cell', async t => {
  await t.commit('B3', '=A3*10');
  await t.commit('A3', '0');
  assert(await t.text('B3') === '0', 'B3 re-evaluated');
  assert(await t.text('C3') === '$0.00', 'C3 re-evaluated');
  assert(await t.text('B51') === '194', 'total re-evaluated');
});

await runTest('cycle is refused and a later valid formula replaces it', async t => {
  await t.commit('A1', '=E1');
  assert(await t.text('A1') === '#CYCLE!', 'A1 -> E1 -> D1 -> A1');
  await t.commit('A1', '7');
  assert(await t.text('A1') === '7', 'literal replaces the error');
  assert(await t.text('C1') === '$7.00', 'dependents recover');
});

await runTest('errors, text and booleans render with their classes', async t => {
  await t.commit('K1', '=1/0');
  assert(await t.text('K1') === '#DIV/0!', 'division error');
  assert((await t.cell('K1').getAttribute('class')).includes(t.s('.error').slice(1)), 'error class');
  await t.commit('K2', '=SUM(A1');
  assert(await t.text('K2') === '#NAME?', 'parse error');
  await t.commit('K3', 'hello');
  assert(await t.text('K3') === 'hello', 'text literal');
  assert((await t.cell('K3').getAttribute('class')).includes(t.s('.text-cell').slice(1)), 'text class');
  await t.commit('K4', '=K3&" world"');
  assert(await t.text('K4') === 'hello world', 'concat');
  await t.commit('K5', '=AND(TRUE,1>0)');
  assert(await t.text('K5') === 'TRUE', 'boolean');
});

await runTest('functions and ranges: SUM, AVERAGE, MIN, MAX, COUNT, IF', async t => {
  await t.commit('L1', '=SUM(A1:A5)');
  assert(await t.text('L1') === '15', 'SUM over a range');
  await t.commit('L2', '=AVERAGE(A1:A4)');
  assert(await t.text('L2') === '2.5', 'AVERAGE');
  await t.commit('L3', '=MAX(A1:A50)-MIN(A1:A50)');
  assert(await t.text('L3') === '49', 'MAX-MIN');
  await t.commit('L4', '=COUNT(A1:C2)');
  assert(await t.text('L4') === '6', 'COUNT');
  await t.commit('L5', '=IF(L1>10,"big","small")');
  assert(await t.text('L5') === 'big', 'IF');
});

await runTest('copy/paste shifts relative refs and keeps $ refs', async t => {
  await t.cell('C1').click();
  await t.button('Copy').click();
  await t.cell('K5').click();
  await t.button('Paste').click();
  assert(await t.barValue() === '=I5*J5', 'relative refs shifted by the paste offset');
  await t.cell('J1').click();
  await t.button('Fill down').click();
  assert(await t.selectedName() === 'J2', 'fill down selects the cell below');
  assert(await t.barValue() === '=ROUND(C2*$B$1%,2)', '$B$1 stays fixed, C1 -> C2');
});

await runTest('keyboard: arrows move, Delete clears, Ctrl+C/V copies', async t => {
  await t.cell('A1').click();
  await t.grid('ArrowDown');
  await t.grid('ArrowRight');
  assert(await t.selectedName() === 'B2', 'arrow navigation');
  await t.grid('Delete');
  assert(await t.text('B2') === '', 'Delete clears the cell');
  assert(await t.text('C2') === '$0.00', 'dependent sees empty as zero');
  await t.cell('C1').click();
  await t.grid('Control+c');
  await t.cell('M3').click();
  await t.grid('Control+v');
  assert(await t.barValue() === '=K3*L3', 'Ctrl+V pasted a shifted formula');
});

await runTest('format toolbar: column percent, row currency, cell override, date literal', async t => {
  await t.cell('A1').click();
  await t.button('Column').click();
  await t.button('12.5%').click();
  assert(await t.text('A1') === '100.0%', 'column percent');
  assert(await t.text('A2') === '200.0%', 'whole column');
  await t.button('Row').click();
  await t.button('$').click();
  assert(await t.text('A1') === '$1.00', 'row format beats column');
  await t.button('Cell').click();
  await t.button('1,234').click();
  assert(await t.text('A1') === '1', 'cell format beats row');
  await t.commit('N1', '2026-10-06');
  assert(await t.text('N1') === '2026-10-06', 'typed ISO date still reads as a date');
});

await runTest('benchmark op runs without errors and batching toggles', async t => {
  await t.page.evaluate(() => window.__sheet.setBatching(false));
  await t.page.evaluate(() => window.__sheet.op('shift'));
  assert(await t.text('A1') === '2', 'All A + 1 applied (sync)');
  await t.page.evaluate(() => window.__sheet.setBatching(true));
  await t.page.evaluate(() => window.__sheet.op('shift'));
  assert(await t.text('A1') === '3', 'All A + 1 applied (batched)');
  assert(await t.text('A51') === '1375', 'totals follow both runs');
});

await browser.close();
server.close();
console.log('\n=== Spreadsheet E2E Results: ' + results.passed + ' passed, ' + results.failed + ' failed ===\n');
process.exit(results.failed > 0 ? 1 : 0);

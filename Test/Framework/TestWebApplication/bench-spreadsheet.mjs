// Headless before/after benchmark for the batched binding flush.
//
// Opens SpreadsheetApp.htm, runs the app's own benchmark (every op at
// 50/100/200 rows, batching off and on, median of 3) and prints the table.
//
//   node bench-spreadsheet.mjs
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

const { server, url } = await startServer();
const browser = await chromium.launch({ headless: true });
const page = await browser.newPage();
page.on('pageerror', e => console.log('[pageerror]', e.message));
await page.goto(url + '/SpreadsheetApp.htm', { waitUntil: 'domcontentloaded' });
await page.waitForFunction(() => window.__sheet && document.getElementById('sheet'), { timeout: 20000 });

await page.evaluate(() => window.__sheet.runAll());
await page.waitForFunction(() => !window.__sheet.isRunning(), { timeout: 600000, polling: 200 });
const results = JSON.parse(await page.evaluate(() => window.__sheet.resultsJson()));

await browser.close();
server.close();

// Pair sync/batched rows per op x rows.
const key = r => r.op + '|' + r.rows;
const sync = new Map(results.filter(r => r.mode === 'sync').map(r => [key(r), r]));
const batched = new Map(results.filter(r => r.mode === 'batched').map(r => [key(r), r]));

const rows = [];
for (const [k, s] of sync) {
  const b = batched.get(k);
  if (!b) continue;
  rows.push({
    op: s.op, rows: s.rows,
    syncMs: s.ms, batchedMs: b.ms, speedup: s.ms > 0 ? (s.ms / Math.max(b.ms, 0.01)) : 0,
    syncWrites: s.writes, batchedWrites: b.writes,
  });
}

const pad = (v, n, right) => { const t = String(v); return right ? t.padStart(n) : t.padEnd(n); };
console.log('\n=== Spreadsheet binding flush: sync vs batched (median of 3) ===\n');
console.log(pad('Operation', 16) + pad('Rows', 6, true) + pad('sync ms', 10, true) + pad('batched ms', 12, true)
  + pad('speedup', 9, true) + pad('sync writes', 13, true) + pad('batched writes', 16, true));
for (const r of rows) {
  console.log(pad(r.op, 16) + pad(r.rows, 6, true) + pad(r.syncMs.toFixed(1), 10, true) + pad(r.batchedMs.toFixed(1), 12, true)
    + pad(r.speedup.toFixed(1) + 'x', 9, true) + pad(r.syncWrites, 13, true) + pad(r.batchedWrites, 16, true));
}
console.log('');
console.log(JSON.stringify(rows));

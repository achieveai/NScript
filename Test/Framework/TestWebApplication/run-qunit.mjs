import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { chromium } from 'playwright';
import { fileURLToPath } from 'node:url';
import { createSourceMapRouteHandler } from './source-map-route.mjs';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const QUNIT_SUITES = [
  {
    name: 'Sunlight.Framework.Test',
    scripts: ['/GeneratedScripts/Sunlight.Framework.Test.js'],
  },
  {
    name: 'Sunlight.Framework.UI.Test',
    scripts: ['/GeneratedScripts/Sunlight.Framework.UI.Test.js'],
  },
  {
    name: 'TodoApp.Test',
    scripts: ['/GeneratedScripts/TodoApp.Test.js'],
  },
];

const MIME_TYPES = {
  '.html': 'text/html', '.htm': 'text/html',
  '.js': 'application/javascript', '.css': 'text/css',
  '.map': 'application/json', '.json': 'application/json',
  '.png': 'image/png', '.jpg': 'image/jpeg',
};

// Directory that NScript-generated .map files sit in — required for the
// /sourcemap/{mapName}/{shortSource} devtools route below.
const GENERATED_SCRIPTS_DIR = path.join(__dirname, 'GeneratedScripts');

// Serve original source files referenced by an NScript .map. Dev ergonomic
// counterpart of the ASP.NET Core SourceMapFileHandler shipped in
// Sources/Compiler/SourceMap.Server — lets browser DevTools load C#/XWML/Razor
// sources alongside the generated JS when the map's sourceRoot points at
// /sourcemap/{mapName}/. Returns 404 on any mismatch so nothing outside the
// map's pre-recorded paths can be served.
function serveSourceMapFile(req, res) {
  const match = req.url.match(/^\/sourcemap\/([^/]+)\/(.+)$/);
  if (!match) {
    res.writeHead(404);
    res.end('Not Found');
    return true;
  }

  const mapName = decodeURIComponent(match[1]);
  const shortName = decodeURIComponent(match[2]);

  if (mapName.includes('..') || mapName.includes('/') || mapName.includes('\\')) {
    res.writeHead(400);
    res.end('Bad Request');
    return true;
  }

  const mapPath = path.join(GENERATED_SCRIPTS_DIR, mapName + '.map');
  let parsed;
  try {
    parsed = JSON.parse(fs.readFileSync(mapPath, 'utf8'));
  } catch {
    res.writeHead(404);
    res.end('Map not found');
    return true;
  }

  const sources = Array.isArray(parsed.sources) ? parsed.sources : null;
  const sourcesLong = Array.isArray(parsed.sourcesLong) ? parsed.sourcesLong : null;
  if (!sources) {
    res.writeHead(404);
    res.end('Map has no sources');
    return true;
  }

  const idx = sources.indexOf(shortName);
  if (idx < 0) {
    res.writeHead(404);
    res.end('Source not listed in map');
    return true;
  }

  const longPath = (sourcesLong && idx < sourcesLong.length) ? sourcesLong[idx] : shortName;
  fs.readFile(longPath, (err, data) => {
    if (err) {
      res.writeHead(404);
      res.end('Source file missing on disk');
      return;
    }
    res.writeHead(200, { 'Content-Type': 'text/plain; charset=utf-8' });
    res.end(data);
  });
  return true;
}

function renderQUnitPage(suite) {
  const scriptTags = suite.scripts
    .map(script => `  <script src="${script}" type="text/javascript"></script>`)
    .join('\n');

  return `<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <title>${suite.name}</title>
  <link href="/Styles/QUnit.2.2.0.css" rel="stylesheet" type="text/css" />
  <script src="/Scripts/QUnit.2.2.0.js" type="text/javascript"></script>
${scriptTags}
</head>
<body>
  <h1 id="qunit-header">${suite.name}</h1>
  <h2 id="qunit-banner"></h2>
  <h2 id="qunit-userAgent"></h2>
  <ol id="qunit-tests"></ol>
  <br />
  <textarea id="qunit-log" rows="10" cols="100"></textarea>
</body>
</html>`;
}

async function runSuite(page, port, suite) {
  await page.goto(`http://localhost:${port}/__qunit?name=${encodeURIComponent(suite.name)}`, { waitUntil: 'domcontentloaded' });

  await page.waitForFunction(() => {
    const b = document.getElementById('qunit-banner');
    return b && (b.className.includes('qunit-pass') || b.className.includes('qunit-fail'));
  }, { timeout: 120000 });

  return await page.evaluate(() => {
    const banner = document.getElementById('qunit-banner');
    const passed = banner.className.includes('qunit-pass');
    const testResult = document.getElementById('qunit-testresult');
    const summary = testResult ? testResult.textContent.trim() : 'No summary';

    const failed = [];
    const testItems = document.querySelectorAll('#qunit-tests > li.fail');
    for (const li of testItems) {
      const nameEl = li.querySelector('.test-name');
      const moduleEl = li.querySelector('.module-name');
      const assertList = li.querySelectorAll('.qunit-assert-list > li.fail');
      const assertions = [];
      for (const a of assertList) {
        assertions.push(a.textContent.trim().substring(0, 200));
      }
      failed.push({
        module: moduleEl ? moduleEl.textContent : '',
        name: nameEl ? nameEl.textContent : '',
        assertions
      });
    }

    const allTests = document.querySelectorAll('#qunit-tests > li');
    let passCount = 0, failCount = 0;
    for (const li of allTests) {
      if (li.className.includes('pass')) passCount++;
      if (li.className.includes('fail')) failCount++;
    }

    return { passed, summary, passCount, failCount, failed };
  });
}

// Simple static file server
const server = http.createServer((req, res) => {
  const requestUrl = new URL(req.url, 'http://localhost');

  if (requestUrl.pathname.startsWith('/sourcemap/')) {
    serveSourceMapFile(req, res);
    return;
  }

  if (requestUrl.pathname === '/__qunit') {
    const suiteName = requestUrl.searchParams.get('name');
    const suite = QUNIT_SUITES.find(candidate => candidate.name === suiteName);

    if (!suite) {
      res.writeHead(404);
      res.end(`Unknown suite: ${suiteName}`);
      return;
    }

    res.writeHead(200, { 'Content-Type': 'text/html' });
    res.end(renderQUnitPage(suite));
    return;
  }

  let filePath = path.join(__dirname, decodeURIComponent(req.url === '/' ? '/TestPage.htm' : req.url));
  const ext = path.extname(filePath).toLowerCase();
  const contentType = MIME_TYPES[ext] || 'application/octet-stream';

  fs.readFile(filePath, (err, data) => {
    if (err) {
      res.writeHead(404);
      res.end('Not Found: ' + req.url);
      return;
    }
    res.writeHead(200, { 'Content-Type': contentType });
    res.end(data);
  });
});

server.listen(0, async () => {
  const port = server.address().port;
  console.log(`Static server on http://localhost:${port}`);

  let browser;
  try {
    browser = await chromium.launch({ headless: true });
    const allResults = [];

    for (const suite of QUNIT_SUITES) {
      const page = await browser.newPage();
      page.on('console', msg => {
        if (msg.type() === 'error') console.error('[browser]', msg.text());
      });

      const results = await runSuite(page, port, suite);
      allResults.push({ suite, results });

      console.log(`\n=== QUnit Results: ${suite.name} ===`);
      console.log(results.summary);
      console.log(`Tests: ${results.passCount} passed, ${results.failCount} failed`);

      if (results.failed.length > 0) {
        console.log('\n=== Failed Tests ===');
        for (const f of results.failed) {
          console.log(`\n❌ ${f.module} > ${f.name}`);
          for (const a of f.assertions) {
            console.log(`   ${a.substring(0, 150)}`);
          }
        }
      }

      await page.close();
    }

    if (allResults.every(x => x.results.passed)) {
      console.log('\n✅ All QUnit tests passed!');
      process.exitCode = 0;
    } else {
      console.log('\n❌ Some QUnit tests failed!');
      process.exitCode = 1;
    }
  } catch (err) {
    console.error('Error running QUnit tests:', err.message);
    process.exitCode = 1;
  } finally {
    if (browser) await browser.close();
    server.close();
  }
});

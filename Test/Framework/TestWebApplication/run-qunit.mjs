import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { chromium } from 'playwright';
import { fileURLToPath } from 'node:url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const MIME_TYPES = {
  '.html': 'text/html', '.htm': 'text/html',
  '.js': 'application/javascript', '.css': 'text/css',
  '.map': 'application/json', '.json': 'application/json',
  '.png': 'image/png', '.jpg': 'image/jpeg',
};

// Simple static file server
const server = http.createServer((req, res) => {
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
    const page = await browser.newPage();

    // Collect console messages
    page.on('console', msg => {
      if (msg.type() === 'error') console.error('[browser]', msg.text());
    });

    await page.goto(`http://localhost:${port}/TestPage.htm`, { waitUntil: 'domcontentloaded' });

    // Wait for QUnit to finish (banner gets class qunit-pass or qunit-fail)
    await page.waitForFunction(() => {
      const b = document.getElementById('qunit-banner');
      return b && (b.className.includes('qunit-pass') || b.className.includes('qunit-fail'));
    }, { timeout: 120000 });

    // Extract results
    const results = await page.evaluate(() => {
      const banner = document.getElementById('qunit-banner');
      const passed = banner.className.includes('qunit-pass');

      // Get summary text
      const testResult = document.getElementById('qunit-testresult');
      const summary = testResult ? testResult.textContent.trim() : 'No summary';

      // Get failed test details
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

      // Count passed/failed
      const allTests = document.querySelectorAll('#qunit-tests > li');
      let passCount = 0, failCount = 0;
      for (const li of allTests) {
        if (li.className.includes('pass')) passCount++;
        if (li.className.includes('fail')) failCount++;
      }

      return { passed, summary, passCount, failCount, failed };
    });

    console.log('\n=== QUnit Results ===');
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

    if (results.passed) {
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

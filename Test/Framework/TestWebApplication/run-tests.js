const { chromium } = require('playwright');

// Each NScript-compiled test bundle must run on its own page — bundles
// collide on Function.prototype runtime metadata when co-loaded. See
// the comment block in TestPage.htm and DataTestPage.htm for details.
const SUITE_PAGES = [
  'Sunlight.Framework.TestPage.htm',
  'Sunlight.Framework.UI.TestPage.htm',
  'DataTestPage.htm',
  'TodoApp.TestPage.htm',
];

(async () => {
  const browser = await chromium.launch({ headless: true });

  let totalPassed = 0;
  let totalFailed = 0;
  const allFailures = [];
  const allErrors = [];
  const allDebugLogs = [];

  try {
    for (const suitePage of SUITE_PAGES) {
      const page = await browser.newPage();

      const errors = [];
      const debugLogs = [];
      page.on('console', msg => {
        if (msg.type() === 'error') errors.push(msg.text());
        const text = msg.text();
        if (text.startsWith('[DBG-')) debugLogs.push(text);
      });
      page.on('pageerror', err => errors.push(err.message));

      await page.goto(`http://localhost:3000/${suitePage}`, { waitUntil: 'domcontentloaded' });

      // Wait for QUnit to finish (banner gets class qunit-pass or qunit-fail)
      await page.waitForFunction(() => {
        const banner = document.getElementById('qunit-banner');
        return banner && (banner.className.includes('qunit-pass') || banner.className.includes('qunit-fail'));
      }, { timeout: 60000 });

      // Extract results
      const results = await page.evaluate(() => {
        const tests = document.querySelectorAll('#qunit-tests > li');
        const output = [];
        tests.forEach(li => {
          const name = li.querySelector('.test-name')?.textContent || '';
          const module = li.querySelector('.module-name')?.textContent || '';
          const status = li.className.includes('pass') ? 'PASS' : 'FAIL';
          let failDetail = '';
          if (status === 'FAIL') {
            const assertList = li.querySelectorAll('.qunit-assert-list > li.fail');
            assertList.forEach(a => {
              const expected = a.querySelector('.test-expected td')?.textContent || '';
              const actual = a.querySelector('.test-actual td')?.textContent || '';
              const msg = a.querySelector('.test-message')?.textContent || '';
              const source = a.querySelector('.test-source')?.textContent || '';
              failDetail += `  [${msg}] expected=${expected} actual=${actual}\n`;
              if (source) failDetail += `  source: ${source.trim().substring(0, 200)}\n`;
            });
            // Check for runtime errors (died on test)
            const diedMsg = li.querySelector('.qunit-assert-list .test-message');
            if (diedMsg && diedMsg.textContent.includes('Died on test')) {
              failDetail += `  DIED: ${diedMsg.textContent}\n`;
              const actual = li.querySelector('.qunit-assert-list .test-actual td');
              if (actual) failDetail += `  Error: ${actual.textContent.substring(0, 300)}\n`;
            }
          }
          output.push({ module, name, status, failDetail: failDetail.trim() });
        });

        const passed = document.querySelectorAll('#qunit-tests > li.pass').length;
        const failed = document.querySelectorAll('#qunit-tests > li.fail').length;
        return { tests: output, passed, failed, total: passed + failed };
      });

      console.log(`\n=== [${suitePage}] QUnit Results: ${results.passed} passed, ${results.failed} failed, ${results.total} total ===\n`);

      const failures = results.tests.filter(t => t.status === 'FAIL');
      failures.forEach(f => {
        console.log(`FAIL: [${suitePage}] [${f.module}] ${f.name}`);
        if (f.failDetail) console.log(f.failDetail);
        console.log('');
        allFailures.push({ suitePage, ...f });
      });

      if (debugLogs.length > 0) {
        debugLogs.slice(0, 50).forEach(e => allDebugLogs.push(`[${suitePage}] ${e}`));
      }

      if (errors.length > 0) {
        errors.slice(0, 20).forEach(e => allErrors.push(`[${suitePage}] ${e}`));
      }

      totalPassed += results.passed;
      totalFailed += results.failed;

      await page.close();
    }

    console.log(`\n=== TOTAL: ${totalPassed} passed, ${totalFailed} failed across ${SUITE_PAGES.length} suite pages ===\n`);

    if (allDebugLogs.length > 0) {
      console.log(`\n=== Debug Logs (${allDebugLogs.length}) ===`);
      allDebugLogs.slice(0, 50).forEach(e => console.log(e));
    }

    if (allErrors.length > 0) {
      console.log(`\n=== Console Errors (${allErrors.length}) ===`);
      allErrors.slice(0, 20).forEach(e => console.log(e));
    }

    process.exitCode = totalFailed > 0 ? 1 : 0;
  } finally {
    await browser.close();
  }
})();

// Self-test for bench-spreadsheet.mjs: its timeouts once went in waitForFunction's arg slot, so
// Playwright's 30 s default applied and a busy host's run timed out with no numbers (B17).
// Runs the real benchmark on stand-in pages that never get ready.

import { test } from 'node:test';
import assert from 'node:assert/strict';
import { spawnSync } from 'node:child_process';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

function runBench(page) {
  const run = spawnSync(process.execPath, [path.join(__dirname, 'bench-spreadsheet.mjs')], {
    cwd: __dirname,
    env: { ...process.env, BENCH_PAGE: page, BENCH_TIMEOUT_MS: '1000' },
    encoding: 'utf8',
    // Well under Playwright's 30 s default, so a timeout that is not applied fails here.
    timeout: 15_000,
  });
  return { status: run.status, output: run.stdout + run.stderr };
}

test('the load wait uses its timeout', () => {
  const run = runBench('/test-fixtures/sheet-never-loads.htm');
  assert.equal(run.status, 1, run.output);
  assert.match(run.output, /Timeout 1000ms exceeded/);
});

test('the run wait uses its timeout', () => {
  const run = runBench('/test-fixtures/sheet-never-finishes.htm');
  assert.equal(run.status, 1, run.output);
  assert.match(run.output, /Timeout 1000ms exceeded/);
});

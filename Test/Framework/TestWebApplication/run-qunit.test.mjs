// Self-test for run-qunit.mjs: QUnit calls a suite that ran no tests a pass, so a suite whose
// bundle did not load once passed with exit 0 (B19). Runs the real harness on stand-in suites.

import { test } from 'node:test';
import assert from 'node:assert/strict';
import { spawnSync } from 'node:child_process';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

function runQUnit(suites) {
  const run = spawnSync(process.execPath, [path.join(__dirname, 'run-qunit.mjs')], {
    cwd: __dirname,
    env: { ...process.env, QUNIT_SUITES: JSON.stringify(suites) },
    encoding: 'utf8',
    timeout: 60_000,
  });
  return { status: run.status, output: run.stdout + run.stderr };
}

test('a suite whose bundle is missing fails', () => {
  const run = runQUnit([{ name: 'Missing', scripts: ['/GeneratedScripts/__missing__.js'] }]);
  assert.equal(run.status, 1, run.output);
  assert.match(run.output, /Not loaded: 404 \/GeneratedScripts\/__missing__\.js/);
});

test('a suite that runs no tests fails', () => {
  const run = runQUnit([{ name: 'Empty', scripts: [] }]);
  assert.equal(run.status, 1, run.output);
  assert.match(run.output, /No tests ran/);
});

test('a suite whose tests pass passes', () => {
  const run = runQUnit([{ name: 'Fixture', scripts: ['/test-fixtures/one-qunit-test.js'] }]);
  assert.equal(run.status, 0, run.output);
  assert.match(run.output, /Tests: 1 passed, 0 failed/);
});

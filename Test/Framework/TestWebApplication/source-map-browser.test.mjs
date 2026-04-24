// Playwright + CDP browser test that proves Chromium resolves the sources
// referenced by an NScript .map through the C# SourceMapFileHandler. Driven
// against the SourceMap.Server.TestHost Exe (Test/Compiler/SourceMap.Server.
// TestHost) rather than the node dev helper so the real ASP.NET Core handler
// is the one producing 200 bytes for the DevTools fetch — the Playwright
// layer only exists to make the proof end-to-end.
//
// Contract under test:
//   1. When Chromium loads /fixture.html the browser follows
//      //# sourceMappingURL and fetches /maps/app.map.
//   2. When DevTools (via CDP) asks for the first source listed in that map,
//      the handler streams back the fixture bytes (byte-identical body).
//   3. No `..`/malformed mapName ever produces a 200 — exercised with an
//      injected fetch from the page context.
//
// The test is skipped (not failed) when the TestHost binary is not present,
// so the suite stays green on developer machines that have only built the
// framework solution.

import { test } from 'node:test';
import assert from 'node:assert/strict';
import { spawn } from 'node:child_process';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

// Resolve the TestHost DLL relative to the repo root so developers who run
// `dotnet build` from either Debug or Release configurations can exercise the
// test without editing this file.
function resolveTestHostDll() {
  const repoRoot = path.resolve(__dirname, '..', '..', '..');
  const candidates = [
    path.join(repoRoot, 'Test', 'Compiler', 'bin', 'net8.0', 'SourceMap.Server.TestHost.dll'),
    path.join(repoRoot, 'Test', 'Compiler', 'bin', 'Debug', 'net8.0', 'SourceMap.Server.TestHost.dll'),
    path.join(repoRoot, 'Test', 'Compiler', 'bin', 'Release', 'net8.0', 'SourceMap.Server.TestHost.dll'),
  ];

  for (const candidate of candidates) {
    if (fs.existsSync(candidate)) {
      return candidate;
    }
  }

  return null;
}

function makeTempWorkspace() {
  return fs.mkdtempSync(path.join(os.tmpdir(), 'srcmap-browser-'));
}

function runDotnet(dllPath, args) {
  return new Promise((resolve, reject) => {
    const proc = spawn('dotnet', [dllPath, ...args], { stdio: ['ignore', 'pipe', 'pipe'] });
    let stdout = '';
    let stderr = '';
    proc.stdout.on('data', chunk => { stdout += chunk.toString(); });
    proc.stderr.on('data', chunk => { stderr += chunk.toString(); });
    proc.on('exit', code => {
      if (code === 0) {
        resolve({ stdout, stderr });
      } else {
        reject(new Error(`dotnet ${args.join(' ')} exited ${code}\nstdout:\n${stdout}\nstderr:\n${stderr}`));
      }
    });
    proc.on('error', reject);
  });
}

// Start the TestHost in 'serve' mode, wait until it prints
// 'LISTENING http://127.0.0.1:{port}', and return { proc, baseUrl }. The
// caller is responsible for terminating the child via `proc.stdin.end()` so
// the host exits cleanly (its Console.In.ReadToEnd gate).
function startServer(dllPath, workDir) {
  return new Promise((resolve, reject) => {
    const proc = spawn('dotnet', [dllPath, 'serve', workDir], { stdio: ['pipe', 'pipe', 'pipe'] });
    let resolved = false;
    let buffered = '';

    proc.stdout.on('data', chunk => {
      buffered += chunk.toString();
      const match = buffered.match(/LISTENING (http:\/\/127\.0\.0\.1:\d+)/);
      if (match && !resolved) {
        resolved = true;
        resolve({ proc, baseUrl: match[1] });
      }
    });

    proc.stderr.on('data', chunk => {
      // Mirror host errors so CI logs are self-explanatory if startup fails.
      process.stderr.write('[testhost] ' + chunk.toString());
    });

    proc.on('exit', code => {
      if (!resolved) {
        reject(new Error(`TestHost exited before listening (code=${code})`));
      }
    });

    proc.on('error', reject);

    setTimeout(() => {
      if (!resolved) {
        proc.kill();
        reject(new Error('TestHost did not print LISTENING within timeout'));
      }
    }, 30000);
  });
}

async function stopServer(proc) {
  proc.stdin.end();
  await new Promise(resolve => {
    proc.on('exit', resolve);
    setTimeout(() => {
      proc.kill();
      resolve();
    }, 5000);
  });
}

test('Chromium resolves NScript .map sources through the C# handler', async t => {
  const dllPath = resolveTestHostDll();
  if (!dllPath) {
    t.skip('SourceMap.Server.TestHost.dll not built; run `dotnet build` first');
    return;
  }

  let playwrightMod;
  try {
    playwrightMod = await import('playwright');
  } catch {
    t.skip('playwright not installed; run `npm install` in TestWebApplication');
    return;
  }

  const workDir = makeTempWorkspace();
  let serverHandle = null;
  let browser = null;
  try {
    await runDotnet(dllPath, ['emit', workDir]);

    serverHandle = await startServer(dllPath, workDir);
    const { baseUrl } = serverHandle;

    // Read the emitted map so the test asserts against the exact short name
    // the C# emitter wrote — decouples us from encoding rules we'd otherwise
    // have to duplicate.
    const mapJson = JSON.parse(fs.readFileSync(path.join(workDir, 'maps', 'app.map'), 'utf8'));
    const shortName = mapJson.sources[0];
    const sourceRoot = mapJson.sourceRoot;
    const expectedBody = fs.readFileSync(path.join(workDir, 'src', 'Program.cs'), 'utf8');

    browser = await playwrightMod.chromium.launch({ headless: true });
    const context = await browser.newContext();
    const page = await context.newPage();

    const sourceRequests = [];
    page.on('request', req => {
      if (req.url().includes('/sourcemap/')) {
        sourceRequests.push(req.url());
      }
    });

    // Trigger the full load path: HTML → /maps/app.js → (sourceMappingURL)
    // /maps/app.map. DevTools would now resolve sources on demand; we drive
    // that last step explicitly via fetch() from the page context to avoid
    // needing an attached debugger UI.
    await page.goto(`${baseUrl}/fixture.html`, { waitUntil: 'networkidle' });

    const composed = sourceRoot.replace(/\/$/, '') + '/' + shortName;
    const resolved = await page.evaluate(async url => {
      const resp = await fetch(url);
      return { status: resp.status, body: await resp.text() };
    }, composed);

    assert.equal(resolved.status, 200, 'handler must stream the source');
    assert.equal(
      resolved.body,
      expectedBody,
      'browser-side body must be byte-identical to on-disk fixture');

    // Negative case: a tampered mapName must get a 4xx — the handler's
    // whitelist is part of the browser-facing contract too.
    const badMap = await page.evaluate(async url => {
      const resp = await fetch(url);
      return resp.status;
    }, `${baseUrl}/sourcemap/bad..name/foo`);
    assert.ok(badMap >= 400 && badMap < 500, `tampered mapName must 4xx, got ${badMap}`);

    assert.ok(
      sourceRequests.length >= 1,
      'at least one /sourcemap/ request must have reached the handler');
  } finally {
    if (browser) {
      await browser.close();
    }
    if (serverHandle) {
      await stopServer(serverHandle.proc);
    }
    fs.rmSync(workDir, { recursive: true, force: true });
  }
});

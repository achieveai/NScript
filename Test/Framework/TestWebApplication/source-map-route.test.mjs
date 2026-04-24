// Smoke test for source-map-route.mjs — the node dev-ergonomic counterpart of
// the C# SourceMapFileHandler. Spins up a tiny http server that routes through
// the extracted helper and drives it with real HTTP requests so the contract
// (200/404/400 shapes, traversal rejection, body bytes) is exercised end-to-end
// without Playwright or a browser in the loop.

import { test } from 'node:test';
import assert from 'node:assert/strict';
import http from 'node:http';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { createSourceMapRouteHandler } from './source-map-route.mjs';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

function makeTempWorkspace() {
  const root = fs.mkdtempSync(path.join(os.tmpdir(), 'srcmap-route-'));
  const mapsDir = path.join(root, 'maps');
  const srcDir = path.join(root, 'src');
  fs.mkdirSync(mapsDir);
  fs.mkdirSync(srcDir);
  return { root, mapsDir, srcDir };
}

// Write a source-map-V3 shaped file that only uses the fields the handler
// reads (sources + sourcesLong). Keeps the test self-contained: no dependency
// on the C# emitter.
function writeMap(mapsDir, mapName, sources, sourcesLong) {
  const mapPath = path.join(mapsDir, mapName + '.map');
  fs.writeFileSync(
    mapPath,
    JSON.stringify({
      version: 3,
      file: mapName + '.js',
      sources,
      sourcesLong,
      names: [],
      mappings: '',
    }),
    'utf8');
  return mapPath;
}

async function withServer(handler, fn) {
  const server = http.createServer((req, res) => {
    if (handler(req, res)) {
      return;
    }

    res.writeHead(500);
    res.end('Unhandled');
  });

  await new Promise(resolve => server.listen(0, '127.0.0.1', resolve));
  const { port } = server.address();
  try {
    await fn(`http://127.0.0.1:${port}`);
  } finally {
    await new Promise(resolve => server.close(resolve));
  }
}

async function fetchText(url) {
  const resp = await fetch(url);
  return { status: resp.status, body: await resp.text() };
}

// Raw http client that preserves the literal request path — fetch/undici
// normalizes '..' and percent-encoded dots, which would collide with the
// behavior under test here.
function fetchRawPath(baseUrl, rawPath) {
  const { hostname, port } = new URL(baseUrl);
  return new Promise((resolve, reject) => {
    const req = http.request({
      host: hostname,
      port,
      method: 'GET',
      path: rawPath,
    }, res => {
      const chunks = [];
      res.on('data', chunk => chunks.push(chunk));
      res.on('end', () => resolve({
        status: res.statusCode,
        body: Buffer.concat(chunks).toString('utf8'),
      }));
    });
    req.on('error', reject);
    req.end();
  });
}

test('GET {sourceRoot}/{shortName} streams the mapped source bytes', async () => {
  const { root, mapsDir, srcDir } = makeTempWorkspace();
  try {
    const programCs = path.join(srcDir, 'Program.cs');
    fs.writeFileSync(programCs, 'namespace Fixture { class P { } }', 'utf8');

    // sources holds the URL-normalized short name the C# emitter writes;
    // sourcesLong holds the absolute on-disk path.
    const shortName = 'C$/fixture/Program.cs';
    writeMap(mapsDir, 'app', [shortName], [programCs]);

    const handler = createSourceMapRouteHandler(mapsDir);
    await withServer(handler, async base => {
      const encoded = shortName.split('/').map(encodeURIComponent).join('/');
      const resp = await fetchText(`${base}/sourcemap/app/${encoded}`);

      assert.equal(resp.status, 200);
      assert.equal(resp.body, 'namespace Fixture { class P { } }');
    });
  } finally {
    fs.rmSync(root, { recursive: true, force: true });
  }
});

test('GET with unknown short name returns 404', async () => {
  const { root, mapsDir, srcDir } = makeTempWorkspace();
  try {
    const programCs = path.join(srcDir, 'Program.cs');
    fs.writeFileSync(programCs, '// fixture\n', 'utf8');
    writeMap(mapsDir, 'app', ['C$/fixture/Program.cs'], [programCs]);

    const handler = createSourceMapRouteHandler(mapsDir);
    await withServer(handler, async base => {
      const resp = await fetchText(`${base}/sourcemap/app/C%24/fixture/Phantom.cs`);
      assert.equal(resp.status, 404);
    });
  } finally {
    fs.rmSync(root, { recursive: true, force: true });
  }
});

test('GET with unknown map returns 404', async () => {
  const { root, mapsDir } = makeTempWorkspace();
  try {
    const handler = createSourceMapRouteHandler(mapsDir);
    await withServer(handler, async base => {
      const resp = await fetchText(`${base}/sourcemap/missing/C%24/fixture/Program.cs`);
      assert.equal(resp.status, 404);
    });
  } finally {
    fs.rmSync(root, { recursive: true, force: true });
  }
});

test('GET with traversal in mapName returns 400', async () => {
  const { root, mapsDir } = makeTempWorkspace();
  try {
    const handler = createSourceMapRouteHandler(mapsDir);
    await withServer(handler, async base => {
      // Raw http request — fetch would URL-normalize '..' before sending, so
      // the handler's own whitelist would never get a chance to reject it.
      const resp = await fetchRawPath(base, '/sourcemap/%2E%2E/Program.cs');
      assert.equal(resp.status, 400);
    });
  } finally {
    fs.rmSync(root, { recursive: true, force: true });
  }
});

test('GET with empty sub-path under /sourcemap/ returns 404', async () => {
  const { root, mapsDir } = makeTempWorkspace();
  try {
    const handler = createSourceMapRouteHandler(mapsDir);
    await withServer(handler, async base => {
      // No second segment — regex does not match and handler responds 404 so
      // the shape cannot leak into the ambient 500 handler.
      const resp = await fetchText(`${base}/sourcemap/`);
      assert.equal(resp.status, 404);
    });
  } finally {
    fs.rmSync(root, { recursive: true, force: true });
  }
});

test('factory rejects missing generatedScriptsDir', () => {
  assert.throws(() => createSourceMapRouteHandler(''), /non-empty string/);
  assert.throws(() => createSourceMapRouteHandler(null), /non-empty string/);
});

// Checks a dev bundle that a patch emit (NSCRIPT_PATCH_EMIT=1) wrote: it boots with no
// error the full emit's bundle does not have, and the bundle's closure holds the patched
// code. A probe is evaluated inside the closure through the bundle's live patch hook
// (window.__nsPatch["<file>.js"]), so it sees the bindings the bundle's own code calls.
//
//   node check-patched-bundle.mjs <patched.js> [--baseline <full.js>] [--page <page.htm>]
//        [--probe <js expression> <text the expression's string must contain>]... [--wait <ms>]
//
// Without --page the bundle loads on a blank page. With --page, the page's folder is served
// and a request for a file named like the bundle gets the bundle under test. Errors are page
// errors and console errors, compared by text with line:column and ports removed.
import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { chromium } from 'playwright';

const MIME = { '.htm': 'text/html', '.html': 'text/html', '.js': 'application/javascript', '.css': 'text/css', '.map': 'application/json' };

function parseArgs(argv) {
  const options = { probes: [], wait: 2000 };
  for (let i = 0; i < argv.length; i++) {
    const arg = argv[i];
    if (arg === '--baseline') options.baseline = path.resolve(argv[++i]);
    else if (arg === '--page') options.page = path.resolve(argv[++i]);
    else if (arg === '--wait') options.wait = Number(argv[++i]);
    else if (arg === '--probe') options.probes.push({ expression: argv[++i], text: argv[++i] });
    else if (!options.bundle) options.bundle = path.resolve(arg);
    else throw new Error('Unexpected argument: ' + arg);
  }

  if (!options.bundle || options.probes.some(p => p.text === undefined) || !(options.wait >= 0)) {
    throw new Error('usage: check-patched-bundle.mjs <patched.js> [--baseline <full.js>] [--page <page.htm>] [--probe <expr> <text>]... [--wait <ms>]');
  }

  return options;
}

function serve(bundle, page) {
  const bundleName = path.basename(bundle);
  const root = page ? path.dirname(page) : null;
  const server = http.createServer((req, res) => {
    const url = decodeURIComponent(req.url.split('?')[0]);
    let file;
    if (url === '/') {
      if (!page) {
        res.writeHead(200, { 'Content-Type': 'text/html' });
        res.end('<!DOCTYPE html><html><head><meta charset="utf-8"></head><body><script src="/' + bundleName + '"></script></body></html>');
        return;
      }

      file = page;
    } else if (path.basename(url) === bundleName) {
      file = bundle;
    } else if (path.basename(url) === bundleName.replace(/\.js$/, '.map')) {
      file = bundle.replace(/\.js$/, '.map');
    } else if (root) {
      file = path.join(root, url);
    }

    if (!file) {
      res.writeHead(404);
      res.end();
      return;
    }

    fs.readFile(file, (err, data) => {
      if (err) {
        res.writeHead(404);
        res.end();
        return;
      }

      res.writeHead(200, { 'Content-Type': MIME[path.extname(file)] || 'application/octet-stream' });
      res.end(data);
    });
  });
  return new Promise(resolve => server.listen(0, () => resolve(server)));
}

const normalize = text => text.replace(/localhost:\d+/g, 'localhost').replace(/:\d+:\d+/g, ':L:C');

async function load(browser, bundle, options) {
  const server = await serve(bundle, options.page);
  const page = await browser.newPage();
  // The runtime replaces Function.prototype.toString: keep the native one for probes.
  await page.addInitScript(() => { window.__nsNativeToString = Function.prototype.toString; });
  const errors = [];
  page.on('pageerror', e => errors.push(normalize(e.message)));
  page.on('console', m => {
    if (m.type() === 'error') errors.push(normalize(m.text()));
  });
  try {
    await page.goto('http://localhost:' + server.address().port + '/', { waitUntil: 'load' });
    await page.waitForTimeout(options.wait);
    const probes = [];
    for (const probe of options.probes) {
      probes.push(await page.evaluate(([name, expression]) => {
        const hook = window.__nsPatch && window.__nsPatch[name];
        if (!hook) {
          return { error: 'no live patch hook for ' + name };
        }

        try {
          window.__nsProbe = undefined;
          hook('window.__nsProbe = (function (v) { return typeof v === "function" ? window.__nsNativeToString.call(v) : String(v); })(' + expression + ');');
          return { value: window.__nsProbe };
        } catch (e) {
          return { error: String(e) };
        }
      }, [path.basename(bundle), probe.expression]));
    }

    return { errors, probes };
  } finally {
    await page.close();
    server.close();
  }
}

let failed = false;
function check(name, ok, detail) {
  console.log((ok ? 'PASS' : 'FAIL') + ': ' + name + (detail ? ' (' + detail + ')' : ''));
  failed = failed || !ok;
}

const options = parseArgs(process.argv.slice(2));
const browser = await chromium.launch();
try {
  const patched = await load(browser, options.bundle, options);
  if (options.baseline) {
    const baseline = await load(browser, options.baseline, options);
    const known = new Map();
    for (const error of baseline.errors) known.set(error, (known.get(error) || 0) + 1);
    const added = [];
    for (const error of patched.errors) {
      const count = known.get(error) || 0;
      if (count > 0) known.set(error, count - 1);
      else added.push(error);
    }

    check('no error the full emit does not have', added.length === 0,
      added.length ? added.slice(0, 3).join(' | ') : patched.errors.length + ' errors, all in the full emit too');
  } else {
    check('no page or console errors', patched.errors.length === 0, patched.errors.slice(0, 3).join(' | '));
  }

  options.probes.forEach((probe, i) => {
    const result = patched.probes[i];
    check('closure holds ' + probe.expression,
      result.error === undefined && result.value.includes(probe.text),
      result.error || (result.value.includes(probe.text) ? 'contains ' + JSON.stringify(probe.text) : result.value.slice(0, 200)));
  });
} finally {
  await browser.close();
}

process.exitCode = failed ? 1 : 0;

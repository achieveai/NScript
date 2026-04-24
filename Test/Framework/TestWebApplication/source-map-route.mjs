// Dev-ergonomic counterpart of the ASP.NET Core SourceMapFileHandler shipped
// in Sources/Compiler/SourceMap.Server. Serves original source files referenced
// by an NScript .map when the map's sourceRoot points at
// /sourcemap/{mapName}/. Returns 404 on any mismatch so nothing outside the
// map's pre-recorded paths can be served.
//
// Factored out of run-qunit.mjs so the behavior is testable in isolation (see
// source-map-route.test.mjs) and reusable from any node http server.

import fs from 'node:fs';
import path from 'node:path';

// Mirror the C# SourceMapFileHandler whitelist (SourceMapFileHandler.cs):
//   ^[A-Za-z0-9_-][A-Za-z0-9._-]*$
// First char rejects leading dots (so ".hidden" fails closed), remaining
// characters reject separators, colons, spaces, and any other URL-hostile
// input that would otherwise reach the filesystem join below.
const MAP_NAME_ALLOWED = /^[A-Za-z0-9_-][A-Za-z0-9._-]*$/;

/**
 * Create a request handler bound to a specific generated-scripts directory.
 * Returns a function (req, res) => boolean — true when the request was
 * handled, false when the caller should continue routing.
 *
 * @param {string} generatedScriptsDir Absolute path to the directory holding
 *   the emitted .map files. Sources referenced from those maps may live
 *   anywhere on disk (sourcesLong entries are absolute paths).
 */
export function createSourceMapRouteHandler(generatedScriptsDir) {
  if (!generatedScriptsDir || typeof generatedScriptsDir !== 'string') {
    throw new TypeError('generatedScriptsDir must be a non-empty string');
  }

  return function handleSourceMapRoute(req, res) {
    const match = req.url.match(/^\/sourcemap\/([^/]+)\/(.+)$/);
    if (!match) {
      res.writeHead(404);
      res.end('Not Found');
      return true;
    }

    const mapName = decodeURIComponent(match[1]);
    const shortName = decodeURIComponent(match[2]);

    if (!MAP_NAME_ALLOWED.test(mapName)) {
      res.writeHead(400);
      res.end('Bad Request');
      return true;
    }

    const mapPath = path.join(generatedScriptsDir, mapName + '.map');
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
  };
}

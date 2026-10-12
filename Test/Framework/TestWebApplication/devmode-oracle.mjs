// Dev-mode byte oracle for the NScript build service.
//
// Replays the last `emitJs` request the service handled for a bundle as a batch
// `nscript.exe` run (same args, same cwd, -outJs redirected to a temp dir with the
// same file name) and byte-compares the .js and .map with what the service wrote.
//
// Usage: node devmode-oracle.mjs <Bundle> [--log <service.jsonl>]
//   <Bundle>  output name without extension, e.g. TodoApp
//   --log     service log to read; default: newest matching request across
//             %LOCALAPPDATA%\NScript\service\*\service.jsonl
// Prints MATCH (exit 0) or DIFF with the first differing line (exit 1).

import { spawnSync } from 'node:child_process';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';

function fail(message) {
  console.error(`ERROR: ${message}`);
  process.exit(2);
}

const argv = process.argv.slice(2);
const bundle = argv[0];
if (!bundle || bundle.startsWith('-')) {
  fail('usage: node devmode-oracle.mjs <Bundle> [--log <service.jsonl>]');
}
const logIndex = argv.indexOf('--log');
const logFiles = logIndex >= 0
  ? [argv[logIndex + 1]]
  : findServiceLogs();
if (logFiles.length === 0) {
  fail('no service.jsonl found under %LOCALAPPDATA%\\NScript\\service');
}

function findServiceLogs() {
  const root = path.join(process.env.LOCALAPPDATA || os.homedir(), 'NScript', 'service');
  if (!fs.existsSync(root)) return [];
  return fs.readdirSync(root)
    .map(dir => path.join(root, dir, 'service.jsonl'))
    .filter(file => fs.existsSync(file));
}

function readEvents(file) {
  return fs.readFileSync(file, 'utf8')
    .split('\n')
    .filter(line => line.startsWith('{'))
    .map(line => {
      try { return JSON.parse(line); } catch { return null; }
    })
    .filter(Boolean);
}

function outJsIndex(args) {
  return (args || []).findIndex(a => a.toLowerCase() === '-outjs');
}

// Newest RequestStart for this bundle across the logs, with the daemon's ServiceStart.
let best = null;
for (const file of logFiles) {
  const events = readEvents(file);
  const startsByPid = new Map();
  for (const e of events) {
    if (e['@mt']?.startsWith('ServiceStart')) startsByPid.set(e.Pid, e);
    if (!e['@mt']?.startsWith('RequestStart') || e.Kind !== 'emitJs') continue;
    const i = outJsIndex(e.Args);
    if (i < 0 || path.win32.basename(e.Args[i + 1]).toLowerCase() !== `${bundle}.js`.toLowerCase()) continue;
    if (!best || e['@t'] > best.request['@t']) {
      const end = events.find(x => x['@mt']?.startsWith('RequestEnd') && x.Pid === e.Pid && x.RequestId === e.RequestId);
      best = { request: e, end, start: startsByPid.get(e.Pid), file };
    }
  }
}
if (!best) fail(`no emitJs request for ${bundle}.js in ${logFiles.join(', ')}`);
const { request, end, start } = best;
if (!start) fail(`no ServiceStart for daemon pid ${request.Pid} in ${best.file}`);
if (!end) fail(`request ${request.RequestId} (pid ${request.Pid}) has no RequestEnd yet`);
if (end.ExitCode !== 0 || end.InternalError) fail(`request ${request.RequestId} ended with exit ${end.ExitCode}, internal error ${end.InternalError}`);

const exe = path.join(start.ToolsetDir, process.platform === 'win32' ? 'NScript.exe' : 'nscript');
const args = [...request.Args];
const i = outJsIndex(args);
const serviceJs = path.resolve(request.Cwd, args[i + 1]);
const tempDir = fs.mkdtempSync(path.join(os.tmpdir(), 'nscript-oracle-'));
const batchJs = path.join(tempDir, path.basename(serviceJs));
args[i + 1] = batchJs;

console.log(`request ${request.RequestId} (daemon pid ${request.Pid}, ${request['@t']})`);
console.log(`batch: ${exe} (cwd ${request.Cwd})`);
const run = spawnSync(exe, args, { cwd: request.Cwd, encoding: 'utf8', stdio: ['ignore', 'pipe', 'pipe'] });
if (run.status !== 0) {
  console.error(run.stdout, run.stderr);
  fail(`batch nscript exited ${run.status}`);
}

function firstDiff(a, b) {
  const la = a.toString('utf8').split('\n');
  const lb = b.toString('utf8').split('\n');
  for (let n = 0; n < Math.max(la.length, lb.length); n++) {
    if (la[n] !== lb[n]) {
      const clip = s => (s === undefined ? '<missing>' : s.slice(0, 200));
      return `line ${n + 1}\n  service: ${clip(la[n])}\n  batch:   ${clip(lb[n])}`;
    }
  }
  return 'same lines, different bytes';
}

let same = true;
for (const ext of ['.js', '.map']) {
  const servicePath = serviceJs.replace(/\.js$/i, ext);
  const batchPath = batchJs.replace(/\.js$/i, ext);
  const a = fs.readFileSync(servicePath);
  const b = fs.readFileSync(batchPath);
  if (a.equals(b)) {
    console.log(`  ${ext}: equal (${a.length} bytes)`);
  } else {
    same = false;
    console.log(`  ${ext}: DIFF ${servicePath} vs ${batchPath}: ${firstDiff(a, b)}`);
  }
}

if (same) {
  fs.rmSync(tempDir, { recursive: true, force: true });
  console.log('MATCH');
  process.exit(0);
}
console.log('DIFF');
process.exit(1);

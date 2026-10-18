// The live patch client (Sources/Compiler/NScript.JST/HmrClient.js) in a fake page: socket,
// fetch, storage, timers and clock are stand-ins the test drives. The client is baked the way
// HmrClient.Bake bakes it, so a statement that leans on a line break fails here too.

import { test } from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import path from 'node:path';
import vm from 'node:vm';
import { fileURLToPath } from 'node:url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const SOURCE = fs.readFileSync(path.join(__dirname, '../../../Sources/Compiler/NScript.JST/HmrClient.js'), 'utf8');
const CLIENT = SOURCE.split('\n').map(line => line.trim()).filter(line => line && !line.startsWith('//')).join(' ');

const FILE = 'TodoApp.js';
// Values from the page's realm, as plain values here (vm arrays have another prototype).
const plain = value => JSON.parse(JSON.stringify(value));
const boot = stamp => `${FILE}|abc123|ws://127.0.0.1:9/nscript-hmr|tok|${stamp}`;
const bundle = stamp => `(function(){window.__nsPatch;})();\n__nsHmrBoot("${boot(stamp)}");`;

// before(win) runs ahead of the boot call, e.g. to seed storage or break the patch hook.
function page(stamp, disk = bundle(stamp), before = () => {}, bootValue = boot(stamp)) {
  const p = { disk, sockets: [], fetches: [], applied: [], logs: [], reloads: 0, now: 1_000_000 };
  const timers = new Map();
  let nextTimer = 1;
  const storage = () => {
    const items = new Map();
    return { getItem: k => (items.has(k) ? items.get(k) : null), setItem: (k, v) => items.set(k, String(v)), removeItem: k => items.delete(k) };
  };

  class FakeSocket {
    // p.refuse: each new socket closes before it opens, as with the daemon down.
    constructor(url) { this.url = url; this.closed = false; p.sockets.push(this); if (p.refuse) queueMicrotask(() => this.drop()); }
    close() { this.drop(); }
    open() { this.onopen(); }
    deliver(message) { this.onmessage({ data: JSON.stringify(message) }); }
    drop() { if (!this.closed) { this.closed = true; this.onclose(); } }
  }

  const win = {
    console: { info: m => p.logs.push(m) },
    Date: { now: () => p.now },
    setTimeout: (fn, ms) => { timers.set(nextTimer, { at: p.now + ms, fn }); return nextTimer++; },
    clearTimeout: id => timers.delete(id),
    WebSocket: FakeSocket,
    fetch: url => {
      p.fetches.push(url);
      // p.response: what the server answers instead of the script, e.g. a 404 or a fallback page.
      const { status = 200, type = 'application/javascript', text = p.disk } = p.response || {};
      return Promise.resolve({ status, headers: { get: () => type }, text: () => Promise.resolve(text) });
    },
    sessionStorage: storage(),
    localStorage: storage(),
    location: { reload: () => { p.reloads++; } },
    document: {
      currentScript: { src: `http://localhost:5000/${FILE}` },
      hidden: false,
      addEventListener() {},
      removeEventListener() {},
      getElementsByTagName: () => [],
    },
    __nsPatch: { [FILE]: js => p.applied.push(js) },
  };
  win.window = win;
  before(win);
  vm.createContext(win);
  vm.runInContext(`${CLIENT}\n__nsHmrBoot(${JSON.stringify(bootValue)});`, win);

  p.win = win;
  p.state = () => win.__nsHmr[FILE];
  p.open = () => p.sockets.filter(socket => !socket.closed);
  // Settles the fetch promise chains.
  p.flush = async () => { for (let i = 0; i < 5; i++) await new Promise(r => setImmediate(r)); };
  // Moves the clock and runs the timers that came due, in order.
  p.advance = async ms => {
    const end = p.now + ms;
    for (;;) {
      const due = [...timers].filter(([, t]) => t.at <= end).sort((a, b) => a[1].at - b[1].at)[0];
      if (!due) break;
      timers.delete(due[0]);
      p.now = due[1].at;
      due[1].fn();
      await p.flush();
    }
    p.now = end;
  };
  return p;
}

test('a daemon that lost the gen keeps the page on its one open socket until a new build', async () => {
  // The daemon restarted: it has no records, so its hello names no gen. The bundle on disk is
  // the page's own, so the page waits; it must wait on the socket it has.
  const p = page('g1:0');
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: '' });
  await p.flush();
  assert.equal(p.fetches.length, 1, 'the page checks its script once');
  assert.equal(p.reloads, 0, 'its script holds its own code');

  await p.advance(60_000);
  assert.equal(p.sockets.length, 1, 'no second socket while the first is open');

  // The next full emit reaches the page through that socket.
  p.disk = bundle('g2:0');
  p.sockets[0].deliver({ gen: 'g2', reload: 'new build' });
  await p.flush();
  assert.equal(p.reloads, 1, 'the new build reloads the page');
});

test('a page served from a lagging copy waits on its one socket, then reloads once the copy updates', async () => {
  // D1: the page's server copies the bundle late. A full emit's gen reaches the socket
  // before the copy, so the page waits; it once opened a socket a second until the cap.
  const p = page('g1:0');
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g2', reload: 'new build' });
  await p.flush();
  assert.equal(p.reloads, 0, 'the copy still holds g1');
  assert.equal(p.state().state, 'waiting');

  await p.advance(60_000);
  assert.equal(p.sockets.length, 1, 'no second socket while the first is open');
  assert.equal(p.reloads, 0);
  // Backoff 1, 2, 4, 8, 16, 30 s: a handful of checks a minute, not one a second.
  assert.ok(p.fetches.length >= 3 && p.fetches.length <= 8, `checks in 60 s: ${p.fetches.length}`);

  // D2: the copy updates with no socket event; the page's own re-check must find it.
  p.disk = bundle('g2:0');
  await p.advance(30_000);
  assert.equal(p.reloads, 1, 'the updated copy reloads the page');
  assert.equal(p.sockets.length, 1);
});

test('a page that cannot reach the daemon says so once and is disconnected', async () => {
  // A2: the socket fails before it opens (daemon down, or 503 at the cap); the script holds
  // the page's own code, so the page keeps trying, and says why.
  const p = page('g1:0');
  p.refuse = true;
  p.sockets[0].drop();
  await p.flush();
  assert.equal(p.state().state, 'disconnected');
  await p.advance(60_000);
  assert.equal(p.logs.filter(line => line.includes('cannot reach the build service')).length, 1, p.logs.join('\n'));
  assert.ok(p.sockets.length > 2, 'it keeps trying');
  assert.equal(p.reloads, 0);
});

test('a page that waits and then takes the next patch is connected again and stops checking', async () => {
  const p = page('g1:0');
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 2, js: 'two', methods: 1 }] });
  await p.flush();
  assert.equal(p.state().state, 'waiting', 'a gap with a lagging copy waits');

  // The server's catch-up arrives on the same socket.
  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 1, js: 'one', methods: 1 }, { seq: 2, js: 'two', methods: 1 }] });
  await p.flush();
  assert.deepEqual(p.applied, ['one', 'two']);
  assert.equal(p.state().state, 'connected');
  const checks = p.fetches.length;
  await p.advance(120_000);
  assert.equal(p.fetches.length, checks, 'no checks once the page is in step');
  assert.equal(p.reloads, 0);
});

test('records apply once each, in order, through the bundle hook; the socket url names the page stamp', async () => {
  const p = page('g1:2');
  assert.match(p.sockets[0].url, /^ws:\/\/127\.0\.0\.1:9\/nscript-hmr\?t=tok&id=abc123&file=TodoApp\.js&at=g1:2$/);
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 1, js: 'old' }, { seq: 2, js: 'held' }, { seq: 3, js: 'three', methods: 2, notes: ['ctor A..ctor'] }] });
  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 3, js: 'three again' }, { seq: 4, js: 'four', methods: 1 }] });
  await p.flush();
  assert.deepEqual(p.applied, ['three', 'four'], 'seq <= held is skipped; each seq runs once');
  assert.deepEqual(plain([p.state().seq, p.state().baseSeq, p.state().applied]), [4, 2, [3, 4]]);
  assert.deepEqual(plain(p.state().notes), ['ctor A..ctor']);
  assert.ok(p.logs.includes('[nscript-hmr] applied g1:3 (2 methods)'), p.logs.join('\n'));
  assert.equal(p.fetches.length, 0, 'an in-step page never checks its script');
});

test('a socket that drops reconnects at the seq the page holds and catches up with no reload', async () => {
  // MT7: two saves while the socket is down reach the page in the next hello.
  const p = page('g1:0');
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 1, js: 'one', methods: 1 }] });
  p.sockets[0].drop();
  assert.equal(p.state().state, 'disconnected');
  assert.ok(p.logs.includes('[nscript-hmr] disconnected; reconnecting in 1s'), p.logs.join('\n'));

  await p.advance(1_000);
  assert.equal(p.open().length, 1, 'one new socket');
  assert.match(p.open()[0].url, /&at=g1:1$/, 'the new socket names the seq the page holds');
  p.open()[0].open();
  p.open()[0].deliver({ gen: 'g1', records: [{ seq: 2, js: 'two', methods: 1 }, { seq: 3, js: 'three', methods: 1 }] });
  await p.flush();
  assert.deepEqual(p.applied, ['one', 'two', 'three']);
  assert.equal(p.state().state, 'connected');
  assert.equal(p.fetches.length, 0, 'an in-step page never checks its script');
  assert.equal(p.reloads, 0);
});

test('a patch that throws is not counted; the page checks its script and reloads onto it', async () => {
  const p = page('g1:0', bundle('g1:1'), win => { win.__nsPatch[FILE] = () => { throw new Error('boom'); }; });
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 1, js: 'x', methods: 1 }] });
  await p.flush();
  assert.equal(p.state().seq, 0);
  assert.ok(p.logs.some(line => line.includes('live patch failed: boom')));
  assert.equal(p.reloads, 1, 'the script on disk holds g1:1');
});

test('at most 2 reloads in 60 s; then live patches still apply, and a build after the 60 s reloads', async () => {
  const p = page('g1:0', bundle('g2:0'), win => win.sessionStorage.setItem('nscriptHmr:reloads', JSON.stringify([999_000, 999_500])));
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g2', reload: 'new build' });
  await p.flush();
  p.sockets[0].deliver({ gen: 'g2', reload: 'new build' });
  await p.flush();
  assert.equal(p.reloads, 0);
  assert.equal(p.state().state, 'reload-only');
  assert.equal(p.logs.filter(line => line.includes('reload-only')).length, 1, 'said once: ' + p.logs.join(' | '));
  assert.equal(p.open().length, 1, 'the socket stays');

  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 1, js: 'one', methods: 1 }] });
  assert.deepEqual(p.applied, ['one'], 'a capped page still takes patches');

  await p.advance(60_000);
  p.disk = bundle('g3:0');
  p.sockets[0].deliver({ gen: 'g3', reload: 'new build' });
  await p.flush();
  assert.equal(p.reloads, 1, 'the reloads that capped it are past the 60 s');
});

test('a bundle that threw while it loaded keeps its client: the next record reloads it, never patches it', async () => {
  // The closure's catch names the error; a patch cannot finish a start that threw.
  const p = page('g1:0', bundle('g1:1'), win => { win.__nsHmrFailed = { [FILE]: 'boom at start' }; });
  assert.ok(p.logs.includes('[nscript-hmr] load failed: boom at start; the next build reloads'), p.logs.join(' | '));
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 1, js: 'fix', methods: 1 }] });
  await p.flush();
  assert.deepEqual(p.applied, []);
  assert.equal(p.reloads, 1);
  assert.ok(p.logs.includes('[nscript-hmr] reloading: load failed'), p.logs.join(' | '));
});

test('localStorage off: no socket, no checks', async () => {
  const p = page('g1:0', undefined, win => win.localStorage.setItem('nscriptHmr', 'off'));
  await p.advance(60_000);
  assert.equal(p.state().state, 'off');
  assert.equal(p.sockets.length + p.fetches.length, 0);
});

test('a bundle built outside the daemon has no url: it only checks its script, and reloads when it changes', async () => {
  const p = page('g1:0', bundle('g1:0'), undefined, `${FILE}|abc123|||g1:0`);
  await p.flush();
  assert.equal(p.sockets.length, 0);
  assert.equal(p.fetches.length, 1);
  assert.equal(p.logs.filter(line => line.includes('cannot reach')).length, 0, 'no daemon to reach');
  p.disk = bundle('g1:1');
  await p.advance(30_000);
  assert.equal(p.reloads, 1);
});

test('a page with no url at the reload cap checks again once the oldest reload leaves the 60 s', async () => {
  // Without a socket no event wakes a capped page; its own timer must, and the cap still holds.
  const p = page('g1:0', bundle('g1:1'), win => win.sessionStorage.setItem('nscriptHmr:reloads', JSON.stringify([999_000, 999_500])), `${FILE}|abc123|||g1:0`);
  await p.flush();
  assert.equal(p.reloads, 0);
  assert.equal(p.state().state, 'reload-only');
  await p.advance(58_000);
  assert.equal(p.fetches.length, 1, 'no checks while the cap holds');
  assert.equal(p.reloads, 0);

  await p.advance(2_000);
  assert.equal(p.fetches.length, 2, 'it checks again with no socket event or new boot');
  assert.equal(p.reloads, 1, 'the oldest reload left the 60 s');
});

test('a page behind the records the server keeps is told to reload, and reloads onto its script', async () => {
  // HmrServer drops old records past its bound; a hello in the page's own gen then names a reload.
  const p = page('g1:3', bundle('g1:150'));
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g1', reload: 'behind the replay window' });
  await p.flush();
  assert.deepEqual(p.applied, []);
  assert.equal(p.reloads, 1);
  assert.ok(p.logs.includes('[nscript-hmr] reloading: behind the replay window'), p.logs.join(' | '));
});

test('a script behind the page (same gen, lower seq) never reloads it backwards', async () => {
  // The copy lags: the page took records 1 and 2 live while its script is still g1:0.
  const p = page('g1:0');
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 1, js: 'one', methods: 1 }, { seq: 2, js: 'two', methods: 1 }] });
  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 3, reload: 'skin' }] });
  await p.flush();
  assert.equal(p.reloads, 0, 'the script holds older code than the page runs');
  assert.equal(p.state().state, 'waiting');

  // Once the copy holds the record that needs the reload, the page reloads onto it.
  p.disk = bundle('g1:3');
  await p.advance(30_000);
  assert.equal(p.reloads, 1);
});

test('a record that needs a reload is never applied: the page reloads onto its script', async () => {
  const p = page('g1:0', bundle('g1:1'));
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 1, reload: 'load-time Program.Main' }] });
  await p.flush();
  assert.deepEqual(p.applied, []);
  assert.equal(p.reloads, 1);
  assert.equal(p.state().seq, 0);
  assert.ok(p.logs.includes('[nscript-hmr] reloading: load-time Program.Main'), p.logs.join(' | '));

  // The reloaded page says why it reloaded, once.
  const reason = p.win.sessionStorage.getItem(`nscriptHmr:${FILE}:reason`);
  assert.equal(reason, 'load-time Program.Main');
  const next = page('g1:1', bundle('g1:1'), win => win.sessionStorage.setItem(`nscriptHmr:${FILE}:reason`, reason));
  assert.ok(next.logs.includes('[nscript-hmr] reloaded: load-time Program.Main'), next.logs.join(' | '));
  assert.equal(next.state().lastReload, 'load-time Program.Main');
  assert.equal(next.win.sessionStorage.getItem(`nscriptHmr:${FILE}:reason`), null, 'said once');
});

test('a self-check that gets no script (404, a fallback page, no boot call) waits and never reloads', async () => {
  for (const [what, response] of [
    ['404', { status: 404, text: '' }],
    ['html', { type: 'text/html', text: '<!doctype html><p>index</p>' }],
    ['no boot call', { text: '(function(){window.__nsPatch;})();' }],
  ]) {
    const p = page('g1:0');
    p.response = response;
    p.sockets[0].open();
    p.sockets[0].deliver({ gen: 'g2', reload: 'new build' });
    await p.flush();
    assert.equal(p.reloads, 0, what);
    assert.equal(p.state().state, 'waiting', what);
    assert.ok(p.logs.some(line => line.includes('self-check failed: ' + (what === '404' ? '404' : what))), what + ': ' + p.logs.join(' | '));

    // The server serves the script again: the page reloads onto it.
    p.response = null;
    p.disk = bundle('g2:0');
    await p.advance(30_000);
    assert.equal(p.reloads, 1, what);
  }
});

test('the reload cap counts only the last 60 s', async () => {
  // Two reloads 61 s ago no longer count: the page reloads, and records the new time.
  const p = page('g1:0', bundle('g2:0'), win => win.sessionStorage.setItem('nscriptHmr:reloads', JSON.stringify([939_000, 939_500])));
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g2', reload: 'new build' });
  await p.flush();
  assert.equal(p.reloads, 1);
});

test('a notice logs once per text, a good build ends it, and the page never checks or reloads', async () => {
  // S6: a red build tells the page why its code is not current; the page keeps running.
  const p = page('g1:0');
  p.sockets[0].open();
  p.sockets[0].deliver({ gen: 'g1' });
  p.sockets[0].deliver({ note: 'build failed: A.cs(1,2): error CS1002: ; expected' });
  p.sockets[0].deliver({ note: 'build failed: A.cs(1,2): error CS1002: ; expected' });
  p.sockets[0].deliver({ note: 'build failed: A.cs(3,4): error CS0029: no' });
  await p.flush();
  const notices = () => p.logs.filter(line => line.includes('build failed') || line.includes('build ok'));
  assert.deepEqual(notices(), ['[nscript-hmr] build failed: A.cs(1,2): error CS1002: ; expected', '[nscript-hmr] build failed: A.cs(3,4): error CS0029: no']);
  assert.equal(p.state().note, 'build failed: A.cs(3,4): error CS0029: no');
  assert.equal(p.state().state, 'connected');

  p.sockets[0].deliver({ note: '' });
  p.sockets[0].deliver({ note: '' });
  assert.equal(p.state().note, null);
  assert.equal(notices().filter(line => line.endsWith('build ok')).length, 1, 'an end with no notice says nothing');

  // A patch that arrives with a notice standing ends it quietly.
  p.sockets[0].deliver({ note: 'NEEDS BUILD App: new file; run dotnet build -p:NScriptWatch=true' });
  p.sockets[0].deliver({ gen: 'g1', records: [{ seq: 1, js: 'one', methods: 1 }] });
  await p.flush();
  assert.equal(p.state().note, null);
  assert.deepEqual(p.applied, ['one']);
  assert.equal(p.fetches.length, 0, 'a notice never checks the script');
  assert.equal(p.reloads, 0);
});

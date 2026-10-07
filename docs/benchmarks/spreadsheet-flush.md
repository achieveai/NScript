# Spreadsheet binding-flush benchmark

Reference numbers for `node bench-spreadsheet.mjs` (headless Chromium, min of 5).
Time = property changes + binding flush + forced style/layout, measured without a
MutationObserver. DOM writes come from a separate counting pass.
Re-run after any framework, compiler, or demo-app change; update when a change is meant to move it.
Gated: write counts and the batched/sync speedup. Absolute ms is host-dependent and only warns.

Machine-readable baseline: `spreadsheet-flush.baseline.json` (checked by `node bench-spreadsheet.mjs --check`,
rewritten by `--update`). Last recorded: 2026-10-06, commit 95934123+.

| Operation | Rows | sync ms | batched ms | speedup | sync writes | batched writes |
|---|---|---|---|---|---|---|
| A1 + 1 | 50 | 3.8 | 3.5 | 1.1x | 111 | 111 |
| All A + 1 | 50 | 27.7 | 20.5 | 1.4x | 3068 | 360 |
| Randomize A,B | 50 | 48.5 | 34.0 | 1.4x | 6688 | 448 |
| A1 + 1 | 100 | 5.5 | 5.4 | 1.0x | 210 | 210 |
| All A + 1 | 100 | 84.9 | 66.9 | 1.3x | 11111 | 712 |
| Randomize A,B | 100 | 158.6 | 127.9 | 1.2x | 22749 | 857 |
| A1 + 1 | 200 | 11.9 | 12.6 | 0.9x | 410 | 410 |
| All A + 1 | 200 | 301.5 | 246.5 | 1.2x | 42208 | 1411 |
| Randomize A,B | 200 | 633.7 | 490.7 | 1.3x | 84236 | 1668 |

Headed run in the desktop browser pane on the same day: 1.3x to 1.5x on the
`All A + 1` and `Randomize A,B` rows, write counts identical.
`A1 + 1` is a single-cell edit touching the same nodes either way; treat it as noise.

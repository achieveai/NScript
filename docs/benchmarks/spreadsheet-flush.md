# Spreadsheet binding-flush benchmark

Reference numbers for `node bench-spreadsheet.mjs` (headless Chromium, median of 3).
Time = property changes + binding flush + forced style/layout, measured without a
MutationObserver. DOM writes come from a separate counting pass.
Re-run after any framework, compiler, or demo-app change; update when a change is meant to move it.

Machine-readable baseline: `spreadsheet-flush.baseline.json` (checked by `node bench-spreadsheet.mjs --check`,
rewritten by `--update`). Last recorded: 2026-10-06, commit 6e00773c.

| Operation | Rows | sync ms | batched ms | speedup | sync writes | batched writes |
|---|---|---|---|---|---|---|
| A1 + 1 | 50 | 4.8 | 5.3 | 0.9x | 110 | 110 |
| All A + 1 | 50 | 60.5 | 48.3 | 1.3x | 3084 | 362 |
| Randomize A,B | 50 | 47.7 | 33.9 | 1.4x | 6669 | 447 |
| A1 + 1 | 100 | 6.1 | 6.2 | 1.0x | 210 | 210 |
| All A + 1 | 100 | 83.7 | 67.8 | 1.2x | 11108 | 709 |
| Randomize A,B | 100 | 156.9 | 118.7 | 1.3x | 22754 | 863 |
| A1 + 1 | 200 | 14.7 | 20.0 | 0.7x | 410 | 410 |
| All A + 1 | 200 | 311.4 | 245.2 | 1.3x | 42210 | 1409 |
| Randomize A,B | 200 | 619.5 | 477.7 | 1.3x | 84636 | 1639 |

Headed run in the desktop browser pane on the same day: 1.3x to 1.5x on the
`All A + 1` and `Randomize A,B` rows, write counts identical.
`A1 + 1` is a single-cell edit touching the same nodes either way; treat it as noise.

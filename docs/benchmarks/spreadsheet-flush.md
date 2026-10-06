# Spreadsheet binding-flush benchmark

Reference numbers for `node bench-spreadsheet.mjs` (headless Chromium, median of 3).
Time = property changes + binding flush + forced style/layout, measured without a
MutationObserver. DOM writes come from a separate counting pass.
Re-run after any framework, compiler, or demo-app change; update when a change is meant to move it.

Last recorded: 2026-10-06, commit (pending).

| Operation | Rows | sync ms | batched ms | speedup | sync writes | batched writes |
|---|---|---|---|---|---|---|
| A1 + 1 | 50 | 4.4 | 4.6 | 1.0x | 110 | 110 |
| All A + 1 | 50 | 31.4 | 22.8 | 1.4x | 3084 | 362 |
| Randomize A,B | 50 | 50.8 | 41.3 | 1.2x | 6669 | 447 |
| A1 + 1 | 100 | 6.4 | 7.0 | 0.9x | 210 | 210 |
| All A + 1 | 100 | 90.0 | 72.8 | 1.2x | 11108 | 709 |
| Randomize A,B | 100 | 181.2 | 149.7 | 1.2x | 22754 | 863 |
| A1 + 1 | 200 | 12.9 | 16.2 | 0.8x | 410 | 410 |
| All A + 1 | 200 | 320.0 | 251.5 | 1.3x | 42210 | 1409 |
| Randomize A,B | 200 | 649.4 | 499.6 | 1.3x | 84636 | 1639 |

Headed run in the desktop browser pane on the same day: 1.3x to 1.5x on the
`All A + 1` and `Randomize A,B` rows, write counts identical.
`A1 + 1` is a single-cell edit touching the same nodes either way; treat it as noise.

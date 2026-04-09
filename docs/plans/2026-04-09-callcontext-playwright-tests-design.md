# CallContext Playwright Browser Tests — Design

## Goal

Validate the CallContext feature end-to-end in a real browser environment using Playwright.
Tests are added to the existing `e2e-todo-tests.js` in `Test/Framework/TestWebApplication/`.

## Scope

| Area | Tests | What's validated |
|------|-------|-----------------|
| Root context on DOM events | 2 | Click creates CallContext with traceId/spanId/actionId; each click creates new root |
| Async propagation | 2 | Context survives async boundaries; null when idle |
| XHR traceparent injection | 2 | Outgoing XHR carries W3C traceparent header |
| TodoApp integration | 2 | Add todo + folder switch work with CallContext active (regression safety) |

## Approach

- **Extend `e2e-todo-tests.js`** — same file, new section at the bottom
- **`page.evaluate()`** to inspect `CallContext.Current` from JS runtime
- **`page.route()`** to intercept XHR and verify traceparent header
- **Graceful skip** when generated JS doesn't include CallContext yet

### Graceful Skip Pattern

```js
const hasCallContext = await page.evaluate(() =>
    typeof Sunlight !== 'undefined' &&
    typeof Sunlight.Framework !== 'undefined' &&
    typeof Sunlight.Framework.CallContext !== 'undefined');
if (!hasCallContext) { console.log('  SKIP (CallContext not in JS)'); return; }
```

## Test Details

### 1. Click creates CallContext
- Click a todo item
- `page.evaluate()` → check `CallContext.Current` is not null
- Verify: traceId (32 hex chars), spanId (16 hex chars), actionId ≥ 0

### 2. Each click gets new root context
- Click todo A → capture traceId
- Click todo B → verify new traceId ≠ old traceId

### 3. Context null when idle
- Fresh page load, no interaction
- Verify `CallContext.Current` is null

### 4. Context survives async boundary
- Click todo → trigger async operation (add new todo via input + Enter)
- After async completes, verify CallContext.Current still has same traceId

### 5. XHR carries traceparent header
- `page.route('**/test-xhr')` to capture request headers
- Click todo → `page.evaluate()` creates XMLHttpRequest to `/test-xhr`
- Verify captured header has traceparent matching active context

### 6. Traceparent format is W3C-compliant
- Verify header matches `^00-[0-9a-f]{32}-[0-9a-f]{16}-01$`

### 7. Add todo with CallContext active
- Full add-todo flow (type title, press Enter)
- Validates EventBinder hooks don't break normal event dispatch

### 8. Folder switch with CallContext active
- Click different folders, verify correct todos display
- Validates EventBinder save/restore doesn't corrupt event handling

## Dependencies

- Generated JS must include CallContext (requires nscript.exe fix for framework rebuild)
- Until then, tests gracefully skip with console message

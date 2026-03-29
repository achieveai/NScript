# ADR 0024: Deduplicate Structurally Identical Functions After Minification

- Status: Accepted
- Date: 2026-03-29
- Deciders: NScript maintainers
- Technical Area: JavaScript output optimization, minification, code size reduction

## Context

After the minifier renames all identifiers (ADR-0021) and non-virtual methods are devirtualized to static functions (ADR-0023), many generated functions end up with identical JavaScript bodies even though they originated from different C# methods on different types.

Consider two unrelated C# types:

```csharp
class Customer : ObservableObject {
    string Name { get; set; }        // backing field: _name
    string Address { get; set; }     // backing field: _address
}

class Product : ObservableObject {
    string Title { get; set; }       // backing field: _title
    string Category { get; set; }    // backing field: _category
}
```

After devirtualization (ADR-0023), these produce four static getter functions:

```javascript
// Before minification — all different
function Customer_get_name(this_) { return this_._name; }
function Customer_get_address(this_) { return this_._address; }
function Product_get_title(this_) { return this_._title; }
function Product_get_category(this_) { return this_._category; }
```

After minification renames fields and parameters:

```javascript
// After minification — structurally identical!
function a(b) { return b.c; }    // was Customer_get_name
function d(b) { return b.e; }    // was Customer_get_address
function f(b) { return b.g; }    // was Product_get_title
function h(b) { return b.i; }    // was Product_get_category
```

These functions have different field names (`c`, `e`, `g`, `i`) so they are NOT identical yet. But now consider that the minifier assigns the same short name to fields that don't conflict (different types, different scopes). If `_name` and `_title` both get minified to `c`:

```javascript
function a(b) { return b.c; }    // Customer_get_name
function f(b) { return b.c; }    // Product_get_title — same body!
```

These two functions are now **structurally identical**. Emitting both is wasteful — one can be eliminated and all references redirected to the survivor.

This pattern is pervasive. Simple getters, setters, event handlers, and utility methods across dozens of types frequently collapse to identical bodies after minification. For a large application, this deduplication can eliminate hundreds of functions.

Relevant paths:

- minified name assignment: [Sources/Compiler/NScript.JST/IdentifierMinifiedNamer.cs](../../Sources/Compiler/NScript.JST/IdentifierMinifiedNamer.cs)
- function expression JST: [Sources/Compiler/NScript.JST/Expressions/FunctionExpression.cs](../../Sources/Compiler/NScript.JST/Expressions/FunctionExpression.cs)
- JS output: [Sources/Compiler/NScript.JST/JSWriter.cs](../../Sources/Compiler/NScript.JST/JSWriter.cs)

## Decision Drivers

- Reduce output size — identical function bodies are pure waste
- The optimization is only possible after minification — before renaming, the functions have distinct names and distinct bodies
- Must be semantically safe — two functions can only merge if they are truly interchangeable at every call site
- Should be automatic — no annotation or manual intervention required

## Options Considered

### Option 1: No deduplication (emit all functions as-is)

Skip post-minification comparison. Every devirtualized function gets its own entry in the output.

Pros:

- Simplest — no comparison or rewriting needed
- 1:1 correspondence between C# methods and JS functions (easier debugging)

Cons:

- Wastes output size on identical function bodies
- The waste grows with codebase size — more types means more duplicate simple accessors

### Option 2: Deduplicate structurally identical functions after minification (chosen)

After minification assigns final names, compare function bodies. Functions with identical structure (same parameter count, same body text after name resolution) are merged: only one copy is emitted, and all references to the duplicates are redirected to the surviving function.

Pros:

- Significant size reduction — simple accessors across many types collapse to shared implementations
- Fully automatic — no source annotation needed
- Semantically safe — if two functions have identical minified bodies, they are interchangeable by definition (they do exactly the same thing with the same names)

Cons:

- Requires a post-minification comparison pass over all emitted functions
- Redirecting references requires updating identifier bindings after the initial assignment
- Makes the mapping from C# method to JS function non-trivial for debugging (multiple C# methods map to one JS function)

## Decision

After minification (ADR-0021) assigns final names to all identifiers, NScript performs a **structural deduplication pass** over all emitted function bodies.

### How It Works

1. **Serialize** — each function's body is serialized to its final JavaScript text using the minified identifier names. This produces the exact string that would appear in the output.

2. **Hash and group** — functions are grouped by their serialized body text. Functions with identical parameter counts and identical body strings form an equivalence class.

3. **Select survivor** — within each equivalence class, one function is chosen as the canonical representative (typically the one with the most references, to maximize the benefit of a short name).

4. **Redirect references** — all identifier references to eliminated functions are repointed to the surviving function's identifier. This means every call site, property assignment, or function-pointer reference that previously pointed to a duplicate now points to the survivor.

5. **Emit** — only the surviving function is written to the output. The eliminated functions produce no output. Their identifiers have been redirected, so no dangling references remain.

### What Can Be Deduplicated

The deduplication applies to any function expression or function declaration in the generated JST, including:

- **Devirtualized instance methods** (ADR-0023) — the most common source of duplicates, since simple getters/setters across many types often collapse
- **Static utility methods** — helper functions with identical logic on different types
- **Template getter functions** — Razor template getters (ADR-0018) that access the same field pattern
- **Factory functions** — constructors with identical initialization patterns

### What Cannot Be Deduplicated

- **Virtual methods on prototypes** — these are accessed by name on the prototype object, not by function reference. Two prototype methods with the same body but on different types cannot be merged because they are accessed as `obj.methodName()` where `methodName` is type-specific.
- **Functions with closure captures** — if a function closes over a variable from its enclosing scope, the body text may look identical but the captured variable binds differently. However, devirtualized static functions don't capture closures (they receive everything as parameters), so this is rarely an issue in practice.

### Why This Is Safe

Two functions with identical minified bodies are provably interchangeable:

- They have the same parameter count (checked before comparison)
- Every identifier in the body has been resolved to its final minified name (ADR-0021)
- The minifier guarantees that two identifiers with the same final name refer to the same runtime entity within the same scope
- Therefore, `function(b) { return b.c; }` does exactly the same thing regardless of which C# method it originated from — `b` is the instance parameter, `c` is the field. If two getters access a field that the minifier named `c`, they return the same field on whatever object is passed.

## Consequences

Positive:

- Measurable output size reduction — in large applications with many model/viewmodel types, hundreds of simple accessor functions collapse to a handful of shared implementations
- The optimization compounds with devirtualization (ADR-0023) — converting instance methods to static functions with explicit parameters makes more functions structurally comparable
- Zero source-level impact — C# authors write normal code; the optimizer handles deduplication transparently
- No runtime overhead — this is a compile-time output optimization, not a runtime indirection

Negative:

- Source maps and debugging become harder — multiple C# methods map to one JS function. Stack traces show the survivor's name, not the original method
- The comparison pass adds compilation time proportional to the number of emitted functions (mitigated by hashing — only functions with the same hash are compared textually)
- The optimization is order-dependent on minification — different minification strategies (name assignment order) could produce different deduplication results, making output non-deterministic across compiler versions if the minifier changes

## References

- Depends on: ADR-0021 (resolved identifiers — all names must be finalized before comparison), ADR-0023 (devirtualization — produces the static functions that are the primary dedup candidates)
- Related: ADR-0022 (demand-driven conversion — dead code elimination removes unreachable functions before dedup runs on the survivors)

# ADR 0023: Devirtualize Non-Virtual Methods and Inline Trivial Accessors

- Status: Accepted
- Date: 2026-03-29
- Deciders: NScript maintainers
- Technical Area: JavaScript code generation, runtime performance optimization

## Context

C# instance methods are normally emitted as prototype methods in JavaScript — called via `obj.method()` which traverses the prototype chain. This has two costs:

1. **Prototype lookup overhead** — every call walks the prototype chain at runtime. For hot methods called millions of times (property getters in binding loops), this is measurable.
2. **Method identity** — prototype methods are shared across instances, which interacts with JavaScript engine optimizations (hidden classes, inline caches) in complex ways.

Additionally, C# properties generate getter and setter methods. For simple auto-properties (`{ get; set; }`) that just read/write a backing field, the getter method adds a function call overhead with zero logic — `get_name()` does nothing more than `return this._name`.

NScript can optimize both patterns at compile time because it has full type information from Cecil and knows which methods are virtual (require prototype dispatch) and which are not.

Relevant paths:

- instance-as-static flag: [Sources/Compiler/NScript.Converter/TypeSystemConverter/RuntimeScopeManager.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/RuntimeScopeManager.cs) (`ImplementInstanceAsStatic`)
- method classification: [Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs](../../Sources/Compiler/NScript.Converter/TypeSystemConverter/MethodConverter.cs) (`IsInstanceStatic`, `HasStaticImplementation`, `IsGlobalStaticImplementation`)
- call-site conversion: [Sources/Compiler/NScript.Converter/ExpressionsConverter/MethodCallExpressionConverter.cs](../../Sources/Compiler/NScript.Converter/ExpressionsConverter/MethodCallExpressionConverter.cs)
- keep-instance attribute: `[KeepInstanceUsage]` custom attribute

## Decision Drivers

- Reduce prototype chain lookups for non-polymorphic method calls
- Eliminate function call overhead for trivial property accessors
- Enable the minifier to treat the instance parameter as a regular local (shorter name, no `this` keyword)
- Maintain correct behavior for virtual/interface methods that require prototype dispatch
- Keep the optimization transparent to C# authors — no source-level annotation required

## Options Considered

### Option 1: Emit all instance methods on the prototype (no optimization)

Every instance method becomes `Type.prototype.method = function() { ... }`. All calls go through `this.method()`.

Pros:

- Simple, matches C# semantics directly
- No analysis needed

Cons:

- Prototype lookup on every call, even for methods that are never overridden
- Trivial getters still incur function call overhead
- `this` keyword cannot be renamed by the minifier

### Option 2: Devirtualize non-virtual methods to static functions, inline trivial accessors (chosen)

Non-virtual instance methods are emitted as static functions with an explicit first parameter (`this_`). Trivial property accessors (simple field read/write) are inlined at the call site to direct field access. Virtual methods remain on the prototype.

Pros:

- Static functions avoid prototype chain lookup
- The instance parameter (`this_`) is a regular identifier — the minifier can shorten it
- Trivial getter inlining eliminates function call overhead entirely
- Virtual methods remain correct — only non-virtual methods are devirtualized

Cons:

- Static functions are slightly larger (explicit parameter) than prototype methods
- Call sites pass the instance explicitly: `method(obj)` instead of `obj.method()`
- Inlining decisions are made at compile time — cannot adapt to runtime profiling
- Some interop scenarios require instance methods; `[KeepInstanceUsage]` is the escape hatch

## Decision

In optimized (Release) builds, NScript applies two method representation optimizations:

### 1. Non-Virtual Instance-to-Static Devirtualization

When `RuntimeScopeManager.ImplementInstanceAsStatic` is `true` (set by `scriptGenerateSettings.optimize`), non-virtual instance methods are converted to static functions with the instance as the first parameter.

**Criteria for devirtualization** (`MethodConverter.IsInstanceStatic`):

A method is devirtualized when ALL of these hold:
- The method has `this` (is an instance method)
- `ImplementInstanceAsStatic` is enabled OR the declaring type is a value type/enum
- The declaring type is not generic
- The declaring type is not an interface
- The method does not have `[KeepInstanceUsage]` attribute

**What changes:**

| Aspect | Before (prototype) | After (static) |
| --- | --- | --- |
| Declaration | `Type.prototype.method = function() { ... }` | `function Type_method(this_) { ... }` |
| Call site | `obj.method(arg1, arg2)` | `Type_method(obj, arg1, arg2)` |
| `this` reference | `this` keyword (not minifiable) | `this_` parameter (minifiable) |
| Scope | Per-type prototype | Global or type scope |
| Virtual dispatch | Yes (prototype chain) | No (direct call) |

**What is NOT devirtualized:**

- Virtual methods (`virtual`, `override`, `abstract`) — must stay on prototype for correct dispatch
- Interface methods — callers may hold an interface reference
- Generic type methods — the generic parameter affects the prototype identity
- Methods with `[KeepInstanceUsage]` — explicitly opted out

### 2. Trivial Accessor Inlining

Property getters and setters that simply read or write a backing field are not emitted as methods. Instead, call sites are rewritten to direct field access.

**What qualifies as trivial:**

A getter is trivial if its IL body contains only: `ldarg.0`, `ldfld <field>`, `ret` (with optional `nop`, `stloc.0`, `ldloc.0`, `br.s` from debug builds). The compiler detects this by analyzing the getter's IL instructions via Cecil.

**What changes:**

| Aspect | Before (method call) | After (inlined) |
| --- | --- | --- |
| Read | `obj.get_name()` | `obj._name` (direct field access) |
| Write | `obj.set_name(val)` | `obj._name = val` (direct field assignment) |
| Generated method | `function get_name(this_) { return this_._name; }` | No method emitted |
| Identifier | Method identifier registered and resolved | Field identifier used directly |

**Consequences for code generators:**

Because trivial getters may not exist as methods at runtime, code generators (including the Razor template pipeline) must check whether a property getter is inlined before emitting a call to it. The `GraphDescriptorJSTEmitter` uses `TryFindBackingFieldOnType()` to detect this: if the getter's IL is a simple field load, the emitter generates `dc.fieldName` instead of `dc.get_propName()`.

If a code generator emits `dc.get_propName()` for an inlined getter, the call will fail at runtime because the method does not exist. This is a correctness requirement, not an optimization preference.

### Interaction Between the Two Optimizations

The optimizations compose:

1. A non-virtual, non-trivial method → devirtualized to static function
2. A non-virtual, trivial getter → inlined to field access (no function at all)
3. A virtual method → stays on prototype regardless of complexity
4. A virtual, trivial getter → stays on prototype (virtual dispatch required)

The demand-driven walk (ADR-0022) and zero-reference elimination further prune the output: if a devirtualized static function ends up with zero call sites (e.g., all callers were inlined), its identifier has zero references and it is dropped from the output.

## Consequences

Positive:

- Non-virtual method calls avoid prototype chain traversal — measurable improvement in tight binding loops
- The `this` parameter becomes a regular minifiable identifier — typically 1-2 characters in Release builds vs. the fixed 4-character `this` keyword
- Trivial getters produce zero runtime overhead — direct field access is the fastest possible property read
- Combined with zero-reference elimination (ADR-0022), inlined methods that have no remaining callers are automatically dropped from output

Negative:

- Call sites for devirtualized methods differ from standard JavaScript patterns — `fn(obj, args)` instead of `obj.fn(args)`. This makes generated JS harder to read and debug.
- Code generators must detect trivial getters and emit field access instead of method calls. Failing to do so produces a runtime error (method not found). This is documented in CLAUDE.md and ADR-0021.
- The `[KeepInstanceUsage]` escape hatch must be used for interop scenarios where external JavaScript expects prototype methods (e.g., callbacks that receive `this` binding)
- Inlining decisions are compile-time and global — a getter that is trivial in the base class but overridden in a subclass stays on the prototype (virtual), but one that is trivial and non-virtual is always inlined, even if a future subclass might want to override it

## References

- Related: ADR-0021 (resolved identifiers — inlined getters affect how identifiers are resolved), ADR-0022 (demand-driven conversion — zero-reference elimination drops unused devirtualized functions)
- Optimization flag: `scriptGenerateSettings.optimize` passed to `RuntimeScopeManager` constructor
- Escape hatch: `[KeepInstanceUsage]` attribute prevents devirtualization for specific methods

> **C# 9–13 status** lives in [`docs/language/csharp9-13-status.md`](docs/language/csharp9-13-status.md). This file tracks residual C# 8 items only.

- ~~Readonly members~~ (Free)
- Default interface methods
- Pattern matching enhancements:
    - Switch expressions
    - Property patterns
    - Tuple patterns
    - Positional patterns
- Using declarations
- ~~Static local functions~~ (Free)
- ~~Disposable ref structs~~ (Irrelevant to JS target)
- Nullable reference types
- Asynchronous streams
- ~~Indices and ranges~~ (Phase F6 — see [`Lang8IndexRangeTests.cs`](Test/Framework/RealScript/Lang8IndexRangeTests.cs))
- ~~Null-coalescing assignment~~
- ~~Unmanaged constructed types~~ (Irrelevant to JS target)
- ~~Stackalloc in nested expressions~~ (Irrelevant to JS target)
- ~~Enhancement of interpolated verbatim strings~~ (Free)

Bug fixes:
1. Generic get only properties doesn't work
2. Fix throw expressions code generation
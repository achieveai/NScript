# Structured Client Logging

The `Sunlight.Framework.Logger` static class is the entry point for all client-side logging in NScript applications. In WI-11 the original console-only emitter was refactored into a sink-based pipeline that preserves the legacy API while adding:

- Named/category loggers via `Logger.ForCategory("MyApp.MyComponent")`
- Pluggable `ILogSink` implementations (`ConsoleSink` default, `HttpLogSink` for server ingestion)
- Structured property bags carried through as stable JSON
- Automatic CallContext correlation (`traceId`, `spanId`, `actionId`, …)
- `Trace` and `Debug` levels that are **stripped from Release builds at compile time** by `[Conditional("DEBUG")]`

This document describes how to use the pipeline, the full JSON schema of emitted events, and how to wire the built-in HTTP transport into Serilog / NLog on the server side.

---

## Quick Start

### Uncategorized logs (legacy API, unchanged)

```csharp
using Sunlight.Framework;

Logger.Info("user clicked Save");
Logger.Warn("retrying request");
Logger.Error("upload failed");
```

### Category loggers

Prefer category loggers for anything in a reusable component. Cache the returned instance at static-init time — `ForCategory` returns the same `NamedLogger` for the same category string so there is no per-call allocation overhead:

```csharp
public static class ListView
{
    private static readonly NamedLogger log = Logger.ForCategory("TodoApp.ListView");

    public static void AddItem(string id)
    {
        log.Info("item added", new string[] { "itemId", id });
    }
}
```

### Structured properties

`string[]` is intentional — it is a flat key/value array: `[k1, v1, k2, v2, …]`. This avoids NScript's minification trap: caller-supplied C# objects would be emitted with minified field names in the generated JS, which would produce unreadable keys in the JSON payload.

```csharp
log.Info("api call complete", new string[]
{
    "method", "POST",
    "path", "/api/items",
    "status", "200",
    "durationMs", "42"
});
```

### Trace and Debug (stripped in Release)

```csharp
log.Trace("rendering item list");   // stripped in Release
log.Debug("applying filter");        // stripped in Release
log.Info("filter applied");          // always emitted
```

See [Compile-Time Stripping](#compile-time-stripping) for the full rules.

---

## Sinks

### Default: ConsoleSink

Writes each event as a single JSON line to the appropriate browser console method (`console.error` / `console.warn` / `console.log`). Installed lazily on the first log call if the application has not otherwise configured sinks.

### HttpLogSink — batched HTTP transport

Register an HTTP sink once at application startup. The constructor parameters:

| Parameter | Purpose |
|-----------|---------|
| `endpoint` | URL to POST batches to. |
| `batchSize` | Flush when the queue reaches this many events. |
| `flushIntervalMs` | Flush on this timer interval regardless of queue size. |
| `maxQueueSize` | Oldest events are dropped above this threshold. |
| `timer` | An `IWindowTimer` — use `new WindowTimer()` in app code, or a test double in tests. |

```csharp
Logger.AddSink(new HttpLogSink(
    "/ClientLogs.ashx",
    batchSize: 20,
    flushIntervalMs: 5000,
    maxQueueSize: 500,
    timer: new WindowTimer()));
```

#### Flush triggers

An HTTP batch flushes on any of:

1. Queue length reaches `batchSize`.
2. Timer fires every `flushIntervalMs`.
3. `Logger.Flush()` is called explicitly.
4. The page is transitioning away (`pagehide` / `beforeunload`). The sink uses `navigator.sendBeacon` with a `Blob` of type `application/json` so pending events survive even forced navigations.

#### Overflow behavior

When the queue exceeds `maxQueueSize` the sink drops the **oldest** events (keeping the most recent ones — typically the most useful when triaging a crash) and stamps the running drop count into the next payload envelope. The drop count is in the payload, not an HTTP header, because `navigator.sendBeacon` cannot set custom headers and custom headers would trigger CORS preflight on the normal XHR path.

#### Lifecycle

`Logger.RemoveSink(sink)` calls `sink.Detach()` — `HttpLogSink.Detach` clears the timer, removes the unload listener, and performs a best-effort final `sendBeacon` flush of any still-queued events.

### Writing a custom sink

```csharp
public class MyCustomSink : ILogSink
{
    public void Handle(LogEvent evt) { /* enqueue / forward / etc. */ }
    public void Flush()              { /* flush buffered events */ }
    public void Detach()             { /* clean up resources */ }
}

Logger.AddSink(new MyCustomSink());
```

`Logger` wraps every `Handle`/`Flush`/`Detach` call in a try/catch so a faulty sink cannot break its peers or the original caller (important: `Logger.Error(...)` is called from exception handlers and must never throw).

---

## JSON Schema

### Individual event

```json
{
  "id":       "abcdef0123456789",
  "ts":       "2026-04-17T18:00:00.000Z",
  "level":    "INFO",
  "cat":      "TodoApp.ListView",
  "msg":      "Item added",
  "props":    { "itemId": "42", "source": "api" },
  "traceId":       "a1b2c3d4e5f6a7b8a1b2c3d4e5f6a7b8",
  "spanId":        "a1b2c3d4e5f6a7b8",
  "parentSpanId":  "f1e2d3c4b5a6f7e8",
  "actionId":      5,
  "depth":         1
}
```

- `id` (WI-99) is the per-emit runtime-unique 64-bit hex id. Always
  the **first** field so the server-side WebSocket ACK path can
  streaming-parse it without building the full object. See
  "WebSocket Transport + Per-Event ACK" below for the semantics.
- `level` is the string form of `LogLevel`: `"TRACE" | "DEBUG" | "INFO" | "WARN" | "ERROR"`.
- `cat` is omitted when the event came through the static facade (i.e. uncategorized).
- `props` is built from the flat `string[]`; omitted when the caller passed none.
- The `traceId` / `spanId` / `parentSpanId` / `actionId` / `depth` group is included together when a `CallContext` was active at emit time; otherwise the whole group is omitted.

### HTTP batch envelope

```json
{
  "events":  [ /* array of events */ ],
  "dropped": 0
}
```

`dropped` is a running counter of events that were evicted from the queue because it exceeded `maxQueueSize`. Always present.

---

## Compile-Time Stripping

`Logger.Trace` and `Logger.Debug` (both the static facade and the `NamedLogger` instance methods) are tagged with `[Conditional("DEBUG")]`. When the **caller's** NScript project is compiled without the `DEBUG` symbol (i.e. Release config), Roslyn removes every call to these methods — including argument evaluation — from the bound tree before it reaches the NScript AST serializer. Result: zero runtime overhead in Release.

### Important: per-assembly

`[Conditional]` strips at the caller's compilation, not the framework's. If a referenced NScript library was built with `DEBUG` defined, its own internal Trace/Debug calls remain even when your app is built in Release. This is standard .NET behavior.

### NScript project template setup

NScript's default project templates already set the right constants:

```xml
<PropertyGroup Condition=" '$(Configuration)' == 'Debug' ">
  <DefineConstants>DEBUG;TRACE</DefineConstants>
</PropertyGroup>
<PropertyGroup Condition=" '$(Configuration)' == 'Release' ">
  <DefineConstants>TRACE</DefineConstants>
</PropertyGroup>
```

So stripping works out of the box — no additional build configuration is required.

### Why Trace/Debug are not on `ILogger`

C# does not allow `[Conditional]` on interface methods. If `ILogger` exposed `Trace`/`Debug`, a call through the interface reference would silently bypass stripping — a subtle bug. `ILogger` intentionally exposes only `Info`/`Warn`/`Error`. Callers that want compile-time stripping should hold the concrete `NamedLogger` returned by `Logger.ForCategory(...)` rather than casting to `ILogger`.

---

## Lifecycle

`Logger` is a process-wide static. The first log call installs a default `ConsoleSink` lazily. Once the application calls `AddSink` or `ClearSinks`, the lazy default is suppressed — so after `ClearSinks()` the logger is truly silent until sinks are added again.

Typical startup wiring:

```csharp
public static class AppBootstrap
{
    public static void ConfigureLogging()
    {
        Logger.ClearSinks();                           // drop the default
        Logger.AddSink(new ConsoleSink());              // keep console output in dev
        Logger.AddSink(new HttpLogSink(                 // + ship to server
            "/ClientLogs.ashx",
            batchSize: 20,
            flushIntervalMs: 5000,
            maxQueueSize: 500,
            timer: new WindowTimer()));
        Logger.MinLevel = LogLevel.Info;
    }
}
```

---

## Server-Side Ingestion

The `TestWebApplication/ClientLogs.ashx` handler is a minimal reference — it forwards each event to `System.Diagnostics.Trace.WriteLine`. Production deployments should pipe the payload into a real logging backend. Below are wiring snippets for two common choices.

### Serilog

```csharp
public class ClientLogsHandler : IHttpHandler
{
    private static readonly ILogger log = Log.ForContext("source", "client");

    public void ProcessRequest(HttpContext context)
    {
        var body = new StreamReader(context.Request.InputStream).ReadToEnd();
        var envelope = JsonConvert.DeserializeObject<ClientLogEnvelope>(body);

        foreach (var evt in envelope.Events)
        {
            var level = evt.Level switch
            {
                "TRACE" => LogEventLevel.Verbose,
                "DEBUG" => LogEventLevel.Debug,
                "INFO"  => LogEventLevel.Information,
                "WARN"  => LogEventLevel.Warning,
                "ERROR" => LogEventLevel.Error,
                _        => LogEventLevel.Information,
            };

            log
                .ForContext("TraceId",  evt.TraceId)
                .ForContext("SpanId",   evt.SpanId)
                .ForContext("Category", evt.Cat)
                .ForContext("Props",    evt.Props, destructureObjects: true)
                .Write(level, "{Message}", evt.Msg);
        }

        context.Response.StatusCode = 204;
    }

    public bool IsReusable => true;
}
```

### NLog

```csharp
public class ClientLogsHandler : IHttpHandler
{
    private static readonly NLog.Logger log = NLog.LogManager.GetLogger("client");

    public void ProcessRequest(HttpContext context)
    {
        var body = new StreamReader(context.Request.InputStream).ReadToEnd();
        var envelope = JsonConvert.DeserializeObject<ClientLogEnvelope>(body);

        foreach (var evt in envelope.Events)
        {
            var theEvent = new LogEventInfo(
                NLog.LogLevel.FromString(MapLevel(evt.Level)),
                "client",
                evt.Msg);

            theEvent.Properties["TraceId"]  = evt.TraceId;
            theEvent.Properties["SpanId"]   = evt.SpanId;
            theEvent.Properties["Category"] = evt.Cat;
            if (evt.Props != null)
            {
                foreach (var kvp in evt.Props)
                {
                    theEvent.Properties[kvp.Key] = kvp.Value;
                }
            }

            log.Log(theEvent);
        }

        context.Response.StatusCode = 204;
    }

    private static string MapLevel(string lvl) => lvl switch
    {
        "TRACE" => "Trace",
        "DEBUG" => "Debug",
        "INFO"  => "Info",
        "WARN"  => "Warn",
        "ERROR" => "Error",
        _        => "Info",
    };

    public bool IsReusable => true;
}
```

---

## WebSocket Transport + Per-Event ACK (WI-99)

WI-99 added a WebSocket sink and a server-side ingestion stack that
share the same wire shape as the HTTP path. Two big additions to the
JSON schema and one new server project come with it.

### Schema change: per-emit event `id`

Every `LogEvent` now carries an `id` field — a per-emit runtime-unique
64-bit hex string generated by `Logger.GenerateEventId()` inside
`DispatchInternal`. It is the **first** field in every event JSON
object so the server-side WebSocket ACK path can streaming-parse it
without building the full object.

```json
{
  "id":   "abcdef0123456789",
  "ts":   "2026-05-29T00:00:00.000Z",
  "level":"INFO",
  "cat":  "TodoApp.ListView",
  "msg":  "Item added"
}
```

Semantics:

- **Per-emit**, not source-stable. Two calls from the same call site at
  two different moments produce two distinct ids. Required for
  WebSocket per-event ACK targeting — a source-stable id would collide
  across concurrent emits.
- Emitted by both `HttpLogSink` and `WebSocketLogSink`. The server
  de-dups by id even on the HTTP path (defensive against retries).

### `WebSocketLogSink` (callback-based / BYOWS)

```csharp
var ws = new WebSocketLogSink(
    isConnected:     () => mySocket.readyState == 1,
    sendPayload:     payload => mySocket.send(payload),
    batchSize:       32,
    flushIntervalMs: 2000,
    maxQueueSize:    500,
    ackTimeoutMs:    5000,
    maxRetry:        3,
    timer:           new WindowTimer());

// When YOUR onmessage handler decodes the ACK frame:
//   socket.onmessage = e => { var ids = JSON.parse(e.data).ackIds; ws.HandleAck(ids); }
// When YOUR socket transitions away from OPEN:
//   socket.onclose = () => ws.NotifyDisconnected();
```

The sink is **bring-your-own-WebSocket** by design. You supply the
`isConnected` probe and `sendPayload` callback; the sink batches,
flushes, retries on `ackTimeoutMs`, and drops events that exceed
`maxRetry`.

Public surface:

| Member | Purpose |
|---|---|
| `Handle(LogEvent)` | `ILogSink` — queue an event |
| `Flush()` | `ILogSink` — best-effort flush |
| `Detach()` | `ILogSink` — clear timer + stop accepting |
| `HandleAck(string[] ackIds)` | Consumer calls when ACK frames arrive |
| `NotifyDisconnected()` | Consumer calls on `socket.onclose` |
| `event RetryExhausted(LogEvent)` | Fires when an event exceeds `maxRetry` |
| `event DisconnectedWithPending(List<LogEvent>)` | Fires when `NotifyDisconnected` surfaces residuals |

### `WindowWebSocketTransport` (optional convenience helper)

For consumers that don't already have an application WebSocket open:

```csharp
var transport = new WindowWebSocketTransport("wss://my-host/_log/ws");
var ws = new WebSocketLogSink(
    transport.IsConnected,
    transport.Send,
    32, 2000, 500, 5000, 3,
    new WindowTimer());
transport.OnAck(ws.HandleAck);
transport.OnDisconnect(ws.NotifyDisconnected);
```

BYOWS consumers should skip this — pass your own closures to
`WebSocketLogSink` directly. The helper exists only so a consumer
without an existing WS doesn't have to hand-roll one.

### `FailoverLogSink` — WS primary + HTTP fallback

```csharp
var failover = new FailoverLogSink(wsSink, httpSink, transport.IsConnected);
Logger.AddSink(failover);
```

`Handle` routes to WS when connected, otherwise HTTP. On
`NotifyDisconnected` the failover sink drains the WS sink's residual
pending + in-flight events into the HTTP path. On `RetryExhausted`,
single events that exceeded WS retries get re-routed onto HTTP.

### `LogSinkFactory.CreateFromBootstrap` — one-line wiring

```csharp
// Host page (e.g. IngestHost.BuildIndexHtml() does this for tests):
//   window.__nscriptLogEndpoint   = '/_log';
//   window.__nscriptLogWsEndpoint = 'ws://127.0.0.1:NNNNN/_log/ws';

Logger.ClearSinks();
Logger.AddSink(LogSinkFactory.CreateFromBootstrap(new WindowTimer()));
```

The factory reads the two `window.*` globals and returns:

| `__nscriptLogEndpoint` | `__nscriptLogWsEndpoint` | Returned sink |
|---|---|---|
| set | set | `FailoverLogSink` (WS primary, HTTP fallback) |
| unset | set | `WebSocketLogSink` + `WindowWebSocketTransport` |
| set | unset | `HttpLogSink` |
| unset | unset | `ConsoleSink` |

### Wire shape: ACK frame

The server replies to each WebSocket envelope with one ACK frame:

```json
{ "ackIds": ["abcdef0123456789", "0123456789abcdef"] }
```

`ackIds` lists the events that were successfully forwarded into MEL.
Events that were dropped by the server-side dedup LRU or that hit a
per-event exception are not acked. An empty array is valid.

---

## Server-Side: `Sources/Runtime/Sunlight.Logging.Server/`

A new top-level `Sources/Runtime/` folder — conceptually distinct from
`Sources/Framework/` (browser runtime compiled to JS) and
`Sources/Compiler/` (build-time toolchain) — hosts an ASP.NET Core
runtime library that ingests browser-side Sunlight logs.

The library ships as `Mcqdb.NScript.Sunlight.Logging.Server` and
targets `net8.0` with `FrameworkReference Microsoft.AspNetCore.App`.

### `ILogIngestionService` — public surface

```csharp
public interface ILogIngestionService
{
    ValueTask<IReadOnlyList<string>> IngestAsync(
        LogEnvelopeDto envelope, CancellationToken ct);
}
```

This is what **consumer apps call directly**. Returns the ids of
events that were successfully forwarded to MEL — the caller ACKs them
back to the client.

### One-stop registration

```csharp
builder.Services.AddSunlightLogIngestion();   // singleton + controller

var app = builder.Build();
app.UseWebSockets();
app.MapSunlightLogIngestion();                // /_log + /_log/ws
```

### BYOWS: consumer with their own WebSocket

When the consumer already has a WS endpoint and just wants to forward
frames through us:

```csharp
public async Task HandleMyWsFrame(WebSocket socket, ReadOnlyMemory<byte> frame, ILogIngestionService svc, CancellationToken ct)
{
    byte[] ackBytes = await WebSocketLogProtocol.HandleFrameAsync(svc, frame, ct);
    await socket.SendAsync(ackBytes, WebSocketMessageType.Text, true, ct);
}
```

`WebSocketLogProtocol.HandleFrameAsync` is a **stateless** static — no
`WebSocket` type touched. Hand it the reassembled UTF-8 frame bytes
and it returns the ACK bytes.

### MEL mapping

Each browser event becomes one `ILogger.Log` call on category
`"Sunlight.Browser." + (Cat ?? "default")` with the following scope
state:

- `EventId`, `Timestamp` — from `id` / `ts`
- `TraceId`, `SpanId`, `ParentSpanId`, `ActionId`, `Depth` — from the
  browser-side `CallContext`
- Every key from the `props` bag (unless it collides with a reserved
  name above — props win on collision)

Level mapping:

| Wire string | MEL `LogLevel` |
|---|---|
| `TRACE` | `Trace` |
| `DEBUG` | `Debug` |
| `INFO` | `Information` |
| `WARN` | `Warning` |
| `ERROR` | `Error` |

If `envelope.Dropped > 0` the service emits a single Warning on
category `Sunlight.Browser.Overflow` with `DroppedCount` so query-time
filters can find queue saturation without scanning every event.

### Re-entrancy guard + dedup LRU

- `[ThreadStatic] _ingesting` short-circuits a nested
  `IngestAsync` call on the same thread (e.g. a misconfigured
  `ILoggerProvider` that itself emits browser-shaped events).
- A capacity-`LogIngestionOptions.DedupCapacity` (default 1024) LRU
  keyed by event id keeps the service idempotent under client
  retransmits.

---

## SunlightTestAdapter Integration (WI-99 addendum)

When `Settings.LogEndpoint` is set in a runsettings file, the
`SunlightTestAdapter`'s `Executor.RunSourceGroup` boots an in-process
Kestrel host (`IngestHost`) that serves BOTH:

- the test-bundle static assets (`/index.html`, `/qunit.js`,
  `/qunit.css`, `/test-bundle.js`), and
- the Sunlight log ingestion endpoints (`/_log`, `/_log/ws`)

on a single kernel-assigned `127.0.0.1:N` port. The browser navigates
to `http://127.0.0.1:N/index.html` so all traffic is same-origin (no
CORS shenanigans).

The host configures Serilog with the in-repo `CompactJsonFormatter`
file sink, writing one JSON line per event to the configured
`LogEndpoint` path. Both browser-side `Sunlight.Browser.*` events
(from the ingestion controller) and adapter-side `SunlightTestAdapter.*`
lifecycle events (run start, browser launch, per-test outcomes, run
end) flow through the same `ILoggerFactory`, so they land in the same
file.

Example runsettings:

```xml
<RunSettings>
  <NSTest>
    <JsFilePath>../TestWebApplication/GeneratedScripts/MyApp.Test.js</JsFilePath>
    <LogEndpoint>./logs/browser-tests.jsonl</LogEndpoint>
  </NSTest>
</RunSettings>
```

When `LogEndpoint` is unset (the default), the adapter keeps today's
Playwright route-interception path verbatim — zero behavioral change
for existing consumers.

---

## Out of Scope (still)

WI-99 closed the WebSocket transport and server-ingestion items from
the WI-11 list. The remaining future work:

- Per-category level overrides (beyond a single global `MinLevel`).
- Sampling / rate limiting.
- Serilog-style message templates (`"item {ItemId} added"`).
- Offline / localStorage persistence and retry.

Additional sinks can be added at any time by implementing `ILogSink`
and calling `Logger.AddSink(...)`.

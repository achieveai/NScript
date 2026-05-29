// -----------------------------------------------------------------------
// <copyright file="WebSocketLogHandlerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sunlight.Logging.Server.Extensions;
    using Sunlight.Logging.Server.Models;

    /// <summary>
    /// End-to-end WS tests using the WAF-provided WebSocketClient.
    /// Confirms the receive loop + reassembly + ACK send all hang together.
    /// </summary>
    [TestClass]
    public class WebSocketLogHandlerTests
    {
        private TestLoggerProvider _provider = null!;
        private SunlightLoggingServerFactory _factory = null!;

        [TestInitialize]
        public void Init()
        {
            this._provider = new TestLoggerProvider();
            TestStartup.CurrentProvider = this._provider;
            this._factory = new SunlightLoggingServerFactory();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this._factory.Dispose();
            TestStartup.CurrentProvider = null;
        }

        [TestMethod]
        public async Task WsEndpoint_OneEnvelope_RoundTripsAck()
        {
            var wsClient = this._factory.Server.CreateWebSocketClient();
            var wsUri = new Uri(this._factory.Server.BaseAddress, EndpointRouteBuilderExtensions.DefaultWebSocketPath);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            using var socket = await wsClient.ConnectAsync(wsUri, cts.Token);

            var env = new LogEnvelopeDto
            {
                Events = new List<LogEventDto>
                {
                    new LogEventDto { Id = "ws-1", Level = "INFO", Msg = "from-ws", Cat = "Fixture" },
                },
            };
            var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(env));
            await socket.SendAsync(payload, WebSocketMessageType.Text, endOfMessage: true, cts.Token);

            var buffer = new byte[8192];
            var receive = await socket.ReceiveAsync(buffer, cts.Token);
            Assert.AreEqual(WebSocketMessageType.Text, receive.MessageType);

            var ack = JsonSerializer.Deserialize<LogAckDto>(buffer.AsSpan(0, receive.Count))!;
            Assert.AreEqual(1, ack.AckIds.Length);
            Assert.AreEqual("ws-1", ack.AckIds[0]);

            await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "done", cts.Token);

            Assert.IsTrue(this._provider.Logs.Any(l => l.Category == "Sunlight.Browser.Fixture" && l.Message == "from-ws"));
        }
    }
}

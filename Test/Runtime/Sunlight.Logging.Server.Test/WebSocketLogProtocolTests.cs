// -----------------------------------------------------------------------
// <copyright file="WebSocketLogProtocolTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Test
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sunlight.Logging.Server.Models;
    using Sunlight.Logging.Server.Protocol;
    using Sunlight.Logging.Server.Services;

    /// <summary>
    /// Pure-function tests for <see cref="WebSocketLogProtocol.HandleFrameAsync"/>.
    /// No WebApplicationFactory, no real WebSocket — just call the helper
    /// with bytes and assert the ACK bytes. This is the BYOWS-consumer
    /// integration sanity check (D7).
    /// </summary>
    [TestClass]
    public class WebSocketLogProtocolTests
    {
        private TestLoggerProvider _provider = null!;
        private LogIngestionService _svc = null!;

        [TestInitialize]
        public void Init()
        {
            this._provider = new TestLoggerProvider();
            var factory = LoggerFactory.Create(b => b.AddProvider(this._provider));
            this._svc = new LogIngestionService(
                factory,
                Options.Create(new LogIngestionOptions()));
        }

        [TestMethod]
        public async Task HandleFrameAsync_OneFrameIn_AckBytesOut()
        {
            var env = new LogEnvelopeDto
            {
                Events = new List<LogEventDto>
                {
                    new LogEventDto { Id = "f-1", Level = "INFO", Msg = "hi" },
                },
            };
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(env));

            var ackBytes = await WebSocketLogProtocol.HandleFrameAsync(this._svc, bytes, CancellationToken.None);

            var ack = JsonSerializer.Deserialize<LogAckDto>(ackBytes)!;
            Assert.AreEqual(1, ack.AckIds.Length);
            Assert.AreEqual("f-1", ack.AckIds[0]);
        }

        [TestMethod]
        public async Task HandleFrameAsync_MalformedJson_ReturnsEmptyAck()
        {
            var bad = Encoding.UTF8.GetBytes("{ this is not json");

            var ackBytes = await WebSocketLogProtocol.HandleFrameAsync(this._svc, bad, CancellationToken.None);

            var ack = JsonSerializer.Deserialize<LogAckDto>(ackBytes)!;
            Assert.AreEqual(0, ack.AckIds.Length, "Malformed frame produces empty-ack response (not an exception)");
        }

        [TestMethod]
        public async Task HandleFrameAsync_EmptyEnvelope_ReturnsEmptyAck()
        {
            var env = new LogEnvelopeDto { Events = new List<LogEventDto>(), Dropped = 0 };
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(env));

            var ackBytes = await WebSocketLogProtocol.HandleFrameAsync(this._svc, bytes, CancellationToken.None);

            var ack = JsonSerializer.Deserialize<LogAckDto>(ackBytes)!;
            Assert.AreEqual(0, ack.AckIds.Length);
        }

        [TestMethod]
        public async Task HandleFrameAsync_ZeroLengthFrame_ReturnsEmptyAck()
        {
            var ackBytes = await WebSocketLogProtocol.HandleFrameAsync(
                this._svc,
                System.ReadOnlyMemory<byte>.Empty,
                CancellationToken.None);

            var ack = JsonSerializer.Deserialize<LogAckDto>(ackBytes)!;
            Assert.AreEqual(0, ack.AckIds.Length);
        }
    }
}

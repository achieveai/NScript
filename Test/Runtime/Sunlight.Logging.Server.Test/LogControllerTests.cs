// -----------------------------------------------------------------------
// <copyright file="LogControllerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Sunlight.Logging.Server.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sunlight.Logging.Server.Models;

    /// <summary>
    /// WAF-based round-trip tests for <see cref="Hosting.LogController"/>.
    /// Confirms that POST /_log deserializes the envelope, drives MEL,
    /// and returns the ack id list.
    /// </summary>
    [TestClass]
    public class LogControllerTests
    {
        private TestLoggerProvider _provider = null!;
        private SunlightLoggingServerFactory _factory = null!;
        private HttpClient _client = null!;

        [TestInitialize]
        public void Init()
        {
            this._provider = new TestLoggerProvider();
            TestStartup.CurrentProvider = this._provider;
            this._factory = new SunlightLoggingServerFactory();
            this._client = this._factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this._client.Dispose();
            this._factory.Dispose();
            TestStartup.CurrentProvider = null;
        }

        [TestMethod]
        public async Task Post_ValidEnvelope_ReturnsOkWithAckIds()
        {
            var env = new LogEnvelopeDto
            {
                Events = new List<LogEventDto>
                {
                    new LogEventDto { Id = "c-1", Level = "INFO", Msg = "controller-1", Cat = "Fixture" },
                    new LogEventDto { Id = "c-2", Level = "WARN", Msg = "controller-2", Cat = "Fixture" },
                },
                Dropped = 0,
            };

            var response = await this._client.PostAsJsonAsync("/_log", env);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var ack = await response.Content.ReadFromJsonAsync<LogAckDto>();
            Assert.IsNotNull(ack);
            CollectionAssert.AreEquivalent(new[] { "c-1", "c-2" }, ack!.AckIds);

            var fixtureLogs = this._provider.Logs.Where(l => l.Category == "Sunlight.Browser.Fixture").ToList();
            Assert.AreEqual(2, fixtureLogs.Count);
            Assert.IsTrue(fixtureLogs.Any(l => l.Message == "controller-1" && l.Level == LogLevel.Information));
            Assert.IsTrue(fixtureLogs.Any(l => l.Message == "controller-2" && l.Level == LogLevel.Warning));
        }

        [TestMethod]
        public async Task Post_DroppedNonZero_EmitsOverflowWarning()
        {
            var env = new LogEnvelopeDto
            {
                Events = new List<LogEventDto>(),
                Dropped = 3,
            };

            var response = await this._client.PostAsJsonAsync("/_log", env);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var overflow = this._provider.Logs.Single(l => l.Category == "Sunlight.Browser.Overflow");
            Assert.AreEqual(LogLevel.Warning, overflow.Level);
            StringAssert.Contains(overflow.Message, "3");
        }
    }
}

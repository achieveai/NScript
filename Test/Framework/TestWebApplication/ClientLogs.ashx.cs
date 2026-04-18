using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace TestWebApplication
{
    /// <summary>
    /// Reference ingestion handler for the client-side HttpLogSink.
    ///
    /// Accepts batched JSON envelopes from the browser (content-type
    /// application/json for XHR or text/plain for sendBeacon), deserializes
    /// each event, and forwards it to <see cref="Trace"/> so the test app can
    /// surface structured client events alongside server diagnostics.
    ///
    /// This handler is intentionally minimal — production integrations should
    /// pipe events to Serilog / NLog / AppInsights / Kafka / etc.
    /// </summary>
    public class ClientLogs : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (!string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 405;
                return;
            }

            string body;
            using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
            {
                body = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(body))
            {
                context.Response.StatusCode = 400;
                return;
            }

            Dictionary<string, object> envelope;
            try
            {
                // sendBeacon sends a Blob which surfaces as application/json,
                // but some older browsers default to text/plain — either way
                // the payload is JSON.
                var serializer = new JavaScriptSerializer();
                envelope = serializer.Deserialize<Dictionary<string, object>>(body);
            }
            catch
            {
                context.Response.StatusCode = 400;
                return;
            }

            if (envelope == null)
            {
                context.Response.StatusCode = 400;
                return;
            }

            object droppedObj;
            int dropped = 0;
            if (envelope.TryGetValue("dropped", out droppedObj) && droppedObj != null)
            {
                int.TryParse(droppedObj.ToString(), out dropped);
            }

            if (dropped > 0)
            {
                Trace.WriteLine("[ClientLogs] dropped=" + dropped);
            }

            object eventsObj;
            if (envelope.TryGetValue("events", out eventsObj) && eventsObj is System.Collections.IEnumerable)
            {
                foreach (var item in (System.Collections.IEnumerable)eventsObj)
                {
                    var evt = item as Dictionary<string, object>;
                    if (evt == null) { continue; }
                    Trace.WriteLine(FormatEvent(evt));
                }
            }

            context.Response.StatusCode = 204;
        }

        private static string FormatEvent(Dictionary<string, object> evt)
        {
            // Compact human-readable line plus a JSON dump of the original event
            // so the developer gets both quick readability and full structure.
            var sb = new StringBuilder();
            sb.Append("[ClientLogs] ");
            AppendField(sb, evt, "ts");
            sb.Append(' ');
            AppendField(sb, evt, "level");
            var cat = Read(evt, "cat");
            if (!string.IsNullOrEmpty(cat))
            {
                sb.Append(' ');
                sb.Append('[');
                sb.Append(cat);
                sb.Append(']');
            }
            sb.Append(' ');
            AppendField(sb, evt, "msg");

            var traceId = Read(evt, "traceId");
            if (!string.IsNullOrEmpty(traceId))
            {
                sb.Append(" traceId=");
                sb.Append(traceId);
            }
            return sb.ToString();
        }

        private static void AppendField(StringBuilder sb, Dictionary<string, object> evt, string key)
        {
            object value;
            if (evt.TryGetValue(key, out value) && value != null)
            {
                sb.Append(value.ToString());
            }
        }

        private static string Read(Dictionary<string, object> evt, string key)
        {
            object value;
            if (evt.TryGetValue(key, out value) && value != null)
            {
                return value.ToString();
            }
            return null;
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}

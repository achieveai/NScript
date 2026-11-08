//-----------------------------------------------------------------------
// <copyright file="DomBindingsTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Html;
    using SunlightUnit;

    /// <summary>
    /// Browser API bindings from #139 and Node.moveBefore from #138.
    /// FileSystemEntry needs a real drop, so only its feature check runs here.
    /// </summary>
    [TestFixture]
    public class DomBindingsTests
    {
        [Test]
        public static void ClosestFindsAncestorOrNull(Assert assert)
        {
            var doc = Window.Instance.Document;
            var outer = doc.CreateElement("div");
            outer.SetAttribute("class", "outer");
            var inner = doc.CreateElement("span");
            outer.AppendChild(inner);

            assert.StrictEqual(inner.Closest("div.outer"), outer, "closest finds the ancestor");
            assert.StrictEqual(inner.Closest("span"), inner, "closest includes the element itself");
            assert.StrictEqual(inner.Closest(".missing"), null, "closest returns null when none match");
        }

        [Test]
        public static void UrlSearchParamsReadsAndWrites(Assert assert)
        {
            var query = new URLSearchParams("?a=1&b=2&a=3");
            assert.StrictEqual(query.Get("a"), "1", "Get returns the first value");
            assert.StrictEqual(query.Get("zz"), null, "Get returns null when missing");
            var all = query.GetAll("a");
            assert.StrictEqual(all.Length, 2, "GetAll returns every value");
            assert.StrictEqual(all[1], "3", "GetAll keeps order");
            assert.IsTrue(query.Has("b"), "Has finds a name");

            query.Set("b", "x");
            query.Append("c", "y z");
            query.Delete("a");
            assert.StrictEqual(query.ToString(), "b=x&c=y+z", "ToString serializes");

            var empty = new URLSearchParams();
            assert.StrictEqual(empty.ToString(), "", "empty ctor");
        }

        // #158
        [Test]
        public static void UrlParsesAndResolvesAgainstBase(Assert assert)
        {
            var url = new URL("/a/b?x=1#h", "https://example.com:8443/root");
            assert.StrictEqual(url.Href, "https://example.com:8443/a/b?x=1#h", "Href");
            assert.StrictEqual(url.Origin, "https://example.com:8443", "Origin");
            assert.StrictEqual(url.Protocol, "https:", "Protocol");
            assert.StrictEqual(url.Host, "example.com:8443", "Host");
            assert.StrictEqual(url.Hostname, "example.com", "Hostname");
            assert.StrictEqual(url.Port, "8443", "Port");
            assert.StrictEqual(url.Pathname, "/a/b", "Pathname");
            assert.StrictEqual(url.Search, "?x=1", "Search");
            assert.StrictEqual(url.Hash, "#h", "Hash");
            assert.StrictEqual(url.SearchParams.Get("x"), "1", "SearchParams");

            assert.StrictEqual(new URL("https://a.test/p").Pathname, "/p", "one-argument form");
            assert.Equal(DomBindingsTests.IsBrowserRefusal(delegate { new URL("not a url"); }), true, "an invalid URL throws");
            assert.IsTrue(URL.HasCreateObjectURL, "the static members still work");
        }

        // #158: no pointer is active here, so capture must reach the browser and be refused.
        [Test]
        public static void PointerCaptureReachesTheBrowser(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            Window.Instance.Document.Body.AppendChild(element);
            assert.StrictEqual(element.HasPointerCapture(999), false, "no capture yet");
            assert.Equal(DomBindingsTests.IsBrowserRefusal(delegate { element.SetPointerCapture(999); }), true, "SetPointerCapture");
            assert.Equal(DomBindingsTests.IsBrowserRefusal(delegate { element.ReleasePointerCapture(999); }), true, "ReleasePointerCapture");
            element.Remove();
        }

        // #158: the id that SetPointerCapture needs.
        [Test]
        public static void PointerEventHandsPointerId(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            int pointerId = 0;
            Action<Element, ElementEvent> handler = (e, evt) => { pointerId = evt.PointerId; };
            element.Bind("pointerdown", handler);
            element.DispatchEvent(DomBindingsTests.MakePointerEvent("pointerdown", 7));
            assert.StrictEqual(pointerId, 7, "PointerId");
        }

        [Test]
        public static void MatchMediaReportsAndRaisesChange(Assert assert)
        {
            var mql = Window.Instance.MatchMedia("(min-width: 1px)");
            assert.StrictEqual(mql.Matches, true, "Matches is true for min-width 1px");
            assert.StrictEqual(mql.Media, "(min-width: 1px)", "Media echoes the query");
            assert.StrictEqual(Window.Instance.MatchMedia("(max-width: 1px)").Matches, false, "Matches is false for max-width 1px");

            int calls = 0;
            Action<MediaQueryList, MediaQueryListEvent> onChange = (list, evt) => { calls++; };
            mql.OnChange += onChange;
            mql.DispatchEvent(DomBindingsTests.MakeEvent("change"));
            mql.OnChange -= onChange;
            mql.DispatchEvent(DomBindingsTests.MakeEvent("change"));
            assert.StrictEqual(calls, 1, "OnChange subscribes and unsubscribes on 'change'");
        }

        [Test]
        public static void WindowKeyDownHandsTypedKeyEvent(Assert assert)
        {
            string key = null;
            string code = null;
            bool ctrl = false;
            Action<Window, ElementEvent> onKeyDown = (w, evt) =>
            {
                key = evt.Key;
                code = evt.Code;
                ctrl = evt.CtrlKey;
            };

            Window.Instance.OnKeyDown += onKeyDown;
            Window.Instance.DispatchEvent(DomBindingsTests.MakeKeyboardEvent("keydown", new KeyInit { Key = "k", Code = "KeyK", CtrlKey = true }));
            Window.Instance.OnKeyDown -= onKeyDown;

            assert.StrictEqual(key, "k", "Key");
            assert.StrictEqual(code, "KeyK", "Code");
            assert.StrictEqual(ctrl, true, "CtrlKey");

            int upCalls = 0;
            Action<Window, ElementEvent> onKeyUp = (w, evt) => { upCalls++; };
            Window.Instance.OnKeyUp += onKeyUp;
            Window.Instance.DispatchEvent(DomBindingsTests.MakeKeyboardEvent("keyup", new KeyInit { Key = "k", Code = "KeyK", CtrlKey = true }));
            Window.Instance.OnKeyUp -= onKeyUp;
            assert.StrictEqual(upCalls, 1, "OnKeyUp listens to 'keyup'");
        }

        [Test]
        public static void WindowUnhandledRejectionHandsReason(Assert assert)
        {
            object reason = null;
            Action<Window, PromiseRejectionEvent> onRejection = (w, evt) =>
            {
                reason = evt.Reason;
                evt.PreventDefault();
            };

            Window.Instance.OnUnhandledRejection += onRejection;
            Window.Instance.DispatchEvent(DomBindingsTests.MakeRejectionEvent(new RejectionInit { Promise = Promise.Resolve<int>(0), Reason = "boom", Cancelable = true }));
            Window.Instance.OnUnhandledRejection -= onRejection;

            assert.StrictEqual(reason, (object)"boom", "Reason");
        }

        [Test]
        public static void ScrollIntoViewOptionsIsPlainObject(Assert assert)
        {
            var options = new ScrollIntoViewOptions
            {
                Block = ScrollLogicalPosition.Center,
                Inline = ScrollLogicalPosition.Nearest,
                Behavior = ScrollIntoViewBehavior.Auto,
            };

            assert.StrictEqual(
                DomBindingsTests.Stringify(options),
                "{\"block\":\"center\",\"inline\":\"nearest\",\"behavior\":\"auto\"}",
                "options emit as a plain object with browser key names");

            // A fixed 100px scroller over 1020px of content; the 20px target starts at 500px.
            // Centered, its middle (510) sits at the scroller's middle (50): scrollTop 460.
            // Start alignment would give 500, no scroll 0.
            var doc = Window.Instance.Document;
            var scroller = doc.CreateElement("div");
            scroller.SetAttribute("style",
                "position:fixed;top:0;left:0;width:100px;height:100px;overflow:auto;margin:0;padding:0;border:0;");
            var before = doc.CreateElement("div");
            before.SetAttribute("style", "margin:0;padding:0;border:0;height:500px;");
            var target = doc.CreateElement("div");
            target.SetAttribute("style", "margin:0;padding:0;border:0;height:20px;");
            var after = doc.CreateElement("div");
            after.SetAttribute("style", "margin:0;padding:0;border:0;height:500px;");
            scroller.AppendChild(before);
            scroller.AppendChild(target);
            scroller.AppendChild(after);
            doc.Body.AppendChild(scroller);
            target.ScrollIntoView(options);
            double scrollTop = scroller.ScrollTop;
            scroller.Remove();
            assert.StrictEqual(scrollTop, 460.0, "block: center centers the target in its scroller");
        }

        [Test]
        public static void MoveOrInsertBeforeMovesAndFallsBack(Assert assert)
        {
            var doc = Window.Instance.Document;

            // Connected: uses moveBefore when the browser has it.
            var host = doc.CreateElement("div");
            var a = doc.CreateElement("i");
            var b = doc.CreateElement("b");
            host.AppendChild(a);
            host.AppendChild(b);
            doc.Body.AppendChild(host);

            bool kept = Node.MoveOrInsertBefore(host, b, a);
            assert.StrictEqual(kept, Node.SupportsMoveBefore, "keeps state exactly when moveBefore exists");
            assert.StrictEqual(host.FirstChild, (Node)b, "node moved before the reference child");

            if (Node.SupportsMoveBefore)
            {
                host.MoveBefore(b, null);
                assert.StrictEqual(host.LastChild, (Node)b, "MoveBefore(node, null) moves to the end");
            }

            host.Remove();

            // Detached parent: falls back to insertBefore.
            var detached = doc.CreateElement("div");
            var c = doc.CreateElement("i");
            var d = doc.CreateElement("b");
            detached.AppendChild(c);
            detached.AppendChild(d);
            assert.StrictEqual(Node.MoveOrInsertBefore(detached, d, c), false, "detached parent falls back");
            assert.StrictEqual(detached.FirstChild, (Node)d, "fallback still moves the node");
        }

        [Test]
        public static void DataTransferTakesAnyMimeType(Assert assert)
        {
            var dt = new DataTransfer();
            dt.SetData("text/plain", "id-1");
            assert.StrictEqual(dt.GetData("text/plain"), "id-1", "string SetData/GetData");
            assert.StrictEqual(dt.Items.Length, 1, "Items lists the entry");
            assert.StrictEqual(dt.Items[0].Kind, "string", "item kind");
            assert.StrictEqual(dt.Items[0].Type, "text/plain", "item type");

            dt.ClearData("text/plain");
            assert.StrictEqual(dt.GetData("text/plain"), "", "string ClearData");

            dt.SetData(DataFormat.Text, "x");
            assert.StrictEqual(dt.GetData(DataFormat.Text), "x", "enum overload still works");

            assert.StrictEqual(
                DataTransferItem.SupportsGetAsEntry,
                DomBindingsTests.HasWebkitGetAsEntry(),
                "SupportsGetAsEntry matches the browser");
        }

        [Test]
        public static void WebSocketTakesProtocolArray(Assert assert)
        {
            // A joined string "p1,p2" is not a valid protocol token and throws, so a
            // successful construction proves the array reached the browser as an array.
            var socket = new WebSocket(
                "ws://127.0.0.1:9/",
                NativeArray<string>.GetNativeArray(new string[] { "p1", "p2" }));
            assert.StrictEqual(socket.ReadyState, WebSocketReadyState.Connecting, "socket created");
            socket.Close();
            assert.StrictEqual(WebSocket.IsAvailable(), true, "IsAvailable is true where WebSocket exists");
        }

        [Test]
        public static void ClipboardIsWired(Assert assert)
        {
            var navigator = Window.Instance.Navigator;
            assert.StrictEqual(navigator.HasClipboard, navigator.Clipboard != null, "HasClipboard matches Clipboard");
            assert.IsTrue(navigator.HasClipboard, "clipboard exists on a localhost (secure) page");
        }

        [Test]
        public static void FileSystemEntryCallbacksBecomeTasks(Assert assert)
        {
            // Only a real drop yields entries, so a stand-in with the browser's
            // callback shape checks the callback-to-Task bridge.
            var done = assert.Async(2);
            var file = DomBindingsTests.MakeFile("a.txt");
            var fileEntry = (FileSystemFileEntry)(object)new FakeFileEntry
            {
                File = (success, error) => success(file),
            };
            DomBindingsTests.AsPromise(fileEntry.File()).Then(f =>
            {
                assert.StrictEqual(f, file, "File() resolves with the callback's file");
                done();
            });

            var child = new FakeFileEntry { Name = "child" };
            var batch = new NativeArray<FileSystemEntry>(0);
            batch.Push((FileSystemEntry)(object)child);
            var reader = (FileSystemDirectoryReader)(object)new FakeReader
            {
                ReadEntries = (success, error) => success(batch),
            };
            DomBindingsTests.AsPromise(reader.ReadEntries()).Then(entries =>
            {
                assert.StrictEqual(entries.Length, 1, "ReadEntries() resolves with the batch");
                assert.StrictEqual(entries[0].Name, "child", "entry name");
                done();
            });
        }

        [Script("return t;")]
        private static extern Promise<T> AsPromise<T>(System.Threading.Task<T> t);

        [Script("return new @:File([], name);")]
        private static extern File MakeFile(string name);

        [Script("return new @:Event(name);")]
        private static extern Event MakeEvent(string name);

        [Script("return new @:PointerEvent(name, { pointerId: id });")]
        private static extern MutableEvent MakePointerEvent(string name, int id);

        // True when the call threw and the method existed.
        private static bool IsBrowserRefusal(Action call)
        {
            try
            {
                call();
                return false;
            }
            catch (Exception ex)
            {
                return ex.Message.IndexOf("is not a function") < 0
                    && ex.Message.IndexOf("is not a constructor") < 0;
            }
        }

        // Init objects are typed C# objects, so the tests read like caller code.
        [Script("return new @:KeyboardEvent(name, init);")]
        private static extern Event MakeKeyboardEvent(string name, KeyInit init);

        // A resolved promise stands in: a real rejected one would itself be unhandled.
        [Script("return new @:PromiseRejectionEvent('unhandledrejection', init);")]
        private static extern Event MakeRejectionEvent(RejectionInit init);

        [Script("return @:JSON.stringify(o);")]
        private static extern string Stringify(object o);

        [Script("return typeof @:DataTransferItem.prototype.webkitGetAsEntry === 'function';")]
        private static extern bool HasWebkitGetAsEntry();

        [JsonType]
        private sealed class KeyInit
        {
            public extern string Key { get; set; }

            public extern string Code { get; set; }

            public extern bool CtrlKey { get; set; }
        }

        [JsonType]
        private sealed class RejectionInit
        {
            public extern Promise Promise { get; set; }

            public extern object Reason { get; set; }

            public extern bool Cancelable { get; set; }
        }

        [JsonType]
        private sealed class FakeFileEntry
        {
            public extern string Name { get; set; }

            public extern Action<Action<File>, Action<object>> File { get; set; }
        }

        [JsonType]
        private sealed class FakeReader
        {
            public extern Action<Action<NativeArray<FileSystemEntry>>, Action<object>> ReadEntries { get; set; }
        }
    }
}

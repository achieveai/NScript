namespace Sunlight.Framework.UI.Test
{
    using System;
    using SunlightUnit;
    using System.Web.Html;

    /// <summary>
    /// Content projection (issue #137): caller markup inside &lt;SlotFrame&gt;...&lt;/SlotFrame&gt;
    /// shows at the frame skin's @RenderBody() and stays bound to the caller's view model.
    /// </summary>
    [TestFixture]
    public class RazorSlotTests
    {
        // Mounts the caller, then gives its frame the skin under test. A sub-control's Skin
        // cannot be bound from a view-model property, so the swap is the HMR-style re-skin.
        private static UISkinableElement Mount(SlotHostVM vm, Skin skin, Skin frameSkin)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            control.DataContext = vm;
            control.Skin = skin;
            control.Activate();
            if (frameSkin != null)
                SlotFrame.LastCreated.Skin = frameSkin;
            return control;
        }

        private static string Text(Element root, string selector)
        {
            var e = root.QuerySelector(selector);
            return e == null ? null : e.TextContent;
        }

        private static int Count(Element root, string selector)
        {
            return root.QuerySelectorAll(selector).Length;
        }

        // RazorProbeControl writes its Text to its host; a sub-control host has no DOM id,
        // so tests find it by position.
        private static string BoundText(Element root, string selector)
        {
            var e = root.QuerySelector(selector);
            return e == null ? null : e.GetAttribute("data-bound-text");
        }

        // The content is live in the outlet: caller bindings, a caller @if, caller clicks.
        private static void AssertLiveContent(Assert assert, SlotHostVM vm, Element root, string section, string step)
        {
            int confirms = vm.ConfirmCount;
            vm.Message = "msg-" + step;
            assert.Equal(Count(root, "p.msg"), 1, step + ": one copy of the content");
            assert.Equal(Text(root, section + " > p.msg"), "msg-" + step, step + ": caller binding updates in the outlet");
            assert.Equal(BoundText(root, section + " > [data-ns-subctl]"), "msg-" + step, step + ": caller sub-control keeps the caller's binding");
            root.QuerySelector(section + " > button.confirm").Click();
            assert.Equal(vm.ConfirmCount, confirms + 1, step + ": caller onclick fires");
            assert.Equal(vm.ExtraClicks, 0, step + ": the click pairs with the button, not the caller's gated em");
            assert.Equal(vm.FrameDblClicks, 0, step + ": the click pairs with the button, not the frame tag");
        }

        [Test]
        public static void TestSlotStaticOutletProjectsLiveContent(Assert assert)
        {
            var vm = new SlotHostVM();
            var host = Mount(vm, RazorSkinTemplatesClass.SlotHost, null);
            var root = host.Element;

            assert.Equal(Text(root, "section.frame-body > p.msg"), "m1", "content renders inside the outlet");
            assert.Equal(Text(root, "header.frame-title"), "t1", "frame binds to its own model");
            AssertLiveContent(assert, vm, root, "section.frame-body", "mounted");
            assert.Equal(Text(root, "span.after"), "msg-mounted", "caller binding after the frame updates");

            vm.Frame.Title = "t2";
            assert.Equal(Text(root, "header.frame-title"), "t2", "frame binding updates");

            vm.ShowExtra = true;
            assert.Equal(Count(root, "section.frame-body > em.extra"), 1, "caller @if inside the content opens in the outlet");
            root.QuerySelector("em.extra").Click();
            assert.Equal(vm.ExtraClicks, 1, "gated caller handler fires");
            vm.ExtraClicks = 0;
            vm.ShowExtra = false;
            assert.Equal(Count(root, "em.extra"), 0, "caller @if inside the content closes");

            var frame = SlotFrame.LastCreated;
            frame.Skin = null;
            assert.Equal(Count(root, "section.frame-body"), 0, "un-skinned: frame DOM gone");
            assert.Equal(Count(root, "p.msg"), 1, "un-skinned: content back on the host");
            frame.Skin = SlotFrame.DefaultSkin;
            AssertLiveContent(assert, vm, root, "section.frame-body", "re-skinned");

            var p = root.QuerySelector("p.msg");
            host.Dispose();
            vm.Message = "after-dispose";
            assert.Equal(p.TextContent, "msg-re-skinned", "disposed caller writes nothing");
        }

        [Test]
        public static void TestSlotGatedOutletFollowsGate(Assert assert)
        {
            var vm = new SlotHostVM();
            var host = Mount(vm, RazorSkinTemplatesClass.SlotHost, null);
            var root = host.Element;
            var p = root.QuerySelector("p.msg");
            var button = root.QuerySelector("button.confirm");
            SlotFrame.LastCreated.Skin = RazorSkinTemplatesClass.SlotGatedFrame;

            assert.Equal(Count(root, "section.frame-body"), 0, "starts closed: no outlet");
            assert.IsTrue(!root.Contains(p), "starts closed: content parked off the page");
            assert.Equal(BoundText(root, "header.frame-title + [data-ns-subctl]"), "t1", "child sub-control before the gate binds");

            vm.Message = "while-closed";
            vm.Frame.IsOpen = true;
            assert.Equal(Text(root, "section.frame-body > p.msg"), "while-closed", "open 1: shows the value set while closed");

            string[] steps = new string[] { "open 1", "open 2", "open 3" };
            for (int i = 0; i < steps.Length; i++)
            {
                string step = steps[i];
                vm.Frame.IsOpen = true;
                vm.Frame.IsOpen = true;
                AssertLiveContent(assert, vm, root, "section.frame-body", step);
                vm.Frame.Title = "title-" + step;
                assert.Equal(Text(root, "footer.frame-foot"), "title-" + step, step + ": child binding after the outlet updates");
                assert.Equal(BoundText(root, "footer.frame-foot + [data-ns-subctl]"), "title-" + step, step + ": child sub-control sits on the child's host");
                assert.Equal(root.QuerySelector("footer.frame-foot + [data-ns-subctl]").ParentNode, root.QuerySelector("section.frame-body").ParentNode,
                    step + ": child sub-control host is the branch sibling of the outlet");

                vm.Frame.IsOpen = false;
                assert.Equal(Count(root, "section.frame-body"), 0, step + " then closed: outlet gone");
                assert.IsTrue(!root.Contains(p), step + " then closed: content parked");
                int confirms = vm.ConfirmCount;
                button.Click();
                assert.Equal(vm.ConfirmCount, confirms + 1, step + " then closed: parked button keeps its listener");
                vm.Message = "parked-" + step;
                assert.Equal(p.TextContent, "parked-" + step, step + " then closed: parked content keeps updating");
            }
        }

        [Test]
        public static void TestSlotGatedOutletReskinAndDispose(Assert assert)
        {
            var vm = new SlotHostVM();
            vm.Frame.IsOpen = true;
            var host = Mount(vm, RazorSkinTemplatesClass.SlotHost, RazorSkinTemplatesClass.SlotGatedFrame);
            var root = host.Element;
            var frame = SlotFrame.LastCreated;

            AssertLiveContent(assert, vm, root, "section.frame-body", "mounted open");

            frame.Skin = null;
            frame.Skin = RazorSkinTemplatesClass.SlotGatedFrame;
            AssertLiveContent(assert, vm, root, "section.frame-body", "re-skinned while open");

            vm.Frame.IsOpen = false;
            frame.Skin = null;
            assert.Equal(Count(root, "p.msg"), 1, "un-skinned while closed: content back on the host");
            frame.Skin = RazorSkinTemplatesClass.SlotGatedFrame;
            assert.Equal(Count(root, "p.msg"), 0, "re-skinned while closed: content parked");
            vm.Frame.IsOpen = true;
            AssertLiveContent(assert, vm, root, "section.frame-body", "re-skinned while closed, then opened");

            // Caller dispose while the frame is closed (content parked).
            var frameElement = frame.Element;
            var p = root.QuerySelector("p.msg");
            string lastMessage = p.TextContent;
            vm.Frame.IsOpen = false;
            string thrown = null;
            try
            {
                host.Dispose();
            }
            catch (Exception e)
            {
                thrown = e.Message;
            }

            assert.Equal(thrown, null, "caller dispose while the frame is closed does not throw");
            assert.IsTrue(frameElement.Contains(p), "disposed: the parked content is back on the frame host");
            vm.Frame.IsOpen = true;
            assert.Equal(frameElement.QuerySelectorAll("section.frame-body").Length, 0,
                "disposed: the frame's gate does not render its branch again");
            vm.Message = "after-dispose";
            assert.Equal(p.TextContent, lastMessage, "disposed: a caller-model change does not reach the old content");
        }

        // Review M1: re-activation must reset the @else side's nested gates, as Flush does.
        [Test]
        public static void TestSlotNestedOutletAfterReactivation(Assert assert)
        {
            var vm = new SlotHostVM();
            vm.Frame.ShowInner = true;
            var root = Mount(vm, RazorSkinTemplatesClass.SlotHost, RazorSkinTemplatesClass.SlotNestedFrame).Element;
            var frame = SlotFrame.LastCreated;
            AssertLiveContent(assert, vm, root, "section.frame-body", "else side, inner open");

            frame.Deactivate();
            vm.Frame.IsOpen = true;
            frame.Activate();
            assert.Equal(Count(root, "b.frame-open"), 1, "reactivated: outer gate shows its open side");
            assert.Equal(Count(root, "section.frame-body"), 0, "reactivated: the @else side and its outlet are gone");

            vm.Frame.IsOpen = false;
            assert.Equal(Count(root, "div.frame-else > div.frame-inner"), 1, "outer closed again: the inner branch renders");
            AssertLiveContent(assert, vm, root, "section.frame-body", "after reactivation, outer closed again");
        }

        // Review M3a + A1: an outlet in a nested @if follows both gates; the inner branch's
        // sub-control is created once per open and its event fires once per click.
        [Test]
        public static void TestSlotOutletInNestedIfFollowsBothGates(Assert assert)
        {
            var vm = new SlotHostVM();
            var host = Mount(vm, RazorSkinTemplatesClass.SlotHost, null);
            var root = host.Element;
            var p = root.QuerySelector("p.msg");
            SlotFrame.LastCreated.Skin = RazorSkinTemplatesClass.SlotNestedFrame;
            assert.IsTrue(!root.Contains(p), "inner closed: content parked");

            for (int round = 0; round < 2; round++)
            {
                string r = "round " + round;
                int created = RazorProbeControl.CreatedCount;
                vm.Frame.ShowInner = true;
                AssertLiveContent(assert, vm, root, "section.frame-body", r + " inner open");
                assert.Equal(RazorProbeControl.CreatedCount, created + 1, r + ": inner sub-control created once");
                int clicks = vm.Frame.InnerClicks;
                root.QuerySelector("i.inner-evt").Click();
                assert.Equal(vm.Frame.InnerClicks, clicks + 1, r + ": inner event fires once");

                vm.Frame.IsOpen = true;
                assert.Equal(Count(root, "div.frame-inner"), 0, r + " outer open: inner branch gone");
                assert.IsTrue(!root.Contains(p), r + " outer open: content parked");

                created = RazorProbeControl.CreatedCount;
                vm.Frame.IsOpen = false;
                AssertLiveContent(assert, vm, root, "section.frame-body", r + " outer closed again");
                assert.Equal(RazorProbeControl.CreatedCount, created + 1, r + ": inner sub-control re-created once");
                clicks = vm.Frame.InnerClicks;
                root.QuerySelector("i.inner-evt").Click();
                assert.Equal(vm.Frame.InnerClicks, clicks + 1, r + ": inner event fires once after the outer reopens");
                assert.Equal(Count(root, "p.msg"), 1, r + ": one copy of the content");

                vm.Frame.ShowInner = false;
                assert.Equal(Count(root, "section.frame-body"), 0, r + " inner closed: outlet gone");
                assert.IsTrue(!root.Contains(p), r + " inner closed: content parked");
            }
        }

        // Review M3b (design 12): re-skin the caller (HMR order) while its content is projected.
        [Test]
        public static void TestSlotCallerReskinRebuildsFrameAndContent(Assert assert)
        {
            var vm = new SlotHostVM();
            var host = Mount(vm, RazorSkinTemplatesClass.SlotHost, null);
            var root = host.Element;
            AssertLiveContent(assert, vm, root, "section.frame-body", "before caller re-skin");

            host.Skin = null;
            assert.Equal(Count(root, "p.msg"), 0, "caller un-skinned: nothing left");
            host.Skin = RazorSkinTemplatesClass.SlotHost;
            assert.Equal(Count(root, "section.frame-body"), 1, "caller re-skinned: one frame");
            AssertLiveContent(assert, vm, root, "section.frame-body", "after caller re-skin");
            assert.Equal(Text(root, "span.after"), vm.Message, "caller binding after the frame still live");
        }

        // Review M3c (design 14 / F4): re-skin a frame whose forwarding sits inside an open gate.
        [Test]
        public static void TestSlotReskinFrameWithForwardingInOpenGate(Assert assert)
        {
            var vm = new SlotHostVM();
            vm.Frame.IsOpen = true;
            var root = Mount(vm, RazorSkinTemplatesClass.SlotHost, null).Element;
            var outer = SlotFrame.LastCreated;
            outer.Skin = RazorSkinTemplatesClass.SlotGatedForwardFrame;
            AssertLiveContent(assert, vm, root, "div.fwd section.frame-body", "forwarded in an open gate");

            outer.Skin = null;
            assert.Equal(Count(root, "div.fwd"), 0, "un-skinned while open: forwarding frame gone");
            assert.Equal(Count(root, "p.msg"), 1, "un-skinned while open: one copy back on the host");
            outer.Skin = RazorSkinTemplatesClass.SlotGatedForwardFrame;
            AssertLiveContent(assert, vm, root, "div.fwd section.frame-body", "re-skinned while open");

            vm.Frame.IsOpen = false;
            assert.Equal(Count(root, "p.msg"), 0, "closed: content parked");
            outer.Skin = null;
            outer.Skin = RazorSkinTemplatesClass.SlotGatedForwardFrame;
            vm.Frame.IsOpen = true;
            AssertLiveContent(assert, vm, root, "div.fwd section.frame-body", "re-skinned while closed, then opened");
        }

        // Review M2 (runtime half): an XWML skin has no outlet, so caller content is refused.
        [Test]
        public static void TestSlotContentOnXwmlSkinThrows(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            element.SetAttribute("data-ns-content", "");
            element.AppendChild(Window.Instance.Document.CreateElement("p"));
            var control = new UISkinableElement(element);
            control.DataContext = new TestViewModelA();
            control.Skin = NScriptsTemplatesClass.TestTemplate1;
            string message = null;
            try
            {
                control.Activate();
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            assert.IsTrue(message != null && message.IndexOf("is not a Razor skin") >= 0,
                "caller content on an XWML skin throws: " + message);
        }

        [Test]
        public static void TestSlotIfElseOutletFollowsBranch(Assert assert)
        {
            var vm = new SlotHostVM();
            var host = Mount(vm, RazorSkinTemplatesClass.SlotHost, RazorSkinTemplatesClass.SlotElseFrame);
            var root = host.Element;

            // The @else outlet sits directly in the branch: content lands right on the frame host.
            AssertLiveContent(assert, vm, root, "[data-ns-content]", "else");
            for (int i = 0; i < 2; i++)
            {
                vm.Frame.IsOpen = true;
                AssertLiveContent(assert, vm, root, "section.frame-open", "if " + i);
                vm.Frame.IsOpen = false;
                assert.Equal(Count(root, "section.frame-open"), 0, "else " + i + ": if branch gone");
                AssertLiveContent(assert, vm, root, "[data-ns-content]", "else " + i);
            }
        }

        [Test]
        public static void TestSlotContentWithoutOutletThrows(Assert assert)
        {
            var vm = new SlotHostVM();
            string message = null;
            try
            {
                Mount(vm, RazorSkinTemplatesClass.SlotHost, RazorSkinTemplatesClass.SlotNoOutletFrame);
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            assert.IsTrue(message != null && message.IndexOf("has no @RenderBody()") >= 0,
                "content for a skin with no outlet throws a named error: " + message);
        }

        [Test]
        public static void TestSlotFramesInForeach(Assert assert)
        {
            var vm = new SlotHostVM();
            vm.Rows.Add(new SlotFrameVM("r0"));
            vm.Rows.Add(new SlotFrameVM("r1"));
            var root = Mount(vm, RazorSkinTemplatesClass.SlotHostRows, null).Element;

            var rows = root.QuerySelectorAll("section.frame-body > p.row-msg");
            assert.Equal(rows.Length, 2, "each row frame shows its content");
            assert.Equal(rows[1].TextContent, "r1", "row 1 content");
            vm.Rows[1].Title = "r1b";
            assert.Equal(rows[1].TextContent, "r1b", "row content stays live");
            vm.Rows.Add(new SlotFrameVM("r2"));
            assert.Equal(Count(root, "section.frame-body > p.row-msg"), 3, "an added row projects its content");
            vm.Rows.RemoveAt(0);
            assert.Equal(Count(root, "p.row-msg"), 2, "a removed row takes its content");
        }

        // Slice 2: <Slot Name="x"> content goes to @RenderSlot("x"); the rest to @RenderBody().
        [Test]
        public static void TestSlotNamedSlotsProjectEachName(Assert assert)
        {
            var vm = new SlotHostVM();
            vm.Frame.IsOpen = true;
            var root = Mount(vm, RazorSkinTemplatesClass.SlotHostNamed, null).Element;

            assert.Equal(Text(root, "header.panel-head > h2.head"), "m1", "head slot content in the head outlet");
            assert.Equal(Text(root, "section.panel-body > p.msg"), "m1", "default content in the body outlet");
            assert.Equal(Count(root, "footer.panel-actions > button.confirm"), 1, "actions slot content in the gated outlet");
            assert.Equal(Count(root, "template[data-ns-slot-for], template[data-ns-slot-end]"), 0,
                "the <Slot> boundary markers stay out of the outlets");

            vm.Message = "m2";
            assert.Equal(Text(root, "h2.head"), "m2", "head slot binding updates");
            assert.Equal(Text(root, "p.msg"), "m2", "default slot binding updates");
            root.QuerySelector("button.confirm").Click();
            assert.Equal(vm.ConfirmCount, 1, "actions slot onclick fires");

            vm.Frame.IsOpen = false;
            assert.Equal(Count(root, "button.confirm"), 0, "closed: actions slot parked");
            assert.Equal(Text(root, "header.panel-head > h2.head"), "m2", "closed: head slot stays");
            vm.Frame.IsOpen = true;
            root.QuerySelector("footer.panel-actions > button.confirm").Click();
            assert.Equal(vm.ConfirmCount, 2, "reopened: actions slot back and live");

            var panel = SlotPanel.LastCreated;
            panel.Skin = null;
            assert.Equal(Count(root, "[data-ns-content] > template[data-ns-slot-for]"), 2, "un-skinned: markers back on the host");
            assert.Equal(Count(root, "[data-ns-content] > p.msg"), 1, "un-skinned: default content back on the host");
            panel.Skin = SlotPanel.DefaultSkin;
            vm.Message = "m3";
            assert.Equal(Text(root, "header.panel-head > h2.head"), "m3", "re-skinned: head slot projected and live");
            assert.Equal(Text(root, "section.panel-body > p.msg"), "m3", "re-skinned: default slot projected and live");
            root.QuerySelector("footer.panel-actions > button.confirm").Click();
            assert.Equal(vm.ConfirmCount, 3, "re-skinned: actions slot live");
        }

        [Test]
        public static void TestSlotMissingNamedOutletThrows(Assert assert)
        {
            var vm = new SlotHostVM();
            string message = null;
            try
            {
                Mount(vm, RazorSkinTemplatesClass.SlotHostMissingSlot, null);
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            assert.IsTrue(message != null && message.IndexOf("has no @RenderSlot(\"nope\")") >= 0,
                "a <Slot> the skin has no outlet for throws naming the slot: " + message);
        }

        [Test]
        public static void TestSlotForwardedThroughNestedFrame(Assert assert)
        {
            var vm = new SlotHostVM();
            var root = Mount(vm, RazorSkinTemplatesClass.SlotHost, RazorSkinTemplatesClass.SlotForwardFrame).Element;

            AssertLiveContent(assert, vm, root, "div.fwd section.frame-body", "forwarded");
        }
    }
}

namespace Sunlight.Framework.UI.Test
{
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;
    using SunlightUnit;
    using System.Web.Html;

    /// <summary>A row with nested rows, for a @foreach inside a @foreach (issue #147).</summary>
    public class NestItem : ObservableObject
    {
        [AutoFire] public string Name { get; set; }
        public ObservableCollection<NestItem> Tabs { get; private set; }
        public int Picks;

        public NestItem(string name)
        {
            this.Name = name;
            this.Tabs = new ObservableCollection<NestItem>();
        }

        public void Pick(Element elem, ElementEvent evt)
        {
            this.Picks++;
        }
    }

    public class NestVm : ObservableObject
    {
        public ObservableCollection<NestItem> Groups { get; private set; }
        [AutoFire] public string Title { get; set; }
        public NestItem Opened;
        public int OpenCount;
        public int PlainClicks;

        public NestVm()
        {
            this.Groups = new ObservableCollection<NestItem>();
            this.Title = "t1";
        }

        public void OpenTab(NestItem tab)
        {
            this.Opened = tab;
            this.OpenCount++;
        }

        public void OnPlain(Element elem, ElementEvent evt)
        {
            this.PlainClicks++;
        }
    }

    /// <summary>A row whose Name is read inside an @if and outside it.</summary>
    public class GateItem : ObservableObject
    {
        [AutoFire] public bool Flag { get; set; }
        [AutoFire] public string Name { get; set; }

        public GateItem(string name)
        {
            this.Name = name;
        }
    }

    public class GateVm : ObservableObject
    {
        [AutoFire] public bool Flag { get; set; }
        [AutoFire] public string Name { get; set; }
        public ObservableCollection<GateItem> Items { get; private set; }

        public GateVm()
        {
            this.Name = "top";
            this.Items = new ObservableCollection<GateItem>();
        }
    }

    /// <summary>Values for id, data-* and aria-* on a control tag.</summary>
    public class HostAttrVm : ObservableObject
    {
        [AutoFire] public string Label { get; set; }
        [AutoFire] public string HostId { get; set; }
        public ObservableCollection<NestItem> Rows { get; private set; }

        public HostAttrVm()
        {
            this.Label = "l1";
            this.HostId = "h1";
            this.Rows = new ObservableCollection<NestItem>();
        }
    }

    /// <summary>A caller that picks its frame's skin from a property.</summary>
    public class SkinChoiceVm : ObservableObject
    {
        [AutoFire] public Skin FrameSkin { get; set; }
        [AutoFire] public string Message { get; set; }
        [AutoFire] public SlotFrameVM Frame { get; set; }
        public int ConfirmCount;

        public SkinChoiceVm()
        {
            this.FrameSkin = SlotFrame.DefaultSkin;
            this.Message = "m1";
            this.Frame = new SlotFrameVM("t1");
        }

        public void Confirm(Element elem, ElementEvent evt)
        {
            this.ConfirmCount++;
        }
    }

    /// <summary>
    /// Compiler follow-ups to the AWB Razor work: handlers in a nested @foreach (#147),
    /// host attributes on a control tag, and a bound Skin on a control tag.
    /// </summary>
    [TestFixture]
    public class RazorCompilerFollowupTests
    {
        private static Element Mount(object vm, Skin skin)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            control.DataContext = vm;
            control.Skin = skin;
            control.Activate();
            return element;
        }

        private static void Click(Element root, string test, int index)
        {
            root.QuerySelectorAll("[data-test=" + test + "]")[index].Click();
        }

        // #147: Model.M(x) in the inner loop, with the inner item and with the outer item.
        [Test]
        public static void TestR147NestedLoopModelHandlers(Assert assert)
        {
            var vm = new NestVm();
            var g0 = new NestItem("g0");
            var g1 = new NestItem("g1");
            g0.Tabs.Add(new NestItem("g0t0"));
            g1.Tabs.Add(new NestItem("g1t0"));
            g1.Tabs.Add(new NestItem("g1t1"));
            vm.Groups.Add(g0);
            vm.Groups.Add(g1);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbR147NestedHandler);
            assert.Equal(root.QuerySelectorAll("[data-test=n-inner]").Length, 3, "3 inner rows");

            Click(root, "n-inner", 2);
            assert.Equal(vm.Opened, g1.Tabs[1], "inner button: Model.OpenTab(tab) gets its tab");
            Click(root, "n-outer", 1);
            assert.Equal(vm.Opened, g1, "outer button: Model.OpenTab(item) gets its item");
            Click(root, "n-inner-outer", 0);
            assert.Equal(vm.Opened, g0, "inner button: Model.OpenTab(item) gets the outer item");
            Click(root, "n-inner-plain", 1);
            assert.Equal(vm.PlainClicks, 1, "inner button: Model.OnPlain reaches the model");
            assert.Equal(vm.OpenCount, 3, "one call per click");

            var added = new NestItem("g0t1");
            g0.Tabs.Add(added);
            Click(root, "n-inner", 1);
            assert.Equal(vm.Opened, added, "an inner row added later gets its own tab");
        }

        // id, data-* and aria-* on a control tag are attributes of its host element.
        [Test]
        public static void TestHostAttributesOnControlTag(Assert assert)
        {
            var vm = new HostAttrVm();
            vm.Rows.Add(new NestItem("r0"));
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuHostAttrs);

            var fixedHost = root.QuerySelector("[data-test=ha-static]");
            assert.NotEqual(fixedHost, null, "static data-test is on the host");
            if (fixedHost == null) return;
            assert.Equal(fixedHost.GetAttribute("id"), "ha-id", "static id is on the host");
            assert.Equal(fixedHost.GetAttribute("aria-label"), "static label", "static aria-label is on the host");
            assert.Equal(fixedHost.GetAttribute("data-bound-text"), "t", "a real property still binds");

            var bound = root.QuerySelector("[data-test=ha-bound]");
            assert.Equal(bound.GetAttribute("data-x"), "l1", "bound data-x on mount");
            assert.Equal(bound.GetAttribute("aria-label"), "l1", "bound aria-label on mount");
            assert.Equal(bound.GetAttribute("id"), "h1", "bound id on mount");

            vm.Label = "l2";
            vm.HostId = "h2";
            assert.Equal(bound.GetAttribute("data-x"), "l2", "bound data-x updates");
            assert.Equal(bound.GetAttribute("aria-label"), "l2", "bound aria-label updates");
            assert.Equal(bound.GetAttribute("id"), "h2", "bound id updates");

            vm.Label = null;
            assert.IsTrue(!bound.HasAttribute("data-x"), "null removes data-x");
            assert.IsTrue(!bound.HasAttribute("aria-label"), "null removes aria-label");

            var row = root.QuerySelector("[data-test=ha-row]");
            assert.Equal(row.GetAttribute("data-name"), "r0", "row data-name on mount");
            vm.Rows[0].Name = "r1";
            assert.Equal(row.GetAttribute("data-name"), "r1", "row data-name updates");
        }

        // A bound Skin re-skins the control; its caller content stays live.
        [Test]
        public static void TestBoundSkinReskinsAndKeepsContent(Assert assert)
        {
            var vm = new SkinChoiceVm();
            vm.Frame.IsOpen = true;
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuSkinBinding);
            assert.Equal(root.QuerySelectorAll("p.msg").Length, 1, "default skin: one copy of the content");
            assert.NotEqual(root.QuerySelector("section.frame-body > p.msg"), null, "default skin: content in the outlet");
            assert.Equal(root.QuerySelector("div.frame-gate"), null, "default skin: no gated frame");

            vm.FrameSkin = RazorSkinTemplatesClass.SlotGatedFrame;
            assert.NotEqual(root.QuerySelector("div.frame-gate section.frame-body > p.msg"), null,
                "bound skin changed: content in the new skin's outlet");
            assert.Equal(root.QuerySelectorAll("p.msg").Length, 1, "bound skin changed: one copy of the content");
            vm.Message = "m2";
            assert.Equal(root.QuerySelector("p.msg").TextContent, "m2", "content binding still live");
            root.QuerySelector("button.confirm").Click();
            assert.Equal(vm.ConfirmCount, 1, "content click still live");

            vm.FrameSkin = SlotFrame.DefaultSkin;
            assert.Equal(root.QuerySelector("div.frame-gate"), null, "back to the default skin");
            assert.NotEqual(root.QuerySelector("section.frame-body > p.msg"), null, "content back in the default outlet");
            vm.Message = "m3";
            assert.Equal(root.QuerySelector("p.msg").TextContent, "m3", "content binding live after the second swap");
        }

        private static NestVm NestedVm()
        {
            var vm = new NestVm();
            var g0 = new NestItem("g0");
            var g1 = new NestItem("g1");
            g0.Tabs.Add(new NestItem("g0t0"));
            g1.Tabs.Add(new NestItem("g1t0"));
            g1.Tabs.Add(new NestItem("g1t1"));
            vm.Groups.Add(g0);
            vm.Groups.Add(g1);
            return vm;
        }

        private static void Dispatch(Element target, string type)
        {
            var evt = Window.Instance.Document.CreateEvent("Event");
            evt.InitEvent(type, true, true);
            target.DispatchEvent(evt);
        }

        private static string Joined(Element root, string selector, bool attribute)
        {
            var nodes = root.QuerySelectorAll(selector);
            string text = "";
            for (int i = 0; i < nodes.Length; i++)
            {
                var node = (Element)nodes[i];
                text += (i > 0 ? "|" : "") + (attribute ? node.GetAttribute("title") : node.TextContent);
            }
            return text;
        }

        // Model.X text, attribute and value bindings in the inner loop read the model.
        [Test]
        public static void TestNestedLoopModelDataBindings(Assert assert)
        {
            var vm = NestedVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuNestedModel);
            assert.Equal(Joined(root, "[data-test=m-row] b", false), "t1|t1|t1", "inner text reads Model.Title");
            assert.Equal(Joined(root, "[data-test=m-row]", true), "t1|t1|t1", "inner attribute reads Model.Title");
            var input = (InputElement)root.QuerySelectorAll("[data-test=m-input]")[1];
            assert.Equal(input.Value, "t1", "inner value reads Model.Title");

            vm.Title = "t2";
            assert.Equal(Joined(root, "[data-test=m-row] b", false), "t2|t2|t2", "inner text follows Model.Title");
            assert.Equal(Joined(root, "[data-test=m-row]", true), "t2|t2|t2", "inner attribute follows Model.Title");
            assert.Equal(input.Value, "t2", "inner value follows Model.Title");

            input.Value = "t3";
            Dispatch(input, "input");
            assert.Equal(vm.Title, "t3", "inner two-way value writes Model.Title");
            assert.Equal(Joined(root, "[data-test=m-row] b", false), "t3|t3|t3", "every row sees the write");

            vm.Groups[0].Tabs.Add(new NestItem("g0t1"));
            assert.Equal(Joined(root, "[data-test=m-row] b", false), "t3|t3|t3|t3", "a row added later reads Model.Title");
            vm.Title = "t4";
            assert.Equal(Joined(root, "[data-test=m-row] b", false), "t4|t4|t4|t4", "a row added later follows Model.Title");

            vm.Groups[0].Tabs.RemoveAt(0);
            vm.Title = "t5";
            assert.Equal(Joined(root, "[data-test=m-row] b", false), "t5|t5|t5", "removing a row keeps the others live");
        }

        // outerItem.M as a plain handler in the inner loop calls it on the outer item.
        [Test]
        public static void TestNestedLoopOuterItemHandler(Assert assert)
        {
            var vm = NestedVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuOuterItemHandler);
            Click(root, "o-outer", 2);
            assert.Equal(vm.Groups[1].Picks, 1, "inner row of g1: item.Pick runs on g1");
            assert.Equal(vm.Groups[0].Picks, 0, "g0 untouched");
            Click(root, "o-outer", 0);
            assert.Equal(vm.Groups[0].Picks, 1, "inner row of g0: item.Pick runs on g0");
            assert.Equal(vm.Groups[1].Tabs[0].Picks, 0, "the inner item is not the target");
        }

        // value="@Model.X" in a single-level loop writes back to the model.
        [Test]
        public static void TestLoopModelValueTwoWay(Assert assert)
        {
            var vm = NestedVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuLoopModelValue);
            var inputs = root.QuerySelectorAll("[data-test=s-input]");
            assert.Equal(inputs.Length, 2, "one input per row");
            var first = (InputElement)inputs[0];
            var second = (InputElement)inputs[1];
            assert.Equal(second.Value, "t1", "row value reads Model.Title");

            second.Value = "w1";
            Dispatch(second, "input");
            assert.Equal(vm.Title, "w1", "row input event writes Model.Title");
            assert.Equal(first.Value, "w1", "the other row sees the write");

            first.Value = "w2";
            Dispatch(first, "change");
            assert.Equal(vm.Title, "w2", "row change event writes Model.Title");

            vm.Title = "w3";
            assert.Equal(second.Value, "w3", "row value follows Model.Title");
        }

        private static string Text(Element root, string test)
        {
            var element = root.QuerySelector("[data-test=" + test + "]");
            return element == null ? null : element.TextContent;
        }

        // A value read inside an @if and outside it still shows outside while the @if is false.
        [Test]
        public static void TestSharedValueOutsideClosedIf(Assert assert)
        {
            var vm = new GateVm();
            var row = new GateItem("z");
            vm.Items.Add(row);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuSharedGate);
            assert.Equal(Text(root, "g-top-out"), "top", "top <b> shows Name while Flag=false");
            assert.Equal(Text(root, "g-top-in"), null, "top <u> not rendered");
            assert.Equal(Text(root, "g-out"), "z", "row <b> shows Name while Flag=false");
            assert.Equal(Text(root, "g-in"), null, "row <u> not rendered");

            vm.Flag = true;
            row.Flag = true;
            assert.Equal(Text(root, "g-top-in") + "|" + Text(root, "g-top-out"), "top|top", "top: both show Name");
            assert.Equal(Text(root, "g-in") + "|" + Text(root, "g-out"), "z|z", "row: both show Name");

            vm.Name = "top2";
            row.Name = "z2";
            assert.Equal(Text(root, "g-top-in") + "|" + Text(root, "g-top-out"), "top2|top2", "top: both follow Name");
            assert.Equal(Text(root, "g-in") + "|" + Text(root, "g-out"), "z2|z2", "row: both follow Name");

            vm.Flag = false;
            row.Flag = false;
            assert.Equal(Text(root, "g-top-out"), "top2", "top <b> keeps Name after the @if closes");
            assert.Equal(Text(root, "g-out"), "z2", "row <b> keeps Name after the @if closes");
            vm.Name = "top3";
            row.Name = "z3";
            assert.Equal(Text(root, "g-top-out"), "top3", "top <b> follows Name while closed");
            assert.Equal(Text(root, "g-out"), "z3", "row <b> follows Name while closed");

            vm.Flag = true;
            row.Flag = true;
            assert.Equal(Text(root, "g-top-in"), "top3", "top <u> shows the latest Name on reopen");
            assert.Equal(Text(root, "g-in"), "z3", "row <u> shows the latest Name on reopen");
        }

        // A control in @if and text in @else read the same row value: the control gets it.
        [Test]
        public static void TestSubControlInIfSharesValueWithElse(Assert assert)
        {
            var vm = new GateVm();
            var row = new GateItem("z");
            row.Flag = true;
            vm.Items.Add(row);
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuSubGate);
            var probe = root.QuerySelector("[data-test=sg-probe]");
            assert.NotEqual(probe, null, "control rendered while Flag=true");
            assert.Equal(probe.GetAttribute("data-probe-value"), "z", "control Value reads i.Name");
            assert.Equal(row.Name, "z", "two-way control did not write a blank back");

            row.Name = "n2";
            assert.Equal(probe.GetAttribute("data-probe-value"), "n2", "control follows i.Name");

            row.Flag = false;
            assert.Equal(Text(root, "sg-text"), "n2", "@else text shows i.Name");
            row.Name = "n3";
            assert.Equal(Text(root, "sg-text"), "n3", "@else text follows i.Name");

            row.Flag = true;
            probe = root.QuerySelector("[data-test=sg-probe]");
            assert.Equal(probe.GetAttribute("data-probe-value"), "n3", "reopened control shows the latest i.Name");
            assert.Equal(row.Name, "n3", "reopened control did not write a blank back");
        }

        // A two-way sub-control property bound to Model.X in an inner loop writes to the model.
        [Test]
        public static void TestNestedLoopSubControlTwoWay(Assert assert)
        {
            var vm = NestedVm();
            var root = Mount(vm, RazorSkinTemplatesClass.AwbFuNestedSubValue);
            var probes = root.QuerySelectorAll("[data-test=sv-probe]");
            assert.Equal(probes.Length, 3, "one probe per inner row");
            assert.Equal(((Element)probes[0]).GetAttribute("data-probe-value"), "t1", "probe reads Model.Title");

            vm.Title = "t2";
            assert.Equal(((Element)probes[2]).GetAttribute("data-probe-value"), "t2", "probe follows Model.Title");

            var last = RazorValueProbeControl.LastCreated;
            last.Value = "c1";
            assert.Equal(vm.Title, "c1", "probe write reaches Model.Title, not the outer row");
            assert.Equal(((Element)probes[0]).GetAttribute("data-probe-value"), "c1", "other probes see the write");
            assert.Equal(vm.Groups[1].Name, "g1", "the outer item is untouched");
        }
    }
}

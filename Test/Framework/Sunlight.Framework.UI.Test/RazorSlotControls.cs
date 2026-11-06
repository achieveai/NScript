namespace Sunlight.Framework.UI.Test
{
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;
    using System.Web.Html;

    /// <summary>
    /// A frame control that takes caller content (issue #137). Its default skin places the
    /// content under a header; tests swap in gated and @if/@else frame skins.
    /// </summary>
    public class SlotFrame : UISkinableElement
    {
        public static SlotFrame LastCreated;

        public SlotFrame(Element element)
            : base(element)
        {
            LastCreated = this;
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotFrame.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }
    }

    /// <summary>A frame with named slots: @RenderSlot("head"), @RenderBody() and a gated @RenderSlot("actions").</summary>
    public class SlotPanel : UISkinableElement
    {
        public static SlotPanel LastCreated;

        // Set once until a test clears it: with panels nested in caller content it tells
        // the outer and inner panels apart without relying on creation order.
        public static SlotPanel FirstCreated;

        public SlotPanel(Element element)
            : base(element)
        {
            LastCreated = this;
            if (FirstCreated == null) FirstCreated = this;
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.SlotPanel.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }
    }

    /// <summary>The frame's own view model.</summary>
    public class SlotFrameVM : ObservableObject
    {
        [AutoFire] public string Title { get; set; }
        [AutoFire] public bool IsOpen { get; set; }
        [AutoFire] public bool ShowInner { get; set; }
        public int InnerClicks;

        public void OnInnerClick(Element elem, ElementEvent evt)
        {
            this.InnerClicks++;
        }

        public SlotFrameVM(string title)
        {
            this.Title = title;
        }
    }

    /// <summary>The caller's view model: the content binds here, not to the frame.</summary>
    public class SlotHostVM : ObservableObject
    {
        [AutoFire] public string Message { get; set; }
        [AutoFire] public bool ShowExtra { get; set; }
        [AutoFire] public SlotFrameVM Frame { get; set; }
        public ObservableCollection<SlotFrameVM> Rows { get; private set; }
        public int ConfirmCount;
        public int ExtraClicks;
        public int FrameDblClicks;

        public SlotHostVM()
        {
            this.Message = "m1";
            this.Frame = new SlotFrameVM("t1");
            this.Rows = new ObservableCollection<SlotFrameVM>();
        }

        public void Confirm(Element elem, ElementEvent evt)
        {
            this.ConfirmCount++;
        }

        public void OnExtraClick(Element elem, ElementEvent evt)
        {
            this.ExtraClicks++;
        }

        public void OnFrameDblClick(Element elem, ElementEvent evt)
        {
            this.FrameDblClicks++;
        }
    }
}

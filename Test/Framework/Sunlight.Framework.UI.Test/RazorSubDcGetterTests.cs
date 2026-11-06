namespace Sunlight.Framework.UI.Test
{
    using System.Web.Html;
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI.Attributes;
    using Sunlight.Framework.UI.Helpers;
    using SunlightUnit;

    /// <summary>Loop item for issue #141. Kid is read only by the skin, never from C#.</summary>
    public class SubDcItem : ObservableObject
    {
        private SubDcVm kid;

        [AutoFire] public string Name { get; set; }

        // Do not read Kid anywhere in C#: a C# read keeps the getter alive and hides the bug.
        public SubDcVm Kid
        {
            get
            {
                if (this.kid == null)
                {
                    this.kid = new SubDcVm();
                    this.kid.Text = "kid-" + this.Name;
                }

                return this.kid;
            }
        }
    }

    /// <summary>View model for issue #141: the outer list and each kid's text.</summary>
    public class SubDcVm : ObservableObject
    {
        [AutoFire] public string Text { get; set; }
        public ObservableCollection<SubDcItem> Items { get; set; }
    }

    /// <summary>Sub-control whose DataContext is bound to SubDcItem.Kid.</summary>
    public class SubDcKidView : UISkinableElement
    {
        public SubDcKidView(Element element) : base(element)
        {
        }

        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubDcKid.skin.cshtml")]
        public static Skin DefaultSkin
        {
            get { return null; }
        }
    }

    /// <summary>Skin registration for the issue #141 fixture.</summary>
    public class RazorSubDcGetterTemplates
    {
        [Skin("Sunlight.Framework.UI.Test.RazorTemplates.RazorSubDcGetter.skin.cshtml")]
        public static Skin RazorSubDcGetter
        {
            get { return null; }
        }
    }

    /// <summary>
    /// Issue #141: a getter read only through a sub-control DataContext="@i.Kid" must survive
    /// dead-code elimination, or mounting throws "get_Kid is not a function".
    /// </summary>
    [TestFixture]
    public class RazorSubDcGetterTests
    {
        [Test]
        public static void TestSubControlDataContextGetterReadOnlyBySkin(Assert assert)
        {
            var vm = new SubDcVm();
            var items = new ObservableCollection<SubDcItem>();
            items.Add(new SubDcItem { Name = "n0" });
            items.Add(new SubDcItem { Name = "n1" });
            vm.Items = items;

            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);
            control.DataContext = vm;
            control.Skin = RazorSubDcGetterTemplates.RazorSubDcGetter;
            control.Activate();

            var spans = element.QuerySelectorAll("[data-test=kid-text]");
            assert.Equal(2, spans.Length, "Each loop item should mount one kid view");
            if (spans.Length != 2) return;
            assert.Equal("kid-n0", spans[0].TextContent, "The first kid view should bind to the first item's Kid");
            assert.Equal("kid-n1", spans[1].TextContent, "The second kid view should bind to the second item's Kid");
        }
    }
}

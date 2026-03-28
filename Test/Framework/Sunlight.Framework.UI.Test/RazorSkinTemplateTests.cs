namespace Sunlight.Framework.UI.Test
{
    using SunlightUnit;
    using System.Web.Html;

    /// <summary>
    /// Browser-based tests for Razor skin templates.
    /// These tests verify that .skin.cshtml templates compiled through the full
    /// NScript pipeline produce correct runtime behavior in the browser.
    /// </summary>
    [TestFixture]
    public class RazorSkinTemplateTests
    {
        [TestSetup]
        public static void Setup()
        {
            TaskScheduler.Instance = new TaskScheduler(
                new TestWindowTimer(),
                10,
                10);
        }

        /// <summary>
        /// Phase 1: Simplest possible test. Verifies the full toolchain:
        /// .skin.cshtml → RazorTemplatingPlugin → JS factory → SkinInstance → DOM.
        /// </summary>
        [Test]
        public static void TestRazorSimpleTextBinding(Assert assert)
        {
            var element = Window.Instance.Document.CreateElement("div");
            var control = new UISkinableElement(element);

            var vm = new TestViewModelA();
            vm.PropStr1 = "Hello Razor";
            control.DataContext = vm;
            control.Skin = RazorSkinTemplatesClass.RazorSimpleText;

            assert.NotEqual(null, control.Skin, "Razor skin should be compiled and available");

            control.Activate();

            var span = element.QuerySelector("[data-test] span");
            assert.NotEqual(null, span, "Skin should render a span element inside the data-test div");
            assert.Equal("Hello Razor", span.TextContent,
                "Span text content should match the bound PropStr1 value");
        }
    }
}

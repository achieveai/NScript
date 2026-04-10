using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NScript.JST;
using NScript.RazorSkin;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class RazorCssManagerTests
    {
        private const string SimpleCss = @"
.header { font-weight: bold; }
.content { padding: 10px; }
.footer { margin-top: 20px; }
";

        private const string SecondCss = @"
.sidebar { width: 200px; }
.header .nested { color: red; }
";

        [TestMethod]
        public void AddStylesheet_RegistersClassNames()
        {
            var manager = new RazorCssManager();
            manager.AddStylesheet("test.css", SimpleCss);

            IIdentifier id;
            manager.TryGetCssClassIdentifier("header", out id).Should().BeTrue();
            manager.TryGetCssClassIdentifier("content", out id).Should().BeTrue();
            manager.TryGetCssClassIdentifier("footer", out id).Should().BeTrue();
        }

        [TestMethod]
        public void TryGetCssClassIdentifier_ReturnsFalseForUnknownClass()
        {
            var manager = new RazorCssManager();
            manager.AddStylesheet("test.css", SimpleCss);

            IIdentifier id;
            manager.TryGetCssClassIdentifier("nonexistent", out id).Should().BeFalse();
            id.Should().BeNull();
        }

        [TestMethod]
        public void MultipleStylesheets_OrderPreserved()
        {
            var manager = new RazorCssManager();
            manager.AddStylesheet("base.css", SimpleCss);
            manager.AddStylesheet("extra.css", SecondCss);

            manager.Sheets.Should().HaveCount(2);
            manager.Sheets[0].ResourceName.Should().Be("base.css");
            manager.Sheets[1].ResourceName.Should().Be("extra.css");
        }

        [TestMethod]
        public void MultipleStylesheets_CanResolveFromBoth()
        {
            var manager = new RazorCssManager();
            manager.AddStylesheet("base.css", SimpleCss);
            manager.AddStylesheet("extra.css", SecondCss);

            IIdentifier id;
            manager.TryGetCssClassIdentifier("header", out id).Should().BeTrue();
            manager.TryGetCssClassIdentifier("sidebar", out id).Should().BeTrue();
        }

        [TestMethod]
        public void TopLevelRedeclaration_Throws()
        {
            var manager = new RazorCssManager();
            manager.AddStylesheet("base.css", ".myClass { color: red; }");

            Action act = () => manager.AddStylesheet("second.css", ".myClass { color: blue; }");

            act.Should().Throw<NScript.Converter.ConverterLocationException>()
                .WithMessage("*already declared*");
        }

        [TestMethod]
        public void NestedReuse_Allowed()
        {
            var manager = new RazorCssManager();
            manager.AddStylesheet("base.css", ".myClass { color: red; }");

            // Nested selector reusing .myClass from base is OK
            Action act = () => manager.AddStylesheet("second.css",
                ".container .myClass { font-weight: bold; }");

            act.Should().NotThrow();
        }

        [TestMethod]
        public void GetSerializedCss_ReturnsContent()
        {
            var manager = new RazorCssManager();
            manager.AddStylesheet("test.css", ".test { color: red; }");

            var css = manager.GetSerializedCss();

            css.Should().NotBeNullOrEmpty();
            css.Should().Contain("color:red");
        }

        [TestMethod]
        public void ReplaceCssClassNames_UsesResolvedIdentifiers()
        {
            var manager = new RazorCssManager();
            manager.AddStylesheet("test.css", ".header { color: red; } .footer { margin: 0; }");

            // Before compression, identifiers should resolve
            IIdentifier headerId;
            manager.TryGetCssClassIdentifier("header", out headerId).Should().BeTrue();

            IIdentifier footerId;
            manager.TryGetCssClassIdentifier("footer", out footerId).Should().BeTrue();

            // ReplaceCssClassNames should use the identifier's GetName()
            var replaced = manager.ReplaceCssClassNames("header footer");

            // The resolved names should be used (before compression, they match originals)
            replaced.Should().Contain(headerId.GetName());
            replaced.Should().Contain(footerId.GetName());
        }

        [TestMethod]
        public void HasStylesheets_FalseWhenEmpty()
        {
            var manager = new RazorCssManager();
            manager.HasStylesheets.Should().BeFalse();
        }

        [TestMethod]
        public void HasStylesheets_TrueAfterAdding()
        {
            var manager = new RazorCssManager();
            manager.AddStylesheet("test.css", ".test { color: red; }");
            manager.HasStylesheets.Should().BeTrue();
        }

        [TestMethod]
        public void CssVariableValidation_DeclaredAndUsed_Passes()
        {
            var manager = new RazorCssManager();
            manager.AddStylesheet("test.css",
                ":root { --main-color: red; } .test { color: var(--main-color); }");

            Action act = () => manager.ValidateCssVariables();
            act.Should().NotThrow();
        }

        [TestMethod]
        public void CssVariableValidation_UndeclaredVariable_Throws()
        {
            var manager = new RazorCssManager();

            // The undeclared variable may be caught during AddStylesheet or ValidateCssVariables
            Action act = () =>
            {
                manager.AddStylesheet("test.css",
                    ".test { color: var(--unknown-color); }");
                manager.ValidateCssVariables();
            };

            act.Should().Throw<NScript.Converter.ConverterLocationException>()
                .WithMessage("*--unknown-color*");
        }

        [TestMethod]
        public void MediaRules_ClassNamesRegistered()
        {
            var manager = new RazorCssManager();
            manager.AddStylesheet("test.css",
                ".normal { color: red; } @media (max-width: 600px) { .mobile { font-size: 14px; } }");

            IIdentifier id;
            manager.TryGetCssClassIdentifier("normal", out id).Should().BeTrue();
            manager.TryGetCssClassIdentifier("mobile", out id).Should().BeTrue();
        }
    }
}

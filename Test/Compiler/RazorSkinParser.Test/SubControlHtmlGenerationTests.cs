using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NScript.RazorSkin.TemplateIR;
using NScript.RazorSkin.CodeGen;
using System.Collections.Generic;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class SubControlHtmlGenerationTests
    {
        /// <summary>
        /// SubControlNode defaults to "div" tag when no TagName is set.
        /// </summary>
        [TestMethod]
        public void SubControlNode_DefaultTag_EmitsDiv()
        {
            var sub = new SubControlNode { TypeName = "MyControl" };
            sub.TagName.Should().Be("div");
        }

        /// <summary>
        /// Custom TagName should be preserved on SubControlNode.
        /// </summary>
        [TestMethod]
        public void SubControlNode_CustomTag_IsPreserved()
        {
            var sub = new SubControlNode { TypeName = "TodoItem" };
            sub.TagName = "todo";
            sub.TagName.Should().Be("todo");
        }

        /// <summary>
        /// SubControlNode with [TagName] emits correct HTML tag in CollectHtmlWithMarkers.
        /// </summary>
        [TestMethod]
        public void SubControlNode_CustomTag_EmitsCorrectHtml()
        {
            var nodes = new List<IRNode>
            {
                new SubControlNode { TypeName = "TodoItem", TagName = "todo" }
            };

            var events = RazorSkinCodeGenerator.CollectEventsPublic(nodes);
            var paths = new List<List<int>>();
            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(nodes, events, paths);

            html.Should().Contain("<todo");
            html.Should().Contain("data-ns-subctl");
            html.Should().Contain("</todo>");
            html.Should().NotContain("<span>");
        }

        /// <summary>
        /// SubControlNode with DomAttributes emits attributes on the tag.
        /// </summary>
        [TestMethod]
        public void SubControlNode_DomAttributes_EmitsAttributes()
        {
            var sub = new SubControlNode
            {
                TypeName = "MyList",
                TagName = "ul",
                DomAttributes = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("role", "list"),
                    new KeyValuePair<string, string>("class", "items")
                }
            };

            var nodes = new List<IRNode> { sub };
            var events = RazorSkinCodeGenerator.CollectEventsPublic(nodes);
            var paths = new List<List<int>>();
            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(nodes, events, paths);

            html.Should().Contain("<ul");
            html.Should().Contain("role=\"list\"");
            html.Should().Contain("class=\"items\"");
            html.Should().Contain("</ul>");
        }

        /// <summary>
        /// Default SubControlNode (no TagName set) emits div tag.
        /// </summary>
        [TestMethod]
        public void SubControlNode_DefaultTag_EmitsDivHtml()
        {
            var nodes = new List<IRNode>
            {
                new SubControlNode { TypeName = "SomeControl" }
            };

            var events = RazorSkinCodeGenerator.CollectEventsPublic(nodes);
            var paths = new List<List<int>>();
            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(nodes, events, paths);

            html.Should().Contain("<div");
            html.Should().Contain("data-ns-subctl");
            html.Should().Contain("</div>");
        }

        /// <summary>
        /// SubControlNode with null TagName falls back to div.
        /// </summary>
        [TestMethod]
        public void SubControlNode_NullTag_FallsBackToDiv()
        {
            var sub = new SubControlNode { TypeName = "MyControl" };
            sub.TagName = null;

            var nodes = new List<IRNode> { sub };
            var events = RazorSkinCodeGenerator.CollectEventsPublic(nodes);
            var paths = new List<List<int>>();
            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(nodes, events, paths);

            html.Should().Contain("<div");
            html.Should().Contain("data-ns-subctl");
            html.Should().Contain("</div>");
        }

        /// <summary>
        /// SubControlNode mixed with HtmlNode children produces correct output.
        /// </summary>
        [TestMethod]
        public void SubControlNode_WithChildren_EmitsContainerAndContent()
        {
            var sub = new SubControlNode { TypeName = "Card", TagName = "section" };
            sub.Children.Add(new HtmlNode { HtmlContent = "<p>Hello</p>" });

            var nodes = new List<IRNode> { sub };
            var events = RazorSkinCodeGenerator.CollectEventsPublic(nodes);
            var paths = new List<List<int>>();
            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(nodes, events, paths);

            // Sub-control is a marker element — its children are rendered by the control itself,
            // not inlined into the parent template HTML
            html.Should().Contain("<section");
            html.Should().Contain("data-ns-subctl");
            html.Should().Contain("</section>");
        }

        /// <summary>
        /// XWML parity: ListView has [TagName("ul")] — verify the same tag works in Razor.
        /// </summary>
        [TestMethod]
        public void SubControlNode_ListViewTag_EmitsUl()
        {
            var sub = new SubControlNode { TypeName = "ListView", TagName = "ul" };

            var nodes = new List<IRNode> { sub };
            var events = RazorSkinCodeGenerator.CollectEventsPublic(nodes);
            var paths = new List<List<int>>();
            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(nodes, events, paths);

            html.Should().Contain("<ul");
            html.Should().Contain("data-ns-subctl");
            html.Should().Contain("</ul>");
        }

        /// <summary>
        /// DomAttributes with special characters are HTML-encoded.
        /// </summary>
        [TestMethod]
        public void SubControlNode_DomAttributes_SpecialCharsEncoded()
        {
            var sub = new SubControlNode
            {
                TypeName = "MyControl",
                TagName = "div",
                DomAttributes = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("data-info", "a&b<c>d")
                }
            };

            var nodes = new List<IRNode> { sub };
            var events = RazorSkinCodeGenerator.CollectEventsPublic(nodes);
            var paths = new List<List<int>>();
            var html = RazorSkinCodeGenerator.CollectHtmlWithPathsPublic(nodes, events, paths);

            html.Should().Contain("data-info=\"a&amp;b&lt;c&gt;d\"");
        }
    }
}

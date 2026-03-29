using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.RazorSkin.TemplateIR;
using Serilog;

namespace NScript.RazorSkin.CodeGen
{
    public static class RazorSkinCodeGenerator
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        // --- Binding and event collection ---

        private static List<ExpressionBindingNode> CollectBindings(List<IRNode> nodes)
            => CollectNodes<ExpressionBindingNode>(nodes);

        private static List<EventNode> CollectEvents(List<IRNode> nodes)
        {
            var result = new List<EventNode>();
            foreach (var node in nodes)
            {
                if (node is EventNode evt)
                    result.Add(evt);
                // Sub-control events are NOT collected here — they are emitted
                // separately in EmitSubControlFactoryCalls via SubControlNode.EventBindings
                result.AddRange(CollectEvents(node.Children));
            }
            return result;
        }

        /// <summary>
        /// Generic collection of typed IR nodes from the tree (M5).
        /// </summary>
        private static List<T> CollectNodes<T>(List<IRNode> nodes) where T : IRNode
        {
            var result = new List<T>();
            foreach (var node in nodes)
            {
                if (node is T typed)
                    result.Add(typed);
                result.AddRange(CollectNodes<T>(node.Children));
            }
            return result;
        }

        // --- Public accessors for RazorSkinJSTGenerator ---

        public static List<ExpressionBindingNode> CollectBindingsPublic(List<IRNode> nodes)
            => CollectBindings(nodes);

        public static List<EventNode> CollectEventsPublic(List<IRNode> nodes)
            => CollectEvents(nodes);

        public static string CollectHtmlWithPathsPublic(
            List<IRNode> nodes, List<EventNode> eventTracker, List<List<int>> outPaths)
            => CollectHtmlWithPaths(nodes, eventTracker, outPaths);

        public static string CollectHtmlPublic(List<IRNode> nodes)
            => CollectHtml(nodes);

        /// <summary>
        /// Builds the innerHTML string AND computes DOM paths for each binding placeholder.
        /// Uses marker attributes (data-bind-idx) during construction, then parses the
        /// final HTML to compute the actual DOM path for each marker.
        /// </summary>
        private static string CollectHtmlWithPaths(
            List<IRNode> nodes, List<EventNode> eventTracker, List<List<int>> outPaths)
        {
            // Phase 1: Build HTML with marker attributes on binding placeholders
            int bindingIdx = 0;
            var rawHtml = CollectHtmlWithMarkers(nodes, eventTracker, ref bindingIdx);

            // Phase 2: Parse the HTML to find each marker's DOM path
            ComputePathsFromHtml(rawHtml, bindingIdx, outPaths);

            // Phase 3: Strip the marker attributes from the final HTML
            var cleanHtml = System.Text.RegularExpressions.Regex.Replace(
                rawHtml, @" data-bind-idx=""\d+""", "");
            return cleanHtml;
        }

        private static string CollectHtmlWithMarkers(
            List<IRNode> nodes, List<EventNode> eventTracker, ref int bindingIdx)
        {
            var sb = new StringBuilder();
            foreach (var node in nodes)
            {
                if (node is HtmlNode html)
                    sb.Append(html.HtmlContent);
                else if (node is ExpressionBindingNode exprBinding)
                {
                    if (exprBinding.Target == ExpressionTarget.TextContent)
                    {
                        // Text bindings get a span placeholder in the HTML
                        sb.Append($"<span data-bind-idx=\"{bindingIdx}\"></span>");
                    }
                    else
                    {
                        // Attribute/Class/Style bindings target the element itself.
                        // The preceding HTML is an unclosed opening tag like '<div data-test="1"'
                        // (the attribute was stripped by the IR builder). Append the marker
                        // as an attribute on that element.
                        sb.Append($" data-bind-idx=\"{bindingIdx}\"");
                    }
                    bindingIdx++;
                }
                else if (node is EventNode)
                {
                    // Events are wired via objStorage element indices, not HTML markers.
                }
                else if (node is ConditionalNode cond)
                {
                    // Gate: emit only an empty marker span. Branch content is stored
                    // as HTML strings in the GateTargetInfo (trueTemplate/falseTemplate)
                    // and dynamically cloned by GraphEngine at runtime.
                    sb.Append("<span></span>");
                    // Still need to recurse into branches so any nested bindings get
                    // their bindingIdx allocated (though they won't have HTML markers)
                }
                else if (node is LoopNode loop)
                {
                    // Collection: emit only an empty marker span. Item template content
                    // is stored as an HTML string in the CollectionTargetInfo and rendered
                    // by GraphEngine for each collection item.
                    sb.Append("<span></span>");
                }
                else if (node is SubControlNode)
                {
                    sb.Append("<span></span>");
                }
                else
                {
                    sb.Append(CollectHtmlWithMarkers(node.Children, eventTracker, ref bindingIdx));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Simple HTML collector for reactive block template fragments (no path tracking needed).
        /// </summary>
        private static string CollectHtml(List<IRNode> nodes)
        {
            int unused = 0;
            var html = CollectHtmlWithMarkers(nodes, new List<EventNode>(), ref unused);
            // Strip any binding markers from the HTML
            return System.Text.RegularExpressions.Regex.Replace(html, @" data-bind-idx=""\d+""", "");
        }

        /// <summary>
        /// Parses HTML to find data-bind-idx markers and compute their DOM tree path.
        /// Tracks open/close tags to maintain a nesting stack with child counts.
        /// </summary>
        private static void ComputePathsFromHtml(string html, int bindingCount, List<List<int>> outPaths)
        {
            var pathMap = new Dictionary<int, List<int>>();

            // Track nesting: stack of (myIndex, childCount) pairs.
            // myIndex = the 0-based index of this element within its parent
            // childCount = number of child elements seen so far at this level
            var indexStack = new List<int>();   // myIndex at each depth
            var childCountStack = new List<int>(); // child count at each depth
            childCountStack.Add(0); // root level child count

            // Track whether we've seen text content at the current level
            // that hasn't been counted yet. Browser childNodes includes text nodes,
            // so we must count them to match GetElementFromPath's index resolution.
            bool hasUnflushedText = false;

            int i = 0;
            while (i < html.Length)
            {
                if (html[i] == '<')
                {
                    // Flush any pending text node before processing the tag
                    if (hasUnflushedText)
                    {
                        childCountStack[childCountStack.Count - 1]++;
                        hasUnflushedText = false;
                    }

                    // Check for closing tag
                    if (i + 1 < html.Length && html[i + 1] == '/')
                    {
                        var closeEnd = html.IndexOf('>', i);
                        if (closeEnd < 0) break;
                        if (indexStack.Count > 0)
                        {
                            indexStack.RemoveAt(indexStack.Count - 1);
                            childCountStack.RemoveAt(childCountStack.Count - 1);
                        }
                        i = closeEnd + 1;
                        hasUnflushedText = false;
                        continue;
                    }

                    // Opening tag
                    var tagEnd = html.IndexOf('>', i);
                    if (tagEnd < 0) break;

                    var tagContent = html.Substring(i + 1, tagEnd - i - 1);
                    bool selfClosing = tagContent.EndsWith("/");

                    // This element's index within its parent = current child count at this level
                    int myIndex = childCountStack[childCountStack.Count - 1];

                    // Check for data-bind-idx markers (may have multiple on same element for attr bindings)
                    var markerMatches = System.Text.RegularExpressions.Regex.Matches(
                        tagContent, @"data-bind-idx=""(\d+)""");
                    foreach (System.Text.RegularExpressions.Match markerMatch in markerMatches)
                    {
                        var idx = int.Parse(markerMatch.Groups[1].Value);
                        // Build path: all ancestor indices + this element's index
                        var path = new List<int>(indexStack) { myIndex };
                        pathMap[idx] = path;
                    }

                    // Increment parent's child count (this is an element node)
                    childCountStack[childCountStack.Count - 1]++;

                    // If not self-closing, push new nesting level
                    if (!selfClosing)
                    {
                        indexStack.Add(myIndex);
                        childCountStack.Add(0);
                    }

                    i = tagEnd + 1;
                    hasUnflushedText = false;
                }
                else
                {
                    // Text content — mark as pending (will be counted as a text node
                    // when we encounter the next tag at the same level)
                    hasUnflushedText = true;
                    i++;
                }
            }

            // Build ordered path list
            for (int idx = 0; idx < bindingCount; idx++)
            {
                if (pathMap.TryGetValue(idx, out var path))
                    outPaths.Add(path);
                else
                    outPaths.Add(new List<int> { idx }); // fallback
            }
        }

    }
}

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

        public static Dictionary<string, int> BuildPartIdMappingPublic(List<IRNode> nodes)
            => BuildPartIdMapping(nodes);

        public static string CollectHtmlPublic(List<IRNode> nodes)
            => CollectHtml(nodes);

        /// <summary>
        /// Build a mapping from element id attribute values to their element indices
        /// in the objStorage array, for the SkinInstance partMap parameter (H8).
        /// </summary>
        private static Dictionary<string, int> BuildPartIdMapping(List<IRNode> nodes)
        {
            var mapping = new Dictionary<string, int>();
            int index = 0;
            BuildPartIdMappingRecursive(nodes, mapping, ref index);
            return mapping;
        }

        private static void BuildPartIdMappingRecursive(
            List<IRNode> nodes, Dictionary<string, int> mapping, ref int index)
        {
            foreach (var node in nodes)
            {
                if (node is ExpressionBindingNode binding)
                {
                    if (!string.IsNullOrEmpty(binding.ElementId))
                        mapping[binding.ElementId] = index;
                    index++;
                }
                else if (node is SubControlNode sub)
                {
                    if (!string.IsNullOrEmpty(sub.ElementId))
                        mapping[sub.ElementId] = index;
                    index++;
                }

                BuildPartIdMappingRecursive(node.Children, mapping, ref index);
            }
        }

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
                else if (node is ExpressionBindingNode)
                {
                    sb.Append($"<span data-bind-idx=\"{bindingIdx}\"></span>");
                    bindingIdx++;
                }
                else if (node is EventNode)
                {
                    // Events are wired via objStorage element indices, not HTML markers.
                }
                else if (node is ConditionalNode cond)
                {
                    sb.Append("<span>");
                    sb.Append(CollectHtmlWithMarkers(cond.TrueBranch, eventTracker, ref bindingIdx));
                    sb.Append("</span>");
                    if (cond.FalseBranch.Count > 0)
                    {
                        sb.Append("<span>");
                        sb.Append(CollectHtmlWithMarkers(cond.FalseBranch, eventTracker, ref bindingIdx));
                        sb.Append("</span>");
                    }
                }
                else if (node is LoopNode loop)
                {
                    sb.Append("<span>");
                    sb.Append(CollectHtmlWithMarkers(loop.ItemTemplate, eventTracker, ref bindingIdx));
                    sb.Append("</span>");
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

            int i = 0;
            while (i < html.Length)
            {
                if (html[i] == '<')
                {
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
                        continue;
                    }

                    // Opening tag
                    var tagEnd = html.IndexOf('>', i);
                    if (tagEnd < 0) break;

                    var tagContent = html.Substring(i + 1, tagEnd - i - 1);
                    bool selfClosing = tagContent.EndsWith("/");

                    // This element's index within its parent = current child count at this level
                    int myIndex = childCountStack[childCountStack.Count - 1];

                    // Check for data-bind-idx marker
                    var markerMatch = System.Text.RegularExpressions.Regex.Match(
                        tagContent, @"data-bind-idx=""(\d+)""");
                    if (markerMatch.Success)
                    {
                        var idx = int.Parse(markerMatch.Groups[1].Value);
                        // Build path: all ancestor indices + this element's index
                        var path = new List<int>(indexStack) { myIndex };
                        pathMap[idx] = path;
                    }

                    // Increment parent's child count
                    childCountStack[childCountStack.Count - 1]++;

                    // If not self-closing, push new nesting level
                    if (!selfClosing)
                    {
                        indexStack.Add(myIndex);
                        childCountStack.Add(0);
                    }

                    i = tagEnd + 1;
                }
                else
                {
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

        /// <summary>
        /// Counts the number of top-level child nodes that an HTML string would add to its parent.
        /// Uses a simple heuristic: count top-level tags and text segments.
        /// </summary>
        private static int CountTopLevelHtmlChildren(string html)
        {
            if (string.IsNullOrEmpty(html))
                return 0;

            int count = 0;
            int depth = 0;
            bool hasText = false;

            for (int i = 0; i < html.Length; i++)
            {
                char c = html[i];
                if (c == '<')
                {
                    // Flush any pending text node at depth 0
                    if (depth == 0 && hasText)
                    {
                        count++;
                        hasText = false;
                    }

                    // Check if closing tag
                    if (i + 1 < html.Length && html[i + 1] == '/')
                    {
                        // Find end of closing tag
                        int end = html.IndexOf('>', i);
                        if (end >= 0)
                        {
                            depth--;
                            i = end;
                        }
                    }
                    else
                    {
                        // Opening tag — check for self-closing
                        int end = html.IndexOf('>', i);
                        if (end >= 0)
                        {
                            bool selfClosing = html[end - 1] == '/' ||
                                IsSelfClosingTag(html, i, end);
                            if (depth == 0)
                                count++;
                            if (!selfClosing)
                                depth++;
                            i = end;
                        }
                    }
                }
                else if (depth == 0 && !char.IsWhiteSpace(c))
                {
                    hasText = true;
                }
            }

            // Trailing text node
            if (depth == 0 && hasText)
                count++;

            return Math.Max(count, 1); // At minimum, the content occupies 1 slot
        }

        private static bool IsSelfClosingTag(string html, int tagStart, int tagEnd)
        {
            // Extract tag name
            int nameStart = tagStart + 1;
            int nameEnd = nameStart;
            while (nameEnd < tagEnd && !char.IsWhiteSpace(html[nameEnd]) && html[nameEnd] != '>' && html[nameEnd] != '/')
                nameEnd++;
            var tagName = html.Substring(nameStart, nameEnd - nameStart).ToLower();

            // HTML void elements
            return tagName == "br" || tagName == "hr" || tagName == "img" || tagName == "input"
                || tagName == "meta" || tagName == "link" || tagName == "area" || tagName == "base"
                || tagName == "col" || tagName == "embed" || tagName == "source" || tagName == "track"
                || tagName == "wbr";
        }
    }
}

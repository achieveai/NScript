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
            List<IRNode> nodes, List<EventNode> eventTracker, List<List<int>> outPaths,
            List<List<int>> outEventPaths = null)
            => CollectHtmlWithPaths(nodes, eventTracker, outPaths, outEventPaths);

        public static string CollectHtmlPublic(List<IRNode> nodes)
            => CollectHtml(nodes);

        /// <summary>
        /// Collects HTML for item templates, inserting <span data-ns-evt></span> marker spans
        /// for event target elements. These marker spans are found by CollectSpanElements at
        /// runtime and occupy the correct ElemIdx positions. The runtime uses parentElement
        /// on marker spans to find the actual event target element.
        /// </summary>
        public static string CollectItemTemplateHtmlPublic(List<IRNode> nodes)
        {
            var sb = new StringBuilder();
            int pendingEvtMarkers = 0;
            int pendingBindMarkers = 0;
            CollectItemTemplateHtmlRecursive(nodes, sb, ref pendingEvtMarkers, ref pendingBindMarkers);

            var html = sb.ToString();

            // Strip outer-template marker attributes (data-bind-idx, data-evt-idx)
            html = System.Text.RegularExpressions.Regex.Replace(html, @" data-(bind|evt)-idx=""\d+""", "");

            // Post-process: for void elements (input, br, img, hr) that have
            // <span data-ns-bind></span> nearby (possibly with event markers in between),
            // move the bind marker to a data-ns-bind attribute on the void element.
            html = System.Text.RegularExpressions.Regex.Replace(
                html,
                @"(<(?:input|br|img|hr|area|base|col|embed|link|meta|param|source|track|wbr)\b[^>]*?)(\s*/?>)((?:<span data-ns-evt></span>)*)<span data-ns-bind></span>",
                "$1 data-ns-bind$2$3",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return html;
        }

        private static void CollectItemTemplateHtmlRecursive(
            List<IRNode> nodes, StringBuilder sb, ref int pendingEvtMarkers, ref int pendingBindMarkers)
        {
            int subControlIdx = 0;
            foreach (var node in nodes)
            {
                if (node is HtmlNode html)
                {
                    var content = html.HtmlContent;
                    // If there are pending event or bind markers, insert them after the first >
                    int totalPending = pendingEvtMarkers + pendingBindMarkers;
                    if (totalPending > 0)
                    {
                        int gtIdx = content.IndexOf('>');
                        if (gtIdx >= 0)
                        {
                            sb.Append(content.Substring(0, gtIdx + 1));
                            for (int m = 0; m < pendingEvtMarkers; m++)
                                sb.Append("<span data-ns-evt></span>");
                            for (int m = 0; m < pendingBindMarkers; m++)
                                sb.Append("<span data-ns-bind></span>");
                            sb.Append(content.Substring(gtIdx + 1));
                            pendingEvtMarkers = 0;
                            pendingBindMarkers = 0;
                            continue;
                        }
                    }
                    sb.Append(content);
                }
                else if (node is ExpressionBindingNode exprBinding)
                {
                    if (exprBinding.Target == ExpressionTarget.TextContent)
                        sb.Append("<span data-ns-ph></span>");
                    else
                    {
                        // Class/style/attribute bindings: insert a marker span inside the
                        // target element. The runtime resolves it to parentNode, like events.
                        pendingBindMarkers++;
                    }
                }
                else if (node is EventNode)
                {
                    // Mark that we need to insert a marker span inside the event target element.
                    // The span will be inserted after the next closing > of the current tag.
                    pendingEvtMarkers++;
                }
                else if (node is ConditionalNode)
                {
                    sb.Append("<span data-ns-ph></span>");
                }
                else if (node is LoopNode)
                {
                    sb.Append("<span data-ns-ph></span>");
                }
                else if (node is SubControlNode sub1)
                {
                    var tag = sub1.TagName ?? "div";
                    sb.Append("<");
                    sb.Append(tag);
                    if (sub1.DomAttributes != null)
                    {
                        foreach (var kvp in sub1.DomAttributes)
                        {
                            sb.Append(" ");
                            sb.Append(kvp.Key);
                            sb.Append("=\"");
                            sb.Append(System.Net.WebUtility.HtmlEncode(kvp.Value));
                            sb.Append("\"");
                        }
                    }
                    sb.Append($" data-ns-subctl=\"{subControlIdx++}\"></{tag}>");
                }
                else
                {
                    CollectItemTemplateHtmlRecursive(node.Children, sb, ref pendingEvtMarkers, ref pendingBindMarkers);
                }
            }
        }

        /// <summary>
        /// Builds the innerHTML string AND computes DOM paths for each binding placeholder.
        /// Uses marker attributes (data-bind-idx) during construction, then parses the
        /// final HTML to compute the actual DOM path for each marker.
        /// </summary>
        private static string CollectHtmlWithPaths(
            List<IRNode> nodes, List<EventNode> eventTracker, List<List<int>> outPaths,
            List<List<int>> outEventPaths = null)
        {
            // Phase 1: Build HTML with marker attributes on binding placeholders
            int bindingIdx = 0;
            int eventIdx = 0;
            var rawHtml = CollectHtmlWithMarkers(nodes, eventTracker, ref bindingIdx, ref eventIdx);

            // Phase 2: Parse the HTML to find each marker's DOM path
            ComputePathsFromHtml(rawHtml, bindingIdx, outPaths, eventIdx, outEventPaths);

            // Phase 3: Strip ALL marker attributes from the final HTML
            var cleanHtml = System.Text.RegularExpressions.Regex.Replace(
                rawHtml, @" data-(bind|evt)-idx=""\d+""", "");
            return cleanHtml;
        }

        private static string CollectHtmlWithMarkers(
            List<IRNode> nodes, List<EventNode> eventTracker, ref int bindingIdx, ref int eventIdx)
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
                        // Text bindings get a span placeholder in the HTML.
                        // data-ns-ph marks this as a compiler-generated placeholder span
                        // so CollectSpanElements can distinguish it from user-authored spans.
                        sb.Append($"<span data-ns-ph data-bind-idx=\"{bindingIdx}\"></span>");
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
                else if (node is EventNode evt)
                {
                    // Mark the event's target element for DOM path computation.
                    // The EventNode follows an unclosed opening tag in the preceding HTML —
                    // this attribute will be on that element. Path is computed in ComputePathsFromHtml.
                    sb.Append($" data-evt-idx=\"{eventIdx}\"");
                    eventIdx++;
                }
                else if (node is ConditionalNode cond)
                {
                    // Gate: emit only an empty marker span. Branch content is stored
                    // as HTML strings in the GateTargetInfo (trueTemplate/falseTemplate)
                    // and dynamically cloned by GraphEngine at runtime.
                    sb.Append("<span data-ns-ph></span>");
                    // Still need to recurse into branches so any nested bindings get
                    // their bindingIdx allocated (though they won't have HTML markers)
                }
                else if (node is LoopNode loop)
                {
                    // Collection: emit only an empty marker span. Item template content
                    // is stored as an HTML string in the CollectionTargetInfo and rendered
                    // by GraphEngine for each collection item.
                    sb.Append("<span data-ns-ph></span>");
                }
                else if (node is SubControlNode sub2)
                {
                    var tag = sub2.TagName ?? "div";
                    sb.Append("<");
                    sb.Append(tag);
                    if (sub2.DomAttributes != null)
                    {
                        foreach (var kvp in sub2.DomAttributes)
                        {
                            sb.Append(" ");
                            sb.Append(kvp.Key);
                            sb.Append("=\"");
                            sb.Append(System.Net.WebUtility.HtmlEncode(kvp.Value));
                            sb.Append("\"");
                        }
                    }
                    sb.Append(" data-ns-subctl></");
                    sb.Append(tag);
                    sb.Append(">");
                }
                else
                {
                    sb.Append(CollectHtmlWithMarkers(node.Children, eventTracker, ref bindingIdx, ref eventIdx));
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
            int unusedEvt = 0;
            var html = CollectHtmlWithMarkers(nodes, new List<EventNode>(), ref unused, ref unusedEvt);
            // Strip any binding and event markers from the HTML
            return System.Text.RegularExpressions.Regex.Replace(html, @" data-(bind|evt)-idx=""\d+""", "");
        }

        /// <summary>
        /// Parses HTML to find data-bind-idx markers and compute their DOM tree path.
        /// Tracks open/close tags to maintain a nesting stack with child counts.
        /// </summary>
        private static void ComputePathsFromHtml(string html, int bindingCount, List<List<int>> outPaths,
            int eventCount = 0, List<List<int>> outEventPaths = null)
        {
            var pathMap = new Dictionary<int, List<int>>();
            var eventPathMap = new Dictionary<int, List<int>>();

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

                    // Check for data-evt-idx markers (event target elements)
                    if (outEventPaths != null)
                    {
                        var evtMarkerMatches = System.Text.RegularExpressions.Regex.Matches(
                            tagContent, @"data-evt-idx=""(\d+)""");
                        foreach (System.Text.RegularExpressions.Match evtMatch in evtMarkerMatches)
                        {
                            var idx = int.Parse(evtMatch.Groups[1].Value);
                            var path = new List<int>(indexStack) { myIndex };
                            eventPathMap[idx] = path;
                        }
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

            // Build ordered path list for bindings
            for (int idx = 0; idx < bindingCount; idx++)
            {
                if (pathMap.TryGetValue(idx, out var path))
                    outPaths.Add(path);
                else
                    outPaths.Add(new List<int> { idx }); // fallback
            }

            // Build ordered path list for events
            if (outEventPaths != null)
            {
                for (int idx = 0; idx < eventCount; idx++)
                {
                    if (eventPathMap.TryGetValue(idx, out var path))
                        outEventPaths.Add(path);
                    else
                        outEventPaths.Add(new List<int> { 0 }); // fallback to root
                }
            }
        }

    }
}

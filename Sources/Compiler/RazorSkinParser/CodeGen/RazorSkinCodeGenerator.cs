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
        /// Gate (@if) and collection (@foreach) anchor. A &lt;template&gt; is legal in every
        /// parent, including table, tbody, tr and select, so the HTML parser never moves it,
        /// and it never renders. Text placeholders stay &lt;span data-ns-ph&gt;.
        /// </summary>
        internal const string AnchorMarkerHtml = "<template data-ns-ph></template>";

        /// <summary>
        /// Collects HTML for item and gate-branch templates. The runtime (GraphEngine.
        /// CollectSpanElements) addresses elements positionally, in document order:
        /// &lt;span data-ns-ph&gt; text placeholders, &lt;template data-ns-ph&gt; anchors,
        /// data-ns-subctl hosts, and every element that owns attribute bindings or events,
        /// marked data-ns-m="k" (k = how many it owns) and listed k times. An owner precedes
        /// its descendants, so document order matches the element-index order.
        /// </summary>
        public static string CollectItemTemplateHtmlPublic(List<IRNode> nodes)
        {
            var sb = new StringBuilder();
            var owner = new OwnerMarker();
            CollectItemTemplateHtmlRecursive(nodes, sb, owner);
            owner.Flush(sb);

            // Strip outer-template marker attributes (data-bind-idx, data-evt-idx)
            return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @" data-(bind|evt)-idx=""\d+""", "");
        }

        /// <summary>
        /// The open tag whose attribute bindings and events are being counted: where its
        /// open tag starts (just after the tag name) and how many it has so far. The count
        /// attribute is written at the end of the open tag once the tag is closed.
        /// </summary>
        private sealed class OwnerMarker
        {
            public int Position = -1;
            public int Count;

            /// <summary>Counts one binding or event of the tag that is open at the end of sb.</summary>
            public void Add(StringBuilder sb)
            {
                if (Count > 0 && UnquotedTagBoundary(sb, Position) < 0)
                {
                    Count++;
                    return;
                }

                Flush(sb);
                Position = OpenTagNameEnd(sb);
                Count = 1;
            }

            /// <summary>
            /// Counts a text binding that targets the element whose open tag was just closed
            /// (<see cref="ExpressionBindingNode.TargetsOwnerElement"/>).
            /// </summary>
            public void AddToLastTag(StringBuilder sb)
            {
                if (Count > 0 && OpenTagNameEnd(sb) == Position)
                {
                    Count++;
                    return;
                }

                Flush(sb);
                Position = OpenTagNameEnd(sb);
                Count = 1;
            }

            public void Flush(StringBuilder sb)
            {
                if (Count == 0) return;
                InsertIntoOpenTag(sb, Position, $"data-ns-m=\"{Count}\"");
                Position = -1;
                Count = 0;
            }

            /// <summary>
            /// Writes <paramref name="attribute"/> just before the '&gt;' (or '/&gt;') that closes
            /// the open tag whose name ends at <paramref name="tagNameEnd"/>.
            /// </summary>
            internal static void InsertIntoOpenTag(StringBuilder sb, int tagNameEnd, string attribute)
            {
                int close = UnquotedTagBoundary(sb, tagNameEnd);
                if (close < 0 || sb[close] != '>')
                    throw new InvalidOperationException(
                        "An element with bindings or events has no closing '>' on its open tag.");
                int insertAt = sb[close - 1] == '/' ? close - 1 : close;
                string separator = char.IsWhiteSpace(sb[insertAt - 1]) ? "" : " ";
                sb.Insert(insertAt, separator + attribute);
            }

            internal static int OpenTagNameEnd(StringBuilder sb)
            {
                int start = sb.Length - 1;
                while (start >= 0 && sb[start] != '<') start--;
                if (start < 0)
                    throw new InvalidOperationException(
                        "An attribute binding or event is not inside an element's open tag.");

                int end = start + 1;
                while (end < sb.Length && !char.IsWhiteSpace(sb[end]) && sb[end] != '/' && sb[end] != '>')
                    end++;
                return end;
            }

            /// <summary>
            /// Index of the first unquoted '&gt;' or '&lt;' at or after <paramref name="from"/>,
            /// or -1 while the HTML since then is still attributes of the same open tag.
            /// </summary>
            private static int UnquotedTagBoundary(StringBuilder sb, int from)
            {
                char quote = '\0';
                for (int i = from; i < sb.Length; i++)
                {
                    char c = sb[i];
                    if (quote != '\0')
                    {
                        if (c == quote) quote = '\0';
                    }
                    else if (c == '"' || c == '\'')
                    {
                        quote = c;
                    }
                    else if (c == '>' || c == '<')
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        private static void CollectItemTemplateHtmlRecursive(
            List<IRNode> nodes, StringBuilder sb, OwnerMarker owner)
        {
            var openContentHosts = new Stack<string>();
            foreach (var node in nodes)
            {
                if (node is HtmlNode html)
                {
                    sb.Append(html.HtmlContent);
                }
                else if (node is ExpressionBindingNode exprBinding)
                {
                    if (exprBinding.TargetsOwnerElement)
                        owner.AddToLastTag(sb); // only content of <textarea>/<option>/<title>
                    else if (exprBinding.Target == ExpressionTarget.TextContent)
                        sb.Append("<span data-ns-ph></span>");
                    else
                        owner.Add(sb); // class/style/attribute binding on the open tag
                }
                else if (node is EventNode)
                {
                    owner.Add(sb);
                }
                else if (node is ConditionalNode || node is LoopNode)
                {
                    sb.Append(AnchorMarkerHtml);
                }
                else if (node is SubControlNode sub1)
                {
                    ValidateSubControlMarker(sub1);
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
                    sb.Append(SubControlHostEnd(sub1, tag, openContentHosts));
                }
                else if (node is SubControlEndNode)
                {
                    sb.Append(CloseContentHost(openContentHosts));
                }
                else
                {
                    CollectItemTemplateHtmlRecursive(node.Children, sb, owner);
                }
            }

            EnsureContentHostsClosed(openContentHosts);
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
            var openContentHosts = new Stack<string>();
            foreach (var node in nodes)
            {
                if (node is HtmlNode html)
                    sb.Append(html.HtmlContent);
                else if (node is ExpressionBindingNode exprBinding)
                {
                    if (exprBinding.TargetsOwnerElement)
                    {
                        // Only content of <textarea>/<option>/<title>: no span can live there,
                        // so mark the element itself, like an attribute binding.
                        OwnerMarker.InsertIntoOpenTag(sb, OwnerMarker.OpenTagNameEnd(sb), $"data-bind-idx=\"{bindingIdx}\"");
                    }
                    else if (exprBinding.Target == ExpressionTarget.TextContent)
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
                    // Gate: emit only an anchor. Branch content is stored
                    // as HTML strings in the GateTargetInfo (trueTemplate/falseTemplate)
                    // and dynamically cloned by GraphEngine at runtime.
                    sb.Append(AnchorMarkerHtml);
                    // Still need to recurse into branches so any nested bindings get
                    // their bindingIdx allocated (though they won't have HTML markers)
                }
                else if (node is LoopNode loop)
                {
                    // Collection: emit only an anchor. Item template content
                    // is stored as an HTML string in the CollectionTargetInfo and rendered
                    // by GraphEngine for each collection item.
                    sb.Append(AnchorMarkerHtml);
                }
                else if (node is SubControlNode sub2)
                {
                    ValidateSubControlMarker(sub2);
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
                    sb.Append(SubControlHostEnd(sub2, tag, openContentHosts));
                }
                else if (node is SubControlEndNode)
                {
                    sb.Append(CloseContentHost(openContentHosts));
                }
                else
                {
                    sb.Append(CollectHtmlWithMarkers(node.Children, eventTracker, ref bindingIdx, ref eventIdx));
                }
            }

            EnsureContentHostsClosed(openContentHosts);
            return sb.ToString();
        }

        /// <summary>
        /// Ends a sub-control host's open tag. A host without caller content closes at once.
        /// A host with content stays open, marked data-ns-content, so the caller's content
        /// (the following sibling IR nodes) lands inside it until its SubControlEndNode.
        /// </summary>
        private static string SubControlHostEnd(SubControlNode sub, string tag, Stack<string> openContentHosts)
        {
            if (!sub.HasContent)
                return $" data-ns-subctl=\"{sub.RuntimeMarkerIdx}\"></{tag}>";

            openContentHosts.Push(tag);
            return $" data-ns-content data-ns-subctl=\"{sub.RuntimeMarkerIdx}\">";
        }

        private static string CloseContentHost(Stack<string> openContentHosts)
        {
            if (openContentHosts.Count == 0)
                throw new InvalidOperationException(
                    "A sub-control closing tag has no open tag at the same level; a sub-control's "
                    + "content cannot start or end inside an @if or @foreach that does not hold both tags.");
            return "</" + openContentHosts.Pop() + ">";
        }

        private static void EnsureContentHostsClosed(Stack<string> openContentHosts)
        {
            if (openContentHosts.Count > 0)
                throw new InvalidOperationException(
                    "A sub-control with content is not closed at the same level; a sub-control's "
                    + "content cannot start or end inside an @if or @foreach that does not hold both tags.");
        }

        private static readonly HashSet<string> VoidTags = new HashSet<string>(
            new[] { "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr" },
            StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// True for an HTML void element (&lt;hr&gt;, &lt;input ...&gt;): it never has
        /// children, with or without a trailing '/'. <paramref name="tagContent"/> is the text
        /// between '&lt;' and '&gt;'.
        /// </summary>
        internal static bool IsVoidTag(string tagContent)
        {
            int end = 0;
            while (end < tagContent.Length && !char.IsWhiteSpace(tagContent[end]) && tagContent[end] != '/')
                end++;
            return VoidTags.Contains(tagContent.Substring(0, end));
        }

        private static void ValidateSubControlMarker(SubControlNode sub)
        {
            if (sub.RuntimeMarkerIdx < 0)
                throw new InvalidOperationException(
                    $"Sub-control {sub.TypeName} has no assigned marker index.");
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
                    bool selfClosing = tagContent.EndsWith("/") || IsVoidTag(tagContent);

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

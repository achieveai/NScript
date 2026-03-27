using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NScript.RazorSkin.TemplateIR;

namespace NScript.RazorSkin.CodeGen
{
    public static class RazorSkinCodeGenerator
    {
        public static string Generate(SkinTemplateNode ir)
        {
            var sb = new StringBuilder();

            // Collect all bindings and compute HTML content with element paths
            var bindings = CollectBindings(ir.Children);
            var htmlContent = CollectHtml(ir.Children);
            var elementPaths = ComputeElementPaths(ir.Children);
            int liveBinderCount = bindings.Count(b => b.Classification.Mode == BindingMode.OneWay);

            // Template store variable (global, matching XWML tmplStore pattern)
            sb.AppendLine($"var tmplStore = new Array(1);");
            sb.AppendLine($"var {ir.TemplateName}_var = null;");
            sb.AppendLine();

            // Factory method
            sb.AppendLine($"function {ir.TemplateName}_factory(skinFactory, doc) {{");
            sb.AppendLine("  var domStore, htmlRoot, objStorage;");
            sb.AppendLine($"  if (!(domStore = DocStorageGetter(doc))[0]) {{");
            sb.AppendLine($"    domStore[0] = doc.createElement(\"div\");");
            sb.AppendLine($"    domStore[0].innerHTML = \"{EscapeJs(htmlContent)}\";");

            // Binders array — stored in tmplStore for reuse across instances
            if (bindings.Count > 0)
            {
                sb.AppendLine("    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [");
                for (int i = 0; i < bindings.Count; i++)
                {
                    var comma = i < bindings.Count - 1 ? "," : "";
                    sb.AppendLine($"      {BinderEmitter.EmitSkinBinderInfo(bindings[i], i, i)}{comma}");
                }
                sb.AppendLine("    ];");
            }
            else
            {
                sb.AppendLine("    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];");
            }

            sb.AppendLine("  }");
            sb.AppendLine("  htmlRoot = domStore[0].cloneNode(true);");
            sb.AppendLine($"  objStorage = new Array({bindings.Count});");

            // Element path mapping — compute actual DOM tree paths
            for (int i = 0; i < bindings.Count; i++)
            {
                var path = i < elementPaths.Count ? elementPaths[i] : new List<int> { i + 1 };
                var pathStr = string.Join(", ", path);
                sb.AppendLine($"  objStorage[{i}] = GetElementFromPath(htmlRoot, [{pathStr}]);");
            }

            sb.AppendLine($"  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, {liveBinderCount}, 0);");
            sb.AppendLine("}");
            sb.AppendLine();

            // Skin getter function
            sb.AppendLine($"function {ir.TemplateName}() {{");
            sb.AppendLine($"  if (!{ir.TemplateName}_var)");
            sb.AppendLine($"    {ir.TemplateName}_var = Skin_factory({ir.ControlTypeName}, {ir.ModelTypeName}, {ir.TemplateName}_factory, \"0\");");
            sb.AppendLine($"  return {ir.TemplateName}_var;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static List<ExpressionBindingNode> CollectBindings(List<IRNode> nodes)
        {
            var result = new List<ExpressionBindingNode>();
            foreach (var node in nodes)
            {
                if (node is ExpressionBindingNode binding)
                    result.Add(binding);
                result.AddRange(CollectBindings(node.Children));
            }
            return result;
        }

        private static string CollectHtml(List<IRNode> nodes)
        {
            var sb = new StringBuilder();
            foreach (var node in nodes)
            {
                if (node is HtmlNode html)
                    sb.Append(html.HtmlContent);
                else if (node is ExpressionBindingNode)
                    sb.Append("<span></span>");
                else if (node is ConditionalNode cond)
                {
                    // Emit a placeholder container for the conditional block
                    sb.Append("<span>");
                    sb.Append(CollectHtml(cond.TrueBranch));
                    sb.Append("</span>");
                    if (cond.FalseBranch.Count > 0)
                    {
                        sb.Append("<span>");
                        sb.Append(CollectHtml(cond.FalseBranch));
                        sb.Append("</span>");
                    }
                }
                else if (node is LoopNode loop)
                {
                    // Emit a placeholder container for the loop items
                    sb.Append("<span>");
                    sb.Append(CollectHtml(loop.ItemTemplate));
                    sb.Append("</span>");
                }
                else
                {
                    // Recurse into any other node's children
                    sb.Append(CollectHtml(node.Children));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Compute DOM tree paths for each binding placeholder element.
        /// Walks the IR nodes in the same order as CollectHtml and tracks a virtual
        /// DOM child index at each nesting level, producing paths like [1, 0], [1, 2].
        /// This mirrors the XWML SkinCodeGenerator.GetNodePath pattern.
        /// </summary>
        private static List<List<int>> ComputeElementPaths(List<IRNode> nodes)
        {
            var paths = new List<List<int>>();
            var currentPath = new List<int>();
            ComputeElementPathsRecursive(nodes, currentPath, paths);
            return paths;
        }

        private static void ComputeElementPathsRecursive(
            List<IRNode> nodes, List<int> parentPath, List<List<int>> paths)
        {
            int childIndex = 0;
            foreach (var node in nodes)
            {
                if (node is HtmlNode html)
                {
                    // Static HTML may produce one or more child nodes in the DOM.
                    // Count the number of top-level elements/text nodes it adds.
                    childIndex += CountTopLevelHtmlChildren(html.HtmlContent);
                }
                else if (node is ExpressionBindingNode)
                {
                    // Each expression becomes a <span></span> placeholder
                    var path = new List<int>(parentPath) { childIndex };
                    paths.Add(path);
                    childIndex++;
                }
                else if (node is ConditionalNode cond)
                {
                    // True branch wrapped in <span>
                    var truePath = new List<int>(parentPath) { childIndex };
                    ComputeElementPathsRecursive(cond.TrueBranch, truePath, paths);
                    childIndex++;

                    if (cond.FalseBranch.Count > 0)
                    {
                        var falsePath = new List<int>(parentPath) { childIndex };
                        ComputeElementPathsRecursive(cond.FalseBranch, falsePath, paths);
                        childIndex++;
                    }
                }
                else if (node is LoopNode loop)
                {
                    // Loop wrapped in <span>
                    var loopPath = new List<int>(parentPath) { childIndex };
                    ComputeElementPathsRecursive(loop.ItemTemplate, loopPath, paths);
                    childIndex++;
                }
                else
                {
                    // Generic children
                    ComputeElementPathsRecursive(node.Children, parentPath, paths);
                }
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

        private static string EscapeJs(string s)
        {
            return s
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t")
                .Replace("\0", "\\0")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f");
        }
    }
}

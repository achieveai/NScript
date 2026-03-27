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

            // Build set of known function names from @functions blocks (M2)
            var knownFunctionNames = new HashSet<string>();
            if (ir.Functions != null)
            {
                foreach (var func in ir.Functions)
                {
                    if (func.FunctionName != "functions_block")
                        knownFunctionNames.Add(func.FunctionName);
                }
            }

            // Emit @functions blocks
            EmitFunctions(sb, ir.Functions);

            // Collect all bindings and compute HTML content with element paths
            var bindings = CollectBindings(ir.Children);
            var events = CollectEvents(ir.Children);
            var subControls = CollectSubControls(ir.Children);
            var htmlContent = CollectHtml(ir.Children, events);
            var elementPaths = ComputeElementPaths(ir.Children);
            int liveBinderCount = bindings.Count(b => b.Classification.Mode == BindingMode.OneWay);

            // Collect reactive blocks for binder setup
            var reactiveConditionals = CollectReactiveConditionals(ir.Children);
            var reactiveLoops = CollectReactiveLoops(ir.Children);

            // Build part ID mapping: elements with id= attributes -> element index (H8)
            var partIdMapping = BuildPartIdMapping(ir.Children);
            var partIdMappingJs = partIdMapping.Count > 0
                ? "{\n" + string.Join(",\n", partIdMapping.Select(kvp => $"    \"{kvp.Key}\": {kvp.Value}")) + "\n  }"
                : "null";

            // Template store variable — prefixed with template name to avoid global collision (M1)
            var tmplStoreVar = $"{ir.TemplateName}_tmplStore";
            sb.AppendLine($"var {tmplStoreVar} = new Array(1);");
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
                sb.AppendLine($"    {tmplStoreVar}[0] = {tmplStoreVar}[0] ? {tmplStoreVar}[0] : [");
                for (int i = 0; i < bindings.Count; i++)
                {
                    var comma = i < bindings.Count - 1 ? "," : "";
                    sb.AppendLine($"      {BinderEmitter.EmitSkinBinderInfo(bindings[i], i, i, knownFunctionNames)}{comma}");
                }
                sb.AppendLine("    ];");
            }
            else
            {
                sb.AppendLine($"    {tmplStoreVar}[0] = {tmplStoreVar}[0] ? {tmplStoreVar}[0] : [];");
            }

            sb.AppendLine("  }");
            sb.AppendLine("  htmlRoot = domStore[0].cloneNode(true);");
            sb.AppendLine($"  objStorage = new Array({bindings.Count});");

            // Element path mapping — compute actual DOM tree paths
            for (int i = 0; i < bindings.Count; i++)
            {
                var path = i < elementPaths.Count ? elementPaths[i] : new List<int> { i + 1 };
                var pathStr = string.Join(", ", path);
                sb.AppendLine($"  objStorage[{i}] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [{pathStr}]);");
            }

            // Emit event binders
            EmitEventBinders(sb, events, bindings.Count);

            // Emit reactive conditional binders
            EmitReactiveConditionalBinders(sb, reactiveConditionals);

            // Emit reactive loop binders
            EmitReactiveLoopBinders(sb, reactiveLoops);

            // Emit sub-control factory calls
            var childElements = EmitSubControlFactoryCalls(sb, subControls);

            sb.AppendLine($"  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [{childElements}], objStorage, {tmplStoreVar}[0], {partIdMappingJs}, {liveBinderCount}, 0);");
            sb.AppendLine("}");
            sb.AppendLine();

            // Skin getter function
            sb.AppendLine($"function {ir.TemplateName}() {{");
            sb.AppendLine($"  if (!{ir.TemplateName}_var)");
            sb.AppendLine($"    {ir.TemplateName}_var = Sunlight__Framework__UI__Skin_factory({MangleTypeName(ir.ControlTypeName)}, {MangleTypeName(ir.ModelTypeName)}, {ir.TemplateName}_factory, \"0\");");
            sb.AppendLine($"  return {ir.TemplateName}_var;");
            sb.AppendLine("}");

            return sb.ToString();
        }

        // --- @functions emission ---

        private static void EmitFunctions(StringBuilder sb, List<FunctionNode> functions)
        {
            if (functions == null || functions.Count == 0) return;

            foreach (var func in functions)
            {
                if (func.FunctionName == "functions_block") continue; // Skip raw block fallbacks

                var jsBody = ConvertFunctionBodyToJs(func);

                if (func.IsPure)
                {
                    // Pure function: standalone JS helper
                    sb.AppendLine(jsBody);
                }
                else
                {
                    // Model-dependent: receives dc (dataContext) parameter
                    sb.AppendLine(jsBody);
                }
                sb.AppendLine();
            }
        }

        /// <summary>
        /// Convert a C# function to JS. Uses simple text transformation
        /// matching the ExpressionJsEmitter patterns.
        /// </summary>
        private static string ConvertFunctionBodyToJs(FunctionNode func)
        {
            var source = func.CSharpSource;

            // Extract function signature and body
            var parenIdx = source.IndexOf('(');
            if (parenIdx < 0) return $"// Could not convert function: {func.FunctionName}";

            // Find parameter list
            var closeParenIdx = source.IndexOf(')', parenIdx);
            if (closeParenIdx < 0) return $"// Could not convert function: {func.FunctionName}";

            var paramStr = source.Substring(parenIdx + 1, closeParenIdx - parenIdx - 1).Trim();
            var jsParams = ConvertParameterList(paramStr, func.IsPure);

            // Check for expression-bodied method: => expr;
            var arrowIdx = source.IndexOf("=>", closeParenIdx);
            if (arrowIdx >= 0 && !source.Substring(closeParenIdx, arrowIdx - closeParenIdx).Contains("{"))
            {
                var exprBody = source.Substring(arrowIdx + 2).Trim().TrimEnd(';');
                var jsExpr = ExpressionJsEmitter.ToJsGetter(exprBody);
                return $"function {func.FunctionName}({jsParams}) {{ return {jsExpr}; }}";
            }

            // Block body: extract content between { }
            var braceStart = source.IndexOf('{', closeParenIdx);
            if (braceStart < 0) return $"// Could not convert function: {func.FunctionName}";

            var braceEnd = source.LastIndexOf('}');
            if (braceEnd <= braceStart) return $"// Could not convert function: {func.FunctionName}";

            var body = source.Substring(braceStart + 1, braceEnd - braceStart - 1).Trim();
            var jsBody = ConvertFunctionBody(body);

            return $"function {func.FunctionName}({jsParams}) {{\n  {jsBody}\n}}";
        }

        private static string ConvertParameterList(string paramStr, bool isPure)
        {
            if (string.IsNullOrWhiteSpace(paramStr))
                return isPure ? "" : "dc";

            // Convert C# params: "decimal price, int qty" -> "price, qty"
            var parts = paramStr.Split(',');
            var jsParams = new List<string>();

            if (!isPure)
                jsParams.Add("dc");

            foreach (var part in parts)
            {
                var tokens = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length >= 2)
                    jsParams.Add(tokens[tokens.Length - 1]); // Last token is param name
                else if (tokens.Length == 1)
                    jsParams.Add(tokens[0]);
            }

            return string.Join(", ", jsParams);
        }

        private static string ConvertFunctionBody(string body)
        {
            // Simple conversion: replace Model. and Control. references, convert property accesses
            var js = body;
            js = ExpressionJsEmitter.ToJsGetter(js);
            // Convert "return X;" to "return X;"
            return js;
        }

        // --- Event collection and emission ---

        private static List<EventNode> CollectEvents(List<IRNode> nodes)
        {
            var result = new List<EventNode>();
            foreach (var node in nodes)
            {
                if (node is EventNode evt)
                    result.Add(evt);
                if (node is SubControlNode sub)
                    result.AddRange(sub.EventBindings);
                result.AddRange(CollectEvents(node.Children));
            }
            return result;
        }

        private static void EmitEventBinders(StringBuilder sb, List<EventNode> events, int bindingCount)
        {
            if (events.Count == 0) return;

            sb.AppendLine("  // Event handlers");
            for (int i = 0; i < events.Count; i++)
            {
                var evt = events[i];
                var jsHandler = ConvertEventHandler(evt);
                // Use objStorage element indices to locate event target elements,
                // matching the XWML pattern of using indexed objStorage entries.
                var elemIdx = bindingCount + i;
                sb.AppendLine($"  objStorage[{elemIdx}] = objStorage[{elemIdx}] || htmlRoot;");
                sb.AppendLine($"  objStorage[{elemIdx}].addEventListener('{evt.DomEventName}', {jsHandler});");
            }
        }

        private static string ConvertEventHandler(EventNode evt)
        {
            if (evt.IsLambda)
            {
                // Lambda: @((evt) => Model.Cancel()) -> function(e) { dc.cancel(); }
                var expr = evt.HandlerExpression.Trim();
                // Extract lambda body: (params) => body
                var arrowIdx = expr.IndexOf("=>");
                if (arrowIdx >= 0)
                {
                    var body = expr.Substring(arrowIdx + 2).Trim();
                    var jsBody = ExpressionJsEmitter.ToJsGetter(body);
                    return $"function(e) {{ {jsBody}; }}";
                }
                return $"function(e) {{ {ExpressionJsEmitter.ToJsGetter(expr)}; }}";
            }
            else
            {
                // Method reference: @Model.OnSubmit -> function(e) { dc.get_onSubmit()(e); }
                var jsGetter = ExpressionJsEmitter.ToJsGetter(evt.HandlerExpression);
                return $"function(e) {{ {jsGetter}(e); }}";
            }
        }

        // --- Reactive block collection and emission ---

        private static List<ConditionalNode> CollectReactiveConditionals(List<IRNode> nodes)
            => CollectNodes<ConditionalNode>(nodes).Where(c => c.IsReactive).ToList();

        private static List<LoopNode> CollectReactiveLoops(List<IRNode> nodes)
            => CollectNodes<LoopNode>(nodes).Where(l => l.IsObservableCollection).ToList();

        private static void EmitReactiveConditionalBinders(StringBuilder sb, List<ConditionalNode> conditionals)
        {
            if (conditionals.Count == 0) return;

            sb.AppendLine("  // Reactive conditional binders");
            for (int i = 0; i < conditionals.Count; i++)
            {
                var cond = conditionals[i];
                var condJs = ExpressionJsEmitter.ToJsGetter(cond.Condition.CSharpExpression);

                // Generate true branch template fragment
                var trueBranchHtml = CollectHtml(cond.TrueBranch, new List<EventNode>());
                var falseBranchHtml = cond.FalseBranch.Count > 0
                    ? CollectHtml(cond.FalseBranch, new List<EventNode>())
                    : "";

                // Property names to watch
                var propNames = string.Join(", ",
                    cond.Condition.Dependencies.Select(d => $"\"{d.PropertyName}\""));

                // Emit ConditionalBinder constructor call using JS-mangled name (H2)
                sb.AppendLine($"  new Sunlight__Framework__UI__Helpers__ConditionalBinder(function(dc) {{ return {condJs}; }}, [{propNames}],");
                sb.AppendLine($"    htmlRoot,");
                sb.AppendLine($"    (function() {{ var e = doc.createElement('div'); e.innerHTML = \"{EscapeJs(trueBranchHtml)}\"; return e; }}()),");
                sb.AppendLine($"    {(string.IsNullOrEmpty(falseBranchHtml) ? "null" : $"(function() {{ var e = doc.createElement('div'); e.innerHTML = \"{EscapeJs(falseBranchHtml)}\"; return e; }}())")});");
            }
        }

        private static void EmitReactiveLoopBinders(StringBuilder sb, List<LoopNode> loops)
        {
            if (loops.Count == 0) return;

            sb.AppendLine("  // Reactive collection binders");
            for (int i = 0; i < loops.Count; i++)
            {
                var loop = loops[i];
                var collectionJs = ExpressionJsEmitter.ToJsGetter(loop.CollectionExpression);

                // Generate item template fragment
                var itemTemplateHtml = CollectHtml(loop.ItemTemplate, new List<EventNode>());

                // Emit CollectionBinder constructor call using JS-mangled name (H2)
                // The collection getter expression retrieves the collection from the DataContext
                sb.AppendLine($"  var _collBinder_{i} = new Sunlight__Framework__UI__Helpers__CollectionBinder(htmlRoot,");
                sb.AppendLine($"    (function() {{ var e = doc.createElement('div'); e.innerHTML = \"{EscapeJs(itemTemplateHtml)}\"; return e; }}()),");
                sb.AppendLine($"    function(tmpl, item) {{ return tmpl.cloneNode(true); }});");
                sb.AppendLine($"  _collBinder_{i}.get_collection = function(dc) {{ return {collectionJs}; }};");
            }
        }

        // --- Sub-control collection and emission ---

        private static List<SubControlNode> CollectSubControls(List<IRNode> nodes)
            => CollectNodes<SubControlNode>(nodes);

        /// <summary>
        /// Emit UIElement factory calls for sub-controls and return a comma-separated
        /// list of child element indices for the SkinInstance_factory childElements parameter.
        /// </summary>
        private static string EmitSubControlFactoryCalls(StringBuilder sb, List<SubControlNode> subControls)
        {
            if (subControls.Count == 0) return "";

            sb.AppendLine("  // Sub-control factory calls");
            var childIndices = new List<string>();

            for (int i = 0; i < subControls.Count; i++)
            {
                var sub = subControls[i];
                var varName = $"child_{sub.TypeName}_{i}";

                sb.AppendLine($"  var {varName} = {sub.TypeName}_factory(skinFactory);");

                // Wire property bindings
                foreach (var propBinding in sub.PropertyBindings)
                {
                    var jsValue = ExpressionJsEmitter.ToJsGetter(propBinding.Classification.CSharpExpression);
                    var setterName = ExpressionJsEmitter.PropertyToSetterName(propBinding.PropertyName);
                    sb.AppendLine($"  {varName}.{setterName}({jsValue});");
                }

                // Wire event bindings
                foreach (var evt in sub.EventBindings)
                {
                    var jsHandler = ConvertEventHandler(evt);
                    sb.AppendLine($"  {varName}.addEventListener('{evt.DomEventName}', {jsHandler});");
                }

                childIndices.Add(varName);
            }

            return string.Join(", ", childIndices);
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

        private static List<ExpressionBindingNode> CollectBindings(List<IRNode> nodes)
            => CollectNodes<ExpressionBindingNode>(nodes);

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

        private static string CollectHtml(List<IRNode> nodes, List<EventNode> eventTracker)
        {
            var sb = new StringBuilder();
            foreach (var node in nodes)
            {
                if (node is HtmlNode html)
                    sb.Append(html.HtmlContent);
                else if (node is ExpressionBindingNode)
                    sb.Append("<span></span>");
                else if (node is EventNode)
                {
                    // Events are wired via objStorage element indices, not HTML markers.
                    // No HTML output needed for event nodes.
                }
                else if (node is ConditionalNode cond)
                {
                    // Emit a placeholder container for the conditional block
                    sb.Append("<span>");
                    sb.Append(CollectHtml(cond.TrueBranch, eventTracker));
                    sb.Append("</span>");
                    if (cond.FalseBranch.Count > 0)
                    {
                        sb.Append("<span>");
                        sb.Append(CollectHtml(cond.FalseBranch, eventTracker));
                        sb.Append("</span>");
                    }
                }
                else if (node is LoopNode loop)
                {
                    // Emit a placeholder container for the loop items
                    sb.Append("<span>");
                    sb.Append(CollectHtml(loop.ItemTemplate, eventTracker));
                    sb.Append("</span>");
                }
                else if (node is SubControlNode)
                {
                    // Sub-controls are handled separately, emit a placeholder
                    sb.Append("<span></span>");
                }
                else
                {
                    // Recurse into any other node's children
                    sb.Append(CollectHtml(node.Children, eventTracker));
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

        /// <summary>
        /// Mangle a C# fully-qualified type name to its NScript JS identifier.
        /// Namespace separators (.) become double underscores (__).
        /// Example: "Sunlight.Framework.UI.UISkinableElement" -> "Sunlight__Framework__UI__UISkinableElement"
        /// </summary>
        private static string MangleTypeName(string csharpTypeName)
        {
            if (string.IsNullOrEmpty(csharpTypeName)) return csharpTypeName;
            return csharpTypeName.Replace(".", "__");
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

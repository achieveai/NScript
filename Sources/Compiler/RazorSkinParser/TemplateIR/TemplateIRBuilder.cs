using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Serilog;

namespace NScript.RazorSkin.TemplateIR
{
    public static class TemplateIRBuilder
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        // Known DOM event attribute names (lowercase)
        private static readonly HashSet<string> EventAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "onclick", "onchange", "onfocus", "onblur", "oninput", "onkeyup",
            "onkeydown", "onsubmit", "onmousedown", "onmouseup", "onmouseover",
            "onmouseout", "onmousemove", "ondblclick", "onscroll", "onresize",
            "onkeypress", "ontouchstart", "ontouchend", "ontouchmove"
        };

        // Regex to detect an event attribute at the end of an HTML fragment: onclick="
        private static readonly Regex EventAttrTailRegex = new Regex(
            @"\b(on\w+)\s*=\s*""?\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Regex to detect a non-event attribute at the end of an HTML fragment: class=", title=", data-count="
        // Matches: attrName=" or attrName="literal-prefix where expression follows
        private static readonly Regex AttrBindTailRegex = new Regex(
            @"\b([\w-]+)\s*=\s*""([^""]*)$",
            RegexOptions.Compiled);

        // Regex to match opening PascalCase tags: <ListView ...> or <ListView ... />
        private static readonly Regex PascalCaseTagRegex = new Regex(
            @"<([A-Z][A-Za-z0-9]+)(\s[^>]*)?\s*/?>",
            RegexOptions.Compiled);

        // Regex to match closing PascalCase tags: </ListView>
        private static readonly Regex PascalCaseClosingTagRegex = new Regex(
            @"</([A-Z][A-Za-z0-9]+)\s*>",
            RegexOptions.Compiled);

        // Regex to extract id attribute from an attribute string
        private static readonly Regex IdAttributeRegex = new Regex(
            @"\bid\s*=\s*(?:""([^""]*)""|'([^']*)')",
            RegexOptions.Compiled);

        // Regex to extract attribute name=value pairs
        private static readonly Regex AttributeRegex = new Regex(
            @"\b(\w+)\s*=\s*(?:""([^""]*)""|'([^']*)')",
            RegexOptions.Compiled);

        /// <summary>
        /// Classify the binding source kind from a C# expression string (M6).
        /// Extracts whether the expression references Model.* or Control.* to determine
        /// if the source is DataContext or TemplateParent.
        /// </summary>
        internal static BindingSourceKind ClassifySource(string expression)
        {
            if (expression.Contains("Control."))
                return BindingSourceKind.TemplateParent;
            return BindingSourceKind.DataContext;
        }

        /// <summary>
        /// Checks whether a tag name is PascalCase (starts with uppercase letter).
        /// Used to detect sub-control tags like &lt;ListView&gt;, &lt;SearchBox&gt;.
        /// </summary>
        internal static bool IsPascalCaseTag(string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) return false;
            return char.IsUpper(tagName[0]) && tagName.Length > 1 && tagName.All(c => char.IsLetterOrDigit(c));
        }
        public static SkinTemplateNode Build(
            string templateName,
            PreprocessorResult preprocessed,
            RazorParseResult parsed)
        {
            var root = new SkinTemplateNode
            {
                TemplateName = templateName,
                ModelTypeName = preprocessed.ModelTypeName,
                ControlTypeName = preprocessed.ControlTypeName,
                UsingNamespaces = preprocessed.UsingNamespaces
            };

            var irDoc = parsed.CodeDocument.GetDocumentIntermediateNode();
            if (irDoc == null)
                return root;

            // Navigate to the class declaration node
            var classNode = FindFirst<ClassDeclarationIntermediateNode>(irDoc);
            if (classNode == null)
                return root;

            // Extract @functions blocks (CSharpCode at class level, outside MethodDeclaration)
            foreach (var child in classNode.Children)
            {
                if (child is CSharpCodeIntermediateNode codeNode &&
                    !(child is MethodDeclarationIntermediateNode))
                {
                    var content = GetTokenContent(codeNode);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var functions = ExtractFunctions(content);
                        root.Functions.AddRange(functions);
                    }
                }
            }

            // Navigate to the method declaration node (ExecuteAsync body)
            var methodNode = FindFirst<MethodDeclarationIntermediateNode>(classNode);
            if (methodNode == null)
                return root;

            // Walk the flat sequence of children in the method body
            WalkMethodBody(methodNode.Children, root, preprocessed.ModelTypeName);

            // Count IR nodes by type
            var htmlCount = CountNodes<HtmlNode>(root.Children);
            var bindingCount = CountNodes<ExpressionBindingNode>(root.Children);
            var eventCount = CountNodes<EventNode>(root.Children);
            var conditionalCount = CountNodes<ConditionalNode>(root.Children);
            var loopCount = CountNodes<LoopNode>(root.Children);
            var subControlCount = CountNodes<SubControlNode>(root.Children);
            var functionCount = root.Functions.Count;

            Log.Debug("IR built: {HtmlNodes} html, {BindingNodes} bindings, {EventNodes} events, {ConditionalNodes} conditionals, {LoopNodes} loops, {SubControlNodes} sub-controls, {FunctionNodes} functions",
                htmlCount, bindingCount, eventCount, conditionalCount, loopCount, subControlCount, functionCount);

            return root;
        }

        private static int CountNodes<T>(List<IRNode> nodes) where T : IRNode
        {
            int count = 0;
            foreach (var node in nodes)
            {
                if (node is T) count++;
                count += CountNodes<T>(node.Children);
                if (node is ConditionalNode cond)
                {
                    count += CountNodes<T>(cond.TrueBranch);
                    count += CountNodes<T>(cond.FalseBranch);
                }
                else if (node is LoopNode loop)
                {
                    count += CountNodes<T>(loop.ItemTemplate);
                }
            }
            return count;
        }

        private static void WalkMethodBody(
            IntermediateNodeCollection children,
            IRNode currentParent,
            string modelTypeName = null)
        {
            var childList = children.ToList();
            int i = 0;
            // Track the last HTML content to detect event attribute context
            string lastHtmlContent = null;

            // Debug: log all child node types and content at this level
            for (int dbgIdx = 0; dbgIdx < childList.Count; dbgIdx++)
            {
                var dbgChild = childList[dbgIdx];
                string dbgContent = "";
                if (dbgChild is HtmlContentIntermediateNode dbgHtml)
                    dbgContent = GetTokenContent(dbgHtml);
                else if (dbgChild is CSharpExpressionIntermediateNode dbgExpr)
                    dbgContent = GetTokenContent(dbgExpr);
                else if (dbgChild is HtmlAttributeIntermediateNode dbgAttr)
                    dbgContent = dbgAttr.AttributeName ?? "(no name)";
                Log.Debug("WalkMethodBody [{Idx}]: {Type} = {Content}",
                    dbgIdx, dbgChild.GetType().Name, dbgContent.Length > 80 ? dbgContent.Substring(0, 80) + "..." : dbgContent);
            }

            while (i < childList.Count)
            {
                var child = childList[i];

                if (child is HtmlContentIntermediateNode htmlNode)
                {
                    var content = GetTokenContent(htmlNode);
                    // Strip the @model directive type name if Razor echoed it into the HTML
                    content = StripModelDirectiveEcho(content, modelTypeName);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        // Check for event attributes and sub-controls before adding HTML
                        var processed = ExtractEventAttributesFromHtml(content.Trim(), currentParent);
                        // Also detect sub-controls in the HTML
                        processed = ExtractSubControlsFromHtml(processed, currentParent);
                        if (!string.IsNullOrWhiteSpace(processed))
                        {
                            currentParent.Children.Add(new HtmlNode { HtmlContent = processed });
                        }
                        lastHtmlContent = content;
                    }
                    i++;
                }
                else if (child is CSharpExpressionIntermediateNode exprNode)
                {
                    var expression = GetTokenContent(exprNode);
                    // Skip @model directive expression
                    if (expression == "model")
                    {
                        i++;
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(expression))
                    {
                        // Check if this expression is inside an event attribute
                        var eventAttr = DetectEventAttributeContext(lastHtmlContent);
                        if (eventAttr != null)
                        {
                            currentParent.Children.Add(CreateEventNode(eventAttr, expression.Trim()));
                            // Skip the closing quote/tag in the next HTML node
                            if (i + 1 < childList.Count && childList[i + 1] is HtmlContentIntermediateNode)
                            {
                                var closingHtml = GetTokenContent(childList[i + 1] as HtmlContentIntermediateNode);
                                // Remove just the closing " from the event attribute
                                closingHtml = closingHtml.TrimStart('"', ' ');
                                closingHtml = StripModelDirectiveEcho(closingHtml, modelTypeName);
                                if (!string.IsNullOrWhiteSpace(closingHtml))
                                {
                                    var processed = ExtractEventAttributesFromHtml(closingHtml.Trim(), currentParent);
                                    processed = ExtractSubControlsFromHtml(processed, currentParent);
                                    if (!string.IsNullOrWhiteSpace(processed))
                                        currentParent.Children.Add(new HtmlNode { HtmlContent = processed });
                                    lastHtmlContent = closingHtml;
                                }
                                i += 2;
                            }
                            else
                            {
                                i++;
                            }
                        }
                        else
                        {
                            // Check if this expression is inside an attribute context (class="@...", title="@...", etc.)
                            var attrCtx = DetectAttributeBindingContext(lastHtmlContent);
                            if (attrCtx != null)
                            {
                                var (attrName, prefix) = attrCtx.Value;
                                var target = ClassifyAttributeTarget(attrName);
                                var binding = CreateExpressionBinding(expression);
                                binding.Target = target;
                                binding.AttributeName = attrName;
                                binding.AttributePrefix = prefix;

                                // Trim the incomplete attribute from the last HTML node
                                // e.g. '<div class="' → '<div' (the attribute is now a binding, not static HTML)
                                var lastChild = currentParent.Children.Count > 0
                                    ? currentParent.Children[currentParent.Children.Count - 1] as HtmlNode
                                    : null;
                                if (lastChild != null)
                                {
                                    var idx = lastChild.HtmlContent.LastIndexOf(attrName + "=",
                                        StringComparison.OrdinalIgnoreCase);
                                    if (idx >= 0)
                                        lastChild.HtmlContent = lastChild.HtmlContent.Substring(0, idx).TrimEnd();
                                }

                                currentParent.Children.Add(binding);

                                // Consume the closing quote from the next HTML node (like events do)
                                if (i + 1 < childList.Count && childList[i + 1] is HtmlContentIntermediateNode)
                                {
                                    var closingHtml = GetTokenContent(childList[i + 1] as HtmlContentIntermediateNode);
                                    // Remove the closing " from the attribute
                                    closingHtml = closingHtml.TrimStart('"', ' ');
                                    closingHtml = StripModelDirectiveEcho(closingHtml, modelTypeName);
                                    if (!string.IsNullOrWhiteSpace(closingHtml))
                                    {
                                        var processed = ExtractEventAttributesFromHtml(closingHtml.Trim(), currentParent);
                                        processed = ExtractSubControlsFromHtml(processed, currentParent);
                                        if (!string.IsNullOrWhiteSpace(processed))
                                            currentParent.Children.Add(new HtmlNode { HtmlContent = processed });
                                        lastHtmlContent = closingHtml;
                                    }
                                    i += 2;
                                }
                                else
                                {
                                    i++;
                                }
                            }
                            else
                            {
                                currentParent.Children.Add(CreateExpressionBinding(expression));
                                i++;
                            }
                        }
                    }
                    else
                    {
                        i++;
                    }
                }
                else if (child is CSharpCodeIntermediateNode codeNode)
                {
                    var code = GetTokenContent(codeNode);
                    var trimmedCode = code.TrimStart();

                    if (trimmedCode.StartsWith("if ") || trimmedCode.StartsWith("if("))
                    {
                        // Parse an if/else block: consumes subsequent siblings
                        i = ParseIfBlock(childList, i, currentParent);
                    }
                    else if (trimmedCode.StartsWith("foreach ") || trimmedCode.StartsWith("foreach("))
                    {
                        // Parse a foreach block: consumes subsequent siblings
                        i = ParseForeachBlock(childList, i, currentParent);
                    }
                    else
                    {
                        // Closing brace or other code - skip
                        i++;
                    }
                }
                else if (child is HtmlAttributeIntermediateNode attrNode)
                {
                    // Razor wraps attribute bindings like class="@Model.X" in structured nodes:
                    //   HtmlAttributeIntermediateNode (AttributeName="class")
                    //     ├── HtmlAttributeValueIntermediateNode (static part)
                    //     └── CSharpExpressionAttributeValueIntermediateNode
                    //           └── LazyIntermediateToken (the expression text)
                    var attrName = attrNode.AttributeName;
                    var exprValue = ExtractCSharpExpressionFromAttribute(attrNode);
                    // Extract static prefix from HtmlAttributeValueIntermediateNode children
                    // e.g., style="display: @Model.X" → prefix = "display: "
                    var attrPrefix = ExtractAttributePrefix(attrNode);

                    if (!string.IsNullOrWhiteSpace(exprValue) && !string.IsNullOrWhiteSpace(attrName))
                    {
                        // Check if this is an event attribute
                        if (attrName.StartsWith("on", StringComparison.OrdinalIgnoreCase))
                        {
                            currentParent.Children.Add(CreateEventNode(attrName, exprValue.Trim()));
                        }
                        else
                        {
                            var target = ClassifyAttributeTarget(attrName);
                            var binding = CreateExpressionBinding(exprValue);
                            binding.Target = target;
                            binding.AttributeName = attrName;
                            binding.AttributePrefix = attrPrefix ?? "";

                            // Trim incomplete attribute from preceding HTML node
                            var lastChild = currentParent.Children.Count > 0
                                ? currentParent.Children[currentParent.Children.Count - 1] as HtmlNode
                                : null;
                            if (lastChild != null)
                            {
                                var idx = lastChild.HtmlContent.LastIndexOf(attrName + "=",
                                    StringComparison.OrdinalIgnoreCase);
                                if (idx >= 0)
                                    lastChild.HtmlContent = lastChild.HtmlContent.Substring(0, idx).TrimEnd();
                            }

                            currentParent.Children.Add(binding);
                        }
                    }
                    i++;
                }
                else
                {
                    // Recurse into children of unknown nodes
                    if (child.Children.Count > 0)
                        WalkMethodBody(child.Children, currentParent, modelTypeName);

                    i++;
                }
            }
        }

        /// <summary>
        /// Parse an @if block from the flat list of intermediate nodes.
        /// Structure: CSharpCode("if (...) {"), HTML/Expr content, CSharpCode("}") or CSharpCode("} else {"), ...
        /// Returns the index after the last consumed node.
        /// </summary>
        private static int ParseIfBlock(
            List<IntermediateNode> nodes,
            int startIndex,
            IRNode parent)
        {
            var code = GetTokenContent(nodes[startIndex] as CSharpCodeIntermediateNode);
            var condExpr = ExtractConditionExpression(code);

            var conditional = new ConditionalNode
            {
                Condition = new BindingClassification
                {
                    CSharpExpression = condExpr,
                    Mode = BindingMode.OneTime,
                    SourceKind = ClassifySource(condExpr)
                },
                IsReactive = false
            };

            int i = startIndex + 1;
            bool inElseBranch = false;
            string lastHtmlContent = null;

            // Collect content nodes until we hit the closing brace
            while (i < nodes.Count)
            {
                var node = nodes[i];

                if (node is CSharpCodeIntermediateNode codeNode)
                {
                    var codeContent = GetTokenContent(codeNode).Trim();

                    if (codeContent == "}")
                    {
                        // End of if (or else) block
                        i++;
                        break;
                    }
                    else if (codeContent.Contains("else"))
                    {
                        // Switch to else branch
                        inElseBranch = true;
                        i++;
                        continue;
                    }
                    else if (codeContent.StartsWith("if ") || codeContent.StartsWith("if("))
                    {
                        // Nested @if block
                        var targetBranchNested = inElseBranch ? conditional.FalseBranch : conditional.TrueBranch;
                        var dummyParent = new SkinTemplateNode();
                        i = ParseIfBlock(nodes, i, dummyParent);
                        targetBranchNested.AddRange(dummyParent.Children);
                        continue;
                    }
                    else if (codeContent.StartsWith("foreach ") || codeContent.StartsWith("foreach("))
                    {
                        // Nested @foreach block
                        var targetBranchNested = inElseBranch ? conditional.FalseBranch : conditional.TrueBranch;
                        var dummyParent = new SkinTemplateNode();
                        i = ParseForeachBlock(nodes, i, dummyParent);
                        targetBranchNested.AddRange(dummyParent.Children);
                        continue;
                    }
                    else
                    {
                        i++;
                        continue;
                    }
                }

                // Add content to appropriate branch
                var targetBranch = inElseBranch ? conditional.FalseBranch : conditional.TrueBranch;

                if (node is HtmlContentIntermediateNode htmlNode)
                {
                    var content = GetTokenContent(htmlNode);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        targetBranch.Add(new HtmlNode { HtmlContent = content.Trim() });
                        lastHtmlContent = content;
                    }
                }
                else if (node is CSharpExpressionIntermediateNode exprNode)
                {
                    var expr = GetTokenContent(exprNode);
                    if (!string.IsNullOrWhiteSpace(expr))
                    {
                        // Check if expression is inside an event attribute context
                        var eventAttr = DetectEventAttributeContext(lastHtmlContent);
                        if (eventAttr != null)
                        {
                            targetBranch.Add(CreateEventNode(eventAttr, expr.Trim()));
                            if (i + 1 < nodes.Count && nodes[i + 1] is HtmlContentIntermediateNode)
                            {
                                var closingHtml = GetTokenContent(nodes[i + 1] as HtmlContentIntermediateNode);
                                closingHtml = closingHtml.TrimStart('"', ' ');
                                if (!string.IsNullOrWhiteSpace(closingHtml))
                                {
                                    targetBranch.Add(new HtmlNode { HtmlContent = closingHtml.Trim() });
                                    lastHtmlContent = closingHtml;
                                }
                                i += 2;
                                continue;
                            }
                        }
                        else
                        {
                            // Check attribute binding context
                            var attrCtx = DetectAttributeBindingContext(lastHtmlContent);
                            if (attrCtx != null)
                            {
                                var (attrName, prefix) = attrCtx.Value;
                                var target = ClassifyAttributeTarget(attrName);
                                var binding = CreateExpressionBinding(expr);
                                binding.Target = target;
                                binding.AttributeName = attrName;
                                binding.AttributePrefix = prefix;

                                var lastChild = targetBranch.Count > 0
                                    ? targetBranch[targetBranch.Count - 1] as HtmlNode : null;
                                if (lastChild != null)
                                {
                                    var idx = lastChild.HtmlContent.LastIndexOf(attrName + "=",
                                        StringComparison.OrdinalIgnoreCase);
                                    if (idx >= 0)
                                        lastChild.HtmlContent = lastChild.HtmlContent.Substring(0, idx).TrimEnd();
                                }
                                targetBranch.Add(binding);

                                if (i + 1 < nodes.Count && nodes[i + 1] is HtmlContentIntermediateNode)
                                {
                                    var closingHtml = GetTokenContent(nodes[i + 1] as HtmlContentIntermediateNode);
                                    closingHtml = closingHtml.TrimStart('"', ' ');
                                    if (!string.IsNullOrWhiteSpace(closingHtml))
                                    {
                                        targetBranch.Add(new HtmlNode { HtmlContent = closingHtml.Trim() });
                                        lastHtmlContent = closingHtml;
                                    }
                                    i += 2;
                                    continue;
                                }
                            }
                            else
                            {
                                targetBranch.Add(CreateExpressionBinding(expr));
                            }
                        }
                    }
                }
                else if (node is HtmlAttributeIntermediateNode attrNode)
                {
                    var attrName = attrNode.AttributeName;
                    var exprValue = ExtractCSharpExpressionFromAttribute(attrNode);
                    var attrPrefix = ExtractAttributePrefix(attrNode);

                    if (!string.IsNullOrWhiteSpace(exprValue) && !string.IsNullOrWhiteSpace(attrName))
                    {
                        if (attrName.StartsWith("on", StringComparison.OrdinalIgnoreCase))
                        {
                            targetBranch.Add(CreateEventNode(attrName, exprValue.Trim()));
                        }
                        else
                        {
                            var target = ClassifyAttributeTarget(attrName);
                            var binding = CreateExpressionBinding(exprValue);
                            binding.Target = target;
                            binding.AttributeName = attrName;
                            binding.AttributePrefix = attrPrefix ?? "";

                            var lastChild = targetBranch.Count > 0
                                ? targetBranch[targetBranch.Count - 1] as HtmlNode : null;
                            if (lastChild != null)
                            {
                                var idx = lastChild.HtmlContent.LastIndexOf(attrName + "=",
                                    StringComparison.OrdinalIgnoreCase);
                                if (idx >= 0)
                                    lastChild.HtmlContent = lastChild.HtmlContent.Substring(0, idx).TrimEnd();
                            }
                            targetBranch.Add(binding);
                        }
                    }
                }

                i++;
            }

            parent.Children.Add(conditional);
            return i;
        }

        /// <summary>
        /// Parse a @foreach block from the flat list of intermediate nodes.
        /// Structure: CSharpCode("foreach (...) {"), HTML/Expr content, CSharpCode("}")
        /// Returns the index after the last consumed node.
        /// </summary>
        private static int ParseForeachBlock(
            List<IntermediateNode> nodes,
            int startIndex,
            IRNode parent)
        {
            var code = GetTokenContent(nodes[startIndex] as CSharpCodeIntermediateNode);
            var foreachParts = ExtractForeachParts(code);

            if (foreachParts == null)
            {
                return startIndex + 1;
            }

            var loop = new LoopNode
            {
                ItemVariableName = foreachParts.Item1,
                CollectionExpression = foreachParts.Item2,
                IsObservableCollection = false,
                CollectionSourceKind = ClassifySource(foreachParts.Item2)
            };

            int i = startIndex + 1;
            string lastHtmlContent = null;

            // Collect content nodes until we hit the closing brace
            while (i < nodes.Count)
            {
                var node = nodes[i];

                if (node is CSharpCodeIntermediateNode codeNode)
                {
                    var codeContent = GetTokenContent(codeNode).Trim();
                    if (codeContent == "}")
                    {
                        i++;
                        break;
                    }
                    else if (codeContent.StartsWith("if ") || codeContent.StartsWith("if("))
                    {
                        // Nested @if block inside foreach
                        var dummyParent = new SkinTemplateNode();
                        i = ParseIfBlock(nodes, i, dummyParent);
                        loop.ItemTemplate.AddRange(dummyParent.Children);
                        continue;
                    }
                    else if (codeContent.StartsWith("foreach ") || codeContent.StartsWith("foreach("))
                    {
                        // Nested @foreach block inside foreach
                        var dummyParent = new SkinTemplateNode();
                        i = ParseForeachBlock(nodes, i, dummyParent);
                        loop.ItemTemplate.AddRange(dummyParent.Children);
                        continue;
                    }
                    i++;
                    continue;
                }

                if (node is HtmlContentIntermediateNode htmlNode)
                {
                    var content = GetTokenContent(htmlNode);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        loop.ItemTemplate.Add(new HtmlNode { HtmlContent = content.Trim() });
                        lastHtmlContent = content;
                    }
                }
                else if (node is CSharpExpressionIntermediateNode exprNode)
                {
                    var expr = GetTokenContent(exprNode);
                    if (!string.IsNullOrWhiteSpace(expr))
                    {
                        // Check if expression is inside an event attribute context
                        var eventAttr = DetectEventAttributeContext(lastHtmlContent);
                        if (eventAttr != null)
                        {
                            loop.ItemTemplate.Add(CreateEventNode(eventAttr, expr.Trim()));
                            // Skip closing quote in the next HTML node
                            if (i + 1 < nodes.Count && nodes[i + 1] is HtmlContentIntermediateNode)
                            {
                                var closingHtml = GetTokenContent(nodes[i + 1] as HtmlContentIntermediateNode);
                                closingHtml = closingHtml.TrimStart('"', ' ');
                                if (!string.IsNullOrWhiteSpace(closingHtml))
                                {
                                    loop.ItemTemplate.Add(new HtmlNode { HtmlContent = closingHtml.Trim() });
                                    lastHtmlContent = closingHtml;
                                }
                                i += 2;
                                continue;
                            }
                        }
                        else
                        {
                            // Check if expression is inside an attribute context
                            var attrCtx = DetectAttributeBindingContext(lastHtmlContent);
                            if (attrCtx != null)
                            {
                                var (attrName, prefix) = attrCtx.Value;
                                var target = ClassifyAttributeTarget(attrName);
                                var binding = CreateExpressionBinding(expr);
                                binding.Target = target;
                                binding.AttributeName = attrName;
                                binding.AttributePrefix = prefix;

                                // Trim the incomplete attribute from the last HTML node
                                var lastChild = loop.ItemTemplate.Count > 0
                                    ? loop.ItemTemplate[loop.ItemTemplate.Count - 1] as HtmlNode
                                    : null;
                                if (lastChild != null)
                                {
                                    var idx = lastChild.HtmlContent.LastIndexOf(attrName + "=",
                                        StringComparison.OrdinalIgnoreCase);
                                    if (idx >= 0)
                                        lastChild.HtmlContent = lastChild.HtmlContent.Substring(0, idx).TrimEnd();
                                }

                                loop.ItemTemplate.Add(binding);

                                // Consume closing quote
                                if (i + 1 < nodes.Count && nodes[i + 1] is HtmlContentIntermediateNode)
                                {
                                    var closingHtml = GetTokenContent(nodes[i + 1] as HtmlContentIntermediateNode);
                                    closingHtml = closingHtml.TrimStart('"', ' ');
                                    if (!string.IsNullOrWhiteSpace(closingHtml))
                                    {
                                        loop.ItemTemplate.Add(new HtmlNode { HtmlContent = closingHtml.Trim() });
                                        lastHtmlContent = closingHtml;
                                    }
                                    i += 2;
                                    continue;
                                }
                            }
                            else
                            {
                                loop.ItemTemplate.Add(CreateExpressionBinding(expr));
                            }
                        }
                    }
                }
                else if (node is HtmlAttributeIntermediateNode attrNode)
                {
                    // Structured attribute bindings (e.g., onclick="@folder.OnSelect", class="@Model.X")
                    var attrName = attrNode.AttributeName;
                    var exprValue = ExtractCSharpExpressionFromAttribute(attrNode);
                    var attrPrefix = ExtractAttributePrefix(attrNode);

                    if (!string.IsNullOrWhiteSpace(exprValue) && !string.IsNullOrWhiteSpace(attrName))
                    {
                        if (attrName.StartsWith("on", StringComparison.OrdinalIgnoreCase))
                        {
                            loop.ItemTemplate.Add(CreateEventNode(attrName, exprValue.Trim()));
                        }
                        else
                        {
                            var target = ClassifyAttributeTarget(attrName);
                            var binding = CreateExpressionBinding(exprValue);
                            binding.Target = target;
                            binding.AttributeName = attrName;
                            binding.AttributePrefix = attrPrefix ?? "";

                            // Trim incomplete attribute from preceding HTML node
                            var lastChild = loop.ItemTemplate.Count > 0
                                ? loop.ItemTemplate[loop.ItemTemplate.Count - 1] as HtmlNode
                                : null;
                            if (lastChild != null)
                            {
                                var idx = lastChild.HtmlContent.LastIndexOf(attrName + "=",
                                    StringComparison.OrdinalIgnoreCase);
                                if (idx >= 0)
                                    lastChild.HtmlContent = lastChild.HtmlContent.Substring(0, idx).TrimEnd();
                            }

                            loop.ItemTemplate.Add(binding);
                        }
                    }
                }

                i++;
            }

            parent.Children.Add(loop);
            return i;
        }

        private static ExpressionBindingNode CreateExpressionBinding(string expression)
        {
            var classification = new BindingClassification
            {
                CSharpExpression = expression.Trim(),
                Mode = BindingMode.OneTime,
                SourceKind = ClassifySource(expression)
            };

            return new ExpressionBindingNode
            {
                Classification = classification,
                Target = ExpressionTarget.TextContent
            };
        }

        private static List<FunctionNode> ExtractFunctions(string content)
        {
            var functions = new List<FunctionNode>();
            if (string.IsNullOrWhiteSpace(content))
                return functions;

            var trimmed = content.Trim();

            // Find method boundaries using brace-matching.
            // We scan for lines that look like method signatures ("type Name("),
            // then track braces to find where each method body ends.
            var lines = trimmed.Split('\n');
            int methodStartLine = -1;
            string currentMethodName = null;
            int braceDepth = 0;
            bool inMethod = false;
            var methodLines = new List<string>();

            for (int li = 0; li < lines.Length; li++)
            {
                var l = lines[li].Trim();

                if (!inMethod)
                {
                    // Look for a method signature
                    var parenIdx = l.IndexOf('(');
                    if (parenIdx > 0)
                    {
                        var beforeParen = l.Substring(0, parenIdx).Trim();
                        var parts = beforeParen.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            currentMethodName = parts[parts.Length - 1];
                            methodStartLine = li;
                            inMethod = true;
                            braceDepth = 0;
                            methodLines.Clear();
                        }
                    }
                }

                if (inMethod)
                {
                    methodLines.Add(lines[li]);

                    // Count braces in this line, skipping string literals and comments
                    bool inString = false;
                    bool inLineComment = false;
                    for (int ci = 0; ci < l.Length; ci++)
                    {
                        var ch = l[ci];
                        if (inLineComment) break;
                        if (ch == '/' && ci + 1 < l.Length && l[ci + 1] == '/')
                        {
                            inLineComment = true;
                            break;
                        }
                        if (ch == '"' && (ci == 0 || l[ci - 1] != '\\'))
                            inString = !inString;
                        if (!inString)
                        {
                            if (ch == '{') braceDepth++;
                            else if (ch == '}') braceDepth--;
                        }
                    }

                    // Expression-bodied method (=>) on a single line with semicolon
                    bool isExpressionBody = methodStartLine == li && l.Contains("=>") && l.EndsWith(";");

                    if ((braceDepth == 0 && li > methodStartLine) || isExpressionBody)
                    {
                        var methodSource = string.Join("\n", methodLines).Trim();
                        functions.Add(new FunctionNode
                        {
                            FunctionName = currentMethodName,
                            CSharpSource = methodSource,
                            IsPure = !methodSource.Contains("Model.") && !methodSource.Contains("Control.")
                        });
                        inMethod = false;
                        currentMethodName = null;
                        methodLines.Clear();
                    }
                }
            }

            // If we were still tracking a method (unclosed braces), add what we have
            if (inMethod && currentMethodName != null)
            {
                var methodSource = string.Join("\n", methodLines).Trim();
                functions.Add(new FunctionNode
                {
                    FunctionName = currentMethodName,
                    CSharpSource = methodSource,
                    IsPure = !methodSource.Contains("Model.") && !methodSource.Contains("Control.")
                });
            }

            // Fallback: if no methods found, add the whole block
            if (functions.Count == 0)
            {
                functions.Add(new FunctionNode
                {
                    FunctionName = "functions_block",
                    CSharpSource = trimmed,
                    IsPure = !trimmed.Contains("Model.") && !trimmed.Contains("Control.")
                });
            }

            return functions;
        }

        // --- Event and sub-control detection helpers ---

        /// <summary>
        /// Detects if the tail of the last HTML content ends with an event attribute assignment.
        /// Returns the event attribute name (e.g., "onclick") or null.
        /// </summary>
        private static string DetectEventAttributeContext(string lastHtmlContent)
        {
            if (string.IsNullOrEmpty(lastHtmlContent)) return null;
            var match = EventAttrTailRegex.Match(lastHtmlContent);
            if (match.Success)
            {
                var attrName = match.Groups[1].Value.ToLower();
                if (EventAttributes.Contains(attrName))
                    return attrName;
            }
            return null;
        }

        /// <summary>
        /// Detects a non-event attribute binding context from the last HTML fragment.
        /// Returns (attrName, prefix) if the HTML ends with  attrName="prefix  where
        /// a @expression follows. Returns null if not in an attribute context.
        /// </summary>
        private static (string attrName, string prefix)? DetectAttributeBindingContext(string lastHtmlContent)
        {
            if (string.IsNullOrEmpty(lastHtmlContent)) return null;
            var match = AttrBindTailRegex.Match(lastHtmlContent);
            if (!match.Success) return null;
            var attrName = match.Groups[1].Value;
            // Skip event attributes — they are handled separately
            if (EventAttributes.Contains(attrName.ToLower())) return null;
            // Skip id attribute
            if (attrName.Equals("id", StringComparison.OrdinalIgnoreCase)) return null;
            var prefix = match.Groups[2].Value; // text before the @expression (e.g. "display: " in style="display: @...")
            return (attrName, prefix);
        }

        /// <summary>
        /// Extracts the static prefix from a HtmlAttributeIntermediateNode.
        /// E.g., for style="display: @Model.X", the prefix is "display: ".
        /// The prefix comes from HtmlAttributeValueIntermediateNode children that appear
        /// before the CSharpExpressionAttributeValueIntermediateNode.
        /// </summary>
        private static string ExtractAttributePrefix(IntermediateNode attrNode)
        {
            var prefix = new System.Text.StringBuilder();
            foreach (var child in attrNode.Children)
            {
                var typeName = child.GetType().Name;
                // Static content before the expression
                if (typeName.Contains("HtmlAttributeValue"))
                {
                    foreach (var token in child.Children)
                    {
                        var content = GetTokenContentFromNode(token);
                        if (!string.IsNullOrEmpty(content))
                            prefix.Append(content);
                    }
                }
                // Stop when we reach the expression part
                else if (typeName.Contains("CSharpExpression"))
                {
                    break;
                }
            }
            return prefix.Length > 0 ? prefix.ToString() : null;
        }

        /// <summary>
        /// Extracts the C# expression text from a structured HtmlAttributeIntermediateNode.
        /// Walks into CSharpExpressionAttributeValueIntermediateNode children to find LazyIntermediateToken.
        /// </summary>
        private static string ExtractCSharpExpressionFromAttribute(IntermediateNode attrNode)
        {
            foreach (var child in attrNode.Children)
            {
                // CSharpExpressionAttributeValueIntermediateNode contains the expression
                if (child.GetType().Name.Contains("CSharpExpression"))
                {
                    // The expression token is inside the children
                    foreach (var token in child.Children)
                    {
                        var content = GetTokenContentFromNode(token);
                        if (!string.IsNullOrWhiteSpace(content))
                            return content;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets token content from an arbitrary IntermediateNode (handles LazyIntermediateToken, etc.)
        /// </summary>
        private static string GetTokenContentFromNode(IntermediateNode node)
        {
            // Try to get Content property via reflection (LazyIntermediateToken has it)
            var contentProp = node.GetType().GetProperty("Content");
            if (contentProp != null)
                return contentProp.GetValue(node) as string;
            return null;
        }

        /// <summary>
        /// Classifies an attribute name to ExpressionTarget.
        /// </summary>
        private static ExpressionTarget ClassifyAttributeTarget(string attrName)
        {
            if (attrName.Equals("class", StringComparison.OrdinalIgnoreCase))
                return ExpressionTarget.CssClass;
            if (attrName.Equals("style", StringComparison.OrdinalIgnoreCase))
                return ExpressionTarget.Style;
            return ExpressionTarget.Attribute;
        }

        /// <summary>
        /// Creates an EventNode from an event attribute name and a C# expression.
        /// </summary>
        private static EventNode CreateEventNode(string eventAttrName, string expression)
        {
            // Strip "on" prefix to get DOM event name: "onclick" -> "click"
            var domEventName = eventAttrName.StartsWith("on")
                ? eventAttrName.Substring(2)
                : eventAttrName;

            var isLambda = expression.Contains("=>") ||
                           expression.TrimStart().StartsWith("(");

            return new EventNode
            {
                DomEventName = domEventName,
                HandlerExpression = expression,
                IsLambda = isLambda
            };
        }

        /// <summary>
        /// Scans HTML content for inline event attributes with static handlers (no @ expression)
        /// and removes them, adding data-event-N markers instead.
        /// This handles patterns where the full event attr is inside a single HTML node.
        /// </summary>
        private static string ExtractEventAttributesFromHtml(string html, IRNode parent)
        {
            // Match event attributes with @-expression values within a single HTML content node
            // Pattern: onclick="@Model.OnSubmit" or onclick="@((e) => ...)"
            // These appear when Razor doesn't split the expression into a separate node.
            // Note: most Razor-split cases are handled in WalkMethodBody above.
            return html;
        }

        /// <summary>
        /// Scans HTML content for PascalCase tags that represent sub-controls.
        /// Extracts them into SubControlNode instances.
        /// </summary>
        private static string ExtractSubControlsFromHtml(string html, IRNode parent)
        {
            if (string.IsNullOrEmpty(html)) return html;

            var matches = PascalCaseTagRegex.Matches(html);

            foreach (Match match in matches)
            {
                var tagName = match.Groups[1].Value;
                if (!IsPascalCaseTag(tagName)) continue;

                var attrsStr = match.Groups[2].Success ? match.Groups[2].Value : "";
                var subControl = new SubControlNode
                {
                    TypeName = tagName,
                    ResolvedTypeName = tagName // Will be resolved later with namespace resolution
                };

                // Extract id attribute
                var idMatch = IdAttributeRegex.Match(attrsStr);
                if (idMatch.Success)
                    subControl.ElementId = idMatch.Groups[1].Success ? idMatch.Groups[1].Value : idMatch.Groups[2].Value;

                // Extract property bindings from attributes (non-event attributes with values)
                var attrMatches = AttributeRegex.Matches(attrsStr);
                foreach (Match attrMatch in attrMatches)
                {
                    var attrName = attrMatch.Groups[1].Value;
                    var attrValue = attrMatch.Groups[2].Success ? attrMatch.Groups[2].Value : attrMatch.Groups[3].Value;

                    if (attrName == "id") continue; // Already handled

                    if (EventAttributes.Contains(attrName.ToLower()) && attrValue.TrimStart().StartsWith("@"))
                    {
                        // Event binding on sub-control
                        var evtExpr = attrValue.TrimStart('@');
                        var domEvtName = attrName.ToLower().StartsWith("on")
                            ? attrName.Substring(2).ToLower()
                            : attrName.ToLower();
                        subControl.EventBindings.Add(new EventNode
                        {
                            DomEventName = domEvtName,
                            HandlerExpression = evtExpr,
                            IsLambda = evtExpr.Contains("=>")
                        });
                    }
                    else if (!attrName.StartsWith("on", StringComparison.OrdinalIgnoreCase))
                    {
                        // Property binding
                        var classification = new BindingClassification
                        {
                            CSharpExpression = attrValue.TrimStart('@'),
                            Mode = BindingMode.OneTime,
                            SourceKind = ClassifySource(attrValue)
                        };
                        subControl.PropertyBindings.Add(new SubControlPropertyBinding
                        {
                            PropertyName = attrName,
                            Classification = classification
                        });
                    }
                }

                parent.Children.Add(subControl);
            }

            // Remove PascalCase tags from HTML (they become sub-controls)
            // Also remove their closing tags
            var result = PascalCaseTagRegex.Replace(html, match =>
            {
                var tagName = match.Groups[1].Value;
                return IsPascalCaseTag(tagName) ? "" : match.Value;
            });
            // Remove closing PascalCase tags
            result = PascalCaseClosingTagRegex.Replace(result, match =>
            {
                var tagName = match.Groups[1].Value;
                return IsPascalCaseTag(tagName) ? "" : match.Value;
            });

            return result;
        }

        // --- Helper methods ---

        private static string GetTokenContent(IntermediateNode node)
        {
            if (node == null) return "";
            var sb = new System.Text.StringBuilder();
            foreach (var child in node.Children)
            {
                if (child is IntermediateToken token)
                    sb.Append(token.Content);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Strips the Razor-echoed @model type name from the beginning of HTML content.
        /// Razor emits the model type name as a text node at the start of the template.
        /// This may appear as its own node or combined with subsequent HTML in the same node.
        /// Returns the content with the type name prefix removed.
        /// </summary>
        private static string StripModelDirectiveEcho(string content, string modelTypeName)
        {
            if (string.IsNullOrEmpty(modelTypeName) || string.IsNullOrEmpty(content))
                return content;

            var trimmed = content.TrimStart();
            // Check if content starts with the model type name
            if (trimmed.StartsWith(modelTypeName))
            {
                // Strip the type name and any trailing whitespace/newlines
                var remainder = trimmed.Substring(modelTypeName.Length).TrimStart('\r', '\n');
                return remainder;
            }

            // Also check short name (last segment after dots)
            var shortName = modelTypeName.Contains(".")
                ? modelTypeName.Substring(modelTypeName.LastIndexOf('.') + 1)
                : null;
            if (shortName != null && trimmed.StartsWith(shortName))
            {
                var remainder = trimmed.Substring(shortName.Length).TrimStart('\r', '\n');
                return remainder;
            }

            return content;
        }

        private static string ExtractConditionExpression(string ifContent)
        {
            if (ifContent == null) return "";
            var start = ifContent.IndexOf('(');
            if (start < 0) return ifContent;

            // Use paren-matching to find the correct closing paren
            int depth = 0;
            for (int pos = start; pos < ifContent.Length; pos++)
            {
                if (ifContent[pos] == '(') depth++;
                else if (ifContent[pos] == ')') depth--;
                if (depth == 0)
                    return ifContent.Substring(start + 1, pos - start - 1).Trim();
            }

            // Fallback: no matching close paren found
            return ifContent.Substring(start + 1).Trim();
        }

        private static Tuple<string, string> ExtractForeachParts(string content)
        {
            if (content == null) return null;

            // Use regex to find " in " as a keyword boundary, not just substring.
            // This avoids matching property names containing "in" (e.g., "CheckInRecords").
            var inMatch = System.Text.RegularExpressions.Regex.Match(content, @"\s+in\s+");
            if (!inMatch.Success) return null;

            var inIdx = inMatch.Index;
            var varPart = content.Substring(0, inIdx).Trim();
            var afterIn = content.Substring(inIdx + inMatch.Length);

            // Remove everything from ")" onward
            var parenIdx = afterIn.IndexOf(')');
            var collPart = parenIdx >= 0 ? afterIn.Substring(0, parenIdx).Trim() : afterIn.Trim();

            // Remove "foreach (var " prefix to get item name
            var lastSpace = varPart.LastIndexOf(' ');
            var itemName = lastSpace >= 0 ? varPart.Substring(lastSpace + 1) : varPart;

            return Tuple.Create(itemName, collPart);
        }

        private static T FindFirst<T>(IntermediateNode node) where T : IntermediateNode
        {
            if (node is T match) return match;
            foreach (var child in node.Children)
            {
                var found = FindFirst<T>(child);
                if (found != null) return found;
            }
            return null;
        }
    }
}

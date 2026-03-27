using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace NScript.RazorSkin.TemplateIR
{
    public static class TemplateIRBuilder
    {
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
            WalkMethodBody(methodNode.Children, root);

            return root;
        }

        private static void WalkMethodBody(
            IntermediateNodeCollection children,
            IRNode currentParent)
        {
            var childList = children.ToList();
            int i = 0;

            while (i < childList.Count)
            {
                var child = childList[i];

                if (child is HtmlContentIntermediateNode htmlNode)
                {
                    var content = GetTokenContent(htmlNode);
                    // Skip the @model directive echoed as HTML (first HtmlContent often contains model type name)
                    if (!string.IsNullOrWhiteSpace(content) && !IsModelDirectiveEcho(content))
                    {
                        currentParent.Children.Add(new HtmlNode { HtmlContent = content.Trim() });
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
                        currentParent.Children.Add(CreateExpressionBinding(expression));
                    }
                    i++;
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
                else
                {
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
                    SourceKind = condExpr.Contains("Model.")
                        ? BindingSourceKind.DataContext
                        : condExpr.Contains("Control.")
                            ? BindingSourceKind.TemplateParent
                            : BindingSourceKind.DataContext
                },
                IsReactive = false
            };

            int i = startIndex + 1;
            bool inElseBranch = false;

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
                        var dummyParent = new HtmlNode { HtmlContent = "" };
                        i = ParseIfBlock(nodes, i, dummyParent);
                        targetBranchNested.AddRange(dummyParent.Children);
                        continue;
                    }
                    else if (codeContent.StartsWith("foreach ") || codeContent.StartsWith("foreach("))
                    {
                        // Nested @foreach block
                        var targetBranchNested = inElseBranch ? conditional.FalseBranch : conditional.TrueBranch;
                        var dummyParent = new HtmlNode { HtmlContent = "" };
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
                        targetBranch.Add(new HtmlNode { HtmlContent = content.Trim() });
                }
                else if (node is CSharpExpressionIntermediateNode exprNode)
                {
                    var expr = GetTokenContent(exprNode);
                    if (!string.IsNullOrWhiteSpace(expr))
                        targetBranch.Add(CreateExpressionBinding(expr));
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
                CollectionSourceKind = foreachParts.Item2.Contains("Model.")
                    ? BindingSourceKind.DataContext
                    : foreachParts.Item2.Contains("Control.")
                        ? BindingSourceKind.TemplateParent
                        : BindingSourceKind.DataContext
            };

            int i = startIndex + 1;

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
                        var dummyParent = new HtmlNode { HtmlContent = "" };
                        i = ParseIfBlock(nodes, i, dummyParent);
                        loop.ItemTemplate.AddRange(dummyParent.Children);
                        continue;
                    }
                    else if (codeContent.StartsWith("foreach ") || codeContent.StartsWith("foreach("))
                    {
                        // Nested @foreach block inside foreach
                        var dummyParent = new HtmlNode { HtmlContent = "" };
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
                        loop.ItemTemplate.Add(new HtmlNode { HtmlContent = content.Trim() });
                }
                else if (node is CSharpExpressionIntermediateNode exprNode)
                {
                    var expr = GetTokenContent(exprNode);
                    if (!string.IsNullOrWhiteSpace(expr))
                        loop.ItemTemplate.Add(CreateExpressionBinding(expr));
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
                SourceKind = expression.Contains("Model.") ? BindingSourceKind.DataContext
                           : expression.Contains("Control.") ? BindingSourceKind.TemplateParent
                           : BindingSourceKind.DataContext
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

                    // Count braces in this line
                    foreach (var ch in l)
                    {
                        if (ch == '{') braceDepth++;
                        else if (ch == '}') braceDepth--;
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
        /// Checks if a text content node is just the Razor echo of the @model directive type name.
        /// Only filters text that exactly matches a valid C# type identifier (with optional namespace dots).
        /// </summary>
        private static bool IsModelDirectiveEcho(string content)
        {
            var trimmed = content.Trim();
            if (string.IsNullOrEmpty(trimmed))
                return false;

            // Only match a single token that looks like a type name:
            // e.g. "TestVM" or "MyApp.ViewModels.OrderVM"
            // Must not contain spaces, HTML tags, or any content beyond an identifier.
            var lines = trimmed.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length != 1)
                return false;

            var singleLine = lines[0].Trim();
            // Must look like a fully qualified type name: only letters, digits, dots, underscores
            // and must start with a letter or underscore (not a digit or punctuation)
            if (singleLine.Length == 0 || !(char.IsLetter(singleLine[0]) || singleLine[0] == '_'))
                return false;

            return singleLine.All(c => char.IsLetterOrDigit(c) || c == '.' || c == '_');
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
            var inIdx = content.IndexOf(" in ");
            if (inIdx < 0) return null;

            var varPart = content.Substring(0, inIdx).Trim();
            var afterIn = content.Substring(inIdx + 4);

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

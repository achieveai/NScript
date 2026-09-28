using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NScript.Utils;
using Serilog;

namespace NScript.RazorSkin
{
    public sealed class RazorSkinPreprocessorException : InvalidOperationException
    {
        public RazorSkinPreprocessorException(Location location, string message) : base(message)
        {
            Location = location;
        }

        public Location Location { get; }
    }

    public class PreprocessorResult
    {
        public string ModelTypeName { get; set; }
        public string ControlTypeName { get; set; }
        public List<string> UsingNamespaces { get; set; } = new List<string>();
        /// <summary>
        /// Ordered list of CSS stylesheet resource names referenced via @styles directives.
        /// The order matters: later stylesheets may depend on earlier ones.
        /// </summary>
        public List<string> StylesheetReferences { get; set; } = new List<string>();
        public string CleanedTemplate { get; set; }
    }

    public static class RazorSkinPreprocessor
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        private const string DefaultControlType = "Sunlight.Framework.UI.UISkinableElement";
        // A single private-use character keeps Razor source columns unchanged.
        internal const string EscapedSubControlAt = "\uE000";
        private static readonly Regex SubControlTagRegex = new Regex(
            @"<(?<closing>/)?(?<name>[A-Z][A-Za-z0-9]*(?:\.[A-Za-z_][A-Za-z0-9_]*)*)(?=[\s/>])(?<attributes>(?:[^>""']|""[^""]*""|'[^']*')*?)(?<selfClosing>/)?>",
            RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex DynamicAttributeRegex = new Regex(
            "(\\b[A-Za-z_][A-Za-z0-9_]*\\s*=\\s*['\"])@",
            RegexOptions.Compiled);
        private static readonly Regex CSharpBlockStartRegex = new Regex(
            @"@functions\s*\{|@\{", RegexOptions.Compiled);

        public static PreprocessorResult Process(string templateSource, string sourceFile = null)
        {
            var result = new PreprocessorResult
            {
                ControlTypeName = DefaultControlType
            };

            var cleanedLines = new StringBuilder();
            var lines = templateSource.Split('\n');
            bool modelSeen = false;

            foreach (var rawLine in lines)
            {
                var line = rawLine.TrimEnd('\r');
                var trimmed = line.TrimStart();

                if (trimmed.StartsWith("@model "))
                {
                    if (modelSeen)
                    {
                        throw new InvalidOperationException(
                            "Duplicate @model directive. Only one @model directive is allowed per template.");
                    }
                    modelSeen = true;
                    result.ModelTypeName = trimmed.Substring("@model ".Length).Trim();
                    cleanedLines.AppendLine(line); // Keep @model for Razor
                }
                else if (trimmed.StartsWith("@control "))
                {
                    result.ControlTypeName = trimmed.Substring("@control ".Length).Trim();
                    cleanedLines.AppendLine(); // Keep source line positions stable.
                }
                else if (trimmed.StartsWith("@styles "))
                {
                    var cssRef = trimmed.Substring("@styles ".Length).Trim().Trim('"', '\'');
                    result.StylesheetReferences.Add(cssRef);
                    cleanedLines.AppendLine(); // Keep source line positions stable.
                }
                else if (trimmed.StartsWith("@using "))
                {
                    var ns = trimmed.Substring("@using ".Length).Trim();
                    result.UsingNamespaces.Add(ns);
                    cleanedLines.AppendLine(line); // Keep @using for Razor
                }
                else
                {
                    cleanedLines.AppendLine(line);
                }
            }

            result.CleanedTemplate = ProtectSubControlAttributes(
                cleanedLines.ToString().TrimEnd('\r', '\n'), sourceFile);

            Log.Verbose("Preprocessor extracted directives: @model={ModelType}, @control={ControlType}, @using count={UsingCount}, @styles count={StylesCount}",
                result.ModelTypeName, result.ControlTypeName, result.UsingNamespaces.Count, result.StylesheetReferences.Count);

            return result;
        }

        private static string ProtectSubControlAttributes(string template, string sourceFile)
        {
            var csharpPositions = FindCSharpBlockPositions(template);
            var openTags = new Stack<(string Name, int TagStart, int ContentStart)>();
            foreach (Match tag in SubControlTagRegex.Matches(template))
            {
                if (csharpPositions[tag.Index]) continue;
                var name = tag.Groups["name"].Value;
                if (tag.Groups["closing"].Success)
                {
                    if (openTags.Count == 0)
                        throw Diagnostic(template, sourceFile, tag.Index,
                            $"Unexpected closing sub-control </{name}>.");
                    if (openTags.Peek().Name != name)
                        throw Diagnostic(template, sourceFile, tag.Index,
                            $"Mismatched sub-control closing tag </{name}>; expected </{openTags.Peek().Name}>.");

                    var open = openTags.Pop();
                    if (!string.IsNullOrWhiteSpace(template.Substring(open.ContentStart,
                            tag.Index - open.ContentStart)))
                    {
                        throw Diagnostic(template, sourceFile, open.TagStart,
                            $"Sub-control <{name}> cannot contain child content.");
                    }
                }
                else if (!tag.Groups["selfClosing"].Success)
                {
                    openTags.Push((name, tag.Index, tag.Index + tag.Length));
                }
            }

            if (openTags.Count > 0)
            {
                var open = openTags.Peek();
                throw Diagnostic(template, sourceFile, open.TagStart,
                    $"Unclosed sub-control <{open.Name}>.");
            }

            return SubControlTagRegex.Replace(template, tag =>
            {
                if (csharpPositions[tag.Index] || tag.Groups["closing"].Success)
                    return tag.Value;
                return DynamicAttributeRegex.Replace(tag.Value,
                    match => match.Groups[1].Value + EscapedSubControlAt);
            });
        }

        private static bool[] FindCSharpBlockPositions(string template)
        {
            var positions = new bool[template.Length];
            foreach (Match block in CSharpBlockStartRegex.Matches(template))
            {
                if (positions[block.Index]) continue;
                int brace = template.IndexOf('{', block.Index, block.Length);
                int depth = 0;
                char quote = '\0';
                bool lineComment = false;
                bool blockComment = false;
                int blockEnd = template.Length;
                for (int i = block.Index; i < template.Length; i++)
                {
                    positions[i] = true;
                    if (i < brace) continue;
                    char ch = template[i];
                    char next = i + 1 < template.Length ? template[i + 1] : '\0';
                    if (lineComment)
                    {
                        if (ch == '\n') lineComment = false;
                        continue;
                    }
                    if (blockComment)
                    {
                        if (ch == '*' && next == '/')
                        {
                            positions[++i] = true;
                            blockComment = false;
                        }
                        continue;
                    }
                    if (quote != '\0')
                    {
                        if (ch == '\\' && i + 1 < template.Length)
                        {
                            positions[++i] = true;
                            continue;
                        }
                        if (ch == quote) quote = '\0';
                        continue;
                    }
                    if (ch == '/' && next == '/')
                    {
                        positions[++i] = true;
                        lineComment = true;
                    }
                    else if (ch == '/' && next == '*')
                    {
                        positions[++i] = true;
                        blockComment = true;
                    }
                    else if (ch == '"' || ch == '\'') quote = ch;
                    else if (ch == '{') depth++;
                    else if (ch == '}' && --depth == 0)
                    {
                        blockEnd = i;
                        break;
                    }
                }

                if (block.Value.StartsWith("@{", StringComparison.Ordinal))
                {
                    int lineStart = brace + 1;
                    while (lineStart < blockEnd)
                    {
                        int lineEnd = template.IndexOf('\n', lineStart);
                        if (lineEnd < 0 || lineEnd > blockEnd) lineEnd = blockEnd;
                        int first = lineStart;
                        while (first < lineEnd && char.IsWhiteSpace(template[first])) first++;
                        if (first < lineEnd && template[first] == '<')
                        {
                            for (int i = first; i < lineEnd; i++) positions[i] = false;
                        }
                        lineStart = lineEnd + 1;
                    }
                }
            }
            return positions;
        }

        private static RazorSkinPreprocessorException Diagnostic(
            string template, string sourceFile, int offset, string message)
        {
            int line = 1;
            int lineStart = 0;
            for (int i = 0; i < offset; i++)
            {
                if (template[i] != '\n') continue;
                line++;
                lineStart = i + 1;
            }
            return new RazorSkinPreprocessorException(
                new Location(sourceFile ?? string.Empty, line, offset - lineStart), message);
        }
    }
}

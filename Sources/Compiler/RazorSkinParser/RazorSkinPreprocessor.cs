using System;
using System.Collections.Generic;
using System.Text;

namespace NScript.RazorSkin
{
    public class PreprocessorResult
    {
        public string ModelTypeName { get; set; }
        public string ControlTypeName { get; set; }
        public List<string> UsingNamespaces { get; set; } = new List<string>();
        public string CleanedTemplate { get; set; }
    }

    public static class RazorSkinPreprocessor
    {
        private const string DefaultControlType = "Sunlight.Framework.UI.UISkinableElement";

        public static PreprocessorResult Process(string templateSource)
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
                    // Remove @control — not valid Razor
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

            result.CleanedTemplate = cleanedLines.ToString().TrimEnd('\r', '\n');
            return result;
        }
    }
}

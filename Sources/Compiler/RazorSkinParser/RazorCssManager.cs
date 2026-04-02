using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NScript.JST;
using NScript.Utils;
using Serilog;

namespace NScript.RazorSkin
{
    /// <summary>
    /// Manages CSS stylesheets for a Razor skin template.
    /// Mirrors XwmlParser's CssStyleSheet + DocumentContext CSS functionality:
    ///   - Parses CSS via CssParser.CssGrammer
    ///   - Registers class names with NScript's IdentifierScope for minification
    ///   - Validates class references from templates
    ///   - Serializes CSS output with minified names
    /// </summary>
    public class RazorCssManager
    {
        private static ILogger Log => RazorSkinCompiler.Logger;

        private readonly IdentifierScope _cssScope;
        private readonly List<RazorCssSheet> _sheets = new List<RazorCssSheet>();

        /// <summary>
        /// Global lookup of all class names across all sheets.
        /// Maps class name → (identifier, isDeclared).
        /// </summary>
        private readonly Dictionary<string, Tuple<IIdentifier, bool>> _allClassNames
            = new Dictionary<string, Tuple<IIdentifier, bool>>();

        public RazorCssManager()
        {
            _cssScope = new IdentifierScope(false);
        }

        /// <summary>
        /// Gets all sheets in declaration order.
        /// </summary>
        public IReadOnlyList<RazorCssSheet> Sheets => _sheets;

        /// <summary>
        /// Whether any stylesheets have been loaded.
        /// </summary>
        public bool HasStylesheets => _sheets.Count > 0;

        /// <summary>
        /// Adds a CSS stylesheet. Stylesheets must be added in the order declared by @styles directives.
        /// Later stylesheets can reference classes from earlier ones (nested selectors only).
        /// </summary>
        public void AddStylesheet(string resourceName, string cssText)
        {
            var sheet = new RazorCssSheet(resourceName);

            try
            {
                var grammar = new CssParser.CssGrammer(cssText, parseProperties: false);

                // Collect CSS variables
                grammar.CollectCssVariablesFromRules();
                foreach (var variable in grammar.DefinedCssVariables)
                    sheet.DeclaredCssVariables.Add(variable);

                grammar.CollectUsedCssVariablesFromRules();
                foreach (var variable in grammar.UsedCssVariables)
                    sheet.UsedCssVariables.Add(variable);

                // Process rules and extract class names
                AddCssRules(sheet, grammar.Rules);
                sheet.KeyFrames.AddRange(grammar.KeyFrames);
                AddMediaRules(sheet, grammar.MediaRules);

                _sheets.Add(sheet);

                Log.Debug("Added stylesheet {ResourceName} with {ClassCount} classes, {RuleCount} rules",
                    resourceName, sheet.ClassNames.Count, sheet.Rules.Count);
            }
            catch (CssParser.ParseException ex)
            {
                throw new NScript.Converter.ConverterLocationException(
                    new Location(resourceName, ex.Line, ex.Position),
                    ex.Message);
            }
        }

        /// <summary>
        /// Tries to resolve a CSS class name to its minified identifier.
        /// Searches all loaded stylesheets.
        /// </summary>
        public bool TryGetCssClassIdentifier(string className, out IIdentifier identifier)
        {
            Tuple<IIdentifier, bool> result;
            if (_allClassNames.TryGetValue(className, out result))
            {
                identifier = result.Item1;
                return true;
            }

            identifier = null;
            return false;
        }

        /// <summary>
        /// Gets the combined serialized CSS from all stylesheets, with minified class names.
        /// Stylesheets are emitted in declaration order.
        /// </summary>
        public string GetSerializedCss()
        {
            var sb = new StringBuilder();

            foreach (var sheet in _sheets)
            {
                // Serialize rules
                foreach (var rule in sheet.Rules)
                {
                    CssParser.CssSerializerVisitor.Instance.Process(
                        sb,
                        rule,
                        (cn) =>
                        {
                            IIdentifier id;
                            TryGetCssClassIdentifier(cn.ClassName, out id);
                            return id?.GetName() ?? cn.ClassName;
                        },
                        (idN) => idN.Id);
                }

                // Serialize keyframes
                foreach (var keyframes in sheet.KeyFrames)
                {
                    CssParser.CssSerializerVisitor.Instance.Process(sb, keyframes);
                }

                // Serialize media rules
                foreach (var media in sheet.MediaRules)
                {
                    CssParser.CssSerializerVisitor.Instance.Process(
                        sb,
                        media,
                        (cn) =>
                        {
                            IIdentifier id;
                            TryGetCssClassIdentifier(cn.ClassName, out id);
                            return id?.GetName() ?? cn.ClassName;
                        },
                        (idN) => idN.Id);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Optimizes CSS class name identifiers for minification.
        /// Call after all templates using this CSS have been processed.
        /// </summary>
        public void CompressNames()
        {
            _cssScope.Optimize();
        }

        /// <summary>
        /// Validates that all used CSS variables are declared in :root.
        /// </summary>
        public void ValidateCssVariables()
        {
            var allDeclared = new HashSet<string>();
            var allUsed = new HashSet<string>();

            foreach (var sheet in _sheets)
            {
                foreach (var v in sheet.DeclaredCssVariables) allDeclared.Add(v);
                foreach (var v in sheet.UsedCssVariables) allUsed.Add(v);
            }

            var undeclared = new List<string>();
            foreach (var used in allUsed)
            {
                if (!allDeclared.Contains(used))
                    undeclared.Add(used);
            }

            if (undeclared.Count > 0)
            {
                var msg = undeclared.Count == 1
                    ? $"CSS variable '{undeclared[0]}' is not defined in :root."
                    : $"CSS variables {string.Join(", ", undeclared.ConvertAll(v => $"'{v}'"))} are not defined in :root.";

                throw new NScript.Converter.ConverterLocationException(
                    new Location("", 0, 0), msg);
            }
        }

        /// <summary>
        /// Replaces CSS class names in an HTML class attribute value with their minified versions.
        /// Returns the original string unchanged if no CSS manager is active.
        /// </summary>
        public string ReplaceCssClassNames(string classAttrValue)
        {
            if (string.IsNullOrEmpty(classAttrValue)) return classAttrValue;

            var classNames = classAttrValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new string[classNames.Length];

            for (int i = 0; i < classNames.Length; i++)
            {
                IIdentifier id;
                if (TryGetCssClassIdentifier(classNames[i], out id))
                    result[i] = id.GetName();
                else
                    result[i] = classNames[i]; // keep original if not in CSS
            }

            return string.Join(" ", result);
        }

        private void AddCssRules(RazorCssSheet sheet, IList<CssParser.CssRule> rules)
        {
            sheet.Rules.AddRange(rules);

            foreach (var rule in rules)
            {
                CssParser.CssClassNameFinderVisitor.Instance.Process(
                    rule,
                    (cn, nested) => AddCssClassName(sheet, cn, nested));
            }
        }

        private void AddMediaRules(RazorCssSheet sheet, IList<CssParser.Media> mediaRules)
        {
            if (mediaRules == null || mediaRules.Count == 0) return;

            sheet.MediaRules.AddRange(mediaRules);

            foreach (var mediaRule in mediaRules)
            {
                foreach (var rule in mediaRule.RuleSet)
                {
                    CssParser.CssClassNameFinderVisitor.Instance.Process(
                        rule,
                        (cn, nested) => AddCssClassName(sheet, cn, nested));
                }
            }
        }

        private void AddCssClassName(RazorCssSheet sheet, CssParser.CssClassName cn, bool nested)
        {
            if (cn == null) return;

            // Check if class already exists in a previous sheet
            IIdentifier previousIdentifier = null;
            bool isDeclared = false;
            RazorCssSheet declaredSheet = null;

            foreach (var prevSheet in _sheets) // _sheets only contains previously added sheets
            {
                Tuple<IIdentifier, bool> prevResult;
                if (prevSheet.ClassNames.TryGetValue(cn.ClassName, out prevResult))
                {
                    if (prevResult.Item2 || previousIdentifier == null)
                    {
                        declaredSheet = prevSheet;
                        previousIdentifier = prevResult.Item1;
                        isDeclared = isDeclared || prevResult.Item2;
                    }
                }
            }

            // Error if re-declaring a class from a previous sheet at top level
            if (isDeclared && !nested)
            {
                throw new NScript.Converter.ConverterLocationException(
                    new Location(sheet.ResourceName, cn.Line, cn.Col),
                    $"Class name {cn.ClassName} is already declared in {declaredSheet.ResourceName}. " +
                    "You can only use this class with modifiers in this file.");
            }

            // Register or reuse the identifier
            Tuple<IIdentifier, bool> result;
            if (!sheet.ClassNames.TryGetValue(cn.ClassName, out result))
            {
                var identifier = previousIdentifier
                    ?? SimpleIdentifier.CreateScopeIdentifier(_cssScope, cn.ClassName, false, true);

                sheet.ClassNames[cn.ClassName] = Tuple.Create(identifier, !nested || previousIdentifier == null);

                // Also register in the global lookup
                _allClassNames[cn.ClassName] = sheet.ClassNames[cn.ClassName];

                identifier.AddUsage(null);
            }
            else if (!result.Item2 && !nested)
            {
                IIdentifier identifier = SimpleIdentifier.CreateScopeIdentifier(_cssScope, cn.ClassName, false, true);
                sheet.ClassNames[cn.ClassName] = Tuple.Create(identifier, true);
                _allClassNames[cn.ClassName] = sheet.ClassNames[cn.ClassName];
                identifier.AddUsage(null);
            }
        }
    }

    /// <summary>
    /// Represents a single parsed CSS stylesheet within a Razor template's @styles chain.
    /// </summary>
    public class RazorCssSheet
    {
        public string ResourceName { get; }

        public List<CssParser.CssRule> Rules { get; } = new List<CssParser.CssRule>();
        public List<CssParser.CssKeyframes> KeyFrames { get; } = new List<CssParser.CssKeyframes>();
        public List<CssParser.Media> MediaRules { get; } = new List<CssParser.Media>();

        /// <summary>
        /// Class names found in this sheet. Maps class name → (identifier, isDeclared).
        /// </summary>
        public Dictionary<string, Tuple<IIdentifier, bool>> ClassNames { get; }
            = new Dictionary<string, Tuple<IIdentifier, bool>>();

        public HashSet<string> DeclaredCssVariables { get; } = new HashSet<string>();
        public HashSet<string> UsedCssVariables { get; } = new HashSet<string>();

        public RazorCssSheet(string resourceName)
        {
            ResourceName = resourceName;
        }
    }
}

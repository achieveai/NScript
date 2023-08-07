// file:	CssGroup.cs
//
// summary:	Implements the CSS group class

namespace XwmlParser
{
    using CssParser;
    using NScript.Converter;
    using NScript.JST;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// CSS style sheet.
    /// </summary>
    public class CssStyleSheet
    {
        private static Microsoft.ClearScript.V8.V8ScriptEngine jsEngine = new Microsoft.ClearScript.V8.V8ScriptEngine();
        private static Autoprefixer.Compiler compiler;

        public static readonly Autoprefixer.BrowserSpecification browserSpecification =
            new Autoprefixer.BrowserSpecification()
                .BrowserVersionGreaterThanOrEqual(Autoprefixer.Browsers.Firefox, 27)
                .BrowserVersionGreaterThanOrEqual(Autoprefixer.Browsers.Chrome, 27)
                .BrowserVersionGreaterThanOrEqual(Autoprefixer.Browsers.Safari, 5)
                .BrowserVersionGreaterThanOrEqual(Autoprefixer.Browsers.Explorer, 10)
                .BrowserVersionGreaterThanOrEqual(Autoprefixer.Browsers.iOS, 5)
                .BrowserVersionGreaterThanOrEqual(Autoprefixer.Browsers.Android, 4);

        /// <summary>
        /// Context for the parser.
        /// </summary>
        ParserContext parserContext;

        /// <summary>
        /// The CSS rules.
        /// </summary>
        List<CssParser.CssRule> cssRules = new List<CssParser.CssRule>();

        /// <summary>
        /// The key frames.
        /// </summary>
        List<CssParser.CssKeyframes> keyFrames = new List<CssParser.CssKeyframes>();

        /// <summary>
        /// The media rules.
        /// </summary>
        List<CssParser.Media> mediaRules = new List<CssParser.Media>();

        /// <summary>
        /// List of names of the class.
        /// </summary>
        Dictionary<string, Tuple<IIdentifier, bool>> classNames = new Dictionary<string, Tuple<IIdentifier, bool>>();

        /// <summary>
        /// The dependencies.
        /// </summary>
        List<CssStyleSheet> dependencies = new List<CssStyleSheet>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parserContext"> Context for the parser. </param>
        public CssStyleSheet(ParserContext parserContext, string resourceName)
        {
            this.parserContext = parserContext;
            this.ResourceName = resourceName;
        }

        public static Autoprefixer.Compiler Compiler
        {
            get
            {
                if (CssStyleSheet.compiler == null)
                {
                    CssStyleSheet.compiler = new Autoprefixer.Compiler(() => CssStyleSheet.jsEngine);
                }

                return CssStyleSheet.compiler;
            }
        }

        public IEnumerable<string> ClassNames
        {
            get { return this.classNames.Keys; }
        }

        public IList<CssStyleSheet> Dependencies
        { get { return this.dependencies; } }

        /// <summary>
        /// Gets or sets the name of the resource.
        /// </summary>
        /// <value>
        /// The name of the resource.
        /// </value>
        public string ResourceName { get; private set; }

        public bool CheckDuplicateCssRule()
        {
            var cssRuleSizeDict = new Dictionary<int, List<CssRule>>();

            foreach (var rule in cssRules)
            {
                var hasComplexSelector = false;

                foreach (var selector in rule.Selectors)
                {
                    if(selector.GetType() != typeof(CssClassName))
                    {
                        hasComplexSelector = true;
                        break;
                    }
                }

                if (hasComplexSelector)
                { continue; }
                
                //put all css with same property count in 1 bucket to compare
                if(!cssRuleSizeDict.TryGetValue(rule.Properties.Count, out var temp))
                {
                    cssRuleSizeDict[rule.Properties.Count] = new List<CssRule>();
                }

                cssRuleSizeDict[rule.Properties.Count].Add(rule);
            }

            var hasMultipleSameSizedCssRules = false;

            foreach (var kvpair in cssRuleSizeDict)
            {
                var sameSizedCssRules = kvpair.Value;

                if(sameSizedCssRules.Count < 2)
                { continue; }

                hasMultipleSameSizedCssRules = true;

                for(int i = 0; i < sameSizedCssRules.Count - 1; i++)
                {
                    for(int j = i+1; j < sameSizedCssRules.Count; j++)
                    {
                        if (CompareCssRules(sameSizedCssRules[i], sameSizedCssRules[j]))
                        {
                            //TODO Partial CSS matches
                            this.parserContext.ConverterContext.AddError(
                                new Location(
                                    ResourceName,
                                    sameSizedCssRules[i].Selectors[0].Line,
                                    sameSizedCssRules[i].Selectors[0].Col),
                                string.Format(
                                    "Class \"{0}\" has same properties as that of \"{1}\". Please reuse existing class",
                                    string.Join(',', sameSizedCssRules[i].Selectors.Select(s => s.SelectorName)),
                                    string.Join(',', sameSizedCssRules[j].Selectors.Select(s => s.SelectorName))),
                                true);
                            /*File.AppendAllText("D:/new_folder/Duplicate_css_errors.txt", string.Format(
                                    "Class \"{0}\" has same properties as that of \"{1}\". Please reuse existing class." + Environment.NewLine,
                                    i + " " + string.Join(',', sameSizedCssRules[i].Selectors.Select(s => s.SelectorName)),
                                    j + " " + string.Join(',', sameSizedCssRules[j].Selectors.Select(s => s.SelectorName))));*/
                        }
                    }
                }
            }

            if(!hasMultipleSameSizedCssRules)
            { return false; }

            return false;
        }

        /// <summary>
        /// Gets CSS string.
        /// </summary>
        /// <returns>
        /// The CSS string.
        /// </returns>
        public string GetCssString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var cssRule in this.cssRules)
            {
                CssSerializerVisitor.Instance.Process(
                    sb,
                    cssRule,
                    (cn) =>
                    {
                        IIdentifier identifier;
                        this.TryGetCssClassIdentifier(cn.ClassName, out identifier);
                        return identifier.GetName();
                    },
                    (idN) =>
                    {
                        return idN.Id;
                    });
            }

            foreach (var keyframes in this.keyFrames)
            {
                CssSerializerVisitor.Instance.Process(
                    sb,
                    keyframes);
            }

            foreach (var media in this.mediaRules)
            {
                CssSerializerVisitor.Instance.Process(
                    sb,
                    media,
                    (cn) =>
                    {
                        IIdentifier identifier;
                        this.TryGetCssClassIdentifier(cn.ClassName, out identifier);
                        return identifier.GetName();
                    },
                    (idN) =>
                    {
                        return idN.Id;
                    });
            }

            return sb.ToString();
        }

        /// <summary>
        /// Adds the CSS.
        /// </summary>
        /// <param name="cssText"> The CSS text. </param>
        internal void AddCss(
            string cssText,
            Location cssBlockStartPosition,
            List<CssStyleSheet> previousStyles)
        {
            try
            {
                var grammer = new CssParser.CssGrammer(cssText);
                this.AddCssRules(
                    grammer.Rules,
                    cssBlockStartPosition,
                    previousStyles);
                this.AddKeyFrames(grammer.KeyFrames);
                this.AddMediaRules(
                    grammer.MediaRules,
                    cssBlockStartPosition,
                    previousStyles);
            }
            catch(CssParser.ParseException ex)
            {
                throw new ConverterLocationException(
                    new Location(
                        cssBlockStartPosition.FileName,
                        ex.Line + cssBlockStartPosition.StartLine,
                        (ex.Line == 1 ? cssBlockStartPosition.StartColumn : 0) + ex.Position),
                    ex.Message);
            }
        }

        /// <summary>
        /// Try get CSS class identifier.
        /// </summary>
        /// <param name="className">  Name of the class. </param>
        /// <param name="identifier"> [out] The identifier. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public bool TryGetCssClassIdentifier(string className, out IIdentifier identifier)
        {
            Tuple<IIdentifier, bool> result;
            if (this.classNames.TryGetValue(className, out result))
            {
                identifier = result.Item1;
                return true;
            }

            identifier = null;
            return false;
        }

        /// <summary>
        /// Adds the CSS rules.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="rules"> The rules. </param>
        private void AddCssRules(
            IList<CssParser.CssRule> rules,
            Location cssBlockStartPosition,
            List<CssStyleSheet> previousStyles)
        {
            this.cssRules.AddRange(rules);

            for (int iRule = 0; iRule < rules.Count; iRule++)
            {
                CssClassNameFinderVisitor.Instance.Process(
                    rules[iRule],
                    (cn, nested) => this.AddCssClassName(cn, nested, cssBlockStartPosition, previousStyles));
            }
        }

        /// <summary>
        /// Adds a key frames.
        /// </summary>
        /// <param name="list"> The list. </param>
        private void AddKeyFrames(IList<CssParser.CssKeyframes> list)
        {
            this.keyFrames.AddRange(list);
        }

        /// <summary>
        /// Adds a media rules.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="mediaRules"> The media rules. </param>
        private void AddMediaRules(
            IList<CssParser.Media> mediaRules,
            Location cssBlockStartPosition,
            IList<CssStyleSheet> prevoiusStyles)
        {
            if (mediaRules != null
                && mediaRules.Count > 0)
            {
                this.mediaRules.AddRange(mediaRules);

                foreach (var mediaRule in mediaRules)
                {
                    for (int iRule = 0; iRule < mediaRule.RuleSet.Count; iRule++)
                    {
                        CssClassNameFinderVisitor.Instance.Process(
                            mediaRule.RuleSet[iRule],
                            (cn, nested) => this.AddCssClassName(cn, nested, cssBlockStartPosition, prevoiusStyles));
                    }
                }
            }
        }

        private void AddCssClassName(
            CssParser.CssClassName cn,
            bool nested,
            Location cssBlockStartPosition,
            IList<CssStyleSheet> previousSheets)
        {
            if (cn == null)
            {
                throw new ApplicationException(
                    "Can't use any other selector than Class name selector in Css");
            }

            IIdentifier previousIdentifier = null;
            bool isDeclared = false;
            Tuple<IIdentifier, bool> result;
            CssStyleSheet declaredSheet = null;
            foreach (var previousStyle in previousSheets)
            {
                if (previousStyle.classNames.TryGetValue(cn.ClassName, out result))
                {
                    if (result.Item2 || previousIdentifier == null)
                    {
                        declaredSheet = previousStyle;
                        previousIdentifier = result.Item1;
                        isDeclared = isDeclared || result.Item2;
                    }
                }
            }

            if (declaredSheet != null
                && this.dependencies.IndexOf(declaredSheet) < 0)
            {
                this.dependencies.Add(declaredSheet);
            }

            if (isDeclared && !nested)
            {
                this.parserContext.ConverterContext.AddError(
                    new Location(
                        cssBlockStartPosition.FileName,
                        cssBlockStartPosition.StartLine + cn.Line,
                        (cn.Line == 1 ? cssBlockStartPosition.StartLine : 0) + cn.Col),
                    string.Format(
                        "Class name {0} is already declared in {1}. You can only use this class with modifiers in this file",
                        cn.ClassName,
                        declaredSheet.ResourceName),
                    false);
            }

            // TODO: there is still a case where all the classes in a selector are shared css classes. We need to
            // disallow this.
            if (!this.classNames.TryGetValue(cn.ClassName, out result))
            {
                this.classNames[cn.ClassName] =
                    Tuple.Create(
                        previousIdentifier ?? parserContext.RegisterCssClassName(cn.ClassName),
                        !nested || previousIdentifier == null);
            }
            else if (!result.Item2 && !nested)
            {
                this.classNames[cn.ClassName] =
                    Tuple.Create(
                        parserContext.RegisterCssClassName(cn.ClassName),
                        true);
            }

            this.classNames[cn.ClassName].Item1.AddUsage(null);
        }

        public void RemoveUnusedCssRules(List<string> usedCssClasses)
        {
            var newCssRules = new List<CssRule>();
            for(int i=0; i<cssRules.Count; i++)
            {
                var cssRule = cssRules[i];
                var newSelectors = new List<CssSelector>();

                foreach(var selector in cssRule.Selectors)
                {
                    //currently only checking simple classnames
                    if(selector.GetType() != typeof(CssClassName))
                    {
                        newSelectors.Add(selector);
                        continue;
                    }

                    if (usedCssClasses.Contains(((CssClassName)selector).ClassName))
                    {
                        newSelectors.Add(selector);
                    }
                }

                if(newSelectors.Count > 0)
                {
                    newCssRules.Add(new CssRule(newSelectors, cssRule.Properties));
                }
            }

            cssRules = newCssRules;
        }

        private bool CompareCssRules(CssRule rule1, CssRule rule2)
        {
            if (rule1.Properties.Count != rule2.Properties.Count)
            {
                return false;
            }

            var properties1 = rule1.Properties.ToList();
            properties1.Sort((l,r) => string.Compare(l.PropertyName, r.PropertyName));

            var properties2 = rule2.Properties.ToList();
            properties2.Sort((l,r) => string.Compare(l.PropertyName, r.PropertyName));

            //currently only checking exact match (property order can be different)
            for(int i=0;i < properties1.Count;i++)
            {
                if (properties1[i].PropertyName != properties2[i].PropertyName)
                { return false; }

                if (properties1[i].PropertyArgs.Count != properties2[i].PropertyArgs.Count)
                { return false; }

                if (properties1[i].Priority != properties2[i].Priority)
                { return false; }

                if (!CompareCssPropertyValues(
                    properties1[i].PropertyArgs,
                    properties2[i].PropertyArgs))
                { return false; }
            }

            return true;
        }

        private bool CompareCssPropertyValues(IList<CssPropertyValueSet> set1, IList<CssPropertyValueSet> set2)
        {
            if(set1.Count != set2.Count)
            { return false; }

            for (int i = 0; i < set1.Count; i++)
            {
                if (set1[i].Values.Count != set2[i].Values.Count)
                { return false; }

                var values1 = set1[i].Values.ToList();
                var values2 = set2[i].Values.ToList();
                values1.Sort((l,r) => string.Compare(l.ToString(), r.ToString()));
                values2.Sort((l,r) => string.Compare(l.ToString(), r.ToString()));

                if(string.Join(' ', values1) != string.Join(' ', values2)) { return false; }
            }

            return true;
        }
    }
}

// file:	CssGroup.cs
//
// summary:	Implements the CSS group class

namespace XwmlParser
{
    using NScript.Converter;
    using NScript.JST;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// CSS style sheet.
    /// </summary>
    public class CssStyleSheet
    {
        private static JavaScriptEngineSwitcher.V8.V8JsEngine jsEngine = new JavaScriptEngineSwitcher.V8.V8JsEngine();
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
    }
}

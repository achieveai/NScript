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

        private static Autoprefixer.BrowserSpecification browserSpecification =
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
        Dictionary<string, IIdentifier> classNames = new Dictionary<string, IIdentifier>();

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

            return CssStyleSheet.Compiler.Prefix(
                sb.ToString(),
                CssStyleSheet.browserSpecification);
        }

        /// <summary>
        /// Adds the CSS.
        /// </summary>
        /// <param name="cssText"> The CSS text. </param>
        internal void AddCss(string cssText)
        {
            try
            {
                var grammer = new CssParser.CssGrammer(cssText);
                this.AddCssRules(grammer.Rules);
                this.AddKeyFrames(grammer.KeyFrames);
                this.AddMediaRules(grammer.MediaRules);
            }
            catch(CssParser.ParseException ex)
            {
                throw new ConverterLocationException(
                    new Location(
                        this.ResourceName,
                        ex.Line,
                        ex.Position),
                    ex.Message);
            }
        }

        /// <summary>
        /// Adds the less.
        /// </summary>
        /// <param name="lessText"> The less text. </param>
        internal void AddLess(string lessText)
        {
            dotless.Core.LessEngine engine = new dotless.Core.LessEngine();
            var css = engine.TransformToCss(lessText, this.ResourceName);
            this.AddCss(css);
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
            return this.classNames.TryGetValue(className, out identifier);
        }

        /// <summary>
        /// Adds the CSS rules.
        /// </summary>
        /// <exception cref="ApplicationException"> Thrown when an Application error condition occurs. </exception>
        /// <param name="rules"> The rules. </param>
        private void AddCssRules(IList<CssParser.CssRule> rules)
        {
            this.cssRules.AddRange(rules);

            for (int iRule = 0; iRule < rules.Count; iRule++)
            {
                CssClassNameFinderVisitor.Instance.Process(
                    rules[iRule],
                    (cn) =>
                    {
                        if (cn == null)
                        {
                            throw new ApplicationException(
                                "Can't use any other selector than Class name selector in Css");
                        }
                        if (!this.classNames.ContainsKey(cn.ClassName))
                        {
                            this.classNames[cn.ClassName] =
                                parserContext.RegisterCssClassName(cn.ClassName);
                        }

                        this.classNames[cn.ClassName].AddUsage(null);
                    });
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
        private void AddMediaRules(IList<CssParser.Media> mediaRules)
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
                            (cn) =>
                            {
                                if (cn == null)
                                {
                                    throw new ApplicationException(
                                        "Can't use any other selector than Class name selector in Css");
                                }
                                if (!this.classNames.ContainsKey(cn.ClassName))
                                {
                                    this.classNames[cn.ClassName] =
                                        parserContext.RegisterCssClassName(cn.ClassName);
                                }

                                this.classNames[cn.ClassName].AddUsage(null);
                            });
                    }
                }
            }
        }
    }
}

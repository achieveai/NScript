//-----------------------------------------------------------------------
// <copyright file="CssGrammer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;

    /// <summary>
    /// Definition for CssGrammer
    /// </summary>
    public partial class CssGrammer
    {
        public static readonly System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        private List<CssRule> rules;
        private List<CssProperty> properties;
        private List<CssKeyframes> keyFrames;
        private List<Media> mediaRules;
        private HashSet<string> definedCssVariables = new HashSet<string>();
        private HashSet<string> usedCssVariables = new HashSet<string>();

        public CssGrammer(string css, bool parseProperties = false)
        {
            try
            {
                stopWatch.Start();
                ANTLRStringStream input = new ANTLRStringStream(css);
                CssGrammerLexer lexer = new CssGrammerLexer(input);
                CommonTokenStream tokenStream = new CommonTokenStream(lexer);
                CssGrammerParser parser = new CssGrammerParser(tokenStream);
                if (parseProperties)
                {
                    CommonTree tree = parser.declarationSet().Tree;

                    this.ParseStyle(tree);
                }
                else
                {
                    CommonTree tree = parser.styleSheet().Tree;

                    this.ParseCss(tree);
                }
            }
            catch(Antlr.Runtime.RecognitionException ex)
            {
                throw new ParseException(ex);
            }
            finally
            {
                stopWatch.Stop();
            }
        }

        public List<CssRule> Rules
        { get { return this.rules; } }

        public List<CssKeyframes> KeyFrames
        { get { return this.keyFrames; } }

        public List<Media> MediaRules
        {
            get { return this.mediaRules; }
        }

        public List<CssProperty> Properties
        { get { return this.properties; } }

        public HashSet<string> DefinedCssVariables
        { get { return this.definedCssVariables; } }

        public HashSet<string> UsedCssVariables
        { get { return this.usedCssVariables; } }

        private void ParseStyle(ITree tree)
        {
            this.properties = new List<CssProperty>();
            switch (tree.Text)
            {
                case "PROPERTY":
                    this.properties.Add(this.ParseProperty(tree));
                    break;
                default:
                    for (int i = 0; i < tree.ChildCount; i++)
                    {
                        this.properties.Add(this.ParseProperty(tree.GetChild(i)));
                    }
                    break;
            }
        }

        private void ParseCss(ITree tree)
        {
            if (this.rules == null)
            {
                this.rules = new List<CssRule>();
            }

            if (this.keyFrames == null)
            {
                this.keyFrames = new List<CssKeyframes>();
            }

            switch (tree.Text)
            {
                case "ruleset":
                case null:
                    for (int i = 0; i < tree.ChildCount; i++)
                    {
                        var child = tree.GetChild(i);
                        this.ParseCss(child);
                    }

                    break;
                case "RULE":
                    this.rules.Add(this.ParseRule(tree));
                    break;
                case "KEYFRAMES":
                    this.keyFrames.Add(this.ParseKeyframes(tree));
                    break;
                case "MEDIA":
                    if (this.mediaRules == null)
                    {
                        this.mediaRules = new List<Media>();
                    }

                    this.mediaRules.Add(this.ParseMediaElement(tree));
                    break;
                default:
                    if (tree is CommonErrorNode)
                    {
                        throw ((CommonErrorNode)tree).trappedException;
                    }

                    break;
            }
        }

        private Media ParseMediaElement(ITree tree)
        {
            var mqKeyFrames = new List<CssKeyframes>();
            var mqRules = new List<CssRule>();
            List<MediaQuery> mediaQueries = new List<MediaQuery>();
            for (int iChild = 0; iChild < tree.ChildCount; iChild++)
            {
                var child = tree.GetChild(iChild);
                switch (child.Text)
                {
                    case "MEDIA_QUERY":
                        mediaQueries.Add(this.ParseMediaQuery(child));
                        break;
                    case "RULE":
                        mqRules.Add(this.ParseRule(child));
                        break;
                    case "KEYFRAMES":
                        mqKeyFrames.Add(this.ParseKeyframes(tree));
                        break;
                    default:
                        break;
                }
            }

            return new Media(
                mediaQueries,
                mqRules,
                mqKeyFrames);
        }

        private MediaQuery ParseMediaQuery(ITree tree)
        {
            bool hasNotOp = false;
            List<MediaRule> mediaRules = new List<MediaRule>();

            for (int iChild = 0; iChild < tree.ChildCount; iChild++)
            {
                var child = tree.GetChild(iChild);
                switch (child.Text)
                {
                    case "MEDIA_FEATURE":
                        mediaRules.Add(this.ParseMediaFeature(child));
                        break;
                    case "not":
                        hasNotOp = true;
                        break;
                    default:
                        mediaRules.Add(new MediaTypeRule(child.Text));
                        break;
                }
            }

            return new MediaQuery(mediaRules, hasNotOp);
        }

        private PropertyRule ParseMediaFeature(ITree child)
        {
            string propertyName = child.GetChild(0).Text;

            if (child.ChildCount == 1)
            {
                return new PropertyRule(propertyName);
            }

            CssPropertyValue value1 = this.ParsePropertyValue(child.GetChild(1));
            if (child.ChildCount == 2)
            {
                return new PropertyEqualityRule(propertyName, value1);
            }

            string op = child.GetChild(2).Text;

            if (child.ChildCount == 3)
            {
                return new PropertyRangeRule(
                    propertyName,
                    op,
                    value1);
            }

            return new PropertyRangeRule(
                value1,
                op,
                propertyName,
                child.GetChild(4).Text,
                this.ParsePropertyValue(child.GetChild(3)));
        }

        private CssKeyframes ParseKeyframes(ITree tree)
        {
            string name = tree.GetChild(0).Text;
            List<CssKeyframe> frames = new List<CssKeyframe>();
            for (int iChild = 1; iChild < tree.ChildCount; iChild++)
            {
                frames.Add(this.ParseKeyframe(tree.GetChild(iChild)));
            }

            return new CssKeyframes(name, frames);
        }

        private CssKeyframe ParseKeyframe(ITree tree)
        {
            List<CssProperty> properties = new List<CssProperty>();
            List<string> selectors = new List<string>();

            for (int i = 0; i < tree.ChildCount; i++)
            {
                var child = tree.GetChild(i);
                switch (child.Text)
                {
                    case "PROPERTY":
                        properties.Add(this.ParseProperty(child));
                        break;
                    case "KEYFRAMESELECTORS":
                        selectors = this.GetKeyframeSelectors(child);
                        break;
                    default:
                        break;
                }
            }

            return new CssKeyframe(selectors, properties);
        }

        private List<string> GetKeyframeSelectors(ITree tree)
        {
            List<string> selectors = new List<string>();
            for (int iChild = 0; iChild < tree.ChildCount; iChild++)
            {
                selectors.Add(tree.GetChild(iChild).Text.Trim());
            }

            return selectors;
        }

        private CssRule ParseRule(ITree tree)
        {
            List<CssProperty> properties = new List<CssProperty>();
            List<CssSelector> selectors = null;

            for (int i = 0; i < tree.ChildCount; i++)
            {
                var child = tree.GetChild(i);
                switch (child.Text)
                {
                    case "PROPERTY":
                        properties.Add(this.ParseProperty(child));
                        break;
                    case "SELECTORS":
                        selectors = this.ParseSelectors(child);
                        break;
                    case "@font-face":
                        selectors = new List<CssSelector>();
                        selectors.Add(
                            new CssTagName("@font-face",child.Line, child.CharPositionInLine));
                        break;
                    default:
                        break;
                }
            }

            return new CssRule(selectors, properties);
        }

        private List<CssSelector> ParseSelectors(ITree tree)
        {
            List<CssSelector> selectors = new List<CssSelector>();

            for (int i = 0; i < tree.ChildCount; i++)
            {
                var child = tree.GetChild(i);
                switch (child.Text)
                {
                    case "SELECTOR":
                        selectors.Add(this.ParseSelector(child));
                        break;
                    case "SIMPLE_SEL":
                        selectors.Add(this.ParseSimpleSelector(child));
                        break;
                    default:
                        break;
                }
            }

            return selectors;
        }

        public CssSelector ParseSelector(ITree tree)
        {
            List<UnitCssSelector> selectors = new List<UnitCssSelector>();
            List<SelectorOp> operators = new List<SelectorOp>();

            for (int i = 0; i < tree.ChildCount; i++)
            {
                var child = tree.GetChild(i);
                switch (child.Text)
                {
                    case "SELECTOR_OP":
                        this.ParseSelectorOps(child, operators, selectors);
                        break;
                    case "SIMPLE_SEL":
                        this.MergeSelectors(
                            operators,
                            selectors,
                            this.ParseSimpleSelector(child));
                        break;
                    default:
                        break;
                }
            }

            if (selectors.Count == 1)
            {
                return selectors[0];
            }

            return new CssRuleSelector(selectors, operators, tree.Line, tree.CharPositionInLine);
        }

        private void ParseSelectorOps(
            ITree tree,
            List<SelectorOp> operators,
            List<UnitCssSelector> selectors)
        {
            SelectorOp op = SelectorOp.ParentOf;
            switch (tree.GetChild(0).Text)
            {
                case "PARENTOF":
                    op = SelectorOp.ParentOf;
                    break;
                case "PRECEDEDS":
                    op = SelectorOp.Neighbor;
                    break;
                case "FOLLOWS":
                    op = SelectorOp.Follows;
                    break;
                case "UNDER":
                    op = SelectorOp.Under;
                    break;
                default:
                    throw new NotSupportedException();
            }

            operators.Add(op);
            var subTree = tree.GetChild(1);
            this.MergeSelectors(operators, selectors, this.ParseSimpleSelector(subTree));
        }

        private void MergeSelectors(List<SelectorOp> operators, List<UnitCssSelector> selectors, CssSelector simpleSelector)
        {
            CssRuleSelector ruleSelector = simpleSelector as CssRuleSelector;
            if (ruleSelector != null)
            {
                operators.AddRange(ruleSelector.Ops);
                selectors.AddRange(ruleSelector.Selectors);
            }
            else
            {
                selectors.Add((UnitCssSelector)simpleSelector);
            }
        }

        private CssSelector ParseSimpleSelector(ITree tree)
        {
            List<UnitCssSelector> ruleSelectors = new List<UnitCssSelector>();
            List<UnitSimpleCssSelector> selectors = new List<UnitSimpleCssSelector>();
            for (int i = 0; i < tree.ChildCount; i++)
            {
                var child = tree.GetChild(i);
                UnitSimpleCssSelector selector = null;
                switch (child.Text)
                {
                    case "CLASS":
                        selector = this.ParseClass(child);
                        break;
                    case "TAG":
                        selector = this.ParseTag(child);
                        break;
                    case "ID":
                        selector = this.ParseId(child);
                        break;
                    case "ALL":
                        selector = this.ParseAll(child);
                        break;
                    case "ATTRIB":
                        selector = this.ParseAttrib(child);
                        break;
                    case "PSEUDO":
                        selector = this.ParsePseudo(child);
                        break;
                    case "PSEUDO_FUNC":
                        selector = this.ParsePseudoFunc(child);
                        break;
                    case "PSEUDO_FUNC_SELECTOR":
                        selector = this.ParsePseudoFuncSelector(child);
                        break;
                    default:
                        break;
                }

                if (selectors.Count == 0
                    || tree.GetChild(i-1).TokenStopIndex == child.TokenStartIndex-1)
                {
                    selectors.Add(selector);
                }
                else
                {
                    var unitCssSelector = selectors.Count == 1
                        ? (UnitCssSelector)selectors[0]
                        : new AndCssSelector(
                            selectors,
                            child.Line, child.CharPositionInLine);

                    ruleSelectors.Add(unitCssSelector);
                    selectors = new List<UnitSimpleCssSelector>();
                    selectors.Add(selector);
                }
            }

            if (ruleSelectors.Count > 0)
            {
                if (selectors.Count > 1)
                {
                    ruleSelectors.Add(
                        new AndCssSelector(
                            selectors,
                            tree.Line,
                            tree.CharPositionInLine));
                }
                else if (selectors.Count == 1)
                {
                    ruleSelectors.Add(selectors[0]);
                }

                List<SelectorOp> selectorOps = new List<SelectorOp>(ruleSelectors.Count);
                for (int i = 1; i < ruleSelectors.Count; i++)
                {
                    selectorOps.Add(SelectorOp.Under);
                }

                return new CssRuleSelector(ruleSelectors, selectorOps, tree.Line, tree.CharPositionInLine);
            }

            if (selectors.Count == 1)
            {
                return selectors[0];
            }

            return new AndCssSelector(selectors, tree.Line, tree.CharPositionInLine);
        }

        private UnitSimpleCssSelector ParsePseudoFuncSelector(ITree tree)
        {
            var name = tree.GetChild(1).Text.Trim();
            var argChild = tree.GetChild(1);
            return new PseudoNestedSelector(
                name,
                tree.GetChild(1).Text.Trim() == "::",
                this.ParseSelector(tree.GetChild(2)),
                tree.Line,
                tree.CharPositionInLine);
        }

        private PseudoSelector ParsePseudo(ITree child)
        {
            return new PseudoSelector(
                child.GetChild(1).Text, child.GetChild(0).Text == "::",
                child.Line,
                child.CharPositionInLine);
        }

        private PseudoSelector ParsePseudoFunc(ITree tree)
        {
            var name = tree.GetChild(1).Text.Trim();
            if (tree.ChildCount == 2)
            {
                return new PseudoSelector(
                    name,
                    tree.GetChild(0).Text.Trim() == "::",
                    tree.Line,
                    tree.CharPositionInLine,
                    string.Empty);
            }
            else
            {
                var argChild = tree.GetChild(2);
                if (tree.ChildCount == 3)
                {
                    return new PseudoSelector(
                        name,
                        tree.GetChild(1).Text.Trim() == "::",
                        tree.Line,
                        tree.CharPositionInLine,
                        argChild.Text);
                }
                else
                {
                    return new PseudoSelector(
                        name,
                        tree.GetChild(1).Text.Trim() == "::",
                        tree.Line,
                        tree.CharPositionInLine,
                        argChild.Text
                        + tree.GetChild(3).Text
                        + tree.GetChild(4).Text);
                }
            }
        }

        private AttributeSelector ParseAttrib(ITree tree)
        {
            AttributeCondition condition = AttributeCondition.None;
            string attribName = tree.GetChild(0).Text;
            string value = null;
            if (tree.ChildCount == 2)
            {
                tree = tree.GetChild(1);
                switch (tree.GetChild(0).Text)
                {
                    case "ATTRIB_EQUALS":
                        condition = AttributeCondition.Equal;
                        break;
                    case "ATTRIB_CONTAINS":
                        condition = AttributeCondition.Contains;
                        break;
                    case "ATTRIB_CONTAINS_WORD":
                        condition = AttributeCondition.ContainsWord;
                        break;
                    case "ATTRIB_STARTS_WITH":
                        condition = AttributeCondition.StartsWith;
                        break;
                    case "ATTRIB_STARTS_WITH_WORD":
                        condition = AttributeCondition.StartsWithWord;
                        break;
                    case "ATTRIB_ENDS_WITH":
                        condition = AttributeCondition.StartsWith;
                        break;
                    default:
                        break;
                }

                value = tree.GetChild(1).Text;
            }

            return new AttributeSelector(attribName, value, condition, tree.Line, tree.CharPositionInLine);
        }

        private CssClassName ParseClass(ITree tree)
        {
            return new CssClassName(tree.GetChild(0).Text, tree.Line, tree.CharPositionInLine);
        }

        private CssTagName ParseTag(ITree tree)
        {
            return new CssTagName(tree.GetChild(0).Text, tree.Line, tree.CharPositionInLine);
        }

        private CssId ParseId(ITree tree)
        {
            return new CssId(tree.GetChild(0).Text.Substring(1), tree.Line, tree.CharPositionInLine);
        }

        private AllSelector ParseAll(ITree tree)
        {
            return new AllSelector(tree.Line, tree.CharPositionInLine);
        }

        private CssProperty ParseProperty(ITree tree)
        {
            var prio = tree.ChildCount == 3 && tree.GetChild(2).Text == "!important";

            return new CssProperty(
                tree.GetChild(0).Text,
                this.ParsePropertyValueSet(tree.GetChild(1)),
                prio);
        }

        private List<CssPropertyValueSet> ParsePropertyValueSet(ITree tree)
        {
            List<CssPropertyValueSet> rv = new List<CssPropertyValueSet>();
            for (int iChild = 0; iChild < tree.ChildCount; iChild++)
            {
                rv.Add(
                    new CssPropertyValueSet(
                        this.ParsePropertyValues(tree.GetChild(iChild))));
            }

            return rv;
        }

        private List<CssPropertyValue> ParsePropertyValues(ITree tree)
        {
            List<CssPropertyValue> rv = new List<CssPropertyValue>();
            for (int iChild = 0; iChild < tree.ChildCount; iChild++)
            {
                var child = tree.GetChild(iChild);
                var propValue = this.ParsePropertyValue(child);
                if (propValue != null)
                {
                    rv.Add(propValue);
                }
            }

            return rv;
        }

        public CssPropertyValue ParsePropertyValue(ITree child)
        {
            try
            {
                switch (child.Text)
                {
                    case "UNITEXPRS":
                        return new CssPropertyValueSet(this.ParsePropertyValues(child));
                    case "UNIT_VAL":
                        return this.ParseUnitValue(child);
                    case "IDENTIFIER":
                        return new CssIdentifierPropertyValue(child.GetChild(0).Text);
                    case "STRING_VAL":
                        return new CssStringPropertyValue(child.GetChild(0).Text);
                    case "FUNCTION":
                        return this.ParseFunction(child);
                    case "URL_VAL":
                        return new CssStringPropertyValue(child.GetChild(0).Text);
                    case "COLOR":
                        return new CssColorPropertyValue(child.GetChild(0).Text);
                    case "CALC":
                        return this.ParseCalcValue(child);
                    case "RGBA":
                        {
                            List<CssPropertyValue> rgbMethodArgs = new List<CssPropertyValue>();
                            for (int iGrandChild = 0; iGrandChild < child.ChildCount; iGrandChild++)
                            {
                                rgbMethodArgs.Add(
                                    new CssNumberPropertyValue(double.Parse(child.GetChild(iGrandChild).Text)));
                            }

                            return new CssFunctionPropertyValue(
                                rgbMethodArgs.Count == 4 ? "rgba" : "rgb",
                                rgbMethodArgs);
                        }
                    default:
                        return null;
                }
            }
            catch(Exception ex)
            {
                throw new ParseException(
                    "Error parsing CSS ",
                    child.Line,
                    child.CharPositionInLine,
                    ex);
            }
        }

        private CssPropertyValue ParseFunction(ITree tree)
        {
            string name = tree.GetChild(0).Text;
            List<CssPropertyValue> args = new List<CssPropertyValue>();
            CssFunctionPropertyValue rv = new CssFunctionPropertyValue(
                name,
                args);

            for (int iChild = 1; iChild < tree.ChildCount; iChild+=2)
            {
                var child = tree.GetChild(iChild);
                args.Add(this.ParsePropertyValue(child));
            }

            return rv;
        }

        private CssPropertyValue ParseCalcValue(ITree tree)
        {
            int operatorCount = tree.ChildCount / 2;
            char[] operators = new char[operatorCount];
            CssUnitPropertyValue[] unitValues = new CssUnitPropertyValue[operatorCount + 1];
            for (int i = 0; i < operatorCount; i++)
            {
                operators[i] = tree.GetChild(i).Text[0];
            }

            for (int i = 0; i <= operatorCount; i++)
            {
                unitValues[i] = (CssUnitPropertyValue)this.ParseUnitValue(tree.GetChild(i + operatorCount).Text, null);
            }

            if (operatorCount == 0)
            {
                return unitValues[0];
            }

            return new CssCalcPropertyValue(unitValues, operators);
        }

        private CssPropertyValue ParseUnitValue(ITree tree)
        {
            return this.ParseUnitValue(
                tree.GetChild(0).Text.Trim(),
                tree.ChildCount > 1 ? tree.GetChild(1).Text.Trim() : null);
        }

        private CssPropertyValue ParseUnitValue(string value, string op)
        {
            string unit = string.Empty;

            int lastIndex = value.Length;
            for (int i = lastIndex - 1; i >= 0; i--)
            {
                if (char.IsDigit(value[i]))
                {
                    if (i < lastIndex - 1)
                    {
                        unit = value.Substring(i + 1);
                        value = value.Substring(0, i + 1);
                    }

                    break;
                }
            }

            double val;
            if (!double.TryParse(value, out val))
            {
                throw new NotSupportedException();
            }

            if (!string.IsNullOrEmpty(op))
            {
                if (op[0] == '-')
                {
                    val = -val;
                }
            }

            if (string.IsNullOrEmpty(unit))
            {
                return new CssNumberPropertyValue(val);
            }
            else
            {
                return new CssUnitPropertyValue(val, unit);
            }
        }

        /// <summary>
        /// Collects all CSS variable definitions from :root selector
        /// </summary>
        public void CollectCssVariablesFromRules()
        {
            if (this.rules == null)
            {
                return;
            }

            foreach (var rule in this.rules)
            {
                // Check if this is a :root selector
                if (this.IsRootSelector(rule))
                {
                    // Collect all custom properties (variables starting with --)
                    foreach (var property in rule.Properties)
                    {
                        if (property.PropertyName.StartsWith("--"))
                        {
                            this.definedCssVariables.Add(property.PropertyName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a CSS rule has :root selector
        /// </summary>
        private bool IsRootSelector(CssRule rule)
        {
            if (rule.Selectors == null || rule.Selectors.Count == 0)
            {
                return false;
            }

            foreach (var selector in rule.Selectors)
            {
                // Check if it's a pseudo selector with name "root"
                if (selector is PseudoSelector pseudoSelector)
                {
                    if (pseudoSelector.Name == "root")
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Collects used CSS variables from var() functions in all rules
        /// </summary>
        public void CollectUsedCssVariablesFromRules()
        {
            if (this.rules != null)
            {
                foreach (var rule in this.rules)
                {
                    this.CollectUsedVariablesFromRule(rule);
                }
            }

            if (this.mediaRules != null)
            {
                foreach (var mediaRule in this.mediaRules)
                {
                    foreach (var rule in mediaRule.RuleSet)
                    {
                        this.CollectUsedVariablesFromRule(rule);
                    }
                }
            }
        }

        /// <summary>
        /// Collects used CSS variables from a single rule
        /// </summary>
        private void CollectUsedVariablesFromRule(CssRule rule)
        {
            if (rule.Properties == null)
            {
                return;
            }

            foreach (var property in rule.Properties)
            {
                this.CollectUsedVariablesFromProperty(property);
            }
        }

        /// <summary>
        /// Collects used CSS variables from a property
        /// </summary>
        private void CollectUsedVariablesFromProperty(CssProperty property)
        {
            if (property.PropertyArgs == null)
            {
                return;
            }

            foreach (var propertyArg in property.PropertyArgs)
            {
                this.CollectUsedVariablesFromValueSet(propertyArg);
            }
        }

        /// <summary>
        /// Collects used CSS variables from a property value set
        /// </summary>
        private void CollectUsedVariablesFromValueSet(CssPropertyValueSet valueSet)
        {
            if (valueSet.Values == null)
            {
                return;
            }

            foreach (var value in valueSet.Values)
            {
                this.CollectUsedVariablesFromValue(value);
            }
        }

        /// <summary>
        /// Collects used CSS variables from a property value
        /// </summary>
        private void CollectUsedVariablesFromValue(CssPropertyValue value)
        {
            if (value is CssFunctionPropertyValue functionValue)
            {
                // Check if this is a var() function
                if (functionValue.FunctionName == "var" && functionValue.Args != null && functionValue.Args.Count > 0)
                {
                    // Get the variable name (first argument) and add it to the used set
                    var variableName = functionValue.Args[0].ToString();
                    this.usedCssVariables.Add(variableName);
                }

                // Recursively check nested function arguments
                if (functionValue.Args != null)
                {
                    foreach (var arg in functionValue.Args)
                    {
                        this.CollectUsedVariablesFromValue(arg);
                    }
                }
            }
            else if (value is CssPropertyValueSet nestedValueSet)
            {
                // Recursively check nested value sets
                this.CollectUsedVariablesFromValueSet(nestedValueSet);
            }
        }

        /// <summary>
        /// Validates that all var() function calls reference defined CSS variables
        /// </summary>
        private void ValidateCssVariables()
        {
            // First, collect all defined CSS variables from :root
            this.CollectCssVariablesFromRules();

            // Then validate all var() usages
            if (this.rules != null)
            {
                foreach (var rule in this.rules)
                {
                    this.ValidateRuleVariables(rule);
                }
            }

            if (this.mediaRules != null)
            {
                foreach (var mediaRule in this.mediaRules)
                {
                    foreach (var rule in mediaRule.RuleSet)
                    {
                        this.ValidateRuleVariables(rule);
                    }
                }
            }

            if (this.keyFrames != null)
            {
                foreach (var keyframe in this.keyFrames)
                {
                    foreach (var frame in keyframe.Frames)
                    {
                        foreach (var property in frame.Properties)
                        {
                            this.ValidatePropertyVariables(property);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates CSS variables in a single rule
        /// </summary>
        private void ValidateRuleVariables(CssRule rule)
        {
            if (rule.Properties == null)
            {
                return;
            }

            foreach (var property in rule.Properties)
            {
                this.ValidatePropertyVariables(property);
            }
        }

        /// <summary>
        /// Validates CSS variables in a property
        /// </summary>
        private void ValidatePropertyVariables(CssProperty property)
        {
            if (property.PropertyArgs == null)
            {
                return;
            }

            foreach (var propertyArg in property.PropertyArgs)
            {
                this.ValidatePropertyValueSetVariables(propertyArg);
            }
        }

        /// <summary>
        /// Validates CSS variables in a property value set
        /// </summary>
        private void ValidatePropertyValueSetVariables(CssPropertyValueSet valueSet)
        {
            if (valueSet.Values == null)
            {
                return;
            }

            foreach (var value in valueSet.Values)
            {
                this.ValidatePropertyValueVariables(value);
            }
        }

        /// <summary>
        /// Validates CSS variables in a property value
        /// </summary>
        private void ValidatePropertyValueVariables(CssPropertyValue value)
        {
            if (value is CssFunctionPropertyValue functionValue)
            {
                // Check if this is a var() function
                if (functionValue.FunctionName == "var" && functionValue.Args != null && functionValue.Args.Count > 0)
                {
                    // Get the variable name (first argument)
                    var variableName = functionValue.Args[0].ToString();
                    
                    // Check if the variable is defined in :root
                    if (!this.definedCssVariables.Contains(variableName))
                    {
                        throw new ParseException(
                            $"CSS variable '{variableName}' is not defined in :root. All CSS variables must be declared in :root before use.",
                            0,
                            0);
                    }
                }

                // Recursively check nested function arguments
                if (functionValue.Args != null)
                {
                    foreach (var arg in functionValue.Args)
                    {
                        this.ValidatePropertyValueVariables(arg);
                    }
                }
            }
            else if (value is CssPropertyValueSet nestedValueSet)
            {
                // Recursively check nested value sets
                this.ValidatePropertyValueSetVariables(nestedValueSet);
            }
        }
    }
}
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
        private List<CssRule> rules;
        private List<CssProperty> properties;
        private List<CssKeyframes> keyFrames;
        public CssGrammer(string css, bool parseProperties = false)
        {
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

        public List<CssRule> Rules
        { get { return this.rules; } }

        public List<CssKeyframes> KeyFrames
        { get { return this.keyFrames; } }

        public List<CssProperty> Properties
        { get { return this.properties; } }

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
                default:
                    break;
            }
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
                        selectors.Add(this.ParseSimpleSelector(child));
                        break;
                    default:
                        break;
                }
            }

            if (selectors.Count == 1)
            {
                return selectors[0];
            }

            return new CssRuleSelector(selectors, operators);
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
            selectors.Add(this.ParseSimpleSelector(subTree));
        }

        private UnitCssSelector ParseSimpleSelector(ITree tree)
        {
            List<UnitSimpleCssSelector> selectors = new List<UnitSimpleCssSelector>();
            for (int i = 0; i < tree.ChildCount; i++)
            {
                var child = tree.GetChild(i);
                switch (child.Text)
                {
                    case "CLASS":
                        selectors.Add(this.ParseClass(child));
                        break;
                    case "TAG":
                        selectors.Add(this.ParseTag(child));
                        break;
                    case "ID":
                        selectors.Add(this.ParseId(child));
                        break;
                    case "ALL":
                        selectors.Add(this.ParseAll(child));
                        break;
                    case "ATTRIB":
                        selectors.Add(this.ParseAttrib(child));
                        break;
                    case "PSEUDO":
                        selectors.Add(this.ParsePseudo(child));
                        break;
                    case "PSEUDO_FUNC":
                        selectors.Add(this.ParsePseudoFunc(child));
                        break;
                    default:
                        break;
                }
            }

            if (selectors.Count == 1)
            {
                return selectors[0];
            }

            return new AndCssSelector(selectors);
        }

        private PseudoSelector ParsePseudo(ITree child)
        {
            return new PseudoSelector(child.GetChild(0).Text);
        }

        private PseudoSelector ParsePseudoFunc(ITree tree)
        {
            var name = tree.GetChild(0).Text.Trim();
            if (tree.ChildCount == 1)
            {
                return new PseudoSelector(name, string.Empty);
            }
            else
            {
                var argChild = tree.GetChild(1);
                if (tree.ChildCount == 2)
                {
                    return new PseudoSelector(name, argChild.Text);
                }
                else
                {
                    return new PseudoSelector(
                        name,
                        argChild.Text
                        + tree.GetChild(2).Text
                        + tree.GetChild(3).Text);
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

            return new AttributeSelector(attribName, value, condition);
        }

        private CssClassName ParseClass(ITree tree)
        {
            return new CssClassName(tree.GetChild(0).Text);
        }

        private CssTagName ParseTag(ITree tree)
        {
            return new CssTagName(tree.GetChild(0).Text);
        }

        private CssId ParseId(ITree tree)
        {
            return new CssId(tree.GetChild(0).Text.Substring(1));
        }

        private AllSelector ParseAll(ITree tree)
        {
            return new AllSelector();
        }

        private CssProperty ParseProperty(ITree tree)
        {
            return new CssProperty(
                tree.GetChild(0).Text,
                this.ParsePropertyValueSet(tree.GetChild(1)));
        }

        private List<CssPropertyValueSet> ParsePropertyValueSet(ITree tree)
        {
            List<CssPropertyValueSet> rv = new List<CssPropertyValueSet>();
            for (int iChild = 0; iChild < tree.ChildCount; iChild++)
            {
                rv.Add(
                    new CssPropertyValueSet(
                        this.ParsePropertyValue(tree.GetChild(iChild))));
            }

            return rv;
        }

        private List<CssPropertyValue> ParsePropertyValue(ITree tree)
        {
            List<CssPropertyValue> rv = new List<CssPropertyValue>();
            for (int iChild = 0; iChild < tree.ChildCount; iChild++)
            {
                var child = tree.GetChild(iChild);
                switch (child.Text)
                {
                    case "UNIT_VAL":
                        rv.Add(this.ParseUnitValue(child));
                        break;
                    case "IDENTIFIER":
                        rv.Add(new CssIdentifierPropertyValue(child.GetChild(0).Text));
                        break;
                    case "STRING_VAL":
                        rv.Add(new CssStringPropertyValue(child.GetChild(0).Text));
                        break;
                    case "FUNCTION":
                        rv.Add(this.ParseFunction(child));
                        break;
                    case "URL_VAL":
                        throw new NotImplementedException();
                    case "COLOR":
                        rv.Add(new CssColorPropertyValue(child.GetChild(0).Text));
                        break;
                    default:
                        break;
                }
            }

            return rv;
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
                switch (child.Text)
                {
                    case "UNIT_VAL":
                        args.Add(this.ParseUnitValue(child));
                        break;
                    case "IDENTIFIER":
                        args.Add(new CssIdentifierPropertyValue(child.GetChild(0).Text));
                        break;
                    case "STRING_VAL":
                        args.Add(new CssStringPropertyValue(child.GetChild(0).Text));
                        break;
                    case "FUNCTION":
                        args.Add(this.ParseFunction(child));
                        break;
                    case "URL_VAL":
                        throw new NotImplementedException();
                    case "COLOR":
                        args.Add(new CssColorPropertyValue(child.GetChild(0).Text));
                        break;
                    default:
                        break;
                }
            }

            return rv;
        }

        private CssPropertyValue ParseUnitValue(ITree tree)
        {
            string value = tree.GetChild(0).Text.Trim();
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

            string op = tree.ChildCount > 1
                ? tree.GetChild(1).Text
                : null;

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
    }
}
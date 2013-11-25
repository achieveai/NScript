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
                default:
                    break;
            }
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

        private AttributeSelector ParsePseudo(ITree child)
        {
            throw new NotImplementedException();
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
            return new CssId(tree.GetChild(0).Text);
        }

        private AllSelector ParseAll(ITree tree)
        {
            return new AllSelector();
        }

/*
        private CssSelector ParseSelector(ITree tree)
        {
            List<CssSelector> selectors = new List<CssSelector>();
            List<SelectorOp> operators = new List<SelectorOp>();

            for (int i = 0; i < tree.ChildCount; i+=2)
            {
                var subTree = tree.GetChild(i);
                CssSelector subSelector = null;

                switch (subTree.Text)
                {
                    case "CLASS":
                        subSelector = this.ParseClass(subTree);
                        break;
                    case "TAG":
                        subSelector = this.ParseTag(subTree);
                        break;
                    case "ID":
                        subSelector = this.ParseId(subTree);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                selectors.Add(subSelector);

                if (i + 1 < tree.ChildCount)
                {
                    SelectorOp op = SelectorOp.ParentOf;
                    switch (tree.GetChild(i + 1).GetChild(0).Text)
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
                }
            }

            if (selectors.Count == 1)
            {
                return selectors[0];
            }

            return new CssRuleSelector(
                selectors,
                operators);
        }
*/
        private CssProperty ParseProperty(ITree tree)
        {
            return new CssProperty(
                tree.GetChild(0).Text,
                this.ParsePropertyValue(tree.GetChild(1)));
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
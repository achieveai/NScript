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
                CommonTree tree = parser.properties().Tree;

                this.ParseStyle(tree);
            }
            else
            {
                CommonTree tree = parser.stylesheet().Tree;

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
            List<CssSelector> selectors = new List<CssSelector>();
            List<CssProperty> properties = new List<CssProperty>();

            for (int i = 0; i < tree.ChildCount; i++)
            {
                var child = tree.GetChild(i);
                switch (child.Text)
                {
                    case "CLASS":
                        selectors.Add(this.ParseSelector(child));
                        break;
                    case "PROPERTY":
                        properties.Add(this.ParseProperty(child));
                        break;
                    default:
                        break;
                }
            }

            return new CssRule(selectors, properties);
        }

        private CssSelector ParseSelector(ITree tree)
        {
            return new CssClass(tree.GetChild(0).Text);
        }

        private CssProperty ParseProperty(ITree tree)
        {
            string propertyName = tree.GetChild(0).Text;
            List<string> arguments = new List<string>();

            for (int iChild = 1; iChild < tree.ChildCount; iChild++)
            {
                var child = tree.GetChild(iChild);
                if (child.Text == "NUMARG")
                {
                    arguments.Add(child.GetChild(0).Text + child.GetChild(1).Text);
                }
                else
                {
                    arguments.Add(tree.GetChild(iChild).Text);
                }
            }

            return new CssProperty(propertyName, arguments);
        }
    }
}
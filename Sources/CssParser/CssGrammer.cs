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
        private List<CssRule> rules = new List<CssRule>();
        public CssGrammer(string css)
        {
            ANTLRStringStream input = new ANTLRStringStream(css);
            CssGrammerLexer lexer = new CssGrammerLexer(input);
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            CssGrammerParser parser = new CssGrammerParser(tokenStream);
            CommonTree tree = parser.stylesheet().Tree;
            this.Parse(tree);
        }

        public List<CssRule> Rules
        { get { return this.rules; } }

        private void Parse(ITree tree)
        {
            switch (tree.Text)
            {
                case "ruleset":
                    for (int i = 0; i < tree.ChildCount; i++)
                    {
                        var child = tree.GetChild(i);
                        this.Parse(child);
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
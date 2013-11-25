//-----------------------------------------------------------------------
// <copyright file="CssClassNameFinderVisitor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CssClassNameFinderVisitor
    /// </summary>
    public class CssClassNameFinderVisitor : CssParser.CssVisitor
    {
        private static CssClassNameFinderVisitor instance;
        private Action<CssParser.CssClassName> onClassNameFound;
        private CssParser.CssVisitorHelper helper;

        public CssClassNameFinderVisitor()
        {
            this.helper = new CssParser.CssVisitorHelper(this);
        }

        public static CssClassNameFinderVisitor Instance
        {
            get
            {
                if (CssClassNameFinderVisitor.instance == null)
                {
                    CssClassNameFinderVisitor.instance = new CssClassNameFinderVisitor();
                }

                return CssClassNameFinderVisitor.instance;
            }
        }

        public void Process(
            CssParser.CssRule rule,
            Action<CssParser.CssClassName> onClassNameFound)
        {
            this.onClassNameFound = onClassNameFound;
            this.helper.Visit(rule);
            this.onClassNameFound = null;
        }

        public override bool Visit(CssParser.CssClassName obj)
        {
            this.onClassNameFound(obj);
            return false;
        }
    }
}

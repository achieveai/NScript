//-----------------------------------------------------------------------
// <copyright file="CssSerializerVisitor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser
{
    using System;
    using System.Collections.Generic;
    using CssParser;
using System.Text;

    /// <summary>
    /// Definition for CssSerializerVisitor
    /// </summary>
    public class CssSerializerVisitor : CssParser.CssVisitor
    {
        private static CssSerializerVisitor instance;
        private CssParser.CssVisitorHelper helper;
        private  Func<CssClassName,string> onClassNameFound;
        private  Func<CssId,string> onIdFound;
        private  StringBuilder stringBuilder;

        public CssSerializerVisitor()
        {
            this.helper = new CssParser.CssVisitorHelper(this);
        }

        public static CssSerializerVisitor Instance
        {
            get
            {
                if (CssSerializerVisitor.instance == null)
                {
                    CssSerializerVisitor.instance = new CssSerializerVisitor();
                }

                return CssSerializerVisitor.instance;
            }
        }

        public void Process(
            StringBuilder sb,
            CssParser.CssRule rule,
            Func<CssParser.CssClassName, string> onClassNameFound,
            Func<CssParser.CssId, string> onIdFound)
        {
            try
            {
                this.stringBuilder = sb;
                this.onClassNameFound = onClassNameFound;
                this.onIdFound = onIdFound;
                this.Visit(rule);
            }
            finally
            {
                this.stringBuilder = null;
                this.onClassNameFound = null;
                this.onIdFound = null;
            }
        }

        public override bool Visit(AllSelector obj)
        {
            this.stringBuilder.Append('*');
            return false;
        }
        public override bool Visit(CssClassName obj)
        {
            this.stringBuilder.Append('.');
            this.stringBuilder.Append(this.onClassNameFound(obj));
            return false;
        }
        public override bool Visit(CssId obj)
        {
            this.stringBuilder.Append('#');
            this.stringBuilder.Append(this.onIdFound(obj));
            return false;
        }
        public override bool Visit(CssTagName obj)
        {
            this.stringBuilder.Append(obj.Tag);
            return false;
        }
        public override bool Visit(AndCssSelector obj)
        {
            return true;
        }
        public override bool Visit(CssRuleSelector obj)
        {
            for (int iSelector = 0; iSelector < obj.Selectors.Count; iSelector++)
            {
                if (iSelector > 0)
                {
                    string tmpStr = "";
                    switch (obj.Ops[iSelector-1])
                    {
                        case SelectorOp.ParentOf:
                            tmpStr = ">";
                            break;
                        case SelectorOp.Under:
                            tmpStr = " ";
                            break;
                        case SelectorOp.Neighbor:
                            tmpStr = "+";
                            break;
                        case SelectorOp.Follows:
                            tmpStr = "~";
                            break;
                        default:
                            break;
                    }

                    this.stringBuilder.Append(tmpStr);
                }

                this.helper.Visit(obj.Selectors[iSelector]);
            }

            return false;
        }

        public override bool Visit(AttributeSelector obj)
        {
            this.stringBuilder.Append('[');
            this.stringBuilder.Append(obj.Attribute);
            if (obj.Condition != AttributeCondition.None)
            {
                string op = "";
                switch (obj.Condition)
                {
                    case AttributeCondition.Equal:
                        op = "=";
                        break;
                    case AttributeCondition.Contains:
                        op = "*=";
                        break;
                    case AttributeCondition.ContainsWord:
                        op = "~=";
                        break;
                    case AttributeCondition.StartsWith:
                        op = "^=";
                        break;
                    case AttributeCondition.StartsWithWord:
                        op = "|=";
                        break;
                    case AttributeCondition.EndsWith:
                        op = "$=";
                        break;
                    default:
                        break;
                }

                this.stringBuilder.Append(op);
                this.stringBuilder.Append(obj.Value);
            }

            this.stringBuilder.Append(']');
            return false;
        }

        public override bool Visit(PseudoSelector obj)
        {
            this.stringBuilder.Append(':');
            this.stringBuilder.Append(obj.Name);
            if (obj.Arg != null)
            {
                this.stringBuilder.Append('(');
                this.stringBuilder.Append(obj.Arg);
                this.stringBuilder.Append(')');
            }

            return false;
        }

        public override bool Visit(CssProperty obj)
        { return true; }

        public override bool Visit(CssStringPropertyValue obj)
        { return true; }

        public override bool Visit(CssNumberPropertyValue obj)
        { return true; }

        public override bool Visit(CssUnitPropertyValue obj)
        { return true; }

        public override bool Visit(CssColorPropertyValue obj)
        { return true; }

        public override bool Visit(CssIdentifierPropertyValue obj)
        { return true; }

        public override bool Visit(CssRule obj)
        {
            for (int iSelector = 0; iSelector < obj.Selectors.Count; iSelector++)
            {
                if (iSelector > 0)
                { this.stringBuilder.Append(", "); }

                this.helper.Visit((object)obj.Selectors[iSelector]);
            }

            this.stringBuilder.Append("{");
            for (int iProp = 0; iProp < obj.Properties.Count; iProp++)
            {
                var prop = obj.Properties[iProp];
                this.stringBuilder.Append(prop.PropertyName);
                this.stringBuilder.Append(':');
                for (int iValue = 0; iValue < prop.PropertyArgs.Count; iValue++)
                {
                    if (iValue > 0)
                    { this.stringBuilder.Append(' '); }
                    this.stringBuilder.Append(prop.PropertyArgs[iValue].ToString());
                }

                this.stringBuilder.Append(';');
            }

            this.stringBuilder.Append("}");
            return false;
        }
    }
}

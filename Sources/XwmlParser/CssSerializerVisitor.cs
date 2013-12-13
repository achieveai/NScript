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
        private static string[] browserPrefixes = new string[] {"-webkit-", "-moz-", "-o-", "-ms-", String.Empty};
        private static string[] standardNoPrefixes = new string[] { String.Empty };
        private static HashSet<string> verndorSpecificProperties = new HashSet<string>{
            "transform",
            "animation",
            "animation-delay",
            "animation-direction",
            "animation-duration",
            "animation-fill-mode",
            "animation-iteration-count",
            "animation-name",
            "animation-play-state",
            "animation-timing-function",
            "backface-visibility",
            "background-clip",
            "background-origin",
            "box-shadow",
            "box-sizing",
            "clip-path",
            "column-count",
            "column-fill",
            "column-gap",
            "column-rule",
            "column-rule-color",
            "column-rule-style",
            "column-rule-width",
            "column-span",
            "column-width",
            "columns",
            "filter",
            "flex",
            "flex-align",
            "flex-direction",
            "flex-wrap",
            "text-overflow",
            "writing-mode",
            "word-wrap",
            "zoom"

        };
        private static CssSerializerVisitor instance;
        private CssParser.CssVisitorHelper helper;
        private  Func<CssClassName,string> onClassNameFound;
        private  Func<CssId,string> onIdFound;
        private  StringBuilder stringBuilder;
        private string processingPrefix = null;

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

        public void Process(
            StringBuilder sb,
            CssParser.CssKeyframes keyFrames)
        {
            try
            {
                this.stringBuilder = sb;
                this.Visit(keyFrames);
            }
            finally
            {
                this.stringBuilder = null;
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
        {
            return true;
        }

        public override bool Visit(CssPropertyValueSet obj)
        {
            for (int iValue = 0; iValue < obj.Values.Count; iValue++)
            {
                if (iValue > 0)
                {
                    this.stringBuilder.Append(' ');
                }

                this.helper.Visit(obj.Values[iValue]);
            }

            return false;
        }

        public override bool Visit(CssStringPropertyValue obj)
        {
            this.stringBuilder.Append(obj.Value);
            return false;
        }

        public override bool Visit(CssNumberPropertyValue obj)
        {
            this.stringBuilder.Append(obj.Value);
            return false;
        }

        public override bool Visit(CssUnitPropertyValue obj)
        {
            this.stringBuilder.Append(obj.ToString());
            return false;
        }

        public override bool Visit(CssColorPropertyValue obj)
        {
            this.stringBuilder.Append(obj.HexValue);
            return true;
        }

        public override bool Visit(CssFunctionPropertyValue obj)
        {
            this.stringBuilder.Append(obj.FunctionName);
            this.stringBuilder.Append('(');
            for (int iArg = 0; iArg < obj.Args.Count; iArg++)
            {
                if (iArg > 0)
                { this.stringBuilder.Append(','); }

                this.helper.Visit(obj.Args[iArg]);
            }
            this.stringBuilder.Append(')');

            return false;
        }

        public override bool Visit(CssIdentifierPropertyValue obj)
        {
            this.stringBuilder.Append(obj.Identifier);
            return false;
        }

        public override bool Visit(CssRule obj)
        {
            for (int iSelector = 0; iSelector < obj.Selectors.Count; iSelector++)
            {
                if (iSelector > 0)
                { this.stringBuilder.Append(", "); }

                this.helper.Visit((object)obj.Selectors[iSelector]);
            }

            this.stringBuilder.Append("{");
            this.WriteProperties(obj.Properties);
            this.stringBuilder.Append("}");
            return false;
        }

        public override bool Visit(CssKeyframes obj)
        {
            foreach (var prefix in CssSerializerVisitor.browserPrefixes)
            {
                this.stringBuilder.AppendFormat(" @{0}keyframes ", prefix);
                this.processingPrefix = prefix;
                this.stringBuilder.Append(obj.Name);
                this.stringBuilder.Append('{');
                foreach (var frame in obj.Frames)
                {
                    for (int iSel = 0; iSel < frame.Selectors.Count; iSel++)
                    {
                        if (iSel > 0)
                        {
                            this.stringBuilder.Append(" , ");
                        }

                        this.stringBuilder.Append(frame.Selectors[iSel]);
                    }

                    this.stringBuilder.Append('{');
                    this.WriteProperties(frame.Properties);
                    this.stringBuilder.Append('}');
                }

                this.stringBuilder.Append('}');
            }

            return false;
        }

        private void WriteProperties(IList<CssProperty> properties, string prefix = "")
        {
            foreach (var prop in properties)
            {
                foreach (var propPrefix in (CssSerializerVisitor.verndorSpecificProperties.Contains(prop.PropertyName)
                                        ? CssSerializerVisitor.browserPrefixes
                                        : CssSerializerVisitor.standardNoPrefixes))
                {
                    if (prefix != propPrefix
                        && prefix.Length != 0
                        && propPrefix.Length != 0)
                    { continue; }

                    this.stringBuilder.Append(propPrefix);
                    this.stringBuilder.Append(prop.PropertyName);
                    this.stringBuilder.Append(':');
                    for (int iValue = 0; iValue < prop.PropertyArgs.Count; iValue++)
                    {
                        if (iValue > 0)
                        { this.stringBuilder.Append(", "); }

                        this.Visit(prop.PropertyArgs[iValue]);
                    }

                    this.stringBuilder.Append(';');
                }
            }
        }
    }
}

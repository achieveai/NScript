//-----------------------------------------------------------------------
// <copyright file="CssVisitor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CssVisitor
    /// </summary>
    public class CssVisitor
    {
        public virtual bool Visit(AllSelector obj)
        { return true; }
        public virtual bool Visit(CssClassName obj)
        { return true; }
        public virtual bool Visit(CssId obj)
        { return true; }
        public virtual bool Visit(CssTagName obj)
        { return true; }
        public virtual bool Visit(AndCssSelector obj)
        { return true; }
        public virtual bool Visit(CssRuleSelector obj)
        { return true; }
        public virtual bool Visit(AttributeSelector obj)
        { return true; }
        public virtual bool Visit(PseudoSelector obj)
        { return true; }
        public virtual bool Visit(PseudoNestedSelector obj)
        { return true; }
        public virtual bool Visit(CssProperty obj)
        { return true; }
        public virtual bool Visit(CssPropertyValueSet obj)
        { return true; }
        public virtual bool Visit(CssStringPropertyValue obj)
        { return true; }
        public virtual bool Visit(CssNumberPropertyValue obj)
        { return true; }
        public virtual bool Visit(CssUnitPropertyValue obj)
        { return true; }
        public virtual bool Visit(CssColorPropertyValue obj)
        { return true; }
        public virtual bool Visit(CssIdentifierPropertyValue obj)
        { return true; }
        public virtual bool Visit(CssRule obj)
        { return true; }
        public virtual bool Visit(CssFunctionPropertyValue obj)
        { return true; }
        public virtual bool Visit(CssKeyframes obj)
        { return true; }
        public virtual bool Visit(CssKeyframe obj)
        { return true; }

        public virtual bool Visit(Media media)
        {
            return true;
        }

        public virtual bool Visit(MediaQuery mediaQuery)
        {
            return true;
        }

        public virtual bool Visit(MediaTypeRule mediaTypeRule)
        {
            return true;
        }

        public virtual bool Visit(PropertyRule propertyRule)
        {
            return true;
        }

        public virtual bool Visit(PropertyEqualityRule propertyEqualityRule)
        {
            return true;
        }

        public virtual bool Visit(PropertyRangeRule propertyRangeRule)
        {
            return true;
        }
    }

    public sealed class CssVisitorHelper
    {
        private CssVisitor innerVisitor;

        public CssVisitorHelper(CssVisitor visitor)
        {
            this.innerVisitor = visitor;
        }
        public void Visit(AllSelector obj)
        { }
        public void Visit(CssClassName obj)
        { }
        public void Visit(CssId obj)
        { }
        public void Visit(CssTagName obj)
        { }
        public void Visit(AndCssSelector obj)
        {
            foreach (var item in obj.Selectors)
            {
                this.Visit(item);
            }
        }
        public void Visit(CssRuleSelector obj)
        {
            foreach (var item in obj.Selectors)
            {
                this.Visit(item);
            }
        }
        public void Visit(AttributeSelector obj)
        { }
        public void Visit(PseudoSelector obj)
        { }
        public void Visit(PseudoNestedSelector obj)
        {
            this.Visit(obj.NestedSelector);
        }
        public void Visit(CssProperty obj)
        {
            foreach (var item in obj.PropertyArgs)
            {
                this.Visit(item);
            }
        }
        public void Visit(CssPropertyValueSet obj)
        {
            foreach (var item in obj.Values)
            {
                this.Visit(item);
            }
        }
        public void Visit(CssStringPropertyValue obj)
        { }
        public void Visit(CssNumberPropertyValue obj)
        { }
        public void Visit(CssUnitPropertyValue obj)
        { }
        public void Visit(CssColorPropertyValue obj)
        { }
        public void Visit(CssIdentifierPropertyValue obj)
        { }
        public void Visit(CssFunctionPropertyValue obj)
        {
            foreach (var item in obj.Args)
            {
                this.Visit(item);
            }
        }
        public void Visit(CssRule obj)
        {
            foreach (var item in obj.Selectors)
            {
                this.Visit(item);
            }

            foreach (var item in obj.Properties)
            {
                this.Visit(item);
            }
        }
        public void Visit(CssKeyframes obj)
        {
            foreach (var item in obj.Frames)
            {
                this.Visit(item);
            }
        }
        public void Visit(CssKeyframe obj)
        {
            foreach (var item in obj.Properties)
            {
                this.Visit(item);
            }
        }
        public void Visit(Media media)
        {
            foreach (var query in media.MediaQueires)
            {
                this.Visit(query);
            }

            foreach (var rule in media.RuleSet)
            {
                this.Visit(rule);
            }
        }
        public void Visit(MediaQuery mediaQuery)
        {
            foreach (var rule in mediaQuery.MediaRules)
            {
                this.Visit(rule);
            }
        }
        public void Visit(MediaTypeRule mediaTypeRule)
        {
        }
        public void Visit(PropertyRule propertyRule)
        {
        }
        public void Visit(PropertyEqualityRule propertyEqualityRule)
        {
            this.Visit(propertyEqualityRule.Value);
        }
        public void Visit(PropertyRangeRule propertyRangeRule)
        {
            if (propertyRangeRule.LeftValue != null)
            {
                this.Visit(propertyRangeRule.LeftValue);
            }

            this.Visit(propertyRangeRule.RightValue);
        }
        public void Visit(object obj)
        {
            if (obj == null)
            {
                return;
            }

            switch (obj.GetType().Name)
            {
                case "CssRule":
                    if (this.innerVisitor.Visit((CssRule)obj))
                        this.Visit((CssRule)obj);
                    break;
                case "CssIdentifierPropertyValue":
                    if (this.innerVisitor.Visit((CssIdentifierPropertyValue)obj))
                        this.Visit((CssIdentifierPropertyValue)obj);
                    break;
                case "CssColorPropertyValue":
                    if (this.innerVisitor.Visit((CssColorPropertyValue)obj))
                        this.Visit((CssColorPropertyValue)obj);
                    break;
                case "CssUnitPropertyValue":
                    if (this.innerVisitor.Visit((CssUnitPropertyValue)obj))
                        this.Visit((CssUnitPropertyValue)obj);
                    break;
                case "CssNumberPropertyValue":
                    if (this.innerVisitor.Visit((CssNumberPropertyValue)obj))
                        this.Visit((CssNumberPropertyValue)obj);
                    break;
                case "CssStringPropertyValue":
                    if (this.innerVisitor.Visit((CssStringPropertyValue)obj))
                        this.Visit((CssStringPropertyValue)obj);
                    break;
                case "CssProperty":
                    if (this.innerVisitor.Visit((CssProperty)obj))
                        this.Visit((CssProperty)obj);
                    break;
                case "PseudoSelector":
                    if (this.innerVisitor.Visit((PseudoSelector)obj))
                        this.Visit((PseudoSelector)obj);
                    break;
                case "AttributeSelector":
                    if (this.innerVisitor.Visit((AttributeSelector)obj))
                        this.Visit((AttributeSelector)obj);
                    break;
                case "CssRuleSelector":
                    if (this.innerVisitor.Visit((CssRuleSelector)obj))
                        this.Visit((CssRuleSelector)obj);
                    break;
                case "AndCssSelector":
                    if (this.innerVisitor.Visit((AndCssSelector)obj))
                        this.Visit((AndCssSelector)obj);
                    break;
                case "CssTagName":
                    if (this.innerVisitor.Visit((CssTagName)obj))
                        this.Visit((CssTagName)obj);
                    break;
                case "CssId":
                    if (this.innerVisitor.Visit((CssId)obj))
                        this.Visit((CssId)obj);
                    break;
                case "CssClassName":
                    if (this.innerVisitor.Visit((CssClassName)obj))
                        this.Visit((CssClassName)obj);
                    break;
                case "Media":
                    if (this.innerVisitor.Visit((Media)obj))
                        this.Visit((Media)obj);
                    break;
                case "MediaQuery":
                    if (this.innerVisitor.Visit((MediaQuery)obj))
                        this.Visit((MediaQuery)obj);
                    break;
                case "PropertyRule":
                    if (this.innerVisitor.Visit((PropertyRule)obj))
                        this.Visit((PropertyRule)obj);
                    break;
                case "PropertyEqualityRule":
                    if (this.innerVisitor.Visit((PropertyEqualityRule)obj))
                        this.Visit((PropertyEqualityRule)obj);
                    break;
                case "PropertyRangeRule":
                    if (this.innerVisitor.Visit((PropertyRangeRule)obj))
                        this.Visit((PropertyRangeRule)obj);
                    break;
                case "AllSelector":
                    if (this.innerVisitor.Visit((AllSelector)obj))
                        this.Visit((AllSelector)obj);
                    break;
                case "CssFunctionPropertyValue":
                    if (this.innerVisitor.Visit((CssFunctionPropertyValue)obj))
                        this.Visit((CssFunctionPropertyValue)obj);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}

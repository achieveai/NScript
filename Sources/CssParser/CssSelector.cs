//-----------------------------------------------------------------------
// <copyright file="CssSelector.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// CSS selector.
    /// </summary>
    public abstract class CssSelector
    {
    }

    /// <summary>
    /// Unit CSS selector.
    /// </summary>
    public abstract class UnitCssSelector : CssSelector
    { }

    /// <summary>
    /// Unit simple CSS selector.
    /// </summary>
    public abstract class UnitSimpleCssSelector : UnitCssSelector
    { }

    /// <summary>
    /// CSS class.
    /// </summary>
    public class CssClassName : UnitSimpleCssSelector
    {
        /// <summary>
        /// Name of the class.
        /// </summary>
        private string className;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="className">    Name of the class. </param>
        public CssClassName(string className)
        {
            this.className = className;
        }

        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        /// <value>
        /// The name of the class.
        /// </value>
        public string ClassName
        { get { return this.className; } }
    }

    /// <summary>
    /// CSS tag name.
    /// </summary>
    public class CssTagName : UnitSimpleCssSelector
    {
        private string tagName;

        public CssTagName(string tagName)
        {
            this.tagName = tagName;
        }

        public string Tag
        { get { return this.tagName; } }
    }

    public class CssId : UnitSimpleCssSelector
    {
        private string id;
        public CssId(string id)
        {
            this.id = id;
        }

        public string Id
        { get { return this.id; } }
    }

    public class AllSelector : UnitSimpleCssSelector
    {
        public AllSelector()
        { }
    }

    public enum AttributeCondition
    {
        None,
        Equal,
        Contains,
        ContainsWord,
        StartsWith,
        StartsWithWord,
        EndsWith,
        EndsWithWord,
    }

    public class AttributeSelector : UnitSimpleCssSelector
    {
        private string attribute;
        private string value;
        private AttributeCondition condition;

        public AttributeSelector(
            string attribute,
            string value,
            AttributeCondition condition)
        {
            this.attribute = attribute;
            this.value = value;
            this.condition = condition;
        }

        public string Attribute
        { get { return this.attribute; } }

        public string Value
        { get { return this.value; } }

        public AttributeCondition Condition
        { get { return this.condition; } }
    }

    public class PseudoSelector : UnitSimpleCssSelector
    {
        private string name;
        private string arg;

        public PseudoSelector(
            string name,
            string arg = null)
        {
            this.name = name;
            this.arg = arg;
        }

        public string Name
        { get { return this.name; } }

        public string Arg
        { get { return this.arg; } }
    }

    public class AndCssSelector : UnitCssSelector
    {
        public AndCssSelector(IList<UnitSimpleCssSelector> selectors)
        {
            this.Selectors = selectors;
        }

        public IList<UnitSimpleCssSelector> Selectors { get; private set; }
    }

    public enum SelectorOp
    {
        ParentOf,
        Under,
        Neighbor,
        Follows
    }

    public class CssRuleSelector : CssSelector
    {
        private List<UnitCssSelector> selectors;
        private List<SelectorOp> ops;
        public CssRuleSelector(
            List<UnitCssSelector> selectors,
            List<SelectorOp> ops)
        {
            this.ops = ops;
            this.selectors = selectors;
        }
        public List<UnitCssSelector> Selectors
        { get { return this.selectors; } }

        public List<SelectorOp> Ops
        { get { return this.ops; } }
    }
}

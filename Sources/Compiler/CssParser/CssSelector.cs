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
        public readonly int Col;
        public readonly int Line;

        public CssSelector(
            int line,
            int col)
        {
            this.Line = line;
            this.Col = col;
        }
    }

    /// <summary>
    /// Unit CSS selector.
    /// </summary>
    public abstract class UnitCssSelector : CssSelector
    {
        public UnitCssSelector(int line, int col) : base(line, col) { }
    }

    /// <summary>
    /// Unit simple CSS selector.
    /// </summary>
    public abstract class UnitSimpleCssSelector : UnitCssSelector
    {
        public UnitSimpleCssSelector(int line, int col) : base(line, col) { }
    }

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
        public CssClassName(
            string className,
            int line,
            int col) : base(line, col)
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

        public CssTagName(string tagName, int line, int col)
            :base(line, col)
        {
            this.tagName = tagName;
        }

        public string Tag
        { get { return this.tagName; } }
    }

    public class CssId : UnitSimpleCssSelector
    {
        private string id;
        public CssId(string id, int line, int col)
            : base(line, col)
        {
            this.id = id;
        }

        public string Id
        { get { return this.id; } }
    }

    public class AllSelector : UnitSimpleCssSelector
    {
        public AllSelector(int line, int col)
            :base(line, col)
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
    }

    public class AttributeSelector : UnitSimpleCssSelector
    {
        private string attribute;
        private string value;
        private AttributeCondition condition;

        public AttributeSelector(
            string attribute,
            string value,
            AttributeCondition condition,
            int line,
            int col)
            :base(line, col)
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
        private bool isDouble;

        public PseudoSelector(
            string name,
            bool isDouble,
            int line,
            int col,
            string arg = null)
            :base (line, col)
        {
            this.name = name;
            this.arg = arg;
            this.isDouble = isDouble;
        }

        public string Name
        { get { return this.name; } }

        public string Arg
        { get { return this.arg; } }

        public bool IsDouble
        { get { return this.isDouble; } }
    }

    public class AndCssSelector : UnitCssSelector
    {
        public AndCssSelector(IList<UnitSimpleCssSelector> selectors, int line, int col)
            : base(line, col)
        {
            this.Selectors = selectors;
        }

        public IList<UnitSimpleCssSelector> Selectors { get; private set; }
    }

    public class PseudoNestedSelector : UnitSimpleCssSelector
    {
        private string name;
        private CssSelector nestedSelector;
        private bool isDouble;

        public PseudoNestedSelector(
            string name,
            bool isDouble,
            CssSelector nestedSelector,
            int line,
            int col)
            :base(line, col)
        {
            this.name = name;
            this.nestedSelector = nestedSelector;
            this.isDouble = isDouble;
        }

        public string Name
        { get { return this.name; } }

        public CssSelector NestedSelector
        { get { return this.nestedSelector; } }

        public bool IsDouble
        { get { return this.isDouble; } }
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
            List<SelectorOp> ops,
            int line,
            int col)
            :base(line, col)
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

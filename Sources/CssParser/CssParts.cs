//-----------------------------------------------------------------------
// <copyright file="CssParts.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CssParts
    /// </summary>
    public class CssRule
    {
        IList<CssSelector> selectors;
        IList<CssProperty> properties;

        public CssRule(
            IList<CssSelector> selectors,
            IList<CssProperty> properties)
        {
            this.selectors = selectors;
            this.properties = properties;
        }

        public IList<CssSelector> Selectors
        { get { return this.selectors; } }

        public IList<CssProperty> Properties
        { get { return this.properties; } }
    }

    public abstract class CssSelector
    {
    }

    public class CssClass : CssSelector
    {
        private string className;

        public CssClass(string className)
        {
            this.className = className;
        }

        public string ClassName
        { get { return this.className; } }
    }

    public class CssTagName : CssSelector
    {
    }

    public class CssProperty
    {
        public string propertyName;
        public IList<string> propertyArgs;

        public CssProperty(string propertyName, IList<string> propertyArgs)
        {
            this.propertyName = propertyName;
            this.propertyArgs = propertyArgs;
        }

        public string PropertyName
        { get { return this.propertyName; } }

        public IList<string> PropertyArgs
        { get { return this.propertyArgs; } }
    }
}

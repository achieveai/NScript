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
        /// <summary>
        /// The selectors.
        /// </summary>
        private IList<CssSelector> selectors;

        /// <summary>
        /// The properties.
        /// </summary>
        private IList<CssProperty> properties;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="selectors">    The selectors. </param>
        /// <param name="properties">   The properties. </param>
        public CssRule(
            IList<CssSelector> selectors,
            IList<CssProperty> properties)
        {
            this.selectors = selectors;
            this.properties = properties;
        }

        /// <summary>
        /// Gets the selectors.
        /// </summary>
        /// <value>
        /// The selectors.
        /// </value>
        public IList<CssSelector> Selectors
        { get { return this.selectors; } }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public IList<CssProperty> Properties
        { get { return this.properties; } }
    }

    /// <summary>
    /// CSS property.
    /// </summary>
    public class CssProperty
    {
        /// <summary>
        /// Name of the property.
        /// </summary>
        private string propertyName;

        /// <summary>
        /// The property arguments.
        /// </summary>
        private IList<CssPropertyValue> propertyArgs;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName"> Name of the property. </param>
        /// <param name="propertyArgs"> The property arguments. </param>
        public CssProperty(string propertyName, IList<CssPropertyValue> propertyArgs)
        {
            this.propertyName = propertyName;
            this.propertyArgs = propertyArgs;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        public string PropertyName
        { get { return this.propertyName; } }

        /// <summary>
        /// Gets the property arguments.
        /// </summary>
        /// <value>
        /// The property arguments.
        /// </value>
        public IList<CssPropertyValue> PropertyArgs
        { get { return this.propertyArgs; } }
    }
}

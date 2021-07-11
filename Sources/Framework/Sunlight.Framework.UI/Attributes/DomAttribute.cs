//-----------------------------------------------------------------------
// <copyright file="DomAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Attributes
{
    using System;

    /// <summary>
    /// Definition for DomAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DomAttributeAttribute : Attribute
    {
        private string attributeName;
        private string attributeValue;

        public DomAttributeAttribute(string name, string value)
        {
            this.attributeName = name;
            this.attributeValue = value;
        }

        public string Name
        {
            get
            {
                return this.attributeName;
            }
        }

        public string Value
        {
            get
            {
                return this.attributeValue;
            }
        }
    }
}

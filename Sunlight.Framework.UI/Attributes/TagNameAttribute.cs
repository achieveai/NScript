//-----------------------------------------------------------------------
// <copyright file="TagNameAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Attributes
{
    using System;

    /// <summary>
    /// Definition for TagNameAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TagNameAttribute : Attribute
    {
        private string tagName;

        public TagNameAttribute(string tagName)
        {
            this.tagName = tagName;
        }

        public string TagName
        {
            get
            {
                return this.tagName;
            }
        }
    }
}

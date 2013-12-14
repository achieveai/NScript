//-----------------------------------------------------------------------
// <copyright file="MediaRule.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for MediaRule
    /// </summary>
    public abstract class MediaRule
    {
    }

    public class MediaTypeRule : MediaRule
    {
        public MediaTypeRule(string mediaType)
        {
            this.MediaType = mediaType;
        }

        public string MediaType { get; set; }

        public override string ToString()
        {
            return this.MediaType;
        }
    }

    public class PropertyRule : MediaRule
    {
        public PropertyRule(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; set; }

        public override string ToString()
        {
            return this.PropertyName;
        }
    }

    public class PropertyEqualityRule : PropertyRule
    {
        public PropertyEqualityRule(string propertyName, CssPropertyValue value)
            : base(propertyName)
        {
            this.Value = value;
        }

        public CssPropertyValue Value { get; set; }

        public override string ToString()
        {
            return string.Format("({0}: {1})", this.PropertyName, this.Value);
        }
    }

    public class PropertyRangeRule : PropertyRule
    {
        public PropertyRangeRule(
            string propertyName,
            string op,
            CssPropertyValue value)
            :base(propertyName)
        {
            this.RightOp = op;
            this.RightValue = value;
        }

        public PropertyRangeRule(
            CssPropertyValue value,
            string leftOp,
            string propertyName,
            string rightOp,
            CssPropertyValue rightValue)
            :base(propertyName)
        {
            this.LeftValue = value;
            this.LeftOp = leftOp;
            this.RightOp = rightOp;
            this.RightValue = rightValue;
        }

        public CssPropertyValue RightValue { get; set; }

        public string RightOp { get; set; }

        public string LeftOp { get; set; }

        public CssPropertyValue LeftValue { get; set; }

        public override string ToString()
        {
            if (this.LeftValue != null)
            {
                return string.Format(
                    "({0} {1} {2} {3} {4})",
                    this.LeftValue,
                    this.LeftOp,
                    this.PropertyName,
                    this.RightOp,
                    this.RightValue);
            }
            else
            {
                return string.Format(
                    "({0} {1} {2})",
                    this.PropertyName,
                    this.RightOp,
                    this.RightValue);
            }
        }
    }
}

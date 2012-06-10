//-----------------------------------------------------------------------
// <copyright file="DefaultBindingAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Attributes
{
    using System;
    using Sunlight.Framework.Binders;

    /// <summary>
    /// Definition for DefaultBindingAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultDataBindingAttribute : Attribute
    {
        private DataBindingMode mode;
        private bool isStrict;
        private object defaultValue;

        public DataBindingMode Mode
        {
            get
            {
                return this.mode;
            }

            set
            {
                this.mode = value;
            }
        }

        public bool IsStrict
        {
            get
            {
                return this.isStrict;
            }

            set
            {
                this.isStrict = value;
            }
        }

        public object DefaultValue
        {
            get
            {
                return this.defaultValue;
            }

            set
            {
                this.defaultValue = value;
            }
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="BindingAttribute.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Template.Compiler.Attributes
{
    using Cs2JsC.Template.Compiler.DataBinders;

    /// <summary>
    /// Binding Attributes.
    /// </summary>
    public class BindingAttribute : AttributeBase
    {
        /// <summary>
        /// Const string for default binding property.
        /// </summary>
        public const string DefaultBinding = "DefaultBinding";

        /// <summary>
        /// Const string for IsStrict property.
        /// </summary>
        public const string IsStrictBinding = "IsStrictBinding";

        /// <summary>
        /// Backing field for mode.
        /// </summary>
        private readonly BindingMode mode;

        /// <summary>
        /// Backing field for isStrict.
        /// </summary>
        private readonly bool isStrict;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingAttribute"/> class.
        /// </summary>
        /// <param name="mode">The   mode.</param>
        /// <param name="isStrict">if set to <c>true</c> [is strict].</param>
        public BindingAttribute(
            BindingMode mode,
            bool isStrict)
        {
            this.mode = mode;
            this.isStrict = isStrict;
        }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        /// <value>The mode  .</value>
        public BindingMode Mode
        {
            get
            {
                return this.mode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is strict.
        /// </summary>
        /// <value><c>true</c> if this instance is strict; otherwise, <c>false</c>.</value>
        public bool IsStrict
        {
            get
            {
                return this.isStrict;
            }
        }
    }
}

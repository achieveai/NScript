//-----------------------------------------------------------------------
// <copyright file="BindingMode.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Template.Compiler.DataBinders
{
    /// <summary>
    /// Binding mode.
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        /// One Time binding
        /// </summary>
        OneTime,

        /// <summary>
        /// One way always upto date binding
        /// </summary>
        OneWay,

        /// <summary>
        /// Two Way binding
        /// </summary>
        TwoWay,
    }
}

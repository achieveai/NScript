//-----------------------------------------------------------------------
// <copyright file="ExtensionAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Definition for ExtensionAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.ExtensionAttribute" /> class. </summary>
        public ExtensionAttribute()
        {
        }
    }
}
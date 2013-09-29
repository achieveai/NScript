//-----------------------------------------------------------------------
// <copyright file="TextContentTargetBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
using System;
using System.Collections.Generic;

    /// <summary>
    /// Definition for TextContentTargetBinder
    /// </summary>
    public class TextContentTargetBinder : TargetBindingInfo
    {
        TypeReference stringType;

        public TextContentTargetBinder(
            TypeReference stringType)
        {
            this.stringType = stringType;
        }

        /// <summary>
        /// Gets the type of the binding.
        /// </summary>
        /// <value>
        /// The type of the binding.
        /// </value>
        public override TypeReference BindingType
        { get { return this.stringType; } }
    }
}

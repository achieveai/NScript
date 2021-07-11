//-----------------------------------------------------------------------
// <copyright file="AttachedPropertyTargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for AttachedPropertyTargetBindingInfo
    /// </summary>
    public class AttachedPropertyTargetBindingInfo : TargetBindingInfo
    {
        public AttachedPropertyTargetBindingInfo(
            PropertyReference property)
            : base(property.PropertyType.GenericParameters[0])
        {
            this.Property = property;
        }

        public PropertyReference Property
        { get; private set; }
    }
}

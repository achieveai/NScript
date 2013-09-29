//-----------------------------------------------------------------------
// <copyright file="PropertySourceBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for PropertySourceBindingInfo
    /// </summary>
    public class PropertySourceBindingInfo : SourceBindingInfo
    {
        public PropertySourceBindingInfo(
            TypeReference sourceType,
            List<PropertyReference> propertyReferences)
        {
            this.SourceType = sourceType;
            this.PropertyReferencePath = propertyReferences;
        }

        public TypeReference SourceType { get; private set; }

        public List<PropertyReference> PropertyReferencePath { get; private set; }
    }
}

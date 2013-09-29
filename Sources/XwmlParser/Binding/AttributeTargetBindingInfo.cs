//-----------------------------------------------------------------------
// <copyright file="AttributeTargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for AttributeTargetBindingInfo
    /// </summary>
    public class AttributeTargetBindingInfo : TargetBindingInfo
    {
        private TypeReference stringType;
        private string attributeName;

        public AttributeTargetBindingInfo(
            TypeReference stringType,
            string attributeName)
        {
            this.attributeName = attributeName;
            this.stringType = stringType;
        }

        public override TypeReference BindingType
        {
            get { throw new NotImplementedException(); }
        }
    }
}

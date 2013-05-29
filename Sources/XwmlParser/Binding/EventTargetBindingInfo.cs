//-----------------------------------------------------------------------
// <copyright file="EventTargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for EventTargetBindingInfo
    /// </summary>
    public class EventTargetBindingInfo : TargetBindingInfo
    {
        public override Mono.Cecil.TypeReference BindingType
        {
            get { throw new NotImplementedException(); }
        }
    }
}

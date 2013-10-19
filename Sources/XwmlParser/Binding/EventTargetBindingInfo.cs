//-----------------------------------------------------------------------
// <copyright file="EventTargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for EventTargetBindingInfo
    /// </summary>
    public class EventTargetBindingInfo : TargetBindingInfo
    {
        public EventTargetBindingInfo(EventReference evt)
            : base(evt.EventType)
        {
            this.Event = evt;
        }

        public EventReference Event { get; private set; }
    }
}

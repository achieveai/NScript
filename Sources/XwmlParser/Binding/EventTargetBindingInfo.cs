//-----------------------------------------------------------------------
// <copyright file="EventTargetBindingInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Binding
{
    using Mono.Cecil;
    using NScript.JST;
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

        internal override
            Tuple<string, IIdentifier, IIdentifier, Expression, Expression>
            GenerateGetterSetter(SkinCodeGenerator codeGenerator, bool isTwoWay)
        {
            return new Tuple<string, IIdentifier, IIdentifier, Expression, Expression>(
                null,
                null,
                null,
                new StringLiteralExpression(
                    codeGenerator.CodeGenerator.ScopeManager.Scope,
                    this.Event.Name),
                new NullLiteralExpression(codeGenerator.Scope));
        }
    }
}

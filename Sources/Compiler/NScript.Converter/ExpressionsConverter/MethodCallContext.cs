//-----------------------------------------------------------------------
// <copyright file="MethodCallContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.ExpressionsConverter
{
    using NScript.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for MethodCallContext
    /// </summary>
    public class MethodCallContext
    {
        private JST.Expression thisExpression;
        private bool isVirtual;
        private MethodReference method;
        private Location location;
        private JST.IdentifierScope scope;

        public MethodCallContext(
            JST.Expression thisExpression,
            MethodReference method,
            bool isVirtual)
        {
            this.thisExpression = thisExpression;
            this.isVirtual = isVirtual;
            this.method = method;
            this.location = thisExpression.Location;
            this.scope = thisExpression.Scope;
        }

        public MethodCallContext(
            MethodReference method,
            Location location,
            JST.IdentifierScope scope)
        {
            this.scope = scope;
            this.location = location;
            this.method = method;
        }

        public JST.Expression ThisExpression
        { get { return this.thisExpression; } }

        public MethodReference Method
        { get { return this.method; } }

        public bool IsVirtual
        { get { return this.isVirtual; } }

        public Location Location
        { get { return this.location; } }

        public JST.IdentifierScope Scope
        { get { return this.scope; } }
    }
}
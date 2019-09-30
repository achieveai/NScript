//-----------------------------------------------------------------------
// <copyright file="InlineCollectionInitializationExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST.Expressions
{
    using NScript.Utils;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for InlineCollectionInitializationExpression
    /// </summary>
    public class InlineCollectionInitializationExpression
        : Expression
    {
        public InlineCollectionInitializationExpression(
            ClrContext context,
            Location locationInfo,
            NewObjectExpression newObjectExpression,
            List<Tuple<MemberReferenceExpression, Expression[]>> setters)
            : base(context, locationInfo)
        {
        }
    }
}

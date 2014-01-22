//-----------------------------------------------------------------------
// <copyright file="DynamicExpressionVisitor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Runtime.CompilerServices;

	public abstract class DynamicExpressionVisitor : ExpressionVisitor
	{
		protected DynamicExpressionVisitor()
		{
		}

		protected extern override Expression VisitDynamic(DynamicExpression node);
	}
}

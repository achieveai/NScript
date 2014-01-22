//-----------------------------------------------------------------------
// <copyright file="ExpressionVisitor.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

	/// <summary>Represents a visitor or rewriter for expression trees.</summary>
	public abstract class ExpressionVisitor
	{
		/// <summary>Initializes a new instance of <see cref="T:System.Linq.Expressions.ExpressionVisitor" />.</summary>
		protected ExpressionVisitor()
		{
		}

		/// <summary>Dispatches the expression to one of the more specialized visit methods in this class.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		public extern virtual Expression Visit(Expression node);

		/// <summary>Dispatches the list of expressions to one of the more specialized visit methods in this class.</summary>
		/// <returns>The modified expression list, if any one of the elements were modified; otherwise, returns the original expression list.</returns>
		/// <param name="nodes">The expressions to visit.</param>
		public extern ReadOnlyCollection<Expression> Visit(ReadOnlyCollection<Expression> nodes);


		/// <summary>Visits all nodes in the collection using a specified element visitor.</summary>
		/// <returns>The modified node list, if any of the elements were modified; otherwise, returns the original node list.</returns>
		/// <param name="nodes">The nodes to visit.</param>
		/// <param name="elementVisitor">A delegate that visits a single element, optionally replacing it with a new element.</param>
		/// <typeparam name="T">The type of the nodes.</typeparam>
		public extern static ReadOnlyCollection<T> Visit<T>(ReadOnlyCollection<T> nodes, Func<T, T> elementVisitor);

		/// <summary>Visits an expression, casting the result back to the original expression type.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		/// <param name="callerName">The name of the calling method; used to report to report a better error message.</param>
		/// <typeparam name="T">The type of the expression.</typeparam>
		/// <exception cref="T:System.InvalidOperationException">The visit method for this node returned a different type.</exception>
		public extern T VisitAndConvert<T>(T node, string callerName) where T : Expression;


		/// <summary>Visits an expression, casting the result back to the original expression type.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="nodes">The expression to visit.</param>
		/// <param name="callerName">The name of the calling method; used to report to report a better error message.</param>
		/// <typeparam name="T">The type of the expression.</typeparam>
		/// <exception cref="T:System.InvalidOperationException">The visit method for this node returned a different type.</exception>
		public extern ReadOnlyCollection<T> VisitAndConvert<T>(ReadOnlyCollection<T> nodes, string callerName) where T : Expression;

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.BinaryExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitBinary(BinaryExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.BlockExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitBlock(BlockExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.ConditionalExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitConditional(ConditionalExpression node);

		/// <summary>Visits the <see cref="T:System.Linq.Expressions.ConstantExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitConstant(ConstantExpression node);

		/// <summary>Visits the <see cref="T:System.Linq.Expressions.DebugInfoExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitDebugInfo(DebugInfoExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.DynamicExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitDynamic(DynamicExpression node);

		/// <summary>Visits the <see cref="T:System.Linq.Expressions.DefaultExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitDefault(DefaultExpression node);

		/// <summary>Visits the children of the extension expression.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitExtension(Expression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.GotoExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitGoto(GotoExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.InvocationExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitInvocation(InvocationExpression node);

		/// <summary>Visits the <see cref="T:System.Linq.Expressions.LabelTarget" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected virtual LabelTarget VisitLabelTarget(LabelTarget node)
		{
			return node;
		}
		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.LabelExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitLabel(LabelExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.Expression`1" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		/// <typeparam name="T">The type of the delegate.</typeparam>
		protected extern virtual Expression VisitLambda<T>(Expression<T> node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.LoopExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitLoop(LoopExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitMember(MemberExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.IndexExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitIndex(IndexExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitMethodCall(MethodCallExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.NewArrayExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitNewArray(NewArrayExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.NewExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitNew(NewExpression node);

		/// <summary>Visits the <see cref="T:System.Linq.Expressions.ParameterExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitParameter(ParameterExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitRuntimeVariables(RuntimeVariablesExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.SwitchCase" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual SwitchCase VisitSwitchCase(SwitchCase node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.SwitchExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitSwitch(SwitchExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.CatchBlock" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual CatchBlock VisitCatchBlock(CatchBlock node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.TryExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitTry(TryExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.TypeBinaryExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitTypeBinary(TypeBinaryExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.UnaryExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitUnary(UnaryExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberInitExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitMemberInit(MemberInitExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.ListInitExpression" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected extern virtual Expression VisitListInit(ListInitExpression node);

		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.ElementInit" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected virtual ElementInit VisitElementInit(ElementInit node)
		{
			return node.Update(this.Visit(node.Arguments));
		}
		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberBinding" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected virtual MemberBinding VisitMemberBinding(MemberBinding node)
		{
			switch (node.BindingType)
			{
			case MemberBindingType.Assignment:
				return this.VisitMemberAssignment((MemberAssignment)node);
			case MemberBindingType.MemberBinding:
				return this.VisitMemberMemberBinding((MemberMemberBinding)node);
			case MemberBindingType.ListBinding:
				return this.VisitMemberListBinding((MemberListBinding)node);
			default:
				// throw Error.UnhandledBindingType(node.BindingType);
                    throw new Exception("Fix Exception");
			}
		}
		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberAssignment" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment node)
		{
			return node.Update(this.Visit(node.Expression));
		}
		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberMemberBinding" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
		{
			return node.Update(ExpressionVisitor.Visit<MemberBinding>(node.Bindings, new Func<MemberBinding, MemberBinding>(this.VisitMemberBinding)));
		}
		/// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberListBinding" />.</summary>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		/// <param name="node">The expression to visit.</param>
		protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding node)
		{
			return node.Update(ExpressionVisitor.Visit<ElementInit>(node.Initializers, new Func<ElementInit, ElementInit>(this.VisitElementInit)));
		}
	}
}

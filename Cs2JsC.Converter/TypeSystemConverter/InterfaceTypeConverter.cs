//-----------------------------------------------------------------------
// <copyright file="InterfaceTypeConverter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.JST;
    using Mono.Cecil;

    /// <summary>
    /// Definition for InterfaceTypeConverter
    /// </summary>
    public class InterfaceTypeConverter : TypeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceTypeConverter"/> class.
        /// </summary>
        /// <param name="runtimeScopeManager">The runtime scope manager.</param>
        /// <param name="typeDefinition">The type definition.</param>
        /// <param name="isSelectiveInit">if set to <c>true</c> [is selective init].</param>
        public InterfaceTypeConverter(
            RuntimeScopeManager runtimeScopeManager,
            TypeDefinition typeDefinition,
            bool isSelectiveInit)
            : base(runtimeScopeManager, typeDefinition, isSelectiveInit)
        {
        }

        /// <summary>
        /// Gets the type registration method.
        /// </summary>
        protected override MethodReference TypeRegistrationMethod
        {
            get { return this.Context.KnownReferences.RegisterIntefaceMethod; }
        }

        /// <summary>
        /// Gets the prototype initializers.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>List of prototype index and initializer expression pair.</returns>
        protected override List<Tuple<Expression, Expression>> GetPrototypeInitializers(Expression prototype)
        {
            return new List<Tuple<Expression, Expression>>();
        }

        /// <summary>
        /// Initializes the virtuals.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>
        /// List of expressions each initializing one field in prototype.
        /// </returns>
        protected override List<ExpressionStatement> InitializeVirtuals(Expression prototype)
        {
            return new List<ExpressionStatement>();
        }

        /// <summary>
        /// Initializes the static functions.
        /// </summary>
        /// <param name="statements">The statements.</param>
        protected override void InitializeStaticFunctions(List<Statement> statements)
        {
        }

        /// <summary>
        /// Initializes the static variables.
        /// </summary>
        /// <param name="statements">The statements.</param>
        protected override void InitializeStaticVariables(List<Statement> statements)
        {
        }

        /// <summary>
        /// Registers the type internal.
        /// </summary>
        /// <param name="typeNameExpression">The type name expression.</param>
        /// <param name="parentTypeExpression">The parent type expression.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns>
        /// Type registration expression.
        /// </returns>
        protected override IList<Expression> RegisterTypeMethodArguments(
            Expression typeNameExpression,
            Expression parentTypeExpression,
            List<Expression> interfaces)
        {
            return new Expression[] { typeNameExpression };
        }
    }
}

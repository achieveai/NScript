//-----------------------------------------------------------------------
// <copyright file="IntiObjectSimplifier.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST.Processors
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.CLR.AST;

    /// <summary>
    /// Definition for IntiObjectSimplifier
    /// </summary>
    internal class IntiObjectSimplifier
    {
        /// <summary>
        /// Processes the InitObjectWithDefaultValue.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="defaultValueInitObj">The default value init obj.</param>
        /// <returns>Converts InitObjectWithDefaultValue to binary expression with defaultValueExpression.</returns>
        public static Expression Process(
            IAstProcessor processor,
            InitObjectWithDefaultValue defaultValueInitObj)
        {
            return new BinaryExpression(
                defaultValueInitObj.Context,
                defaultValueInitObj.Location,
                defaultValueInitObj.ObjectReference.NestedExpression,
                new DefaultValueExpression(
                    defaultValueInitObj.Context,
                    defaultValueInitObj.Location,
                    defaultValueInitObj.TypeReference),
                BinaryOperator.Assignment);
        }
    }
}

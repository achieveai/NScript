//-----------------------------------------------------------------------
// <copyright file="NumberValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.StaticValues
{
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for NumberValue
    /// </summary>
    public class NumberValue : StaticValue
    {
        private double value;

        public NumberValue(string value, bool isInt)
        {
            if (isInt)
            {
                int i;
                if (!int.TryParse(value, out i))
                {
                    throw new ApplicationException(
                        string.Format("Can't convert {0} to int", value));
                }

                this.value = i;
            }
            else
            {
                if (!double.TryParse(value, out this.value))
                {
                    throw new ApplicationException(
                        string.Format("Can't convert {0} to double", value));
                }
            }
        }

        /// <summary>
        /// Gets initialization expression.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="codeGenerator"> The code generator. </param>
        /// <returns>
        /// The initialization expression.
        /// </returns>
        public override Expression GetInitializationExpression(
            SkinCodeGenerator codeGenerator)
        {
            return new DoubleLiteralExpression(
                codeGenerator.Scope,
                this.value);
        }
    }
}

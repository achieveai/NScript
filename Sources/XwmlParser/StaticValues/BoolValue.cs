//-----------------------------------------------------------------------
// <copyright file="BoolValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.StaticValues
{
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for BoolValue
    /// </summary>
    public class BoolValue : StaticValue
    {
        private bool value;

        public BoolValue(string value)
        {
            if (!bool.TryParse(value, out this.value))
            {
                throw new ApplicationException(
                    string.Format("Can't parse {0} to boolean", value));
            }
        }

        /// <summary>
        /// Gets initialization expression.
        /// </summary>
        /// <param name="codeGenerator"> The code generator. </param>
        /// <returns>
        /// The initialization expression.
        /// </returns>
        public override Expression GetInitializationExpression(
            SkinCodeGenerator codeGenerator)
        {
            return new BooleanLiteralExpression(
                codeGenerator.Scope,
                this.value);
        }
    }
}

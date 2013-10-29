//-----------------------------------------------------------------------
// <copyright file="CssNameValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.StaticValues
{
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CssNameValue
    /// </summary>
    public class CssNameValue : StaticValue
    {
        private IIdentifier cssClassName;

        public CssNameValue(
            DocumentContext documentContext,
            string cssClassName)
        {
            if (!documentContext.TryGetCssClassIdentifier(cssClassName, out this.cssClassName))
            {
                throw new ApplicationException(
                    string.Format("CSS class name '{0}' not found.", cssClassName));
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
            return new IdentifierStringExpression(
                null,
                codeGenerator.Scope,
                new IdentifierExpression(
                    this.cssClassName,
                    codeGenerator.Scope));
        }
    }
}

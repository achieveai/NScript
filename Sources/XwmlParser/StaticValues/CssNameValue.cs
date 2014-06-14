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
        private IIdentifier[] cssClassNames;

        public CssNameValue(
            DocumentContext documentContext,
            string cssClassName)
        {
            var cssClassNames = cssClassName.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            this.cssClassNames = new IIdentifier[cssClassNames.Length];
            for (int iCssClassName = 0; iCssClassName < cssClassNames.Length; iCssClassName++)
            {
                IIdentifier classIdentifier;
                if (!documentContext.TryGetCssClassIdentifier(cssClassNames[iCssClassName], out classIdentifier))
                {
                    throw new ApplicationException(
                        string.Format("CSS class name '{0}' not found.", cssClassName));
                }

                this.cssClassNames[iCssClassName] = classIdentifier;
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
            IdentifierStringExpression rv = null;
            for (int iClass = 0; iClass < this.cssClassNames.Length; iClass++)
            {
                if (iClass == 0)
                {
                    rv = new IdentifierStringExpression(
                            null,
                            codeGenerator.Scope,
                            new IdentifierExpression(
                                this.cssClassNames[iClass],
                                codeGenerator.Scope));
                }
                else
                {
                    rv.Append(new StringLiteralExpression(codeGenerator.Scope, " "));
                    rv.Append(
                        new IdentifierExpression(
                                this.cssClassNames[iClass],
                                codeGenerator.Scope));
                }
            }

            return rv;
        }
    }
}

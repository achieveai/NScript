//-----------------------------------------------------------------------
// <copyright file="EnumValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.StaticValues
{
    using Mono.Cecil;
    using NScript.CLR;
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for EnumValue
    /// </summary>
    public class EnumValue : StaticValue
    {
        private int? intValue;

        public EnumValue(
            TypeReference typeReference,
            ParserContext parserContext,
            string value)
        {
            var typeDef = typeReference.Resolve();
            foreach (var enumField in typeDef.GetEnumValues())
            {
                if (enumField.Name == value)
                {
                    intValue = int.Parse(enumField.Constant.ToString());
                    break;
                }
            }

            if (!intValue.HasValue)
            {
                throw new ApplicationException(
                    string.Format(
                        "Can't parse {0} to type {1}",
                        value,
                        typeDef));
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
            return new NumberLiteralExpression(
                codeGenerator.Scope,
                this.intValue.Value);
        }
    }
}

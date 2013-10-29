//-----------------------------------------------------------------------
// <copyright file="StringValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.StaticValues
{
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for StringValue
    /// </summary>
    public class StringValue : StaticValue
    {
        private string value;

        public StringValue(string value)
        {
            // TODO: Complete member initialization
            this.value = value;
        }

        public override Expression GetInitializationExpression(
            SkinCodeGenerator codeGenerator)
        {
            return new StringLiteralExpression(
                codeGenerator.Scope,
                this.value);
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="StaticValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.StaticValues
{
    using NScript.JST;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for StaticValue
    /// </summary>
    public abstract class StaticValue
    {
        public abstract Expression GetInitializationExpression(
            SkinCodeGenerator codeGenerator);
    }
}

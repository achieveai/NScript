//-----------------------------------------------------------------------
// <copyright file="CssFunctionPropertyValue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace CssParser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for CssFunctionPropertyValue
    /// </summary>
    public class CssFunctionPropertyValue : CssPropertyValue
    {
        public CssFunctionPropertyValue(
            string functionName,
            List<CssPropertyValue> args)
        {
            this.FunctionName = functionName;
            this.Args = args;
        }

        public List<CssPropertyValue> Args { get; private set; }

        public string FunctionName { get; private set; }
    }
}

//-----------------------------------------------------------------------
// <copyright file="JsConsole.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.Test.JsExecutionTests
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for JsConsole
    /// </summary>
    public class JsConsole
    {
        private List<string> logLines = new List<string>();

        public void log(string str)
        {
            logLines.Add(str);
        }

        public void info(string str)
        {
            logLines.Add(str);
        }

        public List<string> LogLines
        {
            get { return this.logLines; }
        }
    }
}

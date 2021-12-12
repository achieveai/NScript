//-----------------------------------------------------------------------
// <copyright file="JsConsole.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.Test.JsExecutionTests
{
    using Microsoft.ClearScript;
    using System;
    using System.Collections.Generic;
    using System.Threading;

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

        public void setTimeout(ScriptObject func, int delay) {
            var timer = new Timer(_ => func.Invoke(false));
            timer.Change(delay, Timeout.Infinite);
        }
    }
}

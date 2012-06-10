//-----------------------------------------------------------------------
// <copyright file="ScriptAttribute.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Definition for ScriptAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = true, AllowMultiple = false), NonScriptable, Imported]
    public class ScriptAttribute : Attribute
    {
        private readonly string script;

        public ScriptAttribute(string script)
        {
            this.script = script;
        }

        public string Script
        {
            get { return this.script; }
        }
    }
}

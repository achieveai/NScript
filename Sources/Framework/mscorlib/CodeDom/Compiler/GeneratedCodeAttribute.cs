namespace System.CodeDom.Compiler
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.All, Inherited=false, AllowMultiple=false), Extended, EditorBrowsable(EditorBrowsableState.Never), NonScriptable]
    public sealed class GeneratedCodeAttribute : Attribute
    {
        private string _tool;
        private string _version;

        public GeneratedCodeAttribute(string tool, string version)
        {
            this._tool = tool;
            this._version = version;
        }

        public string Tool
        {
            get
            {
                return this._tool;
            }
        }

        public string Version
        {
            get
            {
                return this._version;
            }
        }
    }
}


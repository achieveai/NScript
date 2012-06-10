namespace System.Runtime.CompilerServices
{
    using System;

    [Imported, AttributeUsage(AttributeTargets.Assembly, Inherited=false, AllowMultiple=false), NonScriptable]
    public sealed class ScriptQualifierAttribute : Attribute
    {
        private string _prefix;

        public ScriptQualifierAttribute(string prefix)
        {
            this._prefix = prefix;
        }

        public string Prefix
        {
            get
            {
                return this._prefix;
            }
        }
    }
}


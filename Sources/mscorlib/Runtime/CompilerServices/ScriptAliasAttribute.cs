namespace System.Runtime.CompilerServices
{
    using System;

    [Extended, AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, Inherited=true, AllowMultiple=false), NonScriptable]
    public sealed class ScriptAliasAttribute : Attribute
    {
        private string _alias;

        public ScriptAliasAttribute(string alias)
        {
            this._alias = alias;
        }

        public string Alias
        {
            get
            {
                return this._alias;
            }
        }
    }
}


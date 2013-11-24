namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Attribute for script alias.
    /// This will generate name in global namespace. If you need to specify just then name, use ScriptName.
    /// </summary>
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


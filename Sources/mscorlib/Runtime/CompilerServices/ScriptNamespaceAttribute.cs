namespace System.Runtime.CompilerServices
{
    using System;

    [NonScriptable, AttributeUsage(AttributeTargets.Type | AttributeTargets.Assembly, AllowMultiple=false), Imported]
    public sealed class ScriptNamespaceAttribute : Attribute
    {
        private string _name;

        public ScriptNamespaceAttribute(string name)
        {
            this._name = name;
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }
    }
}


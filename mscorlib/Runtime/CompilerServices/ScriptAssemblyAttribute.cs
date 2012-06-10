namespace System.Runtime.CompilerServices
{
    using System;

    [Imported, AttributeUsage(AttributeTargets.Assembly, Inherited=false, AllowMultiple=false), NonScriptable]
    public sealed class ScriptAssemblyAttribute : Attribute
    {
        private string _name;

        public ScriptAssemblyAttribute(string name)
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


namespace System.Runtime.CompilerServices
{
    using System;

    [NonScriptable, Extended, AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Struct | AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class ScriptNameAttribute : Attribute
    {
        private string _name;

        public ScriptNameAttribute(string name)
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


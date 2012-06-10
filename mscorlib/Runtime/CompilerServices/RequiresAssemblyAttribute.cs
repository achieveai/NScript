namespace System.Runtime.CompilerServices
{
    using System;

    [Imported, AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, Inherited=true, AllowMultiple=false), NonScriptable]
    public sealed class RequiresAssemblyAttribute : Attribute
    {
        private string _scriptAssemblyName;

        public RequiresAssemblyAttribute(string scriptAssemblyName)
        {
            this._scriptAssemblyName = scriptAssemblyName;
        }

        public string ScriptAssemblyName
        {
            get
            {
                return this._scriptAssemblyName;
            }
        }
    }
}


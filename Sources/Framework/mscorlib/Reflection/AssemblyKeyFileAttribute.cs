namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=false), Extended, NonScriptable]
    public sealed class AssemblyKeyFileAttribute : Attribute
    {
        private string _keyFile;

        public AssemblyKeyFileAttribute(string keyFile)
        {
            this._keyFile = keyFile;
        }

        public string KeyFile
        {
            get
            {
                return this._keyFile;
            }
        }
    }
}


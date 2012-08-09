namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [NonScriptable, Imported, AttributeUsage(AttributeTargets.Assembly, Inherited=false)]
    public sealed class AssemblyVersionAttribute : Attribute
    {
        private string _version;

        public AssemblyVersionAttribute(string version)
        {
            this._version = version;
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


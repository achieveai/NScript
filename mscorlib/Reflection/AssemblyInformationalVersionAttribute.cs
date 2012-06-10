namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=false), Imported, NonScriptable]
    public sealed class AssemblyInformationalVersionAttribute : Attribute
    {
        private string _informationalVersion;

        public AssemblyInformationalVersionAttribute(string informationalVersion)
        {
            this._informationalVersion = informationalVersion;
        }

        public string InformationalVersion
        {
            get
            {
                return this._informationalVersion;
            }
        }
    }
}


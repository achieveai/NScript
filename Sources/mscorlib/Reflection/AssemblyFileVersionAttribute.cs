namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [Extended, AttributeUsage(AttributeTargets.Assembly, Inherited=false), NonScriptable]
    public sealed class AssemblyFileVersionAttribute : Attribute
    {
        private string _version;

        public AssemblyFileVersionAttribute(string version)
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


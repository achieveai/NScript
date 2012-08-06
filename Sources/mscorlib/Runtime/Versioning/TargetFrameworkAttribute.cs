namespace System.Runtime.Versioning
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=false, AllowMultiple=false), Imported, NonScriptable]
    public sealed class TargetFrameworkAttribute : Attribute
    {
        private string _frameworkDisplayName;
        private string _frameworkName;

        public TargetFrameworkAttribute(string frameworkName)
        {
            this._frameworkName = frameworkName;
        }

        public string FrameworkDisplayName
        {
            get
            {
                return this._frameworkDisplayName;
            }
            set
            {
                this._frameworkDisplayName = value;
            }
        }

        public string FrameworkName
        {
            get
            {
                return this._frameworkName;
            }
        }
    }
}


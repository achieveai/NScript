namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=false), NonScriptable, Imported]
    public sealed class AssemblyConfigurationAttribute : Attribute
    {
        private string _configuration;

        public AssemblyConfigurationAttribute(string configuration)
        {
            this._configuration = configuration;
        }

        public string Configuration
        {
            get
            {
                return this._configuration;
            }
        }
    }
}


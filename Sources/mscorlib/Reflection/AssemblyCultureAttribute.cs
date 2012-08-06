namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, AttributeUsage(AttributeTargets.Assembly, Inherited=false), NonScriptable]
    public sealed class AssemblyCultureAttribute : Attribute
    {
        private string _culture;

        public AssemblyCultureAttribute(string culture)
        {
            this._culture = culture;
        }

        public string Culture
        {
            get
            {
                return this._culture;
            }
        }
    }
}


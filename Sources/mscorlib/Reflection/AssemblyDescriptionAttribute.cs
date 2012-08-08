namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [NonScriptable, Imported, AttributeUsage(AttributeTargets.Assembly, Inherited=false)]
    public sealed class AssemblyDescriptionAttribute : Attribute
    {
        private string _description;

        public AssemblyDescriptionAttribute(string description)
        {
            this._description = description;
        }

        public string Description
        {
            get
            {
                return this._description;
            }
        }
    }
}


namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [NonScriptable, Extended, AttributeUsage(AttributeTargets.Assembly, Inherited=false)]
    public sealed class AssemblyCompanyAttribute : Attribute
    {
        private string _company;

        public AssemblyCompanyAttribute(string company)
        {
            this._company = company;
        }

        public string Company
        {
            get
            {
                return this._company;
            }
        }
    }
}


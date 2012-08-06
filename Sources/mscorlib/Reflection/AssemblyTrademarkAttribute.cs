namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=false), Imported, NonScriptable]
    public sealed class AssemblyTrademarkAttribute : Attribute
    {
        private string _trademark;

        public AssemblyTrademarkAttribute(string trademark)
        {
            this._trademark = trademark;
        }

        public string Trademark
        {
            get
            {
                return this._trademark;
            }
        }
    }
}


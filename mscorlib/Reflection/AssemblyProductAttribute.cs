namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, AttributeUsage(AttributeTargets.Assembly, Inherited=false), NonScriptable]
    public sealed class AssemblyProductAttribute : Attribute
    {
        private string _product;

        public AssemblyProductAttribute(string product)
        {
            this._product = product;
        }

        public string Product
        {
            get
            {
                return this._product;
            }
        }
    }
}


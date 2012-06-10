namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [Imported, AttributeUsage(AttributeTargets.Assembly, Inherited=false), NonScriptable]
    public sealed class AssemblyTitleAttribute : Attribute
    {
        private string _title;

        public AssemblyTitleAttribute(string title)
        {
            this._title = title;
        }

        public string Title
        {
            get
            {
                return this._title;
            }
        }
    }
}


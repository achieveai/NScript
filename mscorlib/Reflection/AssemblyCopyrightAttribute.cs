namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [NonScriptable, Imported, AttributeUsage(AttributeTargets.Assembly, Inherited=false)]
    public sealed class AssemblyCopyrightAttribute : Attribute
    {
        private string _copyright;

        public AssemblyCopyrightAttribute(string copyright)
        {
            this._copyright = copyright;
        }

        public string Copyright
        {
            get
            {
                return this._copyright;
            }
        }
    }
}


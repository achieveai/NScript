namespace System
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [EditorBrowsable(EditorBrowsableState.Never), AttributeUsage(AttributeTargets.All, Inherited=true, AllowMultiple=false), NonScriptable, Extended]
    public sealed class CLSCompliantAttribute : Attribute
    {
        private bool _isCompliant;

        public CLSCompliantAttribute(bool isCompliant)
        {
            this._isCompliant = isCompliant;
        }

        public bool IsCompliant
        {
            get
            {
                return this._isCompliant;
            }
        }
    }
}


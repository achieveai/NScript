namespace System
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [EditorBrowsable(EditorBrowsableState.Never), NonScriptable, Imported, AttributeUsage(AttributeTargets.Class, Inherited=true)]
    public sealed class AttributeUsageAttribute : Attribute
    {
        private bool _allowMultiple;
        private AttributeTargets _attributeTarget = AttributeTargets.All;
        private bool _inherited;

        public AttributeUsageAttribute(AttributeTargets validOn)
        {
            this._attributeTarget = validOn;
            this._inherited = true;
        }

        public bool AllowMultiple
        {
            get
            {
                return this._allowMultiple;
            }
            set
            {
                this._allowMultiple = value;
            }
        }

        public bool Inherited
        {
            get
            {
                return this._inherited;
            }
            set
            {
                this._inherited = value;
            }
        }

        public AttributeTargets ValidOn
        {
            get
            {
                return this._attributeTarget;
            }
        }
    }
}


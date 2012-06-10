namespace System
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class ValueType
    {
        private object boxedValue;

        protected ValueType()
        {
        }
    }
}


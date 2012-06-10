namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    [NonScriptable, EditorBrowsable(EditorBrowsableState.Never), AttributeUsage(AttributeTargets.Type | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor), Imported]
    public sealed class EditorBrowsableAttribute : Attribute
    {
        private EditorBrowsableState _browsableState;

        public EditorBrowsableAttribute(EditorBrowsableState state)
        {
            this._browsableState = state;
        }

        public EditorBrowsableState State
        {
            get
            {
                return this._browsableState;
            }
        }
    }
}


namespace System
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Type | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Constructor, Inherited=false), Extended, EditorBrowsable(EditorBrowsableState.Never), NonScriptable]
    public sealed class ObsoleteAttribute : Attribute
    {
        private bool _error;
        private string _message;

        public ObsoleteAttribute()
        {
        }

        public ObsoleteAttribute(string message)
        {
            this._message = message;
        }

        public ObsoleteAttribute(string message, bool error)
        {
            this._message = message;
            this._error = error;
        }

        public bool IsError
        {
            get
            {
                return this._error;
            }
        }

        public string Message
        {
            get
            {
                return this._message;
            }
        }
    }
}


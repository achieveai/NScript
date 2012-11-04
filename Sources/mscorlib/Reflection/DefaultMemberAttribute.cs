namespace System.Reflection
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [NonScriptable, EditorBrowsable(EditorBrowsableState.Never), Extended]
    public sealed class DefaultMemberAttribute
    {
        private string _memberName;

        public DefaultMemberAttribute(string memberName)
        {
            this._memberName = memberName;
        }

        public string MemberName
        {
            get
            {
                return this._memberName;
            }
        }
    }
}


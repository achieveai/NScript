namespace System.Diagnostics.CodeAnalysis
{
    using System;
    using System.Runtime.CompilerServices;

    [Extended, AttributeUsage(AttributeTargets.All, Inherited=false, AllowMultiple=true), NonScriptable]
    public sealed class SuppressMessageAttribute : Attribute
    {
        private string _category;
        private string _checkId;
        private string _justification;
        private string _messageId;
        private string _scope;
        private string _target;

        public SuppressMessageAttribute(string category, string checkId)
        {
            this._category = category;
            this._checkId = checkId;
        }

        public string Category
        {
            get
            {
                return this._category;
            }
        }

        public string CheckId
        {
            get
            {
                return this._checkId;
            }
        }

        public string Justification
        {
            get
            {
                return this._justification;
            }
            set
            {
                this._justification = value;
            }
        }

        public string MessageId
        {
            get
            {
                return this._messageId;
            }
            set
            {
                this._messageId = value;
            }
        }

        public string Scope
        {
            get
            {
                return this._scope;
            }
            set
            {
                this._scope = value;
            }
        }

        public string Target
        {
            get
            {
                return this._target;
            }
            set
            {
                this._target = value;
            }
        }
    }
}


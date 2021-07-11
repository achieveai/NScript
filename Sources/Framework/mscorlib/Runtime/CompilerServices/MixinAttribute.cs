namespace System.Runtime.CompilerServices
{
    using System;

    [Extended, NonScriptable, AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class MixinAttribute : Attribute
    {
        private string _expression;

        public MixinAttribute(string expression)
        {
            this._expression = expression;
        }

        public string Expression
        {
            get
            {
                return this._expression;
            }
        }
    }
}


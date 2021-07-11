namespace System.Diagnostics
{
    using System;
    using System.Runtime.CompilerServices;

    [NonScriptable, AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=true), Extended]
    public sealed class ConditionalAttribute : Attribute
    {
        private string _conditionString;

        public ConditionalAttribute(string conditionString)
        {
            this._conditionString = conditionString;
        }

        public string ConditionString
        {
            get
            {
                return this._conditionString;
            }
        }
    }
}


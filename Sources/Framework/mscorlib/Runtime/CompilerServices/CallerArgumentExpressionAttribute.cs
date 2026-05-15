namespace System.Runtime.CompilerServices
{
    using System;
    using System.ComponentModel;

    [Extended, EditorBrowsable(EditorBrowsableState.Never), NonScriptable, AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            this.ParameterName = parameterName;
        }

        public string ParameterName { get; }
    }
}

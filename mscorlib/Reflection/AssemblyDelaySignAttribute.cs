namespace System.Reflection
{
    using System;
    using System.Runtime.CompilerServices;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=false), Imported, NonScriptable]
    public sealed class AssemblyDelaySignAttribute : Attribute
    {
        private bool _delaySign;

        public AssemblyDelaySignAttribute(bool delaySign)
        {
            this._delaySign = delaySign;
        }

        public bool DelaySign
        {
            get
            {
                return this._delaySign;
            }
        }
    }
}


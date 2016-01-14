namespace System
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [ScriptName("Function"), IgnoreNamespace]
    public abstract class MulticastDelegate : Delegate
    {
        protected extern MulticastDelegate(object target, string method);

        protected extern MulticastDelegate(Type target, string method);
    }
}


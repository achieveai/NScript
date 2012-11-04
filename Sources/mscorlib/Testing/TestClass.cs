namespace System.Testing
{
    using System;
    using System.Runtime.CompilerServices;

    [IgnoreNamespace, Extended]
    public abstract class TestClass
    {
        protected TestClass()
        {
        }

        public virtual void Cleanup()
        {
        }

        public virtual void Setup()
        {
        }
    }
}


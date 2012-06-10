using System;

namespace RealScript
{
    interface BaseInterface
    {
        void Bar();
    }

    class SimplImpl : BaseInterface, Foo
    {
        public virtual void Bar() { }

        #region Foo Members

        int Foo.Foo()
        {
            return 0;
        }

        #endregion
    }

    class SimplImpl2 : SimplImpl
    {
        public override void Bar() { }
    }


    abstract class BaseClass : BaseInterface
    {
        public virtual void Foo()
        {
        }

        public abstract void Bar();
    }

    class TestInheritence : BaseClass
    {
        public override void Foo()
        {
            base.Foo();
        }

        public override void Bar()
        {
        }
    }

    class TestInheritence2 : BaseClass
    {
        public new void Foo()
        {
        }

        public override void Bar()
        { }
    }

    class FooGeneric<T>
    {
        public virtual T Foo<U>(U arg)
        {
            return default(T);
        }
    }
}

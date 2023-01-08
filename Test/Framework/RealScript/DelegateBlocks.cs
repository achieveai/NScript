//-----------------------------------------------------------------------
// <copyright file="DelegateBlocks.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    public delegate int IntFunction(int i);

    /// <summary>
    /// Definition for DelegateBlocks
    /// </summary>
    public class DelegateBlocks
    {
        private Class1 class1;
        private int intVariable;

        public event IntFunction Evt;

        public event IntFunction Evt2
        {
            add
            {
                this.Evt += value;
            }

            remove
            {
                this.Evt -= value;
            }
        }

        public DelegateBlocks(int i)
        {
            this.intVariable = i;
            class1 = new Class1();
        }

        public int IntVariable
        {
            get { return this.intVariable; }
            set { this.intVariable = value; }
        }

        public IntFunction StaticReferencingDelegate()
        {
            return delegate(int i) { return Class1.GetMoreStatic(i + 10); };
        }

        public IntFunction InstanceReferencingDelegate()
        {
            return delegate(int i) { return i + this.intVariable; };
        }

        public IntFunction LocalReferencingDelegate(int j)
        {
            return delegate(int i) { return i + j; };
        }

        public IntFunction LocalAndInstanceReferencingDelegate(int j)
        {
            return delegate(int i) { return i + j + this.intVariable; };
        }

        public int StaticReferencingDelegateM()
        {
            return this.IntDelegateTaker(
                "Foo",
                delegate(int i) { return Class1.GetMoreStatic(i + 10); });
        }

        public int InstanceReferencingDelegateM()
        {
            this.class1.Print(0, 1);
            return this.IntDelegateTaker(
                    "Foo",
                    delegate(int i) { return i + this.intVariable; });
        }

        public int LocalReferencingDelegateM(int j)
        {
            j = this.class1.GetMore(j);
            int k = this.class1.GetMore(j);
            return this.IntDelegateTaker("Foo", delegate(int i) { return i + j + k; });
        }

        public int LocalAndInstanceReferencingDelegateM(int j)
        {
            j = this.class1.GetMore(j);
            int k = this.class1.GetMore(j);
            return this.IntDelegateTaker("Foo", delegate(int i) { return i + j + this.intVariable + k; });
        }

        public Func<int, int> InstanceDelegate()
        {
            return this.LocalAndInstanceReferencingDelegateM;
        }

        public int IntDelegateTaker(string str, IntFunction func)
        {
            return func(str.Length);
        }

        public static void AddEvent(DelegateBlocks db)
        {
            db.Evt += (i) => i + 10;
        }

        public static void RemoveEvent(DelegateBlocks db)
        {
            db.Evt -= (i) => i + 10;
        }

        public static int CallEvent(DelegateBlocks db)
        {
            if (db.Evt != null)
            { return db.Evt(10); }

            return -1;
        }

        public static void ClearEvent(DelegateBlocks db)
        {
            if (db.Evt != null)
            { db.Evt = null; }
        }
    }
}

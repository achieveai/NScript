//-----------------------------------------------------------------------
// <copyright file="StructClass.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for StructClass
    /// </summary>
    public struct StructClass : Foo
    {
        int i;
        Foo foo;

        public StructClass(int i, Foo foo)
        {
            this.i = i;
            this.foo = foo;
        }

        [Script(@"
            this.@{[RealScript]RealScript.StructClass::i} = i + j;
            this.@{[RealScript]RealScript.StructClass::foo} = foo;
            return this;")]
        public extern StructClass(int i, int j, Foo foo);

        public int Value
        {
            get { return this.i; }
            set { this.i = value; }
        }

        public Foo FooValue
        {
            get { return this.foo; }
            set { this.foo = value; }
        }

        public int Simple0ArgObjectAccessMethod()
        {
            return this.foo.Foo();
        }

        public int GetInt()
        {
            return this.i;
        }

        public int Foo()
        {
            return 1;
        }

        public void FooVoid()
        {
        }

        public string CallStructOverride(StructClass sc)
        {
            return sc.ToString();
        }

        public Action GetFunc()
        {
            return this.FooVoid;
        }

        public int GetCalculatedInt(Foo foo)
        {
            return this.i + foo.Foo();
        }

        public override string ToString()
        {
            return string.Format(
                "{0}, {1}",
                this.i,
                this.foo);
        }
    }

    public struct StructClass2 : Foo
    {
        int i;
        Foo foo;

        [Script(@"
            this.@{[RealScript]RealScript.StructClass2::i} = i + j;
            this.@{[RealScript]RealScript.StructClass2::foo} = foo;
            return this;
        ")]
        public extern StructClass2(int i, int j, Foo foo);

        public int Value
        {
            get { return this.i; }
            set { this.i = value; }
        }

        public Foo FooValue
        {
            get { return this.foo; }
            set { this.foo = value; }
        }

        [Script("return this.@{[RealScript]RealScript.StructClass::i} + 10;")]
        public extern int GetScriptedInt();

        public int Foo()
        {
            return 1;
        }

        public static Foo GetFooValue(StructClass2 st2, StructClass st1)
        {
            if (st2.Value < 0)
            {
                return st1.FooValue;
            }

            return st2.FooValue;
        }
    }
}
//-----------------------------------------------------------------------
// <copyright file="ContainerTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using System;
    using SunlightUnit;

    class IocTestType1 : IocTestType1Base, IIocTestType1
    {
        int y;
        public IocTestType1(
            int x, int y)
            : base(x)
        {
            this.y = y;
        }

        public int TestMethod()
        {
            return this.y + base.TestMethodBase();
        }
    }

    class IocTestType2
    {
        int x;
        public IocTestType2(int x)
        {
            this.x = x;
        }

        public int TestMethod()
        {
            return x;
        }
    }

    class IocTestType1Base
    {
        int x;
        public IocTestType1Base(int x)
        {
            this.x = x;
        }

        public int TestMethodBase()
        {
            return this.x;
        }
    }

    interface IIocTestType1
    {
        int TestMethod();
    }

    /// <summary>
    /// Definition for ContainerTests
    /// </summary>
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public static void TestRegisterResolve()
        {
            IocContainer container = new IocContainer();

            int x = 1;
            int y = 2;

            container.Register<IocTestType2>(delegate { return new IocTestType2(x); });
            container.Register<IocTestType1>(delegate { return new IocTestType1(x, y); });

            var t2 = container.Resolve<IocTestType2>();

            Assert.IsTrue(t2 != null, "t2 != null");
            Assert.Equal(1, t2.TestMethod(), "1 == t1.TestMethod()");

            var t1 = container.Resolve<IocTestType1>();

            Assert.IsTrue(t1 != null, "t1 != null");
            Assert.Equal(3, t1.TestMethod(), "3 == t1.TestMethod()");

            x = 10;
            t1 = container.Resolve<IocTestType1>();
            Assert.Equal(12, t1.TestMethod(), "12 == t1.TestMethod()");
        }

        [Test]
        public static void TestRegisterResolveWithAs()
        {
            IocContainer container = new IocContainer();

            int x = 1;
            int y = 2;

            container.Register<IocTestType2>(delegate { return new IocTestType2(x); });
            container.Register<IocTestType1>(delegate { return new IocTestType1(x, y); })
                .As<IIocTestType1>();

            var t2 = container.Resolve<IocTestType2>();

            Assert.IsTrue(t2 != null, "t2 != null");
            Assert.Equal(1, t2.TestMethod(), "1 == t1.TestMethod()");

            var t1 = container.Resolve<IIocTestType1>();

            Assert.IsTrue(t1 != null, "t1 != null");
            Assert.Equal(3, t1.TestMethod(), "3 == t1.TestMethod()");
        }

        [Test]
        public static void TestRegisterResolveIsSingleton()
        {
            IocContainer container = new IocContainer();

            int x = 1;
            int y = 2;

            container.Register<IocTestType2>(delegate { return new IocTestType2(x); });
            container.Register<IocTestType1>(delegate { return new IocTestType1(x, y); })
                .IsSingleton();

            var t2 = container.Resolve<IocTestType2>();

            Assert.IsTrue(t2 != null, "t2 != null");
            Assert.Equal(1, t2.TestMethod(), "1 == t1.TestMethod()");

            var t1 = container.Resolve<IocTestType1>();

            x = 10;
            var t1_ = container.Resolve<IocTestType1>();
            Assert.StrictEqual(t1_, t1, "t1_ === t1");
        }

        [Test]
        public static void TestRegisterResolveLazy()
        {
            IocContainer container = new IocContainer();

            int x = 1;
            int y = 2;

            container.Register<IocTestType1>(delegate { return new IocTestType1(x++, y); })
                .IsSingleton();

            var t1 = container.ResolveLazy<IocTestType1>();
            Assert.Equal(1, x, "x === 1");

            Assert.Equal(3, t1.Value.TestMethod(), "t1.Value.TestMethod() == 3");
            Assert.Equal(2, x, "x === 2");
        }
    }
}

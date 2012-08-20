//-----------------------------------------------------------------------
// <copyright file="EventBusTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Test
{
    using System;
    using SunlightUnit;

    /// <summary>
    /// Definition for EventBusTests
    /// </summary>
    [TestFixture]
    public class EventBusTests
    {
        class Evt1
        {
            public int X;
        }

        struct Evt2
        {
            public readonly int X;
            public Evt2(int x)
            {
                this.X = x;
            }
        }

        [Test]
        public static void TestSubscribeAndRaise()
        {
            EventBus evtBus = new EventBus();
            int x1 = 0;
            int x2 = 0;

            evtBus.Subscribe<Evt1>(evt => x1 = evt.X);
            evtBus.Subscribe<Evt2>(evt => x2 = evt.X);

            evtBus.Raise(new Evt1() { X = 10 });
            Assert.Equal(10, x1, "10 == x1");
            Assert.Equal(0, x2, "0 == x2");
        }

        [Test]
        public static void TestSubscribeAndRaiseOnceTime()
        {
            EventBus evtBus = new EventBus();
            int x1 = 0;
            int x2 = 0;

            Action<Evt1> del = delegate(Evt1 evt) { x1 = evt.X; };
            evtBus.Subscribe<Evt1>(del);
            evtBus.Subscribe<Evt2>(evt => x2 = evt.X);

            evtBus.RaiseOneTime(new Evt1() { X = 10 });
            Assert.Equal(10, x1, "10 == x1");

            x1 = 0;
            evtBus.Subscribe(del);
            Assert.Equal(10, x1, "(2) 10 == x1");
        }

        [Test]
        public static void TestSubscribeUnSubscribeAndRaise()
        {
            EventBus evtBus = new EventBus();
            int x1 = 0;
            int x2 = 0;

            Action<Evt1> del = delegate(Evt1 evt) { x1 = evt.X; };
            evtBus.Subscribe<Evt2>(evt => x2 = evt.X);
            evtBus.Subscribe<Evt1>(evt => x2 = evt.X);
            evtBus.UnSubscribe(del);

            evtBus.Raise(new Evt1() { X = 10 });
            Assert.Equal(0, x1, "0 == x1");
        }
    }
}

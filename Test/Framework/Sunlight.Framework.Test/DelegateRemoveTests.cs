namespace Sunlight.Framework.Test
{
    using System;
    using SunlightUnit;

    /// <summary>
    /// #155: Delegate.Remove on a null delegate, as C# defines it.
    /// </summary>
    [TestFixture]
    public class DelegateRemoveTests
    {
        private sealed class Source
        {
            public event Action Changed;

            public void Raise()
            {
                if (this.Changed != null) this.Changed();
            }

            public bool IsEmpty
            {
                get { return this.Changed == null; }
            }
        }

        private static int calls;

        private static void Handler()
        {
            DelegateRemoveTests.calls++;
        }

        [Test]
        public static void RemovingFromAnEmptyEventIsANoOp(Assert assert)
        {
            var source = new Source();
            source.Changed -= DelegateRemoveTests.Handler;
            assert.IsTrue(source.IsEmpty, "still no handlers");
        }

        [Test]
        public static void RemovingNullKeepsTheHandlers(Assert assert)
        {
            DelegateRemoveTests.calls = 0;
            var source = new Source();
            source.Changed += DelegateRemoveTests.Handler;
            Action none = null;
            source.Changed -= none;
            source.Raise();
            assert.Equal(DelegateRemoveTests.calls, 1, "the handler stayed");
        }
    }
}

namespace Sunlight.Framework.Test
{
    using SunlightUnit;

    /// <summary>
    /// #153: a call to a partial method with no implementation is gone, with its arguments.
    /// </summary>
    [TestFixture]
    public class PartialCallTests
    {
        private sealed partial class Probe
        {
            public int Count;
            public int ArgEvals;

            partial void OnMissing(int value);

            partial void OnPresent();

            private int Arg()
            {
                this.ArgEvals++;
                return 1;
            }

            public void Init()
            {
                this.Count++;
                this.OnMissing(this.Arg());
                this.OnPresent();
            }
        }

        private sealed partial class Probe
        {
            partial void OnPresent()
            {
                this.Count += 10;
            }
        }

        [Test]
        public static void UnimplementedPartialIsDroppedAndImplementedOneRuns(Assert assert)
        {
            var probe = new Probe();
            probe.Init();
            assert.Equal(probe.Count, 11, "Init ran, and so did the implemented partial");
            assert.Equal(probe.ArgEvals, 0, "the dropped call's argument was not evaluated");
        }
    }
}

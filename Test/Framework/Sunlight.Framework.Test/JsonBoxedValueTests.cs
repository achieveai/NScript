namespace Sunlight.Framework.Test
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web;
    using SunlightUnit;

    /// <summary>
    /// #154: a value boxed into an object field serializes as the value, not as its box.
    /// </summary>
    [TestFixture]
    public class JsonBoxedValueTests
    {
        private sealed class Args
        {
            [PreserveCase] public object afterSeq;
        }

        private struct Point
        {
            [PreserveCase] public int x;
        }

        private sealed class PointHolder
        {
            [PreserveCase] public Point point;
        }

        private enum Level
        {
            Low,
            High = 5,
        }

        private static string Json(object value)
        {
            var args = new Args();
            args.afterSeq = value;
            return JSON.Stringify<object>(args);
        }

        [Test]
        public static void BoxedPrimitivesSerializeAsTheirValue(Assert assert)
        {
            assert.Equal(JsonBoxedValueTests.Json(64.5), "{\"afterSeq\":64.5}", "double");
            assert.Equal(JsonBoxedValueTests.Json(3), "{\"afterSeq\":3}", "int");
            assert.Equal(JsonBoxedValueTests.Json(true), "{\"afterSeq\":true}", "bool");
            assert.Equal(JsonBoxedValueTests.Json((double?)1.5), "{\"afterSeq\":1.5}", "double?");
            assert.Equal(JsonBoxedValueTests.Json(null), "{\"afterSeq\":null}", "null");
            assert.Equal(JsonBoxedValueTests.Json("s"), "{\"afterSeq\":\"s\"}", "string");
        }

        // JSON has no NaN or Infinity. A box must not change that into something else.
        [Test]
        public static void BoxedNonFiniteDoublesSerializeAsNull(Assert assert)
        {
            double zero = 0;
            assert.Equal(JsonBoxedValueTests.Json(0 / zero), "{\"afterSeq\":null}", "NaN");
            assert.Equal(JsonBoxedValueTests.Json(1 / zero), "{\"afterSeq\":null}", "Infinity");
            assert.Equal(JsonBoxedValueTests.Json(-1 / zero), "{\"afterSeq\":null}", "-Infinity");
        }

        [Test]
        public static void NestedBoxedValueSerializesAsItsValue(Assert assert)
        {
            var inner = new Args();
            inner.afterSeq = 2.5;
            assert.Equal(JsonBoxedValueTests.Json(inner), "{\"afterSeq\":{\"afterSeq\":2.5}}", "nested");
        }

        [Test]
        public static void StructsSerializeTheirFields(Assert assert)
        {
            var point = new Point();
            point.x = 3;
            var holder = new PointHolder();
            holder.point = point;
            assert.Equal(JSON.Stringify<object>(holder), "{\"point\":{\"x\":3}}", "a struct held in a field");
            assert.Equal(JsonBoxedValueTests.Json(point), "{\"afterSeq\":{\"x\":3}}", "a boxed struct");
        }

        [Test]
        public static void BoxedEnumSerializesAsItsNumber(Assert assert)
        {
            // A variable, because a constant is boxed as a plain int.
            Level level = Level.Low;
            level = Level.High;
            assert.Equal(JsonBoxedValueTests.Json(level), "{\"afterSeq\":5}", "enum");
        }

        [Test]
        public static void UnboxingStillWorks(Assert assert)
        {
            var args = new Args();
            args.afterSeq = 64.5;
            JSON.Stringify<object>(args);
            assert.Equal((double)args.afterSeq, 64.5, "the field is still a boxed double");
        }

        [Test]
        public static void BoxedDateSerializesAsItsIsoString(Assert assert)
        {
            string json = JsonBoxedValueTests.Json(DateTime.Now);
            assert.Equal(json.IndexOf("{\"afterSeq\":\"2"), 0, "an ISO date string, not an empty object: " + json);
        }
    }
}

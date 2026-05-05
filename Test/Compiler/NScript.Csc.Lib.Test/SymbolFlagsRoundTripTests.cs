namespace NScript.Csc.Lib.Test
{
    using System.IO;
    using JsCsc.Lib.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SymbolFlagsRoundTripTests
    {
        [TestMethod]
        public void TypeSpecSer_IsRecord_RoundTrips()
        {
            var original = new TypeSpecSer
            {
                Name = "Person",
                Namespace = "Demo",
                Module = new ModuleSpecSer { Name = "Demo" },
                Arity = 0,
                IsRecord = true,
            };

            var clone = RoundTrip(original);

            Assert.AreEqual("Person", clone.Name);
            Assert.IsTrue(clone.IsRecord);
        }

        [TestMethod]
        public void TypeSpecSer_IsRecordDefaultsFalse()
        {
            var original = new TypeSpecSer
            {
                Name = "Plain",
                Namespace = "Demo",
                Module = new ModuleSpecSer { Name = "Demo" },
            };

            var clone = RoundTrip(original);

            Assert.IsFalse(clone.IsRecord);
        }

        [TestMethod]
        public void PropertySpecSer_InitOnlyAndRequired_RoundTrip()
        {
            var original = new PropertySpecSer
            {
                Setter = 42,
                Getter = 7,
                IsInitOnly = true,
                IsRequired = true,
            };

            var clone = RoundTrip(original);

            Assert.AreEqual(42, clone.Setter);
            Assert.AreEqual(7, clone.Getter);
            Assert.IsTrue(clone.IsInitOnly);
            Assert.IsTrue(clone.IsRequired);
        }

        [TestMethod]
        public void FieldSpecSer_IsRequired_RoundTrips()
        {
            var original = new FieldSpecSer
            {
                Name = "Tag",
                IsRequired = true,
            };

            var clone = RoundTrip(original);

            Assert.AreEqual("Tag", clone.Name);
            Assert.IsTrue(clone.IsRequired);
        }

        private static T RoundTrip<T>(T value)
        {
            using var ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, value);
            ms.Position = 0;
            return ProtoBuf.Serializer.Deserialize<T>(ms);
        }
    }
}

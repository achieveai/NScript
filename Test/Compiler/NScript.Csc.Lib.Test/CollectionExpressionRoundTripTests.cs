namespace NScript.Csc.Lib.Test
{
    using System.Collections.Generic;
    using System.IO;
    using JsCsc.Lib.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CollectionExpressionRoundTripTests
    {
        [TestMethod]
        public void CollectionExpressionSer_PreservesShape()
        {
            var original = new CollectionExpressionSer
            {
                ElementType = 2,
                Elements = new List<ExpressionSer>
                {
                    new IntLiteralExpression { Value = 1 },
                    new IntLiteralExpression { Value = 2 },
                    new IntLiteralExpression { Value = 3 },
                }
            };

            var clone = RoundTripAsExpression(original);

            Assert.IsInstanceOfType(clone, typeof(CollectionExpressionSer));
            var typed = (CollectionExpressionSer)clone;
            Assert.AreEqual(2, typed.ElementType);
            Assert.AreEqual(3, typed.Elements.Count);
            Assert.AreEqual(1, ((IntLiteralExpression)typed.Elements[0]).Value);
            Assert.AreEqual(2, ((IntLiteralExpression)typed.Elements[1]).Value);
            Assert.AreEqual(3, ((IntLiteralExpression)typed.Elements[2]).Value);
        }

        [TestMethod]
        public void CollectionExpressionSer_EmptyElements_RoundTrips()
        {
            var original = new CollectionExpressionSer
            {
                ElementType = 2,
                Elements = new List<ExpressionSer>()
            };

            var clone = RoundTripAsExpression(original);

            var typed = (CollectionExpressionSer)clone;
            Assert.IsNotNull(typed.Elements);
            Assert.AreEqual(0, typed.Elements.Count);
        }

        // Round-trip through the abstract base so we exercise the [ProtoInclude(227)]
        // dispatch — not just the concrete class's surrogate.
        private static ExpressionSer RoundTripAsExpression(ExpressionSer value)
        {
            using var ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, value);
            ms.Position = 0;
            return ProtoBuf.Serializer.Deserialize<ExpressionSer>(ms);
        }
    }
}

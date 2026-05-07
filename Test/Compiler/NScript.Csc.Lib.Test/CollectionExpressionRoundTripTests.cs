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
                Elements = new List<CollectionExpressionElementSer>
                {
                    new LiteralElementSer { Operand = new IntLiteralExpression { Value = 1 } },
                    new LiteralElementSer { Operand = new IntLiteralExpression { Value = 2 } },
                    new LiteralElementSer { Operand = new IntLiteralExpression { Value = 3 } },
                }
            };

            var clone = RoundTripAsExpression(original);

            Assert.IsInstanceOfType(clone, typeof(CollectionExpressionSer));
            var typed = (CollectionExpressionSer)clone;
            Assert.AreEqual(2, typed.ElementType);
            Assert.AreEqual(3, typed.Elements.Count);
            Assert.IsInstanceOfType(typed.Elements[0], typeof(LiteralElementSer));
            Assert.AreEqual(1, ((IntLiteralExpression)((LiteralElementSer)typed.Elements[0]).Operand).Value);
            Assert.AreEqual(2, ((IntLiteralExpression)((LiteralElementSer)typed.Elements[1]).Operand).Value);
            Assert.AreEqual(3, ((IntLiteralExpression)((LiteralElementSer)typed.Elements[2]).Operand).Value);
        }

        [TestMethod]
        public void CollectionExpressionSer_EmptyElements_RoundTrips()
        {
            var original = new CollectionExpressionSer
            {
                ElementType = 2,
                Elements = new List<CollectionExpressionElementSer>()
            };

            var clone = RoundTripAsExpression(original);

            var typed = (CollectionExpressionSer)clone;
            Assert.IsNotNull(typed.Elements);
            Assert.AreEqual(0, typed.Elements.Count);
        }

        // Phase F1: spread elements (..source) target-typed to T[].
        // Exercises both subtype tags (229 LiteralElementSer, 230 SpreadElementSer)
        // and verifies the discriminator dispatch via the abstract base.
        [TestMethod]
        public void CollectionExpressionSer_MixedLiteralAndSpread_RoundTrips()
        {
            var original = new CollectionExpressionSer
            {
                ElementType = 2,
                Elements = new List<CollectionExpressionElementSer>
                {
                    new LiteralElementSer { Operand = new IntLiteralExpression { Value = 7 } },
                    new SpreadElementSer { Operand = new IntLiteralExpression { Value = 11 } },
                    new LiteralElementSer { Operand = new IntLiteralExpression { Value = 13 } },
                }
            };

            var clone = RoundTripAsExpression(original);

            var typed = (CollectionExpressionSer)clone;
            Assert.AreEqual(3, typed.Elements.Count);

            Assert.IsInstanceOfType(typed.Elements[0], typeof(LiteralElementSer));
            Assert.AreEqual(7, ((IntLiteralExpression)((LiteralElementSer)typed.Elements[0]).Operand).Value);

            Assert.IsInstanceOfType(typed.Elements[1], typeof(SpreadElementSer));
            Assert.AreEqual(11, ((IntLiteralExpression)((SpreadElementSer)typed.Elements[1]).Operand).Value);

            Assert.IsInstanceOfType(typed.Elements[2], typeof(LiteralElementSer));
            Assert.AreEqual(13, ((IntLiteralExpression)((LiteralElementSer)typed.Elements[2]).Operand).Value);
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

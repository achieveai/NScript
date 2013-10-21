namespace CssParser.Test
{
    using System;
    using CssParser;
    using NUnit.Framework;

    [TestFixture]
    public class CssGrammerUnitTest
    {
        [Test]
        public void BasicTest()
        {
            CssGrammer grammer = new CssGrammer(".body { border: #ddeeff thin 1px solid; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            Assert.AreEqual(1, grammer.Rules[0].Selectors.Count);
            Assert.IsInstanceOf<CssClass>(grammer.Rules[0].Selectors[0]);

            Assert.AreEqual(1, grammer.Rules[0].Properties.Count);
            Assert.AreEqual("border", grammer.Rules[0].Properties[0].PropertyName);
            var propertyArgs = new string[] { "#ddeeff", "thin", "1px", "solid" };
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Rules[0].Properties[0].PropertyArgs[i]);
            }
        }

        [Test]
        public void PropertyOnlyTest()
        {
            CssGrammer grammer = new CssGrammer("border: #ddeeff thin 1px solid;", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(1, grammer.Properties.Count);

            Assert.AreEqual("border", grammer.Properties[0].PropertyName);

            var propertyArgs = new string[] { "#ddeeff", "thin", "1px", "solid" };
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Rules[0].Properties[0].PropertyArgs[i]);
            }
        }

        [Test]
        public void PropertiesOnlyTest()
        {
            CssGrammer grammer = new CssGrammer("border: #ddeeff thin 1px solid; background-color: #ffeedd;", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(2, grammer.Properties.Count);

            var propertyArgs = new string[] { "#ddeeff", "thin", "1px", "solid" };
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Properties[0].PropertyArgs[i]);
            }

            propertyArgs = new string[] { "#ffeedd" };
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Properties[1].PropertyArgs[i]);
            }
        }
    }
}

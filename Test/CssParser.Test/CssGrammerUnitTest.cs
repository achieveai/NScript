namespace CssParser.Test
{
    using System;
    using CssParser;
    using MbUnit.Framework;

    [TestFixture]
    public class CssGrammerUnitTest
    {
        [Test]
        public void BasicTest()
        {
            CssGrammer grammer = new CssGrammer(".body { border: #ddeeff thin 1px solid; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            Assert.AreEqual(1, grammer.Rules[0].Selectors.Count);
            Assert.IsInstanceOfType<CssClass>(grammer.Rules[0].Selectors[0]);

            Assert.AreEqual(1, grammer.Rules[0].Properties.Count);
            Assert.AreEqual("border", grammer.Rules[0].Properties[0].propertyName);
            Assert.AreElementsNotSame(
                new string[] { "#ddeeff", "thin", "1px", "solid" },
                grammer.Rules[0].Properties[0].propertyArgs);
        }
    }
}

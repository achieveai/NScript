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
            CssGrammer grammer = new CssGrammer("body { color: #ddeeff; }");
        }
    }
}

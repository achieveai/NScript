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
        }
    }
}

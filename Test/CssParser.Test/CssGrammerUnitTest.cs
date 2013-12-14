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
            CssGrammer grammer = new CssGrammer(".body { border: #ddeeff thin 123px solid; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            Assert.AreEqual(1, grammer.Rules[0].Selectors.Count);
            Assert.IsInstanceOf<CssClassName>(grammer.Rules[0].Selectors[0]);

            Assert.AreEqual(1, grammer.Rules[0].Properties.Count);
            Assert.AreEqual("border", grammer.Rules[0].Properties[0].PropertyName);
            var propertyArgs = new string[] { "#ddeeff", "thin", "123px", "solid" };
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs[0].Values.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Rules[0].Properties[0].PropertyArgs[0].Values[i].ToString());
            }
        }

        [Test]
        public void TestBasicKeyFrames()
        {
            CssGrammer grammer = new CssGrammer("@keyframes test { from { border: 1px;} to , 50% { border: 10px;} 10% {border: 20px; } }");
            Assert.AreEqual(0, grammer.Rules.Count);
            Assert.AreEqual(1, grammer.KeyFrames.Count);
            var keyFrames = grammer.KeyFrames[0];
            Assert.AreEqual("test", keyFrames.Name);
            Assert.AreEqual(3, keyFrames.Frames.Count);

            var frame = keyFrames.Frames[0];
            Assert.AreEqual(1, frame.Selectors.Count);
            Assert.AreEqual("from", frame.Selectors[0]);

            frame = keyFrames.Frames[1];
            Assert.AreEqual(2, frame.Selectors.Count);
            Assert.AreEqual("to", frame.Selectors[0]);
            Assert.AreEqual("50%", frame.Selectors[1]);

            frame = keyFrames.Frames[2];
            Assert.AreEqual(1, frame.Selectors.Count);
            Assert.AreEqual("10%", frame.Selectors[0]);
        }

        [Test]
        public void TestCssSelector()
        {
            CssGrammer grammer = new CssGrammer(".list > div { border: #ddeeff thin 1px solid; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOf<CssRuleSelector>(selectors[0]);
            CssRuleSelector ruleSelector = selectors[0] as CssRuleSelector;
            Assert.AreEqual(2, ruleSelector.Selectors.Count);
            Assert.IsInstanceOf<CssClassName>(ruleSelector.Selectors[0]);
            Assert.IsInstanceOf<CssTagName>(ruleSelector.Selectors[1]);
            Assert.AreEqual(SelectorOp.ParentOf, ruleSelector.Ops[0]);
        }

        [Test]
        public void TestCssRuleSelector3Deep()
        {
            CssGrammer grammer = new CssGrammer(".list > a > #div { border: #ddeeff thin 1px solid; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOf<CssRuleSelector>(selectors[0]);
            CssRuleSelector ruleSelector = selectors[0] as CssRuleSelector;
            Assert.AreEqual(3, ruleSelector.Selectors.Count);
            Assert.IsInstanceOf<CssClassName>(ruleSelector.Selectors[0]);
            Assert.IsInstanceOf<CssTagName>(ruleSelector.Selectors[1]);
            Assert.IsInstanceOf<CssId>(ruleSelector.Selectors[2]);
        }

        [Test]
        public void TestAttributeSelector()
        {
            CssGrammer grammer = new CssGrammer("a[t=m] > div { color: red; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOf<CssRuleSelector>(selectors[0]);
            CssRuleSelector ruleSelector = selectors[0] as CssRuleSelector;
            Assert.AreEqual(2, ruleSelector.Selectors.Count);
            Assert.IsInstanceOf<AndCssSelector>(ruleSelector.Selectors[0]);
            Assert.IsInstanceOf<CssTagName>(ruleSelector.Selectors[1]);

            AndCssSelector andCssSelector = (AndCssSelector)ruleSelector.Selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOf<CssTagName>(andCssSelector.Selectors[0]);
            Assert.IsInstanceOf<AttributeSelector>(andCssSelector.Selectors[1]);

            AttributeSelector attrSel = (AttributeSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("t", attrSel.Attribute);
            Assert.AreEqual("m", attrSel.Value);
            Assert.AreEqual(attrSel.Condition, AttributeCondition.Equal);
        }

        [Test]
        public void TestPseudoSelector()
        {
            CssGrammer grammer = new CssGrammer("a:hover { }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOf<AndCssSelector>(selectors[0]);
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOf<CssTagName>(andCssSelector.Selectors[0]);
            Assert.IsInstanceOf<PseudoSelector>(andCssSelector.Selectors[1]);

            PseudoSelector attrSel = (PseudoSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("hover", attrSel.Name);
            Assert.IsNull(attrSel.Arg);
        }

        [Test]
        public void TestPseudoFuncSelector()
        {
            CssGrammer grammer = new CssGrammer("a:first-child() { }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOf<AndCssSelector>(selectors[0]);
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOf<CssTagName>(andCssSelector.Selectors[0]);
            Assert.IsInstanceOf<PseudoSelector>(andCssSelector.Selectors[1]);

            PseudoSelector attrSel = (PseudoSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("first-child", attrSel.Name);
            Assert.AreEqual("", attrSel.Arg);
        }

        [Test]
        public void TestPseudoFuncNumSelector()
        {
            CssGrammer grammer = new CssGrammer("a:nth-child(2) { }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOf<AndCssSelector>(selectors[0]);
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOf<CssTagName>(andCssSelector.Selectors[0]);
            Assert.IsInstanceOf<PseudoSelector>(andCssSelector.Selectors[1]);

            PseudoSelector attrSel = (PseudoSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("nth-child", attrSel.Name);
            Assert.AreEqual("2", attrSel.Arg);
        }

        [Test]
        public void TestPseudoFuncArgIdentSelector()
        {
            CssGrammer grammer = new CssGrammer("a:nth-child(odd) { }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOf<AndCssSelector>(selectors[0]);
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOf<CssTagName>(andCssSelector.Selectors[0]);
            Assert.IsInstanceOf<PseudoSelector>(andCssSelector.Selectors[1]);

            PseudoSelector attrSel = (PseudoSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("nth-child", attrSel.Name);
            Assert.AreEqual("odd", attrSel.Arg);
        }

        [Test]
        public void TestPseudoFuncArgMultSelector()
        {
            CssGrammer grammer = new CssGrammer("a:nth-child(2n + 10) { }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOf<AndCssSelector>(selectors[0]);
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOf<CssTagName>(andCssSelector.Selectors[0]);
            Assert.IsInstanceOf<PseudoSelector>(andCssSelector.Selectors[1]);

            PseudoSelector attrSel = (PseudoSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("nth-child", attrSel.Name);
            Assert.AreEqual("2n+10", attrSel.Arg);
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
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs[0].Values.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Rules[0].Properties[0].PropertyArgs[0].Values[i]);
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
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs[0].Values.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Properties[0].PropertyArgs[0].Values[i]);
            }

            propertyArgs = new string[] { "#ffeedd" };
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs[0].Values.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Properties[1].PropertyArgs[0].Values[i]);
            }
        }

        [Test]
        public void MultiplePropertyValuesTest()
        {
            CssGrammer grammer = new CssGrammer("font: courier, 'New Courier', rockwell;", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(1, grammer.Properties.Count);

            var propertyArgs = new string[] { "courier", "'New Courier'", "rockwell" };
            Assert.AreEqual(propertyArgs.Length, grammer.Properties[0].PropertyArgs.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Properties[0].PropertyArgs[i].Values[0].ToString());
            }
        }

        [Test]
        public void FunctionPropertyValueTest()
        {
            CssGrammer grammer = new CssGrammer("color: rgba(10,20,40,.5);", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(1, grammer.Properties.Count);

            var property = grammer.Properties[0];
            Assert.AreEqual("color", property.PropertyName);
            Assert.AreEqual(1, property.PropertyArgs[0].Values.Count);
            Assert.IsInstanceOf<CssFunctionPropertyValue>(property.PropertyArgs[0].Values[0]);

            var propertyArg = (CssFunctionPropertyValue)property.PropertyArgs[0].Values[0];
            Assert.AreEqual("rgba", propertyArg.FunctionName);
            Assert.AreEqual(4, propertyArg.Args.Count);
        }

        [Test]
        public void MediaQueryTest()
        {
            var css = @"@media all and (max-width: 699px) and (width >= 200px) { #sidebar { } }";
            CssGrammer grammer = new CssGrammer(css, false);
            Assert.IsNotNull(grammer.MediaRules);
            Assert.AreEqual(1, grammer.MediaRules.Count);
            Assert.AreEqual(1, grammer.MediaRules[0].MediaQueires.Count);
            Assert.AreEqual(3, grammer.MediaRules[0].MediaQueires[0].MediaRules.Count);

            Assert.AreEqual(1, grammer.MediaRules[0].RuleSet.Count);

            var mediaQuery = grammer.MediaRules[0].MediaQueires[0];
            Assert.IsInstanceOf<MediaTypeRule>(mediaQuery.MediaRules[0]);
            Assert.IsInstanceOf<PropertyEqualityRule>(mediaQuery.MediaRules[1]);
            Assert.IsInstanceOf<PropertyRangeRule>(mediaQuery.MediaRules[2]);

            Assert.AreEqual("all", ((MediaTypeRule)mediaQuery.MediaRules[0]).MediaType);
            Assert.AreEqual("max-width", ((PropertyRule)mediaQuery.MediaRules[1]).PropertyName);
            Assert.AreEqual("699px", ((PropertyEqualityRule)mediaQuery.MediaRules[1]).Value.ToString());

            Assert.AreEqual("width", ((PropertyRule)mediaQuery.MediaRules[2]).PropertyName);
            Assert.AreEqual("200px", ((PropertyRangeRule)mediaQuery.MediaRules[2]).RightValue.ToString());
            Assert.AreEqual(">=", ((PropertyRangeRule)mediaQuery.MediaRules[2]).RightOp);

            Assert.IsNull(((PropertyRangeRule)mediaQuery.MediaRules[2]).LeftValue);
            Assert.IsNull(((PropertyRangeRule)mediaQuery.MediaRules[2]).LeftOp);
        }
    }
}

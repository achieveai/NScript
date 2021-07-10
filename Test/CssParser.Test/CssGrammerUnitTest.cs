namespace CssParser.Test
{
    using System;
    using CssParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CssGrammerUnitTest
    {
        [TestMethod]
        public void BasicTest()
        {
            CssGrammer grammer = new CssGrammer(".body { border: #ddeeff thin 123px solid; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            Assert.AreEqual(1, grammer.Rules[0].Selectors.Count);
            Assert.IsInstanceOfType(grammer.Rules[0].Selectors[0], typeof(CssClassName));

            Assert.AreEqual(1, grammer.Rules[0].Properties.Count);
            Assert.AreEqual("border", grammer.Rules[0].Properties[0].PropertyName);
            var propertyArgs = new string[] { "#ddeeff", "thin", "123px", "solid" };
            Assert.AreEqual(propertyArgs.Length, grammer.Rules[0].Properties[0].PropertyArgs[0].Values.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Rules[0].Properties[0].PropertyArgs[0].Values[i].ToString());
            }
        }

        [TestMethod]
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

        [TestMethod]
        public void TestCssSelectorUnder()
        {
            CssGrammer grammer = new CssGrammer("div .list {}");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(CssRuleSelector));
            CssRuleSelector ruleSelector = selectors[0] as CssRuleSelector;
            Assert.AreEqual(2, ruleSelector.Selectors.Count);
            Assert.IsInstanceOfType(ruleSelector.Selectors[1], typeof(CssClassName));
            Assert.IsInstanceOfType(ruleSelector.Selectors[0], typeof(CssTagName));
            Assert.AreEqual(SelectorOp.Under, ruleSelector.Ops[0]);
        }

        [TestMethod]
        public void TestCssSelectorUnder1()
        {
            CssGrammer grammer = new CssGrammer("div:first-child() .list {}");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(CssRuleSelector));
            CssRuleSelector ruleSelector = selectors[0] as CssRuleSelector;
            Assert.AreEqual(2, ruleSelector.Selectors.Count);
            Assert.IsInstanceOfType(ruleSelector.Selectors[1], typeof(CssClassName));
            Assert.IsInstanceOfType(ruleSelector.Selectors[0], typeof(AndCssSelector));
            Assert.AreEqual(SelectorOp.Under, ruleSelector.Ops[0]);
        }

        [TestMethod]
        public void TestCssSelectorAnd()
        {
            CssGrammer grammer = new CssGrammer("div.list {}");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(AndCssSelector));
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOfType(andCssSelector.Selectors[0], typeof(CssTagName));
            Assert.IsInstanceOfType(andCssSelector.Selectors[1], typeof(CssClassName));
        }

        [TestMethod]
        public void TestCssSelectorAnd1()
        {
            CssGrammer grammer = new CssGrammer("div:first-child().list {}");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(AndCssSelector));
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(3, andCssSelector.Selectors.Count);
            Assert.IsInstanceOfType(andCssSelector.Selectors[0], typeof(CssTagName));
            Assert.IsInstanceOfType(andCssSelector.Selectors[2], typeof(CssClassName));
        }

        [TestMethod]
        public void TestCssSelector()
        {
            CssGrammer grammer = new CssGrammer(".list > div { border: #ddeeff thin 1px solid; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(CssRuleSelector));
            CssRuleSelector ruleSelector = selectors[0] as CssRuleSelector;
            Assert.AreEqual(2, ruleSelector.Selectors.Count);
            Assert.IsInstanceOfType(ruleSelector.Selectors[0], typeof(CssClassName));
            Assert.IsInstanceOfType(ruleSelector.Selectors[1], typeof(CssTagName));
            Assert.AreEqual(SelectorOp.ParentOf, ruleSelector.Ops[0]);
        }

        [TestMethod]
        public void TestCssRuleSelector3Deep()
        {
            CssGrammer grammer = new CssGrammer(".list > a > #div { border: #ddeeff thin 1px solid; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(CssRuleSelector));
            CssRuleSelector ruleSelector = selectors[0] as CssRuleSelector;
            Assert.AreEqual(3, ruleSelector.Selectors.Count);
            Assert.IsInstanceOfType(ruleSelector.Selectors[0], typeof(CssClassName));
            Assert.IsInstanceOfType(ruleSelector.Selectors[1], typeof(CssTagName));
            Assert.IsInstanceOfType(ruleSelector.Selectors[2], typeof(CssId));
        }

        [TestMethod]
        public void TestAttributeSelector()
        {
            CssGrammer grammer = new CssGrammer("a[t=m] > div { color: red; }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(CssRuleSelector));
            CssRuleSelector ruleSelector = selectors[0] as CssRuleSelector;
            Assert.AreEqual(2, ruleSelector.Selectors.Count);
            Assert.IsInstanceOfType(ruleSelector.Selectors[0], typeof(AndCssSelector));
            Assert.IsInstanceOfType(ruleSelector.Selectors[1], typeof(CssTagName));

            AndCssSelector andCssSelector = (AndCssSelector)ruleSelector.Selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOfType(andCssSelector.Selectors[0], typeof(CssTagName));
            Assert.IsInstanceOfType(andCssSelector.Selectors[1], typeof(AttributeSelector));

            AttributeSelector attrSel = (AttributeSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("t", attrSel.Attribute);
            Assert.AreEqual("m", attrSel.Value);
            Assert.AreEqual(attrSel.Condition, AttributeCondition.Equal);
        }

        [TestMethod]
        public void TestPseudoSelector()
        {
            CssGrammer grammer = new CssGrammer("a:hover { }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(AndCssSelector));
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOfType(andCssSelector.Selectors[0], typeof(CssTagName));
            Assert.IsInstanceOfType(andCssSelector.Selectors[1], typeof(PseudoSelector));

            PseudoSelector attrSel = (PseudoSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("hover", attrSel.Name);
            Assert.IsNull(attrSel.Arg);
        }

        [TestMethod]
        public void TestPseudoFuncSelector()
        {
            CssGrammer grammer = new CssGrammer("a:first-child() { }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(AndCssSelector));
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOfType(andCssSelector.Selectors[0], typeof(CssTagName));
            Assert.IsInstanceOfType(andCssSelector.Selectors[1], typeof(PseudoSelector));

            PseudoSelector attrSel = (PseudoSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("first-child", attrSel.Name);
            Assert.AreEqual("", attrSel.Arg);
        }

        [TestMethod]
        public void TestPseudoFuncNumSelector()
        {
            CssGrammer grammer = new CssGrammer("a:nth-child(2) { }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(AndCssSelector));
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOfType(andCssSelector.Selectors[0], typeof(CssTagName));
            Assert.IsInstanceOfType(andCssSelector.Selectors[1], typeof(PseudoSelector));

            PseudoSelector attrSel = (PseudoSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("nth-child", attrSel.Name);
            Assert.AreEqual("2", attrSel.Arg);
        }

        [TestMethod]
        public void TestPseudoFuncArgIdentSelector()
        {
            CssGrammer grammer = new CssGrammer("a:nth-child(odd) { }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(AndCssSelector));
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOfType(andCssSelector.Selectors[0], typeof(CssTagName));
            Assert.IsInstanceOfType(andCssSelector.Selectors[1], typeof(PseudoSelector));

            PseudoSelector attrSel = (PseudoSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("nth-child", attrSel.Name);
            Assert.AreEqual("odd", attrSel.Arg);
        }

        [TestMethod]
        public void TestPseudoFuncArgMultSelector()
        {
            CssGrammer grammer = new CssGrammer("a:nth-child(2n + 10) { }");
            Assert.AreEqual(1, grammer.Rules.Count);
            var selectors = grammer.Rules[0].Selectors;
            Assert.AreEqual(1, selectors.Count);
            Assert.IsInstanceOfType(selectors[0], typeof(AndCssSelector));
            AndCssSelector andCssSelector = (AndCssSelector)selectors[0];
            Assert.AreEqual(2, andCssSelector.Selectors.Count);
            Assert.IsInstanceOfType(andCssSelector.Selectors[0], typeof(CssTagName));
            Assert.IsInstanceOfType(andCssSelector.Selectors[1], typeof(PseudoSelector));

            PseudoSelector attrSel = (PseudoSelector)andCssSelector.Selectors[1];
            Assert.AreEqual("nth-child", attrSel.Name);
            Assert.AreEqual("2n+10", attrSel.Arg);
        }

        [TestMethod]
        public void PropertyOnlyTest()
        {
            CssGrammer grammer = new CssGrammer("border: #ddeeff thin 1px solid;", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(1, grammer.Properties.Count);

            Assert.AreEqual("border", grammer.Properties[0].PropertyName);

            var propertyArgs = new string[] { "#ddeeff", "thin", "1px", "solid" };
            Assert.AreEqual(propertyArgs.Length, grammer.Properties[0].PropertyArgs[0].Values.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Properties[0].PropertyArgs[0].Values[i].ToString());
            }
        }

        [TestMethod]
        public void PropertiesOnlyTest()
        {
            CssGrammer grammer = new CssGrammer("border: #ddeeff thin 1px solid; background-color: #ffeedd;", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(2, grammer.Properties.Count);

            var propertyArgs = new string[] { "#ddeeff", "thin", "1px", "solid" };
            Assert.AreEqual(propertyArgs.Length, grammer.Properties[0].PropertyArgs[0].Values.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Properties[0].PropertyArgs[0].Values[i].ToString());
            }

            propertyArgs = new string[] { "#ffeedd" };
            Assert.AreEqual(propertyArgs.Length, grammer.Properties[1].PropertyArgs[0].Values.Count);
            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Properties[1].PropertyArgs[0].Values[i].ToString());
            }
        }

        [TestMethod]
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

        [TestMethod]
        public void CalcPropertyValueTest()
        {
            CssGrammer grammer = new CssGrammer("height: calc(10% + 10em);", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(1, grammer.Properties.Count);

            var property = grammer.Properties[0];
            Assert.AreEqual("height", property.PropertyName);
            Assert.AreEqual(1, property.PropertyArgs[0].Values.Count);
            Assert.IsInstanceOfType(property.PropertyArgs[0].Values[0], typeof(CssCalcPropertyValue));

            var propertyArg = (CssCalcPropertyValue)property.PropertyArgs[0].Values[0];
            Assert.AreEqual(1, propertyArg.Operators.Length);
            Assert.AreEqual('+', propertyArg.Operators[0]);
            Assert.AreEqual(2, propertyArg.PropertyValues.Length);
            Assert.AreEqual("10%", propertyArg.PropertyValues[0].ToString());
            Assert.AreEqual("10em", propertyArg.PropertyValues[1].ToString());
        }

        [TestMethod]
        public void FunctionPropertyValueTest()
        {
            CssGrammer grammer = new CssGrammer("color: rgba(10,20,40,.5);", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(1, grammer.Properties.Count);

            var property = grammer.Properties[0];
            Assert.AreEqual("color", property.PropertyName);
            Assert.AreEqual(1, property.PropertyArgs[0].Values.Count);
            Assert.IsInstanceOfType(property.PropertyArgs[0].Values[0], typeof(CssFunctionPropertyValue));

            var propertyArg = (CssFunctionPropertyValue)property.PropertyArgs[0].Values[0];
            Assert.AreEqual("rgba", propertyArg.FunctionName);
            Assert.AreEqual(4, propertyArg.Args.Count);
        }

        [TestMethod]
        public void FunctionPropertyValueTestWithLinearGradient()
        {
            CssGrammer grammer = new CssGrammer("background-image: linear-gradient(top bottom, #03a9f4, #0288d1);", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(1, grammer.Properties.Count);

            var property = grammer.Properties[0];
            Assert.AreEqual("background-image", property.PropertyName);
            Assert.AreEqual(1, property.PropertyArgs[0].Values.Count);
            Assert.IsInstanceOfType(property.PropertyArgs[0].Values[0], typeof(CssFunctionPropertyValue));

            var propertyArg = (CssFunctionPropertyValue)property.PropertyArgs[0].Values[0];
            Assert.AreEqual("linear-gradient", propertyArg.FunctionName);
            Assert.AreEqual(3, propertyArg.Args.Count);
        }

        [TestMethod]
        public void FunctionPropertyWebkitLinearGradientProperty()
        {
            CssGrammer grammer = new CssGrammer(
                "background-image: -webkit-linear-gradient(45deg, rgba(255, 255, 255, 0.15) 25%, transparent 25%, transparent 50%, rgba(255, 255, 255, 0.15) 50%, rgba(255, 255, 255, 0.15) 75%, transparent 75%, transparent);",
                true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(1, grammer.Properties.Count);

            var property = grammer.Properties[0];
            Assert.AreEqual("background-image", property.PropertyName);
            Assert.AreEqual(1, property.PropertyArgs[0].Values.Count);
            Assert.IsInstanceOfType(property.PropertyArgs[0].Values[0], typeof(CssFunctionPropertyValue));

            var propertyArg = (CssFunctionPropertyValue)property.PropertyArgs[0].Values[0];
            Assert.AreEqual("-webkit-linear-gradient", propertyArg.FunctionName);
            Assert.AreEqual(8, propertyArg.Args.Count);
        }


        [TestMethod]
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
            Assert.IsInstanceOfType(mediaQuery.MediaRules[0], typeof(MediaTypeRule));
            Assert.IsInstanceOfType(mediaQuery.MediaRules[1], typeof(PropertyEqualityRule));
            Assert.IsInstanceOfType(mediaQuery.MediaRules[2], typeof(PropertyRangeRule));

            Assert.AreEqual("all", ((MediaTypeRule)mediaQuery.MediaRules[0]).MediaType);
            Assert.AreEqual("max-width", ((PropertyRule)mediaQuery.MediaRules[1]).PropertyName);
            Assert.AreEqual("699px", ((PropertyEqualityRule)mediaQuery.MediaRules[1]).Value.ToString());

            Assert.AreEqual("width", ((PropertyRule)mediaQuery.MediaRules[2]).PropertyName);
            Assert.AreEqual("200px", ((PropertyRangeRule)mediaQuery.MediaRules[2]).RightValue.ToString());
            Assert.AreEqual(">=", ((PropertyRangeRule)mediaQuery.MediaRules[2]).RightOp);

            Assert.IsNull(((PropertyRangeRule)mediaQuery.MediaRules[2]).LeftValue);
            Assert.IsNull(((PropertyRangeRule)mediaQuery.MediaRules[2]).LeftOp);
        }

        [TestMethod]
        public void TestRemPropertyValue()
        {
            var grammer = new CssGrammer("font-size: 3rem;", true);
            Assert.IsNull(grammer.Rules);
            Assert.IsNotNull(grammer.Properties);
            Assert.AreEqual(1, grammer.Properties.Count);

            Assert.AreEqual("font-size", grammer.Properties[0].PropertyName);

            var propertyArgs = new string[] { "3rem" };
            Assert.AreEqual(
                propertyArgs.Length,
                grammer.Properties[0].PropertyArgs[0].Values.Count);

            for (int i = 0; i < propertyArgs.Length; i++)
            {
                Assert.AreEqual(propertyArgs[i], grammer.Properties[0].PropertyArgs[0].Values[i].ToString());
            }
        }
    }
}

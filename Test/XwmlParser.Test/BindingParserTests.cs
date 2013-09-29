//-----------------------------------------------------------------------
// <copyright file="BindingParserTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using System;
    using System.Collections.Generic;
    using NScript.CLR.Test;
    using MbUnit.Framework;
    using XwmlParser.Binding;

    /// <summary>
    /// Definition for BindingParserTests
    /// </summary>
    [TestFixture]
    public class BindingParserTests
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Helper.Initialize();
        }

        [Test]
        [Row("PropStr1")]
        [Row("PropStr1, Mode=OneTime")]
        [Row("PropStr1, Mode=OneTime, Source=DataContext")]
        [Row("Mode=OneTime, Source=DataContext, Path=PropStr1")]
        public void TestParser1(string bindingStr)
        {
            var dataContextType = 
                Helper.Resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelA");
            var controlType =
                Helper.Resolver.GetTypeReference("Sunlight.Framework.UI!Sunlight.Framework.UI.UISkinableElement");
            var propertyBinding = Binding.BindingParser.ParseBinding(
                null,
                bindingStr,
                new MockDocumentContext(null, Helper.Resolver),
                dataContextType,
                controlType);

            Assert.AreEqual(
                BindingMode.OneTime,
                propertyBinding.Mode);

            Assert.AreEqual(
                SourceType.DataContext,
                propertyBinding.SourceType);

            Assert.IsInstanceOfType<PropertySourceBindingInfo>(propertyBinding.SourceBindingInfo);

            Assert.AreEqual(
                null,
                propertyBinding.ConverterInfo);

            Assert.AreEqual(
                dataContextType,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).SourceType);

            Assert.AreEqual(
                1,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).PropertyReferencePath.Count);

            Assert.AreEqual(
                Helper.Resolver.GetPropertyReference(
                    dataContextType,
                    "PropStr1"),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).PropertyReferencePath[0]);
        }

        [Test]
        [Row("TestIface.PropStr2")]
        [Row("TestIface.PropStr2, Mode=OneTime")]
        [Row("TestIface.PropStr2, Mode=OneTime, Source=DataContext")]
        [Row("Mode=OneTime, Source=DataContext, Path=TestIface.PropStr2")]
        public void TestParser2(string bindingStr)
        {
            var dataContextType = 
                Helper.Resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelA");
            var testIfaceType =
                Helper.Resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestInterface");
            var controlType =
                Helper.Resolver.GetTypeReference("Sunlight.Framework.UI!Sunlight.Framework.UI.UISkinableElement");
            var propertyBinding = Binding.BindingParser.ParseBinding(
                null,
                bindingStr,
                new MockDocumentContext(null, Helper.Resolver),
                dataContextType,
                controlType);

            Assert.AreEqual(
                BindingMode.OneTime,
                propertyBinding.Mode);

            Assert.AreEqual(
                SourceType.DataContext,
                propertyBinding.SourceType);

            Assert.IsInstanceOfType<PropertySourceBindingInfo>(propertyBinding.SourceBindingInfo);

            Assert.AreEqual(
                null,
                propertyBinding.ConverterInfo);

            Assert.AreEqual(
                dataContextType,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).SourceType);

            Assert.IsInstanceOfType<PropertySourceBindingInfo>(propertyBinding.SourceBindingInfo);

            Assert.AreEqual(
                2,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).PropertyReferencePath.Count);

            Assert.AreEqual(
                Helper.Resolver.GetPropertyReference(
                    dataContextType,
                    "TestIface"),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).PropertyReferencePath[0]);

            Assert.AreEqual(
                Helper.Resolver.GetPropertyReference(
                    testIfaceType,
                    "PropStr2"),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).PropertyReferencePath[1]);
        }

        [Test]
        [Row("((testVM:TestViewModelB)TestIface).PropStrB", BindingMode.OneTime)]
        [Row("((testVM:TestViewModelB)TestIface).PropStrB, Mode=OneWay", BindingMode.OneWay)]
        [Row("((testVM:TestViewModelB)TestIface).PropStrB, Mode=OneWay, Source=DataContext", BindingMode.OneWay)]
        [Row("Mode=OneTime, Source=DataContext, Path=((testVM:TestViewModelB)TestIface).PropStrB", BindingMode.OneTime)]
        public void TestParserCast(string bindingStr, BindingMode mode)
        {
            var dataContextType = 
                Helper.Resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelA");
            var testVMB =
                Helper.Resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelB");
            var controlType =
                Helper.Resolver.GetTypeReference("Sunlight.Framework.UI!Sunlight.Framework.UI.UISkinableElement");
            var documentContext = new MockDocumentContext(null, Helper.Resolver);
            documentContext.AddNsMapping("testVM", "Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test");
            var propertyBinding = Binding.BindingParser.ParseBinding(
                null,
                bindingStr,
                documentContext,
                dataContextType,
                controlType);

            Assert.AreEqual(
                mode,
                propertyBinding.Mode);

            Assert.AreEqual(
                SourceType.DataContext,
                propertyBinding.SourceType);

            Assert.IsInstanceOfType<PropertySourceBindingInfo>(propertyBinding.SourceBindingInfo);

            Assert.AreEqual(
                null,
                propertyBinding.ConverterInfo);

            Assert.AreEqual(
                dataContextType,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).SourceType);

            Assert.IsInstanceOfType<PropertySourceBindingInfo>(propertyBinding.SourceBindingInfo);

            Assert.AreEqual(
                2,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).PropertyReferencePath.Count);

            Assert.AreEqual(
                Helper.Resolver.GetPropertyReference(
                    dataContextType,
                    "TestIface"),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).PropertyReferencePath[0]);

            Assert.AreEqual(
                Helper.Resolver.GetPropertyReference(
                    testVMB,
                    "PropStrB"),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).PropertyReferencePath[1]);
        }

    }
}

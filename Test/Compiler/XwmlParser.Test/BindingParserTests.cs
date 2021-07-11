//-----------------------------------------------------------------------
// <copyright file="BindingParserTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.Test
{
    using Mono.Cecil;
    using NScript.CLR;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using XwmlParser.Binding;

    /// <summary>
    /// Definition for BindingParserTests
    /// </summary>
    [TestClass]
    public class BindingParserTests
    {
        ClrKnownReferences clrKnownReferences;
        TypeResolver resolver;
        ParserContext parserContext;

        /// <summary>
        /// Setups this instance.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            Helper.Initialize();
            parserContext = Helper.GetParserContext();
            clrKnownReferences = parserContext.KnownTypes.ClrKnownReference;
            resolver = (TypeResolver)parserContext.ClrResolver;
        }

        [DataTestMethod]
        [DataRow("{PropStr1}")]
        [DataRow("{ PropStr1}")]
        [DataRow("{PropStr1 }")]
        [DataRow("{PropStr1, Mode=OneTime}")]
        [DataRow("{ PropStr1, Mode=OneTime }")]
        [DataRow("{PropStr1, Mode=OneTime, Source=DataContext}")]
        [DataRow("{Mode=OneTime, Source=DataContext, Path=PropStr1}")]
        public void TestParser1(string bindingStr)
        {
            var dataContextType =
                resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelA");
            var controlType =
                resolver.GetTypeReference("Sunlight.Framework.UI!Sunlight.Framework.UI.UISkinableElement");
            var propertyBinding = Binding.BindingParser.ParseBinding(
                new TempTargetBinding(clrKnownReferences.String),
                bindingStr,
                new MockDocumentContext(parserContext, resolver),
                dataContextType,
                controlType);

            Assert.AreEqual(
                BindingMode.OneTime,
                propertyBinding.Mode);

            Assert.AreEqual(
                SourceType.DataContext,
                propertyBinding.SourceType);

            Assert.IsInstanceOfType(propertyBinding.SourceBindingInfo, typeof(PropertySourceBindingInfo));

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
                resolver.GetPropertyReference(
                    dataContextType,
                    "PropStr1").GetDefinition(),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo)
                    .PropertyReferencePath[0]
                    .GetDefinition());
        }

        [DataTestMethod]
        [DataRow("{PropStr1}")]
        [DataRow("{PropStr1, Mode=OneTime}")]
        [DataRow("{PropStr1, Mode=OneTime, Source=DataContext}")]
        [DataRow("{Mode=OneTime, Source=DataContext, Path=PropStr1}")]
        public void TestParserWithInheritance(string bindingStr)
        {
            var dataContextType =
                resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelB");
            var controlType =
                resolver.GetTypeReference("Sunlight.Framework.UI!Sunlight.Framework.UI.UISkinableElement");
            var propertyBinding = Binding.BindingParser.ParseBinding(
                new TempTargetBinding(clrKnownReferences.String),
                bindingStr,
                new MockDocumentContext(
                    parserContext,
                    resolver),
                dataContextType,
                controlType);

            Assert.AreEqual(
                BindingMode.OneTime,
                propertyBinding.Mode);

            Assert.AreEqual(
                SourceType.DataContext,
                propertyBinding.SourceType);

            Assert.IsInstanceOfType(propertyBinding.SourceBindingInfo, typeof(PropertySourceBindingInfo));

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
                resolver
                    .GetPropertyReference(
                        dataContextType,
                        "PropStr1")
                    .GetDefinition(),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo)
                    .PropertyReferencePath[0]
                    .GetDefinition());
        }

        [DataTestMethod]
        [DataRow("{TestIface.PropStr2}")]
        [DataRow("{TestIface.PropStr2, Mode=OneTime}")]
        [DataRow("{TestIface.PropStr2, Mode=OneTime, Source=DataContext}")]
        [DataRow("{Mode=OneTime, Source=DataContext, Path=TestIface.PropStr2}")]
        public void TestParser2(string bindingStr)
        {
            var dataContextType =
                resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelA");
            var testIfaceType =
                resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestInterface");
            var controlType =
                resolver.GetTypeReference("Sunlight.Framework.UI!Sunlight.Framework.UI.UISkinableElement");
            var propertyBinding = Binding.BindingParser.ParseBinding(
                new TempTargetBinding(clrKnownReferences.String),
                bindingStr,
                new MockDocumentContext(
                    parserContext,
                    resolver),
                dataContextType,
                controlType);

            Assert.AreEqual(
                BindingMode.OneTime,
                propertyBinding.Mode);

            Assert.AreEqual(
                SourceType.DataContext,
                propertyBinding.SourceType);

            Assert.IsInstanceOfType(propertyBinding.SourceBindingInfo, typeof(PropertySourceBindingInfo));

            Assert.AreEqual(
                null,
                propertyBinding.ConverterInfo);

            Assert.AreEqual(
                dataContextType,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).SourceType);

            Assert.IsInstanceOfType(propertyBinding.SourceBindingInfo, typeof(PropertySourceBindingInfo));

            Assert.AreEqual(
                2,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).PropertyReferencePath.Count);

            Assert.AreEqual(
                resolver
                    .GetPropertyReference(
                        dataContextType,
                        "TestIface")
                    .GetDefinition(),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo)
                    .PropertyReferencePath[0]
                    .GetDefinition());

            Assert.AreEqual(
                resolver
                    .GetPropertyReference(
                        testIfaceType,
                        "PropStr2")
                    .GetDefinition(),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo)
                    .PropertyReferencePath[1]
                    .GetDefinition());
        }

        [DataTestMethod]
        [DataRow("{((testVM:TestViewModelB)TestIface).PropStrB}", BindingMode.OneTime)]
        [DataRow("{((testVM:TestViewModelB)TestIface).PropStrB, Mode=OneWay}", BindingMode.OneWay)]
        [DataRow("{((testVM:TestViewModelB)TestIface).PropStrB, Mode=OneWay, Source=DataContext}", BindingMode.OneWay)]
        [DataRow("{Mode=OneTime, Source=DataContext, Path=((testVM:TestViewModelB)TestIface).PropStrB}", BindingMode.OneTime)]
        public void TestParserCast(string bindingStr, BindingMode mode)
        {
            var dataContextType =
                resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelA");
            var testVMB =
                resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelB");
            var controlType =
                resolver.GetTypeReference("Sunlight.Framework.UI!Sunlight.Framework.UI.UISkinableElement");
            var documentContext = new MockDocumentContext(
                parserContext,
                resolver);
            documentContext.AddNsMapping("testVM", "Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test");
            var propertyBinding = Binding.BindingParser.ParseBinding(
                new TempTargetBinding(clrKnownReferences.String),
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

            Assert.IsInstanceOfType(propertyBinding.SourceBindingInfo, typeof(PropertySourceBindingInfo));

            Assert.AreEqual(
                null,
                propertyBinding.ConverterInfo);

            Assert.AreEqual(
                dataContextType,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).SourceType);

            Assert.IsInstanceOfType(propertyBinding.SourceBindingInfo, typeof(PropertySourceBindingInfo));

            Assert.AreEqual(
                2,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).PropertyReferencePath.Count);

            Assert.AreEqual(
                resolver
                    .GetPropertyReference(
                        dataContextType,
                        "TestIface")
                    .GetDefinition(),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo)
                    .PropertyReferencePath[0]
                    .GetDefinition());

            Assert.AreEqual(
                resolver
                    .GetPropertyReference(
                        testVMB,
                        "PropStrB")
                    .GetDefinition(),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo)
                    .PropertyReferencePath[1]
                    .GetDefinition());
        }

        [DataTestMethod]
        [DataRow("{testVM:TestViewModelB.StaticProp.PropStrB}", BindingMode.OneTime)]
        [DataRow("{testVM:TestViewModelB.StaticProp.PropStrB, Mode=OneWay}", BindingMode.OneWay)]
        [DataRow("{testVM:TestViewModelB.StaticProp.PropStrB, Mode=OneWay, Source=DataContext}", BindingMode.OneWay)]
        [DataRow("{Mode=OneTime, Source=DataContext, Path=testVM:TestViewModelB.StaticProp.PropStrB}", BindingMode.OneTime)]
        public void TestParserStatic(string bindingStr, BindingMode mode)
        {
            var dataContextType =
                resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelA");
            var testVMB =
                resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelB");
            var controlType =
                resolver.GetTypeReference("Sunlight.Framework.UI!Sunlight.Framework.UI.UISkinableElement");
            var documentContext = new MockDocumentContext(
                parserContext,
                resolver);
            documentContext.AddNsMapping("testVM", "Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test");
            var propertyBinding = Binding.BindingParser.ParseBinding(
                new TempTargetBinding(clrKnownReferences.String),
                bindingStr,
                documentContext,
                dataContextType,
                controlType);

            Assert.AreEqual(
                mode,
                propertyBinding.Mode);

            Assert.AreEqual(
                SourceType.Static,
                propertyBinding.SourceType);

            Assert.IsInstanceOfType(propertyBinding.SourceBindingInfo, typeof(PropertySourceBindingInfo));

            Assert.AreEqual(
                null,
                propertyBinding.ConverterInfo);

            Assert.IsInstanceOfType(propertyBinding.SourceBindingInfo, typeof(PropertySourceBindingInfo));

            Assert.AreEqual(
                null,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).SourceType);

            Assert.AreEqual(
                2,
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo).PropertyReferencePath.Count);

            Assert.AreEqual(
                resolver
                    .GetPropertyReference(
                        testVMB,
                        "StaticProp")
                    .GetDefinition(),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo)
                    .PropertyReferencePath[0]
                    .GetDefinition());

            Assert.AreEqual(
                resolver
                    .GetPropertyReference(
                        testVMB,
                        "PropStrB")
                    .GetDefinition(),
                ((PropertySourceBindingInfo)propertyBinding.SourceBindingInfo)
                    .PropertyReferencePath[1]
                    .GetDefinition());
        }

        [DataTestMethod]
        [DataRow("{testVM:TestViewModelB.StaticProp.PropStrB, Converter=testVM:ConverterCollection.ParseStuff}")]
        public void TestParseConverter(string bindingStr)
        {
            var dataContextType =
                resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelA");
            var testVMB =
                resolver.GetTypeReference("Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test.TestViewModelB");
            var controlType =
                resolver.GetTypeReference("Sunlight.Framework.UI!Sunlight.Framework.UI.UISkinableElement");
            var documentContext = new MockDocumentContext(
                parserContext,
                resolver);
            documentContext.AddNsMapping("testVM", "Sunlight.Framework.UI.Test!Sunlight.Framework.UI.Test");
            var propertyBinding = Binding.BindingParser.ParseBinding(
                new TempTargetBinding(clrKnownReferences.Int32),
                bindingStr,
                documentContext,
                dataContextType,
                controlType);

            Assert.IsNotNull(propertyBinding.ConverterInfo);
            Assert.IsInstanceOfType(propertyBinding.ConverterInfo, typeof(DelegateConverterInfo));

            DelegateConverterInfo converterInfo = propertyBinding.ConverterInfo as DelegateConverterInfo;
            Assert.AreEqual(converterInfo.MethodReference.Name, "ParseStuff");
        }
    }

    public class TempTargetBinding : TargetBindingInfo
    {
        public TempTargetBinding(TypeReference stringType)
            : base(stringType)
        {
        }
    }
}

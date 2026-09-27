using System;
using System.Linq;
using BindingFlags = System.Reflection.BindingFlags;
using TargetInvocationException = System.Reflection.TargetInvocationException;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil;
using NScript.RazorSkin.CodeGen;
using NScript.RazorSkin.TemplateIR;
using NScript.Utils;

namespace RazorSkinParser.Test
{
    [TestClass]
    public class SubControlValidationFollowupTests
    {
        [TestMethod]
        public void ControlWithoutElementConstructorReportsCompileError()
        {
            var type = CreateControl(out var module);
            AddConstructor(type, module.TypeSystem.String);
            var sub = new SubControlNode { TypeName = type.Name };

            Action validate = () => Validate(sub, type);

            validate.Should().Throw<RazorSubControlDiagnosticException>()
                .WithMessage("*Element*constructor*");
        }

        [TestMethod]
        public void StaticAttributesUseTheirTargetPropertyTypes()
        {
            var type = CreateControl(out var module);
            AddElementConstructor(type, module);
            AddProperty(type, "IsOn", module.TypeSystem.Boolean);
            AddProperty(type, "Count", module.TypeSystem.Int32);
            AddProperty(type, "Mode", new TypeReference("Tests", "Mode", module, module));
            AddProperty(type, "Text", module.TypeSystem.String);
            var sub = new SubControlNode { TypeName = type.Name };
            AddLiteral(sub, "IsOn", "false");
            AddLiteral(sub, "Count", "5");
            AddLiteral(sub, "Mode", "Mode.Fast");
            AddLiteral(sub, "Text", "plain text");

            Validate(sub, type);
            var topology = GraphTopologyBuilder.Build(new SkinTemplateNode { Children = { sub } });
            var bindings = topology.SubControls.Single().PropertyBindings;

            topology.GetterExpressions[bindings.Single(b => b.TargetPropertyName == "IsOn").NodeIdx]
                .Should().Be("false");
            topology.GetterExpressions[bindings.Single(b => b.TargetPropertyName == "Count").NodeIdx]
                .Should().Be("5");
            topology.GetterExpressions[bindings.Single(b => b.TargetPropertyName == "Mode").NodeIdx]
                .Should().Be("Mode.Fast");
            topology.GetterExpressions[bindings.Single(b => b.TargetPropertyName == "Text").NodeIdx]
                .Should().Be("\"plain text\"");
        }

        [TestMethod]
        public void FuncPropertyDoesNotSilentlyBecomeVoidEventHandler()
        {
            var type = CreateControl(out var module);
            AddElementConstructor(type, module);
            AddProperty(type, "ReadValue", new GenericInstanceType(
                new TypeReference("System", "Func`1", module, module.TypeSystem.CoreLibrary))
            {
                GenericArguments = { module.TypeSystem.Int32 }
            });
            var sub = new SubControlNode { TypeName = type.Name };
            sub.PropertyBindings.Add(new SubControlPropertyBinding
            {
                PropertyName = "ReadValue",
                Classification = new BindingClassification { CSharpExpression = "Model.ReadValue" }
            });

            Action validate = () => Validate(sub, type);

            validate.Should().Throw<RazorSubControlDiagnosticException>()
                .WithMessage("*Func*return*");
        }

        [TestMethod]
        public void TwoWayDefaultFallsBackToOneWayForComplexSourceOrUnreadableTarget()
        {
            var type = CreateControl(out var module);
            AddElementConstructor(type, module);
            AddProperty(type, "Value", module.TypeSystem.String, hasGetter: true, twoWayDefault: true);
            AddProperty(type, "WriteOnly", module.TypeSystem.String, hasGetter: false, twoWayDefault: true);
            var sub = new SubControlNode { TypeName = type.Name };
            sub.PropertyBindings.Add(new SubControlPropertyBinding
            {
                PropertyName = "Value",
                Classification = new BindingClassification
                {
                    CSharpExpression = "Model.A + Model.B", Mode = BindingMode.OneWay
                }
            });
            sub.PropertyBindings.Add(new SubControlPropertyBinding
            {
                PropertyName = "WriteOnly",
                Classification = new BindingClassification
                {
                    CSharpExpression = "Model.A", Mode = BindingMode.OneWay
                }
            });

            Validate(sub, type);

            sub.PropertyBindings.Should().OnlyContain(b => b.Classification.Mode == BindingMode.OneWay);
        }

        [TestMethod]
        public void TwoWayDefaultRequiresWritableModelProperty()
        {
            var control = CreateControl(out var module);
            AddElementConstructor(control, module);
            AddProperty(control, "Value", module.TypeSystem.String, twoWayDefault: true);
            var model = new TypeDefinition("Tests", "Model", TypeAttributes.Public);
            module.Types.Add(model);
            AddProperty(model, "ReadOnly", module.TypeSystem.String, hasSetter: false);
            AddProperty(model, "Writable", module.TypeSystem.String);
            var sub = new SubControlNode { TypeName = control.Name };
            sub.PropertyBindings.Add(new SubControlPropertyBinding
            {
                PropertyName = "Value",
                Classification = new BindingClassification
                { CSharpExpression = "Model.ReadOnly", Mode = BindingMode.OneWay }
            });

            Validate(sub, control, model);
            sub.PropertyBindings[0].Classification.Mode.Should().Be(BindingMode.OneWay);

            sub.PropertyBindings[0].Classification.CSharpExpression = "Model.Writable";
            Validate(sub, control, model);
            sub.PropertyBindings[0].Classification.Mode.Should().Be(BindingMode.TwoWay);
        }

        [TestMethod]
        public void InheritedPublicControlHandlerResolves()
        {
            var module = ModuleDefinition.CreateModule("Handlers", ModuleKind.Dll);
            var baseType = new TypeDefinition("Tests", "Base", TypeAttributes.Public);
            var derivedType = new TypeDefinition("Tests", "Derived", TypeAttributes.Public, baseType);
            module.Types.Add(baseType);
            module.Types.Add(derivedType);
            var method = new MethodDefinition("HandleClick", MethodAttributes.Public,
                module.TypeSystem.Void);
            baseType.Methods.Add(method);

            GraphDescriptorJSTEmitter.RequirePublicControlHandler(derivedType, "HandleClick",
                new Location("Handler.skin.cshtml", 4, 1)).Should().BeSameAs(method);
        }

        [TestMethod]
        public void NonPublicControlHandlerReportsTemplateLocation()
        {
            var module = ModuleDefinition.CreateModule("Handlers", ModuleKind.Dll);
            var control = new TypeDefinition("Tests", "Control", TypeAttributes.Public);
            module.Types.Add(control);
            control.Methods.Add(new MethodDefinition("HandleClick", MethodAttributes.Private,
                module.TypeSystem.Void));

            Action resolve = () => GraphDescriptorJSTEmitter.RequirePublicControlHandler(
                control, "HandleClick", new Location("Handler.skin.cshtml", 4, 1));

            var error = resolve.Should().Throw<RazorSubControlDiagnosticException>().Which;
            error.Message.Should().Contain("Control.HandleClick");
            error.Location.FileName.Should().Be("Handler.skin.cshtml");
            error.Location.StartLine.Should().Be(4);
        }

        [TestMethod]
        public void DuplicatePartIdReportsSecondControlLocation()
        {
            var first = new SubControlNode { TypeName = "Probe", ElementId = "same",
                Location = new Location("Parent.skin.cshtml", 2, 1) };
            var second = new SubControlNode { TypeName = "Probe", ElementId = "same",
                Location = new Location("Parent.skin.cshtml", 9, 5) };
            var topology = GraphTopologyBuilder.Build(new SkinTemplateNode
            { Children = { first, second } });
            var validateMethod = typeof(RazorSkinJSTGenerator).GetMethod(
                "ValidateUniqueSubControlIds", BindingFlags.Static | BindingFlags.NonPublic);
            validateMethod.Should().NotBeNull();

            Action validate = () => validateMethod.Invoke(null, new object[] { topology.SubControls });

            var error = validate.Should().Throw<TargetInvocationException>().Which.InnerException
                .Should().BeOfType<RazorSubControlDiagnosticException>().Which;
            error.Message.Should().Contain("Duplicate Razor sub-control id 'same'");
            error.Location.FileName.Should().Be("Parent.skin.cshtml");
            error.Location.StartLine.Should().Be(9);
        }

        [TestMethod]
        public void EnumLiteralRespectsTemplateUsingWhenShortNamesCollide()
        {
            var module = ModuleDefinition.CreateModule("EnumResolution", ModuleKind.Dll);
            var other = AddEnum(module, "Other", "Mode", 99);
            var chosen = AddEnum(module, "Chosen", "Mode", 1);

            Action validate = () => RazorSkinJSTGenerator.ValidateEnumLiteral(
                "Mode.Fast", chosen, new[] { other, chosen }, new[] { "Chosen" },
                new Location("Parent.skin.cshtml", 4, 2));

            validate.Should().NotThrow();
        }

        [TestMethod]
        public void AmbiguousEnumLiteralReportsCompileErrorAtControlTag()
        {
            var module = ModuleDefinition.CreateModule("EnumResolution", ModuleKind.Dll);
            var first = AddEnum(module, "First", "Mode", 1);
            var second = AddEnum(module, "Second", "Mode", 99);

            Action validate = () => RazorSkinJSTGenerator.ValidateEnumLiteral(
                "Mode.Fast", first, new[] { first, second }, Array.Empty<string>(),
                new Location("Parent.skin.cshtml", 7, 2));

            var error = validate.Should().Throw<RazorSubControlDiagnosticException>().Which;
            error.Message.Should().Contain("Ambiguous").And.Contain("First.Mode").And.Contain("Second.Mode");
            error.Location.StartLine.Should().Be(7);
        }

        [TestMethod]
        public void UnknownEnumLiteralReportsCompileError()
        {
            var module = ModuleDefinition.CreateModule("EnumResolution", ModuleKind.Dll);
            var target = AddEnum(module, "Chosen", "Mode", 1);

            Action validate = () => RazorSkinJSTGenerator.ValidateEnumLiteral(
                "Missing.Fast", target, new[] { target }, new[] { "Chosen" },
                new Location("Parent.skin.cshtml", 3, 2));

            validate.Should().Throw<RazorSubControlDiagnosticException>()
                .WithMessage("*Unknown enum*Missing*");
        }

        [TestMethod]
        public void SameFullEnumNameInTwoAssembliesReportsAmbiguity()
        {
            var firstModule = ModuleDefinition.CreateModule("FirstEnums", ModuleKind.Dll);
            var secondModule = ModuleDefinition.CreateModule("SecondEnums", ModuleKind.Dll);
            var first = AddEnum(firstModule, "Chosen", "Mode", 1);
            var second = AddEnum(secondModule, "Chosen", "Mode", 99);

            Action validate = () => RazorSkinJSTGenerator.ValidateEnumLiteral(
                "Mode.Fast", first, new[] { first, second }, new[] { "Chosen" },
                new Location("Parent.skin.cshtml", 8, 2));

            validate.Should().Throw<RazorSubControlDiagnosticException>()
                .WithMessage("*Ambiguous*");
        }

        private static TypeDefinition AddEnum(ModuleDefinition module, string ns, string name, int value)
        {
            var type = new TypeDefinition(ns, name, TypeAttributes.Public | TypeAttributes.Sealed,
                module.ImportReference(typeof(Enum)));
            module.Types.Add(type);
            type.Fields.Add(new FieldDefinition("Fast",
                FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal, type)
            { Constant = value });
            return type;
        }

        private static TypeDefinition CreateControl(out ModuleDefinition module)
        {
            module = ModuleDefinition.CreateModule("SubControlFollowup", ModuleKind.Dll);
            var ui = new TypeDefinition("Sunlight.Framework.UI", "UIElement", TypeAttributes.Public);
            module.Types.Add(ui);
            var control = new TypeDefinition("Tests", "Probe", TypeAttributes.Public, ui);
            module.Types.Add(control);
            return control;
        }

        private static void AddElementConstructor(TypeDefinition type, ModuleDefinition module)
        {
            var element = new TypeDefinition("System.Web.Html", "Element", TypeAttributes.Public);
            module.Types.Add(element);
            AddConstructor(type, element);
        }

        private static void AddConstructor(TypeDefinition type, TypeReference argumentType)
        {
            var ctor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName, type.Module.TypeSystem.Void);
            ctor.Parameters.Add(new ParameterDefinition(argumentType));
            type.Methods.Add(ctor);
        }

        private static void AddProperty(TypeDefinition type, string name, TypeReference propertyType,
            bool hasGetter = true, bool twoWayDefault = false, bool hasSetter = true)
        {
            var property = new PropertyDefinition(name, PropertyAttributes.None, propertyType);
            if (hasGetter)
            {
                var getter = new MethodDefinition("get_" + name,
                    MethodAttributes.Public | MethodAttributes.SpecialName, propertyType);
                type.Methods.Add(getter);
                property.GetMethod = getter;
            }
            if (hasSetter)
            {
                var setter = new MethodDefinition("set_" + name,
                    MethodAttributes.Public | MethodAttributes.SpecialName, type.Module.TypeSystem.Void);
                setter.Parameters.Add(new ParameterDefinition(propertyType));
                type.Methods.Add(setter);
                property.SetMethod = setter;
            }
            if (twoWayDefault)
            {
                var attributeType = new TypeReference("Sunlight.Framework.UI.Attributes",
                    "DefaultDataBindingAttribute", type.Module, type.Module);
                var ctor = new MethodReference(".ctor", type.Module.TypeSystem.Void, attributeType)
                { HasThis = true };
                var attribute = new CustomAttribute(ctor);
                attribute.Properties.Add(new CustomAttributeNamedArgument("Mode",
                    new CustomAttributeArgument(type.Module.TypeSystem.Int32, 2)));
                property.CustomAttributes.Add(attribute);
            }
            type.Properties.Add(property);
        }

        private static void AddLiteral(SubControlNode sub, string name, string value)
        {
            sub.PropertyBindings.Add(new SubControlPropertyBinding
            {
                PropertyName = name,
                IsLiteral = true,
                Classification = new BindingClassification { CSharpExpression = value }
            });
        }

        private static void Validate(SubControlNode sub, TypeDefinition type, TypeDefinition modelType = null)
        {
            RazorSkinJSTGenerator.ValidateSubControlTagInfo(sub, type,
                (current, name) => current.Properties.FirstOrDefault(p => p.Name == name), modelType);
        }
    }
}

namespace NScript.CLR.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mono.Cecil;

    /// <summary>
    /// <see cref="TypeHelpers.GetPropertyDefinition"/> indexes a type's properties once. A
    /// watch session adds properties to kept types (ModuleRefresh), and the index must see them.
    /// </summary>
    [TestClass]
    public class PropertyLookupTests
    {
        [TestMethod]
        public void GetPropertyDefinition_PropertyAddedAfterALookup_IsFound()
        {
            var module = ModuleDefinition.CreateModule("Props", ModuleKind.Dll);
            var type = new TypeDefinition("N", "T", TypeAttributes.Public | TypeAttributes.Class, module.TypeSystem.Object);
            module.Types.Add(type);
            var (first, firstGetter, firstSetter) = AddProperty(type, "First");

            Assert.AreSame(first, firstGetter.GetPropertyDefinition());
            Assert.AreSame(first, firstSetter.GetPropertyDefinition());

            var (second, secondGetter, _) = AddProperty(type, "Second");
            Assert.AreSame(second, secondGetter.GetPropertyDefinition(), "the index missed a property added after it was built");

            var plain = new MethodDefinition("NotAnAccessor", MethodAttributes.Public, module.TypeSystem.Void);
            type.Methods.Add(plain);
            Assert.IsNull(plain.GetPropertyDefinition());
        }

        private static (PropertyDefinition property, MethodDefinition getter, MethodDefinition setter) AddProperty(TypeDefinition type, string name)
        {
            var typeSystem = type.Module.TypeSystem;
            var accessor = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            var getter = new MethodDefinition("get_" + name, accessor, typeSystem.Int32);
            var setter = new MethodDefinition("set_" + name, accessor, typeSystem.Void);

            // Cecil resets semantics set on an imageless method until they were read once.
            _ = getter.SemanticsAttributes;
            _ = setter.SemanticsAttributes;
            getter.SemanticsAttributes = MethodSemanticsAttributes.Getter;
            setter.SemanticsAttributes = MethodSemanticsAttributes.Setter;
            setter.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, typeSystem.Int32));
            type.Methods.Add(getter);
            type.Methods.Add(setter);
            var property = new PropertyDefinition(name, PropertyAttributes.None, typeSystem.Int32) { GetMethod = getter, SetMethod = setter };
            type.Properties.Add(property);
            return (property, getter, setter);
        }
    }
}

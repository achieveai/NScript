using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NScript.Template.Compiler
{
    class KnowTypeReferences
    {
        public const string FrameworkAssemblyName = "Sunlight.Framework";
        public const string ControlsAssemblyName = "Sunlight.Controls";

        public static readonly Tuple<string, string> FactoryDelegate =
                Tuple.Create(
                    KnowTypeReferences.ControlsAssemblyName,
                    "Sunlight.Controls.Factory`1");

        public static readonly Tuple<string, string> ControlSignature =
                Tuple.Create(
                    KnowTypeReferences.ControlsAssemblyName,
                    "Sunlight.Controls.Control");

        public static readonly Tuple<string, string> PanelSignature =
        Tuple.Create(
            KnowTypeReferences.ControlsAssemblyName,
            "Sunlight.Controls.Panel");

        public static readonly Tuple<string, string> TemplateSignature =
                Tuple.Create(
                    KnowTypeReferences.ControlsAssemblyName,
                    "Sunlight.Controls.Template");

        public static readonly Tuple<string, string> TemplateInstanceSignature =
                Tuple.Create(
                    KnowTypeReferences.ControlsAssemblyName,
                    "Sunlight.Controls.TemplateInstance");

        public static readonly Tuple<string, string> Func1TypeSignature =
                Tuple.Create(
                    String.Empty,
                    "System.Func`1");

        public static readonly Tuple<string,string> UIObjectSignature =
                Tuple.Create(
                    KnowTypeReferences.FrameworkAssemblyName,
                    "Sunlight.Framework.UIObject");

        public readonly static Tuple<string, string> DataBinderSignature =
                Tuple.Create(
                    KnowTypeReferences.FrameworkAssemblyName,
                    "Sunlight.Framework.Binding.DataBinder");

        public readonly static Tuple<string, string> IValueConverterSignature =
                Tuple.Create(
                    KnowTypeReferences.FrameworkAssemblyName,
                    "Sunlight.Framework.Binding.IValueConverter");

        public readonly static Tuple<string, string> Action2TypeSignature =
                Tuple.Create(
                    String.Empty,
                    "System.Action`2");

        public readonly static Tuple<string, string> Func2TypeSignature =
                Tuple.Create(
                    String.Empty,
                    "System.Func`2");

        public readonly static Tuple<string, string> StringTypeSignature =
                Tuple.Create(
                    String.Empty,
                    "System.String");

        public readonly static Tuple<string, string> IntTypeSignature =
                Tuple.Create(
                    String.Empty,
                    "System.Int32");

        public readonly static Tuple<string, string> BoolTypeSignature =
                Tuple.Create(
                    String.Empty,
                    "System.Boolean");

        public readonly static Tuple<string, string> EnumTypeSignature =
                Tuple.Create(
                    String.Empty,
                    "System.Enum");

        public readonly static Tuple<string, string> ObjectTypeSignature =
                Tuple.Create(
                    String.Empty,
                    "System.Object");

        public readonly static Tuple<string, string> TypeTypeSignature =
                Tuple.Create(
                    String.Empty,
                    "System.Type");

        public readonly static Tuple<string, string> CssClassBinderSignature =
                Tuple.Create(
                    KnowTypeReferences.FrameworkAssemblyName,
                    "Sunlight.Framework.Binding.CssClassBinder");

        public readonly static Tuple<string, string> UIObjectTypeSignature =
                Tuple.Create(
                    KnowTypeReferences.FrameworkAssemblyName,
                    "Sunlight.Framework.UIObject");

        public readonly static Tuple<string, string> DepedencyObjectSignature =
                Tuple.Create(
                    KnowTypeReferences.FrameworkAssemblyName,
                    "Sunlight.Framework.DependencyProperty");

        public readonly static Tuple<string, string> TargetPropertyGetter =
                Tuple.Create(
                    KnowTypeReferences.FrameworkAssemblyName,
                    "Sunlight.Framework.Binding.TargetPropertyGetter");

        public readonly static Tuple<string, string> TargetPropertySetter =
                Tuple.Create(
                    KnowTypeReferences.FrameworkAssemblyName,
                    "Sunlight.Framework.Binding.TargetPropertySetter");

        public readonly static Tuple<string, string> BindingMode =
                Tuple.Create(
                    KnowTypeReferences.FrameworkAssemblyName,
                    "Sunlight.Framework.Binding.BindingMode");

        public readonly static Tuple<string, string> IValueConverter =
                Tuple.Create(
                    KnowTypeReferences.FrameworkAssemblyName,
                    "Sunlight.Framework.Binding.IValueConverter");
    }
}

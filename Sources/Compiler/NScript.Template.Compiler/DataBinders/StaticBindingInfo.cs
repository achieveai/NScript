//-----------------------------------------------------------------------
// <copyright file="StaticBindingInfo.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Template.Compiler.DataBinders
{
    using System;
    using System.Collections.Generic;
    using NScript.PELoader.Reflection;

    /// <summary>
    /// Static Binding info.
    /// </summary>
    public class StaticBindingInfo : DataBindingInfo
    {
        private FieldReference staticField;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticBindingInfo"/> class.
        /// </summary>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="bindingValues">The binding values.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="staticField">Information about the static field</param>
        private StaticBindingInfo(
            BoundProperty targetProperty,
            Tuple<string, string, Dictionary<string, string>> bindingValues,
            TypeInfoResolver resolver,
            FieldReference staticField)
            : base(
                targetProperty,
                bindingValues,
                resolver,
                staticField.FieldDefinition.Type.ApplyGenericArguments(
                    staticField.Parent.Parameters,
                    null))
        {
            this.staticField = staticField;
        }

        /// <summary>
        /// Default binding mode
        /// </summary>
        protected override BindingMode DefaultBindingMode
        {
            get
            {
                return BindingMode.OneTime;
            }
        }

        /// <summary>
        /// Create a StaticBindingInfo
        /// </summary>
        /// <remarks>
        /// This cannot be done via a simple constructor because we need to manipulate some of the values that get
        /// passed to the base constructor.
        /// </remarks>
        /// <param name="targetProperty">Target property</param>
        /// <param name="bindingValues">Binding information</param>
        /// <param name="resolver">Type resolver</param>
        /// <returns>Static binding information</returns>
        public static StaticBindingInfo Create(
            BoundProperty targetProperty,
            Tuple<string, string, Dictionary<string, string>> bindingValues,
            TypeInfoResolver resolver)
        {
            // The static path is of the form:
            // "ns:SomeClass.StaticMember.Property1.Property2"
            string staticPath = bindingValues.Item2;

            // Parse the static path into the static field and the property path
            int startOfPath = staticPath.IndexOf('.', staticPath.IndexOf('.') + 1);
            string staticField;
            string propertyPath;

            if (startOfPath >= 0)
            {
                staticField = staticPath.Substring(0, startOfPath);
                propertyPath = staticPath.Substring(startOfPath + 1);
            }
            else
            {
                staticField = staticPath;
                propertyPath = null;
            }

            // Build new binding values based on the path
            Tuple<string, string, Dictionary<string, string>> newBindingValues =
                new Tuple<string, string, Dictionary<string, string>>(
                    bindingValues.Item1,
                    propertyPath,
                    bindingValues.Item3);

            // Determine the type of the static field
            FieldReference staticFieldInfo = resolver.ResolveStaticField(staticField);

            // Create the binding info
            return new StaticBindingInfo(
                targetProperty,
                newBindingValues,
                resolver,
                staticFieldInfo);
        }

        /// <summary>
        /// Writes the binding.
        /// </summary>
        /// <param name="elementId">The element id.</param>
        /// <param name="writer">The writer.</param>
        public override void WriteBinding(string elementId, System.IO.StreamWriter writer)
        {
            /*
            writer.Write(
                "{0}.{1}(",
                elementId,
                this.TargetProperty.ControlType.GetFunctionInfoUsingArgs(this.GetAddBinderString(), DataBindingInfo.DataBinderType).Slot);

            if (this.TargetProperty.FieldInfo != null)
            {
                throw new ApplicationException("Attached properties do not support static binding.");
            }

            writer.Write(
                "{0}.{1}(",
                TypeManager.GetTypeReferenceId(this.binderTypeReference),
                this.binderTypeReference.Type.GetFunctionInfoUsingArgs(
                    "CreateStaticDataBinder",
                    BindingInfo.ObjectType,
                    BindingInfo.StringType,
                    BindingInfo.StringType,
                    BindingInfo.TargetPropertyGetterType,
                    BindingInfo.TargetPropertySetterType,
                    BindingInfo.BindingModeType,
                    BindingInfo.IValueConverterType,
                    BindingInfo.ObjectType).Slot);

            // Reference to the static object
            writer.Write(
                "{0}.{1},",
                this.staticField.ParentType.Name.Item2,
                this.staticField.Slot);

            // Property path
            if (this.SourcePropertyPath != null)
            {
                writer.Write("\"{0}\",", this.SourcePropertyPath);
            }
            else
            {
                writer.Write("null,");
            }

            // Target property name
            writer.Write("\"{0}\",", this.TargetProperty.PropertyInfo.Name);

            // Target property getter
            writer.Write(
                "function(a1){{return a1.{0}();}},",
                this.TargetProperty.PropertyInfo.Getter.Slot);

            // Target property getter
            writer.Write(
                "function(a1,a2){{a1.{0}(a2);}},",
                this.TargetProperty.PropertyInfo.Setter.Slot);

            // Binding mode
            writer.Write("{0},", this.BindingModeToInt());

            // Converter
            if (this.converterTypeReference != null)
            {
                writer.Write(
                    "new {0}(),",
                    TypeManager.GetTypeReferenceId(this.converterTypeReference));
            }
            else
            {
                writer.Write("null,");
            }

            // Converter parameter
            writer.WriteLine("null));");
             */
        }

        /// <summary>
        /// Gets the end property getter.
        /// </summary>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="getterInfo">The getter info.</param>
        /// <returns>If there is a proeprtyGetter for last element in the path.</returns>
        protected override bool InternalGetEndPropertyGetter(
            string propertyPath,
            TypeReference typeInfo,
            out PropertyReference getterInfo)
        {
            if (String.IsNullOrEmpty(propertyPath))
            {
                getterInfo = null;
                return true;
            }
            else
            {
                return base.InternalGetEndPropertyGetter(propertyPath, typeInfo, out getterInfo);
            }
        }
    }
}

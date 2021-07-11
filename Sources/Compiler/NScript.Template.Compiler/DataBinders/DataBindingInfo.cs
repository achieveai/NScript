//-----------------------------------------------------------------------
// <copyright file="DataBindingInfo.cs" company="Microsoft Corp.">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Template.Compiler.DataBinders
{
    using System;
    using System.Collections.Generic;
    using NScript.Template.Compiler.Attributes;
    using NScript.PELoader.Reflection;

    /// <summary>
    /// DataBinding info.
    /// </summary>
    public class DataBindingInfo : BindingInfo
    {
        /// <summary>
        /// One way string.
        /// </summary>
        private const string OneWayStr = "OneWay";

        /// <summary>
        /// One time string.
        /// </summary>
        private const string OneTimeStr = "OneTime";

        /// <summary>
        /// Two way string
        /// </summary>
        private const string TwoWayStr = "TwoWay";

        /// <summary>
        /// Backing field for mode.
        /// </summary>
        private BindingMode bindingMode = BindingMode.OneWay;

        /// <summary>
        /// Backing field for converter class.
        /// </summary>
        private TypeReference converterClass = null;

        /// <summary>
        /// Backing field for target proeprty.
        /// </summary>
        private BoundProperty targetProperty = null;

        /// <summary>
        /// Backing field for binderTypeReference.
        /// </summary>
        protected TypeReference binderTypeReference = null;

        /// <summary>
        /// Backing field for converterTypeReference.
        /// </summary>
        protected TypeReference converterTypeReference = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBindingInfo"/> class.
        /// </summary>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="bindingValues">The binding values.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="dataContextType">Type of the data context.</param>
        public DataBindingInfo(
            BoundProperty targetProperty,
            Tuple<string, string, Dictionary<string, string>> bindingValues,
            TypeInfoResolver resolver,
            TypeReference dataContextType)
        {
            this.targetProperty = targetProperty;
            this.SourcePropertyPath = bindingValues.Item2;

            BindingAttribute bindingAttribute =
                AttributeBase.GetAttribute<BindingAttribute>(targetProperty.PropertyReference.PropertyDefinition.Attributes);

            if (this.targetProperty.PropertyReference != null &&
                (this.targetProperty.PropertyReference.PropertyDefinition.Getter == null ||
                this.targetProperty.PropertyReference.PropertyDefinition.Setter == null))
            {
                throw new ApplicationException(
                    string.Format(
                        "Target property for the control should have both setter and getter"));
            }

            string converterClass;
            string bindingMode;

            if (bindingValues.Item3.TryGetValue(BindingInfo.ModeStr, out bindingMode))
            {
                if (bindingMode == DataBindingInfo.OneTimeStr)
                {
                    this.bindingMode = BindingMode.OneTime;
                }
                else if (bindingMode == DataBindingInfo.OneWayStr)
                {
                    this.bindingMode = BindingMode.OneWay;
                }
                else if (bindingMode == DataBindingInfo.TwoWayStr)
                {
                    this.bindingMode = BindingMode.TwoWay;
                }
                else
                {
                    throw new ApplicationException(
                        string.Format(
                            "Invalid BindingMode: {0}",
                            this.bindingMode));
                }
            }
            else
            {
                this.bindingMode =
                    bindingAttribute != null
                        ? bindingAttribute.Mode
                        : this.DefaultBindingMode;
            }

            if (bindingAttribute != null && bindingAttribute.IsStrict && this.bindingMode != bindingAttribute.Mode)
            {
                throw new ApplicationException(
                    string.Format(
                        "Invalid BindingMode ({0}) used. Only possible binding mode is {1}",
                        this.bindingMode,
                        bindingAttribute.Mode));
            }

            if (bindingValues.Item3.TryGetValue(BindingInfo.ConverterStr, out converterClass))
            {
                this.converterClass = resolver.ResolveType(
                    converterClass,
                    DefaultSettings.Default.ConvertersNS);

                if (this.converterClass == null)
                {
                    throw new ApplicationException(
                        string.Format(
                            "Can't resolve type {0}",
                            converterClass));
                }
            }

            PropertyReference getter;
            if (!this.InternalGetEndPropertyGetter(
                this.SourcePropertyPath,
                dataContextType,
                out getter))
            {
                throw new ApplicationException(
                    string.Format(
                        "Can't find propertyPath {0} on {1}. Please check the propertyPath.",
                        this.SourcePropertyPath,
                        dataContextType));
            }
        }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        /// <value>The mode  .</value>
        public BindingMode Mode
        {
            get
            {
                return this.bindingMode;
            }
        }

        /// <summary>
        /// Gets the converter.
        /// </summary>
        /// <value>The converter.</value>
        public TypeReference Converter
        {
            get
            {
                return this.converterClass;
            }
        }

        /// <summary>
        /// Gets the target property.
        /// </summary>
        /// <value>The target property.</value>
        public BoundProperty TargetProperty
        {
            get
            {
                return this.targetProperty;
            }
        }

        /// <summary>
        /// Default binding mode
        /// </summary>
        protected virtual BindingMode DefaultBindingMode
        {
            get
            {
                return BindingMode.OneWay;
            }
        }

        /// <summary>
        /// Gets the end property getter.
        /// </summary>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="getterInfo">The getter info.</param>
        /// <returns>If there is a proeprtyGetter for last element in the path.</returns>
        protected virtual bool InternalGetEndPropertyGetter(
            string propertyPath,
            TypeReference typeInfo,
            out PropertyReference getterInfo)
        {
            return DataBindingInfo.GetEndPropertyGetter(propertyPath, typeInfo, out getterInfo);
        }

        /// <summary>
        /// Gets the end property getter.
        /// </summary>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="typeInfo">The type info.</param>
        /// <param name="getterInfo">The getter info.</param>
        /// <returns>If there is a proeprtyGetter for last element in the path.</returns>
        public static bool GetEndPropertyGetter(
            string propertyPath,
            TypeReference typeInfo,
            out PropertyReference getterInfo)
        {
            getterInfo = null;

            if (typeInfo == null)
            {
                return true;
            }

            string[] propertyParts = propertyPath.Split('.');

            for (int partIndex = 0; partIndex < propertyParts.Length; partIndex++)
            {
                PropertyDefinition propertyDefinition = getterInfo.PropertyDefinition;
                typeInfo = propertyDefinition.PropertyType.ApplyGenericArguments(
                    getterInfo.Parent.Parameters,
                    null);

                getterInfo = TypeManager.ResolveProperty(
                    typeInfo,
                    propertyParts[partIndex],
                    true);

                if (getterInfo == null ||
                    propertyDefinition.Getter == null)
                {
                    getterInfo = null;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds to reference.
        /// </summary>
        public override void AddToReference()
        {
            var controlTypeRef = 
                this.TargetProperty.PropertyReference != null
                    ? this.TargetProperty.PropertyReference.Parent
                    : this.TargetProperty.FieldReference.Parent;

            var targetPropertyTypeRef = this.TargetProperty.PropertyType;

            TypeManager.AddTypeReference(controlTypeRef);
            TypeManager.AddTypeReference(targetPropertyTypeRef);

            if (this.Converter != null)
            {
                this.converterTypeReference = new TypeReference(this.Converter);
                TypeManager.AddTypeReference(this.converterTypeReference);
            }

            this.binderTypeReference = 
                new TypeReference(
                    DataBindingInfo.DataBinderType,
                    controlTypeRef,
                    targetPropertyTypeRef);

            TypeManager.AddTypeReference(this.binderTypeReference);
        }

        /// <summary>
        /// Writes the binding.
        /// </summary>
        /// <param name="elementId">The element id.</param>
        /// <param name="writer">The writer.</param>
        public virtual void WriteBinding(string elementId, System.IO.StreamWriter writer)
        {
            writer.Write(
                "{0}.{1}(",
                elementId,
                this.TargetProperty.ControlType.ResolveMethod(
                    this.GetAddBinderString(),
                    false,
                    DataBindingInfo.DataBinderType));

            if (this.TargetProperty.FieldReference != null)
            {
                writer.Write(
                    "{0}.{1}(",
                    TypeManager.GetTypeReferenceId(this.binderTypeReference),
                    this.binderTypeReference.ResolveMethod(
                        "CreateAttachedPropertyDataBinder",
                        false,
                        BindingInfo.StringType,
                        BindingInfo.DependencyPropertyType,
                        BindingInfo.BindingModeType,
                        BindingInfo.IValueConverterType,
                        BindingInfo.ObjectType));

                writer.Write("\"{0}\",", this.SourcePropertyPath);
                writer.Write("{0}.{1},",
                        TypeManager.GetTypeReferenceId(new TypeReference(this.TargetProperty.FieldReference.Parent)),
                        this.TargetProperty.FieldReference);
            }
            else
            {
                writer.Write(
                    "{0}.{1}(",
                    TypeManager.GetTypeReferenceId(this.binderTypeReference),
                    this.binderTypeReference.ResolveMethod(
                        "CreatePropertyDataBinder",
                        false,
                        BindingInfo.StringType,
                        BindingInfo.StringType,
                        BindingInfo.TargetPropertyGetterType,
                        BindingInfo.TargetPropertySetterType,
                        BindingInfo.BindingModeType,
                        BindingInfo.IValueConverterType,
                        BindingInfo.ObjectType));

                writer.Write("\"{0}\",", this.SourcePropertyPath);
                writer.Write("\"{0}\",", this.TargetProperty.PropertyReference.Name);

                writer.Write(
                    "function(a1){{return a1.{0}();}},",
                    this.TargetProperty.PropertyReference.PropertyDefinition.Getter);

                writer.Write(
                    "function(a1,a2){{a1.{0}(a2);}},",
                    this.TargetProperty.PropertyReference.PropertyDefinition.Setter);
            }

            writer.Write("{0},", this.BindingModeToInt());
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

            writer.WriteLine("null));");
        }

        /// <summary>
        /// Gets the add binder string.
        /// </summary>
        /// <returns>AddBinder string.</returns>
        protected virtual string GetAddBinderString()
        {
            return "AddDataBinder";
        }

        /// <summary>
        /// Bindings the mode to int.
        /// </summary>
        /// <returns>Converts binding mode to integer.</returns>
        protected int BindingModeToInt()
        {
            switch (this.bindingMode)
            {
                case BindingMode.OneWay:
                    return 1;
                case BindingMode.TwoWay:
                    return 2;
                case BindingMode.OneTime:
                    return 0;
                default:
                    throw new InvalidOperationException("Invalid BindingMode");
            }
        }
    }
}

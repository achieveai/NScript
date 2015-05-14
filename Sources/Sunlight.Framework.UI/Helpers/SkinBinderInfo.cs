//-----------------------------------------------------------------------
// <copyright file="SkinBinderInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using Sunlight.Framework.Binders;
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.Html;

    [Flags]
    public enum BinderType
    {
        DataContext = 0x1,
        Static = 0x2,
        TemplateParent = 0x3,
        TargetTypes = 0x7,
        PropertyBinder = 0x10,
        AttachedPropertyBinder = 0x20,
        EventBinder = 0x30,
        DomEventBinder = 0x40,
        CssBinder = 0x50,
        StyleBinder = 0x60,
        AttributeBinder = 0x70,
        PropertyTypes = 0xf0
    }

    /// <summary>
    /// Definition for SkinBinderInfo
    /// </summary>
    public sealed class SkinBinderInfo
    {
        /// <summary>
        /// Full pathname of the property getter file.
        /// </summary>
        public readonly NativeArray<Func<object, object>> PropertyGetterPath;

        /// <summary>
        /// The property setter.
        /// </summary>
        public readonly Action<object, object> PropertySetter;

        /// <summary>
        /// List of names of the properties.
        /// </summary>
        public readonly NativeArray<string> PropertyNames;

        /// <summary>
        /// Target property setter.
        /// </summary>
        public readonly Action<object, object> TargetPropertySetter;

        /// <summary>
        /// Target property getter.
        /// </summary>
        public readonly Func<object, object> TargetPropertyGetter;

        /// <summary>
        /// Target property setter with argument.
        /// </summary>
        public readonly Action<object, object, object> TargetPropertySetterWithArg;

        /// <summary>
        /// Target property setter argument.
        /// This is mainly used for things like
        /// Attribute Binding, Style Binding, Css Binding, etc.
        /// </summary>
        public readonly object TargetPropertySetterArg;

        /// <summary>
        /// Name of the target property.
        /// </summary>
        public readonly string TargetPropertyName;

        /// <summary>
        /// true if this object is data context binder.
        /// </summary>
        public readonly BinderType BinderType;

        /// <summary>
        /// Zero-based index of the object.
        /// </summary>
        public readonly int ObjectIndex;

        /// <summary>
        /// Zero-based index of the binder.
        /// </summary>
        public readonly int BinderIndex = -1;

        /// <summary>
        /// Zero-based index of the extra object.
        /// </summary>
        public readonly int ExtraObjectIndex = -1;

        /// <summary>
        /// The forward converter.
        /// </summary>
        public readonly Func<object, object> ForwardConverter;

        /// <summary>
        /// The backward converter.
        /// </summary>
        public readonly Func<object, object> BackwardConverter;

        /// <summary>
        /// The default value.
        /// </summary>
        public readonly object DefaultValue;

        /// <summary>
        /// The mode.
        /// </summary>
        public readonly DataBindingMode Mode;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyGetterPath">   Full pathname of the property getter file. </param>
        /// <param name="targetPropertySetter"> Target property setter. </param>
        /// <param name="binderType">  true if this object is data context binder. </param>
        /// <param name="objectIndex">          Zero-based index of the object. </param>
        /// <param name="forwardConverter">     The forward converter. </param>
        /// <param name="defaultValue">         The default value. </param>
        public SkinBinderInfo(
            NativeArray<Func<object, object>> propertyGetterPath,
            Action<object, object> targetPropertySetter,
            BinderType binderType,
            int objectIndex,
            Func<object, object> forwardConverter,
            object defaultValue)
        {
            this.PropertyGetterPath = propertyGetterPath;
            this.TargetPropertySetter = targetPropertySetter;
            this.BinderType = binderType;
            this.ObjectIndex = objectIndex;
            this.ForwardConverter = forwardConverter;
            this.DefaultValue = defaultValue;
            this.Mode = DataBindingMode.OneTime;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyGetterPath">   Full pathname of the property getter file. </param>
        /// <param name="targetPropertySetter"> Target property setter. </param>
        /// <param name="isDataContextBinder">  true if this object is data context binder. </param>
        /// <param name="objectIndex">          Zero-based index of the object. </param>
        /// <param name="forwardConverter">     The forward converter. </param>
        /// <param name="defaultValue">         The default value. </param>
        public SkinBinderInfo(
            NativeArray<Func<object, object>> propertyGetterPath,
            Action<object, object, object> targetPropertySetterWithArg,
            object targetPropertySetterArg,
            BinderType binderType,
            int objectIndex,
            Func<object, object> forwardConverter,
            object defaultValue,
            int extraObjectIndex)
        {
            this.PropertyGetterPath = propertyGetterPath;
            this.TargetPropertySetterArg = targetPropertySetterArg;
            this.TargetPropertySetterWithArg = targetPropertySetterWithArg;
            this.BinderType = binderType;
            this.ObjectIndex = objectIndex;
            this.ForwardConverter = forwardConverter;
            this.DefaultValue = defaultValue;
            this.Mode = DataBindingMode.OneTime;
            this.ExtraObjectIndex = extraObjectIndex;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyGetterPath">   Full pathname of the property getter file. </param>
        /// <param name="propertyNames">        List of names of the properties. </param>
        /// <param name="targetPropertySetter"> Target property setter. </param>
        /// <param name="binderType">  true if this object is data context binder. </param>
        /// <param name="objectIndex">          Zero-based index of the object. </param>
        /// <param name="forwardConverter">     The forward converter. </param>
        /// <param name="defaultValue">         The default value. </param>
        /// <param name="mode">                 The mode. </param>
        public SkinBinderInfo(
            NativeArray<Func<object, object>> propertyGetterPath,
            NativeArray<string> propertyNames,
            Action<object, object> targetPropertySetter,
            BinderType binderType,
            int objectIndex,
            int binderIndex,
            Func<object, object> forwardConverter,
            object defaultValue)
        {
            this.PropertyGetterPath = propertyGetterPath;
            this.PropertyNames = propertyNames;
            this.TargetPropertySetter = targetPropertySetter;
            this.BinderType = binderType;
            this.ObjectIndex = objectIndex;
            this.ForwardConverter = forwardConverter;
            this.DefaultValue = defaultValue;
            this.Mode = DataBindingMode.OneWay;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyGetterPath">   Full pathname of the property getter file. </param>
        /// <param name="propertyNames">        List of names of the properties. </param>
        /// <param name="targetPropertySetter"> Target property setter. </param>
        /// <param name="binderType">  true if this object is data context binder. </param>
        /// <param name="objectIndex">          Zero-based index of the object. </param>
        /// <param name="forwardConverter">     The forward converter. </param>
        /// <param name="defaultValue">         The default value. </param>
        /// <param name="mode">                 The mode. </param>
        public SkinBinderInfo(
            NativeArray<Func<object, object>> propertyGetterPath,
            NativeArray<string> propertyNames,
            Action<object, object, object> targetPropertySetter,
            object targetPropertySetterArg,
            BinderType binderType,
            int objectIndex,
            int binderIndex,
            Func<object, object> forwardConverter,
            object defaultValue,
            int extraObjectIndex)
        {
            this.PropertyGetterPath = propertyGetterPath;
            this.PropertyNames = propertyNames;
            this.TargetPropertySetterWithArg = targetPropertySetter;
            this.TargetPropertySetterArg = targetPropertySetterArg;
            this.BinderType = binderType;
            this.BinderIndex = binderIndex;
            this.ObjectIndex = objectIndex;
            this.ForwardConverter = forwardConverter;
            this.DefaultValue = defaultValue;
            this.Mode = DataBindingMode.OneWay;
            this.ExtraObjectIndex = extraObjectIndex;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyGetterPath">   Full pathname of the property getter file. </param>
        /// <param name="propertySetter">       The property setter. </param>
        /// <param name="propertyNames">        List of names of the properties. </param>
        /// <param name="targetPropertySetter"> Target property setter. </param>
        /// <param name="targetPropertyGetter"> Target property getter. </param>
        /// <param name="targetPropertyName">   Name of the target property. </param>
        /// <param name="binderType">  true if this object is data context binder. </param>
        /// <param name="objectIndex">          Zero-based index of the object. </param>
        /// <param name="forwardConverter">     The forward converter. </param>
        /// <param name="backwardConverter">    The backward converter. </param>
        /// <param name="defaultValue">         The default value. </param>
        public SkinBinderInfo(
            NativeArray<Func<object, object>> propertyGetterPath,
            Action<object, object> propertySetter,
            NativeArray<string> propertyNames,
            Action<object, object> targetPropertySetter,
            Func<object,object> targetPropertyGetter,
            string targetPropertyName,
            BinderType binderType,
            int objectIndex,
            int binderIndex,
            Func<object, object> forwardConverter,
            Func<object, object> backwardConverter,
            object defaultValue)
        {
            this.PropertyGetterPath = propertyGetterPath;
            this.PropertySetter = propertySetter;
            this.PropertyNames = propertyNames;
            this.TargetPropertySetter = targetPropertySetter;
            this.TargetPropertyGetter = targetPropertyGetter;
            this.TargetPropertyName = targetPropertyName;
            this.BinderType = binderType;
            this.ObjectIndex = objectIndex;
            this.BinderIndex = binderIndex;
            this.ForwardConverter = forwardConverter;
            this.BackwardConverter = backwardConverter;
            this.DefaultValue = defaultValue;
            this.Mode = DataBindingMode.TwoWay;
        }

        public void SetTargetValue(
            object target,
            object value,
            NativeArray extraObjectArray)
        {
                var binderInfo = this;
                var propertySetterMode = binderInfo.BinderType & BinderType.PropertyTypes;
                if (propertySetterMode == BinderType.PropertyBinder)
                {
                    binderInfo.TargetPropertySetter(
                        target,
                        value);
                }
                else if (propertySetterMode == BinderType.EventBinder)
                {
                    if (extraObjectArray[binderInfo.ExtraObjectIndex] == value)
                    {
                        return;
                    }

                    if (extraObjectArray[binderInfo.ExtraObjectIndex] != null)
                    {
                        this.TargetPropertySetterWithArg(
                            target,
                            extraObjectArray[binderInfo.ExtraObjectIndex],
                            (BooleanNative)false);
                    }

                    extraObjectArray[binderInfo.ExtraObjectIndex] = value;

                    if (value != null)
                    {
                        this.TargetPropertySetterWithArg(
                            target,
                            value,
                            (BooleanNative)true);
                    }
                }
                else if (propertySetterMode == BinderType.DomEventBinder)
                {
                    Element element = (Element)target;
                    if (extraObjectArray[binderInfo.ExtraObjectIndex] == value)
                    {
                        return;
                    }

                    if (!object.IsNullOrUndefined(extraObjectArray[binderInfo.ExtraObjectIndex]))
                    {
                        element.UnBind(
                            (string)binderInfo.TargetPropertySetterArg,
                            (Action<Element, ElementEvent>)extraObjectArray[binderInfo.ExtraObjectIndex],
                            false);
                    }

                    extraObjectArray[binderInfo.ExtraObjectIndex] = value;

                    if (!object.IsNullOrUndefined(value))
                    {
                        element.Bind(
                            (string)binderInfo.TargetPropertySetterArg,
                            (Action<Element, ElementEvent>)value,
                            false);
                    }
                }
                else if (binderInfo.TargetPropertySetter != null)
                {
                    binderInfo.TargetPropertySetter(
                        target,
                        value);
                }
                else
                {
                    binderInfo.TargetPropertySetterWithArg(
                        target,
                        value,
                        binderInfo.TargetPropertySetterArg);
                }
        }
    }
}

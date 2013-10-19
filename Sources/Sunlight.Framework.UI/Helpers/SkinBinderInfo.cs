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
        public readonly bool IsDataContextBinder;

        /// <summary>
        /// Zero-based index of the object.
        /// </summary>
        public readonly int ObjectIndex;

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
        /// <param name="isDataContextBinder">  true if this object is data context binder. </param>
        /// <param name="objectIndex">          Zero-based index of the object. </param>
        /// <param name="forwardConverter">     The forward converter. </param>
        /// <param name="defaultValue">         The default value. </param>
        public SkinBinderInfo(
            NativeArray<Func<object, object>> propertyGetterPath,
            Action<object, object> targetPropertySetter,
            bool isDataContextBinder,
            int objectIndex,
            Func<object, object> forwardConverter,
            object defaultValue)
        {
            this.PropertyGetterPath = propertyGetterPath;
            this.TargetPropertySetter = targetPropertySetter;
            this.IsDataContextBinder = isDataContextBinder;
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
            Action<object, object, object> targetPropertySetter,
            object targetPropertySetterArg,
            bool isDataContextBinder,
            int objectIndex,
            Func<object, object> forwardConverter,
            object defaultValue)
        {
            this.PropertyGetterPath = propertyGetterPath;
            this.TargetPropertySetterWithArg = targetPropertySetter;
            this.TargetPropertySetterArg = targetPropertySetterArg;
            this.IsDataContextBinder = isDataContextBinder;
            this.ObjectIndex = objectIndex;
            this.ForwardConverter = forwardConverter;
            this.DefaultValue = defaultValue;
            this.Mode = DataBindingMode.OneTime;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyGetterPath">   Full pathname of the property getter file. </param>
        /// <param name="propertyNames">        List of names of the properties. </param>
        /// <param name="targetPropertySetter"> Target property setter. </param>
        /// <param name="isDataContextBinder">  true if this object is data context binder. </param>
        /// <param name="objectIndex">          Zero-based index of the object. </param>
        /// <param name="forwardConverter">     The forward converter. </param>
        /// <param name="defaultValue">         The default value. </param>
        /// <param name="mode">                 The mode. </param>
        public SkinBinderInfo(
            NativeArray<Func<object, object>> propertyGetterPath,
            NativeArray<string> propertyNames,
            Action<object, object> targetPropertySetter,
            bool isDataContextBinder,
            int objectIndex,
            Func<object, object> forwardConverter,
            object defaultValue)
        {
            this.PropertyGetterPath = propertyGetterPath;
            this.PropertyNames = propertyNames;
            this.TargetPropertySetter = targetPropertySetter;
            this.IsDataContextBinder = isDataContextBinder;
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
        /// <param name="isDataContextBinder">  true if this object is data context binder. </param>
        /// <param name="objectIndex">          Zero-based index of the object. </param>
        /// <param name="forwardConverter">     The forward converter. </param>
        /// <param name="defaultValue">         The default value. </param>
        /// <param name="mode">                 The mode. </param>
        public SkinBinderInfo(
            NativeArray<Func<object, object>> propertyGetterPath,
            NativeArray<string> propertyNames,
            Action<object, object, object> targetPropertySetter,
            object targetPropertySetterArg,
            bool isDataContextBinder,
            int objectIndex,
            Func<object, object> forwardConverter,
            object defaultValue)
        {
            this.PropertyGetterPath = propertyGetterPath;
            this.PropertyNames = propertyNames;
            this.TargetPropertySetterWithArg = targetPropertySetter;
            this.TargetPropertySetterArg = targetPropertySetterArg;
            this.IsDataContextBinder = isDataContextBinder;
            this.ObjectIndex = objectIndex;
            this.ForwardConverter = forwardConverter;
            this.DefaultValue = defaultValue;
            this.Mode = DataBindingMode.OneWay;
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
        /// <param name="isDataContextBinder">  true if this object is data context binder. </param>
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
            bool isDataContextBinder,
            int objectIndex,
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
            this.IsDataContextBinder = isDataContextBinder;
            this.ObjectIndex = objectIndex;
            this.ForwardConverter = forwardConverter;
            this.BackwardConverter = backwardConverter;
            this.DefaultValue = defaultValue;
            this.Mode = DataBindingMode.TwoWay;
        }
    }
}

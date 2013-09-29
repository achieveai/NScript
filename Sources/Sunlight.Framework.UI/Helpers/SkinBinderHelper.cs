//-----------------------------------------------------------------------
// <copyright file="SkinBinderHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using Sunlight.Framework.Binders;
    using System;

    /// <summary>
    /// Definition for SkinBinderHelper
    /// </summary>
    public static class SkinBinderHelper
    {
        /// <summary>
        /// Bind data context.
        /// </summary>
        /// <param name="binders">        The binders. </param>
        /// <param name="dataContext">    Context for the data. </param>
        /// <param name="targetElements"> Target elements. </param>
        public static void BindDataContext(
            NativeArray<SkinBinderInfo> binders,
            Object dataContext,
            NativeArray<object> targetElements)
        {
            for (int i = 0, j = binders.Length; i < j; i++)
            {
                SkinBinderInfo info = binders[i];
                if (info.IsDataContextBinder)
                {
                    SkinBinderHelper.SetPropertyValue(
                        info,
                        dataContext,
                        targetElements[info.ObjectIndex]);
                }
            }
        }

        /// <summary>
        /// Bind template parent.
        /// </summary>
        /// <param name="binders">        The binders. </param>
        /// <param name="templateParent"> The template parent. </param>
        /// <param name="targetElements"> Target elements. </param>
        public static void BindTemplateParent(
            NativeArray<SkinBinderInfo> binders,
            UISkinableElement templateParent,
            NativeArray<object> targetElements)
        {
            for (int i = 0, j = binders.Length; i < j; i++)
            {
                SkinBinderInfo info = binders[i];
                if (info.IsDataContextBinder)
                {
                    SkinBinderHelper.SetPropertyValue(
                        info,
                        templateParent,
                        targetElements[info.ObjectIndex]);
                }
            }
        }

        /// <summary>
        /// Sets property value.
        /// </summary>
        /// <param name="binder"> The binder. </param>
        /// <param name="source"> Source for the. </param>
        /// <param name="target"> Target for the. </param>
        private static void SetPropertyValue(
            SkinBinderInfo binder,
            object source,
            object target)
        {
            try
            {
                source = SkinBinderHelper.TraversePropertyPath(
                    binder,
                    source);
            }
            catch
            {
                source = binder.DefaultValue;
            }

            binder.TargetPropertySetter(target, source);
        }

        /// <summary>
        /// Traverse property path.
        /// </summary>
        /// <param name="binder"> The binder. </param>
        /// <param name="source"> Source for the. </param>
        /// <returns>
        /// .
        /// </returns>
        private static object TraversePropertyPath(
            SkinBinderInfo binder,
            object source)
        {
            for (int i = 0, j = binder.PropertyGetterPath.Length; i < j; i++)
            {
                source = binder.PropertyGetterPath[i](source);
            }

            if (binder.ForwardConverter != null)
            {
                source = binder.ForwardConverter(source);
            }

            return source;
        }
    }
}

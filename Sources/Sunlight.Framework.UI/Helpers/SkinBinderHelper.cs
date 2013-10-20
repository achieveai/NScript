//-----------------------------------------------------------------------
// <copyright file="SkinBinderHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using Sunlight.Framework.Binders;
    using System;
    using System.Web.Html;

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
        /// Sets an attribute.
        /// </summary>
        /// <param name="node">     The node. </param>
        /// <param name="value">    The value. </param>
        /// <param name="attrName"> Name of the attribute. </param>
        public static void SetAttribute(
            Node node,
            string value,
            string attrName)
        {
            if (value != null)
            {
                node.SetAttribute(attrName, value);
            }
            else
            {
                node.RemoveAttribute(attrName);
            }
        }

        /// <summary>
        /// Sets text content.
        /// </summary>
        /// <param name="element"> The element. </param>
        /// <param name="value">   The value. </param>
        public static void SetTextContent(
            Element element,
            string value)
        {
            if (value != null)
            {
                element.TextContent = value;
            }
            else
            {
                element.TextContent = String.Empty;
            }
        }

        /// <summary>
        /// Sets CSS class.
        /// </summary>
        /// <param name="element">   The element. </param>
        /// <param name="add">       true to add. </param>
        /// <param name="className"> Name of the class. </param>
        public static void SetCssClass(
            Element element,
            bool add,
            string className)
        {
            if (add)
            {
                element.AddClassName(className);
            }
            else
            {
                element.RemoveClassName(className);
            }
        }

        public static Element GetElementFromPath(
            Element element,
            NativeArray<int> path)
        {
            for (int iPath = 0; iPath < path.Length; iPath++)
            {
                element = (Element)element.ChildNodes[path[iPath]];
            }

            return element;
        }

        public static void InitializeTemplateDocumentStorage(
            Document document,
            string css,
            int index)
        {
            var style = document.CreateElement("style");
            style.TextContent = css;
            document.Body.AppendChild(style);
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

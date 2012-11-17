//-----------------------------------------------------------------------
// <copyright file="SkinInstance.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Html;

    /// <summary>
    /// Definition for SkinInstance
    /// </summary>
    public class SkinInstance : IDisposable
    {
        /// <summary>
        /// The child elements.
        /// </summary>
        private UIElement[] childElements;

        /// <summary>
        /// The root element.
        /// </summary>
        private Element rootElement;

        /// <summary>
        /// The child identifier mappings.
        /// </summary>
        private StringDictionary<UIElement> childIdMappings = new StringDictionary<UIElement>();

        /// <summary>
        /// Type of the skinable.
        /// </summary>
        private Type skinableType;

        /// <summary>
        /// Type of the data context.
        /// </summary>
        private Type dataContextType;

        /// <summary>
        /// true if this object is active.
        /// </summary>
        bool isActive;

        /// <summary>
        /// true if this object is diposed.
        /// </summary>
        bool isDiposed;

        /// <summary>   Initializes a new instance of the SkinInstance class. </summary>
        /// <param name="rootElement">      The root element. </param>
        /// <param name="childElements">    The child elements. </param>
        /// <param name="skinableType">     Type of the skinable. </param>
        /// <param name="dataContextType">  Type of the data context. </param>
        public SkinInstance(
            Element rootElement,
            UIElement[] childElements,
            Type skinableType,
            Type dataContextType)
        {
            ExceptionHelpers.IsNullOrUndefined(rootElement);

            this.rootElement = rootElement;
            this.childElements = childElements;
            this.skinableType = skinableType;
            this.dataContextType = dataContextType;
        }

        /// <summary>   Registers the child by identifier. </summary>
        /// <param name="id">           The identifier. </param>
        /// <param name="childIndex">   Zero-based index of the child. </param>
        public void RegisterChildById(string id, int childIndex)
        {
            this.childIdMappings.Add(id, this.childElements[childIndex]);
        }

        /// <summary>   Gets a child by identifier. </summary>
        /// <param name="id">   The identifier. </param>
        /// <returns>   The child by identifier. </returns>
        public UIElement GetChildById(string id)
        {
            if (this.childIdMappings.ContainsKey(id))
            {
                return this.childIdMappings[id];
            }

            return null;
        }

        /// <summary>   Gets a child by index. </summary>
        /// <param name="index">    Zero-based index of the. </param>
        /// <returns>   The child by index. </returns>
        public UIElement GetChildByIndex(int index)
        {
            return this.childElements[index];
        }

        /// <summary>   Binds the given UISkinableElement. </summary>
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        /// <param name="skinable"> The skinable. </param>
        public void Bind(UISkinableElement skinable)
        {
            if (this.rootElement == null || this.isDiposed)
            {
                throw new Exception("InvalidOperation, Skin already applied");
            }

            if (!this.skinableType.IsInstanceOfType(skinable))
            {
                throw new Exception("Skin being applied to wrong Skinable");
            }

            if (this.rootElement != skinable.Element)
            {
                var list = this.rootElement.ChildNodes;

                for (int i = 0, j = this.childElements.Length; i < j; i++)
                {
                    this.childElements[i].Parent = skinable;
                    this.childElements[i].PreActivate();
                }

                for (int i = 0, j = list.Length; i < j; i++)
                {
                    skinable.Element.AppendChild(list[i].As<Element>());
                }
            }
        }

        /// <summary>
        /// Activates all the childElements.
        /// </summary>
        public void Activate()
        {
            if (!this.isActive && !this.isDiposed)
            {
                this.isActive = true;
                for (int i = 0, j = this.childElements.Length; i < j; i++)
                {
                    this.childElements[i].Activate();
                }
            }
        }

        /// <summary>
        /// Deactivates all the childElements.
        /// </summary>
        public void Deactivate()
        {
            if (this.isActive && !this.isDiposed)
            {
                this.isActive = false;
                for (int i = 0, j = this.childElements.Length; i < j; i++)
                {
                    this.childElements[i].Deactivate();
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDiposed)
            {
                this.isDiposed = true;
                for (int i = 0, j = this.childElements.Length; i < j; i++)
                {
                    this.childElements[i].Dispose();
                }
            }
        }
    }
}

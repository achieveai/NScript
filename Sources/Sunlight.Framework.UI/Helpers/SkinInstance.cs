//-----------------------------------------------------------------------
// <copyright file="SkinInstance.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using Sunlight.Framework.Binders;
    using System;
    using System.Collections.Generic;
    using System.Web.Html;

    /// <summary>
    /// Definition for SkinInstance
    /// </summary>
    public class SkinInstance : IDisposable
    {
        /// <summary>
        /// The parent factory.
        /// </summary>
        Skin parentFactory;

        /// <summary>
        /// The child elements.
        /// </summary>
        private NativeArray<object> elementsOfIntrest;

        /// <summary>
        /// The child elements.
        /// </summary>
        private NativeArray<UIElement> childElements;

        /// <summary>
        /// The root element.
        /// </summary>
        private Element rootElement;

        /// <summary>
        /// The child identifier mappings.
        /// </summary>
        private StringDictionary<UIElement> childIdMappings = new StringDictionary<UIElement>();

        /// <summary>
        /// true if this object is active.
        /// </summary>
        bool isActive;

        /// <summary>
        /// true if this object is diposed.
        /// </summary>
        bool isDiposed;

        /// <summary>
        /// The binders.
        /// </summary>
        private NativeArray<SkinBinderInfo> binders;

        /// <summary>
        /// The live binders.
        /// </summary>
        private NativeArray liveBinders;

        /// <summary>
        /// The extra objects.
        /// </summary>
        private NativeArray extraObjects;

        /// <summary>
        /// The part identifier mapping.
        /// </summary>
        private StringDictionary<int> partIdMapping;

        /// <summary>
        /// Initializes a new instance of the SkinInstance class.
        /// </summary>
        /// <param name="factory">             The factory. </param>
        /// <param name="rootElement">         The root element. </param>
        /// <param name="elementsOfIntrests">  The child elements. </param>
        /// <param name="binders">             Type of the skinable. </param>
        /// <param name="dataContextType"> Type of the data context. </param>
        public SkinInstance(
            Skin factory,
            Element rootElement,
            NativeArray<UIElement> childElements,
            NativeArray<object> elementsOfIntrests,
            NativeArray<SkinBinderInfo> binders,
            Object partIdMapping,
            int liveBinderCount,
            int extraObjectCount)
        {
            ExceptionHelpers.IsNullOrUndefined(rootElement);

            this.parentFactory = factory;
            this.rootElement = rootElement;
            this.binders = binders;
            this.childElements = childElements;
            this.elementsOfIntrest = elementsOfIntrests;

            if (liveBinderCount > 0)
            { this.liveBinders = new NativeArray(liveBinderCount); }

            if (extraObjectCount > 0)
            { this.extraObjects = new NativeArray(extraObjectCount); }

            if (partIdMapping != null)
            { this.partIdMapping = new StringDictionary<int>(partIdMapping); }
        }

        /// <summary>   Registers the child by identifier. </summary>
        /// <param name="id">           The identifier. </param>
        /// <param name="childIndex">   Zero-based index of the child. </param>
        public void RegisterChildById(string id, int childIndex)
        {
            this.childIdMappings.Add(id, (UIElement)this.elementsOfIntrest[childIndex]);
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

        /// <summary>
        /// Binds the given UISkinableElement.
        /// </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        /// <param name="skinable">          The skinable. </param>
        /// <param name="dataContextParent"> The data context parent. </param>
        public void Bind(UISkinableElement skinable)
        {
            if (this.rootElement == null || this.isDiposed)
            {
                throw new Exception("InvalidOperation, Skin already applied");
            }

            if (!this.parentFactory.SkinableType.IsInstanceOfType(skinable))
            {
                throw new Exception("Skin being applied to wrong Skinable");
            }

            if (this.rootElement != skinable.Element)
            {
                SkinBinderHelper.BindTemplateParent(
                    this.binders,
                    skinable,
                    this.elementsOfIntrest);
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
                for (int i = 0, j = this.elementsOfIntrest.Length; i < j; i++)
                {
                    this.childElements[i].Dispose();
                }
            }
        }

        internal void UpdateDataContext()
        {
        }
    }
}

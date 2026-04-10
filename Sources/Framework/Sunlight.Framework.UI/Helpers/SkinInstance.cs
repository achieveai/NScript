//-----------------------------------------------------------------------
// <copyright file="SkinInstance.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using Sunlight.Framework.Binders;
    using Sunlight.Framework.UI.Helpers.BindingGraph;
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
        private NativeArray elementsOfIntrest;

        /// <summary>
        /// The child elements.
        /// </summary>
        private NativeArray<int> childElements;

        /// <summary>
        /// The root element.
        /// </summary>
        private Element rootElement;

        /// <summary>
        /// true if this object is active.
        /// </summary>
        bool isActive;

        /// <summary>
        /// true if this object is diposed.
        /// </summary>
        bool isDiposed;

        /// <summary>
        /// The binding strategy (legacy XWML or graph-based Razor).
        /// </summary>
        private IBindingStrategy bindingStrategy;

        /// <summary>
        /// true if using graph-based binding mode.
        /// </summary>
        private bool isGraphMode;

        /// <summary>
        /// The has data context binding.
        /// </summary>
        private NativeArray<bool> hasDataContextBinding;

        /// <summary>
        /// The part identifier mapping.
        /// </summary>
        private StringDictionary<int> partIdMapping;

        /// <summary>
        /// The skinable parent.
        /// </summary>
        private UISkinableElement skinableParent;

        /// <summary>
        /// Context for the data.
        /// </summary>
        private object dataContext;

        /// <summary>
        /// true to first activation done.
        /// </summary>
        private bool firstActivationDone;

        /// <summary>
        /// true if data context updated.
        /// </summary>
        private bool dataContextUpdated;

        /// <summary>
        /// true if template parent updated.
        /// </summary>
        private bool templateParentUpdated;

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
            NativeArray<int> childElements,
            NativeArray elementsOfIntrests,
            NativeArray<SkinBinderInfo> binders,
            Object partIdMapping,
            int liveBinderCount,
            int extraObjectCount)
        {
            ExceptionHelpers.IsNullOrUndefined(rootElement);

            this.parentFactory = factory;
            this.rootElement = rootElement;
            this.childElements = childElements;
            this.elementsOfIntrest = elementsOfIntrests;
            this.dataContextUpdated = true;
            this.templateParentUpdated = true;

            this.isGraphMode = false;
            var legacy = new LegacyBinderStrategy(binders, liveBinderCount, extraObjectCount, elementsOfIntrests.Length);
            this.bindingStrategy = legacy;
            this.hasDataContextBinding = legacy.HasDataContextBinding;

            if (partIdMapping != null)
            { this.partIdMapping = new StringDictionary<int>(partIdMapping); }
        }

        /// <summary>
        /// Initializes a new instance of the SkinInstance class for graph-based (Razor) skins.
        /// </summary>
        public SkinInstance(
            Skin factory,
            Element rootElement,
            NativeArray<int> childElements,
            NativeArray elementsOfIntrests,
            GraphDescriptor graphDescriptor,
            Object partIdMapping,
            int unused1,
            int unused2)
        {
            ExceptionHelpers.IsNullOrUndefined(rootElement);

            this.parentFactory = factory;
            this.rootElement = rootElement;
            this.childElements = childElements;
            this.elementsOfIntrest = elementsOfIntrests;
            this.dataContextUpdated = true;
            this.templateParentUpdated = true;

            this.isGraphMode = true;
            this.bindingStrategy = new GraphBindingStrategy(graphDescriptor, elementsOfIntrests, 0);
            this.hasDataContextBinding = new NativeArray<bool>(elementsOfIntrests.Length);

            if (partIdMapping != null)
            { this.partIdMapping = new StringDictionary<int>(partIdMapping); }
        }

        /// <summary>   Gets a child by identifier. </summary>
        /// <param name="id">   The identifier. </param>
        /// <returns>   The child by identifier. </returns>
        public object GetChildById(string id)
        {
            if (this.partIdMapping != null
                && this.partIdMapping.ContainsKey(id))
            {
                return this.elementsOfIntrest[this.partIdMapping[id]];
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

            if (this.skinableParent == skinable)
            {
                return;
            }

            if (this.skinableParent != null)
            {
                var childNodes = this.skinableParent.Element.ChildNodes;
                while(childNodes.Length > 0)
                {
                    this.rootElement.AppendChild(childNodes[0]);
                }
            }

            this.skinableParent = skinable;

            if (this.skinableParent != null)
            {
                var childNodes = this.rootElement.ChildNodes;
                var skinableElement = skinable.Element;
                while(childNodes.Length > 0)
                {
                    skinableElement.AppendChild(childNodes[0]);
                }
            }

            if (this.isActive && !this.isDiposed)
            {
                this.UpdateBinderSource(skinable, BinderType.TemplateParent);
            }
            else
            {
                this.templateParentUpdated = true;
            }

            this.UpdateDataContext();
        }

        public void UpdateDataContext()
        {
            if (this.skinableParent != null)
            {
                if (this.skinableParent.DataContext != this.dataContext)
                {
                    this.dataContext = this.skinableParent.Skin.DataContextType.AsType(this.skinableParent.DataContext);
                    this.dataContextUpdated = true;
                }
            }
            else if (this.dataContext != null)
            {
                this.dataContext = null;
                this.dataContextUpdated = true;
            }

            if (this.dataContextUpdated && this.isActive && !this.isDiposed)
            {
                this.UpdateBinderSource(this.dataContext, BinderType.DataContext);
                this.dataContextUpdated = false;
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

                var childElements = this.childElements;
                var childElementLength = childElements.Length;
                var elementsOfIntrest = this.elementsOfIntrest;
                var dataContext = this.dataContext;

                this.bindingStrategy.OnDataContextUpdated(this.dataContextUpdated, this.templateParentUpdated);

                this.bindingStrategy.PushInitialValues(dataContext, this.skinableParent, elementsOfIntrest);

                for (int iChild = 0; iChild < childElementLength; iChild++)
                {
                    var objectIndex = childElements[iChild];
                    var childElement = elementsOfIntrest.GetFrom<UIElement>(childElements[iChild]);
                    if (!this.hasDataContextBinding[objectIndex])
                    {
                        childElement.DataContext = dataContext;
                    }

                    childElement.Activate();
                }

                this.firstActivationDone = true;
                TaskScheduler.Instance.EnqueueLowPriTask(
                    this.QueuedActivation,
                    "SkinInstance.Activate");
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
                var childElements = this.childElements;
                var childElementLength = childElements.Length;

                this.bindingStrategy.Deactivate();

                for (int iChild = 0; iChild < childElementLength; iChild++)
                {
                    this.elementsOfIntrest.GetFrom<UIElement>(childElements[iChild]).Deactivate();
                }

                TaskScheduler.Instance.EnqueueLowPriTask(
                    this.QueuedDeactivation,
                    "SkinInstance.QueuedDeactivate");
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
                if (this.skinableParent != null)
                {
                    var childNodes = this.skinableParent.Element.ChildNodes;
                    while(childNodes.Length > 0)
                    {
                        this.rootElement.AppendChild(childNodes[0]);
                    }
                }

                this.bindingStrategy.Dispose();

                this.isDiposed = true;
                for (int i = 0, j = this.childElements.Length; i < j; i++)
                {
                    var childElement = this.elementsOfIntrest.GetFrom<UIElement>(this.childElements[i]);
                    childElement.Deactivate();
                    childElement.Dispose();
                }
            }
        }

        /// <summary>
        /// Queued activation.
        /// </summary>
        private void QueuedActivation()
        {
            this.bindingStrategy.WireSubscriptions(
                this.skinableParent != null ? this.skinableParent.DataContext : null,
                this.skinableParent);
        }

        /// <summary>
        /// Queued deactivation.  /// </summary>
        private void QueuedDeactivation()
        {
            if (this.isActive || this.isDiposed)
            {
                return;
            }

            this.bindingStrategy.OnQueuedDeactivation(false, false);
        }

        /// <summary>
        /// Updates the binder source.
        /// </summary>
        /// <param name="source">     Source for the. </param>
        /// <param name="sourceType"> Type of the source. </param>
        private void UpdateBinderSource(object source, BinderType sourceType)
        {
            if (sourceType == BinderType.DataContext)
            {
                this.bindingStrategy.OnDataContextChanged(source);
            }
            else if (sourceType == BinderType.TemplateParent)
            {
                this.bindingStrategy.OnTemplateParentChanged(source);
            }

            if (sourceType == BinderType.DataContext)
            {
                var childElements = this.childElements;
                var childElementLength = childElements.Length;
                for (int iChild = 0; iChild < childElementLength; iChild++)
                {
                    var objectIndex = childElements[iChild];
                    var childElement = this.elementsOfIntrest.GetFrom<UIElement>(childElements[iChild]);
                    if (!this.hasDataContextBinding[objectIndex])
                    {
                        childElement.DataContext = dataContext;
                    }

                    childElement.Activate();
                }
            }
        }
    }
}

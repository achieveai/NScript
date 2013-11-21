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
        private NativeArray<LiveBinder> liveBinders;

        /// <summary>
        /// The extra objects.
        /// </summary>
        private NativeArray extraObjects;

        /// <summary>
        /// The part identifier mapping.
        /// </summary>
        private StringDictionary<int> partIdMapping;

        private UISkinableElement skinableParent;

        private object dataContext;

        private bool firstActivationDone;

        private bool dataContextUpdated;

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
            this.dataContextUpdated = true;
            this.templateParentUpdated = true;

            if (liveBinderCount > 0)
            { this.liveBinders = new NativeArray<LiveBinder>(liveBinderCount); }

            if (extraObjectCount > 0)
            { this.extraObjects = new NativeArray(extraObjectCount); }

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
                    this.dataContext = this.skinableParent.DataContext;
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
                var binders = this.binders;
                var childElementLength = childElements.Length;
                var binderLength = binders.Length;
                var skinParent = this.skinableParent;
                var dataContext = this.dataContext;

                for (int iBinder = 0, iLiveBinder = 0; iBinder < binderLength; iBinder++)
                {
                    var binder = binders[iBinder];
                    object source = null;
                    switch (binder.BinderType & BinderType.TargetTypes)
                    {
                        case BinderType.DataContext:
                            if (!this.dataContextUpdated
                                && binder.Mode != DataBindingMode.OneTime)
                            { continue; }

                            source = dataContext;
                            break;
                        case BinderType.Static:
                            if (this.firstActivationDone
                                && binder.Mode != DataBindingMode.OneTime)
                            { continue; }

                            break;
                        case BinderType.TemplateParent:
                            if (!this.templateParentUpdated
                                && binder.Mode != DataBindingMode.OneTime)
                            { continue; }

                            source = this.skinableParent;
                            break;
                    }

                    if (binder.Mode == DataBindingMode.TwoWay)
                    {
                        LiveBinder liveBinder = this.liveBinders[iLiveBinder];
                        if (liveBinder != null)
                        {
                            liveBinder.Source = source;
                            liveBinder.IsActive = true;
                        }
                        else
                        {
                            liveBinder = new LiveBinder(binder);
                            liveBinder.Source = source;
                            liveBinder.Target = this.elementsOfIntrest[binder.ObjectIndex];
                            liveBinder.IsActive = true;
                            this.liveBinders[iLiveBinder] = liveBinder;
                        }
                    }
                    else
                    {
                        SkinBinderHelper.SetPropertyValue(
                            binder,
                            source,
                            this.elementsOfIntrest[binder.ObjectIndex],
                            this.extraObjects);
                    }

                    if (binder.Mode != DataBindingMode.OneTime)
                    {
                        ++iLiveBinder;
                    }
                }

                for (int iChild = 0; iChild < childElementLength; iChild++)
                {
                    var childElement = childElements[iChild];
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
                var liveBinders = this.liveBinders;
                var liveBinderLength = liveBinders.Length;

                for (int iLiveBinder = 0; iLiveBinder < liveBinderLength; iLiveBinder++)
                {
                    if (liveBinders[iLiveBinder] == null)
                    { continue; }

                    liveBinders[iLiveBinder].IsActive = false;
                }

                for (int iChild = 0; iChild < childElementLength; iChild++)
                {
                    childElements[iChild].Deactivate();
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
                for (int iLiveBinder = 0; iLiveBinder < this.liveBinders.Length; iLiveBinder++)
                {
                    var liveBinder = this.liveBinders[iLiveBinder];
                    if (liveBinder == null)
                    {
                        continue;
                    }

                    liveBinder.IsActive = false;
                    liveBinder.Source = null;
                    liveBinder.Target = null;
                    liveBinder.Cleanup();
                    this.liveBinders[iLiveBinder] = null;
                }

                this.isDiposed = true;
                for (int i = 0, j = this.elementsOfIntrest.Length; i < j; i++)
                {
                    this.childElements[i].Deactivate();
                    this.childElements[i].Dispose();
                }
            }
        }

        /// <summary>
        /// Queued activation.
        /// </summary>
        private void QueuedActivation()
        {
            var binders = this.binders;
            var liveBinders = this.liveBinders;
            if (liveBinders == null)
            { return; }

            var binderLength = binders.Length;
            var liveBindersLength = liveBinders.Length;
            for (int iBinderInfo = 0, iLivebinder = 0;
                iBinderInfo < binderLength && iLivebinder < liveBindersLength;
                iBinderInfo++)
            {
                var binder = binders[iBinderInfo];
                if (binder.Mode != DataBindingMode.OneTime)
                {
                    LiveBinder liveBinder = liveBinders[iLivebinder];
                    if (object.IsNullOrUndefined(liveBinder))
                    {
                        liveBinders[iLivebinder] = liveBinder = new LiveBinder(binder);
                        liveBinder.Target = this.elementsOfIntrest[binder.ObjectIndex];
                    }

                    switch (binder.BinderType & BinderType.TargetTypes)
                    {
                        case BinderType.DataContext:
                            liveBinder.Source = this.skinableParent.DataContext;
                            break;
                        case BinderType.TemplateParent:
                            liveBinder.Source = this.skinableParent;
                            break;
                    }

                    liveBinder.IsActive = true;

                    ++iLivebinder;
                }
            }
        }

        /// <summary>
        /// Queued deactivation.  /// </summary>
        private void QueuedDeactivation()
        {
            if (this.isActive || this.isDiposed || this.liveBinders == null)
            {
                return;
            }

            for (int iLiveBinder = 0; iLiveBinder < this.liveBinders.Length; iLiveBinder++)
            {
                var liveBinder = this.liveBinders[iLiveBinder];
                if (object.IsNullOrUndefined(liveBinder))
                {
                    return;
                }

                liveBinder.IsActive = false;
                liveBinder.Cleanup();
            }
        }

        /// <summary>
        /// Updates the binder source.
        /// </summary>
        /// <param name="source">     Source for the. </param>
        /// <param name="sourceType"> Type of the source. </param>
        private void UpdateBinderSource(object source, BinderType sourceType)
        {
            var liveBinders = this.liveBinders;
            if (liveBinders == null)
            { return; }

            var liveBindersLength = this.liveBinders.Length;
            for (int iLiveBinder = 0; iLiveBinder < liveBindersLength; iLiveBinder++)
            {
                var liveBinder = liveBinders[iLiveBinder];
                if (Script.IsNullOrUndefined(liveBinder)
                    && (liveBinder.BinderInfo.BinderType & BinderType.TargetTypes) == sourceType)
                {
                    liveBinder.Source = source;
                }
            }
        }
    }
}

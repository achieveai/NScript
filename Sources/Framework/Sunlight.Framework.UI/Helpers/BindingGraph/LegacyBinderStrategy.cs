//-----------------------------------------------------------------------
// <copyright file="LegacyBinderStrategy.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    using Sunlight.Framework.Binders;
    using System;

    /// <summary>
    /// Wraps the existing XWML binder logic from SkinInstance into the IBindingStrategy contract.
    /// This preserves identical behavior to the original SkinInstance binder management.
    /// </summary>
    public class LegacyBinderStrategy : IBindingStrategy
    {
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
        /// The elements of interest (stored for QueuedActivation/Deactivation access).
        /// </summary>
        private NativeArray elementsOfInterest;

        /// <summary>
        /// Tracks which element indices have a DataContext binder set on them.
        /// </summary>
        private NativeArray<bool> hasDataContextBinding;

        /// <summary>
        /// true if first activation has been done (controls Static binder behavior).
        /// </summary>
        private bool firstActivationDone;

        /// <summary>
        /// Gets the hasDataContextBinding array for SkinInstance child element DataContext propagation.
        /// </summary>
        public NativeArray<bool> HasDataContextBinding
        {
            get { return this.hasDataContextBinding; }
        }

        /// <summary>
        /// true if data context has been updated since last push.
        /// Set by SkinInstance before calling PushInitialValues.
        /// </summary>
        public bool DataContextUpdated;

        /// <summary>
        /// true if template parent has been updated since last push.
        /// Set by SkinInstance before calling PushInitialValues.
        /// </summary>
        public bool TemplateParentUpdated;

        /// <summary>
        /// Initializes a new instance of the LegacyBinderStrategy class.
        /// </summary>
        /// <param name="binders">The binder info array.</param>
        /// <param name="liveBinderCount">Number of live binders to allocate.</param>
        /// <param name="extraObjectCount">Number of extra objects to allocate.</param>
        /// <param name="elementCount">Number of elements of interest (for hasDataContextBinding array).</param>
        public LegacyBinderStrategy(
            NativeArray<SkinBinderInfo> binders,
            int liveBinderCount,
            int extraObjectCount,
            int elementCount)
        {
            this.binders = binders;
            this.hasDataContextBinding = new NativeArray<bool>(elementCount);

            if (liveBinderCount > 0)
            { this.liveBinders = new NativeArray<LiveBinder>(liveBinderCount); }

            if (extraObjectCount > 0)
            { this.extraObjects = new NativeArray(extraObjectCount); }
        }

        /// <summary>
        /// Phase 1: Push initial values to all bound DOM targets synchronously.
        /// This is the exact binder loop from the original SkinInstance.Activate().
        /// </summary>
        public void PushInitialValues(object dataContext, object templateParent, NativeArray elementsOfInterest)
        {
            this.elementsOfInterest = elementsOfInterest;

            var binders = this.binders;
            var binderLength = binders.Length;
            Action<UIElement, object> dataContextSetter = SkinBinderHelper.SetDataContext;

            for (int iBinder = 0, iLiveBinder = 0; iBinder < binderLength; iBinder++)
            {
                var binder = binders[iBinder];
                object source = null;
                switch (binder.BinderType & BinderType.TargetTypes)
                {
                    case BinderType.DataContext:
                        if (!this.DataContextUpdated
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
                        if (!this.TemplateParentUpdated
                            && binder.Mode != DataBindingMode.OneTime)
                        { continue; }

                        source = templateParent;
                        break;
                }

                if (binder.Mode == DataBindingMode.TwoWay)
                {
                    LiveBinder liveBinder = this.liveBinders[iLiveBinder];
                    if (object.IsNullOrUndefined(liveBinder))
                    {
                        liveBinder = new LiveBinder(binder, this.extraObjects);
                        liveBinder.Source = source;
                        liveBinder.Target = elementsOfInterest[binder.ObjectIndex];
                        liveBinder.IsActive = true;
                        this.liveBinders[iLiveBinder] = liveBinder;
                    }
                    else
                    {
                        liveBinder.Source = source;
                        liveBinder.IsActive = true;
                    }
                }
                else
                {
                    SkinBinderHelper.SetPropertyValue(
                        binder,
                        source,
                        elementsOfInterest[binder.ObjectIndex],
                        this.extraObjects);

                    if ((object)binder.TargetPropertySetter == (object)dataContextSetter)
                    {
                        this.hasDataContextBinding[binder.ObjectIndex] = true;
                    }
                }

                if (binder.Mode != DataBindingMode.OneTime)
                {
                    ++iLiveBinder;
                }
            }

            this.firstActivationDone = true;
        }

        /// <summary>
        /// Phase 2: Wire reactive subscriptions (deferred via TaskScheduler).
        /// This is the exact logic from the original SkinInstance.QueuedActivation().
        /// </summary>
        public void WireSubscriptions(object dataContext, object templateParent)
        {
            var binders = this.binders;
            var liveBinders = this.liveBinders;
            if (object.IsNullOrUndefined(liveBinders))
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
                        liveBinders[iLivebinder] = liveBinder = new LiveBinder(binder, this.extraObjects);
                        liveBinder.Target = this.elementsOfInterest[binder.ObjectIndex];
                    }

                    switch (binder.BinderType & BinderType.TargetTypes)
                    {
                        case BinderType.DataContext:
                            liveBinder.Source = dataContext;
                            break;
                        case BinderType.TemplateParent:
                            liveBinder.Source = templateParent;
                            break;
                    }

                    liveBinder.IsActive = true;

                    ++iLivebinder;
                }
            }
        }

        /// <summary>
        /// Called when the DataContext changes after activation.
        /// This is the binder-source-update part of the original SkinInstance.UpdateBinderSource()
        /// for BinderType.DataContext.
        /// </summary>
        public void OnDataContextChanged(object newDataContext)
        {
            this.UpdateBinderSourceInternal(newDataContext, BinderType.DataContext);
        }

        /// <summary>
        /// Called when the TemplateParent changes after activation.
        /// This is the binder-source-update part of the original SkinInstance.UpdateBinderSource()
        /// for BinderType.TemplateParent.
        /// </summary>
        public void OnTemplateParentChanged(object newTemplateParent)
        {
            this.UpdateBinderSourceInternal(newTemplateParent, BinderType.TemplateParent);
        }

        /// <summary>
        /// Queued deactivation logic. Called by SkinInstance.QueuedDeactivation().
        /// </summary>
        public void QueuedDeactivation(bool isActive, bool isDiposed)
        {
            if (isActive || isDiposed || object.IsNullOrUndefined(this.liveBinders))
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
        /// Deactivate: set all live binders inactive.
        /// </summary>
        public void Deactivate()
        {
            var liveBinders = this.liveBinders;

            if (!object.IsNullOrUndefined(liveBinders))
            {
                var liveBinderLength = liveBinders.Length;

                for (int iLiveBinder = 0; iLiveBinder < liveBinderLength; iLiveBinder++)
                {
                    if (object.IsNullOrUndefined(liveBinders[iLiveBinder]))
                    { continue; }

                    liveBinders[iLiveBinder].IsActive = false;
                }
            }
        }

        /// <summary>
        /// Full cleanup: deactivate, release sources/targets, cleanup all live binders.
        /// </summary>
        public void Dispose()
        {
            if (!object.IsNullOrUndefined(this.liveBinders))
            {
                for (int iLiveBinder = 0; iLiveBinder < this.liveBinders.Length; iLiveBinder++)
                {
                    var liveBinder = this.liveBinders[iLiveBinder];
                    if (object.IsNullOrUndefined(liveBinder))
                    {
                        continue;
                    }

                    liveBinder.IsActive = false;
                    liveBinder.Source = null;
                    liveBinder.Target = null;
                    liveBinder.Cleanup();
                    this.liveBinders[iLiveBinder] = null;
                }
            }
        }

        /// <summary>
        /// Notifies the strategy that data context and/or template parent flags have been updated.
        /// </summary>
        public void OnDataContextUpdated(bool dcUpdated, bool tpUpdated)
        {
            this.DataContextUpdated = dcUpdated;
            this.TemplateParentUpdated = tpUpdated;
        }

        /// <summary>
        /// Performs deferred deactivation cleanup (unwiring live binders).
        /// </summary>
        public void OnQueuedDeactivation(bool isActive, bool isDisposed)
        {
            this.QueuedDeactivation(isActive, isDisposed);
        }

        /// <summary>
        /// Internal helper: updates binder sources for the given source type.
        /// This is the binder loop portion of the original SkinInstance.UpdateBinderSource().
        /// </summary>
        private void UpdateBinderSourceInternal(object source, BinderType sourceType)
        {
            var liveBinders = this.liveBinders;
            var binders = this.binders;
            var bindersLength = binders.Length;
            var liveBindersLength = object.IsNullOrUndefined(liveBinders) ? 0 : liveBinders.Length;

            for (int iBinder = 0, iLiveBinder = 0; iBinder < bindersLength; iBinder++)
            {
                var binder = binders[iBinder];
                if (binder.Mode != DataBindingMode.OneTime
                    && iLiveBinder < liveBindersLength
                    && !object.IsNullOrUndefined(liveBinders[iLiveBinder]))
                {
                    if (sourceType == (binder.BinderType & BinderType.TargetTypes))
                    {
                        liveBinders[iLiveBinder].Source = source;
                    }

                    iLiveBinder++;
                }
                else if (sourceType == (binder.BinderType & BinderType.TargetTypes))
                {
                    SkinBinderHelper.SetPropertyValue(
                        binder,
                        source,
                        this.elementsOfInterest[binder.ObjectIndex],
                        this.extraObjects);
                }
            }
        }
    }
}

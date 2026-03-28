namespace Sunlight.Framework.UI.Helpers.BindingGraph
{
    /// <summary>
    /// Internal strategy interface for SkinInstance binding management.
    /// Selected based on whether the factory provides a GraphDescriptor or SkinBinderInfo[].
    /// </summary>
    public interface IBindingStrategy
    {
        /// <summary>
        /// Phase 1: Push initial values to all bound DOM targets synchronously.
        /// Called during SkinInstance.Activate().
        /// </summary>
        void PushInitialValues(object dataContext, object templateParent, NativeArray elementsOfInterest);

        /// <summary>
        /// Phase 2: Wire reactive subscriptions (deferred via TaskScheduler).
        /// Called from SkinInstance.QueuedActivation().
        /// </summary>
        void WireSubscriptions(object dataContext, object templateParent);

        /// <summary>
        /// Called when the DataContext changes after activation.
        /// </summary>
        void OnDataContextChanged(object newDataContext);

        /// <summary>
        /// Called when the TemplateParent changes after activation.
        /// </summary>
        void OnTemplateParentChanged(object newTemplateParent);

        /// <summary>
        /// Deactivate: unsubscribe listeners but keep state for potential reactivation.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Full cleanup: unsubscribe, release DOM references, destroy child graphs.
        /// </summary>
        void Dispose();
    }
}

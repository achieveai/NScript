//-----------------------------------------------------------------------
// <copyright file="UIPanel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI
{
    using System;
    using System.Collections.Generic;
    using System.Web.Html;
    using Sunlight.Framework.Observables;

    /// <summary>
    /// Definition for UIPanel
    /// </summary>
    public abstract class UIPanel : UIElement
    {
        /// <summary>
        /// The inner children.
        /// </summary>
        private List<UIElement> innerChildren = new List<UIElement>();

        /// <summary>
        /// The children.
        /// </summary>
        private ObservableCollection<UIElement> children = new ObservableCollection<UIElement>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="element"> The element. </param>
        public UIPanel(Element element)
            : base(element)
        {
            children.CollectionChanged += ChildrenCollectionChanged;
        }

        public ObservableCollection<UIElement> Children
        { get { return this.children; } }

        /// <summary>
        /// Children collection changed.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="args">   The arguments. </param>
        protected abstract void ChildrenCollectionChanged(
            INotifyCollectionChanged<UIElement> sender,
            CollectionChangedEventArgs<UIElement> args);
    }
}
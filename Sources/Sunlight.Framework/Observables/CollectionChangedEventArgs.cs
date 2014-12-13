//-----------------------------------------------------------------------
// <copyright file="CollectionChangedEventArgs.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Observables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface CollectionChangedEventArgs
    {
        /// <summary>
        /// Gets item after the change
        /// </summary>
        /// <value>The new items.</value>
        IList NewItems
        {
            get;
        }

        /// <summary>
        /// Gets item before the change
        /// </summary>
        /// <value>The old items.</value>
        IList OldItems
        {
            get;
        }

        /// <summary>
        /// Returns index of the item being changed
        /// </summary>
        int ChangeIndex
        {
            get;
        }

        /// <summary>
        /// Get's the change action of this notification
        /// </summary>
        CollectionChangedAction Action
        {
            get;
        }
    }

    /// <summary>
    /// Definition for CollectionChangedEventArgs
    /// </summary>
    public class CollectionChangedEventArgs<T> : CollectionChangedEventArgs
    {
        /// <summary>
        /// private storage for action
        /// </summary>
        private CollectionChangedAction action;
        
        /// <summary>
        /// private storage for item index
        /// </summary>
        private int changeIndex;

        /// <summary>
        /// Backing store for newItems.
        /// </summary>
        private IList<T> newItems;

        /// <summary>
        /// Backing store for oldItems.
        /// </summary>
        private IList<T> oldItems;

        /// <summary>
        /// Initializes a new instance of the CollectionChangedEventArgs class.
        /// </summary>
        /// <param name="action">Change action</param>
        /// <param name="changeIndex">Index of the item changed</param>
        /// <param name="newItems">The new items.</param>
        /// <param name="oldItems">The old items.</param>
        public CollectionChangedEventArgs(
            CollectionChangedAction action,
            int changeIndex,
            IList<T> newItems,
            IList<T> oldItems)
        {
            this.action = action;
            this.changeIndex = changeIndex;

            switch (action)
            {
                case CollectionChangedAction.Add:
                    this.newItems = newItems;
                    break;
                case CollectionChangedAction.Remove:
                    this.oldItems = oldItems;
                    break;
                case CollectionChangedAction.Replace:
                    this.newItems = newItems;
                    this.oldItems = oldItems;
                    break;
                case CollectionChangedAction.Reset:
                    this.changeIndex = -1;
                    break;
            }
        }

        /// <summary>
        /// Returns index of the item being changed
        /// </summary>
        public int ChangeIndex
        {
            get { return this.changeIndex; }
        }

        /// <summary>
        /// Get's the change action of this notification
        /// </summary>
        public CollectionChangedAction Action
        {
            get { return this.action; }
        }

        /// <summary>
        /// Gets item after the change
        /// </summary>
        /// <value>The new items.</value>
        public IList<T> NewItems
        {
            get { return this.newItems; }
        }

        /// <summary>
        /// Gets item before the change
        /// </summary>
        /// <value>The old items.</value>
        public IList<T> OldItems
        {
            get { return this.oldItems; }
        }

        IList CollectionChangedEventArgs.NewItems
        {
            get { return this.NewItems; }
        }

        IList CollectionChangedEventArgs.OldItems
        {
            get { return this.OldItems; }
        }
    }
}

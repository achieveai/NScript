//-----------------------------------------------------------------------
// <copyright file="CollectionBinder.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI.Helpers
{
    using Sunlight.Framework.Observables;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Html;

    /// <summary>
    /// Runtime binder for reactive @foreach on ObservableCollections.
    /// Manages incremental DOM updates: add, remove, replace, reset.
    /// </summary>
    public class CollectionBinder
    {
        /// <summary>
        /// The observable collection being watched.
        /// </summary>
        private IObservableCollection collection;

        /// <summary>
        /// The collection currently attached to (listening for changes).
        /// </summary>
        private IObservableCollection attachedCollection;

        /// <summary>
        /// The parent DOM element that contains the repeated items.
        /// </summary>
        private Element parentElement;

        /// <summary>
        /// Template element to clone for each item.
        /// </summary>
        private Element itemTemplate;

        /// <summary>
        /// Factory function that binds a cloned template to a data item.
        /// </summary>
        private Func<Element, object, Element> itemFactory;

        /// <summary>
        /// List of DOM elements corresponding to each collection item.
        /// </summary>
        private List<Element> itemElements;

        /// <summary>
        /// true if this binder is active.
        /// </summary>
        private bool isActive;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parentElement"> The parent DOM element. </param>
        /// <param name="itemTemplate"> Template element to clone for each item. </param>
        /// <param name="itemFactory"> Factory function that binds a template clone to a data item. </param>
        public CollectionBinder(
            Element parentElement,
            Element itemTemplate,
            Func<Element, object, Element> itemFactory)
        {
            this.parentElement = parentElement;
            this.itemTemplate = itemTemplate;
            this.itemFactory = itemFactory;
            this.itemElements = new List<Element>();
        }

        /// <summary>
        /// Gets or sets the observable collection.
        /// </summary>
        /// <value>
        /// The observable collection.
        /// </value>
        public IObservableCollection Collection
        {
            get { return this.collection; }
            set
            {
                if (this.collection == value)
                {
                    return;
                }

                this.DetachCollection();
                this.collection = value;

                if (this.isActive && !object.IsNullOrUndefined(this.collection))
                {
                    this.AttachCollection();
                    this.Reset();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this binder is active.
        /// </summary>
        /// <value>
        /// true if this binder is active, false if not.
        /// </value>
        public bool IsActive
        {
            get { return this.isActive; }
            set
            {
                if (this.isActive == value)
                {
                    return;
                }

                this.isActive = value;

                if (value && !object.IsNullOrUndefined(this.collection))
                {
                    this.AttachCollection();
                    this.Reset();
                }
                else
                {
                    this.DetachCollection();
                    this.ClearAll();
                }
            }
        }

        /// <summary>
        /// Disposes this binder, detaching from the collection and clearing DOM.
        /// </summary>
        public void Dispose()
        {
            this.DetachCollection();
            this.ClearAll();
            this.collection = null;
        }

        /// <summary>
        /// Attaches to the current collection's change events.
        /// </summary>
        private void AttachCollection()
        {
            if (!object.IsNullOrUndefined(this.attachedCollection))
            {
                return;
            }

            this.attachedCollection = this.collection;
            ((INotifyCollectionChanged)this.attachedCollection).CollectionChanged += this.OnCollectionChanged;
        }

        /// <summary>
        /// Detaches from the currently attached collection's change events.
        /// </summary>
        private void DetachCollection()
        {
            if (object.IsNullOrUndefined(this.attachedCollection))
            {
                return;
            }

            ((INotifyCollectionChanged)this.attachedCollection).CollectionChanged -= this.OnCollectionChanged;
            this.attachedCollection = null;
        }

        /// <summary>
        /// Handles collection changed events.
        /// </summary>
        /// <param name="sender"> The sender. </param>
        /// <param name="args"> The collection changed event args. </param>
        private void OnCollectionChanged(
            INotifyCollectionChanged sender,
            CollectionChangedEventArgs args)
        {
            if (!this.isActive)
            {
                return;
            }

            switch (args.Action)
            {
                case CollectionChangedAction.Add:
                    this.OnAdd(args.ChangeIndex, args.NewItems);
                    break;
                case CollectionChangedAction.Remove:
                    this.OnRemove(args.ChangeIndex, args.OldItems.Count);
                    break;
                case CollectionChangedAction.Replace:
                    this.OnReplace(args.ChangeIndex, args.NewItems);
                    break;
                case CollectionChangedAction.Reset:
                    this.Reset();
                    break;
            }
        }

        /// <summary>
        /// Handles items being added to the collection.
        /// </summary>
        /// <param name="index"> The index at which items were added. </param>
        /// <param name="newItems"> The new items. </param>
        private void OnAdd(int index, IList newItems)
        {
            Element insertBefore = index < this.itemElements.Count
                ? this.itemElements[index]
                : null;

            for (int i = 0; i < newItems.Count; i++)
            {
                var element = this.CreateItemElement(newItems[i]);
                if (!object.IsNullOrUndefined(insertBefore))
                {
                    this.parentElement.InsertBefore(element, insertBefore);
                    this.itemElements.Insert(index + i, element);
                }
                else
                {
                    this.parentElement.AppendChild(element);
                    this.itemElements.Add(element);
                }
            }
        }

        /// <summary>
        /// Handles items being removed from the collection.
        /// </summary>
        /// <param name="index"> The index at which items were removed. </param>
        /// <param name="count"> The number of items removed. </param>
        private void OnRemove(int index, int count)
        {
            for (int i = count - 1; i >= 0; i--)
            {
                var idx = index + i;
                if (idx < this.itemElements.Count)
                {
                    this.itemElements[idx].Remove();
                    this.itemElements.RemoveAt(idx);
                }
            }
        }

        /// <summary>
        /// Handles items being replaced in the collection.
        /// </summary>
        /// <param name="index"> The index at which items were replaced. </param>
        /// <param name="newItems"> The replacement items. </param>
        private void OnReplace(int index, IList newItems)
        {
            for (int i = 0; i < newItems.Count; i++)
            {
                var idx = index + i;
                if (idx < this.itemElements.Count)
                {
                    var oldElement = this.itemElements[idx];
                    var newElement = this.CreateItemElement(newItems[i]);
                    this.parentElement.InsertBefore(newElement, oldElement);
                    oldElement.Remove();
                    this.itemElements[idx] = newElement;
                }
            }
        }

        /// <summary>
        /// Resets the DOM by clearing all items and re-creating from the collection.
        /// </summary>
        private void Reset()
        {
            this.ClearAll();

            if (object.IsNullOrUndefined(this.attachedCollection))
            {
                return;
            }

            int count = this.attachedCollection.Count;
            for (int i = 0; i < count; i++)
            {
                var element = this.CreateItemElement(this.attachedCollection[i]);
                this.parentElement.AppendChild(element);
                this.itemElements.Add(element);
            }
        }

        /// <summary>
        /// Clears all item elements from the DOM.
        /// </summary>
        private void ClearAll()
        {
            for (int i = this.itemElements.Count - 1; i >= 0; i--)
            {
                this.itemElements[i].Remove();
            }

            this.itemElements.Clear();
        }

        /// <summary>
        /// Creates a DOM element for a data item by cloning the template and applying the factory.
        /// </summary>
        /// <param name="dataItem"> The data item. </param>
        /// <returns> The bound DOM element. </returns>
        private Element CreateItemElement(object dataItem)
        {
            var element = this.itemTemplate.CloneNode(true);
            return this.itemFactory(element, dataItem);
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="ObservableCollection.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Observables
{
    using System;
using System.Collections;
using System.Collections.Generic;

    public interface IObservableCollection : INotifyCollectionChanged
    {
        // Summary:
        //     Gets the number of elements contained in the System.Collections.ICollection.
        //
        // Returns:
        //     The number of elements contained in the System.Collections.ICollection.
        int Count { get; }

        // Summary:
        //     Gets or sets the element at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the element to get or set.
        //
        // Returns:
        //     The element at the specified index.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is not a valid index in the System.Collections.IList.
        //
        //   System.NotSupportedException:
        //     The property is set and the System.Collections.IList is read-only.
        object this[int index] { get; }

        //
        // Summary:
        //     Determines whether the System.Collections.IList contains a specific value.
        //
        // Parameters:
        //   value:
        //     The object to locate in the System.Collections.IList.
        //
        // Returns:
        //     true if the System.Object is found in the System.Collections.IList; otherwise,
        //     false.
        bool Contains(object value);

        //
        // Summary:
        //     Determines the index of a specific item in the System.Collections.IList.
        //
        // Parameters:
        //   value:
        //     The object to locate in the System.Collections.IList.
        //
        // Returns:
        //     The index of value if found in the list; otherwise, -1.
        int IndexOf(object value);
    }

    /// <summary>
    /// Represents a dynamic data collection that provides notifications
    /// when items get added, removed, or when the entire list is refreshed.
    /// </summary>
    public class ObservableCollection<T> : ObservableObject, IObservableCollection, INotifyCollectionChanged<T>
    {
        /// <summary>
        /// Key for the event handler
        /// </summary>
        private const string CollectionChangedEvent = "CollectionChanged";

        /// <summary>
        /// Indicates whether the collection is busy sending a notification event.
        /// </summary>
        private bool busy;

        /// <summary>
        /// List that stores the items in the collection.
        /// </summary>
        private readonly List<T> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollection"/> class.
        /// </summary>
        public ObservableCollection()
        {
            this.items = new List<T>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="items">  List that stores the items in the collection. </param>
        /// <param name="shared"> (optional) the shared. </param>
        public ObservableCollection(List<T> items, bool shared = false)
        {
            if (shared)
            {
                this.items = items;
            }
            else
            {
                this.items = new List<T>();
                this.items.AddRange(items);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="items">  List that stores the items in the collection. </param>
        public ObservableCollection(ObservableCollection<T> items)
        {
            this.items = new List<T>();
            this.items.AddRange(items.items);
        }

        /// <summary>
        /// Occurs when an item in the collection changes, or the entire collection changes.
        /// </summary>
        public event Action<INotifyCollectionChanged<T>, CollectionChangedEventArgs<T>> CollectionChanged;

        /// <summary>
        /// Gets the number of elements actually contained in the collection.
        /// </summary>
        /// <value>The number of elements actually contained in the collection.</value>
        public int Count
        {
            get { return this.items.Count; }
        }

        /// <summary>
        /// Gets the object at the specified index.
        /// </summary>
        /// <param name="index">Index of the item to get or set.</param>
        /// <value>The object at the specified index.</value>
        public T this[int index]
        {
            get
            {
                return this.items[index];
            }

            set
            {
                ExceptionHelpers.ThrowOnOutOfRange(
                    index,
                    0,
                    this.Count - 1,
                    "index");

                if (!Object.Equals(this.items[index], value))
                {
                    T oldItem = this.items[index];
                    this.items[index] = value;

                    this.OnCollectionChanged(
                        CollectionChangedAction.Replace,
                        index,
                        new T[] { value },
                        new T[] { oldItem });
                }
            }
        }

        /// <summary>
        /// Inserts an item at given index.
        /// </summary>
        /// <param name="index">Index in the list.</param>
        /// <param name="item">The item to add.</param>
        public void InsertAt(int index, T item)
        {
            this.CheckReentrancy();
            this.items.Insert(
                index,
                item);

            this.OnCollectionChanged(
                CollectionChangedAction.Add,
                index,
                new T[] { item },
                null);

            this.FirePropertyChanged("Count");
        }

        /// <summary>
        /// Inserts the range at.
        /// </summary>
        /// <param name="insertIndex">Index of the insert.</param>
        /// <param name="itemsToAdd">The items to add.</param>
        public void InsertRangeAt(int insertIndex, T[] itemsToAdd)
        {
            ExceptionHelpers.ThrowOnArgumentNull(itemsToAdd, "itemsToAdd");
            ExceptionHelpers.ThrowOnOutOfRange(insertIndex, 0, this.Count, "insertIndex");

            if (insertIndex == this.items.Count)
            {
                this.AddRange(itemsToAdd);
                return;
            }

            this.CheckReentrancy();

            this.items.InsertRange(
                insertIndex,
                itemsToAdd);

            this.OnCollectionChanged(
                CollectionChangedAction.Add,
                insertIndex,
                itemsToAdd,
                null);

            this.FirePropertyChanged("Count");
        }

        /// <summary>
        /// Inserts the range at.
        /// </summary>
        /// <param name="insertIndex">Index of the insert.</param>
        /// <param name="itemsToAdd">The items to add.</param>
        public void InsertRangeAt(int insertIndex, IList<T> itemsToAdd)
        {
            ExceptionHelpers.ThrowOnArgumentNull(itemsToAdd, "itemsToAdd");
            ExceptionHelpers.ThrowOnOutOfRange(insertIndex, 0, this.Count, "insertIndex");

            if (insertIndex == this.items.Count)
            {
                this.AddRange(itemsToAdd);
                return;
            }

            this.CheckReentrancy();

            this.items.InsertRange(
                insertIndex,
                itemsToAdd);

            this.OnCollectionChanged(
                CollectionChangedAction.Add,
                insertIndex,
                itemsToAdd,
                null);

            this.FirePropertyChanged("Count");
        }

        /// <summary>
        /// Checks if a certain object exists in the Observable Collection.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>true if the object is present, false otherwise </returns>
        public bool Contains(T item)
        {
            return this.items.Contains(item);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">Item to search for.</param>
        /// <returns>index of the item.</returns>
        public int IndexOf(T item)
        {
            return this.items.IndexOf(item);
        }

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="o">The object to be added to the end of the collection.</param>
        public void Add(T o)
        {
            this.CheckReentrancy();
            this.items.Add(o);
            this.OnCollectionChanged(
                CollectionChangedAction.Add,
                this.Count - 1,
                new T[] { o },
                null);
            this.FirePropertyChanged("Count");
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="objArray">The obj array.</param>
        public void AddRange(T[] objArray)
        {
            this.CheckReentrancy();
            this.items.AddRange(objArray);
            this.OnCollectionChanged(
                CollectionChangedAction.Add,
                this.Count - objArray.Length,
                objArray,
                null);
            this.FirePropertyChanged("Count");
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="objArray">The obj array.</param>
        public void AddRange(IList<T> objArray)
        {
            this.CheckReentrancy();
            this.items.AddRange(objArray);
            this.OnCollectionChanged(
                CollectionChangedAction.Add,
                this.Count - objArray.Count,
                objArray,
                null);
            this.FirePropertyChanged("Count");
        }

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        public void Clear()
        {
            if (this.Count == 0)
            {
                return;
            }

            this.CheckReentrancy();

            var backupItems = this.items.ToArray();

            this.items.Clear();
            this.OnCollectionChanged(
                CollectionChangedAction.Remove,
                0,
                null,
                backupItems);
            this.FirePropertyChanged("Count");
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>
        /// True if the item was successfully removed from the collection; otherwise, false.
        /// </returns>
        public bool Remove(T item)
        {
            int index = this.items.IndexOf(item);
            if (index < 0)
            {
                return false;
            }
            else
            {
                this.RemoveAt(index);
                return true;
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            this.CheckReentrancy();

            var item = this.items[index];
            this.items.RemoveAt(index);
            this.OnCollectionChanged(
                CollectionChangedAction.Remove,
                index,
                null,
                new T[] { item });
            this.FirePropertyChanged("Count");
        }

        /// <summary>
        /// Removes the range at.
        /// </summary>
        /// <param name="removeIndex">Index of the remove.</param>
        /// <param name="count">The count.</param>
        public void RemoveRangeAt(int removeIndex, int count)
        {
            ExceptionHelpers.ThrowOnOutOfRange(
                count,
                1,
                this.items.Count,
                "count");

            ExceptionHelpers.ThrowOnOutOfRange(
                removeIndex,
                0,
                this.items.Count - count,
                "removeIndex");

            this.CheckReentrancy();

            List<T> itemsToRemove = new List<T>();

            for (int index = count - 1; index >= 0; index--)
            {
                itemsToRemove.Add(this.items[removeIndex]);
                this.items.RemoveAt(removeIndex);
            }

            this.OnCollectionChanged(
                CollectionChangedAction.Remove,
                removeIndex,
                null,
                itemsToRemove);

            this.FirePropertyChanged("Count");
        }

        public void ReplaceRangeAt(int replaceIndex, IList<T> list)
        {
            if (replaceIndex + list.Count > this.Count)
            { throw new Exception("Can't replace, size mismatch"); }

            // TODO: complete method.
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The clone of this instance</returns>
        public ObservableCollection<T> Clone()
        {
            ObservableCollection<T> clone = new ObservableCollection<T>();
            for (int index = 0; index < this.Count; index++)
            {
                clone.Add(this[index]);
            }

            return clone;
        }

        /// <summary>
        /// Checks that the collection is not being changed while it is raising an event.
        /// </summary>
        private void CheckReentrancy()
        {
            if (this.busy)
            {
                throw Exception.Create("InvalidOperationException", null);
            }
        }

        /// <summary>
        /// Called when the collection changed.
        /// </summary>
        /// <param name="action">The action that changed the collection.</param>
        /// <param name="index">The index of the item.</param>
        /// <param name="newItems">The new items.</param>
        /// <param name="oldItems">The old items.</param>
        private void OnCollectionChanged(
            CollectionChangedAction action,
            int index,
            IList<T> newItems,
            IList<T> oldItems)
        {
            if (this.CollectionChanged != null)
            {
                this.busy = true;
                try
                {
                    var collectionChangedArgs = 
                        new CollectionChangedEventArgs<T>(
                            action,
                            index,
                            newItems,
                            oldItems);

                    this.CollectionChanged(this, collectionChangedArgs);
                }
                finally
                {
                    this.busy = false;
                }
            }
        }

        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        /// <param name="index"> Zero-based index of the entry to access. </param>
        /// <returns>
        /// The indexed item.
        /// </returns>
        object IObservableCollection.this[int index]
        {
            get
            {
                return this[index];
            }
        }

        /// <summary>
        /// Query if this object contains the given item.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        /// true if the object is in this collection, false if not.
        /// </returns>
        bool IObservableCollection.Contains(object value)
        {
            return ((IList)this.items).IndexOf(value) >= 0;
        }

        /// <summary>
        /// Index of the given value.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        /// .
        /// </returns>
        int IObservableCollection.IndexOf(object value)
        {
            return ((IList)this.items).IndexOf(value);
        }

        /// <summary>
        /// Event queue for all listeners interested in NotifyCollectionChanged.CollectionChanged events.
        /// </summary>
        event Action<INotifyCollectionChanged, CollectionChangedEventArgs> INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                this.CollectionChanged += (Action<INotifyCollectionChanged<T>, CollectionChangedEventArgs<T>>)value;
            }

            remove
            {
                this.CollectionChanged -= (Action<INotifyCollectionChanged<T>, CollectionChangedEventArgs<T>>)value;
            }
        }
    }
}

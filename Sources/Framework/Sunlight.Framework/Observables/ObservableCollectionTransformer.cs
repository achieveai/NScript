//-----------------------------------------------------------------------
// <copyright file="HeaderInjectableTransformer.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.Observables
{
    using System;
    using System.Collections.Generic;

    //Given a list of type T generates a corresponding Observable Collection of type U
    public class ObservableCollectionGenerator<T, U> where U : class
    {
        private Func<T, U> _transformDelegate;
        private ObservableCollection<U> _outCollection;
        private List<T> _inputCollection = null;

        public ObservableCollectionGenerator(Func<T, U> transformDelegate)
        {
            this._transformDelegate = transformDelegate;
            this._outCollection = new ObservableCollection<U>();
        }

        public List<T> InputCollection
        {
            get { return this._inputCollection; }
            set
            {
                if (this._inputCollection == value)
                { return; }

                this._inputCollection = value;

                // Remove (if applicable, i.e. new list is null or is smaller than previous list).
                // Replace Elements
                // Add, if new list is bigger than older list or older list was null.
                // default null to 0 and check 0 length inside methods
                var newLength = value != null
                    ? value.Count
                    : 0;
                var oldLength = this._outCollection.Count;
                var lengthDifference = newLength - oldLength;

                if (lengthDifference < 0)
                {
                    //Overall extra elements are removed
                    this.RemoveAfterKItems(
                        newLength,
                        -lengthDifference);
                    this.ReplaceFirstKItems(
                        value,
                        newLength);
                }
                else
                {
                    //Overall extra elements are replaced and/or added
                    this.ReplaceFirstKItems(
                        value,
                        oldLength);
                    this.InsertAfterFirstKItems(
                        value,
                        oldLength,
                        lengthDifference);
                }

                return;
            }
        }

        private void ReplaceFirstKItems(List<T> collection, int k)
        {
            if (k == 0 || collection == null)
            {
                return;
            }
            else
            {
                var elementsToReplace = collection.GetRangeAt(
                    0,
                    k);

                _outCollection.ReplaceRangeAt(
                    0,
                    GenerateTransformedCollection(
                        elementsToReplace));
                return;
            }
        }

        private void InsertAfterFirstKItems(List<T> collection, int k, int count)
        {
            if (count == 0 || collection == null)
            {
                return;
            }
            else
            {
                var elementsToInsert = collection.GetRangeAt(
                        k,
                        count);

                _outCollection.InsertRangeAt(
                    k,
                    GenerateTransformedCollection(
                        elementsToInsert));
            }
            return;
        }

        private void RemoveAfterKItems(int k, int count)
        {
            if (count == 0)
            {
                return;
            }
            else
            {
                _outCollection.RemoveRangeAt(
                    k,
                    count);
            }
            return;
        }

        private List<U> GenerateTransformedCollection(List<T> elements)
        {
            var rv = new List<U>();
            foreach (var element in elements)
            {
                U newItem = this._transformDelegate(element);
                rv.Add(newItem);
            }
            return rv;
        }

        public ObservableCollection<U> OutputCollection
        { get { return _outCollection; } }
    }

    //Changes Observable collection of type T of that of type U
    public class ObservableCollectionTransformer<T, U>
    {
        private Func<T, U> _transformDelegate;
        private ObservableCollection<T> sourceCollection;

        public ObservableCollectionTransformer(
            ObservableCollection<T> collection,
            Func<T, U> transformDelegate)
        {
            this._transformDelegate = transformDelegate;
            this.sourceCollection = collection;
            this.TransformedCollection = new ObservableCollection<U>();
            this.sourceCollection.CollectionChanged += this.OnSourceChanged;
            this.BuildCollection();
        }

        public ObservableCollection<U> TransformedCollection { get; private set; }

        private void OnSourceChanged(
            INotifyCollectionChanged<T> obj1,
            CollectionChangedEventArgs<T> obj2)
        {
            switch (obj2.Action)
            {
                case CollectionChangedAction.Add:
                    this.TransformedCollection.InsertRangeAt(
                        obj2.ChangeIndex,
                        GenerateTransformedCollection(obj2.NewItems));
                    break;
                case CollectionChangedAction.Remove:
                    this.TransformedCollection.RemoveRangeAt(
                        obj2.ChangeIndex,
                        obj2.OldItems.Count);
                    break;
                case CollectionChangedAction.Replace:
                    this.TransformedCollection.ReplaceRangeAt(
                        obj2.ChangeIndex,
                        GenerateTransformedCollection(obj2.NewItems));
                    break;
                case CollectionChangedAction.Reset:
                    this.BuildCollection();
                    break;
            }
        }

        private void BuildCollection()
        {
            this.TransformedCollection.Clear();
            for (int idx = 0; idx < this.sourceCollection.Count; idx++)
            { this.TransformedCollection[idx] = this._transformDelegate(this.sourceCollection[idx]); }
        }

        private List<U> GenerateTransformedCollection(IList<T> elements)
        {
            var rv = new List<U>();
            foreach (var element in elements)
            {
                U newItem = this._transformDelegate(element);
                rv.Add(newItem);
            }
            return rv;
        }

    }
}

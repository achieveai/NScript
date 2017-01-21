//-----------------------------------------------------------------------
// <copyright file="ListView.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI
{
    using Sunlight.Framework.Observables;
    using Sunlight.Framework.UI.Attributes;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web.Html;

    /// <summary>
    /// Definition for ListView
    /// </summary>
    [TagName("ul")]
    public class ListView : UIElement
    {
        public const string FixedListPropName = "FixedList",
            ObservableListPropName = "ObservableList",
            ItemSkinPropName = "ItemSkin";

        List<ListViewItem> items = new List<ListViewItem>();
        IObservableCollection observableList;
        IObservableCollection attachedObservableList;
        IList fixedList;
        Skin itemSkin;
        string itemCssClassName;
        bool inlineItems;
        private int topN = 1 << 30;
        private ISelectionHelper selectionHelper;

        public ListView(Element element)
            : base(element)
        { }

        public ListView(Element element, ISelectionHelper selectionHelper)
            :base(element)
        {
            this.selectionHelper = selectionHelper;
        }

        /// <summary>
        /// Gets or sets the top n, mostly one time binding.
        /// </summary>
        /// <value>
        /// The top n.
        /// </value>
        public int TopN
        {
            get { return this.topN; }
            set { this.topN = value; }
        }

        public IList FixedList
        {
            get
            {
                return this.fixedList;
            }

            set
            {
                if (value != null && this.observableList != null)
                {
                    throw new Exception("Can't set FixedList and ObservableList at the same time");
                }

                if (this.fixedList != value)
                {
                    this.fixedList = value;
                    this.FirePropertyChanged(ListView.FixedListPropName);
                    this.ApplyFixedList();
                }
            }
        }

        public IObservableCollection ObservableList
        {
            get
            {
                return this.observableList;
            }

            set
            {
                if (value != null && this.fixedList != null)
                {
                    throw new Exception("Can't set FixedList and ObservableList at the same time");
                }

                if (this.observableList != value)
                {
                    if (this.attachedObservableList != value
                        && this.attachedObservableList != null)
                    {
                        this.attachedObservableList.CollectionChanged -= ObservableListCollectionChanged;
                        this.attachedObservableList = null;
                    }

                    this.observableList = value;

                    this.FirePropertyChanged(ListView.ObservableListPropName);
                    this.ApplyObservableList();
                }
            }
        }

        public Skin ItemSkin
        {
            get
            {
                return this.itemSkin;
            }

            set
            {
                if (this.itemSkin != value)
                {
                    this.itemSkin = value;
                    this.FirePropertyChanged(ListView.ItemSkinPropName);

                    if (this.IsActive)
                    {
                        var items = this.items;
                        int itemCount = items.Count;
                        for (int iItem = 0; iItem < itemCount; iItem++)
                        {
                            // Note that we use this.itemSkin intentionally
                            // the reason being we want to protect against recursive set of this
                            // property to different values.
                            items[iItem].Skin = this.itemSkin;
                        }
                    }
                }
            }
        }

        protected List<ListViewItem> Items
        { get { return this.items; } }

        protected ISelectionHelper SelectionHelper
        { get { return this.selectionHelper; } }

        /// <summary>
        /// Gets or sets the name of the item CSS class.
        /// </summary>
        /// <value>
        /// The name of the item CSS class.
        /// </value>
        [CssName]
        [DefaultDataBinding(Mode=Binders.DataBindingMode.OneTime)]
        public string ItemCssClassName
        {
            get { return this.itemCssClassName; }
            set { this.itemCssClassName = value; }
        }

        [DefaultDataBinding(Mode=Binders.DataBindingMode.OneTime, IsStrict=true, DefaultValue=false)]
        public bool InlineItems
        {
            get { return this.inlineItems; }
            set { this.inlineItems = value; }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (this.fixedList != null)
            {
                this.ApplyFixedList();
            }
            else if (this.observableList != null)
            {
                this.ApplyObservableList();
            }
        }

        protected override void OnDeactivate()
        {
            var items = this.items;
            int itemCount = items.Count;
            if (itemCount > 0)
            {
                for (int iItem = 0; iItem < itemCount; iItem++)
                {
                    items[iItem].Deactivate();
                }
            }

            base.OnDeactivate();
        }

        protected override void InternalDispose()
        {
            var items = this.items;
            int itemCount = items.Count;
            if (itemCount > 0)
            {
                for (int iItem = 0; iItem < itemCount; iItem++)
                {
                    items[iItem].Dispose();
                }

                items.Clear();
            }

            base.InternalDispose();
        }

        private void ApplyFixedList()
        {
            var items = this.items;
            int itemsCount = items.Count;
            if (this.fixedList == null)
            {
                for (int iItem = 0; iItem < itemsCount; iItem++)
                {
                    var item = items[iItem];
                    item.Dispose();
                    item.Element.Remove();
                }

                items.Clear();
                return;
            }

            if (this.IsActive)
            {
                var fixedList = this.fixedList;
                int fixedListCount = Math.Min(fixedList.Count, this.topN);
                for (int iObject = 0; iObject < fixedListCount; iObject++)
                {
                    ListViewItem listViewItem;
                    if (iObject < itemsCount)
                    {
                        listViewItem = items[iObject];
                        listViewItem.IsSelected = false;
                    }
                    else
                    {
                        listViewItem = new ListViewItem(this.CreateElement());

                        if (this.itemCssClassName != null)
                        {
                            listViewItem.Element.ClassName = this.itemCssClassName;
                        }

                        if (!this.inlineItems)
                        {
                            this.Element.AppendChild(listViewItem.Element);
                        }
                        else
                        {
                            this.Element.ParentNode.InsertBefore(
                                listViewItem.Element,
                                this.Element);
                        }
                        listViewItem.Skin = this.itemSkin;
                        items.Add(listViewItem);
                    }

                    listViewItem.DataContext = fixedList[iObject];
                    listViewItem.SelectionHelper = this.selectionHelper;
                    listViewItem.Activate();
                }

                this.RemoveChildren(fixedListCount, itemsCount - fixedListCount);
            }
        }

        private void ApplyObservableList()
        {
            var items = this.items;
            int itemsCount = items.Count;
            if (this.observableList == null)
            {
                for (int iItem = 0; iItem < itemsCount; iItem++)
                {
                    var item = items[iItem];
                    item.Dispose();
                    item.Element.Remove();
                }

                items.Clear();
                return;
            }

            if (this.IsActive
                && this.observableList != null
                && this.observableList != this.attachedObservableList)
            {
                this.attachedObservableList = this.observableList;
                this.attachedObservableList.CollectionChanged += this.ObservableListCollectionChanged;
                this.ResetObservableItems();
            }
        }

        void ObservableListCollectionChanged(
            INotifyCollectionChanged collection,
            CollectionChangedEventArgs args)
        {
            Debug.Assert(collection == this.attachedObservableList);
            var items = this.items;
            var changeIndex = args.ChangeIndex;

            if (args.Action == CollectionChangedAction.Reset)
            { this.ResetObservableItems(); }

            if (changeIndex > this.topN)
            { return; }

            var newItems = args.NewItems;
            var oldItems = args.OldItems;
            var itemCount = args.Action == CollectionChangedAction.Remove
                ? oldItems.Count
                : newItems.Count;
            switch (args.Action)
            {
                case CollectionChangedAction.Add:
                    if (changeIndex + itemCount + this.items.Count > this.topN)
                    {
                        if (this.items.Count >= this.topN)
                        {
                            this.ObservableEventReplace(
                                changeIndex,
                                this.topN - changeIndex,
                                newItems);
                        }
                        else
                        {
                            int addCount = this.topN - this.items.Count;
                            this.ObservableEventAdd(
                                changeIndex,
                                this.topN - this.items.Count,
                                newItems);

                            int replaceCount = this.topN - changeIndex - addCount;
                            List<object> list = new List<object>();
                            for (int i = addCount; i < replaceCount && i < itemCount; i++)
                            { list.Add(newItems[i]); }

                            this.ObservableEventAdd(
                                changeIndex + addCount,
                                replaceCount,
                                list);
                        }
                    }
                    else
                    {
                        this.ObservableEventAdd(
                            changeIndex,
                            itemCount,
                            newItems);
                    }
                    break;
                case CollectionChangedAction.Remove:
                    if (this.observableList.Count <= this.topN)
                    {
                        this.RemoveChildren(args.ChangeIndex, args.OldItems.Count);
                    }
                    else
                    {
                        List<object> replaceList = new List<object>();
                        int replaceStartIndex = changeIndex + itemCount;
                        int replaceCount =
                            Math.Min(
                                changeIndex + itemCount,
                                Math.Min(
                                    this.topN,
                                    this.observableList.Count - itemCount))
                            - changeIndex;

                        for (int i = 0; i < replaceCount; i++)
                        { replaceList.Add(this.observableList[replaceStartIndex + i]); }

                        this.ObservableEventReplace(changeIndex, replaceCount, replaceList);

                        if (this.observableList.Count - itemCount <= this.topN)
                        {
                            this.RemoveChildren(
                                changeIndex + replaceCount,
                                this.items.Count - changeIndex - replaceCount);
                        }

                        throw new Exception("Not Tested");
                    }
                    break;
                case CollectionChangedAction.Replace:
                    this.ObservableEventReplace(
                        changeIndex,
                        Math.Min(changeIndex + itemCount, this.topN) - changeIndex,
                        newItems);
                    break;
                default:
                    throw new Exception("Invalid operation");
            }
        }

        private void ObservableEventReplace(
            int changeIndex,
            int listCount,
            IList list)
        {
            for (int iObject = 0; iObject < listCount; iObject++)
            {
                items[changeIndex + iObject].DataContext = list[iObject];
            }
        }

        private void ObservableEventAdd(int changeIndex, int listCount, IList list)
        {
            Element insertBeforeElem = null;
            if (changeIndex < items.Count)
            {
                insertBeforeElem = items[changeIndex].Element;
            }

            for (int iObject = 0; iObject < listCount; iObject++)
            {
                ListViewItem listViewItem = new ListViewItem(
                        this.CreateElement());
                if (this.itemCssClassName != null)
                {
                    listViewItem.Element.ClassName = this.itemCssClassName;
                }

                listViewItem.Skin = this.itemSkin;

                if (insertBeforeElem == null)
                {
                    if (this.inlineItems)
                    {
                        this.Element.ParentNode.InsertBefore(
                            listViewItem.Element,
                            this.Element);
                    }
                    else
                    {
                        this.Element.AppendChild(listViewItem.Element);
                    }

                    items.Add(listViewItem);
                }
                else
                {
                    if (this.inlineItems)
                    {
                        this.Element.ParentNode.InsertBefore(
                            listViewItem.Element,
                            insertBeforeElem);
                    }
                    else
                    {
                        this.Element.InsertBefore(listViewItem.Element, insertBeforeElem);
                    }

                    items.Insert(changeIndex + iObject, listViewItem);
                }

                listViewItem.DataContext = list[iObject];
                listViewItem.SelectionHelper = this.selectionHelper;
                listViewItem.Activate();
            }
        }

        private void ResetObservableItems()
        {
            var observableList = this.observableList;
            var itemsCount = this.items.Count;
            int listCount = Math.Min(observableList.Count, this.topN);
            for (int iObject = 0; iObject < listCount; iObject++)
            {
                ListViewItem listViewItem;
                if (iObject < itemsCount)
                {
                    listViewItem = items[iObject];
                    listViewItem.IsSelected = false;
                }
                else
                {
                    listViewItem = new ListViewItem(this.CreateElement());
                    if (this.itemCssClassName != null)
                    {
                        listViewItem.Element.ClassName = this.itemCssClassName;
                    }

                    if (!this.inlineItems)
                    {
                        this.Element.AppendChild(listViewItem.Element);
                    }
                    else
                    {
                        this.Element.ParentNode.InsertBefore(
                            listViewItem.Element,
                            this.Element);
                    }

                    listViewItem.Skin = this.itemSkin;
                    items.Add(listViewItem);
                }

                listViewItem.DataContext = observableList[iObject];
                listViewItem.SelectionHelper = this.selectionHelper;
                listViewItem.Activate();
            }

            this.RemoveChildren(listCount, itemsCount - listCount);
        }

        private void RemoveChildren(int changeIndex, int delCount)
        {
            for (int iObject = delCount + changeIndex - 1; iObject >= changeIndex; iObject--)
            {
                var item = items[iObject];
                items.RemoveAt(iObject);
                item.Element.Remove();
                item.Dispose();
            }
        }

        private Element CreateElement()
        {
            return this.Element.OwnerDocument.CreateElement(this.inlineItems ? "div" : "li");
        }
    }
}

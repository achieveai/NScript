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
        private const string HeaderSkinPropName = "HeaderSkin";

        List<ListViewItem> items = new List<ListViewItem>();
        IObservableCollection observableList;
        IObservableCollection attachedObservableList;
        private IList fixedList;
        private ObservableCollectionGenerator<object, object> listToObservableList;
        private Skin headerSkin;
        string headerCssClassName;
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

        public Skin HeaderSkin
        {
            get { return this.headerSkin; }
            set
            {
                if (this.headerSkin == value)
                { return; }

                this.headerSkin = value;
                this.FirePropertyChanged(ListView.HeaderSkinPropName);

                if (this.IsActive)
                {
                    var items = this.items;
                    int itemCount = items.Count;
                    for (int iItem = 0; iItem < itemCount; iItem++)
                    {
                        var listViewItem = this.ResetSkin(
                            items[iItem],
                            items[iItem]);
                        items[iItem] = listViewItem;
                    }
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

        [CssName]
        [DefaultDataBinding(Mode = Binders.DataBindingMode.OneTime)]
        public string HeaderCssClassName
        {
            get { return this.headerCssClassName; }
            set { this.headerCssClassName = value; }
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

            foreach (var item in this.items)
            { item.Activate(); }
        }

        protected override void OnDeactivate()
        {
            foreach (var item in this.items)
            { item.Deactivate(); }

            base.OnDeactivate();
        }

        protected override void InternalDispose()
        {
            var items = this.items;
            int itemCount = items.Count;
            if (this.attachedObservableList != null)
            {
                this.attachedObservableList.CollectionChanged -= ObservableListCollectionChanged;
                this.attachedObservableList = null;
            }

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

        protected virtual ListViewItem CreateListViewItem()
        {
            return new ListViewItem(this.CreateElement());
        }

        private void ApplyFixedList()
        {
            var items = this.items;
            int itemsCount = items.Count;
            if (this.fixedList == null)
            {
                if (this.listToObservableList != null)
                {
                    this.listToObservableList.InputCollection = null;
                }
                else
                {
                    for (int iItem = 0; iItem < itemsCount; iItem++)
                    { RemoveChild(items[iItem]); }

                    items.Clear();
                }

                return;
            }

            if (this.IsActive
                && this.fixedList != null
                && (this.listToObservableList == null
                    || this.fixedList != this.listToObservableList.InputCollection))
            {
                if (this.listToObservableList == null)
                {
                    this.listToObservableList = new ObservableCollectionGenerator<object, object>((a) => a);
                    this.attachedObservableList = this.listToObservableList.OutputCollection;
                    this.attachedObservableList.CollectionChanged += this.ObservableListCollectionChanged;
                }

                var list = new List<object>();
                for (int idx = 0; idx < this.fixedList.Count; idx++)
                { list.Add(this.fixedList[idx]); }

                this.listToObservableList.InputCollection = list;
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
                    RemoveChild(items[iItem]);
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
                    if (changeIndex + itemCount + items.Count > this.topN)
                    {
                        if (items.Count >= this.topN)
                        {
                            this.ObservableEventReplace(
                                changeIndex,
                                this.topN - changeIndex,
                                newItems);
                        }
                        else
                        {
                            int addCount = this.topN - items.Count;
                            this.ObservableEventAdd(
                                changeIndex,
                                this.topN - items.Count,
                                newItems);

                            int replaceCount = this.topN - changeIndex - addCount;
                            List<object> list = new List<object>();
                            var itemsAdded = false;
                            for (int i = addCount; i < replaceCount && i < itemCount; i++)
                            {
                                list.Add(newItems[i]);
                                itemsAdded = true;
                            }

                            if (itemsAdded) {
                                this.ObservableEventAdd(
                                changeIndex + addCount,
                                replaceCount,
                                list);
                            }
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
                    if (this.attachedObservableList != null
                        && this.attachedObservableList.Count + itemCount <= this.topN)
                    { this.RemoveChildren(changeIndex, oldItems.Count); }
                    else
                    {
                        List<object> replaceList = new List<object>();
                        int replaceStartIndex = changeIndex + itemCount;
                        int replaceCount =
                            Math.Min(
                                changeIndex + itemCount,
                                Math.Min(
                                    this.topN,
                                    this.attachedObservableList.Count))
                            - changeIndex;

                        for (int i = 0; i < replaceCount; i++)
                        { replaceList.Add(this.attachedObservableList[replaceStartIndex + i]); }

                        this.ObservableEventReplace(changeIndex, replaceCount, replaceList);

                        if (this.attachedObservableList.Count <= this.topN)
                        {
                            this.RemoveChildren(
                                changeIndex + replaceCount,
                                items.Count - changeIndex - replaceCount);
                        }
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
                var listViewItem = this.ResetSkin(
                        items[changeIndex + iObject],
                        list[iObject]);
                items[changeIndex + iObject] = listViewItem;
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
                ListViewItem listViewItem = this.CreateListViewItem();
                if (this.itemCssClassName != null)
                {
                    listViewItem.Element.ClassName = this.itemCssClassName;
                }

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
                listViewItem = this.ResetSkin(
                    listViewItem,
                    list[iObject]);

                listViewItem.SelectionHelper = this.selectionHelper;
                ActivateChild(listViewItem);
            }
        }

        private ListViewItem ResetSkin(
            ListViewItem listViewItem,
            object dataItem)
        {
            bool hasHeaders = this.headerSkin != null;
            if (hasHeaders)
            {
                var headeredItem = (IHeaderedElement)dataItem;

                if (headeredItem.IsHeader)
                {
                    listViewItem.DataContext = headeredItem.Header;
                    listViewItem.Skin = this.headerSkin;
                    if (this.headerCssClassName != null)
                    {
                        listViewItem.Element.ClassName = this.headerCssClassName;
                    }
                }
                else
                {
                    listViewItem.DataContext = headeredItem.Item;
                    listViewItem.Skin = this.itemSkin;
                    if (this.itemCssClassName != null)
                    {
                        listViewItem.Element.ClassName = this.itemCssClassName;
                    }
                }
            }
            else
            {
                listViewItem.DataContext = dataItem;
                listViewItem.Skin = this.itemSkin;
                if (this.itemCssClassName != null)
                {
                    listViewItem.Element.ClassName = this.itemCssClassName;
                }
            }

            return listViewItem;

        }

        private void ResetObservableItems()
        {
            var observableList = this.attachedObservableList;
            var itemsCount = this.items.Count;
            int listCount = Math.Min(observableList.Count, this.topN);
            for (int iObject = 0; iObject < listCount; iObject++)
            {
                ListViewItem listViewItem;
                if (iObject < itemsCount)
                {
                    listViewItem = items[iObject];
                    listViewItem.IsSelected =
                        selectionHelper != null
                            ? selectionHelper.IsSelected(listViewItem.DataContext)
                            : false;
                }
                else
                {
                    listViewItem = this.CreateListViewItem();
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
                listViewItem = this.ResetSkin(
                        listViewItem,
                        observableList[iObject]);

                ActivateChild(listViewItem);
            }

            this.RemoveChildren(listCount, itemsCount - listCount);
        }

        private void RemoveChildren(int changeIndex, int delCount)
        {
            for (int iObject = delCount + changeIndex - 1; iObject >= changeIndex; iObject--)
            {
                var item = items[iObject];
                RemoveChild(items[iObject]);
                items.RemoveAt(iObject);
            }
        }

        private void ActivateChild(ListViewItem lvi)
        {
            if (this.IsActive)
            { lvi.Activate(); }
        }

        private void RemoveChild(ListViewItem lvi)
        {
            lvi.Dispose();
            lvi.Element.Remove();
            lvi.DataContext = null;
        }

        private Element CreateElement()
        {
            return this.Element.OwnerDocument.CreateElement(this.inlineItems ? "div" : "li");
        }
    }
}

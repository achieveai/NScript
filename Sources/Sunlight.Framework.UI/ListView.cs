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

        public ListView(Element element)
            : base(element)
        { }

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
                int fixedListCount = fixedList.Count;
                for (int iObject = 0; iObject < fixedListCount; iObject++)
                {
                    ListViewItem listViewItem;
                    if (iObject < itemsCount)
                    {
                        listViewItem = items[iObject];
                    }
                    else
                    {
                        listViewItem = new ListViewItem(
                            this.Element.OwnerDocument.CreateElement("li"));
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
            switch (args.Action)
            {
                case CollectionChangedAction.Add:
                    {
                        var list = args.NewItems;
                        var listCount = list.Count;
                        Element insertBeforeElem = null;
                        if (changeIndex < items.Count)
                        {
                            insertBeforeElem = items[changeIndex].Element;
                        }

                        for (int iObject = 0; iObject < listCount; iObject++)
                        {
                            ListViewItem listViewItem = new ListViewItem(
                                    this.Element.OwnerDocument.CreateElement("div"));
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
                            listViewItem.Activate();
                        }

                        break;
                    }
                case CollectionChangedAction.Remove:
                    {
                        this.RemoveChildren(changeIndex, args.OldItems.Count);
                        break;
                    }
                case CollectionChangedAction.Replace:
                    {
                        var list = args.NewItems;
                        var listCount = list.Count;

                        for (int iObject = 0; iObject < listCount; iObject++)
                        {
                            items[changeIndex + iObject].DataContext = list[iObject];
                        }

                        break;
                    }
                case CollectionChangedAction.Reset:
                    this.ResetObservableItems();
                    break;
                default:
                    throw new Exception("Invalid operation");
            }
        }

        private void ResetObservableItems()
        {
            var observableList = this.observableList;
            var itemsCount = this.items.Count;
            int listCount = observableList.Count;
            for (int iObject = 0; iObject < listCount; iObject++)
            {
                ListViewItem listViewItem;
                if (iObject < itemsCount)
                {
                    listViewItem = items[iObject];
                }
                else
                {
                    listViewItem = new ListViewItem(
                        this.Element.OwnerDocument.CreateElement("li"));
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
    }
}

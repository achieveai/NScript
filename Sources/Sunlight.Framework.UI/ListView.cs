//-----------------------------------------------------------------------
// <copyright file="ListView.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI
{
    using Sunlight.Framework.Observables;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Html;

    /// <summary>
    /// Definition for ListView
    /// </summary>
    public class ListView : UIElement
    {
        public const string FixedListPropName = "FixedList",
            ObservableListPropName = "ObservableList",
            ItemSkinPropName = "ItemSkin";

        List<ListViewItem> items = new List<ListViewItem>();
        IObservableCollection observableList;
        IList fixedList;
        Skin itemSkin;

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
                    this.observableList = value;
                    this.FirePropertyChanged(ListView.ObservableListPropName);
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
                    items[iItem].Dispose();
                }

                this.Element.ClearChildren();
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
                            this.Element.OwnerDocument.CreateElement("div"));
                        this.Element.AppendChild(listViewItem.Element);
                        listViewItem.Skin = this.itemSkin;
                        items.Add(listViewItem);
                    }

                    listViewItem.DataContext = fixedList[iObject];
                    listViewItem.Activate();
                }

                for (int iItem = itemsCount; iItem >= fixedListCount; iItem--)
                {
                    items[iItem].Dispose();
                    items.RemoveAt(iItem);
                }
            }
        }

        private void ApplyObservableList()
        {
            throw new Exception("Not Implemented");
        }
    }
}

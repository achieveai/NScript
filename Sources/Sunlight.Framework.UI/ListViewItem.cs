//-----------------------------------------------------------------------
// <copyright file="ListViewItem.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI
{
    using System;
    using System.Web.Html;

    /// <summary>
    /// Definition for ListViewItem
    /// </summary>
    public class ListViewItem : UISkinableElement
    {
        public const string IsSelectedPropName = "IsSelected",
            IsFocusedPropName = "IsFocused";

        private bool isSelected, isFocused;

        public ListViewItem(Element element)
            : base(element)
        {}

        /// <summary>
        /// Gets or sets a value indicating whether this object is selected.
        /// </summary>
        /// <value>
        /// true if this object is selected, false if not.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.FirePropertyChanged(ListViewItem.IsSelectedPropName);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this object is focused.
        /// </summary>
        /// <value>
        /// true if this object is focused, false if not.
        /// </value>
        public bool IsFocused
        {
            get
            {
                return this.isFocused;
            }

            set
            {
                if (this.isFocused != value)
                {
                    this.isFocused = value;
                    this.FirePropertyChanged(ListViewItem.IsFocusedPropName);
                }
            }
        }
    }
}

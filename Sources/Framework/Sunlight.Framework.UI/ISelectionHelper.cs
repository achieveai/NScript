//-----------------------------------------------------------------------
// <copyright file="ISelectionHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sunlight.Framework.UI
{
    using System;

    /// <summary>
    /// Definition for ISelectionHelper
    /// </summary>
    public interface ISelectionHelper
    {
        event Action SelectionChanged;
        bool IsSelected(object item);
        void SelectItem(object item);
        void UnSelectItem(object item);
    }
}

//-----------------------------------------------------------------------
// <copyright file="SelectElement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for SelectElement
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public sealed class SelectElement : Element
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="SelectElement"/> class from being created.
        /// </summary>
        private SelectElement() { }

        /// <summary>
        /// Options
        /// </summary>
        [IntrinsicField]
        public readonly NodeCollection Options;

        /// <summary>
        /// Can select multiple options.
        /// </summary>
        [IntrinsicField]
        public bool Multiple;

        /// <summary>
        /// Selected Index.
        /// </summary>
        [IntrinsicField]
        public int SelectedIndex;

        /// <summary>
        /// Size of options
        /// </summary>
        [IntrinsicField]
        public int Size;

        /// <summary>
        /// Adds an element to the end of the <see cref="Options"/> collection.
        /// </summary>
        /// <param name="option">Element to add to the <see cref="Options"/> collection.</param>
        public extern void Add(OptionElement option);

        /// <summary>
        /// Adds an element to the <see cref="Options"/> collection (IE only).
        /// </summary>
        /// <param name="option">Element to add to the <see cref="Options"/> collection.</param>
        /// <param name="index">Specifies the index position in the collection where the element is placed.</param>
        public extern void Add(OptionElement option, int index);

        /// <summary>
        /// Adds an element to the <see cref="Options"/> collection (Firefox only).
        /// </summary>
        /// <param name="option">Element to add to the <see cref="Options"/> collection.</param>
        /// <param name="before">The element before which to add <paramref name="option"/>.</param>
        public extern void Add(OptionElement option, OptionElement before);

        /// <summary>
        /// Removes an <see cref="OptionElement"/> from the <see cref="Options"/> collection.
        /// </summary>
        /// <param name="index"></param>
        public extern void Remove(int index);
    }
}
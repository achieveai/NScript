//-----------------------------------------------------------------------
// <copyright file="DocumentFragment.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for DocumentFragment.
    /// </summary>
    [IgnoreNamespace]
    public class DocumentFragment
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal extern DocumentFragment();

        /// <summary>
        /// Gets the first child.
        /// </summary>
        /// <value>
        /// The first child.
        /// </value>
        public extern Element FirstChild
        { get; }

        /// <summary>
        /// Gets the last child.
        /// </summary>
        /// <value>
        /// The last child.
        /// </value>
        public extern Element LastChild
        { get; }

        /// <summary>
        /// Gets the next sibling.
        /// </summary>
        /// <value>
        /// The next sibling.
        /// </value>
        public extern Element NextSibling
        { get; }

        /// <summary>
        /// Gets the name of the node.
        /// </summary>
        /// <value>
        /// The name of the node.
        /// </value>
        public extern string NodeName
        { get; }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>
        /// The type of the node.
        /// </value>
        public extern int NodeType
        { get; }

        /// <summary>
        /// Gets the node value.
        /// </summary>
        /// <value>
        /// The node value.
        /// </value>
        public extern string NodeValue
        { get; }

        /// <summary>
        /// Gets the document that owns this item.
        /// </summary>
        /// <value>
        /// The owner document.
        /// </value>
        public extern Document OwnerDocument
        { get; }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <value>
        /// The parent node.
        /// </value>
        public extern Element ParentNode
        { get; }

        /// <summary>
        /// Gets the previous sibling.
        /// </summary>
        /// <value>
        /// The previous sibling.
        /// </value>
        public extern Element PreviousSibling
        { get; }

        /// <summary>
        /// Returns ElementAttribute[] but in nativeFormat.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        private extern NativeArray<NodeAttribute> Attributes
        { get; }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        /// <value>
        /// The child nodes.
        /// </value>
        private extern NativeArray<Element> ChildNodes
        { get; }

        /// <summary>
        /// Appends a child.
        /// </summary>
        /// <param name="child"> The child. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Element AppendChild(Element child);

        /// <summary>
        /// Gets the clone node.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        public extern DocumentFragment CloneNode();

        /// <summary>
        /// Clone node.
        /// </summary>
        /// <param name="deep"> true to deep. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Element CloneNode(bool deep);

        /// <summary>
        /// Query if this object contains the given element.
        /// </summary>
        /// <param name="element"> The Element to test for containment. </param>
        /// <returns>
        /// true if the object is in this collection, false if not.
        /// </returns>
        public extern bool Contains(Element element);

        /// <summary>
        /// Gets an element by identifier.
        /// </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns>
        /// The element by identifier.
        /// </returns>
        public extern Element GetElementById(string id);

        /// <summary>
        /// Query if this object has attributes.
        /// </summary>
        /// <returns>
        /// true if attributes, false if not.
        /// </returns>
        public extern bool HasAttributes();

        /// <summary>
        /// Query if this object has child nodes.
        /// </summary>
        /// <returns>
        /// true if child nodes, false if not.
        /// </returns>
        public extern bool HasChildNodes();

        /// <summary>
        /// Inserts a before.
        /// </summary>
        /// <param name="newChild">       The new child. </param>
        /// <param name="referenceChild"> The reference child. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Element InsertBefore(Element newChild, Element referenceChild);

        /// <summary>
        /// Removes the child described by child.
        /// </summary>
        /// <param name="child"> The child. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Element RemoveChild(Element child);

        /// <summary>
        /// Replace child.
        /// </summary>
        /// <param name="newChild"> The new child. </param>
        /// <param name="oldChild"> The old child. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Element ReplaceChild(Element newChild, Element oldChild);

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <returns>
        /// The attributes.
        /// </returns>
        public DomList<NodeAttribute> GetAttributes()
        {
            return new DomList<NodeAttribute>(this.Attributes);
        }

        /// <summary>
        /// Gets a child nodes.
        /// </summary>
        /// <returns>
        /// The child nodes.
        /// </returns>
        public DomList<Element> GetChildNodes()
        {
            return new DomList<Element>(this.ChildNodes);
        }

        /// <summary>
        /// Gets the elements by tag name.
        /// </summary>
        /// <param name="tagName"> Name of the tag. </param>
        /// <returns>
        /// The elements by tag name.
        /// </returns>
        public DomList<Element> GetElementsByTagName(string tagName)
        {
            return new DomList<Element>(this.GetElementsByTagNameInternal(tagName));
        }

        /// <summary>
        /// Gets the elements by tag name internal.
        /// </summary>
        /// <param name="tagName"> Name of the tag. </param>
        /// <returns>
        /// The elements by tag name internal.
        /// </returns>
        [ScriptName("GetElementsByTagName")]
        private extern NativeArray<Element> GetElementsByTagNameInternal(string tagName);
    }
}
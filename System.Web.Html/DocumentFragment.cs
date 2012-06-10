//-----------------------------------------------------------------------
// <copyright file="DocumentFragment.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for DocumentFragment
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public class DocumentFragment
    {
        internal DocumentFragment() { }

        [IntrinsicField]
        public readonly Element FirstChild;

        [IntrinsicField]
        public readonly Element LastChild;

        [IntrinsicField]
        public readonly Element NextSibling;

        [IntrinsicField]
        public readonly string NodeName;

        [IntrinsicField]
        public readonly int NodeType;

        [IntrinsicField]
        public readonly string NodeValue;

        [IntrinsicField]
        public readonly Document OwnerDocument;

        [IntrinsicField]
        public readonly Element ParentNode;

        [IntrinsicField]
        public readonly Element PreviousSibling;

        /// <summary>
        /// Returns ElementAttribute[] but in nativeFormat.
        /// </summary>
        [IntrinsicField]
        private readonly NativeArray Attributes;

        [IntrinsicField]
        private readonly NativeArray ChildNodes;

        public extern Element AppendChild(Element child);

        public extern DocumentFragment CloneNode();

        public extern Element CloneNode(bool deep);

        public extern bool Contains(Element element);

        public extern Element GetElementById(string id);

        public extern bool HasAttributes();

        public extern bool HasChildNodes();

        public extern Element InsertBefore(Element newChild);

        public extern Element InsertBefore(Element newChild, Element referenceChild);

        public extern Element RemoveChild(Element child);

        public extern Element ReplaceChild(Element newChild, Element oldChild);

        public DomList<NodeAttribute> GetAttributes()
        {
            return new DomList<NodeAttribute>(this.Attributes);
        }

        public DomList<Element> GetChildNodes()
        {
            return new DomList<Element>(this.ChildNodes);
        }

        public DomList<Element> GetElementsByTagName(string tagName)
        {
            return new DomList<Element>(this.GetElementsByTagNameInternal(tagName));
        }

        [ScriptName("GetElementsByTagName")]
        private extern NativeArray GetElementsByTagNameInternal(string tagName);
    }
}
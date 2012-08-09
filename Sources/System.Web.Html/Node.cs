//-----------------------------------------------------------------------
// <copyright file="Node.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Node
    /// </summary>
    [Imported]
    [IgnoreNamespace]
    public class Node
    {
        internal Node() { }

        [IntrinsicField]
        public readonly string BaseURI;

        [IntrinsicField]
        public readonly Node FirstChild;

        [IntrinsicField]
        public readonly Node LastChild;

        [IntrinsicField]
        public readonly Node NextSibling;

        [IntrinsicField]
        public readonly Node PreviousSibling;

        [IntrinsicField]
        public readonly string LocalName;

        [IntrinsicField]
        public readonly string NamespaceURI;

        [IntrinsicField]
        public readonly string NodeName;

        [IntrinsicField]
        public readonly ElementType NodeType;

        [IntrinsicField]
        public readonly string NodeValue;

        [IntrinsicField]
        public readonly Document OwnerDocument;

        [IntrinsicField]
        public readonly Node ParentNode;

        [IntrinsicField]
        public string TextContent;

        /// <summary>
        /// Returns NodeAttribute[] but in nativeFormat.
        /// </summary>
        [IntrinsicField]
        private readonly NativeArray Attributes;

        [IntrinsicField]
        private readonly NativeArray ChildNodes;

        [IntrinsicField]
        private readonly NativeArray Children;

        public extern string GetAttribute(string name);

        public extern NodeAttribute GetAttributeNode(string name);

        public extern bool HasAttribute(string name);

        public extern bool HasChildNodes();

        public extern Node InsertBefore(Node newChild);

        public extern Node InsertBefore(Node newChild, Node referenceChild);

        public extern bool RemoveAttribute(string name);

        public extern NodeAttribute RemoveAttributeNode(NodeAttribute attribute);

        public extern Node RemoveChild(Node child);

        public extern Node ReplaceChild(Node newChild, Node oldChild);

        public extern void SetAttribute(string name, string value);

        public extern NodeAttribute SetAttributeNode(NodeAttribute attribute);

        [IgnoreGenericArguments]
        [Script("return this;")]
        public extern TNode As<TNode>() where TNode : Node;

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <returns>Attributes list</returns>
        public DomList<NodeAttribute> GetAttributes()
        {
            return new DomList<NodeAttribute>(this.Attributes);
        }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        /// <returns>Child nodes.</returns>
        public DomList<Node> GetChildNodes()
        {
            return new DomList<Node>(this.ChildNodes);
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns>Children collection.</returns>
        public DomList<Node> GetChildren()
        {
            return new DomList<Node>(this.Children);
        }
    }
}
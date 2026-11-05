//-----------------------------------------------------------------------
// <copyright file="Node.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Node.
    /// </summary>
    [IgnoreNamespace]
    public class Node
    {
        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        internal extern Node();

        /// <summary>
        /// Gets URI of the base.
        /// </summary>
        /// <value>
        /// The base uri.
        /// </value>
        public extern string BaseURI
        { get; }

        /// <summary>
        /// Gets the first child.
        /// </summary>
        /// <value>
        /// The first child.
        /// </value>
        public extern Node FirstChild
        { get; }

        /// <summary>
        /// Gets the last child.
        /// </summary>
        /// <value>
        /// The last child.
        /// </value>
        public extern Node LastChild
        { get; }

        /// <summary>
        /// Gets the next sibling.
        /// </summary>
        /// <value>
        /// The next sibling.
        /// </value>
        public extern Node NextSibling
        { get; }

        /// <summary>
        /// Gets the previous sibling.
        /// </summary>
        /// <value>
        /// The previous sibling.
        /// </value>
        public extern Node PreviousSibling
        { get; }

        /// <summary>
        /// Gets the name of the local.
        /// </summary>
        /// <value>
        /// The name of the local.
        /// </value>
        public extern string LocalName
        { get; }

        /// <summary>
        /// Gets URI of the namespace.
        /// </summary>
        /// <value>
        /// The namespace uri.
        /// </value>
        public extern string NamespaceURI
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
        public extern ElementType NodeType
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
        public extern Node ParentNode
        { get; }

        /// <summary>
        /// Gets or sets the text content.
        /// </summary>
        /// <value>
        /// The text content.
        /// </value>
        public extern string TextContent
        { get; set; }

        /// <summary>
        /// Returns NodeAttribute[] but in nativeFormat.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public extern NativeArray<NodeAttribute> Attributes
        { get; }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        /// <value>
        /// The child nodes.
        /// </value>
        public extern NativeArray<Node> ChildNodes
        { get; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public extern NativeArray<Node> Children
        { get; }

        /// <summary>
        /// Gets an attribute.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// The attribute.
        /// </returns>
        public extern string GetAttribute(string name);

        /// <summary>
        /// Gets an attribute node.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// The attribute node.
        /// </returns>
        public extern NodeAttribute GetAttributeNode(string name);

        /// <summary>
        /// Query if 'name' has attribute.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// true if attribute, false if not.
        /// </returns>
        public extern bool HasAttribute(string name);

        /// <summary>
        /// Query if this object has child nodes.
        /// </summary>
        /// <returns>
        /// true if child nodes, false if not.
        /// </returns>
        public extern bool HasChildNodes();

        /// <summary>
        /// Inserts a before described by newChild.
        /// </summary>
        /// <param name="newChild"> The new child. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Node InsertBefore(Node newChild);

        /// <summary>
        /// Inserts a before.
        /// </summary>
        /// <param name="newChild">       The new child. </param>
        /// <param name="referenceChild"> The reference child. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Node InsertBefore(Node newChild, Node referenceChild);

        /// <summary>
        /// Gets a value indicating whether the node is attached to a document.
        /// </summary>
        public extern bool IsConnected
        { get; }

        /// <summary>
        /// Gets a value indicating whether the browser has <c>moveBefore</c> (Chromium 133+).
        /// A move with <c>moveBefore</c> keeps the node's state, so an iframe does not reload.
        /// </summary>
        public static extern bool SupportsMoveBefore
        {
            [Script("return typeof @:Element !== 'undefined' && typeof @:Element.prototype.moveBefore === 'function';")]
            get;
        }

        /// <summary>
        /// Moves <paramref name="node"/> before <paramref name="child"/> and keeps its state.
        /// Throws when the browser has no <c>moveBefore</c> (see <see cref="SupportsMoveBefore"/>)
        /// or when either node is not connected to the same document.
        /// </summary>
        /// <param name="node">  The node to move. </param>
        /// <param name="child"> The reference child, or null to move to the end. </param>
        public extern void MoveBefore(Node node, Node child);

        /// <summary>
        /// Moves <paramref name="node"/> into <paramref name="parent"/> before <paramref name="child"/>.
        /// Uses <c>moveBefore</c> when the browser has it and both nodes are connected to the
        /// same document and the move succeeds; otherwise falls back to <c>insertBefore</c>, which resets state
        /// (an iframe reloads).
        /// </summary>
        /// <param name="parent"> The new parent. </param>
        /// <param name="node">   The node to move. </param>
        /// <param name="child">  The reference child, or null to move to the end. </param>
        /// <returns>
        /// true if the move kept the node's state (moveBefore); false if it used insertBefore.
        /// </returns>
        public static bool MoveOrInsertBefore(Node parent, Node node, Node child)
        {
            if (Node.SupportsMoveBefore
                && parent.IsConnected
                && node.IsConnected
                && parent.OwnerDocument == node.OwnerDocument)
            {
                try
                {
                    parent.MoveBefore(node, child);
                    return true;
                }
                catch
                {
                    // moveBefore refuses some moves (e.g. across shadow roots); insert instead.
                }
            }

            parent.InsertBefore(node, child);
            return false;
        }

        /// <summary>
        /// Removes the attribute described by name.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public extern bool RemoveAttribute(string name);

        /// <summary>
        /// Removes the attribute node described by attribute.
        /// </summary>
        /// <param name="attribute"> The attribute. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern NodeAttribute RemoveAttributeNode(NodeAttribute attribute);

        /// <summary>
        /// Clears the children.
        /// </summary>
        [Script("var tmp; while((tmp = this.firstChild)) { this.removeChild(tmp); }")]
        public extern void ClearChildren();

        /// <summary>
        /// Removes this object.
        /// </summary>
        /// <returns>
        /// Node itself.
        /// </returns>
        [Script("return this.parentNode ? this.parentNode.removeChild(this) : this;")]
        public extern Node Remove();

        /// <summary>
        /// Removes the child described by child.
        /// </summary>
        /// <param name="child"> The child. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Node RemoveChild(Node child);

        /// <summary>
        /// Replace child.
        /// </summary>
        /// <param name="newChild"> The new child. </param>
        /// <param name="oldChild"> The old child. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern Node ReplaceChild(Node newChild, Node oldChild);

        /// <summary>
        /// Sets an attribute.
        /// </summary>
        /// <param name="name">  The name. </param>
        /// <param name="value"> The value. </param>
        public extern void SetAttribute(string name, string value);

        /// <summary>
        /// Sets an attribute node.
        /// </summary>
        /// <param name="attribute"> The attribute. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern NodeAttribute SetAttributeNode(NodeAttribute attribute);

        /// <summary>
        /// Gets as.
        /// </summary>
        /// <typeparam name="TNode"> Type of the node. </typeparam>
        /// <returns>
        /// .
        /// </returns>
        [IgnoreGenericArguments]
        [Script("return this;")]
        public extern TNode As<TNode>() where TNode : Node;
    }
}
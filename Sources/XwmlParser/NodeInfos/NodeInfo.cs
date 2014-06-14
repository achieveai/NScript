//-----------------------------------------------------------------------
// <copyright file="NodeInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace XwmlParser.NodeInfos
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Definition for NodeInfo
    /// </summary>
    public abstract class NodeInfo
    {
        /// <summary>
        /// The node.
        /// </summary>
        private HtmlNode node;

        /// <summary>
        /// Information describing the tag.
        /// </summary>
        private Tuple<string, string> tagInfo;

        /// <summary>
        /// The parent node.
        /// </summary>
        NodeInfo parentNode;

        /// <summary>
        /// The bindings.
        /// </summary>
        private List<BinderInfo> bindings = new List<BinderInfo>();

        /// <summary>
        /// The public bindings.
        /// </summary>
        private ReadOnlyCollection<BinderInfo> publicBindings;

        /// <summary>
        /// The child node infos.
        /// </summary>
        private List<NodeInfo> childNodeInfos = new List<NodeInfo>();

        /// <summary>
        /// Identifier for the part.
        /// </summary>
        private string partId;

        /// <summary>
        /// Specialized constructor for use only by derived classes.
        /// </summary>
        /// <param name="node">     The node. </param>
        /// <param name="tagInfo">  Information describing the tag. </param>
        protected NodeInfo(HtmlNode node, Tuple<string, string> tagInfo)
        {
            this.node = node;
            this.tagInfo = tagInfo;
            this.publicBindings = new ReadOnlyCollection<BinderInfo>(this.bindings);
        }

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>
        /// The node.
        /// </value>
        public HtmlNode Node
        { get { return this.node; } }

        /// <summary>
        /// Gets or sets the identifier of the part.
        /// </summary>
        /// <value>
        /// The identifier of the part.
        /// </value>
        public string PartId
        {
            get { return this.partId; }
            protected set { this.partId = value; }
        }

        /// <summary>
        /// Gets the bindings.
        /// </summary>
        /// <value>
        /// The bindings.
        /// </value>
        public IList<BinderInfo> Bindings
        { get { return this.publicBindings; } }

        /// <summary>
        /// Gets the information describing the tag.
        /// </summary>
        /// <value>
        /// Information describing the tag.
        /// </value>
        public Tuple<string, string> TagInfo
        { get { return this.tagInfo; } }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <value>
        /// The parent node.
        /// </value>
        public NodeInfo ParentNode
        { get { return this.parentNode; } }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        /// <value>
        /// The child nodes.
        /// </value>
        public IList<NodeInfo> ChildNodes
        { get { return this.childNodeInfos; } }

        /// <summary>
        /// Parse node.
        /// </summary>
        /// <param name="parser"> The parser. </param>
        public abstract void ParseNode(
            TemplateParser parser);

        /// <summary>
        /// Generates a code.
        /// </summary>
        /// <exception cref="NotImplementedException"> Thrown when the requested operation is
        ///     unimplemented. </exception>
        /// <param name="codeGenerator"> The code generator. </param>
        public virtual void GenerateCode(SkinCodeGenerator codeGenerator)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process the child node described by childNode.
        /// </summary>
        /// <param name="childNode"> The child node. </param>
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public virtual bool ProcessChildNode(NodeInfo childNode)
        {
            return false;
        }

        /// <summary>
        /// Adds a child node information.
        /// </summary>
        /// <param name="nodeInfo"> Information describing the node. </param>
        protected void AddChildNodeInfo(NodeInfo nodeInfo)
        {
            if (nodeInfo.parentNode != null)
            {
                throw new InvalidOperationException();
            }

            nodeInfo.parentNode = this;
            this.childNodeInfos.Add(nodeInfo);
        }

        /// <summary>
        /// Adds a binder.
        /// </summary>
        /// <param name="binder"> The binder. </param>
        protected void AddBinder(BinderInfo binder)
        {
            if (this.bindings == null)
            {
                this.bindings = new List<BinderInfo>();
            }

            if (binder == null)
            {
                throw new InvalidOperationException();
            }

            bindings.Add(binder);
        }

        /// <summary>
        /// Generates a binding code.
        /// </summary>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        /// <param name="objectIndex">   Zero-based index of the object. </param>
        /// <param name="codeGenerator"> The code generator. </param>
        protected void GenerateBindingCode(
            int objectIndex,
            SkinCodeGenerator codeGenerator)
        {
            if (objectIndex < 0)
            {
                throw new InvalidOperationException();
            }

            for (int iBinder = 0; iBinder < this.bindings.Count; iBinder++)
            {
                int bindingObjectIndex =
                    this.GetObjectIndexForBinding(
                        codeGenerator,
                        this.bindings[iBinder]) ?? objectIndex;

                codeGenerator.AddBinding(
                    this.bindings[iBinder].GenerateCode(
                        bindingObjectIndex,
                        codeGenerator,
                        this));
            }
        }

        /// <summary>
        /// Gets object index for binding.
        /// </summary>
        /// <param name="binding"> The binding. </param>
        /// <returns>
        /// The object index for binding.
        /// </returns>
        protected virtual int? GetObjectIndexForBinding(
            SkinCodeGenerator codeGenerator,
            BinderInfo binding)
        {
            return null;
        }
    }
}

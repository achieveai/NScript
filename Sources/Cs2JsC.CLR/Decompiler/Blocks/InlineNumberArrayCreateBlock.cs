//-----------------------------------------------------------------------
// <copyright file="InlineNumberArrayCreateBlock.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.Decompiler.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Mono.Cecil;

    /// <summary>
    /// Definition for InlineNumberArrayCreateBlock
    /// </summary>
    internal class InlineNumberArrayCreateBlock : BasicBlock
    {
        /// <summary>
        /// Backing field for paramDef to Expresssion creator functions.
        /// </summary>
        private readonly static Dictionary<MetadataType, Func<BinaryReader, AST.Expression>> expressionCreator
            = InlineNumberArrayCreateBlock.GetCreators();

        /// <summary>
        /// Backing field for ElementType
        /// </summary>
        private readonly TypeReference arrayElementType;

        /// <summary>
        /// Backing field for ArraySize.
        /// </summary>
        private readonly int arraySize;

        /// <summary>
        /// Backing field for Data.
        /// </summary>
        private readonly byte[] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineNumberArrayCreateBlock"/> class.
        /// </summary>
        /// <param name="arrayElementType">Type of the array element.</param>
        /// <param name="arraySize">Size of the array.</param>
        /// <param name="arrayData">The array data.</param>
        /// <param name="childBlocks">The child blocks.</param>
        public InlineNumberArrayCreateBlock(
            TypeReference arrayElementType,
            int arraySize,
            byte[] arrayData,
            params Block[] childBlocks)
            : base(childBlocks)
        {
            this.arrayElementType = arrayElementType;
            this.arraySize = arraySize;
            this.data = arrayData;
        }

        /// <summary>
        /// Converts current block to AST node.
        /// </summary>
        /// <param name="variableResolver">The variable resolver.</param>
        /// <returns>
        /// AST Node representing current block.
        /// </returns>
        public override AST.Node ToAstNode(VariableResolver variableResolver)
        {
            AST.Expression[] elements = new AST.Expression[this.arraySize];
            Func<BinaryReader, AST.Expression> creator =
                InlineNumberArrayCreateBlock.expressionCreator[this.arrayElementType.MetadataType];
            using (MemoryStream stream = new MemoryStream(this.data, false))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                for (int index = 0; index < this.arraySize; index++)
                {
                    elements[index] = creator(reader);
                }
            }

            return new AST.InlineArrayInitialization(
                this.Context.ClrContext,
                this.ComputeLocation(),
                this.arrayElementType,
                elements);
        }

        /// <summary>
        /// Gets the creators.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<MetadataType, Func<BinaryReader, AST.Expression>> GetCreators()
        {
            Dictionary<MetadataType, Func<BinaryReader, AST.Expression>> rv =
                new Dictionary<MetadataType, Func<BinaryReader, AST.Expression>>();

            rv.Add(
                MetadataType.SByte,
                (stream) => new AST.IntLiteral(null, null, stream.ReadSByte()));

            rv.Add(
                MetadataType.Int16,
                (stream) => new AST.IntLiteral(null, null, stream.ReadInt16()));

            rv.Add(
                MetadataType.Int32,
                (stream) => new AST.IntLiteral(null, null, stream.ReadInt32()));

            rv.Add(
                MetadataType.Int64,
                (stream) => new AST.IntLiteral(null, null, stream.ReadInt64()));

            rv.Add(
                MetadataType.IntPtr,
                (stream) => new AST.IntLiteral(null, null, stream.ReadInt32()));

            rv.Add(
                MetadataType.Byte,
                (stream) => new AST.UIntLiteral(null, null, stream.ReadByte()));

            rv.Add(
                MetadataType.UInt16,
                (stream) => new AST.UIntLiteral(null, null, stream.ReadUInt16()));

            rv.Add(
                MetadataType.UInt32,
                (stream) => new AST.UIntLiteral(null, null, stream.ReadUInt32()));

            rv.Add(
                MetadataType.UInt64,
                (stream) => new AST.UIntLiteral(null, null, stream.ReadUInt64()));

            rv.Add(
                MetadataType.UIntPtr,
                (stream) => new AST.UIntLiteral(null, null, stream.ReadUInt32()));

            rv.Add(
                MetadataType.Boolean,
                (stream) => new AST.BooleanLiteral(null, null, stream.ReadBoolean()));

            rv.Add(
                MetadataType.Char,
                (stream) => new AST.CharLiteral(null, null, stream.ReadChar()));

            rv.Add(
                MetadataType.Single,
                (stream) => new AST.DoubleLiteral(null, null, stream.ReadSingle()));

            rv.Add(
                MetadataType.Double,
                (stream) => new AST.DoubleLiteral(null, null, stream.ReadDouble()));

            return rv;
        }
    }
}

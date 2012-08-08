//-----------------------------------------------------------------------
// <copyright file="UIntLiteral.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System;
    using Cs2JsC.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for UIntLiteral
    /// </summary>
    public class UIntLiteral : LiteralExpression
    {
        /// <summary>
        /// Backing field for Value.
        /// </summary>
        private readonly ulong intValue;

        /// <summary>
        /// Backing field for IntSize.
        /// </summary>
        private readonly IntSize intSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIntLiteral"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        public UIntLiteral(
            ClrContext context,
            Location location,
            byte value)
            : base(context, location)
        {
            this.intSize = IntSize.I8;
            this.intValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIntLiteral"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        public UIntLiteral(
            ClrContext context,
            Location location,
            ushort value)
            : base(context, location)
        {
            this.intSize = IntSize.I16;
            this.intValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIntLiteral"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        public UIntLiteral(
            ClrContext context,
            Location location,
            uint value)
            : base(context, location)
        {
            this.intSize = IntSize.I32;
            this.intValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIntLiteral"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        public UIntLiteral(
            ClrContext context,
            Location location,
            ulong value)
            : base(context, location)
        {
            this.intSize = IntSize.I64;
            this.intValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIntLiteral"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="value">The value.</param>
        public UIntLiteral(
            ClrContext context,
            Location location,
            UIntPtr value)
            : base(context, location)
        {
            this.intSize = IntSize.Ptr;
            this.intValue = (ulong)value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public ulong Value
        {
            get
            {
                return this.intValue;
            }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public IntSize Size
        {
            get
            {
                return this.intSize;
            }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get
            {
                switch (this.Size)
                {
                    case IntSize.I8:
                        return this.KnownReferences.Byte;
                    case IntSize.I16:
                        return this.KnownReferences.UShort;
                    case IntSize.I32:
                        return this.KnownReferences.UInt32;
                    case IntSize.I64:
                        return this.KnownReferences.UInt64;
                    case IntSize.Ptr:
                        return this.KnownReferences.UIntPtr;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("value", this.Value);
            serializationInfo.AddValue("size", this.Size);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            UIntLiteral right = obj as UIntLiteral;

            return right != null
                && this.Size == right.Size
                && this.Value == right.Value;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode() ^ (int)this.Size;
        }
    }
}

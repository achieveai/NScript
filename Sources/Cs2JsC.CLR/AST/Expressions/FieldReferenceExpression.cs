//-----------------------------------------------------------------------
// <copyright file="FieldReferenceExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System;
    using Cs2JsC.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for FieldReferenceExpression
    /// </summary>
    public class FieldReferenceExpression : MemberReferenceExpression
    {
        /// <summary>
        /// Backing field for FieldReference.
        /// </summary>
        private readonly FieldReference fieldReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="fieldReference">The field reference.</param>
        /// <param name="leftExpression">The left expression.</param>
        public FieldReferenceExpression(
            ClrContext context,
            Location location,
            FieldReference fieldReference,
            Expression leftExpression)
            : base(context, location, fieldReference, leftExpression)
        {
            if (fieldReference.Resolve().IsStatic ||
                fieldReference.Resolve().HasConstant)
            {
                throw new ArgumentException("static member can't have LeftExpression");
            }

            this.fieldReference = fieldReference;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="fieldReference">The field reference.</param>
        public FieldReferenceExpression(
            ClrContext context,
            Location location,
            FieldReference fieldReference)
            : base(context, location, fieldReference, null)
        {
            if (!fieldReference.Resolve().IsStatic &&
                !fieldReference.Resolve().HasConstant)
            {
                throw new ArgumentException("leftExpression not passed for instance member");
            }

            this.fieldReference = fieldReference;
        }

        /// <summary>
        /// Gets the field reference.
        /// </summary>
        /// <value>The field reference.</value>
        public FieldReference FieldReference
        {
            get
            {
                return this.fieldReference;
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
                return this.fieldReference.Resolve().FieldType.FixGenericTypeArguments(
                    this.fieldReference.DeclaringType);
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("fieldName", this.fieldReference.Name);
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
            FieldReferenceExpression right = obj as FieldReferenceExpression;

            return right != null
                && object.Equals(this.LeftExpression, right.LeftExpression)
                && this.FieldReference.Equals(right.FieldReference);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(FieldReferenceExpression).GetHashCode()
                ^ this.FieldReference.GetHashCode();
        }
    }
}
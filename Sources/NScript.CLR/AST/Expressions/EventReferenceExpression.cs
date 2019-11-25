//-----------------------------------------------------------------------
// <copyright file="PropertyReferenceExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using NScript.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for PropertyReferenceExpression
    /// </summary>
    public class EventReferenceExpression : MemberReferenceExpression
    {
        /// <summary>
        /// Backing field for FieldReference.
        /// </summary>
        private readonly EventReference eventReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="eventReference">The property reference.</param>
        /// <param name="leftExpression">The left expression.</param>
        /// <param name="arguments">The arguments.</param>
        public EventReferenceExpression(
            ClrContext context,
            Location location,
            EventReference eventReference,
            Expression leftExpression)
            : base(context, location, eventReference, leftExpression)
        {
            if (eventReference.Resolve().IsStatic())
            {
                throw new ArgumentException("static member can't have LeftExpression");
            }

            this.eventReference = eventReference;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldReferenceExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="eventReference">The property reference.</param>
        public EventReferenceExpression(
            ClrContext context,
            Location location,
            EventReference eventReference)
            : base(context, location, eventReference, null)
        {
            if (!eventReference.Resolve().IsStatic())
            {
                throw new ArgumentException("leftExpression not passed for instance member");
            }

            this.eventReference = eventReference;
        }

        /// <summary>
        /// Gets the property reference.
        /// </summary>
        /// <value>The property reference.</value>
        public EventReference EventReference
        {
            get
            {
                return this.eventReference;
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
                return this.eventReference.EventType.FixGenericTypeArguments(
                    this.eventReference.DeclaringType);
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            base.Serialize(serializationInfo);
            serializationInfo.AddValue("propertyName", this.EventReference.Name);
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
            EventReferenceExpression right = obj as EventReferenceExpression;

            if (right == null
                || !this.EventReference.Equals(right.EventReference)
                || !object.Equals(this.LeftExpression, right.LeftExpression))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(EventReferenceExpression).GetHashCode()
                ^ this.EventReference.GetHashCode();
        }
    }
}
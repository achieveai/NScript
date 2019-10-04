//-----------------------------------------------------------------------
// <copyright file="InlinePropertyInitilizationExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Mono.Cecil;
    using NScript.Utils;

    /// <summary>
    /// Definition for InlinePropertyInitilizationExpression
    /// </summary>
    public class InlinePropertyInitilizationExpression : Expression
    {
        /// <summary>
        /// Backing collection for PropertySetters;
        /// </summary>
        private readonly List<Tuple<MemberReferenceExpression, Expression[]>> setters;

        /// <summary>
        /// Backing field for Setters.
        /// </summary>
        private readonly ReadOnlyCollection<Tuple<MemberReferenceExpression, Expression[]>> readonlySetters;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlinePropertyInitilizationExpression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="locationInfo">The location info.</param>
        /// <param name="newObjectExpression">The new object expression.</param>
        /// <param name="setters">The setters.</param>
        public InlinePropertyInitilizationExpression(
            ClrContext context,
            Location locationInfo,
            NewObjectExpression newObjectExpression,
            List<Tuple<MemberReferenceExpression, Expression[]>> setters)
            : base(context, locationInfo)
        {
            this.Constructor = newObjectExpression;
            this.setters = setters;
            this.readonlySetters = new ReadOnlyCollection<Tuple<MemberReferenceExpression, Expression[]>>(this.setters);
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <value>The constructor.</value>
        public NewObjectExpression Constructor { get; private set; }

        /// <summary>
        /// Gets the setters.
        /// </summary>
        /// <value>The setters.</value>
        public IList<Tuple<MemberReferenceExpression, Expression[]>> Setters
        {
            get { return this.readonlySetters; }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get
            {
                return this.Constructor.ResultType;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.Constructor = (NewObjectExpression)processor.Process(this.Constructor);

            for (int setterIndex = 0; setterIndex < this.setters.Count; setterIndex++)
            {
                for (int jSetterExpression = 0; jSetterExpression < this.setters[setterIndex].Item2.Length; jSetterExpression++)
                {
                    this.setters[setterIndex].Item2[jSetterExpression] = (Expression)processor.Process(this.setters[setterIndex].Item2[jSetterExpression]);
                }

                this.setters[setterIndex] =
                    Tuple.Create(
                        (MemberReferenceExpression)processor.Process(this.setters[setterIndex].Item1),
                        this.setters[setterIndex].Item2);
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("constructor", this.Constructor);
            serializationInfo.AddValue(
                "setters",
                this.Setters,
                (serializer, setter) =>
                    {
                        serializer.AddValue("setter", setter.Item1);
                        serializer.AddValue("value", setter.Item2);
                    });
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
            InlinePropertyInitilizationExpression right = obj as InlinePropertyInitilizationExpression;

            if (right != null
                || !this.Constructor.Equals(right.Constructor)
                || this.Setters.Count != right.Setters.Count)
            {
                return false;
            }

            for (int setterIndex = 0; setterIndex < this.Setters.Count; setterIndex++)
            {
                if (!this.Setters[setterIndex].Item1.Equals(right.Setters[setterIndex].Item1))
                {
                    return false;
                }

                for (int jExpresssion = 0; jExpresssion < this.Setters[setterIndex].Item2.Length; jExpresssion++)
                {
                    if (!this.Setters[setterIndex].Item2[jExpresssion].Equals(right.Setters[setterIndex].Item2[jExpresssion]))
                    {
                        return false;
                    }
                }
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
            int rv = typeof(InlinePropertyInitilizationExpression).GetHashCode()
                ^ this.Constructor.GetHashCode();

            foreach (var item in this.Setters)
            {
                rv ^= item.GetHashCode();
            }

            return rv;
        }
    }
}

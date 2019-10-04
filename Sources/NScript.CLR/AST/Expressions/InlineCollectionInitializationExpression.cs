//-----------------------------------------------------------------------
// <copyright file="InlineCollectionInitializationExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using Mono.Cecil;
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Definition for InlineCollectionInitializationExpression
    /// </summary>
    public class InlineCollectionInitializationExpression
        : Expression
    {
        private NewObjectExpression newObjectExpression;
        private readonly List<MethodCallExpression> setters;
        private readonly ReadOnlyCollection<MethodCallExpression> readonlySetters;

        public InlineCollectionInitializationExpression(
            ClrContext context,
            Location locationInfo,
            NewObjectExpression newObjectExpression,
            List<MethodCallExpression> setters)
            : base(context, locationInfo)
        {
            this.newObjectExpression = newObjectExpression;
            this.setters = setters;
            this.readonlySetters = new ReadOnlyCollection<MethodCallExpression>(this.setters);
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <value>The constructor.</value>
        public NewObjectExpression Constructor
        {
            get { return this.newObjectExpression; }
        }

        /// <summary>
        /// Gets the setters.
        /// </summary>
        /// <value>The setters.</value>
        public IList<MethodCallExpression> Setters
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
                return this.newObjectExpression.ResultType;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.newObjectExpression = (NewObjectExpression)processor.Process(this.newObjectExpression);

            for (int setterIndex = 0; setterIndex < this.setters.Count; setterIndex++)
            {
                this.setters[setterIndex] =
                    (MethodCallExpression)processor.Process(this.setters[setterIndex]);
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("constructor", this.newObjectExpression);
            serializationInfo.AddValue(
                "setters",
                this.Setters,
                (serializer, setter) =>
                    {
                        serializer.AddValue("setter", setter);
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
            var right = obj as InlineCollectionInitializationExpression;

            if (right != null
                || !this.Constructor.Equals(right.Constructor)
                || this.Setters.Count != right.Setters.Count)
            {
                return false;
            }

            for (int setterIndex = 0; setterIndex < this.Setters.Count; setterIndex++)
            {
                if (!this.Setters[setterIndex].Equals(right.Setters[setterIndex]))
                {
                    return false;
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
            int rv = typeof(InlineCollectionInitializationExpression).GetHashCode()
                ^ this.Constructor.GetHashCode();

            foreach (var item in this.Setters)
            {
                rv ^= item.GetHashCode();
            }

            return rv;
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="InlineArrayInitialization.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Cs2JsC.Utils;
    using Mono.Cecil;

    /// <summary>
    /// Definition for InlineArrayInitialization
    /// </summary>
    public class InlineArrayInitialization : Expression
    {
        /// <summary>
        /// Backing field for elementType
        /// </summary>
        private readonly TypeReference elementType;

        /// <summary>
        /// Backing collection for ElementInitValues.
        /// </summary>
        private readonly List<Expression> elementInitValues;

        /// <summary>
        /// Backing field for ElementInitValues.
        /// </summary>
        private readonly ReadOnlyCollection<Expression> readonlyElementInitValues;

        /// <summary>
        /// Backing field for ResultType.
        /// </summary>
        private readonly TypeReference resultType;

        private Expression sizeExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineArrayInitialization"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="elementInitValues">The element init values.</param>
        public InlineArrayInitialization(
            ClrContext context,
            Location location,
            TypeReference elementType,
            IList<Expression> elementInitValues,
            Expression sizeExpression = null)
            : base(context, location)
        {
            this.elementType = elementType;
            this.elementInitValues = new List<Expression>(elementInitValues);
            this.resultType = new ArrayType(this.elementType);

            this.readonlyElementInitValues = new ReadOnlyCollection<Expression>(this.elementInitValues);
            this.sizeExpression = sizeExpression;
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <value>The type of the element.</value>
        public TypeReference ElementType
        {
            get { return this.elementType; }
        }

        /// <summary>
        /// Gets the element init values.
        /// </summary>
        /// <value>The element init values.</value>
        public IList<Expression> ElementInitValues
        {
            get { return this.readonlyElementInitValues; }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get { return this.resultType; }
        }

        /// <summary>
        /// Gets the size expression.
        /// </summary>
        public Expression SizeExpression
        {
            get { return this.sizeExpression; }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("elementType", this.elementType.FullName);
            serializationInfo.AddValue("initValues", this.ElementInitValues);
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            for (int elementInitIndex = 0; elementInitIndex < this.elementInitValues.Count; elementInitIndex++)
            {
                this.elementInitValues[elementInitIndex] =
                    (Expression)processor.Process(this.elementInitValues[elementInitIndex]);
            }

            if (sizeExpression != null)
            {
                this.sizeExpression = (Expression)processor.Process(this.sizeExpression);
            }
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
            InlineArrayInitialization right = obj as InlineArrayInitialization;

            if (right == null
                || !this.ResultType.Equals(right.ResultType)
                || this.ElementInitValues.Count != right.ElementInitValues.Count)
            {
                return false;
            }

            for (int elementIndex = 0; elementIndex < this.elementInitValues.Count; elementIndex++)
            {
                if (!this.elementInitValues[elementIndex].Equals(right.elementInitValues[elementIndex]))
                {
                    return false;
                }
            }

            return object.Equals(this.sizeExpression, right.sizeExpression);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            int rv = typeof(InlineArrayInitialization).GetHashCode()
                ^ this.ElementType.GetHashCode();

            foreach (var item in this.ElementInitValues)
            {
                rv ^= item.GetHashCode();
            }

            return rv;
        }
    }
}
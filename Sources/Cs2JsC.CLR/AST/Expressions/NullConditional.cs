//-----------------------------------------------------------------------
// <copyright file="NullConditional.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System;
    using System.Collections.Generic;
    using Mono.Cecil;
    using Cs2JsC.Utils;

    /// <summary>
    /// Definition for NullConditional
    /// </summary>
    public class NullConditional : Expression
    {
        /// <summary>
        /// Backing field for firstChoice,
        /// </summary>
        private Expression firstChoice;

        /// <summary>
        /// Backing field for Alternate choice.
        /// </summary>
        private Expression alternate;

        /// <summary>
        /// Initializes a new instance of the <see cref="NullConditional"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        /// <param name="firstChoice">The first choice.</param>
        /// <param name="alternate">The alternate.</param>
        public NullConditional(
            ClrContext context,
            Location location,
            Expression firstChoice,
            Expression alternate)
            : base(context, location)
        {
            this.firstChoice = firstChoice;
            this.alternate = alternate;
        }

        /// <summary>
        /// Gets the first choice.
        /// </summary>
        /// <value>The first choice.</value>
        public Expression FirstChoice
        {
            get { return this.firstChoice; }
        }

        /// <summary>
        /// Gets the alternate.
        /// </summary>
        /// <value>The alternate.</value>
        public Expression Alternate
        {
            get { return this.alternate; }
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public override TypeReference ResultType
        {
            get { return this.FirstChoice.ResultType; }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor">The processor.</param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            this.firstChoice = (Expression) processor.Process(this.firstChoice);
            this.alternate = (Expression) processor.Process(this.alternate);
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        public override void Serialize(Utils.ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue("First", this.FirstChoice);
            serializationInfo.AddValue("Alternate", this.Alternate);
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
            NullConditional right = obj as NullConditional;

            return right != null
                && this.FirstChoice.Equals(right.FirstChoice)
                && this.Alternate.Equals(right.Alternate);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return typeof(NullLiteral).GetHashCode();
        }
    }
}

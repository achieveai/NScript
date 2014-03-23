//-----------------------------------------------------------------------
// <copyright file="AnonymousNewExpression.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.CLR.AST
{
    using NScript.Utils;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Definition for AnonymousNewExpression
    /// </summary>
    public class AnonymousNewExpression : Expression
    {
        /// <summary>
        /// The values.
        /// </summary>
        List<Tuple<string, Expression>> values = new List<Tuple<string, Expression>>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">  The context. </param>
        /// <param name="location"> The location. </param>
        public AnonymousNewExpression(
            ClrContext context,
            Location location)
            : base(context, location)
        {
            this.Values = new ReadOnlyCollection<Tuple<string, Expression>>(this.values);
        }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public ReadOnlyCollection<Tuple<string, Expression>> Values
        { get; private set; }

        /// <summary>
        /// Adds a value to 'value'.
        /// </summary>
        /// <param name="name">  The name. </param>
        /// <param name="value"> The value. </param>
        public void AddValue(string name, Expression value)
        {
            this.values.Add(Tuple.Create(name, value));
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>
        /// The type of the result.
        /// </value>
        public override Mono.Cecil.TypeReference ResultType
        {
            get
            {
                return this.Context.KnownReferences.Object;
            }
        }

        /// <summary>
        /// Processes the through pipeline.
        /// </summary>
        /// <param name="processor"> The processor. </param>
        public override void ProcessThroughPipeline(IAstProcessor processor)
        {
            for (int iValue = 0; iValue < this.values.Count; iValue++)
            {
                this.values[iValue] = new Tuple<string, Expression>(
                    this.values[iValue].Item1,
                    this.values[iValue].Item2);
            }
        }

        /// <summary>
        /// Serializes the specified serialization info.
        /// </summary>
        /// <param name="serializationInfo">    The serialization info. </param>
        /// ### <param name="streamingContext"> The streaming context. </param>
        public override void Serialize(ICustomSerializer serializationInfo)
        {
            serializationInfo.AddValue(
                "setters",
                this.values,
                (serializer, setter) =>
                    {
                        serializer.AddValue("setter", setter.Item1);
                        serializer.AddValue("value", setter.Item2);
                    });
        }
    }
}

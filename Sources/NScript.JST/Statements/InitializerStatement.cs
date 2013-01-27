//-----------------------------------------------------------------------
// <copyright file="InitializerStatement.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using NScript.Utils;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Definition for InitializerStatement
    /// </summary>
    public class InitializerStatement : Statement
    {
        List<Expression> initializerExpressions = new List<Expression>();
        ReadOnlyCollection<Expression> readonlyInitializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializerStatement"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="initializerExpressions">The initializer expressions.</param>
        public InitializerStatement(
            Location location,
            IdentifierScope scope,
            IList<Expression> initializerExpressions)
            : base(location, scope)
        {
            this.initializerExpressions.AddRange(initializerExpressions);
            this.readonlyInitializers = new ReadOnlyCollection<Expression>(this.initializerExpressions);
        }

        /// <summary>
        /// Gets the initializers.
        /// </summary>
        public IList<Expression> Initializers
        {
            get { return this.readonlyInitializers; }
        }

        /// <summary>
        /// Serializes the specified serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        public override void Serialize(ICustomSerializer serializer)
        {
            base.Serialize(serializer);
            serializer.AddValue("initializers", this.initializerExpressions);
        }

        /// <summary>
        /// Writes to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Write(JSWriter writer)
        {
            writer.WriteNewLine()
                .EnterLocation(this.Location);

            for (int iInitializer = 0; iInitializer < this.initializerExpressions.Count; iInitializer++)
            {
                if (iInitializer > 0)
                {
                    writer.Write(Symbols.Comma);
                }

                this.initializerExpressions[iInitializer].Write(writer);
            }

            writer.Write(Symbols.SemiColon)
                .LeaveLocation();
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="Expression.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.CLR.AST
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Mono.Cecil;
    using Cs2JsC.Utils;

    /// <summary>
    /// Base class of all the expressions.
    /// </summary>
    public abstract class Expression : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Expression"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="location">The location.</param>
        public Expression(
            ClrContext context,
            Location location)
            : base(context, location)
        {
        }

        /// <summary>
        /// Gets the type of the result.
        /// </summary>
        /// <value>The type of the result.</value>
        public virtual TypeReference ResultType
        {
            get
            {
                return null;
            }
        }
    }
}

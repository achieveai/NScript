//-----------------------------------------------------------------------
// <copyright file="Statement.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    using Cs2JsC.Utils;

    /// <summary>
    /// Abstract base for all the statements.
    /// </summary>
    public abstract class Statement : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Statement"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">The scope.</param>
        public Statement(Location location, IdentifierScope scope)
            : base(location, scope)
        {
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="Expression.cs" company="WebApps.Net">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.JST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cs2JsC.Utils;

    /// <summary>
    /// Expression.
    /// </summary>
    public abstract class Expression : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Expression"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="scope">the scope</param>
        public Expression(
            Location location,
            IdentifierScope scope)
            : base(location, scope)
        {
        }

        /// <summary>
        /// Gets the precendence.
        /// </summary>
        /// <value>The precendence.</value>
        public virtual Precedence Precedence
        {
            get
            {
                return Precedence.Expression;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is left to right.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is left to right; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsLeftToRight
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the operator placement.
        /// </summary>
        public virtual OperatorPlacement OperatorPlacement
        {
            get
            {
                if (this.IsLeftToRight)
                {
                    return OperatorPlacement.Postfix;
                }

                return JST.OperatorPlacement.Prefix;
            }
        }
    }
}

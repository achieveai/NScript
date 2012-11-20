//-----------------------------------------------------------------------
// <copyright file="ScopeToken.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST.Writer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for ScopeToken
    /// </summary>
    internal class ScopeToken : TokenBase
    {
        /// <summary>
        /// Backing field for IsExit
        /// </summary>
        private bool isExitScope;

        /// <summary>
        /// Backing field for Depth.
        /// </summary>
        private int scopeDepth;

        public ScopeToken(int scopeDepth, bool isExitScope)
            :base(TokenType.ScopeToken, null)
        {
            this.isExitScope = isExitScope;
            this.scopeDepth = scopeDepth;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is exit.
        /// </summary>
        /// <value><c>true</c> if this instance is exit; otherwise, <c>false</c>.</value>
        public bool IsExit
        {get { return this.isExitScope; }}

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>The depth.</value>
        public int Depth
        { get { return this.scopeDepth; } }
    }
}

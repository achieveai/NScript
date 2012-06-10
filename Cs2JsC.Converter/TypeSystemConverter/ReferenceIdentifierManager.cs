//-----------------------------------------------------------------------
// <copyright file="ReferenceIdentifierManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using Cs2JsC.JST;

    /// <summary>
    /// Definition for ReferenceIdentifierManager
    /// </summary>
    public class ReferenceIdentifierManager
    {
        /// <summary>
        /// Tracker for IdentifierScope.
        /// </summary>
        private readonly IdentifierScope identifierScope =
            new IdentifierScope(false);

        /// <summary>
        /// Backing field for ReaderIdentifier;
        /// </summary>
        private Identifier readerIdentifier;

        /// <summary>
        /// Backing field for WriterIdentifier;
        /// </summary>
        private Identifier writerIdentifier;

        /// <summary>
        /// Gets the reader identifier.
        /// </summary>
        /// <value>The reader identifier.</value>
        public Identifier ReaderIdentifier
        {
            get
            {
                if (this.readerIdentifier == null)
                {
                    this.readerIdentifier = Identifier.CreateScopeIdentifier(this.identifierScope, "read", false);
                }

                return this.readerIdentifier;
            }
        }

        /// <summary>
        /// Gets the writer identifier.
        /// </summary>
        /// <value>The writer identifier.</value>
        public Identifier WriterIdentifier
        {
            get
            {
                if (this.writerIdentifier == null)
                {
                    this.writerIdentifier =
                        Identifier.CreateScopeIdentifier(
                            this.identifierScope,
                            "write",
                            false);
                }

                return this.writerIdentifier;
            }
        }
    }
}

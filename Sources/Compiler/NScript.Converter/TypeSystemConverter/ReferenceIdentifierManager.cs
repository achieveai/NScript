//-----------------------------------------------------------------------
// <copyright file="ReferenceIdentifierManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.Converter.TypeSystemConverter
{
    using System;
    using System.Collections.Generic;
    using NScript.JST;

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
        private IIdentifier readerIdentifier;

        /// <summary>
        /// Backing field for WriterIdentifier;
        /// </summary>
        private IIdentifier writerIdentifier;

        /// <summary>
        /// Gets the reader identifier.
        /// </summary>
        /// <value>The reader identifier.</value>
        public IIdentifier ReaderIdentifier
        {
            get
            {
                if (this.readerIdentifier == null)
                {
                    this.readerIdentifier = SimpleIdentifier.CreateScopeIdentifier(
                        this.identifierScope,
                        "read",
                        false);
                }

                return this.readerIdentifier;
            }
        }

        /// <summary>
        /// Gets the writer identifier.
        /// </summary>
        /// <value>The writer identifier.</value>
        public IIdentifier WriterIdentifier
        {
            get
            {
                if (this.writerIdentifier == null)
                {
                    this.writerIdentifier =
                        SimpleIdentifier.CreateScopeIdentifier(
                            this.identifierScope,
                            "write",
                            false);
                }

                return this.writerIdentifier;
            }
        }
    }
}

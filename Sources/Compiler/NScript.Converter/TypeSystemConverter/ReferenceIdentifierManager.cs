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
            => MethodRecorder.Call(
                this,
                0,
                static (self, _) => self.readerIdentifier ??= SimpleIdentifier.CreateScopeIdentifier(
                    self.identifierScope,
                    "rd",
                    false),
                static (r, _) => r.Runtime.ReferenceManager.ReaderIdentifier);

        /// <summary>
        /// Gets the writer identifier.
        /// </summary>
        /// <value>The writer identifier.</value>
        public IIdentifier WriterIdentifier
            => MethodRecorder.Call(
                this,
                0,
                static (self, _) => self.writerIdentifier ??= SimpleIdentifier.CreateScopeIdentifier(
                    self.identifierScope,
                    "wt",
                    false),
                static (r, _) => r.Runtime.ReferenceManager.WriterIdentifier);
    }
}

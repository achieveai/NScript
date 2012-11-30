//-----------------------------------------------------------------------
// <copyright file="CompoundIdentifier.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Definition for CompoundIdentifier.
    /// </summary>
    public class CompoundIdentifier : IIdentifier
    {
        /// <summary>
        /// The identifiers.
        /// </summary>
        IList<IIdentifier> identifiers;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="identifiers"> A variable-length parameters list containing identifiers. </param>
        public CompoundIdentifier(
            params IIdentifier[] identifiers)
            : this((IList<IIdentifier>)identifiers) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or
        ///     illegal values. </exception>
        /// <param name="identifiers"> A variable-length parameters list containing identifiers. </param>
        public CompoundIdentifier(
            IList<IIdentifier> identifiers)
        {
            if (identifiers == null
                || identifiers.Count == 0)
            {
                throw new ArgumentException("identifiers");
            }

            for (int iIdentifier = 0; iIdentifier < identifiers.Count; iIdentifier++)
            {
                if (identifiers[iIdentifier] == null)
                {
                    throw new ArgumentException("identifiers[" + iIdentifier + "]");
                }
            }

            this.identifiers = identifiers;
        }

        /// <summary>
        /// Gets the name of the suggested.
        /// </summary>
        /// <value>
        /// The name of the suggested.
        /// </value>
        public string SuggestedName
        {
            get
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append(this.identifiers[0].SuggestedName);
                for (int iIdentifier = 1; iIdentifier < identifiers.Count; iIdentifier++)
                {
                    strBuilder.Append('.');
                    strBuilder.Append(identifiers[iIdentifier].SuggestedName);
                }

                return strBuilder.ToString();
            }
        }

        /// <summary>
        /// Gets the identifiers.
        /// </summary>
        /// <value>
        /// The identifiers.
        /// </value>
        public IList<IIdentifier> Identifiers
        { get { return this.identifiers; } }

        /// <summary>
        /// Gets a value indicating whether this object is empty.
        /// </summary>
        /// <value>
        /// true if this object is empty, false if not.
        /// </value>
        public bool IsEmpty
        {
            get
            {
                for (int i = 0; i < this.identifiers.Count; i++)
                {
                    if (!this.identifiers[i].IsEmpty)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns>
        /// The name.
        /// </returns>
        public string GetName()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(this.identifiers[0].GetName());
            for (int iIdentifier = 1; iIdentifier < identifiers.Count; iIdentifier++)
            {
                strBuilder.Append('.');
                strBuilder.Append(identifiers[iIdentifier].GetName());
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Adds an usage.
        /// </summary>
        /// <param name="scope"> The scope. </param>
        public void AddUsage(IdentifierScope scope)
        {
            for (int iIdentifier = 0; iIdentifier < identifiers.Count; iIdentifier++)
            {
                identifiers[iIdentifier].AddUsage(scope);
            }
        }
    }
}

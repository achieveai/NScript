//-----------------------------------------------------------------------
// <copyright file="IIdentifier.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace NScript.JST
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for IIdentifier
    /// </summary>
    public interface IIdentifier
    {
        string GetName();
        void AddUsage(IdentifierScope scope);
        string SuggestedName { get; }
    }
}

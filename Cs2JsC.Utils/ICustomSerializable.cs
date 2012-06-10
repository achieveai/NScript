//-----------------------------------------------------------------------
// <copyright file="ICustomSerializable.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Cs2JsC.Utils
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Definition for ICustomSerializable
    /// </summary>
    public interface ICustomSerializable
    {
        void Serialize(ICustomSerializer serializer);
    }
}

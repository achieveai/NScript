//-----------------------------------------------------------------------
// <copyright file="Enums.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Web.Html.Data
{
    public enum ApplicationCacheStatus
    {
        Uncached = 0,
        Idle,
        Checking,
        Downloading,
        UpdateReady,
        Obsolete
    }

    public enum ApplicationCacheEvent
    {
        Cached = 0,
        Checking,
        Downloading,
        Error,
        NoUpdate,
        Progress,
        UpdateReady
    }
}
// -----------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.CompilerServices;

// The build-service tests drive watcher callbacks the OS raises rarely (ServiceHost.OnWatcherError).
[assembly: InternalsVisibleTo("NScript.Utils.Test")]

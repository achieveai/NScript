//-----------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.CompilerServices;

// Exposes internals to the framework test assembly so unit tests can cover
// JSON envelope serialization (LogJsonBuilder) and inject deterministic
// transports into HttpLogSink via its internal test-only constructor. The
// framework test DLL is the only consumer — no IVT to broader assemblies.
[assembly: InternalsVisibleTo("Sunlight.Framework.Test")]

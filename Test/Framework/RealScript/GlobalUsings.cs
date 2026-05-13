//-----------------------------------------------------------------------
// <copyright file="GlobalUsings.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

// C# 10 — `global using` directive.
//
// A single project-wide global using directive that exercises the C# 10
// grammar. The validating fixture in `Lang10GlobalUsingTests.cs` uses
// `StringBuilder` without a local `using System.Text;` — if the global
// using fails to flow through Roslyn's symbol resolution, that fixture
// will not bind. `System.Text` is chosen because nothing in the RealScript
// namespace declares a colliding `StringBuilder` symbol, so adding it
// project-wide is safe.
global using System.Text;

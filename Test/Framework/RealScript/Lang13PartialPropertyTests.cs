//-----------------------------------------------------------------------
// <copyright file="Lang13PartialPropertyTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;

    /// <summary>
    /// Compile-only fixture for the C# 13 <c>partial</c> properties feature.
    ///
    /// C# 13 introduces a new partial-property declaration shape:
    /// one `partial` *declaring* property (signature only) plus one
    /// `partial` *implementing* property (with accessor bodies) across two
    /// `partial class` declarations. Roslyn merges the two halves into a
    /// single property symbol at bind time, so the bound tree sees an
    /// ordinary property — wire-through is expected to be transparent.
    ///
    /// The two halves live in two `partial class` declarations in the
    /// **same file** (the issue body permits "two files OR two partial-
    /// class declarations in the same file"; single-file form keeps the
    /// fixture self-contained and avoids touching the project file list).
    ///
    /// This class lives in its own file (not in `Lang13Features.cs`)
    /// following the `Lang9RecordTests.cs` / `Lang11RequiredTests.cs`
    /// precedent: `Lang13Features.cs` is in the explicit Roslyn-driven
    /// build list in `NScript.Csc.Lib.Test/TestResources.cs`. The new
    /// partial-property synthesis path through `SymbolSerializer` has not
    /// been audited, so we keep it out of that path conservatively. The
    /// MSBuild framework build still globs this file and exercises it
    /// end-to-end through NScript's compiler.
    /// </summary>
    public partial class Lang13PartialPropertyTests
    {
        // Declaring partial property — signature only, no accessor bodies.
        public partial string Name { get; set; }
    }

    public partial class Lang13PartialPropertyTests
    {
        private string _name = "default";

        // Implementing partial property — supplies the accessor bodies.
        public partial string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public static void RoundTripPartialProperty()
        {
            var instance = new Lang13PartialPropertyTests();
            Console.WriteLine(instance.Name);

            instance.Name = "updated";
            Console.WriteLine(instance.Name);
        }
    }
}

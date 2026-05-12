//-----------------------------------------------------------------------
// <copyright file="Lang12PrimaryCtorExecutionTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript;

/// <summary>
/// V8 runtime execution coverage for C# 12 primary constructors on non-record
/// classes. Delegates directly to the already-shaped helpers in
/// <c>Lang12Features.cs::Lang12PrimaryCtorTests</c> so the same fixture bodies
/// validated by the compile-only path are now also exercised end-to-end through
/// Roslyn → JST → JS → V8.
/// </summary>
public class Lang12PrimaryCtorExecutionTests
{
    public static void Main()
    {
        Lang12PrimaryCtorTests.PrimaryCtorOnClass();
        Lang12PrimaryCtorTests.PrimaryCtorWithBaseCall();
        Lang12PrimaryCtorTests.PrimaryCtorMultipleParams();
    }
}

//-----------------------------------------------------------------------
// <copyright file="JsScriptImport.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for JsScriptImport
    /// </summary>
    public class JsScriptImport
    {
        [Script("var a,b = 10; a = b + 1; b += a;")]
        public static extern void Simple0ArgScript();

        [Script("var b = 10; a = b + 1; b += a;")]
        public static extern void Simple1ArgScript(int a);

        [Script(@"return foo.@{[RealScript]RealScript.Foo::Foo()}();")]
        public static extern int SimpleCsMethodCall(Foo foo);

        [Script(@"return @{[RealScript]RealScript.TmpC::Foo([mscorlib]System.String)}(str);")]
        public static extern int SimpleStatic1ArgCsMethodCall(string str);
    }
}

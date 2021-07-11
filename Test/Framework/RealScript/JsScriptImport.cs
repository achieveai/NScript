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
    [ImportedType]
    public class JsScriptImport
    {
        [ScriptAlias("jsScriptImportTest")]
        public static extern JsScriptImport Instance
        {
            get;
        }

        public extern int FooBar
        { get; set; }

        public int this[int index]
        { get { return 1; } set { } }

        [ScriptName("")]
        public static extern int SelfMethodCall(int i);

        [Script("var a,b = 10; a = b + 1; b += a;")]
        public static extern void Simple0ArgScript();

        [Script("var b = 10; a = b + 1; b += a;")]
        public static extern void Simple1ArgScript(int a);

        [Script(@"return foo.@{[RealScript]RealScript.Foo::Foo()}();")]
        public static extern int SimpleCsMethodCall(Foo foo);

        [Script(@"return @{[RealScript]RealScript.TmpC::Foo([mscorlib]System.String)}(str);")]
        public static extern int SimpleStatic1ArgCsMethodCall(string str);

        public static extern T ExternGenericCall<T>(object t);

        public static int AccessStringElement(string str, int index)
        {
            return str[index] + str[index + 1];
        }

        public static int CheckFooBar()
        {
            return JsScriptImport.Instance.FooBar;
        }

        public static int GetAndSetIndexerProperty(JsScriptImport obj, int foo)
        {
            obj[0] = foo;
            return obj[1];
        }

        public static int GetSomething()
        {
            return JsScriptImport.SelfMethodCall(10);
        }

        public static long TestExternGenericCall()
        {
            return JsScriptImport.ExternGenericCall<long>(10);
        }
    }

    [JsonType]
    public class FlagsEnumOrOperatorRegression
    {
        public extern string Str
        { get; set; }

        public extern FlagEnum? FlagEnum
        { get; set; }
    }
}

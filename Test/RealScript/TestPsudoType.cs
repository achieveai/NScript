//-----------------------------------------------------------------------
// <copyright file="TestPsudoType.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace RealScript
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for TestPsudoType
    /// </summary>
    public class TestJsonType
    {
        private string tmpStr;

        public extern int? TempI
        { get; set; }

        public extern int TempJ
        { get; }

        public extern int[] TheArray
        { get; set; }

        public string Str
        {
            get
            {
                return this.tmpStr;
            }
            set
            {
                this.tmpStr = value;
            }
        }

        public void WorkOnString()
        {
            if (this.tmpStr == null)
            {
                this.tmpStr = "";
            }
            else
            {
                this.tmpStr = this.tmpStr + this.tmpStr.Length;
            }
        }
    }

    public class TestImportedType
    {
        private string tmpStr;

        public int? TempI
        { get; set; }

        public extern int TempJ
        { get; }

        public extern int[] TheArray
        { get; set; }

        public string Str
        {
            get
            {
                return this.tmpStr;
            }
            set
            {
                this.tmpStr = value;
            }
        }

        public extern event Action<int, float> TestEvent;

        public extern string[] ProcessData(System.Collections.Generic.List<int> data);

        public void WorkOnString()
        {
            if (this.tmpStr == null)
            {
                this.tmpStr = "";
            }
            else
            {
                this.tmpStr = this.tmpStr + this.tmpStr.Length;
            }
        }
    }

    public class ImportedGeneric<T>
    {
        public extern ImportedGeneric();

        public extern T[] TheArray
        { get; set; }

        public extern T Something
        {
            [ScriptName("get_Something")]
            get;
            [ScriptName("set_Something")]
            set;
        }

        public extern T KoSelf
        {
            [ScriptName("")]
            get;
            [ScriptName("")]
            set;
        }

        public static extern T HasSomething();
    }

    public static class PsudoUsage
    {
        public static int F1(TestImportedType t, int test)
        {
            return t.TempI.HasValue ? test + t.TempI.Value : test - 1;
        }

        public static int F1(TestJsonType t, int test)
        {
            return t.TempI.HasValue ? test + t.TempI.Value : test - 1;
        }

        public static TestImportedType C1(TestImportedType t, int? i, int[] a)
        {
            return new TestImportedType()
            {
                TempI = i + t.TempJ,
                TheArray = a
            };
        }

        public static TestJsonType C1(TestJsonType t, int? i, int[] a)
        {
            return new TestJsonType()
            {
                TempI = i + t.TempJ,
                TheArray = a
            };
        }

        public static TestJsonType C2(TestJsonType t, int? i, int j, int k)
        {
            return new TestJsonType()
            {
                TempI = i + t.TempJ,
                TheArray = new int[] { j, k}
            };
        }

        public static string[] M1(TestImportedType t)
        {
            return t.ProcessData(new System.Collections.Generic.List<int>(t.TheArray));
        }

        public static string TestImportedGeneric(ImportedGeneric<string> tmp)
        {
            if (tmp.KoSelf == null)
            {
                tmp.Something = tmp.TheArray[0];
                return tmp.KoSelf = tmp.Something;
            }
            else if (tmp.KoSelf.Length == 10)
            {
                tmp.Something = ImportedGeneric<string>.HasSomething();
                tmp.KoSelf = tmp.Something;
                tmp.TheArray[0] = tmp.KoSelf;
            }

            return tmp.KoSelf;
        }
    }
}

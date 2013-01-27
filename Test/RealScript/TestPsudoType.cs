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
    /// Definition for TestPsudoType.
    /// </summary>
    public class TestJsonType
    {
        /// <summary>
        /// The temporary string.
        /// </summary>
        private string tmpStr;

        /// <summary>
        /// Gets or sets the temporary i.
        /// </summary>
        /// <value>
        /// The temporary i.
        /// </value>
        public extern int? TempI
        { get; set; }

        /// <summary>
        /// Gets the temporary j.
        /// </summary>
        /// <value>
        /// The temporary j.
        /// </value>
        public extern int TempJ
        { get; }

        /// <summary>
        /// Gets or sets an array of thes.
        /// </summary>
        /// <value>
        /// An Array of thes.
        /// </value>
        public extern int[] TheArray
        { get; set; }

        /// <summary>
        /// Gets or sets the. 
        /// </summary>
        /// <value>
        /// The string.
        /// </value>
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

        /// <summary>
        /// Work on string.
        /// </summary>
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

    /// <summary>
    /// Test imported type.
    /// </summary>
    public class TestImportedType
    {
        /// <summary>
        /// The temporary string.
        /// </summary>
        private string tmpStr;

        /// <summary>
        /// Gets or sets the temporary i.
        /// </summary>
        /// <value>
        /// The temporary i.
        /// </value>
        public int? TempI
        { get; set; }

        /// <summary>
        /// Gets the temporary j.
        /// </summary>
        /// <value>
        /// The temporary j.
        /// </value>
        public extern int TempJ
        { get; }

        /// <summary>
        /// Gets or sets an array of thes.
        /// </summary>
        /// <value>
        /// An Array of thes.
        /// </value>
        public extern int[] TheArray
        { get; set; }

        /// <summary>
        /// Gets or sets the. 
        /// </summary>
        /// <value>
        /// The string.
        /// </value>
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

        /// <summary>
        /// Event queue for all listeners interested in Test events.
        /// </summary>
        public extern event Action<int, float> TestEvent;

        /// <summary>
        /// Process the data described by data.
        /// </summary>
        /// <param name="data"> The data. </param>
        /// <returns>
        /// .
        /// </returns>
        public extern string[] ProcessData(System.Collections.Generic.List<int> data);

        /// <summary>
        /// Work on string.
        /// </summary>
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

    /// <summary>
    /// Imported generic.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    public class ImportedGeneric<T>
    {
        /// <summary>
        /// Gets the imported generic.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        public extern ImportedGeneric();

        /// <summary>
        /// Gets or sets an array of thes.
        /// </summary>
        /// <value>
        /// An Array of thes.
        /// </value>
        public extern T[] TheArray
        { get; set; }

        /// <summary>
        /// Gets or sets something.
        /// </summary>
        /// <value>
        /// something.
        /// </value>
        public extern T Something
        {
            [ScriptName("get_Something")]
            get;
            [ScriptName("set_Something")]
            set;
        }

        /// <summary>
        /// Gets or sets the ko self.
        /// </summary>
        /// <value>
        /// The ko self.
        /// </value>
        public extern T KoSelf
        {
            [ScriptName("")]
            get;
            [ScriptName("")]
            set;
        }

        /// <summary>
        /// Gets the has something.
        /// </summary>
        /// <returns>
        /// .
        /// </returns>
        public static extern T HasSomething();
    }

    /// <summary>
    /// Psudo usage.
    /// </summary>
    public static class PsudoUsage
    {
        /// <summary>
        /// F 1.
        /// </summary>
        /// <param name="t">    The TestImportedType to process. </param>
        /// <param name="test"> The test. </param>
        /// <returns>
        /// .
        /// </returns>
        public static int F1(TestImportedType t, int test)
        {
            return t.TempI.HasValue ? test + t.TempI.Value : test - 1;
        }

        /// <summary>
        /// F 1.
        /// </summary>
        /// <param name="t">    The TestImportedType to process. </param>
        /// <param name="test"> The test. </param>
        /// <returns>
        /// .
        /// </returns>
        public static int F1(TestJsonType t, int test)
        {
            return t.TempI.HasValue ? test + t.TempI.Value : test - 1;
        }

        /// <summary>
        /// C 1.
        /// </summary>
        /// <param name="t"> The TestImportedType to process. </param>
        /// <param name="i"> Zero-based index of the. </param>
        /// <param name="a"> The int[] to process. </param>
        /// <returns>
        /// .
        /// </returns>
        public static TestImportedType C1(TestImportedType t, int? i, int[] a)
        {
            return new TestImportedType()
            {
                TempI = i + t.TempJ,
                TheArray = a
            };
        }

        /// <summary>
        /// C 1.
        /// </summary>
        /// <param name="t"> The TestImportedType to process. </param>
        /// <param name="i"> Zero-based index of the. </param>
        /// <param name="a"> The int[] to process. </param>
        /// <returns>
        /// .
        /// </returns>
        public static TestJsonType C1(TestJsonType t, int? i, int[] a)
        {
            return new TestJsonType()
            {
                TempI = i + t.TempJ,
                TheArray = a
            };
        }

        /// <summary>
        /// C 2.
        /// </summary>
        /// <param name="t"> The TestImportedType to process. </param>
        /// <param name="i"> Zero-based index of the. </param>
        /// <param name="j"> The int to process. </param>
        /// <param name="k"> The int to process. </param>
        /// <returns>
        /// .
        /// </returns>
        public static TestJsonType C2(TestJsonType t, int? i, int j, int k)
        {
            return new TestJsonType()
            {
                TempI = i + t.TempJ,
                TheArray = new int[] { j, k}
            };
        }

        /// <summary>
        /// M 1.
        /// </summary>
        /// <param name="t"> The TestImportedType to process. </param>
        /// <returns>
        /// .
        /// </returns>
        public static string[] M1(TestImportedType t)
        {
            return t.ProcessData(new System.Collections.Generic.List<int>(t.TheArray));
        }

        /// <summary>
        /// Tests imported generic.
        /// </summary>
        /// <param name="tmp"> The temporary. </param>
        /// <returns>
        /// .
        /// </returns>
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

namespace RealScript
{
    using System;
    using System.Collections.Generic;

    public class TestStdlib
    {
        public static void Main()
        {
            TestString();
        }

        public static void TestString()
        {
            Console.WriteLine("String.Replace tests");

            var str = "as df gh hjk lmnb vcxzqwe rty io df df as as";
            Console.WriteLine(str.Replace("as", "giraffe"));
            Console.WriteLine(str.ReplaceFirst("as", "giraffe"));
        }
    }
}

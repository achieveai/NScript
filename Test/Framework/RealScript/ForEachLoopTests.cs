namespace RealScript
{
    using System;

    public class ForEachLoopTests
    {
        public static void Main()
        {
            Test1();
            Test2();
        }

        private static void Test1()
        {
            foreach (var item in new[] { 1, 2, 3 })
            {
                Console.WriteLine(item);
            }
        }

        private static void Test2()
        {
            var l = new NativeArray<int>(2);
            l[0] = 1;
            l[1] = 2;

            foreach (var item in l.GetArray())
            {
                Console.WriteLine(item);
            }
        }
    }
}

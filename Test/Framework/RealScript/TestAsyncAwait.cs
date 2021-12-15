namespace RealScript
{
    using System;
    using System.Runtime.CompilerServices;

    public class TestAsyncAwait
    {
        public static async void Main()
        {
            var res = await Test3();
            Console.WriteLine(res);
            await Test4();
            await Test5();
        }

        public static async Promise<int> Test1()
        {
            await Utilities.Delay(2000);
            return 12;
        }

        public static async Promise<int> Test2()
        {
            return await Test1();
        }

        public static async Promise<string> Test3()
        {
            Console.WriteLine("Test3");
            var cls = new MyClass(Test2());
            var nativeArray = new NativeArray<Promise<int>>(2);
            nativeArray[0] = Test1();
            nativeArray[1] = Test2();
            var tmp = await nativeArray;
            return (await cls).ToString() + tmp;
        }

        public static async Promise Test4()
        {
            Func<Promise<int>> func = async () => 1;

            Compare(1, await func(), (a, b) => a == b);
        }

        public static async Promise Test5()
        {
            async Promise<int> LocalFunc() => 1;

            Compare(1, await LocalFunc(), (a, b) => a == b);
        }

        private static void Compare<T>(T lhs, T rhs, Func<T, T, bool> compare)
        {
            if (compare(lhs, rhs))
            {
                Console.WriteLine("This should be printed");
            }
            else
            {
                Console.WriteLine("This should not be printed");
            }
        }
    }

    public class MyClass
    {
        Promise<int> promise;

        public MyClass(Promise<int> promise)
        {
            this.promise = promise;
        }

        public TaskAwaiter<int> GetAwaiter()
        {
            return promise.GetAwaiter();
        }
    }

    public static class ArrayExtensions
    {
        public static TaskAwaiter<NativeArray<T>> GetAwaiter<T>(this NativeArray<Promise<T>> nativeArray)
        {
            return Promise<T>.All(nativeArray).GetAwaiter();
        }
    }

    public static class Utilities
    {
        public static Promise Delay(int delay)
        {
            return new Promise((resolve, reject) =>
            {
                Console.SetTimeout(() => resolve(), delay);
            });
        }
    }
}

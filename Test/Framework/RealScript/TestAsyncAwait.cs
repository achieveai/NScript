namespace RealScript
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class TestAsyncAwait
    {
        public static void Main()
        {
            Test3().Then(Console.WriteLine);
            Test5();
        }

        public static async Promise<int> Test1()
        {
            return 12;
        }

        public static async Promise<int> Test2()
        {
            return await Test1();
        }

        public static async Promise<string> Test3()
        {
            var cls = new MyClass(Test2());
            var nativeArray = new NativeArray<Promise<int>>(2);
            nativeArray[0] = Test1();
            nativeArray[1] = Test2();
            var tmp = await nativeArray;
            return (await cls).ToString() + tmp;
        }
        
        public static async Task<int> Test4()
        {
            return 90;
        }

        public static async Task<int> Test5()
        {
            return await Test4();
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

    public static class ArrayExt
    {
        public static TaskAwaiter<NativeArray<T>> GetAwaiter<T>(this NativeArray<Promise<T>> nativeArray)
        {
            return Promise<T>.All(nativeArray).GetAwaiter();
        }
    }
}

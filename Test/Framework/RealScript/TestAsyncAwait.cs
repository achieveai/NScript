namespace RealScript
{
    using System;
    using System.Runtime.CompilerServices;

    public class TestAsyncAwait
    {
        public static void Main()
        {
            Test3().Then(Console.WriteLine);
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
            return (await cls).ToString();
            // var x = await cls;
            // return x.ToString();
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
}

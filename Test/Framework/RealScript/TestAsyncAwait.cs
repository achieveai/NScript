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
            await Test6();
            await Test7();
        }

        public static async Promise<int> Test1()
        {
            await Utilities.Delay(200);
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

        public static async Promise Test6()
        {
            Func<Promise<string>> ff = async () =>
            {
                await Utilities.Delay(100);
                return "asdf";
            };

            var (a, b) = await Utilities.WhenAll(Test1(), ff());
            Console.WriteLine(a);
            Console.WriteLine(b);
        }

        public static async Promise Test7()
        {
            Console.WriteLine(await Sum(
                await Test1(),
                await Sum(
                    await Test1() + 22,
                    await Test2() + 78)));
        }

        public static async Promise<int> Sum(int a, int b)
            => a + b;

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

        public static async Promise<(T1, T2)> WhenAll<T1, T2>(Promise<T1> p1, Promise<T2> p2)
        {
            await Promise.All(p1, p2);
            return (await p1, await p2);
        }
    }
}

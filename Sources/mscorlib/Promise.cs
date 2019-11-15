//-----------------------------------------------------------------------
// <copyright file="Promise.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Definition for Promise
    /// </summary>
    [IgnoreNamespace]
    public class Promise
    {
        public static extern Promise<T> Resolve<T>(T value);
        public static extern Promise<T> Resolve<T>(Promise<T> value);

        public static extern Promise<T> Reject<T>(T value);
        public static extern Promise<NativeArray> All(NativeArray<Promise> values);
        public static extern Promise<NativeArray> All(params Promise[] values);
        public static extern Promise Race(NativeArray<Promise> promises);
        public static extern Promise Race(params Promise[] promises);

        public extern Promise Then(Action onFulfilled);

        public extern Promise Then(Func<Promise> onFulfilled);

        public extern Promise<T> Then<T>(Func<T> onFulfilled);

        public extern Promise<T> Then<T>(Func<Promise<T>> onFulfilled);

        public extern Promise Then(Action onFulfilled, Action onRejected);

        public extern Promise Then<E>(Action onFulfilled, Action<E> onRejected);

        public extern Promise Then<E>(Func<Promise> onFulfilled, Action<E> onRejected);

        public extern Promise<T> Then<T, E>(Func<Promise<T>> onFulfilled, Action<E> onRejected);

        public extern Promise<U> Catch<E, U>(Func<E, U> onRejected);

        public extern Promise<U> Catch<E, U>(Func<E, Promise<U>> onRejected);

        public extern Promise Catch<E>(Action<E> onRejected);

        public static Promise<List<R>> For<R>(int startIdx, Func<int, bool> condition, Func<int, Promise<R>> func)
        {
            return ForInternal<R>(startIdx, condition, func, new List<R>());
        }

        private static Promise<List<R>> ForInternal<R>(
            int startIdx,
            Func<int, bool> condition,
            Func<int, Promise<R>> func,
            List<R> result)
        {
            if (condition(startIdx))
            { return Promise.Resolve(result); }

            return func(startIdx).Then((res) =>
                {
                    result.Add(res);

                    return ForInternal<R>(
                        startIdx + 1,
                        condition,
                        func,
                        result);
                });
        }

        public TaskAwaiter GetAwaiter()
        {
            Action onCompletions = null;
            Exception exception = null;
            bool completed = false;

            Action complete = () =>
            {
                completed = true;
                if (onCompletions != null)
                {
                    onCompletions.Invoke();
                }
            };

            this.Then<Exception>(
                complete,
                (ex) =>
                {
                    exception = ex;
                    complete();
                });

            return new TaskAwaiter(
                (onComplete) =>
                {
                    if (onCompletions == null)
                    { onCompletions = onComplete; }
                    else
                    { onCompletions = onCompletions + onComplete; }
                },
                () =>
                {
                    if (!completed)
                    { throw new Exception("Task not complete"); }

                    if (exception != null)
                    { throw exception; }

                    return;
                });
        }
    }

    [IgnoreNamespace, IgnoreGenericArguments, ScriptName("Promise")]
    public class Promise<T> : Promise
    {
        public extern Promise(Action<Action<T>, Action<object>> callback);

        public extern Promise(
            Action<Action<Promise<T>>, Action<object>> callback);

        public extern Promise(
            Action<Action<T>, Action<Promise>> callback);

        public extern Promise(
            Action<Action<Promise<T>>, Action<Promise>> callback);

        public static extern Promise<NativeArray<T>> All(NativeArray<T> values);
        public static extern Promise<NativeArray<T>> All(NativeArray<Promise<T>> values);
        public static extern Promise<NativeArray<T>> All(params T[] values);
        public static extern Promise<NativeArray<T>> All(params Promise<T>[] values);

        public extern Promise Then(Action<T> onFulfilled);

        public extern Promise Then(Func<T, Promise> onFulfilled);

        public extern Promise Then<E>(Action<T> onFulfilled, Action<E> onRejected);

        public extern Promise Then<E>(Func<T, Promise> onFulfilled, Action<E> onRejected);

        public extern Promise Then<E>(Func<T, Promise> onFulfilled, Action<E, Promise> onRejected);

        public extern Promise<U> Then<U>(Func<T, U> onFulfilled);

        public extern Promise<U> Then<U>(Func<T, Promise<U>> onFulfilled);

        public extern Promise<U> Then<U, E>(
            Func<T, U> onFulfilled,
            Func<E, U> onRejected);

        public extern Promise<U> Then<U, E>(
            Func<T, Promise<U>> onFulfilled,
            Func<E, U> onRejected);

        public extern Promise<U> Then<U, E>(
            Func<T, U> onFulfilled,
            Func<E, Promise<U>> onRejected);

        public extern Promise<U> Then<U, E>(
            Func<T, Promise<U>> onFulfilled,
            Func<E, Promise<U>> onRejected);

        public extern Promise<U> Then<U, E>(
            Func<T, U> onFulfilled,
            Action<E> onRejected);

        public extern Promise<U> Then<U, E>(
            Func<T, Promise<U>> onFulfilled,
            Action<E> onRejected);

        public extern Promise<T> Catch<E>(Func<E, T> onRejected);

        public extern Promise<T> Catch<E>(Func<E, Promise<T>> onRejected);

        public extern new Promise<T> Catch<E>(Action<E> onRejected);

        public new TaskAwaiter<T> GetAwaiter()
        {
            Action onCompletions = null;
            Exception exception = null;
            T result = default(T);
            bool completed = false;

            Action complete = () =>
            {
                completed = true;
                if (onCompletions != null)
                {
                    onCompletions.Invoke();
                }
            };

            this.Then<Exception>(
                (res) =>
                {
                    result = res;
                    complete();
                },
                (ex) =>
                {
                    exception = ex;
                    complete();
                });

            return new TaskAwaiter<T>(
                (onComplete) =>
                {
                    if (onCompletions == null)
                    { onCompletions = onComplete; }
                    else
                    { onCompletions = onCompletions + onComplete; }
                },
                () =>
                {
                    if (!completed)
                    { throw new Exception("Task not complete"); }

                    if (exception != null)
                    { throw exception; }

                    return result;
                });
        }
    }
}

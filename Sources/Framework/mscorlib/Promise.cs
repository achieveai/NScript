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
    [IgnoreNamespace, ScriptName("Promise")]
    [AsyncMethodBuilder(typeof(PromiseBuilder))]
    public class Promise
    {
        public extern Promise();

        public extern Promise(Action<Action, Action<object>> callback);

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

        [Script("return this;")]
        public extern TaskAwaiter GetAwaiter();
    }

    [IgnoreNamespace, ScriptName("Promise")]
    [AsyncMethodBuilder(typeof(PromiseBuilder<>))]
    public class Promise<T>: Promise
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

        [Script("return this;")]
        public extern new TaskAwaiter<T> GetAwaiter();
    }

    public class PromiseBuilder: IAsyncStateMachine
    {
        public Promise Task { get; private set; }

        public static PromiseBuilder Create()
        {
            return new PromiseBuilder();
        }

        public void SetException(Exception e)
        {
        }

        public void SetResult()
        {
        }

        public void AwaitOnCompleted<TAwaiter,TStateMachine> (ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.INotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine
        {
        }

        public void AwaitUnsafeOnCompleted<TAwaiter,TStateMachine> (ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine
        {
        }

        public void Start<TStateMachine> (ref TStateMachine stateMachine) where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine
        {
        }

        public void MoveNext()
        {
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }

    public class PromiseBuilder<T>: IAsyncStateMachine
    {
        public Promise<T> Task { get; private set; }

        public static PromiseBuilder<T> Create()
        {
            return new PromiseBuilder<T>();
        }

        public void SetException(Exception e)
        {
        }

        public void SetResult(T Result)
        {
        }

        public void AwaitOnCompleted<TAwaiter,TStateMachine> (ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.INotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine
        {
        }

        public void AwaitUnsafeOnCompleted<TAwaiter,TStateMachine> (ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine
        {
        }

        public void Start<TStateMachine> (ref TStateMachine stateMachine) where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine
        {
        }

        public void MoveNext()
        {
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }
    }
}

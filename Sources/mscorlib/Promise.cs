//-----------------------------------------------------------------------
// <copyright file="Promise.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System
{
    using System;
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
        public static extern Promise<NativeArray<T>> All<T>(NativeArray<T> values);
        public static extern Promise<NativeArray<T>> All<T>(NativeArray<Promise<T>> values);
        public static extern Promise<NativeArray<T>> All<T>(params T[] values);
        public static extern Promise<NativeArray<T>> All<T>(params Promise<T>[] values);
        public static extern Promise Race(NativeArray<Promise> promises);
        public static extern Promise Race(params Promise[] promises);

        public extern Promise<U> Then<T, U>(
            Func<T, U> onFulfilled);

        public extern Promise<U> Then<T, U>(
            Func<T, Promise<U>> onFulfilled);

        public extern Promise<U> Then<T, U, V>(
            Func<T, U> onFulfilled,
            Func<V, U> onRejected);

        public extern Promise<U> Then<T, U, V>(
            Func<T, Promise<U>> onFulfilled,
            Func<V, U> onRejected);

        public extern Promise<U> Then<T, U, V>(
            Func<T, U> onFulfilled,
            Func<V, Promise<U>> onRejected);

        public extern Promise<U> Then<T, U, V>(
            Func<T, Promise<U>> onFulfilled,
            Func<V, Promise<U>> onRejected);

        public extern Promise<U> Then<T, U, V>(
            Func<T, U> onFulfilled,
            Action<V> onRejected);

        public extern Promise<U> Then<T, U, V>(
            Func<T, Promise<U>> onFulfilled,
            Action<V> onRejected);

        public extern Promise<U> Catch<U, V>(Func<V, U> onRejected);

        public extern Promise<U> Catch<U, V>(Func<V, Promise<U>> onRejected);

        public extern Promise Catch<E>(Action<E> onRejected);
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

        public extern Promise Then(Action<T> onFulfilled);

        public extern Promise Then(Action<T, Promise> onFulfilled);

        public extern Promise Then<E>(Action<T> onFulfilled, Action<E> onRejected);

        public extern Promise Then<E>(Func<T, Promise> onFulfilled, Action<E> onRejected);

        public extern Promise Then<E>(Func<T> onFulfilled, Action<E, Promise> onRejected);

        public extern Promise Then<E>(Func<T, Promise> onFulfilled, Action<E, Promise> onRejected);

        public extern Promise<U> Then<U>(Func<T, U> onFulfilled);

        public extern Promise<U> Then<U>(Func<T, Promise<U>> onFulfilled);

        public extern Promise<U> Then<U, V>(
            Func<T, U> onFulfilled,
            Func<V, U> onRejected);

        public extern Promise<U> Then<U, V>(
            Func<T, Promise<U>> onFulfilled,
            Func<V, U> onRejected);

        public extern Promise<U> Then<U, V>(
            Func<T, U> onFulfilled,
            Func<V, Promise<U>> onRejected);

        public extern Promise<U> Then<U, V>(
            Func<T, Promise<U>> onFulfilled,
            Func<V, Promise<U>> onRejected);

        public extern Promise<U> Then<U, V>(
            Func<T, U> onFulfilled,
            Action<V> onRejected);

        public extern Promise<U> Then<U, V>(
            Func<T, Promise<U>> onFulfilled,
            Action<V> onRejected);

        public extern new Promise<T> Catch<E>(Action<E> onRejected);
    }
}

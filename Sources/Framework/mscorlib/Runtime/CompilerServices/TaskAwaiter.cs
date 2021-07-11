//-----------------------------------------------------------------------
// <copyright file="TaskAwaiter.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    using System;

    /// <summary>
    /// Definition for TaskAwaiter
    /// </summary>
    public struct TaskAwaiter : ICriticalNotifyCompletion
    {
        private readonly Action<Action> _addCompletors;
        private readonly Action _resultGetter;

        public TaskAwaiter(
            Action<Action> addCompletors,
            Action resultGetter)
        {
            _addCompletors = addCompletors;
            _resultGetter = resultGetter;
        }

        public void GetResult()
            => _resultGetter();

        public void OnCompleted(Action continuation)
            => _addCompletors?.Invoke(continuation);

        public void UnsafeOnCompleted(Action continuation)
            => _addCompletors?.Invoke(continuation);
    }

    /// <summary>
    /// Definition for TaskAwaiter
    /// </summary>
    public struct TaskAwaiter<TResult> : ICriticalNotifyCompletion
    {
        private readonly Action<Action> _addCompletors;
        private readonly Func<TResult> _resultGetter;

        public TaskAwaiter(
            Action<Action> addCompletors,
            Func<TResult> resultGetter)
        {
            _addCompletors = addCompletors;
            _resultGetter = resultGetter;
        }

        public TResult GetResult()
            => _resultGetter();

        public void OnCompleted(Action continuation)
            => _addCompletors?.Invoke(continuation);

        public void UnsafeOnCompleted(Action continuation)
            => _addCompletors?.Invoke(continuation);
    }
}

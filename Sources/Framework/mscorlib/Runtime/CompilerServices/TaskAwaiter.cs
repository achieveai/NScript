namespace System.Runtime.CompilerServices
{
    using System;
    using System.ComponentModel;

    public struct TaskAwaiter : INotifyCompletion
    {
        private readonly Promise _promise;

        internal TaskAwaiter(Promise promise)
        {
            _promise = promise;
        }

        // TODO(Vijay): Set it to true when promise is completed
        public bool IsCompleted => false;

        public void OnCompleted(Action continuation)
        {
            _promise.Then(continuation);
        }
    }

    public struct TaskAwaiter<TResult> : INotifyCompletion
    {
        private readonly Promise<TResult> _promise;

        internal TaskAwaiter(Promise<TResult> promise)
        {
            _promise = promise;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsCompleted => false;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnCompleted(Action continuation)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public TResult GetResult()
        {
            return default;
        }
    }
}

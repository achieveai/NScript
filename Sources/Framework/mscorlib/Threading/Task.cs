namespace System.Threading
{
    using System.Runtime.CompilerServices;

    [ScriptName("Promise"), IgnoreGenericArguments]
    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder))]
    public class Task
    {
        [Script("return this;")]
        public extern TaskAwaiter GetAwaiter();
    }

    [ScriptName("Promise"), IgnoreGenericArguments]
    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder<>))]
    public class Task<TRes> : Task
    {
        [Script("return this;")]
        public extern new TaskAwaiter<TRes> GetAwaiter();
    }

    public class AsyncTaskMethodBuilder : IAsyncStateMachine
    {
        public Task Task { get; private set; }

        public void MoveNext()
        {
            throw new NotImplementedException();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            throw new NotImplementedException();
        }

        public void SetException(Exception e)
        {
        }

        public void SetResult() { }

        public void AwaitOnCompleted<TAwaiter,TStateMachine> (ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.INotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine
        {
        }

        public void AwaitUnsafeOnCompleted<TAwaiter,TStateMachine> (ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine
        {
        }

        public void Start<TStateMachine> (ref TStateMachine stateMachine) where TStateMachine : System.Runtime.CompilerServices.IAsyncStateMachine
        {
        }

        public static AsyncTaskMethodBuilder Create()
        {
            return new AsyncTaskMethodBuilder();
        }
    }

    public class AsyncTaskMethodBuilder<T> : IAsyncStateMachine
    {
        public Task<T> Task { get; private set; }

        public static AsyncTaskMethodBuilder<T> Create()
        {
            return new AsyncTaskMethodBuilder<T>();
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

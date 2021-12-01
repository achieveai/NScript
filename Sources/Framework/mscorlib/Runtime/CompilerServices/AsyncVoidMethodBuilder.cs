namespace System.Runtime.CompilerServices
{
    using System;

    public class AsyncVoidMethodBuilder: IAsyncStateMachine
    {
        public Promise Task { get; private set; }

        public static AsyncVoidMethodBuilder Create()
        {
            return new AsyncVoidMethodBuilder();
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
            throw new NotImplementedException();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            throw new NotImplementedException();
        }
    }
}

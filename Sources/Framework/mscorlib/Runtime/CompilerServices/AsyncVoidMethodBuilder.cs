namespace System.Runtime.CompilerServices
{
    using System;
    using System.Threading.Tasks;

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

    public class AsyncIteratorMethodBuilder
    {
        //
        // Summary:
        //     Creates an instance of the System.Runtime.CompilerServices.AsyncIteratorMethodBuilder
        //     struct.
        //
        // Returns:
        //     The initialized instance.
        public extern static AsyncIteratorMethodBuilder Create();
        //
        // Summary:
        //     Schedules the state machine to proceed to the next action when the specified
        //     awaiter completes.
        //
        // Parameters:
        //   awaiter:
        //     The awaiter.
        //
        //   stateMachine:
        //     The state machine.
        //
        // Type parameters:
        //   TAwaiter:
        //     The type of the awaiter.
        //
        //   TStateMachine:
        //     The type of the state machine.
        public extern void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine;
        //
        // Summary:
        //     Schedules the state machine to proceed to the next action when the specified
        //     awaiter completes.
        //
        // Parameters:
        //   awaiter:
        //     The awaiter.
        //
        //   stateMachine:
        //     The state machine.
        //
        // Type parameters:
        //   TAwaiter:
        //     The type of the awaiter.
        //
        //   TStateMachine:
        //     The type of the state machine.
        public extern void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine;
        //
        // Summary:
        //     Marks iteration as being completed, whether successfully or otherwise.
        public extern void Complete();
        //
        // Summary:
        //     Invokes System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext on the state
        //     machine while guarding the System.Threading.ExecutionContext.
        //
        // Parameters:
        //   stateMachine:
        //     The state machine instance, passed by reference.
        //
        // Type parameters:
        //   TStateMachine:
        //     The type of the state machine.
        public extern void MoveNext<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine;
    }

    public struct AsyncValueTaskMethodBuilder
    {
        //
        // Summary:
        //     Gets the task for this builder.
        //
        // Returns:
        //     The task for this builder.
        public ValueTask Task { get; }

        //
        // Summary:
        //     Creates an instance of the System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder
        //     struct.
        //
        // Returns:
        //     The initialized instance.
        public static extern AsyncValueTaskMethodBuilder Create();
        //
        // Summary:
        //     Schedules the state machine to proceed to the next action when the specified
        //     awaiter completes.
        //
        // Parameters:
        //   awaiter:
        //     The awaiter.
        //
        //   stateMachine:
        //     The state machine.
        //
        // Type parameters:
        //   TAwaiter:
        //     The type of the awaiter.
        //
        //   TStateMachine:
        //     The type of the state machine.
        public extern void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine;
        //
        // Summary:
        //     Schedules the state machine to proceed to the next action when the specified
        //     awaiter completes.
        //
        // Parameters:
        //   awaiter:
        //     The awaiter.
        //
        //   stateMachine:
        //     The state machine.
        //
        // Type parameters:
        //   TAwaiter:
        //     The type of the awaiter.
        //
        //   TStateMachine:
        //     The type of the state machine.
        public extern void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine;
        //
        // Summary:
        //     Marks the task as failed and binds the specified exception to the task.
        //
        // Parameters:
        //   exception:
        //     The exception to bind to the task.
        public extern void SetException(Exception exception);
        //
        // Summary:
        //     Marks the task as successfully completed.
        public extern void SetResult();
        //
        // Summary:
        //     Associates the builder with the specified state machine.
        //
        // Parameters:
        //   stateMachine:
        //     The state machine instance to associate with the builder.
        public extern void SetStateMachine(IAsyncStateMachine stateMachine);
        //
        // Summary:
        //     Begins running the builder with the associated state machine.
        //
        // Parameters:
        //   stateMachine:
        //     The state machine instance, passed by reference.
        //
        // Type parameters:
        //   TStateMachine:
        //     The type of the state machine.
        public extern void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine;
    }

    public struct AsyncValueTaskMethodBuilder<TResult>
    {
        //
        // Summary:
        //     Gets the task for this builder.
        //
        // Returns:
        //     The task for this builder.
        public ValueTask<TResult> Task { get; }

        //
        // Summary:
        //     Creates an instance of the System.Runtime.CompilerServices.AsyncValueTaskMethodBuilder`1
        //     struct.
        //
        // Returns:
        //     The initialized instance.
        public extern static AsyncValueTaskMethodBuilder<TResult> Create();
        //
        // Summary:
        //     Schedules the state machine to proceed to the next action when the specified
        //     awaiter completes.
        //
        // Parameters:
        //   awaiter:
        //     The awaiter.
        //
        //   stateMachine:
        //     The state machine.
        //
        // Type parameters:
        //   TAwaiter:
        //     The type of the awaiter.
        //
        //   TStateMachine:
        //     The type of the state machine.
        public extern void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine;
        //
        // Summary:
        //     Schedules the state machine to proceed to the next action when the specified
        //     awaiter completes.
        //
        // Parameters:
        //   awaiter:
        //     The awaiter.
        //
        //   stateMachine:
        //     The state machine.
        //
        // Type parameters:
        //   TAwaiter:
        //     The type of the awaiter.
        //
        //   TStateMachine:
        //     The type of the state machine.
        public extern void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine;
        //
        // Summary:
        //     Marks the task as failed and binds the specified exception to the task.
        //
        // Parameters:
        //   exception:
        //     The exception to bind to the task.
        public extern void SetException(Exception exception);
        //
        // Summary:
        //     Marks the task as successfully completed.
        //
        // Parameters:
        //   result:
        //     The result to use to complete the task.
        public extern void SetResult(TResult result);
        //
        // Summary:
        //     Associates the builder with the specified state machine.
        //
        // Parameters:
        //   stateMachine:
        //     The state machine instance to associate with the builder.
        public extern void SetStateMachine(IAsyncStateMachine stateMachine);
        //
        // Summary:
        //     Begins running the builder with the associated state machine.
        //
        // Parameters:
        //   stateMachine:
        //     The state machine instance, passed by reference.
        //
        // Type parameters:
        //   TStateMachine:
        //     The type of the state machine.
        public extern void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine;
    }
}

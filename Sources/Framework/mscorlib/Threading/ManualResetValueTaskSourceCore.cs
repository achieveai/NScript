namespace System.Threading.Tasks.Sources
{
    //
    // Summary:
    //     Provides the core logic for implementing a manual-reset System.Threading.Tasks.Sources.IValueTaskSource
    //     or System.Threading.Tasks.Sources.IValueTaskSource`1.
    //
    // Type parameters:
    //   TResult:
    //     The type of the result of this manual-reset System.Threading.Tasks.Sources.IValueTaskSource`1.
    public struct ManualResetValueTaskSourceCore<TResult>
    {
        //
        // Summary:
        //     Gets or sets whether to force continuations to run asynchronously.
        //
        // Returns:
        //     true to force continuations to run asynchronously; otherwise, false.
        public extern bool RunContinuationsAsynchronously { get; set; }
        //
        // Summary:
        //     Gets the operation version.
        //
        // Returns:
        //     The operation version.
        public extern short Version { get; }

        //
        // Summary:
        //     Returns the result of the operation.
        //
        // Parameters:
        //   token:
        //     An opaque value that was provided to the System.Threading.Tasks.ValueTask constructor.
        //
        // Returns:
        //     The result of the operation.
        public extern TResult GetResult(short token);

        //
        // Summary:
        //     Gets the status of the operation.
        //
        // Parameters:
        //   token:
        //     An opaque value that was provided to the System.Threading.Tasks.ValueTask constructor.
        //
        // Returns:
        //     A value that indicates the status of the operation.
        public extern ValueTaskSourceStatus GetStatus(short token);

        //
        // Summary:
        //     Schedules the continuation action for this operation.
        //
        // Parameters:
        //   continuation:
        //     The continuation to invoke when the operation has completed.
        //
        //   state:
        //     The state object to pass to continuation when it's invoked.
        //
        //   token:
        //     An opaque value that was provided to the System.Threading.Tasks.ValueTask constructor.
        //
        //   flags:
        //     The flags describing the behavior of the continuation.
        public extern void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags);

        //
        // Summary:
        //     Resets to prepare for the next operation.
        public extern void Reset();

        //
        // Summary:
        //     Completes with an error.
        //
        // Parameters:
        //   error:
        //     The exception.
        public extern void SetException(Exception error);

        //
        // Summary:
        //     Completes with a successful result.
        //
        // Parameters:
        //   result:
        //     The result.
        public extern void SetResult(TResult result);
    }


	public enum ValueTaskSourceStatus
    {
        //
        // Summary:
        //     The operation has not yet completed.
        Pending = 0,
        //
        // Summary:
        //     The operation completed successfully.
        Succeeded = 1,
        //
        // Summary:
        //     The operation completed with an error.
        Faulted = 2,
        //
        // Summary:
        //     The operation completed due to cancellation.
        Canceled = 3
    }

        //
    // Summary:
    //     Represents an object that can be wrapped by a System.Threading.Tasks.ValueTask.
    public interface IValueTaskSource
    {
        //
        // Summary:
        //     Gets the result of the System.Threading.Tasks.Sources.IValueTaskSource.
        //
        // Parameters:
        //   token:
        //     An opaque value that was provided to the System.Threading.Tasks.ValueTask constructor.
        void GetResult(short token);
        //
        // Summary:
        //     Gets the status of the current operation.
        //
        // Parameters:
        //   token:
        //     Opaque value that was provided to the System.Threading.Tasks.ValueTask's constructor.
        //
        // Returns:
        //     The status of the current operation.
        ValueTaskSourceStatus GetStatus(short token);
        //
        // Summary:
        //     Schedules the continuation action for this System.Threading.Tasks.Sources.IValueTaskSource.
        //
        // Parameters:
        //   continuation:
        //     The continuation to invoke when the operation has completed.
        //
        //   state:
        //     The state object to pass to continuation when it's invoked.
        //
        //   token:
        //     An opaque value that was provided to the System.Threading.Tasks.ValueTask's constructor.
        //
        //   flags:
        //     The flags describing the behavior of the continuation.
        void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags);
    }

    public interface IValueTaskSource<out TResult>
    {
        //
        // Summary:
        //     Gets the result of the System.Threading.Tasks.Sources.IValueTaskSource`1.
        //
        // Parameters:
        //   token:
        //     An opaque value that was provided to the System.Threading.Tasks.ValueTask constructor.
        //
        // Returns:
        //     The result of the System.Threading.Tasks.Sources.IValueTaskSource`1.
        TResult GetResult(short token);
        //
        // Summary:
        //     Gets the status of the current operation.
        //
        // Parameters:
        //   token:
        //     Opaque value that was provided to the System.Threading.Tasks.ValueTask's constructor.
        //
        // Returns:
        //     A value that indicates the status of the current operation.
        ValueTaskSourceStatus GetStatus(short token);
        //
        // Summary:
        //     Schedules the continuation action for this System.Threading.Tasks.Sources.IValueTaskSource`1.
        //
        // Parameters:
        //   continuation:
        //     The continuation to invoke when the operation has completed.
        //
        //   state:
        //     The state object to pass to continuation when it's invoked.
        //
        //   token:
        //     An opaque value that was provided to the System.Threading.Tasks.ValueTask constructor.
        //
        //   flags:
        //     The flags describing the behavior of the continuation.
        void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags);
    }

    //
    // Summary:
    //     Provides flags passed from System.Threading.Tasks.ValueTask and System.Threading.Tasks.ValueTask`1
    //     to the OnCompleted method to control the behavior of a continuation.
    [Flags]
    public enum ValueTaskSourceOnCompletedFlags
    {
        //
        // Summary:
        //     No requirements are placed on how the continuation is invoked.
        None = 0,
        //
        // Summary:
        //     OnCompleted should capture the current scheduling context (the System.Threading.SynchronizationContext)
        //     and use it when queueing the continuation for execution. If this flag is not
        //     set, the implementation may choose to execute the continuation in an arbitrary
        //     location.
        UseSchedulingContext = 1,
        //
        // Summary:
        //     OnCompleted should capture the current System.Threading.ExecutionContext and
        //     use it to run the continuation.
        FlowExecutionContext = 2
    }
}

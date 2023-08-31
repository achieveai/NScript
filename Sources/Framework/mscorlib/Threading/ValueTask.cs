namespace System.Threading.Tasks
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks.Sources;

    [ScriptName("Promise"), IgnoreGenericArguments]
    [AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder))]
    public struct ValueTask
    {
        public extern ValueTask(Task task);

        public extern ValueTask(IValueTaskSource source, short token);

        [Script("return this;")]
        public extern TaskAwaiter GetAwaiter();
    }

    [ScriptName("Promise"), IgnoreGenericArguments]
    [AsyncMethodBuilder(typeof(AsyncValueTaskMethodBuilder<>))]
    public struct ValueTask<T>
    {
        public extern ValueTask(Task<T> task);

        public extern ValueTask(T result);

        public extern ValueTask(IValueTaskSource<T> source, short token);

        [Script("return this;")]
        public extern TaskAwaiter<T> GetAwaiter();
    }
}

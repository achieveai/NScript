namespace System.Threading
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Task
    {
        public extern TaskAwaiter GetAwaiter();
    }

    public class Task<T>
    {
        extern public TaskAwaiter<T> GetAwaiter();
    }
}

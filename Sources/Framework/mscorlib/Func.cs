namespace System
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Function delegate.
    /// </summary>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<out TRes>();

    /// <summary>
    /// One parameter function delegate.
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<in T1, out TRes>(T1 arg1);

    /// <summary>
    /// Two parameter Func delegate.
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<in T1, in T2, out TRes>(T1 arg1, T2 arg2);

    /// <summary>
    /// Three parameter Func delegate.
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<in T1, in T2, in T3, out TRes>(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Four parameter Func delegate
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<in T1, in T2, in T3, in T4, out TRes>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    /// <summary>
    /// Five parameter Func delegate.
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<in T1, in T2, in T3, in T4, in T5, out TRes>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    /// <summary>
    /// Six parameter Func delegate.
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="T6">The type of the 6.</typeparam>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<in T1, in T2, in T3, in T4, in T5, in T6, out TRes>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
}


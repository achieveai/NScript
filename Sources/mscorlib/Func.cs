namespace System
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Function delegate.
    /// </summary>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<TRes>();

    /// <summary>
    /// One parameter function delegate.
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<T1, TRes>(T1 arg1);

    /// <summary>
    /// Two parameter Func delegate.
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<T1, T2, TRes>(T1 arg1, T2 arg2);

    /// <summary>
    /// Three parameter Func delegate.
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<T1, T2, T3, TRes>(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Four parameter Func delegate
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="TRes">The type of the res.</typeparam>
    /// <returns></returns>
    public delegate TRes Func<T1, T2, T3, T4, TRes>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

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
    public delegate TRes Func<T1, T2, T3, T4, T5, TRes>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

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
    public delegate TRes Func<T1, T2, T3, T4, T5, T6, TRes>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
}


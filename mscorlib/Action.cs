namespace System
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Action delegate
    /// </summary>
    public delegate void Action();

    /// <summary>
    /// One parameter Action delegate
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <param name="obj1">The obj1.</param>
    public delegate void Action<T1>(T1 obj1);

    /// <summary>
    /// Two parameter Action delegate
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <param name="obj1">The obj1.</param>
    /// <param name="obj2">The obj2.</param>
    public delegate void Action<T1, T2>(T1 obj1, T2 obj2);

    /// <summary>
    /// Three parameter Action delegate
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <param name="obj1">The obj1.</param>
    /// <param name="obj2">The obj2.</param>
    /// <param name="obj3">The obj3.</param>
    public delegate void Action<T1, T2, T3>(T1 obj1, T2 obj2, T3 obj3);

    /// <summary>
    /// Four parameter Action delegate
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <param name="obj1">The obj1.</param>
    /// <param name="obj2">The obj2.</param>
    /// <param name="obj3">The obj3.</param>
    /// <param name="obj4">The obj4.</param>
    public delegate void Action<T1, T2, T3, T4>(T1 obj1, T2 obj2, T3 obj3, T4 obj4);

    /// <summary>
    /// Five parameter Action delegate
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <param name="obj1">The obj1.</param>
    /// <param name="obj2">The obj2.</param>
    /// <param name="obj3">The obj3.</param>
    /// <param name="obj4">The obj4.</param>
    /// <param name="obj5">The obj5.</param>
    public delegate void Action<T1, T2, T3, T4, T5>(T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5);

    /// <summary>
    /// Six parameter Action delegate
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <typeparam name="T4">The type of the 4.</typeparam>
    /// <typeparam name="T5">The type of the 5.</typeparam>
    /// <typeparam name="T6">The type of the 6.</typeparam>
    /// <param name="obj1">The obj1.</param>
    /// <param name="obj2">The obj2.</param>
    /// <param name="obj3">The obj3.</param>
    /// <param name="obj4">The obj4.</param>
    /// <param name="obj5">The obj5.</param>
    /// <param name="obj6">The obj6.</param>
    public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6);
}


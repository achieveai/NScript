namespace System
{
    /// <summary>
    /// Action delegate
    /// </summary>
    public delegate void Action();

    /// <summary>
    /// One parameter Action delegate
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <param name="obj1">The obj1.</param>
    public delegate void Action<in T1>(T1 obj1);

    /// <summary>
    /// Two parameter Action delegate
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <param name="obj1">The obj1.</param>
    /// <param name="obj2">The obj2.</param>
    public delegate void Action<in T1, in T2>(T1 obj1, T2 obj2);

    /// <summary>
    /// Three parameter Action delegate
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    /// <typeparam name="T3">The type of the 3.</typeparam>
    /// <param name="obj1">The obj1.</param>
    /// <param name="obj2">The obj2.</param>
    /// <param name="obj3">The obj3.</param>
    public delegate void Action<in T1, in T2, in T3>(T1 obj1, T2 obj2, T3 obj3);

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
    public delegate void Action<in T1, in T2, in T3, in T4>(T1 obj1, T2 obj2, T3 obj3, T4 obj4);

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
    public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5);

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
    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6>(T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6);

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(
        T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7);

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(
        T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8);

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9>(
        T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9);

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10>(
        T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9, T10 obj10);

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11>(
        T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9, T10 obj10, T11 obj11);

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12>(
        T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9, T10 obj10, T11 obj11, T12 obj12);

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13>(
        T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9, T10 obj10, T11 obj11, T12 obj12, T13 obj13);

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14>(
        T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9, T10 obj10, T11 obj11, T12 obj12, T13 obj13, T14 obj14);

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15>(
        T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9, T10 obj10, T11 obj11, T12 obj12, T13 obj13, T14 obj14, T15 obj15);

    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16>(
        T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9, T10 obj10, T11 obj11, T12 obj12, T13 obj13, T14 obj14, T15 obj15, T16 obj16);
}

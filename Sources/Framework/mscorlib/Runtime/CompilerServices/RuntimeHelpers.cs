namespace System.Runtime.CompilerServices
{
    using System;
    using System.ComponentModel;

    [Extended, EditorBrowsable(EditorBrowsableState.Never), NonScriptable]
    public class RuntimeHelpers
    {
        public static void InitializeArray(Array array, RuntimeFieldHandle handle)
        {
        }

        /// <summary>
        /// Slices an array using a <see cref="Range"/>. Used by Roslyn when
        /// lowering <c>arr[range]</c> for array receivers in C# 8 indexer
        /// access. The Stage-1 NScript visitor lowers
        /// <c>BoundImplicitIndexerAccess</c> with a <c>Range</c> argument
        /// into a call to this method.
        /// </summary>
        public static T[] GetSubArray<T>(T[] array, Range range)
        {
            int start = range.Start.GetOffset(array.Length);
            int end = range.End.GetOffset(array.Length);
            int length = end - start;
            T[] result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = array[start + i];
            }

            return result;
        }
    }
}

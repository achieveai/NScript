using System.Runtime.CompilerServices;

namespace RealScript
{
    [IgnoreGenericArguments]
    public class GenericResponse<T>
    {
        public T Object { get; set; }

        public bool IsError { get; set; }
    }

    public static class GenericResponseExtension
    {
        [IgnoreGenericArguments]
        public static void Deconstruct<T>(
            this GenericResponse<T> myGenericInstance,
            out T obj,
            out bool isErr)
        {
            obj = myGenericInstance.Object;
            isErr = myGenericInstance.IsError;
        }
    }
}

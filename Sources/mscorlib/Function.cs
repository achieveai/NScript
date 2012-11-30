namespace System
{
    using System.Runtime.CompilerServices;

    [IgnoreNamespace]
    public struct Function
    {
        public extern int Length
        { get; }

        [IgnoreGenericArguments]
        [ScriptAlias("apply")]
        public extern void ApplyAction<T>(T instance, NativeArray arguments);

        [IgnoreGenericArguments]
        [ScriptAlias("apply")]
        public extern U ApplyFunc<T,U>(T instance, NativeArray arguments);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern void CallAction<T>(T instance);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern void CallAction<T, A1>(T instance, A1 arg1);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern void CallAction<T, A1, A2>(T instance, A1 arg1, A2 arg2);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern void CallAction<T, A1, A2, A3>(T instance, A1 arg1, A2 arg2, A3 arg3);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern void CallAction<T, A1, A2, A3, A4>(T instance, A1 arg1, A2 arg2, A3 arg3, A4 arg4);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern void CallAction<T, A1, A2, A3, A4, A5>(T instance, A1 arg1, A2 arg2, A3 arg3, A4 arg4, A5 arg5);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern U CallFunc<T, U>(T instance);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern U CallFunc<T, U, A1>(T instance, A1 arg1);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern U CallFunc<T, U, A1, A2>(T instance, A1 arg1, A2 arg2);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern U CallFunc<T, U, A1, A2, A3>(T instance, A1 arg1, A2 arg2, A3 arg3);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern U CallFunc<T, U, A1, A2, A3, A4>(T instance, A1 arg1, A2 arg2, A3 arg3, A4 arg4);

        [IgnoreGenericArguments]
        [ScriptAlias("call")]
        public extern U CallFunc<T, U, A1, A2, A3, A4, A5>(T instance, A1 arg1, A2 arg2, A3 arg3, A4 arg4, A5 arg5);
    }
}


namespace RealScript
{
    using System;
    using System.Collections.Generic;
    public static class TestDefaultExpr
    {
        public static void TestReferenceType()
        {
            MyClass cls = default;
        }
        public static void TestGenericReferenceType()
        {
            MyGenericClass<MyClass, int, MyGenericClass<int, int, MyGenericClass<string, int, (int, char, string)>>> cls
                = default;
        }

        public static void TestBuiltinValueTypes()
        {
            int a = default;
            char c = default;
            double d = default;
            string s = default;
        }

        public static void TestValueType()
        {
            MyStruct str = default;
        }
        public static void TestGenericValueType()
        {
            MyGenericStruct<MyGenericStruct<MyGenericStruct<int, char, (int, MyGenericStruct<string, double, int>)>, int, string>, char, MyClass>
                s = default;
        }

        private class MyClass
        {
            int x;
            MyClass cls;
        }
        private class MyGenericClass<T1, T2, T3>
        {
            T1 x;
            T2 y;
            T3 z;
        }

        private struct MyStruct
        {
            int x;
            MyClass cls;
        }

        private struct MyGenericStruct<T1, T2, T3>
        {
            T1 x;
            T2 y;
            T3 z;
        }
    }
}

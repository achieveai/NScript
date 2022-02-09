namespace RealScript
{
    class TestMethodArguments
    {
        private static int gv = 0;

        public static void TestConstructor()
        {
            var n = new NestedClass(d: "d", c: "c", a: "a", b: 2);
            n = new NestedClass(e: 5, a: 1, b: "b", d: "d", c: 3);
            n = new NestedClass("a", 2, d: "d");

            n = new NestedClass(b: MutatedIntGV(), d: GetString(), c: GetString(), a: "a");
        }

        public static void TestLocalMethod()
        {
            void LocalMethod1(int a, string b, string c = default, string d = "d") { }
            void LocalMethod2(int a, string b, string c, string d) { }

            LocalMethod1(d: "d", a: 12, b: default);
            LocalMethod2(1, "b", d: "d", c: "c");

            LocalMethod1(d: GetString(), a: 1, b: GetString());
            LocalMethod1(b: GetString(), a: 1, d: GetString());

            LocalMethod2(b: GetString(), a: 1, d: GetString(), c: GetString());
            LocalMethod2(b: GetString(), a: MutatedIntGV(), c: GetString(), d: GetString());
        }

        public static void TestInstanceMethods()
        {
            new TestMethodArguments().Method1(1, 2, y: "yp");
            new TestMethodArguments().Method1(1, 2, z: "zp");
            new TestMethodArguments().Method1(1, 2, z: "zp", x: "xp");
            new TestMethodArguments().Method1(1, 2, z: "zp", y: "yp");
            new TestMethodArguments().Method1(1, 2, y: "yp", x: "xp");

            new TestMethodArguments().Method1(b: MutatedIntGV(), a: 2, y: GetString(), x: GetString());
        }

        public static void TestStaticMethods()
        {
            ClassMethod1(b: "b", a: 1);
            ClassMethod1(b: "b", c1: null, a: 1);

            ClassMethod2(b: 100);
            ClassMethod2(b: 100, a: 99, d: "d");

            ClassMethod2(b: MutatedIntGV(), a: MutatedIntGV(), d: GetString());
        }

        public static int MutatedIntGV() => ++gv;

        public static string GetString() => "asdf";

        private static void ClassMethod1(int a, string b, MyClass c1 = null) { }

        private static void ClassMethod2(int a = 1, int b = 2, string c = "c", string d = default) { }

        private void Method1(int a, int b, string x = "x", string y = "y", string z = "z") { }

        private class NestedClass
        {
            public NestedClass (int a, string b, int c = 12, string d = "d", int e = 78) { }

            public NestedClass(string a, int b, string c = "c", string d = "d") { }
        }
    }
}

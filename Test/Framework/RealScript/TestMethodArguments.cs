namespace RealScript
{
    class TestMethodArguments
    {
        public static void Main()
        {
            ClassMethod1(b: "asdf", a: 12);
            ClassMethod2(c: "qwe");
            new TestMethodArguments().Method1(b: 1, a: 2);

            void LocalMethod1(int a, string b, string y = default, string z = "asdlfkj") { }

            LocalMethod1(z: "zzz", a: 12, b: default);

            var n = new NestedClass(c: 90, a: 12, b: "asdf");
            n = new NestedClass(c: "clkj", a: "asdf", b: 2, d: "dflkj");
        }

        private static void ClassMethod1(int a, string b, MyClass c1 = null) { }

        private static void ClassMethod2(int a = 1, int b = 12, string c = "asdf", string d = default) { }

        private void Method1(int a, int b, string x = "asdf", string y = "asdflkj", string z = "qweqwe") { }

        private class NestedClass
        {
            public NestedClass (int a, string b, int c = 12, string d = "dfgh", int e = 78) { }

            public NestedClass(string a, int b, string c = "clkj", string d = "dflkhl") { }
        }
    }
}

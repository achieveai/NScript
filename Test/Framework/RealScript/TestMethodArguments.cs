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
        }

        private static void ClassMethod1(int a, string b, MyClass c1 = null) { }

        private static void ClassMethod2(int a = 1, int b = 12, string c = "asdf", string d = default) { }

        private void Method1(int a, int b, string x = "asdf", string y = "asdflkj", string z = "qweqwe")
        {
        }
    }
}

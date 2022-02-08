function TestMethodArguments__Main() {
  var n;
  TestMethodArguments__ClassMethod1(12, "asdf", null);
  TestMethodArguments__ClassMethod2(1, 12, "qwe", null);
  TestMethodArguments_factory().method1(2, 1, "asdf", "asdflkj", "qweqwe");
  function LocalMethod1(a, b, y, z) {
    return;
  };
  LocalMethod1(12, null, null, "zzz");
  n = NestedClass_factory(12, "asdf", 90, "dfgh", 78);
  n = NestedClass_factorya("asdf", 2, "clkj", "dflkj");
}
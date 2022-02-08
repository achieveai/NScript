function TestMethodArguments__Main() {
  TestMethodArguments__ClassMethod1(12, "asdf", null);
  TestMethodArguments__ClassMethod2(1, 12, "qwe", null);
  TestMethodArguments_factory().method1(2, 1, "asdf", "asdflkj", "qweqwe");
  function LocalMethod1(a, b, y, z) {
    return;
  };
  LocalMethod1(12, null, null, "zzz");
}
function TestMethodArguments__Main() {
  var n, stmtTemp1, stmtTemp1a;
  TestMethodArguments__ClassMethod1(12, "asdf", null);
  TestMethodArguments__ClassMethod2(1, 12, "qwe", null);
  TestMethodArguments_factory().method1(2, 1, "asdf", "asdflkj", "qweqwe");
  function LocalMethod1(a, b, y, z) {
    return;
  };
  LocalMethod1(12, null, null, "zzz");
  n = NestedClass_factory(12, "asdf", 90, "dfgh", 78);
  n = NestedClass_factorya("asdf", 2, "clkj", "dflkj");
  stmtTemp1 = TestMethodArguments__MutateGV(), stmtTemp1a = TestMethodArguments__MutateGV(), TestMethodArguments__ClassMethod2(stmtTemp1a, stmtTemp1, "asdf", null);
  n = (stmtTemp1 = TestMethodArguments__MutateGV(), stmtTemp1a = TestMethodArguments__MutateGV(), NestedClass_factory(stmtTemp1a, "uioyuiy", 12, "dfgh", stmtTemp1));
}
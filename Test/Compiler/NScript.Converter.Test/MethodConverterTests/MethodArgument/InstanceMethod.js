function TestMethodArguments__TestInstanceMethods() {
  var stmtTemp1, stmtTemp1a, stmtTemp1b;
  TestMethodArguments_factory().method1(1, 2, "x", "yp", "z");
  TestMethodArguments_factory().method1(1, 2, "x", "y", "zp");
  TestMethodArguments_factory().method1(1, 2, "xp", "y", "zp");
  TestMethodArguments_factory().method1(1, 2, "x", "yp", "zp");
  TestMethodArguments_factory().method1(1, 2, "xp", "yp", "z");
  stmtTemp1 = TestMethodArguments__MutatedIntGV(), stmtTemp1a = TestMethodArguments__GetString(), stmtTemp1b = TestMethodArguments__GetString(), TestMethodArguments_factory().method1(2, stmtTemp1, stmtTemp1b, stmtTemp1a, "z");
}
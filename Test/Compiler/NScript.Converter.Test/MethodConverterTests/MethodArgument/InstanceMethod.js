function TestMethodArguments__TestInstanceMethods() {
  var stmtTemp1, stmtTemp1a, stmtTemp1b, stmtTemp1c;
  TestMethodArguments_factory().method1(1, 2, "x", "yp", "z");
  TestMethodArguments_factory().method1(1, 2, "x", "y", "zp");
  TestMethodArguments_factory().method1(1, 2, "xp", "y", "zp");
  TestMethodArguments_factory().method1(1, 2, "x", "yp", "zp");
  TestMethodArguments_factory().method1(1, 2, "xp", "yp", "z");
  stmtTemp1 = TestMethodArguments_factory(), stmtTemp1a = TestMethodArguments__MutatedIntGV(), stmtTemp1b = TestMethodArguments__GetString(), stmtTemp1c = TestMethodArguments__GetString(), stmtTemp1.method1(2, stmtTemp1a, stmtTemp1c, stmtTemp1b, "z");
}
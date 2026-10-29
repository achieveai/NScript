function TestMethodArguments__TestInstanceMethods() {
  var stmtTemp1, stmtTemp1a, stmtTemp1b, stmtTemp1c;
  TestMethodArguments__Method1(TestMethodArguments_factory(), 1, 2, "x", "yp", "z");
  TestMethodArguments__Method1(TestMethodArguments_factory(), 1, 2, "x", "y", "zp");
  TestMethodArguments__Method1(TestMethodArguments_factory(), 1, 2, "xp", "y", "zp");
  TestMethodArguments__Method1(TestMethodArguments_factory(), 1, 2, "x", "yp", "zp");
  TestMethodArguments__Method1(TestMethodArguments_factory(), 1, 2, "xp", "yp", "z");
  stmtTemp1 = TestMethodArguments_factory(), stmtTemp1a = TestMethodArguments__MutatedIntGV(), stmtTemp1b = TestMethodArguments__GetString(), stmtTemp1c = TestMethodArguments__GetString(), TestMethodArguments__Method1(stmtTemp1, 2, stmtTemp1a, stmtTemp1c, stmtTemp1b, "z");
}
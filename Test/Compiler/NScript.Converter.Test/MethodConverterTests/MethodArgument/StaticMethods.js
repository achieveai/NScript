function TestMethodArguments__TestStaticMethods() {
  var stmtTemp1, stmtTemp1a, stmtTemp1b;
  TestMethodArguments__ClassMethod1(1, "b", null);
  TestMethodArguments__ClassMethod1(1, "b", null);
  TestMethodArguments__ClassMethod2(1, 100, "c", null);
  TestMethodArguments__ClassMethod2(99, 100, "c", "d");
  stmtTemp1 = TestMethodArguments__MutatedIntGV(), stmtTemp1a = TestMethodArguments__MutatedIntGV(), stmtTemp1b = TestMethodArguments__GetString(), TestMethodArguments__ClassMethod2(stmtTemp1a, stmtTemp1, "c", stmtTemp1b);
}
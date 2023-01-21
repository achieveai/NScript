function TestMethodArguments__TestLocalMethod() {
  var stmtTemp1, stmtTemp1a, stmtTemp1b;
  function LocalMethod1(a, b, c, d) {
    return;
  }
  function LocalMethod2(a, b, c, d) {
    return;
  }
  LocalMethod1(12, null, null, "d");
  LocalMethod2(1, "b", "c", "d");
  stmtTemp1 = TestMethodArguments__GetString(), stmtTemp1a = TestMethodArguments__GetString(), LocalMethod1(1, stmtTemp1a, null, stmtTemp1);
  stmtTemp1 = TestMethodArguments__GetString(), stmtTemp1a = TestMethodArguments__GetString(), LocalMethod1(1, stmtTemp1, null, stmtTemp1a);
  stmtTemp1 = TestMethodArguments__GetString(), stmtTemp1a = TestMethodArguments__GetString(), stmtTemp1b = TestMethodArguments__GetString(), LocalMethod2(1, stmtTemp1, stmtTemp1b, stmtTemp1a);
  stmtTemp1 = TestMethodArguments__GetString(), stmtTemp1a = TestMethodArguments__MutatedIntGV(), LocalMethod2(stmtTemp1a, stmtTemp1, TestMethodArguments__GetString(), TestMethodArguments__GetString());
}
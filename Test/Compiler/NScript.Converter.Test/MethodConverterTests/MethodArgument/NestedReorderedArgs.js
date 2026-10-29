function TestMethodArguments__TestNestedReorderedArgs() {
  var stmtTemp1, stmtTemp1a, stmtTemp1b;
  stmtTemp1 = (stmtTemp1 = TestMethodArguments__gv, stmtTemp1a = TestMethodArguments__GetString(), stmtTemp1b = TestMethodArguments__MutatedIntGV(), TestMethodArguments__Chain(stmtTemp1, stmtTemp1b, stmtTemp1a)), stmtTemp1a = TestMethodArguments__GetString(), stmtTemp1b = TestMethodArguments__MutatedIntGV(), TestMethodArguments__Chain(stmtTemp1, stmtTemp1b, stmtTemp1a);
  stmtTemp1b = (stmtTemp1 = TestMethodArguments__GetString(), stmtTemp1a = TestMethodArguments__MutatedIntGV(), TestMethodArguments__Make(stmtTemp1a, stmtTemp1)), stmtTemp1 = TestMethodArguments__MutatedIntGV(), stmtTemp1b.method1(2, stmtTemp1, "x", "y", "z");
}
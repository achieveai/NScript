function TestMethodArguments__TestLambdaArgs() {
  var stmtTemp1, stmtTemp1a;
  stmtTemp1 = function TestMethodArguments__TestLambdaArgs_del() {
    var stmtTemp1b, stmtTemp1c, stmtTemp1d;
    return stmtTemp1b = TestMethodArguments__gv, stmtTemp1c = TestMethodArguments__GetString(), stmtTemp1d = TestMethodArguments__MutatedIntGV(), TestMethodArguments__Chain(stmtTemp1b, stmtTemp1d, stmtTemp1c);
  }, stmtTemp1a = TestMethodArguments__GetString(), TestMethodArguments__Lambda(stmtTemp1, 1, stmtTemp1a);
  stmtTemp1 = TestMethodArguments__GetString(), stmtTemp1a = function TestMethodArguments__TestLambdaArgs_del2() {
    var stmtTemp1b, stmtTemp1c, stmtTemp1d;
    return stmtTemp1b = TestMethodArguments__gv, stmtTemp1c = TestMethodArguments__GetString(), stmtTemp1d = TestMethodArguments__MutatedIntGV(), TestMethodArguments__Chain(stmtTemp1b, stmtTemp1d, stmtTemp1c);
  }, TestMethodArguments__Lambda(stmtTemp1a, 1, stmtTemp1);
}
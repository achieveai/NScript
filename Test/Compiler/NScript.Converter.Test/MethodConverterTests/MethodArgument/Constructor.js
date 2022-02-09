function TestMethodArguments__TestConstructor() {
  var n, stmtTemp1, stmtTemp1a;
  n = NestedClass_factory("a", 2, "c", "d");
  n = NestedClass_factorya(1, "b", 3, "d", 5);
  n = NestedClass_factory("a", 2, "c", "d");
  n = (stmtTemp1 = TestMethodArguments__MutatedIntGV(), stmtTemp1a = TestMethodArguments__GetString(), NestedClass_factory("a", stmtTemp1, TestMethodArguments__GetString(), stmtTemp1a));
}

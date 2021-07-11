function GenericRegressions__TestGenericMethodCalls() {
  var tmp, tmp1, tmp2;
  tmp = TestGeneric_$Int32$_.defaultConstructor();
  tmp1 = TestGeneric_$Int64$_.defaultConstructor();
  tmp2 = TestGeneric_$Int32$_.defaultConstructor();
  tmp.foo(1);
  tmp.fooa(tmp2);
  tmp.foob(Int32, tmp2);
  tmp.fooc(Int64, tmp2, tmp1);
}
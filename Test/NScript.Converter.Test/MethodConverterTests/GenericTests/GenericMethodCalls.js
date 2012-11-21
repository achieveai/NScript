function RealScript__GenericRegressions__TestGenericMethodCalls() {
  var tmp, tmp1, tmp2;
  tmp = RealScript_TestGeneric_$Int32$_.defaultConstructor();
  tmp1 = RealScript_TestGeneric_$Int64$_.defaultConstructor();
  tmp2 = RealScript_TestGeneric_$Int32$_.defaultConstructor();
  tmp.foo(1);
  tmp.fooa(tmp2);
  tmp.foob(System_Int32, tmp2);
  tmp.fooc(System_Int64, tmp2, tmp1);
}
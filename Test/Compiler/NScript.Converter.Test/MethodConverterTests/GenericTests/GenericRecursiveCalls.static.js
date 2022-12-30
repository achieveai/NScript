function TestGenerics__TestRecursiveTypes() {
  var a, b;
  Console__WriteLine("Testing recursive types...");
  a = TestGenericA_$TestGenericB$_.defaultConstructor();
  b = TestGenericB_factory();
  Console__WriteLinea(a.m(b));
  Console__WriteLinea(TestGenericB__M(b, a));
}
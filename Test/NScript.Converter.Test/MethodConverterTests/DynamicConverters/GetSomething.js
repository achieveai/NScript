function DynamicTest__GetSomething(test) {
  test.bar = DynamicTest__GetFoo();
  test.iB = test.iA + test.iC;
  return Type__CastType(String, test.foo);
}
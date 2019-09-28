function DynamicTest__CheckIndex() {
  DynamicTest__GetFoo().this = 10;
  return Type__CastType(Int32, DynamicTest__GetFoo().bar);
}
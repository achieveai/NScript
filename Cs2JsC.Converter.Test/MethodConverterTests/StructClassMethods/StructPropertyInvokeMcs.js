function RealScript__StructClass2__GetFooValue(st2, st1) {
  if (RealScript__StructClass2__get_Value(st2) < 0)
    return RealScript__StructClass__get_FooValue(st1);
  return RealScript__StructClass2__get_FooValue(st2);
}

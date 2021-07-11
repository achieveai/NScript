function StructClass2__GetFooValue(st2, st1) {
  if (StructClass2__get_Value(st2) < 0)
    return StructClass__get_FooValue(st1);
  return StructClass2__get_FooValue(st2);
}

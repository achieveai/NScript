function StructClass2__GetFooValue(st2, st1) {
  var tmp_;
  if (StructClass2__get_Value(st2) < 0)
    tmp_ = StructClass__get_FooValue(st1);
  else
    tmp_ = StructClass2__get_FooValue(st2);
  return tmp_;
}

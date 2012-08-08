function RealScript__StructClass2__GetFooValue(st2, st1) {
  var tmp_;
  if (RealScript__StructClass2__get_Value(st2) < 0)
    tmp_ = RealScript__StructClass__get_FooValue(st1);
  else
    tmp_ = RealScript__StructClass2__get_FooValue(st2);
  return tmp_;
}

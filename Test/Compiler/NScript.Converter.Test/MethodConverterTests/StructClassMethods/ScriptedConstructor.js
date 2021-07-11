function StructClass2_factory(i, j, foo) {
  var this_;
  this_ = StructClass2.getDefaultValue();
  this_.i = i + j;
  this_.foo = foo;
  return this_;
}
function StructClass_factory(i, foo) {
  var this_;
  this_ = StructClass.getDefaultValue();
  this_.i = i;
  this_.foo = foo;
  return this_;
}

function StructClass(boxedValue) {
  this.boxedValue = boxedValue;
};
StructClass.typeId = "c";
StructClass.getDefaultValue = function() {
  return {
    i: 0,
    foo: null
  };
};
function StructClass_factory(i, foo) {
  var this_;
  this_ = StructClass.getDefaultValue();
  this_.i = i;
  this_.foo = foo;
  return this_;
};
function StructClass_factorya(i, j, foo) {
  var this_;
  this_ = StructClass.getDefaultValue();
  this_.i = i + j;
  this_.foo = foo;
  return this_;
};
function StructClass__get_Value(this_) {
  return this_.i;
};
function StructClass__set_Value(this_, value) {
  this_.i = value;
};
function StructClass__get_FooValue(this_) {
  return this_.foo;
};
function StructClass__set_FooValue(this_, value) {
  this_.foo = value;
};
function StructClass__Simple0ArgObjectAccessMethod(this_) {
  return this_.foo.V_Foo_b();
};
function StructClass__GetInt(this_) {
  return this_.i;
};
function StructClass__Foo(this_) {
  return 1;
};
function StructClass__FooVoid(this_) {
};
function StructClass__CallStructOverride(this_, sc) {
  return StructClass__ToString(sc);
};
function StructClass__GetFunc(this_) {
  return Delegate__StaticInstanceCreate("fooVoid", this_, StructClass__FooVoid);
};
function StructClass__GetCalculatedInt(this_, foo) {
  return this_.i + foo.V_Foo_b();
};
function StructClass__ToString(this_) {
  return String__Format("{0}, {1}", ArrayG_$Object$_.__ctor([Type__BoxTypeInstance(Int32, this_.i), this_.foo]));
};
ptyp_ = new ValueType();
StructClass.prototype = ptyp_;
ptyp_.toString = function() {
  return StructClass__ToString(this.boxedValue);
};
ptyp_.V_Foo_b = function() {
  return StructClass__Foo(this.boxedValue);
};
Type__RegisterStructType(StructClass, "RealScript.StructClass", [Foo]);
function StructClass2(boxedValue) {
  this.boxedValue = boxedValue;
};
StructClass2.typeId = "d";
StructClass2.getDefaultValue = function() {
  return {
    i: 0,
    foo: null
  };
};
function StructClass2_factory(i, j, foo) {
  var this_;
  this_ = StructClass2.getDefaultValue();
  this_.i = i + j;
  this_.foo = foo;
  return this_;
};
function StructClass2__get_Value(this_) {
  return this_.i;
};
function StructClass2__set_Value(this_, value) {
  this_.i = value;
};
function StructClass2__get_FooValue(this_) {
  return this_.foo;
};
function StructClass2__set_FooValue(this_, value) {
  this_.foo = value;
};
function StructClass2__GetScriptedInt(this_) {
  return this_.i + 10;
};
function StructClass2__Foo(this_) {
  return 1;
};
function StructClass2__GetFooValue(st2, st1) {
  if (StructClass2__get_Value(st2) < 0)
    return StructClass__get_FooValue(st1);
  return StructClass2__get_FooValue(st2);
};
ptyp_ = new ValueType();
StructClass2.prototype = ptyp_;
ptyp_.V_Foo_b = function() {
  return StructClass2__Foo(this.boxedValue);
};
Type__RegisterStructType(StructClass2, "RealScript.StructClass2", [Foo]);

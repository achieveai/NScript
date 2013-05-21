function RealScript_StructClass(boxedValue) {
  this.boxedValue = boxedValue;
};
RealScript_StructClass.typeId = "c";
RealScript_StructClass.getDefaultValue = function() {
  return {
    i: 0,
    foo: null
  };
};
function RealScript__StructClass_factory(i, foo) {
  var this_;
  this_ = RealScript_StructClass.getDefaultValue();
  this_.i = i;
  this_.foo = foo;
  return this_;
};
function RealScript__StructClass_factorya(i, j, foo) {
  var this_;
  this_ = RealScript_StructClass.getDefaultValue();
  this_.i = i + j;
  this_.foo = foo;
  return this_;
};
function RealScript__StructClass__get_Value(this_) {
  return this_.i;
};
function RealScript__StructClass__set_Value(this_, value) {
  this_.i = value;
};
function RealScript__StructClass__get_FooValue(this_) {
  return this_.foo;
};
function RealScript__StructClass__set_FooValue(this_, value) {
  this_.foo = value;
};
function RealScript__StructClass__Simple0ArgObjectAccessMethod(this_) {
  return this_.foo.V_Foo_b();
};
function RealScript__StructClass__GetInt(this_) {
  return this_.i;
};
function RealScript__StructClass__Foo(this_) {
  return 1;
};
function RealScript__StructClass__FooVoid(this_) {
};
function RealScript__StructClass__CallStructOverride(this_, sc) {
  return RealScript__StructClass__ToString(sc);
};
function RealScript__StructClass__GetFunc(this_) {
  return System__Delegate__StaticInstanceCreate("fooVoid", this_, RealScript__StructClass__FooVoid);
};
function RealScript__StructClass__GetCalculatedInt(this_, foo) {
  return this_.i + foo.V_Foo_b();
};
function RealScript__StructClass__ToString(this_) {
  return System__String__Format("{0}, {1}", System_ArrayG_$Object$_.__ctor([System__Type__BoxTypeInstance(System_Int32, this_.i), this_.foo]));
};
ptyp_ = new System_ValueType();
RealScript_StructClass.prototype = ptyp_;
ptyp_.toString = function() {
  return RealScript__StructClass__ToString(this.boxedValue);
};
ptyp_.V_Foo_b = function() {
  return RealScript__StructClass__Foo(this.boxedValue);
};
System__Type__RegisterStructType(RealScript_StructClass, "RealScript.StructClass", [RealScript_Foo]);
function RealScript_StructClass2(boxedValue) {
  this.boxedValue = boxedValue;
};
RealScript_StructClass2.typeId = "d";
RealScript_StructClass2.getDefaultValue = function() {
  return {
    i: 0,
    foo: null
  };
};
function RealScript__StructClass2_factory(i, j, foo) {
  var this_;
  this_ = RealScript_StructClass2.getDefaultValue();
  this_.i = i + j;
  this_.foo = foo;
  return this_;
};
function RealScript__StructClass2__get_Value(this_) {
  return this_.i;
};
function RealScript__StructClass2__set_Value(this_, value) {
  this_.i = value;
};
function RealScript__StructClass2__get_FooValue(this_) {
  return this_.foo;
};
function RealScript__StructClass2__set_FooValue(this_, value) {
  this_.foo = value;
};
function RealScript__StructClass2__GetScriptedInt(this_) {
  return this_.i + 10;
};
function RealScript__StructClass2__Foo(this_) {
  return 1;
};
function RealScript__StructClass2__GetFooValue(st2, st1) {
  if (RealScript__StructClass2__get_Value(st2) < 0)
    return RealScript__StructClass__get_FooValue(st1);
  return RealScript__StructClass2__get_FooValue(st2);
};
ptyp_ = new System_ValueType();
RealScript_StructClass2.prototype = ptyp_;
ptyp_.V_Foo_b = function() {
  return RealScript__StructClass2__Foo(this.boxedValue);
};
System__Type__RegisterStructType(RealScript_StructClass2, "RealScript.StructClass2", [RealScript_Foo]);

function SameNameInstanceAndStaticMethod() { }
SameNameInstanceAndStaticMethod.typeId = "b";
function SameNameInstanceAndStaticMethod__GetInt(c) {
  return c.intFiled;
}
function SameNameInstanceAndStaticMethod__GetInta(this_) {
  return this_.intFiled;
}
function SameNameInstanceAndStaticMethod____ctor(this_) { }
function SameNameInstanceAndStaticMethod_factory() {
  var this_;
  this_ = new SameNameInstanceAndStaticMethod();
  SameNameInstanceAndStaticMethod____ctor(this_);
  return this_;
}
SameNameInstanceAndStaticMethod.defaultConstructor = SameNameInstanceAndStaticMethod_factory;
ptyp_ = SameNameInstanceAndStaticMethod.prototype;
ptyp_.intFiled = 0;
Type__RegisterReferenceType(SameNameInstanceAndStaticMethod, "RealScript.SameNameInstanceAndStaticMethod", Object, []);
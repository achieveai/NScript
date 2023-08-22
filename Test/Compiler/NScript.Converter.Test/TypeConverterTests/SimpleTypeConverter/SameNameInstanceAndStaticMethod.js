function SameNameInstanceAndStaticMethod() { }
SameNameInstanceAndStaticMethod.typeId = "b";
function SameNameInstanceAndStaticMethod__GetInt(c) {
  return c.intFiled;
}
function SameNameInstanceAndStaticMethod_factory() {
  var this_;
  this_ = new SameNameInstanceAndStaticMethod();
  this_.__ctor();
  return this_;
}
SameNameInstanceAndStaticMethod.defaultConstructor = SameNameInstanceAndStaticMethod_factory;
ptyp_ = SameNameInstanceAndStaticMethod.prototype;
ptyp_.intFiled = 0;
ptyp_.getInt = function SameNameInstanceAndStaticMethod__GetInta() {
  return this.intFiled;
};
ptyp_.__ctor = function SameNameInstanceAndStaticMethod____ctor() { };
Type__RegisterReferenceType(SameNameInstanceAndStaticMethod, "RealScript.SameNameInstanceAndStaticMethod", Object, []);
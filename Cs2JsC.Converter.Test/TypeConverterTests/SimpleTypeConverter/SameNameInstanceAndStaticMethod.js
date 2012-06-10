function RealScript_SameNameInstanceAndStaticMethod() {
};
RealScript_SameNameInstanceAndStaticMethod.typeId = "b";
function RealScript__SameNameInstanceAndStaticMethod__GetInt(c) {
  return c.intFiled;
};
function RealScript__SameNameInstanceAndStaticMethod_factory() {
  return new RealScript_SameNameInstanceAndStaticMethod();
};
RealScript_SameNameInstanceAndStaticMethod.defaultConstructor = RealScript__SameNameInstanceAndStaticMethod_factory;
ptyp_ = RealScript_SameNameInstanceAndStaticMethod.prototype;
ptyp_.intFiled = 0;
ptyp_.getInt = function RealScript__SameNameInstanceAndStaticMethod__GetInta() {
  return this.intFiled;
};
ptyp_.__ctor = function RealScript__SameNameInstanceAndStaticMethod____ctor() {
};
System__Type__RegisterReferenceType(RealScript_SameNameInstanceAndStaticMethod, "RealScript.SameNameInstanceAndStaticMethod", Object, []);

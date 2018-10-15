function EnumUsingClass() {
};
EnumUsingClass.typeId = "b";
EnumUsingClass__TestValue = 2;
function EnumUsingClass_factory() {
  var this_;
  this_ = new EnumUsingClass();
  this_.__ctor();
  return this_;
};
EnumUsingClass.defaultConstructor = EnumUsingClass_factory;
ptyp_ = EnumUsingClass.prototype;
ptyp_.testValue2 = 0;
ptyp_.getStr = function EnumUsingClass__GetStr(en) {
  return Enum__ToString(SimpleEnumType, en);
};
ptyp_.getStrVoid = function EnumUsingClass__GetStrVoid() {
  return this.getStr(this.testValue2);
};
ptyp_.__ctor = function EnumUsingClass____ctor() {
  this.testValue2 = 1;
};
Type__RegisterReferenceType(EnumUsingClass, "RealScript.EnumUsingClass", Object, []);

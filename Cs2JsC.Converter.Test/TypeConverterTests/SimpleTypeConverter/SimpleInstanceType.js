function RealScript_SimpleInstanceType() {
};
RealScript_SimpleInstanceType.typeId = "b";
function RealScript__SimpleInstanceType_factory() {
  return new RealScript_SimpleInstanceType();
};
RealScript_SimpleInstanceType.defaultConstructor = RealScript__SimpleInstanceType_factory;
ptyp_ = RealScript_SimpleInstanceType.prototype;
ptyp_.intField = 0;
ptyp_.getField = function RealScript__SimpleInstanceType__GetField() {
  return this.intField;
};
ptyp_.__ctor = function RealScript__SimpleInstanceType____ctor() {
};
System__Type__RegisterReferenceType(RealScript_SimpleInstanceType, "RealScript.SimpleInstanceType", Object, []);

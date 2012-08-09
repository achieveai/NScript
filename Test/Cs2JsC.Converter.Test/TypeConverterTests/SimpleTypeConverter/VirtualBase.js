function RealScript_VirtualBase() {
};
RealScript_VirtualBase.typeId = "b";
function RealScript__VirtualBase_factory() {
  return new RealScript_VirtualBase();
};
RealScript_VirtualBase.defaultConstructor = RealScript__VirtualBase_factory;
ptyp_ = RealScript_VirtualBase.prototype;
ptyp_.getInt = function RealScript__VirtualBase__GetInt(i) {
  return i * 2;
};
ptyp_.__ctor = function RealScript__VirtualBase____ctor() {
};
ptyp_.V_GetInt = ptyp_.getInt;
System__Type__RegisterReferenceType(RealScript_VirtualBase, "RealScript.VirtualBase", Object, []);

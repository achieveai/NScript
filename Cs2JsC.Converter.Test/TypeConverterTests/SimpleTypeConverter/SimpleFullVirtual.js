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
function RealScript_VirtualOverride() {
};
RealScript_VirtualOverride.typeId = "c";
function RealScript__VirtualOverride_factory() {
  var this_;
  this_ = new RealScript_VirtualOverride();
  this_.__ctora();
  return this_;
};
RealScript_VirtualOverride.defaultConstructor = RealScript__VirtualOverride_factory;
ptyp_ = new RealScript_VirtualBase();
RealScript_VirtualOverride.prototype = ptyp_;
ptyp_.getInta = function RealScript__VirtualOverride__GetInt(i) {
  return i / 2 | 0;
};
ptyp_.__ctora = function RealScript__VirtualOverride____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInta;
System__Type__RegisterReferenceType(RealScript_VirtualOverride, "RealScript.VirtualOverride", RealScript_VirtualBase, []);

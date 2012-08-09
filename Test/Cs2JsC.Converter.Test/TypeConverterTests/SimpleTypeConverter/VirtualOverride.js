function RealScript_VirtualOverride() {
};
RealScript_VirtualOverride.typeId = "b";
function RealScript__VirtualOverride_factory() {
  var this_;
  this_ = new RealScript_VirtualOverride();
  this_.__ctora();
  return this_;
};
RealScript_VirtualOverride.defaultConstructor = RealScript__VirtualOverride_factory;
ptyp_ = new RealScript_VirtualBase();
RealScript_VirtualOverride.prototype = ptyp_;
ptyp_.getInt = function RealScript__VirtualOverride__GetInt(i) {
  return i / 2 | 0;
};
ptyp_.__ctora = function RealScript__VirtualOverride____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInt;
System__Type__RegisterReferenceType(RealScript_VirtualOverride, "RealScript.VirtualOverride", RealScript_VirtualBase, []);

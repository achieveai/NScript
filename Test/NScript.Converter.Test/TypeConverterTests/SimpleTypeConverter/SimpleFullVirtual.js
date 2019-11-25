function VirtualBase() {
};
VirtualBase.typeId = "b";
function VirtualBase_factory() {
  var this_;
  this_ = new VirtualBase();
  this_.__ctor();
  return this_;
};
VirtualBase.defaultConstructor = VirtualBase_factory;
ptyp_ = VirtualBase.prototype;
ptyp_.getInt = function VirtualBase__GetInt(i) {
  return i * 2;
};
ptyp_.__ctor = function VirtualBase____ctor() {
};
ptyp_.V_GetInt = ptyp_.getInt;
Type__RegisterReferenceType(VirtualBase, "RealScript.VirtualBase", Object, []);
function VirtualOverride() {
};
VirtualOverride.typeId = "c";
function VirtualOverride_factory() {
  var this_;
  this_ = new VirtualOverride();
  this_.__ctora();
  return this_;
};
VirtualOverride.defaultConstructor = VirtualOverride_factory;
ptyp_ = new VirtualBase();
VirtualOverride.prototype = ptyp_;
ptyp_.getInta = function VirtualOverride__GetInt(i) {
  return i / 2 | 0;
};
ptyp_.__ctora = function VirtualOverride____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInta;
Type__RegisterReferenceType(VirtualOverride, "RealScript.VirtualOverride", VirtualBase, []);

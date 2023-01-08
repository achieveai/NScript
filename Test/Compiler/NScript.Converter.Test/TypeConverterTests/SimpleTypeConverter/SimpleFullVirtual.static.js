function VirtualBase() {
};
VirtualBase.typeId = "b";
function VirtualBase__GetInt(this_, i) {
  return i * 2;
};
function VirtualBase____ctor(this_) {
};
function VirtualBase_factory() {
  var this_;
  this_ = new VirtualBase();
  VirtualBase____ctor(this_);
  return this_;
};
VirtualBase.defaultConstructor = VirtualBase_factory;
ptyp_ = VirtualBase.prototype;
ptyp_.V_GetInt = function(i) {
  return VirtualBase__GetInt(this, i);
};
Type__RegisterReferenceType(VirtualBase, "RealScript.VirtualBase", Object, []);
function VirtualOverride() {
};
VirtualOverride.typeId = "c";
function VirtualOverride__GetInt(this_, i) {
  return i / 2 | 0;
};
function VirtualOverride____ctor(this_) {
  VirtualBase____ctor(this_);
};
function VirtualOverride_factory() {
  var this_;
  this_ = new VirtualOverride();
  VirtualOverride____ctor(this_);
  return this_;
};
VirtualOverride.defaultConstructor = VirtualOverride_factory;
ptyp_ = new VirtualBase();
VirtualOverride.prototype = ptyp_;
ptyp_.V_GetInt = function(i) {
  return VirtualOverride__GetInt(this, i);
};
Type__RegisterReferenceType(VirtualOverride, "RealScript.VirtualOverride", VirtualBase, []);

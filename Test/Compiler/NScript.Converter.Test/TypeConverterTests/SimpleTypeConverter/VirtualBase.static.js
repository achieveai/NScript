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

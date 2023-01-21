function VirtualOverride() { }
VirtualOverride.typeId = "b";
function VirtualOverride_factory() {
  var this_;
  this_ = new VirtualOverride();
  this_.__ctora();
  return this_;
}
VirtualOverride.defaultConstructor = VirtualOverride_factory;
ptyp_ = new VirtualBase();
VirtualOverride.prototype = ptyp_;
ptyp_.getInt = function VirtualOverride__GetInt(i) {
  return i / 2 | 0;
};
ptyp_.__ctora = function VirtualOverride____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInt;
Type__RegisterReferenceType(VirtualOverride, "RealScript.VirtualOverride", VirtualBase, []);
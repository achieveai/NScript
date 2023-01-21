function VirtualBase() { }
VirtualBase.typeId = "b";
function VirtualBase_factory() {
  var this_;
  this_ = new VirtualBase();
  this_.__ctor();
  return this_;
}
VirtualBase.defaultConstructor = VirtualBase_factory;
ptyp_ = VirtualBase.prototype;
ptyp_.getInt = function VirtualBase__GetInt(i) {
  return i * 2;
};
ptyp_.__ctor = function VirtualBase____ctor() { };
ptyp_.V_GetInt = ptyp_.getInt;
Type__RegisterReferenceType(VirtualBase, "RealScript.VirtualBase", Object, []);
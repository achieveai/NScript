function InheritInterface() {
};
InheritInterface.typeId = "b";
function InheritInterface_factory() {
  var this_;
  this_ = new InheritInterface();
  this_.__ctor();
  return this_;
};
InheritInterface.defaultConstructor = InheritInterface_factory;
ptyp_ = InheritInterface.prototype;
ptyp_.getInt = function InheritInterface__GetInt(i) {
  return i + 2;
};
ptyp_.__ctor = function InheritInterface____ctor() {
};
ptyp_.V_GetInt_c = function(arg0) {
  return this.V_GetInt(arg0);
};
ptyp_.V_GetInt = ptyp_.getInt;
Type__RegisterReferenceType(InheritInterface, "RealScript.InheritInterface", Object, [SimpleInterface]);

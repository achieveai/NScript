function InheritDerivedInterface() {
};
InheritDerivedInterface.typeId = "b";
function InheritDerivedInterface_factory() {
  var this_;
  this_ = new InheritDerivedInterface();
  this_.__ctor();
  return this_;
};
InheritDerivedInterface.defaultConstructor = InheritDerivedInterface_factory;
ptyp_ = InheritDerivedInterface.prototype;
ptyp_.getInt = function InheritDerivedInterface__GetInt(i) {
  return i + 2;
};
ptyp_.getStr = function InheritDerivedInterface__GetStr(i) {
  return Int32__ToString(i);
};
ptyp_.__ctor = function InheritDerivedInterface____ctor() {
};
ptyp_.V_GetStr_c = function(arg0) {
  return this.V_GetStr(arg0);
};
ptyp_.V_GetInt_d = ptyp_.getInt;
ptyp_.V_GetStr = ptyp_.getStr;
Type__RegisterReferenceType(InheritDerivedInterface, "RealScript.InheritDerivedInterface", Object, [SimpleInheritedInterface, SimpleInterface]);

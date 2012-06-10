function RealScript_InheritDerivedInterface() {
};
RealScript_InheritDerivedInterface.typeId = "b";
function RealScript__InheritDerivedInterface_factory() {
  return new RealScript_InheritDerivedInterface();
};
RealScript_InheritDerivedInterface.defaultConstructor = RealScript__InheritDerivedInterface_factory;
ptyp_ = RealScript_InheritDerivedInterface.prototype;
ptyp_.getInt = function RealScript__InheritDerivedInterface__GetInt(i) {
  return i + 2;
};
ptyp_.getStr = function RealScript__InheritDerivedInterface__GetStr(i) {
  return System__Int32__ToString(i);
};
ptyp_.__ctor = function RealScript__InheritDerivedInterface____ctor() {
};
ptyp_.V_GetStr_c = function(arg0) {
  return this.V_GetStr(arg0);
};
ptyp_.V_GetInt_d = ptyp_.getInt;
ptyp_.V_GetStr = ptyp_.getStr;
System__Type__RegisterReferenceType(RealScript_InheritDerivedInterface, "RealScript.InheritDerivedInterface", Object, [RealScript_SimpleInheritedInterface, RealScript_SimpleInterface]);

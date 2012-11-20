function RealScript_InheritInterface() {
};
RealScript_InheritInterface.typeId = "b";
function RealScript__InheritInterface_factory() {
  return new RealScript_InheritInterface();
};
RealScript_InheritInterface.defaultConstructor = RealScript__InheritInterface_factory;
ptyp_ = RealScript_InheritInterface.prototype;
ptyp_.getInt = function RealScript__InheritInterface__GetInt(i) {
  return i + 2;
};
ptyp_.__ctor = function RealScript__InheritInterface____ctor() {
};
ptyp_.V_GetInt_c = function(arg0) {
  return this.V_GetInt(arg0);
};
ptyp_.V_GetInt = ptyp_.getInt;
System__Type__RegisterReferenceType(RealScript_InheritInterface, "RealScript.InheritInterface", Object, [RealScript_SimpleInterface]);


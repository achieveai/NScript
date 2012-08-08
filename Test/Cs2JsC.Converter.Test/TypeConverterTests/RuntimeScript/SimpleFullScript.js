function RealScript_SimpleInterface() {
};
RealScript_SimpleInterface.typeId = "b";
System__Type__RegisterInterface(RealScript_SimpleInterface, "RealScript.SimpleInterface");
function RealScript_InheritInterface() {
};
RealScript_InheritInterface.typeId = "c";
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
ptyp_.V_GetInt_b = function(arg0) {
  return this.V_GetInt(arg0);
};
ptyp_.V_GetInt = ptyp_.getInt;
System__Type__RegisterReferenceType(RealScript_InheritInterface, "RealScript.InheritInterface", Object, [RealScript_SimpleInterface]);
function RealScript_SecondOrderInterfaceInherit() {
};
RealScript_SecondOrderInterfaceInherit.typeId = "d";
function RealScript__SecondOrderInterfaceInherit_factory() {
  var this_;
  this_ = new RealScript_SecondOrderInterfaceInherit();
  this_.__ctora();
  return this_;
};
RealScript_SecondOrderInterfaceInherit.defaultConstructor = RealScript__SecondOrderInterfaceInherit_factory;
ptyp_ = new RealScript_InheritInterface();
RealScript_SecondOrderInterfaceInherit.prototype = ptyp_;
ptyp_.getInta = function RealScript__SecondOrderInterfaceInherit__GetInt(i) {
  return i + 3;
};
ptyp_.__ctora = function RealScript__SecondOrderInterfaceInherit____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInta;
System__Type__RegisterReferenceType(RealScript_SecondOrderInterfaceInherit, "RealScript.SecondOrderInterfaceInherit", RealScript_InheritInterface, []);

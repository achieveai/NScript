function RealScript_SecondOrderInterfaceInherit() {
};
RealScript_SecondOrderInterfaceInherit.typeId = "b";
function RealScript__SecondOrderInterfaceInherit_factory() {
  var this_;
  this_ = new RealScript_SecondOrderInterfaceInherit();
  this_.__ctora();
  return this_;
};
RealScript_SecondOrderInterfaceInherit.defaultConstructor = RealScript__SecondOrderInterfaceInherit_factory;
ptyp_ = new RealScript_InheritInterface();
RealScript_SecondOrderInterfaceInherit.prototype = ptyp_;
ptyp_.getInt = function RealScript__SecondOrderInterfaceInherit__GetInt(i) {
  return i + 3;
};
ptyp_.__ctora = function RealScript__SecondOrderInterfaceInherit____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInt;
System__Type__RegisterReferenceType(RealScript_SecondOrderInterfaceInherit, "RealScript.SecondOrderInterfaceInherit", RealScript_InheritInterface, []);

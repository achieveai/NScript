function SimpleInterface() {
};
SimpleInterface.typeId = "b";
Type__RegisterInterface(SimpleInterface, "RealScript.SimpleInterface");
function InheritInterface() {
};
InheritInterface.typeId = "c";
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
ptyp_.V_GetInt_b = function(arg0) {
  return this.V_GetInt(arg0);
};
ptyp_.V_GetInt = ptyp_.getInt;
Type__RegisterReferenceType(InheritInterface, "RealScript.InheritInterface", Object, [SimpleInterface]);
function SecondOrderInterfaceInherit() {
};
SecondOrderInterfaceInherit.typeId = "d";
function SecondOrderInterfaceInherit_factory() {
  var this_;
  this_ = new SecondOrderInterfaceInherit();
  this_.__ctora();
  return this_;
};
SecondOrderInterfaceInherit.defaultConstructor = SecondOrderInterfaceInherit_factory;
ptyp_ = new InheritInterface();
SecondOrderInterfaceInherit.prototype = ptyp_;
ptyp_.getInta = function SecondOrderInterfaceInherit__GetInt(i) {
  return i + 3;
};
ptyp_.__ctora = function SecondOrderInterfaceInherit____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInta;
Type__RegisterReferenceType(SecondOrderInterfaceInherit, "RealScript.SecondOrderInterfaceInherit", InheritInterface, []);

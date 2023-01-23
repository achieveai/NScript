function SecondOrderInterfaceInherit() { }
SecondOrderInterfaceInherit.typeId = "b";
function SecondOrderInterfaceInherit_factory() {
  var this_;
  this_ = new SecondOrderInterfaceInherit();
  this_.__ctora();
  return this_;
}
SecondOrderInterfaceInherit.defaultConstructor = SecondOrderInterfaceInherit_factory;
ptyp_ = new InheritInterface();
SecondOrderInterfaceInherit.prototype = ptyp_;
ptyp_.getInt = function SecondOrderInterfaceInherit__GetInt(i) {
  return i + 3;
};
ptyp_.__ctora = function SecondOrderInterfaceInherit____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInt;
Type__RegisterReferenceType(SecondOrderInterfaceInherit, "RealScript.SecondOrderInterfaceInherit", InheritInterface, []);
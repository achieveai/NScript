function SecondOrderInterfaceInherit() {
};
SecondOrderInterfaceInherit.typeId = "b";
function SecondOrderInterfaceInherit__GetInt(this_, i) {
  return i + 3;
};
function SecondOrderInterfaceInherit____ctor(this_) {
  InheritInterface____ctor(this_);
};
function SecondOrderInterfaceInherit_factory() {
  var this_;
  this_ = new SecondOrderInterfaceInherit();
  SecondOrderInterfaceInherit____ctor(this_);
  return this_;
};
SecondOrderInterfaceInherit.defaultConstructor = SecondOrderInterfaceInherit_factory;
ptyp_ = new InheritInterface();
SecondOrderInterfaceInherit.prototype = ptyp_;
ptyp_.V_GetInt = function(i) {
  return SecondOrderInterfaceInherit__GetInt(this, i);
};
Type__RegisterReferenceType(SecondOrderInterfaceInherit, "RealScript.SecondOrderInterfaceInherit", InheritInterface, []);



function RealScript_MultipleConstructorsType() {
};
RealScript_MultipleConstructorsType.typeId = "b";
function RealScript__MultipleConstructorsType_factory(dbl) {
  var this_;
  this_ = new RealScript_MultipleConstructorsType();
  this_.__ctor(dbl);
  return this_;
};
function RealScript__MultipleConstructorsType_factorya(i) {
  var this_;
  this_ = new RealScript_MultipleConstructorsType();
  this_.__ctora(i);
  return this_;
};
ptyp_ = RealScript_MultipleConstructorsType.prototype;
ptyp_.intField = 0;
ptyp_.doubleField = 0;
ptyp_.__ctor = function RealScript__MultipleConstructorsType____ctor(dbl) {
  this.doubleField = dbl;
};
ptyp_.__ctora = function RealScript__MultipleConstructorsType____ctora(i) {
  this.intField = i;
};
System__Type__RegisterReferenceType(RealScript_MultipleConstructorsType, "RealScript.MultipleConstructorsType", Object, []);

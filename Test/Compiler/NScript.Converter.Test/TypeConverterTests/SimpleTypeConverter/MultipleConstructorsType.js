function MultipleConstructorsType() { }
MultipleConstructorsType.typeId = "b";
function MultipleConstructorsType_factory(dbl) {
  var this_;
  this_ = new MultipleConstructorsType();
  this_.__ctor(dbl);
  return this_;
}
function MultipleConstructorsType_factorya(i) {
  var this_;
  this_ = new MultipleConstructorsType();
  this_.__ctora(i);
  return this_;
}
ptyp_ = MultipleConstructorsType.prototype;
ptyp_.intField = 0;
ptyp_.doubleField = 0;
ptyp_.__ctor = function MultipleConstructorsType____ctor(dbl) {
  this.doubleField = dbl;
};
ptyp_.__ctora = function MultipleConstructorsType____ctora(i) {
  this.intField = i;
};
Type__RegisterReferenceType(MultipleConstructorsType, "RealScript.MultipleConstructorsType", Object, []);
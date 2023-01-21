function SimpleInstanceType() { }
SimpleInstanceType.typeId = "b";
function SimpleInstanceType_factory() {
  var this_;
  this_ = new SimpleInstanceType();
  this_.__ctor();
  return this_;
}
SimpleInstanceType.defaultConstructor = SimpleInstanceType_factory;
ptyp_ = SimpleInstanceType.prototype;
ptyp_.intField = 0;
ptyp_.getField = function SimpleInstanceType__GetField() {
  return this.intField;
};
ptyp_.__ctor = function SimpleInstanceType____ctor() { };
Type__RegisterReferenceType(SimpleInstanceType, "RealScript.SimpleInstanceType", Object, []);
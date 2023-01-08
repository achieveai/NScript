function SimpleInstanceType() {
};
SimpleInstanceType.typeId = "b";
function SimpleInstanceType__GetField(this_) {
  return this_.intField;
};
function SimpleInstanceType____ctor(this_) {
};
function SimpleInstanceType_factory() {
  var this_;
  this_ = new SimpleInstanceType();
  SimpleInstanceType____ctor(this_);
  return this_;
};
SimpleInstanceType.defaultConstructor = SimpleInstanceType_factory;
ptyp_ = SimpleInstanceType.prototype;
ptyp_.intField = 0;
Type__RegisterReferenceType(SimpleInstanceType, "RealScript.SimpleInstanceType", Object, []);

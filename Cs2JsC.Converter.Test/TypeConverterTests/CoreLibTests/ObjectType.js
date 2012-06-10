Object.typeId = "b";
function System__Object__GetType(this_) {
  return this_.constructor;
};
function System__Object__Equals(obj1, obj2) {
  return obj1 === obj2;
};
function System__Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
function System__Object_factory() {
  var this_;
  this_ = Object.getDefaultValue();
};
System__Object__GetType = function System__Object__GetType(this_) {
  return this_.constructor;
};
System__Type__RegisterReferenceType(Object, "System.Object", null, []);
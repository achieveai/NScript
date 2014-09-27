Object.typeId = "b";
function System__Object__GetType(this_) {
  return this_.constructor;
};
function System__Object__Equals(obj1, obj2) {
  return obj1 === obj2;
};
function System__Object__Equalsa(this_, obj2) {
  return this_ === obj2;
};
function System__Object__GetHashCode(this_) {
  return 0;
};
function System__Object__ReferenceEquals(obj1, obj2) {
  return this === obj2;
};
function System__Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
function System__Object__GetNewImportedExtension() {
  return {
    "toJSON": System__Object__NoReturn
  };
};
function System__Object__NoReturn() {
  return undefined;
};
Object.defaultConstructor = function System_Object_factory() {
  return new Object();
};
System__Object__GetType = function System__Object__GetType(this_) {
  return this_.constructor;
};
ptyp_ = Object.prototype;
ptyp_.V_Equals = ptyp_.equals;
ptyp_.V_GetHashCode = ptyp_.getHashCode;
System__Type__RegisterReferenceType(Object, "System.Object", null, []);


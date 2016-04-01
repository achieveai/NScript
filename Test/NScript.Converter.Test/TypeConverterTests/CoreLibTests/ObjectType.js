Object.typeId = "b";
function Object__GetType(this_) {
  return this_.constructor;
};
function Object__Equals(obj1, obj2) {
  return obj1 === obj2;
};
function Object__Equalsa(this_, obj2) {
  return this_ === obj2;
};
function Object__GetHashCode(this_) {
  return 0;
};
function Object__ReferenceEquals(obj1, obj2) {
  return this === obj2;
};
function Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
function Object__GetNewImportedExtension() {
  return {
    "toJSON": Object__NoReturn
  };
};
function Object__NoReturn() {
  return undefined;
};
Object.defaultConstructor = function Object_factory() {
  return new Object();
};
Object__GetType = function Object__GetType(this_) {
  return this_.constructor;
};
ptyp_ = Object.prototype;
ptyp_.V_Equals = ptyp_.equals;
ptyp_.V_GetHashCode = ptyp_.getHashCode;
Type__RegisterReferenceType(Object, "System.Object", null, []);


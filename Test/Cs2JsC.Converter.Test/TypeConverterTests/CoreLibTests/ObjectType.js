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
System__Type__RegisterReferenceType(Object, "System.Object", null, []);
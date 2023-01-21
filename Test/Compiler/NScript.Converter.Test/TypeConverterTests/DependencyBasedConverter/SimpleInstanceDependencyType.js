Function.typeId = "b";
Type__typeMapping = null;
function Type__RegisterReferenceType(this_, typeName, parentType, interfaces) {
  this_.isClass = true;
  this_.fullName = typeName;
  this_.baseType = parentType;
  this_.interfaces = parentType ? interfaces.concat(parentType.interfaces) : interfaces;
  if (!Type__typeMapping)
    Type__typeMapping = { };
  Type__typeMapping[this_.fullName] = this_;
}
ptyp_ = Function.prototype;
ptyp_.isClass = false;
ptyp_.baseType = null;
ptyp_.fullName = null;
ptyp_.typeId = null;
ptyp_.interfaces = null;
ptyp_.getDefaultValue = function Type__GetDefaultValue() {
  return null;
};
Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "c";
Type__RegisterReferenceType(Object, "System.Object", null, []);
function SimpleInstanceType() { }
SimpleInstanceType.typeId = "d";
ptyp_ = SimpleInstanceType.prototype;
ptyp_.intField = 0;
ptyp_.getField = function SimpleInstanceType__GetField() {
  return this.intField;
};
Type__RegisterReferenceType(SimpleInstanceType, "RealScript.SimpleInstanceType", Object, []);

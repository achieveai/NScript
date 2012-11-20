Function.typeId = "b";
System__Type__typeMapping = null;
function System__Type__RegisterReferenceType(this_, typeName, parentType, interfaces) {
  this_.isClass = true;
  this_.fullName = typeName;
  this_.baseType = parentType;
  this_.interfaces = parentType ? interfaces.concat(parentType.interfaces) : interfaces;
  if (!System__Type__typeMapping)
    System__Type__typeMapping = {
    };
  System__Type__typeMapping[this_.fullName] = this_;
};
ptyp_ = Function.prototype;
ptyp_.isClass = false;
ptyp_.baseType = null;
ptyp_.fullName = null;
ptyp_.interfaces = null;
ptyp_.getDefaultValue = function System__Type__GetDefaultValue() {
  return null;
};
System__Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "c";
System__Type__RegisterReferenceType(Object, "System.Object", null, []);
function RealScript_SimpleInstanceType() {
};
RealScript_SimpleInstanceType.typeId = "d";
ptyp_ = RealScript_SimpleInstanceType.prototype;
ptyp_.intField = 0;
ptyp_.getField = function RealScript__SimpleInstanceType__GetField() {
  return this.intField;
};
System__Type__RegisterReferenceType(RealScript_SimpleInstanceType, "RealScript.SimpleInstanceType", Object, []);
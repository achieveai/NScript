Function.typeId = "b";
System__Type__typeMapping = null;
function System__Type__GetType(typeName) {
  return System__Type__CastType(Function, System__Type__typeMapping[typeName]);
};
function System__Type__GetInterfaces(this_) {
  return this_.interfaces;
};
function System__Type__AsType(this_, instance) {
  return this_.isInstanceOfType(instance) ? instance : null;
};
function System__Type__CastType(this_, instance) {
  if (this_.isInstanceOfType(instance)) {
    if (this_.isStruct)
      return instance.boxedValue;
    return instance;
  }
  throw "InvalidCast to " + this_.fullName;
};
function System__Type__ToLocaleString(this_) {
  return this_.fullName;
};
function System__Type__ToString(this_) {
  return this_.fullName;
};
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
function System__Type__RegisterStructType(this_, typeName, interfaces) {
  this_.isStruct = true;
  this_.fullName = typeName;
  this_.baseType = System_ValueType;
  this_.interfaces = interfaces;
  if (!System__Type__typeMapping)
    System__Type__typeMapping = {
    };
  System__Type__typeMapping[this_.fullName] = this_;
};
function System__Type__RegisterInterface(this_, typeName) {
  this_.isInterface = true;
  this_.fullName = typeName;
};
function System__Type__RegisterEnum(this_, typeName, isFlag) {
  var enumStrToValueMap, valueToStr, lowerStrToValue, key;
  this_.isEnum = true;
  this_.fullName = typeName;
  this_.isFlagEnum = isFlag;
  this_.baseType = System_Enum;
  enumStrToValueMap = this_.enumStrToValueMap;
  valueToStr = {
  };
  lowerStrToValue = {
  };
  for (key in enumStrToValueMap) {
    valueToStr[enumStrToValueMap[key]] = key;
    lowerStrToValue[key.toLower()] = enumStrToValueMap[key];
  }
  this_.enumValueToStrMap = valueToStr;
  this_.enumLowerStrToValueMap = lowerStrToValue;
  if (!System__Type__typeMapping)
    System__Type__typeMapping = {
    };
  System__Type__typeMapping[this_.fullName] = this_;
};
function System__Type__InitializeBaseInterfaces(type) {
  var rv, baseType, baseInterfaces, key, interfaces;
  if (!type.baseInterfaces) {
    rv = {
    };
    baseType = type.baseType;
    if (baseType != null && baseType != Object) {
      if (baseType)
        System__Type__InitializeBaseInterfaces(type);
      baseInterfaces = baseType.baseInterfaces;
      if (baseInterfaces)
        for (key in baseInterfaces)
          rv[key] = baseInterfaces[key];
    }
    interfaces = type.interfaces;
    if (interfaces)
      for (key = 0; key < interfaces.length; key++)
        rv[interfaces[key].fullName] = interfaces[key];
    type.baseInterfaces = rv;
  }
};
function System__Type_factory() {
  var this_;
  this_ = Function.getDefaultValue();
};
ptyp_ = Function.prototype;
ptyp_.isDelegate = false;
ptyp_.isClass = false;
ptyp_.isEnum = false;
ptyp_.isStruct = false;
ptyp_.isInterface = false;
ptyp_.baseType = null;
ptyp_.fullName = null;
ptyp_.typeId = null;
ptyp_.baseInterfaces = null;
ptyp_.boxedValue = null;
ptyp_.enumValueToStrMap = null;
ptyp_.enumStrToValueMap = null;
ptyp_.enumLowerStrToValueMap = null;
ptyp_.isFlagEnum = false;
ptyp_.interfaces = null;
ptyp_.isInstanceOfType = function System__Type__IsInstanceOfType(instance) {
  if (!this.isInterface)
    return instance instanceof this;
  else if (instance && !instance.constructor.baseInterfaces)
    System__Type__InitializeBaseInterfaces(instance.constructor);
  return instance && instance.constructor.baseInterfaces && instance.constructor.baseInterfaces[this.fullName];
};
ptyp_.box = function System__Type__Box(instance) {
  if (this.isStruct)
    return new this(instance);
  else
    return instance;
};
ptyp_.unbox = function System__Type__Unbox(instance) {
  if (this.isStruct)
    return instance.boxedValue;
  else
    return instance;
};
ptyp_.defaultConstructor = function System__Type__DefaultConstructor() {
  if (this.isStruct)
    return this.getDefaultValue();
  throw "Default constructor not implemented";
};
ptyp_.getDefaultValue = function System__Type__GetDefaultValue() {
  return null;
};
ptyp_.toLocaleString = function() {
  return System__Type__ToLocaleString(this);
};
ptyp_.toString = function() {
  return System__Type__ToString(this);
};
System__Type__RegisterReferenceType(Function, "System.Type", Object, []);
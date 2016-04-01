Function.typeId = "b";
Type__typeMapping = null;
function Type__GetType(typeName) {
  return Type__CastType(Function, Type__typeMapping[typeName]);
};
function Type__GetInterfaces(this_) {
  return this_.interfaces;
};
function Type__AsType(this_, instance) {
  return this_.isInstanceOfType(instance) ? instance : null;
};
function Type__CastType(this_, instance) {
  if (this_.isInstanceOfType(instance) || instance === null || typeof instance === "undefined") {
    if (this_.isStruct)
      return instance.boxedValue;
    return instance;
  }
  throw "InvalidCast to " + this_.fullName;
};
function Type__ToLocaleString(this_) {
  return this_.fullName ? this_.fullName : this_.name;
};
function Type__ToString(this_) {
  return this_.fullName ? this_.fullName : this_.name;
};
function Type__RegisterReferenceType(this_, typeName, parentType, interfaces) {
  this_.isClass = true;
  this_.fullName = typeName;
  this_.baseType = parentType;
  this_.interfaces = parentType ? interfaces.concat(parentType.interfaces) : interfaces;
  if (!Type__typeMapping)
    Type__typeMapping = {
    };
  Type__typeMapping[this_.fullName] = this_;
};
function Type__RegisterStructType(this_, typeName, interfaces) {
  this_.isStruct = true;
  this_.fullName = typeName;
  this_.baseType = ValueType;
  this_.interfaces = interfaces;
  if (!Type__typeMapping)
    Type__typeMapping = {
    };
  Type__typeMapping[this_.fullName] = this_;
};
function Type__RegisterInterface(this_, typeName) {
  this_.isInterface = true;
  this_.fullName = typeName;
};
function Type__RegisterEnum(this_, typeName, isFlag) {
  var enumStrToValueMap, valueToStr, lowerStrToValue, key;
  this_.isEnum = true;
  this_.fullName = typeName;
  this_.isFlagEnum = isFlag;
  this_.baseType = Enum;
  enumStrToValueMap = this_.enumStrToValueMap;
  valueToStr = {
  };
  lowerStrToValue = {
  };
  for (key in enumStrToValueMap) {
    valueToStr[enumStrToValueMap[key]] = key;
    lowerStrToValue[key.toLowerCase()] = enumStrToValueMap[key];
  }
  this_.enumValueToStrMap = valueToStr;
  this_.enumLowerStrToValueMap = lowerStrToValue;
  if (!Type__typeMapping)
    Type__typeMapping = {
    };
  Type__typeMapping[this_.fullName] = this_;
};
function Type__BoxTypeInstance(type, instance) {
  if (type.isNullable)
    return type.nullableBox(instance);
  else if (type.isStruct)
    return new type(instance);
  else
    return instance;
};
function Type__UnBoxTypeInstance(type, instance) {
  if (type.isNullable && instance === null)
    return null;
  else if (type.isStruct)
    return instance.boxedValue;
  else
    return instance;
};
function Type__GetDefaultConstructor(type) {
  if (type.isStruct)
    return type.getDefaultValue;
  else if (type.defaultConstructor)
    return type.defaultConstructor;
  else
    return function() {
        return null;
    };
};
function Type__GetDefaultValueStatic(type) {
  if (type.isStruct)
    return type.getDefaultValue();
  return null;
};
function Type__InitializeBaseInterfaces(type) {
  var rv, baseType, baseInterfaces, key, interfaces;
  if (!type.baseInterfaces) {
    rv = {
    };
    baseType = type.baseType;
    if (baseType != null && baseType != Object) {
      if (baseType)
        Type__InitializeBaseInterfaces(type);
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
function Type_factory() {
  var this_;
  this_ = Function.getDefaultValue();
};
function Type_factory() {
  return new Function();
};
Function.defaultConstructor = Type_factory;
ptyp_ = Function.prototype;
ptyp_.isDelegate = false;
ptyp_.isClass = false;
ptyp_.isEnum = false;
ptyp_.isStruct = false;
ptyp_.isInterface = false;
ptyp_.isNullable = false;
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
ptyp_.isInstanceOfType = function Type__IsInstanceOfType(instance) {
  if (instance === null || typeof instance === "undefined")
    return false;
  if (!this.isInterface)
    return instance instanceof this || instance && instance.constructor == this;
  else if (!instance.constructor.baseInterfaces)
    Type__InitializeBaseInterfaces(instance.constructor);
  return instance.constructor.baseInterfaces && instance.constructor.baseInterfaces[this.fullName];
};
ptyp_.defaultConstructor = function Type__DefaultConstructor() {
  if (this.isStruct)
    return this.getDefaultValue;
  throw "Default constructor not implemented";
};
ptyp_.getDefaultValue = function Type__GetDefaultValue() {
  return null;
};
ptyp_.nullableBox = function Type__NullableBox(instance) {
  return null;
};
ptyp_.toLocaleString = function() {
  return Type__ToLocaleString(this);
};
ptyp_.toString = function() {
  return Type__ToString(this);
};
Type__RegisterReferenceType(Function, "System.Type", Object, []);
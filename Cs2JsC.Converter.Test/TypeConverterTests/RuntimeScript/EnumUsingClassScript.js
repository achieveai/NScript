function RealScript_SimpleEnumType(boxedValue) {
  this.boxedValue = boxedValue;
};
RealScript_SimpleEnumType.typeId = "b";
RealScript__SimpleEnumType__One = 1;
RealScript__SimpleEnumType__Two = 2;
RealScript__SimpleEnumType__Three = 3;
RealScript_SimpleEnumType.enumStrToValueMap = {
  "One": 1,
  "Two": 2,
  "Three": 3
};
RealScript_SimpleEnumType.getDefaultValue = function() {
  return 0;
};
RealScript_SimpleEnumType.prototype = new System_Enum();
System__Type__RegisterEnum(RealScript_SimpleEnumType, "RealScript.SimpleEnumType", false);
function RealScript_EnumUsingClass() {
};
RealScript_EnumUsingClass.typeId = "c";
RealScript__EnumUsingClass__TestValue = 2;
function RealScript__EnumUsingClass_factory() {
  var this_;
  this_ = new RealScript_EnumUsingClass();
  this_.__ctor();
  return this_;
};
RealScript_EnumUsingClass.defaultConstructor = RealScript__EnumUsingClass_factory;
ptyp_ = RealScript_EnumUsingClass.prototype;
ptyp_.testValue2 = 0;
ptyp_.getStr = function RealScript__EnumUsingClass__GetStr(en) {
  return System__Enum__ToString(RealScript_SimpleEnumType, en);
};
ptyp_.getStrVoid = function RealScript__EnumUsingClass__GetStrVoid() {
  return this.getStr(this.testValue2);
};
ptyp_.__ctor = function RealScript__EnumUsingClass____ctor() {
  this.testValue2 = 1;
};
System__Type__RegisterReferenceType(RealScript_EnumUsingClass, "RealScript.EnumUsingClass", Object, []);

function SimpleEnumType(boxedValue) {
  this.boxedValue = boxedValue;
};
SimpleEnumType.typeId = "b";
SimpleEnumType__One = 1;
SimpleEnumType__Two = 2;
SimpleEnumType__Three = 3;
SimpleEnumType.enumStrToValueMap = {
  "One": 1,
  "Two": 2,
  "Three": 3
};
SimpleEnumType.getDefaultValue = function() {
  return 0;
};
SimpleEnumType.prototype = new Enum();
Type__RegisterEnum(SimpleEnumType, "RealScript.SimpleEnumType", false);
function EnumUsingClass() {
};
EnumUsingClass.typeId = "c";
EnumUsingClass__TestValue = 2;
EnumUsingClass__Str = null;
function EnumUsingClass____cctor() {
  EnumUsingClass__Str = Int32__ToString(SimpleStaticType__GetField());
};
function EnumUsingClass_factory() {
  var this_;
  this_ = new EnumUsingClass();
  this_.__ctor();
  return this_;
};
EnumUsingClass.defaultConstructor = EnumUsingClass_factory;
ptyp_ = EnumUsingClass.prototype;
ptyp_.testValue2 = 0;
ptyp_.getStr = function EnumUsingClass__GetStr(en) {
  return Enum__ToString(SimpleEnumType, en);
};
ptyp_.getStrVoid = function EnumUsingClass__GetStrVoid() {
  return this.getStr(this.testValue2);
};
ptyp_.__ctor = function EnumUsingClass____ctor() {
  this.testValue2 = 1;
};
Type__RegisterReferenceType(EnumUsingClass, "RealScript.EnumUsingClass", Object, []);
EnumUsingClass____cctor();

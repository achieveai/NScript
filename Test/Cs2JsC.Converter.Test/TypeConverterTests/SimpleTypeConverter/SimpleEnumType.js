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


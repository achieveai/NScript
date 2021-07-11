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

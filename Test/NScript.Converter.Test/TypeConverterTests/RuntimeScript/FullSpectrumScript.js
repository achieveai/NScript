function RealScript_SimpleInterface() {
};
RealScript_SimpleInterface.typeId = "b";
System__Type__RegisterInterface(RealScript_SimpleInterface, "RealScript.SimpleInterface");
function RealScript_SimpleInheritedInterface() {
};
RealScript_SimpleInheritedInterface.typeId = "c";
System__Type__RegisterInterface(RealScript_SimpleInheritedInterface, "RealScript.SimpleInheritedInterface");
function RealScript_InheritDerivedInterface() {
};
RealScript_InheritDerivedInterface.typeId = "d";
function RealScript__InheritDerivedInterface_factory() {
  return new RealScript_InheritDerivedInterface();
};
RealScript_InheritDerivedInterface.defaultConstructor = RealScript__InheritDerivedInterface_factory;
ptyp_ = RealScript_InheritDerivedInterface.prototype;
ptyp_.getInt = function RealScript__InheritDerivedInterface__GetInt(i) {
  return i + 2;
};
ptyp_.getStr = function RealScript__InheritDerivedInterface__GetStr(i) {
  return System__Int32__ToString(i);
};
ptyp_.__ctor = function RealScript__InheritDerivedInterface____ctor() {
};
ptyp_.V_GetStr_c = function(arg0) {
  return this.V_GetStr(arg0);
};
ptyp_.V_GetInt_b = ptyp_.getInt;
ptyp_.V_GetStr = ptyp_.getStr;
System__Type__RegisterReferenceType(RealScript_InheritDerivedInterface, "RealScript.InheritDerivedInterface", Object, [RealScript_SimpleInheritedInterface, RealScript_SimpleInterface]);
function RealScript_InheritInterface() {
};
RealScript_InheritInterface.typeId = "e";
function RealScript__InheritInterface_factory() {
  return new RealScript_InheritInterface();
};
RealScript_InheritInterface.defaultConstructor = RealScript__InheritInterface_factory;
ptyp_ = RealScript_InheritInterface.prototype;
ptyp_.getInt = function RealScript__InheritInterface__GetInt(i) {
  return i + 2;
};
ptyp_.__ctor = function RealScript__InheritInterface____ctor() {
};
ptyp_.V_GetInt_b = function(arg0) {
  return this.V_GetInt(arg0);
};
ptyp_.V_GetInt = ptyp_.getInt;
System__Type__RegisterReferenceType(RealScript_InheritInterface, "RealScript.InheritInterface", Object, [RealScript_SimpleInterface]);
function RealScript_SecondOrderInterfaceInherit() {
};
RealScript_SecondOrderInterfaceInherit.typeId = "f";
function RealScript__SecondOrderInterfaceInherit_factory() {
  var this_;
  this_ = new RealScript_SecondOrderInterfaceInherit();
  this_.__ctora();
  return this_;
};
RealScript_SecondOrderInterfaceInherit.defaultConstructor = RealScript__SecondOrderInterfaceInherit_factory;
ptyp_ = new RealScript_InheritInterface();
RealScript_SecondOrderInterfaceInherit.prototype = ptyp_;
ptyp_.getInta = function RealScript__SecondOrderInterfaceInherit__GetInt(i) {
  return i + 3;
};
ptyp_.__ctora = function RealScript__SecondOrderInterfaceInherit____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInta;
System__Type__RegisterReferenceType(RealScript_SecondOrderInterfaceInherit, "RealScript.SecondOrderInterfaceInherit", RealScript_InheritInterface, []);
function RealScript_MultipleConstructorsType() {
};
RealScript_MultipleConstructorsType.typeId = "g";
function RealScript__MultipleConstructorsType_factory(dbl) {
  var this_;
  this_ = new RealScript_MultipleConstructorsType();
  this_.__ctor(dbl);
  return this_;
};
function RealScript__MultipleConstructorsType_factorya(i) {
  var this_;
  this_ = new RealScript_MultipleConstructorsType();
  this_.__ctora(i);
  return this_;
};
ptyp_ = RealScript_MultipleConstructorsType.prototype;
ptyp_.intField = 0;
ptyp_.doubleField = 0;
ptyp_.__ctor = function RealScript__MultipleConstructorsType____ctor(dbl) {
  this.doubleField = dbl;
};
ptyp_.__ctora = function RealScript__MultipleConstructorsType____ctora(i) {
  this.intField = i;
};
System__Type__RegisterReferenceType(RealScript_MultipleConstructorsType, "RealScript.MultipleConstructorsType", Object, []);
function RealScript_SameNameInstanceAndStaticMethod() {
};
RealScript_SameNameInstanceAndStaticMethod.typeId = "h";
function RealScript__SameNameInstanceAndStaticMethod__GetInt(c) {
  return c.intFiled;
};
function RealScript__SameNameInstanceAndStaticMethod_factory() {
  return new RealScript_SameNameInstanceAndStaticMethod();
};
RealScript_SameNameInstanceAndStaticMethod.defaultConstructor = RealScript__SameNameInstanceAndStaticMethod_factory;
ptyp_ = RealScript_SameNameInstanceAndStaticMethod.prototype;
ptyp_.intFiled = 0;
ptyp_.getInt = function RealScript__SameNameInstanceAndStaticMethod__GetInta() {
  return this.intFiled;
};
ptyp_.__ctor = function RealScript__SameNameInstanceAndStaticMethod____ctor() {
};
System__Type__RegisterReferenceType(RealScript_SameNameInstanceAndStaticMethod, "RealScript.SameNameInstanceAndStaticMethod", Object, []);
function RealScript_VirtualBase() {
};
RealScript_VirtualBase.typeId = "i";
function RealScript__VirtualBase_factory() {
  return new RealScript_VirtualBase();
};
RealScript_VirtualBase.defaultConstructor = RealScript__VirtualBase_factory;
ptyp_ = RealScript_VirtualBase.prototype;
ptyp_.getInt = function RealScript__VirtualBase__GetInt(i) {
  return i * 2;
};
ptyp_.__ctor = function RealScript__VirtualBase____ctor() {
};
ptyp_.V_GetInt = ptyp_.getInt;
System__Type__RegisterReferenceType(RealScript_VirtualBase, "RealScript.VirtualBase", Object, []);
function RealScript_VirtualOverride() {
};
RealScript_VirtualOverride.typeId = "j";
function RealScript__VirtualOverride_factory() {
  var this_;
  this_ = new RealScript_VirtualOverride();
  this_.__ctora();
  return this_;
};
RealScript_VirtualOverride.defaultConstructor = RealScript__VirtualOverride_factory;
ptyp_ = new RealScript_VirtualBase();
RealScript_VirtualOverride.prototype = ptyp_;
ptyp_.getInta = function RealScript__VirtualOverride__GetInt(i) {
  return i / 2 | 0;
};
ptyp_.__ctora = function RealScript__VirtualOverride____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInta;
System__Type__RegisterReferenceType(RealScript_VirtualOverride, "RealScript.VirtualOverride", RealScript_VirtualBase, []);
function RealScript_SimpleInstanceType() {
};
RealScript_SimpleInstanceType.typeId = "k";
function RealScript__SimpleInstanceType_factory() {
  return new RealScript_SimpleInstanceType();
};
RealScript_SimpleInstanceType.defaultConstructor = RealScript__SimpleInstanceType_factory;
ptyp_ = RealScript_SimpleInstanceType.prototype;
ptyp_.intField = 0;
ptyp_.getField = function RealScript__SimpleInstanceType__GetField() {
  return this.intField;
};
ptyp_.__ctor = function RealScript__SimpleInstanceType____ctor() {
};
System__Type__RegisterReferenceType(RealScript_SimpleInstanceType, "RealScript.SimpleInstanceType", Object, []);
function RealScript_SimpleStaticType() {
};
RealScript__SimpleStaticType__intField = 0;
function RealScript__SimpleStaticType__GetField() {
  return RealScript__SimpleStaticType__intField;
};
function RealScript_StaticConstructorType() {
};
RealScript__StaticConstructorType__tempValue = 0;
function RealScript__StaticConstructorType____cctor() {
  RealScript__StaticConstructorType__tempValue = RealScript__Class1__GetMoreStatic(10);
};
function RealScript__StaticConstructorType__get_Value() {
  return RealScript__StaticConstructorType__tempValue;
};
function RealScript_SimpleEnumType(boxedValue) {
  this.boxedValue = boxedValue;
};
RealScript_SimpleEnumType.typeId = "l";
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
RealScript_EnumUsingClass.typeId = "m";
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
RealScript__StaticConstructorType____cctor();

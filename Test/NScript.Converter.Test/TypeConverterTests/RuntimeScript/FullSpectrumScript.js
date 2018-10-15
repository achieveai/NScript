function SimpleInterface() {
};
SimpleInterface.typeId = "b";
Type__RegisterInterface(SimpleInterface, "RealScript.SimpleInterface");
function SimpleInheritedInterface() {
};
SimpleInheritedInterface.typeId = "c";
Type__RegisterInterface(SimpleInheritedInterface, "RealScript.SimpleInheritedInterface");
function InheritDerivedInterface() {
};
InheritDerivedInterface.typeId = "d";
function InheritDerivedInterface_factory() {
  var this_;
  this_ = new InheritDerivedInterface();
  this_.__ctor();
  return this_;
};
InheritDerivedInterface.defaultConstructor = InheritDerivedInterface_factory;
ptyp_ = InheritDerivedInterface.prototype;
ptyp_.getInt = function InheritDerivedInterface__GetInt(i) {
  return i + 2;
};
ptyp_.getStr = function InheritDerivedInterface__GetStr(i) {
  return Int32__ToString(i);
};
ptyp_.__ctor = function InheritDerivedInterface____ctor() {
};
ptyp_.V_GetStr_c = function(arg0) {
  return this.V_GetStr(arg0);
};
ptyp_.V_GetInt_b = ptyp_.getInt;
ptyp_.V_GetStr = ptyp_.getStr;
Type__RegisterReferenceType(InheritDerivedInterface, "RealScript.InheritDerivedInterface", Object, [SimpleInheritedInterface, SimpleInterface]);
function InheritInterface() {
};
InheritInterface.typeId = "e";
function InheritInterface_factory() {
  var this_;
  this_ = new InheritInterface();
  this_.__ctor();
  return this_;
};
InheritInterface.defaultConstructor = InheritInterface_factory;
ptyp_ = InheritInterface.prototype;
ptyp_.getInt = function InheritInterface__GetInt(i) {
  return i + 2;
};
ptyp_.__ctor = function InheritInterface____ctor() {
};
ptyp_.V_GetInt_b = function(arg0) {
  return this.V_GetInt(arg0);
};
ptyp_.V_GetInt = ptyp_.getInt;
Type__RegisterReferenceType(InheritInterface, "RealScript.InheritInterface", Object, [SimpleInterface]);
function SecondOrderInterfaceInherit() {
};
SecondOrderInterfaceInherit.typeId = "f";
function SecondOrderInterfaceInherit_factory() {
  var this_;
  this_ = new SecondOrderInterfaceInherit();
  this_.__ctora();
  return this_;
};
SecondOrderInterfaceInherit.defaultConstructor = SecondOrderInterfaceInherit_factory;
ptyp_ = new InheritInterface();
SecondOrderInterfaceInherit.prototype = ptyp_;
ptyp_.getInta = function SecondOrderInterfaceInherit__GetInt(i) {
  return i + 3;
};
ptyp_.__ctora = function SecondOrderInterfaceInherit____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInta;
Type__RegisterReferenceType(SecondOrderInterfaceInherit, "RealScript.SecondOrderInterfaceInherit", InheritInterface, []);
function MultipleConstructorsType() {
};
MultipleConstructorsType.typeId = "g";
function MultipleConstructorsType_factory(dbl) {
  var this_;
  this_ = new MultipleConstructorsType();
  this_.__ctor(dbl);
  return this_;
};
function MultipleConstructorsType_factorya(i) {
  var this_;
  this_ = new MultipleConstructorsType();
  this_.__ctora(i);
  return this_;
};
ptyp_ = MultipleConstructorsType.prototype;
ptyp_.intField = 0;
ptyp_.doubleField = 0;
ptyp_.__ctor = function MultipleConstructorsType____ctor(dbl) {
  this.doubleField = dbl;
};
ptyp_.__ctora = function MultipleConstructorsType____ctora(i) {
  this.intField = i;
};
Type__RegisterReferenceType(MultipleConstructorsType, "RealScript.MultipleConstructorsType", Object, []);
function SameNameInstanceAndStaticMethod() {
};
SameNameInstanceAndStaticMethod.typeId = "h";
function SameNameInstanceAndStaticMethod__GetInt(c) {
  return c.intFiled;
};
function SameNameInstanceAndStaticMethod_factory() {
  var this_;
  this_ = new SameNameInstanceAndStaticMethod();
  this_.__ctor();
  return this_;
};
SameNameInstanceAndStaticMethod.defaultConstructor = SameNameInstanceAndStaticMethod_factory;
ptyp_ = SameNameInstanceAndStaticMethod.prototype;
ptyp_.intFiled = 0;
ptyp_.getInt = function SameNameInstanceAndStaticMethod__GetInta() {
  return this.intFiled;
};
ptyp_.__ctor = function SameNameInstanceAndStaticMethod____ctor() {
};
Type__RegisterReferenceType(SameNameInstanceAndStaticMethod, "RealScript.SameNameInstanceAndStaticMethod", Object, []);
function VirtualBase() {
};
VirtualBase.typeId = "i";
function VirtualBase_factory() {
  var this_;
  this_ = new VirtualBase();
  this_.__ctor();
  return this_;
};
VirtualBase.defaultConstructor = VirtualBase_factory;
ptyp_ = VirtualBase.prototype;
ptyp_.getInt = function VirtualBase__GetInt(i) {
  return i * 2;
};
ptyp_.__ctor = function VirtualBase____ctor() {
};
ptyp_.V_GetInt = ptyp_.getInt;
Type__RegisterReferenceType(VirtualBase, "RealScript.VirtualBase", Object, []);
function VirtualOverride() {
};
VirtualOverride.typeId = "j";
function VirtualOverride_factory() {
  var this_;
  this_ = new VirtualOverride();
  this_.__ctora();
  return this_;
};
VirtualOverride.defaultConstructor = VirtualOverride_factory;
ptyp_ = new VirtualBase();
VirtualOverride.prototype = ptyp_;
ptyp_.getInta = function VirtualOverride__GetInt(i) {
  return i / 2 | 0;
};
ptyp_.__ctora = function VirtualOverride____ctor() {
  this.__ctor();
};
ptyp_.V_GetInt = ptyp_.getInta;
Type__RegisterReferenceType(VirtualOverride, "RealScript.VirtualOverride", VirtualBase, []);
function SimpleInstanceType() {
};
SimpleInstanceType.typeId = "k";
function SimpleInstanceType_factory() {
  var this_;
  this_ = new SimpleInstanceType();
  this_.__ctor();
  return this_;
};
SimpleInstanceType.defaultConstructor = SimpleInstanceType_factory;
ptyp_ = SimpleInstanceType.prototype;
ptyp_.intField = 0;
ptyp_.getField = function SimpleInstanceType__GetField() {
  return this.intField;
};
ptyp_.__ctor = function SimpleInstanceType____ctor() {
};
Type__RegisterReferenceType(SimpleInstanceType, "RealScript.SimpleInstanceType", Object, []);
function SimpleStaticType() {
};
SimpleStaticType__intField = 0;
function SimpleStaticType__GetField() {
  return SimpleStaticType__intField;
};
function StaticConstructorType() {
};
StaticConstructorType__tempValue = 0;
function StaticConstructorType____cctor() {
  StaticConstructorType__tempValue = Class1__GetMoreStatic(10);
};
function StaticConstructorType__get_Value() {
  return StaticConstructorType__tempValue;
};
function SimpleEnumType(boxedValue) {
  this.boxedValue = boxedValue;
};
SimpleEnumType.typeId = "l";
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
EnumUsingClass.typeId = "m";
EnumUsingClass__TestValue = 2;
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
StaticConstructorType____cctor();

(function(){
var Sunlight__Framework__UI__Test__ManualTemplateTests__noneValue, Sunlight_Framework_UI_Test_ValueIfTrue_$String$_, System__String__formatHelperRegex, System__String__trimStartHelperRegex, System__String__trimEndHelperRegex, Sunlight__Framework__UI__Test__LiveBinderTests__oneWayBinder, System_ArrayG_$ArrayG_$Func_$Object_x_Object$_$_$_, System_ArrayG_$ArrayG_$String$_$_, Sunlight__Framework__UI__Test__LiveBinderTests__twoWayBinder, System_ArrayG_$Func_$Object_x_Object$_$_, Sunlight__Framework__UI__Test__LiveBinderTests__oneWayMultiBinder, System_ArrayG_$String$_, Sunlight__Framework__UI__Test__LiveBinderTests__twoWayMultiBinder, System_ArrayG_$SkinBinderInfo$_, System_ArrayG_$Object$_, System_ArrayG_$TestViewModelB$_, System__Type__typeMapping, System_Collections_Generic_NumberDictionary_$Task$_, System_Collections_Generic_Queue_$Task$_, Sunlight__Framework__TaskScheduler__instance, System_Collections_Generic_List_$ListViewItem$_, System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_, System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_, System_Collections_Generic_StringDictionary_$Function$_, System_Collections_Generic_KeyValuePair_$String_x_Function$_, System_Collections_Generic_KeyValuePair_$String_x_Action_$UIEvent$_$_, System_ArrayG_$Number$_, ptyp_, tmplStore, Test$5cTemplates$5cTestTemplate1_var, Test$5cTemplates$5cTestTemplateVMB_CssBinding_var, Test$5cTemplates$5cTestTemplateVMB_StyleBinding_var, Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var, Test$5cTemplates$5cTestTemplateB_PropertyBinding_var, Test$5cTemplates$5cTestTemplateVMB1_var, System_Func_$Object_x_Object$_, System_Collections_Generic_KeyValuePair_$Number_x_Task$_, System_Action_$UIEvent$_, System_Action_$INotifyPropertyChanged_x_String$_, System_Collections_Generic_KeyValuePair_$String_x_Action_$INotifyPropertyChanged_x_String$_$_, System_Collections_Generic_StringDictionary_$Int32$_, System_Collections_Generic_KeyValuePair_$String_x_Int32$_;
Function.typeId = "o";
System__Type__typeMapping = null;
function System__Type__CastType(this_, instance) {
  if (this_.isInstanceOfType(instance) || instance === null || typeof instance === "undefined") {
    if (this_.isStruct)
      return instance.boxedValue;
    return instance;
  }
  throw "InvalidCast to " + this_.fullName;
};
function System__Type__ToString(this_) {
  return this_.fullName ? this_.fullName : this_.name;
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
    lowerStrToValue[key.toLowerCase()] = enumStrToValueMap[key];
  }
  this_.enumValueToStrMap = valueToStr;
  this_.enumLowerStrToValueMap = lowerStrToValue;
  if (!System__Type__typeMapping)
    System__Type__typeMapping = {
    };
  System__Type__typeMapping[this_.fullName] = this_;
};
function System__Type__BoxTypeInstance(type, instance) {
  if (type.isNullable)
    return type.nullableBox(instance);
  else if (type.isStruct)
    return new type(instance);
  else
    return instance;
};
function System__Type__UnBoxTypeInstance(type, instance) {
  if (type.isNullable && instance === null)
    return null;
  else if (type.isStruct)
    return instance.boxedValue;
  else
    return instance;
};
function System__Type__GetDefaultValueStatic(type) {
  if (type.isStruct)
    return type.getDefaultValue();
  return null;
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
ptyp_.isInstanceOfType = function System__Type__IsInstanceOfType(instance) {
  if (instance === null || typeof instance === "undefined")
    return false;
  if (!this.isInterface)
    return instance instanceof this || instance && instance.constructor == this;
  else if (!instance.constructor.baseInterfaces)
    System__Type__InitializeBaseInterfaces(instance.constructor);
  return instance.constructor.baseInterfaces && instance.constructor.baseInterfaces[this.fullName];
};
ptyp_.defaultConstructor = function System__Type__DefaultConstructor() {
  if (this.isStruct)
    return this.getDefaultValue;
  throw "Default constructor not implemented";
};
ptyp_.getDefaultValue = function System__Type__GetDefaultValue() {
  return null;
};
ptyp_.nullableBox = function System__Type__NullableBox(instance) {
  return null;
};
ptyp_.toString = function() {
  return System__Type__ToString(this);
};
System__Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "p";
function System__Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
function System__Object__GetNewImportedExtension() {
  return {
    "toJSON": System__Object__NoReturn
  };
};
function System__Object__NoReturn() {
  return undefined;
};
System__Type__RegisterReferenceType(Object, "System.Object", null, []);
function Sunlight_Framework_UI_Test_LiveBinderTests() {
};
Sunlight_Framework_UI_Test_LiveBinderTests.typeId = "q";
Sunlight__Framework__UI__Test__LiveBinderTests__oneWayBinder = null;
Sunlight__Framework__UI__Test__LiveBinderTests__twoWayBinder = null;
Sunlight__Framework__UI__Test__LiveBinderTests__oneWayMultiBinder = null;
Sunlight__Framework__UI__Test__LiveBinderTests__twoWayMultiBinder = null;
function Sunlight__Framework__UI__Test__LiveBinderTests__Setup() {
  Sunlight__Framework__UI__Test__LiveBinderTests__oneWayBinder = Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory(System__NativeArray$1__op_Implicit(System_ArrayG_$ArrayG_$Func_$Object_x_Object$_$_$_.__ctor([function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propStr1();
  }])), System__NativeArray$1__op_Implicit(System_ArrayG_$ArrayG_$String$_$_.__ctor(["PropStr1"])), function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del2(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propStr1(val);
  }, 17, 0, 0, null, null);
  Sunlight__Framework__UI__Test__LiveBinderTests__twoWayBinder = Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del3(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propStr1();
  }])), function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del4(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propStr1(val);
  }, System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor(["PropStr1"])), function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del5(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propStr1(val);
  }, function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del6(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propStr1();
  }, "PropStr1", 17, 0, 0, null, null, null);
  Sunlight__Framework__UI__Test__LiveBinderTests__oneWayMultiBinder = Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del7(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_testVMA();
  }, function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del8(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propStr1();
  }])), System__NativeArray$1__op_Implicit(System_ArrayG_$String$_.__ctor(["TestVMA", "PropStr1"])), function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del9(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propStr1(System__Type__CastType(String, val));
  }, 17, 0, 0, null, null);
  Sunlight__Framework__UI__Test__LiveBinderTests__twoWayMultiBinder = Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del10(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_testVMA();
  }, function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del11(obj) {
    return System__Type__BoxTypeInstance(System_Int32, System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propInt1());
  }])), function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del12(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propInt1(System__Type__UnBoxTypeInstance(System_Int32, val));
  }, System__NativeArray$1__op_Implicit(System_ArrayG_$String$_.__ctor(["TestVMA", "PropInt1"])), function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del13(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propStr1(System__Type__CastType(String, val));
  }, function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del14(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propStr1();
  }, "PropStr1", 17, 0, 0, function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del15(obj) {
    return obj.toString();
  }, function Sunlight__Framework__UI__Test__LiveBinderTests__Setup_del16(obj) {
    return System__Type__BoxTypeInstance(System_Int32, System__Int32__Parse(System__Type__CastType(String, obj)));
  }, null);
};
function Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderOnActivate() {
  var src, target, liveBinder;
  src = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  target = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  src.set_propStr1("test");
  liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(Sunlight__Framework__UI__Test__LiveBinderTests__oneWayBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  QUnit.notEqual(src.get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  QUnit.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
};
function Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderOnChange() {
  var src, target, liveBinder;
  src = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  target = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  src.set_propStr1("test");
  liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(Sunlight__Framework__UI__Test__LiveBinderTests__oneWayBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  QUnit.notEqual(src.get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  QUnit.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  src.set_propStr1("testChanged");
  QUnit.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
};
function Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderMultiOnActivate() {
  var src, target, liveBinder;
  src = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  target = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  src.set_testVMA(Sunlight__Framework__UI__Test__TestViewModelA_factory());
  src.get_testVMA().set_propStr1("test");
  liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(Sunlight__Framework__UI__Test__LiveBinderTests__oneWayMultiBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  QUnit.notEqual(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  QUnit.equal(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
};
function Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderMultiOnChange() {
  var src, target, liveBinder, stmtTemp1;
  src = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  target = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  src.set_testVMA(Sunlight__Framework__UI__Test__TestViewModelA_factory());
  src.get_testVMA().set_propStr1("test");
  liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(Sunlight__Framework__UI__Test__LiveBinderTests__oneWayMultiBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  QUnit.notEqual(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  QUnit.equal(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  src.get_testVMA().set_propStr1("testChanged");
  QUnit.equal(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes on lastElement should have flowed.");
  src.set_testVMA((stmtTemp1 = Sunlight__Framework__UI__Test__TestViewModelA_factory(), stmtTemp1.set_propStr1("ChangedTest"), stmtTemp1));
  QUnit.equal(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes on firstElement should have flowed.");
  src.set_testVMA(null);
  QUnit.equal(null, target.get_propStr1(), "if liveBinder is active and changes on firstElement should have flowed.");
};
function Sunlight__Framework__UI__Test__LiveBinderTests__TestTwoWayLiveBinderOnChange() {
  var src, target, liveBinder;
  src = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  target = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  src.set_propStr1("test");
  liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(Sunlight__Framework__UI__Test__LiveBinderTests__twoWayBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  QUnit.notEqual(src.get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  QUnit.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  src.set_propStr1("testChanged");
  QUnit.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  target.set_propStr1("Changed Again");
  QUnit.equal(target.get_propStr1(), src.get_propStr1(), "if liveBinder is active in twoWay changes on target should flow back.");
};
function Sunlight__Framework__UI__Test__LiveBinderTests__TestTwoWayLiveBinderMultiOnChangeWithConverters() {
  var src, target, liveBinder, stmtTemp1;
  src = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  target = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  src.set_testVMA(Sunlight__Framework__UI__Test__TestViewModelA_factory());
  src.get_testVMA().set_propInt1(1);
  liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(Sunlight__Framework__UI__Test__LiveBinderTests__twoWayMultiBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  QUnit.notEqual(System__Int32__ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  QUnit.equal(System__Int32__ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  src.get_testVMA().set_propInt1(2);
  QUnit.equal(System__Int32__ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active and changes on lastElement should have flowed.");
  src.set_testVMA((stmtTemp1 = Sunlight__Framework__UI__Test__TestViewModelA_factory(), stmtTemp1.set_propInt1(3), stmtTemp1));
  QUnit.equal(System__Int32__ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active and changes on firstElement should have flowed.");
  target.set_propStr1("21");
  QUnit.equal(System__Int32__ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active in twoWay changes on target should flow back.");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_LiveBinderTests, "Sunlight.Framework.UI.Test.LiveBinderTests", Object, []);
function Sunlight_Framework_UI_Test_NScriptsTemplateTests() {
};
Sunlight_Framework_UI_Test_NScriptsTemplateTests.typeId = "r";
function Sunlight__Framework__UI__Test__NScriptsTemplateTests__Setup() {
  Sunlight__Framework__TaskScheduler__set_Instance(Sunlight__Framework__TaskScheduler_factory(Sunlight__Framework__TestWindowTimer_factory(), 10, 10));
};
function Sunlight__Framework__UI__Test__NScriptsTemplateTests__Test() {
  QUnit.notEqual(null, Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplate1(), "Template should not be null");
  QUnit.ok(true, "true should be true");
};
function Sunlight__Framework__UI__Test__NScriptsTemplateTests__TestApplySkin() {
  var element, control;
  element = window.document.createElement("div");
  control = Sunlight__Framework__UI__UISkinableElement_factory(element);
  control.set_dataContext(Sunlight__Framework__UI__Test__TestViewModelA_factory());
  control.set_skin(Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplate1());
  control.activate();
  QUnit.notEqual(null, element.querySelector("[test]"), "After applying skin, skin element should be loaded");
};
function Sunlight__Framework__UI__Test__NScriptsTemplateTests__TestCssBinder() {
  var element, control, vm, elem;
  element = window.document.createElement("div");
  control = Sunlight__Framework__UI__UISkinableElement_factory(element);
  vm = Sunlight__Framework__UI__Test__TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplateVMB_CssBinding());
  control.activate();
  elem = element.querySelector("[test]");
  QUnit.notEqual(null, elem, "After applying skin, skin element should be loaded");
  QUnit.equal("", elem.className, "Class should not be set if PropBool1 is not set.");
  vm.set_propBool1(true);
  QUnit.notEqual("", elem.className, "Class should be set if PropBool1 is set.");
};
function Sunlight__Framework__UI__Test__NScriptsTemplateTests__TestStyleBinder() {
  var element, control, vm, elem;
  element = window.document.createElement("div");
  control = Sunlight__Framework__UI__UISkinableElement_factory(element);
  vm = Sunlight__Framework__UI__Test__TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplateVMB_StyleBinding());
  control.activate();
  elem = element.querySelector("[test]");
  QUnit.notEqual(null, elem, "After applying skin, skin element should be loaded");
  QUnit.equal("", elem.style.height, "Style should not be set if PropStr1 is not set.");
  vm.set_propStr1("10px");
  QUnit.equal("10px", elem.style.height, "Style should be set if PropStr1 is set.");
};
function Sunlight__Framework__UI__Test__NScriptsTemplateTests__TestAttrBinder() {
  var element, control, vm, elem;
  element = window.document.createElement("div");
  control = Sunlight__Framework__UI__UISkinableElement_factory(element);
  vm = Sunlight__Framework__UI__Test__TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplateVMB_AttrBinding());
  control.activate();
  elem = element.querySelector("[test]");
  QUnit.notEqual(elem, null, "After applying skin, skin element should be loaded");
  QUnit.equal(elem.getAttribute("test1"), null, "Attribute 'test' should not be set if PropStr1 is not set.");
  vm.set_propStr1("TTTest");
  QUnit.equal("TTTest", elem.getAttribute("test1"), "Attribute 'test' should be set if PropStr1 is set.");
};
function Sunlight__Framework__UI__Test__NScriptsTemplateTests__TestPropertyBinder() {
  var element, control, vm;
  element = window.document.createElement("div");
  control = Sunlight__Framework__UI__Test__TestSkinableWithTestUIElementPart_factory(element);
  vm = Sunlight__Framework__UI__Test__TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplateB_PropertyBinding());
  control.activate();
  QUnit.ok(control.get_part(), "templatePart should not be null.");
  QUnit.equal(control.get_part().get_oneWayStrictBinding(), vm.get_propStr1(), "vmPropStr1 should be equal to OnewayStrictBinding.");
  vm.set_propStr1("T1");
  QUnit.equal(control.get_part().get_oneWayStrictBinding(), vm.get_propStr1(), "vmPropStr1 should be equal to OnewayStrictBinding.");
  QUnit.equal(control.get_part().get_twoWayLooseBinding(), vm.get_propInt1(), "TwoWayLooseBinding and bound property PropInt1 should be equal.");
  vm.set_propInt1(11);
  QUnit.equal(control.get_part().get_twoWayLooseBinding(), vm.get_propInt1(), "TwoWayLooseBinding and bound property PropInt1 should be equal.");
  control.get_part().set_twoWayLooseBinding(101);
  QUnit.equal(control.get_part().get_twoWayLooseBinding(), vm.get_propInt1(), "TwoWayLooseBinding and bound property PropInt1 should be equal.");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_NScriptsTemplateTests, "Sunlight.Framework.UI.Test.NScriptsTemplateTests", Object, []);
function Sunlight_Framework_UI_Test_SkinBinderHelperTests() {
};
Sunlight_Framework_UI_Test_SkinBinderHelperTests.typeId = "s";
function Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestSimpleBinder() {
  var src, stmtTemp1, tar1;
  src = (stmtTemp1 = Sunlight__Framework__UI__Test__TestViewModelA_factory(), stmtTemp1.set_propStr1("Test"), stmtTemp1.set_propInt1(1), stmtTemp1.set_propBool1(true), stmtTemp1);
  tar1 = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  Sunlight__Framework__UI__Helpers__SkinBinderHelper__Bind(System__NativeArray$1__op_Implicit(System_ArrayG_$SkinBinderInfo$_.__ctor([Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([Sunlight__Framework__UI__Test__TestViewModelA__PropStr1Getter])), Sunlight__Framework__UI__Test__TestViewModelA__PropStr1Setter, 17, 0, null, null)])), src, System__NativeArray$1__op_Implicit(System_ArrayG_$Object$_.__ctor([tar1])));
  QUnit.equal(src.get_propStr1(), tar1.get_propStr1(), "After BindDataContext values should be equal");
};
function Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestAttrBinding() {
  var src, stmtTemp1, target;
  src = (stmtTemp1 = Sunlight__Framework__UI__Test__TestViewModelA_factory(), stmtTemp1.set_propStr1("Test"), stmtTemp1);
  target = window.document.createElement("div");
  Sunlight__Framework__UI__Helpers__SkinBinderHelper__Bind(System__NativeArray$1__op_Implicit(System_ArrayG_$SkinBinderInfo$_.__ctor([Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([Sunlight__Framework__UI__Test__TestViewModelA__PropStr1Getter])), function Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestAttrBinding_del(o1, o2, o3) {
    Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetAttribute(o1, System__Type__CastType(String, o2), System__Type__CastType(String, o3));
  }, "tmp", 113, 0, null, null, 0)])), src, System__NativeArray$1__op_Implicit(System_ArrayG_$Object$_.__ctor([target])));
  QUnit.equal(src.get_propStr1(), target.getAttribute("tmp"), "After BindDataContext values should be equal");
};
function Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestStyleBinding() {
  var src, stmtTemp1, target;
  src = (stmtTemp1 = Sunlight__Framework__UI__Test__TestViewModelA_factory(), stmtTemp1.set_propBool1(true), stmtTemp1);
  target = window.document.createElement("div");
  target.className = "t1";
  Sunlight__Framework__UI__Helpers__SkinBinderHelper__Bind(System__NativeArray$1__op_Implicit(System_ArrayG_$SkinBinderInfo$_.__ctor([Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([Sunlight__Framework__UI__Test__TestViewModelA__PropBool1Getter])), function Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestStyleBinding_del(o1, o2, o3) {
    Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetCssClass(o1, System__Type__UnBoxTypeInstance(System_Boolean, o2), System__Type__CastType(String, o3));
  }, "test", 113, 0, null, null, 0)])), src, System__NativeArray$1__op_Implicit(System_ArrayG_$Object$_.__ctor([target])));
  QUnit.equal("t1 test", target.className, "After BindDataContext values should be equal");
  src.set_propBool1(false);
  Sunlight__Framework__UI__Helpers__SkinBinderHelper__Bind(System__NativeArray$1__op_Implicit(System_ArrayG_$SkinBinderInfo$_.__ctor([Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([Sunlight__Framework__UI__Test__TestViewModelA__PropBool1Getter])), function Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestStyleBinding_del2(o1, o2, o3) {
    Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetCssClass(o1, System__Type__UnBoxTypeInstance(System_Boolean, o2), System__Type__CastType(String, o3));
  }, "test", 113, 0, null, null, 0)])), src, System__NativeArray$1__op_Implicit(System_ArrayG_$Object$_.__ctor([target])));
  QUnit.equal("t1", target.className, "After BindDataContext values should be equal");
};
function Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestTextContentBinding() {
  var src, stmtTemp1, target;
  src = (stmtTemp1 = Sunlight__Framework__UI__Test__TestViewModelA_factory(), stmtTemp1.set_propStr1("Test"), stmtTemp1);
  target = window.document.createElement("div");
  Sunlight__Framework__UI__Helpers__SkinBinderHelper__Bind(System__NativeArray$1__op_Implicit(System_ArrayG_$SkinBinderInfo$_.__ctor([Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([Sunlight__Framework__UI__Test__TestViewModelA__PropStr1Getter])), function Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestTextContentBinding_del(o1, o2) {
    Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent(o1, System__Type__CastType(String, o2));
  }, 17, 0, null, null)])), src, System__NativeArray$1__op_Implicit(System_ArrayG_$Object$_.__ctor([target])));
  QUnit.equal(src.get_propStr1(), target.textContent, "After BindDataContext values should be equal");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_SkinBinderHelperTests, "Sunlight.Framework.UI.Test.SkinBinderHelperTests", Object, []);
function Sunlight_Framework_UI_Test_TestListView() {
};
Sunlight_Framework_UI_Test_TestListView.typeId = "t";
function Sunlight__Framework__UI__Test__TestListView__Setup() {
  Sunlight__Framework__TaskScheduler__set_Instance(Sunlight__Framework__TaskScheduler_factory(Sunlight__Framework__TestWindowTimer_factory(), 10, 10));
};
function Sunlight__Framework__UI__Test__TestListView__TestChildSkin() {
  var document, listView, vmAs, stmtTemp1, stmtTemp10, stmtTemp10, elems, i;
  document = window.document;
  listView = Sunlight__Framework__UI__ListView_factory(document.createElement("div"));
  listView.set_itemSkin(Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplateVMB1());
  vmAs = System_ArrayG_$TestViewModelB$_.__ctor([(stmtTemp1 = Sunlight__Framework__UI__Test__TestViewModelB_factory(), stmtTemp1.set_propStr1("StrTest"), stmtTemp1), (stmtTemp10 = Sunlight__Framework__UI__Test__TestViewModelB_factory(), stmtTemp10.set_propStr1("TestStr"), stmtTemp10), (stmtTemp10 = Sunlight__Framework__UI__Test__TestViewModelB_factory(), stmtTemp10.set_propStr1("TestTT1"), stmtTemp10)]);
  listView.set_fixedList(vmAs);
  listView.set_inactiveIfNullContext(false);
  listView.activate();
  elems = listView.get_element().querySelectorAll("[test]");
  QUnit.equal(3, elems.length, "number of children should be 3");
  for (i = 0; i < 3; i++)
    QUnit.equal(vmAs.get_item(i).get_propStr1(), elems[i].innerText, "Inner text for element should match property it's bound to.");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_TestListView, "Sunlight.Framework.UI.Test.TestListView", Object, []);
function Sunlight_Framework_UI_Test_ManualTemplateTests() {
};
Sunlight_Framework_UI_Test_ManualTemplateTests.typeId = "u";
Sunlight__Framework__UI__Test__ManualTemplateTests__noneValue = null;
function Sunlight__Framework__UI__Test__ManualTemplateTests__Setup() {
};
function Sunlight__Framework__UI__Test__ManualTemplateTests__Test() {
  QUnit.ok(true, "true should be true");
};
function Sunlight__Framework__UI__Test__ManualTemplateTests____cctor() {
  Sunlight__Framework__UI__Test__ManualTemplateTests__noneValue = Sunlight_Framework_UI_Test_ValueIfTrue_$String$_.__ctor("none");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_ManualTemplateTests, "Sunlight.Framework.UI.Test.ManualTemplateTests", Object, []);
function Sunlight_Framework_UI_Test_UIElementTests() {
};
Sunlight_Framework_UI_Test_UIElementTests.typeId = "v";
function Sunlight__Framework__UI__Test__UIElementTests__TestNewUIElement() {
  var doc, element;
  doc = window.document;
  element = Sunlight__Framework__UI__UIElement_factory(doc.createElement("div"));
  QUnit.notEqual(element.get_element(), null, "element.Element != null");
  QUnit.equal(element.get_element().tagName, "DIV", "element.Element.TagName == 'DIV'");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_UIElementTests, "Sunlight.Framework.UI.Test.UIElementTests", Object, []);
String.typeId = "m";
System__String__formatHelperRegex = null;
System__String__trimStartHelperRegex = null;
System__String__trimEndHelperRegex = null;
function System__String____cctor() {
  System__String__formatHelperRegex = new RegExp("(\\{[^\\}^\\{]+\\})", "g");
  System__String__trimStartHelperRegex = new RegExp("^[\\s\\xA0]+");
  System__String__trimEndHelperRegex = new RegExp("[\\s\\xA0]+$");
};
function System__String__Trim(this_) {
  return System__String__TrimEnd(System__String__TrimStart(this_));
};
function System__String__TrimEnd(this_) {
  return this_.trimLeft ? this_.trimLeft() : this_.replace(System__String__trimEndHelperRegex, "");
};
function System__String__TrimStart(this_) {
  return this_.trimRight ? this_.trimRight() : this_.replace(System__String__trimStartHelperRegex, "");
};
function System__String__get_Item(this_, index) {
  return this_.charCodeAt(index);
};
System__String__TrimEnd = function System__String__TrimEnd(this_) {
  return this_.trimLeft ? this_.trimLeft() : this_.replace(System__String__trimEndHelperRegex, "");
};
System__String__TrimStart = function System__String__TrimStart(this_) {
  return this_.trimRight ? this_.trimRight() : this_.replace(System__String__trimStartHelperRegex, "");
};
System__Type__RegisterReferenceType(String, "System.String", Object, []);
RegExp.typeId = "w";
System__Type__RegisterReferenceType(RegExp, "System.RegularExpression", Object, []);
function Sunlight_Framework_UI_Helpers_SkinBinderInfo() {
};
Sunlight_Framework_UI_Helpers_SkinBinderInfo.typeId = "x";
function Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0(propertyGetterPath, targetPropertySetter, binderType, objectIndex, forwardConverter, defaultValue) {
  var this_;
  this_ = new Sunlight_Framework_UI_Helpers_SkinBinderInfo();
  this_.__ctor(propertyGetterPath, targetPropertySetter, binderType, objectIndex, forwardConverter, defaultValue);
  return this_;
};
function Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0(propertyGetterPath, targetPropertySetterWithArg, targetPropertySetterArg, binderType, objectIndex, forwardConverter, defaultValue, extraObjectIndex) {
  var this_;
  this_ = new Sunlight_Framework_UI_Helpers_SkinBinderInfo();
  this_.__ctor0(propertyGetterPath, targetPropertySetterWithArg, targetPropertySetterArg, binderType, objectIndex, forwardConverter, defaultValue, extraObjectIndex);
  return this_;
};
function Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue) {
  var this_;
  this_ = new Sunlight_Framework_UI_Helpers_SkinBinderInfo();
  this_.__ctor0(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue);
  return this_;
};
function Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0(propertyGetterPath, propertyNames, targetPropertySetter, targetPropertySetterArg, binderType, objectIndex, binderIndex, forwardConverter, defaultValue, extraObjectIndex) {
  var this_;
  this_ = new Sunlight_Framework_UI_Helpers_SkinBinderInfo();
  this_.__ctor0(propertyGetterPath, propertyNames, targetPropertySetter, targetPropertySetterArg, binderType, objectIndex, binderIndex, forwardConverter, defaultValue, extraObjectIndex);
  return this_;
};
function Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue) {
  var this_;
  this_ = new Sunlight_Framework_UI_Helpers_SkinBinderInfo();
  this_.__ctor0(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue);
  return this_;
};
ptyp_ = Sunlight_Framework_UI_Helpers_SkinBinderInfo.prototype;
ptyp_.propertyGetterPath = null;
ptyp_.propertySetter = null;
ptyp_.propertyNames = null;
ptyp_.targetPropertySetter = null;
ptyp_.targetPropertyGetter = null;
ptyp_.targetPropertySetterWithArg = null;
ptyp_.targetPropertySetterArg = null;
ptyp_.targetPropertyName = null;
ptyp_.binderType = 0;
ptyp_.objectIndex = 0;
ptyp_.binderIndex = 0;
ptyp_.extraObjectIndex = 0;
ptyp_.forwardConverter = null;
ptyp_.backwardConverter = null;
ptyp_.defaultValue = null;
ptyp_.mode = 0;
ptyp_.__ctor = function Sunlight__Framework__UI__Helpers__SkinBinderInfo____ctor0(propertyGetterPath, targetPropertySetter, binderType, objectIndex, forwardConverter, defaultValue) {
  this.binderIndex = -1;
  this.extraObjectIndex = -1;
  this.propertyGetterPath = propertyGetterPath;
  this.targetPropertySetter = targetPropertySetter;
  this.binderType = binderType;
  this.objectIndex = objectIndex;
  this.forwardConverter = forwardConverter;
  this.defaultValue = defaultValue;
  this.mode = 0;
};
ptyp_.__ctor0 = function Sunlight__Framework__UI__Helpers__SkinBinderInfo____ctor0(propertyGetterPath, targetPropertySetterWithArg, targetPropertySetterArg, binderType, objectIndex, forwardConverter, defaultValue, extraObjectIndex) {
  this.binderIndex = -1;
  this.extraObjectIndex = -1;
  this.propertyGetterPath = propertyGetterPath;
  this.targetPropertySetterArg = targetPropertySetterArg;
  this.targetPropertySetterWithArg = targetPropertySetterWithArg;
  this.binderType = binderType;
  this.objectIndex = objectIndex;
  this.forwardConverter = forwardConverter;
  this.defaultValue = defaultValue;
  this.mode = 0;
  this.extraObjectIndex = extraObjectIndex;
};
ptyp_.__ctor0 = function Sunlight__Framework__UI__Helpers__SkinBinderInfo____ctor(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue) {
  this.binderIndex = -1;
  this.extraObjectIndex = -1;
  this.propertyGetterPath = propertyGetterPath;
  this.propertyNames = propertyNames;
  this.targetPropertySetter = targetPropertySetter;
  this.binderType = binderType;
  this.objectIndex = objectIndex;
  this.forwardConverter = forwardConverter;
  this.defaultValue = defaultValue;
  this.mode = 1;
};
ptyp_.__ctor0 = function Sunlight__Framework__UI__Helpers__SkinBinderInfo____ctor0(propertyGetterPath, propertyNames, targetPropertySetter, targetPropertySetterArg, binderType, objectIndex, binderIndex, forwardConverter, defaultValue, extraObjectIndex) {
  this.binderIndex = -1;
  this.extraObjectIndex = -1;
  this.propertyGetterPath = propertyGetterPath;
  this.propertyNames = propertyNames;
  this.targetPropertySetterWithArg = targetPropertySetter;
  this.targetPropertySetterArg = targetPropertySetterArg;
  this.binderType = binderType;
  this.binderIndex = binderIndex;
  this.objectIndex = objectIndex;
  this.forwardConverter = forwardConverter;
  this.defaultValue = defaultValue;
  this.mode = 1;
  this.extraObjectIndex = extraObjectIndex;
};
ptyp_.__ctor0 = function Sunlight__Framework__UI__Helpers__SkinBinderInfo____ctor0(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue) {
  this.binderIndex = -1;
  this.extraObjectIndex = -1;
  this.propertyGetterPath = propertyGetterPath;
  this.propertySetter = propertySetter;
  this.propertyNames = propertyNames;
  this.targetPropertySetter = targetPropertySetter;
  this.targetPropertyGetter = targetPropertyGetter;
  this.targetPropertyName = targetPropertyName;
  this.binderType = binderType;
  this.objectIndex = objectIndex;
  this.binderIndex = binderIndex;
  this.forwardConverter = forwardConverter;
  this.backwardConverter = backwardConverter;
  this.defaultValue = defaultValue;
  this.mode = 2;
};
ptyp_.setTargetValue = function Sunlight__Framework__UI__Helpers__SkinBinderInfo__SetTargetValue(target, value, extraObjectArray) {
  var binderInfo, propertySetterMode, element;
  binderInfo = this;
  propertySetterMode = binderInfo.binderType & 240;
  if (propertySetterMode == 16)
    binderInfo.targetPropertySetter(target, value);
  else if (propertySetterMode == 48) {
    if (extraObjectArray[binderInfo.extraObjectIndex] === value)
      return;
    if (extraObjectArray[binderInfo.extraObjectIndex] !== null)
      this.targetPropertySetterWithArg(target, extraObjectArray[binderInfo.extraObjectIndex], false);
    extraObjectArray[binderInfo.extraObjectIndex] = value;
    if (value !== null)
      this.targetPropertySetterWithArg(target, value, true);
  }
  else if (propertySetterMode == 64) {
    element = target;
    if (extraObjectArray[binderInfo.extraObjectIndex] === value)
      return;
    if (!System__Object__IsNullOrUndefined(extraObjectArray[binderInfo.extraObjectIndex]))
      System__Web__Html__Element__UnBind(element, System__Type__CastType(String, binderInfo.targetPropertySetterArg), extraObjectArray[binderInfo.extraObjectIndex], false);
    extraObjectArray[binderInfo.extraObjectIndex] = value;
    if (!System__Object__IsNullOrUndefined(value))
      System__Web__Html__Element__Bind(element, System__Type__CastType(String, binderInfo.targetPropertySetterArg), value, false);
  }
  else if (binderInfo.targetPropertySetter)
    binderInfo.targetPropertySetter(target, value);
  else
    binderInfo.targetPropertySetterWithArg(target, value, binderInfo.targetPropertySetterArg);
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Helpers_SkinBinderInfo, "Sunlight.Framework.UI.Helpers.SkinBinderInfo", Object, []);
function Sunlight_Framework_Observables_INotifyPropertyChanged() {
};
Sunlight_Framework_Observables_INotifyPropertyChanged.typeId = "b";
System__Type__RegisterInterface(Sunlight_Framework_Observables_INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function Sunlight_Framework_Observables_ObservableObject() {
};
Sunlight_Framework_Observables_ObservableObject.typeId = "y";
ptyp_ = Sunlight_Framework_Observables_ObservableObject.prototype;
ptyp_.eventHandlers = null;
ptyp_.addPropertyChangedListener = function Sunlight__Framework__Observables__ObservableObject__AddPropertyChangedListener(propertyName, callback) {
  var cb;
  if (!this.eventHandlers)
    this.eventHandlers = System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_.defaultConstructor();
  if (!this.eventHandlers.tryGetValue(propertyName, {
    read: function() {
      return cb;
    },
    write: function(arg0) {
      return cb = arg0;
    }
  }))
    cb = callback;
  else
    cb = System__Delegate__Combine(cb, callback);
  this.eventHandlers.set_item(propertyName, cb);
};
ptyp_.removePropertyChangedListener = function Sunlight__Framework__Observables__ObservableObject__RemovePropertyChangedListener(propertyName, callback) {
  var cb;
  if (!this.eventHandlers)
    return;
  if (this.eventHandlers.tryGetValue(propertyName, {
    read: function() {
      return cb;
    },
    write: function(arg0) {
      return cb = arg0;
    }
  })) {
    cb = System__Delegate__Remove(cb, callback);
    if (cb)
      this.eventHandlers.set_item(propertyName, cb);
    else
      this.eventHandlers.remove(propertyName);
  }
};
ptyp_.clearListeners = function Sunlight__Framework__Observables__ObservableObject__ClearListeners() {
  this.eventHandlers = null;
};
ptyp_.firePropertyChanged = function Sunlight__Framework__Observables__ObservableObject__FirePropertyChanged(propertyName) {
  var cb;
  if (this.eventHandlers) {
    if (this.eventHandlers.tryGetValue(propertyName, {
      read: function() {
        return cb;
      },
      write: function(arg0) {
        return cb = arg0;
      }
    }))
      cb(this, propertyName);
  }
};
ptyp_.__ctor = function Sunlight__Framework__Observables__ObservableObject____ctor() {
};
ptyp_.V_AddPropertyChangedListener_b = ptyp_.addPropertyChangedListener;
ptyp_.V_RemovePropertyChangedListener_b = ptyp_.removePropertyChangedListener;
System__Type__RegisterReferenceType(Sunlight_Framework_Observables_ObservableObject, "Sunlight.Framework.Observables.ObservableObject", Object, [Sunlight_Framework_Observables_INotifyPropertyChanged]);
function Sunlight_Framework_UI_Test_TestViewModelA() {
};
Sunlight_Framework_UI_Test_TestViewModelA.typeId = "z";
function Sunlight__Framework__UI__Test__TestViewModelA__PropStr1Getter(obj) {
  return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propStr1();
};
function Sunlight__Framework__UI__Test__TestViewModelA__PropStr1Setter(obj, val) {
  System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).set_propStr1(System__Type__CastType(String, val));
};
function Sunlight__Framework__UI__Test__TestViewModelA__PropBool1Getter(obj) {
  return System__Type__BoxTypeInstance(System_Boolean, System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propBool1());
};
function Sunlight__Framework__UI__Test__TestViewModelA_factory() {
  var this_;
  this_ = new Sunlight_Framework_UI_Test_TestViewModelA();
  this_.__ctor0();
  return this_;
};
Sunlight_Framework_UI_Test_TestViewModelA.defaultConstructor = Sunlight__Framework__UI__Test__TestViewModelA_factory;
ptyp_ = new Sunlight_Framework_Observables_ObservableObject();
Sunlight_Framework_UI_Test_TestViewModelA.prototype = ptyp_;
ptyp_.str1 = null;
ptyp_.bool1 = false;
ptyp_.testVMA = null;
ptyp_.get_propInt1 = function Sunlight__Framework__UI__Test__TestViewModelA__get_PropInt1() {
  return this.get_int1();
};
ptyp_.set_propInt1 = function Sunlight__Framework__UI__Test__TestViewModelA__set_PropInt1(value) {
  if (this.get_int1() != value) {
    this.set_int1(value);
    this.firePropertyChanged("PropInt1");
  }
};
ptyp_.get_propStr1 = function Sunlight__Framework__UI__Test__TestViewModelA__get_PropStr1() {
  return this.str1;
};
ptyp_.set_propStr1 = function Sunlight__Framework__UI__Test__TestViewModelA__set_PropStr1(value) {
  if (this.str1 !== value) {
    this.str1 = value;
    this.firePropertyChanged("PropStr1");
  }
};
ptyp_.get_propBool1 = function Sunlight__Framework__UI__Test__TestViewModelA__get_PropBool1() {
  return this.bool1;
};
ptyp_.set_propBool1 = function Sunlight__Framework__UI__Test__TestViewModelA__set_PropBool1(value) {
  if (this.bool1 != value) {
    this.bool1 = value;
    this.firePropertyChanged("PropBool1");
  }
};
ptyp_.get_testVMA = function Sunlight__Framework__UI__Test__TestViewModelA__get_TestVMA() {
  return this.testVMA;
};
ptyp_.set_testVMA = function Sunlight__Framework__UI__Test__TestViewModelA__set_TestVMA(value) {
  if (this.testVMA != value) {
    this.testVMA = value;
    this.firePropertyChanged("TestVMA");
  }
};
ptyp_.get_int1 = function Sunlight__Framework__UI__Test__TestViewModelA__get_int1() {
  return this.int1;
};
ptyp_.set_int1 = function Sunlight__Framework__UI__Test__TestViewModelA__set_int1(value) {
  return this.int1 = value;
};
ptyp_.__ctor0 = function Sunlight__Framework__UI__Test__TestViewModelA____ctor() {
  this.__ctor();
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_TestViewModelA, "Sunlight.Framework.UI.Test.TestViewModelA", Sunlight_Framework_Observables_ObservableObject, []);
function System_ValueType() {
};
System_ValueType.typeId = "ab";
ptyp_ = System_ValueType.prototype;
ptyp_.boxedValue = null;
System__Type__RegisterReferenceType(System_ValueType, "System.ValueType", Object, []);
Function.getDefaultValue = function() {
  return {
  };
};
function System_Int32(boxedValue) {
  this.boxedValue = boxedValue;
};
System_Int32.typeId = "bb";
System_Int32.getDefaultValue = function() {
  return 0;
};
function System__Int32__Parse(s) {
  return parseInt(s);
};
function System__Int32__ToString0(this_, radix) {
  return this_.toString(radix);
};
function System__Int32__ToString(this_) {
  return System__Int32__ToString0(this_, 10);
};
ptyp_ = new System_ValueType();
System_Int32.prototype = ptyp_;
ptyp_.toString = function() {
  return System__Int32__ToString(this.boxedValue);
};
System__Type__RegisterStructType(System_Int32, "System.Int32", []);
function System_Collections_IEnumerable() {
};
System_Collections_IEnumerable.typeId = "f";
System__Type__RegisterInterface(System_Collections_IEnumerable, "System.Collections.IEnumerable");
function System_Collections_ICollection() {
};
System_Collections_ICollection.typeId = "c";
System__Type__RegisterInterface(System_Collections_ICollection, "System.Collections.ICollection");
function System_Collections_IList() {
};
System_Collections_IList.typeId = "d";
System__Type__RegisterInterface(System_Collections_IList, "System.Collections.IList");
function System_ArrayImpl() {
};
System_ArrayImpl.typeId = "cb";
ptyp_ = System_ArrayImpl.prototype;
ptyp_.system__Collections__IList__get_Item = function System__ArrayImpl__System__Collections__IList__get_Item(index) {
  return this.V_GetValue(index);
};
ptyp_.system__Collections__IList__Clear = function System__ArrayImpl__System__Collections__IList__Clear() {
  throw System__NotSupportedException_factory();
};
ptyp_.system__Collections__IList__RemoveAt = function System__ArrayImpl__System__Collections__IList__RemoveAt(index) {
  throw System__NotSupportedException_factory();
};
ptyp_.system__Collections__ICollection__get_Count = function System__ArrayImpl__System__Collections__ICollection__get_Count() {
  return this.V_get_Length();
};
ptyp_.__ctor = function System__ArrayImpl____ctor() {
};
ptyp_.V_get_Item_d = ptyp_.system__Collections__IList__get_Item;
ptyp_.V_Clear_d = ptyp_.system__Collections__IList__Clear;
ptyp_.V_RemoveAt_d = ptyp_.system__Collections__IList__RemoveAt;
ptyp_.V_get_Count_c = ptyp_.system__Collections__ICollection__get_Count;
ptyp_.V_Contains_d = function(arg0) {
  return this.V_Contains(arg0);
};
ptyp_.V_IndexOf_d = function(arg0) {
  return this.V_IndexOf(arg0);
};
ptyp_.V_GetEnumerator_f = function() {
  return this.V_GetEnumerator();
};
ptyp_.V_CopyTo_c = function(arg0, arg1) {
  return this.V_CopyTo(arg0, arg1);
};
System__Type__RegisterReferenceType(System_ArrayImpl, "System.ArrayImpl", Object, [System_Collections_IList, System_Collections_IEnumerable, System_Collections_ICollection]);
function System__Delegate__Combine(a, b) {
  var funcs, rv;
  funcs = [];
  if (a != null)
    if (typeof a.funcs != "undefined")
      funcs = funcs.concat(a.funcs);
    else
      funcs.push(a);
  if (typeof b.funcs != "undefined")
    funcs = funcs.concat(b.funcs);
  else if (b)
    funcs.push(b);
  else
    return a;
  rv = System__Delegate__CreateJoinedArray(funcs);
  rv.fullName = "Multicast Delegate";
  return rv;
};
function System__Delegate__Create(functionName, instance) {
  var func, fn;
  func = instance[functionName];
  fn = "__@" + functionName;
  if (!(fn in instance)) {
    instance[fn] = function() {
        return func.apply(instance, arguments);
    };
    instance[fn].fullName = instance.constructor.toString() + "::" + functionName;
    instance[fn].isDelegate = true;
  }
  return instance[fn];
};
function System__Delegate__Remove(source, value) {
  var newArr, valArr, i, fullMatch, j, rv;
  if (source == value)
    return null;
  if (typeof source.funcs == "undefined")
    return source;
  newArr = [].concat(source.funcs);
  if (typeof value.funcs != "undefined") {
    valArr = value.funcs;
    for (i = newArr.length - valArr.length; i >= 0; --i) {
      fullMatch = true;
      for (j = 0; j < valArr.length; ++j)
        if (newArr[i + j] != valArr[j]) {
          fullMatch = false;
          break;
        }
      if (fullMatch) {
        newArr.splice(i, valArr.length);
        break;
      }
    }
  }
  else
    for (i = newArr.length - 1; i >= 0; --i)
      if (newArr[i] === value) {
        newArr.splice(i, 1);
        break;
      }
  if (newArr.length == 0)
    return null;
  else if (newArr.length == 1)
    return newArr[0];
  else if (newArr.length == source.funcs.length)
    return source;
  rv = System__Delegate__CreateJoinedArray(newArr);
  rv.fullName = "Multicast Delegate";
  return rv;
};
function System__Delegate__CreateJoinedArray(array) {
  var rv, i;
  rv = function() {
    var rv1; {
      for (i = 0; i < array.length; ++i)
        rv1 = array[i].apply(null, arguments);
      return rv1;
    }
  };
  rv.funcs = array;
  rv.isDelegate = true;
  return rv;
};
function Function0() {
};
Function0.typeId = "db";
Function0.prototype = new Function();
System__Type__RegisterReferenceType(Function0, "System.MulticastDelegate", Function, []);
function Sunlight_Framework_UI_Helpers_LiveBinder() {
};
Sunlight_Framework_UI_Helpers_LiveBinder.typeId = "eb";
function Sunlight__Framework__UI__Helpers__LiveBinder_factory(binderInfo, extraObjectArray) {
  var this_;
  this_ = new Sunlight_Framework_UI_Helpers_LiveBinder();
  this_.__ctor(binderInfo, extraObjectArray);
  return this_;
};
ptyp_ = Sunlight_Framework_UI_Helpers_LiveBinder.prototype;
ptyp_.binderInfo = null;
ptyp_.isActive = false;
ptyp_.source = null;
ptyp_.target = null;
ptyp_.liveObjects = null;
ptyp_.pathTraversed = 0;
ptyp_.updating = false;
ptyp_.extraObjectArray = null;
ptyp_.__ctor = function Sunlight__Framework__UI__Helpers__LiveBinder____ctor(binderInfo, extraObjectArray) {
  this.binderInfo = binderInfo;
  this.extraObjectArray = extraObjectArray;
};
ptyp_.set_source = function Sunlight__Framework__UI__Helpers__LiveBinder__set_Source(value) {
  if (this.source !== value) {
    this.source = value;
    this.flowValue();
  }
};
ptyp_.set_target = function Sunlight__Framework__UI__Helpers__LiveBinder__set_Target(value) {
  if (this.target !== value) {
    if (this.target !== null && this.binderInfo.mode == 2)
      System__Type__CastType(Sunlight_Framework_Observables_INotifyPropertyChanged, this.target).V_RemovePropertyChangedListener_b(this.binderInfo.targetPropertyName, System__Delegate__Create("onTargetPropertyChanged", this));
    this.target = value;
    if (this.target !== null && this.binderInfo.mode == 2)
      System__Type__CastType(Sunlight_Framework_Observables_INotifyPropertyChanged, this.target).V_AddPropertyChangedListener_b(this.binderInfo.targetPropertyName, System__Delegate__Create("onTargetPropertyChanged", this));
    this.flowValue();
  }
};
ptyp_.set_isActive = function Sunlight__Framework__UI__Helpers__LiveBinder__set_IsActive(value) {
  if (this.isActive != value) {
    this.isActive = value;
    if (this.isActive)
      this.activate();
    else
      this.deactivate();
  }
};
ptyp_.cleanup = function Sunlight__Framework__UI__Helpers__LiveBinder__Cleanup() {
  if (!this.isActive) {
    this.pathTraversed = 0;
    this.cleanRegistrations();
  }
};
ptyp_.activate = function Sunlight__Framework__UI__Helpers__LiveBinder__Activate() {
  this.flowValue();
};
ptyp_.deactivate = function Sunlight__Framework__UI__Helpers__LiveBinder__Deactivate() {
  this.isActive = false;
};
ptyp_.flowValue = function Sunlight__Framework__UI__Helpers__LiveBinder__FlowValue() {
  if (this.target === null || this.updating || !this.isActive)
    return;
  if (this.liveObjects === null)
    this.liveObjects = System_ArrayG_$Object$_.__ctor0(this.binderInfo.propertyGetterPath.length + 1);
  if (this.liveObjects.get_item(0) !== this.source) {
    if (!System__Object__IsNullOrUndefined(this.liveObjects.get_item(0))) {
      this.pathTraversed = 0;
      this.cleanRegistrations();
    }
    this.liveObjects.set_item(0, this.source);
    if (!System__Object__IsNullOrUndefined(this.liveObjects.get_item(0)))
      System__Type__CastType(Sunlight_Framework_Observables_INotifyPropertyChanged, this.liveObjects.get_item(0)).V_AddPropertyChangedListener_b(this.binderInfo.propertyNames[0], System__Delegate__Create("onSourcePropertyChanged", this));
  }
  this.setTargetProperty(this.getValue());
};
ptyp_.setTargetProperty = function Sunlight__Framework__UI__Helpers__LiveBinder__SetTargetProperty(value) {
  try {
    this.updating = true;
    this.binderInfo.setTargetValue(this.target, value, this.extraObjectArray);
  } finally {
    this.updating = false;
  }
};
ptyp_.getValue = function Sunlight__Framework__UI__Helpers__LiveBinder__GetValue() {
  var stmtTemp1, rv;
  try {
    rv = this.getValueInternal();
  } catch (stmtTemp1) {
    rv = this.binderInfo.defaultValue;
  }
  if (this.pathTraversed < this.liveObjects.V_get_Length())
    this.cleanRegistrations();
  return rv;
};
ptyp_.getValueInternal = function Sunlight__Framework__UI__Helpers__LiveBinder__GetValueInternal() {
  var binderInfo, liveObjects, src, propertyGetterPath, iPath, pathLength, propertyNames;
  binderInfo = this.binderInfo;
  liveObjects = this.liveObjects;
  src = liveObjects.get_item(0);
  propertyGetterPath = binderInfo.propertyGetterPath;
  iPath = 1, pathLength = propertyGetterPath.length + 1;
  propertyNames = binderInfo.propertyNames;
  this.pathTraversed = 1;
  for (; iPath < pathLength; iPath++)
    if (src !== null || iPath == 1 && (binderInfo.binderType & 2) == 2) {
      src = propertyGetterPath[iPath - 1](src);
      if (liveObjects.get_item(iPath) !== src) {
        if (liveObjects.get_item(iPath) !== null && iPath < pathLength - 1)
          System__Type__CastType(Sunlight_Framework_Observables_INotifyPropertyChanged, liveObjects.get_item(iPath)).V_RemovePropertyChangedListener_b(propertyNames[iPath], System__Delegate__Create("onSourcePropertyChanged", this));
        liveObjects.set_item(iPath, src);
        if (src !== null && iPath < pathLength - 1 && src !== null)
          System__Type__CastType(Sunlight_Framework_Observables_INotifyPropertyChanged, src).V_AddPropertyChangedListener_b(binderInfo.propertyNames[iPath], System__Delegate__Create("onSourcePropertyChanged", this));
      }
      ++this.pathTraversed;
    }
  if (this.pathTraversed < pathLength)
    return binderInfo.defaultValue;
  else if (binderInfo.forwardConverter)
    return binderInfo.forwardConverter(src);
  else
    return src;
};
ptyp_.cleanRegistrations = function Sunlight__Framework__UI__Helpers__LiveBinder__CleanRegistrations() {
  var liveObjects, iPath, till, item;
  liveObjects = this.liveObjects;
  if (this.pathTraversed < this.liveObjects.V_get_Length()) {
    liveObjects.set_item(liveObjects.V_get_Length() - 1, null);
    for (
    iPath = this.binderInfo.propertyGetterPath.length - 2, till = this.pathTraversed; iPath >= till; iPath--) {
      item = liveObjects.get_item(iPath);
      if (item !== null) {
        System__Type__CastType(Sunlight_Framework_Observables_INotifyPropertyChanged, item).V_RemovePropertyChangedListener_b(this.binderInfo.propertyNames[iPath], System__Delegate__Create("onSourcePropertyChanged", this));
        liveObjects.set_item(iPath, null);
      }
    }
  }
};
ptyp_.onSourcePropertyChanged = function Sunlight__Framework__UI__Helpers__LiveBinder__OnSourcePropertyChanged(obj, str) {
  this.flowValue();
};
ptyp_.onTargetPropertyChanged = function Sunlight__Framework__UI__Helpers__LiveBinder__OnTargetPropertyChanged(obj, str) {
  var binderInfo, target, liveObjects, value;
  if (this.updating || !this.isActive)
    return;
  try {
    binderInfo = this.binderInfo;
    target = this.target;
    liveObjects = this.liveObjects;
    this.updating = true;
    if (target == obj && this.source !== null && (liveObjects.V_get_Length() < 2 || liveObjects.get_item(liveObjects.V_get_Length() - 2) !== null)) {
      value = binderInfo.targetPropertyGetter(target);
      if (binderInfo.backwardConverter)
        value = binderInfo.backwardConverter(value);
      binderInfo.propertySetter(this.liveObjects.V_get_Length() < 2 ? this.source : this.liveObjects.get_item(this.liveObjects.V_get_Length() - 2), value);
    }
  } finally {
    this.updating = false;
  }
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Helpers_LiveBinder, "Sunlight.Framework.UI.Helpers.LiveBinder", Object, []);
function Sunlight_Framework_TaskScheduler() {
};
Sunlight_Framework_TaskScheduler.typeId = "fb";
Sunlight__Framework__TaskScheduler__instance = null;
function Sunlight__Framework__TaskScheduler__get_Instance() {
  if (!Sunlight__Framework__TaskScheduler__instance)
    Sunlight__Framework__TaskScheduler__instance = Sunlight__Framework__TaskScheduler_factory(Sunlight__Framework__WindowTimer_factory(), 16, 25);
  return Sunlight__Framework__TaskScheduler__instance;
};
function Sunlight__Framework__TaskScheduler__set_Instance(value) {
  Sunlight__Framework__TaskScheduler__instance = value;
};
function Sunlight__Framework__TaskScheduler_factory(windowTimer, workQuanta, idleTimeout) {
  var this_;
  this_ = new Sunlight_Framework_TaskScheduler();
  this_.__ctor(windowTimer, workQuanta, idleTimeout);
  return this_;
};
ptyp_ = Sunlight_Framework_TaskScheduler.prototype;
ptyp_.currentTask = null;
ptyp_.workQuanta = 0;
ptyp_.idleTimeout = 0;
ptyp_.nextTimerId = 0;
ptyp_.windowTimer = null;
ptyp_.hiPriTasks = null;
ptyp_.lowPriTasks = null;
ptyp_.idleTasks = null;
ptyp_.tasks = null;
ptyp_.timerId = 0;
ptyp_.highPriSetup = false;
ptyp_.__ctor = function Sunlight__Framework__TaskScheduler____ctor(windowTimer, workQuanta, idleTimeout) {
  this.timerId = -1;
  this.tasks = System_Collections_Generic_NumberDictionary_$Task$_.defaultConstructor();
  this.windowTimer = windowTimer;
  this.workQuanta = workQuanta;
  this.idleTimeout = idleTimeout;
  this.hiPriTasks = System_Collections_Generic_Queue_$Task$_.defaultConstructor();
  this.lowPriTasks = System_Collections_Generic_Queue_$Task$_.defaultConstructor();
  this.idleTasks = System_Collections_Generic_Queue_$Task$_.defaultConstructor();
};
ptyp_.enqueueLowPriTask = function Sunlight__Framework__TaskScheduler__EnqueueLowPriTask(work, traceId) {
  var task;
  task = Sunlight__Framework__Task_factory(this.nextTimerId++, work);
  this.idleTasks.enqueue(task);
  this.tasks.add(task.taskId, task);
  this.scheduleQuanta(false);
  return Sunlight__Framework__TaskHandle_factory(task.taskId);
};
ptyp_.runQuanta = function Sunlight__Framework__TaskScheduler__RunQuanta() {
  if (this.hiPriTasks.get_count() > 0)
    this.flushQueue(this.hiPriTasks, System__DateTime__op_Addition(System__DateTime__get_Now(), this.workQuanta));
  else if (this.idleTasks.get_count() > 0)
    this.flushQueue(this.idleTasks, System__DateTime__op_Addition(System__DateTime__get_Now(), this.workQuanta / 2 | 0));
  else if (this.lowPriTasks.get_count() > 0)
    this.flushQueue(this.lowPriTasks, System__DateTime__op_Addition(System__DateTime__get_Now(), this.workQuanta / 2 | 0));
  this.timerId = -1;
  this.scheduleQuanta(true);
};
ptyp_.flushQueue = function Sunlight__Framework__TaskScheduler__FlushQueue(taskQueue, endBy) {
  var task, now;
  while (taskQueue.get_count() > 0) {
    task = taskQueue.dequeue();
    this.executeTask(task);
    now = System__DateTime__get_Now();
    if (endBy < now)
      return;
  }
};
ptyp_.executeTask = function Sunlight__Framework__TaskScheduler__ExecuteTask(task) {
  var stmtTemp1;
  if (task.state == 0) {
    try {
      this.currentTask = task;
      task.state = 3;
      task.work();
    } catch (stmtTemp1) {
    }
    this.currentTask = null;
  }
  task.state = 2;
  this.tasks.remove(task.taskId);
};
ptyp_.scheduleQuanta = function Sunlight__Framework__TaskScheduler__ScheduleQuanta(force) {
  if (force || !this.highPriSetup && this.hiPriTasks.get_count() > 0) {
    this.windowTimer.V_ClearTimeout_e(this.timerId);
    this.timerId = -1;
  }
  if (this.timerId != -1)
    return;
  if (this.hiPriTasks.get_count() > 0) {
    this.highPriSetup = true;
    this.timerId = this.windowTimer.V_SetImmediate_e(System__Delegate__Create("runQuanta", this));
  }
  else if (this.idleTasks.get_count() + this.lowPriTasks.get_count() > 0) {
    this.highPriSetup = false;
    this.timerId = this.windowTimer.V_SetTimeout_e(System__Delegate__Create("runQuanta", this), this.idleTimeout);
  }
};
System__Type__RegisterReferenceType(Sunlight_Framework_TaskScheduler, "Sunlight.Framework.TaskScheduler", Object, []);
function Sunlight_Framework_IWindowTimer() {
};
Sunlight_Framework_IWindowTimer.typeId = "e";
System__Type__RegisterInterface(Sunlight_Framework_IWindowTimer, "Sunlight.Framework.IWindowTimer");
function Sunlight_Framework_TestWindowTimer() {
};
Sunlight_Framework_TestWindowTimer.typeId = "gb";
function Sunlight__Framework__TestWindowTimer_factory() {
  return new Sunlight_Framework_TestWindowTimer();
};
Sunlight_Framework_TestWindowTimer.defaultConstructor = Sunlight__Framework__TestWindowTimer_factory;
ptyp_ = Sunlight_Framework_TestWindowTimer.prototype;
ptyp_.setImmediate = function Sunlight__Framework__TestWindowTimer__SetImmediate(action) {
  action();
  return 0;
};
ptyp_.setTimeout = function Sunlight__Framework__TestWindowTimer__SetTimeout(action, timoutTime) {
  action();
  return 0;
};
ptyp_.clearTimeout = function Sunlight__Framework__TestWindowTimer__ClearTimeout(timeoutHandle) {
};
ptyp_.__ctor = function Sunlight__Framework__TestWindowTimer____ctor() {
};
ptyp_.V_SetImmediate_e = ptyp_.setImmediate;
ptyp_.V_SetTimeout_e = ptyp_.setTimeout;
ptyp_.V_ClearTimeout_e = ptyp_.clearTimeout;
System__Type__RegisterReferenceType(Sunlight_Framework_TestWindowTimer, "Sunlight.Framework.TestWindowTimer", Object, [Sunlight_Framework_IWindowTimer]);
function Sunlight_Framework_UI_Test_NScriptsTemplatesClass() {
};
Sunlight_Framework_UI_Test_NScriptsTemplatesClass.typeId = "hb";
function Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplate1() {
  return Test$5cTemplates$5cTestTemplate1();
};
function Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplateVMB1() {
  return Test$5cTemplates$5cTestTemplateVMB1();
};
function Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplateVMB_AttrBinding() {
  return Test$5cTemplates$5cTestTemplateVMB_AttrBinding();
};
function Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplateVMB_CssBinding() {
  return Test$5cTemplates$5cTestTemplateVMB_CssBinding();
};
function Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplateVMB_StyleBinding() {
  return Test$5cTemplates$5cTestTemplateVMB_StyleBinding();
};
function Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplateB_PropertyBinding() {
  return Test$5cTemplates$5cTestTemplateB_PropertyBinding();
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_NScriptsTemplatesClass, "Sunlight.Framework.UI.Test.NScriptsTemplatesClass", Object, []);
function Sunlight_Framework_Observables_ExtensibleObservableObject() {
};
Sunlight_Framework_Observables_ExtensibleObservableObject.typeId = "ib";
function Sunlight__Framework__Observables__ExtensibleObservableObject_factory() {
  var this_;
  this_ = new Sunlight_Framework_Observables_ExtensibleObservableObject();
  this_.__ctor0();
  return this_;
};
Sunlight_Framework_Observables_ExtensibleObservableObject.defaultConstructor = Sunlight__Framework__Observables__ExtensibleObservableObject_factory;
ptyp_ = new Sunlight_Framework_Observables_ObservableObject();
Sunlight_Framework_Observables_ExtensibleObservableObject.prototype = ptyp_;
ptyp_.propertyMap = null;
ptyp_.__ctor0 = function Sunlight__Framework__Observables__ExtensibleObservableObject____ctor() {
  this.__ctor();
  this.propertyMap = {
  };
};
System__Type__RegisterReferenceType(Sunlight_Framework_Observables_ExtensibleObservableObject, "Sunlight.Framework.Observables.ExtensibleObservableObject", Sunlight_Framework_Observables_ObservableObject, []);
function Sunlight_Framework_Binders_ContextBindableObject() {
};
Sunlight_Framework_Binders_ContextBindableObject.typeId = "jb";
function Sunlight__Framework__Binders__ContextBindableObject_factory() {
  var this_;
  this_ = new Sunlight_Framework_Binders_ContextBindableObject();
  this_.__ctor0();
  return this_;
};
Sunlight_Framework_Binders_ContextBindableObject.defaultConstructor = Sunlight__Framework__Binders__ContextBindableObject_factory;
ptyp_ = new Sunlight_Framework_Observables_ExtensibleObservableObject();
Sunlight_Framework_Binders_ContextBindableObject.prototype = ptyp_;
ptyp_.parent = null;
ptyp_.dataContext = null;
ptyp_.dataContextSetterCalled = false;
ptyp_.isActive = false;
ptyp_.isPreActivated = false;
ptyp_.isActivated = false;
ptyp_.isDisposing = false;
ptyp_.isDisposed = false;
ptyp_.onDisposed = null;
ptyp_.isInactiveIfNullContext = false;
ptyp_.set_parent = function Sunlight__Framework__Binders__ContextBindableObject__set_Parent(value) {
  if (this.parent != value) {
    if (this.parent) {
      this.parent.removePropertyChangedListener("DataContext", System__Delegate__Create("onParentDataContextUpdated", this));
      this.parent.removePropertyChangedListener("IsActive", System__Delegate__Create("onParentDataContextUpdated", this));
    }
    this.parent = value;
    if (!this.dataContextSetterCalled)
      if (this.parent) {
        this.parent.addPropertyChangedListener("DataContext", System__Delegate__Create("onParentDataContextUpdated", this));
        this.parent.addPropertyChangedListener("IsActive", System__Delegate__Create("onParentDataContextUpdated", this));
        this.onParentDataContextUpdated(null, null);
      }
      else
        this.setDataContext(null);
  }
};
ptyp_.get_dataContext = function Sunlight__Framework__Binders__ContextBindableObject__get_DataContext() {
  return this.dataContext;
};
ptyp_.set_dataContext = function Sunlight__Framework__Binders__ContextBindableObject__set_DataContext(value) {
  this.dataContextSetterCalled = true;
  this.setDataContext(value);
};
ptyp_.get_isActive = function Sunlight__Framework__Binders__ContextBindableObject__get_IsActive() {
  return this.isActivated && !this.V_get_ActivationBlocked();
};
ptyp_.set_inactiveIfNullContext = function Sunlight__Framework__Binders__ContextBindableObject__set_InactiveIfNullContext(value) {
  this.isInactiveIfNullContext = value;
  this.fixActivation();
};
ptyp_.get_activationBlocked = function Sunlight__Framework__Binders__ContextBindableObject__get_ActivationBlocked() {
  return this.isInactiveIfNullContext && this.dataContext === null;
};
ptyp_.dispose = function Sunlight__Framework__Binders__ContextBindableObject__Dispose() {
  if (!this.isDisposed && !this.isDisposing)
    this.V_InternalDispose();
};
ptyp_.activate = function Sunlight__Framework__Binders__ContextBindableObject__Activate() {
  this.isActive = true;
  this.fixActivation();
};
ptyp_.deactivate = function Sunlight__Framework__Binders__ContextBindableObject__Deactivate() {
  if (!this.isActive)
    return;
  this.isActive = false;
  this.fixActivation();
};
ptyp_.onBeforeFirstActivate = function Sunlight__Framework__Binders__ContextBindableObject__OnBeforeFirstActivate() {
};
ptyp_.onActivate = function Sunlight__Framework__Binders__ContextBindableObject__OnActivate() {
};
ptyp_.onDeactivate = function Sunlight__Framework__Binders__ContextBindableObject__OnDeactivate() {
};
ptyp_.fixActivation = function Sunlight__Framework__Binders__ContextBindableObject__FixActivation() {
  if (!this.V_get_ActivationBlocked() && this.isActive) {
    if (!this.isPreActivated) {
      this.isPreActivated = true;
      this.V_OnBeforeFirstActivate();
    }
    if (!this.isActivated) {
      this.isActivated = true;
      this.V_OnActivate();
      this.firePropertyChanged("IsActive");
    }
  }
  else if (this.isActivated) {
    this.isActivated = false;
    this.V_OnDeactivate();
    this.firePropertyChanged("IsActive");
  }
};
ptyp_.internalDispose = function Sunlight__Framework__Binders__ContextBindableObject__InternalDispose() {
  if (this.onDisposed) {
    this.set_parent(null);
    this.clearListeners();
    this.onDisposed();
  }
};
ptyp_.onDataContextUpdating = function Sunlight__Framework__Binders__ContextBindableObject__OnDataContextUpdating(newValue) {
};
ptyp_.onDataContextUpdated = function Sunlight__Framework__Binders__ContextBindableObject__OnDataContextUpdated(oldValue) {
};
ptyp_.setDataContext = function Sunlight__Framework__Binders__ContextBindableObject__SetDataContext(value) {
  var oldValue;
  if (this.dataContext !== value) {
    this.V_OnDataContextUpdating(value);
    oldValue = this.dataContext;
    this.dataContext = value;
    this.V_OnDataContextUpdated(oldValue);
    this.firePropertyChanged("DataContext");
    this.fixActivation();
  }
};
ptyp_.onParentDataContextUpdated = function Sunlight__Framework__Binders__ContextBindableObject__OnParentDataContextUpdated(sender, propertyName) {
  if (this.parent.get_isActive() && !this.dataContextSetterCalled)
    this.setDataContext(this.parent.get_dataContext());
  if (propertyName === "IsActive" || !propertyName)
    if (this.parent.get_isActive())
      this.activate();
    else
      this.deactivate();
};
ptyp_.__ctor0 = function Sunlight__Framework__Binders__ContextBindableObject____ctor() {
  this.__ctor0();
  this.isInactiveIfNullContext = true;
};
ptyp_.V_get_ActivationBlocked = ptyp_.get_activationBlocked;
ptyp_.V_OnBeforeFirstActivate = ptyp_.onBeforeFirstActivate;
ptyp_.V_OnActivate = ptyp_.onActivate;
ptyp_.V_OnDeactivate = ptyp_.onDeactivate;
ptyp_.V_InternalDispose = ptyp_.internalDispose;
ptyp_.V_OnDataContextUpdating = ptyp_.onDataContextUpdating;
ptyp_.V_OnDataContextUpdated = ptyp_.onDataContextUpdated;
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_ContextBindableObject, "Sunlight.Framework.Binders.ContextBindableObject", Sunlight_Framework_Observables_ExtensibleObservableObject, []);
function Sunlight_Framework_UI_UIElement() {
};
Sunlight_Framework_UI_UIElement.typeId = "kb";
function Sunlight__Framework__UI__UIElement_factory(element) {
  var this_;
  this_ = new Sunlight_Framework_UI_UIElement();
  this_.__ctor0(element);
  return this_;
};
ptyp_ = new Sunlight_Framework_Binders_ContextBindableObject();
Sunlight_Framework_UI_UIElement.prototype = ptyp_;
ptyp_.element = null;
ptyp_.isHidden = false;
ptyp_.eventRegistrationDict = null;
ptyp_.__ctor0 = function Sunlight__Framework__UI__UIElement____ctor(element) {
  this.__ctor0();
  this.eventRegistrationDict = System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_.defaultConstructor();
  this.element = element;
};
ptyp_.get_element = function Sunlight__Framework__UI__UIElement__get_Element() {
  return this.element;
};
ptyp_.get_activationBlocked0 = function Sunlight__Framework__UI__UIElement__get_ActivationBlocked() {
  return this.get_activationBlocked() || this.isHidden;
};
ptyp_.internalDispose0 = function Sunlight__Framework__UI__UIElement__InternalDispose() {
  var stmtTemp1, kvPair;
  for (stmtTemp1 = this.eventRegistrationDict.V_GetEnumerator_f(); stmtTemp1.V_MoveNext_g(); ) {
    kvPair = System__Type__UnBoxTypeInstance(System_Collections_Generic_KeyValuePair_$String_x_Action_$UIEvent$_$_, stmtTemp1.V_get_Current_g());
    System__Web__Html__Element__UnBind0(this.element, System_Collections_Generic_KeyValuePair_$String_x_Action_$UIEvent$_$_.get_key(kvPair));
  }
  this.eventRegistrationDict.clear();
  this.internalDispose();
};
ptyp_.V_get_ActivationBlocked = ptyp_.get_activationBlocked0;
ptyp_.V_InternalDispose = ptyp_.internalDispose0;
System__Type__RegisterReferenceType(Sunlight_Framework_UI_UIElement, "Sunlight.Framework.UI.UIElement", Sunlight_Framework_Binders_ContextBindableObject, []);
function Sunlight_Framework_UI_UISkinableElement() {
};
Sunlight_Framework_UI_UISkinableElement.typeId = "lb";
function Sunlight__Framework__UI__UISkinableElement_factory(element) {
  var this_;
  this_ = new Sunlight_Framework_UI_UISkinableElement();
  this_.__ctor0(element);
  return this_;
};
ptyp_ = new Sunlight_Framework_UI_UIElement();
Sunlight_Framework_UI_UISkinableElement.prototype = ptyp_;
ptyp_.skin = null;
ptyp_.skinInstance = null;
ptyp_.__ctor0 = function Sunlight__Framework__UI__UISkinableElement____ctor(element) {
  this.__ctor0(element);
};
ptyp_.set_skin = function Sunlight__Framework__UI__UISkinableElement__set_Skin(value) {
  if (this.skin != value) {
    this.skin = value;
    if (this.skin && this.get_isActive())
      this.set_skinInstance(this.skin.createInstance());
    this.firePropertyChanged("Skin");
  }
};
ptyp_.get_skinInstance = function Sunlight__Framework__UI__UISkinableElement__get_SkinInstance() {
  return this.skinInstance;
};
ptyp_.set_skinInstance = function Sunlight__Framework__UI__UISkinableElement__set_SkinInstance(value) {
  if (this.skinInstance != value) {
    if (this.skinInstance)
      this.skinInstance.dispose();
    this.skinInstance = value;
    if (this.skinInstance) {
      this.skinInstance.bind(this);
      if (this.get_isActive())
        this.skinInstance.activate();
      this.V_ApplySkinInternal(this.skinInstance);
    }
    this.firePropertyChanged("SkinInstance");
  }
};
ptyp_.applySkinInternal = function Sunlight__Framework__UI__UISkinableElement__ApplySkinInternal(skin) {
};
ptyp_.onBeforeFirstActivate0 = function Sunlight__Framework__UI__UISkinableElement__OnBeforeFirstActivate() {
  this.onBeforeFirstActivate();
  if (this.skin && !this.skinInstance)
    this.set_skinInstance(this.skin.createInstance());
};
ptyp_.onActivate0 = function Sunlight__Framework__UI__UISkinableElement__OnActivate() {
  this.onActivate();
  if (this.skin && !this.skinInstance)
    this.set_skinInstance(this.skin.createInstance());
  else if (this.skinInstance)
    this.skinInstance.activate();
};
ptyp_.onDeactivate0 = function Sunlight__Framework__UI__UISkinableElement__OnDeactivate() {
  if (this.skinInstance)
    this.skinInstance.deactivate();
  this.onDeactivate();
};
ptyp_.internalDispose0 = function Sunlight__Framework__UI__UISkinableElement__InternalDispose() {
  if (this.get_skinInstance())
    this.set_skinInstance(null);
  this.set_skin(null);
  this.internalDispose0();
};
ptyp_.onDataContextUpdated0 = function Sunlight__Framework__UI__UISkinableElement__OnDataContextUpdated(oldValue) {
  this.onDataContextUpdated(oldValue);
  if (this.skinInstance)
    this.skinInstance.updateDataContext();
};
ptyp_.V_OnBeforeFirstActivate = ptyp_.onBeforeFirstActivate0;
ptyp_.V_OnActivate = ptyp_.onActivate0;
ptyp_.V_OnDeactivate = ptyp_.onDeactivate0;
ptyp_.V_InternalDispose = ptyp_.internalDispose0;
ptyp_.V_OnDataContextUpdated = ptyp_.onDataContextUpdated0;
ptyp_.V_ApplySkinInternal = ptyp_.applySkinInternal;
System__Type__RegisterReferenceType(Sunlight_Framework_UI_UISkinableElement, "Sunlight.Framework.UI.UISkinableElement", Sunlight_Framework_UI_UIElement, []);
function System__Web__Html__Node__Remove(this_) {
  return this_.parentNode ? this_.parentNode.removeChild(this_) : this_;
};
function System__Web__Html__Element__AddClassName(this_, className) {
  var index;
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  if (!System__Object__IsNullOrUndefined(this_.classList)) {
    this_.classList.add(className);
    return;
  }
  if (className === null || (className = System__String__Trim(className)).length == 0)
    return;
  if (this_.className === null || this_.className.length == 0) {
    this_.className = className;
    return;
  }
  index = 0;
  while ((index = this_.className.indexOf(className, index)) != -1) {
    if ((index == 0 || System__String__get_Item(this_.className, index - 1) == 32) && (index == this_.className.length - className.length || System__String__get_Item(this_.className, index + className.length) == 32))
      return;
    index++;
  }
  this_.className = this_.className + " " + className;
  return;
};
function System__Web__Html__Element__RemoveClassName(this_, className) {
  var index;
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  if (!System__Object__IsNullOrUndefined(this_.classList)) {
    this_.classList.remove(className);
    return;
  }
  if (className === null || (className = System__String__Trim(className)).length == 0 || this_.className === null || this_.className.length == 0)
    return;
  index = 0;
  while ((index = this_.className.indexOf(className, index)) != -1) {
    if ((index == 0 || System__String__get_Item(this_.className, index - 1) == 32) && (index == this_.className.length - className.length || System__String__get_Item(this_.className, index + className.length) == 32)) {
      this_.className = this_.className.substr(0, index > 0 ? index - 1 : 0) + this_.className.substring(index + className.length);
      return;
    }
    index++;
  }
  return;
};
function System__Web__Html__Element__Bind(this_, eventName, handler, capture) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  System__EventBinder__AddEvent(this_, eventName, handler, capture);
};
function System__Web__Html__Element__UnBind(this_, eventName, handler, capture) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  System__EventBinder__RemoveEvent(this_, eventName, handler, capture);
};
function System__Web__Html__Element__UnBind0(this_, eventName) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  System__EventBinder__RemoveEvent0(this_, eventName, true);
  System__EventBinder__RemoveEvent0(this_, eventName, false);
};
function Sunlight_Framework_UI_Test_TestViewModelB() {
};
Sunlight_Framework_UI_Test_TestViewModelB.typeId = "mb";
function Sunlight__Framework__UI__Test__TestViewModelB_factory() {
  var this_;
  this_ = new Sunlight_Framework_UI_Test_TestViewModelB();
  this_.__ctor0();
  return this_;
};
Sunlight_Framework_UI_Test_TestViewModelB.defaultConstructor = Sunlight__Framework__UI__Test__TestViewModelB_factory;
ptyp_ = new Sunlight_Framework_UI_Test_TestViewModelA();
Sunlight_Framework_UI_Test_TestViewModelB.prototype = ptyp_;
ptyp_.__ctor0 = function Sunlight__Framework__UI__Test__TestViewModelB____ctor() {
  this.__ctor0();
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_TestViewModelB, "Sunlight.Framework.UI.Test.TestViewModelB", Sunlight_Framework_UI_Test_TestViewModelA, []);
function Sunlight_Framework_UI_Test_TestSkinableWithTestUIElementPart() {
};
Sunlight_Framework_UI_Test_TestSkinableWithTestUIElementPart.typeId = "nb";
function Sunlight__Framework__UI__Test__TestSkinableWithTestUIElementPart_factory(element) {
  var this_;
  this_ = new Sunlight_Framework_UI_Test_TestSkinableWithTestUIElementPart();
  this_.__ctor0(element);
  return this_;
};
ptyp_ = new Sunlight_Framework_UI_UISkinableElement();
Sunlight_Framework_UI_Test_TestSkinableWithTestUIElementPart.prototype = ptyp_;
ptyp_.__ctor0 = function Sunlight__Framework__UI__Test__TestSkinableWithTestUIElementPart____ctor(element) {
  this.__ctor0(element);
};
ptyp_.get_part = function Sunlight__Framework__UI__Test__TestSkinableWithTestUIElementPart__get_Part() {
  if (this.get_skinInstance())
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestUIElement, this.get_skinInstance().getChildById("Part1"));
  return null;
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_TestSkinableWithTestUIElementPart, "Sunlight.Framework.UI.Test.TestSkinableWithTestUIElementPart", Sunlight_Framework_UI_UISkinableElement, []);
function Sunlight_Framework_UI_Test_TestUIElement() {
};
Sunlight_Framework_UI_Test_TestUIElement.typeId = "ob";
function Sunlight__Framework__UI__Test__TestUIElement_factory(element) {
  var this_;
  this_ = new Sunlight_Framework_UI_Test_TestUIElement();
  this_.__ctor0(element);
  return this_;
};
ptyp_ = new Sunlight_Framework_UI_UIElement();
Sunlight_Framework_UI_Test_TestUIElement.prototype = ptyp_;
ptyp_.twoWayLooseBinding = 0;
ptyp_.__ctor0 = function Sunlight__Framework__UI__Test__TestUIElement____ctor(element) {
  this.__ctor0(element);
};
ptyp_.get_oneWayStrictBinding = function Sunlight__Framework__UI__Test__TestUIElement__get_OneWayStrictBinding() {
  return this.OneWayStrictBinding;
};
ptyp_.set_oneWayStrictBinding = function Sunlight__Framework__UI__Test__TestUIElement__set_OneWayStrictBinding(value) {
  return this.OneWayStrictBinding = value;
};
ptyp_.get_twoWayLooseBinding = function Sunlight__Framework__UI__Test__TestUIElement__get_TwoWayLooseBinding() {
  return this.twoWayLooseBinding;
};
ptyp_.set_twoWayLooseBinding = function Sunlight__Framework__UI__Test__TestUIElement__set_TwoWayLooseBinding(value) {
  if (this.twoWayLooseBinding != value) {
    this.twoWayLooseBinding = value;
    this.firePropertyChanged("TwoWayLooseBinding");
  }
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_TestUIElement, "Sunlight.Framework.UI.Test.TestUIElement", Sunlight_Framework_UI_UIElement, []);
function Sunlight_Framework_UI_Helpers_SkinBinderHelper() {
};
function Sunlight__Framework__UI__Helpers__SkinBinderHelper__Bind(binders, dataContext, targetElements) {
  var i, j, info;
  for (
  i = 0, j = binders.length; i < j; i++) {
    info = binders[i];
    Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetPropertyValue(info, dataContext, targetElements[info.objectIndex], null);
  }
};
function Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetAttribute(node, value, attrName) {
  if (value !== null)
    node.setAttribute(attrName, value);
  else
    node.removeAttribute(attrName);
};
function Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent(element, value) {
  if (value !== null)
    element.textContent = value;
  else
    element.textContent = "";
};
function Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetDataContext(element, value) {
  element.set_dataContext(value);
};
function Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetCssClass(element, add, className) {
  if (add)
    System__Web__Html__Element__AddClassName(element, className);
  else
    System__Web__Html__Element__RemoveClassName(element, className);
};
function Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(element, path) {
  var iPath;
  for (iPath = 0; iPath < path.length; iPath++)
    element = element.childNodes[path[iPath]];
  return element;
};
function Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetPropertyValue(binder, source, target, extraElementArray) {
  var stmtTemp1, stmtTemp10;
  try {
    source = Sunlight__Framework__UI__Helpers__SkinBinderHelper__TraversePropertyPath(binder, source);
  } catch (stmtTemp1) {
    source = binder.defaultValue;
  }
  try {
    binder.setTargetValue(target, source, extraElementArray);
  } catch (stmtTemp10) {
  }
};
function Sunlight__Framework__UI__Helpers__SkinBinderHelper__TraversePropertyPath(binder, source) {
  var iGetter, pathLength;
  for (
  iGetter = 0, pathLength = binder.propertyGetterPath.length; iGetter < pathLength; iGetter++)
    source = binder.propertyGetterPath[iGetter](source);
  if (binder.forwardConverter)
    source = binder.forwardConverter(source);
  return source;
};
function System_Boolean(boxedValue) {
  this.boxedValue = boxedValue;
};
System_Boolean.typeId = "pb";
System_Boolean.getDefaultValue = function() {
  return false;
};
function System__Boolean__ToString(this_) {
  return this_ ? "true" : "false";
};
ptyp_ = new System_ValueType();
System_Boolean.prototype = ptyp_;
ptyp_.toString = function() {
  return System__Boolean__ToString(this.boxedValue);
};
System__Type__RegisterStructType(System_Boolean, "System.Boolean", []);
function Sunlight_Framework_UI_ListView() {
};
Sunlight_Framework_UI_ListView.typeId = "qb";
function Sunlight__Framework__UI__ListView_factory(element) {
  var this_;
  this_ = new Sunlight_Framework_UI_ListView();
  this_.__ctor0(element);
  return this_;
};
ptyp_ = new Sunlight_Framework_UI_UIElement();
Sunlight_Framework_UI_ListView.prototype = ptyp_;
ptyp_.items = null;
ptyp_.observableList = null;
ptyp_.attachedObservableList = null;
ptyp_.fixedList = null;
ptyp_.itemSkin = null;
ptyp_.itemCssClassName = null;
ptyp_.inlineItems = false;
ptyp_.__ctor0 = function Sunlight__Framework__UI__ListView____ctor(element) {
  this.__ctor0(element);
  this.items = System_Collections_Generic_List_$ListViewItem$_.defaultConstructor();
};
ptyp_.set_fixedList = function Sunlight__Framework__UI__ListView__set_FixedList(value) {
  if (value && this.observableList)
    throw new Error("Can't set FixedList and ObservableList at the same time");
  if (this.fixedList != value) {
    this.fixedList = value;
    this.firePropertyChanged("FixedList");
    this.applyFixedList();
  }
};
ptyp_.set_itemSkin = function Sunlight__Framework__UI__ListView__set_ItemSkin(value) {
  var items, itemCount, iItem;
  if (this.itemSkin != value) {
    this.itemSkin = value;
    this.firePropertyChanged("ItemSkin");
    if (this.get_isActive()) {
      items = this.items;
      itemCount = items.get_count();
      for (iItem = 0; iItem < itemCount; iItem++)
        items.get_item(iItem).set_skin(this.itemSkin);
    }
  }
};
ptyp_.onActivate0 = function Sunlight__Framework__UI__ListView__OnActivate() {
  this.onActivate();
  if (this.fixedList)
    this.applyFixedList();
  else if (this.observableList)
    this.applyObservableList();
};
ptyp_.onDeactivate0 = function Sunlight__Framework__UI__ListView__OnDeactivate() {
  var items, itemCount, iItem;
  items = this.items;
  itemCount = items.get_count();
  if (itemCount > 0)
    for (iItem = 0; iItem < itemCount; iItem++)
      items.get_item(iItem).deactivate();
  this.onDeactivate();
};
ptyp_.internalDispose0 = function Sunlight__Framework__UI__ListView__InternalDispose() {
  var items, itemCount, iItem;
  items = this.items;
  itemCount = items.get_count();
  if (itemCount > 0) {
    for (iItem = 0; iItem < itemCount; iItem++)
      items.get_item(iItem).dispose();
    items.clear();
  }
  this.internalDispose0();
};
ptyp_.applyFixedList = function Sunlight__Framework__UI__ListView__ApplyFixedList() {
  var items, itemsCount, iItem, item, fixedList, fixedListCount, iObject, listViewItem;
  items = this.items;
  itemsCount = items.get_count();
  if (!this.fixedList) {
    for (iItem = 0; iItem < itemsCount; iItem++) {
      item = items.get_item(iItem);
      item.dispose();
      System__Web__Html__Node__Remove(item.get_element());
    }
    items.clear();
    return;
  }
  if (this.get_isActive()) {
    fixedList = this.fixedList;
    fixedListCount = fixedList.V_get_Count_c();
    for (iObject = 0; iObject < fixedListCount; iObject++) {
      if (iObject < itemsCount)
        listViewItem = items.get_item(iObject);
      else {
        listViewItem = Sunlight__Framework__UI__ListViewItem_factory(this.get_element().ownerDocument.createElement("li"));
        if (this.itemCssClassName !== null)
          listViewItem.get_element().className = this.itemCssClassName;
        if (!this.inlineItems)
          this.get_element().appendChild(listViewItem.get_element());
        else
          this.get_element().parentNode.insertBefore(listViewItem.get_element(), this.get_element());
        listViewItem.set_skin(this.itemSkin);
        items.add(listViewItem);
      }
      listViewItem.set_dataContext(fixedList.V_get_Item_d(iObject));
      listViewItem.activate();
    }
    this.removeChildren(fixedListCount, itemsCount - fixedListCount);
  }
};
ptyp_.applyObservableList = function Sunlight__Framework__UI__ListView__ApplyObservableList() {
  var items, itemsCount, iItem, item;
  items = this.items;
  itemsCount = items.get_count();
  if (!this.observableList) {
    for (iItem = 0; iItem < itemsCount; iItem++) {
      item = items.get_item(iItem);
      item.dispose();
      System__Web__Html__Node__Remove(item.get_element());
    }
    items.clear();
    return;
  }
  if (this.get_isActive() && this.observableList && this.observableList != this.attachedObservableList) {
    this.attachedObservableList = this.observableList;
    this.attachedObservableList.V_add_CollectionChanged_h(System__Delegate__Create("observableListCollectionChanged", this));
    this.resetObservableItems();
  }
};
ptyp_.observableListCollectionChanged = function Sunlight__Framework__UI__ListView__ObservableListCollectionChanged(collection, args) {
  var items, changeIndex, list, listCount, insertBeforeElem, iObject, listViewItem;
  System__Diagnostics__Debug__Assert(collection == this.attachedObservableList);
  items = this.items;
  changeIndex = args.V_get_ChangeIndex_i();
  switch(args.V_get_Action_i()) {
    case 0: {
      list = args.V_get_NewItems_i();
      listCount = list.V_get_Count_c();
      insertBeforeElem = null;
      if (changeIndex < items.get_count())
        insertBeforeElem = items.get_item(changeIndex).get_element();
      for (iObject = 0; iObject < listCount; iObject++) {
        listViewItem = Sunlight__Framework__UI__ListViewItem_factory(this.get_element().ownerDocument.createElement("div"));
        if (this.itemCssClassName !== null)
          listViewItem.get_element().className = this.itemCssClassName;
        listViewItem.set_skin(this.itemSkin);
        if (!insertBeforeElem) {
          if (this.inlineItems)
            this.get_element().parentNode.insertBefore(listViewItem.get_element(), this.get_element());
          else
            this.get_element().appendChild(listViewItem.get_element());
          items.add(listViewItem);
        }
        else {
          if (this.inlineItems)
            this.get_element().parentNode.insertBefore(listViewItem.get_element(), insertBeforeElem);
          else
            this.get_element().insertBefore(listViewItem.get_element(), insertBeforeElem);
          items.insert(changeIndex + iObject, listViewItem);
        }
        listViewItem.set_dataContext(list.V_get_Item_d(iObject));
        listViewItem.activate();
      }
      break;
    }
    case 1: {
      this.removeChildren(changeIndex, args.V_get_OldItems_i().V_get_Count_c());
      break;
    }
    case 2: {
      list = args.V_get_NewItems_i();
      listCount = list.V_get_Count_c();
      for (iObject = 0; iObject < listCount; iObject++)
        items.get_item(changeIndex + iObject).set_dataContext(list.V_get_Item_d(iObject));
      break;
    }
    case 4: {
      this.resetObservableItems();
      break;
    }
    default:
    throw new Error("Invalid operation");
  }
};
ptyp_.resetObservableItems = function Sunlight__Framework__UI__ListView__ResetObservableItems() {
  var observableList, itemsCount, listCount, iObject, listViewItem;
  observableList = this.observableList;
  itemsCount = this.items.get_count();
  listCount = observableList.V_get_Count_j();
  for (iObject = 0; iObject < listCount; iObject++) {
    if (iObject < itemsCount)
      listViewItem = this.items.get_item(iObject);
    else {
      listViewItem = Sunlight__Framework__UI__ListViewItem_factory(this.get_element().ownerDocument.createElement("li"));
      if (this.itemCssClassName !== null)
        listViewItem.get_element().className = this.itemCssClassName;
      if (!this.inlineItems)
        this.get_element().appendChild(listViewItem.get_element());
      else
        this.get_element().parentNode.insertBefore(listViewItem.get_element(), this.get_element());
      listViewItem.set_skin(this.itemSkin);
      this.items.add(listViewItem);
    }
    listViewItem.set_dataContext(observableList.V_get_Item_j(iObject));
    listViewItem.activate();
  }
  this.removeChildren(listCount, itemsCount - listCount);
};
ptyp_.removeChildren = function Sunlight__Framework__UI__ListView__RemoveChildren(changeIndex, delCount) {
  var iObject, item;
  for (iObject = delCount + changeIndex - 1; iObject >= changeIndex; iObject--) {
    item = this.items.get_item(iObject);
    this.items.removeAt(iObject);
    System__Web__Html__Node__Remove(item.get_element());
    item.dispose();
  }
};
ptyp_.V_OnActivate = ptyp_.onActivate0;
ptyp_.V_OnDeactivate = ptyp_.onDeactivate0;
ptyp_.V_InternalDispose = ptyp_.internalDispose0;
System__Type__RegisterReferenceType(Sunlight_Framework_UI_ListView, "Sunlight.Framework.UI.ListView", Sunlight_Framework_UI_UIElement, []);
function Sunlight_Framework_Task() {
};
Sunlight_Framework_Task.typeId = "rb";
function Sunlight__Framework__Task_factory(taskId, work) {
  var this_;
  this_ = new Sunlight_Framework_Task();
  this_.__ctor(taskId, work);
  return this_;
};
ptyp_ = Sunlight_Framework_Task.prototype;
ptyp_.state = 0;
ptyp_.work = null;
ptyp_.taskId = null;
ptyp_.__ctor = function Sunlight__Framework__Task____ctor(taskId, work) {
  this.taskId = taskId;
  this.work = work;
};
System__Type__RegisterReferenceType(Sunlight_Framework_Task, "Sunlight.Framework.Task", Object, []);
function Sunlight_Framework_UI_Skin() {
};
Sunlight_Framework_UI_Skin.typeId = "sb";
function Sunlight__Framework__UI__Skin_factory(skinableType, dataContextType, factoryMethod, id) {
  var this_;
  this_ = new Sunlight_Framework_UI_Skin();
  this_.__ctor(skinableType, dataContextType, factoryMethod, id);
  return this_;
};
ptyp_ = Sunlight_Framework_UI_Skin.prototype;
ptyp_.factoryMethod = null;
ptyp_.skinableType = null;
ptyp_.dataContextType = null;
ptyp_.id = null;
ptyp_.__ctor = function Sunlight__Framework__UI__Skin____ctor(skinableType, dataContextType, factoryMethod, id) {
  this.factoryMethod = factoryMethod;
  this.skinableType = skinableType;
  this.dataContextType = dataContextType;
  this.id = id;
};
ptyp_.get_skinableType = function Sunlight__Framework__UI__Skin__get_SkinableType() {
  return this.skinableType;
};
ptyp_.createInstance = function Sunlight__Framework__UI__Skin__CreateInstance() {
  return this.factoryMethod(this, window.document);
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Skin, "Sunlight.Framework.UI.Skin", Object, []);
function Sunlight_Framework_UI_Helpers_SkinInstance() {
};
Sunlight_Framework_UI_Helpers_SkinInstance.typeId = "tb";
function Sunlight__Framework__UI__Helpers__SkinInstance_factory(factory, rootElement, childElements, elementsOfIntrests, binders, partIdMapping, liveBinderCount, extraObjectCount) {
  var this_;
  this_ = new Sunlight_Framework_UI_Helpers_SkinInstance();
  this_.__ctor(factory, rootElement, childElements, elementsOfIntrests, binders, partIdMapping, liveBinderCount, extraObjectCount);
  return this_;
};
ptyp_ = Sunlight_Framework_UI_Helpers_SkinInstance.prototype;
ptyp_.parentFactory = null;
ptyp_.elementsOfIntrest = null;
ptyp_.childElements = null;
ptyp_.rootElement = null;
ptyp_.isActive = false;
ptyp_.isDiposed = false;
ptyp_.binders = null;
ptyp_.liveBinders = null;
ptyp_.hasDataContextBinding = null;
ptyp_.extraObjects = null;
ptyp_.partIdMapping = null;
ptyp_.skinableParent = null;
ptyp_.dataContext = null;
ptyp_.firstActivationDone = false;
ptyp_.dataContextUpdated = false;
ptyp_.templateParentUpdated = false;
ptyp_.__ctor = function Sunlight__Framework__UI__Helpers__SkinInstance____ctor(factory, rootElement, childElements, elementsOfIntrests, binders, partIdMapping, liveBinderCount, extraObjectCount) {
  System__Object__IsNullOrUndefined(rootElement);
  this.parentFactory = factory;
  this.rootElement = rootElement;
  this.binders = binders;
  this.childElements = childElements;
  this.elementsOfIntrest = elementsOfIntrests;
  this.dataContextUpdated = true;
  this.templateParentUpdated = true;
  this.hasDataContextBinding = new Array(this.elementsOfIntrest.length);
  if (liveBinderCount > 0)
    this.liveBinders = new Array(liveBinderCount);
  if (extraObjectCount > 0)
    this.extraObjects = new Array(extraObjectCount);
  if (partIdMapping !== null)
    this.partIdMapping = System_Collections_Generic_StringDictionary_$Int32$_.__ctor(partIdMapping);
};
ptyp_.getChildById = function Sunlight__Framework__UI__Helpers__SkinInstance__GetChildById(id) {
  if (this.partIdMapping && this.partIdMapping.containsKey(id))
    return this.elementsOfIntrest[this.partIdMapping.get_item(id)];
  return null;
};
ptyp_.bind = function Sunlight__Framework__UI__Helpers__SkinInstance__Bind(skinable) {
  var childNodes, skinableElement;
  if (!this.rootElement || this.isDiposed)
    throw new Error("InvalidOperation, Skin already applied");
  if (!this.parentFactory.get_skinableType().isInstanceOfType(skinable))
    throw new Error("Skin being applied to wrong Skinable");
  if (this.skinableParent == skinable)
    return;
  if (this.skinableParent) {
    childNodes = this.skinableParent.get_element().childNodes;
    while (childNodes.length > 0)
      this.rootElement.appendChild(childNodes[0]);
  }
  this.skinableParent = skinable;
  if (this.skinableParent) {
    childNodes = this.rootElement.childNodes;
    skinableElement = skinable.get_element();
    while (childNodes.length > 0)
      skinableElement.appendChild(childNodes[0]);
  }
  if (this.isActive && !this.isDiposed)
    this.updateBinderSource(skinable, 3);
  else
    this.templateParentUpdated = true;
  this.updateDataContext();
};
ptyp_.updateDataContext = function Sunlight__Framework__UI__Helpers__SkinInstance__UpdateDataContext() {
  if (this.skinableParent)
    if (this.skinableParent.get_dataContext() !== this.dataContext) {
      this.dataContext = this.skinableParent.get_dataContext();
      this.dataContextUpdated = true;
    }
  else if (this.dataContext !== null) {
    this.dataContext = null;
    this.dataContextUpdated = true;
  }
  if (this.dataContextUpdated && this.isActive && !this.isDiposed) {
    this.updateBinderSource(this.dataContext, 1);
    this.dataContextUpdated = false;
  }
};
ptyp_.activate = function Sunlight__Framework__UI__Helpers__SkinInstance__Activate() {
  var childElements, binders, childElementLength, elementsOfIntrest, binderLength, dataContext, dataContextSetter, iBinder, iLiveBinder, binder, source, liveBinder, iChild, objectIndex, childElement;
  if (!this.isActive && !this.isDiposed) {
    this.isActive = true;
    childElements = this.childElements;
    binders = this.binders;
    childElementLength = childElements.length;
    elementsOfIntrest = this.elementsOfIntrest;
    binderLength = binders.length;
    dataContext = this.dataContext;
    dataContextSetter = Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetDataContext;
    for (
    iBinder = 0, iLiveBinder = 0; iBinder < binderLength; iBinder++) {
      binder = binders[iBinder];
      source = null;
      switch(binder.binderType & 7) {
        case 1: {
          if (!this.dataContextUpdated && binder.mode != 0)
            continue;
          source = dataContext;
          break;
        }
        case 2: {
          if (this.firstActivationDone && binder.mode != 0)
            continue;
          break;
        }
        case 3: {
          if (!this.templateParentUpdated && binder.mode != 0)
            continue;
          source = this.skinableParent;
          break;
        }
      }
      if (binder.mode == 2) {
        liveBinder = this.liveBinders[iLiveBinder];
        if (System__Object__IsNullOrUndefined(liveBinder)) {
          liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(binder, this.extraObjects);
          liveBinder.set_source(source);
          liveBinder.set_target(elementsOfIntrest[binder.objectIndex]);
          liveBinder.set_isActive(true);
          this.liveBinders[iLiveBinder] = liveBinder;
        }
        else {
          liveBinder.set_source(source);
          liveBinder.set_isActive(true);
        }
      }
      else {
        Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetPropertyValue(binder, source, elementsOfIntrest[binder.objectIndex], this.extraObjects);
        if (binder.targetPropertySetter === dataContextSetter)
          this.hasDataContextBinding[binder.objectIndex] = true;
      }
      if (binder.mode != 0)
        ++iLiveBinder;
    }
    for (iChild = 0; iChild < childElementLength; iChild++) {
      objectIndex = childElements[iChild];
      childElement = System__NativeArray__GetFrom(elementsOfIntrest, childElements[iChild]);
      if (!this.hasDataContextBinding[objectIndex])
        childElement.set_dataContext(dataContext);
      childElement.activate();
    }
    this.firstActivationDone = true;
    Sunlight__Framework__TaskScheduler__get_Instance().enqueueLowPriTask(System__Delegate__Create("queuedActivation", this), "SkinInstance.Activate");
  }
};
ptyp_.deactivate = function Sunlight__Framework__UI__Helpers__SkinInstance__Deactivate() {
  var childElements, childElementLength, liveBinders, liveBinderLength, iLiveBinder, iChild;
  if (this.isActive && !this.isDiposed) {
    this.isActive = false;
    childElements = this.childElements;
    childElementLength = childElements.length;
    liveBinders = this.liveBinders;
    if (!System__Object__IsNullOrUndefined(liveBinders)) {
      liveBinderLength = liveBinders.length;
      for (iLiveBinder = 0; iLiveBinder < liveBinderLength; iLiveBinder++) {
        if (System__Object__IsNullOrUndefined(liveBinders[iLiveBinder]))
          continue;
        liveBinders[iLiveBinder].set_isActive(false);
      }
    }
    for (iChild = 0; iChild < childElementLength; iChild++)
      System__NativeArray__GetFrom(this.elementsOfIntrest, childElements[iChild]).deactivate();
    Sunlight__Framework__TaskScheduler__get_Instance().enqueueLowPriTask(System__Delegate__Create("queuedDeactivation", this), "SkinInstance.QueuedDeactivate");
  }
};
ptyp_.dispose = function Sunlight__Framework__UI__Helpers__SkinInstance__Dispose() {
  var childNodes, iLiveBinder, liveBinder, i, j, childElement;
  if (!this.isDiposed) {
    if (this.skinableParent) {
      childNodes = this.skinableParent.get_element().childNodes;
      while (childNodes.length > 0)
        this.rootElement.appendChild(childNodes[0]);
    }
    if (!System__Object__IsNullOrUndefined(this.liveBinders))
      for (iLiveBinder = 0; iLiveBinder < this.liveBinders.length; iLiveBinder++) {
        liveBinder = this.liveBinders[iLiveBinder];
        if (System__Object__IsNullOrUndefined(liveBinder))
          continue;
        liveBinder.set_isActive(false);
        liveBinder.set_source(null);
        liveBinder.set_target(null);
        liveBinder.cleanup();
        this.liveBinders[iLiveBinder] = null;
      }
    this.isDiposed = true;
    for (
    i = 0, j = this.childElements.length; i < j; i++) {
      childElement = System__NativeArray__GetFrom(this.elementsOfIntrest, this.childElements[i]);
      childElement.deactivate();
      childElement.dispose();
    }
  }
};
ptyp_.queuedActivation = function Sunlight__Framework__UI__Helpers__SkinInstance__QueuedActivation() {
  var binders, liveBinders, binderLength, liveBindersLength, iBinderInfo, iLivebinder, binder, liveBinder;
  binders = this.binders;
  liveBinders = this.liveBinders;
  if (System__Object__IsNullOrUndefined(liveBinders))
    return;
  binderLength = binders.length;
  liveBindersLength = liveBinders.length;
  for (
  iBinderInfo = 0, iLivebinder = 0; iBinderInfo < binderLength && iLivebinder < liveBindersLength; iBinderInfo++) {
    binder = binders[iBinderInfo];
    if (binder.mode != 0) {
      liveBinder = liveBinders[iLivebinder];
      if (System__Object__IsNullOrUndefined(liveBinder)) {
        liveBinders[iLivebinder] = liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(binder, this.extraObjects);
        liveBinder.set_target(this.elementsOfIntrest[binder.objectIndex]);
      }
      switch(binder.binderType & 7) {
        case 1: {
          liveBinder.set_source(this.skinableParent.get_dataContext());
          break;
        }
        case 3: {
          liveBinder.set_source(this.skinableParent);
          break;
        }
      }
      liveBinder.set_isActive(true);
      ++iLivebinder;
    }
  }
};
ptyp_.queuedDeactivation = function Sunlight__Framework__UI__Helpers__SkinInstance__QueuedDeactivation() {
  var iLiveBinder, liveBinder;
  if (this.isActive || this.isDiposed || System__Object__IsNullOrUndefined(this.liveBinders))
    return;
  for (iLiveBinder = 0; iLiveBinder < this.liveBinders.length; iLiveBinder++) {
    liveBinder = this.liveBinders[iLiveBinder];
    if (System__Object__IsNullOrUndefined(liveBinder))
      return;
    liveBinder.set_isActive(false);
    liveBinder.cleanup();
  }
};
ptyp_.updateBinderSource = function Sunlight__Framework__UI__Helpers__SkinInstance__UpdateBinderSource(source, sourceType) {
  var liveBinders, binders, bindersLength, liveBindersLength, iBinder, iLiveBinder, binder, childElements, childElementLength, iChild, objectIndex, childElement;
  liveBinders = this.liveBinders;
  binders = this.binders;
  bindersLength = binders.length;
  liveBindersLength = System__Object__IsNullOrUndefined(liveBinders) ? 0 : liveBinders.length;
  for (
  iBinder = 0, iLiveBinder = 0; iBinder < bindersLength; iBinder++) {
    binder = binders[iBinder];
    if (binder.mode != 0 && iLiveBinder < liveBindersLength && !System__Object__IsNullOrUndefined(liveBinders[iLiveBinder])) {
      if (sourceType == (binder.binderType & 7))
        liveBinders[iLiveBinder].set_source(source);
      iLiveBinder++;
    }
    else if (sourceType == (binder.binderType & 7))
      Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetPropertyValue(binder, source, this.elementsOfIntrest[binder.objectIndex], this.extraObjects);
  }
  if (sourceType == 1) {
    childElements = this.childElements;
    childElementLength = childElements.length;
    for (iChild = 0; iChild < childElementLength; iChild++) {
      objectIndex = childElements[iChild];
      childElement = System__NativeArray__GetFrom(this.elementsOfIntrest, childElements[iChild]);
      if (!this.hasDataContextBinding[objectIndex])
        childElement.set_dataContext(this.dataContext);
      childElement.activate();
    }
  }
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Helpers_SkinInstance, "Sunlight.Framework.UI.Helpers.SkinInstance", Object, []);
function Sunlight_Framework_UI_ListViewItem() {
};
Sunlight_Framework_UI_ListViewItem.typeId = "ub";
function Sunlight__Framework__UI__ListViewItem_factory(element) {
  var this_;
  this_ = new Sunlight_Framework_UI_ListViewItem();
  this_.__ctor0(element);
  return this_;
};
ptyp_ = new Sunlight_Framework_UI_UISkinableElement();
Sunlight_Framework_UI_ListViewItem.prototype = ptyp_;
ptyp_.__ctor0 = function Sunlight__Framework__UI__ListViewItem____ctor(element) {
  this.__ctor0(element);
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_ListViewItem, "Sunlight.Framework.UI.ListViewItem", Sunlight_Framework_UI_UISkinableElement, []);
Error.typeId = "vb";
System__Type__RegisterReferenceType(Error, "System.Exception", Object, []);
function Sunlight_Framework_UI_UIEvent() {
};
Sunlight_Framework_UI_UIEvent.typeId = "wb";
System__Type__RegisterReferenceType(Sunlight_Framework_UI_UIEvent, "Sunlight.Framework.UI.UIEvent", Object, []);
function System__NativeArray__GetFrom(this_, index) {
  return this_[index];
};
function System_Enum() {
};
System_Enum.typeId = "xb";
ptyp_ = new System_ValueType();
System_Enum.prototype = ptyp_;
ptyp_.toString0 = function System__Enum__ToString() {
  var enumType, value, rv;
  enumType = this.constructor;
  value = this.boxedValue;
  rv = enumType.enumValueToStrMap[value];
  return typeof rv === "undefined" ? value.toString() : rv;
};
ptyp_.toString = ptyp_.toString0;
System__Type__RegisterReferenceType(System_Enum, "System.Enum", System_ValueType, []);
function Sunlight_Framework_UI_Helpers_BinderType(boxedValue) {
  this.boxedValue = boxedValue;
};
Sunlight_Framework_UI_Helpers_BinderType.typeId = "yb";
Sunlight_Framework_UI_Helpers_BinderType.enumStrToValueMap = {
  "DataContext": 1,
  "Static": 2,
  "TemplateParent": 3,
  "TargetTypes": 7,
  "PropertyBinder": 16,
  "AttachedPropertyBinder": 32,
  "EventBinder": 48,
  "DomEventBinder": 64,
  "CssBinder": 80,
  "StyleBinder": 96,
  "AttributeBinder": 112,
  "PropertyTypes": 240
};
Sunlight_Framework_UI_Helpers_BinderType.getDefaultValue = function() {
  return 0;
};
Sunlight_Framework_UI_Helpers_BinderType.prototype = new System_Enum();
System__Type__RegisterEnum(Sunlight_Framework_UI_Helpers_BinderType, "Sunlight.Framework.UI.Helpers.BinderType", true);
function Sunlight_Framework_WindowTimer() {
};
Sunlight_Framework_WindowTimer.typeId = "zb";
function Sunlight__Framework__WindowTimer_factory() {
  return new Sunlight_Framework_WindowTimer();
};
Sunlight_Framework_WindowTimer.defaultConstructor = Sunlight__Framework__WindowTimer_factory;
ptyp_ = Sunlight_Framework_WindowTimer.prototype;
ptyp_.setImmediate = function Sunlight__Framework__WindowTimer__SetImmediate(action) {
  if (typeof setImmediate != "undefined")
    return setImmediate(action);
  return setTimeout(action, 0);
};
ptyp_.setTimeout = function Sunlight__Framework__WindowTimer__SetTimeout(action, timeoutHandle) {
  if (timeoutHandle == 0)
    return this.setImmediate(action);
  return setTimeout(action, timeoutHandle);
};
ptyp_.clearTimeout = function Sunlight__Framework__WindowTimer__ClearTimeout(timeoutHandle) {
  if (typeof clearImmediate != "undefined")
    clearImmediate(timeoutHandle);
  clearTimeout(timeoutHandle);
};
ptyp_.__ctor = function Sunlight__Framework__WindowTimer____ctor() {
};
ptyp_.V_SetImmediate_e = ptyp_.setImmediate;
ptyp_.V_SetTimeout_e = ptyp_.setTimeout;
ptyp_.V_ClearTimeout_e = ptyp_.clearTimeout;
System__Type__RegisterReferenceType(Sunlight_Framework_WindowTimer, "Sunlight.Framework.WindowTimer", Object, [Sunlight_Framework_IWindowTimer]);
function Sunlight_Framework_TaskHandle() {
};
Sunlight_Framework_TaskHandle.typeId = "ac";
function Sunlight__Framework__TaskHandle_factory(taskId) {
  var this_;
  this_ = new Sunlight_Framework_TaskHandle();
  this_.__ctor(taskId);
  return this_;
};
ptyp_ = Sunlight_Framework_TaskHandle.prototype;
ptyp_.taskId = 0;
ptyp_.__ctor = function Sunlight__Framework__TaskHandle____ctor(taskId) {
  this.taskId = taskId;
};
System__Type__RegisterReferenceType(Sunlight_Framework_TaskHandle, "Sunlight.Framework.TaskHandle", Object, []);
function System_EventBinder() {
};
System_EventBinder.typeId = "bc";
function System__EventBinder__GetBinder(importedElement) {
  if (System__Object__IsNullOrUndefined(importedElement.importedExtension))
    importedElement.importedExtension = {
    };
  if (System__Object__IsNullOrUndefined(importedElement.importedExtension.importedExtension))
    importedElement.importedExtension.importedExtension = System__EventBinder_factory(importedElement);
  return System__Type__CastType(System_EventBinder, importedElement.importedExtension.importedExtension);
};
function System__EventBinder__AddEvent(importedElement, name, action, onCapture) {
  var binder;
  binder = System__EventBinder__GetBinder(importedElement);
  binder.addEvent(name, action, onCapture);
};
function System__EventBinder__RemoveEvent(importedElement, name, action, onCapture) {
  var binder;
  if (importedElement.importedExtension === null || importedElement.importedExtension.importedExtension === null)
    return;
  binder = System__EventBinder__GetBinder(importedElement);
  binder.removeEvent(name, action, onCapture);
};
function System__EventBinder__RemoveEvent0(importedElement, name, onCapture) {
  var binder;
  if (importedElement.importedExtension === null || importedElement.importedExtension.importedExtension === null)
    return;
  binder = System__EventBinder__GetBinder(importedElement);
  binder.removeEvent0(name, onCapture);
};
function System__EventBinder__IsW3wc(element) {
  return !!element.addEventListener;
};
function System__EventBinder__GetEventType(evt) {
  return evt.type;
};
function System__EventBinder_factory(element) {
  var this_;
  this_ = new System_EventBinder();
  this_.__ctor(element);
  return this_;
};
ptyp_ = System_EventBinder.prototype;
ptyp_.capturePhaseEvents = null;
ptyp_.bubblePhaseEvents = null;
ptyp_.target = null;
ptyp_.disposed = false;
ptyp_.__ctor = function System__EventBinder____ctor(element) {
  this.capturePhaseEvents = System_Collections_Generic_StringDictionary_$Function$_.defaultConstructor();
  this.bubblePhaseEvents = System_Collections_Generic_StringDictionary_$Function$_.defaultConstructor();
  this.target = element;
};
ptyp_.addEvent = function System__EventBinder__AddEvent0(name, action, onCapture) {
  var isW3wc, evts, elementEvent;
  isW3wc = System__EventBinder__IsW3wc(this.target);
  onCapture = onCapture && isW3wc;
  evts = onCapture ? this.capturePhaseEvents : this.bubblePhaseEvents;
  if (!evts.tryGetValue(name, {
    read: function() {
      return elementEvent;
    },
    write: function(arg0) {
      return elementEvent = arg0;
    }
  })) {
    elementEvent = action;
    if (onCapture && System__EventBinder__IsW3wc(this.target))
      this.addEventListener(name, System__Delegate__Create("eventHandlerCapture", this), true);
    else if (isW3wc)
      this.addEventListener(name, System__Delegate__Create("eventHandlerBubble", this), false);
    else
      this.attachEvent(name, System__Delegate__Create("eventHandlerIE", this));
  }
  else
    elementEvent = System__Delegate__Combine(elementEvent, action);
  evts.set_item(name, elementEvent);
};
ptyp_.removeEvent = function System__EventBinder__RemoveEvent0(name, handler, onCapture) {
  var isW3wc, evts, elementEvent;
  isW3wc = System__EventBinder__IsW3wc(this.target);
  onCapture = onCapture && isW3wc;
  evts = onCapture ? this.capturePhaseEvents : this.bubblePhaseEvents;
  if (evts.tryGetValue(name, {
    read: function() {
      return elementEvent;
    },
    write: function(arg0) {
      return elementEvent = arg0;
    }
  })) {
    elementEvent = System__Delegate__Remove(elementEvent, handler);
    if (!elementEvent) {
      evts.remove(name);
      if (onCapture)
        this.removeEventListener(name, System__Delegate__Create("eventHandlerCapture", this), true);
      else if (isW3wc)
        this.removeEventListener(name, System__Delegate__Create("eventHandlerBubble", this), false);
      else
        this.detachEvent(name, System__Delegate__Create("eventHandlerIE", this));
    }
    else
      evts.set_item(name, elementEvent);
  }
};
ptyp_.removeEvent0 = function System__EventBinder__RemoveEvent0(name, onCapture) {
  var isW3wc, evts;
  isW3wc = System__EventBinder__IsW3wc(this.target);
  onCapture = onCapture && isW3wc;
  evts = onCapture ? this.capturePhaseEvents : this.bubblePhaseEvents;
  if (evts.remove(name))
    if (onCapture)
      this.removeEventListener(name, System__Delegate__Create("eventHandlerCapture", this), true);
    else if (isW3wc)
      this.removeEventListener(name, System__Delegate__Create("eventHandlerBubble", this), true);
    else
      this.detachEvent(name, System__Delegate__Create("eventHandlerIE", this));
};
ptyp_.addEventListener = function System__EventBinder__AddEventListener(evtName, cb, isCapture) {
  this.target.addEventListener(evtName, cb, isCapture);
};
ptyp_.attachEvent = function System__EventBinder__AttachEvent(evtName, cb) {
  this.target.atachEvent("on" + evtName, cb);
};
ptyp_.removeEventListener = function System__EventBinder__RemoveEventListener(evtName, cb, isCapture) {
  this.target.removeEventListener(evtName, cb, isCapture);
};
ptyp_.detachEvent = function System__EventBinder__DetachEvent(evtName, cb) {
  this.target.detachEvent("on" + evtName, cb);
};
ptyp_.eventHandlerIE = function System__EventBinder__EventHandlerIE() {
  this.eventHandlerBubble(event);
};
ptyp_.eventHandlerCapture = function System__EventBinder__EventHandlerCapture(evt) {
  if (this.disposed)
    return;
  System__Type__CastType(System_Collections_Generic_KeyValuePair_$String_x_Function$_, this.capturePhaseEvents.get_item(System__EventBinder__GetEventType(evt)))(this.target, evt);
};
ptyp_.eventHandlerBubble = function System__EventBinder__EventHandlerBubble(evt) {
  var del;
  if (this.disposed)
    return;
  if (this.bubblePhaseEvents.tryGetValue(System__EventBinder__GetEventType(evt), {
    read: function() {
      return del;
    },
    write: function(arg0) {
      return del = arg0;
    }
  }))
    del(this.target, evt);
};
System__Type__RegisterReferenceType(System_EventBinder, "System.EventBinder", Object, []);
Date.typeId = "cc";
function System__DateTime__op_Addition(a, n) {
  return new Date(a.valueOf() + n);
};
function System__DateTime__get_Now() {
  return new Date();
};
function System__DateTime____cctor() {
  Date.empty = new Date(0);
};
System__Type__RegisterReferenceType(Date, "System.DateTime", Object, []);
function System_Collections_IEnumerator() {
};
System_Collections_IEnumerator.typeId = "g";
System__Type__RegisterInterface(System_Collections_IEnumerator, "System.Collections.IEnumerator");
function Sunlight_Framework_Observables_INotifyCollectionChanged() {
};
Sunlight_Framework_Observables_INotifyCollectionChanged.typeId = "h";
System__Type__RegisterInterface(Sunlight_Framework_Observables_INotifyCollectionChanged, "Sunlight.Framework.Observables.INotifyCollectionChanged");
function System__Diagnostics__Debug__Assert(condition) {
};
function Sunlight_Framework_Observables_CollectionChangedEventArgs() {
};
Sunlight_Framework_Observables_CollectionChangedEventArgs.typeId = "i";
System__Type__RegisterInterface(Sunlight_Framework_Observables_CollectionChangedEventArgs, "Sunlight.Framework.Observables.CollectionChangedEventArgs");
function Sunlight_Framework_Observables_IObservableCollection() {
};
Sunlight_Framework_Observables_IObservableCollection.typeId = "j";
System__Type__RegisterInterface(Sunlight_Framework_Observables_IObservableCollection, "Sunlight.Framework.Observables.IObservableCollection");
Number.typeId = "l";
System__Type__RegisterReferenceType(Number, "System.Number", Object, []);
function System_NotSupportedException() {
};
System_NotSupportedException.typeId = "tc";
function System__NotSupportedException_factory() {
  var this_;
  this_ = new System_NotSupportedException();
  this_.__ctor();
  return this_;
};
System_NotSupportedException.defaultConstructor = System__NotSupportedException_factory;
ptyp_ = new Error();
System_NotSupportedException.prototype = ptyp_;
ptyp_.__ctor = function System__NotSupportedException____ctor() {
};
System__Type__RegisterReferenceType(System_NotSupportedException, "System.NotSupportedException", Error, []);
Array.typeId = "uc";
System__Type__RegisterReferenceType(Array, "System.Array", Object, [System_Collections_IList, System_Collections_IEnumerable, System_Collections_ICollection]);
function Sunlight_Framework_UI_Test_ValueIfTrue(T, $5fcallStatiConstructor) {
  var ValueIfTrue$1_$T$_, $5f_initTracker;
  if (Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId])
    return Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId];
  Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId] = function Sunlight__Framework__UI__Test__ValueIfTrue$10() {
  };
  ValueIfTrue$1_$T$_ = Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId];
  ValueIfTrue$1_$T$_.genericParameters = [T];
  ValueIfTrue$1_$T$_.genericClosure = Sunlight_Framework_UI_Test_ValueIfTrue;
  ValueIfTrue$1_$T$_.typeId = "dc$" + T.typeId + "$";
  ValueIfTrue$1_$T$_.__ctor = function Sunlight_Framework_UI_Test_ValueIfTrue$1_factory0(value) {
    var this_;
    this_ = new ValueIfTrue$1_$T$_();
    this_.__ctor(value);
    return this_;
  };
  ptyp_ = ValueIfTrue$1_$T$_.prototype;
  ptyp_.value = System__Type__GetDefaultValueStatic(T);
  ptyp_.defaultValue = System__Type__GetDefaultValueStatic(T);
  ptyp_.__ctor = function Sunlight__Framework__UI__Test__ValueIfTrue$1____ctor(value) {
    this.value = value;
    this.defaultValue = System__Type__GetDefaultValueStatic(T);
  };
  System__Type__RegisterReferenceType(ValueIfTrue$1_$T$_, "Sunlight.Framework.UI.Test.ValueIfTrue`1<" + T.fullName + ">", Object, []);
  ValueIfTrue$1_$T$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    ValueIfTrue$1_$T$_ = Sunlight_Framework_UI_Test_ValueIfTrue(T, true);
  };
  if ($5fcallStatiConstructor)
    ValueIfTrue$1_$T$_.$5ftri();
  return ValueIfTrue$1_$T$_;
};
function System_ArrayG(T, $5fcallStatiConstructor) {
  var Enumerator_$T$_, ArrayG$1_$T$_, IList$1_$T$_, IEnumerable$1_$T$_, $5f_initTracker, T$5b$5d_$T$_, $5f_initTracker0;
  if (System_ArrayG[T.typeId])
    return System_ArrayG[T.typeId];
  System_ArrayG[T.typeId] = function System__ArrayG$10() {
  };
  ArrayG$1_$T$_ = System_ArrayG[T.typeId];
  ArrayG$1_$T$_.genericParameters = [T];
  ArrayG$1_$T$_.genericClosure = System_ArrayG;
  ArrayG$1_$T$_.typeId = "ec$" + T.typeId + "$";
  IList$1_$T$_ = System_Collections_Generic_IList(T, $5fcallStatiConstructor);
  IEnumerable$1_$T$_ = System_Collections_Generic_IEnumerable(T, $5fcallStatiConstructor);
  ArrayG$1_$T$_.__ctor0 = function System_ArrayG$1_factory0(size) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctor0(size);
    return this_;
  };
  ArrayG$1_$T$_.__ctor = function System_ArrayG$1_factory0(nativeArray) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctor0(nativeArray);
    return this_;
  };
  ptyp_ = new System_ArrayImpl();
  ArrayG$1_$T$_.prototype = ptyp_;
  ptyp_.innerArray = null;
  ptyp_.system__Collections__Generic__IList_$T$___Insert = function System__ArrayG$1__System__Collections__Generic__IList_$T$___Insert(index, item) {
    throw new Error("Not Implemented.");
  };
  ptyp_.system__Collections__Generic__IEnumerable_$T$___GetEnumerator = function System__ArrayG$1__System__Collections__Generic__IEnumerable_$T$___GetEnumerator() {
    return Enumerator_$T$_.__ctor(this);
  };
  ptyp_.__ctor0 = function System__ArrayG$1____ctor0(size) {
    var def, i;
    this.__ctor();
    this.innerArray = new Array(size);
    def = System__Type__GetDefaultValueStatic(T);
    for (i = 0; i < size; i++)
      this.innerArray[i] = def;
  };
  ptyp_.__ctor0 = function System__ArrayG$1____ctor(nativeArray) {
    this.__ctor();
    this.innerArray = nativeArray;
  };
  ptyp_.get_length = function System__ArrayG$1__get_Length() {
    return this.innerArray.length;
  };
  ptyp_.get_item = function System__ArrayG$1__get_Item(index) {
    var arr;
    arr = this.innerArray;
    if (index < 0 || index >= arr.length)
      throw "index " + index + " out of range";
    return arr[index];
  };
  ptyp_.set_item = function System__ArrayG$1__set_Item(index, value) {
    var arr;
    arr = this.innerArray;
    if (index < 0 || index >= arr.length)
      throw "index " + index + " out of range";
    return arr[index] = value;
  };
  ptyp_.get_innerArray = function System__ArrayG$1__get_InnerArray() {
    return this.innerArray;
  };
  ptyp_.contains = function System__ArrayG$1__Contains(item) {
    return this.V_IndexOf(item) >= 0;
  };
  ptyp_.indexOf = function System__ArrayG$1__IndexOf(item) {
    if (!T.isInstanceOfType(item))
      return -1;
    return System__NativeArray$1__IndexOf(this.innerArray, System__Type__UnBoxTypeInstance(T, item), 0);
  };
  ptyp_.getValue = function System__ArrayG$1__GetValue(index) {
    return System__Type__BoxTypeInstance(T, this.get_item(index));
  };
  ptyp_.copyTo = function System__ArrayG$1__CopyTo0(arr, index) {
    var nativeArray, length, nativeArrDst, i;
    nativeArray = this.innerArray;
    length = nativeArray.length;
    nativeArrDst = System__NativeArray$1__op_Implicit(arr);
    if (nativeArrDst.length < index + nativeArray.length)
      throw new Error("can't copy, dest array too small.");
    for (i = 0; i < length; i++)
      nativeArrDst[i + index] = nativeArray[i];
  };
  ptyp_.copyTo0 = function System__ArrayG$1__CopyTo(array, index) {
    var arr;
    arr = System__Type__CastType(T$5b$5d_$T$_, array);
    this.copyTo(arr, index);
  };
  ptyp_.getEnumerator = function System__ArrayG$1__GetEnumerator() {
    return Enumerator_$T$_.__ctor(this);
  };
  ptyp_["V_Insert_" + IList$1_$T$_.typeId] = ptyp_.system__Collections__Generic__IList_$T$___Insert;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.system__Collections__Generic__IEnumerable_$T$___GetEnumerator;
  ptyp_.V_get_Length = ptyp_.get_length;
  ptyp_.V_Contains = ptyp_.contains;
  ptyp_.V_GetEnumerator = ptyp_.getEnumerator;
  ptyp_.V_IndexOf = ptyp_.indexOf;
  ptyp_.V_GetValue = ptyp_.getValue;
  ptyp_.V_CopyTo = ptyp_.copyTo0;
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  System__Type__RegisterReferenceType(ArrayG$1_$T$_, "System.ArrayG`1<" + T.fullName + ">", System_ArrayImpl, [IList$1_$T$_, System_Collections_IList, System_Collections_ICollection, System_Collections_IEnumerable, IEnumerable$1_$T$_]);
  ArrayG$1_$T$_.$5ftri = function() {
    if ($5f_initTracker0)
      return;
    $5f_initTracker0 = true;
    T = T;
    Enumerator_$T$_ = System_ArrayG_Enumerator(T, true);
    ArrayG$1_$T$_ = System_ArrayG(T, true);
    T$5b$5d_$T$_ = System_ArrayG(T, true);
  };
  if ($5fcallStatiConstructor)
    ArrayG$1_$T$_.$5ftri();
  return ArrayG$1_$T$_;
};
function System__NativeArray$1__Pop(this_) {
  return this_.pop();
};
function System__NativeArray$1__Push(this_, value) {
  return this_.push(value);
};
function System__NativeArray$1__IndexOf(this_, value, startIndex) {
  var i;
  startIndex = startIndex < 0 ? 0 : startIndex;
  for (i = this_.length; i >= startIndex && i >= 0; --i)
    if (this_[i] === value)
      return i;
  return -1;
};
function System__NativeArray$1__InsertAt(this_, index, value) {
  var i;
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  for (i = this_.length - 1; i >= index; i--)
    this_[i + 1] = this_[i];
  this_[index] = value;
};
function System__NativeArray$1__RemoveAt(this_, index) {
  var len, i;
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  len = this_.length - 1;
  for (i = index; i < len; i++)
    this_[i] = this_[i + 1];
  this_.pop();
};
function System__NativeArray$1__op_Implicit(n) {
  return n.get_innerArray();
};
function System_Func(T1, TRes, $5fcallStatiConstructor) {
  var Func$2_$T1_x_TRes$_, $5f_initTracker;
  if (System_Func[T1.typeId] && System_Func[T1.typeId][TRes.typeId])
    return System_Func[T1.typeId][TRes.typeId];
    System_Func[T1.typeId] = {
    };
  System_Func[T1.typeId][TRes.typeId] = function System__Func$20() {
  };
  Func$2_$T1_x_TRes$_ = System_Func[T1.typeId][TRes.typeId];
  Func$2_$T1_x_TRes$_.genericParameters = [T1, TRes];
  Func$2_$T1_x_TRes$_.genericClosure = System_Func;
  Func$2_$T1_x_TRes$_.typeId = "fc$" + T1.typeId + "_" + TRes.typeId + "$";
  Func$2_$T1_x_TRes$_.prototype = new Function0();
  System__Type__RegisterReferenceType(Func$2_$T1_x_TRes$_, "System.Func`2<" + T1.fullName + "," + TRes.fullName + ">", Function0, []);
  Func$2_$T1_x_TRes$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T1 = T1;
    TRes = TRes;
    Func$2_$T1_x_TRes$_ = System_Func(T1, TRes, true);
  };
  if ($5fcallStatiConstructor)
    Func$2_$T1_x_TRes$_.$5ftri();
  return Func$2_$T1_x_TRes$_;
};
function Sunlight_Framework_Binders_ContextBindableObject() {
};
Sunlight_Framework_Binders_ContextBindableObject.typeId = "jb";
function Sunlight__Framework__Binders__ContextBindableObject_factory() {
  var this_;
  this_ = new Sunlight_Framework_Binders_ContextBindableObject();
  this_.__ctor0();
  return this_;
};
Sunlight_Framework_Binders_ContextBindableObject.defaultConstructor = Sunlight__Framework__Binders__ContextBindableObject_factory;
ptyp_ = new Sunlight_Framework_Observables_ExtensibleObservableObject();
Sunlight_Framework_Binders_ContextBindableObject.prototype = ptyp_;
ptyp_.parent = null;
ptyp_.dataContext = null;
ptyp_.dataContextSetterCalled = false;
ptyp_.isActive = false;
ptyp_.isPreActivated = false;
ptyp_.isActivated = false;
ptyp_.isDisposing = false;
ptyp_.isDisposed = false;
ptyp_.onDisposed = null;
ptyp_.isInactiveIfNullContext = false;
ptyp_.set_parent = function Sunlight__Framework__Binders__ContextBindableObject__set_Parent(value) {
  if (this.parent != value) {
    if (this.parent) {
      this.parent.removePropertyChangedListener("DataContext", System__Delegate__Create("onParentDataContextUpdated", this));
      this.parent.removePropertyChangedListener("IsActive", System__Delegate__Create("onParentDataContextUpdated", this));
    }
    this.parent = value;
    if (!this.dataContextSetterCalled)
      if (this.parent) {
        this.parent.addPropertyChangedListener("DataContext", System__Delegate__Create("onParentDataContextUpdated", this));
        this.parent.addPropertyChangedListener("IsActive", System__Delegate__Create("onParentDataContextUpdated", this));
        this.onParentDataContextUpdated(null, null);
      }
      else
        this.setDataContext(null);
  }
};
ptyp_.get_dataContext = function Sunlight__Framework__Binders__ContextBindableObject__get_DataContext() {
  return this.dataContext;
};
ptyp_.set_dataContext = function Sunlight__Framework__Binders__ContextBindableObject__set_DataContext(value) {
  this.dataContextSetterCalled = true;
  this.setDataContext(value);
};
ptyp_.get_isActive = function Sunlight__Framework__Binders__ContextBindableObject__get_IsActive() {
  return this.isActivated && !this.V_get_ActivationBlocked();
};
ptyp_.set_inactiveIfNullContext = function Sunlight__Framework__Binders__ContextBindableObject__set_InactiveIfNullContext(value) {
  this.isInactiveIfNullContext = value;
  this.fixActivation();
};
ptyp_.get_activationBlocked = function Sunlight__Framework__Binders__ContextBindableObject__get_ActivationBlocked() {
  return this.isInactiveIfNullContext && this.dataContext === null;
};
ptyp_.dispose = function Sunlight__Framework__Binders__ContextBindableObject__Dispose() {
  if (!this.isDisposed && !this.isDisposing)
    this.V_InternalDispose();
};
ptyp_.activate = function Sunlight__Framework__Binders__ContextBindableObject__Activate() {
  this.isActive = true;
  this.fixActivation();
};
ptyp_.deactivate = function Sunlight__Framework__Binders__ContextBindableObject__Deactivate() {
  if (!this.isActive)
    return;
  this.isActive = false;
  this.fixActivation();
};
ptyp_.onBeforeFirstActivate = function Sunlight__Framework__Binders__ContextBindableObject__OnBeforeFirstActivate() {
};
ptyp_.onActivate = function Sunlight__Framework__Binders__ContextBindableObject__OnActivate() {
};
ptyp_.onDeactivate = function Sunlight__Framework__Binders__ContextBindableObject__OnDeactivate() {
};
ptyp_.fixActivation = function Sunlight__Framework__Binders__ContextBindableObject__FixActivation() {
  if (!this.V_get_ActivationBlocked() && this.isActive) {
    if (!this.isPreActivated) {
      this.isPreActivated = true;
      this.V_OnBeforeFirstActivate();
    }
    if (!this.isActivated) {
      this.isActivated = true;
      this.V_OnActivate();
      this.firePropertyChanged("IsActive");
    }
  }
  else if (this.isActivated) {
    this.isActivated = false;
    this.V_OnDeactivate();
    this.firePropertyChanged("IsActive");
  }
};
ptyp_.internalDispose = function Sunlight__Framework__Binders__ContextBindableObject__InternalDispose() {
  if (this.onDisposed) {
    this.set_parent(null);
    this.clearListeners();
    this.onDisposed();
  }
};
ptyp_.onDataContextUpdating = function Sunlight__Framework__Binders__ContextBindableObject__OnDataContextUpdating(newValue) {
};
ptyp_.onDataContextUpdated = function Sunlight__Framework__Binders__ContextBindableObject__OnDataContextUpdated(oldValue) {
};
ptyp_.setDataContext = function Sunlight__Framework__Binders__ContextBindableObject__SetDataContext(value) {
  var oldValue;
  if (this.dataContext !== value) {
    this.V_OnDataContextUpdating(value);
    oldValue = this.dataContext;
    this.dataContext = value;
    this.V_OnDataContextUpdated(oldValue);
    this.firePropertyChanged("DataContext");
    this.fixActivation();
  }
};
ptyp_.onParentDataContextUpdated = function Sunlight__Framework__Binders__ContextBindableObject__OnParentDataContextUpdated(sender, propertyName) {
  if (this.parent.get_isActive() && !this.dataContextSetterCalled)
    this.setDataContext(this.parent.get_dataContext());
  if (propertyName === "IsActive" || !propertyName)
    if (this.parent.get_isActive())
      this.activate();
    else
      this.deactivate();
};
ptyp_.__ctor0 = function Sunlight__Framework__Binders__ContextBindableObject____ctor() {
  this.__ctor0();
  this.isInactiveIfNullContext = true;
};
ptyp_.V_get_ActivationBlocked = ptyp_.get_activationBlocked;
ptyp_.V_OnBeforeFirstActivate = ptyp_.onBeforeFirstActivate;
ptyp_.V_OnActivate = ptyp_.onActivate;
ptyp_.V_OnDeactivate = ptyp_.onDeactivate;
ptyp_.V_InternalDispose = ptyp_.internalDispose;
ptyp_.V_OnDataContextUpdating = ptyp_.onDataContextUpdating;
ptyp_.V_OnDataContextUpdated = ptyp_.onDataContextUpdated;
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_ContextBindableObject, "Sunlight.Framework.Binders.ContextBindableObject", Sunlight_Framework_Observables_ExtensibleObservableObject, []);
function Sunlight_Framework_Observables_INotifyPropertyChanged() {
};
Sunlight_Framework_Observables_INotifyPropertyChanged.typeId = "b";
System__Type__RegisterInterface(Sunlight_Framework_Observables_INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function System_Collections_Generic_NumberDictionary(TValue, $5fcallStatiConstructor) {
  var Enumerator_$TValue$_, NumberDictionary$1_$TValue$_, KeyValuePair$2_$Number_x_TValue$_, IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_, $5f_initTracker, $5f_initTracker0;
  if (System_Collections_Generic_NumberDictionary[TValue.typeId])
    return System_Collections_Generic_NumberDictionary[TValue.typeId];
  System_Collections_Generic_NumberDictionary[TValue.typeId] = function System__Collections__Generic__NumberDictionary$10() {
  };
  NumberDictionary$1_$TValue$_ = System_Collections_Generic_NumberDictionary[TValue.typeId];
  NumberDictionary$1_$TValue$_.genericParameters = [TValue];
  NumberDictionary$1_$TValue$_.genericClosure = System_Collections_Generic_NumberDictionary;
  NumberDictionary$1_$TValue$_.typeId = "gc$" + TValue.typeId + "$";
  KeyValuePair$2_$Number_x_TValue$_ = System_Collections_Generic_KeyValuePair(Number, TValue, $5fcallStatiConstructor);
  KeyValuePair$2_$Number_x_TValue$_ = System_Collections_Generic_KeyValuePair(Number, TValue, $5fcallStatiConstructor);
  IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_ = System_Collections_Generic_IEnumerable(System_Collections_Generic_KeyValuePair(Number, TValue, $5fcallStatiConstructor), $5fcallStatiConstructor);
  NumberDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_NumberDictionary$1_factory0() {
    var this_;
    this_ = new NumberDictionary$1_$TValue$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = NumberDictionary$1_$TValue$_.prototype;
  ptyp_.innerDict = null;
  ptyp_.count = 0;
  ptyp_.system__Collections__IEnumerable__GetEnumerator = function System__Collections__Generic__NumberDictionary$1__System__Collections__IEnumerable__GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.__ctor = function System__Collections__Generic__NumberDictionary$1____ctor() {
    this.innerDict = {
    };
  };
  ptyp_.get_item = function System__Collections__Generic__NumberDictionary$1__get_Item(index) {
    if (!(index in this.innerDict))
      throw new Error("Key not found");
    return this.innerDict[index];
  };
  ptyp_.set_item = function System__Collections__Generic__NumberDictionary$1__set_Item(index, value) {
    this.innerDict[index] = value;
  };
  ptyp_.get_keys = function System__Collections__Generic__NumberDictionary$1__get_Keys() {
    return System_ArrayG_$Number$_.__ctor(this.getKeys());
  };
  ptyp_.get_count = function System__Collections__Generic__NumberDictionary$1__get_Count() {
    return this.count;
  };
  ptyp_.add = function System__Collections__Generic__NumberDictionary$1__Add(key, value) {
    if (this.containsKey(key))
      throw new Error("Key already exists");
    this.count++;
    this.set_item(key, value);
  };
  ptyp_.containsKey = function System__Collections__Generic__NumberDictionary$1__ContainsKey(key) {
    return key in this.innerDict;
  };
  ptyp_.remove = function System__Collections__Generic__NumberDictionary$1__Remove(key) {
    var rv;
    rv = delete this.innerDict[key];
    if (rv)
      this.count--;
    return rv;
  };
  ptyp_.copyTo = function System__Collections__Generic__NumberDictionary$1__CopyTo(array, index) {
    var keys, i;
    keys = this.get_keys();
    for (i = 0; i < keys.V_get_Length(); i++)
      array.setValue(i + index, System__Type__BoxTypeInstance(KeyValuePair$2_$Number_x_TValue$_, KeyValuePair$2_$Number_x_TValue$_.__ctor(keys.get_item(i), this.get_item(keys.get_item(i)))));
  };
  ptyp_.getEnumerator = function System__Collections__Generic__NumberDictionary$1__GetEnumerator() {
    return Enumerator_$TValue$_.__ctor(this);
  };
  ptyp_.getKeys = function System__Collections__Generic__NumberDictionary$1__GetKeys() {
    var rv, key;
    rv = [];
    for (key in this.innerDict)
      rv.push(key);
    return rv;
  };
  ptyp_.V_GetEnumerator_f = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_.V_get_Count_c = ptyp_.get_count;
  ptyp_.V_CopyTo_c = ptyp_.copyTo;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_.typeId] = ptyp_.getEnumerator;
  System__Type__RegisterReferenceType(NumberDictionary$1_$TValue$_, "System.Collections.Generic.NumberDictionary`1<" + TValue.fullName + ">", Object, [System_Collections_ICollection, IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_, System_Collections_IEnumerable]);
  NumberDictionary$1_$TValue$_.$5ftri = function() {
    if ($5f_initTracker0)
      return;
    $5f_initTracker0 = true;
    Enumerator_$TValue$_ = System_Collections_Generic_NumberDictionary_Enumerator(TValue, true);
    TValue = TValue;
    NumberDictionary$1_$TValue$_ = System_Collections_Generic_NumberDictionary(TValue, true);
  };
  if ($5fcallStatiConstructor)
    NumberDictionary$1_$TValue$_.$5ftri();
  return NumberDictionary$1_$TValue$_;
};
function System_Collections_Generic_Queue(T, $5fcallStatiConstructor) {
  var QueueEnumerator_$T$_, Queue$1_$T$_, IEnumerable$1_$T$_, $5f_initTracker, $5f_initTracker0;
  if (System_Collections_Generic_Queue[T.typeId])
    return System_Collections_Generic_Queue[T.typeId];
  System_Collections_Generic_Queue[T.typeId] = function System__Collections__Generic__Queue$10() {
  };
  Queue$1_$T$_ = System_Collections_Generic_Queue[T.typeId];
  Queue$1_$T$_.genericParameters = [T];
  Queue$1_$T$_.genericClosure = System_Collections_Generic_Queue;
  Queue$1_$T$_.typeId = "hc$" + T.typeId + "$";
  IEnumerable$1_$T$_ = System_Collections_Generic_IEnumerable(T, $5fcallStatiConstructor);
  Queue$1_$T$_.defaultConstructor = function System_Collections_Generic_Queue$1_factory0() {
    var this_;
    this_ = new Queue$1_$T$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = Queue$1_$T$_.prototype;
  ptyp_.nativeArray = null;
  ptyp_.system__Collections__IEnumerable__GetEnumerator = function System__Collections__Generic__Queue$1__System__Collections__IEnumerable__GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.dequeue = function System__Collections__Generic__Queue$1__Dequeue() {
    if (this.get_count() > 0)
      return System__NativeArray$1__Pop(this.nativeArray);
    throw new Error("No elements in stack");
  };
  ptyp_.enqueue = function System__Collections__Generic__Queue$1__Enqueue(item) {
    System__NativeArray$1__InsertAt(this.nativeArray, this.nativeArray.length, item);
  };
  ptyp_.get_count = function System__Collections__Generic__Queue$1__get_Count() {
    return this.nativeArray.length;
  };
  ptyp_.getEnumerator = function System__Collections__Generic__Queue$1__GetEnumerator() {
    return QueueEnumerator_$T$_.__ctor(this);
  };
  ptyp_.__ctor = function System__Collections__Generic__Queue$1____ctor() {
    this.nativeArray = new Array(0);
  };
  ptyp_.V_GetEnumerator_f = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.getEnumerator;
  System__Type__RegisterReferenceType(Queue$1_$T$_, "System.Collections.Generic.Queue`1<" + T.fullName + ">", Object, [IEnumerable$1_$T$_, System_Collections_IEnumerable]);
  Queue$1_$T$_.$5ftri = function() {
    if ($5f_initTracker0)
      return;
    $5f_initTracker0 = true;
    QueueEnumerator_$T$_ = System_Collections_Generic_Queue_QueueEnumerator(T, true);
    T = T;
    Queue$1_$T$_ = System_Collections_Generic_Queue(T, true);
  };
  if ($5fcallStatiConstructor)
    Queue$1_$T$_.$5ftri();
  return Queue$1_$T$_;
};
function System_Collections_Generic_List(T, $5fcallStatiConstructor) {
  var ListEnumerator$1_$T$_, List$1_$T$_, IList$1_$T$_, IEnumerable$1_$T$_, $5f_initTracker, $5f_initTracker0;
  if (System_Collections_Generic_List[T.typeId])
    return System_Collections_Generic_List[T.typeId];
  System_Collections_Generic_List[T.typeId] = function System__Collections__Generic__List$10() {
  };
  List$1_$T$_ = System_Collections_Generic_List[T.typeId];
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = System_Collections_Generic_List;
  List$1_$T$_.typeId = "ic$" + T.typeId + "$";
  IList$1_$T$_ = System_Collections_Generic_IList(T, $5fcallStatiConstructor);
  IEnumerable$1_$T$_ = System_Collections_Generic_IEnumerable(T, $5fcallStatiConstructor);
  List$1_$T$_.defaultConstructor = function System_Collections_Generic_List$1_factory0() {
    var this_;
    this_ = new List$1_$T$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = List$1_$T$_.prototype;
  ptyp_.nativeArray = null;
  ptyp_.system__Collections__IList__IndexOf = function System__Collections__Generic__List$1__System__Collections__IList__IndexOf(value) {
    if (value === null && T.isInstanceOfType(value))
      return System__NativeArray$1__IndexOf(this.nativeArray, System__Type__UnBoxTypeInstance(T, value), 0);
    return -1;
  };
  ptyp_.system__Collections__ICollection__CopyTo = function System__Collections__Generic__List$1__System__Collections__ICollection__CopyTo(array, index) {
    var nativeArray, length, i;
    nativeArray = this.nativeArray;
    length = nativeArray.length;
    for (i = 0; i < length; i++)
      array.setValue(i + index, System__Type__BoxTypeInstance(T, nativeArray[i]));
  };
  ptyp_.system__Collections__IEnumerable__GetEnumerator = function System__Collections__Generic__List$1__System__Collections__IEnumerable__GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.system__Collections__IList__get_Item = function System__Collections__Generic__List$1__System__Collections__IList__get_Item(index) {
    return System__Type__BoxTypeInstance(T, this.get_item(index));
  };
  ptyp_.system__Collections__IList__Contains = function System__Collections__Generic__List$1__System__Collections__IList__Contains(value) {
    return System__Type__CastType(System_Collections_IList, this).V_IndexOf_d(value) >= 0;
  };
  ptyp_.__ctor = function System__Collections__Generic__List$1____ctor() {
    this.nativeArray = new Array(0);
  };
  ptyp_.get_item = function System__Collections__Generic__List$1__get_Item(index) {
    var arr;
    arr = this.nativeArray;
    if (index < 0 || index >= arr.length)
      throw "index " + index + " out of range";
    return arr[index];
  };
  ptyp_.set_item = function System__Collections__Generic__List$1__set_Item(index, value) {
    var arr;
    arr = this.nativeArray;
    if (index < 0 || index >= arr.length)
      throw "index " + index + " out of range";
    return arr[index] = value;
  };
  ptyp_.insert = function System__Collections__Generic__List$1__Insert(index, item) {
    System__NativeArray$1__InsertAt(this.nativeArray, index, item);
  };
  ptyp_.removeAt = function System__Collections__Generic__List$1__RemoveAt(index) {
    System__NativeArray$1__RemoveAt(this.nativeArray, index);
  };
  ptyp_.get_count = function System__Collections__Generic__List$1__get_Count() {
    return this.nativeArray.length;
  };
  ptyp_.add = function System__Collections__Generic__List$1__Add(item) {
    System__NativeArray$1__Push(this.nativeArray, item);
  };
  ptyp_.clear = function System__Collections__Generic__List$1__Clear() {
    this.nativeArray.length = 0;
  };
  ptyp_.getEnumerator = function System__Collections__Generic__List$1__GetEnumerator() {
    return ListEnumerator$1_$T$_.__ctor(this);
  };
  ptyp_.V_IndexOf_d = ptyp_.system__Collections__IList__IndexOf;
  ptyp_.V_CopyTo_c = ptyp_.system__Collections__ICollection__CopyTo;
  ptyp_.V_GetEnumerator_f = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_.V_get_Item_d = ptyp_.system__Collections__IList__get_Item;
  ptyp_.V_Contains_d = ptyp_.system__Collections__IList__Contains;
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  ptyp_["V_Insert_" + IList$1_$T$_.typeId] = ptyp_.insert;
  ptyp_.V_Clear_d = ptyp_.clear;
  ptyp_.V_RemoveAt_d = ptyp_.removeAt;
  ptyp_.V_get_Count_c = ptyp_.get_count;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.getEnumerator;
  System__Type__RegisterReferenceType(List$1_$T$_, "System.Collections.Generic.List`1<" + T.fullName + ">", Object, [IList$1_$T$_, System_Collections_IList, System_Collections_ICollection, System_Collections_IEnumerable, IEnumerable$1_$T$_]);
  List$1_$T$_.$5ftri = function() {
    if ($5f_initTracker0)
      return;
    $5f_initTracker0 = true;
    T = T;
    ListEnumerator$1_$T$_ = System_Collections_Generic_ListEnumerator(T, true);
    List$1_$T$_ = System_Collections_Generic_List(T, true);
  };
  if ($5fcallStatiConstructor)
    List$1_$T$_.$5ftri();
  return List$1_$T$_;
};
function System_Collections_Generic_StringDictionary(TValue, $5fcallStatiConstructor) {
  var Enumerator_$TValue$_, StringDictionary$1_$TValue$_, KeyValuePair$2_$String_x_TValue$_, IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_, $5f_initTracker, $5f_initTracker0;
  if (System_Collections_Generic_StringDictionary[TValue.typeId])
    return System_Collections_Generic_StringDictionary[TValue.typeId];
  System_Collections_Generic_StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$10() {
  };
  StringDictionary$1_$TValue$_ = System_Collections_Generic_StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = System_Collections_Generic_StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "jc$" + TValue.typeId + "$";
  KeyValuePair$2_$String_x_TValue$_ = System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor);
  KeyValuePair$2_$String_x_TValue$_ = System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor);
  IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_ = System_Collections_Generic_IEnumerable(System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor), $5fcallStatiConstructor);
  StringDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_StringDictionary$1_factory0() {
    var this_;
    this_ = new StringDictionary$1_$TValue$_();
    this_.__ctor();
    return this_;
  };
  StringDictionary$1_$TValue$_.__ctor = function System_Collections_Generic_StringDictionary$1_factory0(innerDict) {
    var this_;
    this_ = new StringDictionary$1_$TValue$_();
    this_.__ctor0(innerDict);
    return this_;
  };
  ptyp_ = StringDictionary$1_$TValue$_.prototype;
  ptyp_.innerDict = null;
  ptyp_.count = 0;
  ptyp_.system__Collections__IEnumerable__GetEnumerator = function System__Collections__Generic__StringDictionary$1__System__Collections__IEnumerable__GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.__ctor = function System__Collections__Generic__StringDictionary$1____ctor() {
    this.innerDict = {
    };
  };
  ptyp_.__ctor0 = function System__Collections__Generic__StringDictionary$1____ctor0(innerDict) {
    this.innerDict = innerDict;
    this.count = this.computeCount();
  };
  ptyp_.get_item = function System__Collections__Generic__StringDictionary$1__get_Item(index) {
    if (!(index in this.innerDict))
      throw new Error("Key not found");
    return this.innerDict[index];
  };
  ptyp_.set_item = function System__Collections__Generic__StringDictionary$1__set_Item(index, value) {
    this.innerDict[index] = value;
  };
  ptyp_.get_keys = function System__Collections__Generic__StringDictionary$1__get_Keys() {
    return System_ArrayG_$String$_.__ctor(this.getKeys());
  };
  ptyp_.get_count = function System__Collections__Generic__StringDictionary$1__get_Count() {
    return this.count;
  };
  ptyp_.containsKey = function System__Collections__Generic__StringDictionary$1__ContainsKey(key) {
    return key in this.innerDict;
  };
  ptyp_.remove = function System__Collections__Generic__StringDictionary$1__Remove(key) {
    var rv;
    rv = delete this.innerDict[key];
    if (rv)
      this.count--;
    return rv;
  };
  ptyp_.tryGetValue = function System__Collections__Generic__StringDictionary$1__TryGetValue(key, value) {
    if (this.containsKey(key)) {
      value.write(this.get_item(key));
      return true;
    }
    value.write(System__Type__GetDefaultValueStatic(TValue));
    return false;
  };
  ptyp_.clear = function System__Collections__Generic__StringDictionary$1__Clear() {
    this.innerDict = {
    };
    this.count = 0;
  };
  ptyp_.copyTo = function System__Collections__Generic__StringDictionary$1__CopyTo(array, index) {
    var keys, i;
    keys = this.get_keys();
    for (i = 0; i < keys.V_get_Length(); i++)
      array.setValue(i + index, System__Type__BoxTypeInstance(KeyValuePair$2_$String_x_TValue$_, KeyValuePair$2_$String_x_TValue$_.__ctor(keys.get_item(i), this.get_item(keys.get_item(i)))));
  };
  ptyp_.getEnumerator = function System__Collections__Generic__StringDictionary$1__GetEnumerator() {
    return Enumerator_$TValue$_.__ctor(this);
  };
  ptyp_.getKeys = function System__Collections__Generic__StringDictionary$1__GetKeys() {
    var rv, key;
    rv = [];
    for (key in this.innerDict)
      rv.push(key);
    return rv;
  };
  ptyp_.computeCount = function System__Collections__Generic__StringDictionary$1__ComputeCount() {
    var rv, key;
    rv = 0;
    for (key in this.innerDict)
      rv++;
    return rv;
  };
  ptyp_.V_GetEnumerator_f = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_.V_get_Count_c = ptyp_.get_count;
  ptyp_.V_CopyTo_c = ptyp_.copyTo;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_.typeId] = ptyp_.getEnumerator;
  System__Type__RegisterReferenceType(StringDictionary$1_$TValue$_, "System.Collections.Generic.StringDictionary`1<" + TValue.fullName + ">", Object, [System_Collections_ICollection, IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_, System_Collections_IEnumerable]);
  StringDictionary$1_$TValue$_.$5ftri = function() {
    if ($5f_initTracker0)
      return;
    $5f_initTracker0 = true;
    TValue = TValue;
    Enumerator_$TValue$_ = System_Collections_Generic_StringDictionary_Enumerator(TValue, true);
    StringDictionary$1_$TValue$_ = System_Collections_Generic_StringDictionary(TValue, true);
  };
  if ($5fcallStatiConstructor)
    StringDictionary$1_$TValue$_.$5ftri();
  return StringDictionary$1_$TValue$_;
};
function System_Action(T1, $5fcallStatiConstructor) {
  var Action$1_$T1$_, $5f_initTracker;
  if (System_Action[T1.typeId])
    return System_Action[T1.typeId];
  System_Action[T1.typeId] = function System__Action$10() {
  };
  Action$1_$T1$_ = System_Action[T1.typeId];
  Action$1_$T1$_.genericParameters = [T1];
  Action$1_$T1$_.genericClosure = System_Action;
  Action$1_$T1$_.typeId = "kc$" + T1.typeId + "$";
  Action$1_$T1$_.prototype = new Function0();
  System__Type__RegisterReferenceType(Action$1_$T1$_, "System.Action`1<" + T1.fullName + ">", Function0, []);
  Action$1_$T1$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T1 = T1;
    Action$1_$T1$_ = System_Action(T1, true);
  };
  if ($5fcallStatiConstructor)
    Action$1_$T1$_.$5ftri();
  return Action$1_$T1$_;
};
function System_Action0(T1, T2, $5fcallStatiConstructor) {
  var Action$2_$T1_x_T2$_, $5f_initTracker;
  if (System_Action0[T1.typeId] && System_Action0[T1.typeId][T2.typeId])
    return System_Action0[T1.typeId][T2.typeId];
    System_Action0[T1.typeId] = {
    };
  System_Action0[T1.typeId][T2.typeId] = function System__Action$20() {
  };
  Action$2_$T1_x_T2$_ = System_Action0[T1.typeId][T2.typeId];
  Action$2_$T1_x_T2$_.genericParameters = [T1, T2];
  Action$2_$T1_x_T2$_.genericClosure = System_Action0;
  Action$2_$T1_x_T2$_.typeId = "lc$" + T1.typeId + "_" + T2.typeId + "$";
  Action$2_$T1_x_T2$_.prototype = new Function0();
  System__Type__RegisterReferenceType(Action$2_$T1_x_T2$_, "System.Action`2<" + T1.fullName + "," + T2.fullName + ">", Function0, []);
  Action$2_$T1_x_T2$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T1 = T1;
    T2 = T2;
    Action$2_$T1_x_T2$_ = System_Action0(T1, T2, true);
  };
  if ($5fcallStatiConstructor)
    Action$2_$T1_x_T2$_.$5ftri();
  return Action$2_$T1_x_T2$_;
};
function System_Collections_Generic_KeyValuePair(K, V, $5fcallStatiConstructor) {
  var KeyValuePair$2_$K_x_V$_, $5f_initTracker;
  if (System_Collections_Generic_KeyValuePair[K.typeId] && System_Collections_Generic_KeyValuePair[K.typeId][V.typeId])
    return System_Collections_Generic_KeyValuePair[K.typeId][V.typeId];
    System_Collections_Generic_KeyValuePair[K.typeId] = {
    };
  System_Collections_Generic_KeyValuePair[K.typeId][V.typeId] = function(boxedValue) {
    this.boxedValue = boxedValue;
  };
  KeyValuePair$2_$K_x_V$_ = System_Collections_Generic_KeyValuePair[K.typeId][V.typeId];
  KeyValuePair$2_$K_x_V$_.genericParameters = [K, V];
  KeyValuePair$2_$K_x_V$_.genericClosure = System_Collections_Generic_KeyValuePair;
  KeyValuePair$2_$K_x_V$_.typeId = "mc$" + K.typeId + "_" + V.typeId + "$";
  KeyValuePair$2_$K_x_V$_.getDefaultValue = function() {
    return {
      key: System__Type__GetDefaultValueStatic(K),
      val: System__Type__GetDefaultValueStatic(V)
    };
  };
  KeyValuePair$2_$K_x_V$_.__ctor = function System__Collections__Generic__KeyValuePair$2____ctor(key, value) {
    var this_;
    this_ = KeyValuePair$2_$K_x_V$_.getDefaultValue();
    this_.key = key;
    this_.val = value;
    return this_;
  };
  KeyValuePair$2_$K_x_V$_.get_key = function System__Collections__Generic__KeyValuePair$2__get_Key(this_) {
    return this_.key;
  };
  KeyValuePair$2_$K_x_V$_.prototype = new System_ValueType();
  System__Type__RegisterStructType(KeyValuePair$2_$K_x_V$_, "System.Collections.Generic.KeyValuePair`2<" + K.fullName + "," + V.fullName + ">", []);
  KeyValuePair$2_$K_x_V$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    KeyValuePair$2_$K_x_V$_ = System_Collections_Generic_KeyValuePair(K, V, true);
    K = K;
    V = V;
  };
  if ($5fcallStatiConstructor)
    KeyValuePair$2_$K_x_V$_.$5ftri();
  return KeyValuePair$2_$K_x_V$_;
};
function System_Collections_Generic_NumberDictionary_Enumerator(TValue, $5fcallStatiConstructor) {
  var KeyValuePair$2_$Number_x_TValue$_, Enumerator_$TValue$_, IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_, $5f_initTracker;
  if (System_Collections_Generic_NumberDictionary_Enumerator[TValue.typeId])
    return System_Collections_Generic_NumberDictionary_Enumerator[TValue.typeId];
  System_Collections_Generic_NumberDictionary_Enumerator[TValue.typeId] = function System__Collections__Generic__NumberDictionary$1$2fEnumerator0() {
  };
  Enumerator_$TValue$_ = System_Collections_Generic_NumberDictionary_Enumerator[TValue.typeId];
  Enumerator_$TValue$_.genericParameters = [TValue];
  Enumerator_$TValue$_.genericClosure = System_Collections_Generic_NumberDictionary_Enumerator;
  Enumerator_$TValue$_.typeId = "nc$" + TValue.typeId + "$";
  KeyValuePair$2_$Number_x_TValue$_ = System_Collections_Generic_KeyValuePair(Number, TValue, $5fcallStatiConstructor);
  IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_ = System_Collections_Generic_IEnumerator(System_Collections_Generic_KeyValuePair(Number, TValue, $5fcallStatiConstructor), $5fcallStatiConstructor);
  Enumerator_$TValue$_.__ctor = function System_Collections_Generic_NumberDictionary$1$2fEnumerator_factory0(dict) {
    var this_;
    this_ = new Enumerator_$TValue$_();
    this_.__ctor(dict);
    return this_;
  };
  ptyp_ = Enumerator_$TValue$_.prototype;
  ptyp_.dict = null;
  ptyp_.keys = null;
  ptyp_.system__Collections__IEnumerator__get_Current = function System__Collections__Generic__NumberDictionary$1$2fEnumerator__System__Collections__IEnumerator__get_Current() {
    return System__Type__BoxTypeInstance(KeyValuePair$2_$Number_x_TValue$_, this.get_current());
  };
  ptyp_.__ctor = function System__Collections__Generic__NumberDictionary$1$2fEnumerator____ctor(dict) {
    this.dict = dict;
    this.keys = this.dict.get_keys().V_GetEnumerator_k$l$();
  };
  ptyp_.get_current = function System__Collections__Generic__NumberDictionary$1$2fEnumerator__get_Current() {
    return KeyValuePair$2_$Number_x_TValue$_.__ctor(this.keys.V_get_Current_n$l$(), this.dict.get_item(this.keys.V_get_Current_n$l$()));
  };
  ptyp_.moveNext = function System__Collections__Generic__NumberDictionary$1$2fEnumerator__MoveNext() {
    return this.keys.V_MoveNext_g();
  };
  ptyp_.V_get_Current_g = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_g = ptyp_.moveNext;
  System__Type__RegisterReferenceType(Enumerator_$TValue$_, "System.Collections.Generic.NumberDictionary`1/Enumerator<" + TValue.fullName + ">", Object, [IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_, System_Collections_IEnumerator]);
  Enumerator_$TValue$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    TValue = TValue;
    Enumerator_$TValue$_ = System_Collections_Generic_NumberDictionary_Enumerator(TValue, true);
  };
  if ($5fcallStatiConstructor)
    Enumerator_$TValue$_.$5ftri();
  return Enumerator_$TValue$_;
};
function System_Collections_Generic_Queue_QueueEnumerator(T, $5fcallStatiConstructor) {
  var QueueEnumerator_$T$_, IEnumerator$1_$T$_, $5f_initTracker;
  if (System_Collections_Generic_Queue_QueueEnumerator[T.typeId])
    return System_Collections_Generic_Queue_QueueEnumerator[T.typeId];
  System_Collections_Generic_Queue_QueueEnumerator[T.typeId] = function System__Collections__Generic__Queue$1$2fQueueEnumerator0() {
  };
  QueueEnumerator_$T$_ = System_Collections_Generic_Queue_QueueEnumerator[T.typeId];
  QueueEnumerator_$T$_.genericParameters = [T];
  QueueEnumerator_$T$_.genericClosure = System_Collections_Generic_Queue_QueueEnumerator;
  QueueEnumerator_$T$_.typeId = "oc$" + T.typeId + "$";
  IEnumerator$1_$T$_ = System_Collections_Generic_IEnumerator(T, $5fcallStatiConstructor);
  QueueEnumerator_$T$_.__ctor = function System_Collections_Generic_Queue$1$2fQueueEnumerator_factory0(queue) {
    var this_;
    this_ = new QueueEnumerator_$T$_();
    this_.__ctor(queue);
    return this_;
  };
  ptyp_ = QueueEnumerator_$T$_.prototype;
  ptyp_.queue = null;
  ptyp_.currentIndex = 0;
  ptyp_.system__Collections__IEnumerator__get_Current = function System__Collections__Generic__Queue$1$2fQueueEnumerator__System__Collections__IEnumerator__get_Current() {
    return System__Type__BoxTypeInstance(T, this.get_current());
  };
  ptyp_.__ctor = function System__Collections__Generic__Queue$1$2fQueueEnumerator____ctor(queue) {
    this.queue = queue;
    this.currentIndex = -1;
  };
  ptyp_.get_current = function System__Collections__Generic__Queue$1$2fQueueEnumerator__get_Current() {
    if (this.currentIndex < 0 || this.currentIndex >= this.queue.nativeArray.length)
      throw new Error("Out of range");
    return this.queue.nativeArray[this.currentIndex];
  };
  ptyp_.moveNext = function System__Collections__Generic__Queue$1$2fQueueEnumerator__MoveNext() {
    this.currentIndex++;
    return this.currentIndex < this.queue.nativeArray.length;
  };
  ptyp_.V_get_Current_g = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$T$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_g = ptyp_.moveNext;
  System__Type__RegisterReferenceType(QueueEnumerator_$T$_, "System.Collections.Generic.Queue`1/QueueEnumerator<" + T.fullName + ">", Object, [IEnumerator$1_$T$_, System_Collections_IEnumerator]);
  QueueEnumerator_$T$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    QueueEnumerator_$T$_ = System_Collections_Generic_Queue_QueueEnumerator(T, true);
  };
  if ($5fcallStatiConstructor)
    QueueEnumerator_$T$_.$5ftri();
  return QueueEnumerator_$T$_;
};
function System_Collections_Generic_ListEnumerator(T, $5fcallStatiConstructor) {
  var IList$1_$T$_, ListEnumerator$1_$T$_, IEnumerator$1_$T$_, $5f_initTracker, $5f_initTracker0;
  if (System_Collections_Generic_ListEnumerator[T.typeId])
    return System_Collections_Generic_ListEnumerator[T.typeId];
  System_Collections_Generic_ListEnumerator[T.typeId] = function System__Collections__Generic__ListEnumerator$10() {
  };
  ListEnumerator$1_$T$_ = System_Collections_Generic_ListEnumerator[T.typeId];
  ListEnumerator$1_$T$_.genericParameters = [T];
  ListEnumerator$1_$T$_.genericClosure = System_Collections_Generic_ListEnumerator;
  ListEnumerator$1_$T$_.typeId = "pc$" + T.typeId + "$";
  IEnumerator$1_$T$_ = System_Collections_Generic_IEnumerator(T, $5fcallStatiConstructor);
  ListEnumerator$1_$T$_.__ctor = function System_Collections_Generic_ListEnumerator$1_factory0(list) {
    var this_;
    this_ = new ListEnumerator$1_$T$_();
    this_.__ctor(list);
    return this_;
  };
  ptyp_ = ListEnumerator$1_$T$_.prototype;
  ptyp_.innerList = null;
  ptyp_.index = 0;
  ptyp_.system__Collections__IEnumerator__get_Current = function System__Collections__Generic__ListEnumerator$1__System__Collections__IEnumerator__get_Current() {
    return System__Type__BoxTypeInstance(T, this.get_current());
  };
  ptyp_.__ctor = function System__Collections__Generic__ListEnumerator$1____ctor(list) {
    this.index = -1;
    this.innerList = list;
  };
  ptyp_.get_current = function System__Collections__Generic__ListEnumerator$1__get_Current() {
    return this.innerList["V_get_Item_" + IList$1_$T$_.typeId](this.index);
  };
  ptyp_.moveNext = function System__Collections__Generic__ListEnumerator$1__MoveNext() {
    return ++this.index < this.innerList.V_get_Count_c();
  };
  ptyp_.V_get_Current_g = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$T$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_g = ptyp_.moveNext;
  System__Type__RegisterReferenceType(ListEnumerator$1_$T$_, "System.Collections.Generic.ListEnumerator`1<" + T.fullName + ">", Object, [IEnumerator$1_$T$_, System_Collections_IEnumerator]);
  ListEnumerator$1_$T$_.$5ftri = function() {
    if ($5f_initTracker0)
      return;
    $5f_initTracker0 = true;
    T = T;
    IList$1_$T$_ = System_Collections_Generic_IList(T, true);
    ListEnumerator$1_$T$_ = System_Collections_Generic_ListEnumerator(T, true);
  };
  if ($5fcallStatiConstructor)
    ListEnumerator$1_$T$_.$5ftri();
  return ListEnumerator$1_$T$_;
};
function System_Collections_Generic_StringDictionary_Enumerator(TValue, $5fcallStatiConstructor) {
  var KeyValuePair$2_$String_x_TValue$_, Enumerator_$TValue$_, IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_, $5f_initTracker;
  if (System_Collections_Generic_StringDictionary_Enumerator[TValue.typeId])
    return System_Collections_Generic_StringDictionary_Enumerator[TValue.typeId];
  System_Collections_Generic_StringDictionary_Enumerator[TValue.typeId] = function System__Collections__Generic__StringDictionary$1$2fEnumerator0() {
  };
  Enumerator_$TValue$_ = System_Collections_Generic_StringDictionary_Enumerator[TValue.typeId];
  Enumerator_$TValue$_.genericParameters = [TValue];
  Enumerator_$TValue$_.genericClosure = System_Collections_Generic_StringDictionary_Enumerator;
  Enumerator_$TValue$_.typeId = "qc$" + TValue.typeId + "$";
  KeyValuePair$2_$String_x_TValue$_ = System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor);
  IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_ = System_Collections_Generic_IEnumerator(System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor), $5fcallStatiConstructor);
  Enumerator_$TValue$_.__ctor = function System_Collections_Generic_StringDictionary$1$2fEnumerator_factory0(dict) {
    var this_;
    this_ = new Enumerator_$TValue$_();
    this_.__ctor(dict);
    return this_;
  };
  ptyp_ = Enumerator_$TValue$_.prototype;
  ptyp_.dict = null;
  ptyp_.keys = null;
  ptyp_.system__Collections__IEnumerator__get_Current = function System__Collections__Generic__StringDictionary$1$2fEnumerator__System__Collections__IEnumerator__get_Current() {
    return System__Type__BoxTypeInstance(KeyValuePair$2_$String_x_TValue$_, this.get_current());
  };
  ptyp_.__ctor = function System__Collections__Generic__StringDictionary$1$2fEnumerator____ctor(dict) {
    this.dict = dict;
    this.keys = this.dict.get_keys().V_GetEnumerator_k$m$();
  };
  ptyp_.get_current = function System__Collections__Generic__StringDictionary$1$2fEnumerator__get_Current() {
    return KeyValuePair$2_$String_x_TValue$_.__ctor(this.keys.V_get_Current_n$m$(), this.dict.get_item(this.keys.V_get_Current_n$m$()));
  };
  ptyp_.moveNext = function System__Collections__Generic__StringDictionary$1$2fEnumerator__MoveNext() {
    return this.keys.V_MoveNext_g();
  };
  ptyp_.V_get_Current_g = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_g = ptyp_.moveNext;
  System__Type__RegisterReferenceType(Enumerator_$TValue$_, "System.Collections.Generic.StringDictionary`1/Enumerator<" + TValue.fullName + ">", Object, [IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_, System_Collections_IEnumerator]);
  Enumerator_$TValue$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    TValue = TValue;
    Enumerator_$TValue$_ = System_Collections_Generic_StringDictionary_Enumerator(TValue, true);
  };
  if ($5fcallStatiConstructor)
    Enumerator_$TValue$_.$5ftri();
  return Enumerator_$TValue$_;
};
function System_Collections_Generic_IEnumerable(T, $5fcallStatiConstructor) {
  var IEnumerable$1_$T$_, $5f_initTracker;
  if (System_Collections_Generic_IEnumerable[T.typeId])
    return System_Collections_Generic_IEnumerable[T.typeId];
  System_Collections_Generic_IEnumerable[T.typeId] = function System__Collections__Generic__IEnumerable$10() {
  };
  IEnumerable$1_$T$_ = System_Collections_Generic_IEnumerable[T.typeId];
  IEnumerable$1_$T$_.genericParameters = [T];
  IEnumerable$1_$T$_.genericClosure = System_Collections_Generic_IEnumerable;
  IEnumerable$1_$T$_.typeId = "k$" + T.typeId + "$";
  System__Type__RegisterInterface(IEnumerable$1_$T$_, "System.Collections.Generic.IEnumerable`1<" + T.fullName + ">");
  IEnumerable$1_$T$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    IEnumerable$1_$T$_ = System_Collections_Generic_IEnumerable(T, true);
  };
  if ($5fcallStatiConstructor)
    IEnumerable$1_$T$_.$5ftri();
  return IEnumerable$1_$T$_;
};
function System_ArrayG_Enumerator(T, $5fcallStatiConstructor) {
  var Enumerator_$T$_, IEnumerator$1_$T$_, $5f_initTracker;
  if (System_ArrayG_Enumerator[T.typeId])
    return System_ArrayG_Enumerator[T.typeId];
  System_ArrayG_Enumerator[T.typeId] = function System__ArrayG$1$2fEnumerator0() {
  };
  Enumerator_$T$_ = System_ArrayG_Enumerator[T.typeId];
  Enumerator_$T$_.genericParameters = [T];
  Enumerator_$T$_.genericClosure = System_ArrayG_Enumerator;
  Enumerator_$T$_.typeId = "rc$" + T.typeId + "$";
  IEnumerator$1_$T$_ = System_Collections_Generic_IEnumerator(T, $5fcallStatiConstructor);
  Enumerator_$T$_.__ctor = function System_ArrayG$1$2fEnumerator_factory0(array) {
    var this_;
    this_ = new Enumerator_$T$_();
    this_.__ctor(array);
    return this_;
  };
  ptyp_ = Enumerator_$T$_.prototype;
  ptyp_.currentIndex = 0;
  ptyp_.array = null;
  ptyp_.system__Collections__IEnumerator__get_Current = function System__ArrayG$1$2fEnumerator__System__Collections__IEnumerator__get_Current() {
    return System__Type__BoxTypeInstance(T, this.get_current());
  };
  ptyp_.__ctor = function System__ArrayG$1$2fEnumerator____ctor(array) {
    this.currentIndex = -1;
    this.array = array;
  };
  ptyp_.moveNext = function System__ArrayG$1$2fEnumerator__MoveNext() {
    return ++this.currentIndex < this.array.V_get_Length();
  };
  ptyp_.get_current = function System__ArrayG$1$2fEnumerator__get_Current() {
    return this.array.get_item(this.currentIndex);
  };
  ptyp_.V_get_Current_g = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$T$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_g = ptyp_.moveNext;
  System__Type__RegisterReferenceType(Enumerator_$T$_, "System.ArrayG`1/Enumerator<" + T.fullName + ">", Object, [IEnumerator$1_$T$_, System_Collections_IEnumerator]);
  Enumerator_$T$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    Enumerator_$T$_ = System_ArrayG_Enumerator(T, true);
  };
  if ($5fcallStatiConstructor)
    Enumerator_$T$_.$5ftri();
  return Enumerator_$T$_;
};
function System_Collections_Generic_IEnumerator(T, $5fcallStatiConstructor) {
  var IEnumerator$1_$T$_, $5f_initTracker;
  if (System_Collections_Generic_IEnumerator[T.typeId])
    return System_Collections_Generic_IEnumerator[T.typeId];
  System_Collections_Generic_IEnumerator[T.typeId] = function System__Collections__Generic__IEnumerator$10() {
  };
  IEnumerator$1_$T$_ = System_Collections_Generic_IEnumerator[T.typeId];
  IEnumerator$1_$T$_.genericParameters = [T];
  IEnumerator$1_$T$_.genericClosure = System_Collections_Generic_IEnumerator;
  IEnumerator$1_$T$_.typeId = "n$" + T.typeId + "$";
  System__Type__RegisterInterface(IEnumerator$1_$T$_, "System.Collections.Generic.IEnumerator`1<" + T.fullName + ">");
  IEnumerator$1_$T$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    IEnumerator$1_$T$_ = System_Collections_Generic_IEnumerator(T, true);
  };
  if ($5fcallStatiConstructor)
    IEnumerator$1_$T$_.$5ftri();
  return IEnumerator$1_$T$_;
};
function System_Collections_Generic_IList(T, $5fcallStatiConstructor) {
  var IList$1_$T$_, IEnumerable$1_$T$_, $5f_initTracker;
  if (System_Collections_Generic_IList[T.typeId])
    return System_Collections_Generic_IList[T.typeId];
  System_Collections_Generic_IList[T.typeId] = function System__Collections__Generic__IList$10() {
  };
  IList$1_$T$_ = System_Collections_Generic_IList[T.typeId];
  IList$1_$T$_.genericParameters = [T];
  IList$1_$T$_.genericClosure = System_Collections_Generic_IList;
  IList$1_$T$_.typeId = "sc$" + T.typeId + "$";
  IEnumerable$1_$T$_ = System_Collections_Generic_IEnumerable(T, $5fcallStatiConstructor);
  System__Type__RegisterInterface(IList$1_$T$_, "System.Collections.Generic.IList`1<" + T.fullName + ">");
  IList$1_$T$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    IList$1_$T$_ = System_Collections_Generic_IList(T, true);
  };
  if ($5fcallStatiConstructor)
    IList$1_$T$_.$5ftri();
  return IList$1_$T$_;
};
Sunlight_Framework_UI_Test_ValueIfTrue_$String$_ = Sunlight_Framework_UI_Test_ValueIfTrue(String);
System_Func_$Object_x_Object$_ = System_Func(Object, Object);
System_ArrayG_$Func_$Object_x_Object$_$_ = System_ArrayG(System_Func_$Object_x_Object$_);
System_ArrayG_$ArrayG_$Func_$Object_x_Object$_$_$_ = System_ArrayG(System_ArrayG_$Func_$Object_x_Object$_$_);
System_ArrayG_$String$_ = System_ArrayG(String);
System_ArrayG_$ArrayG_$String$_$_ = System_ArrayG(System_ArrayG_$String$_);
System_ArrayG_$SkinBinderInfo$_ = System_ArrayG(Sunlight_Framework_UI_Helpers_SkinBinderInfo);
System_ArrayG_$Object$_ = System_ArrayG(Object);
System_ArrayG_$TestViewModelB$_ = System_ArrayG(Sunlight_Framework_UI_Test_TestViewModelB);
System_Collections_Generic_KeyValuePair_$Number_x_Task$_ = System_Collections_Generic_KeyValuePair(Number, Sunlight_Framework_Task);
System_Collections_Generic_NumberDictionary_$Task$_ = System_Collections_Generic_NumberDictionary(Sunlight_Framework_Task);
System_Collections_Generic_Queue_$Task$_ = System_Collections_Generic_Queue(Sunlight_Framework_Task);
System_Collections_Generic_List_$ListViewItem$_ = System_Collections_Generic_List(Sunlight_Framework_UI_ListViewItem);
System_Action_$UIEvent$_ = System_Action(Sunlight_Framework_UI_UIEvent);
System_Collections_Generic_KeyValuePair_$String_x_Action_$UIEvent$_$_ = System_Collections_Generic_KeyValuePair(String, System_Action_$UIEvent$_);
System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_ = System_Collections_Generic_StringDictionary(System_Action_$UIEvent$_);
System_Action_$INotifyPropertyChanged_x_String$_ = System_Action0(Sunlight_Framework_Observables_INotifyPropertyChanged, String);
System_Collections_Generic_KeyValuePair_$String_x_Action_$INotifyPropertyChanged_x_String$_$_ = System_Collections_Generic_KeyValuePair(String, System_Action_$INotifyPropertyChanged_x_String$_);
System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_ = System_Collections_Generic_StringDictionary(System_Action_$INotifyPropertyChanged_x_String$_);
System_Collections_Generic_KeyValuePair_$String_x_Function$_ = System_Collections_Generic_KeyValuePair(String, Function);
System_Collections_Generic_StringDictionary_$Function$_ = System_Collections_Generic_StringDictionary(Function);
System_ArrayG_$Number$_ = System_ArrayG(Number);
System_Collections_Generic_KeyValuePair_$String_x_Int32$_ = System_Collections_Generic_KeyValuePair(String, System_Int32);
System_Collections_Generic_StringDictionary_$Int32$_ = System_Collections_Generic_StringDictionary(System_Int32);
Sunlight__Framework__UI__Test__ManualTemplateTests____cctor();
System__String____cctor();
System__DateTime____cctor();
Sunlight_Framework_UI_Test_ValueIfTrue_$String$_.$5ftri();
System_Func_$Object_x_Object$_.$5ftri();
System_ArrayG_$Func_$Object_x_Object$_$_.$5ftri();
System_ArrayG_$String$_.$5ftri();
System_ArrayG_$SkinBinderInfo$_.$5ftri();
System_ArrayG_$Object$_.$5ftri();
System_ArrayG_$TestViewModelB$_.$5ftri();
System_Collections_Generic_KeyValuePair_$Number_x_Task$_.$5ftri();
System_Collections_Generic_NumberDictionary_$Task$_.$5ftri();
System_Collections_Generic_Queue_$Task$_.$5ftri();
System_Collections_Generic_List_$ListViewItem$_.$5ftri();
System_Action_$UIEvent$_.$5ftri();
System_Collections_Generic_KeyValuePair_$String_x_Action_$UIEvent$_$_.$5ftri();
System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_.$5ftri();
System_Action_$INotifyPropertyChanged_x_String$_.$5ftri();
System_Collections_Generic_KeyValuePair_$String_x_Action_$INotifyPropertyChanged_x_String$_$_.$5ftri();
System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_.$5ftri();
System_Collections_Generic_KeyValuePair_$String_x_Function$_.$5ftri();
System_Collections_Generic_StringDictionary_$Function$_.$5ftri();
System_ArrayG_$Number$_.$5ftri();
System_Collections_Generic_KeyValuePair_$String_x_Int32$_.$5ftri();
System_Collections_Generic_StringDictionary_$Int32$_.$5ftri();
module("Sunlight.Framework.UI.Test.LiveBinderTests", {
  "setup": Sunlight__Framework__UI__Test__LiveBinderTests__Setup
});
test("TestLiveBinderOnActivate", 0, Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderOnActivate);
test("TestLiveBinderOnChange", 0, Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderOnChange);
test("TestLiveBinderMultiOnActivate", 0, Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderMultiOnActivate);
test("TestLiveBinderMultiOnChange", 0, Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderMultiOnChange);
test("TestTwoWayLiveBinderOnChange", 0, Sunlight__Framework__UI__Test__LiveBinderTests__TestTwoWayLiveBinderOnChange);
test("TestTwoWayLiveBinderMultiOnChangeWithConverters", 0, Sunlight__Framework__UI__Test__LiveBinderTests__TestTwoWayLiveBinderMultiOnChangeWithConverters);
module("Sunlight.Framework.UI.Test.NScriptsTemplateTests", {
  "setup": Sunlight__Framework__UI__Test__NScriptsTemplateTests__Setup
});
test("Test", 0, Sunlight__Framework__UI__Test__NScriptsTemplateTests__Test);
test("TestApplySkin", 0, Sunlight__Framework__UI__Test__NScriptsTemplateTests__TestApplySkin);
test("TestCssBinder", 0, Sunlight__Framework__UI__Test__NScriptsTemplateTests__TestCssBinder);
test("TestStyleBinder", 0, Sunlight__Framework__UI__Test__NScriptsTemplateTests__TestStyleBinder);
test("TestAttrBinder", 0, Sunlight__Framework__UI__Test__NScriptsTemplateTests__TestAttrBinder);
test("TestPropertyBinder", 0, Sunlight__Framework__UI__Test__NScriptsTemplateTests__TestPropertyBinder);
module("Sunlight.Framework.UI.Test.SkinBinderHelperTests", {
});
test("TestSimpleBinder", 0, Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestSimpleBinder);
test("TestAttrBinding", 0, Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestAttrBinding);
test("TestStyleBinding", 0, Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestStyleBinding);
test("TestTextContentBinding", 0, Sunlight__Framework__UI__Test__SkinBinderHelperTests__TestTextContentBinding);
module("Sunlight.Framework.UI.Test.TestListView", {
  "setup": Sunlight__Framework__UI__Test__TestListView__Setup
});
test("TestChildSkin", 0, Sunlight__Framework__UI__Test__TestListView__TestChildSkin);
module("Sunlight.Framework.UI.Test.ManualTemplateTests", {
  "setup": Sunlight__Framework__UI__Test__ManualTemplateTests__Setup
});
test("Test", 0, Sunlight__Framework__UI__Test__ManualTemplateTests__Test);
module("Sunlight.Framework.UI.Test.UIElementTests", {
});
test("TestNewUIElement", 0, Sunlight__Framework__UI__Test__UIElementTests__TestNewUIElement);
function Test$5cTemplates$5cTestTemplate1_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div test=\"test me\"></div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = [];
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
};
Test$5cTemplates$5cTestTemplate1_var = null;
function Test$5cTemplates$5cTestTemplate1() {
  if (!Test$5cTemplates$5cTestTemplate1_var)
    Test$5cTemplates$5cTestTemplate1_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelA, Test$5cTemplates$5cTestTemplate1_factory, "0");
  return Test$5cTemplates$5cTestTemplate1_var;
};
function getter(src) {
  return src.get_propStr1();
};
function getter0(src) {
  return src.get_propBool1();
};
function Test$5cTemplates$5cTestTemplateVMB_CssBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[1]) {
    domStore[1] = doc.createElement("div");
    domStore[1].innerHTML = " <div test=\"id\"></div> ";
    tmplStore[1] = tmplStore[1] ? tmplStore[1] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0([getter], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 17, 0, null, ""), Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0([getter0], ["PropBool1"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetCssClass, "black", 81, 0, 0, null, false, 0)];
  }
  htmlRoot = domStore[1].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[1], null, 1, 0);
};
Test$5cTemplates$5cTestTemplateVMB_CssBinding_var = null;
function Test$5cTemplates$5cTestTemplateVMB_CssBinding() {
  if (!Test$5cTemplates$5cTestTemplateVMB_CssBinding_var)
    Test$5cTemplates$5cTestTemplateVMB_CssBinding_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelB, Test$5cTemplates$5cTestTemplateVMB_CssBinding_factory, "1");
  return Test$5cTemplates$5cTestTemplateVMB_CssBinding_var;
};
function styleSetter(dom, val) {
  dom.style.height = val;
};
function Test$5cTemplates$5cTestTemplateVMB_StyleBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[2]) {
    domStore[2] = doc.createElement("div");
    domStore[2].innerHTML = " <div test=\"test me\">T1</div> ";
    tmplStore[2] = tmplStore[2] ? tmplStore[2] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], ["PropStr1"], styleSetter, 97, 0, 0, null, null)];
  }
  htmlRoot = domStore[2].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[2], null, 1, 0);
};
Test$5cTemplates$5cTestTemplateVMB_StyleBinding_var = null;
function Test$5cTemplates$5cTestTemplateVMB_StyleBinding() {
  if (!Test$5cTemplates$5cTestTemplateVMB_StyleBinding_var)
    Test$5cTemplates$5cTestTemplateVMB_StyleBinding_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelB, Test$5cTemplates$5cTestTemplateVMB_StyleBinding_factory, "2");
  return Test$5cTemplates$5cTestTemplateVMB_StyleBinding_var;
};
function Test$5cTemplates$5cTestTemplateVMB_AttrBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[3]) {
    domStore[3] = doc.createElement("div");
    domStore[3].innerHTML = " <div test=\"foo\">Test Me</div> ";
    tmplStore[3] = tmplStore[3] ? tmplStore[3] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0([getter], ["PropStr1"], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetAttribute, "test1", 113, 0, 0, null, null, 0)];
  }
  htmlRoot = domStore[3].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[3], null, 1, 0);
};
Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var = null;
function Test$5cTemplates$5cTestTemplateVMB_AttrBinding() {
  if (!Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var)
    Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelB, Test$5cTemplates$5cTestTemplateVMB_AttrBinding_factory, "3");
  return Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var;
};
function getter0(src) {
  return src.get_propInt1();
};
function setter(tar, val) {
  tar.set_propInt1(val);
};
function setter0(tar, val) {
  tar.set_twoWayLooseBinding(val);
};
function getter0(src) {
  return src.get_twoWayLooseBinding();
};
function setter0(tar, val) {
  tar.set_oneWayStrictBinding(val);
};
function Test$5cTemplates$5cTestTemplateB_PropertyBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[4]) {
    domStore[4] = doc.createElement("div");
    domStore[4].innerHTML = " <div> This is a test. </div> ";
    tmplStore[4] = tmplStore[4] ? tmplStore[4] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0([getter0], setter, ["PropInt1"], setter0, getter0, "TwoWayLooseBinding", 17, 0, 0, null, null, 11), Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory([getter], ["PropStr1"], setter0, 17, 0, 1, null, "test")];
  }
  htmlRoot = domStore[4].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Test__TestUIElement_factory(Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]));
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [0], objStorage, tmplStore[4], {
    "Part1": 0
  }, 2, 0);
};
Test$5cTemplates$5cTestTemplateB_PropertyBinding_var = null;
function Test$5cTemplates$5cTestTemplateB_PropertyBinding() {
  if (!Test$5cTemplates$5cTestTemplateB_PropertyBinding_var)
    Test$5cTemplates$5cTestTemplateB_PropertyBinding_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_Test_TestSkinableWithTestUIElementPart, Sunlight_Framework_UI_Test_TestViewModelB, Test$5cTemplates$5cTestTemplateB_PropertyBinding_factory, "4");
  return Test$5cTemplates$5cTestTemplateB_PropertyBinding_var;
};
function Test$5cTemplates$5cTestTemplateVMB1_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[5]) {
    domStore[5] = doc.createElement("div");
    domStore[5].innerHTML = " <div test=\"test me\"></div> ";
    tmplStore[5] = tmplStore[5] ? tmplStore[5] : [Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory0([getter], Sunlight__Framework__UI__Helpers__SkinBinderHelper__SetTextContent, 17, 0, null, "")];
  }
  htmlRoot = domStore[5].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = Sunlight__Framework__UI__Helpers__SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return Sunlight__Framework__UI__Helpers__SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[5], null, 0, 0);
};
Test$5cTemplates$5cTestTemplateVMB1_var = null;
function Test$5cTemplates$5cTestTemplateVMB1() {
  if (!Test$5cTemplates$5cTestTemplateVMB1_var)
    Test$5cTemplates$5cTestTemplateVMB1_var = Sunlight__Framework__UI__Skin_factory(Sunlight_Framework_UI_UISkinableElement, Sunlight_Framework_UI_Test_TestViewModelB, Test$5cTemplates$5cTestTemplateVMB1_factory, "5");
  return Test$5cTemplates$5cTestTemplateVMB1_var;
};
tmplStore = new Array(6);
function DocStorageGetter(doc) {
  var style;
  if (!doc.stateStore) {
    doc.stateStore = new Array(6);
    style = doc.createElement("style");
    style.textContent = ".black{background-color:black;\ncolor:white;\n}\n";
    doc.head.appendChild(style);
  }
  return doc.stateStore;
};
})();
//# sourceMappingURL=Sunlight.Framework.UI.Test.map

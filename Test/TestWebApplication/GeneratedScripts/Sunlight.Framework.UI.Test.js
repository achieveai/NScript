(function(){
var ManualTemplateTests$$noneValue, ValueIfTrue_$String$_, LiveBinderTests$$oneWayBinder, ArrayG_$Func_$Object_x_Object$_$_, ArrayG_$String$_, LiveBinderTests$$twoWayBinder, LiveBinderTests$$oneWayMultiBinder, LiveBinderTests$$twoWayMultiBinder, ArrayG_$SkinBinderInfo$_, ArrayG_$Object$_, ArrayG_$TestViewModelB$_, Type$$typeMapping, NumberDictionary_$Task$_, Queue_$Task$_, TaskScheduler$$instance, List_$ListViewItem$_, StringDictionary_$Action_$UIEvent$_$_, String$$trimStartHelperRegex, String$$trimEndHelperRegex, StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_, StringDictionary_$Function$_, KeyValuePair_$String_x_Action_$UIEvent$_$_, List_$Object$_, ArrayG_$Number$_, ptyp_, tmplStore, Test$5cTemplates$5cTestTemplate1_var, Test$5cTemplates$5cTestTemplateVMB_CssBinding_var, Test$5cTemplates$5cTestTemplateVMB_StyleBinding_var, Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var, Test$5cTemplates$5cTestTemplateB_PropertyBinding_var, Test$5cTemplates$5cTestTemplateVMB1_var, Func_$Object_x_Object$_, KeyValuePair_$Number_x_Task$_, Action_$UIEvent$_, Action_$INotifyPropertyChanged_x_String$_, KeyValuePair_$String_x_Action_$INotifyPropertyChanged_x_String$_$_, KeyValuePair_$String_x_Function$_, StringDictionary_$Int32$_, KeyValuePair_$String_x_Int32$_;
Function.typeId = "p";
Type$$typeMapping = null;
function Type$$CastType(this_, instance) {
  if (this_.isInstanceOfType(instance) || instance === null || typeof instance === "undefined") {
    if (this_.isStruct)
      return instance.boxedValue;
    return instance;
  }
  throw "InvalidCast to " + this_.fullName;
};
function Type$$ToString(this_) {
  return this_.fullName ? this_.fullName : this_.name;
};
function Type$$RegisterReferenceType(this_, typeName, parentType, interfaces) {
  this_.isClass = true;
  this_.fullName = typeName;
  this_.baseType = parentType;
  this_.interfaces = parentType ? interfaces.concat(parentType.interfaces) : interfaces;
  if (!Type$$typeMapping)
    Type$$typeMapping = {
    };
  Type$$typeMapping[this_.fullName] = this_;
};
function Type$$RegisterStructType(this_, typeName, interfaces) {
  this_.isStruct = true;
  this_.fullName = typeName;
  this_.baseType = ValueType;
  this_.interfaces = interfaces;
  if (!Type$$typeMapping)
    Type$$typeMapping = {
    };
  Type$$typeMapping[this_.fullName] = this_;
};
function Type$$RegisterInterface(this_, typeName) {
  this_.isInterface = true;
  this_.fullName = typeName;
};
function Type$$RegisterEnum(this_, typeName, isFlag) {
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
  if (!Type$$typeMapping)
    Type$$typeMapping = {
    };
  Type$$typeMapping[this_.fullName] = this_;
};
function Type$$BoxTypeInstance(type, instance) {
  if (type.isNullable)
    return type.nullableBox(instance);
  else if (type.isStruct)
    return new type(instance);
  else
    return instance;
};
function Type$$UnBoxTypeInstance(type, instance) {
  if (type.isNullable && instance === null)
    return null;
  else if (type.isStruct)
    return instance.boxedValue;
  else
    return instance;
};
function Type$$GetDefaultValueStatic(type) {
  if (type.isStruct)
    return type.getDefaultValue();
  return null;
};
function Type$$InitializeBaseInterfaces(type) {
  var rv, baseType, baseInterfaces, key, interfaces;
  if (!type.baseInterfaces) {
    rv = {
    };
    baseType = type.baseType;
    if (baseType != null && baseType != Object) {
      if (baseType)
        Type$$InitializeBaseInterfaces(type);
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
ptyp_.isInstanceOfType = function Type$$IsInstanceOfType(instance) {
  if (instance === null || typeof instance === "undefined")
    return false;
  if (!this.isInterface)
    return instance instanceof this || instance !== null && instance.constructor == this;
  else if (!instance.constructor.baseInterfaces)
    Type$$InitializeBaseInterfaces(instance.constructor);
  return instance.constructor.baseInterfaces && instance.constructor.baseInterfaces[this.fullName];
};
ptyp_.defaultConstructor = function Type$$DefaultConstructor() {
  if (this.isStruct)
    return this.getDefaultValue;
  throw "Default constructor not implemented";
};
ptyp_.getDefaultValue = function Type$$GetDefaultValue() {
  return null;
};
ptyp_.nullableBox = function Type$$NullableBox(instance) {
  return null;
};
ptyp_.toString = function() {
  return Type$$ToString(this);
};
Type$$RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "q";
function Object$$IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
function Object$$GetNewImportedExtension() {
  return {
    "toJSON": Object$$NoReturn
  };
};
function Object$$NoReturn() {
  return undefined;
};
Type$$RegisterReferenceType(Object, "System.Object", null, []);
function LiveBinderTests() {
};
LiveBinderTests.typeId = "r";
LiveBinderTests$$oneWayBinder = null;
LiveBinderTests$$twoWayBinder = null;
LiveBinderTests$$oneWayMultiBinder = null;
LiveBinderTests$$twoWayMultiBinder = null;
function LiveBinderTests$$Setup() {
  LiveBinderTests$$oneWayBinder = SkinBinderInfo_factory(NativeArray$1$$op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([function LiveBinderTests$24$24Setup_del(obj) {
    return Type$$CastType(TestViewModelA, obj).get_propStr1();
  }])), NativeArray$1$$op_Implicit(ArrayG_$String$_.__ctor(["PropStr1"])), function LiveBinderTests$24$24Setup_del2(tar, val) {
    Type$$CastType(TestViewModelA, tar).set_propStr1(Type$$CastType(String, val));
  }, 17, 0, 0, null, null);
  LiveBinderTests$$twoWayBinder = SkinBinderInfo_factory0(NativeArray$1$$op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([function LiveBinderTests$24$24Setup_del3(obj) {
    return Type$$CastType(TestViewModelA, obj).get_propStr1();
  }])), function LiveBinderTests$24$24Setup_del4(tar, val) {
    Type$$CastType(TestViewModelA, tar).set_propStr1(Type$$CastType(String, val));
  }, NativeArray$1$$op_Implicit(ArrayG_$String$_.__ctor(["PropStr1"])), function LiveBinderTests$24$24Setup_del5(tar, val) {
    Type$$CastType(TestViewModelA, tar).set_propStr1(Type$$CastType(String, val));
  }, function LiveBinderTests$24$24Setup_del6(obj) {
    return Type$$CastType(TestViewModelA, obj).get_propStr1();
  }, "PropStr1", 17, 0, 0, null, null, null);
  LiveBinderTests$$oneWayMultiBinder = SkinBinderInfo_factory(NativeArray$1$$op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([function LiveBinderTests$24$24Setup_del7(obj) {
    return Type$$CastType(TestViewModelA, obj).get_testVMA();
  }, function LiveBinderTests$24$24Setup_del8(obj) {
    return Type$$CastType(TestViewModelA, obj).get_propStr1();
  }])), NativeArray$1$$op_Implicit(ArrayG_$String$_.__ctor(["TestVMA", "PropStr1"])), function LiveBinderTests$24$24Setup_del9(tar, val) {
    Type$$CastType(TestViewModelA, tar).set_propStr1(Type$$CastType(String, val));
  }, 17, 0, 0, null, null);
  LiveBinderTests$$twoWayMultiBinder = SkinBinderInfo_factory0(NativeArray$1$$op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([function LiveBinderTests$24$24Setup_del10(obj) {
    return Type$$CastType(TestViewModelA, obj).get_testVMA();
  }, function LiveBinderTests$24$24Setup_del11(obj) {
    return Type$$BoxTypeInstance(Int32, Type$$CastType(TestViewModelA, obj).get_propInt1());
  }])), function LiveBinderTests$24$24Setup_del12(tar, val) {
    Type$$CastType(TestViewModelA, tar).set_propInt1(Type$$UnBoxTypeInstance(Int32, val));
  }, NativeArray$1$$op_Implicit(ArrayG_$String$_.__ctor(["TestVMA", "PropInt1"])), function LiveBinderTests$24$24Setup_del13(tar, val) {
    Type$$CastType(TestViewModelA, tar).set_propStr1(Type$$CastType(String, val));
  }, function LiveBinderTests$24$24Setup_del14(obj) {
    return Type$$CastType(TestViewModelA, obj).get_propStr1();
  }, "PropStr1", 17, 0, 0, function LiveBinderTests$24$24Setup_del15(obj) {
    return obj.toString();
  }, function LiveBinderTests$24$24Setup_del16(obj) {
    return Type$$BoxTypeInstance(Int32, Int32$$Parse(Type$$CastType(String, obj)));
  }, null);
};
function LiveBinderTests$$TestLiveBinderOnActivate(assert) {
  var src, target, liveBinder;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_propStr1("test");
  liveBinder = LiveBinder_factory(LiveBinderTests$$oneWayBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  assert.notEqual(src.get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  assert.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
};
function LiveBinderTests$$TestLiveBinderOnChange(assert) {
  var src, target, liveBinder;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_propStr1("test");
  liveBinder = LiveBinder_factory(LiveBinderTests$$oneWayBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  assert.notEqual(src.get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  assert.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  src.set_propStr1("testChanged");
  assert.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
};
function LiveBinderTests$$TestLiveBinderMultiOnActivate(assert) {
  var src, target, liveBinder;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_testVMA(TestViewModelA_factory());
  src.get_testVMA().set_propStr1("test");
  liveBinder = LiveBinder_factory(LiveBinderTests$$oneWayMultiBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  assert.notEqual(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  assert.equal(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
};
function LiveBinderTests$$TestLiveBinderMultiOnChange(assert) {
  var src, target, liveBinder, stmtTemp1;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_testVMA(TestViewModelA_factory());
  src.get_testVMA().set_propStr1("test");
  liveBinder = LiveBinder_factory(LiveBinderTests$$oneWayMultiBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  assert.notEqual(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  assert.equal(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  src.get_testVMA().set_propStr1("testChanged");
  assert.equal(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes on lastElement should have flowed.");
  src.set_testVMA((stmtTemp1 = TestViewModelA_factory(), stmtTemp1.set_propStr1("ChangedTest"), stmtTemp1));
  assert.equal(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes on firstElement should have flowed.");
  src.set_testVMA(null);
  assert.equal(null, target.get_propStr1(), "if liveBinder is active and changes on firstElement should have flowed.");
};
function LiveBinderTests$$TestTwoWayLiveBinderOnChange(assert) {
  var src, target, liveBinder;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_propStr1("test");
  liveBinder = LiveBinder_factory(LiveBinderTests$$twoWayBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  assert.notEqual(src.get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  assert.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  src.set_propStr1("testChanged");
  assert.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  target.set_propStr1("Changed Again");
  assert.equal(target.get_propStr1(), src.get_propStr1(), "if liveBinder is active in twoWay changes on target should flow back.");
};
function LiveBinderTests$$TestTwoWayLiveBinderMultiOnChangeWithConverters(assert) {
  var src, target, liveBinder, stmtTemp1;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_testVMA(TestViewModelA_factory());
  src.get_testVMA().set_propInt1(1);
  liveBinder = LiveBinder_factory(LiveBinderTests$$twoWayMultiBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  assert.notEqual(Int32$$ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  assert.equal(Int32$$ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  src.get_testVMA().set_propInt1(2);
  assert.equal(Int32$$ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active and changes on lastElement should have flowed.");
  src.set_testVMA((stmtTemp1 = TestViewModelA_factory(), stmtTemp1.set_propInt1(3), stmtTemp1));
  assert.equal(Int32$$ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active and changes on firstElement should have flowed.");
  target.set_propStr1("21");
  assert.equal(Int32$$ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active in twoWay changes on target should flow back.");
};
Type$$RegisterReferenceType(LiveBinderTests, "Sunlight.Framework.UI.Test.LiveBinderTests", Object, []);
function NScriptsTemplateTests() {
};
NScriptsTemplateTests.typeId = "s";
function NScriptsTemplateTests$$Setup() {
  TaskScheduler$$set_Instance(TaskScheduler_factory(TestWindowTimer_factory(), 10, 10));
};
function NScriptsTemplateTests$$Test(assert) {
  assert.notEqual(null, NScriptsTemplatesClass$$get_TestTemplate1(), "Template should not be null");
  assert.ok(true, "true should be true");
};
function NScriptsTemplateTests$$TestApplySkin(assert) {
  var element, control;
  element = window.document.createElement("div");
  control = UISkinableElement_factory(element);
  control.set_dataContext(TestViewModelA_factory());
  control.set_skin(NScriptsTemplatesClass$$get_TestTemplate1());
  control.activate();
  assert.notEqual(null, element.querySelector("[test]"), "After applying skin, skin element should be loaded");
};
function NScriptsTemplateTests$$TestCssBinder(assert) {
  var element, control, vm, elem;
  element = window.document.createElement("div");
  control = UISkinableElement_factory(element);
  vm = TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(NScriptsTemplatesClass$$get_TestTemplateVMB_CssBinding());
  control.activate();
  elem = element.querySelector("[test]");
  assert.notEqual(null, elem, "After applying skin, skin element should be loaded");
  assert.equal("", elem.className, "Class should not be set if PropBool1 is not set.");
  vm.set_propBool1(true);
  assert.notEqual("", elem.className, "Class should be set if PropBool1 is set.");
};
function NScriptsTemplateTests$$TestStyleBinder(assert) {
  var element, control, vm, elem;
  element = window.document.createElement("div");
  control = UISkinableElement_factory(element);
  vm = TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(NScriptsTemplatesClass$$get_TestTemplateVMB_StyleBinding());
  control.activate();
  elem = element.querySelector("[test]");
  assert.notEqual(null, elem, "After applying skin, skin element should be loaded");
  assert.equal("", elem.style.height, "Style should not be set if PropStr1 is not set.");
  vm.set_propStr1("10px");
  assert.equal("10px", elem.style.height, "Style should be set if PropStr1 is set.");
};
function NScriptsTemplateTests$$TestAttrBinder(assert) {
  var element, control, vm, elem;
  element = window.document.createElement("div");
  control = UISkinableElement_factory(element);
  vm = TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(NScriptsTemplatesClass$$get_TestTemplateVMB_AttrBinding());
  control.activate();
  elem = element.querySelector("[test]");
  assert.notEqual(elem, null, "After applying skin, skin element should be loaded");
  assert.equal(elem.getAttribute("test1"), null, "Attribute 'test' should not be set if PropStr1 is not set.");
  vm.set_propStr1("TTTest");
  assert.equal("TTTest", elem.getAttribute("test1"), "Attribute 'test' should be set if PropStr1 is set.");
};
function NScriptsTemplateTests$$TestPropertyBinder(assert) {
  var element, control, vm;
  element = window.document.createElement("div");
  control = TestSkinableWithTestUIElementPart_factory(element);
  vm = TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(NScriptsTemplatesClass$$get_TestTemplateB_PropertyBinding());
  control.activate();
  assert.ok(control.get_part(), "templatePart should not be null.");
  assert.equal(control.get_part().get_oneWayStrictBinding(), vm.get_propStr1(), "vmPropStr1 should be equal to OnewayStrictBinding.");
  vm.set_propStr1("T1");
  assert.equal(control.get_part().get_oneWayStrictBinding(), vm.get_propStr1(), "vmPropStr1 should be equal to OnewayStrictBinding.");
  assert.equal(control.get_part().get_twoWayLooseBinding(), vm.get_propInt1(), "TwoWayLooseBinding and bound property PropInt1 should be equal.");
  vm.set_propInt1(11);
  assert.equal(control.get_part().get_twoWayLooseBinding(), vm.get_propInt1(), "TwoWayLooseBinding and bound property PropInt1 should be equal.");
  control.get_part().set_twoWayLooseBinding(101);
  assert.equal(control.get_part().get_twoWayLooseBinding(), vm.get_propInt1(), "TwoWayLooseBinding and bound property PropInt1 should be equal.");
};
Type$$RegisterReferenceType(NScriptsTemplateTests, "Sunlight.Framework.UI.Test.NScriptsTemplateTests", Object, []);
function SkinBinderHelperTests() {
};
SkinBinderHelperTests.typeId = "t";
function SkinBinderHelperTests$$TestSimpleBinder(assert) {
  var src, stmtTemp1, tar1;
  src = (stmtTemp1 = TestViewModelA_factory(), stmtTemp1.set_propStr1("Test"), stmtTemp1.set_propInt1(1), stmtTemp1.set_propBool1(true), stmtTemp1);
  tar1 = TestViewModelA_factory();
  SkinBinderHelper$$Bind(NativeArray$1$$op_Implicit(ArrayG_$SkinBinderInfo$_.__ctor([SkinBinderInfo_factory1(NativeArray$1$$op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([TestViewModelA$$PropStr1Getter])), TestViewModelA$$PropStr1Setter, 17, 0, null, null)])), src, NativeArray$1$$op_Implicit(ArrayG_$Object$_.__ctor([tar1])));
  assert.equal(src.get_propStr1(), tar1.get_propStr1(), "After BindDataContext values should be equal");
};
function SkinBinderHelperTests$$TestAttrBinding(assert) {
  var src, stmtTemp1, target;
  src = (stmtTemp1 = TestViewModelA_factory(), stmtTemp1.set_propStr1("Test"), stmtTemp1);
  target = window.document.createElement("div");
  SkinBinderHelper$$Bind(NativeArray$1$$op_Implicit(ArrayG_$SkinBinderInfo$_.__ctor([SkinBinderInfo_factory2(NativeArray$1$$op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([TestViewModelA$$PropStr1Getter])), function SkinBinderHelperTests$24$24TestAttrBinding_del(o1, o2, o3) {
    SkinBinderHelper$$SetAttribute(o1, Type$$CastType(String, o2), Type$$CastType(String, o3));
  }, "tmp", 113, 0, null, null, 0)])), src, NativeArray$1$$op_Implicit(ArrayG_$Object$_.__ctor([target])));
  assert.equal(src.get_propStr1(), target.getAttribute("tmp"), "After BindDataContext values should be equal");
};
function SkinBinderHelperTests$$TestStyleBinding(assert) {
  var src, stmtTemp1, target;
  src = (stmtTemp1 = TestViewModelA_factory(), stmtTemp1.set_propBool1(true), stmtTemp1);
  target = window.document.createElement("div");
  target.className = "t1";
  SkinBinderHelper$$Bind(NativeArray$1$$op_Implicit(ArrayG_$SkinBinderInfo$_.__ctor([SkinBinderInfo_factory2(NativeArray$1$$op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([TestViewModelA$$PropBool1Getter])), function SkinBinderHelperTests$24$24TestStyleBinding_del(o1, o2, o3) {
    SkinBinderHelper$$SetCssClass(o1, Type$$UnBoxTypeInstance(Boolean0, o2), Type$$CastType(String, o3));
  }, "test", 113, 0, null, null, 0)])), src, NativeArray$1$$op_Implicit(ArrayG_$Object$_.__ctor([target])));
  assert.equal("t1 test", target.className, "After BindDataContext values should be equal");
  src.set_propBool1(false);
  SkinBinderHelper$$Bind(NativeArray$1$$op_Implicit(ArrayG_$SkinBinderInfo$_.__ctor([SkinBinderInfo_factory2(NativeArray$1$$op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([TestViewModelA$$PropBool1Getter])), function SkinBinderHelperTests$24$24TestStyleBinding_del2(o1, o2, o3) {
    SkinBinderHelper$$SetCssClass(o1, Type$$UnBoxTypeInstance(Boolean0, o2), Type$$CastType(String, o3));
  }, "test", 113, 0, null, null, 0)])), src, NativeArray$1$$op_Implicit(ArrayG_$Object$_.__ctor([target])));
  assert.equal("t1", target.className, "After BindDataContext values should be equal");
};
function SkinBinderHelperTests$$TestTextContentBinding(assert) {
  var src, stmtTemp1, target;
  src = (stmtTemp1 = TestViewModelA_factory(), stmtTemp1.set_propStr1("Test"), stmtTemp1);
  target = window.document.createElement("div");
  SkinBinderHelper$$Bind(NativeArray$1$$op_Implicit(ArrayG_$SkinBinderInfo$_.__ctor([SkinBinderInfo_factory1(NativeArray$1$$op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([TestViewModelA$$PropStr1Getter])), function SkinBinderHelperTests$24$24TestTextContentBinding_del(o1, o2) {
    SkinBinderHelper$$SetTextContent(o1, Type$$CastType(String, o2));
  }, 17, 0, null, null)])), src, NativeArray$1$$op_Implicit(ArrayG_$Object$_.__ctor([target])));
  assert.equal(src.get_propStr1(), target.textContent, "After BindDataContext values should be equal");
};
Type$$RegisterReferenceType(SkinBinderHelperTests, "Sunlight.Framework.UI.Test.SkinBinderHelperTests", Object, []);
function TestListView() {
};
TestListView.typeId = "u";
function TestListView$$Setup() {
  TaskScheduler$$set_Instance(TaskScheduler_factory(TestWindowTimer_factory(), 10, 10));
};
function TestListView$$TestChildSkin(assert) {
  var document, listView, vmAs, stmtTemp1, stmtTemp10, stmtTemp11, elems, i;
  document = window.document;
  listView = ListView_factory(document.createElement("div"));
  listView.set_itemSkin(NScriptsTemplatesClass$$get_TestTemplateVMB1());
  vmAs = ArrayG_$TestViewModelB$_.__ctor([(stmtTemp1 = TestViewModelB_factory(), stmtTemp1.set_propStr1("StrTest"), stmtTemp1), (stmtTemp10 = TestViewModelB_factory(), stmtTemp10.set_propStr1("TestStr"), stmtTemp10), (stmtTemp11 = TestViewModelB_factory(), stmtTemp11.set_propStr1("TestTT1"), stmtTemp11)]);
  listView.set_fixedList(vmAs);
  listView.set_inactiveIfNullContext(false);
  listView.activate();
  elems = listView.get_element().querySelectorAll("[test]");
  assert.equal(3, elems.length, "number of children should be 3");
  for (i = 0; i < 3; i++)
    assert.equal(vmAs.get_item(i).get_propStr1(), elems[i].innerText, "Inner text for element should match property it's bound to.");
};
Type$$RegisterReferenceType(TestListView, "Sunlight.Framework.UI.Test.TestListView", Object, []);
function ManualTemplateTests() {
};
ManualTemplateTests.typeId = "v";
ManualTemplateTests$$noneValue = null;
function ManualTemplateTests$$Setup() {
};
function ManualTemplateTests$$Test(assert) {
  assert.ok(true, "true should be true");
};
function ManualTemplateTests$$$$cctor() {
  ManualTemplateTests$$noneValue = ValueIfTrue_$String$_.__ctor("none");
};
Type$$RegisterReferenceType(ManualTemplateTests, "Sunlight.Framework.UI.Test.ManualTemplateTests", Object, []);
function UIElementTests() {
};
UIElementTests.typeId = "w";
function UIElementTests$$TestNewUIElement(assert) {
  var doc, element;
  doc = window.document;
  element = UIElement_factory(doc.createElement("div"));
  assert.notEqual(element.get_element(), null, "element.Element != null");
  assert.equal(element.get_element().tagName, "DIV", "element.Element.TagName == 'DIV'");
};
Type$$RegisterReferenceType(UIElementTests, "Sunlight.Framework.UI.Test.UIElementTests", Object, []);
String.typeId = "n";
String$$trimStartHelperRegex = null;
String$$trimEndHelperRegex = null;
function String$$get_TrimEndHelperRegex() {
  if (Object$$IsNullOrUndefined(String$$trimEndHelperRegex))
    String$$trimEndHelperRegex = new RegExp("^[\\s\\xA0]+");
  return String$$trimEndHelperRegex;
};
function String$$get_TrimStartHelperRegex() {
  if (Object$$IsNullOrUndefined(String$$trimStartHelperRegex))
    String$$trimStartHelperRegex = new RegExp("^[\\s\\xA0]+");
  return String$$trimStartHelperRegex;
};
function String$$Trim(this_) {
  return String$$TrimEnd(String$$TrimStart(this_));
};
function String$$TrimEnd(this_) {
  return this_.trimLeft ? this_.trimLeft() : this_.replace(String$$get_TrimEndHelperRegex(), "");
};
function String$$TrimStart(this_) {
  return this_.trimRight ? this_.trimRight() : this_.replace(String$$get_TrimStartHelperRegex(), "");
};
function String$$get_Item(this_, index) {
  return this_.charCodeAt(index);
};
String$$TrimEnd = function String$$TrimEnd(this_) {
  return this_.trimLeft ? this_.trimLeft() : this_.replace(String$$get_TrimEndHelperRegex(), "");
};
String$$TrimStart = function String$$TrimStart(this_) {
  return this_.trimRight ? this_.trimRight() : this_.replace(String$$get_TrimStartHelperRegex(), "");
};
Type$$RegisterReferenceType(String, "System.String", Object, []);
function SkinBinderInfo() {
};
SkinBinderInfo.typeId = "x";
function SkinBinderInfo_factory1(propertyGetterPath, targetPropertySetter, binderType, objectIndex, forwardConverter, defaultValue) {
  var this_;
  this_ = new SkinBinderInfo();
  this_.__ctor(propertyGetterPath, targetPropertySetter, binderType, objectIndex, forwardConverter, defaultValue);
  return this_;
};
function SkinBinderInfo_factory2(propertyGetterPath, targetPropertySetterWithArg, targetPropertySetterArg, binderType, objectIndex, forwardConverter, defaultValue, extraObjectIndex) {
  var this_;
  this_ = new SkinBinderInfo();
  this_.__ctor0(propertyGetterPath, targetPropertySetterWithArg, targetPropertySetterArg, binderType, objectIndex, forwardConverter, defaultValue, extraObjectIndex);
  return this_;
};
function SkinBinderInfo_factory(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue) {
  var this_;
  this_ = new SkinBinderInfo();
  this_.__ctor1(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue);
  return this_;
};
function SkinBinderInfo_factory3(propertyGetterPath, propertyNames, targetPropertySetter, targetPropertySetterArg, binderType, objectIndex, binderIndex, forwardConverter, defaultValue, extraObjectIndex) {
  var this_;
  this_ = new SkinBinderInfo();
  this_.__ctor3(propertyGetterPath, propertyNames, targetPropertySetter, targetPropertySetterArg, binderType, objectIndex, binderIndex, forwardConverter, defaultValue, extraObjectIndex);
  return this_;
};
function SkinBinderInfo_factory0(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue) {
  var this_;
  this_ = new SkinBinderInfo();
  this_.__ctor2(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue);
  return this_;
};
ptyp_ = SkinBinderInfo.prototype;
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
ptyp_.__ctor = function SkinBinderInfo$$$$ctor1(propertyGetterPath, targetPropertySetter, binderType, objectIndex, forwardConverter, defaultValue) {
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
ptyp_.__ctor0 = function SkinBinderInfo$$$$ctor2(propertyGetterPath, targetPropertySetterWithArg, targetPropertySetterArg, binderType, objectIndex, forwardConverter, defaultValue, extraObjectIndex) {
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
ptyp_.__ctor1 = function SkinBinderInfo$$$$ctor(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue) {
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
ptyp_.__ctor3 = function SkinBinderInfo$$$$ctor3(propertyGetterPath, propertyNames, targetPropertySetter, targetPropertySetterArg, binderType, objectIndex, binderIndex, forwardConverter, defaultValue, extraObjectIndex) {
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
ptyp_.__ctor2 = function SkinBinderInfo$$$$ctor0(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue) {
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
ptyp_.setTargetValue = function SkinBinderInfo$$SetTargetValue(target, value, extraObjectArray) {
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
    if (!Object$$IsNullOrUndefined(extraObjectArray[binderInfo.extraObjectIndex]))
      Element$$UnBind(element, Type$$CastType(String, binderInfo.targetPropertySetterArg), extraObjectArray[binderInfo.extraObjectIndex], false);
    extraObjectArray[binderInfo.extraObjectIndex] = value;
    if (!Object$$IsNullOrUndefined(value))
      Element$$Bind(element, Type$$CastType(String, binderInfo.targetPropertySetterArg), value, false);
  }
  else if (binderInfo.targetPropertySetter)
    binderInfo.targetPropertySetter(target, value);
  else
    binderInfo.targetPropertySetterWithArg(target, value, binderInfo.targetPropertySetterArg);
};
Type$$RegisterReferenceType(SkinBinderInfo, "Sunlight.Framework.UI.Helpers.SkinBinderInfo", Object, []);
function INotifyPropertyChanged() {
};
INotifyPropertyChanged.typeId = "b";
Type$$RegisterInterface(INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function ObservableObject() {
};
ObservableObject.typeId = "y";
ptyp_ = ObservableObject.prototype;
ptyp_.eventHandlers = null;
ptyp_.linkedProperties = null;
ptyp_.anyPropertyListener = null;
ptyp_.addPropertyChangedListener = function ObservableObject$$AddPropertyChangedListener(propertyName, callback) {
  var cb;
  if (!this.eventHandlers)
    this.eventHandlers = StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_.defaultConstructor();
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
    cb = Delegate$$Combine(cb, callback);
  this.eventHandlers.set_item(propertyName, cb);
};
ptyp_.removePropertyChangedListener = function ObservableObject$$RemovePropertyChangedListener(propertyName, callback) {
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
    cb = Delegate$$Remove(cb, callback);
    if (cb)
      this.eventHandlers.set_item(propertyName, cb);
    else
      this.eventHandlers.remove(propertyName);
  }
};
ptyp_.clearListeners = function ObservableObject$$ClearListeners() {
  this.eventHandlers = null;
  this.anyPropertyListener = null;
};
ptyp_.firePropertyChanged = function ObservableObject$$FirePropertyChanged(propertyName) {
  var cb, linkedProperties, iProp;
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
    if (this.linkedProperties) {
      if (this.linkedProperties.tryGetValue(propertyName, {
        read: function() {
          return linkedProperties;
        },
        write: function(arg0) {
          return linkedProperties = arg0;
        }
      }))
        for (iProp = 0; iProp < linkedProperties.get_count(); iProp++)
          if (this.eventHandlers.tryGetValue(linkedProperties.get_item(iProp), {
            read: function() {
              return cb;
            },
            write: function(arg0) {
              return cb = arg0;
            }
          }))
            cb(this, propertyName);
    }
  }
  if (this.anyPropertyListener)
    this.anyPropertyListener(this, propertyName);
};
ptyp_.__ctor = function ObservableObject$$$$ctor() {
};
ptyp_.V_AddPropertyChangedListener_b = ptyp_.addPropertyChangedListener;
ptyp_.V_RemovePropertyChangedListener_b = ptyp_.removePropertyChangedListener;
Type$$RegisterReferenceType(ObservableObject, "Sunlight.Framework.Observables.ObservableObject", Object, [INotifyPropertyChanged]);
function TestViewModelA() {
};
TestViewModelA.typeId = "z";
function TestViewModelA$$PropStr1Getter(obj) {
  return Type$$CastType(TestViewModelA, obj).get_propStr1();
};
function TestViewModelA$$PropStr1Setter(obj, val) {
  Type$$CastType(TestViewModelA, obj).set_propStr1(Type$$CastType(String, val));
};
function TestViewModelA$$PropBool1Getter(obj) {
  return Type$$BoxTypeInstance(Boolean0, Type$$CastType(TestViewModelA, obj).get_propBool1());
};
function TestViewModelA_factory() {
  var this_;
  this_ = new TestViewModelA();
  this_.__ctor0();
  return this_;
};
TestViewModelA.defaultConstructor = TestViewModelA_factory;
ptyp_ = new ObservableObject();
TestViewModelA.prototype = ptyp_;
ptyp_.str1 = null;
ptyp_.bool1 = false;
ptyp_.testVMA = null;
ptyp_.get_propInt1 = function TestViewModelA$$get_PropInt1() {
  return this.get_int1();
};
ptyp_.set_propInt1 = function TestViewModelA$$set_PropInt1(value) {
  if (this.get_int1() != value) {
    this.set_int1(value);
    this.firePropertyChanged("PropInt1");
  }
};
ptyp_.get_propStr1 = function TestViewModelA$$get_PropStr1() {
  return this.str1;
};
ptyp_.set_propStr1 = function TestViewModelA$$set_PropStr1(value) {
  if (this.str1 !== value) {
    this.str1 = value;
    this.firePropertyChanged("PropStr1");
  }
};
ptyp_.get_propBool1 = function TestViewModelA$$get_PropBool1() {
  return this.bool1;
};
ptyp_.set_propBool1 = function TestViewModelA$$set_PropBool1(value) {
  if (this.bool1 != value) {
    this.bool1 = value;
    this.firePropertyChanged("PropBool1");
  }
};
ptyp_.get_testVMA = function TestViewModelA$$get_TestVMA() {
  return this.testVMA;
};
ptyp_.set_testVMA = function TestViewModelA$$set_TestVMA(value) {
  if (this.testVMA != value) {
    this.testVMA = value;
    this.firePropertyChanged("TestVMA");
  }
};
ptyp_.get_int1 = function TestViewModelA$$get_int1() {
  return this.int1;
};
ptyp_.set_int1 = function TestViewModelA$$set_int1(value) {
  return this.int1 = value;
};
ptyp_.__ctor0 = function TestViewModelA$$$$ctor() {
  this.__ctor();
};
Type$$RegisterReferenceType(TestViewModelA, "Sunlight.Framework.UI.Test.TestViewModelA", ObservableObject, []);
function ValueType() {
};
ValueType.typeId = "ab";
ptyp_ = ValueType.prototype;
ptyp_.boxedValue = null;
Type$$RegisterReferenceType(ValueType, "System.ValueType", Object, []);
function Int32(boxedValue) {
  this.boxedValue = boxedValue;
};
Int32.typeId = "bb";
Int32.getDefaultValue = function() {
  return 0;
};
function Int32$$Parse(s) {
  return parseInt(s);
};
function Int32$$ToString0(this_, radix) {
  return this_.toString(radix);
};
function Int32$$ToString(this_) {
  return Int32$$ToString0(this_, 10);
};
ptyp_ = new ValueType();
Int32.prototype = ptyp_;
ptyp_.toString = function() {
  return Int32$$ToString(this.boxedValue);
};
Type$$RegisterStructType(Int32, "System.Int32", []);
function IEnumerable() {
};
IEnumerable.typeId = "g";
Type$$RegisterInterface(IEnumerable, "System.Collections.IEnumerable");
function ICollection() {
};
ICollection.typeId = "c";
Type$$RegisterInterface(ICollection, "System.Collections.ICollection");
function IList() {
};
IList.typeId = "e";
Type$$RegisterInterface(IList, "System.Collections.IList");
function ArrayImpl() {
};
ArrayImpl.typeId = "cb";
ptyp_ = ArrayImpl.prototype;
ptyp_.system$$Collections$$IList$$get_Item = function ArrayImpl$$System$$Collections$$IList$$get_Item(index) {
  return this.V_GetValue(index);
};
ptyp_.system$$Collections$$IList$$Clear = function ArrayImpl$$System$$Collections$$IList$$Clear() {
  throw NotSupportedException_factory();
};
ptyp_.system$$Collections$$IList$$RemoveAt = function ArrayImpl$$System$$Collections$$IList$$RemoveAt(index) {
  throw NotSupportedException_factory();
};
ptyp_.system$$Collections$$ICollection$$get_Count = function ArrayImpl$$System$$Collections$$ICollection$$get_Count() {
  return this.V_get_Length();
};
ptyp_.__ctor = function ArrayImpl$$$$ctor() {
};
ptyp_.V_get_Item_e = ptyp_.system$$Collections$$IList$$get_Item;
ptyp_.V_Clear_e = ptyp_.system$$Collections$$IList$$Clear;
ptyp_.V_RemoveAt_e = ptyp_.system$$Collections$$IList$$RemoveAt;
ptyp_.V_get_Count_c = ptyp_.system$$Collections$$ICollection$$get_Count;
ptyp_.V_Contains_e = function(arg0) {
  return this.V_Contains(arg0);
};
ptyp_.V_IndexOf_e = function(arg0) {
  return this.V_IndexOf(arg0);
};
ptyp_.V_GetEnumerator_g = function() {
  return this.V_GetEnumerator();
};
ptyp_.V_CopyTo_c = function(arg0, arg1) {
  return this.V_CopyTo(arg0, arg1);
};
Type$$RegisterReferenceType(ArrayImpl, "System.ArrayImpl", Object, [IList, IEnumerable, ICollection]);
function Delegate$$Combine(a, b) {
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
  rv = Delegate$$CreateJoinedArray(funcs);
  rv.fullName = "Multicast Delegate";
  return rv;
};
function Delegate$$Create(functionName, instance) {
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
function Delegate$$Remove(source, value) {
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
  rv = Delegate$$CreateJoinedArray(newArr);
  rv.fullName = "Multicast Delegate";
  return rv;
};
function Delegate$$CreateJoinedArray(array) {
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
Type$$RegisterReferenceType(Function0, "System.MulticastDelegate", Function, []);
function LiveBinder() {
};
LiveBinder.typeId = "eb";
function LiveBinder_factory(binderInfo, extraObjectArray) {
  var this_;
  this_ = new LiveBinder();
  this_.__ctor(binderInfo, extraObjectArray);
  return this_;
};
ptyp_ = LiveBinder.prototype;
ptyp_.binderInfo = null;
ptyp_.isActive = false;
ptyp_.source = null;
ptyp_.target = null;
ptyp_.liveObjects = null;
ptyp_.pathTraversed = 0;
ptyp_.updating = false;
ptyp_.extraObjectArray = null;
ptyp_.__ctor = function LiveBinder$$$$ctor(binderInfo, extraObjectArray) {
  this.binderInfo = binderInfo;
  this.extraObjectArray = extraObjectArray;
};
ptyp_.set_source = function LiveBinder$$set_Source(value) {
  if (this.source !== value) {
    this.source = value;
    this.flowValue();
  }
};
ptyp_.set_target = function LiveBinder$$set_Target(value) {
  if (this.target !== value) {
    if (this.target !== null && this.binderInfo.mode == 2)
      Type$$CastType(INotifyPropertyChanged, this.target).V_RemovePropertyChangedListener_b(this.binderInfo.targetPropertyName, Delegate$$Create("onTargetPropertyChanged", this));
    this.target = value;
    if (this.target !== null && this.binderInfo.mode == 2)
      Type$$CastType(INotifyPropertyChanged, this.target).V_AddPropertyChangedListener_b(this.binderInfo.targetPropertyName, Delegate$$Create("onTargetPropertyChanged", this));
    this.flowValue();
  }
};
ptyp_.set_isActive = function LiveBinder$$set_IsActive(value) {
  if (this.isActive != value) {
    this.isActive = value;
    if (this.isActive)
      this.activate();
    else
      this.deactivate();
  }
};
ptyp_.cleanup = function LiveBinder$$Cleanup() {
  if (!this.isActive) {
    this.pathTraversed = 0;
    this.cleanRegistrations();
  }
};
ptyp_.activate = function LiveBinder$$Activate() {
  this.flowValue();
};
ptyp_.deactivate = function LiveBinder$$Deactivate() {
  this.isActive = false;
};
ptyp_.flowValue = function LiveBinder$$FlowValue() {
  if (this.target === null || this.updating || !this.isActive)
    return;
  if (!this.liveObjects)
    this.liveObjects = ArrayG_$Object$_.__ctor0(this.binderInfo.propertyGetterPath.length + 1);
  if (this.liveObjects.get_item(0) !== this.source) {
    if (!Object$$IsNullOrUndefined(this.liveObjects.get_item(0))) {
      this.pathTraversed = 0;
      this.cleanRegistrations();
    }
    this.liveObjects.set_item(0, this.source);
    if (!Object$$IsNullOrUndefined(this.liveObjects.get_item(0)))
      Type$$CastType(INotifyPropertyChanged, this.liveObjects.get_item(0)).V_AddPropertyChangedListener_b(this.binderInfo.propertyNames[0], Delegate$$Create("onSourcePropertyChanged", this));
  }
  this.setTargetProperty(this.getValue());
};
ptyp_.setTargetProperty = function LiveBinder$$SetTargetProperty(value) {
  try {
    this.updating = true;
    this.binderInfo.setTargetValue(this.target, value, this.extraObjectArray);
  } finally {
    this.updating = false;
  }
};
ptyp_.getValue = function LiveBinder$$GetValue() {
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
ptyp_.getValueInternal = function LiveBinder$$GetValueInternal() {
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
          Type$$CastType(INotifyPropertyChanged, liveObjects.get_item(iPath)).V_RemovePropertyChangedListener_b(propertyNames[iPath], Delegate$$Create("onSourcePropertyChanged", this));
        liveObjects.set_item(iPath, src);
        if (src !== null && iPath < pathLength - 1 && src !== null)
          Type$$CastType(INotifyPropertyChanged, src).V_AddPropertyChangedListener_b(binderInfo.propertyNames[iPath], Delegate$$Create("onSourcePropertyChanged", this));
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
ptyp_.cleanRegistrations = function LiveBinder$$CleanRegistrations() {
  var liveObjects, iPath, till, item;
  liveObjects = this.liveObjects;
  if (this.pathTraversed < this.liveObjects.V_get_Length()) {
    liveObjects.set_item(liveObjects.V_get_Length() - 1, null);
    for (
    iPath = this.binderInfo.propertyGetterPath.length - 2, till = this.pathTraversed; iPath >= till; iPath--) {
      item = liveObjects.get_item(iPath);
      if (item !== null) {
        Type$$CastType(INotifyPropertyChanged, item).V_RemovePropertyChangedListener_b(this.binderInfo.propertyNames[iPath], Delegate$$Create("onSourcePropertyChanged", this));
        liveObjects.set_item(iPath, null);
      }
    }
  }
};
ptyp_.onSourcePropertyChanged = function LiveBinder$$OnSourcePropertyChanged(obj, str) {
  this.flowValue();
};
ptyp_.onTargetPropertyChanged = function LiveBinder$$OnTargetPropertyChanged(obj, str) {
  var stmtTemp1, binderInfo, target, liveObjects, value;
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
  } catch (stmtTemp1) {
  } finally {
    this.updating = false;
  }
};
Type$$RegisterReferenceType(LiveBinder, "Sunlight.Framework.UI.Helpers.LiveBinder", Object, []);
function TaskScheduler() {
};
TaskScheduler.typeId = "fb";
TaskScheduler$$instance = null;
function TaskScheduler$$get_Instance() {
  if (!TaskScheduler$$instance)
    TaskScheduler$$instance = TaskScheduler_factory(WindowTimer_factory(), 16, 25);
  return TaskScheduler$$instance;
};
function TaskScheduler$$set_Instance(value) {
  TaskScheduler$$instance = value;
};
function TaskScheduler_factory(windowTimer, workQuanta, idleTimeout) {
  var this_;
  this_ = new TaskScheduler();
  this_.__ctor(windowTimer, workQuanta, idleTimeout);
  return this_;
};
ptyp_ = TaskScheduler.prototype;
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
ptyp_.__ctor = function TaskScheduler$$$$ctor(windowTimer, workQuanta, idleTimeout) {
  this.timerId = -1;
  this.tasks = NumberDictionary_$Task$_.defaultConstructor();
  this.windowTimer = windowTimer;
  this.workQuanta = workQuanta;
  this.idleTimeout = idleTimeout;
  this.hiPriTasks = Queue_$Task$_.defaultConstructor();
  this.lowPriTasks = Queue_$Task$_.defaultConstructor();
  this.idleTasks = Queue_$Task$_.defaultConstructor();
};
ptyp_.enqueueLowPriTask = function TaskScheduler$$EnqueueLowPriTask(work, traceId) {
  var task;
  task = Task_factory(this.nextTimerId++, work);
  this.idleTasks.enqueue(task);
  this.tasks.add(task.taskId, task);
  this.scheduleQuanta(false);
  return TaskHandle_factory(task.taskId);
};
ptyp_.runQuanta = function TaskScheduler$$RunQuanta() {
  if (this.hiPriTasks.get_count() > 0)
    this.flushQueue(this.hiPriTasks, DateTime$$op_Addition(DateTime$$get_Now(), this.workQuanta));
  else if (this.idleTasks.get_count() > 0)
    this.flushQueue(this.idleTasks, DateTime$$op_Addition(DateTime$$get_Now(), this.workQuanta / 2 | 0));
  else if (this.lowPriTasks.get_count() > 0)
    this.flushQueue(this.lowPriTasks, DateTime$$op_Addition(DateTime$$get_Now(), this.workQuanta / 2 | 0));
  this.timerId = -1;
  this.scheduleQuanta(true);
};
ptyp_.flushQueue = function TaskScheduler$$FlushQueue(taskQueue, endBy) {
  var task, now;
  while (taskQueue.get_count() > 0) {
    task = taskQueue.dequeue();
    this.executeTask(task);
    now = DateTime$$get_Now();
    if (endBy < now)
      return;
  }
};
ptyp_.executeTask = function TaskScheduler$$ExecuteTask(task) {
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
ptyp_.scheduleQuanta = function TaskScheduler$$ScheduleQuanta(force) {
  if (force || !this.highPriSetup && this.hiPriTasks.get_count() > 0) {
    this.windowTimer.V_ClearTimeout_f(this.timerId);
    this.timerId = -1;
  }
  if (this.timerId != -1)
    return;
  if (this.hiPriTasks.get_count() > 0) {
    this.highPriSetup = true;
    this.timerId = this.windowTimer.V_SetImmediate_f(Delegate$$Create("runQuanta", this));
  }
  else if (this.idleTasks.get_count() + this.lowPriTasks.get_count() > 0) {
    this.highPriSetup = false;
    this.timerId = this.windowTimer.V_SetTimeout_f(Delegate$$Create("runQuanta", this), this.idleTimeout);
  }
};
Type$$RegisterReferenceType(TaskScheduler, "Sunlight.Framework.TaskScheduler", Object, []);
function IWindowTimer() {
};
IWindowTimer.typeId = "f";
Type$$RegisterInterface(IWindowTimer, "Sunlight.Framework.IWindowTimer");
function TestWindowTimer() {
};
TestWindowTimer.typeId = "gb";
function TestWindowTimer_factory() {
  return new TestWindowTimer();
};
TestWindowTimer.defaultConstructor = TestWindowTimer_factory;
ptyp_ = TestWindowTimer.prototype;
ptyp_.setImmediate = function TestWindowTimer$$SetImmediate(action) {
  action();
  return 0;
};
ptyp_.setTimeout = function TestWindowTimer$$SetTimeout(action, timoutTime) {
  action();
  return 0;
};
ptyp_.clearTimeout = function TestWindowTimer$$ClearTimeout(timeoutHandle) {
};
ptyp_.__ctor = function TestWindowTimer$$$$ctor() {
};
ptyp_.V_SetImmediate_f = ptyp_.setImmediate;
ptyp_.V_SetTimeout_f = ptyp_.setTimeout;
ptyp_.V_ClearTimeout_f = ptyp_.clearTimeout;
Type$$RegisterReferenceType(TestWindowTimer, "Sunlight.Framework.TestWindowTimer", Object, [IWindowTimer]);
function NScriptsTemplatesClass() {
};
NScriptsTemplatesClass.typeId = "hb";
function NScriptsTemplatesClass$$get_TestTemplate1() {
  return Test$5cTemplates$5cTestTemplate1();
};
function NScriptsTemplatesClass$$get_TestTemplateVMB1() {
  return Test$5cTemplates$5cTestTemplateVMB1();
};
function NScriptsTemplatesClass$$get_TestTemplateVMB_AttrBinding() {
  return Test$5cTemplates$5cTestTemplateVMB_AttrBinding();
};
function NScriptsTemplatesClass$$get_TestTemplateVMB_CssBinding() {
  return Test$5cTemplates$5cTestTemplateVMB_CssBinding();
};
function NScriptsTemplatesClass$$get_TestTemplateVMB_StyleBinding() {
  return Test$5cTemplates$5cTestTemplateVMB_StyleBinding();
};
function NScriptsTemplatesClass$$get_TestTemplateB_PropertyBinding() {
  return Test$5cTemplates$5cTestTemplateB_PropertyBinding();
};
Type$$RegisterReferenceType(NScriptsTemplatesClass, "Sunlight.Framework.UI.Test.NScriptsTemplatesClass", Object, []);
function ExtensibleObservableObject() {
};
ExtensibleObservableObject.typeId = "ib";
function ExtensibleObservableObject_factory() {
  var this_;
  this_ = new ExtensibleObservableObject();
  this_.__ctor0();
  return this_;
};
ExtensibleObservableObject.defaultConstructor = ExtensibleObservableObject_factory;
ptyp_ = new ObservableObject();
ExtensibleObservableObject.prototype = ptyp_;
ptyp_.propertyMap = null;
ptyp_.__ctor0 = function ExtensibleObservableObject$$$$ctor() {
  this.__ctor();
  this.propertyMap = {
  };
};
Type$$RegisterReferenceType(ExtensibleObservableObject, "Sunlight.Framework.Observables.ExtensibleObservableObject", ObservableObject, []);
function ContextBindableObject() {
};
ContextBindableObject.typeId = "jb";
function ContextBindableObject_factory() {
  var this_;
  this_ = new ContextBindableObject();
  this_.__ctor1();
  return this_;
};
ContextBindableObject.defaultConstructor = ContextBindableObject_factory;
ptyp_ = new ExtensibleObservableObject();
ContextBindableObject.prototype = ptyp_;
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
ptyp_.set_parent = function ContextBindableObject$$set_Parent(value) {
  if (this.parent != value) {
    if (this.parent) {
      this.parent.removePropertyChangedListener("DataContext", Delegate$$Create("onParentDataContextUpdated", this));
      this.parent.removePropertyChangedListener("IsActive", Delegate$$Create("onParentDataContextUpdated", this));
    }
    this.parent = value;
    if (!this.dataContextSetterCalled)
      if (this.parent) {
        this.parent.addPropertyChangedListener("DataContext", Delegate$$Create("onParentDataContextUpdated", this));
        this.parent.addPropertyChangedListener("IsActive", Delegate$$Create("onParentDataContextUpdated", this));
        this.onParentDataContextUpdated(null, null);
      }
      else
        this.setDataContext(null);
  }
};
ptyp_.get_dataContext = function ContextBindableObject$$get_DataContext() {
  return this.dataContext;
};
ptyp_.set_dataContext = function ContextBindableObject$$set_DataContext(value) {
  this.dataContextSetterCalled = true;
  this.setDataContext(value);
};
ptyp_.get_isActive = function ContextBindableObject$$get_IsActive() {
  return this.isActivated && !this.V_get_ActivationBlocked();
};
ptyp_.set_inactiveIfNullContext = function ContextBindableObject$$set_InactiveIfNullContext(value) {
  this.isInactiveIfNullContext = value;
  this.fixActivation();
};
ptyp_.get_activationBlocked = function ContextBindableObject$$get_ActivationBlocked() {
  return this.isInactiveIfNullContext && this.dataContext === null;
};
ptyp_.dispose = function ContextBindableObject$$Dispose() {
  if (!this.isDisposed && !this.isDisposing)
    this.V_InternalDispose();
};
ptyp_.activate = function ContextBindableObject$$Activate() {
  this.isActive = true;
  this.fixActivation();
};
ptyp_.deactivate = function ContextBindableObject$$Deactivate() {
  if (!this.isActive)
    return;
  this.isActive = false;
  this.fixActivation();
};
ptyp_.onBeforeFirstActivate = function ContextBindableObject$$OnBeforeFirstActivate() {
};
ptyp_.onActivate = function ContextBindableObject$$OnActivate() {
};
ptyp_.onDeactivate = function ContextBindableObject$$OnDeactivate() {
};
ptyp_.fixActivation = function ContextBindableObject$$FixActivation() {
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
ptyp_.internalDispose = function ContextBindableObject$$InternalDispose() {
  if (this.onDisposed) {
    this.set_parent(null);
    this.clearListeners();
    this.onDisposed();
  }
};
ptyp_.onDataContextUpdating = function ContextBindableObject$$OnDataContextUpdating(newValue) {
};
ptyp_.onDataContextUpdated = function ContextBindableObject$$OnDataContextUpdated(oldValue) {
};
ptyp_.setDataContext = function ContextBindableObject$$SetDataContext(value) {
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
ptyp_.onParentDataContextUpdated = function ContextBindableObject$$OnParentDataContextUpdated(sender, propertyName) {
  if (this.parent.get_isActive() && !this.dataContextSetterCalled)
    this.setDataContext(this.parent.get_dataContext());
  if (propertyName === "IsActive" || propertyName === null)
    if (this.parent.get_isActive())
      this.activate();
    else
      this.deactivate();
};
ptyp_.__ctor1 = function ContextBindableObject$$$$ctor() {
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
Type$$RegisterReferenceType(ContextBindableObject, "Sunlight.Framework.Binders.ContextBindableObject", ExtensibleObservableObject, []);
function UIElement() {
};
UIElement.typeId = "kb";
function UIElement_factory(element) {
  var this_;
  this_ = new UIElement();
  this_.__ctor2(element);
  return this_;
};
ptyp_ = new ContextBindableObject();
UIElement.prototype = ptyp_;
ptyp_.element = null;
ptyp_.isHidden = false;
ptyp_.eventRegistrationDict = null;
ptyp_.__ctor2 = function UIElement$$$$ctor(element) {
  this.__ctor1();
  this.eventRegistrationDict = StringDictionary_$Action_$UIEvent$_$_.defaultConstructor();
  this.element = element;
};
ptyp_.get_element = function UIElement$$get_Element() {
  return this.element;
};
ptyp_.get_activationBlocked0 = function UIElement$$get_ActivationBlocked() {
  return this.get_activationBlocked() || this.isHidden;
};
ptyp_.internalDispose0 = function UIElement$$InternalDispose() {
  var stmtTemp1, kvPair;
  for (stmtTemp1 = this.eventRegistrationDict.V_GetEnumerator_g(); stmtTemp1.V_MoveNext_h(); ) {
    kvPair = Type$$UnBoxTypeInstance(KeyValuePair_$String_x_Action_$UIEvent$_$_, stmtTemp1.V_get_Current_h());
    Element$$UnBind0(this.element, KeyValuePair_$String_x_Action_$UIEvent$_$_.get_key(kvPair));
  }
  this.eventRegistrationDict.clear();
  this.internalDispose();
};
ptyp_.V_get_ActivationBlocked = ptyp_.get_activationBlocked0;
ptyp_.V_InternalDispose = ptyp_.internalDispose0;
Type$$RegisterReferenceType(UIElement, "Sunlight.Framework.UI.UIElement", ContextBindableObject, []);
function UISkinableElement() {
};
UISkinableElement.typeId = "lb";
function UISkinableElement_factory(element) {
  var this_;
  this_ = new UISkinableElement();
  this_.__ctor3(element);
  return this_;
};
ptyp_ = new UIElement();
UISkinableElement.prototype = ptyp_;
ptyp_.skin = null;
ptyp_.skinInstance = null;
ptyp_.__ctor3 = function UISkinableElement$$$$ctor(element) {
  this.__ctor2(element);
};
ptyp_.set_skin = function UISkinableElement$$set_Skin(value) {
  if (this.skin != value) {
    this.skin = value;
    if (this.skin && this.get_isActive()) {
      this.get_element().setAttribute("skin-id", this.skin.get_id());
      this.set_skinInstance(this.skin.createInstance());
    }
    this.firePropertyChanged("Skin");
  }
};
ptyp_.get_skinInstance = function UISkinableElement$$get_SkinInstance() {
  return this.skinInstance;
};
ptyp_.set_skinInstance = function UISkinableElement$$set_SkinInstance(value) {
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
ptyp_.applySkinInternal = function UISkinableElement$$ApplySkinInternal(skin) {
};
ptyp_.onBeforeFirstActivate0 = function UISkinableElement$$OnBeforeFirstActivate() {
  this.onBeforeFirstActivate();
  if (this.skin && !this.skinInstance)
    this.set_skinInstance(this.skin.createInstance());
};
ptyp_.onActivate0 = function UISkinableElement$$OnActivate() {
  this.onActivate();
  if (this.skin && !this.skinInstance)
    this.set_skinInstance(this.skin.createInstance());
  else if (this.skinInstance)
    this.skinInstance.activate();
};
ptyp_.onDeactivate0 = function UISkinableElement$$OnDeactivate() {
  if (this.skinInstance)
    this.skinInstance.deactivate();
  this.onDeactivate();
};
ptyp_.internalDispose1 = function UISkinableElement$$InternalDispose() {
  if (this.get_skinInstance())
    this.set_skinInstance(null);
  this.set_skin(null);
  this.internalDispose0();
};
ptyp_.onDataContextUpdated0 = function UISkinableElement$$OnDataContextUpdated(oldValue) {
  this.onDataContextUpdated(oldValue);
  if (this.skinInstance)
    this.skinInstance.updateDataContext();
};
ptyp_.V_OnBeforeFirstActivate = ptyp_.onBeforeFirstActivate0;
ptyp_.V_OnActivate = ptyp_.onActivate0;
ptyp_.V_OnDeactivate = ptyp_.onDeactivate0;
ptyp_.V_InternalDispose = ptyp_.internalDispose1;
ptyp_.V_OnDataContextUpdated = ptyp_.onDataContextUpdated0;
ptyp_.V_ApplySkinInternal = ptyp_.applySkinInternal;
Type$$RegisterReferenceType(UISkinableElement, "Sunlight.Framework.UI.UISkinableElement", UIElement, []);
function Node$$Remove(this_) {
  return this_.parentNode ? this_.parentNode.removeChild(this_) : this_;
};
function Element$$AddClassName(this_, className) {
  var index;
  this_.importedExtension = this_.importedExtension || Object$$GetNewImportedExtension();
  if (!Object$$IsNullOrUndefined(this_.classList)) {
    this_.classList.add(className);
    return;
  }
  if (className === null || (className = String$$Trim(className)).length == 0)
    return;
  if (this_.className === null || this_.className.length == 0) {
    this_.className = className;
    return;
  }
  index = 0;
  while ((index = this_.className.indexOf(className, index)) != -1) {
    if ((index == 0 || String$$get_Item(this_.className, index - 1) == 32) && (index == this_.className.length - className.length || String$$get_Item(this_.className, index + className.length) == 32))
      return;
    index++;
  }
  this_.className = this_.className + " " + className;
  return;
};
function Element$$RemoveClassName(this_, className) {
  var index;
  this_.importedExtension = this_.importedExtension || Object$$GetNewImportedExtension();
  if (!Object$$IsNullOrUndefined(this_.classList)) {
    this_.classList.remove(className);
    return;
  }
  if (className === null || (className = String$$Trim(className)).length == 0 || this_.className === null || this_.className.length == 0)
    return;
  index = 0;
  while ((index = this_.className.indexOf(className, index)) != -1) {
    if ((index == 0 || String$$get_Item(this_.className, index - 1) == 32) && (index == this_.className.length - className.length || String$$get_Item(this_.className, index + className.length) == 32)) {
      this_.className = this_.className.substr(0, index > 0 ? index - 1 : 0) + this_.className.substring(index + className.length);
      return;
    }
    index++;
  }
  return;
};
function Element$$Bind(this_, eventName, handler, capture) {
  this_.importedExtension = this_.importedExtension || Object$$GetNewImportedExtension();
  EventBinder$$AddEvent(this_, eventName, handler, capture);
};
function Element$$UnBind(this_, eventName, handler, capture) {
  this_.importedExtension = this_.importedExtension || Object$$GetNewImportedExtension();
  EventBinder$$RemoveEvent(this_, eventName, handler, capture);
};
function Element$$UnBind0(this_, eventName) {
  this_.importedExtension = this_.importedExtension || Object$$GetNewImportedExtension();
  EventBinder$$RemoveEvent1(this_, eventName, true);
  EventBinder$$RemoveEvent1(this_, eventName, false);
};
function TestViewModelB() {
};
TestViewModelB.typeId = "mb";
function TestViewModelB_factory() {
  var this_;
  this_ = new TestViewModelB();
  this_.__ctor1();
  return this_;
};
TestViewModelB.defaultConstructor = TestViewModelB_factory;
ptyp_ = new TestViewModelA();
TestViewModelB.prototype = ptyp_;
ptyp_.__ctor1 = function TestViewModelB$$$$ctor() {
  this.__ctor0();
};
Type$$RegisterReferenceType(TestViewModelB, "Sunlight.Framework.UI.Test.TestViewModelB", TestViewModelA, []);
function TestSkinableWithTestUIElementPart() {
};
TestSkinableWithTestUIElementPart.typeId = "nb";
function TestSkinableWithTestUIElementPart_factory(element) {
  var this_;
  this_ = new TestSkinableWithTestUIElementPart();
  this_.__ctor4(element);
  return this_;
};
ptyp_ = new UISkinableElement();
TestSkinableWithTestUIElementPart.prototype = ptyp_;
ptyp_.__ctor4 = function TestSkinableWithTestUIElementPart$$$$ctor(element) {
  this.__ctor3(element);
};
ptyp_.get_part = function TestSkinableWithTestUIElementPart$$get_Part() {
  if (this.get_skinInstance())
    return Type$$CastType(TestUIElement, this.get_skinInstance().getChildById("Part1"));
  return null;
};
Type$$RegisterReferenceType(TestSkinableWithTestUIElementPart, "Sunlight.Framework.UI.Test.TestSkinableWithTestUIElementPart", UISkinableElement, []);
function TestUIElement() {
};
TestUIElement.typeId = "ob";
function TestUIElement_factory(element) {
  var this_;
  this_ = new TestUIElement();
  this_.__ctor3(element);
  return this_;
};
ptyp_ = new UIElement();
TestUIElement.prototype = ptyp_;
ptyp_.twoWayLooseBinding = 0;
ptyp_.__ctor3 = function TestUIElement$$$$ctor(element) {
  this.__ctor2(element);
};
ptyp_.get_oneWayStrictBinding = function TestUIElement$$get_OneWayStrictBinding() {
  return this.OneWayStrictBinding;
};
ptyp_.set_oneWayStrictBinding = function TestUIElement$$set_OneWayStrictBinding(value) {
  return this.OneWayStrictBinding = value;
};
ptyp_.get_twoWayLooseBinding = function TestUIElement$$get_TwoWayLooseBinding() {
  return this.twoWayLooseBinding;
};
ptyp_.set_twoWayLooseBinding = function TestUIElement$$set_TwoWayLooseBinding(value) {
  if (this.twoWayLooseBinding != value) {
    this.twoWayLooseBinding = value;
    this.firePropertyChanged("TwoWayLooseBinding");
  }
};
Type$$RegisterReferenceType(TestUIElement, "Sunlight.Framework.UI.Test.TestUIElement", UIElement, []);
function SkinBinderHelper() {
};
function SkinBinderHelper$$Bind(binders, dataContext, targetElements) {
  var i, j, info;
  for (
  i = 0, j = binders.length; i < j; i++) {
    info = binders[i];
    SkinBinderHelper$$SetPropertyValue(info, dataContext, targetElements[info.objectIndex], null);
  }
};
function SkinBinderHelper$$SetAttribute(node, value, attrName) {
  if (value !== null)
    node.setAttribute(attrName, value);
  else
    node.removeAttribute(attrName);
};
function SkinBinderHelper$$SetTextContent(element, value) {
  if (value !== null)
    element.textContent = value;
  else
    element.textContent = "";
};
function SkinBinderHelper$$SetDataContext(element, value) {
  element.set_dataContext(value);
};
function SkinBinderHelper$$SetCssClass(element, add, className) {
  if (add)
    Element$$AddClassName(element, className);
  else
    Element$$RemoveClassName(element, className);
};
function SkinBinderHelper$$GetElementFromPath(element, path) {
  var iPath;
  for (iPath = 0; iPath < path.length; iPath++)
    element = element.childNodes[path[iPath]];
  return element;
};
function SkinBinderHelper$$SetPropertyValue(binder, source, target, extraElementArray) {
  var stmtTemp1, stmtTemp10;
  try {
    source = SkinBinderHelper$$TraversePropertyPath(binder, source);
  } catch (stmtTemp1) {
    source = binder.defaultValue;
  }
  try {
    binder.setTargetValue(target, source, extraElementArray);
  } catch (stmtTemp10) {
  }
};
function SkinBinderHelper$$TraversePropertyPath(binder, source) {
  var iGetter, pathLength, isStatic;
  iGetter = 0, pathLength = binder.propertyGetterPath.length;
  isStatic = (binder.binderType & 2) == 2;
  for (; iGetter < pathLength && (isStatic && iGetter == 0 || !Object$$IsNullOrUndefined(source)); iGetter++)
    source = binder.propertyGetterPath[iGetter](source);
  if (iGetter < pathLength)
    return binder.defaultValue;
  else if (binder.forwardConverter)
    source = binder.forwardConverter(source);
  return source;
};
function Boolean0(boxedValue) {
  this.boxedValue = boxedValue;
};
Boolean0.typeId = "pb";
Boolean0.getDefaultValue = function() {
  return false;
};
function Boolean$$ToString(this_) {
  return this_ ? "true" : "false";
};
ptyp_ = new ValueType();
Boolean0.prototype = ptyp_;
ptyp_.toString = function() {
  return Boolean$$ToString(this.boxedValue);
};
Type$$RegisterStructType(Boolean0, "System.Boolean", []);
function ListView() {
};
ListView.typeId = "qb";
function ListView_factory(element) {
  var this_;
  this_ = new ListView();
  this_.__ctor3(element);
  return this_;
};
ptyp_ = new UIElement();
ListView.prototype = ptyp_;
ptyp_.items = null;
ptyp_.observableList = null;
ptyp_.attachedObservableList = null;
ptyp_.fixedList = null;
ptyp_.itemSkin = null;
ptyp_.itemCssClassName = null;
ptyp_.inlineItems = false;
ptyp_.topN = 0;
ptyp_.selectionHelper = null;
ptyp_.__ctor3 = function ListView$$$$ctor(element) {
  this.__ctor2(element);
  this.items = List_$ListViewItem$_.defaultConstructor();
  this.topN = 1073741824;
};
ptyp_.set_fixedList = function ListView$$set_FixedList(value) {
  if (value && this.observableList)
    throw new Error("Can't set FixedList and ObservableList at the same time");
  if (this.fixedList != value) {
    this.fixedList = value;
    this.firePropertyChanged("FixedList");
    this.applyFixedList();
  }
};
ptyp_.set_itemSkin = function ListView$$set_ItemSkin(value) {
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
ptyp_.onActivate0 = function ListView$$OnActivate() {
  this.onActivate();
  if (this.fixedList)
    this.applyFixedList();
  else if (this.observableList)
    this.applyObservableList();
};
ptyp_.onDeactivate0 = function ListView$$OnDeactivate() {
  var items, itemCount, iItem;
  items = this.items;
  itemCount = items.get_count();
  if (itemCount > 0)
    for (iItem = 0; iItem < itemCount; iItem++)
      items.get_item(iItem).deactivate();
  this.onDeactivate();
};
ptyp_.internalDispose1 = function ListView$$InternalDispose() {
  var items, itemCount, iItem;
  items = this.items;
  itemCount = items.get_count();
  if (this.attachedObservableList) {
    this.attachedObservableList.V_remove_CollectionChanged_i(Delegate$$Create("observableListCollectionChanged", this));
    this.attachedObservableList = null;
  }
  if (itemCount > 0) {
    for (iItem = 0; iItem < itemCount; iItem++)
      items.get_item(iItem).dispose();
    items.clear();
  }
  this.internalDispose0();
};
ptyp_.applyFixedList = function ListView$$ApplyFixedList() {
  var items, itemsCount, iItem, item, fixedList, fixedListCount, iObject, listViewItem;
  items = this.items;
  itemsCount = items.get_count();
  if (!this.fixedList) {
    for (iItem = 0; iItem < itemsCount; iItem++) {
      item = items.get_item(iItem);
      item.dispose();
      Node$$Remove(item.get_element());
    }
    items.clear();
    return;
  }
  if (this.get_isActive()) {
    fixedList = this.fixedList;
    fixedListCount = Math.min(fixedList.V_get_Count_c(), this.topN);
    for (iObject = 0; iObject < fixedListCount; iObject++) {
      if (iObject < itemsCount) {
        listViewItem = items.get_item(iObject);
        listViewItem.set_isSelected(this.selectionHelper ? this.selectionHelper.V_IsSelected_d(listViewItem.get_dataContext()) : false);
      }
      else {
        listViewItem = ListViewItem_factory(this.createElement());
        if (this.itemCssClassName !== null)
          listViewItem.get_element().className = this.itemCssClassName;
        if (!this.inlineItems)
          this.get_element().appendChild(listViewItem.get_element());
        else
          this.get_element().parentNode.insertBefore(listViewItem.get_element(), this.get_element());
        listViewItem.set_skin(this.itemSkin);
        items.add(listViewItem);
      }
      listViewItem.set_dataContext(fixedList.V_get_Item_e(iObject));
      listViewItem.set_selectionHelper(this.selectionHelper);
      listViewItem.activate();
    }
    this.removeChildren(fixedListCount, itemsCount - fixedListCount);
  }
};
ptyp_.applyObservableList = function ListView$$ApplyObservableList() {
  var items, itemsCount, iItem, item;
  items = this.items;
  itemsCount = items.get_count();
  if (!this.observableList) {
    for (iItem = 0; iItem < itemsCount; iItem++) {
      item = items.get_item(iItem);
      item.dispose();
      Node$$Remove(item.get_element());
    }
    items.clear();
    return;
  }
  if (this.get_isActive() && this.observableList && this.observableList != this.attachedObservableList) {
    this.attachedObservableList = this.observableList;
    this.attachedObservableList.V_add_CollectionChanged_i(Delegate$$Create("observableListCollectionChanged", this));
    this.resetObservableItems();
  }
};
ptyp_.observableListCollectionChanged = function ListView$$ObservableListCollectionChanged(collection, args) {
  var items, changeIndex, newItems, oldItems, itemCount, addCount, replaceCount, list, i, replaceList, replaceStartIndex;
  Debug$$Assert(collection == this.attachedObservableList);
  items = this.items;
  changeIndex = args.V_get_ChangeIndex_j();
  if (args.V_get_Action_j() == 4)
    this.resetObservableItems();
  if (changeIndex > this.topN)
    return;
  newItems = args.V_get_NewItems_j();
  oldItems = args.V_get_OldItems_j();
  itemCount = args.V_get_Action_j() == 1 ? oldItems.V_get_Count_c() : newItems.V_get_Count_c();
  switch(args.V_get_Action_j()) {
    case 0: {
      if (changeIndex + itemCount + this.items.get_count() > this.topN)
        if (this.items.get_count() >= this.topN)
          this.observableEventReplace(changeIndex, this.topN - changeIndex, newItems);
        else {
          addCount = this.topN - this.items.get_count();
          this.observableEventAdd(changeIndex, this.topN - this.items.get_count(), newItems);
          replaceCount = this.topN - changeIndex - addCount;
          list = List_$Object$_.defaultConstructor();
          for (i = addCount; i < replaceCount && i < itemCount; i++)
            list.add(newItems.V_get_Item_e(i));
          this.observableEventAdd(changeIndex + addCount, replaceCount, list);
        }
      else
        this.observableEventAdd(changeIndex, itemCount, newItems);
      break;
    }
    case 1: {
      if (this.observableList.V_get_Count_k() <= this.topN)
        this.removeChildren(args.V_get_ChangeIndex_j(), args.V_get_OldItems_j().V_get_Count_c());
      else {
        replaceList = List_$Object$_.defaultConstructor();
        replaceStartIndex = changeIndex + itemCount;
        replaceCount = Math.min(changeIndex + itemCount, Math.min(this.topN, this.observableList.V_get_Count_k() - itemCount)) - changeIndex;
        for (i = 0; i < replaceCount; i++)
          replaceList.add(this.observableList.V_get_Item_k(replaceStartIndex + i));
        this.observableEventReplace(changeIndex, replaceCount, replaceList);
        if (this.observableList.V_get_Count_k() - itemCount <= this.topN)
          this.removeChildren(changeIndex + replaceCount, this.items.get_count() - changeIndex - replaceCount);
        throw new Error("Not Tested");
      }
      break;
    }
    case 2: {
      this.observableEventReplace(changeIndex, Math.min(changeIndex + itemCount, this.topN) - changeIndex, newItems);
      break;
    }
    default:
    throw new Error("Invalid operation");
  }
};
ptyp_.observableEventReplace = function ListView$$ObservableEventReplace(changeIndex, listCount, list) {
  var iObject;
  for (iObject = 0; iObject < listCount; iObject++)
    this.items.get_item(changeIndex + iObject).set_dataContext(list.V_get_Item_e(iObject));
};
ptyp_.observableEventAdd = function ListView$$ObservableEventAdd(changeIndex, listCount, list) {
  var insertBeforeElem, iObject, listViewItem;
  insertBeforeElem = null;
  if (changeIndex < this.items.get_count())
    insertBeforeElem = this.items.get_item(changeIndex).get_element();
  for (iObject = 0; iObject < listCount; iObject++) {
    listViewItem = ListViewItem_factory(this.createElement());
    if (this.itemCssClassName !== null)
      listViewItem.get_element().className = this.itemCssClassName;
    listViewItem.set_skin(this.itemSkin);
    if (!insertBeforeElem) {
      if (this.inlineItems)
        this.get_element().parentNode.insertBefore(listViewItem.get_element(), this.get_element());
      else
        this.get_element().appendChild(listViewItem.get_element());
      this.items.add(listViewItem);
    }
    else {
      if (this.inlineItems)
        this.get_element().parentNode.insertBefore(listViewItem.get_element(), insertBeforeElem);
      else
        this.get_element().insertBefore(listViewItem.get_element(), insertBeforeElem);
      this.items.insert(changeIndex + iObject, listViewItem);
    }
    listViewItem.set_dataContext(list.V_get_Item_e(iObject));
    listViewItem.set_selectionHelper(this.selectionHelper);
    listViewItem.activate();
  }
};
ptyp_.resetObservableItems = function ListView$$ResetObservableItems() {
  var observableList, itemsCount, listCount, iObject, listViewItem;
  observableList = this.observableList;
  itemsCount = this.items.get_count();
  listCount = Math.min(observableList.V_get_Count_k(), this.topN);
  for (iObject = 0; iObject < listCount; iObject++) {
    if (iObject < itemsCount) {
      listViewItem = this.items.get_item(iObject);
      listViewItem.set_isSelected(this.selectionHelper ? this.selectionHelper.V_IsSelected_d(listViewItem.get_dataContext()) : false);
    }
    else {
      listViewItem = ListViewItem_factory(this.createElement());
      if (this.itemCssClassName !== null)
        listViewItem.get_element().className = this.itemCssClassName;
      if (!this.inlineItems)
        this.get_element().appendChild(listViewItem.get_element());
      else
        this.get_element().parentNode.insertBefore(listViewItem.get_element(), this.get_element());
      listViewItem.set_skin(this.itemSkin);
      this.items.add(listViewItem);
    }
    listViewItem.set_dataContext(observableList.V_get_Item_k(iObject));
    listViewItem.set_selectionHelper(this.selectionHelper);
    listViewItem.activate();
  }
  this.removeChildren(listCount, itemsCount - listCount);
};
ptyp_.removeChildren = function ListView$$RemoveChildren(changeIndex, delCount) {
  var iObject, item;
  for (iObject = delCount + changeIndex - 1; iObject >= changeIndex; iObject--) {
    item = this.items.get_item(iObject);
    this.items.removeAt(iObject);
    Node$$Remove(item.get_element());
    item.dispose();
  }
};
ptyp_.createElement = function ListView$$CreateElement() {
  return this.get_element().ownerDocument.createElement(this.inlineItems ? "div" : "li");
};
ptyp_.V_OnActivate = ptyp_.onActivate0;
ptyp_.V_OnDeactivate = ptyp_.onDeactivate0;
ptyp_.V_InternalDispose = ptyp_.internalDispose1;
Type$$RegisterReferenceType(ListView, "Sunlight.Framework.UI.ListView", UIElement, []);
function Task() {
};
Task.typeId = "rb";
function Task_factory(taskId, work) {
  var this_;
  this_ = new Task();
  this_.__ctor(taskId, work);
  return this_;
};
ptyp_ = Task.prototype;
ptyp_.state = 0;
ptyp_.work = null;
ptyp_.taskId = null;
ptyp_.__ctor = function Task$$$$ctor(taskId, work) {
  this.taskId = taskId;
  this.work = work;
};
Type$$RegisterReferenceType(Task, "Sunlight.Framework.Task", Object, []);
function Skin() {
};
Skin.typeId = "sb";
function Skin_factory(skinableType, dataContextType, factoryMethod, id) {
  var this_;
  this_ = new Skin();
  this_.__ctor(skinableType, dataContextType, factoryMethod, id);
  return this_;
};
ptyp_ = Skin.prototype;
ptyp_.factoryMethod = null;
ptyp_.skinableType = null;
ptyp_.dataContextType = null;
ptyp_.id = null;
ptyp_.__ctor = function Skin$$$$ctor(skinableType, dataContextType, factoryMethod, id) {
  this.factoryMethod = factoryMethod;
  this.skinableType = skinableType;
  this.dataContextType = dataContextType;
  this.id = id;
};
ptyp_.get_id = function Skin$$get_Id() {
  return this.id;
};
ptyp_.get_skinableType = function Skin$$get_SkinableType() {
  return this.skinableType;
};
ptyp_.createInstance = function Skin$$CreateInstance() {
  return this.factoryMethod(this, window.document);
};
Type$$RegisterReferenceType(Skin, "Sunlight.Framework.UI.Skin", Object, []);
function SkinInstance() {
};
SkinInstance.typeId = "tb";
function SkinInstance_factory(factory, rootElement, childElements, elementsOfIntrests, binders, partIdMapping, liveBinderCount, extraObjectCount) {
  var this_;
  this_ = new SkinInstance();
  this_.__ctor(factory, rootElement, childElements, elementsOfIntrests, binders, partIdMapping, liveBinderCount, extraObjectCount);
  return this_;
};
ptyp_ = SkinInstance.prototype;
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
ptyp_.__ctor = function SkinInstance$$$$ctor(factory, rootElement, childElements, elementsOfIntrests, binders, partIdMapping, liveBinderCount, extraObjectCount) {
  Object$$IsNullOrUndefined(rootElement);
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
    this.partIdMapping = StringDictionary_$Int32$_.__ctor(partIdMapping);
};
ptyp_.getChildById = function SkinInstance$$GetChildById(id) {
  if (this.partIdMapping && this.partIdMapping.containsKey(id))
    return this.elementsOfIntrest[this.partIdMapping.get_item(id)];
  return null;
};
ptyp_.bind = function SkinInstance$$Bind(skinable) {
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
ptyp_.updateDataContext = function SkinInstance$$UpdateDataContext() {
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
ptyp_.activate = function SkinInstance$$Activate() {
  var childElements, binders, childElementLength, elementsOfIntrest, binderLength, dataContext, dataContextSetter, iBinder, iLiveBinder, binder, source, liveBinder, iChild, objectIndex, childElement;
  if (!this.isActive && !this.isDiposed) {
    this.isActive = true;
    childElements = this.childElements;
    binders = this.binders;
    childElementLength = childElements.length;
    elementsOfIntrest = this.elementsOfIntrest;
    binderLength = binders.length;
    dataContext = this.dataContext;
    dataContextSetter = SkinBinderHelper$$SetDataContext;
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
        if (Object$$IsNullOrUndefined(liveBinder)) {
          liveBinder = LiveBinder_factory(binder, this.extraObjects);
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
        SkinBinderHelper$$SetPropertyValue(binder, source, elementsOfIntrest[binder.objectIndex], this.extraObjects);
        if (binder.targetPropertySetter === dataContextSetter)
          this.hasDataContextBinding[binder.objectIndex] = true;
      }
      if (binder.mode != 0)
        ++iLiveBinder;
    }
    for (iChild = 0; iChild < childElementLength; iChild++) {
      objectIndex = childElements[iChild];
      childElement = NativeArray$$GetFrom(elementsOfIntrest, childElements[iChild]);
      if (!this.hasDataContextBinding[objectIndex])
        childElement.set_dataContext(dataContext);
      childElement.activate();
    }
    this.firstActivationDone = true;
    TaskScheduler$$get_Instance().enqueueLowPriTask(Delegate$$Create("queuedActivation", this), "SkinInstance.Activate");
  }
};
ptyp_.deactivate = function SkinInstance$$Deactivate() {
  var childElements, childElementLength, liveBinders, liveBinderLength, iLiveBinder, iChild;
  if (this.isActive && !this.isDiposed) {
    this.isActive = false;
    childElements = this.childElements;
    childElementLength = childElements.length;
    liveBinders = this.liveBinders;
    if (!Object$$IsNullOrUndefined(liveBinders)) {
      liveBinderLength = liveBinders.length;
      for (iLiveBinder = 0; iLiveBinder < liveBinderLength; iLiveBinder++) {
        if (Object$$IsNullOrUndefined(liveBinders[iLiveBinder]))
          continue;
        liveBinders[iLiveBinder].set_isActive(false);
      }
    }
    for (iChild = 0; iChild < childElementLength; iChild++)
      NativeArray$$GetFrom(this.elementsOfIntrest, childElements[iChild]).deactivate();
    TaskScheduler$$get_Instance().enqueueLowPriTask(Delegate$$Create("queuedDeactivation", this), "SkinInstance.QueuedDeactivate");
  }
};
ptyp_.dispose = function SkinInstance$$Dispose() {
  var childNodes, iLiveBinder, liveBinder, i, j, childElement;
  if (!this.isDiposed) {
    if (this.skinableParent) {
      childNodes = this.skinableParent.get_element().childNodes;
      while (childNodes.length > 0)
        this.rootElement.appendChild(childNodes[0]);
    }
    if (!Object$$IsNullOrUndefined(this.liveBinders))
      for (iLiveBinder = 0; iLiveBinder < this.liveBinders.length; iLiveBinder++) {
        liveBinder = this.liveBinders[iLiveBinder];
        if (Object$$IsNullOrUndefined(liveBinder))
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
      childElement = NativeArray$$GetFrom(this.elementsOfIntrest, this.childElements[i]);
      childElement.deactivate();
      childElement.dispose();
    }
  }
};
ptyp_.queuedActivation = function SkinInstance$$QueuedActivation() {
  var binders, liveBinders, binderLength, liveBindersLength, iBinderInfo, iLivebinder, binder, liveBinder;
  binders = this.binders;
  liveBinders = this.liveBinders;
  if (Object$$IsNullOrUndefined(liveBinders))
    return;
  binderLength = binders.length;
  liveBindersLength = liveBinders.length;
  for (
  iBinderInfo = 0, iLivebinder = 0; iBinderInfo < binderLength && iLivebinder < liveBindersLength; iBinderInfo++) {
    binder = binders[iBinderInfo];
    if (binder.mode != 0) {
      liveBinder = liveBinders[iLivebinder];
      if (Object$$IsNullOrUndefined(liveBinder)) {
        liveBinders[iLivebinder] = liveBinder = LiveBinder_factory(binder, this.extraObjects);
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
ptyp_.queuedDeactivation = function SkinInstance$$QueuedDeactivation() {
  var iLiveBinder, liveBinder;
  if (this.isActive || this.isDiposed || Object$$IsNullOrUndefined(this.liveBinders))
    return;
  for (iLiveBinder = 0; iLiveBinder < this.liveBinders.length; iLiveBinder++) {
    liveBinder = this.liveBinders[iLiveBinder];
    if (Object$$IsNullOrUndefined(liveBinder))
      return;
    liveBinder.set_isActive(false);
    liveBinder.cleanup();
  }
};
ptyp_.updateBinderSource = function SkinInstance$$UpdateBinderSource(source, sourceType) {
  var liveBinders, binders, bindersLength, liveBindersLength, iBinder, iLiveBinder, binder, childElements, childElementLength, iChild, objectIndex, childElement;
  liveBinders = this.liveBinders;
  binders = this.binders;
  bindersLength = binders.length;
  liveBindersLength = Object$$IsNullOrUndefined(liveBinders) ? 0 : liveBinders.length;
  for (
  iBinder = 0, iLiveBinder = 0; iBinder < bindersLength; iBinder++) {
    binder = binders[iBinder];
    if (binder.mode != 0 && iLiveBinder < liveBindersLength && !Object$$IsNullOrUndefined(liveBinders[iLiveBinder])) {
      if (sourceType == (binder.binderType & 7))
        liveBinders[iLiveBinder].set_source(source);
      iLiveBinder++;
    }
    else if (sourceType == (binder.binderType & 7))
      SkinBinderHelper$$SetPropertyValue(binder, source, this.elementsOfIntrest[binder.objectIndex], this.extraObjects);
  }
  if (sourceType == 1) {
    childElements = this.childElements;
    childElementLength = childElements.length;
    for (iChild = 0; iChild < childElementLength; iChild++) {
      objectIndex = childElements[iChild];
      childElement = NativeArray$$GetFrom(this.elementsOfIntrest, childElements[iChild]);
      if (!this.hasDataContextBinding[objectIndex])
        childElement.set_dataContext(this.dataContext);
      childElement.activate();
    }
  }
};
Type$$RegisterReferenceType(SkinInstance, "Sunlight.Framework.UI.Helpers.SkinInstance", Object, []);
function ListViewItem() {
};
ListViewItem.typeId = "ub";
function ListViewItem_factory(element) {
  var this_;
  this_ = new ListViewItem();
  this_.__ctor4(element);
  return this_;
};
ptyp_ = new UISkinableElement();
ListViewItem.prototype = ptyp_;
ptyp_.isSelected = false;
ptyp_.selectionHelper = null;
ptyp_.__ctor4 = function ListViewItem$$$$ctor(element) {
  this.__ctor3(element);
};
ptyp_.set_isSelected = function ListViewItem$$set_IsSelected(value) {
  if (this.isSelected != value) {
    this.isSelected = value;
    if (this.selectionHelper)
      if (value)
        this.selectionHelper.V_SelectItem_d(this.get_dataContext());
      else
        this.selectionHelper.V_UnSelectItem_d(this.get_dataContext());
    this.firePropertyChanged("IsSelected");
  }
};
ptyp_.set_selectionHelper = function ListViewItem$$set_SelectionHelper(value) {
  if (this.selectionHelper == value)
    return;
  if (this.selectionHelper)
    this.selectionHelper.V_remove_SelectionChanged_d(Delegate$$Create("onSelectionHelperSelectionChanged", this));
  this.selectionHelper = value;
  if (this.selectionHelper) {
    this.selectionHelper.V_add_SelectionChanged_d(Delegate$$Create("onSelectionHelperSelectionChanged", this));
    this.onSelectionHelperSelectionChanged();
  }
};
ptyp_.onDataContextUpdated1 = function ListViewItem$$OnDataContextUpdated(oldValue) {
  this.onDataContextUpdated0(oldValue);
  if (this.selectionHelper)
    this.onSelectionHelperSelectionChanged();
};
ptyp_.internalDispose2 = function ListViewItem$$InternalDispose() {
  if (this.selectionHelper)
    this.selectionHelper.V_remove_SelectionChanged_d(Delegate$$Create("onSelectionHelperSelectionChanged", this));
  this.internalDispose1();
};
ptyp_.onSelectionHelperSelectionChanged = function ListViewItem$$OnSelectionHelperSelectionChanged() {
  this.set_isSelected(this.selectionHelper.V_IsSelected_d(this.get_dataContext()));
};
ptyp_.V_InternalDispose = ptyp_.internalDispose2;
ptyp_.V_OnDataContextUpdated = ptyp_.onDataContextUpdated1;
Type$$RegisterReferenceType(ListViewItem, "Sunlight.Framework.UI.ListViewItem", UISkinableElement, []);
Error.typeId = "vb";
Type$$RegisterReferenceType(Error, "System.Exception", Object, []);
function UIEvent() {
};
UIEvent.typeId = "wb";
Type$$RegisterReferenceType(UIEvent, "Sunlight.Framework.UI.UIEvent", Object, []);
function ISelectionHelper() {
};
ISelectionHelper.typeId = "d";
Type$$RegisterInterface(ISelectionHelper, "Sunlight.Framework.UI.ISelectionHelper");
function NativeArray$$GetFrom(this_, index) {
  return this_[index];
};
function Enum() {
};
Enum.typeId = "xb";
ptyp_ = new ValueType();
Enum.prototype = ptyp_;
ptyp_.toString0 = function Enum$$ToString() {
  var enumType, value, rv;
  enumType = this.constructor;
  value = this.boxedValue;
  rv = enumType.enumValueToStrMap[value];
  return typeof rv === "undefined" ? value.toString() : rv;
};
ptyp_.toString = ptyp_.toString0;
Type$$RegisterReferenceType(Enum, "System.Enum", ValueType, []);
function BinderType(boxedValue) {
  this.boxedValue = boxedValue;
};
BinderType.typeId = "yb";
BinderType.enumStrToValueMap = {
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
BinderType.getDefaultValue = function() {
  return 0;
};
BinderType.prototype = new Enum();
Type$$RegisterEnum(BinderType, "Sunlight.Framework.UI.Helpers.BinderType", true);
Function.getDefaultValue = function() {
  return {
  };
};
function WindowTimer() {
};
WindowTimer.typeId = "zb";
function WindowTimer_factory() {
  return new WindowTimer();
};
WindowTimer.defaultConstructor = WindowTimer_factory;
ptyp_ = WindowTimer.prototype;
ptyp_.setImmediate = function WindowTimer$$SetImmediate(action) {
  if (typeof setImmediate != "undefined")
    return setImmediate(action);
  return setTimeout(action, 0);
};
ptyp_.setTimeout = function WindowTimer$$SetTimeout(action, timeoutHandle) {
  if (timeoutHandle == 0)
    return this.setImmediate(action);
  return setTimeout(action, timeoutHandle);
};
ptyp_.clearTimeout = function WindowTimer$$ClearTimeout(timeoutHandle) {
  if (typeof clearImmediate != "undefined")
    clearImmediate(timeoutHandle);
  clearTimeout(timeoutHandle);
};
ptyp_.__ctor = function WindowTimer$$$$ctor() {
};
ptyp_.V_SetImmediate_f = ptyp_.setImmediate;
ptyp_.V_SetTimeout_f = ptyp_.setTimeout;
ptyp_.V_ClearTimeout_f = ptyp_.clearTimeout;
Type$$RegisterReferenceType(WindowTimer, "Sunlight.Framework.WindowTimer", Object, [IWindowTimer]);
function TaskHandle() {
};
TaskHandle.typeId = "ac";
function TaskHandle_factory(taskId) {
  var this_;
  this_ = new TaskHandle();
  this_.__ctor(taskId);
  return this_;
};
ptyp_ = TaskHandle.prototype;
ptyp_.taskId = 0;
ptyp_.__ctor = function TaskHandle$$$$ctor(taskId) {
  this.taskId = taskId;
};
Type$$RegisterReferenceType(TaskHandle, "Sunlight.Framework.TaskHandle", Object, []);
function EventBinder() {
};
EventBinder.typeId = "bc";
function EventBinder$$GetBinder(importedElement) {
  if (Object$$IsNullOrUndefined(importedElement.importedExtension))
    importedElement.importedExtension = {
    };
  if (Object$$IsNullOrUndefined(importedElement.importedExtension.importedExtension))
    importedElement.importedExtension.importedExtension = EventBinder_factory(importedElement);
  return Type$$CastType(EventBinder, importedElement.importedExtension.importedExtension);
};
function EventBinder$$AddEvent(importedElement, name, action, onCapture) {
  var binder;
  binder = EventBinder$$GetBinder(importedElement);
  binder.addEvent(name, action, onCapture);
};
function EventBinder$$RemoveEvent(importedElement, name, action, onCapture) {
  var binder;
  if (importedElement.importedExtension === null || importedElement.importedExtension.importedExtension === null)
    return;
  binder = EventBinder$$GetBinder(importedElement);
  binder.removeEvent(name, action, onCapture);
};
function EventBinder$$RemoveEvent1(importedElement, name, onCapture) {
  var binder;
  if (importedElement.importedExtension === null || importedElement.importedExtension.importedExtension === null)
    return;
  binder = EventBinder$$GetBinder(importedElement);
  binder.removeEvent0(name, onCapture);
};
function EventBinder$$IsW3wc(element) {
  return !!element.addEventListener;
};
function EventBinder$$GetEventType(evt) {
  return evt.type;
};
function EventBinder_factory(element) {
  var this_;
  this_ = new EventBinder();
  this_.__ctor(element);
  return this_;
};
ptyp_ = EventBinder.prototype;
ptyp_.capturePhaseEvents = null;
ptyp_.bubblePhaseEvents = null;
ptyp_.target = null;
ptyp_.disposed = false;
ptyp_.__ctor = function EventBinder$$$$ctor(element) {
  this.capturePhaseEvents = StringDictionary_$Function$_.defaultConstructor();
  this.bubblePhaseEvents = StringDictionary_$Function$_.defaultConstructor();
  this.target = element;
};
ptyp_.addEvent = function EventBinder$$AddEvent0(name, action, onCapture) {
  var isW3wc, evts, elementEvent;
  isW3wc = EventBinder$$IsW3wc(this.target);
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
    if (onCapture && EventBinder$$IsW3wc(this.target))
      this.addEventListener(name, Delegate$$Create("eventHandlerCapture", this), true);
    else if (isW3wc)
      this.addEventListener(name, Delegate$$Create("eventHandlerBubble", this), false);
    else
      this.attachEvent(name, Delegate$$Create("eventHandlerIE", this));
  }
  else
    elementEvent = Delegate$$Combine(elementEvent, action);
  evts.set_item(name, elementEvent);
};
ptyp_.removeEvent = function EventBinder$$RemoveEvent0(name, handler, onCapture) {
  var isW3wc, evts, elementEvent;
  isW3wc = EventBinder$$IsW3wc(this.target);
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
    elementEvent = Delegate$$Remove(elementEvent, handler);
    if (!elementEvent) {
      evts.remove(name);
      if (onCapture)
        this.removeEventListener(name, Delegate$$Create("eventHandlerCapture", this), true);
      else if (isW3wc)
        this.removeEventListener(name, Delegate$$Create("eventHandlerBubble", this), false);
      else
        this.detachEvent(name, Delegate$$Create("eventHandlerIE", this));
    }
    else
      evts.set_item(name, elementEvent);
  }
};
ptyp_.removeEvent0 = function EventBinder$$RemoveEvent2(name, onCapture) {
  var isW3wc, evts;
  isW3wc = EventBinder$$IsW3wc(this.target);
  onCapture = onCapture && isW3wc;
  evts = onCapture ? this.capturePhaseEvents : this.bubblePhaseEvents;
  if (evts.remove(name))
    if (onCapture)
      this.removeEventListener(name, Delegate$$Create("eventHandlerCapture", this), true);
    else if (isW3wc)
      this.removeEventListener(name, Delegate$$Create("eventHandlerBubble", this), true);
    else
      this.detachEvent(name, Delegate$$Create("eventHandlerIE", this));
};
ptyp_.addEventListener = function EventBinder$$AddEventListener(evtName, cb, isCapture) {
  this.target.addEventListener(evtName, cb, isCapture);
};
ptyp_.attachEvent = function EventBinder$$AttachEvent(evtName, cb) {
  this.target.atachEvent("on" + evtName, cb);
};
ptyp_.removeEventListener = function EventBinder$$RemoveEventListener(evtName, cb, isCapture) {
  this.target.removeEventListener(evtName, cb, isCapture);
};
ptyp_.detachEvent = function EventBinder$$DetachEvent(evtName, cb) {
  this.target.detachEvent("on" + evtName, cb);
};
ptyp_.eventHandlerIE = function EventBinder$$EventHandlerIE() {
  this.eventHandlerBubble(event);
};
ptyp_.eventHandlerCapture = function EventBinder$$EventHandlerCapture(evt) {
  if (this.disposed)
    return;
  this.capturePhaseEvents.get_item(EventBinder$$GetEventType(evt))(this.target, evt);
};
ptyp_.eventHandlerBubble = function EventBinder$$EventHandlerBubble(evt) {
  var del;
  if (this.disposed)
    return;
  if (this.bubblePhaseEvents.tryGetValue(EventBinder$$GetEventType(evt), {
    read: function() {
      return del;
    },
    write: function(arg0) {
      return del = arg0;
    }
  }))
    del(this.target, evt);
};
Type$$RegisterReferenceType(EventBinder, "System.EventBinder", Object, []);
RegExp.typeId = "cc";
Type$$RegisterReferenceType(RegExp, "System.RegularExpression", Object, []);
Date.typeId = "dc";
function DateTime$$op_Addition(a, n) {
  return new Date(a.valueOf() + n);
};
function DateTime$$get_Now() {
  return new Date();
};
function DateTime$$$$cctor() {
  Date.empty = new Date(0);
};
Type$$RegisterReferenceType(Date, "System.DateTime", Object, []);
function IEnumerator() {
};
IEnumerator.typeId = "h";
Type$$RegisterInterface(IEnumerator, "System.Collections.IEnumerator");
function INotifyCollectionChanged() {
};
INotifyCollectionChanged.typeId = "i";
Type$$RegisterInterface(INotifyCollectionChanged, "Sunlight.Framework.Observables.INotifyCollectionChanged");
function Debug$$Assert(condition) {
};
function CollectionChangedEventArgs() {
};
CollectionChangedEventArgs.typeId = "j";
Type$$RegisterInterface(CollectionChangedEventArgs, "Sunlight.Framework.Observables.CollectionChangedEventArgs");
function IObservableCollection() {
};
IObservableCollection.typeId = "k";
Type$$RegisterInterface(IObservableCollection, "Sunlight.Framework.Observables.IObservableCollection");
Number.typeId = "m";
Type$$RegisterReferenceType(Number, "System.Number", Object, []);
function NotSupportedException() {
};
NotSupportedException.typeId = "uc";
function NotSupportedException_factory() {
  var this_;
  this_ = new NotSupportedException();
  this_.__ctor();
  return this_;
};
NotSupportedException.defaultConstructor = NotSupportedException_factory;
ptyp_ = new Error();
NotSupportedException.prototype = ptyp_;
ptyp_.__ctor = function NotSupportedException$$$$ctor() {
};
Type$$RegisterReferenceType(NotSupportedException, "System.NotSupportedException", Error, []);
Array.typeId = "vc";
Type$$RegisterReferenceType(Array, "System.Array", Object, [IList, IEnumerable, ICollection]);
function ValueIfTrue(T, _callStatiConstructor) {
  var ValueIfTrue$1_$T$_, __initTracker;
  if (ValueIfTrue[T.typeId])
    return ValueIfTrue[T.typeId];
  ValueIfTrue[T.typeId] = function Sunlight$$Framework$$UI$$Test$$ValueIfTrue$10() {
  };
  ValueIfTrue$1_$T$_ = ValueIfTrue[T.typeId];
  ValueIfTrue$1_$T$_.genericParameters = [T];
  ValueIfTrue$1_$T$_.genericClosure = ValueIfTrue;
  ValueIfTrue$1_$T$_.typeId = "ec$" + T.typeId + "$";
  ValueIfTrue$1_$T$_.__ctor = function Sunlight_Framework_UI_Test_ValueIfTrue$1_factory0(value) {
    var this_;
    this_ = new ValueIfTrue$1_$T$_();
    this_.__ctor(value);
    return this_;
  };
  ptyp_ = ValueIfTrue$1_$T$_.prototype;
  ptyp_.value = Type$$GetDefaultValueStatic(T);
  ptyp_.defaultValue = Type$$GetDefaultValueStatic(T);
  ptyp_.__ctor = function ValueIfTrue$1$$$$ctor(value) {
    this.value = value;
    this.defaultValue = Type$$GetDefaultValueStatic(T);
  };
  Type$$RegisterReferenceType(ValueIfTrue$1_$T$_, "Sunlight.Framework.UI.Test.ValueIfTrue`1<" + T.fullName + ">", Object, []);
  ValueIfTrue$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    ValueIfTrue$1_$T$_ = ValueIfTrue(T, true);
  };
  if (_callStatiConstructor)
    ValueIfTrue$1_$T$_._tri();
  return ValueIfTrue$1_$T$_;
};
function ArrayG(T, _callStatiConstructor) {
  var Enumerator_$T$_, ArrayG$1_$T$_, IList$1_$T$_, IEnumerable$1_$T$_, __initTracker, T$5b$5d_$T$_, __initTracker0;
  if (ArrayG[T.typeId])
    return ArrayG[T.typeId];
  ArrayG[T.typeId] = function System$$ArrayG$10() {
  };
  ArrayG$1_$T$_ = ArrayG[T.typeId];
  ArrayG$1_$T$_.genericParameters = [T];
  ArrayG$1_$T$_.genericClosure = ArrayG;
  ArrayG$1_$T$_.typeId = "fc$" + T.typeId + "$";
  IList$1_$T$_ = IList0(T, _callStatiConstructor);
  IEnumerable$1_$T$_ = IEnumerable0(T, _callStatiConstructor);
  ArrayG$1_$T$_.__ctor0 = function System_ArrayG$1_factory1(size) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctor0(size);
    return this_;
  };
  ArrayG$1_$T$_.__ctor = function System_ArrayG$1_factory2(nativeArray) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctor1(nativeArray);
    return this_;
  };
  ptyp_ = new ArrayImpl();
  ArrayG$1_$T$_.prototype = ptyp_;
  ptyp_.innerArray = null;
  ptyp_.system$$Collections$$Generic$$IList_$T$_$$Insert = function ArrayG$1$$System$$Collections$$Generic$$IList_$T$_$$Insert(index, item) {
    throw new Error("Not Implemented.");
  };
  ptyp_.system$$Collections$$Generic$$IEnumerable_$T$_$$GetEnumerator = function ArrayG$1$$System$$Collections$$Generic$$IEnumerable_$T$_$$GetEnumerator() {
    return Enumerator_$T$_.__ctor(this);
  };
  ptyp_.__ctor0 = function ArrayG$1$$$$ctor0(size) {
    var def, i;
    this.__ctor();
    this.innerArray = new Array(size);
    def = Type$$GetDefaultValueStatic(T);
    for (i = 0; i < size; i++)
      this.innerArray[i] = def;
  };
  ptyp_.__ctor1 = function ArrayG$1$$$$ctor(nativeArray) {
    this.__ctor();
    this.innerArray = nativeArray;
  };
  ptyp_.get_length = function ArrayG$1$$get_Length() {
    return this.innerArray.length;
  };
  ptyp_.get_item = function ArrayG$1$$get_Item(index) {
    var arr;
    arr = this.innerArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index];
  };
  ptyp_.set_item = function ArrayG$1$$set_Item(index, value) {
    var arr;
    arr = this.innerArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index] = value;
  };
  ptyp_.get_innerArray = function ArrayG$1$$get_InnerArray() {
    return this.innerArray;
  };
  ptyp_.contains = function ArrayG$1$$Contains(item) {
    return this.V_IndexOf(item) >= 0;
  };
  ptyp_.indexOf = function ArrayG$1$$IndexOf(item) {
    if (!T.isInstanceOfType(item))
      return -1;
    return NativeArray$1$$IndexOf(this.innerArray, Type$$UnBoxTypeInstance(T, item), 0);
  };
  ptyp_.getValue = function ArrayG$1$$GetValue(index) {
    return Type$$BoxTypeInstance(T, this.get_item(index));
  };
  ptyp_.copyTo = function ArrayG$1$$CopyTo0(arr, index) {
    var nativeArray, length, nativeArrDst, i;
    nativeArray = this.innerArray;
    length = nativeArray.length;
    nativeArrDst = NativeArray$1$$op_Implicit(arr);
    if (nativeArrDst.length < index + nativeArray.length)
      throw new Error("can't copy, dest array too small.");
    for (i = 0; i < length; i++)
      nativeArrDst[i + index] = nativeArray[i];
  };
  ptyp_.copyTo0 = function ArrayG$1$$CopyTo(array, index) {
    var arr;
    arr = Type$$CastType(T$5b$5d_$T$_, array);
    this.copyTo(arr, index);
  };
  ptyp_.getEnumerator = function ArrayG$1$$GetEnumerator() {
    return Enumerator_$T$_.__ctor(this);
  };
  ptyp_["V_Insert_" + IList$1_$T$_.typeId] = ptyp_.system$$Collections$$Generic$$IList_$T$_$$Insert;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.system$$Collections$$Generic$$IEnumerable_$T$_$$GetEnumerator;
  ptyp_.V_get_Length = ptyp_.get_length;
  ptyp_.V_Contains = ptyp_.contains;
  ptyp_.V_GetEnumerator = ptyp_.getEnumerator;
  ptyp_.V_IndexOf = ptyp_.indexOf;
  ptyp_.V_GetValue = ptyp_.getValue;
  ptyp_.V_CopyTo = ptyp_.copyTo0;
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  Type$$RegisterReferenceType(ArrayG$1_$T$_, "System.ArrayG`1<" + T.fullName + ">", ArrayImpl, [IList$1_$T$_, IList, ICollection, IEnumerable, IEnumerable$1_$T$_]);
  ArrayG$1_$T$_._tri = function() {
    if (__initTracker0)
      return;
    __initTracker0 = true;
    T = T;
    Enumerator_$T$_ = Enumerator(T, true);
    ArrayG$1_$T$_ = ArrayG(T, true);
    T$5b$5d_$T$_ = ArrayG(T, true);
  };
  if (_callStatiConstructor)
    ArrayG$1_$T$_._tri();
  return ArrayG$1_$T$_;
};
function NativeArray$1$$IndexOf(this_, value, startIndex) {
  var i;
  startIndex = startIndex < 0 ? 0 : startIndex;
  for (i = this_.length; i >= startIndex && i >= 0; --i)
    if (this_[i] === value)
      return i;
  return -1;
};
function NativeArray$1$$InsertAt(this_, index, value) {
  var i;
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  for (i = this_.length - 1; i >= index; i--)
    this_[i + 1] = this_[i];
  this_[index] = value;
};
function NativeArray$1$$RemoveAt(this_, index) {
  var len, i;
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  len = this_.length - 1;
  for (i = index; i < len; i++)
    this_[i] = this_[i + 1];
  this_.pop();
};
function NativeArray$1$$op_Implicit(n) {
  return n.get_innerArray();
};
function Func(T1, TRes, _callStatiConstructor) {
  var Func$2_$T1_x_TRes$_, __initTracker;
  if (Func[T1.typeId] && Func[T1.typeId][TRes.typeId])
    return Func[T1.typeId][TRes.typeId];
    Func[T1.typeId] = {
    };
  Func[T1.typeId][TRes.typeId] = function System$$Func$20() {
  };
  Func$2_$T1_x_TRes$_ = Func[T1.typeId][TRes.typeId];
  Func$2_$T1_x_TRes$_.genericParameters = [T1, TRes];
  Func$2_$T1_x_TRes$_.genericClosure = Func;
  Func$2_$T1_x_TRes$_.typeId = "gc$" + T1.typeId + "_" + TRes.typeId + "$";
  Func$2_$T1_x_TRes$_.prototype = new Function0();
  Type$$RegisterReferenceType(Func$2_$T1_x_TRes$_, "System.Func`2<" + T1.fullName + "," + TRes.fullName + ">", Function0, []);
  Func$2_$T1_x_TRes$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T1 = T1;
    TRes = TRes;
    Func$2_$T1_x_TRes$_ = Func(T1, TRes, true);
  };
  if (_callStatiConstructor)
    Func$2_$T1_x_TRes$_._tri();
  return Func$2_$T1_x_TRes$_;
};
function ContextBindableObject() {
};
ContextBindableObject.typeId = "jb";
function ContextBindableObject_factory() {
  var this_;
  this_ = new ContextBindableObject();
  this_.__ctor1();
  return this_;
};
ContextBindableObject.defaultConstructor = ContextBindableObject_factory;
ptyp_ = new ExtensibleObservableObject();
ContextBindableObject.prototype = ptyp_;
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
ptyp_.set_parent = function ContextBindableObject$$set_Parent(value) {
  if (this.parent != value) {
    if (this.parent) {
      this.parent.removePropertyChangedListener("DataContext", Delegate$$Create("onParentDataContextUpdated", this));
      this.parent.removePropertyChangedListener("IsActive", Delegate$$Create("onParentDataContextUpdated", this));
    }
    this.parent = value;
    if (!this.dataContextSetterCalled)
      if (this.parent) {
        this.parent.addPropertyChangedListener("DataContext", Delegate$$Create("onParentDataContextUpdated", this));
        this.parent.addPropertyChangedListener("IsActive", Delegate$$Create("onParentDataContextUpdated", this));
        this.onParentDataContextUpdated(null, null);
      }
      else
        this.setDataContext(null);
  }
};
ptyp_.get_dataContext = function ContextBindableObject$$get_DataContext() {
  return this.dataContext;
};
ptyp_.set_dataContext = function ContextBindableObject$$set_DataContext(value) {
  this.dataContextSetterCalled = true;
  this.setDataContext(value);
};
ptyp_.get_isActive = function ContextBindableObject$$get_IsActive() {
  return this.isActivated && !this.V_get_ActivationBlocked();
};
ptyp_.set_inactiveIfNullContext = function ContextBindableObject$$set_InactiveIfNullContext(value) {
  this.isInactiveIfNullContext = value;
  this.fixActivation();
};
ptyp_.get_activationBlocked = function ContextBindableObject$$get_ActivationBlocked() {
  return this.isInactiveIfNullContext && this.dataContext === null;
};
ptyp_.dispose = function ContextBindableObject$$Dispose() {
  if (!this.isDisposed && !this.isDisposing)
    this.V_InternalDispose();
};
ptyp_.activate = function ContextBindableObject$$Activate() {
  this.isActive = true;
  this.fixActivation();
};
ptyp_.deactivate = function ContextBindableObject$$Deactivate() {
  if (!this.isActive)
    return;
  this.isActive = false;
  this.fixActivation();
};
ptyp_.onBeforeFirstActivate = function ContextBindableObject$$OnBeforeFirstActivate() {
};
ptyp_.onActivate = function ContextBindableObject$$OnActivate() {
};
ptyp_.onDeactivate = function ContextBindableObject$$OnDeactivate() {
};
ptyp_.fixActivation = function ContextBindableObject$$FixActivation() {
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
ptyp_.internalDispose = function ContextBindableObject$$InternalDispose() {
  if (this.onDisposed) {
    this.set_parent(null);
    this.clearListeners();
    this.onDisposed();
  }
};
ptyp_.onDataContextUpdating = function ContextBindableObject$$OnDataContextUpdating(newValue) {
};
ptyp_.onDataContextUpdated = function ContextBindableObject$$OnDataContextUpdated(oldValue) {
};
ptyp_.setDataContext = function ContextBindableObject$$SetDataContext(value) {
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
ptyp_.onParentDataContextUpdated = function ContextBindableObject$$OnParentDataContextUpdated(sender, propertyName) {
  if (this.parent.get_isActive() && !this.dataContextSetterCalled)
    this.setDataContext(this.parent.get_dataContext());
  if (propertyName === "IsActive" || propertyName === null)
    if (this.parent.get_isActive())
      this.activate();
    else
      this.deactivate();
};
ptyp_.__ctor1 = function ContextBindableObject$$$$ctor() {
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
Type$$RegisterReferenceType(ContextBindableObject, "Sunlight.Framework.Binders.ContextBindableObject", ExtensibleObservableObject, []);
function INotifyPropertyChanged() {
};
INotifyPropertyChanged.typeId = "b";
Type$$RegisterInterface(INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function NumberDictionary(TValue, _callStatiConstructor) {
  var Enumerator_$TValue$_, NumberDictionary$1_$TValue$_, KeyValuePair$2_$Number_x_TValue$_, IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_, __initTracker, __initTracker0;
  if (NumberDictionary[TValue.typeId])
    return NumberDictionary[TValue.typeId];
  NumberDictionary[TValue.typeId] = function System$$Collections$$Generic$$NumberDictionary$10() {
  };
  NumberDictionary$1_$TValue$_ = NumberDictionary[TValue.typeId];
  NumberDictionary$1_$TValue$_.genericParameters = [TValue];
  NumberDictionary$1_$TValue$_.genericClosure = NumberDictionary;
  NumberDictionary$1_$TValue$_.typeId = "hc$" + TValue.typeId + "$";
  KeyValuePair$2_$Number_x_TValue$_ = KeyValuePair(Number, TValue, _callStatiConstructor);
  KeyValuePair$2_$Number_x_TValue$_ = KeyValuePair(Number, TValue, _callStatiConstructor);
  IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_ = IEnumerable0(KeyValuePair(Number, TValue, _callStatiConstructor), _callStatiConstructor);
  NumberDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_NumberDictionary$1_factory0() {
    var this_;
    this_ = new NumberDictionary$1_$TValue$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = NumberDictionary$1_$TValue$_.prototype;
  ptyp_.innerDict = null;
  ptyp_.count = 0;
  ptyp_.system$$Collections$$IEnumerable$$GetEnumerator = function NumberDictionary$1$$System$$Collections$$IEnumerable$$GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.__ctor = function NumberDictionary$1$$$$ctor() {
    this.innerDict = {
    };
  };
  ptyp_.get_item = function NumberDictionary$1$$get_Item(index) {
    if (!(index in this.innerDict))
      throw new Error("Key not found");
    return this.innerDict[index];
  };
  ptyp_.set_item = function NumberDictionary$1$$set_Item(index, value) {
    this.innerDict[index] = value;
  };
  ptyp_.get_keys = function NumberDictionary$1$$get_Keys() {
    return ArrayG_$Number$_.__ctor(this.getKeys());
  };
  ptyp_.get_count = function NumberDictionary$1$$get_Count() {
    return this.count;
  };
  ptyp_.add = function NumberDictionary$1$$Add(key, value) {
    if (this.containsKey(key))
      throw new Error("Key already exists");
    this.count++;
    this.set_item(key, value);
  };
  ptyp_.containsKey = function NumberDictionary$1$$ContainsKey(key) {
    return key in this.innerDict;
  };
  ptyp_.remove = function NumberDictionary$1$$Remove(key) {
    var rv;
    rv = delete this.innerDict[key];
    if (rv)
      this.count--;
    return rv;
  };
  ptyp_.copyTo = function NumberDictionary$1$$CopyTo(array, index) {
    var keys, i;
    keys = Type$$CastType(ArrayG_$Number$_, this.get_keys());
    for (i = 0; i < keys.V_get_Length(); i++)
      array.setValue(i + index, Type$$BoxTypeInstance(KeyValuePair$2_$Number_x_TValue$_, KeyValuePair$2_$Number_x_TValue$_.__ctor(keys.get_item(i), this.get_item(keys.get_item(i)))));
  };
  ptyp_.getEnumerator = function NumberDictionary$1$$GetEnumerator() {
    return Enumerator_$TValue$_.__ctor(this);
  };
  ptyp_.getKeys = function NumberDictionary$1$$GetKeys() {
    var rv, key;
    rv = [];
    for (key in this.innerDict)
      rv.push(key);
    return rv;
  };
  ptyp_.V_GetEnumerator_g = ptyp_.system$$Collections$$IEnumerable$$GetEnumerator;
  ptyp_.V_get_Count_c = ptyp_.get_count;
  ptyp_.V_CopyTo_c = ptyp_.copyTo;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_.typeId] = ptyp_.getEnumerator;
  Type$$RegisterReferenceType(NumberDictionary$1_$TValue$_, "System.Collections.Generic.NumberDictionary`1<" + TValue.fullName + ">", Object, [ICollection, IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_, IEnumerable]);
  NumberDictionary$1_$TValue$_._tri = function() {
    if (__initTracker0)
      return;
    __initTracker0 = true;
    Enumerator_$TValue$_ = Enumerator0(TValue, true);
    TValue = TValue;
    NumberDictionary$1_$TValue$_ = NumberDictionary(TValue, true);
  };
  if (_callStatiConstructor)
    NumberDictionary$1_$TValue$_._tri();
  return NumberDictionary$1_$TValue$_;
};
function Queue(T, _callStatiConstructor) {
  var QueueEnumerator_$T$_, Queue$1_$T$_, IEnumerable$1_$T$_, __initTracker, __initTracker0;
  if (Queue[T.typeId])
    return Queue[T.typeId];
  Queue[T.typeId] = function System$$Collections$$Generic$$Queue$10() {
  };
  Queue$1_$T$_ = Queue[T.typeId];
  Queue$1_$T$_.genericParameters = [T];
  Queue$1_$T$_.genericClosure = Queue;
  Queue$1_$T$_.typeId = "ic$" + T.typeId + "$";
  IEnumerable$1_$T$_ = IEnumerable0(T, _callStatiConstructor);
  Queue$1_$T$_.defaultConstructor = function System_Collections_Generic_Queue$1_factory0() {
    var this_;
    this_ = new Queue$1_$T$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = Queue$1_$T$_.prototype;
  ptyp_.nativeArray = null;
  ptyp_.system$$Collections$$IEnumerable$$GetEnumerator = function Queue$1$$System$$Collections$$IEnumerable$$GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.dequeue = function Queue$1$$Dequeue() {
    var rv;
    if (this.get_count() > 0) {
      rv = this.nativeArray[0];
      NativeArray$1$$RemoveAt(this.nativeArray, 0);
      return rv;
    }
    throw new Error("No elements in stack");
  };
  ptyp_.enqueue = function Queue$1$$Enqueue(item) {
    NativeArray$1$$InsertAt(this.nativeArray, this.nativeArray.length, item);
  };
  ptyp_.get_count = function Queue$1$$get_Count() {
    return this.nativeArray.length;
  };
  ptyp_.getEnumerator = function Queue$1$$GetEnumerator() {
    return QueueEnumerator_$T$_.__ctor(this);
  };
  ptyp_.__ctor = function Queue$1$$$$ctor() {
    this.nativeArray = new Array(0);
  };
  ptyp_.V_GetEnumerator_g = ptyp_.system$$Collections$$IEnumerable$$GetEnumerator;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.getEnumerator;
  Type$$RegisterReferenceType(Queue$1_$T$_, "System.Collections.Generic.Queue`1<" + T.fullName + ">", Object, [IEnumerable$1_$T$_, IEnumerable]);
  Queue$1_$T$_._tri = function() {
    if (__initTracker0)
      return;
    __initTracker0 = true;
    QueueEnumerator_$T$_ = QueueEnumerator(T, true);
    T = T;
    Queue$1_$T$_ = Queue(T, true);
  };
  if (_callStatiConstructor)
    Queue$1_$T$_._tri();
  return Queue$1_$T$_;
};
function List(T, _callStatiConstructor) {
  var ListEnumerator$1_$T$_, List$1_$T$_, IList$1_$T$_, IEnumerable$1_$T$_, __initTracker, __initTracker0;
  if (List[T.typeId])
    return List[T.typeId];
  List[T.typeId] = function System$$Collections$$Generic$$List$10() {
  };
  List$1_$T$_ = List[T.typeId];
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = List;
  List$1_$T$_.typeId = "jc$" + T.typeId + "$";
  IList$1_$T$_ = IList0(T, _callStatiConstructor);
  IEnumerable$1_$T$_ = IEnumerable0(T, _callStatiConstructor);
  List$1_$T$_.defaultConstructor = function System_Collections_Generic_List$1_factory0() {
    var this_;
    this_ = new List$1_$T$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = List$1_$T$_.prototype;
  ptyp_.nativeArray = null;
  ptyp_.system$$Collections$$IList$$IndexOf = function List$1$$System$$Collections$$IList$$IndexOf(value) {
    if (value === null && T.isInstanceOfType(value))
      return NativeArray$1$$IndexOf(this.nativeArray, Type$$UnBoxTypeInstance(T, value), 0);
    return -1;
  };
  ptyp_.system$$Collections$$ICollection$$CopyTo = function List$1$$System$$Collections$$ICollection$$CopyTo(array, index) {
    var nativeArray, length, i;
    nativeArray = this.nativeArray;
    length = nativeArray.length;
    for (i = 0; i < length; i++)
      array.setValue(i + index, Type$$BoxTypeInstance(T, nativeArray[i]));
  };
  ptyp_.system$$Collections$$IEnumerable$$GetEnumerator = function List$1$$System$$Collections$$IEnumerable$$GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.system$$Collections$$IList$$get_Item = function List$1$$System$$Collections$$IList$$get_Item(index) {
    return Type$$BoxTypeInstance(T, this.get_item(index));
  };
  ptyp_.system$$Collections$$IList$$Contains = function List$1$$System$$Collections$$IList$$Contains(value) {
    return Type$$CastType(IList, this).V_IndexOf_e(value) >= 0;
  };
  ptyp_.__ctor = function List$1$$$$ctor() {
    this.nativeArray = new Array(0);
  };
  ptyp_.get_item = function List$1$$get_Item(index) {
    var arr;
    arr = this.nativeArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index];
  };
  ptyp_.set_item = function List$1$$set_Item(index, value) {
    var arr;
    arr = this.nativeArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index] = value;
  };
  ptyp_.insert = function List$1$$Insert(index, item) {
    NativeArray$1$$InsertAt(this.nativeArray, index, item);
  };
  ptyp_.removeAt = function List$1$$RemoveAt(index) {
    NativeArray$1$$RemoveAt(this.nativeArray, index);
  };
  ptyp_.get_count = function List$1$$get_Count() {
    return this.nativeArray.length;
  };
  ptyp_.add = function List$1$$Add(item) {
    this.nativeArray.push(item);
  };
  ptyp_.clear = function List$1$$Clear() {
    this.nativeArray.length = 0;
  };
  ptyp_.getEnumerator = function List$1$$GetEnumerator() {
    return ListEnumerator$1_$T$_.__ctor(this);
  };
  ptyp_.V_IndexOf_e = ptyp_.system$$Collections$$IList$$IndexOf;
  ptyp_.V_CopyTo_c = ptyp_.system$$Collections$$ICollection$$CopyTo;
  ptyp_.V_GetEnumerator_g = ptyp_.system$$Collections$$IEnumerable$$GetEnumerator;
  ptyp_.V_get_Item_e = ptyp_.system$$Collections$$IList$$get_Item;
  ptyp_.V_Contains_e = ptyp_.system$$Collections$$IList$$Contains;
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  ptyp_["V_Insert_" + IList$1_$T$_.typeId] = ptyp_.insert;
  ptyp_.V_Clear_e = ptyp_.clear;
  ptyp_.V_RemoveAt_e = ptyp_.removeAt;
  ptyp_.V_get_Count_c = ptyp_.get_count;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.getEnumerator;
  Type$$RegisterReferenceType(List$1_$T$_, "System.Collections.Generic.List`1<" + T.fullName + ">", Object, [IList$1_$T$_, IList, ICollection, IEnumerable, IEnumerable$1_$T$_]);
  List$1_$T$_._tri = function() {
    if (__initTracker0)
      return;
    __initTracker0 = true;
    T = T;
    ListEnumerator$1_$T$_ = ListEnumerator(T, true);
    List$1_$T$_ = List(T, true);
  };
  if (_callStatiConstructor)
    List$1_$T$_._tri();
  return List$1_$T$_;
};
function StringDictionary(TValue, _callStatiConstructor) {
  var Enumerator_$TValue$_, StringDictionary$1_$TValue$_, KeyValuePair$2_$String_x_TValue$_, IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_, __initTracker, __initTracker0;
  if (StringDictionary[TValue.typeId])
    return StringDictionary[TValue.typeId];
  StringDictionary[TValue.typeId] = function System$$Collections$$Generic$$StringDictionary$10() {
  };
  StringDictionary$1_$TValue$_ = StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "kc$" + TValue.typeId + "$";
  KeyValuePair$2_$String_x_TValue$_ = KeyValuePair(String, TValue, _callStatiConstructor);
  KeyValuePair$2_$String_x_TValue$_ = KeyValuePair(String, TValue, _callStatiConstructor);
  IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_ = IEnumerable0(KeyValuePair(String, TValue, _callStatiConstructor), _callStatiConstructor);
  StringDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_StringDictionary$1_factory0() {
    var this_;
    this_ = new StringDictionary$1_$TValue$_();
    this_.__ctor();
    return this_;
  };
  StringDictionary$1_$TValue$_.__ctor = function System_Collections_Generic_StringDictionary$1_factory1(innerDict) {
    var this_;
    this_ = new StringDictionary$1_$TValue$_();
    this_.__ctor0(innerDict);
    return this_;
  };
  ptyp_ = StringDictionary$1_$TValue$_.prototype;
  ptyp_.innerDict = null;
  ptyp_.count = 0;
  ptyp_.system$$Collections$$IEnumerable$$GetEnumerator = function StringDictionary$1$$System$$Collections$$IEnumerable$$GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.__ctor = function StringDictionary$1$$$$ctor() {
    this.innerDict = {
    };
  };
  ptyp_.__ctor0 = function StringDictionary$1$$$$ctor0(innerDict) {
    this.innerDict = innerDict;
    this.count = this.computeCount();
  };
  ptyp_.get_item = function StringDictionary$1$$get_Item(index) {
    if (!(index in this.innerDict))
      throw new Error("Key not found");
    return this.innerDict[index];
  };
  ptyp_.set_item = function StringDictionary$1$$set_Item(index, value) {
    if (!(index in this.innerDict))
      this.count++;
    this.innerDict[index] = value;
  };
  ptyp_.get_keys = function StringDictionary$1$$get_Keys() {
    return ArrayG_$String$_.__ctor(this.getKeys());
  };
  ptyp_.get_count = function StringDictionary$1$$get_Count() {
    return this.count;
  };
  ptyp_.containsKey = function StringDictionary$1$$ContainsKey(key) {
    return key in this.innerDict;
  };
  ptyp_.remove = function StringDictionary$1$$Remove(key) {
    var rv;
    rv = delete this.innerDict[key];
    if (rv)
      this.count--;
    return rv;
  };
  ptyp_.tryGetValue = function StringDictionary$1$$TryGetValue(key, value) {
    if (this.containsKey(key)) {
      value.write(this.get_item(key));
      return true;
    }
    value.write(Type$$GetDefaultValueStatic(TValue));
    return false;
  };
  ptyp_.clear = function StringDictionary$1$$Clear() {
    this.innerDict = {
    };
    this.count = 0;
  };
  ptyp_.copyTo = function StringDictionary$1$$CopyTo(array, index) {
    var keys, i;
    keys = Type$$CastType(ArrayG_$String$_, this.get_keys());
    for (i = 0; i < keys.V_get_Length(); i++)
      array.setValue(i + index, Type$$BoxTypeInstance(KeyValuePair$2_$String_x_TValue$_, KeyValuePair$2_$String_x_TValue$_.__ctor(keys.get_item(i), this.get_item(keys.get_item(i)))));
  };
  ptyp_.getEnumerator = function StringDictionary$1$$GetEnumerator() {
    return Enumerator_$TValue$_.__ctor(this);
  };
  ptyp_.getKeys = function StringDictionary$1$$GetKeys() {
    var rv, key;
    rv = [];
    for (key in this.innerDict)
      rv.push(key);
    return rv;
  };
  ptyp_.computeCount = function StringDictionary$1$$ComputeCount() {
    var rv, key;
    rv = 0;
    for (key in this.innerDict)
      rv++;
    return rv;
  };
  ptyp_.V_GetEnumerator_g = ptyp_.system$$Collections$$IEnumerable$$GetEnumerator;
  ptyp_.V_get_Count_c = ptyp_.get_count;
  ptyp_.V_CopyTo_c = ptyp_.copyTo;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_.typeId] = ptyp_.getEnumerator;
  Type$$RegisterReferenceType(StringDictionary$1_$TValue$_, "System.Collections.Generic.StringDictionary`1<" + TValue.fullName + ">", Object, [ICollection, IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_, IEnumerable]);
  StringDictionary$1_$TValue$_._tri = function() {
    if (__initTracker0)
      return;
    __initTracker0 = true;
    TValue = TValue;
    Enumerator_$TValue$_ = Enumerator1(TValue, true);
    StringDictionary$1_$TValue$_ = StringDictionary(TValue, true);
  };
  if (_callStatiConstructor)
    StringDictionary$1_$TValue$_._tri();
  return StringDictionary$1_$TValue$_;
};
function Action(T1, _callStatiConstructor) {
  var Action$1_$T1$_, __initTracker;
  if (Action[T1.typeId])
    return Action[T1.typeId];
  Action[T1.typeId] = function System$$Action$10() {
  };
  Action$1_$T1$_ = Action[T1.typeId];
  Action$1_$T1$_.genericParameters = [T1];
  Action$1_$T1$_.genericClosure = Action;
  Action$1_$T1$_.typeId = "lc$" + T1.typeId + "$";
  Action$1_$T1$_.prototype = new Function0();
  Type$$RegisterReferenceType(Action$1_$T1$_, "System.Action`1<" + T1.fullName + ">", Function0, []);
  Action$1_$T1$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T1 = T1;
    Action$1_$T1$_ = Action(T1, true);
  };
  if (_callStatiConstructor)
    Action$1_$T1$_._tri();
  return Action$1_$T1$_;
};
function Action0(T1, T2, _callStatiConstructor) {
  var Action$2_$T1_x_T2$_, __initTracker;
  if (Action0[T1.typeId] && Action0[T1.typeId][T2.typeId])
    return Action0[T1.typeId][T2.typeId];
    Action0[T1.typeId] = {
    };
  Action0[T1.typeId][T2.typeId] = function System$$Action$20() {
  };
  Action$2_$T1_x_T2$_ = Action0[T1.typeId][T2.typeId];
  Action$2_$T1_x_T2$_.genericParameters = [T1, T2];
  Action$2_$T1_x_T2$_.genericClosure = Action0;
  Action$2_$T1_x_T2$_.typeId = "mc$" + T1.typeId + "_" + T2.typeId + "$";
  Action$2_$T1_x_T2$_.prototype = new Function0();
  Type$$RegisterReferenceType(Action$2_$T1_x_T2$_, "System.Action`2<" + T1.fullName + "," + T2.fullName + ">", Function0, []);
  Action$2_$T1_x_T2$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T1 = T1;
    T2 = T2;
    Action$2_$T1_x_T2$_ = Action0(T1, T2, true);
  };
  if (_callStatiConstructor)
    Action$2_$T1_x_T2$_._tri();
  return Action$2_$T1_x_T2$_;
};
function KeyValuePair(K, V, _callStatiConstructor) {
  var KeyValuePair$2_$K_x_V$_, __initTracker;
  if (KeyValuePair[K.typeId] && KeyValuePair[K.typeId][V.typeId])
    return KeyValuePair[K.typeId][V.typeId];
    KeyValuePair[K.typeId] = {
    };
  KeyValuePair[K.typeId][V.typeId] = function(boxedValue) {
    this.boxedValue = boxedValue;
  };
  KeyValuePair$2_$K_x_V$_ = KeyValuePair[K.typeId][V.typeId];
  KeyValuePair$2_$K_x_V$_.genericParameters = [K, V];
  KeyValuePair$2_$K_x_V$_.genericClosure = KeyValuePair;
  KeyValuePair$2_$K_x_V$_.typeId = "nc$" + K.typeId + "_" + V.typeId + "$";
  KeyValuePair$2_$K_x_V$_.getDefaultValue = function() {
    return {
      key: Type$$GetDefaultValueStatic(K),
      val: Type$$GetDefaultValueStatic(V)
    };
  };
  KeyValuePair$2_$K_x_V$_.__ctor = function KeyValuePair$2$$$$ctor(key, value) {
    var this_;
    this_ = KeyValuePair$2_$K_x_V$_.getDefaultValue();
    this_.key = key;
    this_.val = value;
    return this_;
  };
  KeyValuePair$2_$K_x_V$_.get_key = function KeyValuePair$2$$get_Key(this_) {
    return this_.key;
  };
  KeyValuePair$2_$K_x_V$_.prototype = new ValueType();
  Type$$RegisterStructType(KeyValuePair$2_$K_x_V$_, "System.Collections.Generic.KeyValuePair`2<" + K.fullName + "," + V.fullName + ">", []);
  KeyValuePair$2_$K_x_V$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    KeyValuePair$2_$K_x_V$_ = KeyValuePair(K, V, true);
    K = K;
    V = V;
  };
  if (_callStatiConstructor)
    KeyValuePair$2_$K_x_V$_._tri();
  return KeyValuePair$2_$K_x_V$_;
};
function Enumerator0(TValue, _callStatiConstructor) {
  var KeyValuePair$2_$Number_x_TValue$_, Enumerator_$TValue$_, IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_, __initTracker;
  if (Enumerator0[TValue.typeId])
    return Enumerator0[TValue.typeId];
  Enumerator0[TValue.typeId] = function System$$Collections$$Generic$$NumberDictionary$1$2fEnumerator0() {
  };
  Enumerator_$TValue$_ = Enumerator0[TValue.typeId];
  Enumerator_$TValue$_.genericParameters = [TValue];
  Enumerator_$TValue$_.genericClosure = Enumerator0;
  Enumerator_$TValue$_.typeId = "oc$" + TValue.typeId + "$";
  KeyValuePair$2_$Number_x_TValue$_ = KeyValuePair(Number, TValue, _callStatiConstructor);
  IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_ = IEnumerator0(KeyValuePair(Number, TValue, _callStatiConstructor), _callStatiConstructor);
  Enumerator_$TValue$_.__ctor = function System_Collections_Generic_NumberDictionary$1$2fEnumerator_factory0(dict) {
    var this_;
    this_ = new Enumerator_$TValue$_();
    this_.__ctor(dict);
    return this_;
  };
  ptyp_ = Enumerator_$TValue$_.prototype;
  ptyp_.dict = null;
  ptyp_.keys = null;
  ptyp_.system$$Collections$$IEnumerator$$get_Current = function Enumerator$$System$$Collections$$IEnumerator$$get_Current() {
    return Type$$BoxTypeInstance(KeyValuePair$2_$Number_x_TValue$_, this.get_current());
  };
  ptyp_.__ctor = function Enumerator$$$$ctor(dict) {
    this.dict = dict;
    this.keys = this.dict.get_keys().V_GetEnumerator_l$m$();
  };
  ptyp_.get_current = function Enumerator$$get_Current() {
    return KeyValuePair$2_$Number_x_TValue$_.__ctor(this.keys.V_get_Current_o$m$(), this.dict.get_item(this.keys.V_get_Current_o$m$()));
  };
  ptyp_.moveNext = function Enumerator$$MoveNext() {
    return this.keys.V_MoveNext_h();
  };
  ptyp_.V_get_Current_h = ptyp_.system$$Collections$$IEnumerator$$get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_h = ptyp_.moveNext;
  Type$$RegisterReferenceType(Enumerator_$TValue$_, "System.Collections.Generic.NumberDictionary`1/Enumerator<" + TValue.fullName + ">", Object, [IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_, IEnumerator]);
  Enumerator_$TValue$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    TValue = TValue;
    Enumerator_$TValue$_ = Enumerator0(TValue, true);
  };
  if (_callStatiConstructor)
    Enumerator_$TValue$_._tri();
  return Enumerator_$TValue$_;
};
function QueueEnumerator(T, _callStatiConstructor) {
  var QueueEnumerator_$T$_, IEnumerator$1_$T$_, __initTracker;
  if (QueueEnumerator[T.typeId])
    return QueueEnumerator[T.typeId];
  QueueEnumerator[T.typeId] = function System$$Collections$$Generic$$Queue$1$2fQueueEnumerator0() {
  };
  QueueEnumerator_$T$_ = QueueEnumerator[T.typeId];
  QueueEnumerator_$T$_.genericParameters = [T];
  QueueEnumerator_$T$_.genericClosure = QueueEnumerator;
  QueueEnumerator_$T$_.typeId = "pc$" + T.typeId + "$";
  IEnumerator$1_$T$_ = IEnumerator0(T, _callStatiConstructor);
  QueueEnumerator_$T$_.__ctor = function System_Collections_Generic_Queue$1$2fQueueEnumerator_factory0(queue) {
    var this_;
    this_ = new QueueEnumerator_$T$_();
    this_.__ctor(queue);
    return this_;
  };
  ptyp_ = QueueEnumerator_$T$_.prototype;
  ptyp_.queue = null;
  ptyp_.currentIndex = 0;
  ptyp_.system$$Collections$$IEnumerator$$get_Current = function QueueEnumerator$$System$$Collections$$IEnumerator$$get_Current() {
    return Type$$BoxTypeInstance(T, this.get_current());
  };
  ptyp_.__ctor = function QueueEnumerator$$$$ctor(queue) {
    this.queue = queue;
    this.currentIndex = -1;
  };
  ptyp_.get_current = function QueueEnumerator$$get_Current() {
    if (this.currentIndex < 0 || this.currentIndex >= this.queue.nativeArray.length)
      throw new Error("Out of range");
    return this.queue.nativeArray[this.currentIndex];
  };
  ptyp_.moveNext = function QueueEnumerator$$MoveNext() {
    this.currentIndex++;
    return this.currentIndex < this.queue.nativeArray.length;
  };
  ptyp_.V_get_Current_h = ptyp_.system$$Collections$$IEnumerator$$get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$T$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_h = ptyp_.moveNext;
  Type$$RegisterReferenceType(QueueEnumerator_$T$_, "System.Collections.Generic.Queue`1/QueueEnumerator<" + T.fullName + ">", Object, [IEnumerator$1_$T$_, IEnumerator]);
  QueueEnumerator_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    QueueEnumerator_$T$_ = QueueEnumerator(T, true);
  };
  if (_callStatiConstructor)
    QueueEnumerator_$T$_._tri();
  return QueueEnumerator_$T$_;
};
function ListEnumerator(T, _callStatiConstructor) {
  var IList$1_$T$_, ListEnumerator$1_$T$_, IEnumerator$1_$T$_, __initTracker, __initTracker0;
  if (ListEnumerator[T.typeId])
    return ListEnumerator[T.typeId];
  ListEnumerator[T.typeId] = function System$$Collections$$Generic$$ListEnumerator$10() {
  };
  ListEnumerator$1_$T$_ = ListEnumerator[T.typeId];
  ListEnumerator$1_$T$_.genericParameters = [T];
  ListEnumerator$1_$T$_.genericClosure = ListEnumerator;
  ListEnumerator$1_$T$_.typeId = "qc$" + T.typeId + "$";
  IEnumerator$1_$T$_ = IEnumerator0(T, _callStatiConstructor);
  ListEnumerator$1_$T$_.__ctor = function System_Collections_Generic_ListEnumerator$1_factory0(list) {
    var this_;
    this_ = new ListEnumerator$1_$T$_();
    this_.__ctor(list);
    return this_;
  };
  ptyp_ = ListEnumerator$1_$T$_.prototype;
  ptyp_.innerList = null;
  ptyp_.index = 0;
  ptyp_.system$$Collections$$IEnumerator$$get_Current = function ListEnumerator$1$$System$$Collections$$IEnumerator$$get_Current() {
    return Type$$BoxTypeInstance(T, this.get_current());
  };
  ptyp_.__ctor = function ListEnumerator$1$$$$ctor(list) {
    this.index = -1;
    this.innerList = list;
  };
  ptyp_.get_current = function ListEnumerator$1$$get_Current() {
    return this.innerList["V_get_Item_" + IList$1_$T$_.typeId](this.index);
  };
  ptyp_.moveNext = function ListEnumerator$1$$MoveNext() {
    return ++this.index < this.innerList.V_get_Count_c();
  };
  ptyp_.V_get_Current_h = ptyp_.system$$Collections$$IEnumerator$$get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$T$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_h = ptyp_.moveNext;
  Type$$RegisterReferenceType(ListEnumerator$1_$T$_, "System.Collections.Generic.ListEnumerator`1<" + T.fullName + ">", Object, [IEnumerator$1_$T$_, IEnumerator]);
  ListEnumerator$1_$T$_._tri = function() {
    if (__initTracker0)
      return;
    __initTracker0 = true;
    T = T;
    IList$1_$T$_ = IList0(T, true);
    ListEnumerator$1_$T$_ = ListEnumerator(T, true);
  };
  if (_callStatiConstructor)
    ListEnumerator$1_$T$_._tri();
  return ListEnumerator$1_$T$_;
};
function Enumerator1(TValue, _callStatiConstructor) {
  var KeyValuePair$2_$String_x_TValue$_, Enumerator_$TValue$_, IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_, __initTracker;
  if (Enumerator1[TValue.typeId])
    return Enumerator1[TValue.typeId];
  Enumerator1[TValue.typeId] = function System$$Collections$$Generic$$StringDictionary$1$2fEnumerator0() {
  };
  Enumerator_$TValue$_ = Enumerator1[TValue.typeId];
  Enumerator_$TValue$_.genericParameters = [TValue];
  Enumerator_$TValue$_.genericClosure = Enumerator1;
  Enumerator_$TValue$_.typeId = "rc$" + TValue.typeId + "$";
  KeyValuePair$2_$String_x_TValue$_ = KeyValuePair(String, TValue, _callStatiConstructor);
  IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_ = IEnumerator0(KeyValuePair(String, TValue, _callStatiConstructor), _callStatiConstructor);
  Enumerator_$TValue$_.__ctor = function System_Collections_Generic_StringDictionary$1$2fEnumerator_factory0(dict) {
    var this_;
    this_ = new Enumerator_$TValue$_();
    this_.__ctor(dict);
    return this_;
  };
  ptyp_ = Enumerator_$TValue$_.prototype;
  ptyp_.dict = null;
  ptyp_.keys = null;
  ptyp_.system$$Collections$$IEnumerator$$get_Current = function Enumerator$$System$$Collections$$IEnumerator$$get_Current0() {
    return Type$$BoxTypeInstance(KeyValuePair$2_$String_x_TValue$_, this.get_current());
  };
  ptyp_.__ctor = function Enumerator$$$$ctor0(dict) {
    this.dict = dict;
    this.keys = this.dict.get_keys().V_GetEnumerator_l$n$();
  };
  ptyp_.get_current = function Enumerator$$get_Current0() {
    return KeyValuePair$2_$String_x_TValue$_.__ctor(this.keys.V_get_Current_o$n$(), this.dict.get_item(this.keys.V_get_Current_o$n$()));
  };
  ptyp_.moveNext = function Enumerator$$MoveNext0() {
    return this.keys.V_MoveNext_h();
  };
  ptyp_.V_get_Current_h = ptyp_.system$$Collections$$IEnumerator$$get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_h = ptyp_.moveNext;
  Type$$RegisterReferenceType(Enumerator_$TValue$_, "System.Collections.Generic.StringDictionary`1/Enumerator<" + TValue.fullName + ">", Object, [IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_, IEnumerator]);
  Enumerator_$TValue$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    TValue = TValue;
    Enumerator_$TValue$_ = Enumerator1(TValue, true);
  };
  if (_callStatiConstructor)
    Enumerator_$TValue$_._tri();
  return Enumerator_$TValue$_;
};
function IEnumerable0(T, _callStatiConstructor) {
  var IEnumerable$1_$T$_, __initTracker;
  if (IEnumerable0[T.typeId])
    return IEnumerable0[T.typeId];
  IEnumerable0[T.typeId] = function System$$Collections$$Generic$$IEnumerable$10() {
  };
  IEnumerable$1_$T$_ = IEnumerable0[T.typeId];
  IEnumerable$1_$T$_.genericParameters = [T];
  IEnumerable$1_$T$_.genericClosure = IEnumerable0;
  IEnumerable$1_$T$_.typeId = "l$" + T.typeId + "$";
  Type$$RegisterInterface(IEnumerable$1_$T$_, "System.Collections.Generic.IEnumerable`1<" + T.fullName + ">");
  IEnumerable$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    IEnumerable$1_$T$_ = IEnumerable0(T, true);
  };
  if (_callStatiConstructor)
    IEnumerable$1_$T$_._tri();
  return IEnumerable$1_$T$_;
};
function Enumerator(T, _callStatiConstructor) {
  var Enumerator_$T$_, IEnumerator$1_$T$_, __initTracker;
  if (Enumerator[T.typeId])
    return Enumerator[T.typeId];
  Enumerator[T.typeId] = function System$$ArrayG$1$2fEnumerator0() {
  };
  Enumerator_$T$_ = Enumerator[T.typeId];
  Enumerator_$T$_.genericParameters = [T];
  Enumerator_$T$_.genericClosure = Enumerator;
  Enumerator_$T$_.typeId = "sc$" + T.typeId + "$";
  IEnumerator$1_$T$_ = IEnumerator0(T, _callStatiConstructor);
  Enumerator_$T$_.__ctor = function System_ArrayG$1$2fEnumerator_factory0(array) {
    var this_;
    this_ = new Enumerator_$T$_();
    this_.__ctor(array);
    return this_;
  };
  ptyp_ = Enumerator_$T$_.prototype;
  ptyp_.currentIndex = 0;
  ptyp_.array = null;
  ptyp_.system$$Collections$$IEnumerator$$get_Current = function Enumerator$$System$$Collections$$IEnumerator$$get_Current1() {
    return Type$$BoxTypeInstance(T, this.get_current());
  };
  ptyp_.__ctor = function Enumerator$$$$ctor1(array) {
    this.currentIndex = -1;
    this.array = array;
  };
  ptyp_.moveNext = function Enumerator$$MoveNext1() {
    return ++this.currentIndex < this.array.V_get_Length();
  };
  ptyp_.get_current = function Enumerator$$get_Current1() {
    return this.array.get_item(this.currentIndex);
  };
  ptyp_.V_get_Current_h = ptyp_.system$$Collections$$IEnumerator$$get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$T$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_h = ptyp_.moveNext;
  Type$$RegisterReferenceType(Enumerator_$T$_, "System.ArrayG`1/Enumerator<" + T.fullName + ">", Object, [IEnumerator$1_$T$_, IEnumerator]);
  Enumerator_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    Enumerator_$T$_ = Enumerator(T, true);
  };
  if (_callStatiConstructor)
    Enumerator_$T$_._tri();
  return Enumerator_$T$_;
};
function IEnumerator0(T, _callStatiConstructor) {
  var IEnumerator$1_$T$_, __initTracker;
  if (IEnumerator0[T.typeId])
    return IEnumerator0[T.typeId];
  IEnumerator0[T.typeId] = function System$$Collections$$Generic$$IEnumerator$10() {
  };
  IEnumerator$1_$T$_ = IEnumerator0[T.typeId];
  IEnumerator$1_$T$_.genericParameters = [T];
  IEnumerator$1_$T$_.genericClosure = IEnumerator0;
  IEnumerator$1_$T$_.typeId = "o$" + T.typeId + "$";
  Type$$RegisterInterface(IEnumerator$1_$T$_, "System.Collections.Generic.IEnumerator`1<" + T.fullName + ">");
  IEnumerator$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    IEnumerator$1_$T$_ = IEnumerator0(T, true);
  };
  if (_callStatiConstructor)
    IEnumerator$1_$T$_._tri();
  return IEnumerator$1_$T$_;
};
function IList0(T, _callStatiConstructor) {
  var IList$1_$T$_, IEnumerable$1_$T$_, __initTracker;
  if (IList0[T.typeId])
    return IList0[T.typeId];
  IList0[T.typeId] = function System$$Collections$$Generic$$IList$10() {
  };
  IList$1_$T$_ = IList0[T.typeId];
  IList$1_$T$_.genericParameters = [T];
  IList$1_$T$_.genericClosure = IList0;
  IList$1_$T$_.typeId = "tc$" + T.typeId + "$";
  IEnumerable$1_$T$_ = IEnumerable0(T, _callStatiConstructor);
  Type$$RegisterInterface(IList$1_$T$_, "System.Collections.Generic.IList`1<" + T.fullName + ">");
  IList$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    IList$1_$T$_ = IList0(T, true);
  };
  if (_callStatiConstructor)
    IList$1_$T$_._tri();
  return IList$1_$T$_;
};
ValueIfTrue_$String$_ = ValueIfTrue(String);
Func_$Object_x_Object$_ = Func(Object, Object);
ArrayG_$Func_$Object_x_Object$_$_ = ArrayG(Func_$Object_x_Object$_);
ArrayG_$String$_ = ArrayG(String);
ArrayG_$SkinBinderInfo$_ = ArrayG(SkinBinderInfo);
ArrayG_$Object$_ = ArrayG(Object);
ArrayG_$TestViewModelB$_ = ArrayG(TestViewModelB);
KeyValuePair_$Number_x_Task$_ = KeyValuePair(Number, Task);
NumberDictionary_$Task$_ = NumberDictionary(Task);
Queue_$Task$_ = Queue(Task);
List_$ListViewItem$_ = List(ListViewItem);
Action_$UIEvent$_ = Action(UIEvent);
KeyValuePair_$String_x_Action_$UIEvent$_$_ = KeyValuePair(String, Action_$UIEvent$_);
StringDictionary_$Action_$UIEvent$_$_ = StringDictionary(Action_$UIEvent$_);
Action_$INotifyPropertyChanged_x_String$_ = Action0(INotifyPropertyChanged, String);
KeyValuePair_$String_x_Action_$INotifyPropertyChanged_x_String$_$_ = KeyValuePair(String, Action_$INotifyPropertyChanged_x_String$_);
StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_ = StringDictionary(Action_$INotifyPropertyChanged_x_String$_);
KeyValuePair_$String_x_Function$_ = KeyValuePair(String, Function);
StringDictionary_$Function$_ = StringDictionary(Function);
List_$Object$_ = List(Object);
ArrayG_$Number$_ = ArrayG(Number);
KeyValuePair_$String_x_Int32$_ = KeyValuePair(String, Int32);
StringDictionary_$Int32$_ = StringDictionary(Int32);
ManualTemplateTests$$$$cctor();
DateTime$$$$cctor();
ValueIfTrue_$String$_._tri();
Func_$Object_x_Object$_._tri();
ArrayG_$Func_$Object_x_Object$_$_._tri();
ArrayG_$String$_._tri();
ArrayG_$SkinBinderInfo$_._tri();
ArrayG_$Object$_._tri();
ArrayG_$TestViewModelB$_._tri();
KeyValuePair_$Number_x_Task$_._tri();
NumberDictionary_$Task$_._tri();
Queue_$Task$_._tri();
List_$ListViewItem$_._tri();
Action_$UIEvent$_._tri();
KeyValuePair_$String_x_Action_$UIEvent$_$_._tri();
StringDictionary_$Action_$UIEvent$_$_._tri();
Action_$INotifyPropertyChanged_x_String$_._tri();
KeyValuePair_$String_x_Action_$INotifyPropertyChanged_x_String$_$_._tri();
StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_._tri();
KeyValuePair_$String_x_Function$_._tri();
StringDictionary_$Function$_._tri();
List_$Object$_._tri();
ArrayG_$Number$_._tri();
KeyValuePair_$String_x_Int32$_._tri();
StringDictionary_$Int32$_._tri();
QUnit.module("Sunlight.Framework.UI.Test.LiveBinderTests", {
  "before": LiveBinderTests$$Setup
});
QUnit.test("TestLiveBinderOnActivate", LiveBinderTests$$TestLiveBinderOnActivate);
QUnit.test("TestLiveBinderOnChange", LiveBinderTests$$TestLiveBinderOnChange);
QUnit.test("TestLiveBinderMultiOnActivate", LiveBinderTests$$TestLiveBinderMultiOnActivate);
QUnit.test("TestLiveBinderMultiOnChange", LiveBinderTests$$TestLiveBinderMultiOnChange);
QUnit.test("TestTwoWayLiveBinderOnChange", LiveBinderTests$$TestTwoWayLiveBinderOnChange);
QUnit.test("TestTwoWayLiveBinderMultiOnChangeWithConverters", LiveBinderTests$$TestTwoWayLiveBinderMultiOnChangeWithConverters);
QUnit.module("Sunlight.Framework.UI.Test.NScriptsTemplateTests", {
  "before": NScriptsTemplateTests$$Setup
});
QUnit.test("Test", NScriptsTemplateTests$$Test);
QUnit.test("TestApplySkin", NScriptsTemplateTests$$TestApplySkin);
QUnit.test("TestCssBinder", NScriptsTemplateTests$$TestCssBinder);
QUnit.test("TestStyleBinder", NScriptsTemplateTests$$TestStyleBinder);
QUnit.test("TestAttrBinder", NScriptsTemplateTests$$TestAttrBinder);
QUnit.test("TestPropertyBinder", NScriptsTemplateTests$$TestPropertyBinder);
QUnit.module("Sunlight.Framework.UI.Test.SkinBinderHelperTests", {
});
QUnit.test("TestSimpleBinder", SkinBinderHelperTests$$TestSimpleBinder);
QUnit.test("TestAttrBinding", SkinBinderHelperTests$$TestAttrBinding);
QUnit.test("TestStyleBinding", SkinBinderHelperTests$$TestStyleBinding);
QUnit.test("TestTextContentBinding", SkinBinderHelperTests$$TestTextContentBinding);
QUnit.module("Sunlight.Framework.UI.Test.TestListView", {
  "before": TestListView$$Setup
});
QUnit.test("TestChildSkin", TestListView$$TestChildSkin);
QUnit.module("Sunlight.Framework.UI.Test.ManualTemplateTests", {
  "before": ManualTemplateTests$$Setup
});
QUnit.test("Test", ManualTemplateTests$$Test);
QUnit.module("Sunlight.Framework.UI.Test.UIElementTests", {
});
QUnit.test("TestNewUIElement", UIElementTests$$TestNewUIElement);
function Test$5cTemplates$5cTestTemplate1_factory(skinFactory, doc) {
  var domStore, htmlRoot, objStorage;
  if (!(domStore = DocStorageGetter(doc))[0]) {
    domStore[0] = doc.createElement("div");
    domStore[0].innerHTML = " <div test=\"test me\"></div> ";
    tmplStore[0] = tmplStore[0] ? tmplStore[0] : [];
  }
  htmlRoot = domStore[0].cloneNode(true);
  objStorage = [];
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[0], null, 0, 0);
};
Test$5cTemplates$5cTestTemplate1_var = null;
function Test$5cTemplates$5cTestTemplate1() {
  if (!Test$5cTemplates$5cTestTemplate1_var)
    Test$5cTemplates$5cTestTemplate1_var = Skin_factory(UISkinableElement, TestViewModelA, Test$5cTemplates$5cTestTemplate1_factory, "0");
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
    tmplStore[1] = tmplStore[1] ? tmplStore[1] : [SkinBinderInfo_factory1([getter], SkinBinderHelper$$SetTextContent, 17, 0, null, ""), SkinBinderInfo_factory3([getter0], ["PropBool1"], SkinBinderHelper$$SetCssClass, "black", 81, 0, 0, null, false, 0)];
  }
  htmlRoot = domStore[1].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = SkinBinderHelper$$GetElementFromPath(htmlRoot, [1]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[1], null, 1, 0);
};
Test$5cTemplates$5cTestTemplateVMB_CssBinding_var = null;
function Test$5cTemplates$5cTestTemplateVMB_CssBinding() {
  if (!Test$5cTemplates$5cTestTemplateVMB_CssBinding_var)
    Test$5cTemplates$5cTestTemplateVMB_CssBinding_var = Skin_factory(UISkinableElement, TestViewModelB, Test$5cTemplates$5cTestTemplateVMB_CssBinding_factory, "1");
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
    tmplStore[2] = tmplStore[2] ? tmplStore[2] : [SkinBinderInfo_factory([getter], ["PropStr1"], styleSetter, 97, 0, 0, null, null)];
  }
  htmlRoot = domStore[2].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = SkinBinderHelper$$GetElementFromPath(htmlRoot, [1]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[2], null, 1, 0);
};
Test$5cTemplates$5cTestTemplateVMB_StyleBinding_var = null;
function Test$5cTemplates$5cTestTemplateVMB_StyleBinding() {
  if (!Test$5cTemplates$5cTestTemplateVMB_StyleBinding_var)
    Test$5cTemplates$5cTestTemplateVMB_StyleBinding_var = Skin_factory(UISkinableElement, TestViewModelB, Test$5cTemplates$5cTestTemplateVMB_StyleBinding_factory, "2");
  return Test$5cTemplates$5cTestTemplateVMB_StyleBinding_var;
};
function Test$5cTemplates$5cTestTemplateVMB_AttrBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[3]) {
    domStore[3] = doc.createElement("div");
    domStore[3].innerHTML = " <div test=\"foo\">Test Me</div> ";
    tmplStore[3] = tmplStore[3] ? tmplStore[3] : [SkinBinderInfo_factory3([getter], ["PropStr1"], SkinBinderHelper$$SetAttribute, "test1", 113, 0, 0, null, null, 0)];
  }
  htmlRoot = domStore[3].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = SkinBinderHelper$$GetElementFromPath(htmlRoot, [1]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[3], null, 1, 0);
};
Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var = null;
function Test$5cTemplates$5cTestTemplateVMB_AttrBinding() {
  if (!Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var)
    Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var = Skin_factory(UISkinableElement, TestViewModelB, Test$5cTemplates$5cTestTemplateVMB_AttrBinding_factory, "3");
  return Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var;
};
function getter1(src) {
  return src.get_propInt1();
};
function setter(tar, val) {
  tar.set_propInt1(val);
};
function setter0(tar, val) {
  tar.set_twoWayLooseBinding(val);
};
function getter2(src) {
  return src.get_twoWayLooseBinding();
};
function setter1(tar, val) {
  tar.set_oneWayStrictBinding(val);
};
function Test$5cTemplates$5cTestTemplateB_PropertyBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[4]) {
    domStore[4] = doc.createElement("div");
    domStore[4].innerHTML = " <div> This is a test. </div> ";
    tmplStore[4] = tmplStore[4] ? tmplStore[4] : [SkinBinderInfo_factory0([getter1], setter, ["PropInt1"], setter0, getter2, "TwoWayLooseBinding", 17, 0, 0, null, null, 11), SkinBinderInfo_factory([getter], ["PropStr1"], setter1, 17, 0, 1, null, "test")];
  }
  htmlRoot = domStore[4].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = TestUIElement_factory(SkinBinderHelper$$GetElementFromPath(htmlRoot, [1]));
  return SkinInstance_factory(skinFactory, htmlRoot, [0], objStorage, tmplStore[4], {
    "Part1": 0
  }, 2, 0);
};
Test$5cTemplates$5cTestTemplateB_PropertyBinding_var = null;
function Test$5cTemplates$5cTestTemplateB_PropertyBinding() {
  if (!Test$5cTemplates$5cTestTemplateB_PropertyBinding_var)
    Test$5cTemplates$5cTestTemplateB_PropertyBinding_var = Skin_factory(TestSkinableWithTestUIElementPart, TestViewModelB, Test$5cTemplates$5cTestTemplateB_PropertyBinding_factory, "4");
  return Test$5cTemplates$5cTestTemplateB_PropertyBinding_var;
};
function Test$5cTemplates$5cTestTemplateVMB1_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[5]) {
    domStore[5] = doc.createElement("div");
    domStore[5].innerHTML = " <div test=\"test me\"></div> ";
    tmplStore[5] = tmplStore[5] ? tmplStore[5] : [SkinBinderInfo_factory1([getter], SkinBinderHelper$$SetTextContent, 17, 0, null, "")];
  }
  htmlRoot = domStore[5].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = SkinBinderHelper$$GetElementFromPath(htmlRoot, [1]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[5], null, 0, 0);
};
Test$5cTemplates$5cTestTemplateVMB1_var = null;
function Test$5cTemplates$5cTestTemplateVMB1() {
  if (!Test$5cTemplates$5cTestTemplateVMB1_var)
    Test$5cTemplates$5cTestTemplateVMB1_var = Skin_factory(UISkinableElement, TestViewModelB, Test$5cTemplates$5cTestTemplateVMB1_factory, "5");
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
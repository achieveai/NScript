(function(){
var ManualTemplateTests__noneValue, ValueIfTrue_$String$_, LiveBinderTests__oneWayBinder, ArrayG_$Func_$Object_x_Object$_$_, ArrayG_$String$_, LiveBinderTests__twoWayBinder, LiveBinderTests__oneWayMultiBinder, LiveBinderTests__twoWayMultiBinder, ArrayG_$SkinBinderInfo$_, ArrayG_$Object$_, ArrayG_$TestViewModelB$_, TestListView___headerTransformer, HeaderInjectableTransformer_$TestViewModelB_x_TestViewModelC$_, ObservableCollection_$TestViewModelB$_, TestListView___elemSeletor, Type__typeMapping, NumberDictionary_$Task$_, Queue_$Task$_, TaskScheduler__instance, List_$ListViewItem$_, List_$Int32$_, StringDictionary_$Action_$UIEvent$_$_, ObservableCollectionGenerator_$Object_x_Object$_, List_$Object$_, String__trimStartHelperRegex, String__trimEndHelperRegex, StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_, StringDictionary_$Function$_, KeyValuePair_$String_x_Action_$UIEvent$_$_, ArrayG_$Number$_, ptyp_, tmplStore, Test$5cTemplates$5cTestTemplate1_var, Test$5cTemplates$5cTestTemplateVMB_CssBinding_var, Test$5cTemplates$5cTestTemplateVMB_StyleBinding_var, Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var, Test$5cTemplates$5cTestTemplateB_PropertyBinding_var, Test$5cTemplates$5cTestTemplateVMB1_var, Test$5cTemplates$5cTestTemplateVMB1_int_var, Test$5cTemplates$5cTestTemplateVMC1_var, Func_$Object_x_Object$_, KeyValuePair_$Number_x_Task$_, Action_$UIEvent$_, Action_$INotifyPropertyChanged_x_String$_, KeyValuePair_$String_x_Action_$INotifyPropertyChanged_x_String$_$_, KeyValuePair_$String_x_Function$_, StringDictionary_$Int32$_, KeyValuePair_$String_x_Int32$_;
Function.typeId = "q";
Type__typeMapping = null;
function Type__CastType(this_, instance) {
  if (this_.isInstanceOfType(instance) || instance === null || typeof instance === "undefined") {
    if (this_.isStruct)
      return instance.boxedValue;
    return instance;
  }
  throw "InvalidCast to " + this_.fullName;
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
ptyp_ = Function.prototype;
ptyp_.isDelegate = false;
ptyp_.isClass = false;
ptyp_.isStruct = false;
ptyp_.isInterface = false;
ptyp_.isNullable = false;
ptyp_.baseType = null;
ptyp_.fullName = null;
ptyp_.typeId = null;
ptyp_.baseInterfaces = null;
ptyp_.boxedValue = null;
ptyp_.interfaces = null;
ptyp_.isInstanceOfType = function Type__IsInstanceOfType(instance) {
  if (instance === null || typeof instance === "undefined")
    return false;
  if (!this.isInterface)
    return instance instanceof this || instance !== null && instance.constructor == this;
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
ptyp_.toString = function() {
  return Type__ToString(this);
};
Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "r";
function Object__Equals(obj1, obj2) {
  return obj1 === obj2;
};
function Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
Type__RegisterReferenceType(Object, "System.Object", null, []);
function LiveBinderTests() {
};
LiveBinderTests.typeId = "s";
LiveBinderTests__oneWayBinder = null;
LiveBinderTests__twoWayBinder = null;
LiveBinderTests__oneWayMultiBinder = null;
LiveBinderTests__twoWayMultiBinder = null;
function LiveBinderTests__Setup() {
  LiveBinderTests__oneWayBinder = SkinBinderInfo_factory(NativeArray$1__op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([function LiveBinderTests__Setup_del(obj) {
    return Type__CastType(TestViewModelA, obj).get_propStr1();
  }])), NativeArray$1__op_Implicit(ArrayG_$String$_.__ctor(["PropStr1"])), function LiveBinderTests__Setup_del2(tar, val) {
    Type__CastType(TestViewModelA, tar).set_propStr1(Type__CastType(String, val));
    return;
  }, 16 | 1, 0, 0, null, null);
  LiveBinderTests__twoWayBinder = SkinBinderInfo_factorya(NativeArray$1__op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([function LiveBinderTests__Setup_del3(obj) {
    return Type__CastType(TestViewModelA, obj).get_propStr1();
  }])), function LiveBinderTests__Setup_del4(tar, val) {
    Type__CastType(TestViewModelA, tar).set_propStr1(Type__CastType(String, val));
    return;
  }, NativeArray$1__op_Implicit(ArrayG_$String$_.__ctor(["PropStr1"])), function LiveBinderTests__Setup_del5(tar, val) {
    Type__CastType(TestViewModelA, tar).set_propStr1(Type__CastType(String, val));
    return;
  }, function LiveBinderTests__Setup_del6(obj) {
    return Type__CastType(TestViewModelA, obj).get_propStr1();
  }, "PropStr1", 16 | 1, 0, 0, null, null, null);
  LiveBinderTests__oneWayMultiBinder = SkinBinderInfo_factory(NativeArray$1__op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([function LiveBinderTests__Setup_del7(obj) {
    return Type__CastType(TestViewModelA, obj).get_testVMA();
  }, function LiveBinderTests__Setup_del8(obj) {
    return Type__CastType(TestViewModelA, obj).get_propStr1();
  }])), NativeArray$1__op_Implicit(ArrayG_$String$_.__ctor(["TestVMA", "PropStr1"])), function LiveBinderTests__Setup_del9(tar, val) {
    Type__CastType(TestViewModelA, tar).set_propStr1(Type__CastType(String, val));
    return;
  }, 16 | 1, 0, 0, null, null);
  LiveBinderTests__twoWayMultiBinder = SkinBinderInfo_factorya(NativeArray$1__op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([function LiveBinderTests__Setup_del10(obj) {
    return Type__CastType(TestViewModelA, obj).get_testVMA();
  }, function LiveBinderTests__Setup_del11(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(TestViewModelA, obj).get_propInt1());
  }])), function LiveBinderTests__Setup_del12(tar, val) {
    Type__CastType(TestViewModelA, tar).set_propInt1(Type__UnBoxTypeInstance(Int32, val));
    return;
  }, NativeArray$1__op_Implicit(ArrayG_$String$_.__ctor(["TestVMA", "PropInt1"])), function LiveBinderTests__Setup_del13(tar, val) {
    Type__CastType(TestViewModelA, tar).set_propStr1(Type__CastType(String, val));
    return;
  }, function LiveBinderTests__Setup_del14(obj) {
    return Type__CastType(TestViewModelA, obj).get_propStr1();
  }, "PropStr1", 16 | 1, 0, 0, function LiveBinderTests__Setup_del15(obj) {
    return obj.toString();
  }, function LiveBinderTests__Setup_del16(obj) {
    return Type__BoxTypeInstance(Int32, Int32__Parse(Type__CastType(String, obj)));
  }, null);
};
function LiveBinderTests__TestLiveBinderOnActivate(assert) {
  var src, target, liveBinder;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_propStr1("test");
  liveBinder = LiveBinder_factory(LiveBinderTests__oneWayBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  assert.notEqual(src.get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  assert.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
};
function LiveBinderTests__TestLiveBinderOnChange(assert) {
  var src, target, liveBinder;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_propStr1("test");
  liveBinder = LiveBinder_factory(LiveBinderTests__oneWayBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  assert.notEqual(src.get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  assert.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  src.set_propStr1("testChanged");
  assert.equal(src.get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
};
function LiveBinderTests__TestLiveBinderMultiOnActivate(assert) {
  var src, target, liveBinder;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_testVMA(TestViewModelA_factory());
  src.get_testVMA().set_propStr1("test");
  liveBinder = LiveBinder_factory(LiveBinderTests__oneWayMultiBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  assert.notEqual(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  assert.equal(src.get_testVMA().get_propStr1(), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
};
function LiveBinderTests__TestLiveBinderMultiOnChange(assert) {
  var src, target, liveBinder, stmtTemp1;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_testVMA(TestViewModelA_factory());
  src.get_testVMA().set_propStr1("test");
  liveBinder = LiveBinder_factory(LiveBinderTests__oneWayMultiBinder, null);
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
function LiveBinderTests__TestTwoWayLiveBinderOnChange(assert) {
  var src, target, liveBinder;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_propStr1("test");
  liveBinder = LiveBinder_factory(LiveBinderTests__twoWayBinder, null);
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
function LiveBinderTests__TestTwoWayLiveBinderMultiOnChangeWithConverters(assert) {
  var src, target, liveBinder, stmtTemp1;
  src = TestViewModelA_factory();
  target = TestViewModelA_factory();
  src.set_testVMA(TestViewModelA_factory());
  src.get_testVMA().set_propInt1(1);
  liveBinder = LiveBinder_factory(LiveBinderTests__twoWayMultiBinder, null);
  liveBinder.set_source(src);
  liveBinder.set_target(target);
  assert.notEqual(Int32__ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is notActive, changes should not flow");
  liveBinder.set_isActive(true);
  assert.equal(Int32__ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active and changes should have flowed.");
  src.get_testVMA().set_propInt1(2);
  assert.equal(Int32__ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active and changes on lastElement should have flowed.");
  src.set_testVMA((stmtTemp1 = TestViewModelA_factory(), stmtTemp1.set_propInt1(3), stmtTemp1));
  assert.equal(Int32__ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active and changes on firstElement should have flowed.");
  target.set_propStr1("21");
  assert.equal(Int32__ToString(src.get_testVMA().get_propInt1()), target.get_propStr1(), "if liveBinder is active in twoWay changes on target should flow back.");
};
Type__RegisterReferenceType(LiveBinderTests, "Sunlight.Framework.UI.Test.LiveBinderTests", Object, []);
function NScriptsTemplateTests() {
};
NScriptsTemplateTests.typeId = "t";
function NScriptsTemplateTests__Setup() {
  TaskScheduler__set_Instance(TaskScheduler_factory(TestWindowTimer_factory(), 10, 10));
};
function NScriptsTemplateTests__Test(assert) {
  assert.notEqual(null, NScriptsTemplatesClass__get_TestTemplate1(), "Template should not be null");
  assert.ok(true, "true should be true");
};
function NScriptsTemplateTests__TestApplySkin(assert) {
  var element, control;
  element = window.document.createElement("div");
  control = UISkinableElement_factory(element);
  control.set_dataContext(TestViewModelA_factory());
  control.set_skin(NScriptsTemplatesClass__get_TestTemplate1());
  control.activate();
  assert.notEqual(null, element.querySelector("[test]"), "After applying skin, skin element should be loaded");
};
function NScriptsTemplateTests__TestCssBinder(assert) {
  var element, control, vm, elem;
  element = window.document.createElement("div");
  control = UISkinableElement_factory(element);
  vm = TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(NScriptsTemplatesClass__get_TestTemplateVMB_CssBinding());
  control.activate();
  elem = element.querySelector("[test]");
  assert.notEqual(null, elem, "After applying skin, skin element should be loaded");
  assert.equal("", elem.className, "Class should not be set if PropBool1 is not set.");
  vm.set_propBool1(true);
  assert.notEqual("", elem.className, "Class should be set if PropBool1 is set.");
};
function NScriptsTemplateTests__TestStyleBinder(assert) {
  var element, control, vm, elem;
  element = window.document.createElement("div");
  control = UISkinableElement_factory(element);
  vm = TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(NScriptsTemplatesClass__get_TestTemplateVMB_StyleBinding());
  control.activate();
  elem = element.querySelector("[test]");
  assert.notEqual(null, elem, "After applying skin, skin element should be loaded");
  assert.equal("", elem.style.height, "Style should not be set if PropStr1 is not set.");
  vm.set_propStr1("10px");
  assert.equal("10px", elem.style.height, "Style should be set if PropStr1 is set.");
};
function NScriptsTemplateTests__TestAttrBinder(assert) {
  var element, control, vm, elem;
  element = window.document.createElement("div");
  control = UISkinableElement_factory(element);
  vm = TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(NScriptsTemplatesClass__get_TestTemplateVMB_AttrBinding());
  control.activate();
  elem = element.querySelector("[test]");
  assert.notEqual(elem, null, "After applying skin, skin element should be loaded");
  assert.equal(elem.getAttribute("test1"), null, "Attribute 'test' should not be set if PropStr1 is not set.");
  vm.set_propStr1("TTTest");
  assert.equal("TTTest", elem.getAttribute("test1"), "Attribute 'test' should be set if PropStr1 is set.");
};
function NScriptsTemplateTests__TestPropertyBinder(assert) {
  var element, control, vm;
  element = window.document.createElement("div");
  control = TestSkinableWithTestUIElementPart_factory(element);
  vm = TestViewModelB_factory();
  control.set_dataContext(vm);
  control.set_skin(NScriptsTemplatesClass__get_TestTemplateB_PropertyBinding());
  control.activate();
  assert.ok(!!control.get_part(), "templatePart should not be null.");
  assert.equal(control.get_part().get_oneWayStrictBinding(), vm.get_propStr1(), "vmPropStr1 should be equal to OnewayStrictBinding.");
  vm.set_propStr1("T1");
  assert.equal(control.get_part().get_oneWayStrictBinding(), vm.get_propStr1(), "vmPropStr1 should be equal to OnewayStrictBinding.");
  assert.equal(control.get_part().get_twoWayLooseBinding(), vm.get_propInt1(), "TwoWayLooseBinding and bound property PropInt1 should be equal.");
  vm.set_propInt1(11);
  assert.equal(control.get_part().get_twoWayLooseBinding(), vm.get_propInt1(), "TwoWayLooseBinding and bound property PropInt1 should be equal.");
  control.get_part().set_twoWayLooseBinding(101);
  assert.equal(control.get_part().get_twoWayLooseBinding(), vm.get_propInt1(), "TwoWayLooseBinding and bound property PropInt1 should be equal.");
};
Type__RegisterReferenceType(NScriptsTemplateTests, "Sunlight.Framework.UI.Test.NScriptsTemplateTests", Object, []);
function ManualTemplateTests() {
};
ManualTemplateTests.typeId = "u";
ManualTemplateTests__noneValue = null;
function ManualTemplateTests__Setup() {
};
function ManualTemplateTests__Test(assert) {
  assert.ok(true, "true should be true");
};
function ManualTemplateTests____cctor() {
  ManualTemplateTests__noneValue = ValueIfTrue_$String$_.__ctor("none");
};
Type__RegisterReferenceType(ManualTemplateTests, "Sunlight.Framework.UI.Test.ManualTemplateTests", Object, []);
function SkinBinderHelperTests() {
};
SkinBinderHelperTests.typeId = "v";
function SkinBinderHelperTests__TestSimpleBinder(assert) {
  var src, stmtTemp1, tar1;
  src = (stmtTemp1 = TestViewModelA_factory(), stmtTemp1.set_propStr1("Test"), stmtTemp1.set_propInt1(1), stmtTemp1.set_propBool1(true), stmtTemp1);
  tar1 = TestViewModelA_factory();
  SkinBinderHelper__Bind(NativeArray$1__op_Implicit(ArrayG_$SkinBinderInfo$_.__ctor([SkinBinderInfo_factoryb(NativeArray$1__op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([TestViewModelA__PropStr1Getter])), TestViewModelA__PropStr1Setter, 1 | 16, 0, null, null)])), src, NativeArray$1__op_Implicit(ArrayG_$Object$_.__ctor([tar1])));
  assert.equal(src.get_propStr1(), tar1.get_propStr1(), "After BindDataContext values should be equal");
};
function SkinBinderHelperTests__TestAttrBinding(assert) {
  var src, stmtTemp1, target;
  src = (stmtTemp1 = TestViewModelA_factory(), stmtTemp1.set_propStr1("Test"), stmtTemp1);
  target = window.document.createElement("div");
  SkinBinderHelper__Bind(NativeArray$1__op_Implicit(ArrayG_$SkinBinderInfo$_.__ctor([SkinBinderInfo_factoryc(NativeArray$1__op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([TestViewModelA__PropStr1Getter])), function SkinBinderHelperTests__TestAttrBinding_del(o1, o2, o3) {
    SkinBinderHelper__SetAttribute(o1, Type__CastType(String, o2), Type__CastType(String, o3));
    return;
  }, "tmp", 1 | 112, 0, null, null, 0)])), src, NativeArray$1__op_Implicit(ArrayG_$Object$_.__ctor([target])));
  assert.equal(src.get_propStr1(), target.getAttribute("tmp"), "After BindDataContext values should be equal");
};
function SkinBinderHelperTests__TestStyleBinding(assert) {
  var src, stmtTemp1, target;
  src = (stmtTemp1 = TestViewModelA_factory(), stmtTemp1.set_propBool1(true), stmtTemp1);
  target = window.document.createElement("div");
  target.className = "t1";
  SkinBinderHelper__Bind(NativeArray$1__op_Implicit(ArrayG_$SkinBinderInfo$_.__ctor([SkinBinderInfo_factoryc(NativeArray$1__op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([TestViewModelA__PropBool1Getter])), function SkinBinderHelperTests__TestStyleBinding_del(o1, o2, o3) {
    SkinBinderHelper__SetCssClass(o1, Type__UnBoxTypeInstance(Boolean, o2), Type__CastType(String, o3));
    return;
  }, "test", 1 | 112, 0, null, null, 0)])), src, NativeArray$1__op_Implicit(ArrayG_$Object$_.__ctor([target])));
  assert.equal("t1 test", target.className, "After BindDataContext values should be equal");
  src.set_propBool1(false);
  SkinBinderHelper__Bind(NativeArray$1__op_Implicit(ArrayG_$SkinBinderInfo$_.__ctor([SkinBinderInfo_factoryc(NativeArray$1__op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([TestViewModelA__PropBool1Getter])), function SkinBinderHelperTests__TestStyleBinding_del2(o1, o2, o3) {
    SkinBinderHelper__SetCssClass(o1, Type__UnBoxTypeInstance(Boolean, o2), Type__CastType(String, o3));
    return;
  }, "test", 1 | 112, 0, null, null, 0)])), src, NativeArray$1__op_Implicit(ArrayG_$Object$_.__ctor([target])));
  assert.equal("t1", target.className, "After BindDataContext values should be equal");
};
function SkinBinderHelperTests__TestTextContentBinding(assert) {
  var src, stmtTemp1, target;
  src = (stmtTemp1 = TestViewModelA_factory(), stmtTemp1.set_propStr1("Test"), stmtTemp1);
  target = window.document.createElement("div");
  SkinBinderHelper__Bind(NativeArray$1__op_Implicit(ArrayG_$SkinBinderInfo$_.__ctor([SkinBinderInfo_factoryb(NativeArray$1__op_Implicit(ArrayG_$Func_$Object_x_Object$_$_.__ctor([TestViewModelA__PropStr1Getter])), function SkinBinderHelperTests__TestTextContentBinding_del(o1, o2) {
    SkinBinderHelper__SetTextContent(o1, Type__CastType(String, o2));
    return;
  }, 1 | 16, 0, null, null)])), src, NativeArray$1__op_Implicit(ArrayG_$Object$_.__ctor([target])));
  assert.equal(src.get_propStr1(), target.textContent, "After BindDataContext values should be equal");
};
Type__RegisterReferenceType(SkinBinderHelperTests, "Sunlight.Framework.UI.Test.SkinBinderHelperTests", Object, []);
function TestListView() {
};
TestListView.typeId = "w";
TestListView___headerTransformer = null;
TestListView___elemSeletor = null;
function TestListView__Setup() {
  TaskScheduler__set_Instance(TaskScheduler_factory(TestWindowTimer_factory(), 10, 10));
};
function TestListView__TestChildSkin(assert) {
  var document, listView, vmAs, stmtTemp1, stmtTemp1a, stmtTemp1b, aa, elems, i;
  document = window.document;
  listView = ListView_factory(document.createElement("div"));
  listView.set_itemSkin(NScriptsTemplatesClass__get_TestTemplateVMB1());
  vmAs = ArrayG_$TestViewModelB$_.__ctor([(stmtTemp1 = TestViewModelB_factory(), stmtTemp1.set_propStr1("StrTest"), stmtTemp1), (stmtTemp1a = TestViewModelB_factory(), stmtTemp1a.set_propStr1("TestStr"), stmtTemp1a), (stmtTemp1b = TestViewModelB_factory(), stmtTemp1b.set_propStr1("TestTT1"), stmtTemp1b)]);
  listView.set_fixedList(vmAs);
  aa = listView.get_observableList();
  listView.set_inactiveIfNullContext(false);
  listView.activate();
  elems = listView.get_element().querySelectorAll("[test]");
  assert.equal(3, elems.length, "number of children should be 3");
  for (i = 0; i < 3; i++)
    assert.equal(vmAs.get_item(i).get_propStr1(), elems[i].innerText, "Inner text for element should match property it's bound to.");
};
function TestListView__TestHeaderSkin(assert) {
  var document, listView, coll, stmtTemp1, stmtTemp1a, stmtTemp1b, stmtTemp1c, stmtTemp1d, stmtTemp1e, stmtTemp1f, stmtTemp1g, stmtTemp1h, stmtTemp1i, stmtTemp1j;
  document = window.document;
  listView = ListView_factory(document.createElement("div"));
  listView.set_itemSkin(NScriptsTemplatesClass__get_TestTemplateVMB1_int());
  listView.set_headerSkin(NScriptsTemplatesClass__get_TestTemplateVMC1());
  TestListView___headerTransformer = HeaderInjectableTransformer_$TestViewModelB_x_TestViewModelC$_.__ctor(TestListView__GenerateHeader, null);
  coll = ObservableCollection_$TestViewModelB$_.defaultConstructor();
  coll.add((stmtTemp1 = TestViewModelB_factory(), stmtTemp1.set_propInt2(4), stmtTemp1));
  coll.add((stmtTemp1a = TestViewModelB_factory(), stmtTemp1a.set_propInt2(6), stmtTemp1a));
  coll.add((stmtTemp1b = TestViewModelB_factory(), stmtTemp1b.set_propInt2(11), stmtTemp1b));
  TestListView___headerTransformer.set_inputCollection(coll);
  listView.set_observableList(TestListView___headerTransformer.get_transformedCollection());
  listView.set_inactiveIfNullContext(false);
  listView.activate();
  TestListView___elemSeletor = listView.get_element().querySelectorAll("[test]");
  TestListView__TestCase(assert);
  coll.clear();
  coll.add((stmtTemp1c = TestViewModelB_factory(), stmtTemp1c.set_propInt2(4), stmtTemp1c));
  coll.add((stmtTemp1d = TestViewModelB_factory(), stmtTemp1d.set_propInt2(5), stmtTemp1d));
  coll.add((stmtTemp1e = TestViewModelB_factory(), stmtTemp1e.set_propInt2(11), stmtTemp1e));
  TestListView___headerTransformer.set_inputCollection(coll);
  listView.set_observableList(TestListView___headerTransformer.get_transformedCollection());
  listView.set_inactiveIfNullContext(false);
  listView.activate();
  TestListView___elemSeletor = listView.get_element().querySelectorAll("[test]");
  TestListView__TestCase(assert);
  coll.clear();
  coll.add((stmtTemp1f = TestViewModelB_factory(), stmtTemp1f.set_propInt2(4), stmtTemp1f));
  coll.add((stmtTemp1g = TestViewModelB_factory(), stmtTemp1g.set_propInt2(5), stmtTemp1g));
  TestListView___headerTransformer.set_inputCollection(coll);
  listView.set_observableList(TestListView___headerTransformer.get_transformedCollection());
  listView.set_inactiveIfNullContext(false);
  listView.activate();
  TestListView___elemSeletor = listView.get_element().querySelectorAll("[test]");
  TestListView__TestCase(assert);
  coll.clear();
  coll.add((stmtTemp1h = TestViewModelB_factory(), stmtTemp1h.set_propInt2(4), stmtTemp1h));
  coll.add((stmtTemp1i = TestViewModelB_factory(), stmtTemp1i.set_propInt2(6), stmtTemp1i));
  coll.add((stmtTemp1j = TestViewModelB_factory(), stmtTemp1j.set_propInt2(11), stmtTemp1j));
  TestListView___headerTransformer.set_inputCollection(coll);
  listView.set_observableList(TestListView___headerTransformer.get_transformedCollection());
  listView.set_inactiveIfNullContext(false);
  listView.activate();
  TestListView___elemSeletor = listView.get_element().querySelectorAll("[test]");
  TestListView__TestCase(assert);
};
function TestListView__TestCase(assert) {
  var count, i, element;
  count = TestListView___headerTransformer.get_transformedCollection().get_count();
  assert.equal(count, TestListView___elemSeletor.length, "number of children should be " + Type__BoxTypeInstance(Int32, count));
  for (i = 0; i < count; i++) {
    element = TestListView___headerTransformer.get_transformedCollection().get_item(i);
    if (!!element.get_header())
      assert.equal(element.get_header().get_propStr3(), TestListView___elemSeletor[i].innerText, "Inner text for element should match property it's bound to.");
    else
      assert.equal(Int32__ToString(element.get_item().get_propInt2()), TestListView___elemSeletor[i].innerText, "Inner text for element should match property it's bound to.");
  }
};
function TestListView__GenerateHeader(first, second) {
  var secondHead, headerText, stmtTemp1, firstHead, stmtTemp1a;
  secondHead = second.get_propInt2() / 10 | 0;
  headerText = "Header : ";
  if (!first)
    return stmtTemp1 = TestViewModelC_factory(), stmtTemp1.set_propStr3(headerText + Type__BoxTypeInstance(Int32, secondHead)), stmtTemp1;
  firstHead = first.get_propInt2() / 10 | 0;
  if (firstHead == secondHead)
    return null;
  else
    return stmtTemp1a = TestViewModelC_factory(), stmtTemp1a.set_propStr3(headerText + Type__BoxTypeInstance(Int32, secondHead)), stmtTemp1a;
};
Type__RegisterReferenceType(TestListView, "Sunlight.Framework.UI.Test.TestListView", Object, []);
function UIElementTests() {
};
UIElementTests.typeId = "x";
function UIElementTests__TestNewUIElement(assert) {
  var doc, element;
  doc = window.document;
  element = UIElement_factory(doc.createElement("div"));
  assert.notEqual(element.get_element(), null, "element.Element != null");
  assert.equal(element.get_element().tagName, "DIV", "element.Element.TagName == 'DIV'");
};
Type__RegisterReferenceType(UIElementTests, "Sunlight.Framework.UI.Test.UIElementTests", Object, []);
String.typeId = "o";
String__trimStartHelperRegex = null;
String__trimEndHelperRegex = null;
function String__get_TrimEndHelperRegex() {
  if (Object__IsNullOrUndefined(String__trimEndHelperRegex))
    String__trimEndHelperRegex = new RegExp("^[\\s\\xA0]+");
  return String__trimEndHelperRegex;
};
function String__get_TrimStartHelperRegex() {
  if (Object__IsNullOrUndefined(String__trimStartHelperRegex))
    String__trimStartHelperRegex = new RegExp("^[\\s\\xA0]+");
  return String__trimStartHelperRegex;
};
function String__Trim(this_) {
  return String__TrimEnd(String__TrimStart(this_));
};
function String__TrimEnd(this_) {
  return this_.trimLeft ? this_.trimLeft() : this_.replace(String__get_TrimEndHelperRegex(), "");
};
function String__TrimStart(this_) {
  return this_.trimRight ? this_.trimRight() : this_.replace(String__get_TrimStartHelperRegex(), "");
};
function String__get_Item(this_, index) {
  return this_.charCodeAt(index);
};
String__TrimEnd = function String__TrimEnd(this_) {
  return this_.trimLeft ? this_.trimLeft() : this_.replace(String__get_TrimEndHelperRegex(), "");
};
String__TrimStart = function String__TrimStart(this_) {
  return this_.trimRight ? this_.trimRight() : this_.replace(String__get_TrimStartHelperRegex(), "");
};
Type__RegisterReferenceType(String, "System.String", Object, []);
function INotifyPropertyChanged() {
};
INotifyPropertyChanged.typeId = "b";
Type__RegisterInterface(INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function ObservableObject() {
};
ObservableObject.typeId = "y";
ptyp_ = ObservableObject.prototype;
ptyp_.eventHandlers = null;
ptyp_.linkedProperties = null;
ptyp_.anyPropertyListener = null;
ptyp_.addPropertyChangedListener = function ObservableObject__AddPropertyChangedListener(propertyName, callback) {
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
    cb = Delegate__Combine(cb, callback);
  this.eventHandlers.set_item(propertyName, cb);
};
ptyp_.removePropertyChangedListener = function ObservableObject__RemovePropertyChangedListener(propertyName, callback) {
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
    cb = Delegate__Remove(cb, callback);
    if (!!cb)
      this.eventHandlers.set_item(propertyName, cb);
    else
      this.eventHandlers.remove(propertyName);
  }
};
ptyp_.clearListeners = function ObservableObject__ClearListeners() {
  this.eventHandlers = null;
  this.anyPropertyListener = null;
};
ptyp_.firePropertyChanged = function ObservableObject__FirePropertyChanged(propertyName) {
  var cb, linkedProperties, iProp;
  if (!!this.eventHandlers) {
    if (this.eventHandlers.tryGetValue(propertyName, {
      read: function() {
        return cb;
      },
      write: function(arg0) {
        return cb = arg0;
      }
    }))
      cb(this, propertyName);
    if (!!this.linkedProperties)
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
  if (!!this.anyPropertyListener)
    this.anyPropertyListener(this, propertyName);
};
ptyp_.__ctor = function ObservableObject____ctor() {
  this.eventHandlers = null;
  this.linkedProperties = null;
};
ptyp_.V_AddPropertyChangedListener_b = ptyp_.addPropertyChangedListener;
ptyp_.V_RemovePropertyChangedListener_b = ptyp_.removePropertyChangedListener;
Type__RegisterReferenceType(ObservableObject, "Sunlight.Framework.Observables.ObservableObject", Object, [INotifyPropertyChanged]);
function TestViewModelA() {
};
TestViewModelA.typeId = "z";
function TestViewModelA__PropStr1Getter(obj) {
  return Type__CastType(TestViewModelA, obj).get_propStr1();
};
function TestViewModelA__PropStr1Setter(obj, val) {
  Type__CastType(TestViewModelA, obj).set_propStr1(Type__CastType(String, val));
};
function TestViewModelA__PropBool1Getter(obj) {
  return Type__BoxTypeInstance(Boolean, Type__CastType(TestViewModelA, obj).get_propBool1());
};
function TestViewModelA_factory() {
  var this_;
  this_ = new TestViewModelA();
  this_.__ctora();
  return this_;
};
TestViewModelA.defaultConstructor = TestViewModelA_factory;
ptyp_ = new ObservableObject();
TestViewModelA.prototype = ptyp_;
ptyp_.str1 = null;
ptyp_.bool1 = false;
ptyp_.testVMA = null;
ptyp_._$int1$_k__BackingField = 0;
ptyp_.get_propInt1 = function TestViewModelA__get_PropInt1() {
  return this.get_int1();
};
ptyp_.set_propInt1 = function TestViewModelA__set_PropInt1(value) {
  if (this.get_int1() != value) {
    this.set_int1(value);
    this.firePropertyChanged("PropInt1");
  }
};
ptyp_.get_propStr1 = function TestViewModelA__get_PropStr1() {
  return this.str1;
};
ptyp_.set_propStr1 = function TestViewModelA__set_PropStr1(value) {
  if (this.str1 !== value) {
    this.str1 = value;
    this.firePropertyChanged("PropStr1");
  }
};
ptyp_.get_propBool1 = function TestViewModelA__get_PropBool1() {
  return this.bool1;
};
ptyp_.set_propBool1 = function TestViewModelA__set_PropBool1(value) {
  if (this.bool1 != value) {
    this.bool1 = value;
    this.firePropertyChanged("PropBool1");
  }
};
ptyp_.get_testVMA = function TestViewModelA__get_TestVMA() {
  return this.testVMA;
};
ptyp_.set_testVMA = function TestViewModelA__set_TestVMA(value) {
  if (this.testVMA != value) {
    this.testVMA = value;
    this.firePropertyChanged("TestVMA");
  }
};
ptyp_.get_int1 = function TestViewModelA__get_int1() {
  return this._$int1$_k__BackingField;
};
ptyp_.set_int1 = function TestViewModelA__set_int1(value) {
  this._$int1$_k__BackingField = value;
};
ptyp_.__ctora = function TestViewModelA____ctor() {
  this.__ctor();
};
Type__RegisterReferenceType(TestViewModelA, "Sunlight.Framework.UI.Test.TestViewModelA", ObservableObject, []);
function SkinBinderInfo() {
};
SkinBinderInfo.typeId = "ab";
function SkinBinderInfo_factoryb(propertyGetterPath, targetPropertySetter, binderType, objectIndex, forwardConverter, defaultValue) {
  var this_;
  this_ = new SkinBinderInfo();
  this_.__ctor(propertyGetterPath, targetPropertySetter, binderType, objectIndex, forwardConverter, defaultValue);
  return this_;
};
function SkinBinderInfo_factoryc(propertyGetterPath, targetPropertySetterWithArg, targetPropertySetterArg, binderType, objectIndex, forwardConverter, defaultValue, extraObjectIndex) {
  var this_;
  this_ = new SkinBinderInfo();
  this_.__ctora(propertyGetterPath, targetPropertySetterWithArg, targetPropertySetterArg, binderType, objectIndex, forwardConverter, defaultValue, extraObjectIndex);
  return this_;
};
function SkinBinderInfo_factory(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue) {
  var this_;
  this_ = new SkinBinderInfo();
  this_.__ctorb(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue);
  return this_;
};
function SkinBinderInfo_factoryd(propertyGetterPath, propertyNames, targetPropertySetter, targetPropertySetterArg, binderType, objectIndex, binderIndex, forwardConverter, defaultValue, extraObjectIndex) {
  var this_;
  this_ = new SkinBinderInfo();
  this_.__ctord(propertyGetterPath, propertyNames, targetPropertySetter, targetPropertySetterArg, binderType, objectIndex, binderIndex, forwardConverter, defaultValue, extraObjectIndex);
  return this_;
};
function SkinBinderInfo_factorya(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue) {
  var this_;
  this_ = new SkinBinderInfo();
  this_.__ctorc(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue);
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
ptyp_.__ctor = function SkinBinderInfo____ctor(propertyGetterPath, targetPropertySetter, binderType, objectIndex, forwardConverter, defaultValue) {
  this.binderIndex = -1;
  this.extraObjectIndex = -1; {
    this.propertyGetterPath = propertyGetterPath;
    this.targetPropertySetter = targetPropertySetter;
    this.binderType = binderType;
    this.objectIndex = objectIndex;
    this.forwardConverter = forwardConverter;
    this.defaultValue = defaultValue;
    this.mode = 0;
  }
};
ptyp_.__ctora = function SkinBinderInfo____ctor(propertyGetterPath, targetPropertySetterWithArg, targetPropertySetterArg, binderType, objectIndex, forwardConverter, defaultValue, extraObjectIndex) {
  this.binderIndex = -1;
  this.extraObjectIndex = -1; {
    this.propertyGetterPath = propertyGetterPath;
    this.targetPropertySetterArg = targetPropertySetterArg;
    this.targetPropertySetterWithArg = targetPropertySetterWithArg;
    this.binderType = binderType;
    this.objectIndex = objectIndex;
    this.forwardConverter = forwardConverter;
    this.defaultValue = defaultValue;
    this.mode = 0;
    this.extraObjectIndex = extraObjectIndex;
  }
};
ptyp_.__ctorb = function SkinBinderInfo____ctor(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue) {
  this.binderIndex = -1;
  this.extraObjectIndex = -1; {
    this.propertyGetterPath = propertyGetterPath;
    this.propertyNames = propertyNames;
    this.targetPropertySetter = targetPropertySetter;
    this.binderType = binderType;
    this.objectIndex = objectIndex;
    this.forwardConverter = forwardConverter;
    this.defaultValue = defaultValue;
    this.mode = 1;
  }
};
ptyp_.__ctord = function SkinBinderInfo____ctor(propertyGetterPath, propertyNames, targetPropertySetter, targetPropertySetterArg, binderType, objectIndex, binderIndex, forwardConverter, defaultValue, extraObjectIndex) {
  this.binderIndex = -1;
  this.extraObjectIndex = -1; {
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
  }
};
ptyp_.__ctorc = function SkinBinderInfo____ctor(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue) {
  this.binderIndex = -1;
  this.extraObjectIndex = -1; {
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
  }
};
ptyp_.setTargetValue = function SkinBinderInfo__SetTargetValue(target, value, extraObjectArray) {
  var binderInfo, propertySetterMode, element;
  binderInfo = this;
  propertySetterMode = binderInfo.binderType & 240;
  if (propertySetterMode == 16)
    binderInfo.targetPropertySetter(target, value);
  else if (propertySetterMode == 48) {
    if (extraObjectArray[binderInfo.extraObjectIndex] === value)
      return;
    if (extraObjectArray[binderInfo.extraObjectIndex] != null)
      this.targetPropertySetterWithArg(target, extraObjectArray[binderInfo.extraObjectIndex], false);
    extraObjectArray[binderInfo.extraObjectIndex] = value;
    if (value != null)
      this.targetPropertySetterWithArg(target, value, true);
  }
  else if (propertySetterMode == 64) {
    element = target;
    if (extraObjectArray[binderInfo.extraObjectIndex] === value)
      return;
    if (!Object__IsNullOrUndefined(extraObjectArray[binderInfo.extraObjectIndex]))
      Element__UnBind(element, Type__CastType(String, binderInfo.targetPropertySetterArg), extraObjectArray[binderInfo.extraObjectIndex], false);
    extraObjectArray[binderInfo.extraObjectIndex] = value;
    if (!Object__IsNullOrUndefined(value))
      Element__Bind(element, Type__CastType(String, binderInfo.targetPropertySetterArg), value, false);
  }
  else if (!!binderInfo.targetPropertySetter)
    binderInfo.targetPropertySetter(target, value);
  else
    binderInfo.targetPropertySetterWithArg(target, value, binderInfo.targetPropertySetterArg);
};
Type__RegisterReferenceType(SkinBinderInfo, "Sunlight.Framework.UI.Helpers.SkinBinderInfo", Object, []);
function ValueType() {
};
ValueType.typeId = "bb";
ptyp_ = ValueType.prototype;
ptyp_.boxedValue = null;
Type__RegisterReferenceType(ValueType, "System.ValueType", Object, []);
function Int32(boxedValue) {
  this.boxedValue = boxedValue;
};
Int32.typeId = "cb";
Int32.getDefaultValue = function() {
  return 0;
};
function Int32__Parse(s) {
  return parseInt(s);
};
function Int32__ToStringa(this_, radix) {
  return this_.toString(radix);
};
function Int32__ToString(this_) {
  return Int32__ToStringa(this_, 10);
};
ptyp_ = new ValueType();
Int32.prototype = ptyp_;
ptyp_.toString = function() {
  return Int32__ToString(this.boxedValue);
};
Type__RegisterStructType(Int32, "System.Int32", []);
function IEnumerable() {
};
IEnumerable.typeId = "k";
Type__RegisterInterface(IEnumerable, "System.Collections.IEnumerable");
function ICollection() {
};
ICollection.typeId = "e";
Type__RegisterInterface(ICollection, "System.Collections.ICollection");
function IList() {
};
IList.typeId = "f";
Type__RegisterInterface(IList, "System.Collections.IList");
function ArrayImpl() {
};
ArrayImpl.typeId = "db";
ptyp_ = ArrayImpl.prototype;
ptyp_.__ctor = function ArrayImpl____ctor() {
};
ptyp_.system__Collections__IList__get_Item = function ArrayImpl__System__Collections__IList__get_Item(index) {
  return this.V_GetValue(index);
};
ptyp_.system__Collections__IList__Clear = function ArrayImpl__System__Collections__IList__Clear() {
  throw NotSupportedException_factory();
};
ptyp_.system__Collections__IList__RemoveAt = function ArrayImpl__System__Collections__IList__RemoveAt(index) {
  throw NotSupportedException_factory();
};
ptyp_.system__Collections__ICollection__get_Count = function ArrayImpl__System__Collections__ICollection__get_Count() {
  return this.V_get_Length();
};
ptyp_.V_get_Item_f = ptyp_.system__Collections__IList__get_Item;
ptyp_.V_Clear_f = ptyp_.system__Collections__IList__Clear;
ptyp_.V_RemoveAt_f = ptyp_.system__Collections__IList__RemoveAt;
ptyp_.V_get_Count_e = ptyp_.system__Collections__ICollection__get_Count;
ptyp_.V_Contains_f = function(arg0) {
  return this.V_Contains(arg0);
};
ptyp_.V_IndexOf_f = function(arg0) {
  return this.V_IndexOf(arg0);
};
ptyp_.V_CopyTo_e = function(arg0, arg1) {
  return this.V_CopyTo(arg0, arg1);
};
ptyp_.V_GetEnumerator_k = function() {
  return this.V_GetEnumerator();
};
Type__RegisterReferenceType(ArrayImpl, "System.ArrayImpl", Object, [IList, ICollection, IEnumerable]);
function Delegate__Combine(a, b) {
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
  rv = Delegate__CreateJoinedArray(funcs);
  rv.fullName = "Multicast Delegate";
  return rv;
};
function Delegate__Create(functionName, instance) {
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
function Delegate__Remove(source, value) {
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
  rv = Delegate__CreateJoinedArray(newArr);
  rv.fullName = "Multicast Delegate";
  return rv;
};
function Delegate__CreateJoinedArray(array) {
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
function Functiona() {
};
Functiona.typeId = "eb";
Functiona.prototype = new Function();
Type__RegisterReferenceType(Functiona, "System.MulticastDelegate", Function, []);
function LiveBinder() {
};
LiveBinder.typeId = "fb";
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
ptyp_.__ctor = function LiveBinder____ctor(binderInfo, extraObjectArray) { {
    this.binderInfo = binderInfo;
    this.extraObjectArray = extraObjectArray;
  }
};
ptyp_.set_source = function LiveBinder__set_Source(value) {
  if (this.source !== value) {
    this.source = value;
    this.flowValue();
  }
};
ptyp_.set_target = function LiveBinder__set_Target(value) {
  if (this.target !== value) {
    if (!Object__IsNullOrUndefined(this.target) && this.binderInfo.mode == 2)
      Type__CastType(INotifyPropertyChanged, this.target).V_RemovePropertyChangedListener_b(this.binderInfo.targetPropertyName, Delegate__Create("onTargetPropertyChanged", this));
    this.target = value;
    if (!Object__IsNullOrUndefined(this.target) && this.binderInfo.mode == 2)
      Type__CastType(INotifyPropertyChanged, this.target).V_AddPropertyChangedListener_b(this.binderInfo.targetPropertyName, Delegate__Create("onTargetPropertyChanged", this));
    this.flowValue();
  }
};
ptyp_.set_isActive = function LiveBinder__set_IsActive(value) {
  if (this.isActive != value) {
    this.isActive = value;
    if (this.isActive)
      this.activate();
    else
      this.deactivate();
  }
};
ptyp_.cleanup = function LiveBinder__Cleanup() {
  if (!this.isActive) {
    this.pathTraversed = 0;
    this.cleanRegistrations();
  }
};
ptyp_.activate = function LiveBinder__Activate() {
  this.flowValue();
};
ptyp_.deactivate = function LiveBinder__Deactivate() {
  this.isActive = false;
};
ptyp_.flowValue = function LiveBinder__FlowValue() {
  if (this.target == null || this.updating || !this.isActive)
    return;
  if (!this.liveObjects)
    this.liveObjects = ArrayG_$Object$_.__ctora(this.binderInfo.propertyGetterPath.length + 1);
  if (this.liveObjects.get_item(0) !== this.source) {
    if (!Object__IsNullOrUndefined(this.liveObjects.get_item(0))) {
      this.pathTraversed = 0;
      this.cleanRegistrations();
    }
    this.liveObjects.set_item(0, this.source);
    if (!Object__IsNullOrUndefined(this.liveObjects.get_item(0)))
      Type__CastType(INotifyPropertyChanged, this.liveObjects.get_item(0)).V_AddPropertyChangedListener_b(this.binderInfo.propertyNames[0], Delegate__Create("onSourcePropertyChanged", this));
  }
  this.setTargetProperty(this.getValue());
};
ptyp_.setTargetProperty = function LiveBinder__SetTargetProperty(value) {
  try {
    this.updating = true;
    this.binderInfo.setTargetValue(this.target, value, this.extraObjectArray);
  } finally {
    this.updating = false;
  }
};
ptyp_.getValue = function LiveBinder__GetValue() {
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
ptyp_.getValueInternal = function LiveBinder__GetValueInternal() {
  var binderInfo, liveObjects, src, propertyGetterPath, iPath, pathLength, propertyNames;
  binderInfo = this.binderInfo;
  liveObjects = this.liveObjects;
  src = liveObjects.get_item(0);
  propertyGetterPath = binderInfo.propertyGetterPath;
  iPath = 1, pathLength = propertyGetterPath.length + 1;
  propertyNames = binderInfo.propertyNames;
  this.pathTraversed = 1;
  for (; iPath < pathLength; iPath++)
    if (src != null || iPath == 1 && (binderInfo.binderType & 2) == 2) {
      src = propertyGetterPath[iPath - 1](src);
      if (liveObjects.get_item(iPath) !== src) {
        if (liveObjects.get_item(iPath) != null && iPath < pathLength - 1)
          Type__CastType(INotifyPropertyChanged, liveObjects.get_item(iPath)).V_RemovePropertyChangedListener_b(propertyNames[iPath], Delegate__Create("onSourcePropertyChanged", this));
        liveObjects.set_item(iPath, src);
        if (src != null && iPath < pathLength - 1 && src != null)
          Type__CastType(INotifyPropertyChanged, src).V_AddPropertyChangedListener_b(binderInfo.propertyNames[iPath], Delegate__Create("onSourcePropertyChanged", this));
      }
      ++this.pathTraversed;
    }
  if (this.pathTraversed < pathLength)
    return binderInfo.defaultValue;
  else if (!!binderInfo.forwardConverter)
    return binderInfo.forwardConverter(src);
  else
    return src;
};
ptyp_.cleanRegistrations = function LiveBinder__CleanRegistrations() {
  var liveObjects, iPath, till, item;
  liveObjects = this.liveObjects;
  if (this.pathTraversed < this.liveObjects.V_get_Length()) {
    liveObjects.set_item(liveObjects.V_get_Length() - 1, null);
    for (
    iPath = this.binderInfo.propertyGetterPath.length - 2, till = this.pathTraversed; iPath >= till; iPath--) {
      item = liveObjects.get_item(iPath);
      if (!Object__IsNullOrUndefined(item)) {
        Type__CastType(INotifyPropertyChanged, item).V_RemovePropertyChangedListener_b(this.binderInfo.propertyNames[iPath], Delegate__Create("onSourcePropertyChanged", this));
        liveObjects.set_item(iPath, null);
      }
    }
  }
};
ptyp_.onSourcePropertyChanged = function LiveBinder__OnSourcePropertyChanged(obj, str) {
  this.flowValue();
};
ptyp_.onTargetPropertyChanged = function LiveBinder__OnTargetPropertyChanged(obj, str) {
  var stmtTemp1, binderInfo, target, liveObjects, value;
  if (this.updating || !this.isActive)
    return;
  try {
    binderInfo = this.binderInfo;
    target = this.target;
    liveObjects = this.liveObjects;
    this.updating = true;
    if (target == obj && !Object__IsNullOrUndefined(this.source) && (liveObjects.V_get_Length() < 2 || !Object__IsNullOrUndefined(liveObjects.get_item(liveObjects.V_get_Length() - 2)))) {
      value = binderInfo.targetPropertyGetter(target);
      if (!Object__IsNullOrUndefined(binderInfo.backwardConverter))
        value = binderInfo.backwardConverter(value);
      binderInfo.propertySetter(this.liveObjects.V_get_Length() < 2 ? this.source : this.liveObjects.get_item(this.liveObjects.V_get_Length() - 2), value);
    }
  } catch (stmtTemp1) {
  } finally {
    this.updating = false;
  }
};
Type__RegisterReferenceType(LiveBinder, "Sunlight.Framework.UI.Helpers.LiveBinder", Object, []);
function IWindowTimer() {
};
IWindowTimer.typeId = "j";
Type__RegisterInterface(IWindowTimer, "Sunlight.Framework.IWindowTimer");
function TestWindowTimer() {
};
TestWindowTimer.typeId = "gb";
function TestWindowTimer_factory() {
  var this_;
  this_ = new TestWindowTimer();
  this_.__ctor();
  return this_;
};
TestWindowTimer.defaultConstructor = TestWindowTimer_factory;
ptyp_ = TestWindowTimer.prototype;
ptyp_.setImmediate = function TestWindowTimer__SetImmediate(action) {
  action();
  return 0;
};
ptyp_.setTimeout = function TestWindowTimer__SetTimeout(action, timoutTime) {
  action();
  return 0;
};
ptyp_.clearTimeout = function TestWindowTimer__ClearTimeout(timeoutHandle) {
};
ptyp_.__ctor = function TestWindowTimer____ctor() {
};
ptyp_.V_SetImmediate_j = ptyp_.setImmediate;
ptyp_.V_SetTimeout_j = ptyp_.setTimeout;
ptyp_.V_ClearTimeout_j = ptyp_.clearTimeout;
Type__RegisterReferenceType(TestWindowTimer, "Sunlight.Framework.TestWindowTimer", Object, [IWindowTimer]);
function TaskScheduler() {
};
TaskScheduler.typeId = "hb";
TaskScheduler__instance = null;
function TaskScheduler__get_Instance() {
  if (!TaskScheduler__instance)
    TaskScheduler__instance = TaskScheduler_factory(WindowTimer_factory(), 16, 25);
  return TaskScheduler__instance;
};
function TaskScheduler__set_Instance(value) {
  TaskScheduler__instance = value;
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
ptyp_.__ctor = function TaskScheduler____ctor(windowTimer, workQuanta, idleTimeout) {
  this.timerId = -1; {
    this.tasks = NumberDictionary_$Task$_.defaultConstructor();
    this.windowTimer = windowTimer;
    this.workQuanta = workQuanta;
    this.idleTimeout = idleTimeout;
    this.hiPriTasks = Queue_$Task$_.defaultConstructor();
    this.lowPriTasks = Queue_$Task$_.defaultConstructor();
    this.idleTasks = Queue_$Task$_.defaultConstructor();
  }
};
ptyp_.enqueueLowPriTask = function TaskScheduler__EnqueueLowPriTask(work, traceId) {
  var task;
  task = Task_factory(this.nextTimerId++, work);
  this.idleTasks.enqueue(task);
  this.tasks.add(task.taskId, task);
  this.scheduleQuanta(false);
  return TaskHandle_factory(task.taskId);
};
ptyp_.runQuanta = function TaskScheduler__RunQuanta() {
  if (this.hiPriTasks.get_count() > 0)
    this.flushQueue(this.hiPriTasks, DateTime__op_Addition(DateTime__get_Now(), this.workQuanta));
  else if (this.idleTasks.get_count() > 0)
    this.flushQueue(this.idleTasks, DateTime__op_Addition(DateTime__get_Now(), this.workQuanta / 2 | 0));
  else if (this.lowPriTasks.get_count() > 0)
    this.flushQueue(this.lowPriTasks, DateTime__op_Addition(DateTime__get_Now(), this.workQuanta / 2 | 0));
  this.timerId = -1;
  this.scheduleQuanta(true);
};
ptyp_.flushQueue = function TaskScheduler__FlushQueue(taskQueue, endBy) {
  var task, now;
  while (taskQueue.get_count() > 0) {
    task = taskQueue.dequeue();
    this.executeTask(task);
    now = DateTime__get_Now();
    if (DateTime__op_LessThan(endBy, now))
      return;
  }
};
ptyp_.executeTask = function TaskScheduler__ExecuteTask(task) {
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
ptyp_.scheduleQuanta = function TaskScheduler__ScheduleQuanta(force) {
  if (force || !this.highPriSetup && this.hiPriTasks.get_count() > 0) {
    this.windowTimer.V_ClearTimeout_j(this.timerId);
    this.timerId = -1;
  }
  if (this.timerId != -1)
    return;
  if (this.hiPriTasks.get_count() > 0) {
    this.highPriSetup = true;
    this.timerId = this.windowTimer.V_SetImmediate_j(Delegate__Create("runQuanta", this));
  }
  else if (this.idleTasks.get_count() + this.lowPriTasks.get_count() > 0) {
    this.highPriSetup = false;
    this.timerId = this.windowTimer.V_SetTimeout_j(Delegate__Create("runQuanta", this), this.idleTimeout);
  }
};
Type__RegisterReferenceType(TaskScheduler, "Sunlight.Framework.TaskScheduler", Object, []);
function NScriptsTemplatesClass() {
};
NScriptsTemplatesClass.typeId = "ib";
function NScriptsTemplatesClass__get_TestTemplate1() {
  return Test$5cTemplates$5cTestTemplate1();
};
function NScriptsTemplatesClass__get_TestTemplateVMB1() {
  return Test$5cTemplates$5cTestTemplateVMB1();
};
function NScriptsTemplatesClass__get_TestTemplateVMB1_int() {
  return Test$5cTemplates$5cTestTemplateVMB1_int();
};
function NScriptsTemplatesClass__get_TestTemplateVMC1() {
  return Test$5cTemplates$5cTestTemplateVMC1();
};
function NScriptsTemplatesClass__get_TestTemplateVMB_AttrBinding() {
  return Test$5cTemplates$5cTestTemplateVMB_AttrBinding();
};
function NScriptsTemplatesClass__get_TestTemplateVMB_CssBinding() {
  return Test$5cTemplates$5cTestTemplateVMB_CssBinding();
};
function NScriptsTemplatesClass__get_TestTemplateVMB_StyleBinding() {
  return Test$5cTemplates$5cTestTemplateVMB_StyleBinding();
};
function NScriptsTemplatesClass__get_TestTemplateB_PropertyBinding() {
  return Test$5cTemplates$5cTestTemplateB_PropertyBinding();
};
Type__RegisterReferenceType(NScriptsTemplatesClass, "Sunlight.Framework.UI.Test.NScriptsTemplatesClass", Object, []);
function ExtensibleObservableObject() {
};
ExtensibleObservableObject.typeId = "jb";
function ExtensibleObservableObject_factory() {
  var this_;
  this_ = new ExtensibleObservableObject();
  this_.__ctora();
  return this_;
};
ExtensibleObservableObject.defaultConstructor = ExtensibleObservableObject_factory;
ptyp_ = new ObservableObject();
ExtensibleObservableObject.prototype = ptyp_;
ptyp_.propertyMap = null;
ptyp_.__ctora = function ExtensibleObservableObject____ctor() {
  this.propertyMap = {
  };
  this.__ctor();
};
Type__RegisterReferenceType(ExtensibleObservableObject, "Sunlight.Framework.Observables.ExtensibleObservableObject", ObservableObject, []);
function ContextBindableObject() {
};
ContextBindableObject.typeId = "kb";
function ContextBindableObject_factory() {
  var this_;
  this_ = new ContextBindableObject();
  this_.__ctorb();
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
ptyp_.set_parent = function ContextBindableObject__set_Parent(value) {
  if (this.parent != value) {
    if (!!this.parent) {
      this.parent.removePropertyChangedListener("DataContext", Delegate__Create("onParentDataContextUpdated", this));
      this.parent.removePropertyChangedListener("IsActive", Delegate__Create("onParentDataContextUpdated", this));
    }
    this.parent = value;
    if (!this.dataContextSetterCalled)
      if (!!this.parent) {
        this.parent.addPropertyChangedListener("DataContext", Delegate__Create("onParentDataContextUpdated", this));
        this.parent.addPropertyChangedListener("IsActive", Delegate__Create("onParentDataContextUpdated", this));
        this.onParentDataContextUpdated(null, null);
      }
      else
        this.setDataContext(null);
  }
};
ptyp_.get_dataContext = function ContextBindableObject__get_DataContext() {
  return this.dataContext;
};
ptyp_.set_dataContext = function ContextBindableObject__set_DataContext(value) {
  this.dataContextSetterCalled = true;
  this.setDataContext(value);
};
ptyp_.get_isActive = function ContextBindableObject__get_IsActive() {
  return this.isActivated && !this.V_get_ActivationBlocked();
};
ptyp_.set_inactiveIfNullContext = function ContextBindableObject__set_InactiveIfNullContext(value) {
  this.isInactiveIfNullContext = value;
  this.fixActivation();
};
ptyp_.get_activationBlocked = function ContextBindableObject__get_ActivationBlocked() {
  return this.isInactiveIfNullContext && this.dataContext == null;
};
ptyp_.dispose = function ContextBindableObject__Dispose() {
  if (!this.isDisposed && !this.isDisposing)
    this.V_InternalDispose();
};
ptyp_.activate = function ContextBindableObject__Activate() {
  this.isActive = true;
  this.fixActivation();
};
ptyp_.deactivate = function ContextBindableObject__Deactivate() {
  if (!this.isActive)
    return;
  this.isActive = false;
  this.fixActivation();
};
ptyp_.onBeforeFirstActivate = function ContextBindableObject__OnBeforeFirstActivate() {
};
ptyp_.onActivate = function ContextBindableObject__OnActivate() {
};
ptyp_.onDeactivate = function ContextBindableObject__OnDeactivate() {
};
ptyp_.fixActivation = function ContextBindableObject__FixActivation() {
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
ptyp_.internalDispose = function ContextBindableObject__InternalDispose() {
  if (!!this.onDisposed) {
    this.set_parent(null);
    this.clearListeners();
    this.onDisposed();
  }
};
ptyp_.onDataContextUpdating = function ContextBindableObject__OnDataContextUpdating(newValue) {
};
ptyp_.onDataContextUpdated = function ContextBindableObject__OnDataContextUpdated(oldValue) {
};
ptyp_.setDataContext = function ContextBindableObject__SetDataContext(value) {
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
ptyp_.onParentDataContextUpdated = function ContextBindableObject__OnParentDataContextUpdated(sender, propertyName) {
  if (this.parent.get_isActive() && !this.dataContextSetterCalled)
    this.setDataContext(this.parent.get_dataContext());
  if (propertyName === "IsActive" || propertyName == null)
    if (this.parent.get_isActive())
      this.activate();
    else
      this.deactivate();
};
ptyp_.__ctorb = function ContextBindableObject____ctor() {
  this.dataContextSetterCalled = false;
  this.isActive = false;
  this.isPreActivated = false;
  this.isActivated = false;
  this.isDisposing = false;
  this.isDisposed = false;
  this.isInactiveIfNullContext = true;
  this.__ctora();
};
ptyp_.V_get_ActivationBlocked = ptyp_.get_activationBlocked;
ptyp_.V_OnBeforeFirstActivate = ptyp_.onBeforeFirstActivate;
ptyp_.V_OnActivate = ptyp_.onActivate;
ptyp_.V_OnDeactivate = ptyp_.onDeactivate;
ptyp_.V_InternalDispose = ptyp_.internalDispose;
ptyp_.V_OnDataContextUpdating = ptyp_.onDataContextUpdating;
ptyp_.V_OnDataContextUpdated = ptyp_.onDataContextUpdated;
Type__RegisterReferenceType(ContextBindableObject, "Sunlight.Framework.Binders.ContextBindableObject", ExtensibleObservableObject, []);
function UIElement() {
};
UIElement.typeId = "lb";
function UIElement_factory(element) {
  var this_;
  this_ = new UIElement();
  this_.__ctorc(element);
  return this_;
};
ptyp_ = new ContextBindableObject();
UIElement.prototype = ptyp_;
ptyp_.element = null;
ptyp_.isHidden = false;
ptyp_.eventRegistrationDict = null;
ptyp_.__ctorc = function UIElement____ctor(element) {
  this.isHidden = false;
  this.eventRegistrationDict = StringDictionary_$Action_$UIEvent$_$_.defaultConstructor();
  this.__ctorb();
  this.element = element;
};
ptyp_.get_element = function UIElement__get_Element() {
  return this.element;
};
ptyp_.get_activationBlockeda = function UIElement__get_ActivationBlocked() {
  return this.get_activationBlocked() || this.isHidden;
};
ptyp_.internalDisposea = function UIElement__InternalDispose() {
  var stmtTemp1, kvPair;
  for (stmtTemp1 = this.eventRegistrationDict.V_GetEnumerator_k(); stmtTemp1.V_MoveNext_l(); ) {
    kvPair = Type__UnBoxTypeInstance(KeyValuePair_$String_x_Action_$UIEvent$_$_, stmtTemp1.V_get_Current_l());
    Element__UnBinda(this.element, KeyValuePair_$String_x_Action_$UIEvent$_$_.get_key(kvPair));
  }
  this.eventRegistrationDict.clear();
  this.internalDispose();
};
ptyp_.V_get_ActivationBlocked = ptyp_.get_activationBlockeda;
ptyp_.V_InternalDispose = ptyp_.internalDisposea;
Type__RegisterReferenceType(UIElement, "Sunlight.Framework.UI.UIElement", ContextBindableObject, []);
function UISkinableElement() {
};
UISkinableElement.typeId = "mb";
function UISkinableElement_factory(element) {
  var this_;
  this_ = new UISkinableElement();
  this_.__ctord(element);
  return this_;
};
ptyp_ = new UIElement();
UISkinableElement.prototype = ptyp_;
ptyp_.skin = null;
ptyp_.skinInstance = null;
ptyp_.__ctord = function UISkinableElement____ctor(element) {
  this.__ctorc(element);
};
ptyp_.set_skin = function UISkinableElement__set_Skin(value) {
  if (this.skin != value) {
    this.skin = value;
    if (!!this.skin) {
      if (this.get_isActive()) {
        this.get_element().setAttribute("skin-id", this.skin.get_id());
        this.set_skinInstance(this.skin.createInstance());
      }
      else
        this.set_skinInstance(null);
    }
    else if (!this.skin)
      this.set_skinInstance(null);
    this.firePropertyChanged("Skin");
  }
};
ptyp_.get_skinInstance = function UISkinableElement__get_SkinInstance() {
  return this.skinInstance;
};
ptyp_.set_skinInstance = function UISkinableElement__set_SkinInstance(value) {
  if (this.skinInstance != value) {
    if (!!this.skinInstance)
      this.skinInstance.dispose();
    this.skinInstance = value;
    if (!!this.skinInstance) {
      this.skinInstance.bind(this);
      if (this.get_isActive())
        this.skinInstance.activate();
      this.V_ApplySkinInternal(this.skinInstance);
    }
    this.firePropertyChanged("SkinInstance");
  }
};
ptyp_.applySkinInternal = function UISkinableElement__ApplySkinInternal(skin) {
};
ptyp_.onBeforeFirstActivatea = function UISkinableElement__OnBeforeFirstActivate() {
  this.onBeforeFirstActivate();
  if (!!this.skin && !this.skinInstance)
    this.set_skinInstance(this.skin.createInstance());
};
ptyp_.onActivatea = function UISkinableElement__OnActivate() {
  this.onActivate();
  if (!!this.skin && !this.skinInstance)
    this.set_skinInstance(this.skin.createInstance());
  else if (!!this.skinInstance)
    this.skinInstance.activate();
};
ptyp_.onDeactivatea = function UISkinableElement__OnDeactivate() {
  if (!!this.skinInstance)
    this.skinInstance.deactivate();
  this.onDeactivate();
};
ptyp_.internalDisposeb = function UISkinableElement__InternalDispose() {
  if (!!this.get_skinInstance())
    this.set_skinInstance(null);
  this.set_skin(null);
  this.internalDisposea();
};
ptyp_.onDataContextUpdateda = function UISkinableElement__OnDataContextUpdated(oldValue) {
  this.onDataContextUpdated(oldValue);
  if (!!this.skinInstance)
    this.skinInstance.updateDataContext();
};
ptyp_.V_OnBeforeFirstActivate = ptyp_.onBeforeFirstActivatea;
ptyp_.V_OnActivate = ptyp_.onActivatea;
ptyp_.V_OnDeactivate = ptyp_.onDeactivatea;
ptyp_.V_InternalDispose = ptyp_.internalDisposeb;
ptyp_.V_OnDataContextUpdated = ptyp_.onDataContextUpdateda;
ptyp_.V_ApplySkinInternal = ptyp_.applySkinInternal;
Type__RegisterReferenceType(UISkinableElement, "Sunlight.Framework.UI.UISkinableElement", UIElement, []);
function Node__Remove(this_) {
  return this_.parentNode ? this_.parentNode.removeChild(this_) : this_;
};
function Element__AddClassName(this_, className) {
  var index;
  if (!Object__IsNullOrUndefined(this_.classList)) {
    this_.classList.add(className);
    return;
  }
  if (className == null || (className = String__Trim(className)).length == 0)
    return;
  if (this_.className == null || this_.className.length == 0) {
    this_.className = className;
    return;
  }
  index = 0;
  while ((index = this_.className.indexOf(className, index)) != -1) {
    if ((index == 0 || String__get_Item(this_.className, index - 1) == 32) && (index == this_.className.length - className.length || String__get_Item(this_.className, index + className.length) == 32))
      return;
    index++;
  }
  this_.className = this_.className + " " + className;
  return;
};
function Element__RemoveClassName(this_, className) {
  var index;
  if (!Object__IsNullOrUndefined(this_.classList)) {
    this_.classList.remove(className);
    return;
  }
  if (className == null || (className = String__Trim(className)).length == 0 || this_.className == null || this_.className.length == 0)
    return;
  index = 0;
  while ((index = this_.className.indexOf(className, index)) != -1) {
    if ((index == 0 || String__get_Item(this_.className, index - 1) == 32) && (index == this_.className.length - className.length || String__get_Item(this_.className, index + className.length) == 32)) {
      this_.className = this_.className.substr(0, index > 0 ? index - 1 : 0) + this_.className.substring(index + className.length);
      return;
    }
    index++;
  }
  return;
};
function Element__Bind(this_, eventName, handler, capture) {
  EventBinder__AddEvent(this_, eventName, handler, capture);
};
function Element__UnBind(this_, eventName, handler, capture) {
  EventBinder__RemoveEvent(this_, eventName, handler, capture);
};
function Element__UnBinda(this_, eventName) {
  EventBinder__RemoveEventa(this_, eventName, true);
  EventBinder__RemoveEventa(this_, eventName, false);
};
function TestViewModelB() {
};
TestViewModelB.typeId = "nb";
function TestViewModelB_factory() {
  var this_;
  this_ = new TestViewModelB();
  this_.__ctorb();
  return this_;
};
TestViewModelB.defaultConstructor = TestViewModelB_factory;
ptyp_ = new TestViewModelA();
TestViewModelB.prototype = ptyp_;
ptyp_._$PropInt2$_k__BackingField = 0;
ptyp_.get_propInt2 = function TestViewModelB__get_PropInt2() {
  return this._$PropInt2$_k__BackingField;
};
ptyp_.set_propInt2 = function TestViewModelB__set_PropInt2(value) {
  this._$PropInt2$_k__BackingField = value;
};
ptyp_.__ctorb = function TestViewModelB____ctor() {
  this.__ctora();
};
Type__RegisterReferenceType(TestViewModelB, "Sunlight.Framework.UI.Test.TestViewModelB", TestViewModelA, []);
function TestSkinableWithTestUIElementPart() {
};
TestSkinableWithTestUIElementPart.typeId = "ob";
function TestSkinableWithTestUIElementPart_factory(element) {
  var this_;
  this_ = new TestSkinableWithTestUIElementPart();
  this_.__ctore(element);
  return this_;
};
ptyp_ = new UISkinableElement();
TestSkinableWithTestUIElementPart.prototype = ptyp_;
ptyp_.__ctore = function TestSkinableWithTestUIElementPart____ctor(element) {
  this.__ctord(element);
};
ptyp_.get_part = function TestSkinableWithTestUIElementPart__get_Part() {
  if (!!this.get_skinInstance())
    return Type__CastType(TestUIElement, this.get_skinInstance().getChildById("Part1"));
  return null;
};
Type__RegisterReferenceType(TestSkinableWithTestUIElementPart, "Sunlight.Framework.UI.Test.TestSkinableWithTestUIElementPart", UISkinableElement, []);
function TestUIElement() {
};
TestUIElement.typeId = "pb";
function TestUIElement_factory(element) {
  var this_;
  this_ = new TestUIElement();
  this_.__ctord(element);
  return this_;
};
ptyp_ = new UIElement();
TestUIElement.prototype = ptyp_;
ptyp_.twoWayLooseBinding = 0;
ptyp_._$OneWayStrictBinding$_k__BackingField = null;
ptyp_.__ctord = function TestUIElement____ctor(element) {
  this.__ctorc(element);
};
ptyp_.get_oneWayStrictBinding = function TestUIElement__get_OneWayStrictBinding() {
  return this._$OneWayStrictBinding$_k__BackingField;
};
ptyp_.set_oneWayStrictBinding = function TestUIElement__set_OneWayStrictBinding(value) {
  this._$OneWayStrictBinding$_k__BackingField = value;
};
ptyp_.get_twoWayLooseBinding = function TestUIElement__get_TwoWayLooseBinding() {
  return this.twoWayLooseBinding;
};
ptyp_.set_twoWayLooseBinding = function TestUIElement__set_TwoWayLooseBinding(value) {
  if (this.twoWayLooseBinding != value) {
    this.twoWayLooseBinding = value;
    this.firePropertyChanged("TwoWayLooseBinding");
  }
};
Type__RegisterReferenceType(TestUIElement, "Sunlight.Framework.UI.Test.TestUIElement", UIElement, []);
function SkinBinderHelper() {
};
function SkinBinderHelper__Bind(binders, dataContext, targetElements) {
  var i, j, info;
  for (
  i = 0, j = binders.length; i < j; i++) {
    info = binders[i];
    SkinBinderHelper__SetPropertyValue(info, dataContext, targetElements[info.objectIndex], null);
  }
};
function SkinBinderHelper__SetAttribute(node, value, attrName) {
  if (value != null)
    node.setAttribute(attrName, value);
  else
    node.removeAttribute(attrName);
};
function SkinBinderHelper__SetTextContent(element, value) {
  if (value != null)
    element.textContent = value;
  else
    element.textContent = "";
};
function SkinBinderHelper__SetDataContext(element, value) {
  element.set_dataContext(value);
};
function SkinBinderHelper__SetCssClass(element, add, className) {
  if (add)
    Element__AddClassName(element, className);
  else
    Element__RemoveClassName(element, className);
};
function SkinBinderHelper__GetElementFromPath(element, path) {
  var iPath;
  for (iPath = 0; iPath < path.length; iPath++)
    element = element.childNodes[path[iPath]];
  return element;
};
function SkinBinderHelper__SetPropertyValue(binder, source, target, extraElementArray) {
  var stmtTemp1, stmtTemp1a;
  try {
    source = SkinBinderHelper__TraversePropertyPath(binder, source);
  } catch (stmtTemp1) {
    source = binder.defaultValue;
  }
  try {
    binder.setTargetValue(target, source, extraElementArray);
  } catch (stmtTemp1a) {
  }
};
function SkinBinderHelper__TraversePropertyPath(binder, source) {
  var iGetter, pathLength, isStatic;
  iGetter = 0, pathLength = binder.propertyGetterPath.length;
  isStatic = (binder.binderType & 2) == 2;
  for (; iGetter < pathLength && (isStatic && iGetter == 0 || !Object__IsNullOrUndefined(source)); iGetter++)
    source = binder.propertyGetterPath[iGetter](source);
  if (iGetter < pathLength)
    return binder.defaultValue;
  else if (!!binder.forwardConverter)
    source = binder.forwardConverter(source);
  return source;
};
function Boolean(boxedValue) {
  this.boxedValue = boxedValue;
};
Boolean.typeId = "qb";
Boolean.getDefaultValue = function() {
  return false;
};
function Boolean__ToString(this_) {
  return this_ ? "true" : "false";
};
ptyp_ = new ValueType();
Boolean.prototype = ptyp_;
ptyp_.toString = function() {
  return Boolean__ToString(this.boxedValue);
};
Type__RegisterStructType(Boolean, "System.Boolean", []);
function ListView() {
};
ListView.typeId = "rb";
function ListView_factory(element) {
  var this_;
  this_ = new ListView();
  this_.__ctord(element);
  return this_;
};
ptyp_ = new UIElement();
ListView.prototype = ptyp_;
ptyp_.items = null;
ptyp_.observableList = null;
ptyp_.attachedObservableList = null;
ptyp_.fixedList = null;
ptyp_.listToObservableList = null;
ptyp_.headerSkin = null;
ptyp_.headerCssClassName = null;
ptyp_.itemSkin = null;
ptyp_.itemCssClassName = null;
ptyp_.inlineItems = false;
ptyp_.topN = 0;
ptyp_.selectionHelper = null;
ptyp_.__ctord = function ListView____ctor(element) {
  this.items = List_$ListViewItem$_.defaultConstructor();
  this.topN = 1 << 30;
  this.__ctorc(element);
};
ptyp_.get_topN = function ListView__get_TopN() {
  return this.topN;
};
ptyp_.set_fixedList = function ListView__set_FixedList(value) {
  if (!!value && !!this.observableList)
    throw new Error("Can't set FixedList and ObservableList at the same time");
  if (this.fixedList != value) {
    this.fixedList = value;
    this.firePropertyChanged("FixedList");
    this.applyFixedList();
  }
};
ptyp_.get_observableList = function ListView__get_ObservableList() {
  return this.observableList;
};
ptyp_.set_observableList = function ListView__set_ObservableList(value) {
  if (!!value && !!this.fixedList)
    throw new Error("Can't set FixedList and ObservableList at the same time");
  if (this.observableList != value) {
    if (this.attachedObservableList != value && !!this.attachedObservableList) {
      this.attachedObservableList.V_remove_CollectionChanged_d(Delegate__Create("observableListCollectionChanged", this));
      this.attachedObservableList = null;
    }
    this.observableList = value;
    this.firePropertyChanged("ObservableList");
    this.applyObservableList();
  }
};
ptyp_.set_headerSkin = function ListView__set_HeaderSkin(value) {
  var items, itemCount, iItem;
  if (this.headerSkin == value)
    return;
  this.headerSkin = value;
  this.firePropertyChanged("HeaderSkin");
  if (this.get_isActive()) {
    items = this.items;
    itemCount = items.get_count();
    for (iItem = 0; iItem < itemCount; iItem++)
      this.resetSkin(items.get_item(iItem), this.observableList.V_get_Item_c(iItem));
  }
};
ptyp_.set_itemSkin = function ListView__set_ItemSkin(value) {
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
ptyp_.onActivatea = function ListView__OnActivate() {
  var stmtTemp1, item;
  this.onActivate();
  if (!!this.fixedList)
    this.applyFixedList();
  else if (!!this.observableList)
    this.applyObservableList();
  for (stmtTemp1 = this.items.V_GetEnumerator_k(); stmtTemp1.V_MoveNext_l(); ) {
    item = stmtTemp1.V_get_Current_l();
    item.activate();
  }
};
ptyp_.onDeactivatea = function ListView__OnDeactivate() {
  var stmtTemp1, item;
  for (stmtTemp1 = this.items.V_GetEnumerator_k(); stmtTemp1.V_MoveNext_l(); ) {
    item = stmtTemp1.V_get_Current_l();
    item.deactivate();
  }
  this.onDeactivate();
};
ptyp_.internalDisposeb = function ListView__InternalDispose() {
  var items, itemCount, iItem;
  items = this.items;
  itemCount = items.get_count();
  if (!!this.attachedObservableList) {
    this.attachedObservableList.V_remove_CollectionChanged_d(Delegate__Create("observableListCollectionChanged", this));
    this.attachedObservableList = null;
  }
  if (itemCount > 0) {
    for (iItem = 0; iItem < itemCount; iItem++)
      items.get_item(iItem).dispose();
    items.clear();
  }
  this.internalDisposea();
};
ptyp_.createListViewItem = function ListView__CreateListViewItem() {
  return ListViewItem_factory(this.createElement());
};
ptyp_.applyFixedList = function ListView__ApplyFixedList() {
  var items, itemsCount, iItem, list, idx;
  items = this.items;
  itemsCount = items.get_count();
  if (!this.fixedList) {
    if (!!this.listToObservableList)
      this.listToObservableList.set_inputCollection(null);
    else {
      for (iItem = 0; iItem < itemsCount; iItem++)
        this.removeChild(items.get_item(iItem));
      items.clear();
    }
    return;
  }
  if (this.get_isActive() && !!this.fixedList && (!this.listToObservableList || this.fixedList != this.listToObservableList.get_inputCollection())) {
    if (!this.listToObservableList) {
      this.listToObservableList = ObservableCollectionGenerator_$Object_x_Object$_.__ctor(function ListView__ApplyFixedList_del(a) {
        return a;
      });
      this.attachedObservableList = this.listToObservableList.get_outputCollection();
      this.attachedObservableList.V_add_CollectionChanged_d(Delegate__Create("observableListCollectionChanged", this));
    }
    list = List_$Object$_.defaultConstructor();
    for (idx = 0; idx < this.fixedList.V_get_Count_e(); idx++)
      list.add(this.fixedList.V_get_Item_f(idx));
    this.listToObservableList.set_inputCollection(list);
  }
};
ptyp_.applyObservableList = function ListView__ApplyObservableList() {
  var items, itemsCount, iItem;
  items = this.items;
  itemsCount = items.get_count();
  if (!this.observableList) {
    for (iItem = 0; iItem < itemsCount; iItem++)
      this.removeChild(items.get_item(iItem));
    items.clear();
    return;
  }
  if (this.get_isActive() && !!this.observableList && this.observableList != this.attachedObservableList) {
    this.attachedObservableList = this.observableList;
    this.attachedObservableList.V_add_CollectionChanged_d(Delegate__Create("observableListCollectionChanged", this));
    this.resetObservableItems();
  }
};
ptyp_.observableListCollectionChanged = function ListView__ObservableListCollectionChanged(collection, args) {
  var items, changeIndex, newItems, oldItems, itemCount, itemsToDelete, replaceList, replaceStartIndex, replaceCount, i;
  Debug__Assert(collection == this.attachedObservableList);
  items = this.items;
  changeIndex = args.V_get_ChangeIndex_h();
  if (args.V_get_Action_h() == 4)
    this.resetObservableItems();
  if (changeIndex > this.topN)
    return;
  newItems = args.V_get_NewItems_h();
  oldItems = args.V_get_OldItems_h();
  itemCount = args.V_get_Action_h() == 1 ? oldItems.V_get_Count_e() : newItems.V_get_Count_e();
  switch(args.V_get_Action_h()) {
    case 0: {
      if (changeIndex >= this.topN)
        break;
      if (itemCount + items.get_count() > this.topN) {
        if (changeIndex + itemCount > this.topN)
          itemCount = this.get_topN() - changeIndex;
        if (items.get_count() + itemCount > this.topN) {
          itemsToDelete = items.get_count() + itemCount - this.topN;
          this.removeChildren(items.get_count() - itemsToDelete, itemsToDelete);
        }
        this.observableEventAdd(changeIndex, itemCount, newItems);
      }
      else
        this.observableEventAdd(changeIndex, itemCount, newItems);
      break;
    }
    case 1: {
      if (!!this.attachedObservableList && this.attachedObservableList.V_get_Count_c() + itemCount <= this.topN)
        this.removeChildren(changeIndex, oldItems.V_get_Count_e());
      else {
        replaceList = List_$Object$_.defaultConstructor();
        replaceStartIndex = changeIndex + itemCount;
        replaceCount = Math.min(changeIndex + itemCount, Math.min(this.topN, this.attachedObservableList.V_get_Count_c())) - changeIndex;
        for (i = 0; i < replaceCount; i++)
          replaceList.add(this.attachedObservableList.V_get_Item_c(replaceStartIndex + i));
        this.observableEventReplace(changeIndex, replaceCount, replaceList);
        if (this.attachedObservableList.V_get_Count_c() <= this.topN)
          this.removeChildren(changeIndex + replaceCount, items.get_count() - changeIndex - replaceCount);
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
ptyp_.observableEventReplace = function ListView__ObservableEventReplace(changeIndex, listCount, list) {
  var iObject, listItem;
  for (iObject = 0; iObject < listCount; iObject++) {
    listItem = this.items.get_item(changeIndex + iObject);
    listItem.deactivate();
    this.resetSkin(this.items.get_item(changeIndex + iObject), list.V_get_Item_f(iObject));
    if (this.get_isActive())
      listItem.activate();
  }
};
ptyp_.observableEventAdd = function ListView__ObservableEventAdd(changeIndex, listCount, list) {
  var insertBeforeElem, iObject, listViewItem;
  insertBeforeElem = null;
  if (changeIndex < this.items.get_count())
    insertBeforeElem = this.items.get_item(changeIndex).get_element();
  for (iObject = 0; iObject < listCount; iObject++) {
    listViewItem = this.V_CreateListViewItem();
    if (this.itemCssClassName != null)
      listViewItem.get_element().className = this.itemCssClassName;
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
    listViewItem.deactivate();
    this.resetSkin(listViewItem, list.V_get_Item_f(iObject));
    listViewItem.set_selectionHelper(this.selectionHelper);
    this.activateChild(listViewItem);
  }
};
ptyp_.resetSkin = function ListView__ResetSkin(listViewItem, dataItem) {
  var hasHeaders, headeredItem;
  hasHeaders = !!this.headerSkin;
  if (hasHeaders) {
    headeredItem = Type__CastType(IHeaderedElement, dataItem);
    if (headeredItem.V_get_IsHeader_g()) {
      listViewItem.set_dataContext(headeredItem.V_get_Header_g());
      listViewItem.set_skin(this.headerSkin);
      if (this.headerCssClassName != null)
        listViewItem.get_element().className = this.headerCssClassName;
    }
    else {
      listViewItem.set_dataContext(headeredItem.V_get_Item_g());
      listViewItem.set_skin(this.itemSkin);
      if (this.itemCssClassName != null)
        listViewItem.get_element().className = this.itemCssClassName;
    }
  }
  else {
    listViewItem.set_dataContext(dataItem);
    listViewItem.set_skin(this.itemSkin);
    if (this.itemCssClassName != null)
      listViewItem.get_element().className = this.itemCssClassName;
  }
};
ptyp_.resetObservableItems = function ListView__ResetObservableItems() {
  var observableList, itemsCount, listCount, iObject, listViewItem;
  observableList = this.attachedObservableList;
  itemsCount = this.items.get_count();
  listCount = Math.min(observableList.V_get_Count_c(), this.topN);
  for (iObject = 0; iObject < listCount; iObject++) {
    if (iObject < itemsCount) {
      listViewItem = this.items.get_item(iObject);
      listViewItem.set_isSelected(!!this.selectionHelper ? this.selectionHelper.V_IsSelected_i(listViewItem.get_dataContext()) : false);
    }
    else {
      listViewItem = this.V_CreateListViewItem();
      if (this.itemCssClassName != null)
        listViewItem.get_element().className = this.itemCssClassName;
      if (!this.inlineItems)
        this.get_element().appendChild(listViewItem.get_element());
      else
        this.get_element().parentNode.insertBefore(listViewItem.get_element(), this.get_element());
      this.items.add(listViewItem);
    }
    listViewItem.deactivate();
    this.resetSkin(listViewItem, observableList.V_get_Item_c(iObject));
    listViewItem.set_selectionHelper(this.selectionHelper);
    this.activateChild(listViewItem);
  }
  this.removeChildren(listCount, itemsCount - listCount);
};
ptyp_.removeChildren = function ListView__RemoveChildren(changeIndex, delCount) {
  var iObject, item;
  for (iObject = delCount + changeIndex - 1; iObject >= changeIndex; iObject--) {
    item = this.items.get_item(iObject);
    this.removeChild(this.items.get_item(iObject));
    this.items.removeAt(iObject);
  }
};
ptyp_.activateChild = function ListView__ActivateChild(lvi) {
  if (this.get_isActive())
    lvi.activate();
};
ptyp_.removeChild = function ListView__RemoveChild(lvi) {
  lvi.dispose();
  Node__Remove(lvi.get_element());
  lvi.set_dataContext(null);
};
ptyp_.createElement = function ListView__CreateElement() {
  return this.get_element().ownerDocument.createElement(this.inlineItems ? "div" : "li");
};
ptyp_.V_OnActivate = ptyp_.onActivatea;
ptyp_.V_OnDeactivate = ptyp_.onDeactivatea;
ptyp_.V_InternalDispose = ptyp_.internalDisposeb;
ptyp_.V_CreateListViewItem = ptyp_.createListViewItem;
Type__RegisterReferenceType(ListView, "Sunlight.Framework.UI.ListView", UIElement, []);
function TestViewModelC() {
};
TestViewModelC.typeId = "sb";
function TestViewModelC_factory() {
  var this_;
  this_ = new TestViewModelC();
  this_.__ctorb();
  return this_;
};
TestViewModelC.defaultConstructor = TestViewModelC_factory;
ptyp_ = new TestViewModelA();
TestViewModelC.prototype = ptyp_;
ptyp_._$PropStr3$_k__BackingField = null;
ptyp_.get_propStr3 = function TestViewModelC__get_PropStr3() {
  return this._$PropStr3$_k__BackingField;
};
ptyp_.set_propStr3 = function TestViewModelC__set_PropStr3(value) {
  this._$PropStr3$_k__BackingField = value;
};
ptyp_.__ctorb = function TestViewModelC____ctor() {
  this.__ctora();
};
Type__RegisterReferenceType(TestViewModelC, "Sunlight.Framework.UI.Test.TestViewModelC", TestViewModelA, []);
function Task() {
};
Task.typeId = "tb";
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
ptyp_.__ctor = function Task____ctor(taskId, work) { {
    this.taskId = taskId;
    this.work = work;
  }
};
Type__RegisterReferenceType(Task, "Sunlight.Framework.Task", Object, []);
function Skin() {
};
Skin.typeId = "ub";
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
ptyp_.__ctor = function Skin____ctor(skinableType, dataContextType, factoryMethod, id) { {
    this.factoryMethod = factoryMethod;
    this.skinableType = skinableType;
    this.dataContextType = dataContextType;
    this.id = id;
  }
};
ptyp_.get_id = function Skin__get_Id() {
  return this.id;
};
ptyp_.get_skinableType = function Skin__get_SkinableType() {
  return this.skinableType;
};
ptyp_.createInstance = function Skin__CreateInstance() {
  return this.factoryMethod(this, window.document);
};
Type__RegisterReferenceType(Skin, "Sunlight.Framework.UI.Skin", Object, []);
function SkinInstance() {
};
SkinInstance.typeId = "vb";
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
ptyp_.__ctor = function SkinInstance____ctor(factory, rootElement, childElements, elementsOfIntrests, binders, partIdMapping, liveBinderCount, extraObjectCount) { {
    Object__IsNullOrUndefined(rootElement);
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
    if (partIdMapping != null)
      this.partIdMapping = StringDictionary_$Int32$_.__ctor(partIdMapping);
  }
};
ptyp_.getChildById = function SkinInstance__GetChildById(id) {
  if (!!this.partIdMapping && this.partIdMapping.containsKey(id))
    return this.elementsOfIntrest[this.partIdMapping.get_item(id)];
  return null;
};
ptyp_.bind = function SkinInstance__Bind(skinable) {
  var childNodes, skinableElement;
  if (!this.rootElement || this.isDiposed)
    throw new Error("InvalidOperation, Skin already applied");
  if (!this.parentFactory.get_skinableType().isInstanceOfType(skinable))
    throw new Error("Skin being applied to wrong Skinable");
  if (this.skinableParent == skinable)
    return;
  if (!!this.skinableParent) {
    childNodes = this.skinableParent.get_element().childNodes;
    while (childNodes.length > 0)
      this.rootElement.appendChild(childNodes[0]);
  }
  this.skinableParent = skinable;
  if (!!this.skinableParent) {
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
ptyp_.updateDataContext = function SkinInstance__UpdateDataContext() {
  if (!!this.skinableParent) {
    if (this.skinableParent.get_dataContext() !== this.dataContext) {
      this.dataContext = this.skinableParent.get_dataContext();
      this.dataContextUpdated = true;
    }
  }
  else if (this.dataContext != null) {
    this.dataContext = null;
    this.dataContextUpdated = true;
  }
  if (this.dataContextUpdated && this.isActive && !this.isDiposed) {
    this.updateBinderSource(this.dataContext, 1);
    this.dataContextUpdated = false;
  }
};
ptyp_.activate = function SkinInstance__Activate() {
  var childElements, binders, childElementLength, elementsOfIntrest, binderLength, dataContext, dataContextSetter, iBinder, iLiveBinder, binder, source, liveBinder, iChild, objectIndex, childElement;
  if (!this.isActive && !this.isDiposed) {
    this.isActive = true;
    childElements = this.childElements;
    binders = this.binders;
    childElementLength = childElements.length;
    elementsOfIntrest = this.elementsOfIntrest;
    binderLength = binders.length;
    dataContext = this.dataContext;
    dataContextSetter = SkinBinderHelper__SetDataContext;
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
        if (Object__IsNullOrUndefined(liveBinder)) {
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
        SkinBinderHelper__SetPropertyValue(binder, source, elementsOfIntrest[binder.objectIndex], this.extraObjects);
        if (binder.targetPropertySetter == dataContextSetter)
          this.hasDataContextBinding[binder.objectIndex] = true;
      }
      if (binder.mode != 0)
        ++iLiveBinder;
    }
    for (iChild = 0; iChild < childElementLength; iChild++) {
      objectIndex = childElements[iChild];
      childElement = NativeArray__GetFrom(elementsOfIntrest, childElements[iChild]);
      if (!this.hasDataContextBinding[objectIndex])
        childElement.set_dataContext(dataContext);
      childElement.activate();
    }
    this.firstActivationDone = true;
    TaskScheduler__get_Instance().enqueueLowPriTask(Delegate__Create("queuedActivation", this), "SkinInstance.Activate");
  }
};
ptyp_.deactivate = function SkinInstance__Deactivate() {
  var childElements, childElementLength, liveBinders, liveBinderLength, iLiveBinder, iChild;
  if (this.isActive && !this.isDiposed) {
    this.isActive = false;
    childElements = this.childElements;
    childElementLength = childElements.length;
    liveBinders = this.liveBinders;
    if (!Object__IsNullOrUndefined(liveBinders)) {
      liveBinderLength = liveBinders.length;
      for (iLiveBinder = 0; iLiveBinder < liveBinderLength; iLiveBinder++) {
        if (Object__IsNullOrUndefined(liveBinders[iLiveBinder]))
          continue;
        liveBinders[iLiveBinder].set_isActive(false);
      }
    }
    for (iChild = 0; iChild < childElementLength; iChild++)
      NativeArray__GetFrom(this.elementsOfIntrest, childElements[iChild]).deactivate();
    TaskScheduler__get_Instance().enqueueLowPriTask(Delegate__Create("queuedDeactivation", this), "SkinInstance.QueuedDeactivate");
  }
};
ptyp_.dispose = function SkinInstance__Dispose() {
  var childNodes, iLiveBinder, liveBinder, i, j, childElement;
  if (!this.isDiposed) {
    if (!!this.skinableParent) {
      childNodes = this.skinableParent.get_element().childNodes;
      while (childNodes.length > 0)
        this.rootElement.appendChild(childNodes[0]);
    }
    if (!Object__IsNullOrUndefined(this.liveBinders))
      for (iLiveBinder = 0; iLiveBinder < this.liveBinders.length; iLiveBinder++) {
        liveBinder = this.liveBinders[iLiveBinder];
        if (Object__IsNullOrUndefined(liveBinder))
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
      childElement = NativeArray__GetFrom(this.elementsOfIntrest, this.childElements[i]);
      childElement.deactivate();
      childElement.dispose();
    }
  }
};
ptyp_.queuedActivation = function SkinInstance__QueuedActivation() {
  var binders, liveBinders, binderLength, liveBindersLength, iBinderInfo, iLivebinder, binder, liveBinder;
  binders = this.binders;
  liveBinders = this.liveBinders;
  if (Object__IsNullOrUndefined(liveBinders))
    return;
  binderLength = binders.length;
  liveBindersLength = liveBinders.length;
  for (
  iBinderInfo = 0, iLivebinder = 0; iBinderInfo < binderLength && iLivebinder < liveBindersLength; iBinderInfo++) {
    binder = binders[iBinderInfo];
    if (binder.mode != 0) {
      liveBinder = liveBinders[iLivebinder];
      if (Object__IsNullOrUndefined(liveBinder)) {
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
ptyp_.queuedDeactivation = function SkinInstance__QueuedDeactivation() {
  var iLiveBinder, liveBinder;
  if (this.isActive || this.isDiposed || Object__IsNullOrUndefined(this.liveBinders))
    return;
  for (iLiveBinder = 0; iLiveBinder < this.liveBinders.length; iLiveBinder++) {
    liveBinder = this.liveBinders[iLiveBinder];
    if (Object__IsNullOrUndefined(liveBinder))
      return;
    liveBinder.set_isActive(false);
    liveBinder.cleanup();
  }
};
ptyp_.updateBinderSource = function SkinInstance__UpdateBinderSource(source, sourceType) {
  var liveBinders, binders, bindersLength, liveBindersLength, iBinder, iLiveBinder, binder, childElements, childElementLength, iChild, objectIndex, childElement;
  liveBinders = this.liveBinders;
  binders = this.binders;
  bindersLength = binders.length;
  liveBindersLength = Object__IsNullOrUndefined(liveBinders) ? 0 : liveBinders.length;
  for (
  iBinder = 0, iLiveBinder = 0; iBinder < bindersLength; iBinder++) {
    binder = binders[iBinder];
    if (binder.mode != 0 && iLiveBinder < liveBindersLength && !Object__IsNullOrUndefined(liveBinders[iLiveBinder])) {
      if (sourceType == (binder.binderType & 7))
        liveBinders[iLiveBinder].set_source(source);
      iLiveBinder++;
    }
    else if (sourceType == (binder.binderType & 7))
      SkinBinderHelper__SetPropertyValue(binder, source, this.elementsOfIntrest[binder.objectIndex], this.extraObjects);
  }
  if (sourceType == 1) {
    childElements = this.childElements;
    childElementLength = childElements.length;
    for (iChild = 0; iChild < childElementLength; iChild++) {
      objectIndex = childElements[iChild];
      childElement = NativeArray__GetFrom(this.elementsOfIntrest, childElements[iChild]);
      if (!this.hasDataContextBinding[objectIndex])
        childElement.set_dataContext(this.dataContext);
      childElement.activate();
    }
  }
};
Type__RegisterReferenceType(SkinInstance, "Sunlight.Framework.UI.Helpers.SkinInstance", Object, []);
function ListViewItem() {
};
ListViewItem.typeId = "wb";
function ListViewItem_factory(element) {
  var this_;
  this_ = new ListViewItem();
  this_.__ctore(element);
  return this_;
};
ptyp_ = new UISkinableElement();
ListViewItem.prototype = ptyp_;
ptyp_.isSelected = false;
ptyp_.selectionHelper = null;
ptyp_.__ctore = function ListViewItem____ctor(element) {
  this.__ctord(element);
};
ptyp_.set_isSelected = function ListViewItem__set_IsSelected(value) {
  if (this.isSelected != value) {
    this.isSelected = value;
    if (!!this.selectionHelper)
      if (value)
        this.selectionHelper.V_SelectItem_i(this.get_dataContext());
      else
        this.selectionHelper.V_UnSelectItem_i(this.get_dataContext());
    this.firePropertyChanged("IsSelected");
  }
};
ptyp_.set_selectionHelper = function ListViewItem__set_SelectionHelper(value) {
  if (this.selectionHelper == value)
    return;
  if (!!this.selectionHelper)
    this.selectionHelper.V_remove_SelectionChanged_i(Delegate__Create("onSelectionHelperSelectionChanged", this));
  this.selectionHelper = value;
  if (!!this.selectionHelper) {
    this.selectionHelper.V_add_SelectionChanged_i(Delegate__Create("onSelectionHelperSelectionChanged", this));
    this.onSelectionHelperSelectionChanged();
  }
};
ptyp_.onDataContextUpdatedb = function ListViewItem__OnDataContextUpdated(oldValue) {
  this.onDataContextUpdateda(oldValue);
  if (!!this.selectionHelper)
    this.onSelectionHelperSelectionChanged();
};
ptyp_.internalDisposec = function ListViewItem__InternalDispose() {
  if (!!this.selectionHelper)
    this.selectionHelper.V_remove_SelectionChanged_i(Delegate__Create("onSelectionHelperSelectionChanged", this));
  this.internalDisposeb();
};
ptyp_.onSelectionHelperSelectionChanged = function ListViewItem__OnSelectionHelperSelectionChanged() {
  this.set_isSelected(this.selectionHelper.V_IsSelected_i(this.get_dataContext()));
};
ptyp_.V_InternalDispose = ptyp_.internalDisposec;
ptyp_.V_OnDataContextUpdated = ptyp_.onDataContextUpdatedb;
Type__RegisterReferenceType(ListViewItem, "Sunlight.Framework.UI.ListViewItem", UISkinableElement, []);
Error.typeId = "xb";
Type__RegisterReferenceType(Error, "System.Exception", Object, []);
function INotifyCollectionChanged() {
};
INotifyCollectionChanged.typeId = "d";
Type__RegisterInterface(INotifyCollectionChanged, "Sunlight.Framework.Observables.INotifyCollectionChanged");
function IObservableCollection() {
};
IObservableCollection.typeId = "c";
Type__RegisterInterface(IObservableCollection, "Sunlight.Framework.Observables.IObservableCollection");
function UIEvent() {
};
UIEvent.typeId = "yb";
Type__RegisterReferenceType(UIEvent, "Sunlight.Framework.UI.UIEvent", Object, []);
function IHeaderedElement() {
};
IHeaderedElement.typeId = "g";
Type__RegisterInterface(IHeaderedElement, "Sunlight.Framework.Observables.IHeaderedElement");
function Debug__Assert(condition) {
};
function CollectionChangedEventArgs() {
};
CollectionChangedEventArgs.typeId = "h";
Type__RegisterInterface(CollectionChangedEventArgs, "Sunlight.Framework.Observables.CollectionChangedEventArgs");
function NativeArray__GetFrom(this_, index) {
  return this_[index];
};
Function.getDefaultValue = function() {
  return {
  };
};
function ExceptionHelpers() {
};
function ExceptionHelpers__ThrowOnOutOfRange(value, minValue, maxValue, argumentName) {
  if (value < minValue || value > maxValue)
    throw new Error("Out of range exception: " + argumentName);
};
function ExceptionHelpers__ThrowOnArgumentNull(value, argumentName) {
  if (value == null)
    throw new Error("ArgumentNull: " + argumentName);
};
function ISelectionHelper() {
};
ISelectionHelper.typeId = "i";
Type__RegisterInterface(ISelectionHelper, "Sunlight.Framework.UI.ISelectionHelper");
function WindowTimer() {
};
WindowTimer.typeId = "zb";
function WindowTimer_factory() {
  var this_;
  this_ = new WindowTimer();
  this_.__ctor();
  return this_;
};
WindowTimer.defaultConstructor = WindowTimer_factory;
ptyp_ = WindowTimer.prototype;
ptyp_.setImmediate = function WindowTimer__SetImmediate(action) {
  if (typeof setImmediate != "undefined")
    return setImmediate(action);
  return setTimeout(action, 0);
};
ptyp_.setTimeout = function WindowTimer__SetTimeout(action, timeoutHandle) {
  if (timeoutHandle == 0)
    return this.setImmediate(action);
  return setTimeout(action, timeoutHandle);
};
ptyp_.clearTimeout = function WindowTimer__ClearTimeout(timeoutHandle) {
  if (typeof clearImmediate != "undefined")
    clearImmediate(timeoutHandle);
  clearTimeout(timeoutHandle);
};
ptyp_.__ctor = function WindowTimer____ctor() {
};
ptyp_.V_SetImmediate_j = ptyp_.setImmediate;
ptyp_.V_SetTimeout_j = ptyp_.setTimeout;
ptyp_.V_ClearTimeout_j = ptyp_.clearTimeout;
Type__RegisterReferenceType(WindowTimer, "Sunlight.Framework.WindowTimer", Object, [IWindowTimer]);
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
ptyp_.__ctor = function TaskHandle____ctor(taskId) {
  this.taskId = taskId;
};
Type__RegisterReferenceType(TaskHandle, "Sunlight.Framework.TaskHandle", Object, []);
function EventBinder() {
};
EventBinder.typeId = "bc";
function EventBinder__GetBinder(importedElement) {
  if (Object__IsNullOrUndefined(importedElement.importedExtension))
    importedElement.importedExtension = {
    };
  if (Object__IsNullOrUndefined(importedElement.importedExtension.importedExtension))
    importedElement.importedExtension.importedExtension = EventBinder_factory(importedElement);
  return Type__CastType(EventBinder, importedElement.importedExtension.importedExtension);
};
function EventBinder__AddEvent(importedElement, name, action, onCapture) {
  var binder;
  binder = EventBinder__GetBinder(importedElement);
  binder.addEvent(name, action, onCapture);
};
function EventBinder__RemoveEvent(importedElement, name, action, onCapture) {
  var binder;
  if (importedElement.importedExtension == null || importedElement.importedExtension.importedExtension == null)
    return;
  binder = EventBinder__GetBinder(importedElement);
  binder.removeEvent(name, action, onCapture);
};
function EventBinder__RemoveEventa(importedElement, name, onCapture) {
  var binder;
  if (importedElement.importedExtension == null || importedElement.importedExtension.importedExtension == null)
    return;
  binder = EventBinder__GetBinder(importedElement);
  binder.removeEventa(name, onCapture);
};
function EventBinder__IsW3wc(element) {
  return !!element.addEventListener;
};
function EventBinder__GetEventType(evt) {
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
ptyp_.dataDictionary = null;
ptyp_.target = null;
ptyp_.disposed = false;
ptyp_.__ctor = function EventBinder____ctor(element) {
  this.capturePhaseEvents = StringDictionary_$Function$_.defaultConstructor();
  this.bubblePhaseEvents = StringDictionary_$Function$_.defaultConstructor();
  this.dataDictionary = null;
  this.disposed = false;
  this.target = element;
};
ptyp_.addEvent = function EventBinder__AddEventa(name, action, onCapture) {
  var isW3wc, evts, elementEvent;
  isW3wc = EventBinder__IsW3wc(this.target);
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
    if (onCapture && EventBinder__IsW3wc(this.target))
      this.addEventListener(name, Delegate__Create("eventHandlerCapture", this), true);
    else if (isW3wc)
      this.addEventListener(name, Delegate__Create("eventHandlerBubble", this), false);
    else
      this.attachEvent(name, Delegate__Create("eventHandlerIE", this));
  }
  else
    elementEvent = Delegate__Combine(elementEvent, action);
  evts.set_item(name, elementEvent);
};
ptyp_.removeEvent = function EventBinder__RemoveEventa(name, handler, onCapture) {
  var isW3wc, evts, elementEvent;
  isW3wc = EventBinder__IsW3wc(this.target);
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
    elementEvent = Delegate__Remove(elementEvent, handler);
    if (!elementEvent) {
      evts.remove(name);
      if (onCapture)
        this.removeEventListener(name, Delegate__Create("eventHandlerCapture", this), true);
      else if (isW3wc)
        this.removeEventListener(name, Delegate__Create("eventHandlerBubble", this), false);
      else
        this.detachEvent(name, Delegate__Create("eventHandlerIE", this));
    }
    else
      evts.set_item(name, elementEvent);
  }
};
ptyp_.removeEventa = function EventBinder__RemoveEventb(name, onCapture) {
  var isW3wc, evts;
  isW3wc = EventBinder__IsW3wc(this.target);
  onCapture = onCapture && isW3wc;
  evts = onCapture ? this.capturePhaseEvents : this.bubblePhaseEvents;
  if (evts.remove(name))
    if (onCapture)
      this.removeEventListener(name, Delegate__Create("eventHandlerCapture", this), true);
    else if (isW3wc)
      this.removeEventListener(name, Delegate__Create("eventHandlerBubble", this), true);
    else
      this.detachEvent(name, Delegate__Create("eventHandlerIE", this));
};
ptyp_.addEventListener = function EventBinder__AddEventListener(evtName, cb, isCapture) {
  this.target.addEventListener(evtName, cb, isCapture);
};
ptyp_.attachEvent = function EventBinder__AttachEvent(evtName, cb) {
  this.target.atachEvent("on" + evtName, cb);
};
ptyp_.removeEventListener = function EventBinder__RemoveEventListener(evtName, cb, isCapture) {
  this.target.removeEventListener(evtName, cb, isCapture);
};
ptyp_.detachEvent = function EventBinder__DetachEvent(evtName, cb) {
  this.target.detachEvent("on" + evtName, cb);
};
ptyp_.eventHandlerIE = function EventBinder__EventHandlerIE() {
  this.eventHandlerBubble(event);
};
ptyp_.eventHandlerCapture = function EventBinder__EventHandlerCapture(evt) {
  if (this.disposed)
    return;
  this.capturePhaseEvents.get_item(EventBinder__GetEventType(evt))(this.target, evt);
};
ptyp_.eventHandlerBubble = function EventBinder__EventHandlerBubble(evt) {
  var del;
  if (this.disposed)
    return;
  if (this.bubblePhaseEvents.tryGetValue(EventBinder__GetEventType(evt), {
    read: function() {
      return del;
    },
    write: function(arg0) {
      return del = arg0;
    }
  }))
    del(this.target, evt);
};
Type__RegisterReferenceType(EventBinder, "System.EventBinder", Object, []);
RegExp.typeId = "cc";
Type__RegisterReferenceType(RegExp, "System.RegularExpression", Object, []);
function IEnumerator() {
};
IEnumerator.typeId = "l";
Type__RegisterInterface(IEnumerator, "System.Collections.IEnumerator");
Date.typeId = "dc";
function DateTime__op_LessThan(a, b) {
  return a - b < 0;
};
function DateTime__op_Addition(a, n) {
  return new Date(a.valueOf() + n);
};
function DateTime__get_Now() {
  return new Date();
};
function DateTime____cctor() {
  Date.empty = new Date(0);
};
Type__RegisterReferenceType(Date, "System.DateTime", Object, []);
function Int64(boxedValue) {
  this.boxedValue = boxedValue;
};
Int64.typeId = "ec";
Int64.getDefaultValue = function() {
  return 0;
};
function Int64__ToString(this_, radix) {
  return this_.toString(radix);
};
function Int64__ToStringa(this_) {
  return Int64__ToString(this_, 10);
};
ptyp_ = new ValueType();
Int64.prototype = ptyp_;
ptyp_.toString = function() {
  return Int64__ToStringa(this.boxedValue);
};
Type__RegisterStructType(Int64, "System.Int64", []);
Number.typeId = "n";
Type__RegisterReferenceType(Number, "System.Number", Object, []);
function NotSupportedException() {
};
NotSupportedException.typeId = "cd";
function NotSupportedException_factory() {
  var this_;
  this_ = new NotSupportedException();
  this_.__ctor();
  return this_;
};
NotSupportedException.defaultConstructor = NotSupportedException_factory;
ptyp_ = new Error();
NotSupportedException.prototype = ptyp_;
ptyp_.__ctor = function NotSupportedException____ctor() {
};
Type__RegisterReferenceType(NotSupportedException, "System.NotSupportedException", Error, []);
Array.typeId = "dd";
Type__RegisterReferenceType(Array, "System.Array", Object, [IList, ICollection, IEnumerable]);
function ValueIfTrue(T, _callStatiConstructor) {
  var ValueIfTrue$1_$T$_, __initTracker;
  if (ValueIfTrue[T.typeId])
    return ValueIfTrue[T.typeId];
  ValueIfTrue[T.typeId] = function Sunlight__Framework__UI__Test__ValueIfTrue$1() {
  };
  ValueIfTrue$1_$T$_ = ValueIfTrue[T.typeId];
  ValueIfTrue$1_$T$_.genericParameters = [T];
  ValueIfTrue$1_$T$_.genericClosure = ValueIfTrue;
  ValueIfTrue$1_$T$_.typeId = "fc$" + T.typeId + "$";
  ValueIfTrue$1_$T$_.__ctor = function Sunlight_Framework_UI_Test_ValueIfTrue$1_factory(value) {
    var this_;
    this_ = new ValueIfTrue$1_$T$_();
    this_.__ctor(value);
    return this_;
  };
  ptyp_ = ValueIfTrue$1_$T$_.prototype;
  ptyp_.value = Type__GetDefaultValueStatic(T);
  ptyp_.defaultValue = Type__GetDefaultValueStatic(T);
  ptyp_.__ctor = function ValueIfTrue$1____ctor(value) { {
      this.value = value;
      this.defaultValue = Type__GetDefaultValueStatic(T);
    }
  };
  Type__RegisterReferenceType(ValueIfTrue$1_$T$_, "Sunlight.Framework.UI.Test.ValueIfTrue`1<" + T.fullName + ">", Object, []);
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
  var Enumerator_$T$_, ArrayG$1_$T$_, IList$1_$T$_, IEnumerable$1_$T$_, __initTracker, T$5b$5d_$T$_, __initTrackera;
  if (ArrayG[T.typeId])
    return ArrayG[T.typeId];
  ArrayG[T.typeId] = function System__ArrayG$1() {
  };
  ArrayG$1_$T$_ = ArrayG[T.typeId];
  ArrayG$1_$T$_.genericParameters = [T];
  ArrayG$1_$T$_.genericClosure = ArrayG;
  ArrayG$1_$T$_.typeId = "gc$" + T.typeId + "$";
  IList$1_$T$_ = ILista(T, _callStatiConstructor);
  IEnumerable$1_$T$_ = IEnumerablea(T, _callStatiConstructor);
  ArrayG$1_$T$_.__ctora = function System_ArrayG$1_factory(size) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctora(size);
    return this_;
  };
  ArrayG$1_$T$_.__ctor = function System_ArrayG$1_factory(nativeArray) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctorb(nativeArray);
    return this_;
  };
  ptyp_ = new ArrayImpl();
  ArrayG$1_$T$_.prototype = ptyp_;
  ptyp_.innerArray = null;
  ptyp_.__ctora = function ArrayG$1____ctor(size) {
    var def, i;
    this.__ctor(); {
      this.innerArray = new Array(size);
      def = Type__GetDefaultValueStatic(T);
      for (i = 0; i < size; i++)
        this.innerArray[i] = def;
    }
  };
  ptyp_.__ctorb = function ArrayG$1____ctor(nativeArray) {
    this.__ctor();
    this.innerArray = nativeArray;
  };
  ptyp_.get_length = function ArrayG$1__get_Length() {
    return this.innerArray.length;
  };
  ptyp_.get_item = function ArrayG$1__get_Item(index) {
    var arr;
    arr = this.innerArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index];
  };
  ptyp_.set_item = function ArrayG$1__set_Item(index, value) {
    var arr;
    arr = this.innerArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index] = value;
  };
  ptyp_.get_innerArray = function ArrayG$1__get_InnerArray() {
    return this.innerArray;
  };
  ptyp_.contains = function ArrayG$1__Contains(item) {
    return this.V_IndexOf(item) >= 0;
  };
  ptyp_.indexOf = function ArrayG$1__IndexOf(item) {
    if (!T.isInstanceOfType(item))
      return -1;
    return NativeArray$1__IndexOf(this.innerArray, Type__UnBoxTypeInstance(T, item), 0);
  };
  ptyp_.getValue = function ArrayG$1__GetValue(index) {
    return Type__BoxTypeInstance(T, this.get_item(index));
  };
  ptyp_.system__Collections__Generic__IList_$T$___Insert = function ArrayG$1__System__Collections__Generic__IList_$T$___Insert(index, item) {
    throw new Error("Not Implemented.");
  };
  ptyp_.copyTo = function ArrayG$1__CopyTo(arr, index) {
    var nativeArray, length, nativeArrDst, i;
    nativeArray = this.innerArray;
    length = nativeArray.length;
    nativeArrDst = NativeArray$1__op_Implicit(arr);
    if (nativeArrDst.length < index + nativeArray.length)
      throw new Error("can't copy, dest array too small.");
    for (i = 0; i < length; i++)
      nativeArrDst[i + index] = nativeArray[i];
  };
  ptyp_.copyToa = function ArrayG$1__CopyTo(array, index) {
    var arr;
    arr = Type__CastType(T$5b$5d_$T$_, array);
    this.copyTo(arr, index);
  };
  ptyp_.getEnumerator = function ArrayG$1__GetEnumerator() {
    return Enumerator_$T$_.__ctor(this);
  };
  ptyp_.system__Collections__Generic__IEnumerable_$T$___GetEnumerator = function ArrayG$1__System__Collections__Generic__IEnumerable_$T$___GetEnumerator() {
    return Enumerator_$T$_.__ctor(this);
  };
  ptyp_["V_Insert_" + IList$1_$T$_.typeId] = ptyp_.system__Collections__Generic__IList_$T$___Insert;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.system__Collections__Generic__IEnumerable_$T$___GetEnumerator;
  ptyp_.V_get_Length = ptyp_.get_length;
  ptyp_.V_Contains = ptyp_.contains;
  ptyp_.V_GetEnumerator = ptyp_.getEnumerator;
  ptyp_.V_IndexOf = ptyp_.indexOf;
  ptyp_.V_GetValue = ptyp_.getValue;
  ptyp_.V_CopyTo = ptyp_.copyToa;
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  Type__RegisterReferenceType(ArrayG$1_$T$_, "System.ArrayG`1<" + T.fullName + ">", ArrayImpl, [IList$1_$T$_, IList, ICollection, IEnumerable, IEnumerable$1_$T$_]);
  ArrayG$1_$T$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    T = T;
    Enumerator_$T$_ = Enumerator(T, true);
    ArrayG$1_$T$_ = ArrayG(T, true);
    T$5b$5d_$T$_ = ArrayG(T, true);
  };
  if (_callStatiConstructor)
    ArrayG$1_$T$_._tri();
  return ArrayG$1_$T$_;
};
function NativeArray$1__IndexOf(this_, value, startIndex) {
  startIndex = startIndex < 0 ? 0 : startIndex;
  return this_.indexOf(value, startIndex);
};
function NativeArray$1__InsertAt(this_, index, value) {
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  this_.splice(index, 0, value);
};
function NativeArray$1__InsertRangeAt(this_, index, values) {
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  values.unshift(0);
  values.unshift(index);
  this_.splice.apply(this_, values);
  values.shift();
  values.shift();
};
function NativeArray$1__RemoveAt(this_, index) {
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  this_.splice(index, 1);
};
function NativeArray$1__op_Implicit(n) {
  return !n ? null : n.get_innerArray();
};
function Func(T1, TRes, _callStatiConstructor) {
  var Func$2_$T1_x_TRes$_, __initTracker;
  if (Func[T1.typeId] && Func[T1.typeId][TRes.typeId])
    return Func[T1.typeId][TRes.typeId];
    Func[T1.typeId] = {
    };
  Func[T1.typeId][TRes.typeId] = function System__Func$2() {
  };
  Func$2_$T1_x_TRes$_ = Func[T1.typeId][TRes.typeId];
  Func$2_$T1_x_TRes$_.genericParameters = [T1, TRes];
  Func$2_$T1_x_TRes$_.genericClosure = Func;
  Func$2_$T1_x_TRes$_.typeId = "hc$" + T1.typeId + "_" + TRes.typeId + "$";
  Func$2_$T1_x_TRes$_.prototype = new Functiona();
  Type__RegisterReferenceType(Func$2_$T1_x_TRes$_, "System.Func`2<" + T1.fullName + "," + TRes.fullName + ">", Functiona, []);
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
ContextBindableObject.typeId = "kb";
function ContextBindableObject_factory() {
  var this_;
  this_ = new ContextBindableObject();
  this_.__ctorb();
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
ptyp_.set_parent = function ContextBindableObject__set_Parent(value) {
  if (this.parent != value) {
    if (!!this.parent) {
      this.parent.removePropertyChangedListener("DataContext", Delegate__Create("onParentDataContextUpdated", this));
      this.parent.removePropertyChangedListener("IsActive", Delegate__Create("onParentDataContextUpdated", this));
    }
    this.parent = value;
    if (!this.dataContextSetterCalled)
      if (!!this.parent) {
        this.parent.addPropertyChangedListener("DataContext", Delegate__Create("onParentDataContextUpdated", this));
        this.parent.addPropertyChangedListener("IsActive", Delegate__Create("onParentDataContextUpdated", this));
        this.onParentDataContextUpdated(null, null);
      }
      else
        this.setDataContext(null);
  }
};
ptyp_.get_dataContext = function ContextBindableObject__get_DataContext() {
  return this.dataContext;
};
ptyp_.set_dataContext = function ContextBindableObject__set_DataContext(value) {
  this.dataContextSetterCalled = true;
  this.setDataContext(value);
};
ptyp_.get_isActive = function ContextBindableObject__get_IsActive() {
  return this.isActivated && !this.V_get_ActivationBlocked();
};
ptyp_.set_inactiveIfNullContext = function ContextBindableObject__set_InactiveIfNullContext(value) {
  this.isInactiveIfNullContext = value;
  this.fixActivation();
};
ptyp_.get_activationBlocked = function ContextBindableObject__get_ActivationBlocked() {
  return this.isInactiveIfNullContext && this.dataContext == null;
};
ptyp_.dispose = function ContextBindableObject__Dispose() {
  if (!this.isDisposed && !this.isDisposing)
    this.V_InternalDispose();
};
ptyp_.activate = function ContextBindableObject__Activate() {
  this.isActive = true;
  this.fixActivation();
};
ptyp_.deactivate = function ContextBindableObject__Deactivate() {
  if (!this.isActive)
    return;
  this.isActive = false;
  this.fixActivation();
};
ptyp_.onBeforeFirstActivate = function ContextBindableObject__OnBeforeFirstActivate() {
};
ptyp_.onActivate = function ContextBindableObject__OnActivate() {
};
ptyp_.onDeactivate = function ContextBindableObject__OnDeactivate() {
};
ptyp_.fixActivation = function ContextBindableObject__FixActivation() {
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
ptyp_.internalDispose = function ContextBindableObject__InternalDispose() {
  if (!!this.onDisposed) {
    this.set_parent(null);
    this.clearListeners();
    this.onDisposed();
  }
};
ptyp_.onDataContextUpdating = function ContextBindableObject__OnDataContextUpdating(newValue) {
};
ptyp_.onDataContextUpdated = function ContextBindableObject__OnDataContextUpdated(oldValue) {
};
ptyp_.setDataContext = function ContextBindableObject__SetDataContext(value) {
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
ptyp_.onParentDataContextUpdated = function ContextBindableObject__OnParentDataContextUpdated(sender, propertyName) {
  if (this.parent.get_isActive() && !this.dataContextSetterCalled)
    this.setDataContext(this.parent.get_dataContext());
  if (propertyName === "IsActive" || propertyName == null)
    if (this.parent.get_isActive())
      this.activate();
    else
      this.deactivate();
};
ptyp_.__ctorb = function ContextBindableObject____ctor() {
  this.dataContextSetterCalled = false;
  this.isActive = false;
  this.isPreActivated = false;
  this.isActivated = false;
  this.isDisposing = false;
  this.isDisposed = false;
  this.isInactiveIfNullContext = true;
  this.__ctora();
};
ptyp_.V_get_ActivationBlocked = ptyp_.get_activationBlocked;
ptyp_.V_OnBeforeFirstActivate = ptyp_.onBeforeFirstActivate;
ptyp_.V_OnActivate = ptyp_.onActivate;
ptyp_.V_OnDeactivate = ptyp_.onDeactivate;
ptyp_.V_InternalDispose = ptyp_.internalDispose;
ptyp_.V_OnDataContextUpdating = ptyp_.onDataContextUpdating;
ptyp_.V_OnDataContextUpdated = ptyp_.onDataContextUpdated;
Type__RegisterReferenceType(ContextBindableObject, "Sunlight.Framework.Binders.ContextBindableObject", ExtensibleObservableObject, []);
function HeaderInjectableTransformer(T, H, _callStatiConstructor) {
  var ObservableCollection$1_$InjectedElement$2_$T_x_H$_$_, ObservableCollection$1_$T$_, TransformedIndexTuple_$T_x_H$_, ElementsTuple_$T_x_H$_, List$1_$InjectedElement$2_$T_x_H$_$_, IList$1_$T$_, InjectedElement$2_$T_x_H$_, List$1_$T$_, HeaderInjectableTransformer$2_$T_x_H$_, __initTracker, __initTrackera;
  if (HeaderInjectableTransformer[T.typeId] && HeaderInjectableTransformer[T.typeId][H.typeId])
    return HeaderInjectableTransformer[T.typeId][H.typeId];
    HeaderInjectableTransformer[T.typeId] = {
    };
  HeaderInjectableTransformer[T.typeId][H.typeId] = function Sunlight__Framework__Observables__HeaderInjectableTransformer$2() {
  };
  HeaderInjectableTransformer$2_$T_x_H$_ = HeaderInjectableTransformer[T.typeId][H.typeId];
  HeaderInjectableTransformer$2_$T_x_H$_.genericParameters = [T, H];
  HeaderInjectableTransformer$2_$T_x_H$_.genericClosure = HeaderInjectableTransformer;
  HeaderInjectableTransformer$2_$T_x_H$_.typeId = "ic$" + T.typeId + "_" + H.typeId + "$";
  HeaderInjectableTransformer$2_$T_x_H$_.__ctor = function Sunlight_Framework_Observables_HeaderInjectableTransformer$2_factory(headerDelegate, inputCollection) {
    var this_;
    this_ = new HeaderInjectableTransformer$2_$T_x_H$_();
    this_.__ctor(headerDelegate, inputCollection);
    return this_;
  };
  ptyp_ = HeaderInjectableTransformer$2_$T_x_H$_.prototype;
  ptyp_._inputCollection = null;
  ptyp_._transformedCollection = null;
  ptyp_._allHeaderIndexes = null;
  ptyp_._headerDelegate = null;
  ptyp_.__ctor = function HeaderInjectableTransformer$2____ctor(headerDelegate, inputCollection) {
    this._inputCollection = null;
    this._transformedCollection = ObservableCollection$1_$InjectedElement$2_$T_x_H$_$_.defaultConstructor();
    this._allHeaderIndexes = List_$Int32$_.defaultConstructor(); {
      this._headerDelegate = headerDelegate;
      this.set_inputCollection(inputCollection);
    }
  };
  ptyp_.set_inputCollection = function HeaderInjectableTransformer$2__set_InputCollection(value) {
    var inputCollection, newLength, oldLength, lengthDifference;
    if (this._inputCollection == value)
      return;
    if (!!this._inputCollection)
      this._inputCollection.remove_CollectionChanged(Delegate__Create("onSourceChanged", this));
    else
      this._inputCollection = ObservableCollection$1_$T$_.defaultConstructor();
    inputCollection = this._inputCollection;
    this._inputCollection = value;
    if (!!this._inputCollection)
      this._inputCollection.add_CollectionChanged(Delegate__Create("onSourceChanged", this));
    newLength = !!value ? value.get_count() : 0;
    oldLength = !!inputCollection ? inputCollection.get_count() : 0;
    lengthDifference = newLength - oldLength;
    if (lengthDifference < 0) {
      this.removeAfterKItems(inputCollection, newLength, -lengthDifference);
      this.replaceFirstKItems(value, newLength);
    }
    else {
      this.replaceFirstKItems(value, oldLength);
      this.insertAfterFirstKItems(value, oldLength, lengthDifference);
    }
    return;
  };
  ptyp_.get_transformedCollection = function HeaderInjectableTransformer$2__get_TransformedCollection() {
    return this._transformedCollection;
  };
  ptyp_.onSourceChanged = function HeaderInjectableTransformer$2__OnSourceChanged(obj1, obj2) {
    if (obj2.get_action() == 0)
      this.insertElements(obj2.get_newItems(), obj2.get_changeIndex());
    else if (obj2.get_action() == 1)
      this.removeElements(obj2.get_oldItems(), obj2.get_changeIndex());
    else if (obj2.get_action() == 2)
      this.replaceElements(obj2.get_newItems(), obj2.get_changeIndex());
    return;
  };
  ptyp_.insertElements = function HeaderInjectableTransformer$2__InsertElements(changeList, changeIndex) {
    var leftNeighbourTuple, leftElementIndex, leftHeaderCount, rightNeighbourTuple, rightElementIndex, rightHeaderCount, boundaryElementIndex, boundaryHeaderIndex, boundaryElementCount, boundaryHeaderCount, wrappedList, addedBefore, addedAfter, showFirstHead, tupleReturn, elementsToAdd, headersToAdd;
    leftNeighbourTuple = this.getTransformedIndexes(changeIndex - 1);
    leftElementIndex = TransformedIndexTuple_$T_x_H$_.get_elementIndex(leftNeighbourTuple);
    leftHeaderCount = TransformedIndexTuple_$T_x_H$_.get_headerCount(leftNeighbourTuple);
    rightNeighbourTuple = this.getTransformedIndexes(changeIndex);
    rightElementIndex = TransformedIndexTuple_$T_x_H$_.get_elementIndex(rightNeighbourTuple);
    rightHeaderCount = TransformedIndexTuple_$T_x_H$_.get_headerCount(rightNeighbourTuple);
    boundaryElementIndex = leftElementIndex + 1;
    boundaryHeaderIndex = leftHeaderCount;
    boundaryElementCount = rightHeaderCount - leftHeaderCount;
    boundaryHeaderCount = rightHeaderCount - leftHeaderCount;
    if (leftHeaderCount == rightHeaderCount && boundaryElementIndex < this._transformedCollection.get_count()) {
      this._transformedCollection.insertRangeAt(boundaryElementIndex, ElementsTuple_$T_x_H$_.get_items(this.getInsertedHeaderList(changeList, false, true, false, false, 0)));
      this.moveHeaderIndexes(boundaryHeaderIndex, boundaryElementIndex, changeList.V_get_Count_e());
      return;
    }
    wrappedList = this.wrapNeighbours(changeList, changeIndex, true, {
      read: function() {
        return addedBefore;
      },
      write: function(arg0) {
        return addedBefore = arg0;
      }
    }, {
      read: function() {
        return addedAfter;
      },
      write: function(arg0) {
        return addedAfter = arg0;
      }
    });
    showFirstHead = changeIndex == 0;
    tupleReturn = this.getInsertedHeaderList(wrappedList, showFirstHead, false, addedBefore, addedAfter, boundaryElementIndex);
    elementsToAdd = ElementsTuple_$T_x_H$_.get_items(tupleReturn);
    headersToAdd = ElementsTuple_$T_x_H$_.get_headerIndexes(tupleReturn);
    this.removeElementsAndHeaderIndexes(boundaryElementIndex, boundaryHeaderIndex, boundaryElementCount, boundaryHeaderCount);
    this.moveHeaderIndexes(boundaryHeaderIndex, boundaryElementIndex, elementsToAdd.get_count() - boundaryElementCount);
    this._transformedCollection.insertRangeAt(boundaryElementIndex, elementsToAdd);
    this._allHeaderIndexes.insertRange(boundaryHeaderIndex, headersToAdd);
    return;
  };
  ptyp_.removeElements = function HeaderInjectableTransformer$2__RemoveElements(changeList, changeIndex) {
    var leftNeighbourTuple, leftElementIndex, leftHeaderCount, rightNeighbourTuple, rightElementIndex, rightHeaderCount, boundaryElementIndex, boundaryHeaderIndex, boundaryElementCount, boundaryHeaderCount, wrappedList, addedBefore, addedAfter, showFirstHead, tupleReturn, elementsToAdd, headersToAdd;
    leftNeighbourTuple = this.getTransformedIndexes(changeIndex - 1);
    leftElementIndex = TransformedIndexTuple_$T_x_H$_.get_elementIndex(leftNeighbourTuple);
    leftHeaderCount = TransformedIndexTuple_$T_x_H$_.get_headerCount(leftNeighbourTuple);
    rightNeighbourTuple = this.getTransformedIndexes(changeIndex + changeList.V_get_Count_e());
    rightElementIndex = TransformedIndexTuple_$T_x_H$_.get_elementIndex(rightNeighbourTuple);
    rightHeaderCount = TransformedIndexTuple_$T_x_H$_.get_headerCount(rightNeighbourTuple);
    boundaryElementIndex = leftElementIndex + 1;
    boundaryHeaderIndex = leftHeaderCount;
    boundaryElementCount = rightElementIndex - leftElementIndex - 1;
    boundaryHeaderCount = rightHeaderCount - leftHeaderCount;
    if (boundaryElementCount == this._transformedCollection.get_count()) {
      this._transformedCollection.removeRangeAt(0, this._transformedCollection.get_count());
      this._allHeaderIndexes.clear();
      return;
    }
    if (leftHeaderCount == rightHeaderCount && boundaryElementIndex < this._transformedCollection.get_count()) {
      this._transformedCollection.removeRangeAt(boundaryElementIndex, changeList.V_get_Count_e());
      this.moveHeaderIndexes(boundaryHeaderIndex, boundaryElementIndex, -changeList.V_get_Count_e());
      return;
    }
    wrappedList = this.wrapNeighbours(changeList, changeIndex, false, {
      read: function() {
        return addedBefore;
      },
      write: function(arg0) {
        return addedBefore = arg0;
      }
    }, {
      read: function() {
        return addedAfter;
      },
      write: function(arg0) {
        return addedAfter = arg0;
      }
    });
    showFirstHead = changeIndex == 0;
    tupleReturn = this.getInsertedHeaderList(wrappedList, showFirstHead, false, addedBefore, addedAfter, boundaryElementIndex);
    elementsToAdd = ElementsTuple_$T_x_H$_.get_items(tupleReturn);
    headersToAdd = ElementsTuple_$T_x_H$_.get_headerIndexes(tupleReturn);
    this.removeElementsAndHeaderIndexes(boundaryElementIndex, boundaryHeaderIndex, boundaryElementCount, boundaryHeaderCount);
    this.moveHeaderIndexes(boundaryHeaderIndex, boundaryElementIndex, elementsToAdd.get_count() - boundaryElementCount);
    this.insertElementsAndHeaderIndexes(boundaryElementIndex, boundaryHeaderIndex, elementsToAdd, headersToAdd);
    return;
  };
  ptyp_.replaceElements = function HeaderInjectableTransformer$2__ReplaceElements(changeList, changeIndex) {
    var leftNeighbourTuple, leftElementIndex, leftHeaderCount, rightNeighbourTuple, rightElementIndex, rightHeaderCount, boundaryElementIndex, boundaryHeaderIndex, boundaryElementCount, boundaryHeaderCount, wrappedList, addedBefore, addedAfter, showFirstHead, tupleReturn, elementsToAdd, headersToAdd;
    leftNeighbourTuple = this.getTransformedIndexes(changeIndex - 1);
    leftElementIndex = TransformedIndexTuple_$T_x_H$_.get_elementIndex(leftNeighbourTuple);
    leftHeaderCount = TransformedIndexTuple_$T_x_H$_.get_headerCount(leftNeighbourTuple);
    rightNeighbourTuple = this.getTransformedIndexes(changeIndex + changeList.V_get_Count_e());
    rightElementIndex = TransformedIndexTuple_$T_x_H$_.get_elementIndex(rightNeighbourTuple);
    rightHeaderCount = TransformedIndexTuple_$T_x_H$_.get_headerCount(rightNeighbourTuple);
    boundaryElementIndex = leftElementIndex + 1;
    boundaryHeaderIndex = leftHeaderCount;
    boundaryElementCount = rightElementIndex - leftElementIndex - 1;
    boundaryHeaderCount = rightHeaderCount - leftHeaderCount;
    if (leftHeaderCount == rightHeaderCount && boundaryElementIndex < this._transformedCollection.get_count() - 1) {
      this._transformedCollection.replaceRangeAt(boundaryElementIndex, ElementsTuple_$T_x_H$_.get_items(this.getInsertedHeaderList(changeList, false, true, false, false, 0)));
      return;
    }
    wrappedList = this.wrapNeighbours(changeList, changeIndex, true, {
      read: function() {
        return addedBefore;
      },
      write: function(arg0) {
        return addedBefore = arg0;
      }
    }, {
      read: function() {
        return addedAfter;
      },
      write: function(arg0) {
        return addedAfter = arg0;
      }
    });
    showFirstHead = changeIndex == 0;
    tupleReturn = this.getInsertedHeaderList(wrappedList, showFirstHead, false, addedBefore, addedAfter, boundaryElementIndex);
    elementsToAdd = ElementsTuple_$T_x_H$_.get_items(tupleReturn);
    headersToAdd = ElementsTuple_$T_x_H$_.get_headerIndexes(tupleReturn);
    this.removeElementsAndHeaderIndexes(boundaryElementIndex, boundaryHeaderIndex, boundaryElementCount, boundaryHeaderCount);
    this._transformedCollection.insertRangeAt(boundaryElementIndex, elementsToAdd);
    this.moveHeaderIndexes(boundaryHeaderIndex, boundaryElementIndex, elementsToAdd.get_count() - boundaryElementCount);
    this._allHeaderIndexes.insertRange(boundaryHeaderIndex, headersToAdd);
    return;
  };
  ptyp_.replaceFirstKItems = function HeaderInjectableTransformer$2__ReplaceFirstKItems(collection, k) {
    var elementsToReplace;
    if (k == 0 || !collection)
      return;
    else {
      elementsToReplace = collection.getRangeAt(0, k);
      this.replaceElements(elementsToReplace, 0);
    }
  };
  ptyp_.insertAfterFirstKItems = function HeaderInjectableTransformer$2__InsertAfterFirstKItems(collection, k, count) {
    var elementsToInsert;
    if (count == 0 || !collection)
      return;
    else {
      elementsToInsert = collection.getRangeAt(k, count);
      this.insertElements(elementsToInsert, k);
    }
  };
  ptyp_.removeAfterKItems = function HeaderInjectableTransformer$2__RemoveAfterKItems(collection, k, count) {
    var elementsToRemove;
    if (count == 0 || !collection)
      return;
    else {
      elementsToRemove = collection.getRangeAt(k, count);
      this.removeElements(elementsToRemove, k);
    }
  };
  ptyp_.wrapNeighbours = function HeaderInjectableTransformer$2__WrapNeighbours(changeList, changeIdx, addChangeList, addedBefore, addedAfter) {
    var wrappedList, endIndex;
    wrappedList = List$1_$T$_.defaultConstructor();
    addedBefore.write(changeIdx > 0);
    if (addedBefore.read())
      wrappedList.add(this._inputCollection.get_item(changeIdx - 1));
    if (addChangeList)
      wrappedList.addRange(changeList);
    endIndex = addChangeList ? changeIdx + changeList.V_get_Count_e() : changeIdx;
    addedAfter.write(endIndex < this._inputCollection.get_count());
    if (addedAfter.read())
      wrappedList.add(this._inputCollection.get_item(endIndex));
    return wrappedList;
  };
  ptyp_.getInsertedHeaderList = function HeaderInjectableTransformer$2__GetInsertedHeaderList(items, showFirstHead, skipCompare, addedBefore, addedAfter, insertionCount) {
    var rv, headerIdx, idx, item, header, headerEntry, eachItem;
    rv = List$1_$InjectedElement$2_$T_x_H$_$_.defaultConstructor();
    headerIdx = List_$Int32$_.defaultConstructor();
    for (idx = 0; idx < items.V_get_Count_e(); idx++) {
      item = items["V_get_Item_" + IList$1_$T$_.typeId](idx);
      if (idx != 0) {
        if (!skipCompare) {
          header = this._headerDelegate(items["V_get_Item_" + IList$1_$T$_.typeId](idx - 1), item);
          if (header !== Type__GetDefaultValueStatic(H)) {
            headerEntry = InjectedElement$2_$T_x_H$_.defaultConstructor();
            headerEntry.set_header(header);
            rv.add(headerEntry);
            headerIdx.add(insertionCount);
            insertionCount++;
          }
        }
        if (!(addedAfter && idx == items.V_get_Count_e() - 1)) {
          eachItem = InjectedElement$2_$T_x_H$_.defaultConstructor();
          eachItem.set_item(item);
          insertionCount++;
          rv.add(eachItem);
        }
      }
      else if (!addedBefore || showFirstHead) {
        if (!skipCompare) {
          header = this._headerDelegate(Type__GetDefaultValueStatic(T), item);
          if (header !== Type__GetDefaultValueStatic(H)) {
            headerEntry = InjectedElement$2_$T_x_H$_.defaultConstructor();
            headerEntry.set_header(header);
            rv.add(headerEntry);
            headerIdx.add(insertionCount);
            insertionCount++;
          }
        }
        eachItem = InjectedElement$2_$T_x_H$_.defaultConstructor();
        eachItem.set_item(item);
        rv.add(eachItem);
        insertionCount++;
      }
    }
    return ElementsTuple_$T_x_H$_.__ctor(rv, headerIdx);
  };
  ptyp_.getTransformedIndexes = function HeaderInjectableTransformer$2__GetTransformedIndexes(inIndex) {
    var elementIdx, headerCount, headersExhausted, idx, headIdx;
    elementIdx = 0;
    headerCount = 0;
    headersExhausted = true;
    for (idx = 0; idx < this._allHeaderIndexes.get_count(); idx++) {
      headIdx = this._allHeaderIndexes.get_item(idx);
      elementIdx = inIndex + idx;
      if (elementIdx < headIdx) {
        headerCount = idx;
        headersExhausted = false;
        break;
      }
    }
    if (headersExhausted) {
      headerCount = this._allHeaderIndexes.get_count();
      elementIdx = inIndex + headerCount;
    }
    return TransformedIndexTuple_$T_x_H$_.__ctor(elementIdx, headerCount);
  };
  ptyp_.moveHeaderIndexes = function HeaderInjectableTransformer$2__MoveHeaderIndexes(offset, insertionIndex, itemsInsertedCount) {
    var hid;
    if (this._allHeaderIndexes.get_count() == 0 || insertionIndex >= this._allHeaderIndexes.get_item(this._allHeaderIndexes.get_count() - 1))
      return;
    for (hid = offset; hid < this._allHeaderIndexes.get_count(); hid++)
      this._allHeaderIndexes.set_item(hid, this._allHeaderIndexes.get_item(hid) + itemsInsertedCount);
  };
  ptyp_.removeElementsAndHeaderIndexes = function HeaderInjectableTransformer$2__RemoveElementsAndHeaderIndexes(startElementIndex, startHeaderIndex, elementcount, headerCount) {
    if (startElementIndex == this._transformedCollection.get_count() || elementcount == 0)
      return;
    else {
      this._transformedCollection.removeRangeAt(startElementIndex, elementcount);
      this._allHeaderIndexes.removeRangeAt(startHeaderIndex, headerCount);
    }
  };
  ptyp_.insertElementsAndHeaderIndexes = function HeaderInjectableTransformer$2__InsertElementsAndHeaderIndexes(startElementIndex, startHeaderIndex, elementsToAdd, headersToAdd) {
    if (startElementIndex == this._transformedCollection.get_count() || elementsToAdd.get_count() == 0 || headersToAdd.get_count() == 0)
      return;
    else {
      this._transformedCollection.insertRangeAt(startElementIndex, elementsToAdd);
      this._allHeaderIndexes.insertRange(startHeaderIndex, headersToAdd);
    }
  };
  Type__RegisterReferenceType(HeaderInjectableTransformer$2_$T_x_H$_, "Sunlight.Framework.Observables.HeaderInjectableTransformer`2<" + T.fullName + "," + H.fullName + ">", Object, []);
  HeaderInjectableTransformer$2_$T_x_H$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    ObservableCollection$1_$InjectedElement$2_$T_x_H$_$_ = ObservableCollection(InjectedElement(T, H, true), true);
    ObservableCollection$1_$T$_ = ObservableCollection(T, true);
    TransformedIndexTuple_$T_x_H$_ = TransformedIndexTuple(T, H, true);
    ElementsTuple_$T_x_H$_ = ElementsTuple(T, H, true);
    List$1_$InjectedElement$2_$T_x_H$_$_ = List(InjectedElement(T, H, true), true);
    IList$1_$T$_ = ILista(T, true);
    H = H;
    InjectedElement$2_$T_x_H$_ = InjectedElement(T, H, true);
    T = T;
    List$1_$T$_ = List(T, true);
    HeaderInjectableTransformer$2_$T_x_H$_ = HeaderInjectableTransformer(T, H, true);
  };
  if (_callStatiConstructor)
    HeaderInjectableTransformer$2_$T_x_H$_._tri();
  return HeaderInjectableTransformer$2_$T_x_H$_;
};
function ObservableCollection(T, _callStatiConstructor) {
  var List$1_$T$_, ArrayG$1_$T$_, CollectionChangedEventArgs$1_$T$_, IList$1_$T$_, ObservableCollection$1_$T$_, __initTracker, __initTrackera;
  if (ObservableCollection[T.typeId])
    return ObservableCollection[T.typeId];
  ObservableCollection[T.typeId] = function Sunlight__Framework__Observables__ObservableCollection$1() {
  };
  ObservableCollection$1_$T$_ = ObservableCollection[T.typeId];
  ObservableCollection$1_$T$_.genericParameters = [T];
  ObservableCollection$1_$T$_.genericClosure = ObservableCollection;
  ObservableCollection$1_$T$_.typeId = "jc$" + T.typeId + "$";
  ObservableCollection$1_$T$_.defaultConstructor = function Sunlight_Framework_Observables_ObservableCollection$1_factory() {
    var this_;
    this_ = new ObservableCollection$1_$T$_();
    this_.__ctora();
    return this_;
  };
  ptyp_ = new ObservableObject();
  ObservableCollection$1_$T$_.prototype = ptyp_;
  ptyp_.busy = false;
  ptyp_.items = null;
  ptyp_.collectionChanged = null;
  ptyp_.__ctora = function ObservableCollection$1____ctor() {
    this.__ctor();
    this.items = List$1_$T$_.defaultConstructor();
  };
  ptyp_.add_CollectionChanged = function ObservableCollection$1__add_CollectionChanged(value) {
    this.collectionChanged = Delegate__Combine(this.collectionChanged, value);
  };
  ptyp_.remove_CollectionChanged = function ObservableCollection$1__remove_CollectionChanged(value) {
    this.collectionChanged = Delegate__Remove(this.collectionChanged, value);
  };
  ptyp_.get_count = function ObservableCollection$1__get_Count() {
    return this.items.get_count();
  };
  ptyp_.get_item = function ObservableCollection$1__get_Item(index) {
    return this.items.get_item(index);
  };
  ptyp_.insertRangeAt = function ObservableCollection$1__InsertRangeAt(insertIndex, itemsToAdd) {
    ExceptionHelpers__ThrowOnArgumentNull(itemsToAdd, "itemsToAdd");
    ExceptionHelpers__ThrowOnOutOfRange(insertIndex, 0, this.get_count(), "insertIndex");
    if (insertIndex == this.items.get_count()) {
      this.addRange(itemsToAdd);
      return;
    }
    this.checkReentrancy();
    this.items.insertRangea(insertIndex, itemsToAdd);
    this.onCollectionChanged(0, insertIndex, itemsToAdd, null);
    this.firePropertyChanged("Count");
  };
  ptyp_.add = function ObservableCollection$1__Add(o) {
    this.checkReentrancy();
    this.items.add(o);
    this.onCollectionChanged(0, this.get_count() - 1, ArrayG$1_$T$_.__ctor([o]), null);
    this.firePropertyChanged("Count");
  };
  ptyp_.addRange = function ObservableCollection$1__AddRange(objArray) {
    this.checkReentrancy();
    this.items.addRange(objArray);
    this.onCollectionChanged(0, this.get_count() - objArray.V_get_Count_e(), objArray, null);
    this.firePropertyChanged("Count");
  };
  ptyp_.clear = function ObservableCollection$1__Clear() {
    var backupItems;
    if (this.get_count() == 0)
      return;
    this.checkReentrancy();
    backupItems = this.items.toArray();
    this.items.clear();
    this.onCollectionChanged(1, 0, null, backupItems);
    this.firePropertyChanged("Count");
  };
  ptyp_.removeRangeAt = function ObservableCollection$1__RemoveRangeAt(removeIndex, count) {
    var itemsToRemove, index;
    ExceptionHelpers__ThrowOnOutOfRange(count, 1, this.items.get_count(), "count");
    ExceptionHelpers__ThrowOnOutOfRange(removeIndex, 0, this.items.get_count() - count, "removeIndex");
    this.checkReentrancy();
    itemsToRemove = List$1_$T$_.defaultConstructor();
    for (index = count - 1; index >= 0; index--) {
      itemsToRemove.add(this.items.get_item(removeIndex));
      this.items.removeAt(removeIndex);
    }
    this.onCollectionChanged(1, removeIndex, null, itemsToRemove);
    this.firePropertyChanged("Count");
  };
  ptyp_.replaceRangeAt = function ObservableCollection$1__ReplaceRangeAt(replaceIndex, list) {
    var oldItems, index, value, oldItem;
    ExceptionHelpers__ThrowOnArgumentNull(list, "replacedItems");
    ExceptionHelpers__ThrowOnOutOfRange(replaceIndex + list.V_get_Count_e() - 1, 0, this.get_count() - 1, "index");
    oldItems = List$1_$T$_.defaultConstructor();
    for (index = 0; index < list.V_get_Count_e(); index++) {
      value = list["V_get_Item_" + IList$1_$T$_.typeId](index);
      if (!Object__Equals(this.items.get_item(replaceIndex + index), value)) {
        oldItem = this.items.get_item(replaceIndex + index);
        this.items.set_item(replaceIndex + index, value);
        oldItems.add(oldItem);
      }
    }
    if (oldItems.get_count() != 0)
      this.onCollectionChanged(2, replaceIndex, list, oldItems);
  };
  ptyp_.getRangeAt = function ObservableCollection$1__GetRangeAt(index, count) {
    var rv, idx;
    ExceptionHelpers__ThrowOnOutOfRange(index, 0, this.get_count() - 1, "first index");
    ExceptionHelpers__ThrowOnOutOfRange(count, 1, this.get_count(), "count");
    ExceptionHelpers__ThrowOnOutOfRange(index + count - 1, 0, this.get_count() - 1, "last index");
    rv = List$1_$T$_.defaultConstructor();
    for (idx = index; idx < this.get_count(); idx++)
      if (idx < index + count)
        rv.add(this.items.get_item(idx));
    return rv;
  };
  ptyp_.checkReentrancy = function ObservableCollection$1__CheckReentrancy() {
    if (this.busy)
      throw Error.createError("InvalidOperationException", null);
  };
  ptyp_.onCollectionChanged = function ObservableCollection$1__OnCollectionChanged(action, index, newItems, oldItems) {
    var collectionChangedArgs;
    if (!!this.collectionChanged) {
      this.busy = true;
      try {
        collectionChangedArgs = CollectionChangedEventArgs$1_$T$_.__ctor(action, index, newItems, oldItems);
        this.collectionChanged(this, collectionChangedArgs);
      } finally {
        this.busy = false;
      }
    }
  };
  ptyp_.sunlight__Framework__Observables__IObservableCollection__get_Item = function ObservableCollection$1__Sunlight__Framework__Observables__IObservableCollection__get_Item(index) {
    return Type__BoxTypeInstance(T, this.get_item(index));
  };
  ptyp_.sunlight__Framework__Observables__INotifyCollectionChanged__add_CollectionChanged = function ObservableCollection$1__Sunlight__Framework__Observables__INotifyCollectionChanged__add_CollectionChanged(value) {
    this.add_CollectionChanged(value);
  };
  ptyp_.sunlight__Framework__Observables__INotifyCollectionChanged__remove_CollectionChanged = function ObservableCollection$1__Sunlight__Framework__Observables__INotifyCollectionChanged__remove_CollectionChanged(value) {
    this.remove_CollectionChanged(value);
  };
  ptyp_.V_get_Item_c = ptyp_.sunlight__Framework__Observables__IObservableCollection__get_Item;
  ptyp_.V_add_CollectionChanged_d = ptyp_.sunlight__Framework__Observables__INotifyCollectionChanged__add_CollectionChanged;
  ptyp_.V_remove_CollectionChanged_d = ptyp_.sunlight__Framework__Observables__INotifyCollectionChanged__remove_CollectionChanged;
  ptyp_.V_get_Count_c = ptyp_.get_count;
  Type__RegisterReferenceType(ObservableCollection$1_$T$_, "Sunlight.Framework.Observables.ObservableCollection`1<" + T.fullName + ">", ObservableObject, [IObservableCollection, INotifyCollectionChanged, INotifyPropertyChanged]);
  ObservableCollection$1_$T$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    List$1_$T$_ = List(T, true);
    ArrayG$1_$T$_ = ArrayG(T, true);
    CollectionChangedEventArgs$1_$T$_ = CollectionChangedEventArgsa(T, true);
    IList$1_$T$_ = ILista(T, true);
    T = T;
    ObservableCollection$1_$T$_ = ObservableCollection(T, true);
  };
  if (_callStatiConstructor)
    ObservableCollection$1_$T$_._tri();
  return ObservableCollection$1_$T$_;
};
function INotifyPropertyChanged() {
};
INotifyPropertyChanged.typeId = "b";
Type__RegisterInterface(INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function NumberDictionary(TValue, _callStatiConstructor) {
  var Enumerator_$TValue$_, NumberDictionary$1_$TValue$_, KeyValuePair$2_$Number_x_TValue$_, IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_, __initTracker, __initTrackera;
  if (NumberDictionary[TValue.typeId])
    return NumberDictionary[TValue.typeId];
  NumberDictionary[TValue.typeId] = function System__Collections__Generic__NumberDictionary$1() {
  };
  NumberDictionary$1_$TValue$_ = NumberDictionary[TValue.typeId];
  NumberDictionary$1_$TValue$_.genericParameters = [TValue];
  NumberDictionary$1_$TValue$_.genericClosure = NumberDictionary;
  NumberDictionary$1_$TValue$_.typeId = "kc$" + TValue.typeId + "$";
  KeyValuePair$2_$Number_x_TValue$_ = KeyValuePair(Number, TValue, _callStatiConstructor);
  KeyValuePair$2_$Number_x_TValue$_ = KeyValuePair(Number, TValue, _callStatiConstructor);
  IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_ = IEnumerablea(KeyValuePair(Number, TValue, _callStatiConstructor), _callStatiConstructor);
  NumberDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_NumberDictionary$1_factory() {
    var this_;
    this_ = new NumberDictionary$1_$TValue$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = NumberDictionary$1_$TValue$_.prototype;
  ptyp_.innerDict = null;
  ptyp_.count = 0;
  ptyp_.__ctor = function NumberDictionary$1____ctor() {
    this.count = 0;
    this.innerDict = {
    };
  };
  ptyp_.get_item = function NumberDictionary$1__get_Item(index) {
    if (!(index in this.innerDict))
      throw new Error("Key not found");
    return this.innerDict[index];
  };
  ptyp_.set_item = function NumberDictionary$1__set_Item(index, value) {
    this.innerDict[index] = value;
  };
  ptyp_.get_keys = function NumberDictionary$1__get_Keys() {
    return ArrayG_$Number$_.__ctor(this.getKeys());
  };
  ptyp_.get_count = function NumberDictionary$1__get_Count() {
    return this.count;
  };
  ptyp_.add = function NumberDictionary$1__Add(key, value) {
    if (this.containsKey(key))
      throw new Error("Key already exists");
    this.count++;
    this.set_item(key, value);
  };
  ptyp_.containsKey = function NumberDictionary$1__ContainsKey(key) {
    return key in this.innerDict;
  };
  ptyp_.remove = function NumberDictionary$1__Remove(key) {
    var rv;
    rv = delete this.innerDict[key];
    if (rv)
      this.count--;
    return rv;
  };
  ptyp_.copyTo = function NumberDictionary$1__CopyTo(array, index) {
    var keys, i;
    keys = Type__CastType(ArrayG_$Number$_, this.get_keys());
    for (i = 0; i < keys.V_get_Length(); i++)
      array.setValue(i + index, Type__BoxTypeInstance(KeyValuePair$2_$Number_x_TValue$_, KeyValuePair$2_$Number_x_TValue$_.__ctor(keys.get_item(i), this.get_item(keys.get_item(i)))));
  };
  ptyp_.getEnumerator = function NumberDictionary$1__GetEnumerator() {
    return Enumerator_$TValue$_.__ctor(this);
  };
  ptyp_.system__Collections__IEnumerable__GetEnumerator = function NumberDictionary$1__System__Collections__IEnumerable__GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.getKeys = function NumberDictionary$1__GetKeys() {
    var rv, key;
    rv = [];
    for (key in this.innerDict)
      rv.push(key);
    return rv;
  };
  ptyp_.V_GetEnumerator_k = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_.V_get_Count_e = ptyp_.get_count;
  ptyp_.V_CopyTo_e = ptyp_.copyTo;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_.typeId] = ptyp_.getEnumerator;
  Type__RegisterReferenceType(NumberDictionary$1_$TValue$_, "System.Collections.Generic.NumberDictionary`1<" + TValue.fullName + ">", Object, [ICollection, IEnumerable, IEnumerable$1_$KeyValuePair$2_$Number_x_TValue$_$_]);
  NumberDictionary$1_$TValue$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    Enumerator_$TValue$_ = Enumeratora(TValue, true);
    TValue = TValue;
    NumberDictionary$1_$TValue$_ = NumberDictionary(TValue, true);
  };
  if (_callStatiConstructor)
    NumberDictionary$1_$TValue$_._tri();
  return NumberDictionary$1_$TValue$_;
};
function Queue(T, _callStatiConstructor) {
  var QueueEnumerator_$T$_, Queue$1_$T$_, IEnumerable$1_$T$_, __initTracker, __initTrackera;
  if (Queue[T.typeId])
    return Queue[T.typeId];
  Queue[T.typeId] = function System__Collections__Generic__Queue$1() {
  };
  Queue$1_$T$_ = Queue[T.typeId];
  Queue$1_$T$_.genericParameters = [T];
  Queue$1_$T$_.genericClosure = Queue;
  Queue$1_$T$_.typeId = "lc$" + T.typeId + "$";
  IEnumerable$1_$T$_ = IEnumerablea(T, _callStatiConstructor);
  Queue$1_$T$_.defaultConstructor = function System_Collections_Generic_Queue$1_factory() {
    var this_;
    this_ = new Queue$1_$T$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = Queue$1_$T$_.prototype;
  ptyp_.nativeArray = null;
  ptyp_.dequeue = function Queue$1__Dequeue() {
    var rv;
    if (this.get_count() > 0) {
      rv = this.nativeArray[0];
      NativeArray$1__RemoveAt(this.nativeArray, 0);
      return rv;
    }
    throw new Error("No elements in stack");
  };
  ptyp_.enqueue = function Queue$1__Enqueue(item) {
    NativeArray$1__InsertAt(this.nativeArray, this.nativeArray.length, item);
  };
  ptyp_.get_count = function Queue$1__get_Count() {
    return this.nativeArray.length;
  };
  ptyp_.getEnumerator = function Queue$1__GetEnumerator() {
    return QueueEnumerator_$T$_.__ctor(this);
  };
  ptyp_.system__Collections__IEnumerable__GetEnumerator = function Queue$1__System__Collections__IEnumerable__GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.__ctor = function Queue$1____ctor() {
    this.nativeArray = new Array(0);
  };
  ptyp_.V_GetEnumerator_k = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.getEnumerator;
  Type__RegisterReferenceType(Queue$1_$T$_, "System.Collections.Generic.Queue`1<" + T.fullName + ">", Object, [IEnumerable$1_$T$_, IEnumerable]);
  Queue$1_$T$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    QueueEnumerator_$T$_ = QueueEnumerator(T, true);
    T = T;
    Queue$1_$T$_ = Queue(T, true);
  };
  if (_callStatiConstructor)
    Queue$1_$T$_._tri();
  return Queue$1_$T$_;
};
function List(T, _callStatiConstructor) {
  var ArrayG$1_$T$_, T$5b$5d_$T$_, List$1_$T$_, ListEnumerator$1_$T$_, IList$1_$T$_, IEnumerable$1_$T$_, __initTracker, __initTrackera;
  if (List[T.typeId])
    return List[T.typeId];
  List[T.typeId] = function System__Collections__Generic__List$1() {
  };
  List$1_$T$_ = List[T.typeId];
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = List;
  List$1_$T$_.typeId = "mc$" + T.typeId + "$";
  IList$1_$T$_ = ILista(T, _callStatiConstructor);
  IEnumerable$1_$T$_ = IEnumerablea(T, _callStatiConstructor);
  List$1_$T$_.defaultConstructor = function System_Collections_Generic_List$1_factory() {
    var this_;
    this_ = new List$1_$T$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = List$1_$T$_.prototype;
  ptyp_.nativeArray = null;
  ptyp_.__ctor = function List$1____ctor() {
    this.nativeArray = new Array(0);
  };
  ptyp_.get_item = function List$1__get_Item(index) {
    var arr;
    arr = this.nativeArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index];
  };
  ptyp_.set_item = function List$1__set_Item(index, value) {
    var arr;
    arr = this.nativeArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index] = value;
  };
  ptyp_.system__Collections__IList__IndexOf = function List$1__System__Collections__IList__IndexOf(value) {
    if (value == null && T.isInstanceOfType(value))
      return NativeArray$1__IndexOf(this.nativeArray, Type__UnBoxTypeInstance(T, value), 0);
    return -1;
  };
  ptyp_.insert = function List$1__Insert(index, item) {
    NativeArray$1__InsertAt(this.nativeArray, index, item);
  };
  ptyp_.insertRange = function List$1__InsertRange(index, items) {
    NativeArray$1__InsertRangeAt(this.nativeArray, index, items.nativeArray);
  };
  ptyp_.insertRangea = function List$1__InsertRange(index, items) {
    var stmtTemp1, item;
    for (stmtTemp1 = items.V_GetEnumerator_k(); stmtTemp1.V_MoveNext_l(); ) {
      item = Type__UnBoxTypeInstance(T, stmtTemp1.V_get_Current_l());
      NativeArray$1__InsertAt(this.nativeArray, index++, item);
    }
  };
  ptyp_.removeAt = function List$1__RemoveAt(index) {
    NativeArray$1__RemoveAt(this.nativeArray, index);
  };
  ptyp_.removeRangeAt = function List$1__RemoveRangeAt(index, count) {
    while (count-- > 0)
      NativeArray$1__RemoveAt(this.nativeArray, index);
  };
  ptyp_.getRangeAt = function List$1__GetRangeAt(index, count) {
    var rv, idx;
    rv = List$1_$T$_.defaultConstructor();
    for (idx = index; idx < this.get_count(); idx++)
      if (idx < index + count)
        rv.add(this.nativeArray[idx]);
    return rv;
  };
  ptyp_.get_count = function List$1__get_Count() {
    return this.nativeArray.length;
  };
  ptyp_.add = function List$1__Add(item) {
    this.nativeArray.push(item);
  };
  ptyp_.addRange = function List$1__AddRange(items) {
    var stmtTemp1, item;
    for (stmtTemp1 = items.V_GetEnumerator_k(); stmtTemp1.V_MoveNext_l(); ) {
      item = Type__UnBoxTypeInstance(T, stmtTemp1.V_get_Current_l());
      this.nativeArray.push(item);
    }
  };
  ptyp_.clear = function List$1__Clear() {
    this.nativeArray.length = 0;
  };
  ptyp_.system__Collections__ICollection__CopyTo = function List$1__System__Collections__ICollection__CopyTo(array, index) {
    var nativeArray, length, i;
    nativeArray = this.nativeArray;
    length = nativeArray.length;
    for (i = 0; i < length; i++)
      array.setValue(i + index, Type__BoxTypeInstance(T, nativeArray[i]));
  };
  ptyp_.toArray = function List$1__ToArray() {
    return ArrayG$1_$T$_.__ctor(this.nativeArray.slice(0, this.nativeArray.length));
  };
  ptyp_.getEnumerator = function List$1__GetEnumerator() {
    return ListEnumerator$1_$T$_.__ctor(this);
  };
  ptyp_.system__Collections__IEnumerable__GetEnumerator = function List$1__System__Collections__IEnumerable__GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.system__Collections__IList__get_Item = function List$1__System__Collections__IList__get_Item(index) {
    return Type__BoxTypeInstance(T, this.get_item(index));
  };
  ptyp_.system__Collections__IList__Contains = function List$1__System__Collections__IList__Contains(value) {
    return this.V_IndexOf_f(value) >= 0;
  };
  ptyp_.V_IndexOf_f = ptyp_.system__Collections__IList__IndexOf;
  ptyp_.V_CopyTo_e = ptyp_.system__Collections__ICollection__CopyTo;
  ptyp_.V_GetEnumerator_k = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_.V_get_Item_f = ptyp_.system__Collections__IList__get_Item;
  ptyp_.V_Contains_f = ptyp_.system__Collections__IList__Contains;
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  ptyp_["V_Insert_" + IList$1_$T$_.typeId] = ptyp_.insert;
  ptyp_.V_Clear_f = ptyp_.clear;
  ptyp_.V_RemoveAt_f = ptyp_.removeAt;
  ptyp_.V_get_Count_e = ptyp_.get_count;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.getEnumerator;
  Type__RegisterReferenceType(List$1_$T$_, "System.Collections.Generic.List`1<" + T.fullName + ">", Object, [IList$1_$T$_, IList, ICollection, IEnumerable, IEnumerable$1_$T$_]);
  List$1_$T$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    ArrayG$1_$T$_ = ArrayG(T, true);
    T$5b$5d_$T$_ = ArrayG(T, true);
    List$1_$T$_ = List(T, true);
    T = T;
    ListEnumerator$1_$T$_ = ListEnumerator(T, true);
  };
  if (_callStatiConstructor)
    List$1_$T$_._tri();
  return List$1_$T$_;
};
function InjectedElement(I, H, _callStatiConstructor) {
  var InjectedElement$2_$I_x_H$_, __initTracker;
  if (InjectedElement[I.typeId] && InjectedElement[I.typeId][H.typeId])
    return InjectedElement[I.typeId][H.typeId];
    InjectedElement[I.typeId] = {
    };
  InjectedElement[I.typeId][H.typeId] = function Sunlight__Framework__Observables__InjectedElement$2() {
  };
  InjectedElement$2_$I_x_H$_ = InjectedElement[I.typeId][H.typeId];
  InjectedElement$2_$I_x_H$_.genericParameters = [I, H];
  InjectedElement$2_$I_x_H$_.genericClosure = InjectedElement;
  InjectedElement$2_$I_x_H$_.typeId = "nc$" + I.typeId + "_" + H.typeId + "$";
  InjectedElement$2_$I_x_H$_.defaultConstructor = function Sunlight_Framework_Observables_InjectedElement$2_factory() {
    var this_;
    this_ = new InjectedElement$2_$I_x_H$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = InjectedElement$2_$I_x_H$_.prototype;
  ptyp_._header = Type__GetDefaultValueStatic(H);
  ptyp_._item = Type__GetDefaultValueStatic(I);
  ptyp_.get_header = function InjectedElement$2__get_Header() {
    return this._header;
  };
  ptyp_.set_header = function InjectedElement$2__set_Header(value) {
    this._header = value;
  };
  ptyp_.get_item = function InjectedElement$2__get_Item() {
    return this._item;
  };
  ptyp_.set_item = function InjectedElement$2__set_Item(value) {
    this._item = value;
  };
  ptyp_.sunlight__Framework__Observables__IHeaderedElement__get_IsHeader = function InjectedElement$2__Sunlight__Framework__Observables__IHeaderedElement__get_IsHeader() {
    return this.get_header() != null;
  };
  ptyp_.sunlight__Framework__Observables__IHeaderedElement__get_Header = function InjectedElement$2__Sunlight__Framework__Observables__IHeaderedElement__get_Header() {
    return this.get_header();
  };
  ptyp_.sunlight__Framework__Observables__IHeaderedElement__get_Item = function InjectedElement$2__Sunlight__Framework__Observables__IHeaderedElement__get_Item() {
    return Type__BoxTypeInstance(I, this.get_item());
  };
  ptyp_.__ctor = function InjectedElement$2____ctor() {
    this._header = Type__GetDefaultValueStatic(H);
    this._item = Type__GetDefaultValueStatic(I);
  };
  ptyp_.V_get_IsHeader_g = ptyp_.sunlight__Framework__Observables__IHeaderedElement__get_IsHeader;
  ptyp_.V_get_Header_g = ptyp_.sunlight__Framework__Observables__IHeaderedElement__get_Header;
  ptyp_.V_get_Item_g = ptyp_.sunlight__Framework__Observables__IHeaderedElement__get_Item;
  Type__RegisterReferenceType(InjectedElement$2_$I_x_H$_, "Sunlight.Framework.Observables.InjectedElement`2<" + I.fullName + "," + H.fullName + ">", Object, [IHeaderedElement]);
  InjectedElement$2_$I_x_H$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    H = H;
    I = I;
    InjectedElement$2_$I_x_H$_ = InjectedElement(I, H, true);
  };
  if (_callStatiConstructor)
    InjectedElement$2_$I_x_H$_._tri();
  return InjectedElement$2_$I_x_H$_;
};
function StringDictionary(TValue, _callStatiConstructor) {
  var Enumerator_$TValue$_, StringDictionary$1_$TValue$_, KeyValuePair$2_$String_x_TValue$_, IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_, __initTracker, __initTrackera;
  if (StringDictionary[TValue.typeId])
    return StringDictionary[TValue.typeId];
  StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$1() {
  };
  StringDictionary$1_$TValue$_ = StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "oc$" + TValue.typeId + "$";
  KeyValuePair$2_$String_x_TValue$_ = KeyValuePair(String, TValue, _callStatiConstructor);
  KeyValuePair$2_$String_x_TValue$_ = KeyValuePair(String, TValue, _callStatiConstructor);
  IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_ = IEnumerablea(KeyValuePair(String, TValue, _callStatiConstructor), _callStatiConstructor);
  StringDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_StringDictionary$1_factory() {
    var this_;
    this_ = new StringDictionary$1_$TValue$_();
    this_.__ctor();
    return this_;
  };
  StringDictionary$1_$TValue$_.__ctor = function System_Collections_Generic_StringDictionary$1_factory(innerDict) {
    var this_;
    this_ = new StringDictionary$1_$TValue$_();
    this_.__ctora(innerDict);
    return this_;
  };
  ptyp_ = StringDictionary$1_$TValue$_.prototype;
  ptyp_.innerDict = null;
  ptyp_.count = 0;
  ptyp_.__ctor = function StringDictionary$1____ctor() {
    this.count = 0;
    this.innerDict = {
    };
  };
  ptyp_.__ctora = function StringDictionary$1____ctor(innerDict) {
    this.count = 0; {
      this.innerDict = innerDict;
      this.count = this.computeCount();
    }
  };
  ptyp_.get_item = function StringDictionary$1__get_Item(index) {
    if (!(index in this.innerDict))
      throw new Error("Key not found");
    return this.innerDict[index];
  };
  ptyp_.set_item = function StringDictionary$1__set_Item(index, value) {
    if (!(index in this.innerDict))
      this.count++;
    this.innerDict[index] = value;
  };
  ptyp_.get_keys = function StringDictionary$1__get_Keys() {
    return ArrayG_$String$_.__ctor(this.getKeys());
  };
  ptyp_.get_count = function StringDictionary$1__get_Count() {
    return this.count;
  };
  ptyp_.containsKey = function StringDictionary$1__ContainsKey(key) {
    return key in this.innerDict;
  };
  ptyp_.remove = function StringDictionary$1__Remove(key) {
    var rv;
    rv = delete this.innerDict[key];
    if (rv)
      this.count--;
    return rv;
  };
  ptyp_.tryGetValue = function StringDictionary$1__TryGetValue(key, value) {
    if (this.containsKey(key)) {
      value.write(this.get_item(key));
      return true;
    }
    value.write(Type__GetDefaultValueStatic(TValue));
    return false;
  };
  ptyp_.clear = function StringDictionary$1__Clear() {
    this.innerDict = {
    };
    this.count = 0;
  };
  ptyp_.copyTo = function StringDictionary$1__CopyTo(array, index) {
    var keys, i;
    keys = Type__CastType(ArrayG_$String$_, this.get_keys());
    for (i = 0; i < keys.V_get_Length(); i++)
      array.setValue(i + index, Type__BoxTypeInstance(KeyValuePair$2_$String_x_TValue$_, KeyValuePair$2_$String_x_TValue$_.__ctor(keys.get_item(i), this.get_item(keys.get_item(i)))));
  };
  ptyp_.getEnumerator = function StringDictionary$1__GetEnumerator() {
    return Enumerator_$TValue$_.__ctor(this);
  };
  ptyp_.system__Collections__IEnumerable__GetEnumerator = function StringDictionary$1__System__Collections__IEnumerable__GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.getKeys = function StringDictionary$1__GetKeys() {
    var rv, key;
    rv = [];
    for (key in this.innerDict)
      rv.push(key);
    return rv;
  };
  ptyp_.computeCount = function StringDictionary$1__ComputeCount() {
    var rv, key;
    rv = 0;
    for (key in this.innerDict)
      rv++;
    return rv;
  };
  ptyp_.V_GetEnumerator_k = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_.V_get_Count_e = ptyp_.get_count;
  ptyp_.V_CopyTo_e = ptyp_.copyTo;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_.typeId] = ptyp_.getEnumerator;
  Type__RegisterReferenceType(StringDictionary$1_$TValue$_, "System.Collections.Generic.StringDictionary`1<" + TValue.fullName + ">", Object, [ICollection, IEnumerable, IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_]);
  StringDictionary$1_$TValue$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    TValue = TValue;
    Enumerator_$TValue$_ = Enumeratorb(TValue, true);
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
  Action[T1.typeId] = function System__Action$1() {
  };
  Action$1_$T1$_ = Action[T1.typeId];
  Action$1_$T1$_.genericParameters = [T1];
  Action$1_$T1$_.genericClosure = Action;
  Action$1_$T1$_.typeId = "pc$" + T1.typeId + "$";
  Action$1_$T1$_.prototype = new Functiona();
  Type__RegisterReferenceType(Action$1_$T1$_, "System.Action`1<" + T1.fullName + ">", Functiona, []);
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
function Actiona(T1, T2, _callStatiConstructor) {
  var Action$2_$T1_x_T2$_, __initTracker;
  if (Actiona[T1.typeId] && Actiona[T1.typeId][T2.typeId])
    return Actiona[T1.typeId][T2.typeId];
    Actiona[T1.typeId] = {
    };
  Actiona[T1.typeId][T2.typeId] = function System__Action$2() {
  };
  Action$2_$T1_x_T2$_ = Actiona[T1.typeId][T2.typeId];
  Action$2_$T1_x_T2$_.genericParameters = [T1, T2];
  Action$2_$T1_x_T2$_.genericClosure = Actiona;
  Action$2_$T1_x_T2$_.typeId = "qc$" + T1.typeId + "_" + T2.typeId + "$";
  Action$2_$T1_x_T2$_.prototype = new Functiona();
  Type__RegisterReferenceType(Action$2_$T1_x_T2$_, "System.Action`2<" + T1.fullName + "," + T2.fullName + ">", Functiona, []);
  Action$2_$T1_x_T2$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T1 = T1;
    T2 = T2;
    Action$2_$T1_x_T2$_ = Actiona(T1, T2, true);
  };
  if (_callStatiConstructor)
    Action$2_$T1_x_T2$_._tri();
  return Action$2_$T1_x_T2$_;
};
function ObservableCollectionGenerator(T, U, _callStatiConstructor) {
  var ObservableCollection$1_$U$_, List$1_$U$_, ObservableCollectionGenerator$2_$T_x_U$_, __initTracker, __initTrackera;
  if (ObservableCollectionGenerator[T.typeId] && ObservableCollectionGenerator[T.typeId][U.typeId])
    return ObservableCollectionGenerator[T.typeId][U.typeId];
    ObservableCollectionGenerator[T.typeId] = {
    };
  ObservableCollectionGenerator[T.typeId][U.typeId] = function Sunlight__Framework__Observables__ObservableCollectionGenerator$2() {
  };
  ObservableCollectionGenerator$2_$T_x_U$_ = ObservableCollectionGenerator[T.typeId][U.typeId];
  ObservableCollectionGenerator$2_$T_x_U$_.genericParameters = [T, U];
  ObservableCollectionGenerator$2_$T_x_U$_.genericClosure = ObservableCollectionGenerator;
  ObservableCollectionGenerator$2_$T_x_U$_.typeId = "rc$" + T.typeId + "_" + U.typeId + "$";
  ObservableCollectionGenerator$2_$T_x_U$_.__ctor = function Sunlight_Framework_Observables_ObservableCollectionGenerator$2_factory(transformDelegate) {
    var this_;
    this_ = new ObservableCollectionGenerator$2_$T_x_U$_();
    this_.__ctor(transformDelegate);
    return this_;
  };
  ptyp_ = ObservableCollectionGenerator$2_$T_x_U$_.prototype;
  ptyp_._transformDelegate = null;
  ptyp_._outCollection = null;
  ptyp_._inputCollection = null;
  ptyp_.__ctor = function ObservableCollectionGenerator$2____ctor(transformDelegate) {
    this._inputCollection = null; {
      this._transformDelegate = transformDelegate;
      this._outCollection = ObservableCollection$1_$U$_.defaultConstructor();
    }
  };
  ptyp_.get_inputCollection = function ObservableCollectionGenerator$2__get_InputCollection() {
    return this._inputCollection;
  };
  ptyp_.set_inputCollection = function ObservableCollectionGenerator$2__set_InputCollection(value) {
    var newLength, oldLength, lengthDifference;
    if (this._inputCollection == value)
      return;
    this._inputCollection = value;
    newLength = !!value ? value.get_count() : 0;
    oldLength = this._outCollection.get_count();
    lengthDifference = newLength - oldLength;
    if (lengthDifference < 0) {
      this.removeAfterKItems(newLength, -lengthDifference);
      this.replaceFirstKItems(value, newLength);
    }
    else {
      this.replaceFirstKItems(value, oldLength);
      this.insertAfterFirstKItems(value, oldLength, lengthDifference);
    }
    return;
  };
  ptyp_.replaceFirstKItems = function ObservableCollectionGenerator$2__ReplaceFirstKItems(collection, k) {
    var elementsToReplace;
    if (k == 0 || !collection)
      return;
    else {
      elementsToReplace = collection.getRangeAt(0, k);
      this._outCollection.replaceRangeAt(0, this.generateTransformedCollection(elementsToReplace));
      return;
    }
  };
  ptyp_.insertAfterFirstKItems = function ObservableCollectionGenerator$2__InsertAfterFirstKItems(collection, k, count) {
    var elementsToInsert;
    if (count == 0 || !collection)
      return;
    else {
      elementsToInsert = collection.getRangeAt(k, count);
      this._outCollection.insertRangeAt(k, this.generateTransformedCollection(elementsToInsert));
    }
    return;
  };
  ptyp_.removeAfterKItems = function ObservableCollectionGenerator$2__RemoveAfterKItems(k, count) {
    if (count == 0)
      return;
    else
      this._outCollection.removeRangeAt(k, count);
    return;
  };
  ptyp_.generateTransformedCollection = function ObservableCollectionGenerator$2__GenerateTransformedCollection(elements) {
    var rv, stmtTemp1, newItem, element;
    rv = List$1_$U$_.defaultConstructor();
    for (stmtTemp1 = elements.V_GetEnumerator_k(); stmtTemp1.V_MoveNext_l(); ) {
      element = Type__UnBoxTypeInstance(T, stmtTemp1.V_get_Current_l());
      newItem = this._transformDelegate(element);
      rv.add(newItem);
    }
    return rv;
  };
  ptyp_.get_outputCollection = function ObservableCollectionGenerator$2__get_OutputCollection() {
    return this._outCollection;
  };
  Type__RegisterReferenceType(ObservableCollectionGenerator$2_$T_x_U$_, "Sunlight.Framework.Observables.ObservableCollectionGenerator`2<" + T.fullName + "," + U.fullName + ">", Object, []);
  ObservableCollectionGenerator$2_$T_x_U$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    ObservableCollection$1_$U$_ = ObservableCollection(U, true);
    List$1_$U$_ = List(U, true);
    T = T;
    U = U;
    ObservableCollectionGenerator$2_$T_x_U$_ = ObservableCollectionGenerator(T, U, true);
  };
  if (_callStatiConstructor)
    ObservableCollectionGenerator$2_$T_x_U$_._tri();
  return ObservableCollectionGenerator$2_$T_x_U$_;
};
function CollectionChangedEventArgsa(T, _callStatiConstructor) {
  var CollectionChangedEventArgs$1_$T$_, __initTracker;
  if (CollectionChangedEventArgsa[T.typeId])
    return CollectionChangedEventArgsa[T.typeId];
  CollectionChangedEventArgsa[T.typeId] = function Sunlight__Framework__Observables__CollectionChangedEventArgs$1() {
  };
  CollectionChangedEventArgs$1_$T$_ = CollectionChangedEventArgsa[T.typeId];
  CollectionChangedEventArgs$1_$T$_.genericParameters = [T];
  CollectionChangedEventArgs$1_$T$_.genericClosure = CollectionChangedEventArgsa;
  CollectionChangedEventArgs$1_$T$_.typeId = "sc$" + T.typeId + "$";
  CollectionChangedEventArgs$1_$T$_.__ctor = function Sunlight_Framework_Observables_CollectionChangedEventArgs$1_factory(action, changeIndex, newItems, oldItems) {
    var this_;
    this_ = new CollectionChangedEventArgs$1_$T$_();
    this_.__ctor(action, changeIndex, newItems, oldItems);
    return this_;
  };
  ptyp_ = CollectionChangedEventArgs$1_$T$_.prototype;
  ptyp_.action = 0;
  ptyp_.changeIndex = 0;
  ptyp_.newItems = null;
  ptyp_.oldItems = null;
  ptyp_.__ctor = function CollectionChangedEventArgs$1____ctor(action, changeIndex, newItems, oldItems) { {
      this.action = action;
      this.changeIndex = changeIndex;
      switch(action) {
        case 0: {
          this.newItems = newItems;
          break;
        }
        case 1: {
          this.oldItems = oldItems;
          break;
        }
        case 2: {
          this.newItems = newItems;
          this.oldItems = oldItems;
          break;
        }
        case 4: {
          this.changeIndex = -1;
          break;
        }
      }
    }
  };
  ptyp_.get_changeIndex = function CollectionChangedEventArgs$1__get_ChangeIndex() {
    return this.changeIndex;
  };
  ptyp_.get_action = function CollectionChangedEventArgs$1__get_Action() {
    return this.action;
  };
  ptyp_.get_newItems = function CollectionChangedEventArgs$1__get_NewItems() {
    return this.newItems;
  };
  ptyp_.get_oldItems = function CollectionChangedEventArgs$1__get_OldItems() {
    return this.oldItems;
  };
  ptyp_.sunlight__Framework__Observables__CollectionChangedEventArgs__get_NewItems = function CollectionChangedEventArgs$1__Sunlight__Framework__Observables__CollectionChangedEventArgs__get_NewItems() {
    return this.get_newItems();
  };
  ptyp_.sunlight__Framework__Observables__CollectionChangedEventArgs__get_OldItems = function CollectionChangedEventArgs$1__Sunlight__Framework__Observables__CollectionChangedEventArgs__get_OldItems() {
    return this.get_oldItems();
  };
  ptyp_.V_get_NewItems_h = ptyp_.sunlight__Framework__Observables__CollectionChangedEventArgs__get_NewItems;
  ptyp_.V_get_OldItems_h = ptyp_.sunlight__Framework__Observables__CollectionChangedEventArgs__get_OldItems;
  ptyp_.V_get_ChangeIndex_h = ptyp_.get_changeIndex;
  ptyp_.V_get_Action_h = ptyp_.get_action;
  Type__RegisterReferenceType(CollectionChangedEventArgs$1_$T$_, "Sunlight.Framework.Observables.CollectionChangedEventArgs`1<" + T.fullName + ">", Object, [CollectionChangedEventArgs]);
  CollectionChangedEventArgs$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    CollectionChangedEventArgs$1_$T$_ = CollectionChangedEventArgsa(T, true);
  };
  if (_callStatiConstructor)
    CollectionChangedEventArgs$1_$T$_._tri();
  return CollectionChangedEventArgs$1_$T$_;
};
function TransformedIndexTuple(T, H, _callStatiConstructor) {
  var TransformedIndexTuple_$T_x_H$_, __initTracker;
  if (TransformedIndexTuple[T.typeId] && TransformedIndexTuple[T.typeId][H.typeId])
    return TransformedIndexTuple[T.typeId][H.typeId];
    TransformedIndexTuple[T.typeId] = {
    };
  TransformedIndexTuple[T.typeId][H.typeId] = function(boxedValue) {
    this.boxedValue = boxedValue;
  };
  TransformedIndexTuple_$T_x_H$_ = TransformedIndexTuple[T.typeId][H.typeId];
  TransformedIndexTuple_$T_x_H$_.genericParameters = [T, H];
  TransformedIndexTuple_$T_x_H$_.genericClosure = TransformedIndexTuple;
  TransformedIndexTuple_$T_x_H$_.typeId = "tc$" + T.typeId + "_" + H.typeId + "$";
  TransformedIndexTuple_$T_x_H$_.getDefaultValue = function() {
    return {
      _elementIndex: 0,
      _headerCount: 0
    };
  };
  TransformedIndexTuple_$T_x_H$_.__ctor = function TransformedIndexTuple____ctor(elementIndex, headerCount) {
    var this_;
    this_ = TransformedIndexTuple_$T_x_H$_.getDefaultValue();
    this_._elementIndex = elementIndex;
    this_._headerCount = headerCount;
    return this_;
  };
  TransformedIndexTuple_$T_x_H$_.get_elementIndex = function TransformedIndexTuple__get_ElementIndex(this_) {
    return this_._elementIndex;
  };
  TransformedIndexTuple_$T_x_H$_.get_headerCount = function TransformedIndexTuple__get_HeaderCount(this_) {
    return this_._headerCount;
  };
  TransformedIndexTuple_$T_x_H$_.prototype = new ValueType();
  Type__RegisterStructType(TransformedIndexTuple_$T_x_H$_, "Sunlight.Framework.Observables.HeaderInjectableTransformer`2/TransformedIndexTuple<" + T.fullName + "," + H.fullName + ">", []);
  TransformedIndexTuple_$T_x_H$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    TransformedIndexTuple_$T_x_H$_ = TransformedIndexTuple(T, H, true);
    T = T;
    H = H;
  };
  if (_callStatiConstructor)
    TransformedIndexTuple_$T_x_H$_._tri();
  return TransformedIndexTuple_$T_x_H$_;
};
function ElementsTuple(T, H, _callStatiConstructor) {
  var ElementsTuple_$T_x_H$_, __initTracker;
  if (ElementsTuple[T.typeId] && ElementsTuple[T.typeId][H.typeId])
    return ElementsTuple[T.typeId][H.typeId];
    ElementsTuple[T.typeId] = {
    };
  ElementsTuple[T.typeId][H.typeId] = function(boxedValue) {
    this.boxedValue = boxedValue;
  };
  ElementsTuple_$T_x_H$_ = ElementsTuple[T.typeId][H.typeId];
  ElementsTuple_$T_x_H$_.genericParameters = [T, H];
  ElementsTuple_$T_x_H$_.genericClosure = ElementsTuple;
  ElementsTuple_$T_x_H$_.typeId = "uc$" + T.typeId + "_" + H.typeId + "$";
  ElementsTuple_$T_x_H$_.getDefaultValue = function() {
    return {
      _headerIndexes: null,
      _items: null
    };
  };
  ElementsTuple_$T_x_H$_.__ctor = function ElementsTuple____ctor(items, headerIndexes) {
    var this_;
    this_ = ElementsTuple_$T_x_H$_.getDefaultValue();
    this_._items = items;
    this_._headerIndexes = headerIndexes;
    return this_;
  };
  ElementsTuple_$T_x_H$_.get_headerIndexes = function ElementsTuple__get_HeaderIndexes(this_) {
    return this_._headerIndexes;
  };
  ElementsTuple_$T_x_H$_.get_items = function ElementsTuple__get_Items(this_) {
    return this_._items;
  };
  ElementsTuple_$T_x_H$_.prototype = new ValueType();
  Type__RegisterStructType(ElementsTuple_$T_x_H$_, "Sunlight.Framework.Observables.HeaderInjectableTransformer`2/ElementsTuple<" + T.fullName + "," + H.fullName + ">", []);
  ElementsTuple_$T_x_H$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    ElementsTuple_$T_x_H$_ = ElementsTuple(T, H, true);
    T = T;
    H = H;
  };
  if (_callStatiConstructor)
    ElementsTuple_$T_x_H$_._tri();
  return ElementsTuple_$T_x_H$_;
};
function ILista(T, _callStatiConstructor) {
  var IList$1_$T$_, IEnumerable$1_$T$_, __initTracker;
  if (ILista[T.typeId])
    return ILista[T.typeId];
  ILista[T.typeId] = function System__Collections__Generic__IList$1() {
  };
  IList$1_$T$_ = ILista[T.typeId];
  IList$1_$T$_.genericParameters = [T];
  IList$1_$T$_.genericClosure = ILista;
  IList$1_$T$_.typeId = "vc$" + T.typeId + "$";
  IEnumerable$1_$T$_ = IEnumerablea(T, _callStatiConstructor);
  Type__RegisterInterface(IList$1_$T$_, "System.Collections.Generic.IList`1<" + T.fullName + ">");
  IList$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    IList$1_$T$_ = ILista(T, true);
  };
  if (_callStatiConstructor)
    IList$1_$T$_._tri();
  return IList$1_$T$_;
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
  KeyValuePair$2_$K_x_V$_.typeId = "wc$" + K.typeId + "_" + V.typeId + "$";
  KeyValuePair$2_$K_x_V$_.getDefaultValue = function() {
    return {
      key: Type__GetDefaultValueStatic(K),
      val: Type__GetDefaultValueStatic(V)
    };
  };
  KeyValuePair$2_$K_x_V$_.__ctor = function KeyValuePair$2____ctor(key, value) {
    var this_;
    this_ = KeyValuePair$2_$K_x_V$_.getDefaultValue();
    this_.key = key;
    this_.val = value;
    return this_;
  };
  KeyValuePair$2_$K_x_V$_.get_key = function KeyValuePair$2__get_Key(this_) {
    return this_.key;
  };
  KeyValuePair$2_$K_x_V$_.prototype = new ValueType();
  Type__RegisterStructType(KeyValuePair$2_$K_x_V$_, "System.Collections.Generic.KeyValuePair`2<" + K.fullName + "," + V.fullName + ">", []);
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
function Enumeratora(TValue, _callStatiConstructor) {
  var KeyValuePair$2_$Number_x_TValue$_, Enumerator_$TValue$_, IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_, __initTracker;
  if (Enumeratora[TValue.typeId])
    return Enumeratora[TValue.typeId];
  Enumeratora[TValue.typeId] = function System__Collections__Generic__NumberDictionary$1$2fEnumerator() {
  };
  Enumerator_$TValue$_ = Enumeratora[TValue.typeId];
  Enumerator_$TValue$_.genericParameters = [TValue];
  Enumerator_$TValue$_.genericClosure = Enumeratora;
  Enumerator_$TValue$_.typeId = "xc$" + TValue.typeId + "$";
  KeyValuePair$2_$Number_x_TValue$_ = KeyValuePair(Number, TValue, _callStatiConstructor);
  IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_ = IEnumeratora(KeyValuePair(Number, TValue, _callStatiConstructor), _callStatiConstructor);
  Enumerator_$TValue$_.__ctor = function System_Collections_Generic_NumberDictionary$1$2fEnumerator_factory(dict) {
    var this_;
    this_ = new Enumerator_$TValue$_();
    this_.__ctor(dict);
    return this_;
  };
  ptyp_ = Enumerator_$TValue$_.prototype;
  ptyp_.dict = null;
  ptyp_.keys = null;
  ptyp_.__ctor = function Enumerator____ctor(dict) { {
      this.dict = dict;
      this.keys = this.dict.get_keys().V_GetEnumerator_m$n$();
    }
  };
  ptyp_.get_current = function Enumerator__get_Current() {
    return KeyValuePair$2_$Number_x_TValue$_.__ctor(this.keys.V_get_Current_p$n$(), this.dict.get_item(this.keys.V_get_Current_p$n$()));
  };
  ptyp_.moveNext = function Enumerator__MoveNext() {
    return this.keys.V_MoveNext_l();
  };
  ptyp_.system__Collections__IEnumerator__get_Current = function Enumerator__System__Collections__IEnumerator__get_Current() {
    return Type__BoxTypeInstance(KeyValuePair$2_$Number_x_TValue$_, this.get_current());
  };
  ptyp_.V_get_Current_l = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_l = ptyp_.moveNext;
  Type__RegisterReferenceType(Enumerator_$TValue$_, "System.Collections.Generic.NumberDictionary`1/Enumerator<" + TValue.fullName + ">", Object, [IEnumerator$1_$KeyValuePair$2_$Number_x_TValue$_$_, IEnumerator]);
  Enumerator_$TValue$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    TValue = TValue;
    Enumerator_$TValue$_ = Enumeratora(TValue, true);
  };
  if (_callStatiConstructor)
    Enumerator_$TValue$_._tri();
  return Enumerator_$TValue$_;
};
function QueueEnumerator(T, _callStatiConstructor) {
  var QueueEnumerator_$T$_, IEnumerator$1_$T$_, __initTracker;
  if (QueueEnumerator[T.typeId])
    return QueueEnumerator[T.typeId];
  QueueEnumerator[T.typeId] = function System__Collections__Generic__Queue$1$2fQueueEnumerator() {
  };
  QueueEnumerator_$T$_ = QueueEnumerator[T.typeId];
  QueueEnumerator_$T$_.genericParameters = [T];
  QueueEnumerator_$T$_.genericClosure = QueueEnumerator;
  QueueEnumerator_$T$_.typeId = "yc$" + T.typeId + "$";
  IEnumerator$1_$T$_ = IEnumeratora(T, _callStatiConstructor);
  QueueEnumerator_$T$_.__ctor = function System_Collections_Generic_Queue$1$2fQueueEnumerator_factory(queue) {
    var this_;
    this_ = new QueueEnumerator_$T$_();
    this_.__ctor(queue);
    return this_;
  };
  ptyp_ = QueueEnumerator_$T$_.prototype;
  ptyp_.queue = null;
  ptyp_.currentIndex = 0;
  ptyp_.__ctor = function QueueEnumerator____ctor(queue) { {
      this.queue = queue;
      this.currentIndex = -1;
    }
  };
  ptyp_.get_current = function QueueEnumerator__get_Current() {
    if (this.currentIndex < 0 || this.currentIndex >= this.queue.nativeArray.length)
      throw new Error("Out of range");
    return this.queue.nativeArray[this.currentIndex];
  };
  ptyp_.system__Collections__IEnumerator__get_Current = function QueueEnumerator__System__Collections__IEnumerator__get_Current() {
    return Type__BoxTypeInstance(T, this.get_current());
  };
  ptyp_.moveNext = function QueueEnumerator__MoveNext() {
    this.currentIndex++;
    return this.currentIndex < this.queue.nativeArray.length;
  };
  ptyp_.V_get_Current_l = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$T$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_l = ptyp_.moveNext;
  Type__RegisterReferenceType(QueueEnumerator_$T$_, "System.Collections.Generic.Queue`1/QueueEnumerator<" + T.fullName + ">", Object, [IEnumerator$1_$T$_, IEnumerator]);
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
  var IList$1_$T$_, ListEnumerator$1_$T$_, IEnumerator$1_$T$_, __initTracker, __initTrackera;
  if (ListEnumerator[T.typeId])
    return ListEnumerator[T.typeId];
  ListEnumerator[T.typeId] = function System__Collections__Generic__ListEnumerator$1() {
  };
  ListEnumerator$1_$T$_ = ListEnumerator[T.typeId];
  ListEnumerator$1_$T$_.genericParameters = [T];
  ListEnumerator$1_$T$_.genericClosure = ListEnumerator;
  ListEnumerator$1_$T$_.typeId = "zc$" + T.typeId + "$";
  IEnumerator$1_$T$_ = IEnumeratora(T, _callStatiConstructor);
  ListEnumerator$1_$T$_.__ctor = function System_Collections_Generic_ListEnumerator$1_factory(list) {
    var this_;
    this_ = new ListEnumerator$1_$T$_();
    this_.__ctor(list);
    return this_;
  };
  ptyp_ = ListEnumerator$1_$T$_.prototype;
  ptyp_.innerList = null;
  ptyp_.index = 0;
  ptyp_.__ctor = function ListEnumerator$1____ctor(list) {
    this.index = -1;
    this.innerList = list;
  };
  ptyp_.get_current = function ListEnumerator$1__get_Current() {
    return this.innerList["V_get_Item_" + IList$1_$T$_.typeId](this.index);
  };
  ptyp_.moveNext = function ListEnumerator$1__MoveNext() {
    return ++this.index < this.innerList.V_get_Count_e();
  };
  ptyp_.system__Collections__IEnumerator__get_Current = function ListEnumerator$1__System__Collections__IEnumerator__get_Current() {
    return Type__BoxTypeInstance(T, this.get_current());
  };
  ptyp_.V_get_Current_l = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$T$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_l = ptyp_.moveNext;
  Type__RegisterReferenceType(ListEnumerator$1_$T$_, "System.Collections.Generic.ListEnumerator`1<" + T.fullName + ">", Object, [IEnumerator$1_$T$_, IEnumerator]);
  ListEnumerator$1_$T$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    T = T;
    IList$1_$T$_ = ILista(T, true);
    ListEnumerator$1_$T$_ = ListEnumerator(T, true);
  };
  if (_callStatiConstructor)
    ListEnumerator$1_$T$_._tri();
  return ListEnumerator$1_$T$_;
};
function Enumeratorb(TValue, _callStatiConstructor) {
  var KeyValuePair$2_$String_x_TValue$_, Enumerator_$TValue$_, IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_, __initTracker;
  if (Enumeratorb[TValue.typeId])
    return Enumeratorb[TValue.typeId];
  Enumeratorb[TValue.typeId] = function System__Collections__Generic__StringDictionary$1$2fEnumerator() {
  };
  Enumerator_$TValue$_ = Enumeratorb[TValue.typeId];
  Enumerator_$TValue$_.genericParameters = [TValue];
  Enumerator_$TValue$_.genericClosure = Enumeratorb;
  Enumerator_$TValue$_.typeId = "ad$" + TValue.typeId + "$";
  KeyValuePair$2_$String_x_TValue$_ = KeyValuePair(String, TValue, _callStatiConstructor);
  IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_ = IEnumeratora(KeyValuePair(String, TValue, _callStatiConstructor), _callStatiConstructor);
  Enumerator_$TValue$_.__ctor = function System_Collections_Generic_StringDictionary$1$2fEnumerator_factory(dict) {
    var this_;
    this_ = new Enumerator_$TValue$_();
    this_.__ctor(dict);
    return this_;
  };
  ptyp_ = Enumerator_$TValue$_.prototype;
  ptyp_.dict = null;
  ptyp_.keys = null;
  ptyp_.__ctor = function Enumerator____ctor(dict) { {
      this.dict = dict;
      this.keys = this.dict.get_keys().V_GetEnumerator_m$o$();
    }
  };
  ptyp_.get_current = function Enumerator__get_Current() {
    return KeyValuePair$2_$String_x_TValue$_.__ctor(this.keys.V_get_Current_p$o$(), this.dict.get_item(this.keys.V_get_Current_p$o$()));
  };
  ptyp_.moveNext = function Enumerator__MoveNext() {
    return this.keys.V_MoveNext_l();
  };
  ptyp_.system__Collections__IEnumerator__get_Current = function Enumerator__System__Collections__IEnumerator__get_Current() {
    return Type__BoxTypeInstance(KeyValuePair$2_$String_x_TValue$_, this.get_current());
  };
  ptyp_.V_get_Current_l = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_l = ptyp_.moveNext;
  Type__RegisterReferenceType(Enumerator_$TValue$_, "System.Collections.Generic.StringDictionary`1/Enumerator<" + TValue.fullName + ">", Object, [IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_, IEnumerator]);
  Enumerator_$TValue$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    TValue = TValue;
    Enumerator_$TValue$_ = Enumeratorb(TValue, true);
  };
  if (_callStatiConstructor)
    Enumerator_$TValue$_._tri();
  return Enumerator_$TValue$_;
};
function IEnumerablea(T, _callStatiConstructor) {
  var IEnumerable$1_$T$_, __initTracker;
  if (IEnumerablea[T.typeId])
    return IEnumerablea[T.typeId];
  IEnumerablea[T.typeId] = function System__Collections__Generic__IEnumerable$1() {
  };
  IEnumerable$1_$T$_ = IEnumerablea[T.typeId];
  IEnumerable$1_$T$_.genericParameters = [T];
  IEnumerable$1_$T$_.genericClosure = IEnumerablea;
  IEnumerable$1_$T$_.typeId = "m$" + T.typeId + "$";
  Type__RegisterInterface(IEnumerable$1_$T$_, "System.Collections.Generic.IEnumerable`1<" + T.fullName + ">");
  IEnumerable$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    IEnumerable$1_$T$_ = IEnumerablea(T, true);
  };
  if (_callStatiConstructor)
    IEnumerable$1_$T$_._tri();
  return IEnumerable$1_$T$_;
};
function Enumerator(T, _callStatiConstructor) {
  var Enumerator_$T$_, IEnumerator$1_$T$_, __initTracker;
  if (Enumerator[T.typeId])
    return Enumerator[T.typeId];
  Enumerator[T.typeId] = function System__ArrayG$1$2fEnumerator() {
  };
  Enumerator_$T$_ = Enumerator[T.typeId];
  Enumerator_$T$_.genericParameters = [T];
  Enumerator_$T$_.genericClosure = Enumerator;
  Enumerator_$T$_.typeId = "bd$" + T.typeId + "$";
  IEnumerator$1_$T$_ = IEnumeratora(T, _callStatiConstructor);
  Enumerator_$T$_.__ctor = function System_ArrayG$1$2fEnumerator_factory(array) {
    var this_;
    this_ = new Enumerator_$T$_();
    this_.__ctor(array);
    return this_;
  };
  ptyp_ = Enumerator_$T$_.prototype;
  ptyp_.currentIndex = 0;
  ptyp_.array = null;
  ptyp_.__ctor = function Enumerator____ctor(array) { {
      this.currentIndex = -1;
      this.array = array;
    }
  };
  ptyp_.moveNext = function Enumerator__MoveNext() {
    return ++this.currentIndex < this.array.V_get_Length();
  };
  ptyp_.get_current = function Enumerator__get_Current() {
    return this.array.get_item(this.currentIndex);
  };
  ptyp_.system__Collections__IEnumerator__get_Current = function Enumerator__System__Collections__IEnumerator__get_Current() {
    return Type__BoxTypeInstance(T, this.get_current());
  };
  ptyp_.V_get_Current_l = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$T$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_l = ptyp_.moveNext;
  Type__RegisterReferenceType(Enumerator_$T$_, "System.ArrayG`1/Enumerator<" + T.fullName + ">", Object, [IEnumerator$1_$T$_, IEnumerator]);
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
function IEnumeratora(T, _callStatiConstructor) {
  var IEnumerator$1_$T$_, __initTracker;
  if (IEnumeratora[T.typeId])
    return IEnumeratora[T.typeId];
  IEnumeratora[T.typeId] = function System__Collections__Generic__IEnumerator$1() {
  };
  IEnumerator$1_$T$_ = IEnumeratora[T.typeId];
  IEnumerator$1_$T$_.genericParameters = [T];
  IEnumerator$1_$T$_.genericClosure = IEnumeratora;
  IEnumerator$1_$T$_.typeId = "p$" + T.typeId + "$";
  Type__RegisterInterface(IEnumerator$1_$T$_, "System.Collections.Generic.IEnumerator`1<" + T.fullName + ">");
  IEnumerator$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    IEnumerator$1_$T$_ = IEnumeratora(T, true);
  };
  if (_callStatiConstructor)
    IEnumerator$1_$T$_._tri();
  return IEnumerator$1_$T$_;
};
ValueIfTrue_$String$_ = ValueIfTrue(String);
Func_$Object_x_Object$_ = Func(Object, Object);
ArrayG_$Func_$Object_x_Object$_$_ = ArrayG(Func_$Object_x_Object$_);
ArrayG_$String$_ = ArrayG(String);
ArrayG_$SkinBinderInfo$_ = ArrayG(SkinBinderInfo);
ArrayG_$Object$_ = ArrayG(Object);
ArrayG_$TestViewModelB$_ = ArrayG(TestViewModelB);
HeaderInjectableTransformer_$TestViewModelB_x_TestViewModelC$_ = HeaderInjectableTransformer(TestViewModelB, TestViewModelC);
ObservableCollection_$TestViewModelB$_ = ObservableCollection(TestViewModelB);
KeyValuePair_$Number_x_Task$_ = KeyValuePair(Number, Task);
NumberDictionary_$Task$_ = NumberDictionary(Task);
Queue_$Task$_ = Queue(Task);
List_$ListViewItem$_ = List(ListViewItem);
List_$Int32$_ = List(Int32);
Action_$UIEvent$_ = Action(UIEvent);
KeyValuePair_$String_x_Action_$UIEvent$_$_ = KeyValuePair(String, Action_$UIEvent$_);
StringDictionary_$Action_$UIEvent$_$_ = StringDictionary(Action_$UIEvent$_);
ObservableCollectionGenerator_$Object_x_Object$_ = ObservableCollectionGenerator(Object, Object);
List_$Object$_ = List(Object);
Action_$INotifyPropertyChanged_x_String$_ = Actiona(INotifyPropertyChanged, String);
KeyValuePair_$String_x_Action_$INotifyPropertyChanged_x_String$_$_ = KeyValuePair(String, Action_$INotifyPropertyChanged_x_String$_);
StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_ = StringDictionary(Action_$INotifyPropertyChanged_x_String$_);
KeyValuePair_$String_x_Function$_ = KeyValuePair(String, Function);
StringDictionary_$Function$_ = StringDictionary(Function);
ArrayG_$Number$_ = ArrayG(Number);
KeyValuePair_$String_x_Int32$_ = KeyValuePair(String, Int32);
StringDictionary_$Int32$_ = StringDictionary(Int32);
ManualTemplateTests____cctor();
DateTime____cctor();
ValueIfTrue_$String$_._tri();
Func_$Object_x_Object$_._tri();
ArrayG_$Func_$Object_x_Object$_$_._tri();
ArrayG_$String$_._tri();
ArrayG_$SkinBinderInfo$_._tri();
ArrayG_$Object$_._tri();
ArrayG_$TestViewModelB$_._tri();
HeaderInjectableTransformer_$TestViewModelB_x_TestViewModelC$_._tri();
ObservableCollection_$TestViewModelB$_._tri();
KeyValuePair_$Number_x_Task$_._tri();
NumberDictionary_$Task$_._tri();
Queue_$Task$_._tri();
List_$ListViewItem$_._tri();
List_$Int32$_._tri();
Action_$UIEvent$_._tri();
KeyValuePair_$String_x_Action_$UIEvent$_$_._tri();
StringDictionary_$Action_$UIEvent$_$_._tri();
ObservableCollectionGenerator_$Object_x_Object$_._tri();
List_$Object$_._tri();
Action_$INotifyPropertyChanged_x_String$_._tri();
KeyValuePair_$String_x_Action_$INotifyPropertyChanged_x_String$_$_._tri();
StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_._tri();
KeyValuePair_$String_x_Function$_._tri();
StringDictionary_$Function$_._tri();
ArrayG_$Number$_._tri();
KeyValuePair_$String_x_Int32$_._tri();
StringDictionary_$Int32$_._tri();
QUnit.module("Sunlight.Framework.UI.Test.LiveBinderTests", {
  "before": LiveBinderTests__Setup
});
QUnit.test("TestLiveBinderOnActivate", LiveBinderTests__TestLiveBinderOnActivate);
QUnit.test("TestLiveBinderOnChange", LiveBinderTests__TestLiveBinderOnChange);
QUnit.test("TestLiveBinderMultiOnActivate", LiveBinderTests__TestLiveBinderMultiOnActivate);
QUnit.test("TestLiveBinderMultiOnChange", LiveBinderTests__TestLiveBinderMultiOnChange);
QUnit.test("TestTwoWayLiveBinderOnChange", LiveBinderTests__TestTwoWayLiveBinderOnChange);
QUnit.test("TestTwoWayLiveBinderMultiOnChangeWithConverters", LiveBinderTests__TestTwoWayLiveBinderMultiOnChangeWithConverters);
QUnit.module("Sunlight.Framework.UI.Test.NScriptsTemplateTests", {
  "before": NScriptsTemplateTests__Setup
});
QUnit.test("Test", NScriptsTemplateTests__Test);
QUnit.test("TestApplySkin", NScriptsTemplateTests__TestApplySkin);
QUnit.test("TestCssBinder", NScriptsTemplateTests__TestCssBinder);
QUnit.test("TestStyleBinder", NScriptsTemplateTests__TestStyleBinder);
QUnit.test("TestAttrBinder", NScriptsTemplateTests__TestAttrBinder);
QUnit.test("TestPropertyBinder", NScriptsTemplateTests__TestPropertyBinder);
QUnit.module("Sunlight.Framework.UI.Test.ManualTemplateTests", {
  "before": ManualTemplateTests__Setup
});
QUnit.test("Test", ManualTemplateTests__Test);
QUnit.module("Sunlight.Framework.UI.Test.SkinBinderHelperTests", {
});
QUnit.test("TestSimpleBinder", SkinBinderHelperTests__TestSimpleBinder);
QUnit.test("TestAttrBinding", SkinBinderHelperTests__TestAttrBinding);
QUnit.test("TestStyleBinding", SkinBinderHelperTests__TestStyleBinding);
QUnit.test("TestTextContentBinding", SkinBinderHelperTests__TestTextContentBinding);
QUnit.module("Sunlight.Framework.UI.Test.TestListView", {
  "before": TestListView__Setup
});
QUnit.test("TestChildSkin", TestListView__TestChildSkin);
QUnit.test("TestHeaderSkin", TestListView__TestHeaderSkin);
QUnit.module("Sunlight.Framework.UI.Test.UIElementTests", {
});
QUnit.test("TestNewUIElement", UIElementTests__TestNewUIElement);
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
function gettera(src) {
  return src.get_propBool1();
};
function Test$5cTemplates$5cTestTemplateVMB_CssBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[1]) {
    domStore[1] = doc.createElement("div");
    domStore[1].innerHTML = " <div test=\"id\"></div> ";
    tmplStore[1] = tmplStore[1] ? tmplStore[1] : [SkinBinderInfo_factoryb([getter], SkinBinderHelper__SetTextContent, 17, 0, null, ""), SkinBinderInfo_factoryd([gettera], ["PropBool1"], SkinBinderHelper__SetCssClass, "black", 81, 0, 0, null, false, 0)];
  }
  htmlRoot = domStore[1].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
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
  objStorage[0] = SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
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
    tmplStore[3] = tmplStore[3] ? tmplStore[3] : [SkinBinderInfo_factoryd([getter], ["PropStr1"], SkinBinderHelper__SetAttribute, "test1", 113, 0, 0, null, null, 0)];
  }
  htmlRoot = domStore[3].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[3], null, 1, 0);
};
Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var = null;
function Test$5cTemplates$5cTestTemplateVMB_AttrBinding() {
  if (!Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var)
    Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var = Skin_factory(UISkinableElement, TestViewModelB, Test$5cTemplates$5cTestTemplateVMB_AttrBinding_factory, "3");
  return Test$5cTemplates$5cTestTemplateVMB_AttrBinding_var;
};
function getterb(src) {
  return src.get_propInt1();
};
function setter(tar, val) {
  tar.set_propInt1(val);
};
function settera(tar, val) {
  tar.set_twoWayLooseBinding(val);
};
function getterc(src) {
  return src.get_twoWayLooseBinding();
};
function setterb(tar, val) {
  tar.set_oneWayStrictBinding(val);
};
function Test$5cTemplates$5cTestTemplateB_PropertyBinding_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[4]) {
    domStore[4] = doc.createElement("div");
    domStore[4].innerHTML = " <div> This is a test. </div> ";
    tmplStore[4] = tmplStore[4] ? tmplStore[4] : [SkinBinderInfo_factorya([getterb], setter, ["PropInt1"], settera, getterc, "TwoWayLooseBinding", 17, 0, 0, null, null, 11), SkinBinderInfo_factory([getter], ["PropStr1"], setterb, 17, 0, 1, null, "test")];
  }
  htmlRoot = domStore[4].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = TestUIElement_factory(SkinBinderHelper__GetElementFromPath(htmlRoot, [1]));
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
    tmplStore[5] = tmplStore[5] ? tmplStore[5] : [SkinBinderInfo_factoryb([getter], SkinBinderHelper__SetTextContent, 17, 0, null, "")];
  }
  htmlRoot = domStore[5].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[5], null, 0, 0);
};
Test$5cTemplates$5cTestTemplateVMB1_var = null;
function Test$5cTemplates$5cTestTemplateVMB1() {
  if (!Test$5cTemplates$5cTestTemplateVMB1_var)
    Test$5cTemplates$5cTestTemplateVMB1_var = Skin_factory(UISkinableElement, TestViewModelB, Test$5cTemplates$5cTestTemplateVMB1_factory, "5");
  return Test$5cTemplates$5cTestTemplateVMB1_var;
};
function getterd(src) {
  return src.get_propInt2();
};
function Test$5cTemplates$5cTestTemplateVMB1_int_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[6]) {
    domStore[6] = doc.createElement("div");
    domStore[6].innerHTML = " <div test=\"PropInt2\"></div> ";
    tmplStore[6] = tmplStore[6] ? tmplStore[6] : [SkinBinderInfo_factoryb([getterd], SkinBinderHelper__SetTextContent, 17, 0, null, "")];
  }
  htmlRoot = domStore[6].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[6], null, 0, 0);
};
Test$5cTemplates$5cTestTemplateVMB1_int_var = null;
function Test$5cTemplates$5cTestTemplateVMB1_int() {
  if (!Test$5cTemplates$5cTestTemplateVMB1_int_var)
    Test$5cTemplates$5cTestTemplateVMB1_int_var = Skin_factory(UISkinableElement, TestViewModelB, Test$5cTemplates$5cTestTemplateVMB1_int_factory, "6");
  return Test$5cTemplates$5cTestTemplateVMB1_int_var;
};
function gettere(src) {
  return src.get_propStr3();
};
function Test$5cTemplates$5cTestTemplateVMC1_factory(skinFactory, doc) {
  var objStorage, htmlRoot, domStore;
  if (!(domStore = DocStorageGetter(doc))[7]) {
    domStore[7] = doc.createElement("div");
    domStore[7].innerHTML = " <div test=\"PropStr3\"></div> ";
    tmplStore[7] = tmplStore[7] ? tmplStore[7] : [SkinBinderInfo_factoryb([gettere], SkinBinderHelper__SetTextContent, 17, 0, null, "")];
  }
  htmlRoot = domStore[7].cloneNode(true);
  objStorage = new Array(1);
  objStorage[0] = SkinBinderHelper__GetElementFromPath(htmlRoot, [1]);
  return SkinInstance_factory(skinFactory, htmlRoot, [], objStorage, tmplStore[7], null, 0, 0);
};
Test$5cTemplates$5cTestTemplateVMC1_var = null;
function Test$5cTemplates$5cTestTemplateVMC1() {
  if (!Test$5cTemplates$5cTestTemplateVMC1_var)
    Test$5cTemplates$5cTestTemplateVMC1_var = Skin_factory(UISkinableElement, TestViewModelC, Test$5cTemplates$5cTestTemplateVMC1_factory, "7");
  return Test$5cTemplates$5cTestTemplateVMC1_var;
};
tmplStore = new Array(8);
function DocStorageGetter(doc) {
  var style;
  if (!doc.stateStore) {
    doc.stateStore = new Array(8);
    style = doc.createElement("style");
    style.textContent = ".black{background-color:black;\ncolor:white;\n}\n";
    doc.head.appendChild(style);
  }
  return doc.stateStore;
};
})();
//# sourceMappingURL=GeneratedScriptsSunlight.Framework.UI.Test.map
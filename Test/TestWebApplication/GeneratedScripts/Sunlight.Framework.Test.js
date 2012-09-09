
Function.typeId = "i";
System__Type__typeMapping = null;
function System__Type__AsType(this_, instance) {
  return this_.isInstanceOfType(instance) ? instance : null;
};
function System__Type__CastType(this_, instance) {
  if (this_.isInstanceOfType(instance)) {
    if (this_.isStruct)
      return instance.boxedValue;
    return instance;
  }
  throw "InvalidCast to " + this_.fullName;
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
ptyp_.isStruct = false;
ptyp_.isInterface = false;
ptyp_.baseType = null;
ptyp_.fullName = null;
ptyp_.typeId = null;
ptyp_.baseInterfaces = null;
ptyp_.boxedValue = null;
ptyp_.interfaces = null;
ptyp_.isInstanceOfType = function System__Type__IsInstanceOfType(instance) {
  if (!this.isInterface)
    return instance instanceof this;
  else if (instance && !instance.constructor.baseInterfaces)
    System__Type__InitializeBaseInterfaces(instance.constructor);
  return instance && instance.constructor.baseInterfaces && instance.constructor.baseInterfaces[this.fullName];
};
ptyp_.box = function System__Type__Box(instance) {
  if (this.isStruct)
    return new this(instance);
  else
    return instance;
};
ptyp_.unbox = function System__Type__Unbox(instance) {
  if (this.isStruct)
    return instance.boxedValue;
  else
    return instance;
};
ptyp_.defaultConstructor = function System__Type__DefaultConstructor() {
  if (this.isStruct)
    return this.getDefaultValue();
  throw "Default constructor not implemented";
};
ptyp_.getDefaultValue = function System__Type__GetDefaultValue() {
  return null;
};
System__Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "j";
function System__Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
System__Type__RegisterReferenceType(Object, "System.Object", null, []);
function Sunlight_Framework_Test_Binders_DataBinderTests() {
};
Sunlight_Framework_Test_Binders_DataBinderTests.typeId = "k";
function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderSimpleObjectTest() {
  var dataBinder, target, source;
  dataBinder = Sunlight__Framework__Binders__DataBinder_factory(Sunlight__Framework__Binders__SourcePropertyBinder_factory(System_ArrayG_$String$_.__ctor(["IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderSimpleObjectTest_del(obj) {
    return System_Int32.box(System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleObjectWithProperty, obj).get_intProp());
  }]), null), Sunlight__Framework__Binders__TargetBinder_factory("IntProp", null, function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderSimpleObjectTest_del2(obj, value) {
    System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleNotifiableClass, obj).set_intProp(System_Int32.unbox(value));
  }, System_Int32.box( -1)), 1, null);
  target = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepNotifiableObject();
  source = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepSimpleObject();
  source.set_intProp(101);
  dataBinder.setTarget(target);
  QUnit.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(source);
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(null);
  QUnit.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
};
function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderSimpleObjectTestReverseOrder() {
  var dataBinder, target, source;
  dataBinder = Sunlight__Framework__Binders__DataBinder_factory(Sunlight__Framework__Binders__SourcePropertyBinder_factory(System_ArrayG_$String$_.__ctor(["IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderSimpleObjectTestReverseOrder_del(obj) {
    return System_Int32.box(System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleObjectWithProperty, obj).get_intProp());
  }]), null), Sunlight__Framework__Binders__TargetBinder_factory("IntProp", null, function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderSimpleObjectTestReverseOrder_del2(obj, value) {
    System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleNotifiableClass, obj).set_intProp(System_Int32.unbox(value));
  }, System_Int32.box( -1)), 1, null);
  target = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepNotifiableObject();
  source = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepSimpleObject();
  source.set_intProp(101);
  dataBinder.setSource(source);
  dataBinder.setTarget(target);
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(null);
  QUnit.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
};
function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderOneWayNotifiableObjectTest() {
  var dataBinder, target, source;
  dataBinder = Sunlight__Framework__Binders__DataBinder_factory(Sunlight__Framework__Binders__SourcePropertyBinder_factory(System_ArrayG_$String$_.__ctor(["IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderOneWayNotifiableObjectTest_del(obj) {
    return System_Int32.box(System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleNotifiableClass, obj).get_intProp());
  }]), null), Sunlight__Framework__Binders__TargetBinder_factory("IntProp", null, function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderOneWayNotifiableObjectTest_del2(obj, value) {
    System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleNotifiableClass, obj).set_intProp(System_Int32.unbox(value));
  }, System_Int32.box( -1)), 1, null);
  target = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepNotifiableObject();
  source = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepNotifiableObject();
  source.set_intProp(101);
  dataBinder.setTarget(target);
  QUnit.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(source);
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  source.set_intProp(102);
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
};
function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderTwoWayNotifiableObjectTest() {
  var dataBinder, target, source, stmtTemp1;
  dataBinder = Sunlight__Framework__Binders__DataBinder_factory(Sunlight__Framework__Binders__SourcePropertyBinder_factory(System_ArrayG_$String$_.__ctor(["IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderTwoWayNotifiableObjectTest_del(obj) {
    return System_Int32.box(System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleNotifiableClass, obj).get_intProp());
  }]), function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderTwoWayNotifiableObjectTest_del2(obj, value) {
    System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleNotifiableClass, obj).set_intProp(System_Int32.unbox(value));
  }), Sunlight__Framework__Binders__TargetBinder_factory("IntProp", function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderTwoWayNotifiableObjectTest_del3(obj) {
    return System_Int32.box(System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleNotifiableClass, obj).get_intProp());
  }, function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderTwoWayNotifiableObjectTest_del4(obj, value) {
    System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleNotifiableClass, obj).set_intProp(System_Int32.unbox(value));
  }, System_Int32.box( -1)), 2, null);
  target = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepNotifiableObject();
  source = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepNotifiableObject();
  source.set_intProp(101);
  dataBinder.setTarget(target);
  QUnit.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(source);
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  target.set_intProp((stmtTemp1 = target.get_intProp()) + 1), stmtTemp1;
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_Binders_DataBinderTests, "Sunlight.Framework.Test.Binders.DataBinderTests", Object, []);
function Sunlight_Framework_Test_Binders_SourcePropertyBinderTests() {
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.typeId = "l";
function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepNotifiableObject() {
  var rv;
  rv = (function() {
    var $5fipi;
    $5fipi = Sunlight__Framework__Test__Binders__SimpleNotifiableClass_factory();
    $5fipi.set_intProp(10);
    $5fipi.set_strProp("Ten");
    $5fipi.set_objProp((function() {
      var $5fipi;
      $5fipi = Sunlight__Framework__Test__Binders__SimpleObjectWithProperty_factory();
      $5fipi.set_intProp(11);
      $5fipi.set_stringProp("Eleven");
      return $5fipi;
    })());
    return $5fipi;
  })();
  rv.set_selfProp(rv);
  rv.get_objProp().set_selfProp(rv.get_objProp());
  rv.get_objProp().set_notifiableProp(rv);
  return rv;
};
function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepSimpleObject() {
  return Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepNotifiableObject().get_objProp();
};
function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__BasicValueTest() {
  var sourceBinder, helper, src;
  sourceBinder = Sunlight__Framework__Binders__SourcePropertyBinder_factory(System_ArrayG_$String$_.__ctor([null]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__BasicValueTest_del(obj) {
    return System_Int32.box(System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleObjectWithProperty, obj).get_intProp());
  }]), null);
  helper = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper_factory();
  sourceBinder.useDataBinder(helper);
  src = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepSimpleObject();
  sourceBinder.set_source(src);
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
};
function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__BasicValueTestWithNotification() {
  var sourceBinder, helper, src, stmtTemp1;
  sourceBinder = Sunlight__Framework__Binders__SourcePropertyBinder_factory(System_ArrayG_$String$_.__ctor(["IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__BasicValueTestWithNotification_del(obj) {
    return System_Int32.box(System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleNotifiableClass, obj).get_intProp());
  }]), null);
  helper = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper_factory();
  sourceBinder.useDataBinder(helper);
  src = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepNotifiableObject();
  sourceBinder.set_source(src);
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
  helper.set_sourceValueUpdateCalled(false);
  src.set_intProp((stmtTemp1 = src.get_intProp()) + 1), stmtTemp1;
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
};
function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PropertyPathValueNotifiableTest() {
  var sourceBinder, helper, src;
  sourceBinder = Sunlight__Framework__Binders__SourcePropertyBinder_factory(System_ArrayG_$String$_.__ctor(["NotifiableProp", "IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PropertyPathValueNotifiableTest_del(obj) {
    return System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleObjectWithProperty, obj).get_notifiableProp();
  }, function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PropertyPathValueNotifiableTest_del2(obj) {
    return System_Int32.box(System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleNotifiableClass, obj).get_intProp());
  }]), null);
  helper = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper_factory();
  sourceBinder.useDataBinder(helper);
  src = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepSimpleObject();
  sourceBinder.set_source(src);
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_notifiableProp().get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
  helper.set_sourceValueUpdateCalled(false);
  src.get_notifiableProp().set_intProp( -1);
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_notifiableProp().get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
};
function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PropertyPathValueTest() {
  var sourceBinder, helper, src, lastValue;
  sourceBinder = Sunlight__Framework__Binders__SourcePropertyBinder_factory(System_ArrayG_$String$_.__ctor(["SelfProp", "IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PropertyPathValueTest_del(obj) {
    return System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleObjectWithProperty, obj).get_selfProp();
  }, function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PropertyPathValueTest_del2(obj) {
    return System_Int32.box(System__Type__CastType(Sunlight_Framework_Test_Binders_SimpleObjectWithProperty, obj).get_intProp());
  }]), null);
  helper = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper_factory();
  sourceBinder.useDataBinder(helper);
  src = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepSimpleObject();
  sourceBinder.set_source(src);
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_selfProp().get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
  helper.set_sourceValueUpdateCalled(false);
  lastValue = src.get_selfProp().get_intProp();
  src.get_selfProp().set_intProp( -1);
  QUnit.ok(!helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(lastValue, System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_Binders_SourcePropertyBinderTests, "Sunlight.Framework.Test.Binders.SourcePropertyBinderTests", Object, []);
function Sunlight_Framework_Test_ContainerTests() {
};
Sunlight_Framework_Test_ContainerTests.typeId = "m";
function Sunlight__Framework__Test__ContainerTests__TestRegisterResolve() {
  var container, x, y, t2, t1;
  container = Sunlight__Framework__IocContainer_factory();
  x = 1;
  y = 2;
  container.register(Sunlight_Framework_Test_IocTestType2, function Sunlight__Framework__Test__ContainerTests__TestRegisterResolve_del() {
    return Sunlight__Framework__Test__IocTestType2_factory(x);
  });
  container.register(Sunlight_Framework_Test_IocTestType1, function Sunlight__Framework__Test__ContainerTests__TestRegisterResolve_del2() {
    return Sunlight__Framework__Test__IocTestType1_factory(x, y);
  });
  t2 = container.resolve(Sunlight_Framework_Test_IocTestType2);
  QUnit.ok(t2 !== null, "t2 != null");
  QUnit.equal(1, t2.testMethod(), "1 == t1.TestMethod()");
  t1 = container.resolve(Sunlight_Framework_Test_IocTestType1);
  QUnit.ok(t1 !== null, "t1 != null");
  QUnit.equal(3, t1.testMethod(), "3 == t1.TestMethod()");
  x = 10;
  t1 = container.resolve(Sunlight_Framework_Test_IocTestType1);
  QUnit.equal(12, t1.testMethod(), "12 == t1.TestMethod()");
};
function Sunlight__Framework__Test__ContainerTests__TestRegisterResolveWithAs() {
  var container, x, y, t2, t1;
  container = Sunlight__Framework__IocContainer_factory();
  x = 1;
  y = 2;
  container.register(Sunlight_Framework_Test_IocTestType2, function Sunlight__Framework__Test__ContainerTests__TestRegisterResolveWithAs_del() {
    return Sunlight__Framework__Test__IocTestType2_factory(x);
  });
  container.register(Sunlight_Framework_Test_IocTestType1, function Sunlight__Framework__Test__ContainerTests__TestRegisterResolveWithAs_del2() {
    return Sunlight__Framework__Test__IocTestType1_factory(x, y);
  }).as(Sunlight_Framework_Test_IIocTestType1);
  t2 = container.resolve(Sunlight_Framework_Test_IocTestType2);
  QUnit.ok(t2 !== null, "t2 != null");
  QUnit.equal(1, t2.testMethod(), "1 == t1.TestMethod()");
  t1 = container.resolve(Sunlight_Framework_Test_IIocTestType1);
  QUnit.ok(t1 !== null, "t1 != null");
  QUnit.equal(3, t1.V_TestMethod_b(), "3 == t1.TestMethod()");
};
function Sunlight__Framework__Test__ContainerTests__TestRegisterResolveIsSingleton() {
  var container, x, y, t2, t1, t1_;
  container = Sunlight__Framework__IocContainer_factory();
  x = 1;
  y = 2;
  container.register(Sunlight_Framework_Test_IocTestType2, function Sunlight__Framework__Test__ContainerTests__TestRegisterResolveIsSingleton_del() {
    return Sunlight__Framework__Test__IocTestType2_factory(x);
  });
  container.register(Sunlight_Framework_Test_IocTestType1, function Sunlight__Framework__Test__ContainerTests__TestRegisterResolveIsSingleton_del2() {
    return Sunlight__Framework__Test__IocTestType1_factory(x, y);
  }).isSingleton();
  t2 = container.resolve(Sunlight_Framework_Test_IocTestType2);
  QUnit.ok(t2 !== null, "t2 != null");
  QUnit.equal(1, t2.testMethod(), "1 == t1.TestMethod()");
  t1 = container.resolve(Sunlight_Framework_Test_IocTestType1);
  x = 10;
  t1_ = container.resolve(Sunlight_Framework_Test_IocTestType1);
  QUnit.strictEqual(t1_, t1, "t1_ === t1");
};
function Sunlight__Framework__Test__ContainerTests__TestRegisterResolveLazy() {
  var container, x, y, t1;
  container = Sunlight__Framework__IocContainer_factory();
  x = 1;
  y = 2;
  container.register(Sunlight_Framework_Test_IocTestType1, function Sunlight__Framework__Test__ContainerTests__TestRegisterResolveLazy_del() {
    return Sunlight__Framework__Test__IocTestType1_factory(x++, y);
  }).isSingleton();
  t1 = container.resolveLazy(Sunlight_Framework_Test_IocTestType1);
  QUnit.equal(1, x, "x === 1");
  QUnit.equal(3, t1.get_value().testMethod(), "t1.Value.TestMethod() == 3");
  QUnit.equal(2, x, "x === 2");
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_ContainerTests, "Sunlight.Framework.Test.ContainerTests", Object, []);
function Sunlight_Framework_Test_EventBusTests() {
};
Sunlight_Framework_Test_EventBusTests.typeId = "n";
function Sunlight__Framework__Test__EventBusTests__TestSubscribeAndRaise() {
  var evtBus, x1, x2;
  evtBus = Sunlight__Framework__EventBus_factory();
  x1 = 0;
  x2 = 0;
  evtBus.subscribe(Sunlight_Framework_Test_EventBusTests_Evt1, function Sunlight__Framework__Test__EventBusTests__TestSubscribeAndRaise_del(evt) {
    return x1 = evt.x;
  });
  evtBus.subscribe(Sunlight_Framework_Test_EventBusTests_Evt2, function Sunlight__Framework__Test__EventBusTests__TestSubscribeAndRaise_del2(evt) {
    return x2 = evt.x;
  });
  evtBus.raise(Sunlight_Framework_Test_EventBusTests_Evt1, (function() {
    var $5fipi;
    $5fipi = Sunlight__Framework__Test__EventBusTests$2fEvt1_factory();
    $5fipi.x = 10;
    return $5fipi;
  })());
  QUnit.equal(10, x1, "10 == x1");
  QUnit.equal(0, x2, "0 == x2");
};
function Sunlight__Framework__Test__EventBusTests__TestSubscribeAndRaiseOnceTime() {
  var evtBus, x1, x2, del;
  evtBus = Sunlight__Framework__EventBus_factory();
  x1 = 0;
  x2 = 0;
  del = function Sunlight__Framework__Test__EventBusTests__TestSubscribeAndRaiseOnceTime_del(evt) {
    x1 = evt.x;
  };
  evtBus.subscribe(Sunlight_Framework_Test_EventBusTests_Evt1, del);
  evtBus.subscribe(Sunlight_Framework_Test_EventBusTests_Evt2, function Sunlight__Framework__Test__EventBusTests__TestSubscribeAndRaiseOnceTime_del2(evt) {
    return x2 = evt.x;
  });
  evtBus.raiseOneTime(Sunlight_Framework_Test_EventBusTests_Evt1, (function() {
    var $5fipi;
    $5fipi = Sunlight__Framework__Test__EventBusTests$2fEvt1_factory();
    $5fipi.x = 10;
    return $5fipi;
  })());
  QUnit.equal(10, x1, "10 == x1");
  x1 = 0;
  evtBus.subscribe(Sunlight_Framework_Test_EventBusTests_Evt1, del);
  QUnit.equal(10, x1, "(2) 10 == x1");
};
function Sunlight__Framework__Test__EventBusTests__TestSubscribeUnSubscribeAndRaise() {
  var evtBus, x1, x2, del;
  evtBus = Sunlight__Framework__EventBus_factory();
  x1 = 0;
  x2 = 0;
  del = function Sunlight__Framework__Test__EventBusTests__TestSubscribeUnSubscribeAndRaise_del(evt) {
    x1 = evt.x;
  };
  evtBus.subscribe(Sunlight_Framework_Test_EventBusTests_Evt2, function Sunlight__Framework__Test__EventBusTests__TestSubscribeUnSubscribeAndRaise_del2(evt) {
    return x2 = evt.x;
  });
  evtBus.subscribe(Sunlight_Framework_Test_EventBusTests_Evt1, function Sunlight__Framework__Test__EventBusTests__TestSubscribeUnSubscribeAndRaise_del3(evt) {
    return x2 = evt.x;
  });
  evtBus.unSubscribe(Sunlight_Framework_Test_EventBusTests_Evt1, del);
  evtBus.raise(Sunlight_Framework_Test_EventBusTests_Evt1, (function() {
    var $5fipi;
    $5fipi = Sunlight__Framework__Test__EventBusTests$2fEvt1_factory();
    $5fipi.x = 10;
    return $5fipi;
  })());
  QUnit.equal(0, x1, "0 == x1");
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_EventBusTests, "Sunlight.Framework.Test.EventBusTests", Object, []);
function Sunlight_Framework_Test_ObservableCollectionTests() {
};
Sunlight_Framework_Test_ObservableCollectionTests.typeId = "o";
function Sunlight__Framework__Test__ObservableCollectionTests__TestCreateNewObservableCollection() {
  var observableCollection;
  observableCollection = Sunlight_Framework_Observables_ObservableCollection_$Int32$_.defaultConstructor();
  QUnit.notEqual(null, observableCollection, "ObservableCollection should be created");
  QUnit.equal(0, observableCollection.get_count(), "ObservableCollection's size should be 0");
};
function Sunlight__Framework__Test__ObservableCollectionTests__TestAddItemToObservableCollection() {
  var observableCollection, eventRaised;
  observableCollection = Sunlight_Framework_Observables_ObservableCollection_$Int32$_.defaultConstructor();
  eventRaised = false;
  observableCollection.add_CollectionChanged(function Sunlight__Framework__Test__ObservableCollectionTests__TestAddItemToObservableCollection_del(coll, evtArg) {
    QUnit.equal(observableCollection, coll, "ObservableCollection");
    QUnit.equal(1, evtArg.get_newItems().V_get_Count_c$d$(), "evtArg.NewItems.Count");
    QUnit.ok(System__Object__IsNullOrUndefined(evtArg.get_oldItems()), "Object.IsNullOrUndefined(evtArg.OldItems)");
    QUnit.equal(0, evtArg.get_changeIndex(), "evtArg.changeIndex");
    eventRaised = true;
  });
  observableCollection.add(1);
  QUnit.ok(eventRaised, "Change event raised");
};
function Sunlight__Framework__Test__ObservableCollectionTests__TestRemoveItemToObservableCollection() {
  var observableCollection, eventRaised;
  observableCollection = Sunlight_Framework_Observables_ObservableCollection_$Int32$_.defaultConstructor();
  eventRaised = false;
  observableCollection.add(1);
  observableCollection.add(2);
  observableCollection.add(3);
  observableCollection.add_CollectionChanged(function Sunlight__Framework__Test__ObservableCollectionTests__TestRemoveItemToObservableCollection_del(coll, evtArg) {
    QUnit.equal(observableCollection, coll, "ObservableCollection");
    QUnit.equal(2, evtArg.get_oldItems().V_get_Count_c$d$(), "evtArg.OldItems.Count");
    QUnit.ok(System__Object__IsNullOrUndefined(evtArg.get_newItems()), "Object.IsNullOrUndefined(evtArg.NewItems)");
    QUnit.equal(1, evtArg.get_changeIndex(), "evtArg.changeIndex");
    eventRaised = true;
  });
  observableCollection.removeRangeAt(1, 2);
  QUnit.ok(eventRaised, "Change event raised");
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_ObservableCollectionTests, "Sunlight.Framework.Test.ObservableCollectionTests", Object, []);
function Sunlight_Framework_Test_ObservableObjectTests() {
};
Sunlight_Framework_Test_ObservableObjectTests.typeId = "p";
function Sunlight__Framework__Test__ObservableObjectTests__TestCreateNewObservableObject() {
  var observableObject;
  observableObject = Sunlight__Framework__Test__ObservableObjectTests$2fObservableTestObject_factory();
  QUnit.notEqual(null, observableObject, "ObservableObject should be created");
};
function Sunlight__Framework__Test__ObservableObjectTests__TestFirePropertyChanged() {
  var observableObject, strChanged, cbCalled, cb1;
  observableObject = Sunlight__Framework__Test__ObservableObjectTests$2fObservableTestObject_factory();
  strChanged = false;
  cbCalled = false;
  cb1 = function Sunlight__Framework__Test__ObservableObjectTests__TestFirePropertyChanged_del(sender, propName) {
    strChanged = propName === "StringProp" && sender === observableObject;
    cbCalled = true;
  };
  observableObject.addPropertyChangedListener("StringProp", cb1);
  observableObject.set_stringProp("1");
  QUnit.ok(strChanged, "change callback called");
  strChanged = false;
  cbCalled = false;
  observableObject.set_intProp(1);
  QUnit.ok(!strChanged, "String change callback not called.");
  QUnit.ok(!cbCalled, "Callback should not be called for different property change");
};
function Sunlight__Framework__Test__ObservableObjectTests__TestRemovePropertyChangeCallback() {
  var observableObject, cbCalled, cb1;
  observableObject = Sunlight__Framework__Test__ObservableObjectTests$2fObservableTestObject_factory();
  cbCalled = false;
  cb1 = function Sunlight__Framework__Test__ObservableObjectTests__TestRemovePropertyChangeCallback_del(sender, propName) {
    return cbCalled = true;
  };
  observableObject.addPropertyChangedListener("StringProp", cb1);
  observableObject.set_stringProp("1");
  QUnit.ok(cbCalled, "change callback called");
  cbCalled = false;
  observableObject.removePropertyChangedListener("StringProp", cb1);
  observableObject.set_stringProp("2");
  QUnit.ok(!cbCalled, "after removing change listner, callback should not be called.");
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_ObservableObjectTests, "Sunlight.Framework.Test.ObservableObjectTests", Object, []);
function Sunlight_Framework_Binders_ISourceDataBinder() {
};
Sunlight_Framework_Binders_ISourceDataBinder.typeId = "g";
System__Type__RegisterInterface(Sunlight_Framework_Binders_ISourceDataBinder, "Sunlight.Framework.Binders.ISourceDataBinder");
function Sunlight_Framework_Binders_ITargetDataBinder() {
};
Sunlight_Framework_Binders_ITargetDataBinder.typeId = "h";
System__Type__RegisterInterface(Sunlight_Framework_Binders_ITargetDataBinder, "Sunlight.Framework.Binders.ITargetDataBinder");
function Sunlight_Framework_Binders_DataBinder() {
};
Sunlight_Framework_Binders_DataBinder.typeId = "q";
function Sunlight__Framework__Binders__DataBinder_factory(sourceBinder, targetBinder, dataBindingMode, converter) {
  var this_;
  this_ = new Sunlight_Framework_Binders_DataBinder();
  this_.__ctor(sourceBinder, targetBinder, dataBindingMode, converter);
  return this_;
};
ptyp_ = Sunlight_Framework_Binders_DataBinder.prototype;
ptyp_.sourceBinder = null;
ptyp_.targetBinder = null;
ptyp_.converter = null;
ptyp_.bindingMode = 0;
ptyp_.firstBindingSuccessful = false;
ptyp_.__ctor = function Sunlight__Framework__Binders__DataBinder____ctor(sourceBinder, targetBinder, dataBindingMode, converter) {
  this.sourceBinder = sourceBinder;
  this.targetBinder = targetBinder;
  this.bindingMode = dataBindingMode;
  this.converter = converter;
  this.targetBinder.useDataBinder(this);
  this.sourceBinder.useDataBinder(this);
};
ptyp_.setTarget = function Sunlight__Framework__Binders__DataBinder__SetTarget(target) {
  this.targetBinder.set_target(target);
  this.applyBinding();
};
ptyp_.setSource = function Sunlight__Framework__Binders__DataBinder__SetSource(source) {
  this.sourceBinder.set_source(source);
};
ptyp_.sourceValueUpdated = function Sunlight__Framework__Binders__DataBinder__SourceValueUpdated() {
  this.applyBinding();
};
ptyp_.targetValueUpdated = function Sunlight__Framework__Binders__DataBinder__TargetValueUpdated() {
  this.applyBackBinding();
};
ptyp_.applyBinding = function Sunlight__Framework__Binders__DataBinder__ApplyBinding() {
  if (this.targetBinder.get_isActive()) {
    if (this.bindingMode === 0) {
      if (!this.firstBindingSuccessful)
        if (this.sourceBinder.get_isActive()) {
          this.targetBinder.set_value(this.converter === null ? this.sourceBinder.get_value() : this.converter.V_Convert_f(this.sourceBinder.get_value()));
          this.firstBindingSuccessful = true;
        }
        else
          this.targetBinder.setDefault();
      return;
    }
    if (this.sourceBinder.get_isActive()) {
      if (this.converter !== null)
        this.targetBinder.set_value(this.converter.V_Convert_f(this.sourceBinder.get_value()));
      else
        this.targetBinder.set_value(this.sourceBinder.get_value());
    }
    else
      this.targetBinder.setDefault();
  }
};
ptyp_.applyBackBinding = function Sunlight__Framework__Binders__DataBinder__ApplyBackBinding() {
  if (!this.targetBinder.get_isWriteOnly())
    if (this.sourceBinder.get_isActive())
      this.sourceBinder.set_value(this.targetBinder.get_value());
};
ptyp_.V_SourceValueUpdated_g = ptyp_.sourceValueUpdated;
ptyp_.V_TargetValueUpdated_h = ptyp_.targetValueUpdated;
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_DataBinder, "Sunlight.Framework.Binders.DataBinder", Object, [Sunlight_Framework_Binders_ISourceDataBinder, Sunlight_Framework_Binders_ITargetDataBinder]);
function Sunlight_Framework_Binders_SourcePropertyBinder() {
};
Sunlight_Framework_Binders_SourcePropertyBinder.typeId = "r";
function Sunlight__Framework__Binders__SourcePropertyBinder_factory(propertyPartNames, propertyGetterChain, propertySetter) {
  var this_;
  this_ = new Sunlight_Framework_Binders_SourcePropertyBinder();
  this_.__ctor(propertyPartNames, propertyGetterChain, propertySetter);
  return this_;
};
ptyp_ = Sunlight_Framework_Binders_SourcePropertyBinder.prototype;
ptyp_.chainLength = 0;
ptyp_.propertyPartNames = null;
ptyp_.propertyGetterChain = null;
ptyp_.changeRegistrations = null;
ptyp_.objectChain = null;
ptyp_.propertySetter = null;
ptyp_.dataBinderBase = null;
ptyp_.value = null;
ptyp_.isActive = false;
ptyp_.__ctor = function Sunlight__Framework__Binders__SourcePropertyBinder____ctor(propertyPartNames, propertyGetterChain, propertySetter) {
  var i;
  this.propertyPartNames = propertyPartNames;
  this.chainLength = this.propertyPartNames.V_get_Length();
  this.propertyGetterChain = propertyGetterChain;
  this.propertySetter = propertySetter;
  this.objectChain = System_ArrayG_$Object$_.__ctora(this.chainLength);
  this.changeRegistrations = System_ArrayG_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_.__ctora(this.chainLength);
  for (i = this.chainLength - 1; i >= 0; i--)
    this.changeRegistrations.set_item(i, this.getChangeTrackerAt(i));
};
ptyp_.set_source = function Sunlight__Framework__Binders__SourcePropertyBinder__set_Source(value) {
  if (this.objectChain.get_item(0) !== value) {
    this.setObjectChainElementValue(0, value);
    this.calculateValueFrom(0);
  }
};
ptyp_.get_value = function Sunlight__Framework__Binders__SourcePropertyBinder__get_Value() {
  return this.value;
};
ptyp_.set_value = function Sunlight__Framework__Binders__SourcePropertyBinder__set_Value(value) {
  if (this.isActive && this.propertySetter !== null)
    this.propertySetter(this.objectChain.get_item(this.objectChain.V_get_Length() - 1), value);
};
ptyp_.get_isActive = function Sunlight__Framework__Binders__SourcePropertyBinder__get_IsActive() {
  return this.isActive;
};
ptyp_.useDataBinder = function Sunlight__Framework__Binders__SourcePropertyBinder__UseDataBinder(dataBinderBase) {
  this.dataBinderBase = dataBinderBase;
};
ptyp_.calculateValueFrom = function Sunlight__Framework__Binders__SourcePropertyBinder__CalculateValueFrom(index) {
  var i, j, nextValue;
  for (i = index; i < this.chainLength; i++)
    if (this.objectChain.get_item(i) === null) {
      for (j = this.chainLength - 1; j >= i; j--)
        this.setObjectChainElementValue(j, null);
      if (this.value !== null || this.isActive) {
        this.isActive = false;
        this.value = null;
        if (this.dataBinderBase !== null)
          this.dataBinderBase.V_SourceValueUpdated_g();
      }
      break;
    }
    else {
      nextValue = this.propertyGetterChain.get_item(i)(this.objectChain.get_item(i));
      if (i < this.objectChain.V_get_Length() - 1)
        this.setObjectChainElementValue(i + 1, nextValue);
      else if (this.value !== nextValue || !this.isActive) {
        this.isActive = true;
        this.value = nextValue;
        if (this.dataBinderBase !== null)
          this.dataBinderBase.V_SourceValueUpdated_g();
      }
    }
};
ptyp_.setObjectChainElementValue = function Sunlight__Framework__Binders__SourcePropertyBinder__SetObjectChainElementValue(index, value) {
  var oldValue, newNotifiableValue;
  if (index > this.chainLength)
    return;
  oldValue = System__Type__AsType(Sunlight_Framework_Observables_INotifyPropertyChanged, this.objectChain.get_item(index));
  if (oldValue !== null)
    oldValue.V_RemovePropertyChangedListener_e(this.propertyPartNames.get_item(index), this.changeRegistrations.get_item(index));
  this.objectChain.set_item(index, value);
  newNotifiableValue = System__Type__AsType(Sunlight_Framework_Observables_INotifyPropertyChanged, value);
  if (newNotifiableValue !== null)
    newNotifiableValue.V_AddPropertyChangedListener_e(this.propertyPartNames.get_item(index), this.changeRegistrations.get_item(index));
};
ptyp_.getChangeTrackerAt = function Sunlight__Framework__Binders__SourcePropertyBinder__GetChangeTrackerAt(index) {
  var this_;
  this_ = this;
  return function Sunlight__Framework__Binders__SourcePropertyBinder__GetChangeTrackerAt_del(sender, prop) {
    this_.calculateValueFrom(index);
  };
};
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_SourcePropertyBinder, "Sunlight.Framework.Binders.SourcePropertyBinder", Object, []);
function System_ValueType() {
};
System_ValueType.typeId = "s";
ptyp_ = System_ValueType.prototype;
ptyp_.boxedValue = null;
System__Type__RegisterReferenceType(System_ValueType, "System.ValueType", Object, []);
function System_Int32(boxedValue) {
  this.boxedValue = boxedValue;
};
System_Int32.typeId = "d";
System_Int32.getDefaultValue = function() {
  return 0;
};
System_Int32.prototype = new System_ValueType();
System__Type__RegisterStructType(System_Int32, "System.Int32", []);
function Sunlight_Framework_Test_Binders_SimpleObjectWithProperty() {
};
Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.typeId = "t";
function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty_factory() {
  return new Sunlight_Framework_Test_Binders_SimpleObjectWithProperty();
};
Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.defaultConstructor = Sunlight__Framework__Test__Binders__SimpleObjectWithProperty_factory;
ptyp_ = Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.prototype;
ptyp_.set_stringProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__set_StringProp(value) {
  return this.StringProp = value;
};
ptyp_.get_intProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__get_IntProp() {
  return this.IntProp;
};
ptyp_.set_intProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__set_IntProp(value) {
  return this.IntProp = value;
};
ptyp_.get_selfProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__get_SelfProp() {
  return this.SelfProp;
};
ptyp_.set_selfProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__set_SelfProp(value) {
  return this.SelfProp = value;
};
ptyp_.get_notifiableProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__get_NotifiableProp() {
  return this.NotifiableProp;
};
ptyp_.set_notifiableProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__set_NotifiableProp(value) {
  return this.NotifiableProp = value;
};
ptyp_.__ctor = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty____ctor() {
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_Binders_SimpleObjectWithProperty, "Sunlight.Framework.Test.Binders.SimpleObjectWithProperty", Object, []);
function Sunlight_Framework_Binders_TargetBinder() {
};
Sunlight_Framework_Binders_TargetBinder.typeId = "u";
function Sunlight__Framework__Binders__TargetBinder_factory(propertyName, getter, setter, defaultValue) {
  var this_;
  this_ = new Sunlight_Framework_Binders_TargetBinder();
  this_.__ctor(propertyName, getter, setter, defaultValue);
  return this_;
};
ptyp_ = Sunlight_Framework_Binders_TargetBinder.prototype;
ptyp_.value = null;
ptyp_.target = null;
ptyp_.getter = null;
ptyp_.setter = null;
ptyp_.propertyName = null;
ptyp_.defaultValue = null;
ptyp_.dataBinder = null;
ptyp_.__ctor = function Sunlight__Framework__Binders__TargetBinder____ctor(propertyName, getter, setter, defaultValue) {
  this.propertyName = propertyName;
  this.getter = getter;
  this.setter = setter;
  this.defaultValue = defaultValue;
};
ptyp_.get_value = function Sunlight__Framework__Binders__TargetBinder__get_Value() {
  return this.value;
};
ptyp_.set_value = function Sunlight__Framework__Binders__TargetBinder__set_Value(value) {
  if (this.value !== value && this.setter !== null) {
    this.value = value;
    if (this.get_isActive())
      this.setter(this.target, this.value);
  }
};
ptyp_.get_isWriteOnly = function Sunlight__Framework__Binders__TargetBinder__get_IsWriteOnly() {
  return this.getter === null;
};
ptyp_.get_isActive = function Sunlight__Framework__Binders__TargetBinder__get_IsActive() {
  return this.target !== null;
};
ptyp_.set_target = function Sunlight__Framework__Binders__TargetBinder__set_Target(value) {
  if (this.target !== value) {
    if (this.target !== null)
      this.target.V_RemovePropertyChangedListener_e(this.propertyName, System__Delegate__Create("onTargetUpdated", this));
    this.target = value;
    this.value = null;
    if (this.target !== null) {
      this.target.V_AddPropertyChangedListener_e(this.propertyName, System__Delegate__Create("onTargetUpdated", this));
      if (this.getter !== null)
        this.value = this.getter(this.target);
    }
  }
};
ptyp_.setDefault = function Sunlight__Framework__Binders__TargetBinder__SetDefault() {
  this.set_value(this.defaultValue);
};
ptyp_.useDataBinder = function Sunlight__Framework__Binders__TargetBinder__UseDataBinder(dataBinder) {
  this.dataBinder = dataBinder;
};
ptyp_.onTargetUpdated = function Sunlight__Framework__Binders__TargetBinder__OnTargetUpdated(sender, propertyName) {
  if (this.dataBinder !== null && this.getter !== null) {
    this.value = this.getter(this.target);
    this.dataBinder.V_TargetValueUpdated_h();
  }
};
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_TargetBinder, "Sunlight.Framework.Binders.TargetBinder", Object, []);
function Sunlight_Framework_Observables_INotifyPropertyChanged() {
};
Sunlight_Framework_Observables_INotifyPropertyChanged.typeId = "e";
System__Type__RegisterInterface(Sunlight_Framework_Observables_INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function Sunlight_Framework_Observables_ObservableObject() {
};
Sunlight_Framework_Observables_ObservableObject.typeId = "v";
ptyp_ = Sunlight_Framework_Observables_ObservableObject.prototype;
ptyp_.eventHandlers = null;
ptyp_.addPropertyChangedListener = function Sunlight__Framework__Observables__ObservableObject__AddPropertyChangedListener(propertyName, callback) {
  var cb;
  if (this.eventHandlers === null)
    this.eventHandlers = System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_.defaultConstructor();
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
  if (this.eventHandlers === null)
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
    if (cb !== null)
      this.eventHandlers.set_item(propertyName, cb);
    else
      this.eventHandlers.remove(propertyName);
  }
};
ptyp_.firePropertyChanged = function Sunlight__Framework__Observables__ObservableObject__FirePropertyChanged(propertyName) {
  var cb;
  if (this.eventHandlers !== null) {
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
ptyp_.V_AddPropertyChangedListener_e = ptyp_.addPropertyChangedListener;
ptyp_.V_RemovePropertyChangedListener_e = ptyp_.removePropertyChangedListener;
System__Type__RegisterReferenceType(Sunlight_Framework_Observables_ObservableObject, "Sunlight.Framework.Observables.ObservableObject", Object, [Sunlight_Framework_Observables_INotifyPropertyChanged]);
function Sunlight_Framework_Test_Binders_SimpleNotifiableClass() {
};
Sunlight_Framework_Test_Binders_SimpleNotifiableClass.typeId = "w";
function Sunlight__Framework__Test__Binders__SimpleNotifiableClass_factory() {
  var this_;
  this_ = new Sunlight_Framework_Test_Binders_SimpleNotifiableClass();
  this_.__ctora();
  return this_;
};
Sunlight_Framework_Test_Binders_SimpleNotifiableClass.defaultConstructor = Sunlight__Framework__Test__Binders__SimpleNotifiableClass_factory;
ptyp_ = new Sunlight_Framework_Observables_ObservableObject();
Sunlight_Framework_Test_Binders_SimpleNotifiableClass.prototype = ptyp_;
ptyp_.strField = null;
ptyp_.intField = 0;
ptyp_.selfField = null;
ptyp_.objField = null;
ptyp_.set_strProp = function Sunlight__Framework__Test__Binders__SimpleNotifiableClass__set_StrProp(value) {
  if (this.strField !== value) {
    this.strField = value;
    this.firePropertyChanged("StrProp");
  }
};
ptyp_.get_intProp = function Sunlight__Framework__Test__Binders__SimpleNotifiableClass__get_IntProp() {
  return this.intField;
};
ptyp_.set_intProp = function Sunlight__Framework__Test__Binders__SimpleNotifiableClass__set_IntProp(value) {
  if (this.intField !== value) {
    this.intField = value;
    this.firePropertyChanged("IntProp");
  }
};
ptyp_.set_selfProp = function Sunlight__Framework__Test__Binders__SimpleNotifiableClass__set_SelfProp(value) {
  if (this.selfField !== value) {
    this.selfField = value;
    this.firePropertyChanged("SelfProp");
  }
};
ptyp_.get_objProp = function Sunlight__Framework__Test__Binders__SimpleNotifiableClass__get_ObjProp() {
  return this.objField;
};
ptyp_.set_objProp = function Sunlight__Framework__Test__Binders__SimpleNotifiableClass__set_ObjProp(value) {
  if (this.objField !== value) {
    this.objField = value;
    this.firePropertyChanged("ObjProp");
  }
};
ptyp_.__ctora = function Sunlight__Framework__Test__Binders__SimpleNotifiableClass____ctor() {
  this.__ctor();
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_Binders_SimpleNotifiableClass, "Sunlight.Framework.Test.Binders.SimpleNotifiableClass", Sunlight_Framework_Observables_ObservableObject, []);
String.typeId = "x";
System__String__formatHelperRegex = null;
System__String__trimStartHelperRegex = null;
System__String__trimEndHelperRegex = null;
function System__String____cctor() {
  System__String__formatHelperRegex = new RegExp("(\\\\{[^\\\\}^\\\\{]+\\\\})", "g");
  System__String__trimStartHelperRegex = new RegExp("^\\\\s*");
  System__String__trimEndHelperRegex = new RegExp("\\\\s*$");
};
System__Type__RegisterReferenceType(String, "System.String", Object, []);
function System_ArrayImpl() {
};
System_ArrayImpl.typeId = "y";
ptyp_ = System_ArrayImpl.prototype;
ptyp_.__ctor = function System__ArrayImpl____ctor() {
};
System__Type__RegisterReferenceType(System_ArrayImpl, "System.ArrayImpl", Object, []);
RegExp.typeId = "z";
System__Type__RegisterReferenceType(RegExp, "System.RegularExpression", Object, []);
function System_Delegate() {
};
System_Delegate.typeId = "A";
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
function System__Delegate__CreateGeneric(functionName, instanceOrType, f, genericArguments) {
  var fn, i;
  fn = "__@" + functionName;
  for (i = 0; i < genericArguments.length; ++i)
    fn = fn + "_@" + genericArguments[i].typeId + "@";
  if (!(fn in instanceOrType)) {
    instanceOrType[fn] = function() {
        return f.apply(instanceOrType, genericArguments.concat(Array.prototype.slice.call(arguments)));
    };
    instanceOrType[fn].fullName = instanceOrType.constructor.toString() + "::" + functionName;
    instanceOrType[fn].isDelegate = true;
  }
  return instanceOrType[fn];
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
System__Type__RegisterReferenceType(System_Delegate, "System.Delegate", Object, []);
function System_MulticastDelegate() {
};
System_MulticastDelegate.typeId = "bb";
System_MulticastDelegate.prototype = new System_Delegate();
System__Type__RegisterReferenceType(System_MulticastDelegate, "System.MulticastDelegate", System_Delegate, []);
function Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper() {
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper.typeId = "bc";
function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper_factory() {
  return new Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper();
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper.defaultConstructor = Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper_factory;
ptyp_ = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper.prototype;
ptyp_.get_sourceValueUpdateCalled = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper__get_SourceValueUpdateCalled() {
  return this.SourceValueUpdateCalled;
};
ptyp_.set_sourceValueUpdateCalled = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper__set_SourceValueUpdateCalled(value) {
  return this.SourceValueUpdateCalled = value;
};
ptyp_.sourceValueUpdated = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper__SourceValueUpdated() {
  this.set_sourceValueUpdateCalled(true);
};
ptyp_.__ctor = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper____ctor() {
};
ptyp_.V_SourceValueUpdated_g = ptyp_.sourceValueUpdated;
System__Type__RegisterReferenceType(Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper, "Sunlight.Framework.Test.Binders.SourcePropertyBinderTests/BinderTestHelper", Object, [Sunlight_Framework_Binders_ISourceDataBinder]);
function Sunlight_Framework_IocContainer() {
};
Sunlight_Framework_IocContainer.typeId = "bd";
function Sunlight__Framework__IocContainer_factory() {
  var this_;
  this_ = new Sunlight_Framework_IocContainer();
  this_.__ctor();
  return this_;
};
Sunlight_Framework_IocContainer.defaultConstructor = Sunlight__Framework__IocContainer_factory;
ptyp_ = Sunlight_Framework_IocContainer.prototype;
ptyp_.factoryMap = null;
ptyp_.resolutionCount = 0;
ptyp_.register = function Sunlight__Framework__IocContainer__Register(T, factory) {
  var this_, typeRegistry;
  this_ = this;
  typeRegistry = Sunlight__Framework__TypeRegistry_factory(factory);
  this_.factoryMap.set_item(T.typeId, typeRegistry);
  return Sunlight__Framework__IoC__IocHelper_factory(function Sunlight__Framework__IocContainer__Register_del() {
    typeRegistry.set_isStatic(true);
  }, function Sunlight__Framework__IocContainer__Register_del2(type) {
    this_.factoryMap.set_item(type.typeId, typeRegistry);
  });
};
ptyp_.resolve = function Sunlight__Framework__IocContainer__Resolve(T) {
  var typeId;
  if (this.resolutionCount > 100)
    throw new Error("Ioc has cycles, use ResolveLazy<T> to break cycle");
  this.resolutionCount++;
  typeId = T.typeId;
  try {
    return this.factoryMap.get_item(T.typeId).getValue();
  } finally {
    this.resolutionCount--;
  }
};
ptyp_.resolveLazy = function Sunlight__Framework__IocContainer__ResolveLazy(T) {
  var Lazy_$T$_;
  Lazy_$T$_ = Sunlight_Framework_Lazy(T, true);
  return Lazy_$T$_.__ctor(System__Delegate__CreateGeneric("getValue", this.factoryMap.get_item(T.typeId), this.factoryMap.get_item(T.typeId).getValue, [T]));
};
ptyp_.__ctor = function Sunlight__Framework__IocContainer____ctor() {
  this.factoryMap = System_Collections_Generic_StringDictionary_$TypeRegistry$_.defaultConstructor();
};
System__Type__RegisterReferenceType(Sunlight_Framework_IocContainer, "Sunlight.Framework.IocContainer", Object, []);
function Sunlight_Framework_Test_IocTestType2() {
};
Sunlight_Framework_Test_IocTestType2.typeId = "be";
function Sunlight__Framework__Test__IocTestType2_factory(x) {
  var this_;
  this_ = new Sunlight_Framework_Test_IocTestType2();
  this_.__ctor(x);
  return this_;
};
ptyp_ = Sunlight_Framework_Test_IocTestType2.prototype;
ptyp_.x = 0;
ptyp_.__ctor = function Sunlight__Framework__Test__IocTestType2____ctor(x) {
  this.x = x;
};
ptyp_.testMethod = function Sunlight__Framework__Test__IocTestType2__TestMethod() {
  return this.x;
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_IocTestType2, "Sunlight.Framework.Test.IocTestType2", Object, []);
function Sunlight_Framework_Test_IocTestType1Base() {
};
Sunlight_Framework_Test_IocTestType1Base.typeId = "bf";
function Sunlight__Framework__Test__IocTestType1Base_factory(x) {
  var this_;
  this_ = new Sunlight_Framework_Test_IocTestType1Base();
  this_.__ctor(x);
  return this_;
};
ptyp_ = Sunlight_Framework_Test_IocTestType1Base.prototype;
ptyp_.x = 0;
ptyp_.__ctor = function Sunlight__Framework__Test__IocTestType1Base____ctor(x) {
  this.x = x;
};
ptyp_.testMethodBase = function Sunlight__Framework__Test__IocTestType1Base__TestMethodBase() {
  return this.x;
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_IocTestType1Base, "Sunlight.Framework.Test.IocTestType1Base", Object, []);
function Sunlight_Framework_Test_IIocTestType1() {
};
Sunlight_Framework_Test_IIocTestType1.typeId = "b";
System__Type__RegisterInterface(Sunlight_Framework_Test_IIocTestType1, "Sunlight.Framework.Test.IIocTestType1");
function Sunlight_Framework_Test_IocTestType1() {
};
Sunlight_Framework_Test_IocTestType1.typeId = "bg";
function Sunlight__Framework__Test__IocTestType1_factory(x, y) {
  var this_;
  this_ = new Sunlight_Framework_Test_IocTestType1();
  this_.__ctora(x, y);
  return this_;
};
ptyp_ = new Sunlight_Framework_Test_IocTestType1Base();
Sunlight_Framework_Test_IocTestType1.prototype = ptyp_;
ptyp_.y = 0;
ptyp_.__ctora = function Sunlight__Framework__Test__IocTestType1____ctor(x, y) {
  this.__ctor(x);
  this.y = y;
};
ptyp_.testMethod = function Sunlight__Framework__Test__IocTestType1__TestMethod() {
  return this.y + this.testMethodBase();
};
ptyp_.V_TestMethod_b = ptyp_.testMethod;
System__Type__RegisterReferenceType(Sunlight_Framework_Test_IocTestType1, "Sunlight.Framework.Test.IocTestType1", Sunlight_Framework_Test_IocTestType1Base, [Sunlight_Framework_Test_IIocTestType1]);
function Sunlight_Framework_IoC_IocHelper() {
};
Sunlight_Framework_IoC_IocHelper.typeId = "bh";
function Sunlight__Framework__IoC__IocHelper_factory(isSingleton, registerAs) {
  var this_;
  this_ = new Sunlight_Framework_IoC_IocHelper();
  this_.__ctor(isSingleton, registerAs);
  return this_;
};
ptyp_ = Sunlight_Framework_IoC_IocHelper.prototype;
ptyp_.isSingletona = null;
ptyp_.registerAs = null;
ptyp_.__ctor = function Sunlight__Framework__IoC__IocHelper____ctor(isSingleton, registerAs) {
  this.registerAs = registerAs;
  this.isSingletona = isSingleton;
};
ptyp_.as = function Sunlight__Framework__IoC__IocHelper__As(T) {
  this.registerAs(T);
  return this;
};
ptyp_.isSingleton = function Sunlight__Framework__IoC__IocHelper__IsSingleton() {
  this.isSingletona();
  return this;
};
System__Type__RegisterReferenceType(Sunlight_Framework_IoC_IocHelper, "Sunlight.Framework.IoC.IocHelper", Object, []);
function Sunlight_Framework_EventBus() {
};
Sunlight_Framework_EventBus.typeId = "bi";
function Sunlight__Framework__EventBus_factory() {
  var this_;
  this_ = new Sunlight_Framework_EventBus();
  this_.__ctor();
  return this_;
};
Sunlight_Framework_EventBus.defaultConstructor = Sunlight__Framework__EventBus_factory;
ptyp_ = Sunlight_Framework_EventBus.prototype;
ptyp_.eventSubscriptsion = null;
ptyp_.oneTimeValues = null;
ptyp_.subscribe = function Sunlight__Framework__EventBus__Subscribe(T, callback) {
  var typeId, evt, registeredCallback;
  Sunlight__Framework__ExceptionHelpers__ThrowOnArgumentNull(callback, "callback");
  typeId = T.typeId;
  evt = this.oneTimeValues[typeId];
  if (!System__Object__IsNullOrUndefined(evt))
    callback(T.unbox(evt));
  if (!this.eventSubscriptsion.tryGetValue(typeId, {
    read: function() {
      return registeredCallback;
    },
    write: function(arg0) {
      return registeredCallback = arg0;
    }
  })) {
    registeredCallback = callback;
    this.eventSubscriptsion.set_item(typeId, registeredCallback);
  }
  else
    registeredCallback = System__Delegate__Combine(registeredCallback, System__Type__CastType(System_Delegate, callback));
};
ptyp_.unSubscribe = function Sunlight__Framework__EventBus__UnSubscribe(T, callback) {
  var typeId, registeredCallback, act;
  Sunlight__Framework__ExceptionHelpers__ThrowOnArgumentNull(callback, "callback");
  typeId = T.typeId;
  if (this.eventSubscriptsion.tryGetValue(typeId, {
    read: function() {
      return registeredCallback;
    },
    write: function(arg0) {
      return registeredCallback = arg0;
    }
  })) {
    act = registeredCallback;
    act = System__Delegate__Remove(act, callback);
    if (act === null)
      this.eventSubscriptsion.remove(typeId);
  }
};
ptyp_.raise = function Sunlight__Framework__EventBus__Raise(T, evt) {
  var typeId, registeredCallback;
  Sunlight__Framework__ExceptionHelpers__ThrowOnArgumentNull(T.box(evt), "evt");
  typeId = T.typeId;
  if (this.eventSubscriptsion.tryGetValue(typeId, {
    read: function() {
      return registeredCallback;
    },
    write: function(arg0) {
      return registeredCallback = arg0;
    }
  }))
    registeredCallback(evt);
};
ptyp_.raiseOneTime = function Sunlight__Framework__EventBus__RaiseOneTime(T, evt) {
  var typeId, alreadyRegistered;
  Sunlight__Framework__ExceptionHelpers__ThrowOnArgumentNull(T.box(evt), "evt");
  typeId = T.typeId;
  alreadyRegistered = this.oneTimeValues[typeId];
  if (!System__Object__IsNullOrUndefined(alreadyRegistered))
    throw new Error("Event " + T.name + " already raised");
  this.oneTimeValues[typeId] = T.box(evt);
  this.raise(T, evt);
  this.eventSubscriptsion.remove(typeId);
};
ptyp_.__ctor = function Sunlight__Framework__EventBus____ctor() {
  this.eventSubscriptsion = System_Collections_Generic_StringDictionary_$Delegate$_.defaultConstructor();
  this.oneTimeValues = new Object();
};
System__Type__RegisterReferenceType(Sunlight_Framework_EventBus, "Sunlight.Framework.EventBus", Object, []);
function Sunlight_Framework_Test_EventBusTests_Evt1() {
};
Sunlight_Framework_Test_EventBusTests_Evt1.typeId = "bj";
function Sunlight__Framework__Test__EventBusTests$2fEvt1_factory() {
  return new Sunlight_Framework_Test_EventBusTests_Evt1();
};
Sunlight_Framework_Test_EventBusTests_Evt1.defaultConstructor = Sunlight__Framework__Test__EventBusTests$2fEvt1_factory;
ptyp_ = Sunlight_Framework_Test_EventBusTests_Evt1.prototype;
ptyp_.x = 0;
ptyp_.__ctor = function Sunlight__Framework__Test__EventBusTests$2fEvt1____ctor() {
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_EventBusTests_Evt1, "Sunlight.Framework.Test.EventBusTests/Evt1", Object, []);
function Sunlight_Framework_Test_EventBusTests_Evt2(boxedValue) {
  this.boxedValue = boxedValue;
};
Sunlight_Framework_Test_EventBusTests_Evt2.typeId = "bk";
Sunlight_Framework_Test_EventBusTests_Evt2.getDefaultValue = function() {
  return {
    x: 0
  };
};
Sunlight_Framework_Test_EventBusTests_Evt2.prototype = new System_ValueType();
System__Type__RegisterStructType(Sunlight_Framework_Test_EventBusTests_Evt2, "Sunlight.Framework.Test.EventBusTests/Evt2", []);
function Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject() {
};
Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject.typeId = "bl";
function Sunlight__Framework__Test__ObservableObjectTests$2fObservableTestObject_factory() {
  var this_;
  this_ = new Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject();
  this_.__ctora();
  return this_;
};
Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject.defaultConstructor = Sunlight__Framework__Test__ObservableObjectTests$2fObservableTestObject_factory;
ptyp_ = new Sunlight_Framework_Observables_ObservableObject();
Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject.prototype = ptyp_;
ptyp_.strField = null;
ptyp_.intProp = 0;
ptyp_.set_intProp = function Sunlight__Framework__Test__ObservableObjectTests$2fObservableTestObject__set_IntProp(value) {
  if (value !== this.intProp) {
    this.intProp = value;
    this.firePropertyChanged("IntProp");
  }
};
ptyp_.set_stringProp = function Sunlight__Framework__Test__ObservableObjectTests$2fObservableTestObject__set_StringProp(value) {
  if (value !== this.strField) {
    this.strField = value;
    this.firePropertyChanged("StringProp");
  }
};
ptyp_.__ctora = function Sunlight__Framework__Test__ObservableObjectTests$2fObservableTestObject____ctor() {
  this.__ctor();
};
System__Type__RegisterReferenceType(Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject, "Sunlight.Framework.Test.ObservableObjectTests/ObservableTestObject", Sunlight_Framework_Observables_ObservableObject, []);
function Sunlight_Framework_TypeRegistry() {
};
Sunlight_Framework_TypeRegistry.typeId = "bm";
function Sunlight__Framework__TypeRegistry_factory(factory) {
  var this_;
  this_ = new Sunlight_Framework_TypeRegistry();
  this_.__ctor(factory);
  return this_;
};
ptyp_ = Sunlight_Framework_TypeRegistry.prototype;
ptyp_.factory = null;
ptyp_.isCreated = false;
ptyp_.isStatic = false;
ptyp_.value = null;
ptyp_.__ctor = function Sunlight__Framework__TypeRegistry____ctor(factory) {
  this.factory = factory;
};
ptyp_.set_isStatic = function Sunlight__Framework__TypeRegistry__set_IsStatic(value) {
  this.isStatic = value;
};
ptyp_.getValue = function Sunlight__Framework__TypeRegistry__GetValue() {
  if (!this.isStatic)
    return this.factory();
  if (!this.isCreated) {
    this.value = this.factory();
    this.isCreated = true;
  }
  return this.value;
};
System__Type__RegisterReferenceType(Sunlight_Framework_TypeRegistry, "Sunlight.Framework.TypeRegistry", Object, []);
Error.typeId = "bn";
System__Type__RegisterReferenceType(Error, "System.Exception", Object, []);
Object.typeId = "bo";
System__Type__RegisterReferenceType(Object, "System.Collections.Dictionary", Object, []);
function Sunlight_Framework_ExceptionHelpers() {
};
function Sunlight__Framework__ExceptionHelpers__ThrowOnOutOfRange(value, minValue, maxValue, argumentName) {
  if (value < minValue || value > maxValue)
    throw new Error("Out of range exception: " + argumentName);
};
function Sunlight__Framework__ExceptionHelpers__ThrowOnArgumentNull(value, argumentName) {
  if (value === null)
    throw new Error("ArgumentNull: " + argumentName);
};
function System_Function(boxedValue) {
  this.boxedValue = boxedValue;
};
System_Function.typeId = "bp";
System_Function.getDefaultValue = function() {
  return {
    length: 0
  };
};
System_Function.prototype = new System_ValueType();
System__Type__RegisterStructType(System_Function, "System.Function", []);
Array.typeId = "bq";
function System__NativeArray__Push(this_, value) {
  return this_.push(value);
};
function System__NativeArray__RemoveAt(this_, index) {
  var i;
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  for (i = this_.length - 2; i >= index; i--)
    this_[i] = this_[i + 1];
  this_.pop();
};
function System__NativeArray__SetAt(this_, index, value) {
  this_[index] = value;
};
System__Type__RegisterReferenceType(Array, "System.NativeArray", Object, []);
function Sunlight_Framework_Binders_IValueConverter() {
};
Sunlight_Framework_Binders_IValueConverter.typeId = "f";
System__Type__RegisterInterface(Sunlight_Framework_Binders_IValueConverter, "Sunlight.Framework.Binders.IValueConverter");
function System_ArrayG(T, $5fcallStatiConstructor) {
  var ArrayG$1_$T$_, ICollection$1_$T$_, $5f_initTracker;
  if (System_ArrayG[T.typeId])
    return System_ArrayG[T.typeId];
  System_ArrayG[T.typeId] = function System__ArrayG$1a() {
  };
  ArrayG$1_$T$_ = System_ArrayG[T.typeId];
  ArrayG$1_$T$_.genericParameters = [T];
  ArrayG$1_$T$_.genericClosure = System_ArrayG;
  ArrayG$1_$T$_.typeId = "br$" + T.typeId + "$";
  ICollection$1_$T$_ = System_Collections_Generic_ICollection(T, $5fcallStatiConstructor);
  ArrayG$1_$T$_.__ctora = function System_ArrayG$1_factoryb(size) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctora(size);
    return this_;
  };
  ArrayG$1_$T$_.__ctor = function System_ArrayG$1_factoryc(nativeArray) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctorb(nativeArray);
    return this_;
  };
  ptyp_ = new System_ArrayImpl();
  ArrayG$1_$T$_.prototype = ptyp_;
  ptyp_.innerArray = null;
  ptyp_.__ctora = function System__ArrayG$1____ctora(size) {
    var def, i;
    this.__ctor();
    this.innerArray = new Array(size);
    def = T.getDefaultValue();
    for (i = 0; i < size; i++)
      System__NativeArray__SetAt(this.innerArray, i, def);
  };
  ptyp_.__ctorb = function System__ArrayG$1____ctor(nativeArray) {
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
  ptyp_.get_count = function System__ArrayG$1__get_Count() {
    return this.innerArray.length;
  };
  ptyp_.add = function System__ArrayG$1__Add(item) {
    throw new Error("Not Implemented.");
  };
  ptyp_.V_get_Length = ptyp_.get_length;
  ptyp_["V_get_Count_" + ICollection$1_$T$_.typeId] = ptyp_.get_count;
  ptyp_["V_Add_" + ICollection$1_$T$_.typeId] = ptyp_.add;
  System__Type__RegisterReferenceType(ArrayG$1_$T$_, "System.ArrayG`1<" + T.fullName + ">", System_ArrayImpl, [ICollection$1_$T$_]);
  ArrayG$1_$T$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    ArrayG$1_$T$_ = System_ArrayG(T, true);
  };
  if ($5fcallStatiConstructor)
    ArrayG$1_$T$_._tri();
  return ArrayG$1_$T$_;
};
function System_Func(T1, TRes, $5fcallStatiConstructor) {
  var Func$2_$T1_x_T1$_, $5f_initTracker;
  if (System_Func[T1.typeId] && System_Func[T1.typeId][TRes.typeId])
    return System_Func[T1.typeId][TRes.typeId];
    System_Func[T1.typeId] = {
    };
  System_Func[T1.typeId][TRes.typeId] = function System__Func$2a() {
  };
  Func$2_$T1_x_T1$_ = System_Func[T1.typeId][TRes.typeId];
  Func$2_$T1_x_T1$_.genericParameters = [T1, TRes];
  Func$2_$T1_x_T1$_.genericClosure = System_Func;
  Func$2_$T1_x_T1$_.typeId = "bs$" + T1.typeId + "_" + TRes.typeId + "$";
  Func$2_$T1_x_T1$_.prototype = new System_MulticastDelegate();
  System__Type__RegisterReferenceType(Func$2_$T1_x_T1$_, "System.Func`2<" + T1.fullName + "," + TRes.fullName + ">", System_MulticastDelegate, []);
  Func$2_$T1_x_T1$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T1 = T1;
    TRes = TRes;
    Func$2_$T1_x_T1$_ = System_Func(T1, TRes, true);
  };
  if ($5fcallStatiConstructor)
    Func$2_$T1_x_T1$_._tri();
  return Func$2_$T1_x_T1$_;
};
function Sunlight_Framework_Lazy(T, $5fcallStatiConstructor) {
  var Lazy$1_$T$_, $5f_initTracker;
  if (Sunlight_Framework_Lazy[T.typeId])
    return Sunlight_Framework_Lazy[T.typeId];
  Sunlight_Framework_Lazy[T.typeId] = function Sunlight__Framework__Lazy$1a() {
  };
  Lazy$1_$T$_ = Sunlight_Framework_Lazy[T.typeId];
  Lazy$1_$T$_.genericParameters = [T];
  Lazy$1_$T$_.genericClosure = Sunlight_Framework_Lazy;
  Lazy$1_$T$_.typeId = "bt$" + T.typeId + "$";
  Lazy$1_$T$_.__ctor = function Sunlight_Framework_Lazy$1_factorya(factory) {
    var this_;
    this_ = new Lazy$1_$T$_();
    this_.__ctor(factory);
    return this_;
  };
  ptyp_ = Lazy$1_$T$_.prototype;
  ptyp_.factory = null;
  ptyp_.value = T.getDefaultValue();
  ptyp_.createdCallbacks = null;
  ptyp_.__ctor = function Sunlight__Framework__Lazy$1____ctor(factory) {
    Sunlight__Framework__ExceptionHelpers__ThrowOnArgumentNull(factory, "factory");
    this.factory = factory;
  };
  ptyp_.get_value = function Sunlight__Framework__Lazy$1__get_Value() {
    if (this.factory !== null) {
      this.value = this.factory();
      this.factory = null;
      if (this.createdCallbacks !== null) {
        this.createdCallbacks();
        this.createdCallbacks = null;
      }
    }
    return this.value;
  };
  System__Type__RegisterReferenceType(Lazy$1_$T$_, "Sunlight.Framework.Lazy`1<" + T.fullName + ">", Object, []);
  Lazy$1_$T$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    Lazy$1_$T$_ = Sunlight_Framework_Lazy(T, true);
  };
  if ($5fcallStatiConstructor)
    Lazy$1_$T$_._tri();
  return Lazy$1_$T$_;
};
function Sunlight_Framework_Observables_ObservableCollection(T, $5fcallStatiConstructor) {
  var List$1_$T$_, ArrayG$1_$T$_, CollectionChangedEventArgs$1_$T$_, ObservableCollection$1_$T$_, $5f_initTracker, $5f_initTrackera;
  if (Sunlight_Framework_Observables_ObservableCollection[T.typeId])
    return Sunlight_Framework_Observables_ObservableCollection[T.typeId];
  Sunlight_Framework_Observables_ObservableCollection[T.typeId] = function Sunlight__Framework__Observables__ObservableCollection$1a() {
  };
  ObservableCollection$1_$T$_ = Sunlight_Framework_Observables_ObservableCollection[T.typeId];
  ObservableCollection$1_$T$_.genericParameters = [T];
  ObservableCollection$1_$T$_.genericClosure = Sunlight_Framework_Observables_ObservableCollection;
  ObservableCollection$1_$T$_.typeId = "bu$" + T.typeId + "$";
  ObservableCollection$1_$T$_.defaultConstructor = function Sunlight_Framework_Observables_ObservableCollection$1_factorya() {
    var this_;
    this_ = new ObservableCollection$1_$T$_();
    this_.__ctora();
    return this_;
  };
  ptyp_ = new Sunlight_Framework_Observables_ObservableObject();
  ObservableCollection$1_$T$_.prototype = ptyp_;
  ptyp_.busy = false;
  ptyp_.items = null;
  ptyp_.collectionChanged = null;
  ptyp_.__ctora = function Sunlight__Framework__Observables__ObservableCollection$1____ctor() {
    this.__ctor();
    this.items = List$1_$T$_.defaultConstructor();
  };
  ptyp_.add_CollectionChanged = function Sunlight__Framework__Observables__ObservableCollection$1__add_CollectionChanged(value) {
    this.collectionChanged = System__Delegate__Combine(this.collectionChanged, value);
  };
  ptyp_.get_count = function Sunlight__Framework__Observables__ObservableCollection$1__get_Count() {
    return this.items.get_count();
  };
  ptyp_.add = function Sunlight__Framework__Observables__ObservableCollection$1__Add(o) {
    this.checkReentrancy();
    this.items.add(o);
    this.onCollectionChanged(0, this.get_count() - 1, ArrayG$1_$T$_.__ctor([o]), null);
    this.firePropertyChanged("Count");
  };
  ptyp_.removeRangeAt = function Sunlight__Framework__Observables__ObservableCollection$1__RemoveRangeAt(removeIndex, count) {
    var itemsToRemove, index;
    Sunlight__Framework__ExceptionHelpers__ThrowOnOutOfRange(count, 1, this.items.get_count(), "count");
    Sunlight__Framework__ExceptionHelpers__ThrowOnOutOfRange(removeIndex, 0, this.items.get_count() - count, "removeIndex");
    this.checkReentrancy();
    itemsToRemove = List$1_$T$_.defaultConstructor();
    for (index = count - 1; index >= 0; index--) {
      itemsToRemove.add(this.items.get_item(removeIndex));
      this.items.removeAt(removeIndex);
    }
    this.onCollectionChanged(1, removeIndex, null, itemsToRemove);
    this.firePropertyChanged("Count");
  };
  ptyp_.checkReentrancy = function Sunlight__Framework__Observables__ObservableCollection$1__CheckReentrancy() {
    if (this.busy)
      throw Error.createError("InvalidOperationException", null);
  };
  ptyp_.onCollectionChanged = function Sunlight__Framework__Observables__ObservableCollection$1__OnCollectionChanged(action, index, newItems, oldItems) {
    if (this.collectionChanged !== null) {
      this.busy = true;
      try {
        this.collectionChanged(this, CollectionChangedEventArgs$1_$T$_.__ctor(action, index, newItems, oldItems));
      } finally {
        this.busy = false;
      }
    }
  };
  System__Type__RegisterReferenceType(ObservableCollection$1_$T$_, "Sunlight.Framework.Observables.ObservableCollection`1<" + T.fullName + ">", Sunlight_Framework_Observables_ObservableObject, [Sunlight_Framework_Observables_INotifyPropertyChanged]);
  ObservableCollection$1_$T$_._tri = function() {
    if ($5f_initTrackera)
      return;
    $5f_initTrackera = true;
    List$1_$T$_ = System_Collections_Generic_List(T, true);
    ArrayG$1_$T$_ = System_ArrayG(T, true);
    CollectionChangedEventArgs$1_$T$_ = Sunlight_Framework_Observables_CollectionChangedEventArgs(T, true);
    T = T;
    ObservableCollection$1_$T$_ = Sunlight_Framework_Observables_ObservableCollection(T, true);
  };
  if ($5fcallStatiConstructor)
    ObservableCollection$1_$T$_._tri();
  return ObservableCollection$1_$T$_;
};
function System_Collections_Generic_ICollection(T, $5fcallStatiConstructor) {
  var ICollection$1_$T$_, $5f_initTracker;
  if (System_Collections_Generic_ICollection[T.typeId])
    return System_Collections_Generic_ICollection[T.typeId];
  System_Collections_Generic_ICollection[T.typeId] = function System__Collections__Generic__ICollection$1a() {
  };
  ICollection$1_$T$_ = System_Collections_Generic_ICollection[T.typeId];
  ICollection$1_$T$_.genericParameters = [T];
  ICollection$1_$T$_.genericClosure = System_Collections_Generic_ICollection;
  ICollection$1_$T$_.typeId = "c$" + T.typeId + "$";
  System__Type__RegisterInterface(ICollection$1_$T$_, "System.Collections.Generic.ICollection`1<" + T.fullName + ">");
  ICollection$1_$T$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    ICollection$1_$T$_ = System_Collections_Generic_ICollection(T, true);
  };
  if ($5fcallStatiConstructor)
    ICollection$1_$T$_._tri();
  return ICollection$1_$T$_;
};
function Sunlight_Framework_Observables_CollectionChangedEventArgs(T, $5fcallStatiConstructor) {
  var CollectionChangedEventArgs$1_$T$_, $5f_initTracker;
  if (Sunlight_Framework_Observables_CollectionChangedEventArgs[T.typeId])
    return Sunlight_Framework_Observables_CollectionChangedEventArgs[T.typeId];
  Sunlight_Framework_Observables_CollectionChangedEventArgs[T.typeId] = function Sunlight__Framework__Observables__CollectionChangedEventArgs$1a() {
  };
  CollectionChangedEventArgs$1_$T$_ = Sunlight_Framework_Observables_CollectionChangedEventArgs[T.typeId];
  CollectionChangedEventArgs$1_$T$_.genericParameters = [T];
  CollectionChangedEventArgs$1_$T$_.genericClosure = Sunlight_Framework_Observables_CollectionChangedEventArgs;
  CollectionChangedEventArgs$1_$T$_.typeId = "bv$" + T.typeId + "$";
  CollectionChangedEventArgs$1_$T$_.__ctor = function Sunlight_Framework_Observables_CollectionChangedEventArgs$1_factorya(action, changeIndex, newItems, oldItems) {
    var this_;
    this_ = new CollectionChangedEventArgs$1_$T$_();
    this_.__ctor(action, changeIndex, newItems, oldItems);
    return this_;
  };
  ptyp_ = CollectionChangedEventArgs$1_$T$_.prototype;
  ptyp_.action = 0;
  ptyp_.newItems = null;
  ptyp_.oldItems = null;
  ptyp_.changeIndex = 0;
  ptyp_.__ctor = function Sunlight__Framework__Observables__CollectionChangedEventArgs$1____ctor(action, changeIndex, newItems, oldItems) {
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
  };
  ptyp_.get_newItems = function Sunlight__Framework__Observables__CollectionChangedEventArgs$1__get_NewItems() {
    return this.newItems;
  };
  ptyp_.get_oldItems = function Sunlight__Framework__Observables__CollectionChangedEventArgs$1__get_OldItems() {
    return this.oldItems;
  };
  ptyp_.get_changeIndex = function Sunlight__Framework__Observables__CollectionChangedEventArgs$1__get_ChangeIndex() {
    return this.changeIndex;
  };
  System__Type__RegisterReferenceType(CollectionChangedEventArgs$1_$T$_, "Sunlight.Framework.Observables.CollectionChangedEventArgs`1<" + T.fullName + ">", Object, []);
  CollectionChangedEventArgs$1_$T$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    CollectionChangedEventArgs$1_$T$_ = Sunlight_Framework_Observables_CollectionChangedEventArgs(T, true);
  };
  if ($5fcallStatiConstructor)
    CollectionChangedEventArgs$1_$T$_._tri();
  return CollectionChangedEventArgs$1_$T$_;
};
function System_Action(T1, T2, $5fcallStatiConstructor) {
  var Action$2_$T1_x_T1$_, $5f_initTracker;
  if (System_Action[T1.typeId] && System_Action[T1.typeId][T2.typeId])
    return System_Action[T1.typeId][T2.typeId];
    System_Action[T1.typeId] = {
    };
  System_Action[T1.typeId][T2.typeId] = function System__Action$2a() {
  };
  Action$2_$T1_x_T1$_ = System_Action[T1.typeId][T2.typeId];
  Action$2_$T1_x_T1$_.genericParameters = [T1, T2];
  Action$2_$T1_x_T1$_.genericClosure = System_Action;
  Action$2_$T1_x_T1$_.typeId = "bw$" + T1.typeId + "_" + T2.typeId + "$";
  Action$2_$T1_x_T1$_.prototype = new System_MulticastDelegate();
  System__Type__RegisterReferenceType(Action$2_$T1_x_T1$_, "System.Action`2<" + T1.fullName + "," + T2.fullName + ">", System_MulticastDelegate, []);
  Action$2_$T1_x_T1$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T1 = T1;
    T2 = T2;
    Action$2_$T1_x_T1$_ = System_Action(T1, T2, true);
  };
  if ($5fcallStatiConstructor)
    Action$2_$T1_x_T1$_._tri();
  return Action$2_$T1_x_T1$_;
};
function Sunlight_Framework_Observables_INotifyPropertyChanged() {
};
Sunlight_Framework_Observables_INotifyPropertyChanged.typeId = "e";
System__Type__RegisterInterface(Sunlight_Framework_Observables_INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function System_Collections_Generic_StringDictionary(TValue, $5fcallStatiConstructor) {
  var StringDictionary$1_$TValue$_, ICollection$1_$KeyValuePair$2_$String_x_String$_$_, KeyValuePair$2_$String_x_String$_, $5f_initTracker;
  if (System_Collections_Generic_StringDictionary[TValue.typeId])
    return System_Collections_Generic_StringDictionary[TValue.typeId];
  System_Collections_Generic_StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$1a() {
  };
  StringDictionary$1_$TValue$_ = System_Collections_Generic_StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = System_Collections_Generic_StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "bx$" + TValue.typeId + "$";
  KeyValuePair$2_$String_x_String$_ = System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor);
  ICollection$1_$KeyValuePair$2_$String_x_String$_$_ = System_Collections_Generic_ICollection(System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor), $5fcallStatiConstructor);
  KeyValuePair$2_$String_x_String$_ = System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor);
  StringDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_StringDictionary$1_factorya() {
    var this_;
    this_ = new StringDictionary$1_$TValue$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = StringDictionary$1_$TValue$_.prototype;
  ptyp_.innerDict = null;
  ptyp_.count = 0;
  ptyp_.__ctor = function System__Collections__Generic__StringDictionary$1____ctor() {
    this.innerDict = new Object();
  };
  ptyp_.get_item = function System__Collections__Generic__StringDictionary$1__get_Item(index) {
    if (!(index in this.innerDict))
      throw new Error("Key not found");
    return this.innerDict[index];
  };
  ptyp_.set_item = function System__Collections__Generic__StringDictionary$1__set_Item(index, value) {
    this.innerDict[index] = value;
  };
  ptyp_.get_count = function System__Collections__Generic__StringDictionary$1__get_Count() {
    return this.count;
  };
  ptyp_.add = function System__Collections__Generic__StringDictionary$1__Adda(key, value) {
    if (this.containsKey(key))
      throw new Error("Key already exists");
    this.count++;
    this.set_item(key, value);
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
    value.write(TValue.getDefaultValue());
    return false;
  };
  ptyp_.adda = function System__Collections__Generic__StringDictionary$1__Add(item) {
    this.add(KeyValuePair$2_$String_x_String$_.get_key(item), KeyValuePair$2_$String_x_String$_.get_value(item));
  };
  ptyp_["V_get_Count_" + ICollection$1_$KeyValuePair$2_$String_x_String$_$_.typeId] = ptyp_.get_count;
  ptyp_["V_Add_" + ICollection$1_$KeyValuePair$2_$String_x_String$_$_.typeId] = ptyp_.adda;
  System__Type__RegisterReferenceType(StringDictionary$1_$TValue$_, "System.Collections.Generic.StringDictionary`1<" + TValue.fullName + ">", Object, [ICollection$1_$KeyValuePair$2_$String_x_String$_$_]);
  StringDictionary$1_$TValue$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    TValue = TValue;
    StringDictionary$1_$TValue$_ = System_Collections_Generic_StringDictionary(TValue, true);
  };
  if ($5fcallStatiConstructor)
    StringDictionary$1_$TValue$_._tri();
  return StringDictionary$1_$TValue$_;
};
function System_Collections_Generic_List(T, $5fcallStatiConstructor) {
  var List$1_$T$_, ICollection$1_$T$_, $5f_initTracker;
  if (System_Collections_Generic_List[T.typeId])
    return System_Collections_Generic_List[T.typeId];
  System_Collections_Generic_List[T.typeId] = function System__Collections__Generic__List$1a() {
  };
  List$1_$T$_ = System_Collections_Generic_List[T.typeId];
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = System_Collections_Generic_List;
  List$1_$T$_.typeId = "by$" + T.typeId + "$";
  ICollection$1_$T$_ = System_Collections_Generic_ICollection(T, $5fcallStatiConstructor);
  List$1_$T$_.defaultConstructor = function System_Collections_Generic_List$1_factorya() {
    var this_;
    this_ = new List$1_$T$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = List$1_$T$_.prototype;
  ptyp_.nativeArray = null;
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
  ptyp_.removeAt = function System__Collections__Generic__List$1__RemoveAt(index) {
    System__NativeArray__RemoveAt(this.nativeArray, index);
  };
  ptyp_.get_count = function System__Collections__Generic__List$1__get_Count() {
    return this.nativeArray.length;
  };
  ptyp_.add = function System__Collections__Generic__List$1__Add(item) {
    System__NativeArray__Push(this.nativeArray, item);
  };
  ptyp_["V_get_Count_" + ICollection$1_$T$_.typeId] = ptyp_.get_count;
  ptyp_["V_Add_" + ICollection$1_$T$_.typeId] = ptyp_.add;
  System__Type__RegisterReferenceType(List$1_$T$_, "System.Collections.Generic.List`1<" + T.fullName + ">", Object, [ICollection$1_$T$_]);
  List$1_$T$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    List$1_$T$_ = System_Collections_Generic_List(T, true);
  };
  if ($5fcallStatiConstructor)
    List$1_$T$_._tri();
  return List$1_$T$_;
};
function System_Collections_Generic_KeyValuePair(K, V, $5fcallStatiConstructor) {
  var KeyValuePair$2_$K_x_K$_, $5f_initTracker;
  if (System_Collections_Generic_KeyValuePair[K.typeId] && System_Collections_Generic_KeyValuePair[K.typeId][V.typeId])
    return System_Collections_Generic_KeyValuePair[K.typeId][V.typeId];
    System_Collections_Generic_KeyValuePair[K.typeId] = {
    };
  System_Collections_Generic_KeyValuePair[K.typeId][V.typeId] = function(boxedValue) {
    this.boxedValue = boxedValue;
  };
  KeyValuePair$2_$K_x_K$_ = System_Collections_Generic_KeyValuePair[K.typeId][V.typeId];
  KeyValuePair$2_$K_x_K$_.genericParameters = [K, V];
  KeyValuePair$2_$K_x_K$_.genericClosure = System_Collections_Generic_KeyValuePair;
  KeyValuePair$2_$K_x_K$_.typeId = "bz$" + K.typeId + "_" + V.typeId + "$";
  KeyValuePair$2_$K_x_K$_.getDefaultValue = function() {
    return {
      key: K.getDefaultValue(),
      val: V.getDefaultValue()
    };
  };
  KeyValuePair$2_$K_x_K$_.get_key = function System__Collections__Generic__KeyValuePair$2__get_Key(this_) {
    return this_.key;
  };
  KeyValuePair$2_$K_x_K$_.get_value = function System__Collections__Generic__KeyValuePair$2__get_Value(this_) {
    return this_.val;
  };
  KeyValuePair$2_$K_x_K$_.prototype = new System_ValueType();
  System__Type__RegisterStructType(KeyValuePair$2_$K_x_K$_, "System.Collections.Generic.KeyValuePair`2<" + K.fullName + "," + V.fullName + ">", []);
  KeyValuePair$2_$K_x_K$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    KeyValuePair$2_$K_x_K$_ = System_Collections_Generic_KeyValuePair(K, V, true);
  };
  if ($5fcallStatiConstructor)
    KeyValuePair$2_$K_x_K$_._tri();
  return KeyValuePair$2_$K_x_K$_;
};
System_ArrayG_$String$_ = System_ArrayG(String);
System_Func_$Object_x_Object$_ = System_Func(Object, Object);
System_ArrayG_$Func_$Object_x_Object$_$_ = System_ArrayG(System_Func_$Object_x_Object$_);
Sunlight_Framework_Observables_ObservableCollection_$Int32$_ = Sunlight_Framework_Observables_ObservableCollection(System_Int32);
System_ArrayG_$Object$_ = System_ArrayG(Object);
System_Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_ = System_Action(Sunlight_Framework_Observables_INotifyPropertyChanged, String);
System_ArrayG_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_ = System_ArrayG(System_Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_);
System_Collections_Generic_StringDictionary_$TypeRegistry$_ = System_Collections_Generic_StringDictionary(Sunlight_Framework_TypeRegistry);
System_Collections_Generic_StringDictionary_$Delegate$_ = System_Collections_Generic_StringDictionary(System_Delegate);
System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_ = System_Collections_Generic_StringDictionary(System_Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_);
System__String____cctor();
System_ArrayG_$String$_._tri();
System_Func_$Object_x_Object$_._tri();
System_ArrayG_$Func_$Object_x_Object$_$_._tri();
Sunlight_Framework_Observables_ObservableCollection_$Int32$_._tri();
System_ArrayG_$Object$_._tri();
System_Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_._tri();
System_ArrayG_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_._tri();
System_Collections_Generic_StringDictionary_$TypeRegistry$_._tri();
System_Collections_Generic_StringDictionary_$Delegate$_._tri();
System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_._tri();
module("Sunlight.Framework.Test.Binders.DataBinderTests");
test("BasicBinderSimpleObjectTest", 0, Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderSimpleObjectTest);
test("BasicBinderSimpleObjectTestReverseOrder", 0, Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderSimpleObjectTestReverseOrder);
test("BasicBinderOneWayNotifiableObjectTest", 0, Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderOneWayNotifiableObjectTest);
test("BasicBinderTwoWayNotifiableObjectTest", 0, Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderTwoWayNotifiableObjectTest);
module("Sunlight.Framework.Test.Binders.SourcePropertyBinderTests");
test("BasicValueTest", 0, Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__BasicValueTest);
test("BasicValueTestWithNotification", 0, Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__BasicValueTestWithNotification);
test("PropertyPathValueNotifiableTest", 0, Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PropertyPathValueNotifiableTest);
test("PropertyPathValueTest", 0, Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PropertyPathValueTest);
module("Sunlight.Framework.Test.ContainerTests");
test("TestRegisterResolve", 0, Sunlight__Framework__Test__ContainerTests__TestRegisterResolve);
test("TestRegisterResolveWithAs", 0, Sunlight__Framework__Test__ContainerTests__TestRegisterResolveWithAs);
test("TestRegisterResolveIsSingleton", 0, Sunlight__Framework__Test__ContainerTests__TestRegisterResolveIsSingleton);
test("TestRegisterResolveLazy", 0, Sunlight__Framework__Test__ContainerTests__TestRegisterResolveLazy);
module("Sunlight.Framework.Test.EventBusTests");
test("TestSubscribeAndRaise", 0, Sunlight__Framework__Test__EventBusTests__TestSubscribeAndRaise);
test("TestSubscribeAndRaiseOnceTime", 0, Sunlight__Framework__Test__EventBusTests__TestSubscribeAndRaiseOnceTime);
test("TestSubscribeUnSubscribeAndRaise", 0, Sunlight__Framework__Test__EventBusTests__TestSubscribeUnSubscribeAndRaise);
module("Sunlight.Framework.Test.ObservableCollectionTests");
test("TestCreateNewObservableCollection", 0, Sunlight__Framework__Test__ObservableCollectionTests__TestCreateNewObservableCollection);
test("TestAddItemToObservableCollection", 0, Sunlight__Framework__Test__ObservableCollectionTests__TestAddItemToObservableCollection);
test("TestRemoveItemToObservableCollection", 0, Sunlight__Framework__Test__ObservableCollectionTests__TestRemoveItemToObservableCollection);
module("Sunlight.Framework.Test.ObservableObjectTests");
test("TestCreateNewObservableObject", 0, Sunlight__Framework__Test__ObservableObjectTests__TestCreateNewObservableObject);
test("TestFirePropertyChanged", 0, Sunlight__Framework__Test__ObservableObjectTests__TestFirePropertyChanged);
test("TestRemovePropertyChangeCallback", 0, Sunlight__Framework__Test__ObservableObjectTests__TestRemovePropertyChangeCallback);
//@ sourceMappingURL=Sunlight.Framework.Test.js.map
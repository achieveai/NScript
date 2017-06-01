(function(){
var ArrayG_$String$_, ArrayG_$Func_$Object_x_Object$_$_, ObservableCollection_$Int32$_, Type__typeMapping, ArrayG_$Object$_, ArrayG_$Action_$INotifyPropertyChanged_x_String$_$_, StringDictionary_$TypeRegistry$_, StringDictionary_$Function$_, StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_, ptyp_, Func_$Object_x_Object$_, Action_$INotifyPropertyChanged_x_String$_;
Function.typeId = "h";
Type__typeMapping = null;
function Type__AsType(this_, instance) {
  return this_.isInstanceOfType(instance) ? instance : null;
};
function Type__CastType(this_, instance) {
  if (this_.isInstanceOfType(instance) || instance === null || typeof instance === "undefined") {
    if (this_.isStruct)
      return instance.boxedValue;
    return instance;
  }
  throw "InvalidCast to " + this_.fullName;
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
    return instance instanceof this || instance && instance.constructor == this;
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
Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "i";
function Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
Type__RegisterReferenceType(Object, "System.Object", null, []);
function DataBinderTests() {
};
DataBinderTests.typeId = "j";
function DataBinderTests__BasicBinderSimpleObjectTest(assert) {
  var dataBinder, target, source;
  dataBinder = DataBinder_factory(SourcePropertyBinder_factory(ArrayG_$String$_.__ctor(["IntProp"]), ArrayG_$Func_$Object_x_Object$_$_.__ctor([function DataBinderTests__BasicBinderSimpleObjectTest_del(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(SimpleObjectWithProperty, obj).get_intProp());
  }]), null), TargetBinder_factory("IntProp", null, function DataBinderTests__BasicBinderSimpleObjectTest_del2(obj, value) {
    Type__CastType(SimpleNotifiableClass, obj).set_intProp(Type__UnBoxTypeInstance(Int32, value));
  }, Type__BoxTypeInstance(Int32, -1)), 1, null);
  target = SourcePropertyBinderTests__PrepNotifiableObject();
  source = SourcePropertyBinderTests__PrepSimpleObject();
  source.set_intProp(101);
  dataBinder.setTarget(target);
  assert.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(source);
  assert.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(null);
  assert.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
};
function DataBinderTests__BasicBinderSimpleObjectTestReverseOrder(assert) {
  var dataBinder, target, source;
  dataBinder = DataBinder_factory(SourcePropertyBinder_factory(ArrayG_$String$_.__ctor(["IntProp"]), ArrayG_$Func_$Object_x_Object$_$_.__ctor([function DataBinderTests__BasicBinderSimpleObjectTestReverseOrder_del(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(SimpleObjectWithProperty, obj).get_intProp());
  }]), null), TargetBinder_factory("IntProp", null, function DataBinderTests__BasicBinderSimpleObjectTestReverseOrder_del2(obj, value) {
    Type__CastType(SimpleNotifiableClass, obj).set_intProp(Type__UnBoxTypeInstance(Int32, value));
  }, Type__BoxTypeInstance(Int32, -1)), 1, null);
  target = SourcePropertyBinderTests__PrepNotifiableObject();
  source = SourcePropertyBinderTests__PrepSimpleObject();
  source.set_intProp(101);
  dataBinder.setSource(source);
  dataBinder.setTarget(target);
  assert.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(null);
  assert.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
};
function DataBinderTests__BasicBinderOneWayNotifiableObjectTest(assert) {
  var dataBinder, target, source;
  dataBinder = DataBinder_factory(SourcePropertyBinder_factory(ArrayG_$String$_.__ctor(["IntProp"]), ArrayG_$Func_$Object_x_Object$_$_.__ctor([function DataBinderTests__BasicBinderOneWayNotifiableObjectTest_del(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(SimpleNotifiableClass, obj).get_intProp());
  }]), null), TargetBinder_factory("IntProp", null, function DataBinderTests__BasicBinderOneWayNotifiableObjectTest_del2(obj, value) {
    Type__CastType(SimpleNotifiableClass, obj).set_intProp(Type__UnBoxTypeInstance(Int32, value));
  }, Type__BoxTypeInstance(Int32, -1)), 1, null);
  target = SourcePropertyBinderTests__PrepNotifiableObject();
  source = SourcePropertyBinderTests__PrepNotifiableObject();
  source.set_intProp(101);
  dataBinder.setTarget(target);
  assert.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(source);
  assert.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  source.set_intProp(102);
  assert.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
};
function DataBinderTests__BasicBinderTwoWayNotifiableObjectTest(assert) {
  var dataBinder, target, source, stmtTemp1;
  dataBinder = DataBinder_factory(SourcePropertyBinder_factory(ArrayG_$String$_.__ctor(["IntProp"]), ArrayG_$Func_$Object_x_Object$_$_.__ctor([function DataBinderTests__BasicBinderTwoWayNotifiableObjectTest_del(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(SimpleNotifiableClass, obj).get_intProp());
  }]), function DataBinderTests__BasicBinderTwoWayNotifiableObjectTest_del2(obj, value) {
    Type__CastType(SimpleNotifiableClass, obj).set_intProp(Type__UnBoxTypeInstance(Int32, value));
  }), TargetBinder_factory("IntProp", function DataBinderTests__BasicBinderTwoWayNotifiableObjectTest_del3(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(SimpleNotifiableClass, obj).get_intProp());
  }, function DataBinderTests__BasicBinderTwoWayNotifiableObjectTest_del4(obj, value) {
    Type__CastType(SimpleNotifiableClass, obj).set_intProp(Type__UnBoxTypeInstance(Int32, value));
  }, Type__BoxTypeInstance(Int32, -1)), 2, null);
  target = SourcePropertyBinderTests__PrepNotifiableObject();
  source = SourcePropertyBinderTests__PrepNotifiableObject();
  source.set_intProp(101);
  dataBinder.setTarget(target);
  assert.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(source);
  assert.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  target.set_intProp((stmtTemp1 = target.get_intProp()) + 1), stmtTemp1;
  assert.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
};
Type__RegisterReferenceType(DataBinderTests, "Sunlight.Framework.Test.Binders.DataBinderTests", Object, []);
function SourcePropertyBinderTests() {
};
SourcePropertyBinderTests.typeId = "k";
function SourcePropertyBinderTests__PrepNotifiableObject() {
  var rv, stmtTemp1, stmtTemp10, stmtTemp11;
  rv = (stmtTemp1 = SimpleNotifiableClass_factory(), stmtTemp1.set_intProp(10), stmtTemp1.set_strProp("Ten"), stmtTemp1.set_objProp((stmtTemp11 = SimpleObjectWithProperty_factory(), stmtTemp11.set_intProp(11), stmtTemp11.set_stringProp("Eleven"), stmtTemp11)), stmtTemp1);
  rv.set_selfProp(rv);
  rv.get_objProp().set_selfProp(rv.get_objProp());
  rv.get_objProp().set_notifiableProp(rv);
  return rv;
};
function SourcePropertyBinderTests__PrepSimpleObject() {
  return SourcePropertyBinderTests__PrepNotifiableObject().get_objProp();
};
function SourcePropertyBinderTests__BasicValueTest(assert) {
  var sourceBinder, helper, src;
  sourceBinder = SourcePropertyBinder_factory(ArrayG_$String$_.__ctor([null]), ArrayG_$Func_$Object_x_Object$_$_.__ctor([function SourcePropertyBinderTests__BasicValueTest_del(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(SimpleObjectWithProperty, obj).get_intProp());
  }]), null);
  helper = BinderTestHelper_factory();
  sourceBinder.useDataBinder(helper);
  src = SourcePropertyBinderTests__PrepSimpleObject();
  sourceBinder.set_source(src);
  assert.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  assert.equal(src.get_intProp(), Type__UnBoxTypeInstance(Int32, sourceBinder.get_value()), "SourceBinder.Value");
};
function SourcePropertyBinderTests__BasicValueTestWithNotification(assert) {
  var sourceBinder, helper, src, stmtTemp1;
  sourceBinder = SourcePropertyBinder_factory(ArrayG_$String$_.__ctor(["IntProp"]), ArrayG_$Func_$Object_x_Object$_$_.__ctor([function SourcePropertyBinderTests__BasicValueTestWithNotification_del(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(SimpleNotifiableClass, obj).get_intProp());
  }]), null);
  helper = BinderTestHelper_factory();
  sourceBinder.useDataBinder(helper);
  src = SourcePropertyBinderTests__PrepNotifiableObject();
  sourceBinder.set_source(src);
  assert.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  assert.equal(src.get_intProp(), Type__UnBoxTypeInstance(Int32, sourceBinder.get_value()), "SourceBinder.Value");
  helper.set_sourceValueUpdateCalled(false);
  src.set_intProp((stmtTemp1 = src.get_intProp()) + 1), stmtTemp1;
  assert.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  assert.equal(src.get_intProp(), Type__UnBoxTypeInstance(Int32, sourceBinder.get_value()), "SourceBinder.Value");
};
function SourcePropertyBinderTests__PropertyPathValueNotifiableTest(assert) {
  var sourceBinder, helper, src;
  sourceBinder = SourcePropertyBinder_factory(ArrayG_$String$_.__ctor(["NotifiableProp", "IntProp"]), ArrayG_$Func_$Object_x_Object$_$_.__ctor([function SourcePropertyBinderTests__PropertyPathValueNotifiableTest_del(obj) {
    return Type__CastType(SimpleObjectWithProperty, obj).get_notifiableProp();
  }, function SourcePropertyBinderTests__PropertyPathValueNotifiableTest_del2(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(SimpleNotifiableClass, obj).get_intProp());
  }]), null);
  helper = BinderTestHelper_factory();
  sourceBinder.useDataBinder(helper);
  src = SourcePropertyBinderTests__PrepSimpleObject();
  sourceBinder.set_source(src);
  assert.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  assert.equal(src.get_notifiableProp().get_intProp(), Type__UnBoxTypeInstance(Int32, sourceBinder.get_value()), "SourceBinder.Value");
  helper.set_sourceValueUpdateCalled(false);
  src.get_notifiableProp().set_intProp( -1);
  assert.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  assert.equal(src.get_notifiableProp().get_intProp(), Type__UnBoxTypeInstance(Int32, sourceBinder.get_value()), "SourceBinder.Value");
};
function SourcePropertyBinderTests__PropertyPathValueTest(assert) {
  var sourceBinder, helper, src, lastValue;
  sourceBinder = SourcePropertyBinder_factory(ArrayG_$String$_.__ctor(["SelfProp", "IntProp"]), ArrayG_$Func_$Object_x_Object$_$_.__ctor([function SourcePropertyBinderTests__PropertyPathValueTest_del(obj) {
    return Type__CastType(SimpleObjectWithProperty, obj).get_selfProp();
  }, function SourcePropertyBinderTests__PropertyPathValueTest_del2(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(SimpleObjectWithProperty, obj).get_intProp());
  }]), null);
  helper = BinderTestHelper_factory();
  sourceBinder.useDataBinder(helper);
  src = SourcePropertyBinderTests__PrepSimpleObject();
  sourceBinder.set_source(src);
  assert.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  assert.equal(src.get_selfProp().get_intProp(), Type__UnBoxTypeInstance(Int32, sourceBinder.get_value()), "SourceBinder.Value");
  helper.set_sourceValueUpdateCalled(false);
  lastValue = src.get_selfProp().get_intProp();
  src.get_selfProp().set_intProp( -1);
  assert.ok(!helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  assert.equal(lastValue, Type__UnBoxTypeInstance(Int32, sourceBinder.get_value()), "SourceBinder.Value");
};
Type__RegisterReferenceType(SourcePropertyBinderTests, "Sunlight.Framework.Test.Binders.SourcePropertyBinderTests", Object, []);
function ContainerTests() {
};
ContainerTests.typeId = "l";
function ContainerTests__TestRegisterResolve(assert) {
  var container, x, y, t2, t1;
  container = IocContainer_factory();
  x = 1;
  y = 2;
  container.register(IocTestType2, function ContainerTests__TestRegisterResolve_del() {
    return IocTestType2_factory(x);
  });
  container.register(IocTestType1, function ContainerTests__TestRegisterResolve_del2() {
    return IocTestType1_factory(x, y);
  });
  t2 = container.resolve(IocTestType2);
  assert.ok(t2, "t2 != null");
  assert.equal(1, t2.testMethod(), "1 == t1.TestMethod()");
  t1 = container.resolve(IocTestType1);
  assert.ok(t1, "t1 != null");
  assert.equal(3, t1.testMethod(), "3 == t1.TestMethod()");
  x = 10;
  t1 = container.resolve(IocTestType1);
  assert.equal(12, t1.testMethod(), "12 == t1.TestMethod()");
};
function ContainerTests__TestRegisterResolveWithAs(assert) {
  var container, x, y, t2, t1;
  container = IocContainer_factory();
  x = 1;
  y = 2;
  container.register(IocTestType2, function ContainerTests__TestRegisterResolveWithAs_del() {
    return IocTestType2_factory(x);
  });
  container.register(IocTestType1, function ContainerTests__TestRegisterResolveWithAs_del2() {
    return IocTestType1_factory(x, y);
  }).as(IIocTestType1);
  t2 = container.resolve(IocTestType2);
  assert.ok(t2, "t2 != null");
  assert.equal(1, t2.testMethod(), "1 == t1.TestMethod()");
  t1 = container.resolve(IIocTestType1);
  assert.ok(t1, "t1 != null");
  assert.equal(3, t1.V_TestMethod_b(), "3 == t1.TestMethod()");
};
function ContainerTests__TestRegisterResolveIsSingleton(assert) {
  var container, x, y, t2, t1, t1_;
  container = IocContainer_factory();
  x = 1;
  y = 2;
  container.register(IocTestType2, function ContainerTests__TestRegisterResolveIsSingleton_del() {
    return IocTestType2_factory(x);
  });
  container.register(IocTestType1, function ContainerTests__TestRegisterResolveIsSingleton_del2() {
    return IocTestType1_factory(x, y);
  }).isSingleton();
  t2 = container.resolve(IocTestType2);
  assert.ok(t2, "t2 != null");
  assert.equal(1, t2.testMethod(), "1 == t1.TestMethod()");
  t1 = container.resolve(IocTestType1);
  x = 10;
  t1_ = container.resolve(IocTestType1);
  assert.strictEqual(t1_, t1, "t1_ === t1");
};
function ContainerTests__TestRegisterResolveLazy(assert) {
  var container, x, y, t1;
  container = IocContainer_factory();
  x = 1;
  y = 2;
  container.register(IocTestType1, function ContainerTests__TestRegisterResolveLazy_del() {
    return IocTestType1_factory(x++, y);
  }).isSingleton();
  t1 = container.resolveLazy(IocTestType1);
  assert.equal(1, x, "x === 1");
  assert.equal(3, t1.get_value().testMethod(), "t1.Value.TestMethod() == 3");
  assert.equal(2, x, "x === 2");
};
Type__RegisterReferenceType(ContainerTests, "Sunlight.Framework.Test.ContainerTests", Object, []);
function EventBusTests() {
};
EventBusTests.typeId = "m";
function EventBusTests__TestSubscribeAndRaise(assert) {
  var evtBus, x1, x2, stmtTemp1;
  evtBus = EventBus_factory();
  x1 = 0;
  x2 = 0;
  evtBus.subscribe(Evt1, function EventBusTests__TestSubscribeAndRaise_del(evt) {
    return x1 = evt.x;
  });
  evtBus.subscribe(Evt2, function EventBusTests__TestSubscribeAndRaise_del2(evt) {
    return x2 = evt.x;
  });
  evtBus.raise(Evt1, (stmtTemp1 = Evt1_factory(), stmtTemp1.x = 10, stmtTemp1));
  assert.equal(10, x1, "10 == x1");
  assert.equal(0, x2, "0 == x2");
};
function EventBusTests__TestSubscribeAndRaiseOnceTime(assert) {
  var evtBus, x1, x2, del, stmtTemp1;
  evtBus = EventBus_factory();
  x1 = 0;
  x2 = 0;
  del = function EventBusTests__TestSubscribeAndRaiseOnceTime_del(evt) {
    x1 = evt.x;
  };
  evtBus.subscribe(Evt1, del);
  evtBus.subscribe(Evt2, function EventBusTests__TestSubscribeAndRaiseOnceTime_del2(evt) {
    return x2 = evt.x;
  });
  evtBus.raiseOneTime(Evt1, (stmtTemp1 = Evt1_factory(), stmtTemp1.x = 10, stmtTemp1));
  assert.equal(10, x1, "10 == x1");
  x1 = 0;
  evtBus.subscribe(Evt1, del);
  assert.equal(10, x1, "(2) 10 == x1");
};
function EventBusTests__TestSubscribeUnSubscribeAndRaise(assert) {
  var evtBus, x1, x2, del, stmtTemp1;
  evtBus = EventBus_factory();
  x1 = 0;
  x2 = 0;
  del = function EventBusTests__TestSubscribeUnSubscribeAndRaise_del(evt) {
    x1 = evt.x;
  };
  evtBus.subscribe(Evt2, function EventBusTests__TestSubscribeUnSubscribeAndRaise_del2(evt) {
    return x2 = evt.x;
  });
  evtBus.subscribe(Evt1, function EventBusTests__TestSubscribeUnSubscribeAndRaise_del3(evt) {
    return x2 = evt.x;
  });
  evtBus.unSubscribe(Evt1, del);
  evtBus.raise(Evt1, (stmtTemp1 = Evt1_factory(), stmtTemp1.x = 10, stmtTemp1));
  assert.equal(0, x1, "0 == x1");
};
Type__RegisterReferenceType(EventBusTests, "Sunlight.Framework.Test.EventBusTests", Object, []);
function ObservableCollectionTests() {
};
ObservableCollectionTests.typeId = "n";
function ObservableCollectionTests__TestCreateNewObservableCollection(assert) {
  var observableCollection;
  observableCollection = ObservableCollection_$Int32$_.defaultConstructor();
  assert.notEqual(null, observableCollection, "ObservableCollection should be created");
  assert.equal(0, observableCollection.get_count(), "ObservableCollection's size should be 0");
};
function ObservableCollectionTests__TestAddItemToObservableCollection(assert) {
  var observableCollection, eventRaised;
  observableCollection = ObservableCollection_$Int32$_.defaultConstructor();
  eventRaised = false;
  observableCollection.add_CollectionChanged(function ObservableCollectionTests__TestAddItemToObservableCollection_del(coll, evtArg) {
    assert.equal(observableCollection, coll, "ObservableCollection");
    assert.equal(1, evtArg.get_newItems().V_get_Count_c(), "evtArg.NewItems.Count");
    assert.ok(Object__IsNullOrUndefined(evtArg.get_oldItems()), "Object.IsNullOrUndefined(evtArg.OldItems)");
    assert.equal(0, evtArg.get_changeIndex(), "evtArg.changeIndex");
    eventRaised = true;
  });
  observableCollection.add(1);
  assert.ok(eventRaised, "Change event raised");
};
function ObservableCollectionTests__TestRemoveItemToObservableCollection(assert) {
  var observableCollection, eventRaised;
  observableCollection = ObservableCollection_$Int32$_.defaultConstructor();
  eventRaised = false;
  observableCollection.add(1);
  observableCollection.add(2);
  observableCollection.add(3);
  observableCollection.add_CollectionChanged(function ObservableCollectionTests__TestRemoveItemToObservableCollection_del(coll, evtArg) {
    assert.equal(observableCollection, coll, "ObservableCollection");
    assert.equal(2, evtArg.get_oldItems().V_get_Count_c(), "evtArg.OldItems.Count");
    assert.ok(Object__IsNullOrUndefined(evtArg.get_newItems()), "Object.IsNullOrUndefined(evtArg.NewItems)");
    assert.equal(1, evtArg.get_changeIndex(), "evtArg.changeIndex");
    eventRaised = true;
  });
  observableCollection.removeRangeAt(1, 2);
  assert.ok(eventRaised, "Change event raised");
};
Type__RegisterReferenceType(ObservableCollectionTests, "Sunlight.Framework.Test.ObservableCollectionTests", Object, []);
function ObservableObjectTests() {
};
ObservableObjectTests.typeId = "o";
function ObservableObjectTests__TestCreateNewObservableObject(assert) {
  var observableObject;
  observableObject = ObservableTestObject_factory();
  assert.notEqual(null, observableObject, "ObservableObject should be created");
};
function ObservableObjectTests__TestFirePropertyChanged(assert) {
  var observableObject, strChanged, cbCalled, cb1;
  observableObject = ObservableTestObject_factory();
  strChanged = false;
  cbCalled = false;
  cb1 = function ObservableObjectTests__TestFirePropertyChanged_del(sender, propName) {
    strChanged = propName === "StringProp" && sender == observableObject;
    cbCalled = true;
  };
  observableObject.addPropertyChangedListener("StringProp", cb1);
  observableObject.set_stringProp("1");
  assert.ok(strChanged, "change callback called");
  strChanged = false;
  cbCalled = false;
  observableObject.set_intProp(1);
  assert.ok(!strChanged, "String change callback not called.");
  assert.ok(!cbCalled, "Callback should not be called for different property change");
};
function ObservableObjectTests__TestRemovePropertyChangeCallback(assert) {
  var observableObject, cbCalled, cb1;
  observableObject = ObservableTestObject_factory();
  cbCalled = false;
  cb1 = function ObservableObjectTests__TestRemovePropertyChangeCallback_del(sender, propName) {
    return cbCalled = true;
  };
  observableObject.addPropertyChangedListener("StringProp", cb1);
  observableObject.set_stringProp("1");
  assert.ok(cbCalled, "change callback called");
  cbCalled = false;
  observableObject.removePropertyChangedListener("StringProp", cb1);
  observableObject.set_stringProp("2");
  assert.ok(!cbCalled, "after removing change listner, callback should not be called.");
};
Type__RegisterReferenceType(ObservableObjectTests, "Sunlight.Framework.Test.ObservableObjectTests", Object, []);
function ISourceDataBinder() {
};
ISourceDataBinder.typeId = "f";
Type__RegisterInterface(ISourceDataBinder, "Sunlight.Framework.Binders.ISourceDataBinder");
function ITargetDataBinder() {
};
ITargetDataBinder.typeId = "g";
Type__RegisterInterface(ITargetDataBinder, "Sunlight.Framework.Binders.ITargetDataBinder");
function DataBinder() {
};
DataBinder.typeId = "p";
function DataBinder_factory(sourceBinder, targetBinder, dataBindingMode, converter) {
  var this_;
  this_ = new DataBinder();
  this_.__ctor(sourceBinder, targetBinder, dataBindingMode, converter);
  return this_;
};
ptyp_ = DataBinder.prototype;
ptyp_.sourceBinder = null;
ptyp_.targetBinder = null;
ptyp_.converter = null;
ptyp_.bindingMode = 0;
ptyp_.firstBindingSuccessful = false;
ptyp_.__ctor = function DataBinder____ctor(sourceBinder, targetBinder, dataBindingMode, converter) {
  this.sourceBinder = sourceBinder;
  this.targetBinder = targetBinder;
  this.bindingMode = dataBindingMode;
  this.converter = converter;
  this.targetBinder.useDataBinder(this);
  this.sourceBinder.useDataBinder(this);
};
ptyp_.setTarget = function DataBinder__SetTarget(target) {
  this.targetBinder.set_target(target);
  this.applyBinding();
};
ptyp_.setSource = function DataBinder__SetSource(source) {
  this.sourceBinder.set_source(source);
};
ptyp_.sourceValueUpdated = function DataBinder__SourceValueUpdated() {
  this.applyBinding();
};
ptyp_.targetValueUpdated = function DataBinder__TargetValueUpdated() {
  this.applyBackBinding();
};
ptyp_.applyBinding = function DataBinder__ApplyBinding() {
  if (this.targetBinder.get_isActive()) {
    if (this.bindingMode == 0) {
      if (!this.firstBindingSuccessful)
        if (this.sourceBinder.get_isActive()) {
          this.targetBinder.set_value(!this.converter ? this.sourceBinder.get_value() : this.converter.V_Convert_e(this.sourceBinder.get_value()));
          this.firstBindingSuccessful = true;
        }
        else
          this.targetBinder.setDefault();
      return;
    }
    if (this.sourceBinder.get_isActive()) {
      if (this.converter)
        this.targetBinder.set_value(this.converter.V_Convert_e(this.sourceBinder.get_value()));
      else
        this.targetBinder.set_value(this.sourceBinder.get_value());
    }
    else
      this.targetBinder.setDefault();
  }
};
ptyp_.applyBackBinding = function DataBinder__ApplyBackBinding() {
  if (!this.targetBinder.get_isWriteOnly())
    if (this.sourceBinder.get_isActive())
      this.sourceBinder.set_value(this.targetBinder.get_value());
};
ptyp_.V_SourceValueUpdated_f = ptyp_.sourceValueUpdated;
ptyp_.V_TargetValueUpdated_g = ptyp_.targetValueUpdated;
Type__RegisterReferenceType(DataBinder, "Sunlight.Framework.Binders.DataBinder", Object, [ISourceDataBinder, ITargetDataBinder]);
function SourcePropertyBinder() {
};
SourcePropertyBinder.typeId = "q";
function SourcePropertyBinder_factory(propertyPartNames, propertyGetterChain, propertySetter) {
  var this_;
  this_ = new SourcePropertyBinder();
  this_.__ctor(propertyPartNames, propertyGetterChain, propertySetter);
  return this_;
};
ptyp_ = SourcePropertyBinder.prototype;
ptyp_.chainLength = 0;
ptyp_.propertyPartNames = null;
ptyp_.propertyGetterChain = null;
ptyp_.changeRegistrations = null;
ptyp_.objectChain = null;
ptyp_.propertySetter = null;
ptyp_.dataBinderBase = null;
ptyp_.value = null;
ptyp_.isActive = false;
ptyp_.__ctor = function SourcePropertyBinder____ctor(propertyPartNames, propertyGetterChain, propertySetter) {
  var i;
  this.propertyPartNames = propertyPartNames;
  this.chainLength = this.propertyPartNames.V_get_Length();
  this.propertyGetterChain = propertyGetterChain;
  this.propertySetter = propertySetter;
  this.objectChain = ArrayG_$Object$_.__ctor0(this.chainLength);
  this.changeRegistrations = ArrayG_$Action_$INotifyPropertyChanged_x_String$_$_.__ctor0(this.chainLength);
  for (i = this.chainLength - 1; i >= 0; i--)
    this.changeRegistrations.set_item(i, this.getChangeTrackerAt(i));
};
ptyp_.set_source = function SourcePropertyBinder__set_Source(value) {
  if (this.objectChain.get_item(0) !== value) {
    this.setObjectChainElementValue(0, value);
    this.calculateValueFrom(0);
  }
};
ptyp_.get_value = function SourcePropertyBinder__get_Value() {
  return this.value;
};
ptyp_.set_value = function SourcePropertyBinder__set_Value(value) {
  if (this.isActive && this.propertySetter)
    this.propertySetter(this.objectChain.get_item(this.objectChain.V_get_Length() - 1), value);
};
ptyp_.get_isActive = function SourcePropertyBinder__get_IsActive() {
  return this.isActive;
};
ptyp_.useDataBinder = function SourcePropertyBinder__UseDataBinder(dataBinderBase) {
  this.dataBinderBase = dataBinderBase;
};
ptyp_.calculateValueFrom = function SourcePropertyBinder__CalculateValueFrom(index) {
  var i, j, nextValue;
  for (i = index; i < this.chainLength; i++)
    if (this.objectChain.get_item(i) === null) {
      for (j = this.chainLength - 1; j >= i; j--)
        this.setObjectChainElementValue(j, null);
      if (this.value !== null || this.isActive) {
        this.isActive = false;
        this.value = null;
        if (this.dataBinderBase)
          this.dataBinderBase.V_SourceValueUpdated_f();
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
        if (this.dataBinderBase)
          this.dataBinderBase.V_SourceValueUpdated_f();
      }
    }
};
ptyp_.setObjectChainElementValue = function SourcePropertyBinder__SetObjectChainElementValue(index, value) {
  var oldValue, newNotifiableValue;
  if (index > this.chainLength)
    return;
  oldValue = Type__AsType(INotifyPropertyChanged, this.objectChain.get_item(index));
  if (oldValue)
    oldValue.V_RemovePropertyChangedListener_d(this.propertyPartNames.get_item(index), this.changeRegistrations.get_item(index));
  this.objectChain.set_item(index, value);
  newNotifiableValue = Type__AsType(INotifyPropertyChanged, value);
  if (newNotifiableValue)
    newNotifiableValue.V_AddPropertyChangedListener_d(this.propertyPartNames.get_item(index), this.changeRegistrations.get_item(index));
};
ptyp_.getChangeTrackerAt = function SourcePropertyBinder__GetChangeTrackerAt(index) {
  var this_;
  this_ = this;
  return function SourcePropertyBinder__GetChangeTrackerAt_del(sender, prop) {
    this_.calculateValueFrom(index);
  };
};
Type__RegisterReferenceType(SourcePropertyBinder, "Sunlight.Framework.Binders.SourcePropertyBinder", Object, []);
function ValueType() {
};
ValueType.typeId = "r";
ptyp_ = ValueType.prototype;
ptyp_.boxedValue = null;
Type__RegisterReferenceType(ValueType, "System.ValueType", Object, []);
function Int32(boxedValue) {
  this.boxedValue = boxedValue;
};
Int32.typeId = "s";
Int32.getDefaultValue = function() {
  return 0;
};
Int32.prototype = new ValueType();
Type__RegisterStructType(Int32, "System.Int32", []);
function SimpleObjectWithProperty() {
};
SimpleObjectWithProperty.typeId = "t";
function SimpleObjectWithProperty_factory() {
  return new SimpleObjectWithProperty();
};
SimpleObjectWithProperty.defaultConstructor = SimpleObjectWithProperty_factory;
ptyp_ = SimpleObjectWithProperty.prototype;
ptyp_.set_stringProp = function SimpleObjectWithProperty__set_StringProp(value) {
  return this.StringProp = value;
};
ptyp_.get_intProp = function SimpleObjectWithProperty__get_IntProp() {
  return this.IntProp;
};
ptyp_.set_intProp = function SimpleObjectWithProperty__set_IntProp(value) {
  return this.IntProp = value;
};
ptyp_.get_selfProp = function SimpleObjectWithProperty__get_SelfProp() {
  return this.SelfProp;
};
ptyp_.set_selfProp = function SimpleObjectWithProperty__set_SelfProp(value) {
  return this.SelfProp = value;
};
ptyp_.get_notifiableProp = function SimpleObjectWithProperty__get_NotifiableProp() {
  return this.NotifiableProp;
};
ptyp_.set_notifiableProp = function SimpleObjectWithProperty__set_NotifiableProp(value) {
  return this.NotifiableProp = value;
};
ptyp_.__ctor = function SimpleObjectWithProperty____ctor() {
};
Type__RegisterReferenceType(SimpleObjectWithProperty, "Sunlight.Framework.Test.Binders.SimpleObjectWithProperty", Object, []);
function TargetBinder() {
};
TargetBinder.typeId = "u";
function TargetBinder_factory(propertyName, getter, setter, defaultValue) {
  var this_;
  this_ = new TargetBinder();
  this_.__ctor(propertyName, getter, setter, defaultValue);
  return this_;
};
ptyp_ = TargetBinder.prototype;
ptyp_.value = null;
ptyp_.target = null;
ptyp_.getter = null;
ptyp_.setter = null;
ptyp_.propertyName = null;
ptyp_.defaultValue = null;
ptyp_.dataBinder = null;
ptyp_.__ctor = function TargetBinder____ctor(propertyName, getter, setter, defaultValue) {
  this.propertyName = propertyName;
  this.getter = getter;
  this.setter = setter;
  this.defaultValue = defaultValue;
};
ptyp_.get_value = function TargetBinder__get_Value() {
  return this.value;
};
ptyp_.set_value = function TargetBinder__set_Value(value) {
  if (this.value !== value && this.setter) {
    this.value = value;
    if (this.get_isActive())
      this.setter(this.target, this.value);
  }
};
ptyp_.get_isWriteOnly = function TargetBinder__get_IsWriteOnly() {
  return !this.getter;
};
ptyp_.get_isActive = function TargetBinder__get_IsActive() {
  return this.target;
};
ptyp_.set_target = function TargetBinder__set_Target(value) {
  if (this.target != value) {
    if (this.target)
      this.target.V_RemovePropertyChangedListener_d(this.propertyName, Delegate__Create("onTargetUpdated", this));
    this.target = value;
    this.value = null;
    if (this.target) {
      this.target.V_AddPropertyChangedListener_d(this.propertyName, Delegate__Create("onTargetUpdated", this));
      if (this.getter)
        this.value = this.getter(this.target);
    }
  }
};
ptyp_.setDefault = function TargetBinder__SetDefault() {
  this.set_value(this.defaultValue);
};
ptyp_.useDataBinder = function TargetBinder__UseDataBinder(dataBinder) {
  this.dataBinder = dataBinder;
};
ptyp_.onTargetUpdated = function TargetBinder__OnTargetUpdated(sender, propertyName) {
  if (this.dataBinder && this.getter) {
    this.value = this.getter(this.target);
    this.dataBinder.V_TargetValueUpdated_g();
  }
};
Type__RegisterReferenceType(TargetBinder, "Sunlight.Framework.Binders.TargetBinder", Object, []);
function INotifyPropertyChanged() {
};
INotifyPropertyChanged.typeId = "d";
Type__RegisterInterface(INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function ObservableObject() {
};
ObservableObject.typeId = "v";
ptyp_ = ObservableObject.prototype;
ptyp_.eventHandlers = null;
ptyp_.linkedProperties = null;
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
    if (cb)
      this.eventHandlers.set_item(propertyName, cb);
    else
      this.eventHandlers.remove(propertyName);
  }
};
ptyp_.firePropertyChanged = function ObservableObject__FirePropertyChanged(propertyName) {
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
};
ptyp_.__ctor = function ObservableObject____ctor() {
};
ptyp_.V_AddPropertyChangedListener_d = ptyp_.addPropertyChangedListener;
ptyp_.V_RemovePropertyChangedListener_d = ptyp_.removePropertyChangedListener;
Type__RegisterReferenceType(ObservableObject, "Sunlight.Framework.Observables.ObservableObject", Object, [INotifyPropertyChanged]);
function SimpleNotifiableClass() {
};
SimpleNotifiableClass.typeId = "w";
function SimpleNotifiableClass_factory() {
  var this_;
  this_ = new SimpleNotifiableClass();
  this_.__ctor0();
  return this_;
};
SimpleNotifiableClass.defaultConstructor = SimpleNotifiableClass_factory;
ptyp_ = new ObservableObject();
SimpleNotifiableClass.prototype = ptyp_;
ptyp_.strField = null;
ptyp_.intField = 0;
ptyp_.selfField = null;
ptyp_.objField = null;
ptyp_.set_strProp = function SimpleNotifiableClass__set_StrProp(value) {
  if (this.strField !== value) {
    this.strField = value;
    this.firePropertyChanged("StrProp");
  }
};
ptyp_.get_intProp = function SimpleNotifiableClass__get_IntProp() {
  return this.intField;
};
ptyp_.set_intProp = function SimpleNotifiableClass__set_IntProp(value) {
  if (this.intField != value) {
    this.intField = value;
    this.firePropertyChanged("IntProp");
  }
};
ptyp_.set_selfProp = function SimpleNotifiableClass__set_SelfProp(value) {
  if (this.selfField != value) {
    this.selfField = value;
    this.firePropertyChanged("SelfProp");
  }
};
ptyp_.get_objProp = function SimpleNotifiableClass__get_ObjProp() {
  return this.objField;
};
ptyp_.set_objProp = function SimpleNotifiableClass__set_ObjProp(value) {
  if (this.objField != value) {
    this.objField = value;
    this.firePropertyChanged("ObjProp");
  }
};
ptyp_.__ctor0 = function SimpleNotifiableClass____ctor() {
  this.__ctor();
};
Type__RegisterReferenceType(SimpleNotifiableClass, "Sunlight.Framework.Test.Binders.SimpleNotifiableClass", ObservableObject, []);
String.typeId = "x";
Type__RegisterReferenceType(String, "System.String", Object, []);
function ICollection() {
};
ICollection.typeId = "c";
Type__RegisterInterface(ICollection, "System.Collections.ICollection");
function ArrayImpl() {
};
ArrayImpl.typeId = "y";
ptyp_ = ArrayImpl.prototype;
ptyp_.system__Collections__ICollection__get_Count = function ArrayImpl__System__Collections__ICollection__get_Count() {
  return this.V_get_Length();
};
ptyp_.__ctor = function ArrayImpl____ctor() {
};
ptyp_.V_get_Count_c = ptyp_.system__Collections__ICollection__get_Count;
ptyp_.V_CopyTo_c = function(arg0, arg1) {
  return this.V_CopyTo(arg0, arg1);
};
Type__RegisterReferenceType(ArrayImpl, "System.ArrayImpl", Object, [ICollection]);
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
function Delegate__CreateGeneric(functionName, instanceOrType, f, genericArguments) {
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
function Function0() {
};
Function0.typeId = "z";
Function0.prototype = new Function();
Type__RegisterReferenceType(Function0, "System.MulticastDelegate", Function, []);
function BinderTestHelper() {
};
BinderTestHelper.typeId = "ab";
function BinderTestHelper_factory() {
  return new BinderTestHelper();
};
BinderTestHelper.defaultConstructor = BinderTestHelper_factory;
ptyp_ = BinderTestHelper.prototype;
ptyp_.get_sourceValueUpdateCalled = function BinderTestHelper__get_SourceValueUpdateCalled() {
  return this.SourceValueUpdateCalled;
};
ptyp_.set_sourceValueUpdateCalled = function BinderTestHelper__set_SourceValueUpdateCalled(value) {
  return this.SourceValueUpdateCalled = value;
};
ptyp_.sourceValueUpdated = function BinderTestHelper__SourceValueUpdated() {
  this.set_sourceValueUpdateCalled(true);
};
ptyp_.__ctor = function BinderTestHelper____ctor() {
};
ptyp_.V_SourceValueUpdated_f = ptyp_.sourceValueUpdated;
Type__RegisterReferenceType(BinderTestHelper, "Sunlight.Framework.Test.Binders.SourcePropertyBinderTests/BinderTestHelper", Object, [ISourceDataBinder]);
function IocContainer() {
};
IocContainer.typeId = "bb";
function IocContainer_factory() {
  var this_;
  this_ = new IocContainer();
  this_.__ctor();
  return this_;
};
IocContainer.defaultConstructor = IocContainer_factory;
ptyp_ = IocContainer.prototype;
ptyp_.factoryMap = null;
ptyp_.resolutionCount = 0;
ptyp_.register = function IocContainer__Register(T, factory) {
  var this_, typeRegistry;
  this_ = this;
  typeRegistry = TypeRegistry_factory(factory);
  this_.factoryMap.set_item(T.typeId, typeRegistry);
  return IocHelper_factory(function IocContainer__Register_del() {
    typeRegistry.set_isStatic(true);
  }, function IocContainer__Register_del2(type) {
    this_.factoryMap.set_item(type.typeId, typeRegistry);
  });
};
ptyp_.resolve = function IocContainer__Resolve(T) {
  if (this.resolutionCount > 100)
    throw new Error("Ioc has cycles, use ResolveLazy<T> to break cycle");
  this.resolutionCount++;
  try {
    return this.factoryMap.get_item(T.typeId).getValue();
  } finally {
    this.resolutionCount--;
  }
};
ptyp_.resolveLazy = function IocContainer__ResolveLazy(T) {
  var Lazy_$T$_;
  Lazy_$T$_ = Lazy(T, true);
  return Lazy_$T$_.__ctor(Delegate__CreateGeneric("getValue", this.factoryMap.get_item(T.typeId), this.factoryMap.get_item(T.typeId).getValue, [T]));
};
ptyp_.__ctor = function IocContainer____ctor() {
  this.factoryMap = StringDictionary_$TypeRegistry$_.defaultConstructor();
};
Type__RegisterReferenceType(IocContainer, "Sunlight.Framework.IocContainer", Object, []);
function IocTestType2() {
};
IocTestType2.typeId = "cb";
function IocTestType2_factory(x) {
  var this_;
  this_ = new IocTestType2();
  this_.__ctor(x);
  return this_;
};
ptyp_ = IocTestType2.prototype;
ptyp_.x = 0;
ptyp_.__ctor = function IocTestType2____ctor(x) {
  this.x = x;
};
ptyp_.testMethod = function IocTestType2__TestMethod() {
  return this.x;
};
Type__RegisterReferenceType(IocTestType2, "Sunlight.Framework.Test.IocTestType2", Object, []);
function IocTestType1Base() {
};
IocTestType1Base.typeId = "db";
function IocTestType1Base_factory(x) {
  var this_;
  this_ = new IocTestType1Base();
  this_.__ctor(x);
  return this_;
};
ptyp_ = IocTestType1Base.prototype;
ptyp_.x = 0;
ptyp_.__ctor = function IocTestType1Base____ctor(x) {
  this.x = x;
};
ptyp_.testMethodBase = function IocTestType1Base__TestMethodBase() {
  return this.x;
};
Type__RegisterReferenceType(IocTestType1Base, "Sunlight.Framework.Test.IocTestType1Base", Object, []);
function IIocTestType1() {
};
IIocTestType1.typeId = "b";
Type__RegisterInterface(IIocTestType1, "Sunlight.Framework.Test.IIocTestType1");
function IocTestType1() {
};
IocTestType1.typeId = "eb";
function IocTestType1_factory(x, y) {
  var this_;
  this_ = new IocTestType1();
  this_.__ctor0(x, y);
  return this_;
};
ptyp_ = new IocTestType1Base();
IocTestType1.prototype = ptyp_;
ptyp_.y = 0;
ptyp_.__ctor0 = function IocTestType1____ctor(x, y) {
  this.__ctor(x);
  this.y = y;
};
ptyp_.testMethod = function IocTestType1__TestMethod() {
  return this.y + this.testMethodBase();
};
ptyp_.V_TestMethod_b = ptyp_.testMethod;
Type__RegisterReferenceType(IocTestType1, "Sunlight.Framework.Test.IocTestType1", IocTestType1Base, [IIocTestType1]);
function IocHelper() {
};
IocHelper.typeId = "fb";
function IocHelper_factory(isSingleton, registerAs) {
  var this_;
  this_ = new IocHelper();
  this_.__ctor(isSingleton, registerAs);
  return this_;
};
ptyp_ = IocHelper.prototype;
ptyp_.isSingleton0 = null;
ptyp_.registerAs = null;
ptyp_.__ctor = function IocHelper____ctor(isSingleton, registerAs) {
  this.registerAs = registerAs;
  this.isSingleton0 = isSingleton;
};
ptyp_.as = function IocHelper__As(T) {
  this.registerAs(T);
  return this;
};
ptyp_.isSingleton = function IocHelper__IsSingleton() {
  this.isSingleton0();
  return this;
};
Type__RegisterReferenceType(IocHelper, "Sunlight.Framework.IoC.IocHelper", Object, []);
function EventBus() {
};
EventBus.typeId = "gb";
function EventBus_factory() {
  var this_;
  this_ = new EventBus();
  this_.__ctor();
  return this_;
};
EventBus.defaultConstructor = EventBus_factory;
ptyp_ = EventBus.prototype;
ptyp_.eventSubscriptsion = null;
ptyp_.oneTimeValues = null;
ptyp_.subscribe = function EventBus__Subscribe(T, callback) {
  var typeId, evt, registeredCallback;
  ExceptionHelpers__ThrowOnArgumentNull(callback, "callback");
  typeId = T.typeId;
  evt = this.oneTimeValues[typeId];
  if (!Object__IsNullOrUndefined(evt))
    callback(Type__UnBoxTypeInstance(T, evt));
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
    registeredCallback = Delegate__Combine(registeredCallback, callback);
};
ptyp_.unSubscribe = function EventBus__UnSubscribe(T, callback) {
  var typeId, registeredCallback, act;
  ExceptionHelpers__ThrowOnArgumentNull(callback, "callback");
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
    act = Delegate__Remove(act, callback);
    if (!act)
      this.eventSubscriptsion.remove(typeId);
  }
};
ptyp_.raise = function EventBus__Raise(T, evt) {
  var type, typeId, registeredCallback;
  ExceptionHelpers__ThrowOnArgumentNull(Type__BoxTypeInstance(T, evt), "evt");
  type = T;
  do {
    typeId = type.typeId;
    if (this.eventSubscriptsion.tryGetValue(typeId, {
      read: function() {
        return registeredCallback;
      },
      write: function(arg0) {
        return registeredCallback = arg0;
      }
    }))
      registeredCallback(evt);
    type = type.baseType;
  } while (type);
};
ptyp_.raiseOneTime = function EventBus__RaiseOneTime(T, evt) {
  var typeId, alreadyRegistered;
  ExceptionHelpers__ThrowOnArgumentNull(Type__BoxTypeInstance(T, evt), "evt");
  typeId = T.typeId;
  alreadyRegistered = this.oneTimeValues[typeId];
  if (!Object__IsNullOrUndefined(alreadyRegistered))
    throw new Error("Event " + T.name + " already raised");
  this.oneTimeValues[typeId] = Type__BoxTypeInstance(T, evt);
  this.raise(T, evt);
  this.eventSubscriptsion.remove(typeId);
};
ptyp_.__ctor = function EventBus____ctor() {
  this.eventSubscriptsion = StringDictionary_$Function$_.defaultConstructor();
  this.oneTimeValues = {
  };
};
Type__RegisterReferenceType(EventBus, "Sunlight.Framework.EventBus", Object, []);
function Evt1() {
};
Evt1.typeId = "hb";
function Evt1_factory() {
  return new Evt1();
};
Evt1.defaultConstructor = Evt1_factory;
ptyp_ = Evt1.prototype;
ptyp_.x = 0;
ptyp_.__ctor = function Evt1____ctor() {
};
Type__RegisterReferenceType(Evt1, "Sunlight.Framework.Test.EventBusTests/Evt1", Object, []);
function Evt2(boxedValue) {
  this.boxedValue = boxedValue;
};
Evt2.typeId = "ib";
Evt2.getDefaultValue = function() {
  return {
    x: 0
  };
};
Evt2.prototype = new ValueType();
Type__RegisterStructType(Evt2, "Sunlight.Framework.Test.EventBusTests/Evt2", []);
function ObservableTestObject() {
};
ObservableTestObject.typeId = "jb";
function ObservableTestObject_factory() {
  var this_;
  this_ = new ObservableTestObject();
  this_.__ctor0();
  return this_;
};
ObservableTestObject.defaultConstructor = ObservableTestObject_factory;
ptyp_ = new ObservableObject();
ObservableTestObject.prototype = ptyp_;
ptyp_.strField = null;
ptyp_.intProp = 0;
ptyp_.set_intProp = function ObservableTestObject__set_IntProp(value) {
  if (value != this.intProp) {
    this.intProp = value;
    this.firePropertyChanged("IntProp");
  }
};
ptyp_.set_stringProp = function ObservableTestObject__set_StringProp(value) {
  if (value !== this.strField) {
    this.strField = value;
    this.firePropertyChanged("StringProp");
  }
};
ptyp_.__ctor0 = function ObservableTestObject____ctor() {
  this.__ctor();
};
Type__RegisterReferenceType(ObservableTestObject, "Sunlight.Framework.Test.ObservableObjectTests/ObservableTestObject", ObservableObject, []);
function TypeRegistry() {
};
TypeRegistry.typeId = "kb";
function TypeRegistry_factory(factory) {
  var this_;
  this_ = new TypeRegistry();
  this_.__ctor(factory);
  return this_;
};
ptyp_ = TypeRegistry.prototype;
ptyp_.factory = null;
ptyp_.isCreated = false;
ptyp_.isStatic = false;
ptyp_.value = null;
ptyp_.__ctor = function TypeRegistry____ctor(factory) {
  this.factory = factory;
};
ptyp_.set_isStatic = function TypeRegistry__set_IsStatic(value) {
  this.isStatic = value;
};
ptyp_.getValue = function TypeRegistry__GetValue() {
  if (!this.isStatic)
    return this.factory();
  if (!this.isCreated) {
    this.value = this.factory();
    this.isCreated = true;
  }
  return this.value;
};
Type__RegisterReferenceType(TypeRegistry, "Sunlight.Framework.TypeRegistry", Object, []);
Error.typeId = "lb";
Type__RegisterReferenceType(Error, "System.Exception", Object, []);
function ExceptionHelpers() {
};
function ExceptionHelpers__ThrowOnOutOfRange(value, minValue, maxValue, argumentName) {
  if (value < minValue || value > maxValue)
    throw new Error("Out of range exception: " + argumentName);
};
function ExceptionHelpers__ThrowOnArgumentNull(value, argumentName) {
  if (value === null)
    throw new Error("ArgumentNull: " + argumentName);
};
Function.getDefaultValue = function() {
  return {
  };
};
function IValueConverter() {
};
IValueConverter.typeId = "e";
Type__RegisterInterface(IValueConverter, "Sunlight.Framework.Binders.IValueConverter");
Array.typeId = "ub";
Type__RegisterReferenceType(Array, "System.Array", Object, [ICollection]);
function ArrayG(T, $5fcallStatiConstructor) {
  var ArrayG$1_$T$_, T$5b$5d_$T$_, $5f_initTracker;
  if (ArrayG[T.typeId])
    return ArrayG[T.typeId];
  ArrayG[T.typeId] = function System__ArrayG$10() {
  };
  ArrayG$1_$T$_ = ArrayG[T.typeId];
  ArrayG$1_$T$_.genericParameters = [T];
  ArrayG$1_$T$_.genericClosure = ArrayG;
  ArrayG$1_$T$_.typeId = "mb$" + T.typeId + "$";
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
  ptyp_.__ctor0 = function ArrayG$1____ctor0(size) {
    var def, i;
    this.__ctor();
    this.innerArray = new Array(size);
    def = Type__GetDefaultValueStatic(T);
    for (i = 0; i < size; i++)
      this.innerArray[i] = def;
  };
  ptyp_.__ctor1 = function ArrayG$1____ctor(nativeArray) {
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
      throw "index " + index + " out of range";
    return arr[index];
  };
  ptyp_.set_item = function ArrayG$1__set_Item(index, value) {
    var arr;
    arr = this.innerArray;
    if (index < 0 || index >= arr.length)
      throw "index " + index + " out of range";
    return arr[index] = value;
  };
  ptyp_.get_innerArray = function ArrayG$1__get_InnerArray() {
    return this.innerArray;
  };
  ptyp_.copyTo = function ArrayG$1__CopyTo0(arr, index) {
    var nativeArray, length, nativeArrDst, i;
    nativeArray = this.innerArray;
    length = nativeArray.length;
    nativeArrDst = NativeArray$1__op_Implicit(arr);
    if (nativeArrDst.length < index + nativeArray.length)
      throw new Error("can't copy, dest array too small.");
    for (i = 0; i < length; i++)
      nativeArrDst[i + index] = nativeArray[i];
  };
  ptyp_.copyTo0 = function ArrayG$1__CopyTo(array, index) {
    var arr;
    arr = Type__CastType(T$5b$5d_$T$_, array);
    this.copyTo(arr, index);
  };
  ptyp_.V_get_Length = ptyp_.get_length;
  ptyp_.V_CopyTo = ptyp_.copyTo0;
  Type__RegisterReferenceType(ArrayG$1_$T$_, "System.ArrayG`1<" + T.fullName + ">", ArrayImpl, [ICollection]);
  ArrayG$1_$T$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    ArrayG$1_$T$_ = ArrayG(T, true);
    T$5b$5d_$T$_ = ArrayG(T, true);
  };
  if ($5fcallStatiConstructor)
    ArrayG$1_$T$_.$5ftri();
  return ArrayG$1_$T$_;
};
function Func(T1, TRes, $5fcallStatiConstructor) {
  var Func$2_$T1_x_TRes$_, $5f_initTracker;
  if (Func[T1.typeId] && Func[T1.typeId][TRes.typeId])
    return Func[T1.typeId][TRes.typeId];
    Func[T1.typeId] = {
    };
  Func[T1.typeId][TRes.typeId] = function System__Func$20() {
  };
  Func$2_$T1_x_TRes$_ = Func[T1.typeId][TRes.typeId];
  Func$2_$T1_x_TRes$_.genericParameters = [T1, TRes];
  Func$2_$T1_x_TRes$_.genericClosure = Func;
  Func$2_$T1_x_TRes$_.typeId = "nb$" + T1.typeId + "_" + TRes.typeId + "$";
  Func$2_$T1_x_TRes$_.prototype = new Function0();
  Type__RegisterReferenceType(Func$2_$T1_x_TRes$_, "System.Func`2<" + T1.fullName + "," + TRes.fullName + ">", Function0, []);
  Func$2_$T1_x_TRes$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T1 = T1;
    TRes = TRes;
    Func$2_$T1_x_TRes$_ = Func(T1, TRes, true);
  };
  if ($5fcallStatiConstructor)
    Func$2_$T1_x_TRes$_.$5ftri();
  return Func$2_$T1_x_TRes$_;
};
function Lazy(T, $5fcallStatiConstructor) {
  var Lazy$1_$T$_, $5f_initTracker;
  if (Lazy[T.typeId])
    return Lazy[T.typeId];
  Lazy[T.typeId] = function Sunlight__Framework__Lazy$10() {
  };
  Lazy$1_$T$_ = Lazy[T.typeId];
  Lazy$1_$T$_.genericParameters = [T];
  Lazy$1_$T$_.genericClosure = Lazy;
  Lazy$1_$T$_.typeId = "ob$" + T.typeId + "$";
  Lazy$1_$T$_.__ctor = function Sunlight_Framework_Lazy$1_factory0(factory) {
    var this_;
    this_ = new Lazy$1_$T$_();
    this_.__ctor(factory);
    return this_;
  };
  ptyp_ = Lazy$1_$T$_.prototype;
  ptyp_.factory = null;
  ptyp_.value = Type__GetDefaultValueStatic(T);
  ptyp_.createdCallbacks = null;
  ptyp_.__ctor = function Lazy$1____ctor(factory) {
    ExceptionHelpers__ThrowOnArgumentNull(factory, "factory");
    this.factory = factory;
  };
  ptyp_.get_value = function Lazy$1__get_Value() {
    if (this.factory) {
      this.value = this.factory();
      this.factory = null;
      if (this.createdCallbacks) {
        this.createdCallbacks();
        this.createdCallbacks = null;
      }
    }
    return this.value;
  };
  Type__RegisterReferenceType(Lazy$1_$T$_, "Sunlight.Framework.Lazy`1<" + T.fullName + ">", Object, []);
  Lazy$1_$T$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    Lazy$1_$T$_ = Lazy(T, true);
  };
  if ($5fcallStatiConstructor)
    Lazy$1_$T$_.$5ftri();
  return Lazy$1_$T$_;
};
function ObservableCollection(T, $5fcallStatiConstructor) {
  var List$1_$T$_, ArrayG$1_$T$_, CollectionChangedEventArgs$1_$T$_, ObservableCollection$1_$T$_, $5f_initTracker, $5f_initTracker0;
  if (ObservableCollection[T.typeId])
    return ObservableCollection[T.typeId];
  ObservableCollection[T.typeId] = function Sunlight__Framework__Observables__ObservableCollection$10() {
  };
  ObservableCollection$1_$T$_ = ObservableCollection[T.typeId];
  ObservableCollection$1_$T$_.genericParameters = [T];
  ObservableCollection$1_$T$_.genericClosure = ObservableCollection;
  ObservableCollection$1_$T$_.typeId = "pb$" + T.typeId + "$";
  ObservableCollection$1_$T$_.defaultConstructor = function Sunlight_Framework_Observables_ObservableCollection$1_factory0() {
    var this_;
    this_ = new ObservableCollection$1_$T$_();
    this_.__ctor0();
    return this_;
  };
  ptyp_ = new ObservableObject();
  ObservableCollection$1_$T$_.prototype = ptyp_;
  ptyp_.busy = false;
  ptyp_.items = null;
  ptyp_.collectionChanged = null;
  ptyp_.__ctor0 = function ObservableCollection$1____ctor() {
    this.__ctor();
    this.items = List$1_$T$_.defaultConstructor();
  };
  ptyp_.add_CollectionChanged = function ObservableCollection$1__add_CollectionChanged(value) {
    this.collectionChanged = Delegate__Combine(this.collectionChanged, value);
  };
  ptyp_.get_count = function ObservableCollection$1__get_Count() {
    return this.items.get_count();
  };
  ptyp_.add = function ObservableCollection$1__Add(o) {
    this.checkReentrancy();
    this.items.add(o);
    this.onCollectionChanged(0, this.get_count() - 1, ArrayG$1_$T$_.__ctor([o]), null);
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
  ptyp_.checkReentrancy = function ObservableCollection$1__CheckReentrancy() {
    if (this.busy)
      throw Error.createError("InvalidOperationException", null);
  };
  ptyp_.onCollectionChanged = function ObservableCollection$1__OnCollectionChanged(action, index, newItems, oldItems) {
    var collectionChangedArgs;
    if (this.collectionChanged) {
      this.busy = true;
      try {
        collectionChangedArgs = CollectionChangedEventArgs$1_$T$_.__ctor(action, index, newItems, oldItems);
        this.collectionChanged(this, collectionChangedArgs);
      } finally {
        this.busy = false;
      }
    }
  };
  Type__RegisterReferenceType(ObservableCollection$1_$T$_, "Sunlight.Framework.Observables.ObservableCollection`1<" + T.fullName + ">", ObservableObject, [INotifyPropertyChanged]);
  ObservableCollection$1_$T$_.$5ftri = function() {
    if ($5f_initTracker0)
      return;
    $5f_initTracker0 = true;
    List$1_$T$_ = List(T, true);
    ArrayG$1_$T$_ = ArrayG(T, true);
    CollectionChangedEventArgs$1_$T$_ = CollectionChangedEventArgs(T, true);
    T = T;
    ObservableCollection$1_$T$_ = ObservableCollection(T, true);
  };
  if ($5fcallStatiConstructor)
    ObservableCollection$1_$T$_.$5ftri();
  return ObservableCollection$1_$T$_;
};
function CollectionChangedEventArgs(T, $5fcallStatiConstructor) {
  var CollectionChangedEventArgs$1_$T$_, $5f_initTracker;
  if (CollectionChangedEventArgs[T.typeId])
    return CollectionChangedEventArgs[T.typeId];
  CollectionChangedEventArgs[T.typeId] = function Sunlight__Framework__Observables__CollectionChangedEventArgs$10() {
  };
  CollectionChangedEventArgs$1_$T$_ = CollectionChangedEventArgs[T.typeId];
  CollectionChangedEventArgs$1_$T$_.genericParameters = [T];
  CollectionChangedEventArgs$1_$T$_.genericClosure = CollectionChangedEventArgs;
  CollectionChangedEventArgs$1_$T$_.typeId = "qb$" + T.typeId + "$";
  CollectionChangedEventArgs$1_$T$_.__ctor = function Sunlight_Framework_Observables_CollectionChangedEventArgs$1_factory0(action, changeIndex, newItems, oldItems) {
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
  ptyp_.__ctor = function CollectionChangedEventArgs$1____ctor(action, changeIndex, newItems, oldItems) {
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
  ptyp_.get_changeIndex = function CollectionChangedEventArgs$1__get_ChangeIndex() {
    return this.changeIndex;
  };
  ptyp_.get_newItems = function CollectionChangedEventArgs$1__get_NewItems() {
    return this.newItems;
  };
  ptyp_.get_oldItems = function CollectionChangedEventArgs$1__get_OldItems() {
    return this.oldItems;
  };
  Type__RegisterReferenceType(CollectionChangedEventArgs$1_$T$_, "Sunlight.Framework.Observables.CollectionChangedEventArgs`1<" + T.fullName + ">", Object, []);
  CollectionChangedEventArgs$1_$T$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    CollectionChangedEventArgs$1_$T$_ = CollectionChangedEventArgs(T, true);
  };
  if ($5fcallStatiConstructor)
    CollectionChangedEventArgs$1_$T$_.$5ftri();
  return CollectionChangedEventArgs$1_$T$_;
};
function Action(T1, T2, $5fcallStatiConstructor) {
  var Action$2_$T1_x_T2$_, $5f_initTracker;
  if (Action[T1.typeId] && Action[T1.typeId][T2.typeId])
    return Action[T1.typeId][T2.typeId];
    Action[T1.typeId] = {
    };
  Action[T1.typeId][T2.typeId] = function System__Action$20() {
  };
  Action$2_$T1_x_T2$_ = Action[T1.typeId][T2.typeId];
  Action$2_$T1_x_T2$_.genericParameters = [T1, T2];
  Action$2_$T1_x_T2$_.genericClosure = Action;
  Action$2_$T1_x_T2$_.typeId = "rb$" + T1.typeId + "_" + T2.typeId + "$";
  Action$2_$T1_x_T2$_.prototype = new Function0();
  Type__RegisterReferenceType(Action$2_$T1_x_T2$_, "System.Action`2<" + T1.fullName + "," + T2.fullName + ">", Function0, []);
  Action$2_$T1_x_T2$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T1 = T1;
    T2 = T2;
    Action$2_$T1_x_T2$_ = Action(T1, T2, true);
  };
  if ($5fcallStatiConstructor)
    Action$2_$T1_x_T2$_.$5ftri();
  return Action$2_$T1_x_T2$_;
};
function INotifyPropertyChanged() {
};
INotifyPropertyChanged.typeId = "d";
Type__RegisterInterface(INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function StringDictionary(TValue, $5fcallStatiConstructor) {
  var StringDictionary$1_$TValue$_, KeyValuePair$2_$String_x_TValue$_, $5f_initTracker;
  if (StringDictionary[TValue.typeId])
    return StringDictionary[TValue.typeId];
  StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$10() {
  };
  StringDictionary$1_$TValue$_ = StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "sb$" + TValue.typeId + "$";
  KeyValuePair$2_$String_x_TValue$_ = KeyValuePair(String, TValue, $5fcallStatiConstructor);
  KeyValuePair$2_$String_x_TValue$_ = KeyValuePair(String, TValue, $5fcallStatiConstructor);
  StringDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_StringDictionary$1_factory0() {
    var this_;
    this_ = new StringDictionary$1_$TValue$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = StringDictionary$1_$TValue$_.prototype;
  ptyp_.innerDict = null;
  ptyp_.count = 0;
  ptyp_.__ctor = function StringDictionary$1____ctor() {
    this.innerDict = {
    };
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
  ptyp_.copyTo = function StringDictionary$1__CopyTo(array, index) {
    var keys, i;
    keys = Type__CastType(ArrayG_$String$_, this.get_keys());
    for (i = 0; i < keys.V_get_Length(); i++)
      array.setValue(i + index, Type__BoxTypeInstance(KeyValuePair$2_$String_x_TValue$_, KeyValuePair$2_$String_x_TValue$_.__ctor(keys.get_item(i), this.get_item(keys.get_item(i)))));
  };
  ptyp_.getKeys = function StringDictionary$1__GetKeys() {
    var rv, key;
    rv = [];
    for (key in this.innerDict)
      rv.push(key);
    return rv;
  };
  ptyp_.V_get_Count_c = ptyp_.get_count;
  ptyp_.V_CopyTo_c = ptyp_.copyTo;
  Type__RegisterReferenceType(StringDictionary$1_$TValue$_, "System.Collections.Generic.StringDictionary`1<" + TValue.fullName + ">", Object, [ICollection]);
  StringDictionary$1_$TValue$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    TValue = TValue;
    StringDictionary$1_$TValue$_ = StringDictionary(TValue, true);
  };
  if ($5fcallStatiConstructor)
    StringDictionary$1_$TValue$_.$5ftri();
  return StringDictionary$1_$TValue$_;
};
function List(T, $5fcallStatiConstructor) {
  var List$1_$T$_, $5f_initTracker;
  if (List[T.typeId])
    return List[T.typeId];
  List[T.typeId] = function System__Collections__Generic__List$10() {
  };
  List$1_$T$_ = List[T.typeId];
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = List;
  List$1_$T$_.typeId = "tb$" + T.typeId + "$";
  List$1_$T$_.defaultConstructor = function System_Collections_Generic_List$1_factory0() {
    var this_;
    this_ = new List$1_$T$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = List$1_$T$_.prototype;
  ptyp_.nativeArray = null;
  ptyp_.system__Collections__ICollection__CopyTo = function List$1__System__Collections__ICollection__CopyTo(array, index) {
    var nativeArray, length, i;
    nativeArray = this.nativeArray;
    length = nativeArray.length;
    for (i = 0; i < length; i++)
      array.setValue(i + index, Type__BoxTypeInstance(T, nativeArray[i]));
  };
  ptyp_.__ctor = function List$1____ctor() {
    this.nativeArray = new Array(0);
  };
  ptyp_.get_item = function List$1__get_Item(index) {
    var arr;
    arr = this.nativeArray;
    if (index < 0 || index >= arr.length)
      throw "index " + index + " out of range";
    return arr[index];
  };
  ptyp_.removeAt = function List$1__RemoveAt(index) {
    NativeArray$1__RemoveAt(this.nativeArray, index);
  };
  ptyp_.get_count = function List$1__get_Count() {
    return this.nativeArray.length;
  };
  ptyp_.add = function List$1__Add(item) {
    this.nativeArray.push(item);
  };
  ptyp_.V_CopyTo_c = ptyp_.system__Collections__ICollection__CopyTo;
  ptyp_.V_get_Count_c = ptyp_.get_count;
  Type__RegisterReferenceType(List$1_$T$_, "System.Collections.Generic.List`1<" + T.fullName + ">", Object, [ICollection]);
  List$1_$T$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    List$1_$T$_ = List(T, true);
  };
  if ($5fcallStatiConstructor)
    List$1_$T$_.$5ftri();
  return List$1_$T$_;
};
function NativeArray$1__RemoveAt(this_, index) {
  var len, i;
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  len = this_.length - 1;
  for (i = index; i < len; i++)
    this_[i] = this_[i + 1];
  this_.pop();
};
function NativeArray$1__op_Implicit(n) {
  return n.get_innerArray();
};
function KeyValuePair(K, V, $5fcallStatiConstructor) {
  var KeyValuePair$2_$K_x_V$_, $5f_initTracker;
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
  KeyValuePair$2_$K_x_V$_.typeId = "vb$" + K.typeId + "_" + V.typeId + "$";
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
  KeyValuePair$2_$K_x_V$_.prototype = new ValueType();
  Type__RegisterStructType(KeyValuePair$2_$K_x_V$_, "System.Collections.Generic.KeyValuePair`2<" + K.fullName + "," + V.fullName + ">", []);
  KeyValuePair$2_$K_x_V$_.$5ftri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    KeyValuePair$2_$K_x_V$_ = KeyValuePair(K, V, true);
  };
  if ($5fcallStatiConstructor)
    KeyValuePair$2_$K_x_V$_.$5ftri();
  return KeyValuePair$2_$K_x_V$_;
};
ArrayG_$String$_ = ArrayG(String);
Func_$Object_x_Object$_ = Func(Object, Object);
ArrayG_$Func_$Object_x_Object$_$_ = ArrayG(Func_$Object_x_Object$_);
ObservableCollection_$Int32$_ = ObservableCollection(Int32);
ArrayG_$Object$_ = ArrayG(Object);
Action_$INotifyPropertyChanged_x_String$_ = Action(INotifyPropertyChanged, String);
ArrayG_$Action_$INotifyPropertyChanged_x_String$_$_ = ArrayG(Action_$INotifyPropertyChanged_x_String$_);
StringDictionary_$TypeRegistry$_ = StringDictionary(TypeRegistry);
StringDictionary_$Function$_ = StringDictionary(Function);
StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_ = StringDictionary(Action_$INotifyPropertyChanged_x_String$_);
ArrayG_$String$_.$5ftri();
Func_$Object_x_Object$_.$5ftri();
ArrayG_$Func_$Object_x_Object$_$_.$5ftri();
ObservableCollection_$Int32$_.$5ftri();
ArrayG_$Object$_.$5ftri();
Action_$INotifyPropertyChanged_x_String$_.$5ftri();
ArrayG_$Action_$INotifyPropertyChanged_x_String$_$_.$5ftri();
StringDictionary_$TypeRegistry$_.$5ftri();
StringDictionary_$Function$_.$5ftri();
StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_.$5ftri();
QUnit.module("Sunlight.Framework.Test.Binders.DataBinderTests", {
});
QUnit.test("BasicBinderSimpleObjectTest", DataBinderTests__BasicBinderSimpleObjectTest);
QUnit.test("BasicBinderSimpleObjectTestReverseOrder", DataBinderTests__BasicBinderSimpleObjectTestReverseOrder);
QUnit.test("BasicBinderOneWayNotifiableObjectTest", DataBinderTests__BasicBinderOneWayNotifiableObjectTest);
QUnit.test("BasicBinderTwoWayNotifiableObjectTest", DataBinderTests__BasicBinderTwoWayNotifiableObjectTest);
QUnit.module("Sunlight.Framework.Test.Binders.SourcePropertyBinderTests", {
});
QUnit.test("BasicValueTest", SourcePropertyBinderTests__BasicValueTest);
QUnit.test("BasicValueTestWithNotification", SourcePropertyBinderTests__BasicValueTestWithNotification);
QUnit.test("PropertyPathValueNotifiableTest", SourcePropertyBinderTests__PropertyPathValueNotifiableTest);
QUnit.test("PropertyPathValueTest", SourcePropertyBinderTests__PropertyPathValueTest);
QUnit.module("Sunlight.Framework.Test.ContainerTests", {
});
QUnit.test("TestRegisterResolve", ContainerTests__TestRegisterResolve);
QUnit.test("TestRegisterResolveWithAs", ContainerTests__TestRegisterResolveWithAs);
QUnit.test("TestRegisterResolveIsSingleton", ContainerTests__TestRegisterResolveIsSingleton);
QUnit.test("TestRegisterResolveLazy", ContainerTests__TestRegisterResolveLazy);
QUnit.module("Sunlight.Framework.Test.EventBusTests", {
});
QUnit.test("TestSubscribeAndRaise", EventBusTests__TestSubscribeAndRaise);
QUnit.test("TestSubscribeAndRaiseOnceTime", EventBusTests__TestSubscribeAndRaiseOnceTime);
QUnit.test("TestSubscribeUnSubscribeAndRaise", EventBusTests__TestSubscribeUnSubscribeAndRaise);
QUnit.module("Sunlight.Framework.Test.ObservableCollectionTests", {
});
QUnit.test("TestCreateNewObservableCollection", ObservableCollectionTests__TestCreateNewObservableCollection);
QUnit.test("TestAddItemToObservableCollection", ObservableCollectionTests__TestAddItemToObservableCollection);
QUnit.test("TestRemoveItemToObservableCollection", ObservableCollectionTests__TestRemoveItemToObservableCollection);
QUnit.module("Sunlight.Framework.Test.ObservableObjectTests", {
});
QUnit.test("TestCreateNewObservableObject", ObservableObjectTests__TestCreateNewObservableObject);
QUnit.test("TestFirePropertyChanged", ObservableObjectTests__TestFirePropertyChanged);
QUnit.test("TestRemovePropertyChangeCallback", ObservableObjectTests__TestRemovePropertyChangeCallback);
})();
//# sourceMappingURL=Sunlight.Framework.Test.map
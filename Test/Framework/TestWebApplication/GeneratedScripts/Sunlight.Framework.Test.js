(function(){
var TestObservabelCollectionTransformer___notificationCount, TestObservabelCollectionTransformer___expectedCounts, List_$Int32$_, TestObservabelCollectionTransformer___expectedIndexes, TestObservabelCollectionTransformer___list, List_$Number$_, ObservableCollection_$Int32$_, ObservableCollection_$Number$_, HeaderInjectableTransformer_$Number_x_Number$_, ArrayG_$Int32$_, TestObservabelCollectionTransformer___listTransformer, ObservableCollectionGenerator_$Number_x_String$_, ArrayG_$String$_, ArrayG_$Func_$Object_x_Object$_$_, Type__typeMapping, StringDictionary_$TypeRegistry$_, StringDictionary_$Function$_, StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_, ArrayG_$Object$_, ArrayG_$Action_$INotifyPropertyChanged_x_String$_$_, ptyp_, Func_$Object_x_Object$_, Action_$INotifyPropertyChanged_x_String$_;
Function.typeId = "m";
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
Object.typeId = "n";
function Object__Equals(obj1, obj2) {
  return obj1 === obj2;
};
function Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
Type__RegisterReferenceType(Object, "System.Object", null, []);
function ContainerTests() {
};
ContainerTests.typeId = "o";
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
  assert.ok(!!t2, "t2 != null");
  assert.equal(1, t2.testMethod(), "1 == t1.TestMethod()");
  t1 = container.resolve(IocTestType1);
  assert.ok(!!t1, "t1 != null");
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
  assert.ok(!!t2, "t2 != null");
  assert.equal(1, t2.testMethod(), "1 == t1.TestMethod()");
  t1 = container.resolve(IIocTestType1);
  assert.ok(!!t1, "t1 != null");
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
  assert.ok(!!t2, "t2 != null");
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
EventBusTests.typeId = "p";
function EventBusTests__TestSubscribeAndRaise(assert) {
  var evtBus, x1, x2, stmtTemp1;
  evtBus = EventBus_factory();
  x1 = 0;
  x2 = 0;
  evtBus.subscribe(Evt1, function EventBusTests__TestSubscribeAndRaise_del(evt) {
    x1 = evt.x;
    return;
  });
  evtBus.subscribe(Evt2, function EventBusTests__TestSubscribeAndRaise_del2(evt) {
    x2 = evt.x;
    return;
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
    return;
  };
  evtBus.subscribe(Evt1, del);
  evtBus.subscribe(Evt2, function EventBusTests__TestSubscribeAndRaiseOnceTime_del2(evt) {
    x2 = evt.x;
    return;
  });
  evtBus.raiseOneTime(Evt1, (stmtTemp1 = Evt1_factory(), stmtTemp1.x = 10, stmtTemp1));
  assert.equal(10, x1, "10 == x1");
  assert.equal(0, x2, "0 == x2");
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
    return;
  };
  evtBus.subscribe(Evt2, function EventBusTests__TestSubscribeUnSubscribeAndRaise_del2(evt) {
    x2 = evt.x;
    return;
  });
  evtBus.subscribe(Evt1, function EventBusTests__TestSubscribeUnSubscribeAndRaise_del3(evt) {
    x2 = evt.x;
    return;
  });
  evtBus.unSubscribe(Evt1, del);
  evtBus.raise(Evt1, (stmtTemp1 = Evt1_factory(), stmtTemp1.x = 10, stmtTemp1));
  assert.equal(0, x1, "0 == x1");
  assert.equal(10, x2, "10 == x2");
};
Type__RegisterReferenceType(EventBusTests, "Sunlight.Framework.Test.EventBusTests", Object, []);
function ObservableCollectionTests() {
};
ObservableCollectionTests.typeId = "q";
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
    return;
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
    return;
  });
  observableCollection.removeRangeAt(1, 2);
  assert.ok(eventRaised, "Change event raised");
};
function ObservableCollectionTests__TestReplaceRangeObservableCollection(assert) {
  var observableCollection, eventRaised, replaceList;
  observableCollection = ObservableCollection_$Int32$_.defaultConstructor();
  eventRaised = false;
  observableCollection.add(1);
  observableCollection.add(2);
  observableCollection.add(3);
  replaceList = List_$Int32$_.defaultConstructor();
  replaceList.add(12);
  replaceList.add(13);
  observableCollection.add_CollectionChanged(function ObservableCollectionTests__TestReplaceRangeObservableCollection_del(coll, evtArg) {
    assert.equal(evtArg.get_oldItems().V_get_Count_c(), 2, "evtArg.OldItems.Count");
    assert.ok(!Object__IsNullOrUndefined(evtArg.get_newItems()), "Object.IsNullOrUndefined(evtArg.NewItems)");
    assert.equal(evtArg.get_newItems(), replaceList, "ObservableCollection");
    assert.equal(evtArg.get_changeIndex(), 1, "evtArg.changeIndex");
    eventRaised = true;
    return;
  });
  observableCollection.replaceRangeAt(1, replaceList);
  assert.ok(eventRaised, "Change event raised");
};
function ObservableCollectionTests__TestGetRangeObservableCollection(assert) {
  var observableCollection;
  observableCollection = ObservableCollection_$Int32$_.defaultConstructor();
  observableCollection.add(1);
  observableCollection.add(2);
  observableCollection.add(3);
  observableCollection.add(4);
  observableCollection.add(5);
  observableCollection.add(6);
  ObservableCollectionTests__CheckGetAsserts(assert, observableCollection, 1, 0);
  ObservableCollectionTests__CheckGetAsserts(assert, observableCollection, 1, 1);
  ObservableCollectionTests__CheckGetAsserts(assert, observableCollection, 1, 2);
  ObservableCollectionTests__CheckGetAsserts(assert, observableCollection, 1, 5);
  ObservableCollectionTests__CheckGetAsserts(assert, observableCollection, 1, 6);
  ObservableCollectionTests__CheckGetAsserts(assert, observableCollection, 1, -1);
  ObservableCollectionTests__CheckGetAsserts(assert, observableCollection, -1, 0);
  ObservableCollectionTests__CheckGetAsserts(assert, observableCollection, 0, 6);
};
function ObservableCollectionTests__CheckGetAsserts(assert, observableCollection, index, count) {
  var stmtTemp1, range, idx;
  try {
    range = observableCollection.getRangeAt(index, count);
    assert.equal(range.get_count(), count, "count of items returned is equal for index: " + Type__BoxTypeInstance(Int32, index) + " ,count: " + Type__BoxTypeInstance(Int32, count));
    for (idx = 0; idx < count; idx++)
      assert.equal(range.get_item(idx), observableCollection.get_item(index + idx), "Index: " + Type__BoxTypeInstance(Int32, idx) + " value: " + Type__BoxTypeInstance(Int32, range.get_item(idx)) + " is equal");
  } catch (stmtTemp1) {
    assert.ok(index < 0 || index > observableCollection.get_count() - 1 || count <= 0 || count > observableCollection.get_count() || index + count < 0 || index + count > observableCollection.get_count() - 1, "Improper value of parameters for index : " + Type__BoxTypeInstance(Int32, index) + " ,count : " + Type__BoxTypeInstance(Int32, count));
  }
};
Type__RegisterReferenceType(ObservableCollectionTests, "Sunlight.Framework.Test.ObservableCollectionTests", Object, []);
function ObservableObjectTests() {
};
ObservableObjectTests.typeId = "r";
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
    return;
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
    cbCalled = true;
    return;
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
function TestInjectableTransformation() {
};
TestInjectableTransformation.typeId = "s";
function TestInjectableTransformation__TestHeaderInjectionInsert(assert) {
  var collection, newItemCount, itemsRemoved, insertIndex, expectedIndex, rangeToInsert, eventRaised, transformer, transformedCollChanged;
  collection = ObservableCollection_$Number$_.defaultConstructor();
  collection.add(110);
  collection.add(111);
  collection.add(198);
  collection.add(199);
  newItemCount = 0;
  itemsRemoved = 0;
  insertIndex = 0;
  expectedIndex = 0;
  rangeToInsert = List_$Number$_.defaultConstructor();
  eventRaised = false;
  transformer = HeaderInjectableTransformer_$Number_x_Number$_.__ctor(TestInjectableTransformation__GenerateHeader, collection);
  transformedCollChanged = function TestInjectableTransformation__TestHeaderInjectionInsert_del(coll, evtArg) {
    if (evtArg.get_action() == 0) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_newItems()), "----- " + rangeToInsert.get_item(0) + " " + rangeToInsert.get_item(1) + "---- Item Added");
      assert.equal(evtArg.get_newItems().V_get_Count_c(), newItemCount, "Number of Items Added");
      assert.equal(evtArg.get_changeIndex(), expectedIndex, "Index changed");
    }
    else if (evtArg.get_action() == 1) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_oldItems()), "----- " + rangeToInsert.get_item(0) + " " + rangeToInsert.get_item(1) + "----- Item Removed");
      assert.equal(itemsRemoved, evtArg.get_oldItems().V_get_Count_c(), "Number of Items Removed");
      assert.equal(evtArg.get_changeIndex(), expectedIndex, "Index changed");
    }
    eventRaised = true;
    return;
  };
  transformer.get_transformedCollection().add_CollectionChanged(transformedCollChanged);
  rangeToInsert.clear();
  rangeToInsert.add(112);
  rangeToInsert.add(113);
  insertIndex = 0;
  expectedIndex = 0;
  newItemCount = 3;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(196);
  rangeToInsert.add(197);
  insertIndex = 4;
  expectedIndex = 5;
  newItemCount = 3;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(119);
  rangeToInsert.add(120);
  insertIndex = 4;
  expectedIndex = 5;
  newItemCount = 4;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(189);
  rangeToInsert.add(191);
  insertIndex = 6;
  expectedIndex = 8;
  newItemCount = 4;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(142);
  rangeToInsert.add(143);
  insertIndex = 6;
  expectedIndex = 8;
  newItemCount = 4;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(158);
  rangeToInsert.add(159);
  insertIndex = 8;
  expectedIndex = 11;
  newItemCount = 4;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(139);
  rangeToInsert.add(140);
  insertIndex = 6;
  expectedIndex = 8;
  newItemCount = 4;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(169);
  rangeToInsert.add(170);
  insertIndex = 12;
  expectedIndex = 17;
  newItemCount = 5;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(149);
  rangeToInsert.add(150);
  insertIndex = 10;
  expectedIndex = 14;
  newItemCount = 3;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(109);
  rangeToInsert.add(111);
  insertIndex = 0;
  expectedIndex = 0;
  newItemCount = 4;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(108);
  rangeToInsert.add(107);
  insertIndex = 0;
  expectedIndex = 0;
  newItemCount = 3;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(95);
  rangeToInsert.add(99);
  insertIndex = 0;
  expectedIndex = 0;
  newItemCount = 4;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(71);
  rangeToInsert.add(89);
  insertIndex = 0;
  expectedIndex = 0;
  newItemCount = 5;
  itemsRemoved = 1;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(200);
  rangeToInsert.add(201);
  insertIndex = 30;
  expectedIndex = 43;
  newItemCount = 3;
  itemsRemoved = 0;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(211);
  rangeToInsert.add(221);
  insertIndex = 32;
  expectedIndex = 46;
  newItemCount = 4;
  itemsRemoved = 0;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(222);
  rangeToInsert.add(225);
  insertIndex = 34;
  expectedIndex = 50;
  newItemCount = 2;
  itemsRemoved = 0;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(229);
  rangeToInsert.add(231);
  insertIndex = 36;
  expectedIndex = 52;
  newItemCount = 3;
  itemsRemoved = 0;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  rangeToInsert.clear();
  rangeToInsert.add(192);
  rangeToInsert.add(194);
  insertIndex = 26;
  expectedIndex = 39;
  newItemCount = 2;
  itemsRemoved = 0;
  transformer.get_inputCollection().insertRangeAt(insertIndex, rangeToInsert);
  transformer.get_transformedCollection().remove_CollectionChanged(transformedCollChanged);
  assert.ok(eventRaised, "Change event raised");
};
function TestInjectableTransformation__TestHeaderInjectionRemove(assert) {
  var collection, itemsAdded, itemsRemoved, removeIndex, expectedIndex, rangeToRemove, eventRaised, transformer, transformedCollChanged;
  collection = ObservableCollection_$Number$_.defaultConstructor();
  collection.add(1);
  collection.add(2);
  collection.add(3);
  collection.add(5);
  collection.add(12);
  collection.add(22);
  collection.add(32);
  collection.add(33);
  collection.add(34);
  collection.add(35);
  collection.add(36);
  collection.add(37);
  collection.add(38);
  collection.add(39);
  collection.add(43);
  collection.add(60);
  collection.add(61);
  collection.add(61);
  collection.add(64);
  collection.add(65);
  collection.add(76);
  collection.add(77);
  collection.add(82);
  collection.add(91);
  collection.add(92);
  collection.add(97);
  collection.add(98);
  collection.add(99);
  itemsAdded = 0;
  itemsRemoved = 0;
  removeIndex = 0;
  expectedIndex = 0;
  rangeToRemove = List_$Number$_.defaultConstructor();
  eventRaised = false;
  transformer = HeaderInjectableTransformer_$Number_x_Number$_.__ctor(TestInjectableTransformation__GenerateHeader, collection);
  transformedCollChanged = function TestInjectableTransformation__TestHeaderInjectionRemove_del(coll, evtArg) {
    if (evtArg.get_action() == 0) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_newItems()), "----- " + rangeToRemove.get_item(0) + " - " + rangeToRemove.get_item(rangeToRemove.get_count() - 1) + "----- Item Added");
      assert.equal(evtArg.get_newItems().V_get_Count_c(), itemsAdded, "Number of Items Added");
      assert.equal(evtArg.get_changeIndex(), expectedIndex, "Index changed");
    }
    else if (evtArg.get_action() == 1) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_oldItems()), "----- " + rangeToRemove.get_item(0) + " - " + rangeToRemove.get_item(rangeToRemove.get_count() - 1) + "----- Item Removed");
      assert.equal(itemsRemoved, evtArg.get_oldItems().V_get_Count_c(), "Number of Items Removed");
      assert.equal(evtArg.get_changeIndex(), expectedIndex, "Index changed");
    }
    eventRaised = true;
    return;
  };
  transformer.get_transformedCollection().add_CollectionChanged(transformedCollChanged);
  rangeToRemove.clear();
  rangeToRemove.add(12);
  rangeToRemove.add(22);
  removeIndex = 4;
  expectedIndex = 5;
  itemsAdded = 1;
  itemsRemoved = 5;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  rangeToRemove.clear();
  rangeToRemove.add(33);
  rangeToRemove.add(34);
  removeIndex = 5;
  expectedIndex = 7;
  itemsAdded = 0;
  itemsRemoved = 2;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  rangeToRemove.clear();
  rangeToRemove.add(36);
  rangeToRemove.add(37);
  removeIndex = 6;
  expectedIndex = 8;
  itemsAdded = 0;
  itemsRemoved = 2;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  rangeToRemove.clear();
  rangeToRemove.add(39);
  rangeToRemove.add(43);
  removeIndex = 7;
  expectedIndex = 9;
  itemsAdded = 1;
  itemsRemoved = 4;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  rangeToRemove.clear();
  rangeToRemove.add(38);
  rangeToRemove.add(60);
  removeIndex = 6;
  expectedIndex = 8;
  itemsAdded = 1;
  itemsRemoved = 3;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  rangeToRemove.clear();
  rangeToRemove.add(61);
  rangeToRemove.add(61);
  removeIndex = 6;
  expectedIndex = 8;
  itemsAdded = 1;
  itemsRemoved = 3;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  rangeToRemove.clear();
  rangeToRemove.add(64);
  rangeToRemove.add(65);
  removeIndex = 6;
  expectedIndex = 8;
  itemsAdded = 1;
  itemsRemoved = 4;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  rangeToRemove.clear();
  rangeToRemove.add(32);
  rangeToRemove.add(35);
  rangeToRemove.add(76);
  rangeToRemove.add(77);
  rangeToRemove.add(82);
  removeIndex = 4;
  expectedIndex = 5;
  itemsAdded = 1;
  itemsRemoved = 9;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  rangeToRemove.clear();
  rangeToRemove.add(98);
  rangeToRemove.add(99);
  removeIndex = 7;
  expectedIndex = 9;
  itemsAdded = 0;
  itemsRemoved = 2;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  rangeToRemove.clear();
  rangeToRemove.add(91);
  rangeToRemove.add(92);
  removeIndex = 4;
  expectedIndex = 5;
  itemsAdded = 1;
  itemsRemoved = 3;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  rangeToRemove.clear();
  rangeToRemove.add(97);
  removeIndex = 4;
  expectedIndex = 5;
  itemsAdded = 0;
  itemsRemoved = 2;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  rangeToRemove.clear();
  rangeToRemove.add(1);
  rangeToRemove.add(2);
  rangeToRemove.add(3);
  rangeToRemove.add(5);
  removeIndex = 0;
  expectedIndex = 0;
  itemsAdded = 0;
  itemsRemoved = 5;
  transformer.get_inputCollection().removeRangeAt(removeIndex, rangeToRemove.get_count());
  transformer.get_transformedCollection().remove_CollectionChanged(transformedCollChanged);
  assert.ok(eventRaised, "Change event raised");
};
function TestInjectableTransformation__TestRegressionReplace(assert) {
  var collection, transformer;
  collection = ObservableCollection_$Number$_.defaultConstructor();
  collection.add(11);
  collection.add(15);
  collection.add(96);
  collection.add(97);
  transformer = HeaderInjectableTransformer_$Number_x_Number$_.__ctor(TestInjectableTransformation__GenerateHeader, collection);
  assert.equal(transformer.get_transformedCollection().get_count(), 6, "total output items");
  collection.set_item(0, 12);
  collection.set_item(1, 16);
  assert.equal(transformer.get_transformedCollection().get_count(), 6, "total output items");
};
function TestInjectableTransformation__TestHeaderInjectionReplace(assert) {
  var collection, newItemCount, itemsRemoved, itemsReplaced, insertIndex, expectedIndex, listToReplace, transformer, transformedCollChanged;
  assert.expect(38);
  collection = ObservableCollection_$Number$_.defaultConstructor();
  collection.add(11);
  collection.add(12);
  collection.add(13);
  collection.add(14);
  collection.add(96);
  collection.add(97);
  collection.add(98);
  collection.add(99);
  newItemCount = 0;
  itemsRemoved = 0;
  itemsReplaced = 0;
  insertIndex = 0;
  expectedIndex = 0;
  listToReplace = List_$Number$_.defaultConstructor();
  transformer = HeaderInjectableTransformer_$Number_x_Number$_.__ctor(TestInjectableTransformation__GenerateHeader, collection);
  transformedCollChanged = function TestInjectableTransformation__TestHeaderInjectionReplace_del(coll, evtArg) {
    if (evtArg.get_action() == 0) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_newItems()), "----------------------- " + " -- " + listToReplace.get_item(0) + "----- Item Added");
      assert.equal(evtArg.get_newItems().V_get_Count_c(), newItemCount, "Number of Items Added");
      assert.equal(expectedIndex, evtArg.get_changeIndex(), "Index changed");
    }
    else if (evtArg.get_action() == 1) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_oldItems()), "----------------------- " + " -- " + listToReplace.get_item(0) + "----- Item Removed");
      assert.equal(evtArg.get_oldItems().V_get_Count_c(), itemsRemoved, "Number of Items Removed");
      assert.equal(evtArg.get_changeIndex(), expectedIndex, "Index changed");
    }
    else if (evtArg.get_action() == 2) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_oldItems()) && !Object__IsNullOrUndefined(evtArg.get_newItems()), "----------------------- " + " -- " + listToReplace.get_item(0) + "----- Item Replaced");
      assert.equal(evtArg.get_oldItems().V_get_Count_c(), itemsReplaced, "Number of Items Removed");
      assert.equal(evtArg.get_newItems().V_get_Count_c(), itemsReplaced, "Number of Items Removed");
      assert.equal(evtArg.get_changeIndex(), expectedIndex, "Index changed");
    }
    return;
  };
  transformer.get_transformedCollection().add_CollectionChanged(transformedCollChanged);
  listToReplace.clear();
  listToReplace.add(17);
  insertIndex = 1;
  expectedIndex = 2;
  itemsReplaced = 1;
  transformer.get_inputCollection().replaceRangeAt(insertIndex, listToReplace);
  listToReplace.clear();
  listToReplace.add(92);
  listToReplace.add(93);
  insertIndex = 5;
  expectedIndex = 7;
  itemsReplaced = 2;
  transformer.get_inputCollection().replaceRangeAt(insertIndex, listToReplace);
  listToReplace.clear();
  listToReplace.add(94);
  insertIndex = 7;
  expectedIndex = 9;
  newItemCount = 1;
  itemsRemoved = 1;
  transformer.get_inputCollection().replaceRangeAt(insertIndex, listToReplace);
  listToReplace.clear();
  listToReplace.add(196);
  insertIndex = 4;
  expectedIndex = 5;
  newItemCount = 3;
  itemsRemoved = 2;
  transformer.get_inputCollection().replaceRangeAt(insertIndex, listToReplace);
  listToReplace.clear();
  listToReplace.add(111);
  insertIndex = 0;
  expectedIndex = 0;
  newItemCount = 3;
  itemsRemoved = 2;
  transformer.get_inputCollection().replaceRangeAt(insertIndex, listToReplace);
  listToReplace.clear();
  listToReplace.add(113);
  listToReplace.add(114);
  insertIndex = 2;
  expectedIndex = 4;
  newItemCount = 4;
  itemsRemoved = 3;
  transformer.get_inputCollection().replaceRangeAt(insertIndex, listToReplace);
  listToReplace.clear();
  listToReplace.add(194);
  insertIndex = 7;
  expectedIndex = 12;
  newItemCount = 2;
  itemsRemoved = 1;
  transformer.get_inputCollection().replaceRangeAt(insertIndex, listToReplace);
  transformer.get_transformedCollection().remove_CollectionChanged(transformedCollChanged);
};
function TestInjectableTransformation__TestHeaderInjectionReset(assert) {
  var resetCollection, expectedCounts, expectedIndexes, eventRaised, notificationCount, transformer, transformedCollChanged;
  assert.expect(63);
  resetCollection = ObservableCollection_$Number$_.defaultConstructor();
  resetCollection.add(11);
  resetCollection.add(12);
  expectedCounts = List_$Int32$_.defaultConstructor();
  expectedIndexes = List_$Int32$_.defaultConstructor();
  eventRaised = false;
  notificationCount = 0;
  transformer = HeaderInjectableTransformer_$Number_x_Number$_.__ctor(TestInjectableTransformation__GenerateHeader, null);
  transformedCollChanged = function TestInjectableTransformation__TestHeaderInjectionReset_del(coll, evtArg) {
    if (evtArg.get_action() == 0) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_newItems()), "------------------------" + Type__BoxTypeInstance(Int32, notificationCount) + " Add in Reset from " + resetCollection.get_item(0) + " - " + resetCollection.get_item(resetCollection.get_count() - 1));
      assert.equal(evtArg.get_newItems().V_get_Count_c(), expectedCounts.get_item(notificationCount), "Number of Items Added" + Type__BoxTypeInstance(Int32, evtArg.get_newItems().V_get_Count_c()));
      assert.equal(evtArg.get_changeIndex(), expectedIndexes.get_item(notificationCount), "Index changed" + Type__BoxTypeInstance(Int32, evtArg.get_changeIndex()));
    }
    else if (evtArg.get_action() == 1) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_oldItems()), "------------------------" + Type__BoxTypeInstance(Int32, notificationCount) + "Remove in Reset from " + resetCollection.get_item(0) + " - " + resetCollection.get_item(resetCollection.get_count() - 1));
      assert.equal(evtArg.get_oldItems().V_get_Count_c(), expectedCounts.get_item(notificationCount), "Number of Items Removed" + Type__BoxTypeInstance(Int32, evtArg.get_changeIndex()));
      assert.equal(evtArg.get_changeIndex(), expectedIndexes.get_item(notificationCount), "Index changed" + Type__BoxTypeInstance(Int32, evtArg.get_changeIndex()));
    }
    else if (evtArg.get_action() == 2) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_oldItems()) && !Object__IsNullOrUndefined(evtArg.get_newItems()), "------------------------" + Type__BoxTypeInstance(Int32, notificationCount) + "Remove in Reset from " + resetCollection.get_item(0) + " - " + resetCollection.get_item(resetCollection.get_count() - 1));
      assert.equal(evtArg.get_oldItems().V_get_Count_c(), expectedCounts.get_item(notificationCount), "Number of Items Removed" + Type__BoxTypeInstance(Int32, evtArg.get_oldItems().V_get_Count_c()));
      assert.equal(evtArg.get_newItems().V_get_Count_c(), expectedCounts.get_item(notificationCount), "Number of Items Added" + Type__BoxTypeInstance(Int32, evtArg.get_newItems().V_get_Count_c()));
      assert.equal(evtArg.get_changeIndex(), expectedIndexes.get_item(notificationCount), "Index changed" + Type__BoxTypeInstance(Int32, evtArg.get_newItems().V_get_Count_c()));
    }
    notificationCount++;
    eventRaised = true;
    return;
  };
  transformer.get_transformedCollection().add_CollectionChanged(transformedCollChanged);
  expectedCounts.insertRange(expectedCounts.get_count(), ArrayG_$Int32$_.__ctor([3]));
  expectedIndexes.insertRange(expectedIndexes.get_count(), ArrayG_$Int32$_.__ctor([0]));
  transformer.set_inputCollection(resetCollection.clone());
  assert.ok(notificationCount == expectedIndexes.get_count(), "no extra expected items" + Type__BoxTypeInstance(Int32, 1));
  resetCollection.clear();
  resetCollection.add(21);
  resetCollection.add(31);
  expectedCounts.insertRange(expectedCounts.get_count(), ArrayG_$Int32$_.__ctor([3, 4]));
  expectedIndexes.insertRange(expectedIndexes.get_count(), ArrayG_$Int32$_.__ctor([0, 0]));
  transformer.set_inputCollection(resetCollection.clone());
  assert.ok(notificationCount == expectedIndexes.get_count(), "no extra expected items" + Type__BoxTypeInstance(Int32, 2));
  expectedCounts.insertRange(expectedCounts.get_count(), ArrayG_$Int32$_.__ctor([4]));
  expectedIndexes.insertRange(expectedIndexes.get_count(), ArrayG_$Int32$_.__ctor([0]));
  transformer.set_inputCollection(null);
  transformer.set_inputCollection(null);
  assert.ok(notificationCount == expectedIndexes.get_count(), "no extra expected items" + Type__BoxTypeInstance(Int32, 3));
  resetCollection.clear();
  resetCollection.add(13);
  resetCollection.add(23);
  resetCollection.add(33);
  expectedCounts.insertRange(expectedCounts.get_count(), ArrayG_$Int32$_.__ctor([6]));
  expectedIndexes.insertRange(expectedIndexes.get_count(), ArrayG_$Int32$_.__ctor([0]));
  transformer.set_inputCollection(resetCollection.clone());
  assert.ok(notificationCount == expectedIndexes.get_count(), "no extra expected items" + Type__BoxTypeInstance(Int32, 4));
  resetCollection.clear();
  resetCollection.add(13);
  resetCollection.add(23);
  expectedCounts.insertRange(expectedCounts.get_count(), ArrayG_$Int32$_.__ctor([2, 4, 4]));
  expectedIndexes.insertRange(expectedIndexes.get_count(), ArrayG_$Int32$_.__ctor([4, 0, 0]));
  transformer.set_inputCollection(resetCollection.clone());
  assert.ok(notificationCount == expectedIndexes.get_count(), "no extra expected items" + Type__BoxTypeInstance(Int32, 5));
  resetCollection.clear();
  resetCollection.add(13);
  resetCollection.add(23);
  resetCollection.add(25);
  expectedCounts.insertRange(expectedCounts.get_count(), ArrayG_$Int32$_.__ctor([4, 4, 1]));
  expectedIndexes.insertRange(expectedIndexes.get_count(), ArrayG_$Int32$_.__ctor([0, 0, 4]));
  transformer.set_inputCollection(resetCollection.clone());
  assert.ok(notificationCount == expectedIndexes.get_count(), "no extra expected items" + Type__BoxTypeInstance(Int32, 6));
  resetCollection.clear();
  resetCollection.add(13);
  resetCollection.add(23);
  expectedCounts.insertRange(expectedCounts.get_count(), ArrayG_$Int32$_.__ctor([1, 4, 4]));
  expectedIndexes.insertRange(expectedIndexes.get_count(), ArrayG_$Int32$_.__ctor([4, 0, 0]));
  transformer.set_inputCollection(resetCollection.clone());
  assert.ok(notificationCount == expectedIndexes.get_count(), "no extra expected items" + Type__BoxTypeInstance(Int32, 7));
  resetCollection.clear();
  resetCollection.add(13);
  resetCollection.add(23);
  resetCollection.add(33);
  expectedCounts.insertRange(expectedCounts.get_count(), ArrayG_$Int32$_.__ctor([4, 5, 1, 2]));
  expectedIndexes.insertRange(expectedIndexes.get_count(), ArrayG_$Int32$_.__ctor([0, 0, 4, 4]));
  transformer.set_inputCollection(resetCollection.clone());
  assert.ok(notificationCount == expectedIndexes.get_count(), "no extra expected items" + Type__BoxTypeInstance(Int32, 8));
  assert.ok(eventRaised, "Change event raised");
};
function TestInjectableTransformation__GenerateHeader(first, second) {
  var firstHead, secondHead, rv;
  firstHead = first / 10 | 0;
  secondHead = second / 10 | 0;
  if (!first)
    return secondHead;
  if (firstHead == secondHead)
    return null;
  else
    rv = secondHead;
  return rv;
};
Type__RegisterReferenceType(TestInjectableTransformation, "Sunlight.Framework.Test.TestInjectableTransformation", Object, []);
function TestObservabelCollectionTransformer() {
};
TestObservabelCollectionTransformer.typeId = "t";
TestObservabelCollectionTransformer___listTransformer = null;
TestObservabelCollectionTransformer___notificationCount = 0;
TestObservabelCollectionTransformer___expectedCounts = null;
TestObservabelCollectionTransformer___expectedIndexes = null;
TestObservabelCollectionTransformer___list = null;
function TestObservabelCollectionTransformer__TestObservableCollectionTransformer(assert) {
  var transformedCollChanged, stmtTemp1, stmtTemp1a;
  TestObservabelCollectionTransformer___listTransformer = ObservableCollectionGenerator_$Number_x_String$_.__ctor(TestObservabelCollectionTransformer__ChangeDataType);
  transformedCollChanged = function TestObservabelCollectionTransformer__TestObservableCollectionTransformer_del(coll, evtArg) {
    var listBounds;
    listBounds = !!TestObservabelCollectionTransformer___list ? TestObservabelCollectionTransformer___list.get_item(0) + " - " + TestObservabelCollectionTransformer___list.get_item(TestObservabelCollectionTransformer___list.get_count() - 1) : "null";
    if (evtArg.get_action() == 0) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_newItems()), "------------------------" + Type__BoxTypeInstance(Int32, TestObservabelCollectionTransformer___notificationCount) + " Add in Reset from " + listBounds);
      assert.equal(evtArg.get_newItems().V_get_Count_c(), TestObservabelCollectionTransformer___expectedCounts.get_item(TestObservabelCollectionTransformer___notificationCount), "Number of Items Added" + Type__BoxTypeInstance(Int32, evtArg.get_newItems().V_get_Count_c()));
      assert.equal(evtArg.get_changeIndex(), TestObservabelCollectionTransformer___expectedIndexes.get_item(TestObservabelCollectionTransformer___notificationCount), "Index changed" + Type__BoxTypeInstance(Int32, evtArg.get_changeIndex()));
    }
    else if (evtArg.get_action() == 1) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_oldItems()), "------------------------" + Type__BoxTypeInstance(Int32, TestObservabelCollectionTransformer___notificationCount) + "Remove in Reset from " + listBounds);
      assert.equal(evtArg.get_oldItems().V_get_Count_c(), TestObservabelCollectionTransformer___expectedCounts.get_item(TestObservabelCollectionTransformer___notificationCount), "Number of Items Removed" + Type__BoxTypeInstance(Int32, evtArg.get_changeIndex()));
      assert.equal(evtArg.get_changeIndex(), TestObservabelCollectionTransformer___expectedIndexes.get_item(TestObservabelCollectionTransformer___notificationCount), "Index changed" + Type__BoxTypeInstance(Int32, evtArg.get_changeIndex()));
    }
    else if (evtArg.get_action() == 2) {
      assert.ok(!Object__IsNullOrUndefined(evtArg.get_oldItems()) && !Object__IsNullOrUndefined(evtArg.get_newItems()), "------------------------" + Type__BoxTypeInstance(Int32, TestObservabelCollectionTransformer___notificationCount) + "Replace in Reset from " + listBounds);
      assert.equal(evtArg.get_newItems().V_get_Count_c(), TestObservabelCollectionTransformer___expectedCounts.get_item(TestObservabelCollectionTransformer___notificationCount), "Number of Items Added in replace" + Type__BoxTypeInstance(Int32, evtArg.get_newItems().V_get_Count_c()));
      assert.equal(evtArg.get_changeIndex(), TestObservabelCollectionTransformer___expectedIndexes.get_item(TestObservabelCollectionTransformer___notificationCount), "Index changed" + Type__BoxTypeInstance(Int32, evtArg.get_newItems().V_get_Count_c()));
    }
    TestObservabelCollectionTransformer___notificationCount++;
    return;
  };
  TestObservabelCollectionTransformer___listTransformer.get_outputCollection().add_CollectionChanged(transformedCollChanged);
  TestObservabelCollectionTransformer__TestWithNewList(assert, List_$Number$_.__ctor([11, 12]), List_$Int32$_.__ctor([2]), List_$Int32$_.__ctor([0]));
  TestObservabelCollectionTransformer__TestWithNewList(assert, List_$Number$_.__ctor([21, 31]), List_$Int32$_.__ctor([2]), List_$Int32$_.__ctor([0]));
  TestObservabelCollectionTransformer__TestWithNewList(assert, null, List_$Int32$_.__ctor([2]), List_$Int32$_.__ctor([0]));
  TestObservabelCollectionTransformer__TestWithNewList(assert, List_$Number$_.__ctor([13, 23, 33]), List_$Int32$_.__ctor([3]), List_$Int32$_.__ctor([0]));
  TestObservabelCollectionTransformer__TestWithNewList(assert, List_$Number$_.__ctor([13, 23]), List_$Int32$_.__ctor([1]), List_$Int32$_.__ctor([2]));
  TestObservabelCollectionTransformer__TestWithNewList(assert, List_$Number$_.__ctor([13, 23, 25]), List_$Int32$_.__ctor([1]), List_$Int32$_.__ctor([2]));
  TestObservabelCollectionTransformer__TestWithNewList(assert, List_$Number$_.__ctor([13, 23]), List_$Int32$_.__ctor([1]), List_$Int32$_.__ctor([2]));
  TestObservabelCollectionTransformer__TestWithNewList(assert, List_$Number$_.__ctor([13, 23, 33]), List_$Int32$_.__ctor([1]), List_$Int32$_.__ctor([2]));
  TestObservabelCollectionTransformer__TestWithNewList(assert, List_$Number$_.__ctor([13, 23, 33]), (stmtTemp1 = List_$Int32$_.defaultConstructor(), stmtTemp1), (stmtTemp1a = List_$Int32$_.defaultConstructor(), stmtTemp1a));
  TestObservabelCollectionTransformer__TestWithNewList(assert, List_$Number$_.__ctor([12, 23, 33]), List_$Int32$_.__ctor([1]), List_$Int32$_.__ctor([0]));
  return;
};
function TestObservabelCollectionTransformer__TestWithNewList(assert, list, expectedCount, expectedIndex) {
  var length, lastIndex;
  TestObservabelCollectionTransformer___list = list;
  length = TestObservabelCollectionTransformer___expectedCounts.get_count();
  lastIndex = Math.max(length, 0);
  TestObservabelCollectionTransformer___expectedCounts.insertRangeb(lastIndex, expectedCount);
  TestObservabelCollectionTransformer___expectedIndexes.insertRangeb(lastIndex, expectedIndex);
  TestObservabelCollectionTransformer___listTransformer.set_inputCollection(list);
};
function TestObservabelCollectionTransformer__ChangeDataType(first) {
  return "This is number : " + first.toString();
};
function TestObservabelCollectionTransformer____cctor() {
  TestObservabelCollectionTransformer___notificationCount = 0;
  TestObservabelCollectionTransformer___expectedCounts = List_$Int32$_.defaultConstructor();
  TestObservabelCollectionTransformer___expectedIndexes = List_$Int32$_.defaultConstructor();
  TestObservabelCollectionTransformer___list = List_$Number$_.defaultConstructor();
};
Type__RegisterReferenceType(TestObservabelCollectionTransformer, "Sunlight.Framework.Test.TestObservabelCollectionTransformer", Object, []);
function DataBinderTests() {
};
DataBinderTests.typeId = "u";
function DataBinderTests__BasicBinderSimpleObjectTest(assert) {
  var dataBinder, target, source;
  dataBinder = DataBinder_factory(SourcePropertyBinder_factory(ArrayG_$String$_.__ctor(["IntProp"]), ArrayG_$Func_$Object_x_Object$_$_.__ctor([function DataBinderTests__BasicBinderSimpleObjectTest_del(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(SimpleObjectWithProperty, obj).get_intProp());
  }]), null), TargetBinder_factory("IntProp", null, function DataBinderTests__BasicBinderSimpleObjectTest_del2(obj, value) {
    Type__CastType(SimpleNotifiableClass, obj).set_intProp(Type__UnBoxTypeInstance(Int32, value));
    return;
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
    return;
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
    return;
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
    return;
  }), TargetBinder_factory("IntProp", function DataBinderTests__BasicBinderTwoWayNotifiableObjectTest_del3(obj) {
    return Type__BoxTypeInstance(Int32, Type__CastType(SimpleNotifiableClass, obj).get_intProp());
  }, function DataBinderTests__BasicBinderTwoWayNotifiableObjectTest_del4(obj, value) {
    Type__CastType(SimpleNotifiableClass, obj).set_intProp(Type__UnBoxTypeInstance(Int32, value));
    return;
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
SourcePropertyBinderTests.typeId = "v";
function SourcePropertyBinderTests__PrepNotifiableObject() {
  var rv, stmtTemp1, stmtTemp1a, stmtTemp1b;
  rv = (stmtTemp1 = SimpleNotifiableClass_factory(), stmtTemp1.set_intProp(10), stmtTemp1.set_strProp("Ten"), stmtTemp1.set_objProp((stmtTemp1b = SimpleObjectWithProperty_factory(), stmtTemp1b.set_intProp(11), stmtTemp1b.set_stringProp("Eleven"), stmtTemp1b)), stmtTemp1);
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
function ValueType() {
};
ValueType.typeId = "w";
ptyp_ = ValueType.prototype;
ptyp_.boxedValue = null;
Type__RegisterReferenceType(ValueType, "System.ValueType", Object, []);
function Int32(boxedValue) {
  this.boxedValue = boxedValue;
};
Int32.typeId = "x";
Int32.getDefaultValue = function() {
  return 0;
};
function Int32__ToString(this_, radix) {
  return this_.toString(radix);
};
function Int32__ToStringa(this_) {
  return Int32__ToString(this_, 10);
};
ptyp_ = new ValueType();
Int32.prototype = ptyp_;
ptyp_.toString = function() {
  return Int32__ToStringa(this.boxedValue);
};
Type__RegisterStructType(Int32, "System.Int32", []);
Number.typeId = "y";
Type__RegisterReferenceType(Number, "System.Number", Object, []);
function IocContainer() {
};
IocContainer.typeId = "z";
function IocContainer_factory() {
  var this_;
  this_ = new IocContainer();
  this_.__ctor();
  return this_;
};
IocContainer.defaultConstructor = IocContainer_factory;
ptyp_ = IocContainer.prototype;
ptyp_.factoryMap = null;
ptyp_.asyncFactoryMap = null;
ptyp_.resolutionCount = 0;
ptyp_.register = function IocContainer__Register(T, factory) {
  var this_, typeRegistry;
  this_ = this;
  typeRegistry = TypeRegistry_factory(factory);
  this_.factoryMap.set_item(T.typeId, typeRegistry);
  return IocHelper_factory(function IocContainer__Register_del() {
    typeRegistry.set_isStatic(true);
    return;
  }, function IocContainer__Register_del2(type) {
    this_.factoryMap.set_item(type.typeId, typeRegistry);
    return;
  });
};
ptyp_.resolve = function IocContainer__Resolve(T) {
  var rv;
  rv = this.tryResolve(T);
  if (rv == null)
    throw new Error("type not registreed");
  return rv;
};
ptyp_.tryResolve = function IocContainer__TryResolve(T) {
  var typeRegistry;
  if (this.resolutionCount > 100)
    throw new Error("Ioc has cycles, use ResolveLazy<T> to break cycle");
  this.resolutionCount++;
  try {
    typeRegistry = null;
    if (this.factoryMap.tryGetValue(T.typeId, {
      read: function() {
        return typeRegistry;
      },
      write: function(arg0) {
        return typeRegistry = arg0;
      }
    }))
      return typeRegistry.getValue(this);
    return null;
  } finally {
    this.resolutionCount--;
  }
};
ptyp_.resolveLazy = function IocContainer__ResolveLazy(T) {
  var this_, Lazy_$T$_;
  Lazy_$T$_ = Lazy(T, true);
  this_ = this;
  return Lazy_$T$_.__ctor(function IocContainer__ResolveLazy_del() {
    return this_.factoryMap.get_item(T.typeId).getValue(this_);
  });
};
ptyp_.__ctor = function IocContainer____ctor() {
  this.factoryMap = StringDictionary_$TypeRegistry$_.defaultConstructor();
  this.asyncFactoryMap = StringDictionary_$TypeRegistry$_.defaultConstructor();
  this.resolutionCount = 0;
};
Type__RegisterReferenceType(IocContainer, "Sunlight.Framework.IocContainer", Object, []);
function IocTestType2() {
};
IocTestType2.typeId = "ab";
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
IocTestType1Base.typeId = "bb";
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
IocTestType1.typeId = "cb";
function IocTestType1_factory(x, y) {
  var this_;
  this_ = new IocTestType1();
  this_.__ctora(x, y);
  return this_;
};
ptyp_ = new IocTestType1Base();
IocTestType1.prototype = ptyp_;
ptyp_.y = 0;
ptyp_.__ctora = function IocTestType1____ctor(x, y) {
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
IocHelper.typeId = "db";
function IocHelper_factory(isSingleton, registerAs) {
  var this_;
  this_ = new IocHelper();
  this_.__ctor(isSingleton, registerAs);
  return this_;
};
ptyp_ = IocHelper.prototype;
ptyp_.isSingletona = null;
ptyp_.registerAs = null;
ptyp_.__ctor = function IocHelper____ctor(isSingleton, registerAs) { {
    this.registerAs = registerAs;
    this.isSingletona = isSingleton;
  }
};
ptyp_.as = function IocHelper__As(T) {
  this.registerAs(T);
  return this;
};
ptyp_.isSingleton = function IocHelper__IsSingleton() {
  this.isSingletona();
  return this;
};
Type__RegisterReferenceType(IocHelper, "Sunlight.Framework.IoC.IocHelper", Object, []);
function EventBus() {
};
EventBus.typeId = "eb";
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
  }))
    this.eventSubscriptsion.set_item(typeId, callback);
  else
    this.eventSubscriptsion.set_item(typeId, Delegate__Combine(registeredCallback, callback));
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
    else
      this.eventSubscriptsion.set_item(typeId, act);
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
  } while (!!type);
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
Evt1.typeId = "fb";
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
Evt2.typeId = "gb";
Evt2.getDefaultValue = function() {
  return {
    x: 0
  };
};
Evt2.prototype = new ValueType();
Type__RegisterStructType(Evt2, "Sunlight.Framework.Test.EventBusTests/Evt2", []);
function INotifyPropertyChanged() {
};
INotifyPropertyChanged.typeId = "f";
Type__RegisterInterface(INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function ObservableObject() {
};
ObservableObject.typeId = "hb";
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
ptyp_.V_AddPropertyChangedListener_f = ptyp_.addPropertyChangedListener;
ptyp_.V_RemovePropertyChangedListener_f = ptyp_.removePropertyChangedListener;
Type__RegisterReferenceType(ObservableObject, "Sunlight.Framework.Observables.ObservableObject", Object, [INotifyPropertyChanged]);
function IEnumerable() {
};
IEnumerable.typeId = "d";
Type__RegisterInterface(IEnumerable, "System.Collections.IEnumerable");
function ICollection() {
};
ICollection.typeId = "c";
Type__RegisterInterface(ICollection, "System.Collections.ICollection");
function ObservableTestObject() {
};
ObservableTestObject.typeId = "ib";
function ObservableTestObject_factory() {
  var this_;
  this_ = new ObservableTestObject();
  this_.__ctora();
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
ptyp_.__ctora = function ObservableTestObject____ctor() {
  this.__ctor();
};
Type__RegisterReferenceType(ObservableTestObject, "Sunlight.Framework.Test.ObservableObjectTests/ObservableTestObject", ObservableObject, []);
function ArrayImpl() {
};
ArrayImpl.typeId = "jb";
ptyp_ = ArrayImpl.prototype;
ptyp_.__ctor = function ArrayImpl____ctor() {
};
ptyp_.system__Collections__ICollection__get_Count = function ArrayImpl__System__Collections__ICollection__get_Count() {
  return this.V_get_Length();
};
ptyp_.V_get_Count_c = ptyp_.system__Collections__ICollection__get_Count;
ptyp_.V_CopyTo_c = function(arg0, arg1) {
  return this.V_CopyTo(arg0, arg1);
};
ptyp_.V_GetEnumerator_d = function() {
  return this.V_GetEnumerator();
};
Type__RegisterReferenceType(ArrayImpl, "System.ArrayImpl", Object, [ICollection, IEnumerable]);
String.typeId = "k";
Type__RegisterReferenceType(String, "System.String", Object, []);
function ISourceDataBinder() {
};
ISourceDataBinder.typeId = "h";
Type__RegisterInterface(ISourceDataBinder, "Sunlight.Framework.Binders.ISourceDataBinder");
function ITargetDataBinder() {
};
ITargetDataBinder.typeId = "i";
Type__RegisterInterface(ITargetDataBinder, "Sunlight.Framework.Binders.ITargetDataBinder");
function DataBinder() {
};
DataBinder.typeId = "kb";
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
  this.firstBindingSuccessful = false; {
    this.sourceBinder = sourceBinder;
    this.targetBinder = targetBinder;
    this.bindingMode = dataBindingMode;
    this.converter = converter;
    this.targetBinder.useDataBinder(this);
    this.sourceBinder.useDataBinder(this);
  }
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
          this.targetBinder.set_value(!this.converter ? this.sourceBinder.get_value() : this.converter.V_Convert_g(this.sourceBinder.get_value()));
          this.firstBindingSuccessful = true;
        }
        else
          this.targetBinder.setDefault();
      return;
    }
    if (this.sourceBinder.get_isActive()) {
      if (!!this.converter)
        this.targetBinder.set_value(this.converter.V_Convert_g(this.sourceBinder.get_value()));
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
ptyp_.V_SourceValueUpdated_h = ptyp_.sourceValueUpdated;
ptyp_.V_TargetValueUpdated_i = ptyp_.targetValueUpdated;
Type__RegisterReferenceType(DataBinder, "Sunlight.Framework.Binders.DataBinder", Object, [ISourceDataBinder, ITargetDataBinder]);
function SourcePropertyBinder() {
};
SourcePropertyBinder.typeId = "lb";
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
  var i; {
    this.propertyPartNames = propertyPartNames;
    this.chainLength = this.propertyPartNames.V_get_Length();
    this.propertyGetterChain = propertyGetterChain;
    this.propertySetter = propertySetter;
    this.objectChain = ArrayG_$Object$_.__ctora(this.chainLength);
    this.changeRegistrations = ArrayG_$Action_$INotifyPropertyChanged_x_String$_$_.__ctora(this.chainLength);
    for (i = this.chainLength - 1; i >= 0; i--)
      this.changeRegistrations.set_item(i, this.getChangeTrackerAt(i));
  }
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
  if (this.isActive && !!this.propertySetter)
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
    if (this.objectChain.get_item(i) == null) {
      for (j = this.chainLength - 1; j >= i; j--)
        this.setObjectChainElementValue(j, null);
      if (this.value != null || this.isActive) {
        this.isActive = false;
        this.value = null;
        if (!!this.dataBinderBase)
          this.dataBinderBase.V_SourceValueUpdated_h();
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
        if (!!this.dataBinderBase)
          this.dataBinderBase.V_SourceValueUpdated_h();
      }
    }
};
ptyp_.setObjectChainElementValue = function SourcePropertyBinder__SetObjectChainElementValue(index, value) {
  var oldValue, newNotifiableValue;
  if (index > this.chainLength)
    return;
  oldValue = Type__AsType(INotifyPropertyChanged, this.objectChain.get_item(index));
  if (!!oldValue)
    oldValue.V_RemovePropertyChangedListener_f(this.propertyPartNames.get_item(index), this.changeRegistrations.get_item(index));
  this.objectChain.set_item(index, value);
  newNotifiableValue = Type__AsType(INotifyPropertyChanged, value);
  if (!!newNotifiableValue)
    newNotifiableValue.V_AddPropertyChangedListener_f(this.propertyPartNames.get_item(index), this.changeRegistrations.get_item(index));
};
ptyp_.getChangeTrackerAt = function SourcePropertyBinder__GetChangeTrackerAt(index) {
  var this_;
  this_ = this;
  return function SourcePropertyBinder__GetChangeTrackerAt_del(sender, prop) {
    this_.calculateValueFrom(index);
    return;
  };
};
Type__RegisterReferenceType(SourcePropertyBinder, "Sunlight.Framework.Binders.SourcePropertyBinder", Object, []);
function SimpleObjectWithProperty() {
};
SimpleObjectWithProperty.typeId = "mb";
function SimpleObjectWithProperty_factory() {
  return new SimpleObjectWithProperty();
};
SimpleObjectWithProperty.defaultConstructor = SimpleObjectWithProperty_factory;
ptyp_ = SimpleObjectWithProperty.prototype;
ptyp_._$StringProp$_k__BackingField = null;
ptyp_._$IntProp$_k__BackingField = 0;
ptyp_._$SelfProp$_k__BackingField = null;
ptyp_._$NotifiableProp$_k__BackingField = null;
ptyp_.set_stringProp = function SimpleObjectWithProperty__set_StringProp(value) {
  this._$StringProp$_k__BackingField = value;
};
ptyp_.get_intProp = function SimpleObjectWithProperty__get_IntProp() {
  return this._$IntProp$_k__BackingField;
};
ptyp_.set_intProp = function SimpleObjectWithProperty__set_IntProp(value) {
  this._$IntProp$_k__BackingField = value;
};
ptyp_.get_selfProp = function SimpleObjectWithProperty__get_SelfProp() {
  return this._$SelfProp$_k__BackingField;
};
ptyp_.set_selfProp = function SimpleObjectWithProperty__set_SelfProp(value) {
  this._$SelfProp$_k__BackingField = value;
};
ptyp_.get_notifiableProp = function SimpleObjectWithProperty__get_NotifiableProp() {
  return this._$NotifiableProp$_k__BackingField;
};
ptyp_.set_notifiableProp = function SimpleObjectWithProperty__set_NotifiableProp(value) {
  this._$NotifiableProp$_k__BackingField = value;
};
ptyp_.__ctor = function SimpleObjectWithProperty____ctor() {
};
Type__RegisterReferenceType(SimpleObjectWithProperty, "Sunlight.Framework.Test.Binders.SimpleObjectWithProperty", Object, []);
function TargetBinder() {
};
TargetBinder.typeId = "nb";
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
ptyp_.__ctor = function TargetBinder____ctor(propertyName, getter, setter, defaultValue) { {
    this.propertyName = propertyName;
    this.getter = getter;
    this.setter = setter;
    this.defaultValue = defaultValue;
  }
};
ptyp_.get_value = function TargetBinder__get_Value() {
  return this.value;
};
ptyp_.set_value = function TargetBinder__set_Value(value) {
  if (this.value !== value && !!this.setter) {
    this.value = value;
    if (this.get_isActive())
      this.setter(this.target, this.value);
  }
};
ptyp_.get_isWriteOnly = function TargetBinder__get_IsWriteOnly() {
  return !this.getter;
};
ptyp_.get_isActive = function TargetBinder__get_IsActive() {
  return !!this.target;
};
ptyp_.set_target = function TargetBinder__set_Target(value) {
  if (this.target != value) {
    if (!!this.target)
      this.target.V_RemovePropertyChangedListener_f(this.propertyName, Delegate__Create("onTargetUpdated", this));
    this.target = value;
    this.value = null;
    if (!!this.target) {
      this.target.V_AddPropertyChangedListener_f(this.propertyName, Delegate__Create("onTargetUpdated", this));
      if (!!this.getter)
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
  if (!!this.dataBinder && !!this.getter) {
    this.value = this.getter(this.target);
    this.dataBinder.V_TargetValueUpdated_i();
  }
};
Type__RegisterReferenceType(TargetBinder, "Sunlight.Framework.Binders.TargetBinder", Object, []);
function SimpleNotifiableClass() {
};
SimpleNotifiableClass.typeId = "ob";
function SimpleNotifiableClass_factory() {
  var this_;
  this_ = new SimpleNotifiableClass();
  this_.__ctora();
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
ptyp_.__ctora = function SimpleNotifiableClass____ctor() {
  this.__ctor();
};
Type__RegisterReferenceType(SimpleNotifiableClass, "Sunlight.Framework.Test.Binders.SimpleNotifiableClass", ObservableObject, []);
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
Functiona.typeId = "pb";
Functiona.prototype = new Function();
Type__RegisterReferenceType(Functiona, "System.MulticastDelegate", Function, []);
function BinderTestHelper() {
};
BinderTestHelper.typeId = "qb";
function BinderTestHelper_factory() {
  return new BinderTestHelper();
};
BinderTestHelper.defaultConstructor = BinderTestHelper_factory;
ptyp_ = BinderTestHelper.prototype;
ptyp_._$SourceValueUpdateCalled$_k__BackingField = false;
ptyp_.get_sourceValueUpdateCalled = function BinderTestHelper__get_SourceValueUpdateCalled() {
  return this._$SourceValueUpdateCalled$_k__BackingField;
};
ptyp_.set_sourceValueUpdateCalled = function BinderTestHelper__set_SourceValueUpdateCalled(value) {
  this._$SourceValueUpdateCalled$_k__BackingField = value;
};
ptyp_.sourceValueUpdated = function BinderTestHelper__SourceValueUpdated() {
  this.set_sourceValueUpdateCalled(true);
};
ptyp_.__ctor = function BinderTestHelper____ctor() {
};
ptyp_.V_SourceValueUpdated_h = ptyp_.sourceValueUpdated;
Type__RegisterReferenceType(BinderTestHelper, "Sunlight.Framework.Test.Binders.SourcePropertyBinderTests/BinderTestHelper", Object, [ISourceDataBinder]);
function TypeRegistry() {
};
TypeRegistry.typeId = "rb";
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
ptyp_.getValue = function TypeRegistry__GetValue(ioc) {
  if (!this.isStatic)
    return this.factory(ioc);
  if (!this.isCreated) {
    this.value = this.factory(ioc);
    this.isCreated = true;
  }
  return this.value;
};
Type__RegisterReferenceType(TypeRegistry, "Sunlight.Framework.TypeRegistry", Object, []);
Error.typeId = "sb";
Type__RegisterReferenceType(Error, "System.Exception", Object, []);
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
Function.getDefaultValue = function() {
  return {
  };
};
function IEnumerator() {
};
IEnumerator.typeId = "e";
Type__RegisterInterface(IEnumerator, "System.Collections.IEnumerator");
function IValueConverter() {
};
IValueConverter.typeId = "g";
Type__RegisterInterface(IValueConverter, "Sunlight.Framework.Binders.IValueConverter");
Array.typeId = "lc";
Type__RegisterReferenceType(Array, "System.Array", Object, [ICollection, IEnumerable]);
function List(T, _callStatiConstructor) {
  var ArrayG$1_$T$_, T$5b$5d_$T$_, List$1_$T$_, ListEnumerator$1_$T$_, IList$1_$T$_, IEnumerable$1_$T$_, __initTracker, __initTrackera;
  if (List[T.typeId])
    return List[T.typeId];
  List[T.typeId] = function System__Collections__Generic__List$1() {
  };
  List$1_$T$_ = List[T.typeId];
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = List;
  List$1_$T$_.typeId = "tb$" + T.typeId + "$";
  IList$1_$T$_ = IList(T, _callStatiConstructor);
  IEnumerable$1_$T$_ = IEnumerablea(T, _callStatiConstructor);
  List$1_$T$_.defaultConstructor = function System_Collections_Generic_List$1_factory() {
    var this_;
    this_ = new List$1_$T$_();
    this_.__ctor();
    return this_;
  };
  List$1_$T$_.__ctor = function System_Collections_Generic_List$1_factory(nativeArray) {
    var this_;
    this_ = new List$1_$T$_();
    this_.__ctora(nativeArray);
    return this_;
  };
  ptyp_ = List$1_$T$_.prototype;
  ptyp_.nativeArray = null;
  ptyp_.__ctor = function List$1____ctor() {
    this.nativeArray = new Array(0);
  };
  ptyp_.__ctora = function List$1____ctor(nativeArray) {
    this.nativeArray = nativeArray;
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
  ptyp_.insertRange = function List$1__InsertRange(index, items) {
    NativeArray$1__InsertRangeAt(this.nativeArray, index, NativeArray$1__GetNativeArray(items));
  };
  ptyp_.insertRangeb = function List$1__InsertRange(index, items) {
    NativeArray$1__InsertRangeAt(this.nativeArray, index, items.nativeArray);
  };
  ptyp_.insertRangea = function List$1__InsertRange(index, items) {
    var stmtTemp1, item;
    for (stmtTemp1 = items.V_GetEnumerator_d(); stmtTemp1.V_MoveNext_e(); ) {
      item = Type__UnBoxTypeInstance(T, stmtTemp1.V_get_Current_e());
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
    for (stmtTemp1 = items.V_GetEnumerator_d(); stmtTemp1.V_MoveNext_e(); ) {
      item = Type__UnBoxTypeInstance(T, stmtTemp1.V_get_Current_e());
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
  ptyp_.V_CopyTo_c = ptyp_.system__Collections__ICollection__CopyTo;
  ptyp_.V_GetEnumerator_d = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  ptyp_.V_get_Count_c = ptyp_.get_count;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.getEnumerator;
  Type__RegisterReferenceType(List$1_$T$_, "System.Collections.Generic.List`1<" + T.fullName + ">", Object, [IList$1_$T$_, ICollection, IEnumerable, IEnumerable$1_$T$_]);
  List$1_$T$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    T = T;
    ArrayG$1_$T$_ = ArrayG(T, true);
    T$5b$5d_$T$_ = ArrayG(T, true);
    List$1_$T$_ = List(T, true);
    ListEnumerator$1_$T$_ = ListEnumerator(T, true);
  };
  if (_callStatiConstructor)
    List$1_$T$_._tri();
  return List$1_$T$_;
};
function Lazy(T, _callStatiConstructor) {
  var Lazy$1_$T$_, __initTracker;
  if (Lazy[T.typeId])
    return Lazy[T.typeId];
  Lazy[T.typeId] = function Sunlight__Framework__Lazy$1() {
  };
  Lazy$1_$T$_ = Lazy[T.typeId];
  Lazy$1_$T$_.genericParameters = [T];
  Lazy$1_$T$_.genericClosure = Lazy;
  Lazy$1_$T$_.typeId = "ub$" + T.typeId + "$";
  Lazy$1_$T$_.__ctor = function Sunlight_Framework_Lazy$1_factory(factory) {
    var this_;
    this_ = new Lazy$1_$T$_();
    this_.__ctor(factory);
    return this_;
  };
  ptyp_ = Lazy$1_$T$_.prototype;
  ptyp_.factory = null;
  ptyp_.value = Type__GetDefaultValueStatic(T);
  ptyp_.createdCallbacks = null;
  ptyp_.__ctor = function Lazy$1____ctor(factory) { {
      ExceptionHelpers__ThrowOnArgumentNull(factory, "factory");
      this.factory = factory;
    }
  };
  ptyp_.get_value = function Lazy$1__get_Value() {
    if (!!this.factory) {
      this.value = this.factory();
      this.factory = null;
      if (!!this.createdCallbacks) {
        this.createdCallbacks();
        this.createdCallbacks = null;
      }
    }
    return this.value;
  };
  Type__RegisterReferenceType(Lazy$1_$T$_, "Sunlight.Framework.Lazy`1<" + T.fullName + ">", Object, []);
  Lazy$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    Lazy$1_$T$_ = Lazy(T, true);
  };
  if (_callStatiConstructor)
    Lazy$1_$T$_._tri();
  return Lazy$1_$T$_;
};
function ObservableCollection(T, _callStatiConstructor) {
  var List$1_$T$_, ArrayG$1_$T$_, IList$1_$T$_, ObservableCollection$1_$T$_, CollectionChangedEventArgs$1_$T$_, __initTracker, __initTrackera;
  if (ObservableCollection[T.typeId])
    return ObservableCollection[T.typeId];
  ObservableCollection[T.typeId] = function Sunlight__Framework__Observables__ObservableCollection$1() {
  };
  ObservableCollection$1_$T$_ = ObservableCollection[T.typeId];
  ObservableCollection$1_$T$_.genericParameters = [T];
  ObservableCollection$1_$T$_.genericClosure = ObservableCollection;
  ObservableCollection$1_$T$_.typeId = "vb$" + T.typeId + "$";
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
  ptyp_.set_item = function ObservableCollection$1__set_Item(index, value) {
    var oldItem;
    ExceptionHelpers__ThrowOnOutOfRange(index, 0, this.get_count() - 1, "index");
    if (!Object__Equals(this.items.get_item(index), value)) {
      oldItem = this.items.get_item(index);
      this.items.set_item(index, value);
      this.onCollectionChanged(2, index, ArrayG$1_$T$_.__ctor([value]), ArrayG$1_$T$_.__ctor([oldItem]));
    }
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
    this.onCollectionChanged(0, this.get_count() - objArray.V_get_Count_c(), objArray, null);
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
    ExceptionHelpers__ThrowOnOutOfRange(replaceIndex + list.V_get_Count_c() - 1, 0, this.get_count() - 1, "index");
    oldItems = List$1_$T$_.defaultConstructor();
    for (index = 0; index < list.V_get_Count_c(); index++) {
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
  ptyp_.clone = function ObservableCollection$1__Clone() {
    var clone, index;
    clone = ObservableCollection$1_$T$_.defaultConstructor();
    for (index = 0; index < this.get_count(); index++)
      clone.add(this.get_item(index));
    return clone;
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
  Type__RegisterReferenceType(ObservableCollection$1_$T$_, "Sunlight.Framework.Observables.ObservableCollection`1<" + T.fullName + ">", ObservableObject, [INotifyPropertyChanged]);
  ObservableCollection$1_$T$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    List$1_$T$_ = List(T, true);
    ArrayG$1_$T$_ = ArrayG(T, true);
    IList$1_$T$_ = IList(T, true);
    ObservableCollection$1_$T$_ = ObservableCollection(T, true);
    CollectionChangedEventArgs$1_$T$_ = CollectionChangedEventArgs(T, true);
    T = T;
  };
  if (_callStatiConstructor)
    ObservableCollection$1_$T$_._tri();
  return ObservableCollection$1_$T$_;
};
function CollectionChangedEventArgs(T, _callStatiConstructor) {
  var CollectionChangedEventArgs$1_$T$_, __initTracker;
  if (CollectionChangedEventArgs[T.typeId])
    return CollectionChangedEventArgs[T.typeId];
  CollectionChangedEventArgs[T.typeId] = function Sunlight__Framework__Observables__CollectionChangedEventArgs$1() {
  };
  CollectionChangedEventArgs$1_$T$_ = CollectionChangedEventArgs[T.typeId];
  CollectionChangedEventArgs$1_$T$_.genericParameters = [T];
  CollectionChangedEventArgs$1_$T$_.genericClosure = CollectionChangedEventArgs;
  CollectionChangedEventArgs$1_$T$_.typeId = "wb$" + T.typeId + "$";
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
  Type__RegisterReferenceType(CollectionChangedEventArgs$1_$T$_, "Sunlight.Framework.Observables.CollectionChangedEventArgs`1<" + T.fullName + ">", Object, []);
  CollectionChangedEventArgs$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    CollectionChangedEventArgs$1_$T$_ = CollectionChangedEventArgs(T, true);
  };
  if (_callStatiConstructor)
    CollectionChangedEventArgs$1_$T$_._tri();
  return CollectionChangedEventArgs$1_$T$_;
};
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
  HeaderInjectableTransformer$2_$T_x_H$_.typeId = "xb$" + T.typeId + "_" + H.typeId + "$";
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
  ptyp_.get_inputCollection = function HeaderInjectableTransformer$2__get_InputCollection() {
    return this._inputCollection;
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
      this.moveHeaderIndexes(boundaryHeaderIndex, boundaryElementIndex, changeList.V_get_Count_c());
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
    this._allHeaderIndexes.insertRangeb(boundaryHeaderIndex, headersToAdd);
    return;
  };
  ptyp_.removeElements = function HeaderInjectableTransformer$2__RemoveElements(changeList, changeIndex) {
    var leftNeighbourTuple, leftElementIndex, leftHeaderCount, rightNeighbourTuple, rightElementIndex, rightHeaderCount, boundaryElementIndex, boundaryHeaderIndex, boundaryElementCount, boundaryHeaderCount, wrappedList, addedBefore, addedAfter, showFirstHead, tupleReturn, elementsToAdd, headersToAdd;
    leftNeighbourTuple = this.getTransformedIndexes(changeIndex - 1);
    leftElementIndex = TransformedIndexTuple_$T_x_H$_.get_elementIndex(leftNeighbourTuple);
    leftHeaderCount = TransformedIndexTuple_$T_x_H$_.get_headerCount(leftNeighbourTuple);
    rightNeighbourTuple = this.getTransformedIndexes(changeIndex + changeList.V_get_Count_c());
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
      this._transformedCollection.removeRangeAt(boundaryElementIndex, changeList.V_get_Count_c());
      this.moveHeaderIndexes(boundaryHeaderIndex, boundaryElementIndex, -changeList.V_get_Count_c());
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
    rightNeighbourTuple = this.getTransformedIndexes(changeIndex + changeList.V_get_Count_c());
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
    this._allHeaderIndexes.insertRangeb(boundaryHeaderIndex, headersToAdd);
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
    endIndex = addChangeList ? changeIdx + changeList.V_get_Count_c() : changeIdx;
    addedAfter.write(endIndex < this._inputCollection.get_count());
    if (addedAfter.read())
      wrappedList.add(this._inputCollection.get_item(endIndex));
    return wrappedList;
  };
  ptyp_.getInsertedHeaderList = function HeaderInjectableTransformer$2__GetInsertedHeaderList(items, showFirstHead, skipCompare, addedBefore, addedAfter, insertionCount) {
    var rv, headerIdx, idx, item, header, headerEntry, eachItem;
    rv = List$1_$InjectedElement$2_$T_x_H$_$_.defaultConstructor();
    headerIdx = List_$Int32$_.defaultConstructor();
    for (idx = 0; idx < items.V_get_Count_c(); idx++) {
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
        if (!(addedAfter && idx == items.V_get_Count_c() - 1)) {
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
      this._allHeaderIndexes.insertRangeb(startHeaderIndex, headersToAdd);
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
    IList$1_$T$_ = IList(T, true);
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
  InjectedElement$2_$I_x_H$_.typeId = "yb$" + I.typeId + "_" + H.typeId + "$";
  InjectedElement$2_$I_x_H$_.defaultConstructor = function Sunlight_Framework_Observables_InjectedElement$2_factory() {
    return new InjectedElement$2_$I_x_H$_();
  };
  ptyp_ = InjectedElement$2_$I_x_H$_.prototype;
  ptyp_._header = Type__GetDefaultValueStatic(H);
  ptyp_._item = Type__GetDefaultValueStatic(I);
  ptyp_.set_header = function InjectedElement$2__set_Header(value) {
    this._header = value;
  };
  ptyp_.set_item = function InjectedElement$2__set_Item(value) {
    this._item = value;
  };
  ptyp_.__ctor = function InjectedElement$2____ctor() {
    this._header = Type__GetDefaultValueStatic(H);
    this._item = Type__GetDefaultValueStatic(I);
  };
  Type__RegisterReferenceType(InjectedElement$2_$I_x_H$_, "Sunlight.Framework.Observables.InjectedElement`2<" + I.fullName + "," + H.fullName + ">", Object, []);
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
function ArrayG(T, _callStatiConstructor) {
  var Enumerator_$T$_, ArrayG$1_$T$_, IList$1_$T$_, IEnumerable$1_$T$_, __initTracker, T$5b$5d_$T$_, __initTrackera;
  if (ArrayG[T.typeId])
    return ArrayG[T.typeId];
  ArrayG[T.typeId] = function System__ArrayG$1() {
  };
  ArrayG$1_$T$_ = ArrayG[T.typeId];
  ArrayG$1_$T$_.genericParameters = [T];
  ArrayG$1_$T$_.genericClosure = ArrayG;
  ArrayG$1_$T$_.typeId = "zb$" + T.typeId + "$";
  IList$1_$T$_ = IList(T, _callStatiConstructor);
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
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.system__Collections__Generic__IEnumerable_$T$___GetEnumerator;
  ptyp_.V_get_Length = ptyp_.get_length;
  ptyp_.V_GetEnumerator = ptyp_.getEnumerator;
  ptyp_.V_CopyTo = ptyp_.copyToa;
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  Type__RegisterReferenceType(ArrayG$1_$T$_, "System.ArrayG`1<" + T.fullName + ">", ArrayImpl, [IList$1_$T$_, ICollection, IEnumerable, IEnumerable$1_$T$_]);
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
  ObservableCollectionGenerator$2_$T_x_U$_.typeId = "ac$" + T.typeId + "_" + U.typeId + "$";
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
    for (stmtTemp1 = elements.V_GetEnumerator_d(); stmtTemp1.V_MoveNext_e(); ) {
      element = Type__UnBoxTypeInstance(T, stmtTemp1.V_get_Current_e());
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
  Func$2_$T1_x_TRes$_.typeId = "bc$" + T1.typeId + "_" + TRes.typeId + "$";
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
function NativeArray$1__GetNativeArray(array) {
  return array ? array.innerArray : null;
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
function StringDictionary(TValue, _callStatiConstructor) {
  var Enumerator_$TValue$_, StringDictionary$1_$TValue$_, KeyValuePair$2_$String_x_TValue$_, IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_, __initTracker, __initTrackera;
  if (StringDictionary[TValue.typeId])
    return StringDictionary[TValue.typeId];
  StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$1() {
  };
  StringDictionary$1_$TValue$_ = StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "cc$" + TValue.typeId + "$";
  KeyValuePair$2_$String_x_TValue$_ = KeyValuePair(String, TValue, _callStatiConstructor);
  KeyValuePair$2_$String_x_TValue$_ = KeyValuePair(String, TValue, _callStatiConstructor);
  IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_ = IEnumerablea(KeyValuePair(String, TValue, _callStatiConstructor), _callStatiConstructor);
  StringDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_StringDictionary$1_factory() {
    var this_;
    this_ = new StringDictionary$1_$TValue$_();
    this_.__ctor();
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
  ptyp_.V_GetEnumerator_d = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_.V_get_Count_c = ptyp_.get_count;
  ptyp_.V_CopyTo_c = ptyp_.copyTo;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_.typeId] = ptyp_.getEnumerator;
  Type__RegisterReferenceType(StringDictionary$1_$TValue$_, "System.Collections.Generic.StringDictionary`1<" + TValue.fullName + ">", Object, [ICollection, IEnumerable, IEnumerable$1_$KeyValuePair$2_$String_x_TValue$_$_]);
  StringDictionary$1_$TValue$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    TValue = TValue;
    Enumerator_$TValue$_ = Enumeratora(TValue, true);
    StringDictionary$1_$TValue$_ = StringDictionary(TValue, true);
  };
  if (_callStatiConstructor)
    StringDictionary$1_$TValue$_._tri();
  return StringDictionary$1_$TValue$_;
};
function IList(T, _callStatiConstructor) {
  var IList$1_$T$_, IEnumerable$1_$T$_, __initTracker;
  if (IList[T.typeId])
    return IList[T.typeId];
  IList[T.typeId] = function System__Collections__Generic__IList$1() {
  };
  IList$1_$T$_ = IList[T.typeId];
  IList$1_$T$_.genericParameters = [T];
  IList$1_$T$_.genericClosure = IList;
  IList$1_$T$_.typeId = "dc$" + T.typeId + "$";
  IEnumerable$1_$T$_ = IEnumerablea(T, _callStatiConstructor);
  Type__RegisterInterface(IList$1_$T$_, "System.Collections.Generic.IList`1<" + T.fullName + ">");
  IList$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T = T;
    IList$1_$T$_ = IList(T, true);
  };
  if (_callStatiConstructor)
    IList$1_$T$_._tri();
  return IList$1_$T$_;
};
function Action(T1, T2, _callStatiConstructor) {
  var Action$2_$T1_x_T2$_, __initTracker;
  if (Action[T1.typeId] && Action[T1.typeId][T2.typeId])
    return Action[T1.typeId][T2.typeId];
    Action[T1.typeId] = {
    };
  Action[T1.typeId][T2.typeId] = function System__Action$2() {
  };
  Action$2_$T1_x_T2$_ = Action[T1.typeId][T2.typeId];
  Action$2_$T1_x_T2$_.genericParameters = [T1, T2];
  Action$2_$T1_x_T2$_.genericClosure = Action;
  Action$2_$T1_x_T2$_.typeId = "ec$" + T1.typeId + "_" + T2.typeId + "$";
  Action$2_$T1_x_T2$_.prototype = new Functiona();
  Type__RegisterReferenceType(Action$2_$T1_x_T2$_, "System.Action`2<" + T1.fullName + "," + T2.fullName + ">", Functiona, []);
  Action$2_$T1_x_T2$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T1 = T1;
    T2 = T2;
    Action$2_$T1_x_T2$_ = Action(T1, T2, true);
  };
  if (_callStatiConstructor)
    Action$2_$T1_x_T2$_._tri();
  return Action$2_$T1_x_T2$_;
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
  TransformedIndexTuple_$T_x_H$_.typeId = "fc$" + T.typeId + "_" + H.typeId + "$";
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
  ElementsTuple_$T_x_H$_.typeId = "gc$" + T.typeId + "_" + H.typeId + "$";
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
function ListEnumerator(T, _callStatiConstructor) {
  var IList$1_$T$_, ListEnumerator$1_$T$_, IEnumerator$1_$T$_, __initTracker, __initTrackera;
  if (ListEnumerator[T.typeId])
    return ListEnumerator[T.typeId];
  ListEnumerator[T.typeId] = function System__Collections__Generic__ListEnumerator$1() {
  };
  ListEnumerator$1_$T$_ = ListEnumerator[T.typeId];
  ListEnumerator$1_$T$_.genericParameters = [T];
  ListEnumerator$1_$T$_.genericClosure = ListEnumerator;
  ListEnumerator$1_$T$_.typeId = "hc$" + T.typeId + "$";
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
    return ++this.index < this.innerList.V_get_Count_c();
  };
  ptyp_.system__Collections__IEnumerator__get_Current = function ListEnumerator$1__System__Collections__IEnumerator__get_Current() {
    return Type__BoxTypeInstance(T, this.get_current());
  };
  ptyp_.V_get_Current_e = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$T$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_e = ptyp_.moveNext;
  Type__RegisterReferenceType(ListEnumerator$1_$T$_, "System.Collections.Generic.ListEnumerator`1<" + T.fullName + ">", Object, [IEnumerator$1_$T$_, IEnumerator]);
  ListEnumerator$1_$T$_._tri = function() {
    if (__initTrackera)
      return;
    __initTrackera = true;
    T = T;
    IList$1_$T$_ = IList(T, true);
    ListEnumerator$1_$T$_ = ListEnumerator(T, true);
  };
  if (_callStatiConstructor)
    ListEnumerator$1_$T$_._tri();
  return ListEnumerator$1_$T$_;
};
function Enumeratora(TValue, _callStatiConstructor) {
  var KeyValuePair$2_$String_x_TValue$_, Enumerator_$TValue$_, IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_, __initTracker;
  if (Enumeratora[TValue.typeId])
    return Enumeratora[TValue.typeId];
  Enumeratora[TValue.typeId] = function System__Collections__Generic__StringDictionary$1$2fEnumerator() {
  };
  Enumerator_$TValue$_ = Enumeratora[TValue.typeId];
  Enumerator_$TValue$_.genericParameters = [TValue];
  Enumerator_$TValue$_.genericClosure = Enumeratora;
  Enumerator_$TValue$_.typeId = "ic$" + TValue.typeId + "$";
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
      this.keys = this.dict.get_keys().V_GetEnumerator_j$k$();
    }
  };
  ptyp_.get_current = function Enumerator__get_Current() {
    return KeyValuePair$2_$String_x_TValue$_.__ctor(this.keys.V_get_Current_l$k$(), this.dict.get_item(this.keys.V_get_Current_l$k$()));
  };
  ptyp_.moveNext = function Enumerator__MoveNext() {
    return this.keys.V_MoveNext_e();
  };
  ptyp_.system__Collections__IEnumerator__get_Current = function Enumerator__System__Collections__IEnumerator__get_Current() {
    return Type__BoxTypeInstance(KeyValuePair$2_$String_x_TValue$_, this.get_current());
  };
  ptyp_.V_get_Current_e = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_e = ptyp_.moveNext;
  Type__RegisterReferenceType(Enumerator_$TValue$_, "System.Collections.Generic.StringDictionary`1/Enumerator<" + TValue.fullName + ">", Object, [IEnumerator$1_$KeyValuePair$2_$String_x_TValue$_$_, IEnumerator]);
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
function IEnumerablea(T, _callStatiConstructor) {
  var IEnumerable$1_$T$_, __initTracker;
  if (IEnumerablea[T.typeId])
    return IEnumerablea[T.typeId];
  IEnumerablea[T.typeId] = function System__Collections__Generic__IEnumerable$1() {
  };
  IEnumerable$1_$T$_ = IEnumerablea[T.typeId];
  IEnumerable$1_$T$_.genericParameters = [T];
  IEnumerable$1_$T$_.genericClosure = IEnumerablea;
  IEnumerable$1_$T$_.typeId = "j$" + T.typeId + "$";
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
  Enumerator_$T$_.typeId = "jc$" + T.typeId + "$";
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
  ptyp_.V_get_Current_e = ptyp_.system__Collections__IEnumerator__get_Current;
  ptyp_["V_get_Current_" + IEnumerator$1_$T$_.typeId] = ptyp_.get_current;
  ptyp_.V_MoveNext_e = ptyp_.moveNext;
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
  KeyValuePair$2_$K_x_V$_.typeId = "kc$" + K.typeId + "_" + V.typeId + "$";
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
function IEnumeratora(T, _callStatiConstructor) {
  var IEnumerator$1_$T$_, __initTracker;
  if (IEnumeratora[T.typeId])
    return IEnumeratora[T.typeId];
  IEnumeratora[T.typeId] = function System__Collections__Generic__IEnumerator$1() {
  };
  IEnumerator$1_$T$_ = IEnumeratora[T.typeId];
  IEnumerator$1_$T$_.genericParameters = [T];
  IEnumerator$1_$T$_.genericClosure = IEnumeratora;
  IEnumerator$1_$T$_.typeId = "l$" + T.typeId + "$";
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
List_$Int32$_ = List(Int32);
List_$Number$_ = List(Number);
ObservableCollection_$Int32$_ = ObservableCollection(Int32);
ObservableCollection_$Number$_ = ObservableCollection(Number);
HeaderInjectableTransformer_$Number_x_Number$_ = HeaderInjectableTransformer(Number, Number);
ArrayG_$Int32$_ = ArrayG(Int32);
ObservableCollectionGenerator_$Number_x_String$_ = ObservableCollectionGenerator(Number, String);
ArrayG_$String$_ = ArrayG(String);
Func_$Object_x_Object$_ = Func(Object, Object);
ArrayG_$Func_$Object_x_Object$_$_ = ArrayG(Func_$Object_x_Object$_);
StringDictionary_$TypeRegistry$_ = StringDictionary(TypeRegistry);
StringDictionary_$Function$_ = StringDictionary(Function);
Action_$INotifyPropertyChanged_x_String$_ = Action(INotifyPropertyChanged, String);
StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_ = StringDictionary(Action_$INotifyPropertyChanged_x_String$_);
ArrayG_$Object$_ = ArrayG(Object);
ArrayG_$Action_$INotifyPropertyChanged_x_String$_$_ = ArrayG(Action_$INotifyPropertyChanged_x_String$_);
TestObservabelCollectionTransformer____cctor();
List_$Int32$_._tri();
List_$Number$_._tri();
ObservableCollection_$Int32$_._tri();
ObservableCollection_$Number$_._tri();
HeaderInjectableTransformer_$Number_x_Number$_._tri();
ArrayG_$Int32$_._tri();
ObservableCollectionGenerator_$Number_x_String$_._tri();
ArrayG_$String$_._tri();
Func_$Object_x_Object$_._tri();
ArrayG_$Func_$Object_x_Object$_$_._tri();
StringDictionary_$TypeRegistry$_._tri();
StringDictionary_$Function$_._tri();
Action_$INotifyPropertyChanged_x_String$_._tri();
StringDictionary_$Action_$INotifyPropertyChanged_x_String$_$_._tri();
ArrayG_$Object$_._tri();
ArrayG_$Action_$INotifyPropertyChanged_x_String$_$_._tri();
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
QUnit.test("TestReplaceRangeObservableCollection", ObservableCollectionTests__TestReplaceRangeObservableCollection);
QUnit.test("TestGetRangeObservableCollection", ObservableCollectionTests__TestGetRangeObservableCollection);
QUnit.module("Sunlight.Framework.Test.ObservableObjectTests", {
});
QUnit.test("TestCreateNewObservableObject", ObservableObjectTests__TestCreateNewObservableObject);
QUnit.test("TestFirePropertyChanged", ObservableObjectTests__TestFirePropertyChanged);
QUnit.test("TestRemovePropertyChangeCallback", ObservableObjectTests__TestRemovePropertyChangeCallback);
QUnit.module("Sunlight.Framework.Test.TestInjectableTransformation", {
});
QUnit.test("TestHeaderInjectionInsert", TestInjectableTransformation__TestHeaderInjectionInsert);
QUnit.test("TestHeaderInjectionRemove", TestInjectableTransformation__TestHeaderInjectionRemove);
QUnit.test("TestRegressionReplace", TestInjectableTransformation__TestRegressionReplace);
QUnit.test("TestHeaderInjectionReplace", TestInjectableTransformation__TestHeaderInjectionReplace);
QUnit.test("TestHeaderInjectionReset", TestInjectableTransformation__TestHeaderInjectionReset);
QUnit.module("Sunlight.Framework.Test.TestObservabelCollectionTransformer", {
});
QUnit.test("TestObservableCollectionTransformer", TestObservabelCollectionTransformer__TestObservableCollectionTransformer);
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
})();
//# sourceMappingURL=Sunlight.Framework.Test.map
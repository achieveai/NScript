
Function.typeId = "g";
Function.typeMapping = null;
Function.initializeBaseInterfaces = function System__Type__InitializeBaseInterfaces(type) {
  var rv, baseType, baseInterfaces, key, interfaces;
  if (!type.baseInterfaces) {
    rv = {
    };
    baseType = type.baseType;
    if (baseType != null && baseType != Object) {
      if (baseType)
        Function.initializeBaseInterfaces(type);
      baseInterfaces = baseType.baseInterfaces;
      if (baseInterfaces)
        for (key in baseInterfaces)
          rv[key] = baseInterfaces[key];
    }
    interfaces = type.interfaces;
    if (interfaces)
      for (key = 0; key < interfaces.length; key++)
        rv[interfaces[key]] = interfaces[key];
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
    Function.initializeBaseInterfaces(instance.constructor);
  return instance && instance.constructor.baseInterfaces && instance.constructor.baseInterfaces[this.fullName];
};
ptyp_.asType = function System__Type__AsType(instance) {
  return this.isInstanceOfType(instance) ? instance : null;
};
ptyp_.castType = function System__Type__CastType(instance) {
  if (this.isInstanceOfType(instance)) {
    if (this.isStruct)
      return instance.boxedValue;
    return instance;
  }
  throw "InvalidCast to " + this.fullName;
};
ptyp_.toLocaleStringa = function System__Type__ToLocaleString() {
  return this.fullName;
};
ptyp_.toStringa = function System__Type__ToString() {
  return this.fullName;
};
ptyp_.registerReferenceType = function System__Type__RegisterReferenceType(typeName, parentType, interfaces) {
  this.isClass = true;
  this.fullName = typeName;
  this.baseType = parentType;
  this.interfaces = interfaces;
  if (!Function.typeMapping)
    Function.typeMapping = {
    };
  Function.typeMapping[this.fullName] = this;
};
ptyp_.registerStructType = function System__Type__RegisterStructType(typeName, interfaces) {
  this.isStruct = true;
  this.fullName = typeName;
  this.baseType = System_ValueType;
  this.interfaces = interfaces;
  if (!Function.typeMapping)
    Function.typeMapping = {
    };
  Function.typeMapping[this.fullName] = this;
};
ptyp_.registerInterface = function System__Type__RegisterInterface(typeName) {
  this.isInterface = true;
  this.fullName = typeName;
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
ptyp_.getDefaultValue = function System__Type__GetDefaultValue() {
  return null;
};
ptyp_.toLocaleString = ptyp_.toLocaleStringa;
ptyp_.toString = ptyp_.toStringa;
Function.registerReferenceType("System.Type", Object, []);
Object.typeId = "h";
Object.isNullOrUndefined = function System__Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
Object.registerReferenceType("System.Object", null, []);
Sunlight_Framework_Test_ObservableObjectTests = function Sunlight__Framework__Test__ObservableObjectTestsa() {
};
Sunlight_Framework_Test_ObservableObjectTests.typeId = "i";
Sunlight_Framework_Test_ObservableObjectTests.testCreateNewObservableObject = function Sunlight__Framework__Test__ObservableObjectTests__TestCreateNewObservableObject() {
  var observableObject;
  observableObject = Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject.__ctor();
  QUnit.notEqual(null, observableObject, "ObservableObject should be created");
};
Sunlight_Framework_Test_ObservableObjectTests.testFirePropertyChanged = function Sunlight__Framework__Test__ObservableObjectTests__TestFirePropertyChanged() {
  var locals2, cb1;
  locals2 = Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass1.__ctor();
  locals2.observableObject = Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject.__ctor();
  locals2.strChanged = false;
  locals2.cbCalled = false;
  cb1 = System_Delegate.create("testFirePropertyChanged_Helper", locals2);
  locals2.observableObject.addPropertyChangedListener("StringProp", cb1);
  locals2.observableObject.set_stringProp("1");
  QUnit.ok(locals2.strChanged, "change callback called");
  locals2.strChanged = false;
  locals2.cbCalled = false;
  locals2.observableObject.set_intProp(1);
  QUnit.ok(!locals2.strChanged, "String change callback not called.");
  QUnit.ok(!locals2.cbCalled, "Callback should not be called for different property change");
};
Sunlight_Framework_Test_ObservableObjectTests.testRemovePropertyChangeCallback = function Sunlight__Framework__Test__ObservableObjectTests__TestRemovePropertyChangeCallback() {
  var locals5, observableObject, cb1;
  locals5 = Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass4.__ctor();
  observableObject = Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject.__ctor();
  locals5.cbCalled = false;
  cb1 = System_Delegate.create("testRemovePropertyChangeCallback_Helper", locals5);
  observableObject.addPropertyChangedListener("StringProp", cb1);
  observableObject.set_stringProp("1");
  QUnit.ok(locals5.cbCalled, "change callback called");
  locals5.cbCalled = false;
  observableObject.removePropertyChangedListener("StringProp", cb1);
  observableObject.set_stringProp("2");
  QUnit.ok(!locals5.cbCalled, "after removing change listner, callback should not be called.");
};
Sunlight_Framework_Test_ObservableObjectTests.registerReferenceType("Sunlight.Framework.Test.ObservableObjectTests", Object, []);
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTestsa() {
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.typeId = "j";
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepNotifiableObject = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepNotifiableObject() {
  var rv;
  rv = (function() {
    var $5fipi;
    $5fipi = Sunlight_Framework_Test_Binders_SimpleNotifiableClass.__ctor();
    $5fipi.set_intProp(10);
    $5fipi.set_strProp("Ten");
    $5fipi.set_objProp((function() {
      var $5fipi;
      $5fipi = Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.__ctor();
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
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepSimpleObject = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PrepSimpleObject() {
  return Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepNotifiableObject().get_objProp();
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.basicValueTest = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__BasicValueTest() {
  var sourceBinder, helper, src;
  sourceBinder = Sunlight_Framework_Binders_SourcePropertyBinder.__ctor(System_ArrayG_$String$_.__ctor(1), System_ArrayG_$Func_$Object_x_Object$_$_.__ctora([Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.basicValueTest_Helper]), null);
  helper = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper.__ctor();
  sourceBinder.useDataBinder(helper);
  src = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepSimpleObject();
  sourceBinder.set_source(src);
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.basicValueTestWithNotification = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__BasicValueTestWithNotification() {
  var sourceBinder, helper, src;
  sourceBinder = Sunlight_Framework_Binders_SourcePropertyBinder.__ctor(System_ArrayG_$String$_.__ctora(["IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctora([Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.basicValueTestWithNotification_Helper]), null);
  helper = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper.__ctor();
  sourceBinder.useDataBinder(helper);
  src = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepNotifiableObject();
  sourceBinder.set_source(src);
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
  helper.set_sourceValueUpdateCalled(false);
  src.set_intProp(src.get_intProp() + 1);
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueNotifiableTest = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PropertyPathValueNotifiableTest() {
  var sourceBinder, helper, src;
  sourceBinder = Sunlight_Framework_Binders_SourcePropertyBinder.__ctor(System_ArrayG_$String$_.__ctora(["NotifiableProp", "IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctora([Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueNotifiableTest_Helper, Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueNotifiableTest_Helpera]), null);
  helper = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper.__ctor();
  sourceBinder.useDataBinder(helper);
  src = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepSimpleObject();
  sourceBinder.set_source(src);
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_notifiableProp().get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
  helper.set_sourceValueUpdateCalled(false);
  src.get_notifiableProp().set_intProp( -1);
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_notifiableProp().get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueTest = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests__PropertyPathValueTest() {
  var sourceBinder, helper, src, lastValue;
  sourceBinder = Sunlight_Framework_Binders_SourcePropertyBinder.__ctor(System_ArrayG_$String$_.__ctora(["SelfProp", "IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctora([Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueTest_Helper, Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueTest_Helpera]), null);
  helper = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper.__ctor();
  sourceBinder.useDataBinder(helper);
  src = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepSimpleObject();
  sourceBinder.set_source(src);
  QUnit.ok(helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(src.get_selfProp().get_intProp(), System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
  helper.set_sourceValueUpdateCalled(false);
  lastValue = src.get_selfProp().get_intProp();
  src.get_selfProp().set_intProp( -1);
  QUnit.ok(!helper.get_sourceValueUpdateCalled(), "SourceValueUpdate called");
  QUnit.equal(lastValue, System_Int32.unbox(sourceBinder.get_value()), "SourceBinder.Value");
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.basicValueTest_Helper = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests___$BasicValueTest$_b__2(obj) {
  return System_Int32.box(Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.castType(obj).get_intProp());
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.basicValueTestWithNotification_Helper = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests___$BasicValueTestWithNotification$_b__4(obj) {
  return System_Int32.box(Sunlight_Framework_Test_Binders_SimpleNotifiableClass.castType(obj).get_intProp());
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueNotifiableTest_Helper = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests___$PropertyPathValueNotifiableTest$_b__6(obj) {
  return Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.castType(obj).get_notifiableProp();
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueNotifiableTest_Helpera = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests___$PropertyPathValueNotifiableTest$_b__7(obj) {
  return System_Int32.box(Sunlight_Framework_Test_Binders_SimpleNotifiableClass.castType(obj).get_intProp());
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueTest_Helper = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests___$PropertyPathValueTest$_b__a(obj) {
  return Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.castType(obj).get_selfProp();
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueTest_Helpera = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests___$PropertyPathValueTest$_b__b(obj) {
  return System_Int32.box(Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.castType(obj).get_intProp());
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.registerReferenceType("Sunlight.Framework.Test.Binders.SourcePropertyBinderTests", Object, []);
Sunlight_Framework_Test_ObservableCollectionTests = function Sunlight__Framework__Test__ObservableCollectionTestsa() {
};
Sunlight_Framework_Test_ObservableCollectionTests.typeId = "k";
Sunlight_Framework_Test_ObservableCollectionTests.testCreateNewObservableCollection = function Sunlight__Framework__Test__ObservableCollectionTests__TestCreateNewObservableCollection() {
  var observableCollection;
  observableCollection = Sunlight_Framework_Observables_ObservableCollection_$Int32$_.__ctor();
  QUnit.notEqual(null, observableCollection, "ObservableCollection should be created");
  QUnit.equal(0, observableCollection.get_count(), "ObservableCollection's size should be 0");
};
Sunlight_Framework_Test_ObservableCollectionTests.testAddItemToObservableCollection = function Sunlight__Framework__Test__ObservableCollectionTests__TestAddItemToObservableCollection() {
  var locals2;
  locals2 = Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass1.__ctor();
  locals2.observableCollection = Sunlight_Framework_Observables_ObservableCollection_$Int32$_.__ctor();
  locals2.eventRaised = false;
  locals2.observableCollection.add_CollectionChanged(System_Delegate.create("testAddItemToObservableCollection_Helper", locals2));
  locals2.observableCollection.add(1);
  QUnit.ok(locals2.eventRaised, "Change event raised");
};
Sunlight_Framework_Test_ObservableCollectionTests.testRemoveItemToObservableCollection = function Sunlight__Framework__Test__ObservableCollectionTests__TestRemoveItemToObservableCollection() {
  var locals5;
  locals5 = Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass4.__ctor();
  locals5.observableCollection = Sunlight_Framework_Observables_ObservableCollection_$Int32$_.__ctor();
  locals5.eventRaised = false;
  locals5.observableCollection.add(1);
  locals5.observableCollection.add(2);
  locals5.observableCollection.add(3);
  locals5.observableCollection.add_CollectionChanged(System_Delegate.create("testRemoveItemToObservableCollection_Helper", locals5));
  locals5.observableCollection.removeRangeAt(1, 2);
  QUnit.ok(locals5.eventRaised, "Change event raised");
};
Sunlight_Framework_Test_ObservableCollectionTests.registerReferenceType("Sunlight.Framework.Test.ObservableCollectionTests", Object, []);
Sunlight_Framework_Test_Binders_DataBinderTests = function Sunlight__Framework__Test__Binders__DataBinderTestsa() {
};
Sunlight_Framework_Test_Binders_DataBinderTests.typeId = "l";
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTest = function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderSimpleObjectTest() {
  var dataBinder, target, source;
  dataBinder = Sunlight_Framework_Binders_DataBinder.__ctor(Sunlight_Framework_Binders_SourcePropertyBinder.__ctor(System_ArrayG_$String$_.__ctora(["IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctora([Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTest_Helper]), null), Sunlight_Framework_Binders_TargetBinder.__ctor("IntProp", null, Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTest_Helpera, System_Int32.box( -1)), 1, null);
  target = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepNotifiableObject();
  source = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepSimpleObject();
  source.set_intProp(101);
  dataBinder.setTarget(target);
  QUnit.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(source);
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(null);
  QUnit.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTestReverseOrder = function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderSimpleObjectTestReverseOrder() {
  var dataBinder, target, source;
  dataBinder = Sunlight_Framework_Binders_DataBinder.__ctor(Sunlight_Framework_Binders_SourcePropertyBinder.__ctor(System_ArrayG_$String$_.__ctora(["IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctora([Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTestReverseOrder_Helper]), null), Sunlight_Framework_Binders_TargetBinder.__ctor("IntProp", null, Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTestReverseOrder_Helpera, System_Int32.box( -1)), 1, null);
  target = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepNotifiableObject();
  source = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepSimpleObject();
  source.set_intProp(101);
  dataBinder.setSource(source);
  dataBinder.setTarget(target);
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(null);
  QUnit.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderOneWayNotifiableObjectTest = function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderOneWayNotifiableObjectTest() {
  var dataBinder, target, source;
  dataBinder = Sunlight_Framework_Binders_DataBinder.__ctor(Sunlight_Framework_Binders_SourcePropertyBinder.__ctor(System_ArrayG_$String$_.__ctora(["IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctora([Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderOneWayNotifiableObjectTest_Helper]), null), Sunlight_Framework_Binders_TargetBinder.__ctor("IntProp", null, Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderOneWayNotifiableObjectTest_Helpera, System_Int32.box( -1)), 1, null);
  target = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepNotifiableObject();
  source = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepNotifiableObject();
  source.set_intProp(101);
  dataBinder.setTarget(target);
  QUnit.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(source);
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  source.set_intProp(102);
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderTwoWayNotifiableObjectTest = function Sunlight__Framework__Test__Binders__DataBinderTests__BasicBinderTwoWayNotifiableObjectTest() {
  var dataBinder, target, source;
  dataBinder = Sunlight_Framework_Binders_DataBinder.__ctor(Sunlight_Framework_Binders_SourcePropertyBinder.__ctor(System_ArrayG_$String$_.__ctora(["IntProp"]), System_ArrayG_$Func_$Object_x_Object$_$_.__ctora([Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderTwoWayNotifiableObjectTest_Helper]), Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderTwoWayNotifiableObjectTest_Helpera), Sunlight_Framework_Binders_TargetBinder.__ctor("IntProp", Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderTwoWayNotifiableObjectTest_Helperb, Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderTwoWayNotifiableObjectTest_Helperc, System_Int32.box( -1)), 2, null);
  target = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepNotifiableObject();
  source = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.prepNotifiableObject();
  source.set_intProp(101);
  dataBinder.setTarget(target);
  QUnit.equal( -1, target.get_intProp(), "source.IntProp == target.IntProp");
  dataBinder.setSource(source);
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
  target.set_intProp(target.get_intProp() + 1);
  QUnit.equal(source.get_intProp(), target.get_intProp(), "source.IntProp == target.IntProp");
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTest_Helper = function Sunlight__Framework__Test__Binders__DataBinderTests___$BasicBinderSimpleObjectTest$_b__0(obj) {
  return System_Int32.box(Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.castType(obj).get_intProp());
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTest_Helpera = function Sunlight__Framework__Test__Binders__DataBinderTests___$BasicBinderSimpleObjectTest$_b__1(obj, value) {
  Sunlight_Framework_Test_Binders_SimpleNotifiableClass.castType(obj).set_intProp(System_Int32.unbox(value));
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTestReverseOrder_Helper = function Sunlight__Framework__Test__Binders__DataBinderTests___$BasicBinderSimpleObjectTestReverseOrder$_b__4(obj) {
  return System_Int32.box(Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.castType(obj).get_intProp());
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTestReverseOrder_Helpera = function Sunlight__Framework__Test__Binders__DataBinderTests___$BasicBinderSimpleObjectTestReverseOrder$_b__5(obj, value) {
  Sunlight_Framework_Test_Binders_SimpleNotifiableClass.castType(obj).set_intProp(System_Int32.unbox(value));
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderOneWayNotifiableObjectTest_Helper = function Sunlight__Framework__Test__Binders__DataBinderTests___$BasicBinderOneWayNotifiableObjectTest$_b__8(obj) {
  return System_Int32.box(Sunlight_Framework_Test_Binders_SimpleNotifiableClass.castType(obj).get_intProp());
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderOneWayNotifiableObjectTest_Helpera = function Sunlight__Framework__Test__Binders__DataBinderTests___$BasicBinderOneWayNotifiableObjectTest$_b__9(obj, value) {
  Sunlight_Framework_Test_Binders_SimpleNotifiableClass.castType(obj).set_intProp(System_Int32.unbox(value));
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderTwoWayNotifiableObjectTest_Helper = function Sunlight__Framework__Test__Binders__DataBinderTests___$BasicBinderTwoWayNotifiableObjectTest$_b__c(obj) {
  return System_Int32.box(Sunlight_Framework_Test_Binders_SimpleNotifiableClass.castType(obj).get_intProp());
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderTwoWayNotifiableObjectTest_Helpera = function Sunlight__Framework__Test__Binders__DataBinderTests___$BasicBinderTwoWayNotifiableObjectTest$_b__d(obj, value) {
  Sunlight_Framework_Test_Binders_SimpleNotifiableClass.castType(obj).set_intProp(System_Int32.unbox(value));
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderTwoWayNotifiableObjectTest_Helperb = function Sunlight__Framework__Test__Binders__DataBinderTests___$BasicBinderTwoWayNotifiableObjectTest$_b__e(obj) {
  return System_Int32.box(Sunlight_Framework_Test_Binders_SimpleNotifiableClass.castType(obj).get_intProp());
};
Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderTwoWayNotifiableObjectTest_Helperc = function Sunlight__Framework__Test__Binders__DataBinderTests___$BasicBinderTwoWayNotifiableObjectTest$_b__f(obj, value) {
  Sunlight_Framework_Test_Binders_SimpleNotifiableClass.castType(obj).set_intProp(System_Int32.unbox(value));
};
Sunlight_Framework_Test_Binders_DataBinderTests.registerReferenceType("Sunlight.Framework.Test.Binders.DataBinderTests", Object, []);
Sunlight_Framework_Observables_INotifyPropertyChanged = function Sunlight__Framework__Observables__INotifyPropertyChangeda() {
};
Sunlight_Framework_Observables_INotifyPropertyChanged.typeId = "d";
Sunlight_Framework_Observables_INotifyPropertyChanged.registerInterface("Sunlight.Framework.Observables.INotifyPropertyChanged");
Sunlight_Framework_Observables_ObservableObject = function Sunlight__Framework__Observables__ObservableObjecta() {
};
Sunlight_Framework_Observables_ObservableObject.typeId = "m";
ptyp_ = Sunlight_Framework_Observables_ObservableObject.prototype;
ptyp_.eventHandlers = null;
ptyp_.addPropertyChangedListener = function Sunlight__Framework__Observables__ObservableObject__AddPropertyChangedListener(propertyName, callback) {
  var V_0;
  if (this.eventHandlers == null)
    this.eventHandlers = System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_.__ctor();
  if (!this.eventHandlers.tryGetValue(propertyName, {
    read: function() {
      return V_0;
    },
    write: function(arg0) {
      return V_0 = arg0;
    }
  }))
    V_0 = callback;
  else
    V_0 = System_Delegate.combine(V_0, callback);
  this.eventHandlers.set_item(propertyName, V_0);
};
ptyp_.removePropertyChangedListener = function Sunlight__Framework__Observables__ObservableObject__RemovePropertyChangedListener(propertyName, callback) {
  var V_0;
  if (this.eventHandlers == null)
    return;
  if (this.eventHandlers.tryGetValue(propertyName, {
    read: function() {
      return V_0;
    },
    write: function(arg0) {
      return V_0 = arg0;
    }
  })) {
    V_0 = System_Delegate.remove(V_0, callback);
    if (V_0 != null)
      this.eventHandlers.set_item(propertyName, V_0);
    else
      this.eventHandlers.remove(propertyName);
  }
};
ptyp_.firePropertyChanged = function Sunlight__Framework__Observables__ObservableObject__FirePropertyChanged(propertyName) {
  var V_0;
  if (this.eventHandlers != null && this.eventHandlers.tryGetValue(propertyName, {
    read: function() {
      return V_0;
    },
    write: function(arg0) {
      return V_0 = arg0;
    }
  }))
    V_0(this, propertyName);
};
ptyp_.__ctor = function Sunlight__Framework__Observables__ObservableObject____ctor() {
  this.eventHandlers = null;
};
ptyp_.V_AddPropertyChangedListener_d = ptyp_.addPropertyChangedListener;
ptyp_.V_RemovePropertyChangedListener_d = ptyp_.removePropertyChangedListener;
Sunlight_Framework_Observables_ObservableObject.registerReferenceType("Sunlight.Framework.Observables.ObservableObject", Object, [Sunlight_Framework_Observables_INotifyPropertyChanged]);
Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject = function Sunlight__Framework__Test__ObservableObjectTests$2fObservableTestObjecta() {
};
Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject.typeId = "n";
Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject.__ctor = function Sunlight_Framework_Test_ObservableObjectTests$2fObservableTestObject_factorya() {
  var this_;
  this_ = new Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject();
  this_.__ctora();
  return this_;
};
ptyp_ = new Sunlight_Framework_Observables_ObservableObject();
Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject.prototype = ptyp_;
ptyp_.strField = null;
ptyp_.intProp = 0;
ptyp_.set_intProp = function Sunlight__Framework__Test__ObservableObjectTests$2fObservableTestObject__set_IntProp(value) {
  if (value != this.intProp) {
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
Sunlight_Framework_Test_ObservableObjectTests_ObservableTestObject.registerReferenceType("Sunlight.Framework.Test.ObservableObjectTests/ObservableTestObject", Sunlight_Framework_Observables_ObservableObject, []);
Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass1 = function Sunlight__Framework__Test__ObservableObjectTests$2f_$$_c__DisplayClass1a() {
};
Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass1.typeId = "o";
Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass1.__ctor = function Sunlight_Framework_Test_ObservableObjectTests$2f_$$_c__DisplayClass1_factorya() {
  return new Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass1();
};
ptyp_ = Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass1.prototype;
ptyp_.observableObject = null;
ptyp_.strChanged = false;
ptyp_.cbCalled = false;
ptyp_.__ctor = function Sunlight__Framework__Test__ObservableObjectTests$2f_$$_c__DisplayClass1____ctor() {
};
ptyp_.testFirePropertyChanged_Helper = function Sunlight__Framework__Test__ObservableObjectTests$2f_$$_c__DisplayClass1___$TestFirePropertyChanged$_b__0(sender, propName) {
  this.strChanged = propName === "StringProp" && sender == this.observableObject;
  this.cbCalled = true;
};
Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass1.registerReferenceType("Sunlight.Framework.Test.ObservableObjectTests/<>c__DisplayClass1", Object, []);
System_Delegate = function System__Delegatea() {
};
System_Delegate.typeId = "p";
System_Delegate.combine = function System__Delegate__Combine(a, b) {
  var funcs;
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
  return System_Delegate.createJoinedArray(funcs);
};
System_Delegate.create = function System__Delegate__Create(functionName, instance) {
  var func, functionName;
  func = instance[functionName];
  functionName = "__@" + functionName;
  if (!(functionName in instance)) {
    instance[functionName] = function() {
        return func.apply(instance, arguments);
    };
    instance[functionName].isDelegate = true;
  }
  return instance[functionName];
};
System_Delegate.remove = function System__Delegate__Remove(source, value) {
  var newArr, idx, valArr, i, fullMatch, j;
  if (source == value)
    return null;
  if (typeof source.funcs == "undefined")
    return source;
  newArr = [].concat(source.funcs);
  if (typeof value.funcs == "undefined") {
    idx = newArr.lastIndexOf(value);
    if (idx >= 0)
      newArr.splice(idx, 1);
  }
  else {
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
  if (newArr.length == 0)
    return null;
  else if (newArr.length == 1)
    return newArr[0];
  else if (newArr.length == source.funcs.length)
    return source;
  return System_Delegate.createJoinedArray(newArr);
};
System_Delegate.createJoinedArray = function System__Delegate__CreateJoinedArray(array) {
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
System_Delegate.registerReferenceType("System.Delegate", Object, []);
Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass4 = function Sunlight__Framework__Test__ObservableObjectTests$2f_$$_c__DisplayClass4a() {
};
Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass4.typeId = "q";
Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass4.__ctor = function Sunlight_Framework_Test_ObservableObjectTests$2f_$$_c__DisplayClass4_factorya() {
  return new Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass4();
};
ptyp_ = Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass4.prototype;
ptyp_.cbCalled = false;
ptyp_.__ctor = function Sunlight__Framework__Test__ObservableObjectTests$2f_$$_c__DisplayClass4____ctor() {
};
ptyp_.testRemovePropertyChangeCallback_Helper = function Sunlight__Framework__Test__ObservableObjectTests$2f_$$_c__DisplayClass4___$TestRemovePropertyChangeCallback$_b__3(sender, propName) {
  this.cbCalled = true;
};
Sunlight_Framework_Test_ObservableObjectTests__$$_c__DisplayClass4.registerReferenceType("Sunlight.Framework.Test.ObservableObjectTests/<>c__DisplayClass4", Object, []);
Sunlight_Framework_Binders_SourcePropertyBinder = function Sunlight__Framework__Binders__SourcePropertyBindera() {
};
Sunlight_Framework_Binders_SourcePropertyBinder.typeId = "r";
Sunlight_Framework_Binders_SourcePropertyBinder.__ctor = function Sunlight_Framework_Binders_SourcePropertyBinder_factorya(propertyPartNames, propertyGetterChain, propertySetter) {
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
  var V_0;
  this.propertyPartNames = propertyPartNames;
  this.chainLength = this.propertyPartNames.V_get_Length();
  this.propertyGetterChain = propertyGetterChain;
  this.propertySetter = propertySetter;
  this.objectChain = System_ArrayG_$Object$_.__ctor(this.chainLength);
  this.changeRegistrations = System_ArrayG_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_.__ctor(this.chainLength);
  for (V_0 = this.chainLength - 1; V_0 >= 0; --V_0)
    this.changeRegistrations.set_item(V_0, this.getChangeTrackerAt(V_0));
};
ptyp_.set_source = function Sunlight__Framework__Binders__SourcePropertyBinder__set_Source(value) {
  if (this.objectChain.get_item(0) != value) {
    this.setObjectChainElementValue(0, value);
    this.calculateValueFrom(0);
  }
};
ptyp_.get_value = function Sunlight__Framework__Binders__SourcePropertyBinder__get_Value() {
  return this.value;
};
ptyp_.set_value = function Sunlight__Framework__Binders__SourcePropertyBinder__set_Value(value) {
  if (this.isActive && this.propertySetter != null)
    this.propertySetter(this.objectChain.get_item(this.objectChain.V_get_Length() - 1), value);
};
ptyp_.get_isActive = function Sunlight__Framework__Binders__SourcePropertyBinder__get_IsActive() {
  return this.isActive;
};
ptyp_.useDataBinder = function Sunlight__Framework__Binders__SourcePropertyBinder__UseDataBinder(dataBinderBase) {
  this.dataBinderBase = dataBinderBase;
};
ptyp_.calculateValueFrom = function Sunlight__Framework__Binders__SourcePropertyBinder__CalculateValueFrom(index) {
  var V_0, V_1, V_2;
  for (V_0 = index; V_0 < this.chainLength; ++V_0) {
    if (this.objectChain.get_item(V_0) == null) {
      for (V_1 = this.chainLength - 1; V_1 >= V_0; --V_1)
        this.setObjectChainElementValue(V_1, null);
      if (!(this.value != null || this.isActive))
        break;
      this.isActive = false;
      this.value = null;
      if (this.dataBinderBase != null)
        this.dataBinderBase.V_SourceValueUpdated_e();
      break;
    }
    V_2 = this.propertyGetterChain.get_item(V_0)(this.objectChain.get_item(V_0));
    if (V_0 < this.objectChain.V_get_Length() - 1)
      this.setObjectChainElementValue(V_0 + 1, V_2);
    else if (!(this.value != V_2 ? 0 : this.isActive)) {
      this.isActive = true;
      this.value = V_2;
      if (this.dataBinderBase != null)
        this.dataBinderBase.V_SourceValueUpdated_e();
    }
  }
};
ptyp_.setObjectChainElementValue = function Sunlight__Framework__Binders__SourcePropertyBinder__SetObjectChainElementValue(index, value) {
  var V_0, V_1;
  if (index > this.chainLength)
    return;
  V_0 = Sunlight_Framework_Observables_INotifyPropertyChanged.asType(this.objectChain.get_item(index));
  if (V_0 != null)
    V_0.V_RemovePropertyChangedListener_d(this.propertyPartNames.get_item(index), this.changeRegistrations.get_item(index));
  this.objectChain.set_item(index, value);
  V_1 = Sunlight_Framework_Observables_INotifyPropertyChanged.asType(value);
  if (V_1 != null)
    V_1.V_AddPropertyChangedListener_d(this.propertyPartNames.get_item(index), this.changeRegistrations.get_item(index));
};
ptyp_.getChangeTrackerAt = function Sunlight__Framework__Binders__SourcePropertyBinder__GetChangeTrackerAt(index) {
  var V_0;
  V_0 = Sunlight_Framework_Binders_SourcePropertyBinder__$$_c__DisplayClass1.__ctor();
  V_0.index = index;
  V_0.this_ = this;
  return System_Delegate.create("getChangeTrackerAt_Helper", V_0);
};
Sunlight_Framework_Binders_SourcePropertyBinder.registerReferenceType("Sunlight.Framework.Binders.SourcePropertyBinder", Object, []);
Sunlight_Framework_Binders_ISourceDataBinder = function Sunlight__Framework__Binders__ISourceDataBindera() {
};
Sunlight_Framework_Binders_ISourceDataBinder.typeId = "e";
Sunlight_Framework_Binders_ISourceDataBinder.registerInterface("Sunlight.Framework.Binders.ISourceDataBinder");
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelpera() {
};
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper.typeId = "s";
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper.__ctor = function Sunlight_Framework_Test_Binders_SourcePropertyBinderTests$2fBinderTestHelper_factorya() {
  return new Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper();
};
ptyp_ = Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper.prototype;
ptyp_._$SourceValueUpdateCalled$_k__BackingField = false;
ptyp_.get_sourceValueUpdateCalled = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper__get_SourceValueUpdateCalled() {
  return this._$SourceValueUpdateCalled$_k__BackingField;
};
ptyp_.set_sourceValueUpdateCalled = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper__set_SourceValueUpdateCalled(value) {
  this._$SourceValueUpdateCalled$_k__BackingField = value;
};
ptyp_.sourceValueUpdated = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper__SourceValueUpdated() {
  this.set_sourceValueUpdateCalled(true);
};
ptyp_.__ctor = function Sunlight__Framework__Test__Binders__SourcePropertyBinderTests$2fBinderTestHelper____ctor() {
};
ptyp_.V_SourceValueUpdated_e = ptyp_.sourceValueUpdated;
Sunlight_Framework_Test_Binders_SourcePropertyBinderTests_BinderTestHelper.registerReferenceType("Sunlight.Framework.Test.Binders.SourcePropertyBinderTests/BinderTestHelper", Object, [Sunlight_Framework_Binders_ISourceDataBinder]);
Sunlight_Framework_Test_Binders_SimpleObjectWithProperty = function Sunlight__Framework__Test__Binders__SimpleObjectWithPropertya() {
};
Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.typeId = "t";
Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.__ctor = function Sunlight_Framework_Test_Binders_SimpleObjectWithProperty_factorya() {
  return new Sunlight_Framework_Test_Binders_SimpleObjectWithProperty();
};
ptyp_ = Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.prototype;
ptyp_._$StringProp$_k__BackingField = null;
ptyp_._$IntProp$_k__BackingField = 0;
ptyp_._$SelfProp$_k__BackingField = null;
ptyp_._$NotifiableProp$_k__BackingField = null;
ptyp_.set_stringProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__set_StringProp(value) {
  this._$StringProp$_k__BackingField = value;
};
ptyp_.get_intProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__get_IntProp() {
  return this._$IntProp$_k__BackingField;
};
ptyp_.set_intProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__set_IntProp(value) {
  this._$IntProp$_k__BackingField = value;
};
ptyp_.get_selfProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__get_SelfProp() {
  return this._$SelfProp$_k__BackingField;
};
ptyp_.set_selfProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__set_SelfProp(value) {
  this._$SelfProp$_k__BackingField = value;
};
ptyp_.get_notifiableProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__get_NotifiableProp() {
  return this._$NotifiableProp$_k__BackingField;
};
ptyp_.set_notifiableProp = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty__set_NotifiableProp(value) {
  this._$NotifiableProp$_k__BackingField = value;
};
ptyp_.__ctor = function Sunlight__Framework__Test__Binders__SimpleObjectWithProperty____ctor() {
};
Sunlight_Framework_Test_Binders_SimpleObjectWithProperty.registerReferenceType("Sunlight.Framework.Test.Binders.SimpleObjectWithProperty", Object, []);
System_ValueType = function System__ValueTypea() {
};
System_ValueType.typeId = "u";
System_ValueType.registerReferenceType("System.ValueType", Object, []);
System_Int32 = function(boxedValue) {
  this.boxedValue = boxedValue;
};
System_Int32.typeId = "c";
System_Int32.getDefaultValue = function() {
  return 0;
};
System_Int32.toStringc = function System__Int32__ToStringa(this_, radix) {
  return this_.toString(radix);
};
System_Int32.toStringb = function System__Int32__ToString(this_) {
  return System_Int32.toStringc(this_, 10);
};
System_Int32.toLocaleStringb = function System__Int32__ToLocaleString(this_) {
  return System_Int32.toStringb(this_);
};
ptyp_ = new System_ValueType();
System_Int32.prototype = ptyp_;
ptyp_.toLocaleString = function() {
  return System_Int32.toLocaleStringb(this.boxedValue);
};
ptyp_.toString = function() {
  return System_Int32.toStringb(this.boxedValue);
};
System_Int32.registerStructType("System.Int32", []);
String.typeId = "v";
String.formatHelperRegex = null;
String.trimStartHelperRegex = null;
String.trimEndHelperRegex = null;
String.__cctor = function System__String____cctor() {
  String.formatHelperRegex = new RegExp("(\\\\{[^\\\\}^\\\\{]+\\\\})", "g");
  String.trimStartHelperRegex = new RegExp("^\\\\s*");
  String.trimEndHelperRegex = new RegExp("\\\\s*$");
};
String.registerReferenceType("System.String", Object, []);
System_Array = function System__Arraya() {
};
System_Array.typeId = "w";
ptyp_ = System_Array.prototype;
ptyp_.__ctor = function System__Array____ctor() {
};
System_Array.registerReferenceType("System.Array", Object, []);
RegExp.typeId = "x";
RegExp.registerReferenceType("System.RegularExpression", Object, []);
System_MulticastDelegate = function System__MulticastDelegatea() {
};
System_MulticastDelegate.typeId = "y";
System_MulticastDelegate.prototype = new System_Delegate();
System_MulticastDelegate.registerReferenceType("System.MulticastDelegate", System_Delegate, []);
Sunlight_Framework_Test_Binders_SimpleNotifiableClass = function Sunlight__Framework__Test__Binders__SimpleNotifiableClassa() {
};
Sunlight_Framework_Test_Binders_SimpleNotifiableClass.typeId = "z";
Sunlight_Framework_Test_Binders_SimpleNotifiableClass.__ctor = function Sunlight_Framework_Test_Binders_SimpleNotifiableClass_factorya() {
  var this_;
  this_ = new Sunlight_Framework_Test_Binders_SimpleNotifiableClass();
  this_.__ctora();
  return this_;
};
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
  if (this.intField != value) {
    this.intField = value;
    this.firePropertyChanged("IntProp");
  }
};
ptyp_.set_selfProp = function Sunlight__Framework__Test__Binders__SimpleNotifiableClass__set_SelfProp(value) {
  if (this.selfField != value) {
    this.selfField = value;
    this.firePropertyChanged("SelfProp");
  }
};
ptyp_.get_objProp = function Sunlight__Framework__Test__Binders__SimpleNotifiableClass__get_ObjProp() {
  return this.objField;
};
ptyp_.set_objProp = function Sunlight__Framework__Test__Binders__SimpleNotifiableClass__set_ObjProp(value) {
  if (this.objField != value) {
    this.objField = value;
    this.firePropertyChanged("ObjProp");
  }
};
ptyp_.__ctora = function Sunlight__Framework__Test__Binders__SimpleNotifiableClass____ctor() {
  this.__ctor();
};
Sunlight_Framework_Test_Binders_SimpleNotifiableClass.registerReferenceType("Sunlight.Framework.Test.Binders.SimpleNotifiableClass", Sunlight_Framework_Observables_ObservableObject, []);
Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass1 = function Sunlight__Framework__Test__ObservableCollectionTests$2f_$$_c__DisplayClass1a() {
};
Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass1.typeId = "A";
Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass1.__ctor = function Sunlight_Framework_Test_ObservableCollectionTests$2f_$$_c__DisplayClass1_factorya() {
  return new Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass1();
};
ptyp_ = Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass1.prototype;
ptyp_.observableCollection = null;
ptyp_.eventRaised = false;
ptyp_.__ctor = function Sunlight__Framework__Test__ObservableCollectionTests$2f_$$_c__DisplayClass1____ctor() {
};
ptyp_.testAddItemToObservableCollection_Helper = function Sunlight__Framework__Test__ObservableCollectionTests$2f_$$_c__DisplayClass1___$TestAddItemToObservableCollection$_b__0(coll, evtArg) {
  QUnit.equal(this.observableCollection, coll, "ObservableCollection");
  QUnit.equal(1, evtArg.get_newItems().V_get_Count_b$c$(), "evtArg.NewItems.Count");
  QUnit.ok(Object.isNullOrUndefined(evtArg.get_oldItems()), "Object.IsNullOrUndefined(evtArg.OldItems)");
  QUnit.equal(0, evtArg.get_changeIndex(), "evtArg.changeIndex");
  this.eventRaised = true;
};
Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass1.registerReferenceType("Sunlight.Framework.Test.ObservableCollectionTests/<>c__DisplayClass1", Object, []);
Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass4 = function Sunlight__Framework__Test__ObservableCollectionTests$2f_$$_c__DisplayClass4a() {
};
Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass4.typeId = "bb";
Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass4.__ctor = function Sunlight_Framework_Test_ObservableCollectionTests$2f_$$_c__DisplayClass4_factorya() {
  return new Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass4();
};
ptyp_ = Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass4.prototype;
ptyp_.observableCollection = null;
ptyp_.eventRaised = false;
ptyp_.__ctor = function Sunlight__Framework__Test__ObservableCollectionTests$2f_$$_c__DisplayClass4____ctor() {
};
ptyp_.testRemoveItemToObservableCollection_Helper = function Sunlight__Framework__Test__ObservableCollectionTests$2f_$$_c__DisplayClass4___$TestRemoveItemToObservableCollection$_b__3(coll, evtArg) {
  QUnit.equal(this.observableCollection, coll, "ObservableCollection");
  QUnit.equal(2, evtArg.get_oldItems().V_get_Count_b$c$(), "evtArg.OldItems.Count");
  QUnit.ok(Object.isNullOrUndefined(evtArg.get_newItems()), "Object.IsNullOrUndefined(evtArg.NewItems)");
  QUnit.equal(1, evtArg.get_changeIndex(), "evtArg.changeIndex");
  this.eventRaised = true;
};
Sunlight_Framework_Test_ObservableCollectionTests__$$_c__DisplayClass4.registerReferenceType("Sunlight.Framework.Test.ObservableCollectionTests/<>c__DisplayClass4", Object, []);
Sunlight_Framework_Binders_ITargetDataBinder = function Sunlight__Framework__Binders__ITargetDataBindera() {
};
Sunlight_Framework_Binders_ITargetDataBinder.typeId = "f";
Sunlight_Framework_Binders_ITargetDataBinder.registerInterface("Sunlight.Framework.Binders.ITargetDataBinder");
Sunlight_Framework_Binders_DataBinder = function Sunlight__Framework__Binders__DataBindera() {
};
Sunlight_Framework_Binders_DataBinder.typeId = "bc";
Sunlight_Framework_Binders_DataBinder.__ctor = function Sunlight_Framework_Binders_DataBinder_factorya(sourceBinder, targetBinder, dataBindingMode, converter) {
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
  this.firstBindingSuccessful = false;
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
  if (this.targetBinder.get_isActive())
    if (this.bindingMode == 0) {
      return;
      if (this.sourceBinder.get_isActive()) {
        this.targetBinder.set_value(this.sourceBinder.get_value());
        this.firstBindingSuccessful = true;
      }
      else
        this.targetBinder.setDefault();
    }
    else if (this.sourceBinder.get_isActive())
      this.targetBinder.set_value(this.sourceBinder.get_value());
    else
      this.targetBinder.setDefault();
};
ptyp_.applyBackBinding = function Sunlight__Framework__Binders__DataBinder__ApplyBackBinding() {
  if (!this.targetBinder.get_isWriteOnly() && this.sourceBinder.get_isActive())
    this.sourceBinder.set_value(this.targetBinder.get_value());
};
ptyp_.V_SourceValueUpdated_e = ptyp_.sourceValueUpdated;
ptyp_.V_TargetValueUpdated_f = ptyp_.targetValueUpdated;
Sunlight_Framework_Binders_DataBinder.registerReferenceType("Sunlight.Framework.Binders.DataBinder", Object, [Sunlight_Framework_Binders_ISourceDataBinder, Sunlight_Framework_Binders_ITargetDataBinder]);
Sunlight_Framework_Binders_TargetBinder = function Sunlight__Framework__Binders__TargetBindera() {
};
Sunlight_Framework_Binders_TargetBinder.typeId = "bd";
Sunlight_Framework_Binders_TargetBinder.__ctor = function Sunlight_Framework_Binders_TargetBinder_factorya(propertyName, getter, setter, defaultValue) {
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
  if (this.value != value && this.setter != null) {
    this.value = value;
    if (this.get_isActive())
      this.setter(this.target, this.value);
  }
};
ptyp_.get_isWriteOnly = function Sunlight__Framework__Binders__TargetBinder__get_IsWriteOnly() {
  return this.getter == null;
};
ptyp_.get_isActive = function Sunlight__Framework__Binders__TargetBinder__get_IsActive() {
  return this.target != null;
};
ptyp_.set_target = function Sunlight__Framework__Binders__TargetBinder__set_Target(value) {
  if (this.target != value) {
    if (this.target != null)
      this.target.V_RemovePropertyChangedListener_d(this.propertyName, System_Delegate.create("onTargetUpdated", this));
    this.target = value;
    this.value = null;
    if (this.target != null) {
      this.target.V_AddPropertyChangedListener_d(this.propertyName, System_Delegate.create("onTargetUpdated", this));
      if (this.getter != null)
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
  if (this.dataBinder != null && this.getter != null) {
    this.value = this.getter(this.target);
    this.dataBinder.V_TargetValueUpdated_f();
  }
};
Sunlight_Framework_Binders_TargetBinder.registerReferenceType("Sunlight.Framework.Binders.TargetBinder", Object, []);
System_Function = function(boxedValue) {
  this.boxedValue = boxedValue;
};
System_Function.typeId = "be";
System_Function.getDefaultValue = function() {
  return {
    length: 0
  };
};
System_Function.prototype = new System_ValueType();
System_Function.registerStructType("System.Function", []);
Array.typeId = "bf";
ptyp_ = Array.prototype;
ptyp_.pusha = function System__NativeArray__Push(value) {
  return this.push(value);
};
ptyp_.removeAt = function System__NativeArray__RemoveAt(index) {
  var i;
  if (index < 0 || index > this.length)
    throw new Error("Index out of range");
  for (i = this.length - 2; i >= index; i--)
    this[i] = this[i + 1];
  this.pop();
};
ptyp_.setAt = function System__NativeArray__SetAt(index, value) {
  this[index] = value;
};
Array.registerReferenceType("System.NativeArray", Object, []);
Sunlight_Framework_ExceptionHelpers = function Sunlight__Framework__ExceptionHelpersa() {
};
Sunlight_Framework_ExceptionHelpers.throwOnOutOfRange = function Sunlight__Framework__ExceptionHelpers__ThrowOnOutOfRange(value, minValue, maxValue, argumentName) {
  if (value < minValue || value > maxValue)
    throw new Error("Out of range exception: " + argumentName);
};
Sunlight_Framework_Binders_SourcePropertyBinder__$$_c__DisplayClass1 = function Sunlight__Framework__Binders__SourcePropertyBinder$2f_$$_c__DisplayClass1a() {
};
Sunlight_Framework_Binders_SourcePropertyBinder__$$_c__DisplayClass1.typeId = "bg";
Sunlight_Framework_Binders_SourcePropertyBinder__$$_c__DisplayClass1.__ctor = function Sunlight_Framework_Binders_SourcePropertyBinder$2f_$$_c__DisplayClass1_factorya() {
  return new Sunlight_Framework_Binders_SourcePropertyBinder__$$_c__DisplayClass1();
};
ptyp_ = Sunlight_Framework_Binders_SourcePropertyBinder__$$_c__DisplayClass1.prototype;
ptyp_.this_ = null;
ptyp_.index = 0;
ptyp_.__ctor = function Sunlight__Framework__Binders__SourcePropertyBinder$2f_$$_c__DisplayClass1____ctor() {
};
ptyp_.getChangeTrackerAt_Helper = function Sunlight__Framework__Binders__SourcePropertyBinder$2f_$$_c__DisplayClass1___$GetChangeTrackerAt$_b__0(sender, prop) {
  this.this_.calculateValueFrom(this.index);
};
Sunlight_Framework_Binders_SourcePropertyBinder__$$_c__DisplayClass1.registerReferenceType("Sunlight.Framework.Binders.SourcePropertyBinder/<>c__DisplayClass1", Object, []);
Error.typeId = "bh";
Error.registerReferenceType("System.Exception", Object, []);
System_Arraya = function(T, $5fcallStatiConstructor) {
  var ArrayG$1_$T$_, ICollection$1_$T$_, $5f_initTracker;
  if (System_Arraya[T.typeId])
    return System_Arraya[T.typeId];
  System_Arraya[T.typeId] = function System__ArrayG$1a() {
  };
  ArrayG$1_$T$_ = System_Arraya[T.typeId];
  ArrayG$1_$T$_.typeId = "bi$" + T.typeId + "$";
  ICollection$1_$T$_ = System_Collections_Generic_ICollection(T, $5fcallStatiConstructor);
  ArrayG$1_$T$_.__ctor = function System_Array$1_factoryb(size) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctora(size);
    return this_;
  };
  ArrayG$1_$T$_.__ctora = function System_Array$1_factoryc(nativeArray) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctorb(nativeArray);
    return this_;
  };
  ptyp_ = new System_Array();
  ArrayG$1_$T$_.prototype = ptyp_;
  ptyp_.innerArray = null;
  ptyp_.__ctora = function System__ArrayG$1____ctor(size) {
    var i, def;
    this.__ctor();
    this.innerArray = new Array(size);
    for (def = T.getDefaultValue(), i = 0; i < size; ++i)
      this.innerArray.setAt(i, def);
  };
  ptyp_.__ctorb = function System__ArrayG$1____ctora(nativeArray) {
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
  ArrayG$1_$T$_.registerReferenceType("System.ArrayG`1<" + T.fullName + ">", System_Array, [ICollection$1_$T$_]);
  ArrayG$1_$T$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    ArrayG$1_$T$_ = System_Arraya(T, true);
  };
  if ($5fcallStatiConstructor)
    ArrayG$1_$T$_._tri();
  return ArrayG$1_$T$_;
};
System_Func = function(T1, TRes, $5fcallStatiConstructor) {
  var Func$2_$T1_x_T1$_, $5f_initTracker;
  if (System_Func[T1.typeId] && System_Func[T1.typeId][TRes.typeId])
    return System_Func[T1.typeId][TRes.typeId];
    System_Func[T1.typeId] = {
    };
  System_Func[T1.typeId][TRes.typeId] = function System__Func$2a() {
  };
  Func$2_$T1_x_T1$_ = System_Func[T1.typeId][TRes.typeId];
  Func$2_$T1_x_T1$_.typeId = "bj$" + T1.typeId + "_" + TRes.typeId + "$";
  Func$2_$T1_x_T1$_.prototype = new System_MulticastDelegate();
  Func$2_$T1_x_T1$_.registerReferenceType("System.Func`2<" + T1.fullName + "," + TRes.fullName + ">", System_MulticastDelegate, []);
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
Sunlight_Framework_Observables_ObservableCollection = function(T, $5fcallStatiConstructor) {
  var List$1_$T$_, ArrayG$1_$T$_, CollectionChangedEventArgs$1_$T$_, ObservableCollection$1_$T$_, $5f_initTracker, $5f_initTrackera;
  if (Sunlight_Framework_Observables_ObservableCollection[T.typeId])
    return Sunlight_Framework_Observables_ObservableCollection[T.typeId];
  Sunlight_Framework_Observables_ObservableCollection[T.typeId] = function Sunlight__Framework__Observables__ObservableCollection$1a() {
  };
  ObservableCollection$1_$T$_ = Sunlight_Framework_Observables_ObservableCollection[T.typeId];
  ObservableCollection$1_$T$_.typeId = "bk$" + T.typeId + "$";
  ObservableCollection$1_$T$_.__ctor = function Sunlight_Framework_Observables_ObservableCollection$1_factorya() {
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
    this.items = List$1_$T$_.__ctor();
    this.__ctor();
  };
  ptyp_.add_CollectionChanged = function Sunlight__Framework__Observables__ObservableCollection$1__add_CollectionChanged(value) {
    this.collectionChanged = System_Delegate.combine(this.collectionChanged, value);
  };
  ptyp_.get_count = function Sunlight__Framework__Observables__ObservableCollection$1__get_Count() {
    return this.items.get_count();
  };
  ptyp_.add = function Sunlight__Framework__Observables__ObservableCollection$1__Add(o) {
    this.checkReentrancy();
    this.items.add(o);
    this.onCollectionChanged(0, this.get_count() - 1, ArrayG$1_$T$_.__ctora([o]), null);
    this.firePropertyChanged("Count");
  };
  ptyp_.removeRangeAt = function Sunlight__Framework__Observables__ObservableCollection$1__RemoveRangeAt(removeIndex, count) {
    var V_0, V_1;
    Sunlight_Framework_ExceptionHelpers.throwOnOutOfRange(count, 1, this.items.get_count(), "count");
    Sunlight_Framework_ExceptionHelpers.throwOnOutOfRange(removeIndex, 0, this.items.get_count() - count, "removeIndex");
    this.checkReentrancy();
    V_0 = List$1_$T$_.__ctor();
    for (V_1 = count - 1; V_1 >= 0; --V_1) {
      V_0.add(this.items.get_item(removeIndex));
      this.items.removeAt(removeIndex);
    }
    this.onCollectionChanged(1, removeIndex, null, V_0);
    this.firePropertyChanged("Count");
  };
  ptyp_.checkReentrancy = function Sunlight__Framework__Observables__ObservableCollection$1__CheckReentrancy() {
    if (this.busy)
      throw Error.createError("InvalidOperationException", null);
  };
  ptyp_.onCollectionChanged = function Sunlight__Framework__Observables__ObservableCollection$1__OnCollectionChanged(action, index, newItems, oldItems) {
    if (this.collectionChanged != null) {
      this.busy = true;
      try {
        this.collectionChanged(this, CollectionChangedEventArgs$1_$T$_.__ctor(action, index, newItems, oldItems));
      } finally {
        this.busy = false;
      }
    }
  };
  ObservableCollection$1_$T$_.registerReferenceType("Sunlight.Framework.Observables.ObservableCollection`1<" + T.fullName + ">", Sunlight_Framework_Observables_ObservableObject, [Sunlight_Framework_Observables_INotifyPropertyChanged]);
  ObservableCollection$1_$T$_._tri = function() {
    if ($5f_initTrackera)
      return;
    $5f_initTrackera = true;
    List$1_$T$_ = System_Collections_Generic_List(T, true);
    ArrayG$1_$T$_ = System_Arraya(T, true);
    CollectionChangedEventArgs$1_$T$_ = Sunlight_Framework_Observables_CollectionChangedEventArgs(T, true);
    T = T;
    ObservableCollection$1_$T$_ = Sunlight_Framework_Observables_ObservableCollection(T, true);
  };
  if ($5fcallStatiConstructor)
    ObservableCollection$1_$T$_._tri();
  return ObservableCollection$1_$T$_;
};
System_Collections_Generic_StringDictionary = function(TValue, $5fcallStatiConstructor) {
  var StringDictionary$1_$TValue$_, ICollection$1_$KeyValuePair$2_$String_x_String$_$_, KeyValuePair$2_$String_x_String$_, $5f_initTracker;
  if (System_Collections_Generic_StringDictionary[TValue.typeId])
    return System_Collections_Generic_StringDictionary[TValue.typeId];
  System_Collections_Generic_StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$1a() {
  };
  StringDictionary$1_$TValue$_ = System_Collections_Generic_StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.typeId = "bl$" + TValue.typeId + "$";
  KeyValuePair$2_$String_x_String$_ = System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor);
  ICollection$1_$KeyValuePair$2_$String_x_String$_$_ = System_Collections_Generic_ICollection(System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor), $5fcallStatiConstructor);
  KeyValuePair$2_$String_x_String$_ = System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor);
  StringDictionary$1_$TValue$_.__ctor = function System_Collections_Generic_StringDictionary$1_factorya() {
    var this_;
    this_ = new StringDictionary$1_$TValue$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = StringDictionary$1_$TValue$_.prototype;
  ptyp_.innerDict = null;
  ptyp_.count = 0;
  ptyp_.__ctor = function System__Collections__Generic__StringDictionary$1____ctor() {
    this.count = 0;
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
    ++this.count;
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
    var V_0;
    if (this.containsKey(key)) {
      value.write(this.get_item(key));
      V_0 = true;
    }
    else {
      value.write(TValue.getDefaultValue());
      V_0 = false;
    }
    return V_0;
  };
  ptyp_.adda = function System__Collections__Generic__StringDictionary$1__Add(item) {
    this.add(KeyValuePair$2_$String_x_String$_.get_key(item), KeyValuePair$2_$String_x_String$_.get_value(item));
  };
  ptyp_["V_get_Count_" + ICollection$1_$KeyValuePair$2_$String_x_String$_$_.typeId] = ptyp_.get_count;
  ptyp_["V_Add_" + ICollection$1_$KeyValuePair$2_$String_x_String$_$_.typeId] = ptyp_.adda;
  StringDictionary$1_$TValue$_.registerReferenceType("System.Collections.Generic.StringDictionary`1<" + TValue.fullName + ">", Object, [ICollection$1_$KeyValuePair$2_$String_x_String$_$_]);
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
System_Action = function(T1, T2, $5fcallStatiConstructor) {
  var Action$2_$T1_x_T1$_, $5f_initTracker;
  if (System_Action[T1.typeId] && System_Action[T1.typeId][T2.typeId])
    return System_Action[T1.typeId][T2.typeId];
    System_Action[T1.typeId] = {
    };
  System_Action[T1.typeId][T2.typeId] = function System__Action$2a() {
  };
  Action$2_$T1_x_T1$_ = System_Action[T1.typeId][T2.typeId];
  Action$2_$T1_x_T1$_.typeId = "bm$" + T1.typeId + "_" + T2.typeId + "$";
  Action$2_$T1_x_T1$_.prototype = new System_MulticastDelegate();
  Action$2_$T1_x_T1$_.registerReferenceType("System.Action`2<" + T1.fullName + "," + T2.fullName + ">", System_MulticastDelegate, []);
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
System_Collections_Generic_List = function(T, $5fcallStatiConstructor) {
  var List$1_$T$_, ICollection$1_$T$_, $5f_initTracker;
  if (System_Collections_Generic_List[T.typeId])
    return System_Collections_Generic_List[T.typeId];
  System_Collections_Generic_List[T.typeId] = function System__Collections__Generic__List$1a() {
  };
  List$1_$T$_ = System_Collections_Generic_List[T.typeId];
  List$1_$T$_.typeId = "bn$" + T.typeId + "$";
  ICollection$1_$T$_ = System_Collections_Generic_ICollection(T, $5fcallStatiConstructor);
  List$1_$T$_.__ctor = function System_Collections_Generic_List$1_factorya() {
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
    this.nativeArray.removeAt(index);
  };
  ptyp_.get_count = function System__Collections__Generic__List$1__get_Count() {
    return this.nativeArray.length;
  };
  ptyp_.add = function System__Collections__Generic__List$1__Add(item) {
    this.nativeArray.pusha(item);
  };
  ptyp_["V_get_Count_" + ICollection$1_$T$_.typeId] = ptyp_.get_count;
  ptyp_["V_Add_" + ICollection$1_$T$_.typeId] = ptyp_.add;
  List$1_$T$_.registerReferenceType("System.Collections.Generic.List`1<" + T.fullName + ">", Object, [ICollection$1_$T$_]);
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
System_Collections_Generic_ICollection = function(T, $5fcallStatiConstructor) {
  var ICollection$1_$T$_, $5f_initTracker;
  if (System_Collections_Generic_ICollection[T.typeId])
    return System_Collections_Generic_ICollection[T.typeId];
  System_Collections_Generic_ICollection[T.typeId] = function System__Collections__Generic__ICollection$1a() {
  };
  ICollection$1_$T$_ = System_Collections_Generic_ICollection[T.typeId];
  ICollection$1_$T$_.typeId = "b$" + T.typeId + "$";
  ICollection$1_$T$_.registerInterface("System.Collections.Generic.ICollection`1<" + T.fullName + ">");
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
Sunlight_Framework_Observables_CollectionChangedEventArgs = function(T, $5fcallStatiConstructor) {
  var CollectionChangedEventArgs$1_$T$_, $5f_initTracker;
  if (Sunlight_Framework_Observables_CollectionChangedEventArgs[T.typeId])
    return Sunlight_Framework_Observables_CollectionChangedEventArgs[T.typeId];
  Sunlight_Framework_Observables_CollectionChangedEventArgs[T.typeId] = function Sunlight__Framework__Observables__CollectionChangedEventArgs$1a() {
  };
  CollectionChangedEventArgs$1_$T$_ = Sunlight_Framework_Observables_CollectionChangedEventArgs[T.typeId];
  CollectionChangedEventArgs$1_$T$_.typeId = "bo$" + T.typeId + "$";
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
  CollectionChangedEventArgs$1_$T$_.registerReferenceType("Sunlight.Framework.Observables.CollectionChangedEventArgs`1<" + T.fullName + ">", Object, []);
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
System_Collections_Generic_KeyValuePair = function(K, V, $5fcallStatiConstructor) {
  var KeyValuePair$2_$K_x_K$_, $5f_initTracker;
  if (System_Collections_Generic_KeyValuePair[K.typeId] && System_Collections_Generic_KeyValuePair[K.typeId][V.typeId])
    return System_Collections_Generic_KeyValuePair[K.typeId][V.typeId];
    System_Collections_Generic_KeyValuePair[K.typeId] = {
    };
  System_Collections_Generic_KeyValuePair[K.typeId][V.typeId] = function(boxedValue) {
    this.boxedValue = boxedValue;
  };
  KeyValuePair$2_$K_x_K$_ = System_Collections_Generic_KeyValuePair[K.typeId][V.typeId];
  KeyValuePair$2_$K_x_K$_.typeId = "bp$" + K.typeId + "_" + V.typeId + "$";
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
  KeyValuePair$2_$K_x_K$_.registerStructType("System.Collections.Generic.KeyValuePair`2<" + K.fullName + "," + V.fullName + ">", []);
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
System_ArrayG_$String$_ = System_Arraya(String);
System_Func_$Object_x_Object$_ = System_Func(Object, Object);
System_ArrayG_$Func_$Object_x_Object$_$_ = System_Arraya(System_Func_$Object_x_Object$_);
Sunlight_Framework_Observables_ObservableCollection_$Int32$_ = Sunlight_Framework_Observables_ObservableCollection(System_Int32);
System_Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_ = System_Action(Sunlight_Framework_Observables_INotifyPropertyChanged, String);
System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_ = System_Collections_Generic_StringDictionary(System_Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_);
System_ArrayG_$Object$_ = System_Arraya(Object);
System_ArrayG_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_ = System_Arraya(System_Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_);
String.__cctor();
System_ArrayG_$String$_._tri();
System_Func_$Object_x_Object$_._tri();
System_ArrayG_$Func_$Object_x_Object$_$_._tri();
Sunlight_Framework_Observables_ObservableCollection_$Int32$_._tri();
System_Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_._tri();
System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_._tri();
System_ArrayG_$Object$_._tri();
System_ArrayG_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_._tri();
module("Sunlight.Framework.Test.ObservableObjectTests");
test("TestCreateNewObservableObject", 0, Sunlight_Framework_Test_ObservableObjectTests.testCreateNewObservableObject);
test("TestFirePropertyChanged", 0, Sunlight_Framework_Test_ObservableObjectTests.testFirePropertyChanged);
test("TestRemovePropertyChangeCallback", 0, Sunlight_Framework_Test_ObservableObjectTests.testRemovePropertyChangeCallback);
module("Sunlight.Framework.Test.Binders.SourcePropertyBinderTests");
test("BasicValueTest", 0, Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.basicValueTest);
test("BasicValueTestWithNotification", 0, Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.basicValueTestWithNotification);
test("PropertyPathValueNotifiableTest", 0, Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueNotifiableTest);
test("PropertyPathValueTest", 0, Sunlight_Framework_Test_Binders_SourcePropertyBinderTests.propertyPathValueTest);
module("Sunlight.Framework.Test.ObservableCollectionTests");
test("TestCreateNewObservableCollection", 0, Sunlight_Framework_Test_ObservableCollectionTests.testCreateNewObservableCollection);
test("TestAddItemToObservableCollection", 0, Sunlight_Framework_Test_ObservableCollectionTests.testAddItemToObservableCollection);
test("TestRemoveItemToObservableCollection", 0, Sunlight_Framework_Test_ObservableCollectionTests.testRemoveItemToObservableCollection);
module("Sunlight.Framework.Test.Binders.DataBinderTests");
test("BasicBinderSimpleObjectTest", 0, Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTest);
test("BasicBinderSimpleObjectTestReverseOrder", 0, Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderSimpleObjectTestReverseOrder);
test("BasicBinderOneWayNotifiableObjectTest", 0, Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderOneWayNotifiableObjectTest);
test("BasicBinderTwoWayNotifiableObjectTest", 0, Sunlight_Framework_Test_Binders_DataBinderTests.basicBinderTwoWayNotifiableObjectTest);
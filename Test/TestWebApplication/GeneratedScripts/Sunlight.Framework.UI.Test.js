(function(){
Function.typeId = "c";
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
ptyp_.isStruct = false;
ptyp_.isInterface = false;
ptyp_.isNullable = false;
ptyp_.baseType = null;
ptyp_.fullName = null;
ptyp_.typeId = null;
ptyp_.baseInterfaces = null;
ptyp_.boxedValue = null;
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
Object.typeId = "d";
System__Type__RegisterReferenceType(Object, "System.Object", null, []);
function Sunlight_Framework_UI_Test_LiveBinderTests() {
};
Sunlight_Framework_UI_Test_LiveBinderTests.typeId = "e";
Sunlight__Framework__UI__Test__LiveBinderTests__oneWayBinder = null;
Sunlight__Framework__UI__Test__LiveBinderTests__twoWayBinder = null;
Sunlight__Framework__UI__Test__LiveBinderTests__oneWayMultiBinder = null;
Sunlight__Framework__UI__Test__LiveBinderTests__twoWayMultiBinder = null;
function Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderOnActivate() {
  var src, target, liveBinder;
  src = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  target = Sunlight__Framework__UI__Test__TestViewModelA_factory();
  src.set_propStr1("test");
  liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(Sunlight__Framework__UI__Test__LiveBinderTests__oneWayBinder);
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
  liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(Sunlight__Framework__UI__Test__LiveBinderTests__oneWayBinder);
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
  liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(Sunlight__Framework__UI__Test__LiveBinderTests__oneWayMultiBinder);
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
  liveBinder = Sunlight__Framework__UI__Helpers__LiveBinder_factory(Sunlight__Framework__UI__Test__LiveBinderTests__oneWayMultiBinder);
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
function Sunlight__Framework__UI__Test__LiveBinderTests____cctor() {
  Sunlight__Framework__UI__Test__LiveBinderTests__oneWayBinder = Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propStr1();
  }])), System__NativeArray$1__op_Implicit(System_ArrayG_$String$_.__ctor(["PropStr1"])), function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del2(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propStr1(System__Type__CastType(String, val));
  }, 17, 0, 0, null, null);
  Sunlight__Framework__UI__Test__LiveBinderTests__twoWayBinder = Sunlight__Framework__UI__Helpers__SkinBinderInfo_factorya(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del3(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propStr1();
  }])), function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del4(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propStr1(System__Type__CastType(String, val));
  }, System__NativeArray$1__op_Implicit(System_ArrayG_$String$_.__ctor(["PropStr1"])), function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del5(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propStr1(System__Type__CastType(String, val));
  }, function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del6(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propStr1();
  }, "PropStr1", 17, 0, 0, null, null, null);
  Sunlight__Framework__UI__Test__LiveBinderTests__oneWayMultiBinder = Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del7(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_testVMA();
  }, function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del8(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propStr1();
  }])), System__NativeArray$1__op_Implicit(System_ArrayG_$String$_.__ctor(["TestVMA", "PropStr1"])), function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del9(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propStr1(System__Type__CastType(String, val));
  }, 17, 0, 0, null, null);
  Sunlight__Framework__UI__Test__LiveBinderTests__twoWayMultiBinder = Sunlight__Framework__UI__Helpers__SkinBinderInfo_factorya(System__NativeArray$1__op_Implicit(System_ArrayG_$Func_$Object_x_Object$_$_.__ctor([function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del10(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_testVMA();
  }, function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del11(obj) {
    return System__Type__BoxTypeInstance(System_Int32, System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propInt1());
  }])), function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del12(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propInt1(System__Type__UnBoxTypeInstance(System_Int32, val));
  }, System__NativeArray$1__op_Implicit(System_ArrayG_$String$_.__ctor(["TestVMA", "PropInt1"])), function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del13(tar, val) {
    System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, tar).set_propStr1(System__Type__CastType(String, val));
  }, function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del14(obj) {
    return System__Type__CastType(Sunlight_Framework_UI_Test_TestViewModelA, obj).get_propStr1();
  }, "PropStr1", 17, 0, 0, function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del15(obj) {
    return obj.toString();
  }, function Sunlight__Framework__UI__Test__LiveBinderTests____cctor_del16(obj) {
    return System__Type__BoxTypeInstance(System_Int32, System__Int32__Parse(System__Type__CastType(String, obj)));
  }, null);
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_LiveBinderTests, "Sunlight.Framework.UI.Test.LiveBinderTests", Object, []);
function Sunlight_Framework_UI_Test_NScriptsTemplateTests() {
};
Sunlight_Framework_UI_Test_NScriptsTemplateTests.typeId = "f";
function Sunlight__Framework__UI__Test__NScriptsTemplateTests__Test() {
  QUnit.notEqual(null, Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplate1(), "Template should not be null");
  QUnit.ok(true, "true should be true");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_NScriptsTemplateTests, "Sunlight.Framework.UI.Test.NScriptsTemplateTests", Object, []);
function Sunlight_Framework_UI_Test_ManualTemplateTests() {
};
Sunlight_Framework_UI_Test_ManualTemplateTests.typeId = "g";
Sunlight__Framework__UI__Test__ManualTemplateTests__noneValue = null;
function Sunlight__Framework__UI__Test__ManualTemplateTests__Test() {
  QUnit.ok(true, "true should be true");
};
function Sunlight__Framework__UI__Test__ManualTemplateTests____cctor() {
  Sunlight__Framework__UI__Test__ManualTemplateTests__noneValue = Sunlight_Framework_UI_Test_ValueIfTrue_$String$_.__ctor("none");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_ManualTemplateTests, "Sunlight.Framework.UI.Test.ManualTemplateTests", Object, []);
function Sunlight_Framework_UI_Test_UIElementTests() {
};
Sunlight_Framework_UI_Test_UIElementTests.typeId = "h";
function Sunlight__Framework__UI__Test__UIElementTests__TestNewUIElement() {
  var doc, element;
  doc = window.document;
  element = Sunlight__Framework__UI__UIElement_factory(doc.createElement("div"));
  QUnit.notEqual(element.get_element(), null, "element.Element != null");
  QUnit.equal(element.get_element().tagName, "DIV", "element.Element.TagName == 'DIV'");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_UIElementTests, "Sunlight.Framework.UI.Test.UIElementTests", Object, []);
function Sunlight_Framework_UI_Helpers_SkinBinderInfo() {
};
Sunlight_Framework_UI_Helpers_SkinBinderInfo.typeId = "i";
function Sunlight__Framework__UI__Helpers__SkinBinderInfo_factory(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue) {
  var this_;
  this_ = new Sunlight_Framework_UI_Helpers_SkinBinderInfo();
  this_.__ctor(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue);
  return this_;
};
function Sunlight__Framework__UI__Helpers__SkinBinderInfo_factorya(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue) {
  var this_;
  this_ = new Sunlight_Framework_UI_Helpers_SkinBinderInfo();
  this_.__ctora(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue);
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
ptyp_.__ctor = function Sunlight__Framework__UI__Helpers__SkinBinderInfo____ctor(propertyGetterPath, propertyNames, targetPropertySetter, binderType, objectIndex, binderIndex, forwardConverter, defaultValue) {
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
ptyp_.__ctora = function Sunlight__Framework__UI__Helpers__SkinBinderInfo____ctora(propertyGetterPath, propertySetter, propertyNames, targetPropertySetter, targetPropertyGetter, targetPropertyName, binderType, objectIndex, binderIndex, forwardConverter, backwardConverter, defaultValue) {
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
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Helpers_SkinBinderInfo, "Sunlight.Framework.UI.Helpers.SkinBinderInfo", Object, []);
function Sunlight_Framework_Observables_INotifyPropertyChanged() {
};
Sunlight_Framework_Observables_INotifyPropertyChanged.typeId = "b";
System__Type__RegisterInterface(Sunlight_Framework_Observables_INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function Sunlight_Framework_Observables_ObservableObject() {
};
Sunlight_Framework_Observables_ObservableObject.typeId = "j";
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
ptyp_.V_AddPropertyChangedListener_b = ptyp_.addPropertyChangedListener;
ptyp_.V_RemovePropertyChangedListener_b = ptyp_.removePropertyChangedListener;
System__Type__RegisterReferenceType(Sunlight_Framework_Observables_ObservableObject, "Sunlight.Framework.Observables.ObservableObject", Object, [Sunlight_Framework_Observables_INotifyPropertyChanged]);
function Sunlight_Framework_UI_Test_TestViewModelA() {
};
Sunlight_Framework_UI_Test_TestViewModelA.typeId = "k";
function Sunlight__Framework__UI__Test__TestViewModelA_factory() {
  var this_;
  this_ = new Sunlight_Framework_UI_Test_TestViewModelA();
  this_.__ctora();
  return this_;
};
Sunlight_Framework_UI_Test_TestViewModelA.defaultConstructor = Sunlight__Framework__UI__Test__TestViewModelA_factory;
ptyp_ = new Sunlight_Framework_Observables_ObservableObject();
Sunlight_Framework_UI_Test_TestViewModelA.prototype = ptyp_;
ptyp_.str1 = null;
ptyp_.testVMA = null;
ptyp_.get_propInt1 = function Sunlight__Framework__UI__Test__TestViewModelA__get_PropInt1() {
  return this.get_int1();
};
ptyp_.set_propInt1 = function Sunlight__Framework__UI__Test__TestViewModelA__set_PropInt1(value) {
  if (this.get_int1() !== value) {
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
ptyp_.get_testVMA = function Sunlight__Framework__UI__Test__TestViewModelA__get_TestVMA() {
  return this.testVMA;
};
ptyp_.set_testVMA = function Sunlight__Framework__UI__Test__TestViewModelA__set_TestVMA(value) {
  if (this.testVMA !== value) {
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
ptyp_.__ctora = function Sunlight__Framework__UI__Test__TestViewModelA____ctor() {
  this.__ctor();
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_TestViewModelA, "Sunlight.Framework.UI.Test.TestViewModelA", Sunlight_Framework_Observables_ObservableObject, []);
String.typeId = "l";
System__String__formatHelperRegex = null;
System__String__trimStartHelperRegex = null;
System__String__trimEndHelperRegex = null;
function System__String____cctor() {
  System__String__formatHelperRegex = new RegExp("(\\{[^\\}^\\{]+\\})", "g");
  System__String__trimStartHelperRegex = new RegExp("^[\\s\\xA0]+");
  System__String__trimEndHelperRegex = new RegExp("[\\s\\xA0]+$");
};
System__Type__RegisterReferenceType(String, "System.String", Object, []);
function System_ValueType() {
};
System_ValueType.typeId = "m";
ptyp_ = System_ValueType.prototype;
ptyp_.boxedValue = null;
System__Type__RegisterReferenceType(System_ValueType, "System.ValueType", Object, []);
function System_Int32(boxedValue) {
  this.boxedValue = boxedValue;
};
System_Int32.typeId = "n";
System_Int32.getDefaultValue = function() {
  return 0;
};
function System__Int32__Parse(s) {
  return parseInt(s);
};
function System__Int32__ToString(this_, radix) {
  return this_.toString(radix);
};
function System__Int32__ToStringa(this_) {
  return System__Int32__ToString(this_, 10);
};
ptyp_ = new System_ValueType();
System_Int32.prototype = ptyp_;
ptyp_.toString = function() {
  return System__Int32__ToStringa(this.boxedValue);
};
System__Type__RegisterStructType(System_Int32, "System.Int32", []);
function System_ArrayImpl() {
};
System_ArrayImpl.typeId = "o";
ptyp_ = System_ArrayImpl.prototype;
ptyp_.__ctor = function System__ArrayImpl____ctor() {
};
System__Type__RegisterReferenceType(System_ArrayImpl, "System.ArrayImpl", Object, []);
RegExp.typeId = "p";
System__Type__RegisterReferenceType(RegExp, "System.RegularExpression", Object, []);
function System_Delegate() {
};
System_Delegate.typeId = "q";
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
System__Type__RegisterReferenceType(System_Delegate, "System.Delegate", Object, []);
function System_MulticastDelegate() {
};
System_MulticastDelegate.typeId = "r";
System_MulticastDelegate.prototype = new System_Delegate();
System__Type__RegisterReferenceType(System_MulticastDelegate, "System.MulticastDelegate", System_Delegate, []);
function Sunlight_Framework_UI_Helpers_LiveBinder() {
};
Sunlight_Framework_UI_Helpers_LiveBinder.typeId = "s";
function Sunlight__Framework__UI__Helpers__LiveBinder_factory(binderInfo) {
  var this_;
  this_ = new Sunlight_Framework_UI_Helpers_LiveBinder();
  this_.__ctor(binderInfo);
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
ptyp_.__ctor = function Sunlight__Framework__UI__Helpers__LiveBinder____ctor(binderInfo) {
  this.binderInfo = binderInfo;
};
ptyp_.set_source = function Sunlight__Framework__UI__Helpers__LiveBinder__set_Source(value) {
  if (this.source !== value) {
    this.source = value;
    this.flowValue();
  }
};
ptyp_.set_target = function Sunlight__Framework__UI__Helpers__LiveBinder__set_Target(value) {
  if (this.target !== value) {
    if (this.target !== null && this.binderInfo.mode === 2)
      System__Type__CastType(Sunlight_Framework_Observables_INotifyPropertyChanged, this.target).V_AddPropertyChangedListener_b(this.binderInfo.targetPropertyName, System__Delegate__Create("onTargetPropertyChanged", this));
    this.target = value;
    this.flowValue();
  }
};
ptyp_.set_isActive = function Sunlight__Framework__UI__Helpers__LiveBinder__set_IsActive(value) {
  if (this.isActive !== value) {
    this.isActive = value;
    if (this.isActive)
      this.activate();
    else
      this.deactivate();
  }
};
ptyp_.activate = function Sunlight__Framework__UI__Helpers__LiveBinder__Activate() {
  this.flowValue();
};
ptyp_.deactivate = function Sunlight__Framework__UI__Helpers__LiveBinder__Deactivate() {
  this.isActive = false;
  Sunlight__Framework__TaskScheduler__get_Instance().enqueueLowPriTask(System__Delegate__Create("deactivateLater", this), "LiveBinder.DeactivateLater");
};
ptyp_.flowValue = function Sunlight__Framework__UI__Helpers__LiveBinder__FlowValue() {
  if (this.target === null || this.updating || !this.isActive)
    return;
  if (this.liveObjects === null)
    this.liveObjects = System_ArrayG_$Object$_.__ctora(this.binderInfo.propertyGetterPath.length + 1);
  if (this.liveObjects.get_item(0) !== this.source) {
    if (this.liveObjects.get_item(0) !== null) {
      this.pathTraversed = 0;
      this.cleanRegistrations();
    }
    this.liveObjects.set_item(0, this.source);
    if (this.liveObjects.get_item(0) !== null)
      System__Type__CastType(Sunlight_Framework_Observables_INotifyPropertyChanged, this.liveObjects.get_item(0)).V_AddPropertyChangedListener_b(this.binderInfo.propertyNames[0], System__Delegate__Create("onSourcePropertyChanged", this));
  }
  this.setTargetProperty(this.getValue());
};
ptyp_.setTargetProperty = function Sunlight__Framework__UI__Helpers__LiveBinder__SetTargetProperty(value) {
  var binderInfo, target;
  try {
    this.updating = true;
    binderInfo = this.binderInfo;
    target = this.target;
    if (binderInfo.targetPropertySetter !== null)
      binderInfo.targetPropertySetter(target, value);
    else
      binderInfo.targetPropertySetterWithArg(target, value, binderInfo.targetPropertySetterArg);
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
  var binderInfo, path, liveObjects, src, iPath, pathLength, propertyGetterPath, propertyNames;
  binderInfo = this.binderInfo;
  path = binderInfo.propertyGetterPath;
  liveObjects = this.liveObjects;
  src = liveObjects.get_item(0);
  iPath = 1, pathLength = binderInfo.propertyGetterPath.length + 1;
  propertyGetterPath = binderInfo.propertyGetterPath;
  propertyNames = binderInfo.propertyNames;
  this.pathTraversed = 1;
  for (; iPath < pathLength; iPath++)
    if (src !== null || iPath === 1 && (binderInfo.binderType & 2) === 2) {
      src = propertyGetterPath[iPath - 1](src);
      if (liveObjects.get_item(iPath) !== src) {
        if (liveObjects.get_item(iPath) !== null && iPath < pathLength - 1)
          System__Type__CastType(Sunlight_Framework_Observables_INotifyPropertyChanged, liveObjects.get_item(iPath)).V_RemovePropertyChangedListener_b(binderInfo.propertyNames[iPath], System__Delegate__Create("onSourcePropertyChanged", this));
        liveObjects.set_item(iPath, src);
        if (src !== null && iPath < pathLength - 1)
          System__Type__CastType(Sunlight_Framework_Observables_INotifyPropertyChanged, src).V_AddPropertyChangedListener_b(binderInfo.propertyNames[iPath], System__Delegate__Create("onSourcePropertyChanged", this));
      }
      ++this.pathTraversed;
    }
  if (this.pathTraversed < pathLength)
    return binderInfo.defaultValue;
  else if (binderInfo.forwardConverter !== null)
    return binderInfo.forwardConverter(src);
  else
    return src;
};
ptyp_.cleanRegistrations = function Sunlight__Framework__UI__Helpers__LiveBinder__CleanRegistrations() {
  var liveObjects, iPath, till;
  liveObjects = this.liveObjects;
  if (this.pathTraversed < this.liveObjects.V_get_Length()) {
    liveObjects.set_item(liveObjects.V_get_Length() - 1, null);
    for (
    iPath = this.binderInfo.propertyGetterPath.length - 2, till = this.pathTraversed; iPath >= till; iPath--) {
      System__Type__CastType(Sunlight_Framework_Observables_INotifyPropertyChanged, liveObjects.get_item(iPath)).V_RemovePropertyChangedListener_b(this.binderInfo.propertyNames[iPath], System__Delegate__Create("onSourcePropertyChanged", this));
      liveObjects.set_item(iPath, null);
    }
  }
};
ptyp_.deactivateLater = function Sunlight__Framework__UI__Helpers__LiveBinder__DeactivateLater() {
  if (!this.isActive) {
    this.pathTraversed = 0;
    this.cleanRegistrations();
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
    if (target === obj && this.source !== null && (liveObjects.V_get_Length() < 2 || liveObjects.get_item(liveObjects.V_get_Length() - 2) !== null)) {
      value = binderInfo.targetPropertyGetter(target);
      if (binderInfo.backwardConverter !== null)
        value = binderInfo.backwardConverter(value);
      binderInfo.propertySetter(this.liveObjects.V_get_Length() < 2 ? this.source : this.liveObjects.get_item(this.liveObjects.V_get_Length() - 2), value);
    }
  } finally {
    this.updating = false;
  }
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Helpers_LiveBinder, "Sunlight.Framework.UI.Helpers.LiveBinder", Object, []);
function Sunlight_Framework_UI_Test_NScriptsTemplatesClass() {
};
Sunlight_Framework_UI_Test_NScriptsTemplatesClass.typeId = "t";
function Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplate1() {
  return null;
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_NScriptsTemplatesClass, "Sunlight.Framework.UI.Test.NScriptsTemplatesClass", Object, []);
function Sunlight_Framework_Observables_ExtensibleObservableObject() {
};
Sunlight_Framework_Observables_ExtensibleObservableObject.typeId = "u";
function Sunlight__Framework__Observables__ExtensibleObservableObject_factory() {
  var this_;
  this_ = new Sunlight_Framework_Observables_ExtensibleObservableObject();
  this_.__ctora();
  return this_;
};
Sunlight_Framework_Observables_ExtensibleObservableObject.defaultConstructor = Sunlight__Framework__Observables__ExtensibleObservableObject_factory;
ptyp_ = new Sunlight_Framework_Observables_ObservableObject();
Sunlight_Framework_Observables_ExtensibleObservableObject.prototype = ptyp_;
ptyp_.propertyMap = null;
ptyp_.__ctora = function Sunlight__Framework__Observables__ExtensibleObservableObject____ctor() {
  this.__ctor();
  this.propertyMap = {
  };
};
System__Type__RegisterReferenceType(Sunlight_Framework_Observables_ExtensibleObservableObject, "Sunlight.Framework.Observables.ExtensibleObservableObject", Sunlight_Framework_Observables_ObservableObject, []);
function Sunlight_Framework_Binders_ContextBindableObject() {
};
Sunlight_Framework_Binders_ContextBindableObject.typeId = "v";
function Sunlight__Framework__Binders__ContextBindableObject_factory() {
  var this_;
  this_ = new Sunlight_Framework_Binders_ContextBindableObject();
  this_.__ctorb();
  return this_;
};
Sunlight_Framework_Binders_ContextBindableObject.defaultConstructor = Sunlight__Framework__Binders__ContextBindableObject_factory;
ptyp_ = new Sunlight_Framework_Observables_ExtensibleObservableObject();
Sunlight_Framework_Binders_ContextBindableObject.prototype = ptyp_;
ptyp_.isInactiveIfNullContext = false;
ptyp_.__ctorb = function Sunlight__Framework__Binders__ContextBindableObject____ctor() {
  this.__ctora();
  this.isInactiveIfNullContext = true;
};
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_ContextBindableObject, "Sunlight.Framework.Binders.ContextBindableObject", Sunlight_Framework_Observables_ExtensibleObservableObject, []);
function Sunlight_Framework_UI_UIElement() {
};
Sunlight_Framework_UI_UIElement.typeId = "w";
function Sunlight__Framework__UI__UIElement_factory(element) {
  var this_;
  this_ = new Sunlight_Framework_UI_UIElement();
  this_.__ctorc(element);
  return this_;
};
ptyp_ = new Sunlight_Framework_Binders_ContextBindableObject();
Sunlight_Framework_UI_UIElement.prototype = ptyp_;
ptyp_.element = null;
ptyp_.eventRegistrationDict = null;
ptyp_.__ctorc = function Sunlight__Framework__UI__UIElement____ctor(element) {
  this.__ctorb();
  this.eventRegistrationDict = System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_.defaultConstructor();
  this.element = element;
};
ptyp_.get_element = function Sunlight__Framework__UI__UIElement__get_Element() {
  return this.element;
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_UIElement, "Sunlight.Framework.UI.UIElement", Sunlight_Framework_Binders_ContextBindableObject, []);
function Sunlight_Framework_UI_UIEvent() {
};
Sunlight_Framework_UI_UIEvent.typeId = "x";
System__Type__RegisterReferenceType(Sunlight_Framework_UI_UIEvent, "Sunlight.Framework.UI.UIEvent", Object, []);
function Sunlight_Framework_TaskScheduler() {
};
Sunlight_Framework_TaskScheduler.typeId = "y";
Sunlight__Framework__TaskScheduler__instance = null;
function Sunlight__Framework__TaskScheduler__get_Instance() {
  if (Sunlight__Framework__TaskScheduler__instance !== null)
    Sunlight__Framework__TaskScheduler__instance = Sunlight__Framework__TaskScheduler_factory(16, 25);
  return Sunlight__Framework__TaskScheduler__instance;
};
function Sunlight__Framework__TaskScheduler_factory(workQuanta, idleTimeOut) {
  var this_;
  this_ = new Sunlight_Framework_TaskScheduler();
  this_.__ctor(workQuanta, idleTimeOut);
  return this_;
};
ptyp_ = Sunlight_Framework_TaskScheduler.prototype;
ptyp_.__ctor = function Sunlight__Framework__TaskScheduler____ctor(workQuanta, idleTimeOut) {
};
ptyp_.enqueueLowPriTask = function Sunlight__Framework__TaskScheduler__EnqueueLowPriTask(work, traceId) {
  return null;
};
System__Type__RegisterReferenceType(Sunlight_Framework_TaskScheduler, "Sunlight.Framework.TaskScheduler", Object, []);
Error.typeId = "z";
System__Type__RegisterReferenceType(Error, "System.Exception", Object, []);
Function.getDefaultValue = function() {
  return {
  };
};
function System_ArrayG(T, $5fcallStatiConstructor) {
  var ArrayG$1_$T$_, $5f_initTracker;
  if (System_ArrayG[T.typeId])
    return System_ArrayG[T.typeId];
  System_ArrayG[T.typeId] = function System__ArrayG$1a() {
  };
  ArrayG$1_$T$_ = System_ArrayG[T.typeId];
  ArrayG$1_$T$_.genericParameters = [T];
  ArrayG$1_$T$_.genericClosure = System_ArrayG;
  ArrayG$1_$T$_.typeId = "A$" + T.typeId + "$";
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
    def = System__Type__GetDefaultValueStatic(T);
    for (i = 0; i < size; i++)
      this.innerArray[i] = def;
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
  ptyp_.get_innerArray = function System__ArrayG$1__get_InnerArray() {
    return this.innerArray;
  };
  ptyp_.V_get_Length = ptyp_.get_length;
  System__Type__RegisterReferenceType(ArrayG$1_$T$_, "System.ArrayG`1<" + T.fullName + ">", System_ArrayImpl, []);
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
function System__NativeArray$1__op_Implicit(n) {
  return n.get_innerArray();
};
Object.typeId = "d";
System__Type__RegisterReferenceType(Object, "System.Object", null, []);
function Sunlight_Framework_UI_Test_ValueIfTrue(T, $5fcallStatiConstructor) {
  var ValueIfTrue$1_$T$_, $5f_initTracker;
  if (Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId])
    return Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId];
  Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId] = function Sunlight__Framework__UI__Test__ValueIfTrue$1a() {
  };
  ValueIfTrue$1_$T$_ = Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId];
  ValueIfTrue$1_$T$_.genericParameters = [T];
  ValueIfTrue$1_$T$_.genericClosure = Sunlight_Framework_UI_Test_ValueIfTrue;
  ValueIfTrue$1_$T$_.typeId = "bb$" + T.typeId + "$";
  ValueIfTrue$1_$T$_.__ctor = function Sunlight_Framework_UI_Test_ValueIfTrue$1_factorya(value) {
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
  ValueIfTrue$1_$T$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T = T;
    ValueIfTrue$1_$T$_ = Sunlight_Framework_UI_Test_ValueIfTrue(T, true);
  };
  if ($5fcallStatiConstructor)
    ValueIfTrue$1_$T$_._tri();
  return ValueIfTrue$1_$T$_;
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
  Func$2_$T1_x_T1$_.typeId = "bc$" + T1.typeId + "_" + TRes.typeId + "$";
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
function Sunlight_Framework_Observables_INotifyPropertyChanged() {
};
Sunlight_Framework_Observables_INotifyPropertyChanged.typeId = "b";
System__Type__RegisterInterface(Sunlight_Framework_Observables_INotifyPropertyChanged, "Sunlight.Framework.Observables.INotifyPropertyChanged");
function System_Collections_Generic_StringDictionary(TValue, $5fcallStatiConstructor) {
  var StringDictionary$1_$TValue$_, $5f_initTracker;
  if (System_Collections_Generic_StringDictionary[TValue.typeId])
    return System_Collections_Generic_StringDictionary[TValue.typeId];
  System_Collections_Generic_StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$1a() {
  };
  StringDictionary$1_$TValue$_ = System_Collections_Generic_StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = System_Collections_Generic_StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "bd$" + TValue.typeId + "$";
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
    this.innerDict = {
    };
  };
  ptyp_.get_item = function System__Collections__Generic__StringDictionary$1__get_Item(index) {
    if (!(index in this.innerDict))
      throw new Error("Key not found");
    return this.innerDict[index];
  };
  ptyp_.set_item = function System__Collections__Generic__StringDictionary$1__set_Item(index, value) {
    this.innerDict[index] = value;
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
  System__Type__RegisterReferenceType(StringDictionary$1_$TValue$_, "System.Collections.Generic.StringDictionary`1<" + TValue.fullName + ">", Object, []);
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
function System_Action(T1, $5fcallStatiConstructor) {
  var Action$1_$T1$_, $5f_initTracker;
  if (System_Action[T1.typeId])
    return System_Action[T1.typeId];
  System_Action[T1.typeId] = function System__Action$1a() {
  };
  Action$1_$T1$_ = System_Action[T1.typeId];
  Action$1_$T1$_.genericParameters = [T1];
  Action$1_$T1$_.genericClosure = System_Action;
  Action$1_$T1$_.typeId = "be$" + T1.typeId + "$";
  Action$1_$T1$_.prototype = new System_MulticastDelegate();
  System__Type__RegisterReferenceType(Action$1_$T1$_, "System.Action`1<" + T1.fullName + ">", System_MulticastDelegate, []);
  Action$1_$T1$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T1 = T1;
    Action$1_$T1$_ = System_Action(T1, true);
  };
  if ($5fcallStatiConstructor)
    Action$1_$T1$_._tri();
  return Action$1_$T1$_;
};
function System_Actiona(T1, T2, $5fcallStatiConstructor) {
  var Action$2_$T1_x_T1$_, $5f_initTracker;
  if (System_Actiona[T1.typeId] && System_Actiona[T1.typeId][T2.typeId])
    return System_Actiona[T1.typeId][T2.typeId];
    System_Actiona[T1.typeId] = {
    };
  System_Actiona[T1.typeId][T2.typeId] = function System__Action$2a() {
  };
  Action$2_$T1_x_T1$_ = System_Actiona[T1.typeId][T2.typeId];
  Action$2_$T1_x_T1$_.genericParameters = [T1, T2];
  Action$2_$T1_x_T1$_.genericClosure = System_Actiona;
  Action$2_$T1_x_T1$_.typeId = "bf$" + T1.typeId + "_" + T2.typeId + "$";
  Action$2_$T1_x_T1$_.prototype = new System_MulticastDelegate();
  System__Type__RegisterReferenceType(Action$2_$T1_x_T1$_, "System.Action`2<" + T1.fullName + "," + T2.fullName + ">", System_MulticastDelegate, []);
  Action$2_$T1_x_T1$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    T1 = T1;
    T2 = T2;
    Action$2_$T1_x_T1$_ = System_Actiona(T1, T2, true);
  };
  if ($5fcallStatiConstructor)
    Action$2_$T1_x_T1$_._tri();
  return Action$2_$T1_x_T1$_;
};
System_Func_$Object_x_Object$_ = System_Func(Object, Object);
System_ArrayG_$Func_$Object_x_Object$_$_ = System_ArrayG(System_Func_$Object_x_Object$_);
System_ArrayG_$String$_ = System_ArrayG(String);
Sunlight_Framework_UI_Test_ValueIfTrue_$String$_ = Sunlight_Framework_UI_Test_ValueIfTrue(String);
System_Action_$UIEvent$_ = System_Action(Sunlight_Framework_UI_UIEvent);
System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_ = System_Collections_Generic_StringDictionary(System_Action_$UIEvent$_);
System_ArrayG_$Object$_ = System_ArrayG(Object);
System_Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_ = System_Actiona(Sunlight_Framework_Observables_INotifyPropertyChanged, String);
System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_ = System_Collections_Generic_StringDictionary(System_Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_);
Sunlight__Framework__UI__Test__LiveBinderTests____cctor();
Sunlight__Framework__UI__Test__ManualTemplateTests____cctor();
System__String____cctor();
System_Func_$Object_x_Object$_._tri();
System_ArrayG_$Func_$Object_x_Object$_$_._tri();
System_ArrayG_$String$_._tri();
Sunlight_Framework_UI_Test_ValueIfTrue_$String$_._tri();
System_Action_$UIEvent$_._tri();
System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_._tri();
System_ArrayG_$Object$_._tri();
System_Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_._tri();
System_Collections_Generic_StringDictionary_$Action_$INotifyPropertyChanged_x_INotifyPropertyChanged$_$_._tri();
module("Sunlight.Framework.UI.Test.LiveBinderTests");
test("TestLiveBinderOnActivate", 0, Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderOnActivate);
test("TestLiveBinderOnChange", 0, Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderOnChange);
test("TestLiveBinderMultiOnActivate", 0, Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderMultiOnActivate);
test("TestLiveBinderMultiOnChange", 0, Sunlight__Framework__UI__Test__LiveBinderTests__TestLiveBinderMultiOnChange);
module("Sunlight.Framework.UI.Test.NScriptsTemplateTests");
test("Test", 0, Sunlight__Framework__UI__Test__NScriptsTemplateTests__Test);
module("Sunlight.Framework.UI.Test.ManualTemplateTests");
test("Test", 0, Sunlight__Framework__UI__Test__ManualTemplateTests__Test);
module("Sunlight.Framework.UI.Test.UIElementTests");
test("TestNewUIElement", 0, Sunlight__Framework__UI__Test__UIElementTests__TestNewUIElement);
tmplStore = [];
function DocStorageGetter(doc) {
  var style;
  if (!doc.stateStore) {
    doc.stateStore = [];
    style = doc.createElement("style");
    style.textContent = "";
    doc.body.appendChild(style);
  }
  return doc.stateStore;
};
//@ sourceMappingURL=Sunlight.Framework.UI.Test.map
})();
(function(){
Function.typeId = "b";
System__Type__typeMapping = null;
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
function System__Type__GetDefaultValueStatic(type) {
  if (type.isStruct)
    return type.getDefaultValue();
  return null;
};
ptyp_ = Function.prototype;
ptyp_.isClass = false;
ptyp_.isStruct = false;
ptyp_.baseType = null;
ptyp_.fullName = null;
ptyp_.typeId = null;
ptyp_.interfaces = null;
ptyp_.defaultConstructor = function System__Type__DefaultConstructor() {
  if (this.isStruct)
    return this.getDefaultValue;
  throw "Default constructor not implemented";
};
ptyp_.getDefaultValue = function System__Type__GetDefaultValue() {
  return null;
};
System__Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "c";
System__Type__RegisterReferenceType(Object, "System.Object", null, []);
function Sunlight_Framework_UI_Test_NScriptsTemplateTests() {
};
Sunlight_Framework_UI_Test_NScriptsTemplateTests.typeId = "d";
function Sunlight__Framework__UI__Test__NScriptsTemplateTests__Test() {
  QUnit.notEqual(null, Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplate1(), "Template should not be null");
  QUnit.ok(true, "true should be true");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_NScriptsTemplateTests, "Sunlight.Framework.UI.Test.NScriptsTemplateTests", Object, []);
function Sunlight_Framework_UI_Test_ManualTemplateTests() {
};
Sunlight_Framework_UI_Test_ManualTemplateTests.typeId = "e";
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
Sunlight_Framework_UI_Test_UIElementTests.typeId = "f";
function Sunlight__Framework__UI__Test__UIElementTests__TestNewUIElement() {
  var doc, element;
  doc = window.document;
  element = Sunlight__Framework__UI__UIElement_factory(doc.createElement("div"));
  QUnit.notEqual(element.get_element(), null, "element.Element != null");
  QUnit.equal(element.get_element().tagName, "DIV", "element.Element.TagName == 'DIV'");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_UIElementTests, "Sunlight.Framework.UI.Test.UIElementTests", Object, []);
String.typeId = "g";
System__String__formatHelperRegex = null;
System__String__trimStartHelperRegex = null;
System__String__trimEndHelperRegex = null;
function System__String____cctor() {
  System__String__formatHelperRegex = new RegExp("(\\{[^\\}^\\{]+\\})", "g");
  System__String__trimStartHelperRegex = new RegExp("^[\\s\\xA0]+");
  System__String__trimEndHelperRegex = new RegExp("[\\s\\xA0]+$");
};
System__Type__RegisterReferenceType(String, "System.String", Object, []);
RegExp.typeId = "h";
System__Type__RegisterReferenceType(RegExp, "System.RegularExpression", Object, []);
function Sunlight_Framework_UI_Test_NScriptsTemplatesClass() {
};
Sunlight_Framework_UI_Test_NScriptsTemplatesClass.typeId = "i";
function Sunlight__Framework__UI__Test__NScriptsTemplatesClass__get_TestTemplate1() {
  return null;
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_NScriptsTemplatesClass, "Sunlight.Framework.UI.Test.NScriptsTemplatesClass", Object, []);
function Sunlight_Framework_Observables_ObservableObject() {
};
Sunlight_Framework_Observables_ObservableObject.typeId = "j";
ptyp_ = Sunlight_Framework_Observables_ObservableObject.prototype;
ptyp_.__ctor = function Sunlight__Framework__Observables__ObservableObject____ctor() {
};
System__Type__RegisterReferenceType(Sunlight_Framework_Observables_ObservableObject, "Sunlight.Framework.Observables.ObservableObject", Object, []);
function Sunlight_Framework_Observables_ExtensibleObservableObject() {
};
Sunlight_Framework_Observables_ExtensibleObservableObject.typeId = "k";
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
Sunlight_Framework_Binders_ContextBindableObject.typeId = "l";
function Sunlight__Framework__Binders__ContextBindableObject_factory() {
  var this_;
  this_ = new Sunlight_Framework_Binders_ContextBindableObject();
  this_.__ctorb();
  return this_;
};
Sunlight_Framework_Binders_ContextBindableObject.defaultConstructor = Sunlight__Framework__Binders__ContextBindableObject_factory;
ptyp_ = new Sunlight_Framework_Observables_ExtensibleObservableObject();
Sunlight_Framework_Binders_ContextBindableObject.prototype = ptyp_;
ptyp_.oneTimeBinders = null;
ptyp_.oneTimeParentBinders = null;
ptyp_.liveDataBinders = null;
ptyp_.liveContextParentBinder = null;
ptyp_.propertyDict = null;
ptyp_.isInactiveIfNullContext = false;
ptyp_.__ctorb = function Sunlight__Framework__Binders__ContextBindableObject____ctor() {
  this.__ctora();
  this.oneTimeBinders = System_Collections_Generic_List_$OneTimeDataBinder$_.defaultConstructor();
  this.oneTimeParentBinders = System_Collections_Generic_List_$OneTimeDataBinder$_.defaultConstructor();
  this.liveDataBinders = System_Collections_Generic_List_$DataBinder$_.defaultConstructor();
  this.liveContextParentBinder = System_Collections_Generic_List_$DataBinder$_.defaultConstructor();
  this.propertyDict = {
  };
  this.isInactiveIfNullContext = true;
};
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_ContextBindableObject, "Sunlight.Framework.Binders.ContextBindableObject", Sunlight_Framework_Observables_ExtensibleObservableObject, []);
function Sunlight_Framework_UI_UIElement() {
};
Sunlight_Framework_UI_UIElement.typeId = "m";
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
Sunlight_Framework_UI_UIEvent.typeId = "n";
System__Type__RegisterReferenceType(Sunlight_Framework_UI_UIEvent, "Sunlight.Framework.UI.UIEvent", Object, []);
function System_Delegate() {
};
System_Delegate.typeId = "o";
System__Type__RegisterReferenceType(System_Delegate, "System.Delegate", Object, []);
function System_MulticastDelegate() {
};
System_MulticastDelegate.typeId = "p";
System_MulticastDelegate.prototype = new System_Delegate();
System__Type__RegisterReferenceType(System_MulticastDelegate, "System.MulticastDelegate", System_Delegate, []);
function Sunlight_Framework_Binders_OneTimeDataBinder() {
};
Sunlight_Framework_Binders_OneTimeDataBinder.typeId = "q";
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_OneTimeDataBinder, "Sunlight.Framework.Binders.OneTimeDataBinder", Object, []);
function Sunlight_Framework_Binders_DataBinder() {
};
Sunlight_Framework_Binders_DataBinder.typeId = "r";
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_DataBinder, "Sunlight.Framework.Binders.DataBinder", Object, []);
function Sunlight_Framework_UI_Test_ValueIfTrue(T, $5fcallStatiConstructor) {
  var ValueIfTrue$1_$T$_, $5f_initTracker;
  if (Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId])
    return Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId];
  Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId] = function Sunlight__Framework__UI__Test__ValueIfTrue$1a() {
  };
  ValueIfTrue$1_$T$_ = Sunlight_Framework_UI_Test_ValueIfTrue[T.typeId];
  ValueIfTrue$1_$T$_.genericParameters = [T];
  ValueIfTrue$1_$T$_.genericClosure = Sunlight_Framework_UI_Test_ValueIfTrue;
  ValueIfTrue$1_$T$_.typeId = "s$" + T.typeId + "$";
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
function System_Collections_Generic_StringDictionary(TValue, $5fcallStatiConstructor) {
  var StringDictionary$1_$TValue$_, $5f_initTracker;
  if (System_Collections_Generic_StringDictionary[TValue.typeId])
    return System_Collections_Generic_StringDictionary[TValue.typeId];
  System_Collections_Generic_StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$1a() {
  };
  StringDictionary$1_$TValue$_ = System_Collections_Generic_StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = System_Collections_Generic_StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "t$" + TValue.typeId + "$";
  StringDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_StringDictionary$1_factorya() {
    var this_;
    this_ = new StringDictionary$1_$TValue$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = StringDictionary$1_$TValue$_.prototype;
  ptyp_.innerDict = null;
  ptyp_.__ctor = function System__Collections__Generic__StringDictionary$1____ctor() {
    this.innerDict = {
    };
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
  Action$1_$T1$_.typeId = "u$" + T1.typeId + "$";
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
function System_Collections_Generic_List(T, $5fcallStatiConstructor) {
  var List$1_$T$_, $5f_initTracker;
  if (System_Collections_Generic_List[T.typeId])
    return System_Collections_Generic_List[T.typeId];
  System_Collections_Generic_List[T.typeId] = function System__Collections__Generic__List$1a() {
  };
  List$1_$T$_ = System_Collections_Generic_List[T.typeId];
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = System_Collections_Generic_List;
  List$1_$T$_.typeId = "v$" + T.typeId + "$";
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
  System__Type__RegisterReferenceType(List$1_$T$_, "System.Collections.Generic.List`1<" + T.fullName + ">", Object, []);
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
Sunlight_Framework_UI_Test_ValueIfTrue_$String$_ = Sunlight_Framework_UI_Test_ValueIfTrue(String);
System_Action_$UIEvent$_ = System_Action(Sunlight_Framework_UI_UIEvent);
System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_ = System_Collections_Generic_StringDictionary(System_Action_$UIEvent$_);
System_Collections_Generic_List_$OneTimeDataBinder$_ = System_Collections_Generic_List(Sunlight_Framework_Binders_OneTimeDataBinder);
System_Collections_Generic_List_$DataBinder$_ = System_Collections_Generic_List(Sunlight_Framework_Binders_DataBinder);
Sunlight__Framework__UI__Test__ManualTemplateTests____cctor();
System__String____cctor();
Sunlight_Framework_UI_Test_ValueIfTrue_$String$_._tri();
System_Action_$UIEvent$_._tri();
System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_._tri();
System_Collections_Generic_List_$OneTimeDataBinder$_._tri();
System_Collections_Generic_List_$DataBinder$_._tri();
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
    style = doc.body.appendChild(style);
  }
  return doc.stateStore;
};
//@ sourceMappingURL=Sunlight.Framework.UI.Test.map
})();
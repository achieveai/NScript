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
function Sunlight_Framework_UI_Test_UIElementTests() {
};
Sunlight_Framework_UI_Test_UIElementTests.typeId = "d";
function Sunlight__Framework__UI__Test__UIElementTests__TestNewUIElement() {
  var doc, element;
  doc = window.document;
  element = Sunlight__Framework__UI__UIElement_factory(doc.createElement("div"));
  QUnit.notEqual(element.get_element(), null, "element.Element != null");
  QUnit.equal(element.get_element().tagName, "DIV", "element.Element.TagName == 'DIV'");
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_UIElementTests, "Sunlight.Framework.UI.Test.UIElementTests", Object, []);
function Sunlight_Framework_Observables_ObservableObject() {
};
Sunlight_Framework_Observables_ObservableObject.typeId = "e";
ptyp_ = Sunlight_Framework_Observables_ObservableObject.prototype;
ptyp_.__ctor = function Sunlight__Framework__Observables__ObservableObject____ctor() {
};
System__Type__RegisterReferenceType(Sunlight_Framework_Observables_ObservableObject, "Sunlight.Framework.Observables.ObservableObject", Object, []);
function Sunlight_Framework_Observables_ExtensibleObservableObject() {
};
Sunlight_Framework_Observables_ExtensibleObservableObject.typeId = "f";
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
Sunlight_Framework_Binders_ContextBindableObject.typeId = "g";
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
Sunlight_Framework_UI_UIElement.typeId = "h";
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
Sunlight_Framework_UI_UIEvent.typeId = "i";
System__Type__RegisterReferenceType(Sunlight_Framework_UI_UIEvent, "Sunlight.Framework.UI.UIEvent", Object, []);
function System_Delegate() {
};
System_Delegate.typeId = "j";
System__Type__RegisterReferenceType(System_Delegate, "System.Delegate", Object, []);
function System_MulticastDelegate() {
};
System_MulticastDelegate.typeId = "k";
System_MulticastDelegate.prototype = new System_Delegate();
System__Type__RegisterReferenceType(System_MulticastDelegate, "System.MulticastDelegate", System_Delegate, []);
function Sunlight_Framework_Binders_OneTimeDataBinder() {
};
Sunlight_Framework_Binders_OneTimeDataBinder.typeId = "l";
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_OneTimeDataBinder, "Sunlight.Framework.Binders.OneTimeDataBinder", Object, []);
function Sunlight_Framework_Binders_DataBinder() {
};
Sunlight_Framework_Binders_DataBinder.typeId = "m";
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_DataBinder, "Sunlight.Framework.Binders.DataBinder", Object, []);
function System_Collections_Generic_StringDictionary(TValue, $5fcallStatiConstructor) {
  var StringDictionary$1_$TValue$_, $5f_initTracker;
  if (System_Collections_Generic_StringDictionary[TValue.typeId])
    return System_Collections_Generic_StringDictionary[TValue.typeId];
  System_Collections_Generic_StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$1a() {
  };
  StringDictionary$1_$TValue$_ = System_Collections_Generic_StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = System_Collections_Generic_StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "n$" + TValue.typeId + "$";
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
  Action$1_$T1$_.typeId = "o$" + T1.typeId + "$";
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
  List$1_$T$_.typeId = "p$" + T.typeId + "$";
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
System_Action_$UIEvent$_ = System_Action(Sunlight_Framework_UI_UIEvent);
System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_ = System_Collections_Generic_StringDictionary(System_Action_$UIEvent$_);
System_Collections_Generic_List_$OneTimeDataBinder$_ = System_Collections_Generic_List(Sunlight_Framework_Binders_OneTimeDataBinder);
System_Collections_Generic_List_$DataBinder$_ = System_Collections_Generic_List(Sunlight_Framework_Binders_DataBinder);
System_Action_$UIEvent$_._tri();
System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_._tri();
System_Collections_Generic_List_$OneTimeDataBinder$_._tri();
System_Collections_Generic_List_$DataBinder$_._tri();
module("Sunlight.Framework.UI.Test.UIElementTests");
test("TestNewUIElement", 0, Sunlight__Framework__UI__Test__UIElementTests__TestNewUIElement);
//@ sourceMappingURL=Sunlight.Framework.UI.Test.map
})();
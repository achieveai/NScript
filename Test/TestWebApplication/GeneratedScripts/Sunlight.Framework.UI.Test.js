(function(){
Function.typeId = "b";
System__Type__typeMapping = null;
function System__Type__CastType(this_, instance) {
  if (this_.isInstanceOfType(instance) || instance === null || typeof instance === "undefined") {
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
ptyp_.isClass = false;
ptyp_.isStruct = false;
ptyp_.isInterface = false;
ptyp_.baseType = null;
ptyp_.fullName = null;
ptyp_.typeId = null;
ptyp_.baseInterfaces = null;
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
System__Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "c";
function System__Object__GetNewImportedExtension() {
  return {
    "toJSON": System__Object__NoReturn
  };
};
function System__Object__NoReturn() {
  return undefined;
};
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
function Sunlight__Framework__UI__Test__ManualTemplateTests__GetValue(func, defaultValue) {
  var stmtTemp1;
  try {
    return func();
  } catch (stmtTemp1) {
    return defaultValue;
  }
};
function Sunlight__Framework__UI__Test__ManualTemplateTests__Test() {
  var instanceFactory;
  instanceFactory = function Sunlight__Framework__UI__Test__ManualTemplateTests__Test_del7(document) {
    var elem, elem1, elem2, elem3, elem4, binder;
    elem = document.createElement("div");
    elem.innerHTML = "<div class='x y'>Hi <span></span>!!!</div><div class='x'>Your last name is: <span></span></div>";
    elem1 = System__Web__Html__Node__As(System__Web__Html__Node__get_Children(elem).get_item(0));
    elem2 = System__Web__Html__Node__As(System__Web__Html__Node__get_Children(elem1).get_item(0));
    elem3 = System__Web__Html__Node__As(System__Web__Html__Node__get_Children(elem).get_item(1));
    elem4 = System__Web__Html__Node__As(System__Web__Html__Node__get_Children(elem3).get_item(0));
    binder = function Sunlight__Framework__UI__Test__ManualTemplateTests__Test_del6(obj) {
      var appViewModel;
      appViewModel = System__Type__CastType(Sunlight_Framework_UI_Test_AppViewModel, obj);
      Sunlight__Framework__UI__Helpers__CssTargetBinder__Bind(elem1, "z", Sunlight__Framework__UI__Test__ManualTemplateTests__GetValue(function Sunlight__Framework__UI__Test__ManualTemplateTests__Test_del() {
        return appViewModel.get_fullContact();
      }, false));
      elem2.textContent = Sunlight__Framework__UI__Test__ManualTemplateTests__GetValue(function Sunlight__Framework__UI__Test__ManualTemplateTests__Test_del2() {
        return appViewModel.get_name();
      }, null);
      elem3.style.display = Sunlight__Framework__UI__Test__ManualTemplateTests__GetValue(function Sunlight__Framework__UI__Test__ManualTemplateTests__Test_del3() {
        return Sunlight__Framework__UI__Test__ManualTemplateTests__noneValue.convert(appViewModel.get_hasLastName());
      }, null);
      elem4.textContent = Sunlight__Framework__UI__Test__ManualTemplateTests__GetValue(function Sunlight__Framework__UI__Test__ManualTemplateTests__Test_del4() {
        return appViewModel.get_lastName();
      }, null);
      Sunlight__Framework__TaskScheduler__get_Instance().enqueueTask(function Sunlight__Framework__UI__Test__ManualTemplateTests__Test_del5() {
      }, "initializing binders");
    };
    return Sunlight__Framework__UI__TemplateInstance_factory(elem, binder, null);
  };
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
function System__String__IsNullOrEmpty(s) {
  return !s;
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
function System__Web__Html__Node__get_ChildNodes(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  return this_.importedExtension.ChildNodes = this_.importedExtension.ChildNodes || this_.childNodes ? System_ArrayG_$Node$_.__ctor(this_.childNodes) : null;
};
function System__Web__Html__Node__get_Children(this_) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  return this_.importedExtension.Children = this_.importedExtension.Children || this_.children ? System_ArrayG_$Node$_.__ctor(this_.children) : null;
};
function System__Web__Html__Node__As(this_) {
  return this_;
};
function Sunlight_Framework_UI_Test_AppViewModel() {
};
Sunlight_Framework_UI_Test_AppViewModel.typeId = "j";
ptyp_ = Sunlight_Framework_UI_Test_AppViewModel.prototype;
ptyp_.get_fullContact = function Sunlight__Framework__UI__Test__AppViewModel__get_FullContact() {
  return this.FullContact;
};
ptyp_.get_lastName = function Sunlight__Framework__UI__Test__AppViewModel__get_LastName() {
  return this.LastName;
};
ptyp_.get_name = function Sunlight__Framework__UI__Test__AppViewModel__get_Name() {
  return this.Name;
};
ptyp_.get_hasLastName = function Sunlight__Framework__UI__Test__AppViewModel__get_HasLastName() {
  return System__String__IsNullOrEmpty(this.get_lastName());
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Test_AppViewModel, "Sunlight.Framework.UI.Test.AppViewModel", Object, []);
function Sunlight_Framework_Binders_TargetBinder() {
};
Sunlight_Framework_Binders_TargetBinder.typeId = "k";
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_TargetBinder, "Sunlight.Framework.Binders.TargetBinder", Object, []);
function Sunlight_Framework_UI_Helpers_CssTargetBinder() {
};
Sunlight_Framework_UI_Helpers_CssTargetBinder.typeId = "l";
function Sunlight__Framework__UI__Helpers__CssTargetBinder__Bind(element, className, value) {
  if (value)
    element.classList.add(className);
  else
    element.classList.remove(className);
};
Sunlight_Framework_UI_Helpers_CssTargetBinder.prototype = new Sunlight_Framework_Binders_TargetBinder();
System__Type__RegisterReferenceType(Sunlight_Framework_UI_Helpers_CssTargetBinder, "Sunlight.Framework.UI.Helpers.CssTargetBinder", Sunlight_Framework_Binders_TargetBinder, []);
function Sunlight_Framework_TaskScheduler() {
};
Sunlight_Framework_TaskScheduler.typeId = "m";
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
ptyp_.enqueueTask = function Sunlight__Framework__TaskScheduler__EnqueueTask(work, traceId) {
  return null;
};
System__Type__RegisterReferenceType(Sunlight_Framework_TaskScheduler, "Sunlight.Framework.TaskScheduler", Object, []);
function Sunlight_Framework_UI_TemplateInstance() {
};
Sunlight_Framework_UI_TemplateInstance.typeId = "n";
function Sunlight__Framework__UI__TemplateInstance_factory(rootElement, dataContextBinders, templateParentBinders) {
  var this_;
  this_ = new Sunlight_Framework_UI_TemplateInstance();
  this_.__ctor(rootElement, dataContextBinders, templateParentBinders);
  return this_;
};
ptyp_ = Sunlight_Framework_UI_TemplateInstance.prototype;
ptyp_.rootElements = null;
ptyp_.elementDataBinders = null;
ptyp_.elementTemplateParentBinders = null;
ptyp_.__ctor = function Sunlight__Framework__UI__TemplateInstance____ctor(rootElement, dataContextBinders, templateParentBinders) {
  var childNodes, i;
  childNodes = System__Web__Html__Node__get_ChildNodes(rootElement);
  this.rootElements = System_ArrayG_$Node$_.__ctora(childNodes.V_get_Length());
  this.elementDataBinders = dataContextBinders;
  this.elementTemplateParentBinders = templateParentBinders;
  for (i = this.rootElements.V_get_Length() - 1; i >= 0; i--)
    this.rootElements.set_item(i, childNodes.get_item(i));
};
System__Type__RegisterReferenceType(Sunlight_Framework_UI_TemplateInstance, "Sunlight.Framework.UI.TemplateInstance", Object, []);
function System_ArrayImpl() {
};
System_ArrayImpl.typeId = "o";
ptyp_ = System_ArrayImpl.prototype;
ptyp_.__ctor = function System__ArrayImpl____ctor() {
};
System__Type__RegisterReferenceType(System_ArrayImpl, "System.ArrayImpl", Object, []);
function Sunlight_Framework_Observables_ObservableObject() {
};
Sunlight_Framework_Observables_ObservableObject.typeId = "p";
ptyp_ = Sunlight_Framework_Observables_ObservableObject.prototype;
ptyp_.__ctor = function Sunlight__Framework__Observables__ObservableObject____ctor() {
};
System__Type__RegisterReferenceType(Sunlight_Framework_Observables_ObservableObject, "Sunlight.Framework.Observables.ObservableObject", Object, []);
function Sunlight_Framework_Observables_ExtensibleObservableObject() {
};
Sunlight_Framework_Observables_ExtensibleObservableObject.typeId = "q";
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
Sunlight_Framework_Binders_ContextBindableObject.typeId = "r";
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
Sunlight_Framework_UI_UIElement.typeId = "s";
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
function System_ValueType() {
};
System_ValueType.typeId = "t";
ptyp_ = System_ValueType.prototype;
ptyp_.boxedValue = null;
System__Type__RegisterReferenceType(System_ValueType, "System.ValueType", Object, []);
function Sunlight_Framework_UI_UIEvent() {
};
Sunlight_Framework_UI_UIEvent.typeId = "u";
System__Type__RegisterReferenceType(Sunlight_Framework_UI_UIEvent, "Sunlight.Framework.UI.UIEvent", Object, []);
function System_Delegate() {
};
System_Delegate.typeId = "v";
System__Type__RegisterReferenceType(System_Delegate, "System.Delegate", Object, []);
function System_MulticastDelegate() {
};
System_MulticastDelegate.typeId = "w";
System_MulticastDelegate.prototype = new System_Delegate();
System__Type__RegisterReferenceType(System_MulticastDelegate, "System.MulticastDelegate", System_Delegate, []);
function Sunlight_Framework_Binders_OneTimeDataBinder() {
};
Sunlight_Framework_Binders_OneTimeDataBinder.typeId = "x";
System__Type__RegisterReferenceType(Sunlight_Framework_Binders_OneTimeDataBinder, "Sunlight.Framework.Binders.OneTimeDataBinder", Object, []);
function Sunlight_Framework_Binders_DataBinder() {
};
Sunlight_Framework_Binders_DataBinder.typeId = "y";
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
  ValueIfTrue$1_$T$_.typeId = "z$" + T.typeId + "$";
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
  ptyp_.convert = function Sunlight__Framework__UI__Test__ValueIfTrue$1__Convert(isTrue) {
    return isTrue ? this.value : this.defaultValue;
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
function System_Collections_Generic_StringDictionary(TValue, $5fcallStatiConstructor) {
  var StringDictionary$1_$TValue$_, $5f_initTracker;
  if (System_Collections_Generic_StringDictionary[TValue.typeId])
    return System_Collections_Generic_StringDictionary[TValue.typeId];
  System_Collections_Generic_StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$1a() {
  };
  StringDictionary$1_$TValue$_ = System_Collections_Generic_StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = System_Collections_Generic_StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "bb$" + TValue.typeId + "$";
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
  Action$1_$T1$_.typeId = "bc$" + T1.typeId + "$";
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
  List$1_$T$_.typeId = "bd$" + T.typeId + "$";
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
System_ArrayG_$Node$_ = System_ArrayG(Node);
System_Action_$UIEvent$_ = System_Action(Sunlight_Framework_UI_UIEvent);
System_Collections_Generic_StringDictionary_$Action_$UIEvent$_$_ = System_Collections_Generic_StringDictionary(System_Action_$UIEvent$_);
System_Collections_Generic_List_$OneTimeDataBinder$_ = System_Collections_Generic_List(Sunlight_Framework_Binders_OneTimeDataBinder);
System_Collections_Generic_List_$DataBinder$_ = System_Collections_Generic_List(Sunlight_Framework_Binders_DataBinder);
Sunlight__Framework__UI__Test__ManualTemplateTests____cctor();
System__String____cctor();
Sunlight_Framework_UI_Test_ValueIfTrue_$String$_._tri();
System_ArrayG_$Node$_._tri();
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
//@ sourceMappingURL=Sunlight.Framework.UI.Test.map
})();
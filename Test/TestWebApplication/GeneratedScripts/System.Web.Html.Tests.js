(function(){
var System__Type__typeMapping, System_Collections_Generic_StringDictionary_$Function$_, System_Collections_Generic_KeyValuePair_$String_x_Function$_, System__String__formatHelperRegex, System__String__trimStartHelperRegex, System__String__trimEndHelperRegex, ptyp_;
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
ptyp_.getDefaultValue = function System__Type__GetDefaultValue() {
  return null;
};
System__Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "c";
function System__Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
function System__Object__GetNewImportedExtension() {
  return {
    "toJSON": System__Object__NoReturn
  };
};
function System__Object__NoReturn() {
  return undefined;
};
System__Type__RegisterReferenceType(Object, "System.Object", null, []);
function System_Web_Html_Tests_TestElement() {
};
System_Web_Html_Tests_TestElement.typeId = "d";
function System__Web__Html__Tests__TestElement__Setup() {
};
function System__Web__Html__Tests__TestElement__TestCreateElement() {
  var element;
  element = window.document.createElement("div");
  QUnit.notEqual(null, element, "element should not be null.");
  QUnit.equal("DIV", element.tagName, "TagName of element");
};
function System__Web__Html__Tests__TestElement__TestAttribute() {
  var element, attributes;
  element = System__Web__Html__Node__As(window.document.createElement("div"));
  element.setAttribute("_id", "test");
  attributes = element.attributes;
  QUnit.equal(1, attributes.length, "attributes.Length");
  QUnit.equal("_id", attributes[0].name, "attributes[0].Name");
  QUnit.equal("test", attributes[0].value, "attributes[0].Value");
};
function System__Web__Html__Tests__TestElement__TestEventBinding() {
  var element, handlerCalled, eventType, handler, testEvt;
  element = window.document.createElement("div");
  element.textContent = "Foo";
  window.document.body.appendChild(element);
  handlerCalled = false;
  eventType = null;
  handler = function System__Web__Html__Tests__TestElement__TestEventBinding_del(elem, evt) {
    handlerCalled = true;
    eventType = evt.type;
  };
  System__Web__Html__Element__Bind(element, "click", handler, false);
  testEvt = window.document.createEvent("MouseEvent");
  testEvt.initMouseEvent("click", true, true, window, "", 0, 0, 0, 0, false, false, false, false, "", element);
  element.dispatchEvent(testEvt);
  QUnit.ok(handlerCalled, "Handler should be called");
  QUnit.equal("click", eventType, "EventType");
  System__Web__Html__Element__UnBind(element, "click", handler, false);
  handlerCalled = false;
  eventType = null;
  element.dispatchEvent(testEvt);
  QUnit.ok(!handlerCalled, "Handler should not be called");
};
System__Type__RegisterReferenceType(System_Web_Html_Tests_TestElement, "System.Web.Html.Tests.TestElement", Object, []);
function System__Web__Html__Node__As(this_) {
  return this_;
};
function System__Web__Html__Element__Bind(this_, eventName, handler, capture) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  System__EventBinder__AddEvent(this_, eventName, handler, capture);
};
function System__Web__Html__Element__UnBind(this_, eventName, handler, capture) {
  this_.importedExtension = this_.importedExtension || System__Object__GetNewImportedExtension();
  System__EventBinder__RemoveEvent(this_, eventName, handler, capture);
};
function System_EventBinder() {
};
System_EventBinder.typeId = "e";
function System__EventBinder__GetBinder(importedElement) {
  if (System__Object__IsNullOrUndefined(importedElement.importedExtension))
    importedElement.importedExtension = {
    };
  if (System__Object__IsNullOrUndefined(importedElement.importedExtension.importedExtension))
    importedElement.importedExtension.importedExtension = System__EventBinder_factory(importedElement);
  return System__Type__CastType(System_EventBinder, importedElement.importedExtension.importedExtension);
};
function System__EventBinder__AddEvent(importedElement, name, action, onCapture) {
  var binder;
  binder = System__EventBinder__GetBinder(importedElement);
  binder.addEvent(name, action, onCapture);
};
function System__EventBinder__RemoveEvent(importedElement, name, action, onCapture) {
  var binder;
  if (importedElement.importedExtension === null || importedElement.importedExtension.importedExtension === null)
    return;
  binder = System__EventBinder__GetBinder(importedElement);
  binder.removeEvent(name, action, onCapture);
};
function System__EventBinder__IsW3wc(element) {
  return !!element.addEventListener;
};
function System__EventBinder__GetEventType(evt) {
  return evt.type;
};
function System__EventBinder_factory(element) {
  var this_;
  this_ = new System_EventBinder();
  this_.__ctor(element);
  return this_;
};
ptyp_ = System_EventBinder.prototype;
ptyp_.capturePhaseEvents = null;
ptyp_.bubblePhaseEvents = null;
ptyp_.target = null;
ptyp_.disposed = false;
ptyp_.__ctor = function System__EventBinder____ctor(element) {
  this.capturePhaseEvents = System_Collections_Generic_StringDictionary_$Function$_.defaultConstructor();
  this.bubblePhaseEvents = System_Collections_Generic_StringDictionary_$Function$_.defaultConstructor();
  this.target = element;
};
ptyp_.addEvent = function System__EventBinder__AddEvent0(name, action, onCapture) {
  var isW3wc, evts, elementEvent;
  isW3wc = System__EventBinder__IsW3wc(this.target);
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
    if (onCapture && System__EventBinder__IsW3wc(this.target))
      this.addEventListener(name, System__Delegate__Create("eventHandlerCapture", this), true);
    else if (isW3wc)
      this.addEventListener(name, System__Delegate__Create("eventHandlerBubble", this), false);
    else
      this.attachEvent(name, System__Delegate__Create("eventHandlerIE", this));
  }
  else
    elementEvent = System__Delegate__Combine(elementEvent, action);
  evts.set_item(name, elementEvent);
};
ptyp_.removeEvent = function System__EventBinder__RemoveEvent0(name, handler, onCapture) {
  var isW3wc, evts, elementEvent;
  isW3wc = System__EventBinder__IsW3wc(this.target);
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
    elementEvent = System__Delegate__Remove(elementEvent, handler);
    if (!elementEvent) {
      evts.remove(name);
      if (onCapture)
        this.removeEventListener(name, System__Delegate__Create("eventHandlerCapture", this), true);
      else if (isW3wc)
        this.removeEventListener(name, System__Delegate__Create("eventHandlerBubble", this), false);
      else
        this.detachEvent(name, System__Delegate__Create("eventHandlerIE", this));
    }
    else
      evts.set_item(name, elementEvent);
  }
};
ptyp_.addEventListener = function System__EventBinder__AddEventListener(evtName, cb, isCapture) {
  this.target.addEventListener(evtName, cb, isCapture);
};
ptyp_.attachEvent = function System__EventBinder__AttachEvent(evtName, cb) {
  this.target.atachEvent("on" + evtName, cb);
};
ptyp_.removeEventListener = function System__EventBinder__RemoveEventListener(evtName, cb, isCapture) {
  this.target.removeEventListener(evtName, cb, isCapture);
};
ptyp_.detachEvent = function System__EventBinder__DetachEvent(evtName, cb) {
  this.target.detachEvent("on" + evtName, cb);
};
ptyp_.eventHandlerIE = function System__EventBinder__EventHandlerIE() {
  this.eventHandlerBubble(event);
};
ptyp_.eventHandlerCapture = function System__EventBinder__EventHandlerCapture(evt) {
  if (this.disposed)
    return;
  System__Type__CastType(System_Collections_Generic_KeyValuePair_$String_x_Function$_, this.capturePhaseEvents.get_item(System__EventBinder__GetEventType(evt)))(this.target, evt);
};
ptyp_.eventHandlerBubble = function System__EventBinder__EventHandlerBubble(evt) {
  var del;
  if (this.disposed)
    return;
  if (this.bubblePhaseEvents.tryGetValue(System__EventBinder__GetEventType(evt), {
    read: function() {
      return del;
    },
    write: function(arg0) {
      return del = arg0;
    }
  }))
    del(this.target, evt);
};
System__Type__RegisterReferenceType(System_EventBinder, "System.EventBinder", Object, []);
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
function System_ValueType() {
};
System_ValueType.typeId = "f";
ptyp_ = System_ValueType.prototype;
ptyp_.boxedValue = null;
System__Type__RegisterReferenceType(System_ValueType, "System.ValueType", Object, []);
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
Function.getDefaultValue = function() {
  return {
  };
};
Error.typeId = "i";
System__Type__RegisterReferenceType(Error, "System.Exception", Object, []);
function System_Collections_Generic_StringDictionary(TValue, $5fcallStatiConstructor) {
  var StringDictionary$1_$TValue$_, KeyValuePair$2_$String_x_TValue$_, $5f_initTracker;
  if (System_Collections_Generic_StringDictionary[TValue.typeId])
    return System_Collections_Generic_StringDictionary[TValue.typeId];
  System_Collections_Generic_StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$10() {
  };
  StringDictionary$1_$TValue$_ = System_Collections_Generic_StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = System_Collections_Generic_StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "j$" + TValue.typeId + "$";
  KeyValuePair$2_$String_x_TValue$_ = System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor);
  KeyValuePair$2_$String_x_TValue$_ = System_Collections_Generic_KeyValuePair(String, TValue, $5fcallStatiConstructor);
  StringDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_StringDictionary$1_factory0() {
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
function System_Collections_Generic_KeyValuePair(K, V, $5fcallStatiConstructor) {
  var KeyValuePair$2_$K_x_V$_, $5f_initTracker;
  if (System_Collections_Generic_KeyValuePair[K.typeId] && System_Collections_Generic_KeyValuePair[K.typeId][V.typeId])
    return System_Collections_Generic_KeyValuePair[K.typeId][V.typeId];
    System_Collections_Generic_KeyValuePair[K.typeId] = {
    };
  System_Collections_Generic_KeyValuePair[K.typeId][V.typeId] = function(boxedValue) {
    this.boxedValue = boxedValue;
  };
  KeyValuePair$2_$K_x_V$_ = System_Collections_Generic_KeyValuePair[K.typeId][V.typeId];
  KeyValuePair$2_$K_x_V$_.genericParameters = [K, V];
  KeyValuePair$2_$K_x_V$_.genericClosure = System_Collections_Generic_KeyValuePair;
  KeyValuePair$2_$K_x_V$_.typeId = "k$" + K.typeId + "_" + V.typeId + "$";
  KeyValuePair$2_$K_x_V$_.getDefaultValue = function() {
    return {
      key: System__Type__GetDefaultValueStatic(K),
      val: System__Type__GetDefaultValueStatic(V)
    };
  };
  KeyValuePair$2_$K_x_V$_.prototype = new System_ValueType();
  System__Type__RegisterStructType(KeyValuePair$2_$K_x_V$_, "System.Collections.Generic.KeyValuePair`2<" + K.fullName + "," + V.fullName + ">", []);
  KeyValuePair$2_$K_x_V$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    K = K;
    V = V;
    KeyValuePair$2_$K_x_V$_ = System_Collections_Generic_KeyValuePair(K, V, true);
  };
  if ($5fcallStatiConstructor)
    KeyValuePair$2_$K_x_V$_._tri();
  return KeyValuePair$2_$K_x_V$_;
};
System_Collections_Generic_StringDictionary_$Function$_ = System_Collections_Generic_StringDictionary(Function);
System_Collections_Generic_KeyValuePair_$String_x_Function$_ = System_Collections_Generic_KeyValuePair(String, Function);
System__String____cctor();
System_Collections_Generic_StringDictionary_$Function$_._tri();
System_Collections_Generic_KeyValuePair_$String_x_Function$_._tri();
module("System.Web.Html.Tests.TestElement", {
  "setup": System__Web__Html__Tests__TestElement__Setup
});
test("TestCreateElement", 0, System__Web__Html__Tests__TestElement__TestCreateElement);
test("TestAttribute", 0, System__Web__Html__Tests__TestElement__TestAttribute);
test("TestEventBinding", 0, System__Web__Html__Tests__TestElement__TestEventBinding);
})();
//# sourceMappingURL=System.Web.Html.Tests.map
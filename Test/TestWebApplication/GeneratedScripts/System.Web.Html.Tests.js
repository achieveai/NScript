(function(){
var Type__typeMapping, StringDictionary_$Function$_, ptyp_;
Function.typeId = "b";
Type__typeMapping = null;
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
ptyp_.baseType = null;
ptyp_.fullName = null;
ptyp_.typeId = null;
ptyp_.baseInterfaces = null;
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
ptyp_.getDefaultValue = function Type__GetDefaultValue() {
  return null;
};
Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "c";
function Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
function Object__GetNewImportedExtension() {
  return {
    "toJSON": Object__NoReturn
  };
};
function Object__NoReturn() {
  return undefined;
};
Type__RegisterReferenceType(Object, "System.Object", null, []);
function TestElement() {
};
TestElement.typeId = "d";
function TestElement__Setup() {
};
function TestElement__TestCreateElement(assert) {
  var element;
  element = window.document.createElement("div");
  assert.notEqual(null, element, "element should not be null.");
  assert.equal("DIV", element.tagName, "TagName of element");
};
function TestElement__TestAttribute(assert) {
  var element, attributes;
  element = Node__As(window.document.createElement("div"));
  element.setAttribute("_id", "test");
  attributes = element.attributes;
  assert.equal(1, attributes.length, "attributes.Length");
  assert.equal("_id", attributes[0].name, "attributes[0].Name");
  assert.equal("test", attributes[0].value, "attributes[0].Value");
};
function TestElement__TestEventBinding(assert) {
  var element, handlerCalled, eventType, handler, testEvt;
  element = window.document.createElement("div");
  element.textContent = "Foo";
  window.document.body.appendChild(element);
  handlerCalled = false;
  eventType = null;
  handler = function TestElement__TestEventBinding_del(elem, evt) {
    handlerCalled = true;
    eventType = evt.type;
  };
  Element__Bind(element, "click", handler, false);
  testEvt = window.document.createEvent("MouseEvent");
  testEvt.initMouseEvent("click", true, true, window, "", 0, 0, 0, 0, false, false, false, false, "", element);
  element.dispatchEvent(testEvt);
  assert.ok(handlerCalled, "Handler should be called");
  assert.equal("click", eventType, "EventType");
  Element__UnBind(element, "click", handler, false);
  handlerCalled = false;
  eventType = null;
  element.dispatchEvent(testEvt);
  assert.ok(!handlerCalled, "Handler should not be called");
};
Type__RegisterReferenceType(TestElement, "System.Web.Html.Tests.TestElement", Object, []);
function Node__As(this_) {
  return this_;
};
function Element__Bind(this_, eventName, handler, capture) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  EventBinder__AddEvent(this_, eventName, handler, capture);
};
function Element__UnBind(this_, eventName, handler, capture) {
  this_.importedExtension = this_.importedExtension || Object__GetNewImportedExtension();
  EventBinder__RemoveEvent(this_, eventName, handler, capture);
};
function EventBinder() {
};
EventBinder.typeId = "e";
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
  if (importedElement.importedExtension === null || importedElement.importedExtension.importedExtension === null)
    return;
  binder = EventBinder__GetBinder(importedElement);
  binder.removeEvent(name, action, onCapture);
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
ptyp_.target = null;
ptyp_.disposed = false;
ptyp_.__ctor = function EventBinder____ctor(element) {
  this.capturePhaseEvents = StringDictionary_$Function$_.defaultConstructor();
  this.bubblePhaseEvents = StringDictionary_$Function$_.defaultConstructor();
  this.target = element;
};
ptyp_.addEvent = function EventBinder__AddEvent0(name, action, onCapture) {
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
ptyp_.removeEvent = function EventBinder__RemoveEvent0(name, handler, onCapture) {
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
function ValueType() {
};
ValueType.typeId = "f";
ptyp_ = ValueType.prototype;
ptyp_.boxedValue = null;
Type__RegisterReferenceType(ValueType, "System.ValueType", Object, []);
Function.getDefaultValue = function() {
  return {
  };
};
Error.typeId = "g";
Type__RegisterReferenceType(Error, "System.Exception", Object, []);
function StringDictionary(TValue, $5fcallStatiConstructor) {
  var StringDictionary$1_$TValue$_, $5f_initTracker;
  if (StringDictionary[TValue.typeId])
    return StringDictionary[TValue.typeId];
  StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$10() {
  };
  StringDictionary$1_$TValue$_ = StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "h$" + TValue.typeId + "$";
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
  Type__RegisterReferenceType(StringDictionary$1_$TValue$_, "System.Collections.Generic.StringDictionary`1<" + TValue.fullName + ">", Object, []);
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
StringDictionary_$Function$_ = StringDictionary(Function);
StringDictionary_$Function$_.$5ftri();
QUnit.module("System.Web.Html.Tests.TestElement", {
  "before": TestElement__Setup
});
QUnit.test("TestCreateElement", TestElement__TestCreateElement);
QUnit.test("TestAttribute", TestElement__TestAttribute);
QUnit.test("TestEventBinding", TestElement__TestEventBinding);
})();
//# sourceMappingURL=System.Web.Html.Tests.map
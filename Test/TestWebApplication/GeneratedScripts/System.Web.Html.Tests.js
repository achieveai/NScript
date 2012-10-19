
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
ptyp_.isDelegate = false;
ptyp_.isClass = false;
ptyp_.baseType = null;
ptyp_.fullName = null;
ptyp_.interfaces = null;
ptyp_.getDefaultValue = function System__Type__GetDefaultValue() {
  return null;
};
System__Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "c";
function System__Object__IsNullOrUndefined(obj) {
  return obj === null || typeof obj == "undefined";
};
System__Type__RegisterReferenceType(Object, "System.Object", null, []);
function System_Web_Html_Tests_TestElement() {
};
System_Web_Html_Tests_TestElement.typeId = "d";
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
  attributes = System__Web__Html__Node__GetAttributes(element);
  QUnit.equal(1, attributes.get_count(), "attributes.Length");
  QUnit.equal("_id", attributes.get_item(0).name, "attributes[0].Name");
  QUnit.equal("test", attributes.get_item(0).value, "attributes[0].Value");
};
function System__Web__Html__Tests__TestElement__TestEventBinding() {
  var element, handlerCalled, eventType, handler, testEvt;
  element = window.document.createElement("div");
  element.textContent = "Foo";
  window.document.body.appendChild(element);
  handlerCalled = false;
  eventType = null;
  handler = function System__Web__Html__Tests__TestElement__TestEventBinding_del(evt) {
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
function System__Web__Html__Node__GetAttributes(this_) {
  return System_Web_Html_DomList_$Attr$_.__ctor(this_.attributes);
};
function System__Web__Html__Element__Bind(this_, eventName, handler, capture) {
  if (System__Object__IsNullOrUndefined(this_.elementBinder))
    this_.elementBinder = System_Web_DomDataCache_$ElementEvent$_.__ctor(this_);
  this_.elementBinder.addEvent(eventName, handler, capture);
};
function System__Web__Html__Element__UnBind(this_, eventName, handler, capture) {
  if (!System__Object__IsNullOrUndefined(this_.elementBinder))
    this_.elementBinder.removeEvent(eventName, handler, capture);
};
Attr.typeId = "e";
System__Type__RegisterReferenceType(Attr, "System.Web.Html.NodeAttribute", Object, []);
MutationEvent.typeId = "f";
System__Type__RegisterReferenceType(MutationEvent, "System.Web.Html.MutableEvent", Object, []);
Array.typeId = "g";
function System__NativeArray__GetFrom(this_, index) {
  return this_[index];
};
System__Type__RegisterReferenceType(Array, "System.NativeArray", Object, []);
Error.typeId = "h";
System__Type__RegisterReferenceType(Error, "System.Exception", Object, []);
Object.typeId = "i";
System__Type__RegisterReferenceType(Object, "System.Collections.Dictionary", Object, []);
function System_Delegate() {
};
System_Delegate.typeId = "j";
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
System_MulticastDelegate.typeId = "k";
System_MulticastDelegate.prototype = new System_Delegate();
System__Type__RegisterReferenceType(System_MulticastDelegate, "System.MulticastDelegate", System_Delegate, []);
function System_Web_Html_DomList(T, $5fcallStatiConstructor) {
  var DomList$1_$T$_;
  if (System_Web_Html_DomList[T.typeId])
    return System_Web_Html_DomList[T.typeId];
  System_Web_Html_DomList[T.typeId] = function System__Web__Html__DomList$1() {
  };
  DomList$1_$T$_ = System_Web_Html_DomList[T.typeId];
  DomList$1_$T$_.genericParameters = [T];
  DomList$1_$T$_.genericClosure = System_Web_Html_DomList;
  DomList$1_$T$_.typeId = "l$" + T.typeId + "$";
  DomList$1_$T$_.__ctor = function System_Web_Html_DomList$1_factory(array) {
    var this_;
    this_ = new DomList$1_$T$_();
    this_.__ctor(array);
    return this_;
  };
  ptyp_ = DomList$1_$T$_.prototype;
  ptyp_.array = null;
  ptyp_.__ctor = function System__Web__Html__DomList$1____ctor(array) {
    this.array = array;
  };
  ptyp_.get_item = function System__Web__Html__DomList$1__get_Item(index) {
    if (index >= this.array.length)
      throw new Error("index not in range");
    else
      return System__NativeArray__GetFrom(this.array, index);
  };
  ptyp_.get_count = function System__Web__Html__DomList$1__get_Count() {
    return this.array.length;
  };
  System__Type__RegisterReferenceType(DomList$1_$T$_, "System.Web.Html.DomList`1<" + T.fullName + ">", Object, []);
  return DomList$1_$T$_;
};
function System_Web_DomDataCache(T, $5fcallStatiConstructor) {
  var StringDictionary$1_$Action$1_$T$_$_, DomDataCache$1_$T$_, $5f_initTracker;
  if (System_Web_DomDataCache[T.typeId])
    return System_Web_DomDataCache[T.typeId];
  System_Web_DomDataCache[T.typeId] = function System__Web__DomDataCache$1() {
  };
  DomDataCache$1_$T$_ = System_Web_DomDataCache[T.typeId];
  DomDataCache$1_$T$_.genericParameters = [T];
  DomDataCache$1_$T$_.genericClosure = System_Web_DomDataCache;
  DomDataCache$1_$T$_.typeId = "m$" + T.typeId + "$";
  DomDataCache$1_$T$_.isW3wc = function System__Web__DomDataCache$1__IsW3wc(element) {
    return !!element.addEventListener;
  };
  DomDataCache$1_$T$_.getEventType = function System__Web__DomDataCache$1__GetEventType(evt) {
    return evt.type;
  };
  DomDataCache$1_$T$_.__ctor = function System_Web_DomDataCache$1_factory(element) {
    var this_;
    this_ = new DomDataCache$1_$T$_();
    this_.__ctor(element);
    return this_;
  };
  ptyp_ = DomDataCache$1_$T$_.prototype;
  ptyp_.capturePhaseEvents = null;
  ptyp_.bubblePhaseEvents = null;
  ptyp_.dataDictionary = null;
  ptyp_.target = null;
  ptyp_.disposed = false;
  ptyp_.__ctor = function System__Web__DomDataCache$1____ctor(element) {
    this.capturePhaseEvents = StringDictionary$1_$Action$1_$T$_$_.defaultConstructor();
    this.bubblePhaseEvents = StringDictionary$1_$Action$1_$T$_$_.defaultConstructor();
    this.dataDictionary = new Object();
    this.target = element;
  };
  ptyp_.addEvent = function System__Web__DomDataCache$1__AddEvent(name, action, onCapture) {
    var isW3wc, evts, elementEvent;
    isW3wc = DomDataCache$1_$T$_.isW3wc(this.target);
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
      if (onCapture && DomDataCache$1_$T$_.isW3wc(this.target))
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
  ptyp_.removeEvent = function System__Web__DomDataCache$1__RemoveEvent(name, handler, onCapture) {
    var isW3wc, evts, elementEvent;
    isW3wc = DomDataCache$1_$T$_.isW3wc(this.target);
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
      if (elementEvent === null) {
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
  ptyp_.addEventListener = function System__Web__DomDataCache$1__AddEventListener(evtName, cb, isCapture) {
    this.target.addEventListener(evtName, cb, isCapture);
  };
  ptyp_.attachEvent = function System__Web__DomDataCache$1__AttachEvent(evtName, cb) {
    this.target.atachEvent("on" + evtName, cb);
  };
  ptyp_.removeEventListener = function System__Web__DomDataCache$1__RemoveEventListener(evtName, cb, isCapture) {
    this.target.removeEventListener(evtName, cb, isCapture);
  };
  ptyp_.detachEvent = function System__Web__DomDataCache$1__DetachEvent(evtName, cb) {
    this.target.detachEvent("on" + evtName, cb);
  };
  ptyp_.eventHandlerIE = function System__Web__DomDataCache$1__EventHandlerIE() {
    this.eventHandlerBubble(event);
  };
  ptyp_.eventHandlerCapture = function System__Web__DomDataCache$1__EventHandlerCapture(evt) {
    if (this.disposed)
      return;
    this.capturePhaseEvents.get_item(DomDataCache$1_$T$_.getEventType(evt))(evt);
  };
  ptyp_.eventHandlerBubble = function System__Web__DomDataCache$1__EventHandlerBubble(evt) {
    if (this.disposed)
      return;
    this.bubblePhaseEvents.get_item(DomDataCache$1_$T$_.getEventType(evt))(evt);
  };
  System__Type__RegisterReferenceType(DomDataCache$1_$T$_, "System.Web.DomDataCache`1<" + T.fullName + ">", Object, []);
  DomDataCache$1_$T$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    StringDictionary$1_$Action$1_$T$_$_ = System_Collections_Generic_StringDictionary(System_Action(T, true), true);
  };
  if ($5fcallStatiConstructor)
    DomDataCache$1_$T$_._tri();
  return DomDataCache$1_$T$_;
};
function System_Collections_Generic_StringDictionary(TValue, $5fcallStatiConstructor) {
  var StringDictionary$1_$TValue$_;
  if (System_Collections_Generic_StringDictionary[TValue.typeId])
    return System_Collections_Generic_StringDictionary[TValue.typeId];
  System_Collections_Generic_StringDictionary[TValue.typeId] = function System__Collections__Generic__StringDictionary$1() {
  };
  StringDictionary$1_$TValue$_ = System_Collections_Generic_StringDictionary[TValue.typeId];
  StringDictionary$1_$TValue$_.genericParameters = [TValue];
  StringDictionary$1_$TValue$_.genericClosure = System_Collections_Generic_StringDictionary;
  StringDictionary$1_$TValue$_.typeId = "n$" + TValue.typeId + "$";
  StringDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_StringDictionary$1_factory() {
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
  System__Type__RegisterReferenceType(StringDictionary$1_$TValue$_, "System.Collections.Generic.StringDictionary`1<" + TValue.fullName + ">", Object, []);
  return StringDictionary$1_$TValue$_;
};
function System_Action(T1, $5fcallStatiConstructor) {
  var Action$1_$T1$_;
  if (System_Action[T1.typeId])
    return System_Action[T1.typeId];
  System_Action[T1.typeId] = function System__Action$1() {
  };
  Action$1_$T1$_ = System_Action[T1.typeId];
  Action$1_$T1$_.genericParameters = [T1];
  Action$1_$T1$_.genericClosure = System_Action;
  Action$1_$T1$_.typeId = "o$" + T1.typeId + "$";
  Action$1_$T1$_.prototype = new System_MulticastDelegate();
  System__Type__RegisterReferenceType(Action$1_$T1$_, "System.Action`1<" + T1.fullName + ">", System_MulticastDelegate, []);
  return Action$1_$T1$_;
};
System_Web_Html_DomList_$Attr$_ = System_Web_Html_DomList(Attr);
System_Web_DomDataCache_$ElementEvent$_ = System_Web_DomDataCache(Object);
System_Web_DomDataCache_$ElementEvent$_._tri();
module("System.Web.Html.Tests.TestElement");
test("TestCreateElement", 0, System__Web__Html__Tests__TestElement__TestCreateElement);
test("TestAttribute", 0, System__Web__Html__Tests__TestElement__TestAttribute);
test("TestEventBinding", 0, System__Web__Html__Tests__TestElement__TestEventBinding);
//@ sourceMappingURL=System.Web.Html.Tests.js.map

Function.typeId = "b";
System__Type__typeMapping = null;
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
ptyp_ = Function.prototype;
ptyp_.isDelegate = false;
ptyp_.isClass = false;
ptyp_.isStruct = false;
ptyp_.baseType = null;
ptyp_.fullName = null;
ptyp_.interfaces = null;
ptyp_.box = function System__Type__Box(instance) {
  if (this.isStruct)
    return new this(instance);
  else
    return instance;
};
ptyp_.getDefaultValue = function System__Type__GetDefaultValue() {
  return null;
};
ptyp_.toString = function() {
  return System__Type__ToString(this);
};
System__Type__RegisterReferenceType(Function, "System.Type", Object, []);
Object.typeId = "c";
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
  System__Web__Html__Element__Bind(element, "click", handler);
  testEvt = window.document.createEvent("MouseEvent");
  testEvt.initMouseEvent("click", true, true, window, "", 0, 0, 0, 0, false, false, false, false, "", element);
  element.dispatchEvent(testEvt);
  QUnit.ok(handlerCalled, "Handler should be called");
  QUnit.equal("click", eventType, "EventType");
  System__Web__Html__Element__UnBind(element, "click", handler);
  handlerCalled = false;
  eventType = null;
  element.dispatchEvent(testEvt);
  QUnit.ok(!handlerCalled, "Handler should not be called");
};
System__Type__RegisterReferenceType(System_Web_Html_Tests_TestElement, "System.Web.Html.Tests.TestElement", Object, []);
Window.typeId = "e";
System__Type__RegisterReferenceType(Window, "System.Web.Html.Window", Object, []);
Document.typeId = "f";
System__Type__RegisterReferenceType(Document, "System.Web.Html.Document", Object, []);
Node.typeId = "g";
function System__Web__Html__Node__As(this_) {
  return this_;
};
function System__Web__Html__Node__GetAttributes(this_) {
  return System_Web_Html_DomList_$Attr$_.__ctor(this_.attributes);
};
System__Type__RegisterReferenceType(Node, "System.Web.Html.Node", Object, []);
Element.typeId = "h";
function System__Web__Html__Element__Bind(this_, eventName, handler) {
  var dataCache;
  dataCache = System__Web__Html__DomDataCache__GetDataCache(this_);
  dataCache.addEvent(eventName, handler, false);
};
function System__Web__Html__Element__UnBind(this_, eventName, handler) {
  var dataCache;
  dataCache = System__Web__Html__DomDataCache__GetDataCache(this_);
  dataCache.removeEvent(eventName, handler, false);
};
System__Type__RegisterReferenceType(Element, "System.Web.Html.Element", Node, []);
Attr.typeId = "i";
System__Type__RegisterReferenceType(Attr, "System.Web.Html.NodeAttribute", Object, []);
Event.typeId = "j";
System__Type__RegisterReferenceType(Event, "System.Web.Html.ElementEvent", Object, []);
MutationEvent.typeId = "k";
System__Type__RegisterReferenceType(MutationEvent, "System.Web.Html.MutableEvent", Object, []);
Array.typeId = "l";
function System__NativeArray__GetFrom(this_, index) {
  return this_[index];
};
System__Type__RegisterReferenceType(Array, "System.NativeArray", Object, []);
Error.typeId = "m";
System__Type__RegisterReferenceType(Error, "System.Exception", Object, []);
function System_Web_Html_DomDataCache() {
};
System_Web_Html_DomDataCache.typeId = "n";
System__Web__Html__DomDataCache__instance = null;
System__Web__Html__DomDataCache__cacheId = 0;
System__Web__Html__DomDataCache__cacheIdString = null;
function System__Web__Html__DomDataCache__GetDataCache(element) {
  var attr, number, rv;
  if (System__Web__Html__DomDataCache__cacheIdString === null)
    System__Web__Html__DomDataCache__cacheIdString = System__String__Concat("__Id", System_Int32.box(System__Math__Random(10000)), "_", System_Int32.box(System__DateTime__get_Now().getTime()));
  attr = element.getAttribute(System__Web__Html__DomDataCache__cacheIdString);
  if (attr !== null) {
    number = parseInt(attr);
    rv = System__Web__Html__DomDataCache__instance.get_item(number);
  }
  else {
    rv = System__Web__Html__DomDataCache_factory(element);
    number = System__Web__Html__DomDataCache__cacheId++;
    element.setAttribute(System__Web__Html__DomDataCache__cacheIdString, number.toString());
    System__Web__Html__DomDataCache__instance.set_item(number, rv);
  }
  return rv;
};
function System__Web__Html__DomDataCache__IsW3wc(element) {
  return !!element.addEventListener;
};
function System__Web__Html__DomDataCache____cctor() {
  System__Web__Html__DomDataCache__instance = System_Collections_Generic_NumberDictionary_$DomDataCache$_.defaultConstructor();
  System__Web__Html__DomDataCache__cacheId = 0;
};
function System__Web__Html__DomDataCache_factory(element) {
  var this_;
  this_ = new System_Web_Html_DomDataCache();
  this_.__ctor(element);
  return this_;
};
ptyp_ = System_Web_Html_DomDataCache.prototype;
ptyp_.capturePhaseEvents = null;
ptyp_.bubblePhaseEvents = null;
ptyp_.dataDictionary = null;
ptyp_.target = null;
ptyp_.__ctor = function System__Web__Html__DomDataCache____ctor(element) {
  this.capturePhaseEvents = System_Collections_Generic_StringDictionary_$Action_$Event$_$_.defaultConstructor();
  this.bubblePhaseEvents = System_Collections_Generic_StringDictionary_$Action_$Event$_$_.defaultConstructor();
  this.dataDictionary = new Object();
  this.target = System__Web__Html__Node__As(element);
};
ptyp_.addEvent = function System__Web__Html__DomDataCache__AddEvent(name, action, onCapture) {
  var isW3wc, evts, elementEvent;
  isW3wc = System__Web__Html__DomDataCache__IsW3wc(this.target);
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
    if (onCapture && System__Web__Html__DomDataCache__IsW3wc(this.target))
      this.target.addEventListener(name, System__Delegate__Create("eventHandlerCapture", this), true);
    else if (isW3wc)
      this.target.addEventListener(name, System__Delegate__Create("eventHandlerBubble", this), false);
    else
      this.target.attachEvent("on" + name, System__Delegate__Create("eventHandlerIE", this));
  }
  else
    elementEvent = System__Delegate__Combine(elementEvent, action);
  evts.set_item(name, elementEvent);
};
ptyp_.removeEvent = function System__Web__Html__DomDataCache__RemoveEvent(name, handler, onCapture) {
  var isW3wc, evts, elementEvent;
  isW3wc = System__Web__Html__DomDataCache__IsW3wc(this.target);
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
        this.target.removeEventListener(name, System__Delegate__Create("eventHandlerCapture", this), true);
      else if (isW3wc)
        this.target.removeEventListener(name, System__Delegate__Create("eventHandlerBubble", this), false);
      else
        this.target.detachEvent("on" + name, System__Delegate__Create("eventHandlerIE", this));
    }
    else
      evts.set_item(name, elementEvent);
  }
};
ptyp_.eventHandlerIE = function System__Web__Html__DomDataCache__EventHandlerIE() {
  this.eventHandlerBubble(event);
};
ptyp_.eventHandlerCapture = function System__Web__Html__DomDataCache__EventHandlerCapture(evt) {
  this.capturePhaseEvents.get_item(evt.type)(evt);
};
ptyp_.eventHandlerBubble = function System__Web__Html__DomDataCache__EventHandlerBubble(evt) {
  this.bubblePhaseEvents.get_item(evt.type)(evt);
};
System__Type__RegisterReferenceType(System_Web_Html_DomDataCache, "System.Web.Html.DomDataCache", Object, []);
function System_ValueType() {
};
System_ValueType.typeId = "o";
System__Type__RegisterReferenceType(System_ValueType, "System.ValueType", Object, []);
function System_Int32(boxedValue) {
  this.boxedValue = boxedValue;
};
System_Int32.typeId = "p";
System_Int32.getDefaultValue = function() {
  return 0;
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
function System__Math__Random(number) {
  return Math.floor(Math.random() * number) | 0;
};
Date.typeId = "q";
function System__DateTime__get_Now() {
  return new Date();
};
function System__DateTime____cctor() {
  Date.empty = new Date(0);
};
System__Type__RegisterReferenceType(Date, "System.DateTime", Object, []);
String.typeId = "r";
System__String__formatHelperRegex = null;
System__String__trimStartHelperRegex = null;
System__String__trimEndHelperRegex = null;
function System__String____cctor() {
  System__String__formatHelperRegex = new RegExp("(\\\\{[^\\\\}^\\\\{]+\\\\})", "g");
  System__String__trimStartHelperRegex = new RegExp("^\\\\s*");
  System__String__trimEndHelperRegex = new RegExp("\\\\s*$");
};
function System__String__Concat(s1, s2, s3, s4) {
  return s1.toString() + s2.toString() + s3.toString() + s4.toString();
};
System__Type__RegisterReferenceType(String, "System.String", Object, []);
RegExp.typeId = "s";
System__Type__RegisterReferenceType(RegExp, "System.RegularExpression", Object, []);
function System_Delegate() {
};
System_Delegate.typeId = "t";
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
System_MulticastDelegate.typeId = "u";
System_MulticastDelegate.prototype = new System_Delegate();
System__Type__RegisterReferenceType(System_MulticastDelegate, "System.MulticastDelegate", System_Delegate, []);
function System_Double(boxedValue) {
  this.boxedValue = boxedValue;
};
System_Double.typeId = "v";
System_Double.getDefaultValue = function() {
  return 0;
};
function System__Double__ToString(this_) {
  return this_.toString();
};
ptyp_ = new System_ValueType();
System_Double.prototype = ptyp_;
ptyp_.toString = function() {
  return System__Double__ToString(this.boxedValue);
};
System__Type__RegisterStructType(System_Double, "System.Double", []);
Object.typeId = "w";
System__Type__RegisterReferenceType(Object, "System.Collections.Dictionary", Object, []);
function System_Web_Html_DomList(T, $5fcallStatiConstructor) {
  var DomList$1_$T$_;
  if (System_Web_Html_DomList[T.typeId])
    return System_Web_Html_DomList[T.typeId];
  System_Web_Html_DomList[T.typeId] = function System__Web__Html__DomList$1() {
  };
  DomList$1_$T$_ = System_Web_Html_DomList[T.typeId];
  DomList$1_$T$_.genericParameters = [T];
  DomList$1_$T$_.genericClosure = System_Web_Html_DomList;
  DomList$1_$T$_.typeId = "x$" + T.typeId + "$";
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
function System_Collections_Generic_NumberDictionary(TValue, $5fcallStatiConstructor) {
  var NumberDictionary$1_$TValue$_;
  if (System_Collections_Generic_NumberDictionary[TValue.typeId])
    return System_Collections_Generic_NumberDictionary[TValue.typeId];
  System_Collections_Generic_NumberDictionary[TValue.typeId] = function System__Collections__Generic__NumberDictionary$1() {
  };
  NumberDictionary$1_$TValue$_ = System_Collections_Generic_NumberDictionary[TValue.typeId];
  NumberDictionary$1_$TValue$_.genericParameters = [TValue];
  NumberDictionary$1_$TValue$_.genericClosure = System_Collections_Generic_NumberDictionary;
  NumberDictionary$1_$TValue$_.typeId = "y$" + TValue.typeId + "$";
  NumberDictionary$1_$TValue$_.defaultConstructor = function System_Collections_Generic_NumberDictionary$1_factory() {
    var this_;
    this_ = new NumberDictionary$1_$TValue$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = NumberDictionary$1_$TValue$_.prototype;
  ptyp_.innerDict = null;
  ptyp_.__ctor = function System__Collections__Generic__NumberDictionary$1____ctor() {
    this.innerDict = new Object();
  };
  ptyp_.get_item = function System__Collections__Generic__NumberDictionary$1__get_Item(index) {
    if (!(index in this.innerDict))
      throw new Error("Key not found");
    return this.innerDict[index];
  };
  ptyp_.set_item = function System__Collections__Generic__NumberDictionary$1__set_Item(index, value) {
    this.innerDict[index] = value;
  };
  System__Type__RegisterReferenceType(NumberDictionary$1_$TValue$_, "System.Collections.Generic.NumberDictionary`1<" + TValue.fullName + ">", Object, []);
  return NumberDictionary$1_$TValue$_;
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
  StringDictionary$1_$TValue$_.typeId = "z$" + TValue.typeId + "$";
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
  Action$1_$T1$_.typeId = "A$" + T1.typeId + "$";
  Action$1_$T1$_.prototype = new System_MulticastDelegate();
  System__Type__RegisterReferenceType(Action$1_$T1$_, "System.Action`1<" + T1.fullName + ">", System_MulticastDelegate, []);
  return Action$1_$T1$_;
};
System_Web_Html_DomList_$Attr$_ = System_Web_Html_DomList(Attr);
System_Collections_Generic_NumberDictionary_$DomDataCache$_ = System_Collections_Generic_NumberDictionary(System_Web_Html_DomDataCache);
System_Action_$Event$_ = System_Action(Event);
System_Collections_Generic_StringDictionary_$Action_$Event$_$_ = System_Collections_Generic_StringDictionary(System_Action_$Event$_);
System__Web__Html__DomDataCache____cctor();
System__DateTime____cctor();
System__String____cctor();
module("System.Web.Html.Tests.TestElement");
test("TestCreateElement", 0, System__Web__Html__Tests__TestElement__TestCreateElement);
test("TestAttribute", 0, System__Web__Html__Tests__TestElement__TestAttribute);
test("TestEventBinding", 0, System__Web__Html__Tests__TestElement__TestEventBinding);
//@ sourceMappingURL=System.Web.Html.Tests.js.map
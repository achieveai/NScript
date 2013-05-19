function RealScript_GenericSamples() {
};
RealScript_GenericSamples.typeId = "b";
function RealScript__GenericSamples__NewGenericObject() {
  return RealScript_List_$GenericSamples$_.defaultConstructor();
};
function RealScript__GenericSamples__NewGenericObject2(T) {
  var List_$T$_;
  List_$T$_ = RealScript_List(T, true);
  return List_$T$_.defaultConstructor();
};
function RealScript__GenericSamples_factory() {
  return new RealScript_GenericSamples();
};
RealScript_GenericSamples.defaultConstructor = RealScript__GenericSamples_factory;
ptyp_ = RealScript_GenericSamples.prototype;
ptyp_.genericMethodCall = function RealScript__GenericSamples__GenericMethodCall(bar, foo) {
  return RealScript_TestGeneric_TestSubClass_$Int32$_.foo(Boolean, foo, foo);
};
ptyp_.genericMethodCall2 = function RealScript__GenericSamples__GenericMethodCall2(T, U, bar, foo) {
  var TestSubClass_$T$_;
  TestSubClass_$T$_ = RealScript_TestGeneric_TestSubClass(T, true);
  return TestSubClass_$T$_.foo(Boolean, foo, foo);
};
ptyp_.genericMethodCall3 = function RealScript__GenericSamples__GenericMethodCall3(T, U, bar, foo) {
  var TestSubClass2_$T_x_T$_;
  TestSubClass2_$T_x_T$_ = RealScript_TestGeneric_TestSubClass2(T, U, true);
  return TestSubClass2_$T_x_T$_.foo(Boolean, bar, foo, true);
};
ptyp_.getDefaultValue = function RealScript__GenericSamples__GetDefaultValue(T) {
  return System__Type__GetDefaultValueStatic(T);
};
ptyp_.__ctor = function RealScript__GenericSamples____ctor() {
};
System__Type__RegisterReferenceType(RealScript_GenericSamples, "RealScript.GenericSamples", Object, []);
function RealScript_List(T, $5fcallStatiConstructor) {
  var List$1_$T$_, IList$1_$T$_;
  if (RealScript_List[T.typeId])
    return RealScript_List[T.typeId];
  RealScript_List[T.typeId] = function RealScript__List$1() {
  };
  List$1_$T$_ = RealScript_List[T.typeId];
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = RealScript_List;
  List$1_$T$_.typeId = "c$" + T.typeId + "$";
  IList$1_$T$_ = RealScript_IList(T, $5fcallStatiConstructor);
  List$1_$T$_.defaultConstructor = function RealScript_List$1_factory() {
    return new List$1_$T$_();
  };
  ptyp_ = List$1_$T$_.prototype;
  ptyp_.get_item = function RealScript__List$1__get_Item(i) {
    throw new Error("Not Implemented");
  };
  ptyp_.set_item = function RealScript__List$1__set_Item(i, value) {
    throw new Error("Not Implemented");
  };
  ptyp_.get_count = function RealScript__List$1__get_Count() {
    throw new Error("Not Implemented");
  };
  ptyp_.set_count = function RealScript__List$1__set_Count(value) {
    throw new Error("Not Implemented");
  };
  ptyp_.add = function RealScript__List$1__Add(elem) {
    throw new Error("Not Implemented");
  };
  ptyp_.removeAt = function RealScript__List$1__RemoveAt(index) {
    throw new Error("Not Implemented");
  };
  ptyp_.insertAt = function RealScript__List$1__InsertAt(elem, index) {
    throw new Error("Not Implemented");
  };
  ptyp_.__ctor = function RealScript__List$1____ctor() {
  };
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  ptyp_["V_get_Count_" + IList$1_$T$_.typeId] = ptyp_.get_count;
  ptyp_["V_set_Count_" + IList$1_$T$_.typeId] = ptyp_.set_count;
  ptyp_["V_Add_" + IList$1_$T$_.typeId] = function(arg0) {
    return this.V_Add(arg0);
  };
  ptyp_["V_RemoveAt_" + IList$1_$T$_.typeId] = function(arg0) {
    return this.V_RemoveAt(arg0);
  };
  ptyp_["V_InsertAt_" + IList$1_$T$_.typeId] = function(arg0, arg1) {
    return this.V_InsertAt(arg0, arg1);
  };
  ptyp_.V_Add = ptyp_.add;
  ptyp_.V_RemoveAt = ptyp_.removeAt;
  ptyp_.V_InsertAt = ptyp_.insertAt;
  System__Type__RegisterReferenceType(List$1_$T$_, "RealScript.List`1<" + T.fullName + ">", Object, [IList$1_$T$_]);
  return List$1_$T$_;
};
function RealScript_GenericSamplesList() {
};
RealScript_GenericSamplesList.typeId = "d";
function RealScript__GenericSamplesList_factory(count) {
  var this_;
  this_ = new RealScript_GenericSamplesList();
  this_.__ctora(count);
  return this_;
};
RealScript_List_$GenericSamples$_ = RealScript_List(RealScript_GenericSamples);
ptyp_ = new RealScript_List_$GenericSamples$_();
RealScript_GenericSamplesList.prototype = ptyp_;
ptyp_.__ctora = function RealScript__GenericSamplesList____ctor(count) {
  this.__ctor();
};
ptyp_.insertAta = function RealScript__GenericSamplesList__InsertAt(elem, index) {
  this.insertAt(elem, index);
};
ptyp_.removeAta = function RealScript__GenericSamplesList__RemoveAt(index) {
  this.removeAt(index);
};
ptyp_.V_RemoveAt = ptyp_.removeAta;
ptyp_.V_InsertAt = ptyp_.insertAta;
System__Type__RegisterReferenceType(RealScript_GenericSamplesList, "RealScript.GenericSamplesList", RealScript_List_$GenericSamples$_, []);
function RealScript_TestGeneric() {
};
RealScript_TestGeneric.typeId = "e";
function RealScript__TestGeneric_factory() {
  return new RealScript_TestGeneric();
};
RealScript_TestGeneric.defaultConstructor = RealScript__TestGeneric_factory;
ptyp_ = RealScript_TestGeneric.prototype;
ptyp_.foo = function RealScript__TestGeneric__Foo(bar) {
  return bar;
};
ptyp_.genericTypeMethod = function RealScript__TestGeneric__GenericTypeMethod(bar) {
  return RealScript_TestGeneric_$Int32$_.defaultConstructor().foo(bar);
};
ptyp_.fooa = function RealScript__TestGeneric__Fooa(Too, Uoo, bar, var2) {
  return bar;
};
ptyp_.foob = function RealScript__TestGeneric__Foob(T, U, bar, foo) {
  var TestSubClass_$T$_;
  TestSubClass_$T$_ = RealScript_TestGeneric_TestSubClass(T, true);
  return TestSubClass_$T$_.foo(Boolean, foo, foo);
};
ptyp_.__ctor = function RealScript__TestGeneric____ctor() {
};
System__Type__RegisterReferenceType(RealScript_TestGeneric, "RealScript.TestGeneric", Object, []);

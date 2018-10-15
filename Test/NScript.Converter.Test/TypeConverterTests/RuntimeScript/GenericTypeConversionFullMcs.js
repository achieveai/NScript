function GenericSamples() {
};
GenericSamples.typeId = "b";
function GenericSamples__NewGenericObject() {
  return List_$GenericSamples$_.defaultConstructor();
};
function GenericSamples__NewGenericObject2(T) {
  var List_$T$_;
  List_$T$_ = List(T, true);
  return List_$T$_.defaultConstructor();
};
function GenericSamples_factory() {
  var this_;
  this_ = new GenericSamples();
  this_.__ctor();
  return this_;
};
GenericSamples.defaultConstructor = GenericSamples_factory;
ptyp_ = GenericSamples.prototype;
ptyp_.genericMethodCall = function GenericSamples__GenericMethodCall(bar, foo) {
  return TestSubClass_$Int32$_.foo(Booleana, foo, foo);
};
ptyp_.genericMethodCall2 = function GenericSamples__GenericMethodCall2(T, U, bar, foo) {
  var TestSubClass_$T$_;
  TestSubClass_$T$_ = TestSubClass(T, true);
  return TestSubClass_$T$_.foo(Booleana, foo, foo);
};
ptyp_.genericMethodCall3 = function GenericSamples__GenericMethodCall3(T, U, bar, foo) {
  var TestSubClass2_$U_x_T$_;
  TestSubClass2_$U_x_T$_ = TestSubClass2(U, U, true);
  return TestSubClass2_$U_x_T$_.foo(Booleana, bar, foo, true);
};
ptyp_.getDefaultValue = function GenericSamples__GetDefaultValue(T) {
  return Type__GetDefaultValueStatic(T);
};
ptyp_.__ctor = function GenericSamples____ctor() {
};
Type__RegisterReferenceType(GenericSamples, "RealScript.GenericSamples", Object, []);
function List(T, _callStatiConstructor) {
  var List$1_$T$_, IList$1_$T$_;
  if (List[T.typeId])
    return List[T.typeId];
  List[T.typeId] = function RealScript__List$1() {
  };
  List$1_$T$_ = List[T.typeId];
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = List;
  List$1_$T$_.typeId = "c$" + T.typeId + "$";
  IList$1_$T$_ = IList(T, _callStatiConstructor);
  List$1_$T$_.defaultConstructor = function RealScript_List$1_factory() {
    var this_;
    this_ = new List$1_$T$_();
    this_.__ctor();
    return this_;
  };
  ptyp_ = List$1_$T$_.prototype;
  ptyp_.get_item = function List$1__get_Item(i) {
    throw new Error("Not Implemented");
  };
  ptyp_.set_item = function List$1__set_Item(i, value) {
    throw new Error("Not Implemented");
  };
  ptyp_.get_count = function List$1__get_Count() {
    throw new Error("Not Implemented");
  };
  ptyp_.set_count = function List$1__set_Count(value) {
    throw new Error("Not Implemented");
  };
  ptyp_.add = function List$1__Add(elem) {
    throw new Error("Not Implemented");
  };
  ptyp_.removeAt = function List$1__RemoveAt(index) {
    throw new Error("Not Implemented");
  };
  ptyp_.insertAt = function List$1__InsertAt(elem, index) {
    throw new Error("Not Implemented");
  };
  ptyp_.__ctor = function List$1____ctor() {
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
  Type__RegisterReferenceType(List$1_$T$_, "RealScript.List`1<" + T.fullName + ">", Object, [IList$1_$T$_]);
  return List$1_$T$_;
};
function GenericSamplesList() {
};
GenericSamplesList.typeId = "d";
function GenericSamplesList_factory(count) {
  var this_;
  this_ = new GenericSamplesList();
  this_.__ctora(count);
  return this_;
};
List_$GenericSamples$_ = List(GenericSamples);
ptyp_ = new List_$GenericSamples$_();
GenericSamplesList.prototype = ptyp_;
ptyp_.__ctora = function GenericSamplesList____ctor(count) {
  this.__ctor();
};
ptyp_.insertAta = function GenericSamplesList__InsertAt(elem, index) {
  this.insertAt(elem, index);
};
ptyp_.removeAta = function GenericSamplesList__RemoveAt(index) {
  this.removeAt(index);
};
ptyp_.V_RemoveAt = ptyp_.removeAta;
ptyp_.V_InsertAt = ptyp_.insertAta;
Type__RegisterReferenceType(GenericSamplesList, "RealScript.GenericSamplesList", List_$GenericSamples$_, []);
function TestGeneric() {
};
TestGeneric.typeId = "e";
function TestGeneric_factory() {
  var this_;
  this_ = new TestGeneric();
  this_.__ctor();
  return this_;
};
TestGeneric.defaultConstructor = TestGeneric_factory;
ptyp_ = TestGeneric.prototype;
ptyp_.foo = function TestGeneric__Foo(bar) {
  return bar;
};
ptyp_.genericTypeMethod = function TestGeneric__GenericTypeMethod(bar) {
  return TestGeneric_$Int32$_.defaultConstructor().foo(bar);
};
ptyp_.fooa = function TestGeneric__Fooa(Too, Uoo, bar, var2) {
  return bar;
};
ptyp_.foob = function TestGeneric__Foob(T, U, bar, foo) {
  var TestSubClass_$T$_;
  TestSubClass_$T$_ = TestSubClass(T, true);
  return TestSubClass_$T$_.foo(Booleana, foo, foo);
};
ptyp_.__ctor = function TestGeneric____ctor() {
};
Type__RegisterReferenceType(TestGeneric, "RealScript.TestGeneric", Object, []);

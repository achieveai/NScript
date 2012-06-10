System_Type.registerNamespace("RealScript");
RealScript_List$1 = function(T) {
  var List$1, IList$1_$T$_;
  if (RealScript_List$1[T.typeId])
    return RealScript_List$1[T.typeId];
  RealScript_List$1[T.typeId] = function RealScript__List$1() {
    this.__ctor();
  };
  List$1 = RealScript_List$1[T.typeId];
  RealScript_List$1.typeId = "b$" + T.typeId + "$";
  IList$1_$T$_ = RealScript_IList$1[T.typeId];
  List$1.registerClass("RealScript.List`1<" + T.fullName + ">", null, IList$1_$T$_);
  List$1.prototype = {
    get_item: function RealScript__List$1__get_Item(i) {
      throw new Error("Not Implemented");
    },
    set_item: function RealScript__List$1__set_Item(i, value) {
      throw new Error("Not Implemented");
    },
    get_count: function RealScript__List$1__get_Count() {
      throw new Error("Not Implemented");
    },
    set_count: function RealScript__List$1__set_Count(value) {
      throw new Error("Not Implemented");
    },
    add: function RealScript__List$1__Add(elem) {
      throw new Error("Not Implemented");
    },
    removeAt: function RealScript__List$1__RemoveAt(index) {
      throw new Error("Not Implemented");
    },
    insertAt: function RealScript__List$1__InsertAt(elem, index) {
      throw new Error("Not Implemented");
    },
    __ctor: function RealScript__List$1____ctor() {
      return this;
    }
  };
  (function(ptype) {
    ptype["V_get_Item_" + IList$1_$T$_.typeId] = ptype.get_item;
    ptype["V_set_Item_" + IList$1_$T$_.typeId] = ptype.set_item;
    ptype["V_get_Count_" + IList$1_$T$_.typeId] = ptype.get_count;
    ptype["V_set_Count_" + IList$1_$T$_.typeId] = ptype.set_count;
    ptype["V_Add_" + IList$1_$T$_.typeId] = function(arg0) {
      return this.V_Add(arg0);
    };
    ptype.V_Add = ptype.add;
    ptype["V_RemoveAt_" + IList$1_$T$_.typeId] = function(arg0) {
      return this.V_RemoveAt(arg0);
    };
    ptype.V_RemoveAt = ptype.removeAt;
    ptype["V_InsertAt_" + IList$1_$T$_.typeId] = function(arg0, arg1) {
      return this.V_InsertAt(arg0, arg1);
    };
    ptype.V_InsertAt = ptype.insertAt;
  })(RealScript_List$1.prototype);
};
RealScript_GenericSamplesList = function RealScript__GenericSamplesList(count) {
  this.__ctora(count);
};
RealScript_GenericSamplesList.typeId = "c";
RealScript_GenericSamplesList.prototype = {
  __ctora: function RealScript__GenericSamplesList____ctor(count) {
    this.__ctor();
    return this;
  },
  insertAta: function RealScript__GenericSamplesList__InsertAt(elem, index) {
    this.insertAt(elem, index);
  },
  removeAta: function RealScript__GenericSamplesList__RemoveAt(index) {
    this.removeAt(index);
  }
};
(function(ptype) {
  ptype.V_InsertAt = ptype.insertAta;
  ptype.V_RemoveAt = ptype.removeAta;
})(RealScript_GenericSamplesList.prototype);
RealScript_GenericSamples = function RealScript__GenericSamples() {
  this.__ctor();
};
RealScript_GenericSamples.typeId = "d";
RealScript_GenericSamples.newGenericObject = function RealScript__GenericSamples__NewGenericObject() {
  return new RealScript_List_$GenericSamples$_();
};
RealScript_GenericSamples.newGenericObject2 = function RealScript__GenericSamples__NewGenericObject2(T) {
  var List_$T$_;
  List_$T$_ = RealScript_List$1[T.typeId];
  return new List_$T$_();
};
RealScript_GenericSamples.prototype = {
  genericMethodCall: function RealScript__GenericSamples__GenericMethodCall(bar, foo) {
    return TestGeneric_$Number$_.foo(Boolean, foo, foo);
  },
  genericMethodCall2: function RealScript__GenericSamples__GenericMethodCall2(T, U, bar, foo) {
    var TestGeneric_$T$_;
    TestGeneric_$T$_ = RealScript_TestGeneric$1$24TestSubClass[T.typeId];
    return TestGeneric_$T$_.foo(Boolean, foo, foo);
  },
  getDefaultValue: function RealScript__GenericSamples__GetDefaultValue(T) {
    var $24v$241;
    $24v$241 = Type.getDefaultValue(T);
    return $24v$241;
  },
  __ctor: function RealScript__GenericSamples____ctor() {
    return this;
  }
};
RealScript_TestGeneric = function RealScript__TestGeneric() {
  this.__ctor();
};
RealScript_TestGeneric.typeId = "e";
RealScript_TestGeneric.prototype = {
  foo: function RealScript__TestGeneric__Foo(bar) {
    return bar;
  },
  fooa: function RealScript__TestGeneric__Fooa(Too, Uoo, bar, var2) {
    return bar;
  },
  foob: function RealScript__TestGeneric__Foob(T, U, bar, foo) {
    var TestGeneric_$T$_;
    TestGeneric_$T$_ = RealScript_TestGeneric$1$24TestSubClass[T.typeId];
    return TestGeneric_$T$_.foo(Boolean, foo, foo);
  },
  __ctor: function RealScript__TestGeneric____ctor() {
    return this;
  }
};
RealScript_List_$GenericSamples$_ = RealScript_List_$GenericSamples$_(RealScript_GenericSamples);
RealScript_GenericSamplesList.registerClass("RealScript.GenericSamplesList", RealScript_List_$GenericSamples$_);
RealScript_GenericSamples.registerClass("RealScript.GenericSamples", null);
RealScript_TestGeneric.registerClass("RealScript.TestGeneric", null);

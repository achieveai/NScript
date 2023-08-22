function List(T, _callStatiConstructor) {
  var List$1_$T$_, IList$1_$T$_;
  if (List["9" + T.typeId])
    return List["9" + T.typeId];
  List["9" + T.typeId] = List$1_$T$_ = function RealScript__List$1() { };
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = List;
  List$1_$T$_.typeId = "b$" + T.typeId + "$";
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
  ptyp_.__ctor = function List$1____ctor() { };
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
}
Array.typeId = "b";
function Array__Sort(T, array, comparator) {
  var ArrayG_$T$_;
  ArrayG_$T$_ = ArrayG(T, true);
  array.sort(comparator);
};
Type__RegisterReferenceType(Array, "System.Array", Object, [IList, ICollection, IEnumerable]);
function NativeArray$1__GetNativeArray(array) {
  return array ? array.innerArray : null;
};
function NativeArray$1__GetNativeArraya(array) {
  return array ? array.nativeArray : null;
};
function NativeArray$1__Concat(this_, arrays) {
  return this_.concat.apply(this_, arrays.nativeArray);
};
function NativeArray$1__Concata(this_, array) {
  return this_.concat(array);
};
function NativeArray$1__Concatb(this_, array, array2) {
  return this_.concat(array, array2);
};
function NativeArray$1__Concatc(this_, array, array2, array3) {
  return this_.concat(array, array2, array3);
};
function NativeArray$1__Splice(this_, index, howMany, elements) {
  var newArray, rv;
  newArray = elements.innerArray;
  newArray.unshift(howMany);
  newArray.unshift(index);
  rv = this_.splice.apply(this_, newArray);
  newArray.shift();
  newArray.shift();
  return rv;
};
function NativeArray$1__IndexOf(this_, value, startIndex) {
  startIndex = startIndex < 0 ? 0 : startIndex;
  return this_.indexOf(value, startIndex);
};
function NativeArray$1__InsertAt(this_, index, value) {
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  this_.splice(index, 0, value);
};
function NativeArray$1__InsertRangeAt(this_, index, values) {
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  values.unshift(0);
  values.unshift(index);
  this_.splice.apply(this_, values);
  values.shift();
  values.shift();
};
function NativeArray$1__RemoveAt(this_, index) {
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  this_.splice(index, 1);
};
function NativeArray$1__op_Implicit(n) {
  return !n ? null : n.get_innerArray();
};
function List(T, _callStatiConstructor) {
  var List$1_$T$_, ArrayG$1_$T$_, T$5b$5d_$T$_, ListEnumerator$1_$T$_, IList$1_$T$_, ICollection$1_$T$_, IEnumerable$1_$T$_, __initTracker;
  if (List[T.typeId])
    return List[T.typeId];
  List[T.typeId] = function System__Collections__Generic__List$1() {
  };
  List$1_$T$_ = List[T.typeId];
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = List;
  List$1_$T$_.typeId = "f$" + T.typeId + "$";
  IList$1_$T$_ = ILista(T, _callStatiConstructor);
  ICollection$1_$T$_ = ICollectiona(T, _callStatiConstructor);
  IEnumerable$1_$T$_ = IEnumerablea(T, _callStatiConstructor);
  List$1_$T$_.op_Implicit = function List$1__op_Implicit(n) {
    return n.nativeArray;
  };
  List$1_$T$_.op_Implicita = function List$1__op_Implicita(n) {
    return !n ? null : List$1_$T$_.__ctor(n);
  };
  List$1_$T$_.defaultConstructor = function System_Collections_Generic_List$1_factory() {
    var this_;
    this_ = new List$1_$T$_();
    this_.__ctor();
    return this_;
  };
  List$1_$T$_.__ctor = function System_Collections_Generic_List$1_factorya(nativeArray) {
    var this_;
    this_ = new List$1_$T$_();
    this_.__ctora(nativeArray);
    return this_;
  };
  List$1_$T$_.__ctora = function System_Collections_Generic_List$1_factoryb(array) {
    var this_;
    this_ = new List$1_$T$_();
    this_.__ctorb(array);
    return this_;
  };
  ptyp_ = List$1_$T$_.prototype;
  ptyp_.nativeArray = null;
  ptyp_.__ctor = function List$1____ctor() {
    this.nativeArray = new Array(0);
  };
  ptyp_.__ctora = function List$1____ctora(nativeArray) {
    this.nativeArray = nativeArray;
  };
  ptyp_.__ctorb = function List$1____ctorb(array) {
    var arrayNativeArray, i; {
      arrayNativeArray = NativeArray$1__GetNativeArray(array);
      if (true)
        this.nativeArray = NativeArray$1__GetNativeArray(array).slice(0, arrayNativeArray.length);
      else {
        this.nativeArray = new Array(arrayNativeArray.length);
        for (i = arrayNativeArray.length - 1; i >= 0; i--)
          this.nativeArray[i] = arrayNativeArray[i];
      }
    }
  };
  ptyp_.get_item = function List$1__get_Item(index) {
    var arr;
    arr = this.nativeArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index];
  };
  ptyp_.set_item = function List$1__set_Item(index, value) {
    var arr;
    arr = this.nativeArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index] = value;
  };
  ptyp_.get_innerArray = function List$1__get_InnerArray() {
    return this.nativeArray;
  };
  ptyp_.indexOf = function List$1__IndexOf(item) {
    return NativeArray$1__IndexOf(this.nativeArray, item, 0);
  };
  ptyp_.system__Collections__IList__IndexOf = function List$1__System__Collections__IList__IndexOf(value) {
    if (value === null && T.isInstanceOfType(value))
      return NativeArray$1__IndexOf(this.nativeArray, Type__UnBoxTypeInstance(T, value), 0);
    return -1;
  };
  ptyp_.insert = function List$1__Insert(index, item) {
    NativeArray$1__InsertAt(this.nativeArray, index, item);
  };
  ptyp_.system__Collections__IList__Insert = function List$1__System__Collections__IList__Insert(index, value) {
    this.insert(index, Type__UnBoxTypeInstance(T, value));
  };
  ptyp_.insertRange = function List$1__InsertRange(index, items) {
    NativeArray$1__InsertRangeAt(this.nativeArray, index, NativeArray$1__GetNativeArray(items));
  };
  ptyp_.insertRangea = function List$1__InsertRangea(index, items) {
    NativeArray$1__InsertRangeAt(this.nativeArray, index, items.nativeArray);
  };
  ptyp_.insertRangeb = function List$1__InsertRangeb(index, items) {
    var stmtTemp1, item;
    for (stmtTemp1 = items.V_GetEnumerator_c(); stmtTemp1.V_MoveNext_d(); ) {
      item = Type__UnBoxTypeInstance(T, stmtTemp1.V_get_Current_d());
      NativeArray$1__InsertAt(this.nativeArray, index++, item);
    }
  };
  ptyp_.removeAt = function List$1__RemoveAt(index) {
    NativeArray$1__RemoveAt(this.nativeArray, index);
  };
  ptyp_.get_count = function List$1__get_Count() {
    return this.nativeArray.length;
  };
  ptyp_.system__Collections__Generic__ICollection_$T$___get_IsReadOnly = function List$1__System__Collections__Generic__ICollection_$T$___get_IsReadOnly() {
    return false;
  };
  ptyp_.system__Collections__IList__get_IsReadOnly = function List$1__System__Collections__IList__get_IsReadOnly() {
    return false;
  };
  ptyp_.add = function List$1__Add(item) {
    this.nativeArray.push(item);
  };
  ptyp_.system__Collections__IList__Add = function List$1__System__Collections__IList__Add(value) {
    this.add(Type__UnBoxTypeInstance(T, value));
  };
  ptyp_.addRange = function List$1__AddRange(items) {
    this.nativeArray = NativeArray$1__Concata(this.nativeArray, NativeArray$1__GetNativeArray(items));
  };
  ptyp_.addRangea = function List$1__AddRangea(items) {
    this.nativeArray = NativeArray$1__Concata(this.nativeArray, items.nativeArray);
  };
  ptyp_.addRangeb = function List$1__AddRangeb(items) {
    var stmtTemp1, item;
    for (stmtTemp1 = items.V_GetEnumerator_c(); stmtTemp1.V_MoveNext_d(); ) {
      item = Type__UnBoxTypeInstance(T, stmtTemp1.V_get_Current_d());
      this.nativeArray.push(item);
    }
  };
  ptyp_.clear = function List$1__Clear() {
    this.nativeArray.length = 0;
  };
  ptyp_.contains = function List$1__Contains(item) {
    return NativeArray$1__IndexOf(this.nativeArray, item, 0) >= 0;
  };
  ptyp_.copyTo = function List$1__CopyTo(arr, index) {
    var nativeArray, length, i;
    nativeArray = this.nativeArray;
    length = nativeArray.length;
    for (i = 0; i < length; i++)
      arr.set_item(i + index, nativeArray[i]);
  };
  ptyp_.system__Collections__ICollection__CopyTo = function List$1__System__Collections__ICollection__CopyTo(array, index) {
    var nativeArray, length, i;
    nativeArray = this.nativeArray;
    length = nativeArray.length;
    for (i = 0; i < length; i++)
      array.setValue(i + index, Type__BoxTypeInstance(T, nativeArray[i]));
  };
  ptyp_.toArray = function List$1__ToArray() {
    return ArrayG$1_$T$_.__ctor(this.nativeArray.slice(0, this.nativeArray.length));
  };
  ptyp_.remove = function List$1__Remove(item) {
    var index;
    index = NativeArray$1__IndexOf(this.nativeArray, item, 0);
    if (index >= 0)
      NativeArray$1__RemoveAt(this.nativeArray, index);
    return index >= 0;
  };
  ptyp_.system__Collections__IList__Remove = function List$1__System__Collections__IList__Remove(value) {
    if (value === null && T.isInstanceOfType(value))
      this.remove(Type__UnBoxTypeInstance(T, value));
  };
  ptyp_.sort = function List$1__Sort(sortFunction) {
    this.nativeArray.sort(sortFunction);
  };
  ptyp_.getEnumerator = function List$1__GetEnumerator() {
    return ListEnumerator$1_$T$_.__ctor(this);
  };
  ptyp_.system__Collections__IEnumerable__GetEnumerator = function List$1__System__Collections__IEnumerable__GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.system__Collections__IList__get_IsFixedSize = function List$1__System__Collections__IList__get_IsFixedSize() {
    return false;
  };
  ptyp_.system__Collections__IList__get_Item = function List$1__System__Collections__IList__get_Item(index) {
    return Type__BoxTypeInstance(T, this.get_item(index));
  };
  ptyp_.system__Collections__IList__set_Item = function List$1__System__Collections__IList__set_Item(index, value) {
    this.set_item(index, Type__UnBoxTypeInstance(T, value));
  };
  ptyp_.system__Collections__IList__Contains = function List$1__System__Collections__IList__Contains(value) {
    return this.V_IndexOf_e(value) >= 0;
  };
  ptyp_.V_IndexOf_e = ptyp_.system__Collections__IList__IndexOf;
  ptyp_.V_Insert_e = ptyp_.system__Collections__IList__Insert;
  ptyp_["V_get_IsReadOnly_" + ICollection$1_$T$_.typeId] = ptyp_.system__Collections__Generic__ICollection_$T$___get_IsReadOnly;
  ptyp_.V_get_IsReadOnly_e = ptyp_.system__Collections__IList__get_IsReadOnly;
  ptyp_.V_Add_e = ptyp_.system__Collections__IList__Add;
  ptyp_.V_CopyTo_g = ptyp_.system__Collections__ICollection__CopyTo;
  ptyp_.V_Remove_e = ptyp_.system__Collections__IList__Remove;
  ptyp_.V_GetEnumerator_c = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_.V_get_IsFixedSize_e = ptyp_.system__Collections__IList__get_IsFixedSize;
  ptyp_.V_get_Item_e = ptyp_.system__Collections__IList__get_Item;
  ptyp_.V_set_Item_e = ptyp_.system__Collections__IList__set_Item;
  ptyp_.V_Contains_e = ptyp_.system__Collections__IList__Contains;
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  ptyp_["V_IndexOf_" + IList$1_$T$_.typeId] = ptyp_.indexOf;
  ptyp_["V_Insert_" + IList$1_$T$_.typeId] = ptyp_.insert;
  ptyp_.V_Clear_e = ptyp_.clear;
  ptyp_.V_RemoveAt_e = ptyp_.removeAt;
  ptyp_.V_get_Count_g = ptyp_.get_count;
  ptyp_["V_Add_" + ICollection$1_$T$_.typeId] = ptyp_.add;
  ptyp_["V_Clear_" + ICollection$1_$T$_.typeId] = ptyp_.clear;
  ptyp_["V_Contains_" + ICollection$1_$T$_.typeId] = ptyp_.contains;
  ptyp_["V_CopyTo_" + ICollection$1_$T$_.typeId] = ptyp_.copyTo;
  ptyp_["V_Remove_" + ICollection$1_$T$_.typeId] = ptyp_.remove;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.getEnumerator;
  Type__RegisterReferenceType(List$1_$T$_, "System.Collections.Generic.List`1<" + T.fullName + ">", Object, [IList$1_$T$_, IList, ICollection, IEnumerable, ICollection$1_$T$_, IEnumerable$1_$T$_]);
  List$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    ArrayG$1_$T$_ = ArrayG(T, true);
    T$5b$5d_$T$_ = ArrayG(T, true);
    ListEnumerator$1_$T$_ = ListEnumerator(T, true);
  };
  if (_callStatiConstructor)
    List$1_$T$_._tri();
  return List$1_$T$_;
};
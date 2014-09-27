Array.typeId = "b";
function System__Array__Sort(T, array, comparator) {
  var ArrayG_$T$_;
  ArrayG_$T$_ = System_ArrayG(T, true);
  array.sort(comparator);
};
System__Type__RegisterReferenceType(Array, "System.Array", Object, [System_Collections_IList, System_Collections_IEnumerable, System_Collections_ICollection]);
function System__NativeArray$1__GetArray(this_) {
  return ArrayG$1_$T$_.__ctor(this_);
};
function System__NativeArray$1__GetNativeArray(array) {
  return array ? array.innerArray : null;
};
function System__NativeArray$1__GetNativeArraya(array) {
  return array ? array.nativeArray : null;
};
function System__NativeArray$1__Concat(this_, arrays) {
  return this_.concat.apply(this_, arrays.nativeArray);
};
function System__NativeArray$1__Concata(this_, array) {
  return this_.concat(array);
};
function System__NativeArray$1__Concatb(this_, array, array2) {
  return this_.concat(array, array2);
};
function System__NativeArray$1__Concatc(this_, array, array2, array3) {
  return this_.concat(array, array2, array3);
};
function System__NativeArray$1__Pop(this_) {
  return this_.pop();
};
function System__NativeArray$1__Push(this_, value) {
  return this_.push(value);
};
function System__NativeArray$1__Shift(this_) {
  return this_.shift();
};
function System__NativeArray$1__Unshift(this_, value) {
  return this_.unshift(value);
};
function System__NativeArray$1__Splice(this_, index, howMany, elements) {
  var newArray;
  newArray = elements.innerArray.slice(0);
  newArray.unshift(howMany);
  newArray.unshift(index);
  return this_.splice.apply(this_, newArray);
};
function System__NativeArray$1__IndexOf(this_, value, startIndex) {
  var i;
  startIndex = startIndex < 0 ? 0 : startIndex;
  for (i = this_.length; i >= startIndex && i >= 0; --i)
    if (this_[i] === value)
      return i;
  return -1;
};
function System__NativeArray$1__InsertAt(this_, index, value) {
  var i;
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  for (i = this_.length - 1; i >= index; i--)
    this_[i + 1] = this_[i];
  this_[index] = value;
};
function System__NativeArray$1__InsertRangeAt(this_, index, values) {
  var len, i;
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  len = values.length;
  for (i = this_.length - 1; i >= index; index--)
    this_[i + len] = this_[i];
  for (i = len - 1; --i; )
    this_[index + i] = values[i];
};
function System__NativeArray$1__RemoveAt(this_, index) {
  var len, i;
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  len = this_.length - 1;
  for (i = index; i < len; i++)
    this_[i] = this_[i + 1];
  this_.pop();
};
function System__NativeArray$1__op_Implicit(n) {
  return n.get_innerArray();
};
function System_Collections_Generic_List(T, $5fcallStatiConstructor) {
  var List$1_$T$_, ArrayG$1_$T$_, T$5b$5d_$T$_, ListEnumerator$1_$T$_, IList$1_$T$_, ICollection$1_$T$_, IEnumerable$1_$T$_, $5f_initTracker;
  if (System_Collections_Generic_List[T.typeId])
    return System_Collections_Generic_List[T.typeId];
  System_Collections_Generic_List[T.typeId] = function System__Collections__Generic__List$1() {
  };
  List$1_$T$_ = System_Collections_Generic_List[T.typeId];
  List$1_$T$_.genericParameters = [T];
  List$1_$T$_.genericClosure = System_Collections_Generic_List;
  List$1_$T$_.typeId = "f$" + T.typeId + "$";
  IList$1_$T$_ = System_Collections_Generic_IList(T, $5fcallStatiConstructor);
  ICollection$1_$T$_ = System_Collections_Generic_ICollection(T, $5fcallStatiConstructor);
  IEnumerable$1_$T$_ = System_Collections_Generic_IEnumerable(T, $5fcallStatiConstructor);
  List$1_$T$_.op_Implicit = function System__Collections__Generic__List$1__op_Implicit(n) {
    return n.nativeArray;
  };
  List$1_$T$_.op_Implicita = function System__Collections__Generic__List$1__op_Implicita(n) {
    return n ? List$1_$T$_.__ctor(n) : null;
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
  ptyp_.system__Collections__IList__IndexOf = function System__Collections__Generic__List$1__System__Collections__IList__IndexOf(value) {
    if (value === null && T.isInstanceOfType(value))
      return System__NativeArray$1__IndexOf(this.nativeArray, System__Type__UnBoxTypeInstance(T, value), 0);
    return -1;
  };
  ptyp_.system__Collections__IList__Insert = function System__Collections__Generic__List$1__System__Collections__IList__Insert(index, value) {
    this.insert(index, System__Type__UnBoxTypeInstance(T, value));
  };
  ptyp_.system__Collections__Generic__ICollection_$T$___get_IsReadOnly = function System__Collections__Generic__List$1__System__Collections__Generic__ICollection_$T$___get_IsReadOnly() {
    return false;
  };
  ptyp_.system__Collections__IList__get_IsReadOnly = function System__Collections__Generic__List$1__System__Collections__IList__get_IsReadOnly() {
    return false;
  };
  ptyp_.system__Collections__IList__Add = function System__Collections__Generic__List$1__System__Collections__IList__Add(value) {
    this.add(System__Type__UnBoxTypeInstance(T, value));
  };
  ptyp_.system__Collections__ICollection__CopyTo = function System__Collections__Generic__List$1__System__Collections__ICollection__CopyTo(array, index) {
    var nativeArray, length, i;
    nativeArray = this.nativeArray;
    length = nativeArray.length;
    for (i = 0; i < length; i++)
      array.setValue(i + index, System__Type__BoxTypeInstance(T, nativeArray[i]));
  };
  ptyp_.system__Collections__IList__Remove = function System__Collections__Generic__List$1__System__Collections__IList__Remove(value) {
    if (value === null && T.isInstanceOfType(value))
      this.remove(System__Type__UnBoxTypeInstance(T, value));
  };
  ptyp_.system__Collections__IEnumerable__GetEnumerator = function System__Collections__Generic__List$1__System__Collections__IEnumerable__GetEnumerator() {
    return this.getEnumerator();
  };
  ptyp_.system__Collections__IList__get_IsFixedSize = function System__Collections__Generic__List$1__System__Collections__IList__get_IsFixedSize() {
    return false;
  };
  ptyp_.system__Collections__IList__get_Item = function System__Collections__Generic__List$1__System__Collections__IList__get_Item(index) {
    return System__Type__BoxTypeInstance(T, this.get_item(index));
  };
  ptyp_.system__Collections__IList__set_Item = function System__Collections__Generic__List$1__System__Collections__IList__set_Item(index, value) {
    this.set_item(index, System__Type__UnBoxTypeInstance(T, value));
  };
  ptyp_.system__Collections__IList__Contains = function System__Collections__Generic__List$1__System__Collections__IList__Contains(value) {
    return System__Type__CastType(System_Collections_IList, this).V_IndexOf_c(value) >= 0;
  };
  ptyp_.__ctor = function System__Collections__Generic__List$1____ctor() {
    this.nativeArray = new Array(0);
  };
  ptyp_.__ctora = function System__Collections__Generic__List$1____ctora(nativeArray) {
    this.nativeArray = nativeArray;
  };
  ptyp_.__ctorb = function System__Collections__Generic__List$1____ctorb(array) {
    var arrayNativeArray, i;
    arrayNativeArray = System__NativeArray$1__GetNativeArray(array);
    if (true)
      this.nativeArray = System__NativeArray$1__GetNativeArray(array).slice(0, 0);
    else {
      this.nativeArray = new Array(arrayNativeArray.length);
      for (i = arrayNativeArray.length - 1; i >= 0; i--)
        this.nativeArray[i] = arrayNativeArray[i];
    }
  };
  ptyp_.get_item = function System__Collections__Generic__List$1__get_Item(index) {
    var arr;
    arr = this.nativeArray;
    if (index < 0 || index >= arr.length)
      throw "index " + index + " out of range";
    return arr[index];
  };
  ptyp_.set_item = function System__Collections__Generic__List$1__set_Item(index, value) {
    var arr;
    arr = this.nativeArray;
    if (index < 0 || index >= arr.length)
      throw "index " + index + " out of range";
    return arr[index] = value;
  };
  ptyp_.get_innerArray = function System__Collections__Generic__List$1__get_InnerArray() {
    return this.nativeArray;
  };
  ptyp_.indexOf = function System__Collections__Generic__List$1__IndexOf(item) {
    return System__NativeArray$1__IndexOf(this.nativeArray, item, 0);
  };
  ptyp_.insert = function System__Collections__Generic__List$1__Insert(index, item) {
    System__NativeArray$1__InsertAt(this.nativeArray, index, item);
  };
  ptyp_.insertRange = function System__Collections__Generic__List$1__InsertRange(index, items) {
    System__NativeArray$1__InsertRangeAt(this.nativeArray, index, System__NativeArray$1__GetNativeArray(items));
  };
  ptyp_.insertRangea = function System__Collections__Generic__List$1__InsertRangea(index, items) {
    System__NativeArray$1__InsertRangeAt(this.nativeArray, index, items.nativeArray);
  };
  ptyp_.insertRangeb = function System__Collections__Generic__List$1__InsertRangeb(index, items) {
    var stmtTemp1, item;
    for (stmtTemp1 = items.V_GetEnumerator_d(); stmtTemp1.V_MoveNext_e(); ) {
      item = System__Type__UnBoxTypeInstance(T, stmtTemp1.V_get_Current_e());
      System__NativeArray$1__InsertAt(this.nativeArray, index++, item);
    }
  };
  ptyp_.removeAt = function System__Collections__Generic__List$1__RemoveAt(index) {
    System__NativeArray$1__RemoveAt(this.nativeArray, index);
  };
  ptyp_.get_count = function System__Collections__Generic__List$1__get_Count() {
    return this.nativeArray.length;
  };
  ptyp_.add = function System__Collections__Generic__List$1__Add(item) {
    System__NativeArray$1__Push(this.nativeArray, item);
  };
  ptyp_.addRange = function System__Collections__Generic__List$1__AddRange(items) {
    this.nativeArray = System__NativeArray$1__Concata(this.nativeArray, System__NativeArray$1__GetNativeArray(items));
  };
  ptyp_.addRangea = function System__Collections__Generic__List$1__AddRangea(items) {
    this.nativeArray = System__NativeArray$1__Concata(this.nativeArray, items.nativeArray);
  };
  ptyp_.addRangeb = function System__Collections__Generic__List$1__AddRangeb(items) {
    var stmtTemp1, item;
    for (stmtTemp1 = items.V_GetEnumerator_d(); stmtTemp1.V_MoveNext_e(); ) {
      item = System__Type__UnBoxTypeInstance(T, stmtTemp1.V_get_Current_e());
      System__NativeArray$1__Push(this.nativeArray, item);
    }
  };
  ptyp_.clear = function System__Collections__Generic__List$1__Clear() {
    this.nativeArray.length = 0;
  };
  ptyp_.contains = function System__Collections__Generic__List$1__Contains(item) {
    return System__NativeArray$1__IndexOf(this.nativeArray, item, 0) >= 0;
  };
  ptyp_.copyTo = function System__Collections__Generic__List$1__CopyTo(arr, index) {
    var nativeArray, length, i;
    nativeArray = this.nativeArray;
    length = nativeArray.length;
    for (i = 0; i < length; i++)
      arr.set_item(i + index, nativeArray[i]);
  };
  ptyp_.toArray = function System__Collections__Generic__List$1__ToArray() {
    return ArrayG$1_$T$_.__ctor(this.nativeArray.slice(0, this.nativeArray.length));
  };
  ptyp_.remove = function System__Collections__Generic__List$1__Remove(item) {
    var index;
    index = System__NativeArray$1__IndexOf(this.nativeArray, item, 0);
    if (index >= 0)
      System__NativeArray$1__RemoveAt(this.nativeArray, index);
    return index >= 0;
  };
  ptyp_.sort = function System__Collections__Generic__List$1__Sort(sortFunction) {
    this.nativeArray.sort(sortFunction);
  };
  ptyp_.getEnumerator = function System__Collections__Generic__List$1__GetEnumerator() {
    return ListEnumerator$1_$T$_.__ctor(this);
  };
  ptyp_.V_IndexOf_c = ptyp_.system__Collections__IList__IndexOf;
  ptyp_.V_Insert_c = ptyp_.system__Collections__IList__Insert;
  ptyp_["V_get_IsReadOnly_" + ICollection$1_$T$_.typeId] = ptyp_.system__Collections__Generic__ICollection_$T$___get_IsReadOnly;
  ptyp_.V_get_IsReadOnly_c = ptyp_.system__Collections__IList__get_IsReadOnly;
  ptyp_.V_Add_c = ptyp_.system__Collections__IList__Add;
  ptyp_.V_CopyTo_g = ptyp_.system__Collections__ICollection__CopyTo;
  ptyp_.V_Remove_c = ptyp_.system__Collections__IList__Remove;
  ptyp_.V_GetEnumerator_d = ptyp_.system__Collections__IEnumerable__GetEnumerator;
  ptyp_.V_get_IsFixedSize_c = ptyp_.system__Collections__IList__get_IsFixedSize;
  ptyp_.V_get_Item_c = ptyp_.system__Collections__IList__get_Item;
  ptyp_.V_set_Item_c = ptyp_.system__Collections__IList__set_Item;
  ptyp_.V_Contains_c = ptyp_.system__Collections__IList__Contains;
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  ptyp_["V_IndexOf_" + IList$1_$T$_.typeId] = ptyp_.indexOf;
  ptyp_["V_Insert_" + IList$1_$T$_.typeId] = ptyp_.insert;
  ptyp_.V_Clear_c = ptyp_.clear;
  ptyp_.V_RemoveAt_c = ptyp_.removeAt;
  ptyp_["V_Add_" + ICollection$1_$T$_.typeId] = ptyp_.add;
  ptyp_["V_Clear_" + ICollection$1_$T$_.typeId] = ptyp_.clear;
  ptyp_["V_Contains_" + ICollection$1_$T$_.typeId] = ptyp_.contains;
  ptyp_["V_CopyTo_" + ICollection$1_$T$_.typeId] = ptyp_.copyTo;
  ptyp_["V_Remove_" + ICollection$1_$T$_.typeId] = ptyp_.remove;
  ptyp_.V_get_Count_g = ptyp_.get_count;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.getEnumerator;
  System__Type__RegisterReferenceType(List$1_$T$_, "System.Collections.Generic.List`1<" + T.fullName + ">", Object, [IList$1_$T$_, System_Collections_IList, ICollection$1_$T$_, System_Collections_ICollection, System_Collections_IEnumerable, IEnumerable$1_$T$_]);
  List$1_$T$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    ArrayG$1_$T$_ = System_ArrayG(T, true);
    T$5b$5d_$T$_ = System_ArrayG(T, true);
    ListEnumerator$1_$T$_ = System_Collections_Generic_ListEnumerator(T, true);
  };
  if ($5fcallStatiConstructor)
    List$1_$T$_._tri();
  return List$1_$T$_;
};
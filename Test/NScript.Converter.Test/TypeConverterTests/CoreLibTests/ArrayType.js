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
    if (this_[i] == value)
      return i;
  return -1;
};
function System__NativeArray$1__InsertAt(this_, index, value) {
  var i;
  if (index < 0 || index > this_.length)
    throw new Error("Index out of range");
  for (i = this_.length - 1; i >= index; index--)
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
function System_ArrayImpl() {
};
System_ArrayImpl.typeId = "c";
function System__ArrayImpl__Sort(T, array, comparator) {
  var ArrayG_$T$_;
  ArrayG_$T$_ = System_ArrayG(T, true);
  array.sort(comparator);
};
ptyp_ = System_ArrayImpl.prototype;
ptyp_.system__Collections__IList__get_IsFixedSize = function System__ArrayImpl__System__Collections__IList__get_IsFixedSize() {
  return true;
};
ptyp_.system__Collections__IList__get_IsReadOnly = function System__ArrayImpl__System__Collections__IList__get_IsReadOnly() {
  return false;
};
ptyp_.system__Collections__IList__get_Item = function System__ArrayImpl__System__Collections__IList__get_Item(index) {
  return this.V_GetValue(index);
};
ptyp_.system__Collections__IList__set_Item = function System__ArrayImpl__System__Collections__IList__set_Item(index, value) {
  this.V_SetValue(index, value);
};
ptyp_.system__Collections__IList__Add = function System__ArrayImpl__System__Collections__IList__Add(value) {
  throw System__NotSupportedException_factory();
};
ptyp_.system__Collections__IList__Clear = function System__ArrayImpl__System__Collections__IList__Clear() {
  throw System__NotSupportedException_factory();
};
ptyp_.system__Collections__IList__Insert = function System__ArrayImpl__System__Collections__IList__Insert(index, value) {
  throw System__NotSupportedException_factory();
};
ptyp_.system__Collections__IList__Remove = function System__ArrayImpl__System__Collections__IList__Remove(value) {
  throw System__NotSupportedException_factory();
};
ptyp_.system__Collections__IList__RemoveAt = function System__ArrayImpl__System__Collections__IList__RemoveAt(index) {
  throw System__NotSupportedException_factory();
};
ptyp_.system__Collections__ICollection__get_Count = function System__ArrayImpl__System__Collections__ICollection__get_Count() {
  return this.V_get_Length();
};
ptyp_.__ctor = function System__ArrayImpl____ctor() {
};
ptyp_.V_get_IsFixedSize_d = ptyp_.system__Collections__IList__get_IsFixedSize;
ptyp_.V_get_IsReadOnly_d = ptyp_.system__Collections__IList__get_IsReadOnly;
ptyp_.V_get_Item_d = ptyp_.system__Collections__IList__get_Item;
ptyp_.V_set_Item_d = ptyp_.system__Collections__IList__set_Item;
ptyp_.V_Add_d = ptyp_.system__Collections__IList__Add;
ptyp_.V_Clear_d = ptyp_.system__Collections__IList__Clear;
ptyp_.V_Insert_d = ptyp_.system__Collections__IList__Insert;
ptyp_.V_Remove_d = ptyp_.system__Collections__IList__Remove;
ptyp_.V_RemoveAt_d = ptyp_.system__Collections__IList__RemoveAt;
ptyp_.V_get_Count_e = ptyp_.system__Collections__ICollection__get_Count;
ptyp_.V_Contains_d = function(arg0) {
  return this.V_Contains(arg0);
};
ptyp_.V_IndexOf_d = function(arg0) {
  return this.V_IndexOf(arg0);
};
ptyp_.V_GetEnumerator_f = function() {
  return this.V_GetEnumerator();
};
ptyp_.V_CopyTo_e = function(arg0, arg1) {
  return this.V_CopyTo(arg0, arg1);
};
System__Type__RegisterReferenceType(System_ArrayImpl, "System.ArrayImpl", Object, [System_Collections_IList, System_Collections_IEnumerable, System_Collections_ICollection]);
function System_ArrayG(T, $5fcallStatiConstructor) {
  var Enumerator_$T$_, ArrayG$1_$T$_, IList$1_$T$_, ICollection$1_$T$_, IEnumerable$1_$T$_, $5f_initTracker;
  if (System_ArrayG[T.typeId])
    return System_ArrayG[T.typeId];
  System_ArrayG[T.typeId] = function System__ArrayG$1() {
  };
  ArrayG$1_$T$_ = System_ArrayG[T.typeId];
  ArrayG$1_$T$_.genericParameters = [T];
  ArrayG$1_$T$_.genericClosure = System_ArrayG;
  ArrayG$1_$T$_.typeId = "g$" + T.typeId + "$";
  IList$1_$T$_ = System_Collections_Generic_IList(T, $5fcallStatiConstructor);
  ICollection$1_$T$_ = System_Collections_Generic_ICollection(T, $5fcallStatiConstructor);
  IEnumerable$1_$T$_ = System_Collections_Generic_IEnumerable(T, $5fcallStatiConstructor);
  ArrayG$1_$T$_.__ctora = function System_ArrayG$1_factory(size) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctora(size);
    return this_;
  };
  ArrayG$1_$T$_.__ctor = function System_ArrayG$1_factorya(nativeArray) {
    var this_;
    this_ = new ArrayG$1_$T$_();
    this_.__ctorb(nativeArray);
    return this_;
  };
  ptyp_ = new System_ArrayImpl();
  ArrayG$1_$T$_.prototype = ptyp_;
  ptyp_.innerArray = null;
  ptyp_.system__Collections__Generic__ICollection_$T$___get_IsReadOnly = function System__ArrayG$1__System__Collections__Generic__ICollection_$T$___get_IsReadOnly() {
    return false;
  };
  ptyp_.system__Collections__Generic__IList_$T$___Insert = function System__ArrayG$1__System__Collections__Generic__IList_$T$___Insert(index, item) {
    throw new Error("Not Implemented.");
  };
  ptyp_.system__Collections__Generic__ICollection_$T$___Add = function System__ArrayG$1__System__Collections__Generic__ICollection_$T$___Add(item) {
    throw new Error("Not Implemented.");
  };
  ptyp_.system__Collections__Generic__ICollection_$T$___Clear = function System__ArrayG$1__System__Collections__Generic__ICollection_$T$___Clear() {
    throw new Error("Not Implemented.");
  };
  ptyp_.system__Collections__Generic__ICollection_$T$___Remove = function System__ArrayG$1__System__Collections__Generic__ICollection_$T$___Remove(item) {
    return System__NativeArray$1__IndexOf(this.innerArray, item, 0) !== -1;
  };
  ptyp_.system__Collections__Generic__IEnumerable_$T$___GetEnumerator = function System__ArrayG$1__System__Collections__Generic__IEnumerable_$T$___GetEnumerator() {
    return Enumerator_$T$_.__ctor(this);
  };
  ptyp_.__ctora = function System__ArrayG$1____ctor(size) {
    var def, i;
    this.__ctor();
    this.innerArray = new Array(size);
    def = System__Type__GetDefaultValueStatic(T);
    for (i = 0; i < size; i++)
      this.innerArray[i] = def;
  };
  ptyp_.__ctorb = function System__ArrayG$1____ctora(nativeArray) {
    this.__ctor();
    this.innerArray = nativeArray;
  };
  ptyp_.get_length = function System__ArrayG$1__get_Length() {
    return this.innerArray.length;
  };
  ptyp_.get_item = function System__ArrayG$1__get_Item(index) {
    var arr;
    arr = this.innerArray;
    if (index < 0 || index >= arr.length)
      throw "index " + index + " out of range";
    return arr[index];
  };
  ptyp_.set_item = function System__ArrayG$1__set_Item(index, value) {
    var arr;
    arr = this.innerArray;
    if (index < 0 || index >= arr.length)
      throw "index " + index + " out of range";
    return arr[index] = value;
  };
  ptyp_.get_innerArray = function System__ArrayG$1__get_InnerArray() {
    return this.innerArray;
  };
  ptyp_.clone = function System__ArrayG$1__Clone() {
    return ArrayG$1_$T$_.__ctor(this.innerArray.slice(0, this.innerArray.length));
  };
  ptyp_.contains = function System__ArrayG$1__Contains(item) {
    return this.V_IndexOf(item) >= 0;
  };
  ptyp_.indexOf = function System__ArrayG$1__IndexOf(item) {
    if (!T.isInstanceOfType(item))
      return -1;
    return System__NativeArray$1__IndexOf(this.innerArray, System__Type__UnBoxTypeInstance(T, item), 0);
  };
  ptyp_.indexOfa = function System__ArrayG$1__IndexOfa(item, startIndex) {
    if (!T.isInstanceOfType(item))
      return -1;
    return System__NativeArray$1__IndexOf(this.innerArray, System__Type__UnBoxTypeInstance(T, item), startIndex);
  };
  ptyp_.reverse = function System__ArrayG$1__Reverse() {
    this.innerArray.reverse();
  };
  ptyp_.sort = function System__ArrayG$1__Sort(compareCallback) {
    this.innerArray.sort(compareCallback);
  };
  ptyp_.getValue = function System__ArrayG$1__GetValue(index) {
    return System__Type__BoxTypeInstance(T, this.get_item(index));
  };
  ptyp_.setValue = function System__ArrayG$1__SetValue(index, value) {
    this.set_item(index, System__Type__UnBoxTypeInstance(T, value));
  };
  ptyp_.indexOfb = function System__ArrayG$1__IndexOfb(item) {
    return System__NativeArray$1__IndexOf(this.innerArray, item, 0);
  };
  ptyp_.containsa = function System__ArrayG$1__Containsa(item) {
    return System__NativeArray$1__IndexOf(this.innerArray, item, 0) !== -1;
  };
  ptyp_.copyTo = function System__ArrayG$1__CopyTo(arr, index) {
    var nativeArray, length, i;
    nativeArray = this.innerArray;
    length = nativeArray.length;
    for (i = 0; i < length; i++)
      arr.set_item(i + index, nativeArray[i]);
  };
  ptyp_.copyToa = function System__ArrayG$1__CopyToa(array, index) {
    var nativeArray, length, i;
    nativeArray = this.innerArray;
    length = nativeArray.length;
    for (i = 0; i < length; i++)
      array.setValue(i + index, System__Type__BoxTypeInstance(T, nativeArray[i]));
  };
  ptyp_.getEnumerator = function System__ArrayG$1__GetEnumerator() {
    return Enumerator_$T$_.__ctor(this);
  };
  ptyp_["V_get_IsReadOnly_" + ICollection$1_$T$_.typeId] = ptyp_.system__Collections__Generic__ICollection_$T$___get_IsReadOnly;
  ptyp_["V_Insert_" + IList$1_$T$_.typeId] = ptyp_.system__Collections__Generic__IList_$T$___Insert;
  ptyp_["V_Add_" + ICollection$1_$T$_.typeId] = ptyp_.system__Collections__Generic__ICollection_$T$___Add;
  ptyp_["V_Clear_" + ICollection$1_$T$_.typeId] = ptyp_.system__Collections__Generic__ICollection_$T$___Clear;
  ptyp_["V_Remove_" + ICollection$1_$T$_.typeId] = ptyp_.system__Collections__Generic__ICollection_$T$___Remove;
  ptyp_["V_GetEnumerator_" + IEnumerable$1_$T$_.typeId] = ptyp_.system__Collections__Generic__IEnumerable_$T$___GetEnumerator;
  ptyp_.V_get_Length = ptyp_.get_length;
  ptyp_.V_Clone = ptyp_.clone;
  ptyp_.V_Contains = ptyp_.contains;
  ptyp_.V_GetEnumerator = ptyp_.getEnumerator;
  ptyp_.V_IndexOf = ptyp_.indexOf;
  ptyp_.V_IndexOfa = ptyp_.indexOfa;
  ptyp_.V_Reverse = ptyp_.reverse;
  ptyp_.V_GetValue = ptyp_.getValue;
  ptyp_.V_SetValue = ptyp_.setValue;
  ptyp_.V_CopyTo = ptyp_.copyToa;
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  ptyp_["V_IndexOf_" + IList$1_$T$_.typeId] = ptyp_.indexOfb;
  ptyp_["V_Contains_" + ICollection$1_$T$_.typeId] = ptyp_.containsa;
  ptyp_["V_CopyTo_" + ICollection$1_$T$_.typeId] = ptyp_.copyTo;
  System__Type__RegisterReferenceType(ArrayG$1_$T$_, "System.ArrayG`1<" + T.fullName + ">", System_ArrayImpl, [IList$1_$T$_, System_Collections_IList, ICollection$1_$T$_, System_Collections_ICollection, System_Collections_IEnumerable, IEnumerable$1_$T$_]);
  ArrayG$1_$T$_._tri = function() {
    if ($5f_initTracker)
      return;
    $5f_initTracker = true;
    Enumerator_$T$_ = System_ArrayG_Enumerator(T, true);
  };
  if ($5fcallStatiConstructor)
    ArrayG$1_$T$_._tri();
  return ArrayG$1_$T$_;
};
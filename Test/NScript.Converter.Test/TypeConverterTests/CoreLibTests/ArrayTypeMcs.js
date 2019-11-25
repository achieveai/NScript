Array.typeId = "b";
function Array__Sort(T, array, comparator) {
  var ArrayG_$T$_;
  ArrayG_$T$_ = ArrayG(T, true);
  array.sort(comparator);
};
Type__RegisterReferenceType(Array, "System.Array", Object, [IList, ICollection, IEnumerable]);
function ArrayImpl() {
};
ArrayImpl.typeId = "c";
function ArrayImpl__Sort(T, array, comparator) {
  var ArrayG_$T$_;
  ArrayG_$T$_ = ArrayG(T, true);
  array.sort(comparator);
};
ptyp_ = ArrayImpl.prototype;
ptyp_.__ctor = function ArrayImpl____ctor() {
};
ptyp_.system__Collections__IList__get_IsFixedSize = function ArrayImpl__System__Collections__IList__get_IsFixedSize() {
  return true;
};
ptyp_.system__Collections__IList__get_IsReadOnly = function ArrayImpl__System__Collections__IList__get_IsReadOnly() {
  return false;
};
ptyp_.system__Collections__IList__get_Item = function ArrayImpl__System__Collections__IList__get_Item(index) {
  return this.V_GetValue(index);
};
ptyp_.system__Collections__IList__set_Item = function ArrayImpl__System__Collections__IList__set_Item(index, value) {
  this.V_SetValue(index, value);
};
ptyp_.system__Collections__IList__Add = function ArrayImpl__System__Collections__IList__Add(value) {
  throw NotSupportedException_factory();
};
ptyp_.system__Collections__IList__Clear = function ArrayImpl__System__Collections__IList__Clear() {
  throw NotSupportedException_factory();
};
ptyp_.system__Collections__IList__Insert = function ArrayImpl__System__Collections__IList__Insert(index, value) {
  throw NotSupportedException_factory();
};
ptyp_.system__Collections__IList__Remove = function ArrayImpl__System__Collections__IList__Remove(value) {
  throw NotSupportedException_factory();
};
ptyp_.system__Collections__IList__RemoveAt = function ArrayImpl__System__Collections__IList__RemoveAt(index) {
  throw NotSupportedException_factory();
};
ptyp_.system__Collections__ICollection__get_Count = function ArrayImpl__System__Collections__ICollection__get_Count() {
  return this.V_get_Length();
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
ptyp_.V_CopyTo_e = function(arg0, arg1) {
  return this.V_CopyTo(arg0, arg1);
};
ptyp_.V_GetEnumerator_f = function() {
  return this.V_GetEnumerator();
};
Type__RegisterReferenceType(ArrayImpl, "System.ArrayImpl", Object, [IList, ICollection, IEnumerable]);
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
function ArrayG(T, _callStatiConstructor) {
  var ArrayG$1_$T$_, T$5b$5d_$T$_, Enumerator_$T$_, IList$1_$T$_, ICollection$1_$T$_, IEnumerable$1_$T$_, __initTracker;
  if (ArrayG[T.typeId])
    return ArrayG[T.typeId];
  ArrayG[T.typeId] = function System__ArrayG$1() {
  };
  ArrayG$1_$T$_ = ArrayG[T.typeId];
  ArrayG$1_$T$_.genericParameters = [T];
  ArrayG$1_$T$_.genericClosure = ArrayG;
  ArrayG$1_$T$_.typeId = "g$" + T.typeId + "$";
  IList$1_$T$_ = ILista(T, _callStatiConstructor);
  ICollection$1_$T$_ = ICollectiona(T, _callStatiConstructor);
  IEnumerable$1_$T$_ = IEnumerablea(T, _callStatiConstructor);
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
  ptyp_ = new ArrayImpl();
  ArrayG$1_$T$_.prototype = ptyp_;
  ptyp_.innerArray = null;
  ptyp_.__ctora = function ArrayG$1____ctor(size) {
    var def, i;
    this.__ctor(); {
      this.innerArray = new Array(size);
      def = Type__GetDefaultValueStatic(T);
      for (i = 0; i < size; i++)
        this.innerArray[i] = def;
    }
  };
  ptyp_.__ctorb = function ArrayG$1____ctora(nativeArray) {
    this.__ctor();
    this.innerArray = nativeArray;
  };
  ptyp_.get_length = function ArrayG$1__get_Length() {
    return this.innerArray.length;
  };
  ptyp_.get_item = function ArrayG$1__get_Item(index) {
    var arr;
    arr = this.innerArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index];
  };
  ptyp_.set_item = function ArrayG$1__set_Item(index, value) {
    var arr;
    arr = this.innerArray;
    if (index < 0 || index >= arr.length)
      throw new Error("index " + index + " out of range");
    return arr[index] = value;
  };
  ptyp_.system__Collections__Generic__ICollection_$T$___get_IsReadOnly = function ArrayG$1__System__Collections__Generic__ICollection_$T$___get_IsReadOnly() {
    return false;
  };
  ptyp_.get_innerArray = function ArrayG$1__get_InnerArray() {
    return this.innerArray;
  };
  ptyp_.clone = function ArrayG$1__Clone() {
    return ArrayG$1_$T$_.__ctor(this.innerArray.slice(0, this.innerArray.length));
  };
  ptyp_.contains = function ArrayG$1__Contains(item) {
    return this.V_IndexOf(item) >= 0;
  };
  ptyp_.indexOf = function ArrayG$1__IndexOf(item) {
    if (!T.isInstanceOfType(item))
      return -1;
    return NativeArray$1__IndexOf(this.innerArray, Type__UnBoxTypeInstance(T, item), 0);
  };
  ptyp_.indexOfa = function ArrayG$1__IndexOfa(item, startIndex) {
    if (!T.isInstanceOfType(item))
      return -1;
    return NativeArray$1__IndexOf(this.innerArray, Type__UnBoxTypeInstance(T, item), startIndex);
  };
  ptyp_.reverse = function ArrayG$1__Reverse() {
    this.innerArray.reverse();
  };
  ptyp_.sort = function ArrayG$1__Sort(compareCallback) {
    this.innerArray.sort(compareCallback);
  };
  ptyp_.getValue = function ArrayG$1__GetValue(index) {
    return Type__BoxTypeInstance(T, this.get_item(index));
  };
  ptyp_.setValue = function ArrayG$1__SetValue(index, value) {
    this.set_item(index, Type__UnBoxTypeInstance(T, value));
  };
  ptyp_.indexOfb = function ArrayG$1__IndexOfb(item) {
    return NativeArray$1__IndexOf(this.innerArray, item, 0);
  };
  ptyp_.system__Collections__Generic__IList_$T$___Insert = function ArrayG$1__System__Collections__Generic__IList_$T$___Insert(index, item) {
    throw new Error("Not Implemented.");
  };
  ptyp_.system__Collections__Generic__ICollection_$T$___Add = function ArrayG$1__System__Collections__Generic__ICollection_$T$___Add(item) {
    throw new Error("Not Implemented.");
  };
  ptyp_.system__Collections__Generic__ICollection_$T$___Clear = function ArrayG$1__System__Collections__Generic__ICollection_$T$___Clear() {
    throw new Error("Not Implemented.");
  };
  ptyp_.containsa = function ArrayG$1__Containsa(item) {
    return NativeArray$1__IndexOf(this.innerArray, item, 0) != -1;
  };
  ptyp_.copyTo = function ArrayG$1__CopyTo(arr, index) {
    var nativeArray, length, nativeArrDst, i;
    nativeArray = this.innerArray;
    length = nativeArray.length;
    nativeArrDst = NativeArray$1__op_Implicit(arr);
    if (nativeArrDst.length < index + nativeArray.length)
      throw new Error("can't copy, dest array too small.");
    for (i = 0; i < length; i++)
      nativeArrDst[i + index] = nativeArray[i];
  };
  ptyp_.copyToa = function ArrayG$1__CopyToa(array, index) {
    var arr;
    arr = Type__CastType(T$5b$5d_$T$_, array);
    this.copyTo(arr, index);
  };
  ptyp_.system__Collections__Generic__ICollection_$T$___Remove = function ArrayG$1__System__Collections__Generic__ICollection_$T$___Remove(item) {
    return NativeArray$1__IndexOf(this.innerArray, item, 0) != -1;
  };
  ptyp_.getEnumerator = function ArrayG$1__GetEnumerator() {
    return Enumerator_$T$_.__ctor(this);
  };
  ptyp_.system__Collections__Generic__IEnumerable_$T$___GetEnumerator = function ArrayG$1__System__Collections__Generic__IEnumerable_$T$___GetEnumerator() {
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
  Type__RegisterReferenceType(ArrayG$1_$T$_, "System.ArrayG`1<" + T.fullName + ">", ArrayImpl, [IList$1_$T$_, IList, ICollection, IEnumerable, ICollection$1_$T$_, IEnumerable$1_$T$_]);
  ArrayG$1_$T$_._tri = function() {
    if (__initTracker)
      return;
    __initTracker = true;
    T$5b$5d_$T$_ = ArrayG(T, true);
    Enumerator_$T$_ = Enumerator(T, true);
  };
  if (_callStatiConstructor)
    ArrayG$1_$T$_._tri();
  return ArrayG$1_$T$_;
};

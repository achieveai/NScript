Array.typeId = "b";
function System__Array__Sort(T, array, comparator) {
  var ArrayG_$T$_;
  ArrayG_$T$_ = System_ArrayG(T, true);
  array.sort(comparator);
};
System__Type__RegisterReferenceType(Array, "System.Array", Object, [System_Collections_IEnumerable]);
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
function System__NativeArray$1__op_Implicita(n) {
  return ArrayG$1_$T$_.__ctor(n);
};
function System__NativeArray$1__op_Implicita(n) {
  return List$1_$T$_.__ctor(n);
};
function System_ArrayG(T, $5fcallStatiConstructor) {
  var Enumerator_$T$_, ArrayG$1_$T$_, IList$1_$T$_, ICollection$1_$T$_, IEnumerable$1_$T$_, $5f_initTracker;
  if (System_ArrayG[T.typeId])
    return System_ArrayG[T.typeId];
  System_ArrayG[T.typeId] = function System__ArrayG$1() {
  };
  ArrayG$1_$T$_ = System_ArrayG[T.typeId];
  ArrayG$1_$T$_.genericParameters = [T];
  ArrayG$1_$T$_.genericClosure = System_ArrayG;
  ArrayG$1_$T$_.typeId = "c$" + T.typeId + "$";
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
  ptyp_.get_count = function System__ArrayG$1__get_Count() {
    return this.innerArray.length;
  };
  ptyp_.get_isReadOnly = function System__ArrayG$1__get_IsReadOnly() {
    return false;
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
  ptyp_.insert = function System__ArrayG$1__Insert(index, item) {
    throw new Error("Not Implemented.");
  };
  ptyp_.removeAt = function System__ArrayG$1__RemoveAt(index) {
    throw new Error("Not Implemented.");
  };
  ptyp_.add = function System__ArrayG$1__Add(item) {
    throw new Error("Not Implemented.");
  };
  ptyp_.clear = function System__ArrayG$1__Clear() {
    throw new Error("Not Implemented.");
  };
  ptyp_.containsa = function System__ArrayG$1__Containsa(item) {
    return System__NativeArray$1__IndexOf(this.innerArray, item, 0) !== -1;
  };
  ptyp_.copyTo = function System__ArrayG$1__CopyTo(arr, index) {
    var i;
    for (i = 0; i < this.innerArray.length; i++)
      arr.set_item(index + i, this.get_item(i));
  };
  ptyp_.remove = function System__ArrayG$1__Remove(item) {
    return System__NativeArray$1__IndexOf(this.innerArray, item, 0) !== -1;
  };
  ptyp_.getEnumerator = function System__ArrayG$1__GetEnumerator() {
    return Enumerator_$T$_.__ctor(this);
  };
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
  ptyp_["V_get_Item_" + IList$1_$T$_.typeId] = ptyp_.get_item;
  ptyp_["V_set_Item_" + IList$1_$T$_.typeId] = ptyp_.set_item;
  ptyp_["V_IndexOf_" + IList$1_$T$_.typeId] = ptyp_.indexOfb;
  ptyp_["V_Insert_" + IList$1_$T$_.typeId] = ptyp_.insert;
  ptyp_["V_RemoveAt_" + IList$1_$T$_.typeId] = ptyp_.removeAt;
  ptyp_["V_get_Count_" + ICollection$1_$T$_.typeId] = ptyp_.get_count;
  ptyp_["V_get_IsReadOnly_" + ICollection$1_$T$_.typeId] = ptyp_.get_isReadOnly;
  ptyp_["V_Add_" + ICollection$1_$T$_.typeId] = ptyp_.add;
  ptyp_["V_Clear_" + ICollection$1_$T$_.typeId] = ptyp_.clear;
  ptyp_["V_Contains_" + ICollection$1_$T$_.typeId] = ptyp_.containsa;
  ptyp_["V_CopyTo_" + ICollection$1_$T$_.typeId] = ptyp_.copyTo;
  ptyp_["V_Remove_" + ICollection$1_$T$_.typeId] = ptyp_.remove;
  System__Type__RegisterReferenceType(ArrayG$1_$T$_, "System.ArrayG`1<" + T.fullName + ">", System_ArrayImpl, [IList$1_$T$_, ICollection$1_$T$_, IEnumerable$1_$T$_, System_Collections_IEnumerable]);
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
function GenericRegressions__TestGenericInterfacePropertyCall(T, t) {
  var tmp_, IEnumerable_$T$_, i, IEnumerator_$T$_;
  IEnumerator_$T$_ = IEnumerator(T, true);
  IEnumerable_$T$_ = IEnumerable(T, true);
  tmp_ = t["V_GetEnumerator_" + IEnumerable_$T$_.typeId]();
  try {
    while (tmp_.V_MoveNext_c()) {
      i = tmp_["V_get_Current_" + IEnumerator_$T$_.typeId]();
      return i;
    }
  } finally {
    if (!!tmp_)
      tmp_.V_Dispose_b();
  }
  return Type__GetDefaultValueStatic(T);
}
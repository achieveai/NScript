function RealScript__GenericRegressions__TestGenericInterfacePropertyCall(T, t) {
  var tmp_, IEnumerable_$T$_, i, IEnumerator_$T$_;
  IEnumerator_$T$_ = System_Collections_Generic_IEnumerator(T, true);
  IEnumerable_$T$_ = System_Collections_Generic_IEnumerable(T, true);
  tmp_ = t["V_GetEnumerator_" + IEnumerable_$T$_.typeId]();
  try {
    while (tmp_.V_MoveNext_c()) {
      i = tmp_["V_get_Current_" + IEnumerator_$T$_.typeId]();
      return i;
    }
  } finally {
    if (tmp_ !== null)
      tmp_.V_Dispose_b();
  }
  return T.getDefaultValue();
}
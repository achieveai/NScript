function RealScript__GenericRegressions__GenericStructBoxing(dict) {
  var tmp_, kvPair;
  tmp_ = dict.V_GetEnumerator_b$c$d_e$$();
  try {
    while (tmp_.V_MoveNext_g()) {
      kvPair = tmp_.V_get_Current_h$c$d_e$$();
      RealScript__Console__WriteLine(System__String__Concat(System_Collections_Generic_KeyValuePair_$String_x_String$_.get_key(kvPair), " -> ", System_Int32.box(System_Collections_Generic_KeyValuePair_$String_x_String$_.get_value(kvPair))));
    }
  } finally {
    if (tmp_ !== null)
      tmp_.V_Dispose_f();
  }
}

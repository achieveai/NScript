function GenericRegressions__GenericStructBoxing(dict) {
  var tmp_, kvPair;
  tmp_ = dict.V_GetEnumerator_b$c$d_e$$();
  try {
    while (tmp_.V_MoveNext_g()) {
      kvPair = tmp_.V_get_Current_h$c$d_e$$();
      Console__WriteLine(KeyValuePair_$String_x_Int32$_.get_key(kvPair) + " -> " + Type__BoxTypeInstance(Int32, KeyValuePair_$String_x_Int32$_.get_value(kvPair)));
    }
  } finally {
    if (!!tmp_)
      tmp_.V_Dispose_f();
  }
}

function FuncRegressions__IfElseInForBlock(array) {
  var rv, i;
  rv = "[";
  for (i = 0; i < array.V_get_Length(); ++i)
    if (i > 0)
      rv = rv + "," + array.get_item(i);
    else
      rv = rv + array.get_item(i);
  return rv + "]";
}

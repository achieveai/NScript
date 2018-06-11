function FuncRegressions__NestedWhileLoops(array) {
  var randomValue, i, j, tmp;
  randomValue = 11;
  i = 0;
  j = 10;
  while (i < j) {
    while (array.get_item(i) <= randomValue)
      ++i;
    while (array.get_item(j) >= randomValue)
      --j;
    if (i < j) {
      tmp = array.get_item(i);
      array.set_item(i, array.get_item(j));
      array.set_item(j, tmp);
    }
  }
  array.set_item(11, array.get_item(i));
}

function BasicBlockTestFunctions__Complex3IfCondition(i1, i2) {
  var b, returnValue;
  b = false;
  returnValue = 0;
  if (i2 == 3 && i1 == 10 && (i1 == 1 || i2 == 9) && (i1 == 2 || i2 == 3) || i1 == 4 && i2 == 3)
    returnValue = i1 + i2 * 2;
  else if (i1 > 2 && i2 < 3)
    returnValue = i1 + i2;
  else if (b)
    returnValue = i2 - i1;
  return returnValue + i2;
}

function TupleTests__TestTempVarCreation() {
  var a, b, c, x, j, k, l, tmp, stmtTemp1;
  a = 1, b = 2, c = 3, {
    item1: 1,
    item2: 2,
    item3: 3
  };
  x = {
    item1: a,
    item2: b,
    item3: c
  };
  j = x.item1, k = x.item2, l = x.item3, x;
  tmp = {
    item1: x,
    item2: "asdf"
  };
  stmtTemp1 = tmp.item1, a = stmtTemp1.item1, b = stmtTemp1.item2, c = stmtTemp1.item3, stmtTemp1;
}
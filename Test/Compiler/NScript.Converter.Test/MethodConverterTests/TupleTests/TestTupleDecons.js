function TupleTests__TestTupleDecons() {
  var stmtTemp1, a, b, c, x, y, z, w, i, j, k, l, stmtTemp1a, p, q, r, s;
  stmtTemp1 = TupleTests__FuncReturningTuple(), a = stmtTemp1.item1, b = stmtTemp1.item2, c = stmtTemp1.item3, stmtTemp1;
  stmtTemp1 = TupleTests__FuncReturningTuple(), x = stmtTemp1.item1, y = stmtTemp1.item2.item1, z = stmtTemp1.item2.item2, w = stmtTemp1.item3, stmtTemp1;
  i = 0, stmtTemp1 = (j = 1, k = 2, {
    item1: j,
    item2: k
  }), l = "rand", {
    item1: i,
    item2: stmtTemp1,
    item3: l
  };
  stmtTemp1a = TupleTests__TestTupleReturn(1, "asdf"), p = stmtTemp1a.item1.item1, q = stmtTemp1a.item1.item2.item1, r = stmtTemp1a.item1.item2.item2, s = stmtTemp1a.item2, stmtTemp1a;
}
function TupleTests__TestTupleDecons() {
  var stmtTemp1, a, b, c, x, y, z, w, i, j, k, l, p, q, r, s;
  stmtTemp1 = TupleTests__FuncReturningTuple(), a = stmtTemp1.item1, b = stmtTemp1.item2, c = stmtTemp1.item3, stmtTemp1;
  stmtTemp1 = TupleTests__FuncReturningTuple(), x = stmtTemp1.item1, y = stmtTemp1.item2.item1, z = stmtTemp1.item2.item2, w = stmtTemp1.item3, stmtTemp1;
  i = 0, j = 1, k = 2, l = "rand", {
    item1: 0,
    item2: {
      item1: 1,
      item2: 2
    },
    item3: "rand"
  };
  stmtTemp1 = TupleTests__TestTupleReturn(1, "asdf"), p = stmtTemp1.item1.item1, q = stmtTemp1.item1.item2.item1, r = stmtTemp1.item1.item2.item2, s = stmtTemp1.item2, stmtTemp1;
}
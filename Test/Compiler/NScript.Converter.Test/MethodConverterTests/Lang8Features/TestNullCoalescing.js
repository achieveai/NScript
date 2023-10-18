function Lang8Features__NullCoalescingAssignment() {
  var tupl, stmtTemp1;
  tupl = null;
  tupl = Object__IsNullOrUndefined(stmtTemp1 = tupl) ? {
    item1: 1,
    item2: 2
  } : stmtTemp1;
  (tupl = Object__IsNullOrUndefined(stmtTemp1 = tupl) ? {
    item1: 3,
    item2: 4
  } : stmtTemp1).item1;
}
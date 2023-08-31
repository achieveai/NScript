async function Lang8Features__TestAsyncForEach() {
  var stmtTemp1, stmtTemp1a, item;
  for (stmtTemp1a = Lang8Features__GetIntsAsync(), stmtTemp1 = stmtTemp1a.V_GetAsyncEnumerator_b(); await stmtTemp1.V_MoveNextAsync_c(); ) {
    item = Type__UnBoxTypeInstance(Int32, stmtTemp1.V_get_Current_c());
    Console__WriteLine(item);
  }
}

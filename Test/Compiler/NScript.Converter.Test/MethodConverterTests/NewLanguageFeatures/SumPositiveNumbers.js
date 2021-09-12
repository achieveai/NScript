function Lang7Features__SumPositiveNumbers(sequence) {
  var sum, stmtTemp1, i, childSequence, stmtTemp1a, item, n;
  sum = 0;
  for (stmtTemp1 = sequence.V_GetEnumerator_b(); stmtTemp1.V_MoveNext_c(); ) {
    i = stmtTemp1.V_get_Current_c();
    switch(true) {
      case i == 0:
      case i == 10:
      break;
      case(childSequence = Type__AsType(IEnumerable, i)) != null: {
        for (stmtTemp1a = childSequence.V_GetEnumerator_b(); stmtTemp1a.V_MoveNext_c(); ) {
          item = Type__UnBoxTypeInstance(Int32, stmtTemp1a.V_get_Current_c());
          sum += item > 0 ? item : 0;
        }
        break;
      }
      case(n = Type__AsType(Number, i)) != null && n > 0: {
        sum += n;
        break;
      }
      case i == null:
      throw ArgumentException_factory("Null found in sequence");
      default:
      throw InvalidProgramException_factory("Unrecognized type");
    }
  }
  return sum;
}

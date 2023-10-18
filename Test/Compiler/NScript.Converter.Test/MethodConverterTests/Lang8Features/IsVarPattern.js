function Lang8Features__IsVarPattern() {
  var isNonZero, x;
  isNonZero = ((x = GetInstance()) || true) && x.get_x() != 0;
  function GetInstance() {
    var stmtTemp1;
    return stmtTemp1 = BaseClass_factory(), stmtTemp1.set_x(90), stmtTemp1;
  }
}

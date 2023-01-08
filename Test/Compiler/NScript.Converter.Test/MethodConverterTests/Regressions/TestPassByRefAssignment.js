function FuncRegressions__TestPassByRefAssignment(list, value) {
  if (list.get_count() > 10) {
    value.wt(list.get_item(10));
    return true;
  }
  value.wt(0);
  return false;
}
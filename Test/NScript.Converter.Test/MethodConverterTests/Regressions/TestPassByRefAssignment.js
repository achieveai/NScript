function FuncRegressions__TestPassByRefAssignment(list, value) {
  if (list.get_count() > 10) {
    value.write(list.get_item(10));
    return true;
  }
  value.write(0);
  return false;
}
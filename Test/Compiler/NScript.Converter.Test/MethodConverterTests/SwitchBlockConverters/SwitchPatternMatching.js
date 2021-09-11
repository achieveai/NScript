function SwitchTest__SwitchPatternMatching() {
  var o, a;
  o = Type__BoxTypeInstance(Int32, 12);
  switch(true) {
    case Type__AsType(String, o) != null:
    return;
    case(a = Type__AsType(Int16Array, o)) != null && a.length > 12:
    return;
  }
}

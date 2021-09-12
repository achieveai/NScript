function Lang7Features__TestSwitchPatternMatching_2() {
  var o, obj;
  o = "asdf";
  switch(true) {
    case(obj = Type__AsType(Object, o)) != null && obj.V_Equals(Type__BoxTypeInstance(Int32, 12)):
    break;
    case(obj = Type__AsType(String, o)) != null && obj.length < 3:
    break;
    case Type__AsType(String, o) != null:
    break;
    default:
    throw InvalidProgramException_factory();
  }
}
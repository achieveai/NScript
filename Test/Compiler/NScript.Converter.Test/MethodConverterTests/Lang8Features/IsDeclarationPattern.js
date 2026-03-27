function Lang8Features__IsDeclarationPattern() {
  var b, isZero, sb, isSubClass;
  b = SubClass_factory();
  isZero = (sb = Type__AsType(SubClass, b)) != null && sb.get_x() == 0;
  isSubClass = SubClass.isInstanceOfType(b);
}

function Lang7Features__TestSwitchPatternMatching_1() {
  var l, age, ageBlock, stmtTemp1, testAge;
  l = ArrayG_$Int32$_.__ctor([1, 2, 3, 90]);
  age = Type__BoxTypeInstance(Int32, 90);
  ageBlock = null;
  switch(stmtTemp1 = Type__UnBoxTypeInstance(Int32, age), true) {
    case stmtTemp1 == 50: {
      ageBlock = "the big five-oh";
      break;
    }
    case(testAge = Type__AsType(Int32, stmtTemp1)) != null && l.cons(Type__BoxTypeInstance(Int32, testAge)): {
      ageBlock = "octogenarian";
      break;
    }
    case(testAge = Type__AsType(Int32, stmtTemp1)) != null && testAge >= 90 & testAge <= 99: {
      ageBlock = "nonagenarian";
      break;
    }
    case(testAge = Type__AsType(Int32, stmtTemp1)) != null && testAge >= 100: {
      ageBlock = "centenarian";
      break;
    }
    default: {
      ageBlock = "just old";
      break;
    }
  }
}
function RealScript__SwitchTest__SwitchWithReturnsOnly(i) {
  switch(i) {
    case 10: {
      System__String__Format("Ten", System_ArrayG_$Object$_.__ctor(0));
      return;
    }
    case 2:
    case 16:
    case 32: {
      System__String__Format("Power Of Two", System_ArrayG_$Object$_.__ctor(0));
      return;
    }
    case 103: {
      System__String__Format("Contigous Test 4", System_ArrayG_$Object$_.__ctor(0));
      return;
    }
    default: {
      System__String__Format("Default", System_ArrayG_$Object$_.__ctor(0));
      return;
    }
  }
}

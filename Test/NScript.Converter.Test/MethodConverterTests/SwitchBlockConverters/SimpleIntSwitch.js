function RealScript__SwitchTest__SimpleIntSwitch(i) {
  System__String__Format("{}", System_ArrayG_$Object$_.__ctor(0));
  switch(i) {
    case 100: {
      System__String__Format("OneHundred", System_ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 10: {
      System__String__Format("Ten", System_ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 2:
    case 16:
    case 32: {
      System__String__Format("Power Of Two", System_ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 101: {
      System__String__Format("Contigous Test", System_ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 104: {
      System__String__Format("Contigous Test 2", System_ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 102: {
      System__String__Format("Contigous Test 3", System_ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 103: {
      System__String__Format("Contigous Test 4", System_ArrayG_$Object$_.__ctor(0));
      break;
    }
    default: {
      System__String__Format("Default", System_ArrayG_$Object$_.__ctor(0));
      break;
    }
  }
  System__String__Format("{0}", System_ArrayG_$Object$_.__ctora(["done with NoDefault stuff"]));
}

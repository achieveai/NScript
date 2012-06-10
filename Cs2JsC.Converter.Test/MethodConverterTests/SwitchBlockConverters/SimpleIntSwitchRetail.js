function RealScript__SwitchTest__SimpleIntSwitch(i) {
  System__String__Format("{}", []);
  switch(i) {
    case 100: {
      System__String__Format("OneHundred", []);
      break;
    }
    case 10: {
      System__String__Format("Ten", []);
      break;
    }
    case 2:
    case 16:
    case 32: {
      System__String__Format("Power Of Two", []);
      break;
    }
    case 101: {
      System__String__Format("Contigous Test", []);
      break;
    }
    case 104: {
      System__String__Format("Contigous Test 2", []);
      break;
    }
    case 102: {
      System__String__Format("Contigous Test 3", []);
      break;
    }
    case 103: {
      System__String__Format("Contigous Test 4", []);
      break;
    }
    default: {
      System__String__Format("Default", []);
      break;
    }
  }
  System__String__Format("{0}", ["done with NoDefault stuff"]);
}

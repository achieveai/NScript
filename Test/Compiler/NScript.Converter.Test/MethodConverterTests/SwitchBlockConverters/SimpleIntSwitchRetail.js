function SwitchTest__SimpleIntSwitch(i) {
  String__Format("{}", []);
  switch(i) {
    case 100: {
      String__Format("OneHundred", []);
      break;
    }
    case 10: {
      String__Format("Ten", []);
      break;
    }
    case 2:
    case 16:
    case 32: {
      String__Format("Power Of Two", []);
      break;
    }
    case 101: {
      String__Format("Contigous Test", []);
      break;
    }
    case 104: {
      String__Format("Contigous Test 2", []);
      break;
    }
    case 102: {
      String__Format("Contigous Test 3", []);
      break;
    }
    case 103: {
      String__Format("Contigous Test 4", []);
      break;
    }
    default: {
      String__Format("Default", []);
      break;
    }
  }
  String__Format("{0}", ["done with NoDefault stuff"]);
}

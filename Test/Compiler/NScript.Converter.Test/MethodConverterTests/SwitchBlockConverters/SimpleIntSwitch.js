function SwitchTest__SimpleIntSwitch(i) {
  String__Format("{}", ArrayG_$Object$_.__ctor(0));
  switch(i) {
    case 100: {
      String__Format("OneHundred", ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 10: {
      String__Format("Ten", ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 2:
    case 16:
    case 32: {
      String__Format("Power Of Two", ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 101: {
      String__Format("Contigous Test", ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 104: {
      String__Format("Contigous Test 2", ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 102: {
      String__Format("Contigous Test 3", ArrayG_$Object$_.__ctor(0));
      break;
    }
    case 103: {
      String__Format("Contigous Test 4", ArrayG_$Object$_.__ctor(0));
      break;
    }
    default: {
      String__Format("Default", ArrayG_$Object$_.__ctor(0));
      break;
    }
  }
  String__Formata("{0}", "done with NoDefault stuff");
}

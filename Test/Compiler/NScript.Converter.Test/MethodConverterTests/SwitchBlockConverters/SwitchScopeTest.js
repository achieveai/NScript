function SwitchTest__SwitchScopeTest() {
  var x, y;
  switch(0) {
    case 0: {
      x = 10;
      break;
    }
    default:
    break;
    case 1: {
      x = 90;
      y = x + 90;
      break;
    }
  }
}
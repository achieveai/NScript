function RealScript__ForLoopBlocks__ForLoopWithBreak(num) {
  var j;
  for (j = 0; j < num; ++j) {
    RealScript__Class1__staticBar = RealScript__Class1__GetMoreStatic(num);
    if (num == 100) {
      num = num / 2 | 0;
      return;
    }
    num += RealScript__Class1__staticBar;
  }
}

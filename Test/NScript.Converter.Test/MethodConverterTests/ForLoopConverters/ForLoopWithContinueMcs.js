function ForLoopBlocks__ForLoopWithContinue(num) {
  var j;
  for (j = 0; j < num; ++j) {
    Class1__staticBar = Class1__GetMoreStatic(num);
    if (num == 100) {
      num = num / 2 | 0;
      continue;
    }
    num += Class1__staticBar;
  }
}
